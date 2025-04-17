document.addEventListener("DOMContentLoaded", function () {
    const slider = document.querySelector(".testimonial-slider");
    const prevBtn = document.querySelector(".prev");
    const nextBtn = document.querySelector(".next");
    const items = document.querySelectorAll(".testimonial-item");


    let itemsArray = Array.from(items);
    for (let i = 0; i < 20; i++) {
        itemsArray.forEach(item => {
            const clonedItem = item.cloneNode(true);
            slider.appendChild(clonedItem);
        });
    }


    slider.scrollLeft = slider.clientWidth;


    const itemWidth = items[0].offsetWidth + 20;


    nextBtn.addEventListener("click", () => {
        slider.scrollBy({ left: itemWidth, behavior: "smooth" });


        if (slider.scrollLeft >= slider.scrollWidth - slider.clientWidth) {
            slider.scrollLeft = slider.clientWidth;
        }
    });


    prevBtn.addEventListener("click", () => {
        slider.scrollBy({ left: -itemWidth, behavior: "smooth" });


        if (slider.scrollLeft <= 0) {
            slider.scrollLeft = slider.scrollWidth - slider.clientWidth * 20;
        }
    });


    slider.addEventListener("click", (event) => {
        const clickPosition = event.clientX;
        const sliderWidth = slider.offsetWidth;
        const centerPosition = slider.offsetLeft + sliderWidth / 2;


        if (clickPosition < centerPosition) {

            slider.scrollBy({ left: -itemWidth, behavior: "smooth" });
        } else {

            slider.scrollBy({ left: itemWidth, behavior: "smooth" });
        }
    });
});

window.addEventListener("scroll", function () {
    let backToTop = document.getElementById("backToTop");
    if (window.scrollY > 300) {
        backToTop.style.display = "block";
    } else {
        backToTop.style.display = "none";
    }
});


document.getElementById("backToTop").addEventListener("click", function () {
    window.scrollTo({ top: 0, behavior: "smooth" });
});


document.addEventListener("DOMContentLoaded", function () {
    let header = document.querySelector(".custom-new-shadow");

    window.addEventListener("scroll", function () {
        if (window.scrollY > 50) {
            header.classList.add("scrolled");
        } else {
            header.classList.remove("scrolled");
        }
    });
});
document.getElementById("menuToggle").addEventListener("click", function () {
    let menuIcon = document.getElementById("menuIcon");


    if (menuIcon.src.includes("menu.png")) {
        menuIcon.src = "/assets/images/imagenew/close.png";
    } else {
        menuIcon.src = "/assets/images/imagenew/menu.png";
    }
});

document.addEventListener('DOMContentLoaded', function () {
    const navbarCollapse = document.getElementById('navbarSupportedContent');
    const body = document.body;

    if (navbarCollapse) {
        navbarCollapse.addEventListener('show.bs.collapse', function () {
            body.classList.add('mobile-menu-open');
        });

        navbarCollapse.addEventListener('hide.bs.collapse', function () {
            body.classList.remove('mobile-menu-open');
        });
    }
});
//document.addEventListener("DOMContentLoaded", function () {
//    // Testimonial Slider Logic
//    const slider = document.querySelector(".testimonial-slider");
//    const prevBtn = document.querySelector(".prev");
//    const nextBtn = document.querySelector(".next");
//    const items = document.querySelectorAll(".testimonial-item");

//    if (slider && items.length > 0) {
//        let itemsArray = Array.from(items);
//        for (let i = 0; i < 2; i++) { // Reduced from 29 to 2
//            itemsArray.forEach(item => {
//                const clonedItem = item.cloneNode(true);
//                slider.appendChild(clonedItem);
//            });
//        }

//        slider.scrollLeft = slider.clientWidth;
//        const itemWidth = items[0].offsetWidth + 20;

//        nextBtn?.addEventListener("click", () => {
//            slider.scrollBy({ left: itemWidth, behavior: "smooth" });
//            if (slider.scrollLeft >= slider.scrollWidth - slider.clientWidth) {
//                slider.scrollLeft = slider.clientWidth;
//            }
//        });

//        prevBtn?.addEventListener("click", () => {
//            slider.scrollBy({ left: -itemWidth, behavior: "smooth" });
//            if (slider.scrollLeft <= 0) {
//                slider.scrollLeft = slider.scrollWidth - slider.clientWidth * 2;
//            }
//        });

//        slider.addEventListener("click", (event) => {
//            const clickPosition = event.clientX;
//            const sliderWidth = slider.offsetWidth;
//            const centerPosition = slider.offsetLeft + sliderWidth / 2;

//            if (clickPosition < centerPosition) {
//                slider.scrollBy({ left: -itemWidth, behavior: "smooth" });
//            } else {
//                slider.scrollBy({ left: itemWidth, behavior: "smooth" });
//            }
//        });
//    }

//    // Header Scroll Shadow
//    const header = document.querySelector(".custom-new-shadow");
//    if (header) {
//        window.addEventListener("scroll", function () {
//            if (window.scrollY > 50) {
//                header.classList.add("scrolled");
//            } else {
//                header.classList.remove("scrolled");
//            }
//        });
//    }

//    // Navbar Collapse Add/Remove Body Class
//    const navbarCollapse = document.getElementById('navbarSupportedContent');
//    const body = document.body;
//    if (navbarCollapse) {
//        navbarCollapse.addEventListener('show.bs.collapse', function () {
//            body.classList.add('mobile-menu-open');
//        });

//        navbarCollapse.addEventListener('hide.bs.collapse', function () {
//            body.classList.remove('mobile-menu-open');
//        });
//    }
//});

// Back to Top Button (Outside DOMContentLoaded to allow early binding)
window.addEventListener("scroll", function () {
    let backToTop = document.getElementById("backToTop");
    if (backToTop) {
        backToTop.style.display = window.scrollY > 300 ? "block" : "none";
    }
});

document.getElementById("backToTop")?.addEventListener("click", function () {
    window.scrollTo({ top: 0, behavior: "smooth" });
});

// Menu Toggle Icon Switch
document.getElementById("menuToggle")?.addEventListener("click", function () {
    let menuIcon = document.getElementById("menuIcon");
    if (menuIcon) {
        menuIcon.src = menuIcon.src.includes("menu.png")
            ? "/assets/images/imagenew/close.png"
            : "/assets/images/imagenew/menu.png";
    }
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

