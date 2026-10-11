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
    // Remove the localized gallery prefix before calling the image API.
    var apiPath = galleryUrl.replace(/^\/(?:Bilder|images)\//i, '');
    // Koda URL korrekt (bevara "/" men koda ÅÄÖ etc.)
    var encodedPath = apiPath.split('/').map(encodeURIComponent).join('/');
    var apiUrl = '/api/Bilder/' + encodedPath + '?page=' + _nextPageToLoad
        + '&culture=' + (document.documentElement.lang === 'en' ? 'en-US' : 'sv-SE');

    try {
        var response = await fetch(apiUrl);
        if (!response.ok) throw new Error('API-svar: ' + response.status);

        var data = await response.json();

        if (data.images && data.images.length > 0) {
            var pageOffset = (_nextPageToLoad - 1) * _pageSize;

            data.images.forEach(function (img, idx) {
                var imgNum = pageOffset + idx + 1;
                if (!img.urlImage || !/^bilder\//i.test(img.urlImage) || img.urlImage.includes('..')) return;
                var imgSrc = 'https://arvidsonfoto.se/' + img.urlImage.split('/').map(encodeURIComponent).join('/') + '.jpg';
                var imgName = (img.urlImage || '').split('/').pop() + '.jpg';
                var imgDate = img.dateImageTaken
                    ? img.dateImageTaken.substring(0, 10)
                    : _galleryEl.dataset.imageMissing;
                var categoryName = escapeGalleryHtml(img.categoryName || img.name || '');
                var imgHref = escapeGalleryHtml(galleryUrl);
                var imageId = Number.isSafeInteger(img.imageId) && img.imageId > 0 ? img.imageId : '';
                var description = img.description ? ', ' + escapeGalleryHtml(img.description) + '.' : '';

                var descHtml = '<div class="row">'
                    + '<p>' + formatGalleryText(_galleryEl.dataset.imagePosition, imgNum, _totalImages) + ' \u2013 '
                    + '<a href="' + imgHref + '" title="' + formatGalleryText(_galleryEl.dataset.imageViewAll, categoryName) + '">' + categoryName + '</a>'
                    + description + ' ' + escapeGalleryHtml(_galleryEl.dataset.imageDate) + ': ' + escapeGalleryHtml(imgDate) + '</p>'
                    + '<p>' + escapeGalleryHtml(_galleryEl.dataset.imageFilename) + ': ' + escapeGalleryHtml(imgName)
                    + ' <a rel="nofollow" href="' + escapeGalleryHtml(_galleryEl.dataset.imageCopyrightUrl) + '">\u00A9 Torbj\u00F6rn Arvidson</a>.'
                    + ' ' + escapeGalleryHtml(_galleryEl.dataset.imageBuyPrompt) + ' <a rel="nofollow" href="'
                    + escapeGalleryHtml(_galleryEl.dataset.imageBuyUrl) + '?imgId=' + imageId + '">'
                    + escapeGalleryHtml(_galleryEl.dataset.imageBuyLabel) + '</a></p>'
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

function escapeGalleryHtml(value) {
    return String(value ?? '').replace(/[&<>"']/g, ch => ({
        '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;'
    })[ch]);
}

function formatGalleryText(template, ...values) {
    return escapeGalleryHtml(template).replace(/\{(\d+)\}/g, (_, index) =>
        values[Number(index)] === undefined ? '' : String(values[Number(index)]));
}

document.addEventListener('DOMContentLoaded', function () {
    initGalleryInfiniteScroll();
});