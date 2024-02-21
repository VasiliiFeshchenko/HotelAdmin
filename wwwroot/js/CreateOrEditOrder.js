function SubmitButtonClick() {
    const data = GetData();
    // Send the AJAX request
    const xhr = new XMLHttpRequest();
    xhr.open('POST', '/Orders/CreateRoot');
    xhr.setRequestHeader('Content-Type', 'application/json;charset=UTF-8');
    xhr.onload = function () {
        if (xhr.status === 200) {
            // Check if the response is an HTML file
            if (xhr.responseText.includes("RoomsReservationsTable")) {
                window.location.replace(xhr.responseText);
            }
            else {
                alert(xhr.responseText);
            }

        } else {
            // Error
            console.log(xhr.statusText);
        }
    };
    xhr.send(JSON.stringify(data));
}

function SaveButtonClick() {
    const data = GetData();
    // Send the AJAX request
    const xhr = new XMLHttpRequest();
    xhr.open('POST', '/Orders/UpdateRoot');
    xhr.setRequestHeader('Content-Type', 'application/json;charset=UTF-8');
    xhr.onload = function () {
        if (xhr.status === 200) {
            // Check if the response is a URL
            if (xhr.responseText.includes("RoomsReservationsTable")) {
                window.location.replace(xhr.responseText);
            }
            else {
                alert(xhr.responseText);
            }

        } else {
            console.log(xhr.statusText);
        }
    };
    xhr.send(JSON.stringify(data));
}

function GetData()
{
    telephoneNumber = document.getElementById('TelephoneNumber').value;
    email = document.getElementById('EMail').value;
    clientName = document.getElementById('ClientName').value;
    price = document.getElementById('Price').value.replace(",", ".");
    comment = document.getElementById('Comment').value;
    hotelId = document.getElementById('HotelId').value;
    prepayment = document.getElementById('Prepayment').value.replace(",", ".");
    /*prepaymentDeadline = document.getElementById('PrepaymentDeadline').value;*/
    option = document.getElementById('option').value;
    id = document.getElementById('Id').value;

    if (telephoneNumber == "") {
        telephoneNumber = " ";
    }
    if (email == "") {
        email = " ";
    }
    if (clientName == "") {
        clientName = " ";
    }
    if (telephoneNumber == "") {
        telephoneNumber = " ";
    }
    if (price == "") {
        price = 0;
    }
    //if (payedPrice == "") {
    //    payedPrice = 0;
    //}
    if (comment == "") {
        comment = " ";
    }
    if (prepayment == "") {
        prepayment = 0
    }
    //if (prepaymentDeadline == "") {
    //    prepaymentDeadline = "0001-01-01"
    //}

    var sections = [];
    $('#sections section').each(function () {
        var sectionData = {};
        sectionData['Room'] = $(this).find('select').val();
        sectionData['NumberOfPeople'] = $(this).find('input[type="number"]').val();
        sectionData['StartDate'] = $(this).find('input[type="date"]').eq(0).val();
        sectionData['EndDate'] = $(this).find('input[type="date"]').eq(1).val();
        sectionData['Comment'] = $(this).find('textarea').val();
        if (sectionData['StartDate'] == "") {
            sectionData['StartDate'] = "0001-01-01"
        }
        if (sectionData['EndDate'] == "") {
            sectionData['EndDate'] = "0001-01-01"
        }
        if (sectionData['NumberOfPeople'] == "") {
            sectionData['NumberOfPeople'] = 0
        }
        if (sectionData['Comment'] == "") {
            sectionData['Comment'] = " "
        }
        if (sectionData['Room'] == "") {
            sectionData['Room'] = " "
        }
        sections.push(sectionData);
    });
    var moneyTransactions = [];

    // Function to get transaction data from a given section
    function getTransactionData(section) {
        var transactionData = {};
        transactionData['Method'] = section.find('select').val();
        transactionData['Amount'] = section.find('input[type="number"]').val().replace(",", ".");;
        transactionData['Date'] = section.find('input[type="datetime-local"]').val();
        transactionData['Comment'] = section.find('textarea').val();
        if (transactionData['Method'] == "") {
            transactionData['Method'] = " ";
        }
        if (transactionData['Amount'] == "") {
            transactionData['Amount'] = 0;
        }
        if (transactionData['Date'] == "") {
            transactionData['Date'] = "0001-01-01";
        }
        if (transactionData['Comment'] == "") {
            transactionData['Comment'] = " ";
        }
        return transactionData;
    }

    // Get income transactions
    $('#incomeTransactions section').each(function () {
        var transactionData = getTransactionData($(this));
        transactionData['IsRefund'] = false;
        moneyTransactions.push(transactionData);
    });

    // Get outcome transactions
    $('#outcomeTransactions section').each(function () {
        var transactionData = getTransactionData($(this));
        transactionData['IsRefund'] = true;
        moneyTransactions.push(transactionData);
    });
    const data = {
        TelephoneNumber: telephoneNumber,
        EMail: email,
        ClientName: clientName,
        Price: price,
        Prepayment: prepayment,
        Comment: comment,
        HotelId: hotelId,
        Sections: sections,
        Id: id,
        Source: option,
        MoneyTransactions: moneyTransactions
    };
    return data;
}

