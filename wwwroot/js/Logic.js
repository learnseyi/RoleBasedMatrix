"use strict"


export const Logic = () => {
    document.addEventListener("DOMContentLoaded", (event) => {


        const divisionName = document.querySelector("#DivisionName");
        const departmentName = document.querySelector("#DepartmentName");
        const roleName = document.querySelector("#RoleName");
        const searchSubmitBtn = document.querySelector(".search-btn");
        const loader = document.querySelector(".spinner-container");
        const editBtn = document.querySelector(".edit-btn");
        const resultHeader = document.querySelector(".result-header span");
        const tableContainer = document.querySelector(".table-container");
        const modal = document.querySelector("#edit-modal");
        let editIcons;

        let tempDb = [];


        /*** This sections declares helper functions used for fetch calls and event handlers ***/

        /**
         * sets the attributes for a specified element
         * @param {any} elem
         * @param {any} attrs
         */
        const setAttributes = (elem, attrs) => {
            Object.keys(attrs).forEach(key => {
                elem.setAttribute(key, attrs[key]);
            })
        }

        /**
         * activates and deactivates the loading effect
         */
        const showLoader = () => {
            setTimeout(() => {
                loader.classList.add("invisible")
            }, 2000)
            loader.classList.remove("invisible")
        }


        /**
         * Gets the current permission on the edit modal 
         * @param {any} elem
         */
        const getModalElements = (elem) => {
            const deletePermsBtn = elem.querySelectorAll(".delete-btn");
            deletePermsBtn.forEach(btn => {
                btn.addEventListener("click", (e) => { deleteColumns(e.target.classname)})
            })
            console.log(deletePermsBtn);
        }

        /**
         * Creates and populates the results for a search
         * @param {any} tableData
         * @param {any} i
         */
        const createTable = (tableData, i) => {
            const { name, Authorizations } = tableData;
            console.log(Authorizations);
            let comments;
            const tableContainer = document.querySelector(".table-container");
            const notes = document.createElement("span");
            const label = document.createElement("label");
            label.classList.add("app-name", "hover");
            label.setAttribute("id", i);
            tableContainer.append(label);
            const labelText = document.getElementById(i);
            labelText.innerText = name;
            const span = document.createElement("span");
            const icon = document.createElement("i");
            icon.classList.add("bi-pencil-square", "invisible");
            span.append(icon);
            tableContainer.append(span);
            editIcons = document.querySelectorAll(".bi-pencil-square");
            const table = document.createElement("table");
            table.classList.add("result-table", "table", "table-sm", "table-hover", "container", "table-bordered", "mt-0", "rounded-3");
            const row = table.insertRow();
            row.classList.add("border-1");
            Authorizations.forEach(auth => {
                console.log(auth)
                const { AppPermissions } = auth;
                let { NOTES } = AppPermissions[0] || "";
                comments = NOTES
                let cell = row.insertCell();
                cell.innerText = auth.permissions;
                if (NOTES != null) {
                    cell.append(notes);
                }
                row.append(cell);
            })


            notes.innerText = "notes";
            notes.classList.add("badge", "bg-warning", "text-dark", "float-end", "mt-1", "permission-note", "shadow");
            setAttributes(notes, {
                "data-bs-container": "body",
                "data-bs-toggle": "popover",
                "data-bs-trigger": "hover focus",
                "data-bs-content": comments
            })

            tableContainer.appendChild(table);
            document.querySelectorAll('[data-bs-toggle="popover"]').forEach(el => new bootstrap.Popover(el));
        }



        /**
         * Resets the select option in the form
         * @param {current select-form} target
         * @returns
         */
        const clearOptions = (target) => {
            /*assignmentTable.classList.add("invisible");*/
            switch (target) {
                case "DivisionName":
                    target = {
                        "name": departmentName,
                        "key": "departmentId",
                        "value": "departmentName",
                        "desc": "- Select Department -"
                    }
                    break;
                case "DepartmentName":
                    target = target = {
                        "name": roleName,
                        "key": "roleId",
                        "value": "roleName",
                        "desc": "- Select Role -"
                    }
                    break;
                default:
                    target = searchSubmitBtn
                    break;

            }
            showLoader();
            if (target.name.options.length != 0) {
                tempDb = [];
                while (target.name.options.length > 0) {
                    target.name.remove(0);
                }
                let desc = new Option(target.desc);
                target.name.add(desc);
            }
            target.name.disabled = true;
            return target;
        }


        /*** Creates new coulumns for the permission edit modal ****/
        const addColumns = (content) => {
            const colClass = ["col-md-auto", "d-flex", "mb-3",];
            const div = document.createElement("div");
            colClass.forEach(classname => {
                div.classList.add(classname)
            })
            div.innerHTML = content;
            const span = div.getElementsByTagName("span");
            activateTooltip(span);
            return div;
        }


        /**
         * Gets and sets the current permissions
         * @param {any} elem
         */
        const updatePermissions = (elem) => {
            const currentPermissions = [];
            const getCurrentPermissions = Array.from(elem.getElementsByTagName("select"))
            getCurrentPermissions.forEach(permissions=> console.log(permissions.options[permissions.selectedIndex].text))
        }

        /**
         *  Deletes permissions from the permissions edit modal
         */
        const deleteColumns = (elem) => {
            const deleteBtns = Array.from(elem.getElementsByClassName("delete-btn"));
            const permissions = elem.getElementsByClassName("permissions-row")
 
            if (deleteBtns.length === 0) return;
            deleteBtns.forEach(btn => {
                btn.addEventListener("click", (event) => {
                    updatePermissions(elem);
                    const currentPermission = event.target.parentNode.parentNode
                    Swal.fire({
                        icon: 'question',
                        title: 'Delete',
                        text: 'Do you want to delete this permission?!',
                    })
                    permissions[0].removeChild(currentPermission);


                    console.log(currentPermission);
                })
            })

        }

        /************This section declares the event handlers ********************/

        /**
         * Handles the selection change event of the select form options
         * @param {event} e
         */
        const handleSelectionChange = async (e) => {
            tableContainer.innerHTML = "";
            resultHeader.innerText = "";
            editBtn.classList.add("invisible");
            let search = e.target.value
            let name = e.target.name
            let nextOption;
            fetch(`/home/getResult/${name}/${search}`)
                .then(response => {
                    if (response.status !== 200) {
                        console.log('Looks like there was a problem. Status Code: \n' +
                            response.status);
                        return;
                    }
                    response.json().then(function (data) {
                        nextOption = clearOptions(name);
                        data.forEach(row => tempDb.push(row))
                        setTimeout(() => {
                            nextOption.name.disabled = false;
                            let { key, value } = nextOption;
                            tempDb.forEach((item) => {
                                let newOption = new Option(item[value], item[key]);
                                nextOption.name.add(newOption);
                            })
                        }, 2000)

                    });
                }
                )
                .catch((err) => {
                    console.log('Fetch Error :-S', err);
                });


        }

        /**
         * Handles the click event of the search button
         * @param {any} e
         */

        const searchSubmitClick = (e) => {
            e.preventDefault();
            fetch(`home/getAssignments/${roleName.value}`)
                .then(response => {
                    if (response.status !== 200) {
                        console.log('There was an error. See Status Code: \n' +
                            response.status);
                        return;
                    }
                    response.json()
                        .then(data => {
                            console.log(data)
                            let { Applications } = data;
                            let { role } = data;
                            /*console.log(Applications)*/
                            resultHeader.innerText = role;
                            Applications.forEach((app, index) => {
                                createTable(app, index);
                                tableContainer.classList.remove("invisible");
                                editBtn.classList.remove("invisible");

                                // revisit form validation and duplicate form submission
                                /*e.target.classList.add("disabled")*/
                                /*console.log(app);*/
                            })


                        })
                }).catch(error => console.log(error.message))

        }

        /**
         * Toggles the visibility of edit icons on the results
         * and attaches an event listener to the edit icons
         */

        const showEditIcons = () => {
            editIcons.forEach(icon => {
                editIconClick(icon);
                icon.classList.toggle("invisible");
            })
        }


        /**
         * Handles the click event of the edit icon
         * @param {any} target
         */

        const editIconClick = (target) => {
            target.addEventListener("click", () => {
                const role = document.querySelector('#result-display span').getInnerHTML()
                const appName = target.parentNode.previousElementSibling.innerText;
                const currentTarget = target.parentNode.nextElementSibling.querySelectorAll("td");
                /*const crrentPermissions = [];*/

                console.log(target.parentNode.nextElementSibling.querySelectorAll("td"))
                fetch(`/home/editPermissions/${role}/${appName}`, { Method: 'POST' })
                    .then(response => {
                        response.text()
                            .then(data => {
                                const parser = new DOMParser();
                                const content = parser.parseFromString(data, 'text/html');
                                const modalContent = content.querySelector(".modal-dialog")
                                modal.innerHTML = modalContent.innerHTML;
                                const editModal = new bootstrap.Modal("#edit-modal", { backdrop: "static ", keyboard: false });
                                editModal.show()
                                /*getModalElements(modal)*/
                                deleteColumns(modal)
                                const permissionSet = document.querySelector(".permissions-set");
                                console.log(permissionSet)
                            }).catch(error => console.error(error));
                    })
            })

        }


      






        // ***** Attaching all event listeners to elements *** //

        divisionName.addEventListener("change", handleSelectionChange);
        departmentName.addEventListener("change", handleSelectionChange);
        searchSubmitBtn.addEventListener("click", searchSubmitClick);
        editBtn.addEventListener("click", showEditIcons);

    })

}

