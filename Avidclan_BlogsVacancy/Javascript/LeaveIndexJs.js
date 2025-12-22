var calendar;
var isCompensationLeave = false;
var leaveBalance = {};
var userLeaveDates = [];

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
        GetTotalCompensationLeave(),
        // GetPastLeaveBalance(),
        //delay(1500).then(GetTotalLeaveBalance)
    ).done(function () {
        hideSpinner();
    }).fail(function () {
        hideSpinner(); // Hide spinner even if there's an error
    });
});

function GetTotalCompensationLeave() {
    $.ajax({
        url: '/Leave/GetTotalCompensationLeave',
        contentType: 'application/json',
        type: "GET",
        success: function (compensatedata) {
            isCompensationLeave = compensatedata.length > 0
        },
        error: function (result) {
        }
    });
}

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
                UserLeaveDates();
                refreshLeaveTypes();
                SetMinDate();
            }
        },
        events: [],
        eventClick: function (info) {            
            var StartTime = moment(info.event.start).format('MM/DD/YYYY');
            var Eventlist = calendar.getEvents();
            refreshLeaveTypes();
            EditLeaves(info.event.id, StartTime);
        }
    });

    calendar.render();
}

var oldToDate = null;
var oldFromDate = null;

// Save the previous value when the input is focused
$("#txtToDate").on("focusin", function () {
    oldToDate = $(this).val();  // store old value before change
});

$("#txtFromDate").on("focusin", function () {
    oldFromDate = $(this).val();  // store old value before change
});

function refreshLeaveTypes() {
    const today = new Date();
    today.setHours(0, 0, 0, 0);

    const fromDateVal = $("#txtFromDate").val();
    const toDateVal = $("#txtToDate").val();
    const leaveType = $("#drpLeaveType");

    // Reset dropdown (keep only LWP)
    leaveType.find("option[value='PL']").remove();
    leaveType.find("option[value='SL']").remove();
    leaveType.find("option[value='CL']").remove();

    var getJoiningDate = $('#JoiningDate').val();
    getJoiningDate = new Date(getJoiningDate);

    var getProbationMonth = $('#ProbationMonth').val();
    getProbationMonth = parseInt(getProbationMonth, 10);

    // Normalize dates (remove time)
    getJoiningDate.setHours(0, 0, 0, 0);

    var probationEndDate = new Date(getJoiningDate);
    probationEndDate.setMonth(probationEndDate.getMonth() + getProbationMonth);

    var tdate = new Date();
    tdate.setHours(0, 0, 0, 0);

    // ✅ Check if probation is over
    var isProbationOver = tdate >= probationEndDate;

    var noticePeriod = $("#NoticePeriodDate").val();

    if (!fromDateVal || (noticePeriod && noticePeriod != "") || !isProbationOver) {
        return;
    }

    const fromDate = new Date(fromDateVal);
    fromDate.setHours(0, 0, 0, 0);

    const toDate = toDateVal ? new Date(toDateVal) : new Date(fromDateVal);
    toDate.setHours(0, 0, 0, 0);

    const diffDays = Math.ceil((fromDate - today) / (1000 * 3600 * 24));
    const totalDays = Math.ceil((toDate - fromDate) / (1000 * 3600 * 24)) + 1;

    // ---------------------------
    // Sick Leave (SL) — Logic
    // ---------------------------
    const diffDaysSL = Math.ceil((fromDate - today) / (1000 * 3600 * 24));

    if (diffDaysSL <= 1) {
        leaveType.append('<option value="SL">Sick Leave (SL)</option>');
    }

    // ---------------------------
    // Paid Leave (PL) — Logic
    // ---------------------------
    if (totalDays === 1 && diffDays >= 7) {
        leaveType.append('<option value="PL">Paid Leave (PL)</option>');
    }
    else if (totalDays >= 2 && diffDays >= 14) {
        leaveType.append('<option value="PL">Paid Leave (PL)</option>');
    }

    // ---------------------------
    // Compensation Leave (CL) — Same Logic as PL
    // ---------------------------
    if (isCompensationLeave && totalDays === 1 && diffDays >= 7) {
        leaveType.append('<option value="CL">Compensation Leave (CL)</option>');
    }
    else if (isCompensationLeave && totalDays >= 2 && diffDays >= 14) {
        leaveType.append('<option value="CL">Compensation Leave (CL)</option>');
    }
    validateLeaveDays();
}

