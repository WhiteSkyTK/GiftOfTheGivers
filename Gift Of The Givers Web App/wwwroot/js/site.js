// Wait for the DOM to be fully loaded before running scripts
document.addEventListener('DOMContentLoaded', function () {

    // --- Navbar on Scroll ---
    const navbar = document.querySelector('.navbar');
    if (navbar) {
        window.addEventListener('scroll', function () {
            if (window.scrollY > 50) { // Add class after scrolling 50px
                navbar.classList.add('navbar-scrolled');
            } else {
                navbar.classList.remove('navbar-scrolled');
            }
        });
    }

    // --- Back to Top Button ---
    const btnBackToTop = document.getElementById('btnBackToTop');

    if (btnBackToTop) {
        window.addEventListener('scroll', function () {
            if (window.scrollY > 200) { // Show button after scrolling 200px
                btnBackToTop.style.display = 'block';
            } else {
                btnBackToTop.style.display = 'none';
            }
        });

        btnBackToTop.addEventListener('click', function () {
            window.scrollTo({ top: 0, behavior: 'smooth' });
        });
    }

});