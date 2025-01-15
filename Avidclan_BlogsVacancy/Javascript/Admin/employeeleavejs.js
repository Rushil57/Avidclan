var calendar;
$(function () {
    SelectCalenderDate(),
    GetAllEmployeesLeaveDates()

});

function SelectCalenderDate() {
    var calendarEl = document.getElementById('calendar');
    calendar = new FullCalendar.Calendar(calendarEl, {
        initialDate: new Date(),
        selectable: true
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
                            id: val.Id,
                            title: val.FirstName + "'s " + val.Halfday,
                            start: fromdate,
                            className: className,
                        }), true;
                    }
                    else {
                        calendar.addEvent({
                            id: val.Id,
                            title: val.FirstName + "'s " + val.Halfday,
                            start: fromdate,
                            className: className,
                        }), true;
                    }
                }
                else {
                    calendar.addEvent({
                        id: val.Id,
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