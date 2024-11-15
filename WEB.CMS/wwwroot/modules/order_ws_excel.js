
$('#import-file-ws').change(function () {
    $('#file_import_hotel_error_message').addClass('mfp-hide');
    _order_ws_excel.UploadFileExcel();
});
var _order_ws_excel = {
    ImportWSExcel: function () {
        let title = 'Tải lên danh sách đơn Thể thao biển';
        let url = '/Order/ImportWSExcel';
        let param = {};
        _magnific.OpenSmallPopup(title, url, param);
    },
    UploadFileExcel: function () {
        let url = '/Order/ImportWSExcelListing';
        let file = document.getElementById("import-file-ws").files[0];

        var file_type = file['type'];
        if (file_type !== "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet") {
            $('#import-ws-error').removeClass('mfp-hide');
            $('#grid_ws').html('');
            return;
        }
        $('#import-ws-error').hide()

        let formData = new FormData();
        formData.append("file", file)
        _global_function.AddLoading()

        $.ajax({
            url: url,
            type: "POST",
            data: formData,
            processData: false,
            contentType: false,
            success: function (result) {
                _global_function.RemoveLoading()

                if (result != null) {
                    $('#grid_ws').html(result);
                    $('#confirm-ws-import').show()
                } else {
                    $('#import-ws-error').removeClass('mfp-hide');
                }
            }, error: function (error) {
                console.log(error);
                $('#import-ws-error').removeClass('mfp-hide');
            }
        });
    },
    ConfirmImport: function () {
        let url = '/Order/ImportWSExcelUpload';
        let data = [];
        $('#import-ws-error').hide()
        $('#confirm-ws-import').hide()
        $('#grid_ws tbody tr').each(function () {
            let seft = $(this);
            data.push({
                client_code: seft.find('.client_code').text(),
                label: seft.find('.label').text(),
                used_date_str: seft.find('.used_date').text(),
                client_name: seft.find('.client_name').text(),
                conf_no: seft.find('.conf_no').text(),
                room_no: seft.find('.room_no').text(),
                serial_no: seft.find('.serial_no').text(),
                product_name: seft.find('.product_name').text(),
                service_type: seft.find('.service_type').text(),
                quanity: seft.find('.quanity').text(),
                base_price: seft.find('.base_price').text(),
                amount: seft.find('.amount').text(),
                amount_before_vat: seft.find('.amount_before_vat').text(),
                vat_value: seft.find('.vat_value').text(),
                note: seft.find('.note').text(),
            });

        });
        debugger
        _global_function.AddLoading()
        $.ajax({
            url: url,
            type: "POST",
            data: { model: JSON.stringify(data) },
            success: function (result) {
                _global_function.RemoveLoading()
                $('#grid_ws').html(result);
            }
        });
    }
}