function SetMinDate() {
    var start = $("#txtFromDate").datepicker('getDate');
    var WeekDay = start?.getDay();

    // Revert if it's weekend
    if (WeekDay === 6 || WeekDay === 0) {
        revertFromDate();
        return;
    }

    // Check against existing calendar events
    const formatted = moment(start).format("YYYY-MM-DD");
    const existingDates = getAllCalendarEventDates();

    if (existingDates.has(formatted)) {
        alert(`Date ${formatted} already has an request.`);
        revertFromDate();
        return;
    }

    // Valid case
    $("#txtToDate").datepicker("option", "minDate", start);
    DateChange();
}

function revertFromDate() {
    if (oldFromDate) {
        $("#txtFromDate").datepicker('setDate', oldFromDate);
    } else {
        $("#txtFromDate").val("");
    }
    setTimeout(() => $("#txtFromDate").datepicker("show"), 5);
}

function revertToDate() {
    if (oldToDate) {
        $("#txtToDate").datepicker('setDate', oldToDate);
    } else {
        $("#txtToDate").val("");
    }
    setTimeout(() => $("#txtToDate").datepicker("show"), 5);
}


let hasConflict = false;
//function DateChange() {
//    const start = $("#txtFromDate").datepicker('getDate');
//    const end = $("#txtToDate").datepicker('getDate');

//    if (!start || !end) return;

//    const weekDay = end.getDay();
//    if (weekDay === 6 || weekDay === 0) {
//        if (oldToDate) {
//            $("#txtToDate").datepicker('setDate', oldToDate);
//        } else {
//            $("#txtToDate").val("");
//        }
//        setTimeout(() => $("#txtToDate").datepicker("show"), 5);
//        return;
//    }

//    const newFromStr = moment(start).format("MM/DD/YYYY");
//    const newToStr = moment(end).format("MM/DD/YYYY");
//    const oldFromStr = oldFromDate ? moment(oldFromDate).format("MM/DD/YYYY") : "";
//    const oldToStr = oldToDate ? moment(oldToDate).format("MM/DD/YYYY") : "";

//    const bothChanged = newFromStr !== oldFromStr && newToStr !== oldToStr;

//    const result = [];


//    //function pushIfWeekday(date, list) {
//    //    const day = date.getDay();
//    //    if (day === 0 || day === 6) return;

//    //    const formatted = moment(date).format("YYYY-MM-DD");
//    //    const existingDates = getAllCalendarEventDates();

//    //    if (existingDates.has(formatted)) {
//    //        alert(`Date ${formatted} already has an request.`);
//    //        hasConflict = true;
//    //        return;
//    //    } else {
//    //        hasConflict = false;
//    //    }

//    //    list.push({ LeaveDates: new Date(date) });
//    //}

//    function pushIfWeekday(date, list) {
//        const day = date.getDay();
//        if (day === 0 || day === 6) return;

//        const formatted = moment(date).format("YYYY-MM-DD");
//        const existingDates = getAllCalendarEventDates();

//        const oldDatesSet = new Set();
//        document.querySelectorAll('#LeaveDateTable tbody tr').forEach(row => {
//            const dateStr = row.querySelector('td')?.textContent.trim();
//            if (dateStr) {
//                const parsed = moment(dateStr, "MM/DD/YYYY").format("YYYY-MM-DD");
//                oldDatesSet.add(parsed);
//            }
//        });

//        // Skip if already added to the list to avoid duplicates
//        const alreadyInList = list.some(item =>
//            moment(item.LeaveDates).format("YYYY-MM-DD") === formatted
//        );
//        if (alreadyInList) return;

//        if (existingDates.has(formatted) && !oldDatesSet.has(formatted)) {
//            alert(`Date ${formatted} already has a request.`);
//            hasConflict = true;
//            return;
//        }

//        hasConflict = false;
//        list.push({ LeaveDates: new Date(date) });
//    }

//    const isEdit = $("#LeaveId").val() != 0 || $("#WfhId").val() != 0;

//    if (isEdit) {
//        if (bothChanged) {
//            const arr = [];
//            const dt = new Date(start);
//            while (dt <= end) {
//                pushIfWeekday(dt, arr);
//                dt.setDate(dt.getDate() + 1);
//            }

