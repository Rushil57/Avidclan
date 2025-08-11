// File: /assets/js/app.js
// Is file mein aapki website ka saara custom JavaScript logic hai.

document.addEventListener("DOMContentLoaded", function () {

    /**
     * Function 1: Owl Carousel
     * Yeh sirf un pages par chalega jahan '.owl-carousel' element maujood hai.
     */
    const owlCarouselElement = document.querySelector(".owl-carousel");
    if (owlCarouselElement && typeof $ === 'function') { // jQuery aur element dono check karein
        $(owlCarouselElement).owlCarousel({
            loop: true,
            margin: 0,
            nav: true,
            dots: true,
            // CLS aue accessibility ke liye width/height add kiya gaya hai
            navText: ['<img loading="lazy" src="/assets/images/imagenew/chevron-left.svg" alt="Previous" width="50" height="50">', '<img loading="lazy" src="/assets/images/imagenew/chevron-right.svg" alt="Next" width="50" height="50">'],
            responsive: { 0: { items: 1 }, 768: { items: 3 }, 1200: { items: 6 } },
            onInitialized: setCarouselAriaLabels,
            onTranslated: setCarouselAriaLabels
        });
    }
    function setCarouselAriaLabels() {
        document.querySelectorAll(".owl-dot").forEach((dot, index) => {
            dot.setAttribute("aria-label", `Go to slide ${index + 1}`);
        });
    }

    /**
     * Function 2: Swiper Slider
     * Yeh testimonial slider ke liye hai.
     */
    if (typeof Swiper !== 'undefined' && document.querySelector(".cinkes_testimonial_spage_active")) {
        new Swiper(".cinkes_testimonial_spage_active", {
            loop: true,
            navigation: { nextEl: ".custom-swiper-next", prevEl: ".custom-swiper-prev" },
            slidesPerView: 1,
            spaceBetween: 30,
            breakpoints: { 768: { slidesPerView: 2 }, 1200: { slidesPerView: 2 } }
        });
    }

    /**
     * Function 3: Portfolio/Case Study Filter Logic
     * Yeh code ab ek hi jagah hai aur har page par kaam karega.
     */
    const filterTabs = document.querySelectorAll(".filter-tabs .filter");
    const masonryItems = document.querySelectorAll(".masonry-item");
    if (filterTabs.length > 0 && masonryItems.length > 0) {
        filterTabs.forEach(tab => {
            tab.addEventListener("click", function (e) {
                e.preventDefault(); // Good practice
                filterTabs.forEach(t => t.classList.remove("active"));
                this.classList.add("active");
                const filter = this.getAttribute("data-filter").substring(1);

                masonryItems.forEach(item => {
                    item.style.display = (filter === 'all' || item.classList.contains(filter)) ? "block" : "none";
                });
            });
        });
        // Shuruat mein active filter ko trigger karein (agar koi hai)
        document.querySelector(".filter-tabs .filter.active")?.click();
    }

    /**
     * Function 4: Mobile Menu Icon Toggle (Hamburger to Close)
     */
    const menuIcon = document.getElementById("menuIcon");
    const menuToggle = document.getElementById("menuToggle");
    if (menuToggle && menuIcon) {
        menuToggle.addEventListener("click", () => {
            const isMenuOpen = menuIcon.src.includes("menu.png");
            menuIcon.src = isMenuOpen ? "/assets/images/imagenew/close.png" : "/assets/images/imagenew/menu.png";
        });
    }

    /**
     * Function 5: Bootstrap Tooltips Initializer
     */
    if (typeof bootstrap !== 'undefined' && typeof bootstrap.Tooltip === 'function') {
        const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
        tooltipTriggerList.map(tooltipTriggerEl => new bootstrap.Tooltip(tooltipTriggerEl));
    }

});