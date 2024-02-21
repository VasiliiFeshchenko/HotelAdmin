function DeleteOrder(orderId,hotelId)
{
    const confirmation = confirm("Вы действительно хотите УДАЛИТЬ этот заказ?");
    if (confirmation)
    {
        const params = new URLSearchParams();
        params.append('orderId', orderId);
        params.append('hotelId', hotelId);
        const url = `/Orders/Delete?${params.toString()}`;
        window.location.replace(url);
    }
    else
    {
        
    }
}
function CancelOrder(orderId, hotelId)
{
    const confirmation = confirm("Вы действительно хотите ОТМЕНИТЬ этот заказ?");
    if (confirmation) {
        const params = new URLSearchParams();
        params.append('orderId', orderId);
        params.append('hotelId', hotelId);
        const url = `/Orders/Cancel?${params.toString()}`;
        window.location.replace(url);
    }
    else {

    }
}
function RecoverOrder(orderId, hotelId) {
    const confirmation = confirm("Вы действительно хотите ВОССТАНОВИТЬ этот заказ?");
    if (confirmation) {
        const params = new URLSearchParams();
        params.append('orderId', orderId);
        params.append('hotelId', hotelId);
        const url = `/Orders/Recover?${params.toString()}`;


        const xhr = new XMLHttpRequest();
        xhr.open('POST', url);
        xhr.onload = function () {
            if (xhr.status === 200) {
                // Check if the response is an HTML file
                if (xhr.responseText.includes("Orders")) {
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
        xhr.send();

    }
    else {

    }
}