//            // ✅ FIX HERE: Push to arr, not result
//            if (oldToDate) {
//                const oldStr = moment(oldToDate).format("YYYY-MM-DD");
//                const exists = arr.some(item => moment(item.LeaveDates).format("YYYY-MM-DD") === oldStr);

//                if (!exists) {
//                    arr.push({ LeaveDates: new Date(oldToDate) });
//                }
//            }

//            BindLeaveTable(arr, true);
//            return;
//        }


//        // Partial change: retain and extend existing rows
//        const rows = document.querySelectorAll('#LeaveDateTable tbody tr');

//        rows.forEach((row, index) => {
//            const cells = row.querySelectorAll('td');
//            if (cells.length === 0) return;

//            const leaveDate = cells[0].textContent.trim();
//            const checkbox = cells[1].querySelector('input[type="checkbox"]');
//            const isChecked = checkbox?.checked ?? false;

//            const radioName = `HalfDayLeave_${index + 1}`;
//            const selectedRadio = row.querySelector(`input[name='${radioName}']:checked`);
//            const halfDayValue = selectedRadio?.value ?? null;

//            const wfhRadioName = `WFHHalfDay_${index + 1}`;
//            const wfhSelectedRadio = row.querySelector(`input[name='${wfhRadioName}']:checked`);
//            const wfhHalfDayValue = wfhSelectedRadio?.value ?? null;

//            const rowData = { LeaveDates: leaveDate };

//            if (isChecked) rowData.Title = "WFH";
//            if (halfDayValue) rowData.Halfday = halfDayValue;

//            if (wfhHalfDayValue === "HalfDayLeave" && selectedRadio) {
//                rowData.Title = "LeaveAndWfh";
//                rowData.LeaveHalfday = selectedRadio.value === "FirstHalf" ? "SecondHalf" : "FirstHalf";
//            }

//            result.push(rowData);
//        });

//        // Add new dates within range if not already included
//        const current = new Date(start);
//        while (current <= end) {
//            pushIfWeekday(current, result);
//            current.setDate(current.getDate() + 1);
//        }
//        // Ensure the original edit date is included
//        if (oldToDate) {
//            const oldStr = moment(oldToDate).format("YYYY-MM-DD");
//            const exists = result.some(item => moment(item.LeaveDates).format("YYYY-MM-DD") === oldStr);

//            if (!exists) {
//                result.push({ LeaveDates: new Date(oldToDate) });
//            }
//        }

//        oldToDate = moment(end).format("MM-DD-YYYY");
//        oldFromDate = moment(start).format("MM-DD-YYYY");
//        BindLeaveTable(result, true);
//    } else {
//        // New request (not edit)
//        const arr = [];
//        const dt = new Date(start);
//        while (dt <= end) {
//            pushIfWeekday(dt, arr);
//            dt.setDate(dt.getDate() + 1);
//        }

//        if (hasConflict) {
//            revertToDate();
//            return;
//        } else {
//            // ✅ Update the stored oldToDate only when no conflict
//            oldToDate = new Date(end);
//        }
//        BindLeaveTable(arr, false);
//    }
//}

