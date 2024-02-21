function SubmitButtonClick() {
    // Retrieve the hotel ID
    const hotelId = document.querySelector('#hotelId input').value;

    // Retrieve all the section elements
    const sections = document.querySelectorAll('#sections > section');

    // Create an array to store the input values
    const data = [];
    let isValid = true; // Flag to track if all fields are valid
    sections.forEach(section => {
        const CheckIn = section.querySelector('input[type="date"]:nth-of-type(1)').value;
        const CheckOut = section.querySelector('#endDate').value;
        const Adults = section.querySelector('input[type="number"]').value ? section.querySelector('input[type="number"]').value : 0;
        const HasChildren = section.querySelector('input[type="checkbox"]:nth-of-type(1)').checked;
        var ChildrenBeds = section.querySelector('#numKidsInput').value ? section.querySelector('#numKidsInput').value : 0;
        const HasKitchen = section.querySelector('#ownKitchenCheckbox').checked;
        const HasDog = section.querySelector('#dogCheckbox').checked;

        if (!HasChildren) {
            ChildrenBeds = 0;
        }
        // Add the input values to the array
        data.push({
            CheckIn,
            CheckOut,
            Adults,
            HasChildren,
            ChildrenBeds,
            HasKitchen,
            HasDog
        });

        // Perform validation for each field
        if (CheckIn === '') {
            section.querySelector('#startError').textContent = 'Введите дату заезда';
            section.querySelector('#endError').textContent = 'ㅤ';
            isValid = false;
        } else {
            section.querySelector('#startError').textContent = '';
        }


        if (CheckOut === '') {
            section.querySelector('#endError').textContent = 'Введите дату выезда';
            if (section.querySelector('#startError').textContent=='') {
                section.querySelector('#startError').textContent = 'ㅤ';
            }
            isValid = false;
        }
        else if (CheckOut <= CheckIn) {
            section.querySelector('#endError').textContent = 'Дата выезда должна быть позже даты заезда';
            if (section.querySelector('#startError').textContent == '') {
                section.querySelector('#startError').textContent = 'ㅤ';
            }
            isValid = false;
        }
        else {
            if (section.querySelector('#endError').textContent != 'ㅤ') {
                section.querySelector('#endError').textContent = '';
            }
                
            }


            if (Adults === 0 && ChildrenBeds === 0) {
                section.querySelector('#peopleError').textContent = 'Введите количество проживающих в номере взрослых (и/или детей)';
                isValid = false;
            }
            else {
                section.querySelector('#peopleError').textContent = '';
            }
        });

    if (!isValid) {
        adjustHeight();
        return; // Abort sending the request if any field is invalid
    }

    const Data = {
        HotelId: hotelId,
        Sections: data
    };

    // Send a POST request to the server
    const xhr = new XMLHttpRequest();
    xhr.open('POST', '/BookingOptions/SendQuery');
    xhr.setRequestHeader('Content-Type', 'application/json;charset=UTF-8');
    xhr.onload = function () {
        if (xhr.status === 200) {
            if (xhr.responseText.includes("BookingOptions")) {
                window.location.href = xhr.responseText;
            } else {
                alert(xhr.responseText);
            }
        } else {
            console.log(xhr.statusText);
        }
    };
    xhr.send(JSON.stringify(Data));
}
function addNewSection(showPetCheckbox) {
    // Create a new section element with Bootstrap classes and fade-in animation
    const newSection = document.createElement("section");
    newSection.className = "p-3 bg-light border rounded fade-in";
    newSection.id = "section";
    newSection.style.animationDuration = "0.5s";
    newSection.style.animationFillMode = "forwards";

    // Create a delete button with Bootstrap classes and fade-out animation
    const deleteButton = document.createElement("button");
    deleteButton.type = "button";
    deleteButton.className = "btn-close";
    deleteButton.setAttribute("aria-label", "Close");
    deleteButton.addEventListener("click", () => {
        newSection.classList.remove("fade-in");
        newSection.classList.add("fade-out");
        setTimeout(() => newSection.remove(), 500);
        adjustHeight();

    });
    newSection.appendChild(deleteButton);


    const startEndDiv = document.createElement("div");
    startEndDiv.className = "mb-6";
    const startLabel = document.createElement("label");
    startLabel.textContent = "Заезд";
    startLabel.className = "form-label";
    const startDateTimePicker = document.createElement("input");
    startDateTimePicker.type = "date";
    startDateTimePicker.className = "form-control";
    startDateTimePicker.required = true;

    const startErrorSpan = document.createElement("span");
    startErrorSpan.id = "startError";
    startErrorSpan.className = "text-danger";

    startLabel.appendChild(startDateTimePicker);
    startLabel.appendChild(startErrorSpan);
    startEndDiv.appendChild(startLabel);

    const endLabel = document.createElement("label");
    endLabel.textContent = "Выезд";
    endLabel.className = "form-label";

    const endDateTimePicker = document.createElement("input");
    endDateTimePicker.type = "date";
    endDateTimePicker.className = "form-control";
    endDateTimePicker.required = true;
    endDateTimePicker.id = "endDate";

    const endErrorSpan = document.createElement("span");
    endErrorSpan.id = "endError";
    endErrorSpan.className = "text-danger";

    endLabel.appendChild(endDateTimePicker);
    endLabel.appendChild(endErrorSpan);
    startEndDiv.appendChild(endLabel);

    newSection.appendChild(startEndDiv);

    // Create a number field for "Number of People" with Bootstrap classes
    const peopleDiv = document.createElement("div");
    peopleDiv.className = "mb-3";
    const peopleLabel = document.createElement("label");
    peopleLabel.textContent = "Количество взрослых (старше 10 лет)";
    peopleLabel.className = "form-label";
    const peopleInput = document.createElement("input");
    peopleInput.type = "number";
    peopleInput.min = 1;
    peopleInput.max = 1000;
    peopleInput.className = "form-control";
    peopleInput.width = startDateTimePicker.width;
    peopleInput.required = true;
    const peopleErrorSpan = document.createElement("span");
    peopleErrorSpan.id = "peopleError";
    peopleErrorSpan.className = "text-danger";
    peopleLabel.appendChild(peopleInput);
    peopleLabel.appendChild(peopleErrorSpan);
    peopleDiv.appendChild(peopleLabel);
    newSection.appendChild(peopleDiv);

    // Create kids checkbox
    const kidsDiv = document.createElement("div");
    kidsDiv.className = "mb-3";
    const kidsLabel = document.createElement("label");
    kidsLabel.textContent = "С детьми младше 10 лет";
    kidsLabel.className = "form-check-label";
    const kidsInput = document.createElement("input");
    kidsInput.type = "checkbox";
    kidsInput.className = "form-check-input";
    kidsInput.addEventListener("change", () => {
        if (kidsInput.checked) {
            numKidsInputContainer.style.display = "block";
            adjustHeight();
        } else {
            numKidsInputContainer.style.display = "none";
            numKidsInput.value = "";
            adjustHeight();
        }
    });
    kidsDiv.appendChild(kidsInput);
    kidsDiv.appendChild(kidsLabel);
    newSection.appendChild(kidsDiv);

    // Create number of kids input (only appears when kids checkbox is checked)
    const numKidsInputContainer = document.createElement("div");
    numKidsInputContainer.style.display = "none";
    const numKidsLabel = document.createElement("label");
    numKidsLabel.textContent = "Количество спальных мест для детей (дети до 10-и лет могут бесплатно размещаться на кроватях  вместе с родителями, в таком случае не указывайте необходимость спальных мест для них)";
    numKidsLabel.className = "form-label";
    const numKidsInput = document.createElement("input");
    numKidsInput.type = "number";
    numKidsInput.id = "numKidsInput";
    numKidsInput.min = 0;
    numKidsInput.max = 1000;
    numKidsInput.className = "form-control";
    numKidsInputContainer.appendChild(numKidsLabel);
    numKidsInputContainer.appendChild(numKidsInput);
    newSection.appendChild(numKidsInputContainer);

    const kitchenDiv = document.createElement("div");
    kitchenDiv.className = "mb-3";
    // Create a checkbox for "Own kitchen" with Bootstrap classes
    const ownKitchenLabel = document.createElement("label");
    ownKitchenLabel.textContent = "Со своей кухней";
    ownKitchenLabel.className = "form-check-label";
    const ownKitchenCheckbox = document.createElement("input");
    ownKitchenCheckbox.type = "checkbox";
    /*    ownKitchenCheckbox.type = "hidden"*/
    ownKitchenCheckbox.id = "ownKitchenCheckbox";
    ownKitchenCheckbox.className = "form-check-input";
    kitchenDiv.appendChild(ownKitchenCheckbox);
    kitchenDiv.appendChild(ownKitchenLabel);
    newSection.appendChild(kitchenDiv);
    

    // Create a checkbox for "Dog" with Bootstrap classes
    const dogDiv = document.createElement("div");
    dogDiv.className = "mb-3";
    const dogLabel = document.createElement("label");
    dogLabel.textContent = "С домашним питомцем";
    dogLabel.className = "form-check-label";
    const dogCheckbox = document.createElement("input");
    dogCheckbox.type = "checkbox";
    dogCheckbox.id = "dogCheckbox";
    dogCheckbox.className = "form-check-input";
    dogDiv.appendChild(dogCheckbox);
    dogDiv.appendChild(dogLabel);
    newSection.appendChild(dogDiv);
    if (!showPetCheckbox) {

        dogCheckbox.style.display = "none";
        dogLabel.style.display = "none";
    }


    const sectionsDiv = document.getElementById("sections");
    sectionsDiv.appendChild(newSection);

    adjustHeight();
    adjustWidth();

}
var k = 0;
function adjustWidth() {
    const sections = document.querySelectorAll('#sections > section');
    sections.forEach(section => {
        const CheckIn = section.querySelector('input[type="date"]:nth-of-type(1)');
        const Adults = section.querySelector('input[type="number"]');
        var ChildrenBeds = section.querySelector('#numKidsInput');
        const startInputWidth = window.getComputedStyle(CheckIn).getPropertyValue("width");
        Adults.style.width = startInputWidth;
        ChildrenBeds.style.width = startInputWidth;
    })
}
function adjustHeight() {
    var scrollHeight = Math.max(
        document.body.scrollHeight,
        document.documentElement.scrollHeight,
        document.body.offsetHeight,
        document.documentElement.offsetHeight,
        document.body.clientHeight,
        document.documentElement.clientHeight
    );

    var body = document.getElementsByTagName("body")[0];
    if (k == 1||k==0) {
        body.style.height = scrollHeight + 'px';
    }
    else {
        body.style.height = scrollHeight +25 + 'px';
    }
    k = k + 1;
} 


