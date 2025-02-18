var calendar;

$(function () {
    $(".chosen-select").chosen({
        no_results_text: "Oops, nothing found!"
    })

    $("#txtFromDate").datepicker();
    $("#txtToDate").datepicker();
    $("#TotalLeaveDates").hide();

    showSpinner();
    $.when(
        SelectCalenderDate(),
        GetLeaveDates(),
        GetReportingPerson(),
        GetPastLeaveBalance(),
        delay(1500).then(GetTotalLeaveBalance)
    ).done(function () {
        hideSpinner();
    }).fail(function () {
        hideSpinner(); // Hide spinner even if there's an error
    });
});

function delay(time) {
    return new Promise(function (resolve) {
        setTimeout(resolve, time);
    });
}
function SelectCalenderDate() {
    var calendarEl = document.getElementById('calendar');
    calendar = new FullCalendar.Calendar(calendarEl, {
        initialDate: new Date(),
        selectable: true,
        dateClick: function (info) {
            var responseDate = moment(info.dateStr).format('MM/DD/YYYY');
            //var getDateday = new Date(responseDate)
            //if (getDateday.getDay() === 6 || getDateday.getDay() === 0) {
            //    return
            //}
            //$("#ApplyLeaveModel").show();
            //$("#txtFromDate").val(responseDate);
            //$("#txtToDate").val(responseDate);
            //SetMinDate();

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
                var getDateday = new Date(responseDate)
                if (getDateday.getDay() === 6 || getDateday.getDay() === 0) {
                    return
                }
                $("#ApplyLeaveModel").show();
                $("#txtFromDate").val(responseDate);
                $("#txtToDate").val(responseDate);
                SetMinDate();
            }
        },
        events: []
        //eventClick: function (info) {
        //    var StartTime = moment(info.event.start).format('MM/DD/YYYY');
        //    var Eventlist = calendar.getEvents();
        //    EditLeaves(info.event.id, StartTime);
        //}
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
            if (arr[i].Title == "WFH") {
                addcontrols += '<td><i class="mdi mdi-delete-outline delete-icon" id="WfhDetailsId_' + arr[i].WFHDetailId + '" onclick=DeleteWFHDetailsOfEmployee(' + arr[i].WFHDetailId + ')></i></td>'
            }
            else if (arr[i].Title == "LeaveAndWfh")
            {
                addcontrols += '<td><i class="mdi mdi-delete-outline delete-icon" onclick=DeleteWFHandLeaveDetailsOfEmployee(' + arr[i].WFHDetailsId + ',' + arr[i].LeaveDetailsId + ')></i></td>'
            }
            else {
                addcontrols += '<td><i class="mdi mdi-delete-outline delete-icon" id="LeaveDetailsId_' + arr[i].LeaveDetailsId + '" onclick=DeleteLeaveDetailsOfEmployee(' + arr[i].LeaveDetailsId + ')></i></td>'
            }
            
        }
        addcontrols += "</td></tr>";


        $("#LeaveDateTable .LeaveDatebody").append(addcontrols);

        if (showDeleteIcon) {
            if (arr[i].Halfday != null && arr[i].Halfday != "") {
                $(".HalfDaydiv_" + count).show();
                /*  $("#WorkFromcheck_" + count).prop("checked", true);*/
                $("input[name=HalfDayLeave_" + count + "][value=" + arr[i].Halfday + "]").prop("checked", true);
            }
            //if (arr[i].Title == "Leave" && arr[i].Halfday != null) {
            //    $("#WFHHalfLeave_" + count).text(arr[i].Halfday)
            //    $(".WorkAndLeave_" + count).show();
            //}
            if (arr[i].Title == "WFH") {
                $("#WorkFromcheck_" + count).prop("checked", true);
                if (arr[i].Halfday != null && arr[i].Halfday != "") {
                    $(".WorkAndLeave_" + count).show();
                    if (arr[i].Halfday == 'SecondHalf') {
                        $("#WFHHalfLeave_" + count).text("FirstHalf")
                    } else {
                        $("#WFHHalfLeave_" + count).text("SecondHalf")
                    }
                    $("input[name=WFHHalfDay_" + count + "][value=HalfDayWFO]").prop("checked", true);
                }
            }
            if (arr[i].Title == "LeaveAndWfh") {
                $("#WorkFromcheck_" + count).prop("checked", true);
                $("input[name=HalfDayLeave_" + count + "][value=" + arr[i].WFHHalfDay + "]").prop("checked", true);
                $("#WFHHalfLeave_" + count).show();
                $("#WFHHalfLeave_" + count).text(arr[i].LeaveHalfday)
                $(".WorkAndLeave_" + count).show();
                $("input[name=WFHHalfDay_" + count + "][value=HalfDayLeave]").prop("checked", true);
            }
        }
        //else {
        //    $(".HalfDaydiv_" + count).hide();
        //}
        count++;
    }
}