function DateChange() {
    const start = $("#txtFromDate").datepicker('getDate');
    const end = $("#txtToDate").datepicker('getDate');

    if (!start || !end) return;

    const weekDay = end.getDay();
    if (weekDay === 6 || weekDay === 0) {
        if (oldToDate) {
            $("#txtToDate").datepicker('setDate', oldToDate);
        } else {
            $("#txtToDate").val("");
        }
        setTimeout(() => $("#txtToDate").datepicker("show"), 5);
        return;
    }

    const newFromStr = moment(start).format("MM/DD/YYYY");
    const newToStr = moment(end).format("MM/DD/YYYY");
    const oldFromStr = oldFromDate ? moment(oldFromDate).format("MM/DD/YYYY") : "";
    const oldToStr = oldToDate ? moment(oldToDate).format("MM/DD/YYYY") : "";
    const bothChanged = newFromStr !== oldFromStr && newToStr !== oldToStr;

    let result = [];

    function pushIfWeekday(date, list) {
        const day = date.getDay();
        if (day === 0 || day === 6) return;

        const formatted = moment(date).format("YYYY-MM-DD");
        const existingDates = getAllCalendarEventDates();

        const oldDatesSet = new Set();
        document.querySelectorAll('#LeaveDateTable tbody tr').forEach(row => {
            const dateStr = row.querySelector('td')?.textContent.trim();
            if (dateStr) {
                const parsed = moment(dateStr, "MM/DD/YYYY").format("YYYY-MM-DD");
                oldDatesSet.add(parsed);
            }
        });

        const alreadyInList = list.some(item =>
            moment(item.LeaveDates).format("YYYY-MM-DD") === formatted
        );
        if (alreadyInList) return;

        if (existingDates.has(formatted) && !oldDatesSet.has(formatted)) {
            alert(`Date ${formatted} already has a request.`);
            hasConflict = true;
            return;
        }

        hasConflict = false;
        list.push({ LeaveDates: new Date(date) });
    }

    const isEdit = $("#LeaveId").val() != 0 || $("#WfhId").val() != 0;

    if (isEdit) {
        if (bothChanged) {
            const arr = [];

            if (newFromStr === newToStr) {
                pushIfWeekday(start, arr);
            } else {
                const dt = new Date(start);
                while (dt <= end) {
                    pushIfWeekday(new Date(dt), arr);
                    dt.setDate(dt.getDate() + 1);
                }
            }

            BindLeaveTable(arr, true);
            return;
        }

        const rows = document.querySelectorAll('#LeaveDateTable tbody tr');
        rows.forEach((row, index) => {
            const cells = row.querySelectorAll('td');
            if (cells.length === 0) return;

            const leaveDate = cells[0].textContent.trim();
            const checkbox = cells[1].querySelector('input[type="checkbox"]');
            const isChecked = checkbox?.checked ?? false;

            const radioName = `HalfDayLeave_${index + 1}`;
            const selectedRadio = row.querySelector(`input[name='${radioName}']:checked`);
            const halfDayValue = selectedRadio?.value ?? null;

            const wfhRadioName = `WFHHalfDay_${index + 1}`;
            const wfhSelectedRadio = row.querySelector(`input[name='${wfhRadioName}']:checked`);
            const wfhHalfDayValue = wfhSelectedRadio?.value ?? null;

            const rowData = { LeaveDates: new Date(leaveDate) };

            if (isChecked) rowData.Title = "WFH";
            if (halfDayValue) rowData.Halfday = halfDayValue;

            if (wfhHalfDayValue === "HalfDayLeave" && selectedRadio) {
                rowData.Title = "LeaveAndWfh";
                rowData.LeaveHalfday = selectedRadio.value === "FirstHalf" ? "SecondHalf" : "FirstHalf";
            }

            result.push(rowData);
        });

        const current = new Date(start);
        if (newFromStr === newToStr) {
            pushIfWeekday(start, result);
        } else {
            while (current <= end) {
                pushIfWeekday(new Date(current), result);
                current.setDate(current.getDate() + 1);
            }
        }

        if (oldToDate) {
            const oldStr = moment(oldToDate).format("YYYY-MM-DD");
            const exists = result.some(item => moment(item.LeaveDates).format("YYYY-MM-DD") === oldStr);
            if (!exists) {
                result.push({ LeaveDates: new Date(oldToDate) });
            }
        }

        // ✅ Filter out any dates that fall outside the selected range (timestamp-based)
        const fromTime = new Date(newFromStr).setHours(0, 0, 0, 0);
        const toTime = new Date(newToStr).setHours(0, 0, 0, 0);

        result = result.filter(item => {
            const leaveTime = new Date(item.LeaveDates).setHours(0, 0, 0, 0);
            return leaveTime >= fromTime && leaveTime <= toTime;
        });

        oldToDate = moment(end).format("MM-DD-YYYY");
        oldFromDate = moment(start).format("MM-DD-YYYY");

        BindLeaveTable(result, true);
    } else {
        const arr = [];
        if (newFromStr === newToStr) {
            pushIfWeekday(start, arr);
        } else {
            const dt = new Date(start);
            while (dt <= end) {
                pushIfWeekday(new Date(dt), arr);
                dt.setDate(dt.getDate() + 1);
            }
        }

        if (hasConflict) {
            revertToDate();
            return;
        } else {
            oldToDate = new Date(end);
        }
        BindLeaveTable(arr, false);
    }

    refreshLeaveTypes();
}

