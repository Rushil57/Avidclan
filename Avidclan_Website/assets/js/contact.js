$(function () {
  // Initialize form validation on the registration form.
  // It has the name attribute "registration"
  $("form[name='contactinfo']").validate({
    errorPlacement: function (error, element) {
      if ($(element).is("#phone")) {
        element.closest(".intl-tel-input").after(error);
      } else {
        error.insertAfter(element);
      }
    },

    // Specify validation rules
    rules: {
      // The key name on the left side is the name attribute
      // of an input field. Validation rules are defined
      // on the right side
      firstname: "required",
      lastname: "required",
      email: {
        required: true,
        // Specify that email should be validated
        // by the built-in "email" rule
        email: true,
      },
      phone: {
        required: true,
        digits: true,
        //minlength: 10,
        //maxlength: 10,
      },
      message: {
        required: true,
        minlength: 25,
      },
      //   fcaptcha: {
      //     required: true,
      //   },
    },
    // Specify validation error messages
    messages: {
      firstname: "Please enter your First Name",
      lastname: "Please enter your Last Name",
      phone: {
        required: "Please enter phone number",
        digits: "Please enter valid phone number",
        //minlength: "Please enter at least 10 digits",
        //maxlength: "Phone number field accept only 10 digits",
      },
      email: {
        required: "Please enter email address",
        email: "Please enter a valid email address.",
      },
      message: {
        required: "Please write your Message",
        minlength: "Please write at least more than 25 characters",
      },
      //   fcaptcha: {
      //     required: "Please verify Captcha",
      //   },
    },
    // Make sure the form is submitted to the destination defined
    // in the "action" attribute of the form when valid
    submitHandler: function (form) {
      form.submit();
    },
  });

  $("form[name='career_form_data']").validate({
    errorPlacement: function (error, element) {
      if ($(element).is("select")) {
        element.next().after(error);
      } else {
        error.insertAfter(element);
      }
    },

    debug: true,
    success: "valid",
    // Specify validation rules
    rules: {
      // The key name on the left side is the name attribute
      // of an input field. Validation rules are defined
      // on the right side
      caseOption: "required",
      name: "required",
      email: {
        required: true,
        // Specify that email should be validated
        // by the built-in "email" rule
        email: true,
      },
      contactno: {
        required: true,
        digits: true,
        minlength: 10,
        maxlength: 10,
      },
      attachmentFile: {
        required: true,
        extension: "pdf,doc,docx",
      },
      message: {
        required: true,
      },
    },
    // Specify validation error messages
    messages: {
      caseOption: "Please select position",
      name: "Please enter your Name",
      contactno: {
        required: "Please enter phone number",
        digits: "Please enter valid phone number",
        minlength: "Phone number field accept only 10 digits",
        maxlength: "Phone number field accept only 10 digits",
      },
      email: {
        required: "Please enter email address",
        email: "Please enter a valid email address.",
      },
      //attachmentFile: "Please attach your Resume",
      attachmentFile: {
        required: "Please choose a file",
        extension: "Please choose a valid file type",
        //maxfilesize: "File size must not be more than 10 MB.",
      },
      message: {
        required: "Please write your Message",
      },
    },
    // Make sure the form is submitted to the destination defined
    // in the "action" attribute of the form when valid
    submitHandler: function (form) {
      form.submit();
    },
  });

  $("form[name='project_form_data']").validate({
    errorPlacement: function (error, element) {
      if ($(element).is("#phone")) {
        element.closest(".intl-tel-input").after(error);
      } else if (element.attr("class").indexOf("selectpicker") != -1) {
        //get main div
        var mpar = $(element).closest("div.bootstrap-select");
        //insert after .dropdown-toggle div
        error.insertAfter($(".dropdown-toggle", mpar));
      } else {
        error.insertAfter(element);
      }
    },

    // errorPlacement: function (error, element) {
    //   //check if element has class "kt_selectpicker"
    //   if (element.attr("class").indexOf("selectpicker") != -1) {
    //     //get main div
    //     var mpar = $(element).closest("div.bootstrap-select");
    //     //insert after .dropdown-toggle div
    //     error.insertAfter($(".dropdown-toggle", mpar));
    //   } else {
    //     //for rest of the elements, show error in same way.
    //     error.insertAfter(element);
    //   }
    // },
    rules: {
      firstname: "required",
      lastname: "required",
      phone: "required",
      servicename: "required",
      email: {
        required: true,
        // Specify that email should be validated
        // by the built-in "email" rule
        email: true,
      },
      budget: "required",
      starttime: "required",
      prequirement: "required",
      projectbrief: {
        required: true,
        minlength: 25,
      },
    },
    // Specify validation error messages
    messages: {
      firstname: "Please enter your First Name",
      lastname: "Please enter your Last Name",
      phone: "Please enter your Phone Number",
      servicename: "Please select a Service",
      email: "Please enter a valid Email Address",
      budget: "Please select a valid Budget",
      starttime: "Please select Start Time",
      prequirement: "Please select Requirement",
      projectbrief: {
        required: "Please write brief Project Description",
        minlength: "Please write at least more than 25 characters",
      },
    },
    // Make sure the form is submitted to the destination defined
    // in the "action" attribute of the form when valid
    submitHandler: function (form) {
      form.submit();
    },
  });
});




