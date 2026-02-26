// Hamburger menu toggle
const hamburger = document.getElementById('hamburger');
const mobileMenu = document.getElementById('mobileMenu');

if (hamburger && mobileMenu) {
    hamburger.addEventListener('click', function (e) {
        e.stopPropagation();
        hamburger.classList.toggle('active');
        mobileMenu.classList.toggle('active');
        document.body.style.overflow = mobileMenu.classList.contains('active') ? 'hidden' : '';
    });

    // Close menu when clicking on a link
    document.querySelectorAll('.mobile-nav-link').forEach(link => {
        link.addEventListener('click', function () {
            hamburger.classList.remove('active');
            mobileMenu.classList.remove('active');
            document.body.style.overflow = '';
        });
    });

    // Close menu when clicking outside
    document.addEventListener('click', function (e) {
        if (mobileMenu.classList.contains('active') &&
            !mobileMenu.contains(e.target) &&
            !hamburger.contains(e.target)) {
            hamburger.classList.remove('active');
            mobileMenu.classList.remove('active');
            document.body.style.overflow = '';
        }
    });
}

// Smooth scrolling for navigation links
document.querySelectorAll('a[href^="#"]').forEach(anchor => {
    anchor.addEventListener('click', function (e) {
        e.preventDefault();
        const target = document.querySelector(this.getAttribute('href'));
        if (target) {
            target.scrollIntoView({
                behavior: 'smooth',
                block: 'start'
            });
        }
    });
});

// Form submission handler
const contactForm = document.querySelector('.contact-form');
if (contactForm) {
    contactForm.addEventListener('submit', function (e) {
        e.preventDefault();

        // Get form data
        const formData = new FormData(this);

        // Here you would typically send the data to a server
        console.log('Form submitted with data:', Object.fromEntries(formData));

        // Show success message
        alert('Thank you for your inquiry! We will get back to you within 18 hours.');

        // Reset form
        this.reset();
    });
}

// Add animation on scroll
const observerOptions = {
    threshold: 0.1,
    rootMargin: '0px 0px -50px 0px'
};

const observer = new IntersectionObserver(function (entries) {
    entries.forEach(entry => {
        if (entry.isIntersecting) {
            entry.target.style.opacity = '1';
            entry.target.style.transform = 'translateY(0)';
        }
    });
}, observerOptions);

window.addEventListener("scroll", function () {
    const header = document.querySelector(".header");
    if (window.scrollY > 50) {
        header.classList.add("scrolled");
    } else {
        header.classList.remove("scrolled");
    }
});
document.querySelectorAll('.testimonial-card').forEach(card => {
    const thumbnail = card.querySelector('.testimonial-image-wrapper');
    const video = card.querySelector('.testimonial-video');
    const playButton = card.querySelector('.play-button');

    playButton.addEventListener('click', (e) => {
        e.stopPropagation();

        // Pause all other videos
        document.querySelectorAll('.testimonial-card.playing .testimonial-video').forEach(otherVideo => {
            if (otherVideo !== video) {
                otherVideo.pause();
                const otherCard = otherVideo.closest('.testimonial-card');
                otherCard.classList.remove('playing');
                otherVideo.style.opacity = '0';
                otherCard.querySelector('.testimonial-image-wrapper').style.opacity = '1';
                otherVideo.removeAttribute('controls');
                otherVideo.style.pointerEvents = 'none';
            }
        });

        // Play current video
        video.play();
        thumbnail.style.opacity = '0';
        video.style.opacity = '1';
        card.classList.add('playing');
        video.setAttribute('controls', ''); // native controls
        video.style.pointerEvents = 'auto';
    });

    // When video ends — reset to thumbnail
    video.addEventListener('ended', () => {
        thumbnail.style.opacity = '1';
        video.style.opacity = '0';
        card.classList.remove('playing');
        video.currentTime = 0;
        video.removeAttribute('controls');
        video.style.pointerEvents = 'none';
    });
});

