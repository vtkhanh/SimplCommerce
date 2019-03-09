/*global jQuery, window*/
(function ($) {
    $(window).load(function () {
        $('.lang-selector li').on('click', function (e) {
            var lang = $(this).find('a').attr('data-value'),
                $langForm = $('#lang-form'),
                $cultureInput = $langForm.find('input[name=culture]');

            if ($cultureInput.val() === lang) {
                e.preventDefault();
                return;
            } else {
                $cultureInput.val(lang);
                $langForm.submit();
            }
        });

        if (typeof $('.product-list .thumbnail').matchHeight === "function") {
            $('.product-list .thumbnail').matchHeight({
                byRow: true,
                property: 'height',
                target: null,
                remove: false
            });
        }

        if (window.simplGlobalSetting) {
            $('input.rating-loading').rating({
                language: window.simplGlobalSetting.lang,
                filledStar: '<i class="fa fa-star"></i>',
                emptyStar: '<i class="fa fa-star-o"></i>'
            });
        }
    });
})(jQuery);


Number.prototype.toCurrency = function () {
    // TODO: Currently use US format hard-codedly
    return this.toLocaleString('vi-VN', {
        style: 'currency',
        currency: 'VND'
    });
}

function downloadFile(data) {
    const headers = data.headers();
    const contentDisposition = headers['content-disposition'] || '';
    const filename = /filename[^;=\n]*=((['"]).*?\2|[^;\n]*)/.exec(contentDisposition)[1].replace(/"/g, '');

    const contentType = headers['content-type'];

    const linkElement = document.createElement('a');
    try {
        const blob = new Blob([data.data], { type: contentType });
        const url = window.URL.createObjectURL(blob);

        linkElement.setAttribute('href', url);
        linkElement.setAttribute("download", filename);

        const clickEvent = new MouseEvent("click", {
            "view": window,
            "bubbles": true,
            "cancelable": false
        });
        linkElement.dispatchEvent(clickEvent);
    } catch (ex) {
        console.log(ex);
    }
}
