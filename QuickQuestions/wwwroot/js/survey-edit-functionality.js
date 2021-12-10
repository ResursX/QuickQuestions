// Script for Survey Create and Edit forms dynamic functionality
const question_index = "<input name='Questions.Index' class='index' type='hidden' value='"
const answer_index_1 = "<input name='Questions[";
const answer_index_2 = "].Answers.Index' type='hidden' value='";
const index_end = "' />";

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

//Makes AJAX request to move content
function AnswerMove(questionIndexOld, indexOld, questionIndexNew, indexNew) {
    UpdateTextEdits();

    var data = $('#form').serializeArray();

    data.push({ name: "questionIndexOld", value: questionIndexOld });
    data.push({ name: "indexOld", value: indexOld });
    data.push({ name: "questionIndexNew", value: questionIndexNew });
    data.push({ name: "indexNew", value: indexNew });

    $.ajax({
        async: true,
        data: data,
        type: "POST",
        url: '/Admin/Surveys/MoveAnswer',
        success: function (partialView) {
            $(".questions-container").replaceWith(partialView);

            Initialize();
        }
    });
}

function RecalculateIndexes(container) {
    container.children().each(function (index) {
        $(this).children(".index-edit").val(index);
    });
}

function UpdateTextEdits() {
    $(".text-edit-input").each(function (i) {
        $(this).val($(this).siblings(".text-edit-field").contents().html());
    });
    $(".text-edit-summary").each(function (i) {
        $(this).val(Quill.find($(this).siblings(".text-edit-field").get(0)).getText());
    })
}

function Initialize() {
    var container = $(".questions-container");

    container.find(".text-edit-field").each(function (index) {
        $(this).append($(this).siblings(".text-edit-input").val());

        var quill = new Quill(this, quill_options);
    });

    container.children().each(function (i) {
        $(this).prepend(question_index + i + index_end);

        $(this).children(".edit-body").children(".answers").children(".answers-container").children().each(function (j) {
            $(this).prepend(answer_index_1 + i + answer_index_2 + j + index_end);
        });
    });

    container.sortable({
        group: {
            name: 'questions-group',
            pull: false,
            put: false
        },
        animation: 0,
        //easing: "cubic-bezier(1, 0, 0, 1)",
        handle: ".question-handle",
        draggable: ".question",
        //ghostClass: "bg-info",
        revertOnSpill: false,
        removeOnSpill: false,
        swapThreshold: 0.6,
        direction: "vertical",
        onUpdate: function (evt) {
            RecalculateIndexes($(evt.from));
        }
    });

    container.children(".question").each(function (i) {
        $(this).children(".edit-body").children(".answers").children(".answers-container").sortable({
            group: {
                name: 'answers-group',
                pull: ['answers-group'],
                put: ['answers-group']
            },
            animation: 0,
            //easing: "cubic-bezier(1, 0, 0, 1)",
            handle: ".answer-handle",
            draggable: ".answer",
            //ghostClass: "bg-info",
            revertOnSpill: false,
            removeOnSpill: false,
            swapThreshold: 0.6,
            direction: "vertical",
            onAdd: function (evt) {
                var questionOld = $(evt.from).parent().parent().siblings(".index-edit").val();
                var questionNew = $(evt.to).parent().parent().siblings(".index-edit").val();

                AnswerMove(questionOld, evt.oldDraggableIndex, questionNew, evt.newDraggableIndex);
            },
            onUpdate: function (evt) {
                RecalculateIndexes($(evt.from));
            }
        });
    });
}

Initialize();

//Init quill for survey text edit
$(".text-edit-survey").children(".text-edit-field").append($(".text-edit-survey").children(".text-edit-input").val());
var quill_survey = new Quill($(".text-edit-survey").children(".text-edit-field").get(0), quill_options);

$(document).ready(function () {
    $(document).on('submit', '#form', function () {
        UpdateTextEdits();

        return true;
    });

    $(document).on("click", ".add-question", function () {
        UpdateTextEdits();

        var data = $('#form').serialize();

        $.ajax({
            async: true,
            data: data,
            type: "POST",
            url: '/Admin/Surveys/AddQuestion',
            success: function (partialView) {
                $(".questions-container").replaceWith(partialView);

                Initialize();
            }
        });
    });

    $(document).on("click", ".add-answer", function () {
        UpdateTextEdits();

        var index = $(this).parent().parent().parent().siblings(".index-edit").val();
        var data = $('#form').serializeArray();

        data.push({ name: "index", value: index });

        $.ajax({
            async: true,
            data: data,
            type: "POST",
            url: '/Admin/Surveys/AddAnswer',
            success: function (partialView) {
                $(".questions-container").replaceWith(partialView);

                Initialize();
            }
        });
    });

    $(document).on("click", ".debug-reload", function () {
        UpdateTextEdits();

        var data = $('#form').serialize();

        $.ajax({
            data: data,
            type: "POST",
            url: '/Admin/Surveys/DebugReload',
            success: function (partialView) {
                $(".questions-container").replaceWith(partialView);

                Initialize();
            }
        });
    });

    $(document).on("click", ".remove-element", function () {
        var container = $(this).parent().parent().parent();

        $(this).parent().parent().remove();

        RecalculateIndexes(container);
    });
});