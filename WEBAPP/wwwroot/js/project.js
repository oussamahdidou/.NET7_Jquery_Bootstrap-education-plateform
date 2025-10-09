
//Search Algorithmes
$(document).ready(function () {
    // Attach an input event to the filter input
    $("#search").on("input", function () {
        var filterText = $(this).val().toLowerCase();

        $(".cours-container").each(function () {
            var $course = $(this);
            var courseTitle = $course.find("h4").text().toLowerCase();
            var courseDescription = $course.find("p").text().toLowerCase();

            // Check if the course title contains the filter text
            if (
                courseTitle.includes(filterText) ||
                courseDescription.includes(filterText)
            ) {
                $course.show();
                // Show the course if it matches the filter
            } else {
                $course.hide(); // Hide the course if it doesn't match the filter
            }
        });
        notfound();
    });
});
function notfound() {
    var allHidden = true; // Assume all elements are hidden initially

    $(".cours-container").each(function () {
        var element = $(this);

        if (element.css("display") !== "none") {
            allHidden = false; // At least one element is not hidden
            // Exit the loop early
        }
    });

    if (allHidden) {
        $(".not-found").show();
    } else {
        $(".not-found").hide();
    }
}
$(document).ready(function () {
    $(".like-container").click(function () {
        var item = $(this).attr("id");
        var clickedContainer = $(this);

        $.ajax({
            url: '/Application/Like',
            method: 'POST',
            data: { number: item },
            success: function (result) {
                console.log(result);
                clickedContainer.find(".likes").text(result + " Like");
                console.log("Like container clicked.");
                clickedContainer.find(".fa-solid.fa-heart").toggleClass("checked unchecked");
            },
            error: function (error) {
                console.log(error.status);
                if (error.status == 401) {
                    Swal.fire({
                        title: 'Connection',
                        text: 'you need connection to access to this feature',
                        icon: 'info',
                        showCancelButton: true,
                        confirmButtonText: 'Login',
                        cancelButtonText: 'Register',
                    }).then((result) => {
                        if (result.isConfirmed) {
                            window.location.href = 'Account/Login';
                        } else if (result.dismiss === Swal.DismissReason.cancel) {
                            window.location.href = 'Account/Register';
                        }
                    });
                }

            }
        });
    });
});
var itemtodelete;
var cardbutton;
$(document).ready(function () {
    $(".delete").click(function () {
        console.log("desactivate button");
        $(".model-layer").css("display", "flex");
        itemtodelete = $(this).attr("id");
        cardbutton = $(this);
    });
    $(".desactivate").click(function () {
        console.log("desactivate button" + itemtodelete);
        $(".model-layer").hide();
        $.ajax({
            url: '/Application/Delete', // Replace with your controller and action route
            method: 'POST',
            data: { number: itemtodelete }, // Send the number as a parameter
            success: function (result) {
                // Handle the response from the server
                console.log(result);
                Swal.fire(
                    'Deleted!',
                    'Your file has been deleted.',
                    'success'
                )
                cardbutton.closest(".cours-container").remove();

            },
            error: function (error) {
                console.error(error);
            }
        });

    });
    $(".cancel").click(function () {
        $(".model-layer").hide();
    });
});
