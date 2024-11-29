document.addEventListener("DOMContentLoaded", () => {
    const Carousel = (() => {
        const getActiveSlide = () =>
            document.querySelector(".carousel__slide.active");

        const getFirstSlide = () => {
            const slider = document.querySelector(".carousel__slider");
            if (!slider) {
                console.error("Carousel slider not found in the DOM");
                return null;
            }
            return slider.firstElementChild;
        };

        const getLastSlide = () =>
            document.querySelector(".carousel__slider").lastElementChild;

        const getSiblingSlide = (slide, direction) =>
            direction === "prev"
                ? slide.previousElementSibling
                : slide.nextElementSibling;

        const updateActiveSlideClass = (activeSlide) => {
            document
                .querySelectorAll(".carousel__slide.active")
                .forEach((slide) => slide.classList.remove("active"));
            activeSlide.classList.add("active");
        };

        const updateScreen = (activeSlide) => {
            const carouselScreen = document.querySelector(".image-display .screen");
            const img = activeSlide.querySelector("img").cloneNode(true);
            carouselScreen.innerHTML = "";
            carouselScreen.appendChild(img);
        };

        const scrollToActiveSlide = (activeSlide) => {
            const carouselSlider = document.querySelector(".carousel__slider");
            const { offsetLeft, offsetWidth } = activeSlide;
            const { clientWidth } = carouselSlider;

            carouselSlider.scrollTo({
                left: offsetLeft - clientWidth / 2 + offsetWidth / 2,
                behavior: "smooth"
            });
        };

        const updateButtonStates = (activeSlide) => {
            const prevButton = document.querySelector(".carousel__btn.prev");
            const nextButton = document.querySelector(".carousel__btn.next");

            prevButton.disabled = !getSiblingSlide(activeSlide, "prev");
            nextButton.disabled = !getSiblingSlide(activeSlide, "next");
        };

        const updateCarousel = (activeSlide) => {
            updateActiveSlideClass(activeSlide);
            updateScreen(activeSlide);
            scrollToActiveSlide(activeSlide);
            updateButtonStates(activeSlide);
        };

        const handleKeydown = (e) => {
            if (!e.target.closest(".carousel__slider")) return;
            const activeSlide = getActiveSlide();
            const newActiveSlide = getSiblingSlide(activeSlide, e.key === "ArrowLeft" ? "prev" : "next");

            if (newActiveSlide) {
                e.preventDefault();
                updateCarousel(newActiveSlide);
            }
        };

        const handleButtonClick = (e) => {
            const activeSlide = getActiveSlide();
            const newActiveSlide = getSiblingSlide(
                activeSlide,
                e.currentTarget.classList.contains("prev") ? "prev" : "next"
            );

            if (newActiveSlide) {
                updateCarousel(newActiveSlide);
            }
        };

        const handleCarouselClick = (e) => {
            const clickedSlide = e.target.closest(".carousel__slide");
            if (clickedSlide) {
                updateCarousel(clickedSlide);
            }
        };

        const initCarousel = () => {
            const carouselSlider = document.querySelector(".carousel__slider");
            const prevButton = document.querySelector(".carousel__btn.prev");
            const nextButton = document.querySelector(".carousel__btn.next");

            if (!carouselSlider || !prevButton || !nextButton) {
                console.error("One or more carousel elements not found");
                return;
            }

            const firstSlide = getFirstSlide();
            if (firstSlide) {
                updateCarousel(firstSlide);
            }

            document.addEventListener("keydown", handleKeydown);
            prevButton.addEventListener("click", handleButtonClick);
            nextButton.addEventListener("click", handleButtonClick);
            carouselSlider.addEventListener("click", handleCarouselClick);
        };

        // Return the initialization method
        return {
            init: initCarousel
        };
    })();

    // Initialize the carousel
    Carousel.init();
});
