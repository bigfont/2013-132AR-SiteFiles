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
        var status;
        var saved;

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

    });


})();