//function SubmitButtonClick() {
//    // Retrieve the hotel ID
//    const hotelId = document.querySelector('#hotelId input').value;

//    // Retrieve all the section elements
//    const sections = document.querySelectorAll('#sections > section');

//    // Create an array to store the input values
//    const data = [];
//    sections.forEach(section => {
//        const CheckIn = section.querySelector('input[type="date"]:nth-of-type(1)').value ? section.querySelector('input[type="date"]:nth-of-type(1)').value : "0001-01-01";
//        const CheckOut = section.querySelector('#endDate').value ? section.querySelector('#endDate').value : "0001-01-01";
//        const Adults = section.querySelector('input[type="number"]').value ? section.querySelector('input[type="number"]').value : 0;
//        const HasChildren = section.querySelector('input[type="checkbox"]:nth-of-type(1)').checked;
//        const ChildrenBeds = section.querySelector('#numKidsInput').value ? section.querySelector('#numKidsInput').value : 0;
//        const HasKitchen = section.querySelector('#ownKitchenCheckbox').checked;
//        const HasDog = section.querySelector('#dogCheckbox').checked;

//        // Add the input values to the array
//        data.push({
//            CheckIn,
//            CheckOut,
//            Adults,
//            HasChildren,
//            ChildrenBeds,
//            HasKitchen,
//            HasDog
//        });
//    });

