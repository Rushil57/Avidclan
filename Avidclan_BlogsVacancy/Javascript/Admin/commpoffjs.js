$(function () {
    GetComppOffDataList()
})

function GetComppOffDataList() {
    showSpinner();
    var Statusid = $("#StatusId").val();
    $.ajax({
        url: "/Leave/GetComppOffDataList",
        contentType: 'application/json',
        type: "GET",
        success: function (data) {
            table = $('#CommpOffTable').DataTable({
                destroy: true,
                data: data,
                order: [[0, 'desc']],
                "columns": [
                    {"data": "Id", visible: false},
                    { "data": "FirstName" },
                    {
                        "data": "NumberOfDays",
                        "render": function (data, type, row) {
                            return "<span title='" + data + "'>" + data + "</span>";
                        }
                    },
                    {
                        "data": "Reason",
                        "render": function (data, type, row) {
                            var reason = data.length > 35 ? data.substring(0, 35) + "..." : data;
                            return "<span title='" + data + "'>" + reason + "</span>";
                        }
                    },
                    { "data": "Status" }
                ]
            });

            hideSpinner();
        },
        error: function (result) {
            hideSpinner();
            alert(result.responsetext);
        }
    });
}

$('#CommpOffTable tbody').on('click', 'tr', function () {
    var data = table.row($(this).closest('tr')).data();
    var id = data[Object.keys(data)[0]];
    $.ajax({
        url: "/Leave/GetCompOfDetails",
        type: "GET",
        data: {
            Id: id,
        },
        contentType: 'application/json; charset=utf-8',
        success: function (data) {
            $("#CompensationDetailsModel").show();
            $("#txtNumberOfDays").val(data.NumberOfDays)
            $("#Reason").val(data.Reason)
            $("#CompensationId").val(data.Id)
            $("#FirstName").val(data.FirstName)
            $("#EmailId").val(data.EmailId)
        }
    });
});

function ResetModel() {
    $("#txtNumberOfDays").val("")
    $("#Reason").val("")
    $("#CompensationId").val(0);
    $("#FirstName").val("");
    $("#EmailId").val("");
    $("#CompensationDetailsModel").hide();
}

function ApprovedOrRejectRequest(status) {
    var Leavedata = {
        Id: $("#CompensationId").val(),
        Status: status,
        Reason: $("#Reason").val(),
        NumberOfDays: $("#txtNumberOfDays").val(),
        FirstName : $("#FirstName").val(),
        EmailId : $("#EmailId").val()
    }
    showSpinner();
    $.ajax({
        type: "POST",
        url: "/api/Admin/ReplyToCompOffRequest",
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify(Leavedata),
        success: function (result) {
            alert(result);
            ResetModel();
            GetComppOffDataList();
            hideSpinner();
        },
        error: function (result) {
            alert(result.responsetext);
            hideSpinner();
        },
    })
}