!(function (e) {
    "use strict";
    let t;
    function i() {
        if (e(".main-header").length) {
            var t = e(window).scrollTop(),
                i = e(".main-header"),
                n = e(".scroll-to-top"),
                a = e(".main-header .sticky-header");
            t > 100 ? (i.addClass("fixed-header"), a.addClass("animated slideInDown"), n.fadeIn(300)) : (i.removeClass("fixed-header"), a.removeClass("animated slideInDown"), n.fadeOut(300));
        }
    }
    e(".curved-circle").length && e(".curved-circle").circleType({ position: "absolute", dir: 1, radius: 83, forceHeight: !0, forceWidth: !0 }),
        (n = e(".main-menu").find(".navigation")),
        (t = window.location.href.slice(24)),
        n.find("li").each(function () {
            let i = e(this).find("a");
            e(i).attr("href") == t && e(this).addClass("current");
        }),
        //n.children("li").each(function () {
        //    e(this).find(".current").length && e(this).addClass("current");
        //}),
        "services/" == t && n.find("li").eq(0).addClass("current"),
        i();
    var n,
        a = e(".nav-menu");
    function o() {
        var t = e(".sub-menu-toggle");
        e.each(t, function () {
            e(this).outerHeight(e(this).siblings("a").outerHeight()), e(this).css("line-height", e(this).siblings("a").outerHeight() + "px");
        });
    }
    function s() {
        if (e(".sortable-masonry").length) {
            var t = e(window),
                i = e(".sortable-masonry .items-container"),
                n = e(".filter-btns");
            i.isotope({ filter: ".fe-tech, .por-1, .p-viewall", animationOptions: { duration: 500, easing: "linear" } }),
                n.find("li").on("click", function () {
                    var t = e(this).attr("data-filter");
                    try {
                        i.isotope({ filter: t, animationOptions: { duration: 500, easing: "linear", queue: !1 } });
                    } catch (e) { }
                    return !1;
                }),
                t.on("resize", function () {
                    var e = n.find("li.active").attr("data-filter");
                    i.isotope({ filter: e, animationOptions: { duration: 500, easing: "linear", queue: !1 } }), i.isotope();
                });
            var a = e(".filter-btns li");
            a.on("click", function () {
                var t = e(this);
                t.hasClass("active") || (a.removeClass("active"), t.addClass("active"));
            }),
                i.isotope("on", "layoutComplete", function (t, i) {
                    t = i.length;
                    e(".filters .count").html(t);
                });
        }
    }
    function l() {
        e(".isotope-block").length && e(".isotope-block").isotope();
    }
    function c(t) {
        e(t.target).prev(".panel-heading").find(".more-less").toggleClass("fa-plus fa-minus"), e(t.target).prev(".panel-heading").toggleClass("active");
    }
    e(".mobile-nav-toggler").on("click", function () {
        a.toggleClass("active"),
            (function () {
                var t = 0;
                if (a.hasClass("active")) {
                    var i = a.children("li");
                    e.each(i, function () {
                        t += e(this).outerHeight();
                    });
                }
                a.css("height", t);
            })();
    }),
        e(".sub-menu-toggle").on("click", function () {
            var t, i;
            e(this).siblings(".sub-menu").toggleClass("active"),
                o(),
                (t = a.outerHeight()),
                (i = e(".sub-menu")),
                e.each(i, function () {
                    e(this).outerHeight(t);
                });
        }),
        e(".close").on("click", function () {
            e(this).parent(".sub-menu").toggleClass("active");
        }),
        o(),
        e(".case-tabs").length &&
        e(".case-tabs .case-tab-btns .case-tab-btn").on("click", function (t) {
            t.preventDefault();
            var i = e(e(this).attr("data-tab"));
            if (e(i).hasClass("actve-tab")) return !1;
            e(".case-tabs .case-tab-btns .case-tab-btn").removeClass("active-btn"), e(this).addClass("active-btn"), e(".case-tabs .case-tabs-content .case-tab").removeClass("active-tab"), e(i).addClass("active-tab");
        }),
        e(".lazy-image").length && new LazyLoad({ elements_selector: ".lazy-image", load_delay: 0, threshold: 300 }),
        e(".theme_carousel").length &&
        e(".theme_carousel").each(function (t) {
            var i = {},
                n = e(this).data("options");
            e.extend(i, n), e(this).owlCarousel(i);
        }),
        e(".lightbox-image").length && e(".lightbox-image").fancybox({ openEffect: "fade", closeEffect: "fade", helpers: { media: {} } }),
        s(),
        e(".scroll-to-target").length &&
        e(".scroll-to-target").on("click", function () {
            var t = e(this).attr("data-target");
            e("html, body").animate({ scrollTop: e(t).offset().top }, 1500);
        }),
        l(),
        e(".wow").length && new WOW({ boxClass: "wow", animateClass: "animated", offset: 0, mobile: !0, live: !0 }).init(),
        e(window).on("scroll", function () {
            i();
        }),
        e(window).on("load", function () {
            s(), l();
        }),
        e(".panel-group").on("hidden.bs.collapse", c),
        e(".panel-group").on("shown.bs.collapse", c);
})(window.jQuery),
    $(window).on("load", function () {
        (function () {
            var e = document.querySelector(".cookies-infobar"),
                t = document.querySelector("#cookies-infobar-close");
            function i() {
                e.className = e.classList.value + " cookies-infobar_accepted";
            }
            "1" !==
                (function () {
                    for (var e = "cookieInfoHidden=", t = document.cookie.split(";"), i = 0; i < t.length; i++) {
                        for (var n = t[i]; " " == n.charAt(0);) n = n.substring(1);
                        if (0 === n.indexOf(e)) return n.substring(e.length, n.length);
                    }
                    return "";
                })()
                ? ((e.className = "cookies-infobar"),
                    t.addEventListener("click", function (e) {
                        var t, n, a;
                        e.preventDefault(), i(), (n = (t = new Date()).getTime() + 6048e5), (a = (a = new Date(t.setTime(n))).toUTCString()), (document.cookie = "cookieInfoHidden=1; expires=" + a + "; path=/");
                    }))
                : i();
        })(),
            $(document).ready(function () {
                $("#btnResetCookiePolicy").on("click", function () {
                    console.log("btn: reset"), Cookies.remove("acceptedCookiesPolicy"), $("#alertCookiePolicy").show();
                });
            }),
            $("#c_openings").click(function () {
                $("body").hasClass("mobile-menu-visible") && window.location.reload();
            }),
            $(function () {
                var e = RegExp(window.location.pathname.replace(/\/$/, "") + "$");
                $(".mobile-menu a").each(function () {
                    e.test(this.href.replace(/\/$/, "")) && $(this).addClass("active");
                });
            }),
            $(function () {
                $('[data-toggle="tooltip"]').tooltip();
            });
    });
