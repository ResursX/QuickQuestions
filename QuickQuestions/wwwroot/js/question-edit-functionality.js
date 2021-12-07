// Script for Question Create and Edit forms dynamic functionality
const answer_index = "<input name='Answers.Index' type='hidden' value='";
const index_end = "' />";

function RecalculateIndexes(container) {
    container.children().each(function (index) {
        $(this).children(".index-edit").val(index);
    });
}

function UpdateContainer(container) {

    container.children().each(function (i) {
        $(this).prepend(answer_index + i + index_end);
    });

    container.sortable({
        group: {
            name: 'answers-group',
            pull: false,
            put: false
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
        onUpdate: function (evt) {
            RecalculateIndexes($(evt.from));
        }
    });
}

UpdateContainer($(".questions-container"));

$(document).ready(function () {
    $(document).on("click", ".add-answer", function () {
        var data = $('#form').serialize();

        $.ajax({
            async: true,
            data: data,
            type: "POST",
            url: '/Admin/Questions/AddAnswer',
            success: function (partialView) {
                $(".answers-container").replaceWith(partialView);

                UpdateContainer($(".answers-container"));
            }
        });
    });

    $(document).on("click", ".debug-reload", function () {
        var data = $('#form').serialize();

        $.ajax({
            data: data,
            type: "POST",
            url: '/Admin/Questions/DebugReload',
            success: function (partialView) {
                $(".answers-container").replaceWith(partialView);

                UpdateContainer($(".answers-container"));
            }
        });
    });

    $(document).on("click", ".remove-element", function () {
        var container = $(this).parent().parent().parent();

        $(this).parent().parent().remove();

        RecalculateIndexes(container);
    });
});