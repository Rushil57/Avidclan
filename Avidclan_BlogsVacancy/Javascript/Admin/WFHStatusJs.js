
var table;
$(function () {
    GetWfhList();
});

function GetWfhList() {
    showSpinner();
    var Statusid = $("#StatusId").val();
    $.ajax({
        url: "/Leave/GetWfhList",
        contentType: 'application/json',
        type: "GET",
        data: { wfhStatus: Statusid },
        success: function (data) {
            table = $('#WfhTable').DataTable({
                destroy: true,
                data: data,
                order: [[0, 'desc']],
                "columns": [
                    {
                        "data": "Id", visible: false
                    },
                    { "data": "FirstName" },
                    {
                        "data": "Fromdate",
                        "render": function (data) {
                            const dateValue = new Date(parseInt(data.substr(6)));
                            return moment(dateValue).format('DD/MM/YYYY');
                        }
                    },
                    {
                        "data": "Todate",
                        "render": function (data) {
                            const dateValue = new Date(parseInt(data.substr(6)));
                            return moment(dateValue).format('DD/MM/YYYY');
                        }
                    },
                    { "data": "WFHStatus" },
                    { "data": "WFHDays" }
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

$('#WfhTable tbody').on('click', 'tr', function () {
    var data = table.row($(this).closest('tr')).data();
    var id = data[Object.keys(data)[0]];

    showSpinner();
    $.ajax({
        url: "/Leave/GetWFHDetails",
        type: "GET",
        data: {
            Id: id,
        },
        contentType: 'application/json; charset=utf-8',
        success: function (data) {
            $("#WFHModel").show();
            var fromdate = moment(data[0].Fromdate).format('MM/DD/YYYY');
            var todate = moment(data[0].Todate).format('MM/DD/YYYY');
            $("#txtFromDate").val(fromdate);
            $("#txtToDate").val(todate);
            $("#WFHId").val(data[0].Id)
            $("#EmailId").val(data[0].EmailId)
            $("#FirstName").val(data[0].FirstName)
            var arr = new Array();
            $.each(data, function (key, val) {
                var WFHDetails = {
                    LeaveDates: val.WFHDates,
                    Halfday: val.Halfday
                }

                arr.push(WFHDetails);
            })
            BindWFHTable(arr);

            hideSpinner();
        }
    });
});

function BindWFHTable(arr) {
    $("#TotalLeaveDates").show();
    $("#LeaveDateTable .LeaveDatebody").html("");
    var count = 1
    for (var i = 0; i < arr.length; i++) {
        var addcontrols = "<tr id=" + count + ">" + count + ">"
        addcontrols += "<td>" + moment(arr[i].LeaveDates).format('MM/DD/YYYY') + "</td>" +
            "<td>" + '<input type="checkbox" id="HalfDaycheck_' + count + '" name="HalfDaycheck" onclick="return false">' + "</td>" +
            "<td>" + '<div class="HalfDaydiv_' + count + '"><input type="radio" id="FirstHalf" name="HalfDayLeave_' + count + '" value="FirstHalf" onclick="return false"> <label for="firsthalf">First Half</label>&nbsp;&nbsp;' +
            '<input type="radio" id="SecondHalf" name="HalfDayLeave_' + count + '" value="SecondHalf" onclick="return false"> <label for="secondhalf">Second Half</label></div>' + "</td>"
        "</tr>";
        $("#LeaveDateTable .LeaveDatebody").append(addcontrols);
        if (arr[i].Halfday != null && arr[i].Halfday != "") {
            $(".HalfDaydiv_" + count).show();
            $("#HalfDaycheck_" + count).prop("checked", true);
            $("input[name=HalfDayLeave_" + count + "][value=" + arr[i].Halfday + "]").prop("checked", true);

        }
        else {
            $(".HalfDaydiv_" + count).hide();
        }
        count++;
    }
}

function ResetWFHModel() {
    $("#WFHModel").hide();
    $("#WFHId").val(0)
    $("#txtFromDate").val("");
    $("#txtToDate").val("");
    $("#LeaveDateTable .LeaveDatebody").html("");
    $("#TotalLeaveDates").hide();
    $("#EmailId").val("");
    $("#FirstName").val("");
    GetWfhList();
}

function ApprovedOrRejectWFH(status) {
    if (status == "") {
        return;
    }
    showSpinner();
    var WFHDetails = new Array();
    $("#LeaveDateTable tbody tr").each(function () {
        var row = $(this);
        var WFH = {};
        WFH.WFHDate = row.find("td:eq(0)").text();
        var checked = row.find("td:eq(1) input:checkbox").is(':checked');
        if (checked) {
            var Radiochecked = row.find("td:eq(2) input[type='radio']:checked").is(':checked');
            if (Radiochecked) {
                WFH.Halfday = row.find("td:eq(2) input[type='radio']:checked").val();
            }
        }
        WFHDetails.push(WFH);
    })

    var WFHdata = {
        Id: $("#WFHId").val(),
        WFHStatus: status,
        EmailId: $("#EmailId").val(),
        FirstName: $("#FirstName").val(),
        WFH: WFHDetails
    }
    $.ajax({
        type: "POST",
        url: "/api/Admin/ReplyToWFHRequest",
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify(WFHdata),
        success: function (result) {
            alert(result);
            ResetWFHModel();
            hideSpinner();
        },
        error: function (result) {
            hideSpinner();
            alert(result.responsetext);
        },
    })

}