function validateLeaveDays() {
    $("#txtLeaveNotification").text('');
    const start = $("#txtFromDate").datepicker('getDate');
    const end = $("#txtToDate").datepicker('getDate');
    let leaveType = $("#drpLeaveType").val();
    if (leaveType == "LWP")
        return;
    if (leaveType != "SL" && !ValidateLeaveAdjacency()) {
        $("#txtLeaveNotification").text(`You have taken leave on a day that is immediately before or after the selected dates. As per policy, the entire leave period will be considered as LWP.`);
    }
    else if (start && end) {
        const diffTime = end.getTime() - start.getTime();
        const selectedDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24)) + 1;
        let balance = leaveBalance[leaveType] || 0;
        if (selectedDays > balance) {
            $("#txtLeaveNotification").text(`${leaveType} leave balance is ${balance}, so only ${balance} day(s) can be applied as ${leaveType}. Extra days will be treated as LWP.`);
        }
    }
}

function UserLeaveDates() {
    $.ajax({
        url: "/Leave/LeaveDates",
        contentType: 'application/json',
        type: "GET",
        success: function (data) {

            userLeaveDates = data.leavelist.map(item => {

                // extract timestamp inside /Date(1705861800000)/
                let timestamp = parseInt(item.LeaveDate.replace(/[^\d]/g, ''));

                return {
                    ...item,
                    LeaveJsDate: new Date(timestamp)   // Converted date
                };
            });

            console.log("Converted Leave Dates:", userLeaveDates);
        },
        error: function (result) {
            alert(result.responseText);
        }
    });
}

function normalize(d) {
    let dt = new Date(d);
    dt.setHours(0, 0, 0, 0);
    return dt;
}

function ValidateLeaveAdjacency() {
    let fromDate = normalize($("#txtFromDate").val());
    let toDate = normalize($("#txtToDate").val());

    if (!userLeaveDates || userLeaveDates.length === 0)
        return true;

    // 1️⃣ Skip validation if leave starts after 15 days
    let today = normalize(new Date());
    let limitDate = new Date(today);
    limitDate.setDate(limitDate.getDate() + 15);

    if (fromDate > limitDate)
        return true;

    // 2️⃣ Adjacent dates
    let prevT = new Date(fromDate);
    prevT.setDate(prevT.getDate() - 1);

    let nextT = new Date(toDate);
    nextT.setDate(nextT.getDate() + 1);

    // Weekend bridge
    let fridayBefore = new Date(fromDate);
    fridayBefore.setDate(fridayBefore.getDate() - 3);

    let mondayAfter = new Date(toDate);
    mondayAfter.setDate(mondayAfter.getDate() + 3);

    let hasAdjacent = userLeaveDates.some(item => {
        let d = normalize(item.LeaveJsDate);

        return (
            d.getTime() === prevT.getTime() ||
            d.getTime() === nextT.getTime() ||
            (d.getTime() === fridayBefore.getTime() && fromDate.getDay() === 1) ||
            (d.getTime() === mondayAfter.getTime() && toDate.getDay() === 5)
        );
    });

    return !hasAdjacent;
}

function isSameDate(date1, date2) {
    return (
        date1.getFullYear() === date2.getFullYear() &&
        date1.getMonth() === date2.getMonth() &&
        date1.getDate() === date2.getDate()
    );
}

