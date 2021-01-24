var lightbox = GLightbox({
    moreLength: 0 //Döljer texten: "See more" i mobil vyn. Alternativt kan man sätta den till x antal tecken... Vill man ändra See More, så är det "moreText: "Se mer...""
});

lightbox.on('open', (target) => {
    console.log('lightbox opened');
});
    //var lightboxDescription = GLightbox({
    //    selector: '.glightbox2'
    //});
    //var lightboxVideo = GLightbox({
    //    selector: '.glightbox3'
    //});
    //lightboxVideo.on('slide_changed', ({prev, current}) => {
    //    console.log('Prev slide', prev);
    //    console.log('Current slide', current);

    //    const {slideIndex, slideNode, slideConfig, player} = current;

    //    if (player) {
    //        if (!player.ready) {
    //            // If player is not ready
    //            player.on('ready', (event) => {
    //                // Do something when video is ready
    //            });
    //        }

    //        player.on('play', (event) => {
    //            console.log('Started play');
    //        });

    //        player.on('volumechange', (event) => {
    //            console.log('Volume change');
    //        });

    //        player.on('ended', (event) => {
    //            console.log('Video ended');
    //        });
    //    }
    //});

    //var lightboxInlineIframe = GLightbox({
    //    selector: '.glightbox4'
    //});