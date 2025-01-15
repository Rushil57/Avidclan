﻿var validator;
$(function () {
    validator = $("form[name='registration']").validate({
        rules: {
            firstname: { required: true },
            lastname: {
                required: true,
            },
            phonenumber: {
                required: true,
                minlength: 10,
                maxlength: 10
            },
            emailaddress: {
                required: true,
                email: true
            },
            password: {
                required: true,
            },
            txtjoiningDate: { required: true, },
            role: {
                required: true,
            },
            txtprobationmonth: { required: true, },
            Confirmpassword: {
                required: true,
                equalTo: "#PassWord"
            }
        },
        messages: {
            firstname: { required: "Please enter a firstname" },
            lastname: { required: "Please enter a lastname" },
            phonenumber: {
                required: "Please enter a phonenumber",
                minlength: "Please enter valid phone number",
                maxlength: "Please enter valid phone number",
            },
            emailaddress: {
                required: "Please enter email address",
                email: "Please enter a valid email address"
            },
            password: {
                required: "Please enter a password",
            },
            txtjoiningDate: { required: "Select valid Date", },
            role: { required: "Please Select valid role" },
            txtprobationmonth: { required: "Please enter probation months" },
            Confirmpassword: {
                required: 'Confirm Password is required',
                equalTo: "Password is not same"
            }
        },
        submitHandler: function (form) {
            SaveNewUser();
        }
    });

    $(".toggle-password").click(function () {
        var input = $("#PassWord");
        if (input.attr("type") === "password") {
            input.attr("type", "text");
        } else {
            input.attr("type", "password");
        }
        $(this).toggleClass("mdi-eye-outline");
    });

    $(".toggle-confirm-password").click(function () {
        var input = $("#ConfirmPassWord");
        if (input.attr("type") === "password") {
            input.attr("type", "text");
        } else {
            input.attr("type", "password");
        }
        $(this).toggleClass("mdi-eye-outline");
    });

    BindRoles();
    $('#JoiningDate').datepicker();
    GetEmployeeList();
});

function SaveNewUser() {

    showSpinner();
    var data = {
        Id: $("#EmployeeId").val(),
        FirstName: $("#FirstName").val(),
        LastName: $("#LastName").val(),
        PhoneNumber: $("#PhoneNumber").val(),
        EmailId: $("#EmailAddress").val(),
        Password: $("#PassWord").val(),
        Role: $("#RoleName").val(),
        JoiningDate: $("#JoiningDate").val(),
        ProbationPeriod: $("#ProbationMonth").val(),
        IsNoticePeriod: $("#NoticePeriod").prop('checked'),
        OnBreak: $("#OnBreak").prop('checked'),
        BreakMonth: $("#BreakMonth").val()
    }
    $.ajax({
        type: "POST",
        url: "/BlogVacancy/AddNewUser",
        contentType: 'application/json',
        data: JSON.stringify(data),
        success: function (result) {
            if (result == "Data Saved!") {
                alert("User Created Successfully!")
                ResetForm()
                GetEmployeeList()
            }
            else {
                alert(result)
            }
            hideSpinner();
        },
        error: function (result) {
            alert(result.responseText);
            hideSpinner();
        }
    });

}
function CheckExistingEmailAddress() {
    var Id = $("#EmployeeId").val();
    if (Id == 0) {
        var emailId = $("#EmailAddress").val();
        if (emailId == null || emailId == "") {
            return
        }

        $.ajax({
            type: "GET",
            url: "/BlogVacancy/CheckEmailAdress",
            contentType: 'application/json',
            data: { Email: emailId },
            success: function (result) {
                if (result == "") {

                }
                else {
                    alert(result)
                    $("#EmailAddress").val('');
                }
            },
            error: function (result) {
                alert(result.responseText);
            }
        });
    }


}
function BindRoles() {
    $.ajax({
        type: "GET",
        url: "/BlogVacancy/GetRoles",
        contentType: 'application/json',
        success: function (result) {
            if (result != null) {
                $.each(result, function (index, item) {
                    var opt = new Option(item.RoleName, item.Id);
                    $('.cls-role').append(opt);

                });
            }
        },
        error: function (result) {
            alert(result.responseText);
        }
    });
}
function ResetForm() {
    validator.resetForm();
    $("#EmployeeId").val(0);
    $("#FirstName").val(''),
        $("#LastName").val(''),
        $("#PhoneNumber").val(''),
        $("#EmailAddress").val(''),
        $("#PassWord").val(''),
        $("#ConfirmPassWord").val('')
    $("#RoleName").val(''),
        $("#JoiningDate").val(''),
        $("#ProbationMonth").val('')
    $("#EmployeeModal").modal('hide');
    $('#NoticePeriod').prop('checked', false);
    $('#OnBreak').prop('checked', false);
    $('#BreakMonth').val("0");
}
function ShowEmployeRegisterModel() {
    $("#EmployeeModal").modal('show')
}
function GetEmployeeList() {
    showSpinner();
    $.ajax({
        type: "Get",
        url: "/BlogVacancy/GetAllEmployees",
        success: function (result) {
            $('#EmployeeList').dataTable({
                destroy: true,
                data: result,
                "columns": [
                    {
                        "data": "FirstName",
                    },
                    { "data": "LastName" },
                    {
                        "data": "JoiningDate",
                        "render": function (data) {
                            const dateValue = new Date(parseInt(data.substr(6)));
                            return moment(dateValue).format('DD/MM/YYYY');
                        }
                    },
                    { "data": "Password" },
                    { "data": "ProbationPeriod" },
                    { "data": "PaidLeave" },
                    { "data": "SickLeave" },
                    {
                        "data": "Id",
                        "render": function (data, type, row) {
                            return '<a class="btn btn-info btn-sm" style="color: white;" onclick="GetEmployeeDetailsById(' + data + ')"><i class="mdi mdi-pencil"></i></a>';
                        }
                    },
                    {
                        "data": "Id",
                        "render": function (data, type, row) {
                            return '<a class="btn btn-danger btn-sm" style="color: white;" onclick=DeleteEmployee(' + data + ')><i class="mdi mdi-delete"></i></a>';
                        }
                    }
                ]
            });

            hideSpinner();

        },
        error: function (result) {
            alert(result.responseText);
            hideSpinner();
        }
    });
}
function GetEmployeeDetailsById(id) {
    showSpinner();
    $("#EmployeeModal").modal('show');
    $.ajax({
        url: "/BlogVacancy/GetEmployeeById",
        contentType: 'application/json',
        data: { Id: id },
        type: "GET",
        success: function (result) {
            if (result != null) {
                $("#EmployeeId").val(result.Id);
                $("#FirstName").val(result.FirstName);
                $("#LastName").val(result.LastName)
                $("#PhoneNumber").val(result.PhoneNumber)
                $("#EmailAddress").val(result.EmailId)
                $("#PassWord").val(result.Password)
                $("#ConfirmPassWord").val(result.Password)
                var date = moment(result.JoiningDate).format('MM/DD/YYYY');
                $("#JoiningDate").val(date)
                $("#ProbationMonth").val(result.ProbationPeriod)
                $("#RoleName").val(result.Role);
                if (result.IsNoticePeriod) {
                    $('#NoticePeriod').prop('checked', true);
                } else {
                    $('#NoticePeriod').prop('checked', false);
                }
                if (result.OnBreak) {
                    $('#OnBreak').prop('checked', true);
                } else {
                    $('#OnBreak').prop('checked', false);
                }
                $("#BreakMonth").val(result.BreakMonth);
            }

            hideSpinner();
        },
        error: function () {
            alert(result.responseText);
            hideSpinner();
        }
    });
}

