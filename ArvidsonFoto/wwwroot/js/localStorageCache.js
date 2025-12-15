/**
 * LocalStorage Cache Utility
 * Provides centralized caching functionality with TTL (Time To Live) support
 * to reduce database queries and improve performance
 * 
 * Default TTL: 1 hour (3600000 ms)
 */
(function (window) {
    'use strict';

    // Configuration
    var config = {
        defaultTTL: 3600000, // 1 hour in milliseconds
        cacheVersion: '1.0', // Version for cache invalidation
        prefix: 'arvidsonfoto_' // Prefix for all cache keys
    };

    /**
     * LocalStorage Cache Manager
     */
    var LocalStorageCache = {
        /**
         * Check if localStorage is available and working
         * @returns {boolean} - True if localStorage is available
         */
        isAvailable: function () {
            try {
                var test = '__localStorage_test__';
                localStorage.setItem(test, test);
                localStorage.removeItem(test);
                return true;
            } catch (e) {
                return false;
            }
        },

        /**
         * Generate a cache key with prefix
         * @param {string} key - The cache key
         * @returns {string} - Prefixed cache key
         */
        _getKey: function (key) {
            return config.prefix + key;
        },

        /**
         * Set a value in cache with TTL
         * @param {string} key - The cache key
         * @param {*} value - The value to cache (will be JSON stringified)
         * @param {number} ttl - Time to live in milliseconds (optional, defaults to 1 hour)
         * @returns {boolean} - True if successfully cached
         */
        set: function (key, value, ttl) {
            if (!this.isAvailable()) {
                console.warn('LocalStorage is not available');
                return false;
            }

            try {
                var cacheKey = this._getKey(key);
                var ttlValue = ttl || config.defaultTTL;
                var now = new Date().getTime();
                var item = {
                    value: value,
                    timestamp: now,
                    expiry: now + ttlValue,
                    version: config.cacheVersion
                };

                localStorage.setItem(cacheKey, JSON.stringify(item));
                return true;
            } catch (e) {
                console.error('Error setting cache for key: ' + key, e);
                // Handle quota exceeded error
                if (e.name === 'QuotaExceededError') {
                    console.warn('LocalStorage quota exceeded, clearing old cache...');
                    this.clearExpired();
                }
                return false;
            }
        },

        /**
         * Get a value from cache
         * @param {string} key - The cache key
         * @returns {*} - The cached value or null if not found/expired
         */
        get: function (key) {
            if (!this.isAvailable()) {
                return null;
            }

            try {
                var cacheKey = this._getKey(key);
                var itemStr = localStorage.getItem(cacheKey);

                if (!itemStr) {
                    return null;
                }

                var item = JSON.parse(itemStr);
                var now = new Date().getTime();

                // Check version
                if (item.version !== config.cacheVersion) {
                    this.remove(key);
                    return null;
                }

                // Check if expired
                if (now > item.expiry) {
                    this.remove(key);
                    return null;
                }

                return item.value;
            } catch (e) {
                console.error('Error getting cache for key: ' + key, e);
                return null;
            }
        },

        /**
         * Remove a specific item from cache
         * @param {string} key - The cache key
         * @returns {boolean} - True if successfully removed
         */
        remove: function (key) {
            if (!this.isAvailable()) {
                return false;
            }

            try {
                var cacheKey = this._getKey(key);
                localStorage.removeItem(cacheKey);
                return true;
            } catch (e) {
                console.error('Error removing cache for key: ' + key, e);
                return false;
            }
        },

        /**
         * Check if a cache key exists and is not expired
         * @param {string} key - The cache key
         * @returns {boolean} - True if key exists and is valid
         */
        has: function (key) {
            return this.get(key) !== null;
        },

        /**
         * Clear all expired cache entries
         * @returns {number} - Number of entries cleared
         */
        clearExpired: function () {
            if (!this.isAvailable()) {
                return 0;
            }

            var cleared = 0;
            var now = new Date().getTime();
            var keysToRemove = [];

            try {
                // First pass: collect keys to remove
                for (var i = 0; i < localStorage.length; i++) {
                    var key = localStorage.key(i);
                    if (key && key.startsWith(config.prefix)) {
                        var itemStr = localStorage.getItem(key);
                        if (itemStr) {
                            try {
                                var item = JSON.parse(itemStr);
                                // Mark for removal if expired or wrong version
                                if (item.expiry < now || item.version !== config.cacheVersion) {
                                    keysToRemove.push(key);
                                }
                            } catch (e) {
                                // Invalid JSON, mark for removal
                                keysToRemove.push(key);
                            }
                        }
                    }
                }

                // Second pass: remove collected keys
                for (var j = 0; j < keysToRemove.length; j++) {
                    localStorage.removeItem(keysToRemove[j]);
                    cleared++;
                }
            } catch (e) {
                console.error('Error clearing expired cache', e);
            }

            return cleared;
        },

        /**
         * Clear all cache entries with the app prefix
         * @returns {number} - Number of entries cleared
         */
        clearAll: function () {
            if (!this.isAvailable()) {
                return 0;
            }

            var cleared = 0;
            var keysToRemove = [];

            try {
                // First pass: collect keys to remove
                for (var i = 0; i < localStorage.length; i++) {
                    var key = localStorage.key(i);
                    if (key && key.startsWith(config.prefix)) {
                        keysToRemove.push(key);
                    }
                }

                // Second pass: remove collected keys
                for (var j = 0; j < keysToRemove.length; j++) {
                    localStorage.removeItem(keysToRemove[j]);
                    cleared++;
                }
            } catch (e) {
                console.error('Error clearing all cache', e);
            }

            return cleared;
        },

        /**
         * Get cache statistics
         * @returns {Object} - Object with cache statistics
         */
        getStats: function () {
            if (!this.isAvailable()) {
                return { total: 0, valid: 0, expired: 0 };
            }

            var stats = {
                total: 0,
                valid: 0,
                expired: 0,
                wrongVersion: 0
            };

            var now = new Date().getTime();

            try {
                for (var i = 0; i < localStorage.length; i++) {
                    var key = localStorage.key(i);
                    if (key && key.startsWith(config.prefix)) {
                        stats.total++;
                        var itemStr = localStorage.getItem(key);
                        if (itemStr) {
                            try {
                                var item = JSON.parse(itemStr);
                                if (item.version !== config.cacheVersion) {
                                    stats.wrongVersion++;
                                } else if (item.expiry < now) {
                                    stats.expired++;
                                } else {
                                    stats.valid++;
                                }
                            } catch (e) {
                                // Invalid entry
                            }
                        }
                    }
                }
            } catch (e) {
                console.error('Error getting cache stats', e);
            }

            return stats;
        },

        /**
         * Get remaining TTL for a cached item
         * @param {string} key - The cache key
         * @returns {number} - Remaining time in milliseconds, or -1 if not found/expired
         */
        getRemainingTTL: function (key) {
            if (!this.isAvailable()) {
                return -1;
            }

            try {
                var cacheKey = this._getKey(key);
                var itemStr = localStorage.getItem(cacheKey);

                if (!itemStr) {
                    return -1;
                }

                var item = JSON.parse(itemStr);
                var now = new Date().getTime();

                if (item.version !== config.cacheVersion || now > item.expiry) {
                    return -1;
                }

                return item.expiry - now;
            } catch (e) {
                console.error('Error getting remaining TTL for key: ' + key, e);
                return -1;
            }
        },

        /**
         * Update cache version (will invalidate all existing cache)
         * @param {string} version - New version string
         */
        setVersion: function (version) {
            config.cacheVersion = version;
            this.clearExpired(); // Clear old version cache
        }
    };

    // Expose to window
    window.LocalStorageCache = LocalStorageCache;

    // Clean up expired cache on page load
    if (LocalStorageCache.isAvailable()) {
        try {
            var cleared = LocalStorageCache.clearExpired();
            if (cleared > 0) {
                console.log('Cleared ' + cleared + ' expired cache entries');
            }
        } catch (e) {
            console.error('Error during initial cache cleanup', e);
        }
    }

})(window);
