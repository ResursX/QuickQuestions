// Script for Survey cration and deletion forms dynamic functionality

const question_index = "<input name='Questions.Index' class='index' type='hidden' value='"
const answer_index_1 = "<input name='Questions[";
const answer_index_2 = "].Answers.Index' type='hidden' value='";
const index_end = "' />";

function RecalculateIndexes(container) {
    container.children().each(function (index) {
        $(this).children(".index-edit").val(index);
    });
}

function UpdateContainer(container) {
    container.children().each(function (i) {
        $(this).prepend(question_index + i + index_end);

        $(this).children(".answers").children(".answers-container").children().each(function (j) {
            $(this).prepend(answer_index_1 + i + answer_index_2 + j + index_end);
        })
    });

    container.sortable({
        handle: ".question-handle",
        axis: "y",
        cursor: "move",
        revert: 100,
        stop: function (event, ui) {
            RecalculateIndexes(ui.item.parent());
        }
    });

    container.children().each(function (i) {
        $(this).children(".answers").children(".answers-container").sortable({
            handle: ".answer-handle",
            axis: "y",
            cursor: "move",
            revert: 100,
            stop: function (event, ui) {
                RecalculateIndexes(ui.item.parent());
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

                var container = $(".questions-container");

                UpdateContainer(container);
            }
        });
    });

    $(document).on("click", ".add-answer", function () {
        var index = $(this).parent().parent().siblings(".index-edit").val();
        var data = $('#form').serializeArray();

        data.push({ name: "index", value: index });

        $.ajax({
            async: true,
            data: data,
            type: "POST",
            url: '/Admin/Surveys/AddAnswer',
            success: function (partialView) {
                $(".questions-container").replaceWith(partialView);

                var container = $(".questions-container");

                UpdateContainer(container);
            }
        });
    });

    $(document).on("click", ".debug-reload", function () {
        var t = this;
        var data = $('#form').serialize();

        $.ajax({
            data: data,
            type: "POST",
            url: '/Admin/Surveys/DebugReload',
            success: function (partialView) {
                $(t).parent().siblings(".questions-container").replaceWith(partialView);

                var container = $(t).parent().siblings(".questions-container");

                UpdateContainer(container);
            }
        });
    });

    $(document).on("click", ".remove-element", function () {
        var $container = $(this).parent().parent().parent();

        $(this).parent().parent().remove();

        RecalculateIndexes($container);
    });
});