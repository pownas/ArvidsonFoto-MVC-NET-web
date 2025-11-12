// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Initialize Bootstrap popovers for category tooltips
$(document).ready(function () {
    $('[data-toggle="popover"]').popover({
        container: 'body',
        template: '<div class="popover category-popover" role="tooltip"><div class="arrow"></div><h3 class="popover-header"></h3><div class="popover-body"></div></div>'
    });
});

// <!-- Google analytics START -->
var _gaq = _gaq || [];
_gaq.push(['_setAccount', 'UA-29010909-1']);
_gaq.push(['_trackPageview']);
(function () {
    var ga = document.createElement('script'); ga.type = 'text/javascript'; ga.async = true;
    ga.src = ('https:' == document.location.protocol ? 'https://ssl' : 'http://www') + '.google-analytics.com/ga.js';
    var s = document.getElementsByTagName('script')[0]; s.parentNode.insertBefore(ga, s);
})();
// <!-- Google analytics END -->