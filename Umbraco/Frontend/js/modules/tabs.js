document.addEventListener("DOMContentLoaded", () => {
    
    
    // check if we have a hash, then load that tab
    if (window.location.hash) {
        const trigger = document.querySelector(window.location.hash)
        let tab = new bootstrap.Tab(trigger)
        tab.show()
    }
    
    // We may have been returned to the page from Umbraco
    // Let's grab the hidden field and reset the window hash
    const hiddenTabElement = document.getElementById("CurrentTab");
    if (hiddenTabElement && hiddenTabElement.value){
        console.log(hiddenTabElement)
        const trigger = document.getElementById(hiddenTabElement.value)
        let tab = new bootstrap.Tab(trigger)
        tab.show()
    }
    

    const tabElements = document.querySelectorAll('[data-bs-toggle="pill"]');
    tabElements.forEach(tabElement => tabElement.addEventListener('shown.bs.tab', function (event) {
        console.log(event.target.id);
        window.location.hash = event.target.id;
        event.target // newly activated tab
        event.relatedTarget // previous active tab
        
        window.scrollTo(0, 0);
    }));
})

