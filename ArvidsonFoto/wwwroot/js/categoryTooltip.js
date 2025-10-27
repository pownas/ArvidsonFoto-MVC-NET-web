/**
 * Category Tooltip Feature
 * Displays a popover with the latest image when hovering over subcategory links
 */
(function ($) {
    'use strict';

    // Cache for storing fetched images to avoid repeated API calls
    var imageCache = {};

    // Configuration
    var config = {
        delay: 500,           // Delay before showing tooltip (ms)
        imageMaxWidth: 300,   // Max width of preview image
        imageMaxHeight: 200,  // Max height of preview image
        apiEndpoint: '/api/image/GetOneImageFromCategory/'
    };

    /**
     * Fetches the latest image for a category from the API
     * @param {number} categoryId - The category ID
     * @returns {Promise} - Promise that resolves with image data
     */
    function fetchCategoryImage(categoryId) {
        // Check cache first
        if (imageCache[categoryId]) {
            return Promise.resolve(imageCache[categoryId]);
        }

        // Fetch from API
        return $.ajax({
            url: config.apiEndpoint + categoryId,
            method: 'GET',
            dataType: 'json',
            timeout: 5000
        }).then(function (imageData) {
            // Cache the result
            imageCache[categoryId] = imageData;
            return imageData;
        }).catch(function (error) {
            console.warn('Failed to fetch image for category ' + categoryId + ':', error);
            return null;
        });
    }

    /**
     * Generates HTML content for the popover
     * @param {Object} imageData - Image data from API
     * @returns {string} - HTML content
     */
    function generatePopoverContent(imageData) {
        if (!imageData || !imageData.UrlImage) {
            return '<div class="text-muted">Ingen bild tillgänglig</div>';
        }

        var imagePath = imageData.UrlImage;
        // Ensure the path starts with a slash
        if (!imagePath.startsWith('/')) {
            imagePath = '/' + imagePath;
        }

        var html = '<div class="category-tooltip-content">';
        html += '<img src="' + imagePath + '" alt="' + (imageData.Name || 'Preview') + '" ';
        html += 'style="max-width: ' + config.imageMaxWidth + 'px; max-height: ' + config.imageMaxHeight + 'px; width: auto; height: auto; display: block;" />';
        
        if (imageData.DateImageTaken) {
            var date = new Date(imageData.DateImageTaken);
            var formattedDate = date.toLocaleDateString('sv-SE');
            html += '<div class="mt-2 small text-muted">Fotograferad: ' + formattedDate + '</div>';
        }
        
        html += '</div>';
        return html;
    }

    /**
     * Initializes popovers for category links
     */
    function initializeCategoryTooltips() {
        var $links = $('.has-category-tooltip');
        
        $links.each(function () {
            var $link = $(this);
            var categoryId = $link.data('category-id');
            var hoverTimer = null;

            if (!categoryId) {
                return; // Skip if no category ID
            }

            // Initialize popover with placeholder content
            $link.popover({
                trigger: 'manual',
                html: true,
                placement: 'right',
                container: 'body',
                content: '<div class="text-center"><span class="spinner-border spinner-border-sm" role="status"></span> Laddar...</div>',
                template: '<div class="popover category-tooltip-popover" role="tooltip"><div class="arrow"></div><div class="popover-body"></div></div>'
            });

            // Mouse enter event - start timer
            $link.on('mouseenter', function () {
                var $currentLink = $(this);
                
                // Clear any existing timer
                if (hoverTimer) {
                    clearTimeout(hoverTimer);
                }

                // Set timer to show popover after delay
                hoverTimer = setTimeout(function () {
                    // Show popover with loading state
                    $currentLink.popover('show');

                    // Fetch and update content
                    fetchCategoryImage(categoryId).then(function (imageData) {
                        if (imageData) {
                            var content = generatePopoverContent(imageData);
                            $currentLink.attr('data-content', content);
                            
                            // Update popover if still visible
                            if ($currentLink.attr('aria-describedby')) {
                                $currentLink.popover('dispose');
                                $currentLink.popover({
                                    trigger: 'manual',
                                    html: true,
                                    placement: 'right',
                                    container: 'body',
                                    content: content,
                                    template: '<div class="popover category-tooltip-popover" role="tooltip"><div class="arrow"></div><div class="popover-body"></div></div>'
                                });
                                $currentLink.popover('show');
                            }
                        }
                    });
                }, config.delay);
            });

            // Mouse leave event - cancel timer and hide popover
            $link.on('mouseleave', function () {
                var $currentLink = $(this);
                
                // Clear timer
                if (hoverTimer) {
                    clearTimeout(hoverTimer);
                    hoverTimer = null;
                }

                // Hide popover
                $currentLink.popover('hide');
            });
        });

        // Also hide popover when clicking anywhere
        $(document).on('click', function () {
            $links.popover('hide');
        });
    }

    // Initialize when DOM is ready
    $(document).ready(function () {
        initializeCategoryTooltips();
    });

})(jQuery);
