
var _vin_wonder = {
    Init: function () {
        //this.elModal = $('#global_modal_popup');
        //this.supplier_id = $('#global_supplier_id').val();
        //this.validImageTypes = ['image/gif', 'image/jpeg', 'image/png'];
        //this.validImageSize = 5 * 1024 * 1024;
        //this.noImageSrc = "/images/icons/noimage.png";
        //this.ListingRoomParams = {
        //    supplier_id: this.supplier_id,
        //    page_index: 1,
        //    page_size: 10
        //};

        //this.ListingRoom(this.ListingRoomParams);
    },

    GetFormData: function ($form) {
        var unindexed_array = $form.serializeArray();
        var indexed_array = {};

        $.map(unindexed_array, function (n, i) {
            indexed_array[n['name']] = n['value'];
        });

        return indexed_array;
    },

    ChangeButtonCommonProfit: function (type) {
        if (type == 1) {
            $('#ip_common_profit').prop('disabled', false);
            $('#vinwonder_common_profit').css('background', '#fff');
            $('#ip_common_profit').focus();
            $('#btn_save_common_profit, #btn_cancel_common_profit').removeClass('mfp-hide');
            $('#btn_edit_common_profit').addClass('mfp-hide');
        } else {
            $('#ip_common_profit').prop('disabled', true);
            $('#vinwonder_common_profit').css('background', '#F3F3F3');
            $('#btn_save_common_profit, #btn_cancel_common_profit').addClass('mfp-hide');
            $('#btn_edit_common_profit').removeClass('mfp-hide');
        }
    },

    UpdateCommonProfit: function (type, value) {
        let url = '/VinWonderPolicy/UpdateCommonProfit';
        let data_value = ConvertMoneyToNumber(value);

        if (type == 1 && data_value > 100) {
            _msgalert.error("Giá trị phần trăm không được lớn hơn 100%");
            return;
        }

        _ajax_caller.post(url, { type: type, value: data_value }, function (result) {
            if (result.isSuccess) {
                _msgalert.success(result.message);
                _vin_wonder.ChangeButtonCommonProfit(0);
                $(`#vinwonder_common_profit .currency_type[data-type="${type}"]`).data('value', data_value);
            } else {
                _msgalert.error(result.message);
            }
        });
    },

    UploadFileExcel: function () {
        let url = '/VinWonderPolicy/ImportDataPrice';
        let file = document.getElementById("file_import_vinwonder").files[0];

        var file_type = file['type'];
        if (file_type !== "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet") {
            $('#file_import_vinwonder_error_message').removeClass('mfp-hide');
            $('#grid_vinwonder_price').html('');
            return;
        }

        let formData = new FormData();
        formData.append("file", file)

        $.ajax({
            url: url,
            type: "POST",
            data: formData,
            processData: false,
            contentType: false,
            success: function (result) {
                if (result != null) {
                    $('#grid_vinwonder_price').html(result);
                    //$('#grid_vinwonder_price .scrollbar-block').scrollbar();
                    _vin_wonder.LoadPriceCommon();
                } else {
                    $('#file_import_vinwonder_error_message').removeClass('mfp-hide');
                }
            }, error: function (error) {
                console.log(error);
                $('#file_import_vinwonder_error_message').removeClass('mfp-hide');
            }
        });
    },

    LoadPriceCommon: function () {
        let input_common_profit = $('#ip_common_profit');
        let type = input_common_profit.data('type');
        let value = input_common_profit.val();

        $('#grid_vinwonder_price tbody tr').each(function () {
            let seft = $(this);
            let btn_active_unit = $(`.btn_currency_item_type[data-type="${type}"]`);
            btn_active_unit.addClass('active status-green').removeClass('status-gray');
            btn_active_unit.siblings('.btn_currency_item_type').removeClass('active status-green').addClass('status-gray');
            btn_active_unit.siblings('input.item_profit').val(value.toLocaleString());

            let profit = ConvertMoneyToNumber(value);
            let price = ConvertMoneyToNumber(seft.find('.item_base_price').text());

            let weekend_price_element = seft.find('.item_weekend_rate');
            let price_weekend = weekend_price_element != null ? ConvertMoneyToNumber(weekend_price_element.text()) : 0;

            let amount = 0;
            let amount_weekend = 0;
            if (type == 1) {
                amount = price * (100 + profit) / 100;
                amount_weekend = price_weekend * (100 + profit) / 100;
            } else {
                amount = price + profit;
                amount_weekend = price_weekend + profit;
            }

            seft.find('.item_amount').text(amount.toLocaleString())
            seft.find('.item_weekend_amount').text(amount_weekend.toLocaleString())
        });
    },

    UpsertCampaign: function () {
        let url = '/VinWonderPolicy/UpsertCampaign';

        let range_date = $('#ip_effect_time').val();
        if (range_date == null || range_date == "") {
            _msgalert.error("Bạn phải chọn khoảng thời gian hiệu lực cho chiến dịch");
            return;
        }

        let prices = [];
        $('#grid_vinwonder_price tbody tr').each(function () {
            let seft = $(this);
            let ckb = seft.find('.ckb_vin_grid_item');

            if (ckb.is(':checked')) {
                let profit = seft.find('.item_profit').val();

                prices.push({
                    Id: seft.find('.item_vinwonder_price_id').val(),
                    SiteId: seft.find('.item_site_id').text(),
                    SiteName: seft.find('.item_site_name').text(),
                    ServiceId: seft.find('.item_service_id').text(),
                    ServiceCode: seft.find('.item_service_code').text(),
                    Name: seft.find('.item_name_vin_price').text(),
                    BasePrice: ConvertMoneyToNumber(seft.find('.item_base_price').text()),
                    WeekendRate: ConvertMoneyToNumber(seft.find('.item_weekend_rate').text()),
                    UnitType: seft.find('.btn_currency_item_type.active').data('type'),
                    Profit: profit != "" && profit != null ? ConvertMoneyToNumber(profit) : 0,
                    RateCode: seft.find('.item_rate_code').text(),
                });
            }
        });

        let from_date = moment(range_date.split('-')[0].trim(), "DD/MM/YYYY HH:mm").toDate();
        let to_date = moment(range_date.split('-')[1].trim(), "DD/MM/YYYY HH:mm").toDate();

        let data = {
            Id: $('#ip_campaign_id').val(),
            CampaignCode: $('#ip_campaign_name').val(),
            FromDate: from_date.toJSON(),
            ToDate: to_date.toJSON(),
            Description: $('#ip_campaign_name').val(),
            PricePolycies: prices
        };

        _ajax_caller.post(url, { model: data }, function (result) {
            if (result.isSuccess) {
                _msgalert.success(result.message);

                let objSearch = _pricepolicymanagement.SearchParam;
                objSearch.currentPage = 1;
                _pricepolicymanagement.Search(_pricepolicymanagement.SearchParam);

                $('#global_modal_popup').modal('hide');
            } else {
                _msgalert.error(result.message);
            }
        });
    },
}