// ... (Your existing video control JavaScript code goes here) ...

// NEW JAVASCRIPT: Intersection Observer for Centered Snap Effect
document.addEventListener('DOMContentLoaded', () => {
    const testimonialsGrid = document.getElementById('testimonialsGrid');
    const cards = document.querySelectorAll('.testimonial-card');

    if (!testimonialsGrid || cards.length === 0) return;

    // Intersection Observer Options:
    // root: the scrollable container (.testimonials-grid)
    // threshold: 0.8 means the card is considered "intersecting" (active) when 80% of it is visible.
    const observerOptions = {
        root: testimonialsGrid,
        rootMargin: '0px',
        threshold: 0.8
    };

    const observer = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                // When a card snaps into view (becomes 80% visible or more):

                // 1. Remove active-snap from all cards
                cards.forEach(c => c.classList.remove('active-snap'));

                // 2. Add active-snap to the currently intersecting (centered) card
                entry.target.classList.add('active-snap');
            }
        });
    }, observerOptions);

    // Observe each card
    cards.forEach(card => {
        observer.observe(card);
    });

    // To ensure the first card is active on load (important for initial state):
    if (cards.length > 0) {
        cards[0].classList.add('active-snap');
    }
});
document.addEventListener('DOMContentLoaded', () => {
    const grid = document.getElementById('testimonialsGrid');
    const leftImg = document.getElementById('slideLeftBtn');
    const rightImg = document.getElementById('slideRightBtn');

    if (grid && leftImg && rightImg) {

        function updateArrows() {
            const maxScroll = grid.scrollWidth - grid.clientWidth;

            // Start
            if (grid.scrollLeft <= 0) {
                leftImg.classList.add("disabled-arrow");
            } else {
                leftImg.classList.remove("disabled-arrow");
            }

            // End
            if (grid.scrollLeft >= maxScroll - 5) {
                rightImg.classList.add("disabled-arrow");
            } else {
                rightImg.classList.remove("disabled-arrow");
            }
        }

        updateArrows();
        grid.addEventListener("scroll", updateArrows);

        rightImg.addEventListener('click', () => {
            const scrollAmount = grid.clientWidth / 3;
            grid.scrollBy({
                left: scrollAmount,
                behavior: 'smooth'
            });
        });

        leftImg.addEventListener('click', () => {
            const scrollAmount = grid.clientWidth / 3;
            grid.scrollBy({
                left: -scrollAmount,
                behavior: 'smooth'
            });
        });
    }
});

