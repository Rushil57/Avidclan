(function ($) {
    "use strict";

    if ($(".curved-circle").length) {
        $(".curved-circle").circleType({ position: "absolute", dir: 1, radius: 83, forceHeight: true, forceWidth: true });
    }

    //Hide Loading Box (Preloader)
    function handlePreloader() {
        if ($(".loader-wrap").length) {
            $(".loader-wrap").delay(1000).fadeOut(500);
        }
        TweenMax.to($(".loader-wrap .overlay"), 1.2, {
            force3D: true,
            left: "100%",
            ease: Expo.easeInOut,
        });
    }

    function dynamicCurrentMenuClass(selector) {
        /*let FileName = window.location.href.split("/").reverse()[0];*/
        /*let FileName = window.location.href.slice(23);*/
        let FileName = window.location.href.slice(24);
        selector.find("li").each(function () {
            let anchor = $(this).find("a");
            
            if ($(anchor).attr("href") == FileName) {
                $(this).addClass("current");
            }
        });
        // if any li has .current elmnt add class
        selector.children("li").each(function () {
            if ($(this).find(".current").length) {
                $(this).addClass("current");
            }
        });
        // if no file name return
        if ("" == FileName) {
            selector.find("li").eq(0).addClass("current");
        }
    }

    // dynamic current class
    let mainNavUL = $(".main-menu").find(".navigation");
    dynamicCurrentMenuClass(mainNavUL);

    //Update Header Style and Scroll to Top
    function headerStyle() {
        if ($(".main-header").length) {
            var windowpos = $(window).scrollTop();
            var siteHeader = $(".main-header");
            var scrollLink = $(".scroll-to-top");
            var sticky_header = $(".main-header .sticky-header");
            if (windowpos > 100) {
                siteHeader.addClass("fixed-header");
                sticky_header.addClass("animated slideInDown");
                scrollLink.fadeIn(300);
            } else {
                siteHeader.removeClass("fixed-header");
                sticky_header.removeClass("animated slideInDown");
                scrollLink.fadeOut(300);
            }
        }
    }

    headerStyle();

    /*Mobile Menu*/
    //GLOBALS
    var nav_menu = $(".nav-menu");

    //main menu toggle
    $(".mobile-nav-toggler").on("click", function () {
        nav_menu.toggleClass("active");
        calculate_main_menu_height();
    });

    //sub menu toggle
    $(".sub-menu-toggle").on("click", function () {
        var sub_menu = $(this).siblings(".sub-menu");
        sub_menu.toggleClass("active");
        set_sub_menu_toggles();
        set_sub_menu_height();
    });

    //calculates the overall height of the nav menu
    function calculate_main_menu_height() {
        var menu_height = 0;
        if (nav_menu.hasClass("active")) {
            var menu_items = nav_menu.children("li");
            $.each(menu_items, function () {
                menu_height += $(this).outerHeight();
            });
        }

        nav_menu.css("height", menu_height);
    }

    //sets all sub-menus to be as long as the main nav menu
    function set_sub_menu_height() {
        var menu_height = nav_menu.outerHeight();
        var sub_menus = $(".sub-menu");
        $.each(sub_menus, function () {
            $(this).outerHeight(menu_height);
        });
    }

    //close sub menu
    $(".close").on("click", function () {
        $(this).parent(".sub-menu").toggleClass("active");
    });

    //SUB MENU TOGGLE HEIGHT
    //set the height of the sub-menu toggle perfectly
    function set_sub_menu_toggles() {
        var sub_menu_toggles = $(".sub-menu-toggle");
        $.each(sub_menu_toggles, function () {
            $(this).outerHeight($(this).siblings("a").outerHeight());
            $(this).css("line-height", $(this).siblings("a").outerHeight() + "px");
            //change the class of the toggle (if menu is active or not)
            if ($(this).siblings(".sub-menu").hasClass("active")) {
                // $(this).removeClass('fa-angle-down').addClass('fa-angle-up');
                $(this).addClass("flip");
            } else {
                // $(this).removeClass('fa-angle-up').addClass('fa-angle-down');
                $(this).removeClass("flip");
            }
        });
    }
    set_sub_menu_toggles();

    //Case Tabs
    if ($(".case-tabs").length) {
        $(".case-tabs .case-tab-btns .case-tab-btn").on("click", function (e) {
            e.preventDefault();
            var target = $($(this).attr("data-tab"));

            if ($(target).hasClass("actve-tab")) {
                return false;
            } else {
                $(".case-tabs .case-tab-btns .case-tab-btn").removeClass("active-btn");
                $(this).addClass("active-btn");
                $(".case-tabs .case-tabs-content .case-tab").removeClass("active-tab");
                $(target).addClass("active-tab");
            }
        });
    }

    // Lazyload Images
    if ($(".lazy-image").length) {
        new LazyLoad({
            elements_selector: ".lazy-image",
            load_delay: 0,
            threshold: 300,
        });
    }

    /////////////////////////////
    //Universal Code for All Owl Carousel Sliders
    /////////////////////////////

    if ($(".theme_carousel").length) {
        $(".theme_carousel").each(function (index) {
            var $owlAttr = {},
                $extraAttr = $(this).data("options");
            $.extend($owlAttr, $extraAttr);
            $(this).owlCarousel($owlAttr);
        });
    }

    //LightBox / Fancybox
    if ($(".lightbox-image").length) {
        $(".lightbox-image").fancybox({
            openEffect: "fade",
            closeEffect: "fade",
            helpers: {
                media: {},
            },
        });
    }

    //Sortable Masonary with Filters
    function sortableMasonry() {
        if ($(".sortable-masonry").length) {
            var winDow = $(window);
            // Needed variables
            var $container = $(".sortable-masonry .items-container");
            var $filter = $(".filter-btns");
            $container.isotope({
                filter: ".fe-tech, .por-1, .p-viewall",
                animationOptions: {
                    duration: 500,
                    easing: "linear",
                },
            });
            // Isotope Filter
            $filter.find("li").on("click", function () {
                var selector = $(this).attr("data-filter");
                try {
                    $container.isotope({
                        filter: selector,
                        animationOptions: {
                            duration: 500,
                            easing: "linear",
                            queue: false,
                        },
                    });
                } catch (err) { }
                return false;
            });
            winDow.on("resize", function () {
                var selector = $filter.find("li.active").attr("data-filter");
                $container.isotope({
                    filter: selector,
                    animationOptions: {
                        duration: 500,
                        easing: "linear",
                        queue: false,
                    },
                });
                $container.isotope();
            });
            var filterItemA = $(".filter-btns li");
            filterItemA.on("click", function () {
                var $this = $(this);
                if (!$this.hasClass("active")) {
                    filterItemA.removeClass("active");
                    $this.addClass("active");
                }
            });
            $container.isotope("on", "layoutComplete", function (a, b) {
                var a = b.length,
                    pcn = $(".filters .count");
                pcn.html(a);
            });
        }
    }
    sortableMasonry();

    // Scroll to a Specific Div
    if ($(".scroll-to-target").length) {
        $(".scroll-to-target").on("click", function () {
            var target = $(this).attr("data-target");
            // animate
            $("html, body").animate(
                {
                    scrollTop: $(target).offset().top,
                },
                1500
            );
        });
    }

    // Isotop Layout
    function isotopeBlock() {
        if ($(".isotope-block").length) {
            var $grid = $(".isotope-block").isotope();
        }
    }

    isotopeBlock();

    // Elements Animation
    if ($(".wow").length) {
        var wow = new WOW({
            boxClass: "wow", // animated element css class (default is wow)
            animateClass: "animated", // animation css class (default is animated)
            offset: 0, // distance to the element when triggering the animation (default is 0)
            mobile: true, // trigger animations on mobile devices (default is true)
            live: true, // act on asynchronously loaded content (default is true)
        });
        wow.init();
    }

    /* ==========================================================================
         When document is Scrollig, do
         ========================================================================== */

    $(window).on("scroll", function () {
        headerStyle();
    });

    /* ==========================================================================
         When document is loading, do
         ========================================================================== */

    $(window).on("load", function () {
        handlePreloader();
        sortableMasonry();
        isotopeBlock();
    });

    function toggleIcon(e) {
        $(e.target).prev(".panel-heading").find(".more-less").toggleClass("fa-plus fa-minus");
        $(e.target).prev(".panel-heading").toggleClass("active");
    }
    $(".panel-group").on("hidden.bs.collapse", toggleIcon);
    $(".panel-group").on("shown.bs.collapse", toggleIcon);
    function isNumberKey(evt) {
        var charCode = evt.which ? evt.which : evt.keyCode;
        if (charCode != 46 && charCode > 31 && (charCode < 48 || charCode > 57)) return false;
        return true;
    }
})(window.jQuery);

