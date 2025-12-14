// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Dark mode toggle functionality using Bootstrap 5.3 color modes
document.addEventListener('DOMContentLoaded', function() {
    const darkModeToggle = document.getElementById('dark-mode-toggle');
    
    if (darkModeToggle) {
        const htmlElement = document.documentElement;
        
        // Load saved theme preference from localStorage
        const savedTheme = localStorage.getItem('bs-theme');
        
        if (savedTheme === 'dark') {
            htmlElement.setAttribute('data-bs-theme', 'dark');
        } else {
            htmlElement.setAttribute('data-bs-theme', 'light');
        }

        // Toggle dark mode on click
        darkModeToggle.addEventListener('click', function (e) {
            e.preventDefault();
            
            const currentTheme = htmlElement.getAttribute('data-bs-theme');
            const newTheme = currentTheme === 'dark' ? 'light' : 'dark';
            
            htmlElement.setAttribute('data-bs-theme', newTheme);
            localStorage.setItem('bs-theme', newTheme);
        });
    }
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