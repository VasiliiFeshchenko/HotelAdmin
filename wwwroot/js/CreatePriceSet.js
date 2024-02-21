function SubmitButtonClick()
{
    category = document.getElementById('Category').value;
    start = document.getElementById('Start').value;
    end = document.getElementById('End').value;
    price = document.getElementById('Price').value;
    comment = document.getElementById('Comment').value;
    hotelId = document.getElementById('BookableObject_HotelId').value;

    if (category == "") {
        category = " ";
    }
    if (price == "") {
        price = 0;
    }
    if (comment == "") {
        comment = " ";
    }
    if (start == "") {
        start = "0001-01-01"
    }
    if (end == "") {
        end = "0001-01-01"
    }
 
    const data = {
        Category: category,
        Start: start,
        End: end,
        Price: price,
        Comment: comment,
        HotelId: hotelId
    };

    // Send the AJAX request
    const xhr = new XMLHttpRequest();
    xhr.open('POST', '/SetPrice/CreateRoot');
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
