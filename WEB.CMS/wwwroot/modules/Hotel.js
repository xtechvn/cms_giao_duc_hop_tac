
$('#file_import_hotel').change(function () {
    $('#file_import_hotel_error_message').addClass('mfp-hide');
    _hotel.UploadFileExcel();
});

var _hotel = {
    NewHotel: function () {
        let title = 'Thêm Khách sạn';
        let url = '/Hotel/UpsertHotel';
        let param = {};
        _magnific.OpenSmallPopup(title, url, param);
    },
    UploadFileExcel: function () {
        let url = '/Hotel/ImportHotel';
        let file = document.getElementById("file_import_hotel").files[0];

        var file_type = file['type'];
        if (file_type !== "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet") {
            $('#file_import_hotel_error_message').removeClass('mfp-hide');
            $('#grid_hotel').html('');
            return;
        }

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
                    $('#grid_hotel').html(result);
                    //$('#grid_vinwonder_price .scrollbar-block').scrollbar();
                   
                } else {
                    $('#file_import_hotel_error_message').removeClass('mfp-hide');
                }
            }, error: function (error) {
                console.log(error);
                $('#file_import_hotel_error_message').removeClass('mfp-hide');
            }
        });
    },
    UpsertCampaign: function () {
        let url = '/Hotel/SetUpHotel';
        let hotel = [];
        $('#grid_hotel tbody tr').each(function () {
            let seft = $(this);
                hotel.push({
                    Name: seft.find('.item_name').text(),
                    Street: seft.find('.item_street').text(),
                    AccountNumber: seft.find('.bank_id').text(),
                    BankName: seft.find('.bank_name').text(),
                    Branch: seft.find('.branch').text(),
                    BankAccountName: seft.find('.account_name').text(),
                });
        
        });
        _global_function.AddLoading()
        _ajax_caller.post(url, { model: hotel }, function (result) {
            if (result.isSuccess) {
                _global_function.RemoveLoading()
                _msgalert.success(result.message);
                $.magnificPopup.close();
                _supplier_service.ReLoad();
            } else {
                _global_function.RemoveLoading()
                _msgalert.error(result.message);
            }
        });
    }
}