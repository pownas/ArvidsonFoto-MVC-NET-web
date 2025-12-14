// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Dark mode toggle functionality
document.addEventListener('DOMContentLoaded', function() {
    const darkModeToggle = document.getElementById('dark-mode-toggle');
    
    if (darkModeToggle) {
        const icon = darkModeToggle.querySelector('i');
        
        // Function to update icon and tooltip based on theme
        function updateIcon(isDark) {
            if (isDark) {
                // Dark mode: Show filled/lit bulb (yellow/white)
                icon.className = 'bi bi-lightbulb-fill';
                darkModeToggle.setAttribute('aria-label', 'Tänd ljuset');
                darkModeToggle.setAttribute('title', 'Tänd ljuset');
            } else {
                // Light mode: Show empty/off bulb
                icon.className = 'bi bi-lightbulb';
                darkModeToggle.setAttribute('aria-label', 'Släck ljuset');
                darkModeToggle.setAttribute('title', 'Släck ljuset');
            }
        }
        
        // Check for saved theme preference or use system preference
        const savedTheme = localStorage.getItem('theme');
        let isDarkMode = false;
        
        if (savedTheme) {
            // User has explicitly chosen a theme
            isDarkMode = savedTheme === 'dark';
        } else {
            // Check system preference
            isDarkMode = window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches;
        }
        
        // Apply theme
        if (isDarkMode) {
            document.documentElement.setAttribute('data-bs-theme', 'dark');
            updateIcon(true);
        } else {
            document.documentElement.removeAttribute('data-bs-theme');
            updateIcon(false);
        }
        
        // Listen for system theme changes (if user hasn't set preference)
        if (!savedTheme && window.matchMedia) {
            window.matchMedia('(prefers-color-scheme: dark)').addEventListener('change', function(e) {
                // Only apply if user hasn't manually set a preference
                if (!localStorage.getItem('theme')) {
                    if (e.matches) {
                        document.documentElement.setAttribute('data-bs-theme', 'dark');
                        updateIcon(true);
                    } else {
                        document.documentElement.removeAttribute('data-bs-theme');
                        updateIcon(false);
                    }
                }
            });
        }

        // Toggle dark mode on click
        darkModeToggle.addEventListener('click', function (e) {
            e.preventDefault();
            
            const currentTheme = document.documentElement.getAttribute('data-bs-theme');
            
            if (currentTheme === 'dark') {
                document.documentElement.removeAttribute('data-bs-theme');
                localStorage.setItem('theme', 'light');
                updateIcon(false);
            } else {
                document.documentElement.setAttribute('data-bs-theme', 'dark');
                localStorage.setItem('theme', 'dark');
                updateIcon(true);
            }
        });
    }
});

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