function getAllCalendarEventDates() {
    const events = calendar.getEvents();
    const eventDates = new Set();

    events.forEach(event => {
        const startDate = moment(event.start);
        const endDate = moment(event.end || event.start);

        let current = moment(startDate);
        while (current <= endDate) {
            eventDates.add(current.format("YYYY-MM-DD"));
            current.add(1, 'days');
        }
    });

    return eventDates;
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
            else if (arr[i].Title == "LeaveAndWfh") {
                addcontrols += '<td><i class="mdi mdi-delete-outline delete-icon" onclick=DeleteWFHandLeaveDetailsOfEmployee(' + arr[i].WFHDetailsId + ',' + arr[i].LeaveDetailsId + ')></i></td>'
            }
            else {
                addcontrols += '<td><i class="mdi mdi-delete-outline delete-icon" id="LeaveDetailsId_' + arr[i].LeaveDetailsId + '" onclick=DeleteLeaveDetailsOfEmployee(' + arr[i].LeaveDetailsId + ',this)></i></td>'
            }

        }
        addcontrols += "</td></tr>";


        $("#LeaveDateTable .LeaveDatebody").append(addcontrols);

        if (showDeleteIcon) {
            if (arr[i].Halfday != null && arr[i].Halfday != "") {
                $(".HalfDaydiv_" + count).show();
                /*  $("#WorkFromcheck_" + count).prop("checked", true);*/
                $("input[name=HalfDayLeave_" + count + "][value=" + arr[i].Halfday + "]").prop("checked", true);
                $("input[name=HalfDayLeave_" + count + "][value=" + arr[i].Halfday + "]").attr("data-checked", "true");
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
    var Reason = $("#txtReason").val().trim();
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
        LeaveType: $("#drpLeaveType").val(),
        Leaves: LeaveDetails,
        ReportingPerson: $("#ReportingPersonId").val(),
        ReasonForLeave: $("#txtReason").val(),
        WorkFromHome: $("#WorkFromHome").prop('checked'),
        WFHId: $("#WfhId").val()
    }
    showSpinner();
    $.ajax({
        type: "POST",
        url: "/api/Admin/RequestForLeaveNew",
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify(Leavedata),
        success: function (result) {
            /*alert("Request Sent..");*/
            alert(result)
            ResetModel();
            //GetPastLeaveBalance();
            delay(1500).then(function () {
                //GetTotalLeaveBalance();

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

function GetUserLeaveBalance() {
    var data = {
        LeaveType: $("#drpLeaveType").val(),
        FromDate: $("#txtFromDate").val(),
        ToDate: $("#txtToDate").val()
    };

    $.ajax({
        type: "POST",
        url: "/api/Admin/GetUserLeaveBalance",
        dataType: 'text',
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify(data),
        success: function (result) {
            leaveBalance = JSON.parse(result);
            validateLeaveDays();
        },
        error: function (err) {
            //hideSpinner();
        }
    });
}

function ResetModel() {
    console.clear();
    $("#ApplyLeaveModel").hide();
    $("#LeaveId").val(0)
    $("#WfhId").val(0)
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
                        id: "WFH_" + val.Id,
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
        alert("Please inform to management team for changes in leave.")
        //OpenEmployeeEditDetails(id)
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
            $("#ReportingPersonId").empty();
            $('#ReportingPersonId').trigger("chosen:updated");
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
                var paidleave = data.TotalLeaveList.FinalBalance;
                //SaveSickAndPaidLeave(sickLeave, paidleave);
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
    const today = new Date();
    const start = new Date(startDate);

    // Normalize time for both dates to avoid partial day issues
    today.setHours(0, 0, 0, 0);
    start.setHours(0, 0, 0, 0);

    let differenceInDays = 0;
    let current = new Date(today);

    // Loop from today to start date
    while (current < start) {
        const dayOfWeek = current.getDay(); // 0 = Sunday, 6 = Saturday
        if (dayOfWeek !== 0 && dayOfWeek !== 6) {
            differenceInDays++;
        }
        current.setDate(current.getDate() + 1);
    }

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

            if (data.leavelist.length == 0 && data.wfhdetaillist.length == 0) {
                ResetModel();
                return;
            }

            if (data.leavelist.length > 0 && data.wfhdetaillist.length > 0) {
                var combinedList = data.leavelist.concat(data.wfhdetaillist);

                // Extract all dates and convert them to JavaScript Date objects
                var allDates = combinedList.flatMap(item => [
                    new Date(parseInt(item.Fromdate.match(/\d+/)[0])),
                    new Date(parseInt(item.Todate.match(/\d+/)[0]))
                ]);

                // Get the lowest (earliest) and highest (latest) dates
                var minDate = new Date(Math.min(...allDates));
                var maxDate = new Date(Math.max(...allDates));

                // Format the dates as YYYY-MM-DD
                var minDateFormatted = minDate.toLocaleDateString('en-CA'); // gives yyyy-mm-dd
                var maxDateFormatted = maxDate.toLocaleDateString('en-CA');

                console.log("Earliest Date:", minDateFormatted);
                console.log("Latest Date:", maxDateFormatted);

                var fromdate = moment(minDateFormatted).format('MM/DD/YYYY');
                var todate = moment(maxDateFormatted).format('MM/DD/YYYY');

                // Example: comma-separated values from database
                if (data.wfhdetaillist[0].ReportingPersonEdit != null) {
                    // Convert to array
                    var personArray = data.wfhdetaillist[0].ReportingPersonEdit.split(',').map(p => p.trim());
                    $("#ReportingPersonId").val(personArray);

                    // Trigger update for Chosen to recognize new values
                    $("#ReportingPersonId").trigger("chosen:updated");
                }

                $("#txtFromDate").val(fromdate);
                $("#txtToDate").val(todate);
                oldFromDate = fromdate;
                oldToDate = todate;
            }
            else if (data.leavelist.length > 0) {
                var fromdate = moment(record.Fromdate).format('MM/DD/YYYY');
                var todate = moment(record.Todate).format('MM/DD/YYYY');

                // Example: comma-separated values from database
                if (data.leavelist[0].ReportingPersonEdit != null) {
                    // Convert to array
                    var personArray = data.leavelist[0].ReportingPersonEdit.split(',').map(p => p.trim());
                    $("#ReportingPersonId").val(personArray);

                    // Trigger update for Chosen to recognize new values
                    $("#ReportingPersonId").trigger("chosen:updated");
                }
                $("#txtFromDate").val(fromdate);
                $("#txtToDate").val(todate);

                oldFromDate = fromdate;
                oldToDate = todate;
               
            }
            else if (data.wfhdetaillist.length > 0) {
                var fromdate = moment(record.Fromdate).format('MM/DD/YYYY');
                var todate = moment(record.Todate).format('MM/DD/YYYY');


                // Example: comma-separated values from database
                if (data.wfhdetaillist[0].ReportingPersonEdit != null) {
                    // Convert to array
                    var personArray = data.wfhdetaillist[0].ReportingPersonEdit.split(',').map(p => p.trim());

                    // Clear existing options (optional)
                    // $("#ReportingPersonId").empty();

                    $("#ReportingPersonId").val(personArray);

                    // Trigger update for Chosen to recognize new values
                    $("#ReportingPersonId").trigger("chosen:updated");
                }

                $("#txtFromDate").val(fromdate);
                $("#txtToDate").val(todate);
                oldFromDate = fromdate;
                oldToDate = todate;
            }

           
            // Check leave edit restrictions if applicable
            if (isLeave) {
                var differenceDay = checkedEditedLeavesday(fromdate);
                if (differenceDay <= 2) {
                    alert("Please inform the management team about changes in leave. Thanks!");
                    hideSpinner();
                    return;
                }
            }
            if (data.leavelist.length > 0) {
                $("#LeaveId").val(data.leavelist[0].Id)
            }
            if (data.wfhdetaillist.length > 0) {
                $("#WfhId").val(data.wfhdetaillist[0].WFHId)
            }
            // Populate modal fields
            $("#ApplyLeaveModel").show();
            $("#EmailId").val(record.EmailId);
            $("#FirstName").val(record.FirstName);
            $("#txtReason").val(isLeave ? record.ReasonForLeave : record.ReasonForWFH);
            //SetMinDate();
            var start = $("#txtFromDate").datepicker('getDate');
            $("#txtToDate").datepicker("option", "minDate", start);
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


function DeleteLeaveDetailsOfEmployee(leaveDetailsId, button) {
    if (confirm('Are you sure you want to delete leave?')) {

        // If leaveDetailsId is undefined, remove row directly
        if (typeof leaveDetailsId === 'undefined' || leaveDetailsId === null) {
            // Traverse from button to the row and remove it
            const row = button.closest('tr');
            if (row) row.remove();
            return;
        }

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
                var leaveId = $("#WfhId").val();
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
        $.ajax({
            url: "/Leave/DeleteWFHandLeaveDetailsOfEmployee",
            type: "GET",
            data: {
                wfhDetailId: wfhdetailId,
                LeaveApplicationId: leaveApplicationId
            },
            contentType: 'application/json; charset=utf-8',
            success: function (data) {
                alert(data);
                var leaveId = $("#WfhId").val();
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