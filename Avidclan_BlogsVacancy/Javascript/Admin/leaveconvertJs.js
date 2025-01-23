var calendar;
$(function () {

    $(".chosen-select").chosen({
        no_results_text: "Oops, nothing found!"
    })

    $("#txtFromDate").datepicker();
    $("#txtToDate").datepicker();

    SelectCalenderDate();
});

function SelectCalenderDate() {
    var calendarEl = document.getElementById('calendar');
    calendar = new FullCalendar.Calendar(calendarEl, {
        initialDate: new Date(),
        selectable: true,
        dateClick: function (info) {
            var responseDate = moment(info.dateStr).format('MM/DD/YYYY');
            var Eventlist = calendar.getEvents();
            if (Eventlist.length > 0) {
                var existingEvent = false;
                for (var i = 0; i < Eventlist.length; i++) {
                    if (moment(Eventlist[i]._instance.range.start).format('MM/DD/YYYY')
                        == responseDate) {
                        existingEvent = true;
                    }
                }
            }
            if (existingEvent) {
                return
            }
            else {

                showSpinner();

                var getDateday = new Date(responseDate)
                if (getDateday.getDay() === 6 || getDateday.getDay() === 0) {
                    return
                }
                $("#ApplyLeaveModel").show();
                $("#txtFromDate").val(responseDate);
                $("#txtToDate").val(responseDate);
                SetMinDate();
                GetListOfUser();
                GetReportingPerson();

                hideSpinner();
            }
        },
    });

    calendar.render();
}

function SetMinDate() {
    var start = $("#txtFromDate").datepicker('getDate');
    $("#txtToDate").datepicker("option", "minDate", start);
    DateChange();
}

function DateChange() {
    var start = $("#txtFromDate").datepicker('getDate');
    var end = $("#txtToDate").datepicker('getDate');
    var arr = new Array();
    var dt = new Date(start);
    while (dt <= end) {
        var WeekDay = dt.getDay();
        if (WeekDay != 6 && WeekDay != 0) {
            //arr.push(new Date(dt));
            var LeaveDetails = {
                LeaveDates: new Date(dt),
            }
            arr.push(LeaveDetails);
        }
        dt.setDate(dt.getDate() + 1);
    }

    BindLeaveTable(arr, false);
}

function BindLeaveTable(arr, showDeleteIcon) {
    $("#TotalLeaveDates").show();
    $("#LeaveDateTable .LeaveDatebody").html("");
    var count = 1
    for (var i = 0; i < arr.length; i++) {
        var addcontrols = "<tr id=" + count + ">" + count + ">"
        addcontrols += "<td>" + moment(arr[i].LeaveDates).format('MM/DD/YYYY') + "</td>" +
            "<td>" + '<input type="checkbox" id="WorkFromcheck_' + count + '" name="WorkFromHomecheck" onchange="WorkFromHomecheckboxchecked(' + count + ')">' + "</td>" +
            "<td>" + '<div class="HalfDaydiv_' + count + '">' +
            '<input type="radio" id="FirstHalf_' + count + '" name="HalfDayLeave_' + count + '" value="FirstHalf" onclick="checkRadioButton(FirstHalf_' + count + ')" data-checked="false"> <label for="firsthalf">First Half</label>&nbsp;&nbsp;' +
            '<input type="radio" id="SecondHalf_' + count + '" name="HalfDayLeave_' + count + '" value="SecondHalf" onclick="checkRadioButton(SecondHalf_' + count + ')" data-checked="false"> <label for= "secondhalf" > Second Half</label>' +
            '</div>' +
            "<td>" +
            '<label id="WFHHalfLeave_' + count + '" style="float:left;"></label>&nbsp;' +
            '<div class="WorkAndLeave_' + count + '" style="display:none;float:right;">' +
            ':&nbsp; <input type="radio" id="WFHHalfLeave" name="WFHHalfDay_' + count + '" value="HalfDayLeave" onchange="CheckWfhRadioButton(' + count + ')"> <label for="HalfDay">Leave</label>&nbsp;&nbsp;' +
            '<input type="radio" id="WorkFromOffice" name="WFHHalfDay_' + count + '" value="HalfDayWFO" onchange="CheckWfhRadioButton(' + count + ')"> <label for= "WFO" > WFO</label>' +
            '</div>' +
            '<label class="error" id="error_' + count + '" for="errormessage" style="display: none;color: red;padding-top: 5px;">Please select one of the options</label>'
        if (showDeleteIcon) {
            addcontrols += '<td><i class="mdi mdi-delete-outline delete-icon" id="LeaveDetailsId_' + arr[i].LeaveDetailsId + '" onclick=DeleteLeaveDetailsOfEmployee(' + arr[i].LeaveDetailsId + ')></i></td>'

        }
        addcontrols += "</td></tr>";


        $("#LeaveDateTable .LeaveDatebody").append(addcontrols);

        if (showDeleteIcon) {
            if (arr[i].Halfday != null && arr[i].Halfday != "") {
                $(".HalfDaydiv_" + count).show();
                $("input[name=HalfDayLeave_" + count + "][value=" + arr[i].Halfday + "]").prop("checked", true);
            }
        }

        count++;
    }
}