//    const Data = {
//        HotelId: hotelId,
//        Sections: data
//    };
//    // Send a POST request to the server
//    const xhr = new XMLHttpRequest();
//    xhr.open('POST', '/BookingOptions/SendQuery');
//    xhr.setRequestHeader('Content-Type', 'application/json;charset=UTF-8');
//    xhr.onload = function () {
//        if (xhr.status === 200) {
//            if (xhr.responseText.includes("BookingOptions")) {
//                window.location.replace(xhr.responseText);
//            }
//            else {
//                alert(xhr.responseText);
//            }
//        } else {
//            console.log(xhr.statusText);
//        }
//    };
//    xhr.send(JSON.stringify(Data));
//}
//function addNewSection() {
//    // Create a new section element with Bootstrap classes and fade-in animation
//    const newSection = document.createElement("section");
//    newSection.className = "p-3 bg-light border rounded fade-in";
//    newSection.style.animationDuration = "0.5s";
//    newSection.style.animationFillMode = "forwards";

//    // Create a delete button with Bootstrap classes and fade-out animation
//    const deleteButton = document.createElement("button");
//    deleteButton.type = "button";
//    deleteButton.className = "btn-close";
//    deleteButton.setAttribute("aria-label", "Close");
//    deleteButton.addEventListener("click", () => {
//        newSection.classList.remove("fade-in");
//        newSection.classList.add("fade-out");
//        setTimeout(() => newSection.remove(), 500);
//    });
//    newSection.appendChild(deleteButton);

