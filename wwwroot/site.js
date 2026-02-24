// Add your custom JavaScript here
console.log('Registry System loaded');

// Theme and accessibility helpers
(function () {
    const layoutRoot = document.getElementById('layout-root');

    function applySettings() {
        const theme = localStorage.getItem('registry_theme') || 'light';
        const zoom = localStorage.getItem('registry_zoom') || '100';
        if (theme === 'dark') document.body.classList.add('theme-dark'); else document.body.classList.remove('theme-dark');
        document.body.classList.remove('zoom-100', 'zoom-110', 'zoom-125', 'zoom-150');
        document.body.classList.add('zoom-' + zoom);
    }

    const toggle = document.getElementById('sidebarToggle');
    if (toggle) {
        toggle.addEventListener('click', function () {
            document.body.classList.toggle('collapsed');
        });
    }

    // Expose save functions
    window.registrySettings = {
        setTheme: function (theme) { localStorage.setItem('registry_theme', theme); applySettings(); },
        setZoom: function (z) { localStorage.setItem('registry_zoom', z); applySettings(); }
    };

    applySettings();
})();