function GetListOfUser() {
    $.ajax({
        type: "GET",
        url: "/BlogVacancy/GetEmployeeList",
        contentType: 'application/json',
        success: function (result) {
            if (result != null) {
                $.each(result, function (index, item) {
                    var opt = new Option(item.FirstName, item.Id);
                    $('.cls-employee').append(opt);

                });
            }
        },
        error: function (result) {
            alert(result.responseText);
        }
    });
}

function GetReportingPerson() {
    $.ajax({
        type: "Get",
        url: "/BlogVacancy/GetListOfReportingPerson",
        success: function (result) {
            $.each(result, function (index, item) {
                $('.cls-reportingdrp').append('<option value="' + item.ReportingPersonEmail + '"> ' + item.ReportingPersonEmail + ' </option>');
                $('.cls-reportingdrp').trigger('chosen:updated');

            });
        },
        error: function (result) {
            alert(result.responseText);
        }
    });
}

function ResetModel() {
    console.clear();
    $("#LeaveId").val(0);
    $("#Employee").val("");
    $("#txtFromDate").val("");
    $("#txtToDate").val("");
    $("#LeaveDateTable .LeaveDatebody").html("");
    $("#TotalLeaveDates").hide();
    $("#LeaveReportingPersonId").val("");
    $('#LeaveReportingPersonId').trigger("chosen:updated");
    $("#txtReason").val('');
    $("#Reason-error").css("display", "none");
    $("#ApplyLeaveModel").hide();
}

//function AdminLeaveRequest() {
//    var Reason = $("#txtReason").val();
//    if (Reason.length == 0) {
//        if ($('#Reason-error').is(':visible')) {
//            return
//        }
//        else {
//            $("<label id='Reason-error' class='error' style=' color: #ff0000;font-weight: normal!important;' for='reason'>Please enter reasons for leave</label>").insertAfter("#txtReason");
//            return
//        }
//    }

//    var employeeSelect = $("#Employee").val();
//    if (employeeSelect.length == 0) {
//        if ($('#select-error').is(':visible')) {
//            return
//        }
//        else {
//            $("<label id='select-error' class='error' style=' color: #ff0000;font-weight: normal!important;' for='reason'>Please select employee name</label>").insertAfter("#Employee");
//            return
//        }
//    }
//    var LeaveDetails = new Array();
//    var exit = false;
//    $("#LeaveDateTable tbody tr").each(function () {
//        var row = $(this);
//        var Leave = {};

//        Leave.LeaveDate = row.find("td:eq(0)").text();
//        var WfhCheck = row.find("td:eq(1) input:checkbox").is(':checked');
//        if (WfhCheck) {
//            Leave.WorkFromHome = WfhCheck;
//            var halfdaycheckbox = row.find("td:eq(2) input[type='radio']:checked").is(':checked');
//            if (halfdaycheckbox) {
//                var HalfdayCriteria = row.find("td:eq(3) input[type='radio']:checked").is(':checked');
//                if (HalfdayCriteria) {
//                    var WFHHalfDayReason = row.find("td:eq(3) input[type='radio']:checked").val();
//                    if (WFHHalfDayReason.includes("WFO") == true) {
//                        Leave.WorkAndHalfLeave = false
//                    }
//                    else {
//                        Leave.WorkAndHalfLeave = true
//                    }
//                }
//                else {
//                    exit = true
//                    row.find("td:eq(3) .error").css("display", "block");
//                }
//            }
//        }
//        Leave.Halfday = row.find("td:eq(2) input[type='radio']:checked").val();
//        LeaveDetails.push(Leave);
//    })
//    if (exit) {
//        return;
//    }
//    var Leavedata = {
//        Id: $("#LeaveId").val(),
//        Fromdate: $("#txtFromDate").val(),
//        Todate: $("#txtToDate").val(),
//        Leaves: LeaveDetails,
//        ReportingPerson: $("#ReportingPersonId").val(),
//        ReasonForLeave: $("#txtReason").val(),
//        WorkFromHome: $("#WorkFromHome").prop('checked'),
//        UserId: $("#Employee").val()
//    }

