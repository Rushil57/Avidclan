var calendar;
$(function () {
    SelectCalenderDate(),
    GetAllEmployeesLeaveDates()

});

function SelectCalenderDate() {
    var calendarEl = document.getElementById('calendar');
    calendar = new FullCalendar.Calendar(calendarEl, {
        initialDate: new Date(),
        selectable: true,
        eventClick: function (info) {
            EmployeeLeaveDetails(info.event.id);
        }
    });

    calendar.render();
}

function GetAllEmployeesLeaveDates() {

    showSpinner();
    $.ajax({
        url: "/BlogVacancy/GetAllEmployeesLeaveDates",
        contentType: 'application/json',
        type: "GET",
        success: function (data) {
            calendar.removeAllEvents();
            $.each(data, function (key, val) {
                // Add the "leave-approved" class if LeaveStatus is not "Pending"
                var className = val.LeaveStatus != "Pending" ? "leave-approved" : "";
                var fromdate = moment(val.LeaveDate).format('YYYY-MM-DD');
                if (val.Halfday != null && val.Halfday != "") {
                    if (val.Halfday == "FirstHalf") {
                        calendar.addEvent({
                            id: val.LeaveApplicationId,
                            title: val.FirstName + "'s " + val.Halfday,
                            start: fromdate,
                            className: className,
                        }), true;
                    }
                    else {
                        calendar.addEvent({
                            id: val.LeaveApplicationId,
                            title: val.FirstName + "'s " + val.Halfday,
                            start: fromdate,
                            className: className,
                        }), true;
                    }
                }
                else {
                    calendar.addEvent({
                        id: val.LeaveApplicationId,
                        title: val.FirstName + "'s " + 'Leave',
                        start: fromdate,
                        className: className,
                        allDay: true
                    });
                }

            });

            hideSpinner();
        },
        error: function (result) {
            alert(result.responseText);
            hideSpinner();
        }
    });
}

function EmployeeLeaveDetails(leaveId) {
    showSpinner();
   
    $.ajax({
        url: "/Leave/GetLeaveDetails",
        type: "GET",
        data: {
            Id: leaveId
        },
        contentType: 'application/json; charset=utf-8',
        success: function (data) {
            $("#ApplyLeaveModel").show();
            var fromdate = moment(data[0].Fromdate).format('MM/DD/YYYY');
            var todate = moment(data[0].Todate).format('MM/DD/YYYY');
            $("#txtFromDate").val(fromdate);
            $("#txtToDate").val(todate);
            $("#LeaveId").val(data[0].Id)
            $("#EmailId").val(data[0].EmailId)
            $("#FirstName").val(data[0].FirstName)
            var arr = new Array();
            $.each(data, function (key, val) {
                var LeaveDetails = {
                    LeaveDates: val.LeaveDates,
                    Halfday: val.Halfday
                }

                arr.push(LeaveDetails);
            })
            BindLeaveTable(arr);
            hideSpinner();
        },
        error: function (result) {
            console.log(result.responseText);  // Log the raw response text
            alert("Error: " + result);
            hideSpinner();
        },
    });
}

function BindLeaveTable(arr) {
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

function ResetModel() {
    $("#ApplyLeaveModel").hide();
    $("#LeaveId").val(0)
    $("#txtFromDate").val("");
    $("#txtToDate").val("");
    $("#LeaveDateTable .LeaveDatebody").html("");
    $("#TotalLeaveDates").hide();
    $("#EmailId").val("");
    $("#FirstName").val("");
}