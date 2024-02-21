function SubmitButtonClick() {
    bookableObject = document.getElementById('BookableObject').value;
    start = document.getElementById('Start').value;
    end = document.getElementById('End').value;
    comment = document.getElementById('Comment').value;
    hotelId = document.getElementById('BookableObject_HotelId').value;

    if (bookableObject == "") {
        telephoneNumber = " ";
    }
    if (comment.trim()=="") {
        comment = "Номер Закрыт";
    }
    if (start == "") {
        start = "0001-01-01"
    }
    if (end == "") {
        end = "0001-01-01"
    }

    const data = {
        BookableObject: bookableObject,
        Start: start,
        End: end,
        Comment: comment,
        HotelId: hotelId
    };

    // Send the AJAX request
    const xhr = new XMLHttpRequest();
    xhr.open('POST', '/ClosureSets/CreateRoot');
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