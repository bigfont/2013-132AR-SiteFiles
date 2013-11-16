/*jslint browser: true*/
/*global $, jQuery, CKEDITOR*/

(function () {

    "use strict";

    function ajaxSave(source, apiControllerName, doAlert) {

        var status;

        $.ajax({
            url: "/api/" + apiControllerName,
            dataType: "json",
            data: source,
            type: "POST"
        })
            .done(function (data) {

                status = data.status;

            })
            .fail(function (data) {

                status = data.status;

            })
            .always(function () {

                if (doAlert && status === 200) {

                    window.alert("Saved!");
                }

            });
    }

    CKEDITOR.replace('editor1');

    function saveEditor(doAlert) {
        var action_name, editor_data, source;

        action_name = $("#actionName").val();

        editor_data = CKEDITOR.instances.editor1.getData();
        $("#preview").html(editor_data);

        source = {
            'EditorData': editor_data,
            'ActionName': action_name
        };

        ajaxSave(source, "ckeditorapi", doAlert);
    }

    function saveContactInfo(doAlert) {
        var source;

        source = {

            'CompanyName': $("#CompanyName").val(),
            'Street': $("#Street").val(),
            'City': $("#City").val(),
            'Province': $("#Province").val(),
            'PostalCode': $("#PostalCode").val(),
            'Country': $("#Country").val(),
            'Phone': $("#Phone").val(),
            'Fax': $("#Fax").val(),
            'FirstName': $("#FirstName").val(),
            'LastName': $("#LastName").val(),
            'Email': $("#Email").val()

        };

        ajaxSave(source, 'ContactInfoApi', doAlert);
    }

    $("#btn-save").click(function () {

        saveEditor();
        saveContactInfo(true);

    });

}());