$('#vinwonder_common_profit .currency_type').click(function () {
    let seft = $(this);
    let value = parseInt(seft.data('value'));
    let type = parseInt(seft.data('type'));

    seft.addClass('status-green').removeClass('status-gray');
    seft.siblings('.currency_type').addClass('status-gray').removeClass('status-green');

    seft.siblings('input').val(value.toLocaleString());
    seft.siblings('input').data('type', type);
});

$('#btn_edit_common_profit').click(function () {
    _vin_wonder.ChangeButtonCommonProfit(1);
});

$('#btn_cancel_common_profit').click(function () {
    _vin_wonder.ChangeButtonCommonProfit(0);
});

$('#btn_save_common_profit').click(function () {
    let value = $('#ip_common_profit').val();
    let type = $('#ip_common_profit').data('type');

    _vin_wonder.UpdateCommonProfit(type, value);
});

$('#ip_effect_time').change(function () {
    let value = $(this).val();
    let from_date = value.split('-')[0].trim().split(' ')[0];
    let to_date = value.split('-')[1].trim().split(' ')[0];
    $('#ip_campaign_name').val(`Giá vé Vinwonder từ: ${from_date} - ${to_date}`);
});

$('#file_import_vinwonder').change(function () {
    $('#file_import_vinwonder_error_message').addClass('mfp-hide');
    _vin_wonder.UploadFileExcel();
});

