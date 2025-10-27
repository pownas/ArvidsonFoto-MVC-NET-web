// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Dark mode toggle functionality
(function () {
    // Check if user has a dark mode preference
    const darkModeToggle = document.getElementById('dark-mode-toggle');
    
    if (darkModeToggle) {
        // Load saved preference from localStorage
        const savedDarkMode = localStorage.getItem('darker-mode');
        if (savedDarkMode === 'enabled') {
            document.body.classList.add('darker-mode');
        }

        // Toggle dark mode on click
        darkModeToggle.addEventListener('click', function (e) {
            e.preventDefault();
            document.body.classList.toggle('darker-mode');

            // Save preference to localStorage
            if (document.body.classList.contains('darker-mode')) {
                localStorage.setItem('darker-mode', 'enabled');
            } else {
                localStorage.setItem('darker-mode', 'disabled');
            }
        });
    }
})();

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