$(window).on("load", function () {
    var fn = {
        /*
                Launch
            */

        Launch: function () {
            fn.IntroCanvas();
        },

        /*
                Intro Canvas
            */

        IntroCanvas: function () {
            const canvas = document.getElementById("intro__canvas");
            if (canvas) {
                const ctx = canvas.getContext("2d");
                let width = (canvas.width = canvas.offsetWidth);
                let height = (canvas.height = canvas.offsetHeight);

                const colours = JSON.parse(canvas.getAttribute("data-colors"));

                const maxParticles = 200;
                let particles = [];

                // init client x and y values
                let cx = width / 2;
                let cy = height / 2;

                /*window.addEventListener('mousemove', (e) => {
                            var bounds = canvas.getBoundingClientRect();
        	
                            cx = e.clientX - bounds.left;
                            cy = e.clientY - bounds.top;
                        });*/

                class Particle {
                    constructor(x, y, vx, vy, radius, colour) {
                        this.x = x;
                        this.y = y;
                        this.vx = vx;
                        this.vy = vy;
                        this.radius = radius;
                        this.colour = colour;
                    }
                    move() {
                        // Reset particle if it goes off screen
                        if (this.y > height || this.y < 0 || this.x > width || this.x < 0) {
                            this.reset();
                        }
                        // Move particles with respect to velocity vectors
                        this.x += this.vx;
                        this.y += this.vy;
                    }
                    reset() {
                        this.x = cx;
                        this.y = cy;
                        this.vx = 2 + Math.random() * -4;
                        this.vy = 2 + Math.random() * -4;
                        this.radius = 1 + Math.random() * 3;
                    }
                    draw(ctx) {
                        ctx.beginPath();
                        ctx.arc(this.x, this.y, this.radius, 0, 2 * Math.PI, false);
                        ctx.fillStyle = this.colour;
                        ctx.fill();
                    }
                }

                function initParticles() {
                    for (let i = 0; i < maxParticles; i++) {
                        setTimeout(createParticle, 10 * i, i);
                    }
                }

                function createParticle(i) {
                    let p = new Particle(
                        Math.floor(Math.random() * width), // x
                        Math.floor(Math.random() * height), // y
                        1.5 + Math.random() * -3, // vx
                        1.5 + Math.random() * -3, // vy
                        1 + Math.random(), // radius
                        colours[Math.floor(Math.random() * colours.length)]
                    );
                    particles.push(p);
                }

                function loop() {
                    ctx.clearRect(0, 0, width, height);
                    for (let particle of particles) {
                        particle.move();
                        particle.draw(ctx);
                    }
                    requestAnimationFrame(loop);
                }

                // Start animation
                initParticles();
                loop();

                // Resize canvas - responsive
                window.addEventListener("resize", resize);
                function resize() {
                    width = canvas.width = window.innerWidth;
                    height = canvas.height = window.innerHeight;
                }
            }
        },
    };

    // $('.navigation .dropdown').mouseenter(function() {
    // 	var anchorWidth =  ($(this).find("> a").width())/2;
    // 	var menuWidth =  ($(this).find(".dropdown-menu").outerWidth())/2;

    // 	var finalWidth = -menuWidth + anchorWidth;
    // 	$(this).find(".dropdown-menu.half").css({'transform' : 'translate('+ finalWidth +'px)'});
    // });
    // .mouseleave(function() {
    // 	$(this).find(".dropdown-menu.half").css({'transform' : 'none'});
    // });

    // function cookiesPolicyPrompt() {
    //   if (Cookies.get("acceptedCookiesPolicy") !== "yes") {
    //     //console.log('accepted policy', chk);
    //     $("#alertCookiePolicy").show();
    //   }
    //   $(".btnAcceptCookiePolicy").on("click", function () {
    //     //console.log('btn: accept');
    //     Cookies.set("acceptedCookiesPolicy", "yes", { expires: 30 });
    //   });
    //   $("#btnDeclineCookiePolicy").on("click", function () {
    //     //console.log('btn: decline');
    //     Cookies.set("acceptedCookiesPolicy", "No");
    //   });
    // }

    (function () {
        var infoBar = document.querySelector(".cookies-infobar");
        var btnAccept = document.querySelector("#cookies-infobar-close");
        // Check if user has already accepted the notification
        if (wasAccepted()) {
            hideInfobar();
            return;
        }
        //listen for the click event on Accept button
        btnAccept.addEventListener("click", function (e) {
            e.preventDefault();
            hideInfobar();
            saveAcceptInCookies(7);
        });
        //hide cookie info bar
        function hideInfobar() {
            infoBar.className = infoBar.classList.value + " cookies-infobar_accepted";
        }
        // Check if user has already accepted the notification
        function wasAccepted() {
            return checkCookie() === "1";
        }
        // get cookie
        function checkCookie() {
            var name = "cookieInfoHidden=";
            var cookies = document.cookie.split(";");
            for (var i = 0; i < cookies.length; i++) {
                var cookie = cookies[i];
                while (cookie.charAt(0) == " ") {
                    cookie = cookie.substring(1);
                }
                if (cookie.indexOf(name) === 0) {
                    return cookie.substring(name.length, cookie.length);
                }
            }
            return "";
        }
        //save cookie
        function saveAcceptInCookies(daysOfValidity) {
            var now = new Date();
            var time = now.getTime() + daysOfValidity * 24 * 60 * 60 * 1000;
            var newTime = new Date(now.setTime(time));

            newTime = newTime.toUTCString();

            document.cookie = "cookieInfoHidden=1; expires=" + newTime + "; path=/";
        }
    })();

    $(document).ready(function () {
        var anchorWidth_Ser = $(".navigation .ser_menu").find("> a").width() / 2;
        var menuWidth_Ser = $(".navigation .ser_menu").find(".dropdown-menu").outerWidth() / 2;
        var finalWidth_Ser = -menuWidth_Ser + anchorWidth_Ser;
        $(".navigation .ser_menu")
            .find(".dropdown-menu.half")
            .css({ transform: "translate(" + finalWidth_Ser + "px)" });

        var anchorWidth_Tec = $(".navigation .tec_menu").find("> a").width() / 2;
        var menuWidth_Tec = $(".navigation .tec_menu").find(".dropdown-menu").outerWidth() / 2;
        var finalWidth_Tec = -menuWidth_Tec + anchorWidth_Tec;
        $(".navigation .tec_menu")
            .find(".dropdown-menu.half")
            .css({ transform: "translate(" + finalWidth_Tec + "px)" });

        var anchorWidth_Car = $(".navigation .car_menu").find("> a").width() / 2;
        var menuWidth_Car = $(".navigation .car_menu").find(".dropdown-menu").outerWidth() / 2;
        var finalWidth_Car = -menuWidth_Car + anchorWidth_Car;
        $(".navigation .car_menu")
            .find(".dropdown-menu.half")
            .css({ transform: "translate(" + finalWidth_Car + "px)" });

        // Add smooth scrolling to all links
        // $(".w3_megamenu-content .row ul li a").on('click', function (event) {
        // 	// Make sure this.hash has a value before overriding default behavior
        // 	if (this.hash !== "") {
        // 		// Prevent default anchor click behavior
        // 		event.preventDefault();

        // 		// Store hash
        // 		var hash = this.hash;

        // 		// Using jQuery's animate() method to add smooth page scroll
        // 		// The optional number (800) specifies the number of milliseconds it takes to scroll to the specified area
        // 		$('html, body').animate({
        // 			scrollTop: $(hash).offset().top
        // 		}, 800, function () {

        // 			// Add hash (#) to URL when done scrolling (default click behavior)
        // 			window.location.hash = hash;
        // 		});
        // 	} // End if
        // });

        //-- following not for production ------
        $("#btnResetCookiePolicy").on("click", function () {
            console.log("btn: reset");
            Cookies.remove("acceptedCookiesPolicy");
            $("#alertCookiePolicy").show();
        });
        // ---------------------------
    });

    $("#c_openings").click(function () {
        if ($("body").hasClass("mobile-menu-visible")) {
            window.location.reload();
        }
    });

    $(function () {
        var url = window.location.pathname,
            urlRegExp = new RegExp(url.replace(/\/$/, "") + "$"); // create regexp to match current url pathname and remove trailing slash if present as it could collide with the link in navigation in case trailing slash wasn't present there
        // now grab every link from the navigation
        $(".mobile-menu a").each(function () {
            // and test its normalized href against the url pathname regexp
            if (urlRegExp.test(this.href.replace(/\/$/, ""))) {
                $(this).addClass("active");
            }
        });
    });

    $(function () {
        $('[data-toggle="tooltip"]').tooltip();
    });
});