//    // Create date time pickers for start and end dates with Bootstrap classes
//    const startEndDiv = document.createElement("div");
//    startEndDiv.className = "mb-3";
//    const startLabel = document.createElement("label");
//    startLabel.textContent = "Заезд";
//    startLabel.className = "form-label";
//    const startDateTimePicker = document.createElement("input");
//    startDateTimePicker.type = "date";
//    startDateTimePicker.className = "form-control";
//    startLabel.appendChild(startDateTimePicker);
//    startEndDiv.appendChild(startLabel);

//    const endLabel = document.createElement("label");
//    endLabel.textContent = "Выезд";
//    endLabel.className = "form-label";
//    const endDateTimePicker = document.createElement("input");
//    endDateTimePicker.type = "date";
//    endDateTimePicker.id = 'endDate';
//    startDateTimePicker.setAttribute("step", "1d");
//    endDateTimePicker.className = "form-control";
//    endLabel.appendChild(endDateTimePicker);
//    startEndDiv.appendChild(endLabel);
//    newSection.appendChild(startEndDiv);

//    // Create a number field for "Number of People" with Bootstrap classes
//    const peopleDiv = document.createElement("div");
//    peopleDiv.className = "mb-3";
//    const peopleLabel = document.createElement("label");
//    peopleLabel.textContent = "Количество взрослых (старше 10 лет)";
//    peopleLabel.className = "form-label";
//    const peopleInput = document.createElement("input");
//    peopleInput.type = "number";
//    peopleInput.min = 1;
//    peopleInput.max = 1000;
//    peopleInput.className = "form-control";
//    peopleLabel.appendChild(peopleInput);
//    peopleDiv.appendChild(peopleLabel);
//    newSection.appendChild(peopleDiv);