// // REMOVE THE ENTIRE initMobileSlider FUNCTION AND ITS CALLS
// // function initMobileSlider() { ... }
// // window.addEventListener('load', initMobileSlider);
// // window.addEventListener('resize', initMobileSlider);
document.addEventListener('DOMContentLoaded', function () {
    const techSliderContainer = document.querySelector('.tech-slider-container');
    const techSliderTrack = document.querySelector('.tech-slider-track');
    const techSlides = document.querySelectorAll('.tech-slider-slide');
    const techIndicators = document.querySelectorAll('.tech-slider-indicator');
    const techPrev = document.getElementById('tech-prev');
    const techNext = document.getElementById('tech-next');

    const totalTechSlides = techSlides.length;
    let currentTechSlide = 0;

    if (techSliderContainer) {
        updateTechSlideWidth();
        window.addEventListener('resize', updateTechSlideWidth);
    }

    function updateTechSlideWidth() {
        if (!techSliderContainer) return;
        const containerWidth = techSliderContainer.offsetWidth;
        techSlides.forEach(slide => (slide.style.width = `${containerWidth}px`));
        updateTechSliderPosition();
        updateTechIndicators();
        updateTechArrowVisibility();
    }

    function goToTechSlide(index) {
        if (index < 0 || index >= totalTechSlides) return;

        currentTechSlide = index;
        updateTechSliderPosition();
        updateTechIndicators();
        updateTechArrowVisibility();
    }

    function updateTechSliderPosition() {
        techSliderTrack.style.transform = `translateX(-${currentTechSlide * 100}%)`;
    }

    function updateTechIndicators() {
        techIndicators.forEach((indicator, i) =>
            indicator.classList.toggle('active', i === currentTechSlide)
        );
    }

    function updateTechArrowVisibility() {
        if (techPrev) {
            techPrev.style.opacity = currentTechSlide === 0 ? '0.5' : '1';
            techPrev.style.pointerEvents = currentTechSlide === 0 ? 'none' : 'auto';
        }
        if (techNext) {
            techNext.style.opacity = currentTechSlide === totalTechSlides - 1 ? '0.5' : '1';
            techNext.style.pointerEvents = currentTechSlide === totalTechSlides - 1 ? 'none' : 'auto';
        }
    }

    if (techPrev) {
        techPrev.addEventListener('click', () => {
            goToTechSlide(currentTechSlide - 1);
        });
    }

    if (techNext) {
        techNext.addEventListener('click', () => {
            goToTechSlide(currentTechSlide + 1);
        });
    }

    if (techSliderContainer) {
        updateTechArrowVisibility();
        updateTechIndicators();
    }

    let startX = 0;
    let isSwiping = false;
    let currentX = 0;

    if (techSliderTrack) {
        techSliderTrack.addEventListener('pointerdown', e => {
            if (window.innerWidth > 768.99) return;
            isSwiping = true;
            startX = e.clientX;
            currentX = startX;
            techSliderTrack.style.transition = 'none';
        });

        techSliderTrack.addEventListener('pointermove', e => {
            if (!isSwiping) return;
            currentX = e.clientX;
            const diff = startX - currentX;
            techSliderTrack.style.transform = `translateX(calc(-${currentTechSlide * 100}% - ${diff}px))`;
        });

        techSliderTrack.addEventListener('pointerup', () => endSwipe());
        techSliderTrack.addEventListener('pointercancel', () => endSwipe());
    }

    function endSwipe() {
        if (!isSwiping) return;
        const diff = startX - currentX;
        const threshold = 50;
        isSwiping = false;
        techSliderTrack.style.transition = 'transform 0.4s ease';

        if (Math.abs(diff) > threshold) {
            if (diff > 0 && currentTechSlide < totalTechSlides - 1) {
                currentTechSlide++;
            } else if (diff < 0 && currentTechSlide > 0) {
                currentTechSlide--;
            }
        }

        updateTechSliderPosition();
        updateTechIndicators();
        updateTechArrowVisibility();
    }

    techIndicators.forEach(indicator => {
        indicator.addEventListener('click', () => {
            const index = parseInt(indicator.dataset.slide);
            goToTechSlide(index);
        });
    });


    // --- Services Slider Logic (New) ---
    const servicesSliderContainer = document.querySelector('.services-slider .slider-container');
    const servicesSliderTrack = document.querySelector('.services-slider .slider-track');
    const servicesSlides = document.querySelectorAll('.services-slider .slider-slide');
    const servicesIndicators = document.querySelectorAll('.services-slider .slider-indicator');
    const servicePrev = document.getElementById('service-prev');
    const serviceNext = document.getElementById('service-next');

    const totalServicesSlides = servicesSlides.length;
    let currentServiceSlide = 0;

    if (servicesSliderContainer) {
        updateServiceSlideWidth();
        window.addEventListener('resize', updateServiceSlideWidth);
    }

    function updateServiceSlideWidth() {
        if (!servicesSliderContainer) return;
        const containerWidth = servicesSliderContainer.offsetWidth;
        servicesSlides.forEach(slide => (slide.style.width = `${containerWidth}px`));
        updateServiceSliderPosition();
        updateServiceIndicators();
        updateServiceArrowVisibility();
    }

    function goToServiceSlide(index) {
        if (index < 0 || index >= totalServicesSlides) return;

        currentServiceSlide = index;
        updateServiceSliderPosition();
        updateServiceIndicators();
        updateServiceArrowVisibility();
    }

    function updateServiceSliderPosition() {
        servicesSliderTrack.style.transform = `translateX(-${currentServiceSlide * 100}%)`;
    }

    function updateServiceIndicators() {
        servicesIndicators.forEach((indicator, i) =>
            indicator.classList.toggle('active', i === currentServiceSlide)
        );
    }

    function updateServiceArrowVisibility() {
        if (servicePrev) {
            servicePrev.style.opacity = currentServiceSlide === 0 ? '0.5' : '1';
            servicePrev.style.pointerEvents = currentServiceSlide === 0 ? 'none' : 'auto';
        }
        if (serviceNext) {
            serviceNext.style.opacity = currentServiceSlide === totalServicesSlides - 1 ? '0.5' : '1';
            serviceNext.style.pointerEvents = currentServiceSlide === totalServicesSlides - 1 ? 'none' : 'auto';
        }
    }

    if (servicePrev) {
        servicePrev.addEventListener('click', () => {
            goToServiceSlide(currentServiceSlide - 1);
        });
    }

    if (serviceNext) {
        serviceNext.addEventListener('click', () => {
            goToServiceSlide(currentServiceSlide + 1);
        });
    }

    if (servicesSliderContainer) {
        updateServiceArrowVisibility();
        updateServiceIndicators();
    }

    // Services Swipe Handling
    let servicesStartX = 0;
    let isServicesSwiping = false;
    let servicesCurrentX = 0;

    if (servicesSliderTrack) {
        servicesSliderTrack.addEventListener('pointerdown', e => {
            if (window.innerWidth > 768.99) return;
            isServicesSwiping = true;
            servicesStartX = e.clientX;
            servicesCurrentX = servicesStartX;
            servicesSliderTrack.style.transition = 'none';
        });

        servicesSliderTrack.addEventListener('pointermove', e => {
            if (!isServicesSwiping) return;
            servicesCurrentX = e.clientX;
            const diff = servicesStartX - servicesCurrentX;
            servicesSliderTrack.style.transform = `translateX(calc(-${currentServiceSlide * 100}% - ${diff}px))`;
        });

        servicesSliderTrack.addEventListener('pointerup', () => endServicesSwipe());
        servicesSliderTrack.addEventListener('pointercancel', () => endServicesSwipe());
    }

    function endServicesSwipe() {
        if (!isServicesSwiping) return;
        const diff = servicesStartX - servicesCurrentX;
        const threshold = 50;
        isServicesSwiping = false;
        servicesSliderTrack.style.transition = 'transform 0.3s ease';

        if (Math.abs(diff) > threshold) {
            if (diff > 0 && currentServiceSlide < totalServicesSlides - 1) {
                currentServiceSlide++;
            } else if (diff < 0 && currentServiceSlide > 0) {
                currentServiceSlide--;
            }
        }

        updateServiceSliderPosition();
        updateServiceIndicators();
        updateServiceArrowVisibility();
    }

    servicesIndicators.forEach(indicator => {
        indicator.addEventListener('click', () => {
            const index = parseInt(indicator.dataset.slide);
            goToServiceSlide(index);
        });
    });
});
const grid = document.getElementById('reviews-grid');
const next = document.getElementById('review-next');
const prev = document.getElementById('review-prev');
const nextMobile = document.getElementById('review-next-mobile');
const prevMobile = document.getElementById('review-prev-mobile');
const totalCards = grid.querySelectorAll('.review-card').length;
let currentIndex = 0;

