/**
 * Category Tooltip Feature
 * Displays a popover with the latest image when hovering over subcategory links
 * Updated for Bootstrap 5.3 - using vanilla JavaScript
 * Now with localStorage caching support for better performance
 */
(function () {
    'use strict';

    // In-memory cache for quick access (secondary layer)
    var imageCache = {};

    // Configuration
    var config = {
        delay: 1000,           // Delay before showing tooltip (ms)
        imageMaxWidth: 300,   // Max width of preview image
        imageMaxHeight: 200,  // Max height of preview image
        apiEndpoint: '/api/image/GetOneImageFromCategory/',
        imageBaseUrl: 'https://arvidsonfoto.se', // Base URL for images
        cacheKeyPrefix: 'category_image_' // Prefix for localStorage cache keys
    };

    /**
     * Checks if current viewport is mobile (max-width: 767px)
     * @returns {boolean} - True if mobile viewport
     */
    function isMobileView() {
        return window.innerWidth <= 767;
    }

    /**
     * Fetches the latest image for a category from the API
     * Uses both localStorage and in-memory cache for optimal performance
     * @param {number} categoryId - The category ID
     * @returns {Promise} - Promise that resolves with image data
     */
    function fetchCategoryImage(categoryId) {
        var cacheKey = config.cacheKeyPrefix + categoryId;
        
        // Check in-memory cache first (fastest)
        if (imageCache[categoryId]) {
            return Promise.resolve(imageCache[categoryId]);
        }

        // Check localStorage cache (if available)
        if (window.LocalStorageCache) {
            var cachedData = LocalStorageCache.get(cacheKey);
            if (cachedData) {
                // Store in memory cache for even faster subsequent access
                imageCache[categoryId] = cachedData;
                return Promise.resolve(cachedData);
            }
        }

        // Fetch from API
        return fetch(config.apiEndpoint + categoryId, {
            method: 'GET',
            headers: {
                'Accept': 'application/json'
            }
        })
        .then(function (response) {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json();
        })
        .then(function (imageData) {
            // Cache the result in memory
            imageCache[categoryId] = imageData;
            
            // Cache in localStorage with 1 hour TTL
            if (window.LocalStorageCache) {
                LocalStorageCache.set(cacheKey, imageData);
            }
            
            return imageData;
        })
        .catch(function (error) {
            console.warn('Failed to fetch image for category ' + categoryId + ':', error);
            return null;
        });
    }

    /**
     * Generates HTML content for the popover
     * @param {Object} imageData - Image data from API
     * @param {string} categoryName - Name of the category
     * @returns {string} - HTML content
     */
    function generatePopoverContent(imageData, categoryName) {
        if (!imageData || !imageData.urlImage) {
            return '<div class="text-muted">Ingen bild tillgänglig</div>';
        }

        var imagePath = imageData.urlImage;
        // Ensure the path starts with a slash
        if (!imagePath.startsWith('/')) {
            imagePath = '/' + imagePath;
        }
        // Add .thumb.jpg extension for thumbnail and prepend the base URL
        imagePath = config.imageBaseUrl + imagePath + '.thumb.jpg';

        var html = '<div class="category-tooltip-content">';
        html += '<img src="' + imagePath + '" alt="' + (imageData.name || 'Preview') + '" ';
        html += 'style="max-width: ' + config.imageMaxWidth + 'px; max-height: ' + config.imageMaxHeight + 'px; width: auto; height: auto; display: block;" />';
        
        // Add category name below the image
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
     * @param {string} content - The HTML content for the popover
     * @returns {Object} - Popover configuration object
     */
    function getPopoverConfig(content) {
        // On mobile, add significant horizontal offset to avoid covering menu buttons
        var offsetConfig = isMobileView() ? [40, 10] : [0, 10];
        
        return {
            trigger: 'manual',
            html: true,
            placement: 'right', // Default to right side
            fallbackPlacements: ['left', 'bottom', 'top'], // Avoid top placement that might cover menu
            container: 'body',
            content: content,
            template: '<div class="popover category-tooltip-popover" role="tooltip"><div class="popover-arrow"></div><div class="popover-body"></div></div>',
            boundary: 'viewport', // Keep within viewport
            offset: offsetConfig // Larger offset on mobile to avoid +/- buttons
        };
    }

    /**
     * Initializes popovers for category links
     */
    function initializeCategoryTooltips() {
        var links = document.querySelectorAll('.has-category-tooltip');
        
        links.forEach(function (linkElement) {
            var categoryId = linkElement.getAttribute('data-category-id');
            var categoryName = linkElement.getAttribute('data-category-name');
            var hoverTimer = null; // Timer specific to this link
            var popoverInstance = null;
            var isVisible = false; // Track visibility state

            if (!categoryId) {
                return; // Skip if no category ID
            }

            // Mouse enter event - start timer
            linkElement.addEventListener('mouseenter', function () {
                // Hide any other visible popovers first
                links.forEach(function (otherLink) {
                    if (otherLink !== linkElement) {
                        var otherPopover = bootstrap.Popover.getInstance(otherLink);
                        if (otherPopover) {
                            otherPopover.hide();
                        }
                    }
                });
                
                // Clear any existing timer for this link
                if (hoverTimer) {
                    clearTimeout(hoverTimer);
                }

                // Set timer to show popover after delay
                hoverTimer = setTimeout(function () {
                    // Initialize popover with loading state if not already initialized
                    var loadingContent = '<div class="text-center"><span class="spinner-border spinner-border-sm" role="status"></span> Laddar...</div>';
                    
                    if (!popoverInstance) {
                        popoverInstance = new bootstrap.Popover(linkElement, getPopoverConfig(loadingContent));
                    }
                    
                    // Show popover with loading state
                    popoverInstance.show();
                    isVisible = true;

                    // Fetch and update content
                    fetchCategoryImage(categoryId).then(function (imageData) {
                        // Only update if popover is still visible
                        if (isVisible) {
                            var content = generatePopoverContent(imageData, categoryName);
                            
                            // Dispose old popover and create new one with actual content
                            popoverInstance.dispose();
                            popoverInstance = new bootstrap.Popover(linkElement, getPopoverConfig(content));
                            popoverInstance.show();
                        }
                    });
                }, config.delay);
            });

            // Mouse leave event - cancel timer and hide popover
            linkElement.addEventListener('mouseleave', function () {
                // Mark as not visible
                isVisible = false;
                
                // Clear timer for this link
                if (hoverTimer) {
                    clearTimeout(hoverTimer);
                    hoverTimer = null;
                }

                // Hide popover
                if (popoverInstance) {
                    popoverInstance.hide();
                }
            });
        });

        // Also hide popover when clicking anywhere
        document.addEventListener('click', function () {
            links.forEach(function (link) {
                var popover = bootstrap.Popover.getInstance(link);
                if (popover) {
                    popover.hide();
                }
            });
        });
    }

    // Initialize when DOM is ready
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', initializeCategoryTooltips);
    } else {
        initializeCategoryTooltips();
    }

})();