//    showSpinner();
//    $.ajax({
//        type: "POST",
//        url: "/api/Admin/AdminLeaveRequest",
//        dataType: 'json',
//        contentType: 'application/json; charset=utf-8',
//        data: JSON.stringify(Leavedata),
//        success: function (result) {
//            alert(result)
//            ResetModel();
//            hideSpinner();
//        },
//        error: function (result) {
//            alert(result.responseText);
//            hideSpinner();
//        },
//    })
//}

function AdminLeaveRequest() {
    // Helper function to validate fields and display errors
    function validateField(selector, errorId, errorMessage) {
        const field = $(selector);
        if (!field.val() || field.val().length === 0) {
            if (!$(`#${errorId}`).is(':visible')) {
                $(`<label id='${errorId}' class='error' style='color: #ff0000; font-weight: normal!important;' for='${selector}'>${errorMessage}</label>`).insertAfter(selector);
            }
            return false;
        }
        $(`#${errorId}`).remove(); // Remove error if it exists
        return true;
    }

    // Validate Reason for Leave
    if (!validateField("#txtReason", "Reason-error", "Please enter reasons for leave")) {
        return;
    }

    // Validate Employee selection
    if (!validateField("#Employee", "select-error", "Please select employee name")) {
        return;
    }

    let LeaveDetails = [];
    let exit = false;

    // Process Leave Details table
    $("#LeaveDateTable tbody tr").each(function () {
        const row = $(this);
        const Leave = {};

        Leave.LeaveDate = row.find("td:eq(0)").text();

        const WfhCheck = row.find("td:eq(1) input:checkbox").is(':checked');
        if (WfhCheck) {
            Leave.WorkFromHome = WfhCheck;

            const halfdayCheckbox = row.find("td:eq(2) input[type='radio']:checked").is(':checked');
            if (halfdayCheckbox) {
                const halfdayCriteria = row.find("td:eq(3) input[type='radio']:checked").is(':checked');
                if (halfdayCriteria) {
                    const WFHHalfDayReason = row.find("td:eq(3) input[type='radio']:checked").val();
                    Leave.WorkAndHalfLeave = !WFHHalfDayReason.includes("WFO");
                } else {
                    exit = true;
                    row.find("td:eq(3) .error").css("display", "block");
                }
            }
        }

        Leave.Halfday = row.find("td:eq(2) input[type='radio']:checked").val();
        LeaveDetails.push(Leave);
    });

    if (exit) {
        return;
    }

    // Create Leave data object
    const Leavedata = {
        Id: $("#LeaveId").val(),
        Fromdate: $("#txtFromDate").val(),
        Todate: $("#txtToDate").val(),
        Leaves: LeaveDetails,
        ReportingPerson: $("#LeaveReportingPersonId").val(),
        ReasonForLeave: $("#txtReason").val(),
        WorkFromHome: $("#WorkFromHome").prop('checked'),
        UserId: $("#Employee").val()
    };

    // Show spinner and make API call
    showSpinner();
    $.ajax({
        type: "POST",
        url: "/api/Admin/AdminLeaveRequest",
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify(Leavedata),
        success: function (result) {
            alert(result);
            ResetModel();
            hideSpinner();
        },
        error: function (result) {
            alert(result.responseText);
            hideSpinner();
        }
    });
}


function CheckReason() {
    var reason = $("#txtReason").val();
    if (reason.length != 0) {
        $("#Reason-error").css("display", "none")
    }
    else {
        $("#Reason-error").css("display", "block")

    }
}