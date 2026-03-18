var lightbox = GLightbox({
    moreLength: 0 //Döljer texten: "See more" i mobil vyn. Alternativt kan man sätta den till x antal tecken... Vill man ändra See More, så är det "moreText: "Se mer...""
});

lightbox.on('open', () => {
    console.log('lightbox opened');
});

// === Infinite scroll: ladda fler bilder från nästa sida när användaren bläddrar nära slutet ===
var _galleryEl = null;
var _nextPageToLoad = -1;
var _totalPages = -1;
var _isLoadingMore = false;
var _totalImages = 0;
var _pageSize = 48;

function initGalleryInfiniteScroll() {
    _galleryEl = document.getElementById('gallery');
    if (!_galleryEl) return;

    var currentPage = parseInt(_galleryEl.dataset.currentPage, 10) || -1;
    _totalPages = parseInt(_galleryEl.dataset.totalPages, 10) || -1;
    _pageSize = parseInt(_galleryEl.dataset.pageSize, 10) || 48;
    _totalImages = parseInt(_galleryEl.dataset.totalImages, 10) || 0;

    // Aktivera infinite scroll om det finns flera sidor och vi är på en giltig sida
    if (_totalPages > 1 && currentPage > 0) {
        _nextPageToLoad = currentPage + 1;
    }
}

lightbox.on('slide_changed', function ({ prev, current }) {
    if (!_galleryEl || _isLoadingMore || _nextPageToLoad < 0 || _nextPageToLoad > _totalPages) return;

    var slideIndex = current.slideIndex;
    var totalSlides = lightbox.elements.length;

    // Ladda nästa sida när 3 bilder från slutet
    if (slideIndex >= totalSlides - 3) {
        loadNextGalleryPage();
    }
});

async function loadNextGalleryPage() {
    if (_isLoadingMore || _nextPageToLoad < 0 || _nextPageToLoad > _totalPages) return;
    _isLoadingMore = true;

    var galleryUrl = _galleryEl.dataset.galleryUrl; // e.g. "/Bilder/Fåglar/Svanar/Sångsvan"
    // Ta bort det inledande "/Bilder/" för att få API-sökvägen
    var apiPath = galleryUrl.replace(/^\/Bilder\//i, '');
    // Koda URL korrekt (bevara "/" men koda ÅÄÖ etc.)
    var encodedPath = apiPath.split('/').map(encodeURIComponent).join('/');
    var apiUrl = '/api/Bilder/' + encodedPath + '?page=' + _nextPageToLoad;

    try {
        var response = await fetch(apiUrl);
        if (!response.ok) throw new Error('API-svar: ' + response.status);

        var data = await response.json();

        if (data.images && data.images.length > 0) {
            var pageOffset = (_nextPageToLoad - 1) * _pageSize;

            data.images.forEach(function (img, idx) {
                var imgNum = pageOffset + idx + 1;
                var imgSrc = 'https://arvidsonfoto.se/' + img.urlImage + '.jpg';
                var imgName = (img.urlImage || '').split('/').pop() + '.jpg';
                var imgDate = img.dateImageTaken
                    ? img.dateImageTaken.substring(0, 10)
                    : 'saknas';
                var categoryName = img.categoryName || img.name || '';
                var imgHref = img.urlCategory || '';
                var imageId = img.imageId || '';

                var descHtml = '<div class="row">'
                    + '<p>' + imgNum + ' av ' + _totalImages + ' \u2013 '
                    + '<a href="' + imgHref + '" title="Se alla bilder av ' + categoryName + '">' + categoryName + '</a>'
                    + ' Fotodatum: ' + imgDate + '</p>'
                    + '<p>Bildnamn: ' + imgName
                    + ' <a rel="nofollow" href="/Info/Copyright">\u00A9 Torbj\u00F6rn Arvidson</a>.'
                    + ' Intresserad av bilden? Se: <a rel="nofollow" href="/Info/Kop_av_bilder?imgId=' + imageId + '">K\u00F6p av bilder</a></p>'
                    + '</div>';

                lightbox.insertSlide({
                    href: imgSrc,
                    title: categoryName,
                    description: descHtml,
                    type: 'image',
                    descPosition: 'bottom'
                });
            });

            _nextPageToLoad++;
        }
    } catch (err) {
        console.error('Kunde inte ladda nästa gallerisida:', err);
    } finally {
        _isLoadingMore = false;
    }
}

document.addEventListener('DOMContentLoaded', function () {
    initGalleryInfiniteScroll();
});