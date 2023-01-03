
$(document).ready(function () {
    $.ajax({
        url: "/Avidclan/GetCareerOption",
        type: "GET",
        dataType: "json",
        success: function (data) {
            $.each(data, function (index, item) {
                var opt = new Option(item.JobTitle, item.Id);
                $('.dropdownjobtitle').append(opt);
            });
        }
    })
});



