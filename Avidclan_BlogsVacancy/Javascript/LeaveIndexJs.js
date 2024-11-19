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
        events: [],
        //eventClick: function (info) {
        //    var StartTime = moment(info.event.start).format('MM/DD/YYYY');
        //    EditLeaves(info.event.id,StartTime);
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

    BindLeaveTable(arr);
}

function BindLeaveTable(arr) {
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
            '<label class="error" id="error_' + count + '" for="errormessage" style="display: none;color: red;padding-top: 5px;">Please select one of the options</label>' +
            "</td>"

        "</tr >";


        $("#LeaveDateTable .LeaveDatebody").append(addcontrols);
        //if (arr[i].Halfday != null && arr[i].Halfday != "") {
        //    $(".HalfDaydiv_" + count).show();
        //    $("#HalfDaycheck_"+ count).prop("checked", true);
        //    $("input[name=HalfDayLeave_" + count + "][value=" + arr[i].Halfday +"]").prop("checked", true);

        //}
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
        WorkFromHome: $("#WorkFromHome").prop('checked')
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
                var fromdate = moment(val.LeaveDate).format('YYYY-MM-DD');
                if (val.Halfday != null && val.Halfday != "") {
                    if (val.Halfday == "FirstHalf") {
                        calendar.addEvent({
                            id: val.Id,
                            title: val.Halfday,
                            start: fromdate,
                            className: "firsthalf",
                        }), true;
                    }
                    else {
                        calendar.addEvent({
                            id: val.Id,
                            title: val.Halfday,
                            start: fromdate,
                            className: "secondhalf",
                        }), true;
                    }
                }
                else {
                    calendar.addEvent({
                        id: val.Id,
                        title: 'Leave',
                        start: fromdate,
                        allDay: true
                    });
                }

            });

            $.each(data.wfhdetaillist, function (key, val) {
                var fromdate = moment(val.LeaveDate).format('YYYY-MM-DD');
                if (val.HalfDay != null && val.HalfDay != "") {
                    if (val.HalfDay == "FirstHalf") {
                        calendar.addEvent({
                            id: val.Id,
                            title: "WFH " + val.HalfDay,
                            start: fromdate,
                            className: "Wfhfirsthalf",
                        }), true;
                    }
                    else {
                        calendar.addEvent({
                            id: val.Id,
                            title: "WFH " + val.HalfDay,
                            start: fromdate,
                            className: "Wfhsecondhalf",
                        }), true;
                    }
                }
                else {
                    calendar.addEvent({
                        id: val.Id,
                        title: 'Work From Home',
                        start: fromdate,
                        className: "Wfh",
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

function EditLeaves(id, date) {
    var TodayDate = new Date();
    TodayDate = moment(TodayDate).format('MM/DD/YYYY');
    if (date >= TodayDate) {
        $.ajax({
            url: "Leave/GetLeaveDetails",
            type: "GET",
            data: {
                Id: id,
            },
            contentType: 'application/json; charset=utf-8',
            success: function (data) {
                $("#ApplyLeaveModel").show();
                var fromdate = moment(data[0].Fromdate).format('MM/DD/YYYY');
                var todate = moment(data[0].Todate).format('MM/DD/YYYY');
                $("#txtFromDate").val(fromdate);
                $("#txtToDate").val(todate);
                $("#LeaveId").val(data[0].Id)
                var arr = new Array();
                $.each(data, function (key, val) {
                    var LeaveDetails = {
                        LeaveDates: val.LeaveDates,
                        Halfday: val.Halfday
                    }

                    arr.push(LeaveDetails);
                })
                BindLeaveTable(arr);
            }
        });
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

    var getJoiningDate = $('#JoiningDate').val();
    getJoiningDate = new Date(getJoiningDate);
    var getProbationMonth = $('#ProbationMonth').val();
    getProbationMonth = parseInt(getProbationMonth)

    var CurrentDate = new Date();
    var CurrentYear = CurrentDate.getFullYear();

    var months = CurrentDate.getMonth() - getJoiningDate.getMonth() +
        (12 * (CurrentDate.getFullYear() - getJoiningDate.getFullYear()))

    var MonthlySickLeave;
    if (months >= getProbationMonth) {
        var JoiningDateAfterProbation = new Date(getJoiningDate.setMonth(getJoiningDate.getMonth() + getProbationMonth));
        var JoiningYear = JoiningDateAfterProbation.getFullYear();
        if (JoiningYear == CurrentYear) {
            // checking month wise
            if (JoiningDateAfterProbation > CurrentDate) {
                MonthlySickLeave = 0;
                return MonthlySickLeave;
            }
            months = (CurrentDate.getMonth() - JoiningDateAfterProbation.getMonth() + 1) +
                (12 * (CurrentDate.getFullYear() - getJoiningDate.getFullYear()))
            MonthlySickLeave = months / 2 - sickleave;
            if (MonthlySickLeave < 0 && MonthlySickLeave != -0.5) {
                MonthlySickLeave = 0;
            } else if (MonthlySickLeave == -0.5) {
                MonthlySickLeave = 0.5;
            }
            return MonthlySickLeave;
        }

        var Pastsickleave = $("#SickLeave").val();
        if (Pastsickleave == '') {
            Pastsickleave = 0.0;
        }

        var CurrentMonth = CurrentDate.getMonth() + 1;
        MonthlySickLeave = CurrentMonth / 2;
        MonthlySickLeave = (MonthlySickLeave - sickleave);
        if (MonthlySickLeave < 0 && MonthlySickLeave != -0.5) {
            MonthlySickLeave = 0 + parseFloat(Pastsickleave);
        } else if (MonthlySickLeave == -0.5) {
            MonthlySickLeave = 0.5;
        } else { MonthlySickLeave = MonthlySickLeave + parseFloat(Pastsickleave); }
    }
    else {
        MonthlySickLeave = 0.0;
    }
    return MonthlySickLeave;
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

        var CurrentDate = new Date();
        var CurrentMonth = CurrentDate.getMonth() + 1;
        TotalPersonalLeave = parseFloat(Pastpersonalleave) + (CurrentMonth - personalLeave)
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
        PaidLeave : paidleave
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