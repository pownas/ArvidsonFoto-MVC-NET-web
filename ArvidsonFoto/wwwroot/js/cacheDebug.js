/**
 * Cache Debug Utility
 * Developer utility for inspecting and managing localStorage cache
 * Only included in development builds
 * 
 * Usage in browser console:
 * - CacheDebug.showAll() - Display all cache entries
 * - CacheDebug.showStats() - Display cache statistics
 * - CacheDebug.clearAll() - Clear all cache
 * - CacheDebug.clearExpired() - Clear expired cache
 * - CacheDebug.testCache() - Run cache tests
 */
(function (window) {
    'use strict';

    var CacheDebug = {
        /**
         * Display all cache entries in console
         */
        showAll: function () {
            if (!window.LocalStorageCache || !LocalStorageCache.isAvailable()) {
                console.log('LocalStorage is not available');
                return;
            }

            console.group('🗄️ ArvidsonFoto Cache Contents');
            var count = 0;
            var prefix = 'arvidsonfoto_';

            for (var i = 0; i < localStorage.length; i++) {
                var key = localStorage.key(i);
                if (key && key.startsWith(prefix)) {
                    try {
                        var itemStr = localStorage.getItem(key);
                        var item = JSON.parse(itemStr);
                        var remaining = LocalStorageCache.getRemainingTTL(key.substring(prefix.length));
                        var remainingMin = Math.floor(remaining / 60000);
                        
                        console.log('Key:', key.substring(prefix.length));
                        console.log('  Version:', item.version);
                        console.log('  Expires in:', remainingMin, 'minutes');
                        console.log('  Value:', item.value);
                        console.log('---');
                        count++;
                    } catch (e) {
                        console.error('Error parsing cache entry:', key, e);
                    }
                }
            }

            console.log('Total cache entries:', count);
            console.groupEnd();
        },

        /**
         * Display cache statistics
         */
        showStats: function () {
            if (!window.LocalStorageCache) {
                console.log('LocalStorageCache is not available');
                return;
            }

            var stats = LocalStorageCache.getStats();
            console.group('📊 Cache Statistics');
            console.log('Total entries:', stats.total);
            console.log('Valid entries:', stats.valid);
            console.log('Expired entries:', stats.expired);
            console.log('Wrong version entries:', stats.wrongVersion);
            console.groupEnd();
        },

        /**
         * Clear all cache
         */
        clearAll: function () {
            if (!window.LocalStorageCache) {
                console.log('LocalStorageCache is not available');
                return;
            }

            var cleared = LocalStorageCache.clearAll();
            console.log('✅ Cleared', cleared, 'cache entries');
        },

        /**
         * Clear expired cache
         */
        clearExpired: function () {
            if (!window.LocalStorageCache) {
                console.log('LocalStorageCache is not available');
                return;
            }

            var cleared = LocalStorageCache.clearExpired();
            console.log('✅ Cleared', cleared, 'expired cache entries');
        },

        /**
         * Test cache functionality
         */
        testCache: function () {
            if (!window.LocalStorageCache || !LocalStorageCache.isAvailable()) {
                console.error('❌ LocalStorage is not available');
                return;
            }

            console.group('🧪 Running Cache Tests');

            // Test 1: Set and get
            console.log('Test 1: Set and get cache');
            var testKey = 'test_key';
            var testValue = { message: 'Hello from cache', timestamp: Date.now() };
            LocalStorageCache.set(testKey, testValue);
            var retrieved = LocalStorageCache.get(testKey);
            if (retrieved && retrieved.message === testValue.message) {
                console.log('✅ Test 1 passed');
            } else {
                console.error('❌ Test 1 failed');
            }

            // Test 2: Has function
            console.log('Test 2: Has function');
            if (LocalStorageCache.has(testKey)) {
                console.log('✅ Test 2 passed');
            } else {
                console.error('❌ Test 2 failed');
            }

            // Test 3: TTL
            console.log('Test 3: TTL check');
            var ttl = LocalStorageCache.getRemainingTTL(testKey);
            if (ttl > 0 && ttl <= 3600000) {
                console.log('✅ Test 3 passed - TTL:', Math.floor(ttl / 60000), 'minutes');
            } else {
                console.error('❌ Test 3 failed');
            }

            // Test 4: Remove
            console.log('Test 4: Remove cache');
            LocalStorageCache.remove(testKey);
            if (!LocalStorageCache.has(testKey)) {
                console.log('✅ Test 4 passed');
            } else {
                console.error('❌ Test 4 failed');
            }

            // Test 5: Short TTL expiration
            console.log('Test 5: Short TTL expiration (will take 2 seconds)');
            LocalStorageCache.set('test_expire', { data: 'expires soon' }, 1000); // 1 second TTL
            setTimeout(function () {
                if (!LocalStorageCache.has('test_expire')) {
                    console.log('✅ Test 5 passed - Cache expired correctly');
                } else {
                    console.error('❌ Test 5 failed - Cache should have expired');
                }
                console.groupEnd();
            }, 2000);

            console.log('Running async test 5...');
        },

        /**
         * Test specific cache modules
         */
        testModules: function () {
            console.group('🧪 Testing Cache Modules');

            // Test NavigationCache
            if (window.NavigationCache) {
                console.log('NavigationCache available:');
                console.log('  Is cached:', NavigationCache.isCached());
                console.log('  Remaining TTL:', Math.floor(NavigationCache.getRemainingTTL() / 60000), 'minutes');
            }

            // Test HomepageGalleryCache
            if (window.HomepageGalleryCache) {
                console.log('HomepageGalleryCache available:');
                console.log('  Is cached:', HomepageGalleryCache.isCached());
            }

            // Test StaticContentCache
            if (window.StaticContentCache) {
                console.log('StaticContentCache available');
            }

            console.groupEnd();
        },

        /**
         * Display help
         */
        help: function () {
            console.group('💡 Cache Debug Utility - Available Commands');
            console.log('CacheDebug.showAll()       - Display all cache entries');
            console.log('CacheDebug.showStats()     - Display cache statistics');
            console.log('CacheDebug.clearAll()      - Clear all cache');
            console.log('CacheDebug.clearExpired()  - Clear expired cache');
            console.log('CacheDebug.testCache()     - Run cache tests');
            console.log('CacheDebug.testModules()   - Test cache modules');
            console.log('CacheDebug.help()          - Display this help');
            console.groupEnd();
        }
    };

    // Expose to window
    window.CacheDebug = CacheDebug;

    // Display help on load
    console.log('💡 Cache Debug Utility loaded. Type CacheDebug.help() for available commands.');

})(window);
