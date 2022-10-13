$(document).ready(function () {
    // actions binding on buttons
    $(document).on('click', '#refreshJobOffersBtn', function (event) {
        refreshJobOffers();
    });
});


function refreshJobOffers() {
    var spanInfo = $("#spanInfo");
    spanInfo.html('Work in progress... Please wait :)');
    var refreshJobOffersBtn = $("#refreshJobOffersBtn");
    refreshJobOffersBtn.prop("disabled", true);
    
    $.ajax({
        url: "/Home/RefreshJobOffers",
        type: "POST",
        data: {  __RequestVerificationToken: document.querySelector("input[name=__RequestVerificationToken]").value },
        success: function (data) {
            if (data.success) {
                spanInfo.html('Work completed :) : ' + data.rowsAddedNumber + ' rows(s) added and ' + data.rowsUpdatedNumber + ' rows(s) updated. ');
            }
            else {
                spanInfo.html('An error occured : ' + data.error);
            }
            refreshJobOffersBtn.prop("disabled", false);
        },
        error: function (xhr, error) {
            console.log(" : error" + error);
            refreshJobOffersBtn.prop("disabled", false);
            spanInfo.html('An error occured : '+error);
        }
    });
  
}