function WorkFromHomecheckboxchecked(id) {
    var checked = $("#WorkFromcheck_" + id).is(':checked');
    if (checked) {
        var halfdaycheckbox = $('input[type=radio][name=HalfDayLeave_' + id + ']').is(':checked')
        if (halfdaycheckbox) {
            var checkcheckboValue = $('input[name="HalfDayLeave_' + id + '"]:checked').val();
            if (checkcheckboValue != null) {
                if (checkcheckboValue == "SecondHalf") {
                    $("#WFHHalfLeave_" + id).text("First Half")
                }
                else {
                    $("#WFHHalfLeave_" + id).text("Second Half")
                }
            }
            $(".WorkAndLeave_" + id).show();
        }
    }
    else {
        $('input[type=radio][name=WFHHalfDay_' + id).prop("checked", false);
        $(".WorkAndLeave_" + id).hide();
        $("#WFHHalfLeave_" + id).text("")
    }
}

function SendLeaveRequest() {
    var Reason = $("#txtReason").val();
    if (Reason.length == 0) {
        if ($('#Reason-error').is(':visible')) {
            return
        }
        else {
            $("<label id='Reason-error' class='error' style=' color: #ff0000;font-weight: normal!important;' for='reason'>Please enter reasons for leave</label>").insertAfter("#txtReason");
            return
        }
    }
    var LeaveDetails = new Array();
    var exit = false;
    $("#LeaveDateTable tbody tr").each(function () {
        var row = $(this);
        var Leave = {};

        Leave.LeaveDate = row.find("td:eq(0)").text();
        var WfhCheck = row.find("td:eq(1) input:checkbox").is(':checked');
        if (WfhCheck) {
            Leave.WorkFromHome = WfhCheck;
            var halfdaycheckbox = row.find("td:eq(2) input[type='radio']:checked").is(':checked');
            if (halfdaycheckbox) {
                var HalfdayCriteria = row.find("td:eq(3) input[type='radio']:checked").is(':checked');
                if (HalfdayCriteria) {
                    var WFHHalfDayReason = row.find("td:eq(3) input[type='radio']:checked").val();
                    if (WFHHalfDayReason.includes("WFO") == true) {
                        Leave.WorkAndHalfLeave = false
                    }
                    else {
                        Leave.WorkAndHalfLeave = true
                    }
                }
                else {
                    exit = true
                    row.find("td:eq(3) .error").css("display", "block");
                }
            }
        }
        Leave.Halfday = row.find("td:eq(2) input[type='radio']:checked").val();
        LeaveDetails.push(Leave);
    })
    if (exit) {
        return;
    }
    var Leavedata = {
        Id: $("#LeaveId").val(),
        Fromdate: $("#txtFromDate").val(),
        Todate: $("#txtToDate").val(),
        Leaves: LeaveDetails,
        ReportingPerson: $("#ReportingPersonId").val(),
        ReasonForLeave: $("#txtReason").val(),
        WorkFromHome: $("#WorkFromHome").prop('checked'),
        WFHId : $("#WfhId").val() 
    }
    showSpinner();
    $.ajax({
        type: "POST",
        url: "/api/Admin/RequestForLeave",
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify(Leavedata),
        success: function (result) {
            /*alert("Request Sent..");*/
            alert(result)
            ResetModel();
            GetPastLeaveBalance();
            delay(1500).then(function () {
                GetTotalLeaveBalance();

                // Ensure spinner is hidden after everything completes
                setTimeout(function () {
                    hideSpinner();
                }, 1000);  // Slight delay after all tasks
            });
        },
        error: function (result) {
            alert(result.responseText);
            hideSpinner();
        },
    })
}

