/**
 * Static Content Cache
 * Caches static page content in localStorage to reduce server requests
 * Suitable for pages that rarely change: Om mig, Copyright, Kontakta info, etc.
 * Uses the LocalStorageCache utility for TTL management (1 hour default)
 */
(function (window) {
    'use strict';

    /**
     * Static Content Cache Manager
     */
    var StaticContentCache = {
        /**
         * Generate cache key for a page
         * @param {string} pageId - Unique identifier for the page
         * @returns {string} - Cache key
         */
        _getCacheKey: function (pageId) {
            return 'static_content_' + pageId;
        },

        /**
         * Cache page content
         * @param {string} pageId - Unique identifier for the page
         * @param {Object} content - Content object to cache (can contain HTML, data, etc.)
         * @param {number} ttl - Optional custom TTL in milliseconds
         * @returns {boolean} - True if successfully cached
         */
        setContent: function (pageId, content, ttl) {
            if (!window.LocalStorageCache || !LocalStorageCache.isAvailable()) {
                return false;
            }
            var cacheKey = this._getCacheKey(pageId);
            return LocalStorageCache.set(cacheKey, content, ttl);
        },

        /**
         * Get cached page content
         * @param {string} pageId - Unique identifier for the page
         * @returns {Object|null} - The cached content or null if not found/expired
         */
        getContent: function (pageId) {
            if (!window.LocalStorageCache || !LocalStorageCache.isAvailable()) {
                return null;
            }
            var cacheKey = this._getCacheKey(pageId);
            return LocalStorageCache.get(cacheKey);
        },

        /**
         * Clear cache for a specific page
         * @param {string} pageId - Unique identifier for the page
         */
        clearContent: function (pageId) {
            if (window.LocalStorageCache) {
                var cacheKey = this._getCacheKey(pageId);
                LocalStorageCache.remove(cacheKey);
                console.log('Static content cache cleared for: ' + pageId);
            }
        },

        /**
         * Check if page content is cached
         * @param {string} pageId - Unique identifier for the page
         * @returns {boolean} - True if cached and valid
         */
        isCached: function (pageId) {
            if (!window.LocalStorageCache) {
                return false;
            }
            var cacheKey = this._getCacheKey(pageId);
            return LocalStorageCache.has(cacheKey);
        },

        /**
         * Cache element's HTML content by selector
         * @param {string} pageId - Unique identifier for the page
         * @param {string} selector - CSS selector for the element to cache
         * @returns {boolean} - True if successfully cached
         */
        cacheElementContent: function (pageId, selector) {
            var element = document.querySelector(selector);
            if (!element) {
                console.warn('Element not found for selector: ' + selector);
                return false;
            }
            
            var content = {
                html: element.innerHTML,
                timestamp: new Date().toISOString()
            };
            
            return this.setContent(pageId, content);
        },

        /**
         * Restore cached HTML content to an element
         * @param {string} pageId - Unique identifier for the page
         * @param {string} selector - CSS selector for the target element
         * @returns {boolean} - True if successfully restored
         */
        restoreElementContent: function (pageId, selector) {
            var content = this.getContent(pageId);
            if (!content || !content.html) {
                return false;
            }
            
            var element = document.querySelector(selector);
            if (!element) {
                console.warn('Element not found for selector: ' + selector);
                return false;
            }
            
            element.innerHTML = content.html;
            console.log('Restored cached content for: ' + pageId);
            return true;
        }
    };

    // Expose to window
    window.StaticContentCache = StaticContentCache;

})(window);