function addNewSection(roomValue = "", peopleValue = "", startDateValue = "", endDateValue = "", commentValue = "") {
    roomValue = roomValue.replace("&#x2B;", " ");

    // Create a new section element with Bootstrap classes and fade-in animation
    const newSection = document.createElement("section");
    newSection.className = "p-3 bg-light border rounded fade-in";
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
    });
    newSection.appendChild(deleteButton);


    const row1 = document.createElement("div");
    row1.className = "row align-items-end";
    newSection.appendChild(row1);
    const row2 = document.createElement("div");
    row2.className = "row align-items-end";
    newSection.appendChild(row2);
    const row3 = document.createElement("div");
    row3.className = "row align-items-end";
    newSection.appendChild(row3);
    const row4 = document.createElement("div");
    row4.className = "row align-items-end";
    newSection.appendChild(row4);

    const col1_1 = document.createElement("div");
    col1_1.className = "col";
    row1.appendChild(col1_1);
    const col1_2 = document.createElement("div");
    col1_2.className = "col";
    row1.appendChild(col1_2);

    const col2_1 = document.createElement("div");
    col2_1.className = "col";
    row2.appendChild(col2_1);
    const col2_2 = document.createElement("div");
    col2_2.className = "col";
    row2.appendChild(col2_2);

    const col3_1 = document.createElement("div");
    col3_1.className = "col";
    row3.appendChild(col3_1);
    const col3_2 = document.createElement("div");
    col3_2.className = "col";
    row3.appendChild(col3_2);

    const col4_1 = document.createElement("div");
    col4_1.className = "col";
    row4.appendChild(col4_1);
    const col4_2 = document.createElement("div");
    col4_2.className = "col";
    row4.appendChild(col4_2);

    // Create a dropdown list of rooms with Bootstrap classes
    const roomLabel = document.createElement("label");
    roomLabel.textContent = "Номер: ";
    const roomSelect = document.createElement("select");
    roomSelect.className = "form-select";
    const hotelIdInput = document.getElementById("HotelId");
    const hotelIdValue = parseInt(hotelIdInput.value);
    const url = "/Orders/GetRooms?hotelId=" + hotelIdValue;
    fetch(url)
        .then(response => response.json())
        .then(data => {
            // add each room as an option in the dropdown list
            data.forEach(room => {
                const roomOption = document.createElement("option");
                roomOption.textContent = room.name;
                roomSelect.appendChild(roomOption);
            });
            roomSelect.value = roomValue;
        })
        .catch(error => console.error(error));

    col1_1.appendChild(roomLabel);
    col2_1.appendChild(roomSelect);

    // Create a people number field with Bootstrap classes
    const peopleLabel = document.createElement("label");
    peopleLabel.textContent = "Количество спальных мест: ";
    const peopleInput = document.createElement("input");
    peopleInput.type = "number";
    peopleInput.min = 1;
    peopleInput.max = 1000;
    peopleInput.value = peopleValue;
    peopleInput.className = "form-control";
    col1_2.appendChild(peopleLabel);
    col2_2.appendChild(peopleInput);




    // Create date pickers for start and end dates with Bootstrap classes
    const startLabel = document.createElement("label");
    startLabel.textContent = "Начальная дата: ";
    const startDatePicker = document.createElement("input");
    startDatePicker.type = "date";
    startDatePicker.value = startDateValue;
    startDatePicker.className = "form-control";
    col3_1.appendChild(startLabel);
    col4_1.appendChild(startDatePicker);

    const endLabel = document.createElement("label");
    endLabel.textContent = "Конечная дата (последняя закрытая клетка в шахматке): ";
    const endDatePicker = document.createElement("input");
    endDatePicker.type = "date";
    endDatePicker.value = endDateValue;
    endDatePicker.className = "form-control";
    col3_2.appendChild(endLabel);
    col4_2.appendChild(endDatePicker);


    // Create a comment field with Bootstrap classes
    const commentLabel = document.createElement("label");
    commentLabel.textContent = "Комментарий: ";
    const commentInput = document.createElement("textarea");
    commentInput.rows = 1;
    commentInput.value = commentValue;
    commentInput.className = "form-control";
    commentInput.setAttribute("oninput", "this.style.height = ''; this.style.height = this.scrollHeight + 'px'");
    commentLabel.appendChild(commentInput);
    newSection.appendChild(commentLabel);

    // Add the new section to the existing div element with ID "sections"
    const sectionsDiv = document.getElementById("sections");
    sectionsDiv.appendChild(newSection);
}