function ResetModel() {
    console.clear();
    $("#ApplyLeaveModel").hide();
    $("#LeaveId").val(0)
    $("#txtFromDate").val("");
    $("#txtToDate").val("");
    $("#LeaveDateTable .LeaveDatebody").html("");
    $("#TotalLeaveDates").hide();
    $("#ReportingPersonId").val("");
    $('#ReportingPersonId').trigger("chosen:updated");
    $("#txtReason").val('');
    $("#Reason-error").css("display", "none");
    GetLeaveDates();
}

function GetLeaveDates() {
    $.ajax({
        url: "/Leave/LeaveDates",
        contentType: 'application/json',
        type: "GET",
        success: function (data) {
            calendar.removeAllEvents();
            $.each(data.leavelist, function (key, val) {
                // Add the "leave-approved" class if LeaveStatus is not "Pending"
                var className = val.LeaveStatus != "Pending" ? "leave-approved" : "";
                var fromdate = moment(val.LeaveDate).format('YYYY-MM-DD');
                if (val.Halfday != null && val.Halfday != "") {
                    if (val.Halfday == "FirstHalf") {
                        calendar.addEvent({
                            id: "Leave_" + val.Id,
                            title: val.Halfday,
                            start: fromdate,
                            className: className + " firsthalf",
                        }), true;
                    }
                    else {
                        calendar.addEvent({
                            id: "Leave_" + val.Id,
                            title: val.Halfday,
                            start: fromdate,
                            className: className + " secondhalf",
                        }), true;
                    }
                }
                else {
                    calendar.addEvent({
                        id: "Leave_" + val.Id,
                        title: 'Leave',
                        start: fromdate,
                        className: className,
                        allDay: true
                    });
                }

            });

            $.each(data.wfhdetaillist, function (key, val) {

                // Add the "leave-approved" class if LeaveStatus is not "Pending"
                var className = val.WFHStatus != "Pending" ? "leave-approved" : "";
                if (val.LeaveId == 0) {
                    val.LeaveId = val.Id;
                }
                var fromdate = moment(val.LeaveDate).format('YYYY-MM-DD');
                if (val.HalfDay != null && val.HalfDay != "") {
                    if (val.HalfDay == "FirstHalf") {
                        calendar.addEvent({
                            id: "WFH_" + val.Id,
                            title: "WFH " + val.HalfDay,
                            start: fromdate,
                            className: className + " firsthalf",
                        }), true;
                    }
                    else {
                        calendar.addEvent({
                            id: "WFH_" + val.Id,
                            title: "WFH " + val.HalfDay,
                            start: fromdate,
                            className: className + " secondhalf",
                        }), true;
                    }
                }
                else {
                    calendar.addEvent({
                        id: "WFH_" +val.Id,
                        title: 'Work From Home',
                        start: fromdate,
                        className: className,
                        allDay: true
                    });
                }
            });
        },
        error: function (result) {
            alert(result.responseText);
        }
    });
}

function EditLeaves(id, startDate) {

    var diffDays = checkedEditedLeavesday(startDate);
    if (diffDays > 2) {
        OpenEmployeeEditDetails(id)
    } else {
        // alert("Please inform to management team for changes in leave.Thanks!")
        OpenEmployeeEditDetails(id)
    }


}