// Get card width including gap
function getCardWidth() {
    const card = grid.querySelector('.review-card');
    const gap = 30; // same as CSS gap
    return card.offsetWidth + gap;
}

// Move to specific slide
function goToSlide(index) {
    const maxIndex = totalCards - visibleCards();
    currentIndex = Math.max(0, Math.min(index, maxIndex)); // restrict within bounds
    const cardWidth = getCardWidth();
    grid.style.transition = 'transform 0.5s ease';
    grid.style.transform = `translateX(-${currentIndex * cardWidth}px)`;
    updateArrowState();
}

// Detect how many cards are visible (based on screen size)
function visibleCards() {
    if (window.innerWidth <= 768) return 1;
    else if (window.innerWidth <= 992) return 2;
    else return 3;
}

// Update arrow enable/disable state
function updateArrowState() {
    const maxIndex = totalCards - visibleCards();

    [prev, prevMobile].forEach(btn => {
        if (!btn) return;
        if (currentIndex === 0) {
            btn.classList.add('disabled');
        } else {
            btn.classList.remove('disabled');
        }
    });

    [next, nextMobile].forEach(btn => {
        if (!btn) return;
        if (currentIndex >= maxIndex) {
            btn.classList.add('disabled');
        } else {
            btn.classList.remove('disabled');
        }
    });
}

