
// ---- This section is for UI manipulation -------
//***************************************************
// Add event listners to DOM Objects

export const siteEvents = () => {
    document.addEventListener("DOMContentLoaded", (event) => {
        const showNavbar = (toggleId, navId, bodyId, headerId) => {
            const toggle = document.getElementById(toggleId),
                nav = document.getElementById(navId),
                bodypd = document.getElementById(bodyId),
                headerpd = document.getElementById(headerId)

            // Validate that all variables exist
            if (toggle && nav && bodypd && headerpd) {
                toggle.addEventListener('click', () => {
                    // show navbar
                    nav.classList.toggle('show-navbar')
                    // change icon
                    toggle.classList.toggle('bx-x')
                    // add padding to body
                    bodypd.classList.toggle('body-pd')
                    // add padding to header
                    headerpd.classList.toggle('body-pd')
                })
            }
        }



        showNavbar('header-toggle', 'nav-bar', 'body-pd', 'header')

        /*===== LINK ACTIVE =====*/
        const linkColor = document.querySelectorAll('.nav_link')

        function colorLink() {
            if (linkColor) {
                linkColor.forEach(l => l.classList.remove('active'))
                this.classList.add('active')
            }
        }
        linkColor.forEach(l => l.addEventListener('click', colorLink))



        const alert = document.querySelector(".show"); //Represents database connection status notification DOM element

        //// Handles Database connection Alerts
        //const handleAlertEffect = (element) => {

        //    if (element.classList.contains("show")) {
        //        setTimeout(() => {
        //            element.classList.remove("show")
        //            element.classList.add("fade-out")
        //        }, 50)

        //    }
        //}

        //handleAlertEffect(alert);

    });


}