function checkRadioButton(data) {
    var id = data.id.split("_")[1];

    var checked = $("input[type=radio][name=HalfDayLeave_" + id + "]");
    if (checked) {
        $("#error_" + id).css("display", "none");
    }

    var checkRadio = $(data).attr('data-checked');
    if (checkRadio == "true") {
        $(data).attr('data-checked', "false")
        $("#" + data.id).prop('checked', false);
    }
    else {
        $(data).attr('data-checked', "true")
    }

    if (data.value == 'SecondHalf') {
        $("#FirstHalf_" + id).attr('data-checked', "false")
    }
    else {
        $("#SecondHalf_" + id).attr('data-checked', "false")

    }
    var wfhcheckboxccheck = $('#WorkFromcheck_' + id).is(":checked")
    if (wfhcheckboxccheck) {
        var checkhalfdayradiobutton = $(data).attr('data-checked');
        if (checkhalfdayradiobutton == "true") {
            $(".WorkAndLeave_" + id).css("display", "block")
            if (data.value == "SecondHalf") {
                $("#WFHHalfLeave_" + id).text("First Half")
            }
            else {
                $("#WFHHalfLeave_" + id).text("Second Half")
            }

        }
        else {
            $('input[type=radio][name=WFHHalfDay_' + id).prop("checked", false);
            $(".WorkAndLeave_" + id).hide();
            $("#WFHHalfLeave_" + id).text("")
        }
    }

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

function CheckWfhRadioButton(id) {
    var checked = $("input[type=radio][name=WFHHalfDay_" + id + "]").is(":checked");
    if (checked) {
        $("#error_" + id).css("display", "none");
    }
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

function GetPastLeaveBalance() {
    $.ajax({
        url: "/Leave/GetTotalPastBalanaceList",
        contentType: 'application/json',
        type: "GET",
        success: function (data) {
            if (data.length > 0) {
                $("#PersonalLeave").val(data[0].PersonalLeave);
                $("#SickLeave").val(data[0].SickLeave);
            }
        },
        error: function (result) {
            alert(result.responseText);
        }
    });
}

function GetTotalLeaveBalance() {
    $.ajax({
        url: "/Leave/GetTotalLeaveBalanaceList",
        contentType: 'application/json',
        type: "GET",
        success: function (data) {
            var userOnbreak = data.UserBreakData.OnBreak;
            if (!userOnbreak) {
                var sickLeave = TotalSickLeaveCount(data.TotalLeaveList.SickLeave, userOnbreak);
                var paidleave = TotalPersonalLeaveCount(data.TotalLeaveList.PersonalLeave, userOnbreak);
                SaveSickAndPaidLeave(sickLeave, paidleave);
            }

        },
        error: function (result) {
            alert(result.responseText);
        }
    });
}

function TotalSickLeaveCount(sickleave, userBreak) {

    var SickLeavePerMonth = 0.5;
    var TotalMonths = 12;

    var OverAllSickLeave = 12 * 0.5

    var getJoiningDate = $('#JoiningDate').val();
    getJoiningDate = new Date(getJoiningDate);
    var getProbationMonth = $('#ProbationMonth').val();
    getProbationMonth = parseInt(getProbationMonth)

    var CurrentDate = new Date();
    var CurrentYear = CurrentDate.getFullYear();

    var months = CurrentDate.getMonth() - getJoiningDate.getMonth() +
        (12 * (CurrentDate.getFullYear() - getJoiningDate.getFullYear()))

    // check probation month
    if (months >= getProbationMonth) {
        if (sickleave > 0) {
            OverAllSickLeave = OverAllSickLeave - sickleave;
            return OverAllSickLeave
        } else {
            return OverAllSickLeave;
        }
    } else {
        return OverAllSickLeave = 0.0
    }
}

function TotalPersonalLeaveCount(personalLeave, userBreak) {
    var getJoiningDate = $('#JoiningDate').val();
    getJoiningDate = new Date(getJoiningDate);
    var getProbationMonth = $('#ProbationMonth').val();
    getProbationMonth = parseInt(getProbationMonth)

    var CurrentDate = new Date();
    var CurrentYear = CurrentDate.getFullYear();

    var months = CurrentDate.getMonth() - getJoiningDate.getMonth() +
        (12 * (CurrentDate.getFullYear() - getJoiningDate.getFullYear()))

    var TotalPersonalLeave;
    if (months >= getProbationMonth) {
        var JoiningDateAfterProbation = new Date(getJoiningDate.setMonth(getJoiningDate.getMonth() + getProbationMonth));
        var JoiningYear = JoiningDateAfterProbation.getFullYear();
        if (JoiningYear == CurrentYear) {
            // checking month wise
            if (JoiningDateAfterProbation > CurrentDate) {
                TotalPersonalLeave = 0;
                return TotalPersonalLeave;
            }
            months = (CurrentDate.getMonth() - JoiningDateAfterProbation.getMonth() + 1) +
                (12 * (CurrentDate.getFullYear() - getJoiningDate.getFullYear()))
            TotalPersonalLeave = months - personalLeave;
            if (TotalPersonalLeave < 0 && TotalPersonalLeave != -0.5) {
                TotalPersonalLeave = 0;
            } else {
                TotalPersonalLeave = TotalPersonalLeave;
            }
            return TotalPersonalLeave;
        }


        var Pastpersonalleave = $("#PersonalLeave").val();
        if (Pastpersonalleave == '') {
            Pastpersonalleave = 0.0;
        }
        months = (CurrentDate.getMonth() - JoiningDateAfterProbation.getMonth() + 1) +
            (12 * (CurrentDate.getFullYear() - getJoiningDate.getFullYear()))
        var CurrentDate = new Date();
        var CurrentMonth = CurrentDate.getMonth() + 1;
        TotalPersonalLeave = parseFloat(Pastpersonalleave) + (months - personalLeave)
        if (TotalPersonalLeave < 0 && TotalPersonalLeave != -0.5) {
            TotalPersonalLeave = 0;
        } else if (TotalPersonalLeave == -0.5) {
            TotalPersonalLeave = 0.5;
        }
    }
    else {
        TotalPersonalLeave = 0.0;
    }

    return TotalPersonalLeave;
}

function SaveSickAndPaidLeave(sickleave, paidleave) {
    var data = {
        SickLeave: sickleave,
        PaidLeave: paidleave
    }
    $.ajax({
        type: "POST",
        url: "/Leave/SaveSickAndPaidLeave",
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify(data),
        success: function (result) {
            if (result.message != "Saved Successfully") {
                alert(result.message);
            }
        },
        error: function (result) {
            console.log(xhr.responseText);  // Log the raw response text
            alert("Error: " + error);
        },
    })
}


function checkedEditedLeavesday(startDate) {
    var today = new Date();
    var start = new Date(startDate); // Convert startDate to a Date object

    // Calculate the difference in milliseconds
    var differenceInMilliseconds = start - today;

    // Convert milliseconds to days
    var differenceInDays = Math.floor(differenceInMilliseconds / (1000 * 60 * 60 * 24));

    return differenceInDays;
}
function OpenEmployeeEditDetails(leaveId) {

    if (leaveId == 0) {
        return
    }
    showSpinner();

    $.ajax({
        url: "/Leave/GetEmployeeLeaveDetails",
        type: "GET",
        data: {
            Id: leaveId,
        },
        contentType: 'application/json; charset=utf-8',
        success: function (data) {
            // Determine which dataset to use
            var isLeave = data.leavelist.length > 0;
            var record = isLeave ? data.leavelist[0] : data.wfhdetaillist[0];
            var fromdate = moment(record.Fromdate).format('MM/DD/YYYY');
            var todate = moment(record.Todate).format('MM/DD/YYYY');

            // Check leave edit restrictions if applicable
            //if (isLeave) {
            //    var differenceDay = checkedEditedLeavesday(fromdate);
            //    if (differenceDay <= 2) {
            //        alert("Please inform the management team about changes in leave. Thanks!");
            //        hideSpinner();
            //        return;
            //    }
            //}

            // Populate modal fields
            $("#ApplyLeaveModel").show();
            $("#txtFromDate").val(fromdate);
            $("#txtToDate").val(todate);
            isLeave ? $("#LeaveId").val(record.Id) : $("#WfhId").val(record.WFHId) 
            $("#EmailId").val(record.EmailId);
            $("#FirstName").val(record.FirstName);
            $("#txtReason").val(isLeave ? record.ReasonForLeave : record.ReasonForWFH);
            SetMinDate();
            var arr = [];

            // Step 1: Push all unique leave and WFH records to `arr`
            $.each(data.leavelist, function (key, val) {
                var LeaveDetails = {
                    Title: "Leave",
                    LeaveDates: val.LeaveDates,
                    Halfday: val.Halfday,
                    LeaveDetailsId: val.LeaveApplicationId
                }
                arr.push(LeaveDetails);
            });

            $.each(data.wfhdetaillist, function (key, val) {
                var WFHDetails = {
                    Title: "WFH",
                    LeaveDates: val.WFHDates,
                    Halfday: val.Halfday,
                    WFHDetailId: val.WFHDetailId
                }
                arr.push(WFHDetails);
            });

            // Step 2: Find common dates between leaveDates and wfhDates
            var leaveDates = data.leavelist.map(function (val) { return val.LeaveDates; });
            var wfhDates = data.wfhdetaillist.map(function (val) { return val.WFHDates; });
            var commonDates = leaveDates.filter(function (date) { return wfhDates.includes(date); });

            // Step 3: If common dates exist, consolidate records for those dates
            if (commonDates.length > 0) {
                console.log("Common Dates:", commonDates);

                // Get details for the common dates from each list
                var commonLeaveDetails = data.leavelist.filter(function (val) {
                    return commonDates.includes(val.LeaveDates);
                });

                var commonWFHDetails = data.wfhdetaillist.filter(function (val) {
                    return commonDates.includes(val.WFHDates);
                });

                // Consolidate common details and push to `arr`
                $.each(commonDates, function (index, date) {
                    var leaveDetail = commonLeaveDetails.find(function (leave) {
                        return leave.LeaveDates === date;
                    });
                    var wfhDetail = commonWFHDetails.find(function (wfh) {
                        return wfh.WFHDates === date;
                    });

                    // Check if both leave and WFH details exist for this date
                    if (leaveDetail && wfhDetail) {
                        arr = getUniqueRecords(arr, 'LeaveDates');

                        var LeaveAndWorkFromDetails = {
                            Title: "LeaveAndWfh",
                            LeaveDates: date,
                            LeaveHalfday: leaveDetail.Halfday,
                            LeaveDetailsId: leaveDetail.LeaveApplicationId,
                            WFHDetailsId: wfhDetail.WFHDetailId,
                            WFHHalfDay: wfhDetail.Halfday
                        };
                        arr.push(LeaveAndWorkFromDetails);
                    }
                });

                console.log("Consolidated Common Details:", arr);

            } else {
                console.log("No common dates found.");
            }

            BindLeaveTable(arr, true);

            hideSpinner();
        },
        error: function (result) {
            console.log(xhr.responseText);  // Log the raw response text
            alert("Error: " + error);
            hideSpinner();
        },
    });
}

function getUniqueRecords(arr, property) {
    const countMap = arr.reduce((acc, item) => {
        acc[item[property]] = (acc[item[property]] || 0) + 1;
        return acc;
    }, {});

    return arr.filter(item => countMap[item[property]] === 1);
}


function DeleteLeaveDetailsOfEmployee(leaveDetailsId) {
    if (confirm('Are you sure you want to delete leave?')) {
        showSpinner();
        var leaveId = $("#LeaveId").val();
        $.ajax({
            url: "/Leave/DeleteLeaveDetailsApplication",
            type: "GET",
            data: {
                LeaveApplicationId: leaveDetailsId,
                LeaveId: leaveId
            },
            contentType: 'application/json; charset=utf-8',
            success: function (data) {
                alert(data);
                OpenEmployeeEditDetails("Leave_" + leaveId);

                hideSpinner();
            },
            error: function (result) {
                console.log(result.responseText);  // Log the raw response text
                alert("Error: " + result);
                hideSpinner();
            },
        });
    }
}

function DeleteWFHDetailsOfEmployee(wfhdetailId) {
    if (confirm('Are you sure you want to delete Wfh?')) {
        showSpinner();
        $.ajax({
            url: "/Leave/DeleteWFHDetailsOfEmployee",
            type: "GET",
            data: {
                wfhDetailId: wfhdetailId
            },
            contentType: 'application/json; charset=utf-8',
            success: function (data) {
                alert(data);
                var leaveId = $("#LeaveId").val();
                OpenEmployeeEditDetails("WFH_" + leaveId);

                hideSpinner();
            },
            error: function (result) {
                console.log(result.responseText);  // Log the raw response text
                alert("Error: " + result);
                hideSpinner();
            },
        });
    }
}

function DeleteWFHandLeaveDetailsOfEmployee(wfhdetailId, leaveApplicationId) {
    if (confirm('Are you sure you want to delete leave?')) {
        showSpinner();
        DeleteLeaveDetailsApplication(leaveApplicationId);
        DeleteWFHDetailsOfEmployee(wfhdetailId);
        hideSpinner();
    }
}