document.addEventListener("DOMContentLoaded", function () {
    const slider = document.querySelector(".testimonial-slider");
    const prevBtn = document.querySelector(".prev");
    const nextBtn = document.querySelector(".next");
    const items = document.querySelectorAll(".testimonial-item");

    if (slider && items.length > 0) {
        let itemsArray = Array.from(items);

        // ✅ Reduced cloning from 20x to 2x
        for (let i = 0; i < 5; i++) {
            itemsArray.forEach(item => {
                const clonedItem = item.cloneNode(true);
                slider.appendChild(clonedItem);
            });
        }

        slider.scrollLeft = slider.clientWidth;
        const itemWidth = items[0].offsetWidth + 20;

        nextBtn?.addEventListener("click", () => {
            slider.scrollBy({ left: itemWidth, behavior: "smooth" });
            if (slider.scrollLeft >= slider.scrollWidth - slider.clientWidth) {
                slider.scrollLeft = slider.clientWidth;
            }
        });

        prevBtn?.addEventListener("click", () => {
            slider.scrollBy({ left: -itemWidth, behavior: "smooth" });
            if (slider.scrollLeft <= 0) {
                slider.scrollLeft = slider.scrollWidth - slider.clientWidth * 2;
            }
        });

        slider.addEventListener("click", (event) => {
            const clickPosition = event.clientX;
            const centerPosition = slider.offsetLeft + slider.offsetWidth / 2;

            slider.scrollBy({
                left: clickPosition < centerPosition ? -itemWidth : itemWidth,
                behavior: "smooth",
            });
        });
    }

    // Header shadow toggle on scroll
    const header = document.querySelector(".custom-new-shadow");
    if (header) {
        window.addEventListener("scroll", () => {
            header.classList.toggle("scrolled", window.scrollY > 50);
        });
    }

    // Mobile menu toggle
    document.getElementById("menuToggle")?.addEventListener("click", () => {
        let menuIcon = document.getElementById("menuIcon");
        if (menuIcon) {
            menuIcon.src = menuIcon.src.includes("menu.png")
                ? "/assets/images/imagenew/close.png"
                : "/assets/images/imagenew/menu.png";
        }
    });

    // Body class toggle for mobile menu
    const navbarCollapse = document.getElementById('navbarSupportedContent');
    if (navbarCollapse) {
        navbarCollapse.addEventListener('show.bs.collapse', () => document.body.classList.add('mobile-menu-open'));
        navbarCollapse.addEventListener('hide.bs.collapse', () => document.body.classList.remove('mobile-menu-open'));
    }

    // Swiper Init in idle time
    if ('requestIdleCallback' in window) {
        requestIdleCallback(() => {
            testimonialActive();
            sliderTestimonialActive();
        });
    } else {
        setTimeout(() => {
            testimonialActive();
            sliderTestimonialActive();
        }, 300);
    }

    // Tooltip Init
    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    tooltipTriggerList.map(el => new bootstrap.Tooltip(el));

    // Nav expand logic
    const navExpand = [].slice.call(document.querySelectorAll(".nav-expand")),
        backLink = `<li class="nav-item"><a class="nav-link nav-back-link1" href="javascript:;"><span class="iconify me-2" data-icon="fa:angle-left"></span> Back</a></li>`;
    navExpand.forEach(e => {
        e.querySelector(".nav-expand-content").insertAdjacentHTML("afterbegin", backLink);
        e.querySelector(".dropdown-btn").addEventListener("click", () => e.classList.add("active"));
        e.querySelector(".nav-back-link1").addEventListener("click", () => e.classList.remove("active"));
    });


});

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


// ✅ Debounced scroll for back to top
let scrollTimeout;
window.addEventListener("scroll", () => {
    clearTimeout(scrollTimeout);
    scrollTimeout = setTimeout(() => {
        const backToTop = document.getElementById("backToTop");
        if (backToTop) {
            backToTop.style.display = window.scrollY > 300 ? "block" : "none";
        }
    }, 100);
});

// Back to top button
document.getElementById("backToTop")?.addEventListener("click", () => {
    window.scrollTo({ top: 0, behavior: "smooth" });
});

// Swiper functions
function testimonialActive() {
    new Swiper(".cinkes_testimonial_spage_active", {
        slidesPerView: 2,
        spaceBetween: 30,
        loop: true,
        rtl: false,
        navigation: {
            nextEl: ".cinkes_testimonial_spage_next",
            prevEl: ".cinkes_testimonial_spage_prev",
        },
        breakpoints: {
            0: { slidesPerView: 1 },
            576: { slidesPerView: 1 },
            768: { slidesPerView: 1 },
            992: { slidesPerView: 2 },
        },
    });
}

function sliderTestimonialActive() {
    let thumbs = new Swiper(".cinkes_testimonial_thumbs_active", {
        loop: true,
        spaceBetween: 5,
        slidesPerView: 3,
        freeMode: true,
        rtl: false,
        watchSlidesVisibility: true,
        watchSlidesProgress: true,
        breakpoints: {
            320: { slidesPerView: 1 },
            576: { slidesPerView: 1 },
            768: { slidesPerView: 3 },
            993: { slidesPerView: 3 },
        },
    });

    new Swiper(".cinkes_testimonial_message_active", {
        loop: true,
        spaceBetween: 30,
        rtl: false,
        slidesPerView: 1,
        effect: "fade",
        fadeEffect: { crossFade: true },    
        navigation: {
            nextEl: ".cinkes_testimonial_next",
            prevEl: ".cinkes_testimonial_prev",
        },
        pagination: { el: ".cinkes_testimonial_pagination" },
        thumbs: { swiper: thumbs },
    });
}
