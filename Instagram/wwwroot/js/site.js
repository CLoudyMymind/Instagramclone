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
});
var deleteButton = document.getElementById('deleteButton');
deleteButton.addEventListener('click', function(e) {
    e.preventDefault(); 

    var postId = deleteButton.getAttribute('data-post-id'); 
    $.ajax({
        url: '/Posts/DeletePost',
        type: 'POST',
        data: { id: postId }, 
        success: function(result) {
            if (result.success) {
                deleteButton.closest('.post').remove();
            }
        }
    });
});