//    // Create kids checkbox
//    const kidsDiv = document.createElement("div");
//    kidsDiv.className = "mb-3";
//    const kidsLabel = document.createElement("label");
//    kidsLabel.textContent = "С детьми младше 10 лет";
//    kidsLabel.className = "form-check-label";
//    const kidsInput = document.createElement("input");
//    kidsInput.type = "checkbox";
//    kidsInput.className = "form-check-input";
//    kidsInput.addEventListener("change", () => {
//        if (kidsInput.checked) {
//            numKidsInputContainer.style.display = "block";
//        } else {
//            numKidsInputContainer.style.display = "none";
//            numKidsInput.value = "";
//        }
//    });
//    kidsLabel.appendChild(kidsInput);
//    kidsDiv.appendChild(kidsLabel);
//    newSection.appendChild(kidsDiv);

//    // Create number of kids input (only appears when kids checkbox is checked)
//    const numKidsInputContainer = document.createElement("div");
//    numKidsInputContainer.style.display = "none";
//    const numKidsLabel = document.createElement("label");
//    numKidsLabel.textContent = "Количество спальных мест для детей (дети до 10-и лет могут спать с родителями, в таком случе не указывайте необходимость спальных мест для них)";
//    numKidsLabel.className = "form-label";
//    const numKidsInput = document.createElement("input");
//    numKidsInput.type = "number";
//    numKidsInput.id = "numKidsInput";
//    numKidsInput.min = 0;
//    numKidsInput.max = 1000;
//    numKidsInput.className = "form-control";
//    numKidsInputContainer.appendChild(numKidsLabel);
//    numKidsInputContainer.appendChild(numKidsInput);
//    newSection.appendChild(numKidsInputContainer);

//    // Create a checkbox for "Own kitchen" with Bootstrap classes
//    const ownKitchenLabel = document.createElement("label");
//    ownKitchenLabel.textContent = "Со своей кухней";
//    ownKitchenLabel.className = "form-check-label d-block";
//    const ownKitchenCheckbox = document.createElement("input");
//    ownKitchenCheckbox.type = "checkbox";
///*    ownKitchenCheckbox.type = "hidden"*/
//    ownKitchenCheckbox.id = "ownKitchenCheckbox";
//    ownKitchenCheckbox.className = "form-check-input";
//    ownKitchenLabel.appendChild(ownKitchenCheckbox);
//    newSection.appendChild(ownKitchenLabel);

//    // Create a checkbox for "Dog" with Bootstrap classes
//    const dogLabel = document.createElement("label");
//    dogLabel.textContent = "С собакой";
//    dogLabel.className = "form-check-label d-block";
//    const dogCheckbox = document.createElement("input");
//    dogCheckbox.type = "checkbox";
//    dogCheckbox.id = "dogCheckbox";
//    dogCheckbox.className = "form-check-input";
//    dogLabel.appendChild(dogCheckbox);
//    newSection.appendChild(dogLabel);

//    const sectionsDiv = document.getElementById("sections");
//    sectionsDiv.appendChild(newSection);
//}

//function SubmitButtonClick() {
//    // Retrieve the hotel ID
//    const hotelId = document.querySelector('#hotelId input').value;

//    // Retrieve all the section elements
//    const sections = document.querySelectorAll('#sections > section');

//    // Create an array to store the input values
//    const data = [];
//    let isValid = true; // Flag to track if all fields are valid
//    sections.forEach(section => {
//        const CheckIn = section.querySelector('input[type="date"]:nth-of-type(1)').value;
//        const CheckOut = section.querySelector('#endDate').value;
//        const Adults = section.querySelector('input[type="number"]').value;
//        const HasChildren = section.querySelector('input[type="checkbox"]:nth-of-type(1)').checked;
//        const ChildrenBeds = section.querySelector('#numKidsInput').value;
//        const HasKitchen = section.querySelector('#ownKitchenCheckbox').checked;
//        const HasDog = section.querySelector('#dogCheckbox').checked;

