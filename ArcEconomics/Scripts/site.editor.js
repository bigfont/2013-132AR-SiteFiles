(function () {

    function AjaxSave(source, apiControllerName, doAlert) {

        var jqxhr;

        jqxhr = $.ajax({
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

                if (alert && status == 200) {

                    window.alert("Saved!");
                }

            });

        // Perform other work here ...
        // Set another completion function for the request above
        jqxhr.always(function () {

        });
    }

    CKEDITOR.replace('editor1')

    function SaveEditor(doAlert)
    {
        var action_name;
        var editor_data;
        var source;
        var status;
        var saved;

        action_name = $("#actionName").val();

        editor_data = CKEDITOR.instances.editor1.getData();
        $("#preview").html(editor_data);

        source = {
            'EditorData': editor_data,
            'ActionName': action_name
        }

        AjaxSave(source, "ckeditorapi", doAlert);
    }

    function SaveContactInfo(doAlert)
    {
        var source;

        source = {

            'CompanyName': $("#CompanyName").val(),
            'Street': $("#Street").val(),
            'City': $("#City").val(),
            'Province': $("#Province").val(),
            'PostalCode': $("#PostalCode").val(),
            'Phone': $("#Phone").val(),
            'FirstName': $("#FirstName").val(),
            'LastName': $("#LastName").val(),
            'Email': $("#Email").val()

        }

        AjaxSave(source, 'ContactInfoApi', doAlert);
    }

    $("#btn-save").click(function () {

        SaveEditor();
        SaveContactInfo();

    });

})();

