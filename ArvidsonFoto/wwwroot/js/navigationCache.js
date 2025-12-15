/**
 * Navigation Menu Cache
 * Caches the navigation menu data in localStorage to reduce database queries
 * Uses the LocalStorageCache utility for TTL management (1 hour default)
 */
(function (window) {
    'use strict';

    // Configuration
    var config = {
        cacheKey: 'navigation_menu_data',
        apiEndpoint: '/api/category/All'
    };

    /**
     * Navigation Cache Manager
     */
    var NavigationCache = {
        /**
         * Fetch categories from API
         * @returns {Promise} - Promise that resolves with categories data
         */
        fetchCategories: function () {
            return fetch(config.apiEndpoint, {
                method: 'GET',
                headers: {
                    'Accept': 'application/json'
                }
            })
            .then(function (response) {
                if (!response.ok) {
                    throw new Error('Failed to fetch categories: ' + response.status);
                }
                return response.json();
            })
            .catch(function (error) {
                console.error('Error fetching categories:', error);
                return null;
            });
        },

        /**
         * Get categories with caching
         * @returns {Promise} - Promise that resolves with categories data
         */
        getCategories: function () {
            // Check if LocalStorageCache is available
            if (!window.LocalStorageCache || !LocalStorageCache.isAvailable()) {
                // No caching available, fetch directly
                return this.fetchCategories();
            }

            // Try to get from cache
            var cachedData = LocalStorageCache.get(config.cacheKey);
            if (cachedData) {
                console.log('Using cached navigation menu data');
                return Promise.resolve(cachedData);
            }

            // Cache miss, fetch from API
            console.log('Cache miss, fetching navigation menu from API');
            return this.fetchCategories().then(function (data) {
                if (data) {
                    // Store in cache with 1 hour TTL (default)
                    LocalStorageCache.set(config.cacheKey, data);
                }
                return data;
            });
        },

        /**
         * Clear navigation cache
         */
        clearCache: function () {
            if (window.LocalStorageCache) {
                LocalStorageCache.remove(config.cacheKey);
                console.log('Navigation cache cleared');
            }
        },

        /**
         * Check if navigation data is cached
         * @returns {boolean} - True if cached
         */
        isCached: function () {
            if (!window.LocalStorageCache) {
                return false;
            }
            return LocalStorageCache.has(config.cacheKey);
        },

        /**
         * Get remaining TTL for cached navigation data
         * @returns {number} - Remaining time in milliseconds, or -1 if not cached
         */
        getRemainingTTL: function () {
            if (!window.LocalStorageCache) {
                return -1;
            }
            return LocalStorageCache.getRemainingTTL(config.cacheKey);
        }
    };

    // Expose to window
    window.NavigationCache = NavigationCache;

    // Preload categories on page load if not cached
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', function () {
            NavigationCache.getCategories();
        });
    } else {
        NavigationCache.getCategories();
    }

})(window);
