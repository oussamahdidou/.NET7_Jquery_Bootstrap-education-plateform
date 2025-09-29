$(document).ready(function () {
    $(".follow").click(function () {
        var item = $(this).attr("id");

        var clickedContainer = $(this); // Store a reference to the clicked container

        $.ajax({
            url: '/User/Follow', // Replace with your controller and action route
            method: 'POST',
            data: { number: item }, // Send the number as a parameter
            success: function (result) {
                clickedContainer.text(result.buttonStatus);
                $("#following").text(result.followers);
                $("#followers").text(result.following)


            },
            error: function (error) {
                console.error(error);
            }
        });
    });


    // Function to execute after 5 seconds







});
$(document).ready(function () {

    function executeFunction() {
        $.ajax({
            url: '/User/Checknotificationsupdate',
            method: 'Get',
            success: function (result) {
                console.log(result);
                console.log(result + " new notifications");
                $(".notificationsCount").text(result + " new notifications");
                if (result != 0) {
                    $(".fa-solid.fa-circle.fa-beat").show();
                }
            },
            error: function (error) {
                console.error(error);
            }
        });

    }
    let timer = setInterval(function () {
        executeFunction();
    }, 2000);
    $(".fa-bell").click(function () {

        $.ajax({
            url: '/User/Notification', // Replace with your controller and action route
            method: 'Get',
            // Send the number as a parameter
            success: function (result) {
                $(".fa-solid.fa-circle.fa-beat").hide();
                $(".notificationsCount").text("0 new notifications");
                console.log(result);
                $(".notifications").empty();
                const options = { month: 'numeric', day: 'numeric', hour: 'numeric', minute: 'numeric' };
                $.each(result, function (index, notification) {
                    // Create a new list item for each object
                    var listItem = $('<li>');
                    listItem.addClass("notification p-3");
                    var heading = $('<h5>').text(notification.title);
                    var paragraph = $('<h6>').text(notification.message);
                    var date = $('<p>').text(new Date(notification.eventTime).toLocaleDateString(undefined, options));
                    listItem.append(heading, paragraph, date);

                    // Prepend the list item to the beginning of the ul
                    $(".notifications").append(listItem);
                });
                $(".notifications-container").toggle();
            },
            error: function (error) {
                console.error(error);
            }
        });
    });
});