function DeleteEmployee(id) {
    if (confirm('Are you sure want to delete Employee?')) {
        showSpinner();
        $.ajax({
            url: "/BlogVacancy/DeleteEmployee",
            contentType: 'application/json',
            data: { Id: id },
            type: "GET",
            success: function (result) {
                GetEmployeeList();
                alert('Delete Successully!');
                hideSpinner();
            },
            error: function (result) {
                alert(result.responseText);
                hideSpinner();
            }
        });
    }
}

function ShowBreakMonthInput() {
    var breakCheck = $("#OnBreak").prop('checked');
    if (breakCheck) {
        $(".cls-breakmonth").removeClass("d-none");
    } else {
        $(".cls-breakmonth").addClass("d-none");
    }
}

// Export to Excel Function
function ExportEmployeeLeaveDetails() {
    // Show loader
    showSpinner();

    // Simulate the process of exporting to Excel
    try {
        var table = $('#EmployeeList').DataTable();
        var data = table.rows().data().toArray();
        var currentDate = new Date();
        var date = formatDate(currentDate);
        
        // Prepare data for Excel
        var exportData = [];
        exportData.push(["First Name", "Last Name", "PaidLeave_On_" + date, "SickLeave_On_" + date]); // Header row
        data.forEach(row => {
            exportData.push([
                row.FirstName,     // First Name
                row.LastName,      // Last Name
                row.PaidLeave,     // Paid Leave
                row.SickLeave      // Sick Leave
            ]);
        });

        // Create Excel file
        var ws = XLSX.utils.aoa_to_sheet(exportData);
        var wb = XLSX.utils.book_new();
        XLSX.utils.book_append_sheet(wb, ws, "EmployeeData");

        // Export file
        XLSX.writeFile(wb, "EmployeeLeaveDetails_" + date +".xlsx");

    } catch (error) {
        alert('Error while exporting data: ' + error.message);
    } finally {
        // Hide loader after process ends
        hideSpinner();
    }
}

function formatDate(dateString) {
    const date = new Date(dateString);
    const day = String(date.getDate()).padStart(2, '0'); // Get the day (2 digits)
    const month = date.toLocaleString('en-US', { month: 'short' }); // Get the short month name
    const year = date.getFullYear(); // Get the year
    return `${day}_${month}_${year}`; // Combine into the desired format
}
