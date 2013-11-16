(function () {

    function AjaxSave(source, apiControllerName) {

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

                if (status == 200) {

                    saved = $('<div id="editor1-saved" class="alert alert-success alert-dismissable">' +
                            '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                            '<strong>Saved!</strong> Your content is safe with us.' +
                            '</div>');

                    saved.prependTo("body").fadeOut(5000, null);

                }

            });

        // Perform other work here ...
        // Set another completion function for the request above
        jqxhr.always(function () {

        });
    }

    CKEDITOR.replace('editor1')

    function SaveEditor()
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

        AjaxSave(source, "ckeditorapi");
    }

    function SaveContactInfo()
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

        AjaxSave(source, 'ContactInfoApi')
    }

    $("#btn-save").click(function () {

        SaveEditor();
        SaveContactInfo();

    });

})();

