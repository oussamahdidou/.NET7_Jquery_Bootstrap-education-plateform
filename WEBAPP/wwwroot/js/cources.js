
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
