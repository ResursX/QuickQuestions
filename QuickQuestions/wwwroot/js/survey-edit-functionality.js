// Script for Survey Create and Edit forms dynamic functionality
const question_index = "<input name='Questions.Index' class='index' type='hidden' value='"
const answer_index_1 = "<input name='Questions[";
const answer_index_2 = "].Answers.Index' type='hidden' value='";
const index_end = "' />";

//Makes AJAX request to move content
function AnswerMove(questionIndexOld, indexOld, questionIndexNew, indexNew) {
    console.log(questionIndexOld);
    console.log(indexOld);
    console.log(questionIndexNew);
    console.log(indexNew);

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

            UpdateContainer($(".questions-container"));
        }
    });
}

function RecalculateIndexes(container) {
    container.children().each(function (index) {
        $(this).children(".index-edit").val(index);
    });
}

function UpdateContainer(container) {

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
        animation: 150,
        easing: "cubic-bezier(1, 0, 0, 1)",
        handle: ".question-handle",
        draggable: ".question",
        ghostClass: "bg-info",
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
            animation: 150,
            easing: "cubic-bezier(1, 0, 0, 1)",
            handle: ".answer-handle",
            draggable: ".answer",
            ghostClass: "bg-info",
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

UpdateContainer($(".questions-container"));

$(document).ready(function () {
    $(document).on("click", ".add-question", function () {
        var data = $('#form').serialize();

        $.ajax({
            async: true,
            data: data,
            type: "POST",
            url: '/Admin/Surveys/AddQuestion',
            success: function (partialView) {
                $(".questions-container").replaceWith(partialView);

                UpdateContainer($(".questions-container"));
            }
        });
    });

    $(document).on("click", ".add-answer", function () {
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

                UpdateContainer($(".questions-container"));
            }
        });
    });

    $(document).on("click", ".debug-reload", function () {
        var data = $('#form').serialize();

        $.ajax({
            data: data,
            type: "POST",
            url: '/Admin/Surveys/DebugReload',
            success: function (partialView) {
                $(".questions-container").replaceWith(partialView);

                UpdateContainer($(".questions-container"));
            }
        });
    });

    $(document).on("click", ".remove-element", function () {
        var container = $(this).parent().parent().parent();

        $(this).parent().parent().remove();

        RecalculateIndexes(container);
    });
});