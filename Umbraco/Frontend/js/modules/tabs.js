document.addEventListener("DOMContentLoaded", () => {

    function showTab(trigger){
        const tab = new bootstrap.Tab(trigger)
        tab.show()
    }
    
    // check if we have a hash, then load that tab
    if (window.location.hash) {
        const trigger = document.querySelector(window.location.hash)
        showTab(trigger);
    }
    
    // We may have been returned to the page from Umbraco
    // Let's grab the hidden field and reset the window hash
    const hiddenTabElement = document.getElementById("CurrentTab");
    if (hiddenTabElement){
        const trigger = document.getElementById(hiddenTabElement.value)
        showTab(trigger);
    }
    

    const tabElements = document.querySelectorAll('[data-bs-toggle="pill"]');
    tabElements.forEach(tabElement => tabElement.addEventListener('shown.bs.tab', function (event) {
        window.location.hash = event.target.id;
        event.target // newly activated tab
        event.relatedTarget // previous active tab
        
        window.scrollTo(0, 0);
    }));
})

