/**
 * Navigation Menu Cache Manager
 * Handles client-side caching of navigation menu categories in localStorage
 * to improve page load performance by avoiding server-side rendering of 550+ categories
 */
(function (window) {
    'use strict';

    const CACHE_KEY = 'arvidsonfoto_navigation_menu_data';
    const CACHE_VERSION_KEY = 'arvidsonfoto_navigation_menu_version';
    const CACHE_VERSION = '1.0'; // Increment this to invalidate all caches
    const CACHE_EXPIRY_HOURS = 24; // Cache expires after 24 hours
    const API_ENDPOINT = '/api/category/All';

    /**
     * Navigation Menu Cache Manager
     */
    const NavigationMenuCache = {
        /**
         * Checks if cached data exists and is still valid
         * @returns {boolean}
         */
        isCacheValid: function () {
            try {
                const cachedVersion = localStorage.getItem(CACHE_VERSION_KEY);
                const cachedData = localStorage.getItem(CACHE_KEY);

                if (!cachedData || !cachedVersion || cachedVersion !== CACHE_VERSION) {
                    return false;
                }

                const cache = JSON.parse(cachedData);
                if (!cache.timestamp || !cache.categories) {
                    return false;
                }

                // Check if cache has expired
                const cacheAge = Date.now() - cache.timestamp;
                const maxAge = CACHE_EXPIRY_HOURS * 60 * 60 * 1000;

                return cacheAge < maxAge;
            } catch (error) {
                console.warn('Error checking cache validity:', error);
                return false;
            }
        },

        /**
         * Gets categories from cache
         * @returns {Array|null} Array of categories or null if cache is invalid
         */
        getCachedCategories: function () {
            try {
                if (!this.isCacheValid()) {
                    return null;
                }

                const cachedData = localStorage.getItem(CACHE_KEY);
                const cache = JSON.parse(cachedData);
                console.log(`Loaded ${cache.categories.length} categories from localStorage cache`);
                return cache.categories;
            } catch (error) {
                console.error('Error getting cached categories:', error);
                return null;
            }
        },

        /**
         * Saves categories to cache
         * @param {Array} categories - Array of category objects
         */
        setCachedCategories: function (categories) {
            try {
                const cacheData = {
                    timestamp: Date.now(),
                    categories: categories,
                    count: categories.length
                };

                localStorage.setItem(CACHE_KEY, JSON.stringify(cacheData));
                localStorage.setItem(CACHE_VERSION_KEY, CACHE_VERSION);
                console.log(`Cached ${categories.length} categories to localStorage`);
            } catch (error) {
                console.error('Error caching categories:', error);
                // If localStorage is full, try to clear old data
                if (error.name === 'QuotaExceededError') {
                    console.warn('localStorage quota exceeded, clearing cache');
                    this.clearCache();
                }
            }
        },

        /**
         * Fetches categories from API
         * @returns {Promise<Array>} Promise that resolves with array of categories
         */
        fetchCategories: function () {
            console.log('Fetching categories from API:', API_ENDPOINT);
            return fetch(API_ENDPOINT, {
                method: 'GET',
                headers: {
                    'Accept': 'application/json',
                    'Cache-Control': 'no-cache'
                }
            })
                .then(function (response) {
                    if (!response.ok) {
                        throw new Error('Network response was not ok: ' + response.statusText);
                    }
                    return response.json();
                })
                .then(function (categories) {
                    console.log(`Fetched ${categories.length} categories from API`);
                    return categories;
                })
                .catch(function (error) {
                    console.error('Error fetching categories from API:', error);
                    throw error;
                });
        },

        /**
         * Gets categories from cache or fetches from API if needed
         * @returns {Promise<Array>} Promise that resolves with array of categories
         */
        getCategories: function () {
            const cachedCategories = this.getCachedCategories();

            if (cachedCategories) {
                // Return cached data as a resolved promise
                return Promise.resolve(cachedCategories);
            }

            // Fetch from API and cache the result
            return this.fetchCategories()
                .then(function (categories) {
                    NavigationMenuCache.setCachedCategories(categories);
                    return categories;
                });
        },

        /**
         * Forces a refresh of the cache by fetching from API
         * @returns {Promise<Array>} Promise that resolves with array of categories
         */
        refreshCache: function () {
            console.log('Force refreshing category cache...');
            return this.fetchCategories()
                .then(function (categories) {
                    NavigationMenuCache.setCachedCategories(categories);
                    return categories;
                });
        },

        /**
         * Clears the cached data
         */
        clearCache: function () {
            try {
                localStorage.removeItem(CACHE_KEY);
                localStorage.removeItem(CACHE_VERSION_KEY);
                console.log('Navigation menu cache cleared');
            } catch (error) {
                console.error('Error clearing cache:', error);
            }
        },

        /**
         * Gets cache statistics
         * @returns {Object} Cache statistics
         */
        getCacheStats: function () {
            try {
                const cachedData = localStorage.getItem(CACHE_KEY);
                if (!cachedData) {
                    return { exists: false };
                }

                const cache = JSON.parse(cachedData);
                const cacheAge = Date.now() - cache.timestamp;
                const hoursOld = Math.floor(cacheAge / (60 * 60 * 1000));

                return {
                    exists: true,
                    count: cache.count,
                    timestamp: new Date(cache.timestamp).toLocaleString('sv-SE'),
                    hoursOld: hoursOld,
                    sizeKB: Math.round((cachedData.length * 2) / 1024), // Rough estimate
                    version: localStorage.getItem(CACHE_VERSION_KEY),
                    isValid: this.isCacheValid()
                };
            } catch (error) {
                console.error('Error getting cache stats:', error);
                return { exists: false, error: error.message };
            }
        }
    };

    // Expose to global scope
    window.NavigationMenuCache = NavigationMenuCache;

    // Log cache stats on page load (in development)
    if (window.location.hostname === 'localhost' || window.location.hostname === '127.0.0.1') {
        const stats = NavigationMenuCache.getCacheStats();
        if (stats.exists) {
            console.log('Navigation menu cache stats:', stats);
        }
    }

})(window);
