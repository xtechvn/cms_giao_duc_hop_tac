let Price_percent = 0.03;
let Price_percent_2 = 0.08;
var _order_detail_tour = {
    ServiceType:5,
    Initialization: function () {
        var tour_id = $('#add-service-tour').attr('data-tour-id')
        $('#add-service-tour').addClass('show')
        _order_detail_common.Select2WithFixedOptionAndNoSearch($('.service-tour-type'))
        _order_detail_tour.OnChageTourTypeToLocationSelect(true)
        _order_detail_common.Select2WithFixedOptionAndNoSearch($('.service-tour-organizing_type'))


        _order_detail_common.SingleDatePicker($('.service-tour-startdate'))
        _order_detail_common.SingleDatePicker($('.service-tour-enddate'))

        _order_detail_common.UserSuggesstion($('.service-tour-main-staff'))
        _order_detail_common.ExistsTourSuggesstion($('.service-tour-exists-tour'))
        _order_detail_tour.AddTourServicePackagesPopup(tour_id)
        _order_detail_tour.AddTourServiceTouristsPopup(tour_id)
        _order_detail_tour.OnEnableTourProduct($('.on-enable-tour-product:checked'),true)
        _order_detail_tour.DynamicBindInput()
        $('.service-tour-exists-tour').trigger('change')
        _order_detail_common.FileAttachment(tour_id, _order_detail_tour.ServiceType)


    },
    DynamicBindInput: function () {
        

        $('body').on('keyup', '.service-tour-extrapackage-amount', function () {
            var table_element = $(this).closest('.service-tour-extrapackage-tbody')
            var row_element = $(this).closest('.service-tour-extrapackage-row')
            _order_detail_tour.CalucateRowProfit(row_element)
            _order_detail_tour.CalucateAmount(table_element)
            _order_detail_tour.CalucateProfit(table_element)
            _order_detail_tour.CalucateFundCustomerCare(table_element)

        });
        $('body').on('keyup', '.service-tour-extrapackage-price', function () {
            var table_element = $(this).closest('.service-tour-extrapackage-tbody')
            var row_element = $(this).closest('.service-tour-extrapackage-row')
            _order_detail_tour.CalucatePrice(table_element)
            _order_detail_tour.CalucateRowProfit(row_element)
            _order_detail_tour.CalucateProfit(table_element)
            _order_detail_tour.CalucateFundCustomerCare(table_element)
        });
        $('body').on('keyup', '.servicemanual-tour-other-amount, .servicemanual-tour-commission', function () {

            _order_detail_tour.CalucateTotalServiceProfit()
        });
        $("body").on('apply.daterangepicker', ".service-tour-startdate", function (ev, picker) {
            var element = $(this)

            element.val(_global_function.GetDayText(element.data('daterangepicker').startDate._d).split(' ')[0])
            _order_detail_common.OnApplyStartDateOfBookingRange($(this), $('.service-tour-enddate'))

        });
        $('body').on('change', '.service-tour-exists-tour', function () {

            _order_detail_tour.OnApplyExistsProductTour($(this))

        });
        $('body').on('change', '.service-tour-type', function () {
            _order_detail_tour.OnChageTourTypeToLocationSelect()

        });
        $('body').on('change', '.on-enable-tour-product', function () {
            _order_detail_tour.OnEnableTourProduct($(this))
        });
        /*
        $("body").on('apply.daterangepicker', ".service-tour-guest-birthday", function (ev, picker) {
            var element = $(this)
            element.val(_global_function.GetDayText(element.data('daterangepicker').startDate._d).split(' ')[0])
        });
        $("body").on('keyup', ".service-tour-guest-birthday", function (ev, picker) {
            var element = $(this)
            var value = element.val()
            element.val(value)
        });
        $("body").on('apply.daterangepicker', ".service-tour-enddate", function (ev, picker) {
            var element = $(this)
            element.val(_global_function.GetDayText(element.data('daterangepicker').startDate._d).split(' ')[0])
        });
        */
        $("body").on('change', ".import_data_guest", function (ev, picker) {
            var element = $(this)

            if (element[0].files) {
                $('.img_loading_upload_file').show()
                $('.upload-file-guest-btn').attr('disabled', 'disabled')

                var formData = new FormData();
                $(element[0].files).each(function (index, item) {
                    formData.append("file", item);
                    return false
                });
               
                formData.append("service_type", "5");

                $.ajax({
                    url: "GetGuestFromFile",
                    type: "post",
                    processData: false,
                    contentType: false,
                    data: formData,
                    success: function (result) {
                        var incorrect_data = false
                        if (result != undefined && result.data != undefined && result.data.length > 0) {
                            var incorrect_data = false
                            $(result.data).each(function (index, item) {
                                var name = _global_function.RemoveUnicode(item.fullName)
                                var note = _global_function.RemoveUnicode(item.note)
                                if (name == null || _global_function.CheckIfSpecialCharracter(name) || !_global_function.CheckIfStringIsDate(item.birthday)) {
                                    _msgalert.error('Import danh sách thất bại, dữ liệu trong tệp tin không chính xác, vui lòng kiểm tra lại / liên hệ IT')
                                    incorrect_data = true;
                                    return false;
                                }
                            });
                            if (incorrect_data) {
                                $('.img_loading_upload_file').hide()
                                return
                            }
                            $('.service-tour-guest-row').remove()
                            $(result.data).each(function (index, item) {
                                var summary_row = $('.service-tour-guest-summary-row')
                                var new_position = _order_detail_tour.GetLastestGuestNo(summary_row) + 1;
                                summary_row.before(_order_detail_html.html_service_tour_add_guest_tr.replaceAll('@item.Id', '0').replaceAll('@(++index)', '' + new_position).replaceAll('{guest_name}', item.fullName == undefined || item.fullName == null ? '' : item.fullName).replaceAll('{genre_male_if_selected}', item.gender.toLowerCase().includes('nam') ? 'selected=\"selected\"' : '').replaceAll('{genre_female_if_selected}', item.gender.toLowerCase().includes('nữ') || item.gender.toLowerCase().includes('nu') ? 'selected=\"selected\"' : '').replaceAll('{guest_name}', item.name).replaceAll('{birthday}', item.birthday == null ? '' : item.birthday).replaceAll('{cccd}', item.cccd == null || item.cccd == undefined ? '' : item.cccd).replaceAll('{room_number}', item.roomNumber == null || item.roomNumber == undefined ? '' : item.roomNumber).replaceAll('{note}', item.note == null || item.note == undefined ? '' : _global_function.RemoveSpecialCharacter(item.note)))
                            });
                            $('.new_tour_guest_row').each(function (index, item) {
                                var row_element = $(this);
                                _order_detail_common.SingleDatePickerBirthDay(row_element.find('.service-tour-guest-birthday'), 'up')
                                _order_detail_common.Select2WithFixedOptionAndNoSearch(row_element.find('.service-tour-guest-gender'))
                                row_element.removeClass('new_tour_guest_row')
                            });
                            _order_detail_tour.ReIndexGuest($('.service-tour-guest-tbody'))

                        }
                        else {

                            _msgalert.error(result.msg)

                        }
                        $('.img_loading_upload_file').hide()
                    }
                });
            }
            $('.upload-file-guest-btn').removeAttr('disabled')
            element.val('');

        });
        $('.service-tour-note').keydown(function (e) {
            e.stopPropagation();
        });
    },
    OnEnableTourProduct: function (element, is_gen_new = false) {
        var value = element.val()
        switch (value) {
            case '1': {
                $('.tour-product-box').hide();
                $('.tour-product-box-manual').show();
                $('.service-tour-type').prop('disabled', false);
                $('.service-tour-startpoint').prop('disabled', false);
                $('.service-tour-organizing_type').prop('disabled', false);
                $('.service-tour-endpoint').prop('disabled', false);
                $('.service-tour-type').removeClass('input-disabled-background')
                $('.service-tour-startpoint').removeClass('input-disabled-background')
                $('.service-tour-endpoint').removeClass('input-disabled-background')
                $('.service-tour-organizing_type').removeClass('input-disabled-background')
               
            } break;
            case '0': {
                $('.tour-product-box').show();
                $('.tour-product-box-manual').hide();
                $('.service-tour-type').prop('disabled', true);
                $('.service-tour-startpoint').prop('disabled', true);
                $('.service-tour-endpoint').prop('disabled', true);
                $('.service-tour-organizing_type').prop('disabled', true);
                $('.service-tour-type').addClass('input-disabled-background')
                $('.service-tour-startpoint').addClass('input-disabled-background')
                $('.service-tour-endpoint').addClass('input-disabled-background')
                $('.service-tour-organizing_type').addClass('input-disabled-background')
              
            } break;
        }
        if (!is_gen_new) {
            $('.service-tour-product-name-manual').val('')
            $('.service-tour-exists-tour').val(null).trigger("change")
            $('.service-tour-type').val(null).trigger("change")
            $('.service-tour-startpoint').val(null).trigger("change")
            $('.service-tour-endpoint').val(null).trigger("change")
            $('.service-tour-organizing_type').val(null).trigger("change")
        }
      
    },
    OnChageTourTypeToLocationSelect: function (is_not_gen_new = false) {
        var tour_type = $('.service-tour-type').find(':selected').val();
        if (!is_not_gen_new) {

            _order_detail_tour.ChangeTourTypeSelect($('.service-tour-endpoint'))
            _order_detail_tour.ChangeTourTypeSelect($('.service-tour-startpoint'))
        }
        if (tour_type == undefined) tour_type = '1';
        switch (parseInt(tour_type.trim())) {
            //-- noi dia:
            case 1: {
                _order_detail_tour.SingleSelect2ProvinceAjax($('.service-tour-startpoint'))
                _order_detail_tour.SingleSelect2ProvinceAjax($('.service-tour-endpoint'))
            } break;
            //-- inbound
            case 2: {
                _order_detail_tour.SingleSelect2CountryAjax($('.service-tour-startpoint'))
                _order_detail_tour.SingleSelect2ProvinceAjax($('.service-tour-endpoint'))
            } break;
            //-- outbound
            case 3: {

                _order_detail_tour.SingleSelect2ProvinceAjax($('.service-tour-startpoint'))
                _order_detail_tour.SingleSelect2CountryAjax($('.service-tour-endpoint'))
            } break;
        }
    },
    Close: function () {
        $('#add-service-tour').removeClass('show')
        setTimeout(function () {
            $('#add-service-tour').remove();
            _order_detail_create_service.StartScrollingBody();
            _order_detail_tour.RemoveDynamicBind();
        }, 300);
    },
    RemoveDynamicBind: function () {

        $('body').off('keyup', '.service-tour-extrapackage-amount', function () {

        });
        $('body').off('keyup', '.service-tour-extrapackage-profit', function () {

        });
        $("body").off('apply.daterangepicker', ".service-tour-startdate", function (ev, picker) {

        });
        $('body').off('change', '.service-tour-exists-tour', function () {

        });
        $('body').off('change', '.service-tour-type', function () {

        });
        $('body').off('change', '.on-enable-tour-product', function () {
        });
        $("body").off('apply.daterangepicker', ".service-tour-guest-birthday", function (ev, picker) {

        });
        $("body").off('keyup', ".service-tour-guest-birthday", function (ev, picker) {

        });
        $("body").off('apply.daterangepicker', ".service-tour-enddate", function (ev, picker) {

        });
        $("body").off('change', ".import_data_guest", function (ev, picker) {

        });
        $('body').off('keyup', '.servicemanual-tour-other-amount, .servicemanual-tour-commission', null);

        $('.service-tour-note').keydown(null);

    },
    ChangeTourTypeSelect: function (element) {
        element.html('')
    },
    AddTourServiceTouristsPopup: function (tour_id) {
        $.ajax({
            url: "AddTourServiceTourists",
            type: "post",
            data: { tour_id: tour_id },
            success: function (result) {
                $('.service-tour-tourists').html(result)
                _order_detail_common.SingleDatePickerBirthDay($('.service-tour-guest-birthday'), 'up')
                _order_detail_common.Select2WithFixedOptionAndNoSearch($('.service-tour-guest-gender'))

            }
        });
    },
    AddTourServicePackagesPopup: function (tour_id) {
        if (tour_id != undefined && !isNaN(parseInt(tour_id))) {
            $.ajax({
                url: "AddTourServicePackages",
                type: "post",
                data: { tour_id: tour_id },
                success: function (result) {
                    $('.service-tour-packages').html(result)
                    _order_detail_common.Select2WithFixedOptionAndNoSearch($('.service-tour-extrapackage-packagename-select'))
                    _order_detail_tour.CalucateTotalServiceAmount()
                    _order_detail_tour.CalucateTotalServiceProfit()
                    _order_detail_tour.CalucatePrice($('.service-tour-extrapackage-tbody'))
                }
            });
        }
        
    },
   
    AddTourExtraPackage: function (element) {
        var table_element = element.closest('.service-tour-extrapackage-tbody')
        var new_position = _order_detail_tour.GetLastestExtraPackagesNo(element) + 1;
        table_element.find('.service-tour-extrapackage-summary-row').before(_order_detail_html.html_service_tour_packages_add_extra_package_tr.replaceAll('@item.Id', '0').replaceAll('@(++index)', '' + new_position).replaceAll('@(((double)item.BasePrice).ToString("N0"))', '').replaceAll('@(((int)item.Quantity).ToString("N0"))', '').replaceAll('@(((double)item.AmountBeforeVat).ToString("N0"))', '').replaceAll('@(((double)item.Vat).ToString("N0"))', '').replaceAll('@(((double)item.Amount).ToString("N0"))', '').replaceAll('@item.PackageName', ''))
    },
    DeleteTourPackage: function (element) {
        var table_element = element.closest('.service-tour-extrapackage-tbody')
        element.closest('.service-tour-extrapackage-row').remove()
        _order_detail_tour.ReIndexExtraPackages(table_element)
        _order_detail_tour.CalucateAmount(table_element)
        _order_detail_tour.CalucateProfit(table_element)
        _order_detail_tour.CalucateFundCustomerCare(table_element)
    },
    AddTourGuest: function (element) {
        var table_element = element.closest('.service-tour-guest-tbody')
        var new_position = _order_detail_tour.GetLastestGuestNo(element) + 1;
        table_element.find('.service-tour-guest-summary-row').before(_order_detail_html.html_service_tour_add_guest_tr.replaceAll('@item.Id', '0').replaceAll('@(++index)', '' + new_position).replaceAll('{guest_name}', '').replaceAll('{cccd}', '').replaceAll('{room_number}', '').replaceAll('{birthday}', '').replaceAll('{note}', ''))
        _order_detail_common.SingleDatePickerBirthDay($('.new_tour_guest_row').find('.service-tour-guest-birthday'), 'up')
        $('.new_tour_guest_row').each(function (index, item) {
            var row_element = $(this);
            _order_detail_common.SingleDatePickerBirthDay(row_element.find('.service-tour-guest-birthday'), 'up')
            _order_detail_common.Select2WithFixedOptionAndNoSearch(row_element.find('.service-tour-guest-gender'))
            row_element.removeClass('new_tour_guest_row')
        });

        _order_detail_tour.ReIndexGuest(table_element)

    },
    DeleteTourGuest: function (element) {
        var table_element = element.closest('.service-tour-guest-tbody')
        element.closest('.service-tour-guest-row').remove()
        _order_detail_tour.ReIndexGuest(table_element)
       
    },
    ReIndexGuest: function (table_element) {
        var total = 0;
        table_element.find('.service-tour-guest-row').each(function (index, item) {
            var row_element = $(this);
            total++;
            row_element.find('.service-tour-guest-order').html('' + total)
        });
    },
    ReIndexExtraPackages: function (table_element) {
        var total = 0;
        table_element.find('.service-tour-extrapackage-row').each(function (index, item) {
            var row_element = $(this);
            total++;
            row_element.find('.service-tour-extrapackage-order').html('' + total)
        });
    },
    CalucateRowProfit: function (row_element) {
        var sale_element = row_element.find('.service-tour-extrapackage-amount')
        var operator_element = row_element.find('.service-tour-extrapackage-price')
        
        var operator_price = isNaN(parseFloat(operator_element.val().replaceAll(',', ''))) ? 0 : parseFloat(operator_element.val().replaceAll(',', ''))
        var sale_price = isNaN(parseFloat(sale_element.val().replaceAll(',', ''))) ? 0 : parseFloat(sale_element.val().replaceAll(',', ''))

        var profit=sale_price - operator_price
        row_element.find('.service-tour-extrapackage-profit').val((profit >= 0 ? '' : '-')+ _global_function.Comma(profit))

    },
    CalucateFundCustomerCare: function (table_element) {
        var total_amout_before_vat = 0;
   
        _order_detail_tour.CalucateTotalServiceFundCustomerCare()

    },
    CalucateProfit: function (table_element) {
        var total_amout_before_vat = 0;
        table_element.find('.service-tour-extrapackage-profit').each(function (index, item) {
            var element = $(this);
            var amount = !isNaN(parseFloat(element.val().replaceAll(',', ''))) && parseFloat(element.val().replaceAll(',', '')) > 0 ? parseFloat(element.val().replaceAll(',', '')) : 0
            total_amout_before_vat += amount
        });
        table_element.find('.service-tour-total-extrapackage-profit').html((total_amout_before_vat >= 0 ? '' : '-') +_global_function.Comma(total_amout_before_vat))
        _order_detail_tour.CalucateTotalServiceProfit()

    },
    CalucateAmount: function (table_element) {
        var total_amout_after_vat = 0;
        table_element.find('.service-tour-extrapackage-amount').each(function (index, item) {
            var element = $(this);
            var amount = !isNaN(parseFloat(element.val().replaceAll(',', ''))) && parseFloat(element.val().replaceAll(',', ''))>0 ? parseFloat(element.val().replaceAll(',', '')) : 0
            total_amout_after_vat += amount
        });
        table_element.find('.service-tour-total-extrapackage-amount-after-vat').html(_global_function.Comma(total_amout_after_vat))
        _order_detail_tour.CalucateTotalServiceAmount()
    },
    CalucatePrice: function (table_element) {
        var total_amout_before_vat = 0;
        table_element.find('.service-tour-extrapackage-price').each(function (index, item) {
            var element = $(this);
            var amount = !isNaN(parseFloat(element.val().replaceAll(',', ''))) && parseFloat(element.val().replaceAll(',', '')) > 0 ? parseFloat(element.val().replaceAll(',', '')) : 0
            total_amout_before_vat += amount
        });
        $('.service-tour-total-extrapackage-amount-before-vat').html(_global_function.Comma(total_amout_before_vat))
    },
    CalucateRowAmount: function (row_element) {
        var element_base_price = row_element.find('.service-tour-extrapackage-baseprice')
        var element_quanity= row_element.find('.service-tour-extrapackage-quantity')
        var profit = row_element.find('.service-tour-extrapackage-profit')
        var base_price = !isNaN(parseFloat(element_base_price.val().replaceAll(',', ''))) && parseFloat(element_base_price.val().replaceAll(',', '')) > 0 ? parseFloat(element_base_price.val().replaceAll(',', '')) : 0
        var quanity = !isNaN(parseFloat(element_quanity.val().replaceAll(',', ''))) && parseFloat(element_quanity.val().replaceAll(',', '')) > 0 ? parseFloat(element_quanity.val().replaceAll(',', '')) : 0
        var profit = !isNaN(parseFloat(profit.val().replaceAll(',', ''))) && parseFloat(profit.val().replaceAll(',', '')) > 0 ? parseFloat(profit.val().replaceAll(',', '')) : 0
        var amount = (base_price * quanity) + profit;
        row_element.find('.service-tour-extrapackage-amount').val(_global_function.Comma(amount.toFixed(0))).change()

    },
    GetLastestExtraPackagesNo: function (element) {
        var total = 0;
        element.closest('.service-tour-extrapackage-tbody').find('.service-tour-extrapackage-row').each(function (index, item) {
            total++;
        });
        total++;
        return total
    },
    GetLastestGuestNo: function (element) {
        var total = 0;
        element.closest('.service-tour-extrapackage-tbody').find('.service-tour-guest-row').each(function (index, item) {
            total++;
        });
        return total
    },
    CalucateTotalServiceAmount: function () {
        var amount_text = $('.service-tour-total-extrapackage-amount-after-vat').html()
        $('.service-manual-tour-total-service-amount').html(amount_text)
    },
    CalucateTotalServiceProfit: function () {
        var profit_text = $('.service-tour-total-extrapackage-profit').html()
        var profit = parseFloat(profit_text.replaceAll(',', ''))

        var other_amount_element = $('#servicemanual-tour-other-amount')
        var other_amount =  parseFloat(other_amount_element.val().replaceAll(',', ''))

        var discount_element = $('#servicemanual-tour-commission')
        var discount =  parseFloat(discount_element.val().replaceAll(',', ''))
        var total_profit = profit - other_amount - discount
        $('.service-manual-tour-total-service-profit').html((total_profit >= 0 ? '' : '-') + _global_function.Comma(total_profit))
    },
    CalucateTotalServiceFundCustomerCare: function () {
        var profit_text = $('.service-tour-total-extrapackage-profit').html()
        var fundCustomerCare_tour = $('#servicemanual-tour-fundCustomerCare').val();
        var profit = parseFloat(profit_text.replaceAll(',', ''))
        var FundCustomerCare = 0
        if (parseFloat(fundCustomerCare_tour.replaceAll(',', '')) > 0) {
            FundCustomerCare = (profit - (profit * Price_percent_2)) * Price_percent;
        }
        $('#servicemanual-tour-fundCustomerCare').val((FundCustomerCare >= 0 ? '' : '-') + _global_function.Comma(FundCustomerCare))
    },
    SingleSelect2CountryAjax: function (element) {
        var selected = element.find(':selected').val()
        element.select2({
            ajax: {
                url: "NationalSuggestion",
                type: "post",
                dataType: 'json',
                delay: 250,
                data: function (params) {
                    var query = {
                        txt_search: params.term,
                    }
                    return query;
                },
                processResults: function (response) {
                    return {
                        results: $.map(response.data, function (item) {
                            return {
                                text: item.name,
                                id: item.id,
                            }
                        })
                    };
                },
                cache: true
            }
        });
        element.val(selected).trigger('change');
    },
    SingleSelect2ProvinceAjax: function (element) {
        var selected = element.find(':selected').val()
        element.select2({
            ajax: {
                url: "ProvinceSuggestion",
                type: "post",
                dataType: 'json',
                delay: 250,
                data: function (params) {
                    var query = {
                        txt_search: params.term,
                    }
                    return query;
                },
                processResults: function (response) {
                    return {
                        results: $.map(response.data, function (item) {
                            return {
                                text: item.name,
                                id: item.id,
                            }
                        })
                    };
                },
                cache: true
            }
        });
        element.val(selected).trigger('change');
    },
    SingleSelect2ProvincefromDb: function (element) {
        var selected = element.find(':selected').val()
        $.ajax({
            url: "/Location/LoadProvince",
            type: "get",
            success: function (result) {
                if (result != undefined && result.data != undefined && result.data.length > 0) {
                    var html = '';
                    $(result.data).each(function (index, item) {
                        html += _order_detail_html.html_hotel_option.replaceAll('{if_selected}', '').replaceAll('{if_selected}', '').replaceAll('{hotel_id}', item.id).replaceAll('{name}', item.name)
                    });
                    element.html(html)
                    element.val(selected).trigger('change');
                    element.select2({

                    });
                }
            }
        });
    },
    Summit: function () {

        switch ($('.on-enable-tour-product:checked').val()) {
            case '0': {
                if ($('.service-tour-exists-tour').find(':selected').val() == undefined) {
                    _msgalert.error("Vui lòng chọn sản phẩm tour")
                    return;
                }
            } break;
            case '1': {
                if ($('.service-tour-product-name-manual').val() == undefined || $('.service-tour-product-name-manual').val() == '') {
                    _msgalert.error("Vui lòng điền tên sản phẩm tour")
                    return;
                }
            } break;
        }
       
    
        if ($('.service-tour-main-staff option:selected').val() == undefined || isNaN($('.service-tour-main-staff option:selected').val())) {
            _msgalert.error("Vui lòng chọn điều hành viên")
            return;
        }
      
        var from_date = _order_detail_common.GetDateFromNoUpdateDateRangeElement($('.service-tour-startdate'))
        var to_date = _order_detail_common.GetDateFromNoUpdateDateRangeElement($('.service-tour-enddate'))
        /*
        if (from_date >= to_date) {
            _msgalert.error("Ngày về không được nhỏ hơn hoặc bằng ngày đi")
            return;
        }*/
        var validate_failed = false
        var pathname = window.location.pathname.split('/');

        var arrive_date = _order_detail_common.GetDateFromNoUpdateDateRangeElement($('.service-tour-startdate'), false)
        var departure_date = _order_detail_common.GetDateFromNoUpdateDateRangeElement($('.service-tour-enddate'), false)

        var other_amount_element = $('#servicemanual-tour-other-amount')
        var other_amount = other_amount_element.val() == undefined || isNaN(parseFloat(other_amount_element.val().replaceAll(',', ''))) ? 0 : parseFloat(other_amount_element.val().replaceAll(',', ''))

        var discount_element = $('#servicemanual-tour-commission')
        var discount = discount_element.val() == undefined || isNaN(parseFloat(discount_element.val().replaceAll(',', ''))) ? 0 : parseFloat(discount_element.val().replaceAll(',', ''))

        var fundCustomerCare_element = $('#servicemanual-tour-fundCustomerCare')
        var fundCustomerCare = fundCustomerCare_element.val() == undefined || isNaN(parseFloat(fundCustomerCare_element.val().replaceAll(',', ''))) ? 0 : parseFloat(fundCustomerCare_element.val().replaceAll(',', ''))


        var object_summit = {
            order_id: pathname[pathname.length - 1],
            tour_id: $(".tour_service").attr('data-tour-id'),
            service_code: $(".tour_service").attr('data-service-code'),
            organizing_type: $(".service-tour-organizing_type option:selected").val(),
            tour_type: $(".service-tour-type option:selected").val(),
            main_staff: $(".service-tour-main-staff option:selected").val(),
            start_date: _global_function.GetDayText(arrive_date, true),
            end_date: _global_function.GetDayText(departure_date, true),
            start_point: $('.service-tour-startpoint option:selected').val(),
            end_point: $('.service-tour-endpoint').select2('data').map(a => a.id).join(","),
            tour_product_id: $(".service-tour-exists-tour option:selected").val() == undefined ? 0 : $(".service-tour-exists-tour option:selected").val(),
            is_self_designed: $('.on-enable-tour-product:checked').val(),
            tour_product_name: $('.service-tour-product-name-manual').val(),
            note: $('.service-tour-note').val(),
            fundCustomerCare: fundCustomerCare,
            guest: [],
            extra_packages: [],
            other_amount: other_amount,
            commission: discount
        }
        if (object_summit.note == null || object_summit.note == undefined || object_summit.note.trim() == '') object_summit.note = "."

        var adt_quanity = $('.service-tour-extrapackage-passenger').find('.service-tour-adt-quanity').val() == undefined || isNaN(parseInt($('.service-tour-extrapackage-passenger').find('.service-tour-adt-quanity').val().replaceAll(',', ''))) ? 0 : parseInt($('.service-tour-extrapackage-passenger').find('.service-tour-adt-quanity').val().replaceAll(',', ''))
        var chd_quanity = $('.service-tour-extrapackage-passenger').find('.service-tour-chd-quanity').val() == undefined || isNaN(parseInt($('.service-tour-extrapackage-passenger').find('.service-tour-chd-quanity').val().replaceAll(',', ''))) ? 0 : parseInt($('.service-tour-extrapackage-passenger').find('.service-tour-chd-quanity').val().replaceAll(',', ''))
        var inf_quanity = $('.service-tour-extrapackage-passenger').find('.service-tour-inf-quanity').val() == undefined || isNaN(parseInt($('.service-tour-extrapackage-passenger').find('.service-tour-inf-quanity').val().replaceAll(',', ''))) ? 0 : parseInt($('.service-tour-extrapackage-passenger').find('.service-tour-inf-quanity').val().replaceAll(',', ''))

        //-- Calucate base_price
        var amountstr = $('.service-tour-extrapackage-passenger').find('.service-tour-extrapackage-amount').val().replaceAll(',', '')
        var amount = 0;
        if (!isNaN(amountstr)) {
            amount = parseFloat(amountstr)
        }
        var base_amount_per_person = amount
        if (adt_quanity <= 0 || amount <= 0) {
            base_amount_per_person = amount
        }
        else {
            base_amount_per_person = amount / adt_quanity
        }
        
        var pricestr = $('.service-tour-extrapackage-passenger').find('.service-tour-extrapackage-price').val().replaceAll(',', '')
        var price=0
        if (!isNaN(pricestr)) {
            price = parseFloat(pricestr)
        }
        //var base_price_per_person = price
        //if ((adt_quanity + chd_quanity + inf_quanity) <= 0 || amount <= 0) {
        //    base_price_per_person = price
        //}
        //else {
        //    base_price_per_person = price / (adt_quanity + chd_quanity + inf_quanity)
        //}

        //var profitstr = $('.service-tour-extrapackage-passenger').find('.service-tour-extrapackage-profit').val().replaceAll(',', '')
        //var profit = 0;
        //if (!isNaN(profitstr)) {
        //    profit = parseFloat(profitstr)
        //}

        //var profit_per_person = 0
        //if ((adt_quanity + chd_quanity + inf_quanity) <= 0 || amount <= 0) {
        //    profit_per_person = profit
        //}
        //else {
        //    profit_per_person = profit / (adt_quanity + chd_quanity + inf_quanity)
        //}
        //-- adt_amount
        var adt_package = {
            id: $('.service-tour-extrapackage-passenger').attr('data-adt-id'),
            package_id: 'adt_amount',
            package_code: 'adt_amount',
            base_price: price,
            amount: amount,
            price: price,
            profit: amount - price,
            quantity: adt_quanity,
        }
        object_summit.extra_packages.push(adt_package);
        //-- chd_amount
        var chd_package = {
            id: $('.service-tour-extrapackage-passenger').attr('data-chd-id'),
            package_id: 'chd_amount',
            package_code: 'chd_amount',
            base_price: 0,
            quantity: chd_quanity,
            amount:0,
            price: 0,
            profit: 0,
        }
        object_summit.extra_packages.push(chd_package);
        //-- inf_amount
        var inf_package = {
            id: $('.service-tour-extrapackage-passenger').attr('data-inf-id'),
            package_id: 'inf_amount',
            package_code: 'inf_amount',
            base_price: 0,
            quantity: inf_quanity,
            amount: 0,
            price:0,
            profit: 0,
        }
        object_summit.extra_packages.push(inf_package);

        $('.service-tour-extrapackage-row').each(function (index, item) {
            var extra_package_element = $(item);
            var package_id = extra_package_element.find('.service-tour-extrapackage-packagename-select option:selected').val();
            var package_name = extra_package_element.find('.service-tour-extrapackage-packagename-select option:selected').text();
            if (extra_package_element.hasClass('service-tour-extrapackage-passenger')) {
                return true;
            }
            if (package_id == undefined || package_name == undefined) {
                package_id = extra_package_element.find('.service-tour-extrapackage-packagename-select-input').val();
                package_name = extra_package_element.find('.service-tour-extrapackage-packagename-select-input').val();
                if ( package_id == undefined || package_id.trim() == '' || package_name == undefined || package_name.trim() == '') {
                    _msgalert.error("Nội dung tại dịch vụ thứ " + extra_package_element.find('.service-tour-extrapackage-order').text() + ' của Bảng kê dịch vụ không được bỏ trống')
                    validate_failed = true
                    return false;
                }
            }
                         
            var extra_package = {
                id: extra_package_element.attr('data-extra-package-id'),
                package_id: package_id,
                package_code: package_name,
                base_price: extra_package_element.find('.service-tour-extrapackage-baseprice').val().replaceAll(',', ''),
                quantity: extra_package_element.find('.service-tour-extrapackage-quantity').val().replaceAll(',', ''),
                amount: extra_package_element.find('.service-tour-extrapackage-amount').val().replaceAll(',', ''),
                price: extra_package_element.find('.service-tour-extrapackage-price').val().replaceAll(',', ''),
                profit: extra_package_element.find('.service-tour-extrapackage-profit').val().replaceAll(',', '')
            }
            object_summit.extra_packages.push(extra_package);
        });
        if (validate_failed) return;
      
        $('.service-tour-guest-row').each(function (index, item) {
            var guest_element = $(item);
            /*
            var name = guest_element.find('.service-tour-guest-name').val()
            if (name == undefined || name.trim() == '') {
                _msgalert.error('Vui lòng nhập vào tên của thành viên danh sách đoàn thứ ' + guest_element.find('.service-tour-guest-order').val())
                validate_failed = true
                return false;
            }*/
            var guest = {
                id: guest_element.attr('data-extra-guest-id'),
                name: guest_element.find('.service-tour-guest-name').val(),
                gender: guest_element.find('.service-tour-guest-gender').val(),
                birthday: guest_element.find('.service-tour-guest-birthday').val() == undefined || guest_element.find('.service-tour-guest-birthday').val().trim()==''?null: _global_function.GetDayText(guest_element.find('.service-tour-guest-birthday').data('daterangepicker').startDate._d, true),
                cccd: guest_element.find('.service-tour-guest-cccd').val(),
                room_number: guest_element.find('.service-tour-guest-room-number').val(),
                note: guest_element.find('.service-tour-guest-note').val(),
            }
            object_summit.guest.push(guest);
        });

        if (validate_failed) return;

        var descriptiion = _order_detail_html.summit_confirmbox_create_tour_service_description.replace('{is_new}', 'đã được sửa')
        _msgconfirm.openDialog(_order_detail_html.summit_confirmbox_title, descriptiion, function () {
            $('.btn-summit-service-tour').attr('disabled', 'disabled')
            $('.btn-summit-service-tour').addClass('disabled')

            $.ajax({
                url: "SummitTourServiceData",
                type: "post",
                data: { data: object_summit },
                success: function (result) {
                    if (result != undefined && result.status == 0) {
                        _msgalert.success(result.msg);
                        _order_detail_tour.Close();
                        _global_function.ConfirmFileUpload($('.attachment-file-block'), result.data)

                        setTimeout(function () {
                            window.location.reload();
                        }, 1000);
                    }
                    else {
                        _msgalert.error(result.msg);
                        $('.btn-summit-service-tour').removeAttr('disabled')
                        $('.btn-summit-service-tour').removeClass('disabled')
                        // $('.img_loading_summit').remove()
                    }
                }
            });
        });
       

    },
    OnApplyExistsProductTour: function (select_element) {
        var tour_product_id = select_element.find(':selected').val()
        $('.img_loading_select_tour_product').show()
        if (tour_product_id != undefined && !isNaN(parseInt(tour_product_id))) {
            $.ajax({
                url: "GetTourProductDetail",
                type: "post",
                data: { id: parseInt(tour_product_id) },
                success: function (result) {
                    if (result != undefined && result.status == 0) {
                        var model = result.data
                        if (model.id > 0) {
                            $('.service-tour-organizing_type').val(model.organizingType).trigger('change')
                           // $('.service-tour-startdate').data('daterangepicker').setStartDate(_global_function.GetDayText(new Date(Date.parse(model.start_date))));
                           // $('.service-tour-enddate').data('daterangepicker').setStartDate(_global_function.GetDayText(new Date(Date.parse(model.end_date))));
                            $('.service-tour-type').val(model.tourType).trigger('change')
                            $('.service-tour-product-name-manual').val(model.tourName)
                            $('.service-tour-startpoint').append(_order_detail_html.html_tour_option.replaceAll('{tour_product_id}', model.startPoint).replaceAll('{tour_name}', model.start_point_name).replaceAll('{selected}', 'selected'))
                            var endpoint_html = '';
                            if (model.end_point_name != undefined) {
                                $(model.end_point_name).each(function (index, item) {
                                    endpoint_html += _order_detail_html.html_tour_option.replaceAll('{tour_product_id}', item.id).replace('{tour_name}', item.endpoint_name).replaceAll('{selected}', 'selected')
                                });
                            }
                            $('.service-tour-endpoint').append(endpoint_html)

                        }
                    }
                }
            });
        }
        $('.img_loading_select_tour_product').hide()

    },
}


