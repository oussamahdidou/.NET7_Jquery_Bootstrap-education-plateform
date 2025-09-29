$(".level-slider").slick({
    infinite: true,
    slidesToShow: 2,
    slidesToScroll: 1,
    dots: true,
    autoplay: true, // Enable auto-scrolling
    autoplaySpeed: 2000,
    cssEase: "linear",
    arrows: false,
});
$(document).ready(function () {
    let playbutton = $(".video-control-play");
    let pausebutton = $(".video-control-pause");
    let video = $("video")[0]; // Get the DOM element of the video

    // Play/Pause video on click of the buttons.
    playbutton.click(function () {
        $(this).hide();
        video.play();
        pausebutton.show();
        $(".video-overlay").css("background-color", "transparent");
    });

    pausebutton.click(function () {
        $(this).hide();
        video.pause();
        playbutton.show();
        $(".video-overlay").css("background-color", "rgba(0,0,0,0.5)");
    });
});
