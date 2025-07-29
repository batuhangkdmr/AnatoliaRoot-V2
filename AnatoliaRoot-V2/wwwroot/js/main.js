window.jQuery(document).ready(function($) {

    'use strict';

    if ($(document).width() >= 769) {
        $(window).on("resize", function() {
            if ($(window).width() < 769) {
                $('.header-content').height("auto");
            } else {
                var height = $(window).height();
                $('.header-content').height(height - 250);
            }
        }).resize();
    } else {}


    $('a[href*="#"]').on('click', function(e) {
        e.preventDefault();
        var target = this.hash;
        var $target = $(target);
        $('html, body').stop().animate({
            'scrollTop': $target.offset().top
        }, 900, 'swing', function() {
            window.location.hash = target;
        });
    });

    jQuery(document).ready(function($) {
        var offset = 300,
            offset_opacity = 1200,
            scroll_top_duration = 700,
            $back_to_top = $('.cd-top');
        $(window).scroll(function() {
            ($(this).scrollTop() > offset) ? $back_to_top.addClass('cd-is-visible'): $back_to_top.removeClass('cd-is-visible cd-fade-out');
            if ($(this).scrollTop() > offset_opacity) {
                $back_to_top.addClass('cd-fade-out');
            }
        });
        $back_to_top.on('click', function(event) {
            event.preventDefault();
            $('body,html').animate({
                scrollTop: 0,
            }, scroll_top_duration);
        });
    });


    $('body').scrollspy({
        target: '#navigation .navbar-collapse',
        offset: parseInt($('#navigation').height(), 0)
    });

    $(window).on('scroll', function() {
        var scroll = $(window).scrollTop();
        if (scroll < 245) {
            $("#header").removeClass("sticky-menu");
        } else {
            $("#header").addClass("sticky-menu");
        }
    });

    $('.popup-image').magnificPopup({
        type: 'image',
        gallery: {
            enabled: true
        }
    });


    $('.popup-video').magnificPopup({
        type: 'iframe',
        gallery: {
            enabled: true
        }
    });


    $('.btn-tooltip').tooltip();
    $('.btn-popover').popover();

    var sliderBgSetting = $(".bck");
    sliderBgSetting.each(function(indx) {
        if ($(this).attr("data-background")) {
            $(this).css("background-image", "url(" + $(this).data("background") + ")");
        }
    });

    $('.carousel-slider.gallery-slider').slick({
        arrows: false,
        dots: true,
        slidesToShow: 4,
        slidesToScroll: 1,
        autoplay: true,
        autoplaySpeed: 5000,
        draggable: true,
        responsive: [{
                breakpoint: 1250,
                settings: {
                    slidesToShow: 3,
                    draggable: true
                }
            },
            {
                breakpoint: 990,
                settings: {
                    slidesToShow: 1,
                    draggable: true
                }
            },
            {
                breakpoint: 767,
                settings: {
                    slidesToShow: 1,
                    draggable: true
                }

            }
        ]
    });


    $('.carousel-slider.projects-slider').slick({
        arrows: false,
        dots: true,
        slidesToShow: 4,
        slidesToScroll: 1,
        autoplay: true,
        autoplaySpeed: 5000,
        draggable: true,
        responsive: [{
                breakpoint: 1200,
                settings: {
                    slidesToShow: 3,
                    draggable: true
                }
            },
            {
                breakpoint: 990,
                settings: {
                    slidesToShow: 2,
                    draggable: true
                }
            },
            {
                breakpoint: 767,
                settings: {
                    slidesToShow: 1,
                    draggable: true
                }

            }
        ]
    });

    $('.carousel-slider.sponsor-slider').slick({
        arrows: false,
        dots: false,
        margin: 10,
        slidesToShow: 6,
        slidesToScroll: 1,
        autoplay: true,
        autoplaySpeed: 5000,
        draggable: true,
        responsive: [{
                breakpoint: 1200,
                settings: {
                    slidesToShow: 6,
                    draggable: true
                }
            },
            {
                breakpoint: 990,
                settings: {
                    slidesToShow: 4,
                    draggable: true
                }
            },
            {
                breakpoint: 767,
                settings: {
                    slidesToShow: 3,
                    draggable: true
                }

            }
        ]
    });
    $('.carousel-slider.blog-slider').slick({
        arrows: false,
        dots: false,
        slidesToShow: 3,
        slidesToScroll: 1,
        autoplay: true,
        autoplaySpeed: 5000,
        draggable: true,
        responsive: [{
                breakpoint: 1200,
                settings: {
                    slidesToShow: 3,
                    draggable: true
                }
            },
            {
                breakpoint: 990,
                settings: {
                    slidesToShow: 2,
                    draggable: true
                }
            },
            {
                breakpoint: 767,
                settings: {
                    slidesToShow: 1,
                    draggable: true
                }

            }
        ]
    });

    $('.carousel-slider.general-slider').each(function() {
        $(this).slick({
            arrows: false,
            dots: false,
            slidesToShow: 1,
            slidesToScroll: 1,
            autoplay: true,
            autoplaySpeed: 5000,
            draggable: true,
            responsive: [{
                breakpoint: 767,
                settings: {
                    slidesToShow: 1,
                    draggable: true
                }
            }]
        });
    });


    $('.fancybox').fancybox({
        loop: false
    });

    $('.themeioan_counter > h4').counterUp({
        delay: 10,
        time: 3000
    });


});