// Click handlers
function nextSlide() {
    goToSlide(currentIndex + 1);
}

function prevSlide() {
    goToSlide(currentIndex - 1);
}

next.addEventListener('click', nextSlide);
prev.addEventListener('click', prevSlide);
nextMobile.addEventListener('click', nextSlide);
prevMobile.addEventListener('click', prevSlide);

// Touch swipe for mobile
let startX = 0;
let endX = 0;

grid.addEventListener('touchstart', (e) => {
    startX = e.touches[0].clientX;
});

grid.addEventListener('touchmove', (e) => {
    endX = e.touches[0].clientX;
});

grid.addEventListener('touchend', () => {
    const diff = startX - endX;
    const threshold = 50;

    if (Math.abs(diff) > threshold) {
        if (diff > 0) nextSlide(); // Swipe left → next
        else prevSlide();          // Swipe right → prev
    }
});

// On resize, reset transform position
window.addEventListener('resize', () => {
    goToSlide(currentIndex);
});

updateArrowState();

document.addEventListener('DOMContentLoaded', function () {
    const sliderTrack = document.querySelector('.slider-track');
    const slides = document.querySelectorAll('.slider-slide');
    const indicators = document.querySelectorAll('.slider-indicator');
    const totalSlides = slides.length;
    let currentSlide = 0;

    // Set initial slide width
    updateSlideWidth();

    // Update slide width on window resize
    window.addEventListener('resize', updateSlideWidth);

    // Touch events for mobile swiping
    let startX = 0;
    let endX = 0;

    sliderTrack.addEventListener('touchstart', (e) => {
        startX = e.touches[0].clientX;
    });

    sliderTrack.addEventListener('touchmove', (e) => {
        endX = e.touches[0].clientX;
    });

    sliderTrack.addEventListener('touchend', () => {
        handleSwipe();
    });

    // Mouse events for desktop testing
    sliderTrack.addEventListener('mousedown', (e) => {
        startX = e.clientX;
        sliderTrack.addEventListener('mousemove', handleMouseMove);
        sliderTrack.addEventListener('mouseup', handleMouseUp);
        sliderTrack.addEventListener('mouseleave', handleMouseUp);
    });

    function handleMouseMove(e) {
        endX = e.clientX;
    }

    function handleMouseUp() {
        handleSwipe();
        sliderTrack.removeEventListener('mousemove', handleMouseMove);
        sliderTrack.removeEventListener('mouseup', handleMouseUp);
        sliderTrack.removeEventListener('mouseleave', handleMouseUp);
    }

    function handleSwipe() {
        const diff = startX - endX;
        const threshold = 50; // Minimum swipe distance

        if (Math.abs(diff) > threshold) {
            if (diff > 0 && currentSlide < totalSlides - 1) {
                // Swipe left - next slide
                goToSlide(currentSlide + 1);
            } else if (diff < 0 && currentSlide > 0) {
                // Swipe right - previous slide
                goToSlide(currentSlide - 1);
            }
        }
    }

    // Indicator click events
    indicators.forEach(indicator => {
        indicator.addEventListener('click', () => {
            const slideIndex = parseInt(indicator.getAttribute('data-slide'));
            goToSlide(slideIndex);
        });
    });

    function goToSlide(slideIndex) {
        currentSlide = slideIndex;
        updateSliderPosition();
        updateIndicators();
    }

    function updateSliderPosition() {
        sliderTrack.style.transform = `translateX(-${currentSlide * 100}%)`;
    }

    function updateIndicators() {
        indicators.forEach((indicator, index) => {
            if (index === currentSlide) {
                indicator.classList.add('active');
            } else {
                indicator.classList.remove('active');
            }
        });
    }

    function updateSlideWidth() {
        const containerWidth = document.querySelector('.slider-container').offsetWidth;
        slides.forEach(slide => {
            slide.style.width = `${containerWidth}px`;
        });
    }
});
document.addEventListener('DOMContentLoaded', () => {
    const originalLogosContainer = document.querySelector('.trusted-logos.original-logos');
    const originalLogoImages = originalLogosContainer ? originalLogosContainer.querySelectorAll('img') : [];
    const animatedLogosContainer = document.querySelector('.animated-trusted-logos');
    const animatedLogoElements = animatedLogosContainer ? animatedLogosContainer.querySelectorAll('.animated-logo') : [];

    if (!originalLogoImages.length || !animatedLogoElements.length) {
        return; // Exit if elements are not found
    }

    const totalLogos = originalLogoImages.length;
    let currentIndex = 0; // Index of the *first* logo to display in the pair
    const itemsToShow = 2; // Always show 2 logos
    const fadeInterval = 3000; // 3 seconds

    // Collect all logo source paths and alt texts
    const logoData = Array.from(originalLogoImages).map(img => ({
        src: img.src,
        alt: img.alt
    }));

    const mediaQuery = window.matchMedia('(max-width: 576.99px)');
    let intervalId;

    function updateAnimatedLogos() {
        if (!mediaQuery.matches) {
            // If not on mobile, stop the animation and hide animated logos
            if (intervalId) clearInterval(intervalId);
            if (animatedLogosContainer) animatedLogosContainer.style.display = 'none';
            if (originalLogosContainer) originalLogosContainer.style.display = 'flex'; // Show original
            return;
        }

        // Ensure animated container is visible and original is hidden
        if (animatedLogosContainer) animatedLogosContainer.style.display = 'flex';
        if (originalLogosContainer) originalLogosContainer.style.display = 'none';

        // Apply fade out
        animatedLogoElements.forEach(img => img.style.opacity = '0');

        setTimeout(() => {
            // Update src and alt text after fade out
            const logo1Index = currentIndex;
            const logo2Index = (currentIndex + 1) % totalLogos; // Ensure it wraps around

            if (animatedLogoElements[0]) {
                animatedLogoElements[0].src = logoData[logo1Index].src;
                animatedLogoElements[0].alt = logoData[logo1Index].alt;
            }
            if (animatedLogoElements[1]) {
                animatedLogoElements[1].src = logoData[logo2Index].src;
                animatedLogoElements[1].alt = logoData[logo2Index].alt;
            }

            // Apply fade in
            animatedLogoElements.forEach(img => img.style.opacity = '1');

            // Move to the next set of logos
            currentIndex = (currentIndex + 1) % totalLogos; // Increment by 1 to show the next *pair*
        }, 800); // This timeout should match the CSS transition duration for opacity
    }

    function startAnimation() {
        if (mediaQuery.matches) {
            // Initialize with the first pair immediately
            updateAnimatedLogos();
            // Start the interval after initial display
            intervalId = setInterval(updateAnimatedLogos, fadeInterval);
        } else {
            if (intervalId) clearInterval(intervalId);
            if (animatedLogosContainer) animatedLogosContainer.style.display = 'none';
            if (originalLogosContainer) originalLogosContainer.style.display = 'flex'; // Show original
        }
    }

    // Initial check and start
    startAnimation();

    // Re-evaluate on window resize
    mediaQuery.addEventListener('change', startAnimation);
});