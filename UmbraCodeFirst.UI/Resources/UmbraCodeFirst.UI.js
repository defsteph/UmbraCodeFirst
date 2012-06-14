var UmbraCodeFirst = {
    UI: {
        EnlargePreview: function (id) {
            var iframe = $('#' + id).addClass('tabpageContent').css('width', '98%').css('height', '99%').css('border', '0');
            iframe.parents('.tabpageContent').replaceWith(iframe);
        }
    }
};