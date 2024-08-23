function checkWindowSize() {
    const menu = document.querySelector('.store-leftmeau-n');
    console.log(menu); // Check if element is null
    if (menu) {
        if (window.innerWidth <= 767) {
            
            menu.classList.remove('show');
        } else {
            menu.classList.add('show');
        }
    } else {
        console.error('Element with class "store-leftmenu-n" not found');
    }
}

// Run checkWindowSize on initial load
checkWindowSize();

// Run checkWindowSize on window resize
window.addEventListener('resize', checkWindowSize);