//        // Add the input values to the array
//        data.push({
//            CheckIn,
//            CheckOut,
//            Adults,
//            HasChildren,
//            ChildrenBeds,
//            HasKitchen,
//            HasDog
//        });

//        // Perform validation for each field
//        if (CheckIn === '') {
//            section.querySelector('#startError').textContent = 'Введите дату заезда';
//            isValid = false;
//        } else {
//            section.querySelector('#startError').textContent = '';
//        }

//        if (CheckOut === '') {
//            section.querySelector('#endError').textContent = 'Введите дату выезда';
//            isValid = false;
//        } else {
//            section.querySelector('#endError').textContent = '';
//        }

//        if (Adults === '') {
//            section.querySelector('#peopleError').textContent = 'Введите количество взрослых';
//            isValid = false;
//        } else {
//            section.querySelector('#peopleError').textContent = '';
//        }
//    });

//    if (!isValid) {
//        return; // Abort sending the request if any field is invalid
//    }

//    const Data = {
//        HotelId: hotelId,
//        Sections: data
//    };

//    // Send a POST request to the server
//    const xhr = new XMLHttpRequest();
//    xhr.open('POST', '/BookingOptions/SendQuery');
//    xhr.setRequestHeader('Content-Type', 'application/json;charset=UTF-8');
//    xhr.onload = function () {
//        if (xhr.status === 200) {
//            if (xhr.responseText.includes("BookingOptions")) {
//                window.location.replace(xhr.responseText);
//            } else {
//                alert(xhr.responseText);
//            }
//        } else {
//            console.log(xhr.statusText);
//        }
//    };
//    xhr.send(JSON.stringify(Data));
//}
//function addNewSection() {
//    // Create a new section element with Bootstrap classes and fade-in animation
//    const newSection = document.createElement("section");
//    newSection.className = "p-3 bg-light border rounded fade-in";
//    newSection.style.animationDuration = "0.5s";
//    newSection.style.animationFillMode = "forwards";

//    // Create a delete button with Bootstrap classes and fade-out animation
//    const deleteButton = document.createElement("button");
//    deleteButton.type = "button";
//    deleteButton.className = "btn-close";
//    deleteButton.setAttribute("aria-label", "Close");
//    deleteButton.addEventListener("click", () => {
//        newSection.classList.remove("fade-in");
//        newSection.classList.add("fade-out");
//        setTimeout(() => newSection.remove(), 500);
//    });
//    newSection.appendChild(deleteButton);


//    const startEndDiv = document.createElement("div");
//    startEndDiv.className = "mb-6";
//    const startLabel = document.createElement("label");
//    startLabel.textContent = "Заезд";
//    startLabel.className = "form-label";
//    const startDateTimePicker = document.createElement("input");
//    startDateTimePicker.type = "date";
//    startDateTimePicker.className = "form-control";
//    startDateTimePicker.required = true;

//    const startErrorSpan = document.createElement("span");
//    startErrorSpan.id = "startError";
//    startErrorSpan.className = "text-danger";

//    startLabel.appendChild(startDateTimePicker);
//    startLabel.appendChild(startErrorSpan);
//    startEndDiv.appendChild(startLabel);

//    const endLabel = document.createElement("label");
//    endLabel.textContent = "Выезд";
//    endLabel.className = "form-label";

//    const endDateTimePicker = document.createElement("input");
//    endDateTimePicker.type = "date";
//    endDateTimePicker.className = "form-control";
//    endDateTimePicker.required = true;
//    endDateTimePicker.id = "endDate";

//    const endErrorSpan = document.createElement("span");
//    endErrorSpan.id = "endError";
//    endErrorSpan.className = "text-danger";

//    endLabel.appendChild(endDateTimePicker);
//    endLabel.appendChild(endErrorSpan);
//    startEndDiv.appendChild(endLabel);

//    newSection.appendChild(startEndDiv);