//console.clear();
const navExpand = [].slice.call(document.querySelectorAll(".nav-expand")),
    backLink = '<li class="nav-item">\n\t<a class="nav-link nav-back-link1" href="javascript:;">\n  <span class="iconify me-2" data-icon="fa:angle-left"></span> Back\n\t</a>\n</li>';
function testimonialActive() {
    new Swiper(".cinkes_testimonial_spage_active", {
        slidesPerView: 2,
        spaceBetween: 30,
        loop: !0,
        rtl: !1,
        navigation: { nextEl: ".cinkes_testimonial_spage_next", prevEl: ".cinkes_testimonial_spage_prev" },
        breakpoints: { 0: { slidesPerView: 1 }, 576: { slidesPerView: 1 }, 768: { slidesPerView: 1 }, 992: { slidesPerView: 2 } },
    });
}
function sliderTestimonialActive() {
    let e = new Swiper(".cinkes_testimonial_thumbs_active", {
        loop: !0,
        spaceBetween: 5,
        slidesPerView: 3,
        freeMode: !0,
        rtl: !1,
        watchSlidesVisibility: !0,
        watchSlidesProgress: !0,
        breakpoints: { 320: { slidesPerView: 1 }, 576: { slidesPerView: 1 }, 768: { slidesPerView: 3 }, 993: { slidesPerView: 3 } },
    });
    new Swiper(".cinkes_testimonial_message_active", {
        loop: !0,
        spaceBetween: 30,
        rtl: !1,
        slidesPerView: 1,
        effect: "fade",
        fadeEffect: { crossFade: !0 },
        navigation: { nextEl: ".cinkes_testimonial_next", prevEl: ".cinkes_testimonial_prev" },
        pagination: { el: ".cinkes_testimonial_pagination" },
        thumbs: { swiper: e },
    });
}
navExpand.forEach((e) => {
    e.querySelector(".nav-expand-content").insertAdjacentHTML("afterbegin", backLink),
        e.querySelector(".dropdown-btn").addEventListener("click", () => e.classList.add("active")),
        e.querySelector(".nav-back-link1").addEventListener("click", () => e.classList.remove("active"));
}),
    testimonialActive(),
    sliderTestimonialActive();
var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]')),
    tooltipList = tooltipTriggerList.map(function (e) {
        return new bootstrap.Tooltip(e);
    });
