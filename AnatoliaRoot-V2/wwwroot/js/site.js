// Site.js - Ana site JavaScript dosyası
// Bu dosya, sitenin genel JavaScript işlevlerini içerir

// DOM yüklendiğinde çalışacak fonksiyonlar
document.addEventListener('DOMContentLoaded', function() {
    // Smooth scroll için
    const links = document.querySelectorAll('a[href^="#"]');
    links.forEach(link => {
        link.addEventListener('click', function(e) {
            e.preventDefault();
            const targetId = this.getAttribute('href');
            const targetElement = document.querySelector(targetId);
            if (targetElement) {
                targetElement.scrollIntoView({
                    behavior: 'smooth',
                    block: 'start'
                });
            }
        });
    });

    // Navbar scroll efekti
    const navbar = document.querySelector('#navigation');
    if (navbar) {
        window.addEventListener('scroll', function() {
            if (window.scrollY > 100) {
                navbar.classList.add('scrolled');
            } else {
                navbar.classList.remove('scrolled');
            }
        });
    }

    // Mobil menü toggle
    const burgerIcon = document.querySelector('.burger-icon');
    const navbarCollapse = document.querySelector('#navbarCollapse');
    
    if (burgerIcon && navbarCollapse) {
        burgerIcon.addEventListener('click', function() {
            navbarCollapse.classList.toggle('show');
            burgerIcon.classList.toggle('active');
        });
    }

    // Dropdown menüler için
    const subnavs = document.querySelectorAll('.subnav');
    subnavs.forEach(subnav => {
        const expandIcon = subnav.querySelector('.nav__expand');
        const subMenu = subnav.querySelector('.sub-menu');
        
        if (expandIcon && subMenu) {
            expandIcon.addEventListener('click', function(e) {
                e.preventDefault();
                subMenu.classList.toggle('show');
                expandIcon.classList.toggle('expanded');
            });
        }
    });

    // Form validasyonu için genel fonksiyonlar
    const forms = document.querySelectorAll('form');
    forms.forEach(form => {
        form.addEventListener('submit', function(e) {
            const requiredFields = form.querySelectorAll('[required]');
            let isValid = true;
            
            requiredFields.forEach(field => {
                if (!field.value.trim()) {
                    isValid = false;
                    field.classList.add('error');
                } else {
                    field.classList.remove('error');
                }
            });
            
            if (!isValid) {
                e.preventDefault();
                alert('Lütfen tüm zorunlu alanları doldurun.');
            }
        });
    });

    // Lazy loading için
    const images = document.querySelectorAll('img[data-src]');
    const imageObserver = new IntersectionObserver((entries, observer) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                const img = entry.target;
                img.src = img.dataset.src;
                img.classList.remove('lazy');
                imageObserver.unobserve(img);
            }
        });
    });

    images.forEach(img => imageObserver.observe(img));
});

// Utility fonksiyonlar
window.utils = {
    // Debounce fonksiyonu
    debounce: function(func, wait) {
        let timeout;
        return function executedFunction(...args) {
            const later = () => {
                clearTimeout(timeout);
                func(...args);
            };
            clearTimeout(timeout);
            timeout = setTimeout(later, wait);
        };
    },

    // Throttle fonksiyonu
    throttle: function(func, limit) {
        let inThrottle;
        return function() {
            const args = arguments;
            const context = this;
            if (!inThrottle) {
                func.apply(context, args);
                inThrottle = true;
                setTimeout(() => inThrottle = false, limit);
            }
        };
    },

    // Local storage yardımcı fonksiyonları
    storage: {
        set: function(key, value) {
            try {
                localStorage.setItem(key, JSON.stringify(value));
            } catch (e) {
                console.error('Local storage error:', e);
            }
        },
        get: function(key) {
            try {
                const item = localStorage.getItem(key);
                return item ? JSON.parse(item) : null;
            } catch (e) {
                console.error('Local storage error:', e);
                return null;
            }
        },
        remove: function(key) {
            try {
                localStorage.removeItem(key);
            } catch (e) {
                console.error('Local storage error:', e);
            }
        }
    }
};

// Global error handler
window.addEventListener('error', function(e) {
    console.error('Global error:', e.error);
});

// Unhandled promise rejection handler
window.addEventListener('unhandledrejection', function(e) {
    console.error('Unhandled promise rejection:', e.reason);
}); 