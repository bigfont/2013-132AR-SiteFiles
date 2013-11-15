(function () {

    CKEDITOR.replace('editor1')

    $("#editor1-preview").click(function () {

        var editor_data = CKEDITOR.instances.editor1.getData();
        $("#preview").html(editor_data);

    });

    $("#editor1-save").click(function () {

        var action_name;
        var editor_data;
        var source;
        var jqxhr;

        action_name = $("#actionName").val();

        editor_data = CKEDITOR.instances.editor1.getData();
        $("#preview").html(editor_data);

        source = {
            'EditorData': editor_data,
            'ActionName': action_name
        }

        jqxhr = $.ajax({
            url: "/api/homeapi",
            dataType: "json",
            data: source,
            type: "POST"
        })
            .done(function (data) {
                console.log(data.responseText);
            })
            .fail(function (data) {
                console.log(data.responseText);
            })
            .always(function () {

            });

        // Perform other work here ...
        // Set another completion function for the request above
        jqxhr.always(function () {

        });

    });


})();

