/**
 * Category Tooltip Feature - Optimized Version
 * Displays a popover with the latest image when hovering over subcategory links
 * Uses lazy loading to avoid N database queries on page load
 * Updated for Bootstrap 5.3 - vanilla JavaScript
 */
(function () {
    'use strict';

    // Cache for storing fetched images to avoid repeated API calls
    var imageCache = {};
    
    // Prefetch queue for smart preloading
    var prefetchQueue = new Set();
    var isPrefetching = false;

    // Configuration
    var config = {
        delay: 400,           // Reduced delay for faster response (was 1000ms)
        prefetchDelay: 2000,  // Start prefetching after 2 seconds of inactivity
        imageMaxWidth: 300,
        imageMaxHeight: 200,
        apiEndpoint: '/api/image/GetOneImageFromCategory/',
        imageBaseUrl: 'https://arvidsonfoto.se'
    };

    /**
     * Checks if current viewport is mobile (max-width: 767px)
     */
    function isMobileView() {
        return window.innerWidth <= 767;
    }

    /**
     * Fetches the latest image for a category from the API
     */
    function fetchCategoryImage(categoryId) {
        // Check cache first
        if (imageCache[categoryId]) {
            return Promise.resolve(imageCache[categoryId]);
        }

        // Check if already fetching
        if (imageCache[categoryId + '_pending']) {
            return imageCache[categoryId + '_pending'];
        }

        // Mark as pending and fetch
        var fetchPromise = fetch(config.apiEndpoint + categoryId, {
            method: 'GET',
            headers: {
                'Accept': 'application/json',
                'Cache-Control': 'max-age=3600' // Cache for 1 hour
            }
        })
        .then(function (response) {
            if (!response.ok) {
                throw new Error('HTTP ' + response.status);
            }
            return response.json();
        })
        .then(function (imageData) {
            // Cache the result
            imageCache[categoryId] = imageData;
            delete imageCache[categoryId + '_pending'];
            return imageData;
        })
        .catch(function (error) {
            console.warn('Failed to fetch image for category ' + categoryId + ':', error.message);
            delete imageCache[categoryId + '_pending'];
            return null;
        });

        imageCache[categoryId + '_pending'] = fetchPromise;
        return fetchPromise;
    }

    /**
     * Generates HTML content for the popover
     */
    function generatePopoverContent(imageData, categoryName) {
        if (!imageData || !imageData.urlImage) {
            return '<div class="text-muted small">Ingen bild tillgänglig</div>';
        }

        var imagePath = imageData.urlImage;
        if (!imagePath.startsWith('/')) {
            imagePath = '/' + imagePath;
        }
        imagePath = config.imageBaseUrl + imagePath + '.thumb.jpg';

        var html = '<div class="category-tooltip-content">';
        html += '<img src="' + imagePath + '" alt="' + (imageData.name || 'Preview') + '" ';
        html += 'loading="lazy" '; // Native lazy loading
        html += 'style="max-width: ' + config.imageMaxWidth + 'px; max-height: ' + config.imageMaxHeight + 'px; width: auto; height: auto; display: block;" />';
        
        if (categoryName) {
            html += '<div class="mt-2 fw-bold" style="color: #ddd !important;">' + categoryName + '</div>';
        }
        
        if (imageData.dateImageTaken) {
            var date = new Date(imageData.dateImageTaken);
            var formattedDate = date.toLocaleDateString('sv-SE');
            html += '<div class="mt-1 small text-muted" style="color: #aaa !important;">Fotograferad: ' + formattedDate + '</div>';
        }
        
        html += '</div>';
        return html;
    }

    /**
     * Creates popover configuration object
     */
    function getPopoverConfig(content) {
        var offsetConfig = isMobileView() ? [40, 10] : [0, 10];
        
        return {
            trigger: 'manual',
            html: true,
            placement: 'right',
            fallbackPlacements: ['left', 'bottom', 'top'],
            container: 'body',
            content: content,
            template: '<div class="popover category-tooltip-popover" role="tooltip"><div class="popover-arrow"></div><div class="popover-body"></div></div>',
            boundary: 'viewport',
            offset: offsetConfig,
            animation: true // Smooth fade-in
        };
    }

    /**
     * Smart prefetching - preload images for visible category links
     */
    function startPrefetching() {
        if (isPrefetching || prefetchQueue.size === 0) return;
        
        isPrefetching = true;
        var categoryId = Array.from(prefetchQueue)[0];
        prefetchQueue.delete(categoryId);
        
        fetchCategoryImage(categoryId).finally(function() {
            isPrefetching = false;
            // Prefetch next in queue after a small delay
            if (prefetchQueue.size > 0) {
                setTimeout(startPrefetching, 100);
            }
        });
    }

    /**
     * Add category to prefetch queue
     */
    function queuePrefetch(categoryId) {
        if (!imageCache[categoryId] && !imageCache[categoryId + '_pending']) {
            prefetchQueue.add(categoryId);
        }
    }

    /**
     * Initializes popovers for category links
     */
    function initializeCategoryTooltips() {
        var links = document.querySelectorAll('.has-category-tooltip');
        
        // Collect visible links for smart prefetching
        var visibleLinks = [];
        links.forEach(function(link) {
            var rect = link.getBoundingClientRect();
            if (rect.top < window.innerHeight && rect.bottom > 0) {
                visibleLinks.push(link);
            }
        });

        // Queue prefetch for first few visible links after a delay
        setTimeout(function() {
            visibleLinks.slice(0, 5).forEach(function(link) {
                var categoryId = link.getAttribute('data-category-id');
                if (categoryId) {
                    queuePrefetch(categoryId);
                }
            });
            startPrefetching();
        }, config.prefetchDelay);
        
        links.forEach(function (linkElement) {
            var categoryId = linkElement.getAttribute('data-category-id');
            var categoryName = linkElement.getAttribute('data-category-name');
            var hoverTimer = null;
            var popoverInstance = null;
            var isVisible = false;

            if (!categoryId) return;

            // Mouse enter event
            linkElement.addEventListener('mouseenter', function () {
                // Hide other popovers
                links.forEach(function (otherLink) {
                    if (otherLink !== linkElement) {
                        var otherPopover = bootstrap.Popover.getInstance(otherLink);
                        if (otherPopover) {
                            otherPopover.hide();
                        }
                    }
                });
                
                if (hoverTimer) {
                    clearTimeout(hoverTimer);
                }

                // Set timer to show popover
                hoverTimer = setTimeout(function () {
                    // Check if already in cache - instant show!
                    if (imageCache[categoryId]) {
                        var content = generatePopoverContent(imageCache[categoryId], categoryName);
                        popoverInstance = new bootstrap.Popover(linkElement, getPopoverConfig(content));
                        popoverInstance.show();
                        isVisible = true;
                    } else {
                        // Show loading state
                        var loadingContent = '<div class="text-center"><span class="spinner-border spinner-border-sm" role="status"></span> Laddar...</div>';
                        popoverInstance = new bootstrap.Popover(linkElement, getPopoverConfig(loadingContent));
                        popoverInstance.show();
                        isVisible = true;

                        // Fetch and update
                        fetchCategoryImage(categoryId).then(function (imageData) {
                            if (isVisible) {
                                var content = generatePopoverContent(imageData, categoryName);
                                popoverInstance.dispose();
                                popoverInstance = new bootstrap.Popover(linkElement, getPopoverConfig(content));
                                popoverInstance.show();
                            }
                        });
                    }
                }, config.delay);
            });

            // Mouse leave event
            linkElement.addEventListener('mouseleave', function () {
                isVisible = false;
                
                if (hoverTimer) {
                    clearTimeout(hoverTimer);
                    hoverTimer = null;
                }

                if (popoverInstance) {
                    popoverInstance.hide();
                }
            });
        });

        // Hide popover on click
        document.addEventListener('click', function () {
            links.forEach(function (link) {
                var popover = bootstrap.Popover.getInstance(link);
                if (popover) {
                    popover.hide();
                }
            });
        });
        
        console.log('✓ Category tooltips initialized with lazy loading (' + links.length + ' links)');
    }

    // Initialize when DOM is ready
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', initializeCategoryTooltips);
    } else {
        initializeCategoryTooltips();
    }

})();
