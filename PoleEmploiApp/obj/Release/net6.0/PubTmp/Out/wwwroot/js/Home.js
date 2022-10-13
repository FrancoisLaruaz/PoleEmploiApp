$(document).ready(function () {
    // actions binding on buttons
    $(document).on('click', '#refreshJobOffersBtn', function (event) {
        refreshJobOffers();
    });
});


function refreshJobOffers() {
    $.ajax({
        url: "/Home/RefreshJobOffers",
        type: "POST",
        data: {  __RequestVerificationToken: document.querySelector("input[name=__RequestVerificationToken]").value },
        success: function (data) {
            if (data.success) {

            }
            else {
                alert(data.comment);
            }
        },
        error: function (xhr, error) {
            console.log(" : error" + error);
            alert(error);
        }
    });
}
