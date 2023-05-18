$.ajaxSetup({
    headers: {"RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val()}
});
$(function() {
    $(document).on('click', '.subscribe-link', function(event) {
        event.preventDefault();
        var userId = $(this).attr('userId');
        var folowersCount = $('.profile-following').text();
        console.log(userId);

        var requestVerificationToken = $('input[name="__RequestVerificationToken"]').val();
        var headers = {
            "RequestVerificationToken": requestVerificationToken
        };

        $.ajaxSetup({
            headers: headers
        });

        var subscribeLink = $(this);

        $.post('/Profiles/Follow/', {id: userId})
            .done(function(response) {
                if (!subscribeLink.hasClass('subscribed')) {
                    subscribeLink.text('Отписаться').addClass('subscribed btn btn-outline-danger');
                    $('.profile-following').text(parseInt(folowersCount) + 1);
                } else {
                    subscribeLink.text('Подписаться').removeClass('subscribed btn btn-outline-danger').addClass('btn btn-outline-primary');
                    $('.profile-following').text(parseInt(folowersCount) - 1);
                }
            })
            .fail(function(error) {
                console.log(error);
            });
    });

    $(document).on('click', '#deleteButton', function (event) {
        event.preventDefault();
        const postId = $(this).attr('postId');
        const countPosts = $('.profile-posts').text();
        console.log(countPosts);
        console.log(postId);

        $.ajax({
            type: 'POST',
            url: '/Posts/DeletePost/',
            data: {id: postId},
            success: function () {
                $(`.post-${postId}`).remove();
                $('.profile-posts').text(`${parseInt(countPosts) - 1}`);
            },
            error: function (error) {
                console.log(error);
            }
        });
    });
});

$(document).ready(function() {
    $('#likeForm').submit(function(e) {
        e.preventDefault();
        var form = $(this);
        var url = form.attr('action');

        console.log(form.serialize())
        $.ajax({
            type: 'POST',
            url: url,
            data: form.serialize(),
            success: function(response) {
                if (response.success) {
                    $('.like-count').text(response.likeCount); 
                } else {
                    console.log(response.message);
                }
            },
            error: function(xhr) {
                console.log(xhr.responseText);
            }
        });
    });
});
