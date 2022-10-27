// document ready
document.addEventListener('DOMContentLoaded', fn, false);

function fn() {
    var currentScrollTop = 0;
    
    function refreshNavbar() {
        const newScrollTop = window.pageYOffset || document.documentElement.scrollTop;
        
        
        if (newScrollTop > currentScrollTop && currentScrollTop > 300) {
            console.log("hide navbar")
            document.getElementById("navbar").classList.add("navbar-hide");
        } else if (newScrollTop < currentScrollTop) {
            document.getElementById("navbar").classList.remove("navbar-hide");
        }
        currentScrollTop = newScrollTop;
    }

    window.addEventListener("scroll", refreshNavbar, {passive: true});
    
    refreshNavbar();
}  