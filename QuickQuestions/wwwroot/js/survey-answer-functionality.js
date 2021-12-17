// Script for Survey Answer forms rich text edit functionality

//Quill toolbar button handlers
function imageHandler() {
    var range = this.quill.getSelection();
    var url = prompt("Enter image URL");

    if (url != null) {
        this.quill.insertEmbed(range.index, 'image', url, Quill.sources.USER);
    }
}

const quill_options = {
    theme: "snow",
    modules: {
        toolbar: {
            container: [
                [{ 'header': [1, 2, 3, 4, 5, 6, false] }],

                [{ 'color': [] }, { 'background': [] }],

                ['bold', 'italic', 'underline', 'strike'],
                [{ 'script': 'sub' }, { 'script': 'super' }],

                [{ 'list': 'ordered' }, { 'list': 'bullet' }],
                [{ 'align': [] }],
                [{ 'indent': '-1' }, { 'indent': '+1' }],

                ['blockquote', 'code-block'],

                ['link', 'image', 'video'],

                ['clean']
            ],
            handlers: {
                'image': imageHandler
            }
        }
    }
}

function UpdateTextEdits() {
    $(".rich-input").each(function (i) {
        $(this).val($(this).siblings(".rich-edit").contents().html());
    });
}

function Initialize() {
    $(".rich-edit").each(function (index) {
        var quill = new Quill(this, quill_options);
    });
}

Initialize();

$(document).ready(function () {
    $(document).on('submit', '#form', function () {
        UpdateTextEdits();

        return true;
    });
});