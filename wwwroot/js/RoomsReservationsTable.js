function DeleteClosureSet(closureSetId, hotelId) {
    const confirmation = confirm("Вы действительно хотите открыть этот номер?");
    if (confirmation) {
        const params = new URLSearchParams();
        params.append('closureSetId', closureSetId);
        params.append('hotelId', hotelId);
        const url = `/ClosureSets/Delete?${params.toString()}`;
        window.location.replace(url);
    }
    else {

    }
}
function LoadNewDates(hotelId) {
    const start = document.getElementById("start").value;
    const end = document.getElementById("end").value;

    const params = new URLSearchParams();
    params.append('hotelId', hotelId);
    params.append('start', start);
    params.append('end', end);
    const url = `/RoomsReservationsTable/Index?${params.toString()}`;
    window.location.replace(url);


}