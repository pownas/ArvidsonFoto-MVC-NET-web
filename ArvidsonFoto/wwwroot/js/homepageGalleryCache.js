/**
 * Homepage Gallery Cache
 * Caches homepage random gallery images in localStorage to reduce database queries
 * Uses the LocalStorageCache utility for TTL management (1 hour default)
 */
(function (window) {
    'use strict';

    // Configuration
    var config = {
        cacheKey: 'homepage_gallery_images',
        // Note: This would require a new API endpoint or we use existing functionality
        // For now, we'll cache the rendered gallery HTML or image data
    };

    /**
     * Homepage Gallery Cache Manager
     */
    var HomepageGalleryCache = {
        /**
         * Cache gallery data
         * @param {*} data - The gallery data to cache
         * @returns {boolean} - True if successfully cached
         */
        setGalleryData: function (data) {
            if (!window.LocalStorageCache || !LocalStorageCache.isAvailable()) {
                return false;
            }
            return LocalStorageCache.set(config.cacheKey, data);
        },

        /**
         * Get cached gallery data
         * @returns {*} - The cached gallery data or null if not found/expired
         */
        getGalleryData: function () {
            if (!window.LocalStorageCache || !LocalStorageCache.isAvailable()) {
                return null;
            }
            return LocalStorageCache.get(config.cacheKey);
        },

        /**
         * Clear gallery cache
         */
        clearCache: function () {
            if (window.LocalStorageCache) {
                LocalStorageCache.remove(config.cacheKey);
                console.log('Homepage gallery cache cleared');
            }
        },

        /**
         * Check if gallery data is cached
         * @returns {boolean} - True if cached
         */
        isCached: function () {
            if (!window.LocalStorageCache) {
                return false;
            }
            return LocalStorageCache.has(config.cacheKey);
        }
    };

    // Expose to window
    window.HomepageGalleryCache = HomepageGalleryCache;

})(window);