console.clear();

const navExpand = [].slice.call(document.querySelectorAll(".nav-expand"));
const backLink = `<li class="nav-item">
	<a class="nav-link nav-back-link1" href="javascript:;">
  <span class="iconify me-2" data-icon="fa:angle-left"></span> Back
	</a>
</li>`;

navExpand.forEach((item) => {
    item.querySelector(".nav-expand-content").insertAdjacentHTML("afterbegin", backLink);
    item.querySelector(".dropdown-btn").addEventListener("click", () => item.classList.add("active"));
    item.querySelector(".nav-back-link1").addEventListener("click", () => item.classList.remove("active"));
});

// cinkes_testimonial_spage_active

function testimonialActive() {
    const cinkes_testimonial_spage_active = new Swiper(".cinkes_testimonial_spage_active", {
        slidesPerView: 2,
        spaceBetween: 30,
        loop: true,
        rtl: false,
        navigation: {
            nextEl: ".cinkes_testimonial_spage_next",
            prevEl: ".cinkes_testimonial_spage_prev",
        },
        breakpoints: {
            0: {
                slidesPerView: 1,
            },
            576: {
                slidesPerView: 1,
            },
            768: {
                slidesPerView: 1,
            },
            992: {
                slidesPerView: 2,
            },
        },
    });
}

function sliderTestimonialActive() {
    // Testimonial slider active

    const slider_thumb = new Swiper(".cinkes_testimonial_thumbs_active", {
        loop: true,
        spaceBetween: 5,
        slidesPerView: 3,
        freeMode: true,
        rtl: false,
        watchSlidesVisibility: true,
        watchSlidesProgress: true,
        breakpoints: {
            320: {
                slidesPerView: 1,
            },
            576: {
                slidesPerView: 1,
            },
            768: {
                slidesPerView: 3,
            },
            993: {
                slidesPerView: 3,
            },
        },
    });
    const slider3 = new Swiper(".cinkes_testimonial_message_active", {
        loop: true,
        spaceBetween: 30,
        rtl: false,
        slidesPerView: 1,
        effect: "fade",
        fadeEffect: {
            crossFade: true,
        },
        navigation: {
            nextEl: ".cinkes_testimonial_next",
            prevEl: ".cinkes_testimonial_prev",
        },
        pagination: {
            el: ".cinkes_testimonial_pagination",
        },
        thumbs: {
            swiper: slider_thumb,
        },
    });
}

testimonialActive();
sliderTestimonialActive();

var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
    return new bootstrap.Tooltip(tooltipTriggerEl);
});



