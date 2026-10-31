(function () {
    'use strict';

    var pageParent = document.querySelector('meta[name="arvidsonfoto-traceparent"]')?.content;
    var operations = new Set(['tooltip.prefetch', 'tooltip.hover', 'gallery.next-page', 'category.cache-load']);

    window.ArvidsonFotoTelemetry = {
        fetch: function (url, options, operation) {
            var target = new URL(url, document.baseURI);
            if (pageParent && /^00-[0-9a-f]{32}-[0-9a-f]{16}-[0-9a-f]{2}$/.test(pageParent)
                && target.origin === window.location.origin && target.pathname.startsWith('/api/')) {
                var headers = new Headers(options?.headers);
                if (!headers.has('traceparent')) {
                    headers.set('traceparent', pageParent);
                }
                if (operations.has(operation)) {
                    headers.set('X-ArvidsonFoto-Operation', operation);
                }
                options = Object.assign({}, options, { headers: headers });
            }

            return window.fetch(url, options);
        }
    };
})();