// Burger menü kodu - tamamen yeniden yazıldı
$(document).ready(function() {
    var isAnimating = false;
    
    $(document).off('click.burger').on('click.burger', '.burger-icon', function(e) {
        e.preventDefault();
        e.stopPropagation();
        e.stopImmediatePropagation();
        
        if (isAnimating) return; // Animasyon devam ediyorsa işlemi engelle
        
        var $burgerIcon = $(this);
        var $navbarCollapse = $("#navbarCollapse");
        var isVisible = $navbarCollapse.is(':visible');
        
        isAnimating = true;
        
        if (isVisible) {
            // Menüyü kapat
            $burgerIcon.removeClass('change');
            $navbarCollapse.slideUp(300, function() {
                $navbarCollapse.removeClass('show');
                isAnimating = false;
            });
        } else {
            // Menüyü aç
            $burgerIcon.addClass('change');
            $navbarCollapse.slideDown(300, function() {
                $navbarCollapse.addClass('show');
                isAnimating = false;
            });
        }
    });
});

// Sadece alt menü başlıklarını engelle, alt menü içindeki linkleri engelleme
$(document).on('click', '.subnav > a[href="javascript:void(0)"]', function(e) {
    e.preventDefault();
});

// Alt menü genişletme/daraltma - bubbling ve animasyon çakışmasını engelle
$(document).off('click.subnav').on('click.subnav', '.nav__expand', function(e) {
    e.preventDefault();
    e.stopImmediatePropagation();
    $(this).next("ul").stop(true, true).slideToggle("slow");
    $(this).find('i').toggleClass('fa-chevron-down fa-chevron-up');
});

document.addEventListener('DOMContentLoaded', function() {


    $('#contact-us-form').submit(function() {
        var form = $(this),
            hasError = false;

        form.find('.error-msg, .success-msg').remove();

        form.find('.required-field').each(function() {
            $(this).removeClass('not-valid');
            if ($.trim($(this).val()) === '') {
                $(this).addClass('not-valid').parent().append('<div class="error-msg">This is a required field.</div>');
                hasError = true;
            } else if ($(this).hasClass('email-field')) {
                var emailReg = /^([\w-\.]+@([\w-]+\.)+[\w-]{2,4})?$/;
                if (!emailReg.test($.trim($(this).val()))) {
                    $(this).addClass('not-valid').parent().append('<div class="error-msg">You entered an invalid Email.</div>');
                    hasError = true;
                }
            }
        });
        if (!hasError) {
            var formData = $(this).serialize();
            $.post('contact-process.php', formData, function(data) {
                form.find('.required-field').val('');
                form.append('<div class="success-msg">Thank you! We will contact you shortly.</div>');
            }).fail(function() {
                form.append('<div class="error-msg">Error occurred. Please try again later.</div>');
            });
        }
        return false;
    });

    $('.owl-navigation ').owlCarousel({
        loop: true,
        margin: 0,
        nav: true,
        touchDrag: true,
        mouseDrag: true,
        autoplay: true,
        autoplayTimeout: 5000,
        smartSpeed: 1000,
        autoplayHoverPause: true,
        responsive: {
            0: {
                items: 1
            }
        }
    });

    $('.owl-carousel').owlCarousel({
        loop: true,
        margin: 0,
        nav: false,
        touchDrag: true,
        mouseDrag: true,
        autoplay: true,
        autoplayTimeout: 5000,
        smartSpeed: 1000,
        autoplayHoverPause: true,
        responsive: {
            0: {
                items: 1
            }
        }
    });
});

document.addEventListener('DOMContentLoaded', function() {

    'use strict';

    if ($("#typed")[0]) {
        var typed = new Typed('#typed', {
            stringsElement: '#typed-strings',
            typeSpeed: 60,
            backSpeed: 60,
            startDelay: 1000,
            loop: true,
            loopCount: Infinity,
            onComplete: function(self) {
                prettyLog('onComplete ' + self);
            },
            preStringTyped: function(pos, self) {
                prettyLog('preStringTyped ' + pos + ' ' + self);
            },
            onStringTyped: function(pos, self) {
                prettyLog('onStringTyped ' + pos + ' ' + self);
            },
            onLastStringBackspaced: function(self) {
                prettyLog('onLastStringBackspaced ' + self);
            },
            onTypingPaused: function(pos, self) {
                prettyLog('onTypingPaused ' + pos + ' ' + self);
            },
            onTypingResumed: function(pos, self) {
                prettyLog('onTypingResumed ' + pos + ' ' + self);
            },
            onReset: function(self) {
                prettyLog('onReset ' + self);
            },
            onStop: function(pos, self) {
                prettyLog('onStop ' + pos + ' ' + self);
            },
            onStart: function(pos, self) {
                prettyLog('onStart ' + pos + ' ' + self);
            },
            onDestroy: function(self) {
                prettyLog('onDestroy ' + self);
            }
        });
    } else {}
});

function prettyLog(str) {}