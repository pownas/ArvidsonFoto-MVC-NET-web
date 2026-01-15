/**
 * Category Cache Manager for ArvidsonFoto
 * 
 * This module provides client-side caching of categories using localStorage.
 * Since categories are updated only ~1 time per month, aggressive caching is beneficial.
 * 
 * Features:
 * - localStorage persistence across sessions
 * - Version control for cache invalidation
 * - Automatic pre-loading on page load
 * - Lightweight data structure (~300 KB for ~650 categories)
 * 
 * @module CategoryCache
 */

const CategoryCache = {
    // Configuration
    CACHE_KEY: 'arvidsonfoto_categories',
    CACHE_VERSION_KEY: 'arvidsonfoto_categories_version',
    CACHE_TIMESTAMP_KEY: 'arvidsonfoto_categories_timestamp',
    CURRENT_VERSION: '1.0', // Increment this when categories are updated via UploadAdmin
    API_ENDPOINT: '/api/category/AllLightweight',
    
    // Cache duration in milliseconds (24 hours)
    CACHE_DURATION: 24 * 60 * 60 * 1000,
    
    /**
     * Get all categories from cache or server
     * @returns {Promise<Array>} Array of category objects
     */
    async getCategories() {
        const cached = this.getFromLocalStorage();
        if (cached) {
            console.log(`✓ Categories loaded from localStorage cache (${cached.length} categories)`);
            return cached;
        }
        
        console.log('⌛ Loading categories from server...');
        try {
            const response = await fetch(this.API_ENDPOINT);
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            
            const categories = await response.json();
            this.saveToLocalStorage(categories);
            console.log(`✓ Loaded ${categories.length} categories from server and cached in localStorage`);
            return categories;
        } catch (error) {
            console.error('✗ Failed to load categories from server:', error);
            return [];
        }
    },
    
    /**
     * Get category name by ID (from cache)
     * @param {number} categoryId - Category ID
     * @returns {string|null} Category name or null if not found
     */
    getCategoryName(categoryId) {
        const categories = this.getFromLocalStorage();
        if (!categories) return null;
        
        const category = categories.find(c => c.categoryId === categoryId);
        return category ? category.name : null;
    },
    
    /**
     * Get category by ID (from cache)
     * @param {number} categoryId - Category ID
     * @returns {object|null} Category object or null if not found
     */
    getCategoryById(categoryId) {
        const categories = this.getFromLocalStorage();
        if (!categories) return null;
        
        return categories.find(c => c.categoryId === categoryId) || null;
    },
    
    /**
     * Get subcategories by parent ID (from cache)
     * @param {number} parentId - Parent category ID
     * @returns {Array} Array of subcategory objects
     */
    getSubcategories(parentId) {
        const categories = this.getFromLocalStorage();
        if (!categories) return [];
        
        return categories.filter(c => c.parentCategoryId === parentId);
    },
    
    /**
     * Get main categories (categories without parent) from cache
     * @returns {Array} Array of main category objects
     */
    getMainCategories() {
        const categories = this.getFromLocalStorage();
        if (!categories) return [];
        
        return categories.filter(c => !c.parentCategoryId || c.parentCategoryId === 0);
    },
    
    /**
     * Save categories to localStorage
     * @param {Array} categories - Array of category objects
     */
    saveToLocalStorage(categories) {
        try {
            localStorage.setItem(this.CACHE_KEY, JSON.stringify(categories));
            localStorage.setItem(this.CACHE_VERSION_KEY, this.CURRENT_VERSION);
            localStorage.setItem(this.CACHE_TIMESTAMP_KEY, Date.now().toString());
            
            const sizeInKB = (JSON.stringify(categories).length / 1024).toFixed(2);
            console.log(`✓ Cached ${categories.length} categories in localStorage (${sizeInKB} KB)`);
        } catch (e) {
            console.error('✗ Failed to cache categories in localStorage:', e);
            // localStorage might be full - try to clear old data
            if (e.name === 'QuotaExceededError') {
                console.warn('⚠ localStorage quota exceeded, clearing category cache...');
                this.clearCache();
            }
        }
    },
    
    /**
     * Get categories from localStorage
     * @returns {Array|null} Array of categories or null if not cached or expired
     */
    getFromLocalStorage() {
        try {
            // Check version
            const version = localStorage.getItem(this.CACHE_VERSION_KEY);
            if (version !== this.CURRENT_VERSION) {
                console.log('⚠ Cache version mismatch, clearing...');
                this.clearCache();
                return null;
            }
            
            // Check timestamp
            const timestamp = localStorage.getItem(this.CACHE_TIMESTAMP_KEY);
            if (timestamp) {
                const age = Date.now() - parseInt(timestamp);
                if (age > this.CACHE_DURATION) {
                    console.log('⚠ Cache expired (age: ' + Math.floor(age / 1000 / 60 / 60) + ' hours), refreshing...');
                    this.clearCache();
                    return null;
                }
            }
            
            // Get cached data
            const cached = localStorage.getItem(this.CACHE_KEY);
            return cached ? JSON.parse(cached) : null;
        } catch (e) {
            console.error('✗ Failed to read categories from localStorage:', e);
            return null;
        }
    },
    
    /**
     * Clear category cache
     */
    clearCache() {
        localStorage.removeItem(this.CACHE_KEY);
        localStorage.removeItem(this.CACHE_VERSION_KEY);
        localStorage.removeItem(this.CACHE_TIMESTAMP_KEY);
        console.log('✓ Category cache cleared');
    },
    
    /**
     * Invalidate cache (called when categories are updated via UploadAdmin)
     */
    invalidateCache() {
        this.clearCache();
        console.log('✓ Category cache invalidated - will reload on next request');
    },
    
    /**
     * Get cache statistics
     * @returns {object} Cache statistics object
     */
    getCacheStats() {
        const cached = this.getFromLocalStorage();
        const timestamp = localStorage.getItem(this.CACHE_TIMESTAMP_KEY);
        const version = localStorage.getItem(this.CACHE_VERSION_KEY);
        
        if (!cached) {
            return {
                isCached: false,
                count: 0,
                version: null,
                age: 0,
                sizeKB: 0
            };
        }
        
        const age = timestamp ? Date.now() - parseInt(timestamp) : 0;
        const sizeKB = (JSON.stringify(cached).length / 1024).toFixed(2);
        
        return {
            isCached: true,
            count: cached.length,
            version: version,
            ageHours: (age / 1000 / 60 / 60).toFixed(2),
            sizeKB: sizeKB,
            expiresIn: Math.max(0, this.CACHE_DURATION - age)
        };
    }
};

// Pre-load categories on page load (non-blocking)
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', async () => {
        await CategoryCache.getCategories();
    });
} else {
    // DOM already loaded
    CategoryCache.getCategories();
}

// Expose to window for global access
window.CategoryCache = CategoryCache;

// Log cache stats in development
if (window.location.hostname === 'localhost' || window.location.hostname === '127.0.0.1') {
    setTimeout(() => {
        const stats = CategoryCache.getCacheStats();
        console.log('📊 Category Cache Stats:', stats);
    }, 1000);
}