function addNewTransaction(isRefund, methodValue = "", amountValue = "", dateValue = "", commentValue = "") {
    // Create a new section element with Bootstrap classes and fade-in animation
    const newSection = document.createElement("section");
    newSection.className = "p-3 bg-light border rounded fade-in";
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
    });
    newSection.appendChild(deleteButton);

    const row1 = document.createElement("div");
    row1.className = "row align-items-end";
    newSection.appendChild(row1);
    const row2 = document.createElement("div");
    row2.className = "row align-items-end";
    newSection.appendChild(row2);

    const col1_1 = document.createElement("div");
    col1_1.className = "col";
    row1.appendChild(col1_1);
    const col1_2 = document.createElement("div");
    col1_2.className = "col";
    row1.appendChild(col1_2);
    const col1_3 = document.createElement("div");
    col1_3.className = "col";
    row1.appendChild(col1_3);

    const col2_1 = document.createElement("div");
    col2_1.className = "col";
    row2.appendChild(col2_1);
    const col2_2 = document.createElement("div");
    col2_2.className = "col";
    row2.appendChild(col2_2);
    const col2_3 = document.createElement("div");
    col2_3.className = "col";
    row2.appendChild(col2_3);


    // Create a dropdown list of methods with Bootstrap classes
    const methodLabel = document.createElement("label");
    methodLabel.textContent = "Метод оплаты: ";
    const methodSelect = document.createElement("select");
    methodSelect.className = "form-select";
    const hotelIdInput = document.getElementById("HotelId");
    const hotelIdValue = parseInt(hotelIdInput.value);
    const url = "/Orders/GetTransactionMethods?hotelId=" + hotelIdValue;
    fetch(url)
        .then(response => response.json())
        .then(data => {
            // add each method as an option in the dropdown list
            data.forEach(method => {
                const methodOption = document.createElement("option");
                methodOption.textContent = method.name;
                methodSelect.appendChild(methodOption);
            });
            methodSelect.value = methodValue;
        })
        .catch(error => console.error(error));

    col1_1.appendChild(methodLabel);
    col2_1.appendChild(methodSelect);

    // Create an amount field with Bootstrap classes
    const amountLabel = document.createElement("label");
    amountLabel.textContent = "Сумма: ";
    const amountInput = document.createElement("input");
    amountInput.type = "number";
    amountInput.min = 0;
    amountInput.step = "any";
    amountInput.value = amountValue;
    amountInput.className = "form-control";
    col1_2.appendChild(amountLabel);
    col2_2.appendChild(amountInput);

    // Create a date picker for the transaction date with Bootstrap classes
    const dateLabel = document.createElement("label");
    dateLabel.textContent = "Дата транзакции: ";
    const datePicker = document.createElement("input");
    datePicker.type = "datetime-local";
    datePicker.value = dateValue;
    datePicker.className = "form-control";
    col1_3.appendChild(dateLabel);
    col2_3.appendChild(datePicker);

    // Create a comment field with Bootstrap classes
    const commentLabel = document.createElement("label");
    commentLabel.textContent = "Комментарий: ";
    const commentInput = document.createElement("textarea");
    commentInput.rows = 1;
    commentInput.value = commentValue;
    commentInput.className = "form-control";
    commentInput.setAttribute("oninput", "this.style.height = ''; this.style.height = this.scrollHeight + 'px'");
    commentLabel.appendChild(commentInput);
    newSection.appendChild(commentLabel);

    // Add the new section to the existing div element with ID "transactions"
    var transactionsDiv = null;
    if (isRefund) {
        transactionsDiv = document.getElementById("outcomeTransactions");
    }
    else {
        transactionsDiv = document.getElementById("incomeTransactions");
    }
    
    transactionsDiv.appendChild(newSection);
}