//    // Create a number field for "Number of People" with Bootstrap classes
//    const peopleDiv = document.createElement("div");
//    peopleDiv.className = "mb-3";
//    const peopleLabel = document.createElement("label");
//    peopleLabel.textContent = "Количество взрослых (старше 10 лет)";
//    peopleLabel.className = "form-label";
//    const peopleInput = document.createElement("input");
//    peopleInput.type = "number";
//    peopleInput.min = 1;
//    peopleInput.max = 1000;
//    peopleInput.className = "form-control";
//    peopleInput.required = true;
//    const peopleErrorSpan = document.createElement("span");
//    peopleErrorSpan.id = "peopleError";
//    peopleErrorSpan.className = "text-danger";
//    peopleLabel.appendChild(peopleInput);
//    peopleLabel.appendChild(peopleErrorSpan);
//    peopleDiv.appendChild(peopleLabel);
//    newSection.appendChild(peopleDiv);

//    // Create kids checkbox
//    const kidsDiv = document.createElement("div");
//    kidsDiv.className = "mb-3";
//    const kidsLabel = document.createElement("label");
//    kidsLabel.textContent = "С детьми младше 10 лет";
//    kidsLabel.className = "form-check-label";
//    const kidsInput = document.createElement("input");
//    kidsInput.type = "checkbox";
//    kidsInput.className = "form-check-input";
//    kidsInput.addEventListener("change", () => {
//        if (kidsInput.checked) {
//            numKidsInputContainer.style.display = "block";
//        } else {
//            numKidsInputContainer.style.display = "none";
//            numKidsInput.value = "";
//        }
//    });
//    kidsLabel.appendChild(kidsInput);
//    kidsDiv.appendChild(kidsLabel);
//    newSection.appendChild(kidsDiv);

//    // Create number of kids input (only appears when kids checkbox is checked)
//    const numKidsInputContainer = document.createElement("div");
//    numKidsInputContainer.style.display = "none";
//    const numKidsLabel = document.createElement("label");
//    numKidsLabel.textContent = "Количество спальных мест для детей (дети до 10-и лет могут спать с родителями, в таком случае не указывайте необходимость спальных мест для них)";
//    numKidsLabel.className = "form-label";
//    const numKidsInput = document.createElement("input");
//    numKidsInput.type = "number";
//    numKidsInput.id = "numKidsInput";
//    numKidsInput.min = 0;
//    numKidsInput.max = 1000;
//    numKidsInput.className = "form-control";
//    numKidsInputContainer.appendChild(numKidsLabel);
//    numKidsInputContainer.appendChild(numKidsInput);
//    newSection.appendChild(numKidsInputContainer);

//    // Create a checkbox for "Own kitchen" with Bootstrap classes
//    const ownKitchenLabel = document.createElement("label");
//    ownKitchenLabel.textContent = "Со своей кухней";
//    ownKitchenLabel.className = "form-check-label d-block";
//    const ownKitchenCheckbox = document.createElement("input");
//    ownKitchenCheckbox.type = "checkbox";
//    /*    ownKitchenCheckbox.type = "hidden"*/
//    ownKitchenCheckbox.id = "ownKitchenCheckbox";
//    ownKitchenCheckbox.className = "form-check-input";
//    ownKitchenLabel.appendChild(ownKitchenCheckbox);
//    newSection.appendChild(ownKitchenLabel);

//    // Create a checkbox for "Dog" with Bootstrap classes
//    const dogLabel = document.createElement("label");
//    dogLabel.textContent = "С собакой";
//    dogLabel.className = "form-check-label d-block";
//    const dogCheckbox = document.createElement("input");
//    dogCheckbox.type = "checkbox";
//    dogCheckbox.id = "dogCheckbox";
//    dogCheckbox.className = "form-check-input";
//    dogLabel.appendChild(dogCheckbox);
//    newSection.appendChild(dogLabel);

//    const sectionsDiv = document.getElementById("sections");
//    sectionsDiv.appendChild(newSection);
//}