$('#grid_vinwonder_price').on('click', '.btn_item_common_profit', function () {
    let seft = $(this);
    let type = $('#ip_common_profit').data('type');
    let profit = $('#ip_common_profit').siblings(`.currency_type[data-type="${type}"]`).data('value');

    seft.siblings('input.item_profit').val(profit.toLocaleString());
    seft.siblings('.btn_currency_item_type').removeClass('active status-green').addClass('status-gray');
    let btn_currency_item_active = seft.siblings(`.btn_currency_item_type[data-type="${type}"]`);
    btn_currency_item_active.addClass('active status-green').removeClass('status-gray');

    let price = ConvertMoneyToNumber(seft.closest('tr').find('.item_base_price').text());
    let price_weekend = ConvertMoneyToNumber(seft.closest('tr').find('.item_weekend_rate').text());

    let amount = 0;
    let amount_weekend = 0;
    if (type == 1) {
        amount = price * (100 + profit) / 100;
        amount_weekend = price_weekend * (100 + profit) / 100;
    } else {
        amount = price + profit;
        amount_weekend = price_weekend + profit;
    }
    seft.closest('tr').find('.item_amount').text(amount.toLocaleString())
    seft.closest('tr').find('.item_weekend_amount').text(amount_weekend.toLocaleString())
});

$('#grid_vinwonder_price').on('click', '.btn_currency_item_type', function () {
    let seft = $(this);
    if (!seft.hasClass('active status-green')) {
        seft.addClass('active status-green').removeClass('status-gray');
        seft.siblings('.btn_currency_item_type').removeClass('active status-green').addClass('status-gray');
        seft.siblings('input.item_profit').val(0).trigger('change');
    }
});

$('#grid_vinwonder_price').on('change', '.item_profit', function () {
    let seft = $(this);
    let type = seft.siblings('.btn_currency_item_type.active').data('type');
    let profit = seft.val() != null && seft.val() != "" ? ConvertMoneyToNumber(seft.val()) : 0;

    let price = ConvertMoneyToNumber(seft.closest('tr').find('.item_base_price').text());
    let price_weekend = ConvertMoneyToNumber(seft.closest('tr').find('.item_weekend_rate').text());

    let amount = 0;
    let amount_weekend = 0;
    if (type == 1) {
        if (profit > 100) {
            _msgalert.error("Giá trị phần trăm không được lớn hơn 100%");
            seft.val(0);
            return;
        }
        amount = price * (100 + profit) / 100;
        amount_weekend = price_weekend * (100 + profit) / 100;
    } else {
        amount = price + profit;
        amount_weekend = price_weekend + profit;
    }

    seft.closest('tr').find('.item_amount').text(amount.toLocaleString())
    seft.closest('tr').find('.item_weekend_amount').text(amount_weekend.toLocaleString())
});

$('#grid_vinwonder_price').on('click', '#ckb_vin_grid_all', function () {
    let seft = $(this);
    if (seft.is(":checked")) {
        $('#grid_vinwonder_price .ckb_vin_grid_item').prop('checked', true);
    } else {
        $('#grid_vinwonder_price .ckb_vin_grid_item').prop('checked', false);
    }
});

$('#grid_vinwonder_price').on('click', '.ckb_vin_grid_item', function () {
    let seft = $(this);
    let checked_value = false;

    let total_ckb = $('#grid_vinwonder_price .ckb_vin_grid_item').length;
    let total_ckb_checked = $('#grid_vinwonder_price .ckb_vin_grid_item:checked').length;

    if (total_ckb == total_ckb_checked) {
        $('#grid_vinwonder_price #ckb_vin_grid_all').prop('checked', true);
    } else {
        $('#grid_vinwonder_price #ckb_vin_grid_all').prop('checked', false);
    }
});

$(document).ready(function () {

    let startDate = new Date();
    let endDate = new Date();
    let data_date = $('#ip_effect_time').val();

    if (data_date && data_date != null && data_date != "") {
        startDate = moment(data_date.split('-')[0].trim(), "DD/MM/YYYY HH:mm").toDate();
        endDate = moment(data_date.split('-')[1].trim(), "DD/MM/YYYY HH:mm").toDate();
    }

    $(`.dateranger_picker_input`).daterangepicker({
        autoUpdateInput: false,
        timePicker: true,
        minDate: new Date(),
        startDate: startDate,
        endDate: endDate,
    }, function (start, end, label) {
        $(this).val(`${start.format('DD/MM/YYYY HH:mm')} - ${end.format('DD/MM/YYYY HH:mm')}`).change();
    }).on('apply.daterangepicker', function (ev, picker) {
        $(this).val(`${picker.startDate.format('DD/MM/YYYY HH:mm')} - ${picker.endDate.format('DD/MM/YYYY HH:mm')}`).change();
    }).on('cancel.daterangepicker', function (ev, picker) {
        $(this).val('');
        $('#ip_campaign_name').val('');
    });
});

