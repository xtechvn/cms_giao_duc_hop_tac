var _order_detail_vinwonder = {
    ServiceType:6,
    Initialization: function (order_id, booking_id) {
        $('#vinwonderbooking-service').addClass('show')
        _order_detail_vinwonder.AddvinwonderServicePackage(order_id, booking_id)
        _order_detail_vinwonder.AddVinWonderServiceGuests(booking_id)
        _order_detail_common.UserSuggesstion($('.add-service-vinwonder-main-staff'))
        _order_detail_vinwonder.VinWonderLocationSuggestion($('.add-service-vinwonder-select-location'))
        _order_detail_common.FileAttachment(booking_id, _order_detail_vinwonder.ServiceType)
        _order_detail_vinwonder.DynamicBind()
    },
    DynamicBind: function () {
        $('body').on('keyup', '.service-vinwonder-extrapackage-baseprice, .service-vinwonder-extrapackage-quantity, .service-vinwonder-extrapackage-saleprice', function () {
            var element = $(this)
            var row_element = element.closest('.service-vinwonder-packages-row')
            var table_element = element.closest('.service-vinwonder-packages-tbody')
            _order_detail_vinwonder.CalucateRowAmountandProfit(row_element)
            _order_detail_vinwonder.CalucateAmount(table_element)
            _order_detail_vinwonder.CalucateProfit(table_element)
            _order_detail_vinwonder.CalucateServiceAmountandProfit()
        });
        $('body').on('keyup', '#servicemanual-vinwonder-commission, #servicemanual-vinwonder-other-amount', function () {
            _order_detail_vinwonder.CalucateServiceAmountandProfit()

        });
        $('body').on('apply.daterangepicker', '.service-vinwonder-from-date', function () {
            _order_detail_common.OnApplyStartDateOfBookingRangeDatetime($(this), $('.service-vinwonder-to-date'))
        });
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

                formData.append("service_type", "6");

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
                                if (name == null || _global_function.CheckIfSpecialCharracter(name)) {
                                    _msgalert.error('Import danh sách thất bại, dữ liệu trong tệp tin không chính xác, vui lòng kiểm tra lại / liên hệ IT')
                                    incorrect_data = true;
                                    return false;
                                }
                            });
                            if (incorrect_data) {
                                $('.img_loading_upload_file').hide()
                                return
                            }
                            $('.service-vinwonder-guest-row').remove()
                            $(result.data).each(function (index, item) {
                                var summary_row = $('.service-vinwonder-guest-summary-row')
                                var new_position = _order_detail_tour.GetLastestGuestNo(summary_row) + 1;
                                summary_row.before(_order_detail_html.html_service_vinwonder_add_guest_tr.replaceAll('@(++index)', '' + new_position).replaceAll('{guest-name}', item.fullName == undefined || item.fullName == null ? '' : item.fullName).replaceAll('{email}', item.email == undefined || item.email == null ? '' : item.email).replaceAll('{phone}', item.phone == undefined || item.phone == null ? '' : item.phone).replaceAll('{note}', item.note == null || item.note == undefined ? '' : _global_function.RemoveSpecialCharacter(item.note)))

                            });
                            $('.new_vinwonder_guest_row').each(function (index, item) {
                                var row_element = $(this);
                                _order_detail_common.SingleDatePickerBirthDay(row_element.find('.service-vinwonder-guest-birthday'), 'up')
                                _order_detail_common.Select2WithFixedOptionAndNoSearch(row_element.find('.service-vinwonder-guest-gender'))
                                row_element.removeClass('new_vinwonder_guest_row')
                            });
                            _order_detail_tour.ReIndexGuest($('.service-vinwonder-guest-tbody'))

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
    },
    RemoveDynamicBind: function () {
        $('body').off('keyup', '.service-vinwonder-packages-baseprice, .service-vinwonder-packages-quantity, .service-vinwonder-packages-profit', function () {

        });
        $('body').off('apply.daterangepicker', '.service-vinwonder-from-date', function () {

        });
    },
    AddvinwonderServicePackage: function (order_id, booking_id) {
        if (booking_id != undefined && !isNaN(parseInt(booking_id))) {
            $.ajax({
                url: "AddvinwonderServicePackages",
                type: "post",
                data: { booking_id: booking_id},
                success: function (result) {
                    $('.service-vinwonder-packages').html(result)
                    _order_detail_common.SingleDateTimePicker($('.service-vinwonder-packages-date'))

                }
            });
        }

    },
    AddVinWonderServiceGuests: function (booking_id) {
        $.ajax({
            url: "AddVinWonderServiceGuests",
            type: "post",
            data: { booking_id: booking_id },
            success: function (result) {
                $('.service-vinwonder-guests').html(result)

            }
        });
    },
    DeletevinwonderBookingpackages: function (element) {
        var row_element = element.closest('.service-vinwonder-packages-row')
        var table_element = element.closest('.service-vinwonder-packages-tbody')

        row_element.remove()

        _order_detail_vinwonder.CalucateProfit(table_element)
        _order_detail_vinwonder.CalucateAmount(table_element)
        _order_detail_vinwonder.ReIndexPackages(table_element)

    },
    Summit: function () {
        var service_code = $('#add-service-vinwonder-form-select').attr('data-service-code')
        var location_id = $('.add-service-vinwonder-select-location').find(':selected').val()
        var operator_id = $('.add-service-vinwonder-main-staff').find(':selected').val()
        var note = $('.service-vinwonder-note').val()

        if (location_id == undefined || location_id == null || location_id.trim() == '') {
            _msgalert.error("Vui lòng chọn địa điểm")
            return;
        }
        if (operator_id == undefined || operator_id == null || operator_id.trim() == '') {
            _msgalert.error("Vui lòng chọn điều hành viên")
            return;
        }
        var pathname = window.location.pathname.split('/');
        var other_amount_element = $('#servicemanual-vinwonder-other-amount')
        var other_amount = other_amount_element.val() == undefined || isNaN(parseFloat(other_amount_element.val().replaceAll(',', ''))) ? 0 : parseFloat(other_amount_element.val().replaceAll(',', ''))

        var discount_element = $('#servicemanual-vinwonder-commission')
        var discount = discount_element.val() == undefined || isNaN(parseFloat(discount_element.val().replaceAll(',', ''))) ? 0 : parseFloat(discount_element.val().replaceAll(',', ''))
        var object_summit = {
            order_id: pathname[pathname.length - 1],
            id: $('#add-service-vinwonder-form-select').attr('data-id'),
            location_id: location_id,
            location_name: $('.add-service-vinwonder-select-location').find(':selected').text(),
            operator_id: operator_id,
            note: note,
            service_code: service_code,
            packages: [],
            guest: [],
            others_amount: other_amount,
            commission: discount
        }
        var validate_failed = false
        $('.service-vinwonder-packages-row').each(function (index, item) {
            var extra_package_element = $(item);
            var package_name = extra_package_element.find('.service-vinwonder-packages-packagename').val();
            if (package_name == undefined || package_name == null || package_name.trim() == '') {
                _msgalert.error("Nội dung tại dịch vụ thứ " + extra_package_element.find('.service-vinwonder-packages-order').text() + ' của Bảng kê dịch vụ không được bỏ trống')
                validate_failed = true
                return false;
            }

            var extra_package = {
                id: extra_package_element.attr('data-extra-package-id'),
                package_name: package_name,
                date_used: _global_function.GetDayText(extra_package_element.find('.service-vinwonder-packages-date').data('daterangepicker').startDate._d,true),
                base_price: _global_function.GetAmountFromCurrencyInput(extra_package_element.find('.service-vinwonder-extrapackage-baseprice')),
                quantity: _global_function.GetAmountFromCurrencyInput(extra_package_element.find('.service-vinwonder-extrapackage-quantity')),
                amount: _global_function.GetAmountFromCurrencyInput(extra_package_element.find('.service-vinwonder-packages-amount')),
                profit: _global_function.GetAmountFromCurrencyInput(extra_package_element.find('.service-vinwonder-packages-profit')),
            }
            object_summit.packages.push(extra_package);
        });
        $('.service-vinwonder-guest-row').each(function (index, item) {
            var guest_element = $(item);
            var guest = {
                id: guest_element.attr('data-extra-guest-id'),
                name: guest_element.find('.service-vinwonder-guest-name').val(),
                email: guest_element.find('.service-vinwonder-guest-email').val(),
                phone: guest_element.find('.service-vinwonder-guest-phone').val(),
                note: guest_element.find('.service-vinwonder-guest-note').val(),
            }
            object_summit.guest.push(guest);
        });
        if (validate_failed) {
            return
        }
        var descriptiion = _order_detail_html.summit_confirmbox_create_other_service_description
        _msgconfirm.openDialog(_order_detail_html.summit_confirmbox_title, descriptiion, function () {
            $('.btn-summit-service-vinwonder').attr('disabled', 'disabled')
            $('.btn-summit-service-vinwonder').addClass('disabled')
            $.ajax({
                url: "SummitvinwonderServicePackages",
                type: "post",
                data: { data: object_summit},
                success: function (result) {
                    if (result != undefined && result.status == 0) {
                        _msgalert.success(result.msg);
                        _order_detail_vinwonder.Close();
                        _global_function.ConfirmFileUpload($('.attachment-file-block'), result.data)

                        setTimeout(function () {
                            window.location.reload();
                        }, 1000);
                    }
                    else {
                        _msgalert.error(result.msg);
                        $('.btn-summit-service-vinwonder').removeAttr('disabled')
                        $('.btn-summit-service-vinwonder').removeClass('disabled')
                    }
                }
            });
        });
    },
    Close: function () {
        $('#vinwonderbooking-service').removeClass('show')
        setTimeout(function () {
            $('#vinwonderbooking-service').remove();
            _order_detail_create_service.StartScrollingBody();
            _order_detail_vinwonder.RemoveDynamicBind();
        }, 300);
    },
    AddvinwonderBookingpackages: function () {
        var table_element = $('.service-vinwonder-packages-tbody')
        var new_position = _order_detail_vinwonder.GetLastestPackagesNo() + 1;
        table_element.find('.service-vinwonder-packages-summary-row').before(_order_detail_html.html_service_vinwonder_new_packages.replaceAll('@(++index)', new_position))
        var date = new Date();
        $('.new-service-vinwonder-packages-date').val(_global_function.GetDayText(date))
        _order_detail_common.SingleDateTimePicker($('.new-service-vinwonder-packages-date'))
        $('.new-service-vinwonder-packages-date').removeClass('new-service-vinwonder-packages-date')

    },
    GetLastestPackagesNo: function () {
        var total = 0;
        $('.service-vinwonder-packages-row').each(function (index, item) {
            total++;
        });
        return total
    },
    ReIndexPackages: function (table_element) {
        var total = 0;
        if (!table_element.find('.service-vinwonder-packages-row')[0]) return;
        table_element.find('.service-vinwonder-packages-row').each(function (index, item) {
            var row_element = $(this);
            total++;
            row_element.find('.service-vinwonder-packages-order').html('' + total)
        });
    },
    CalucateProfit: function (table_element) {
        var profit = 0;

        table_element.find('.service-vinwonder-packages-profit').each(function (index, item) {
            var element = $(this);
            var amount = !isNaN(parseFloat(element.val().replaceAll(',', ''))) && parseFloat(element.val().replaceAll(',', '')) > 0 ? parseFloat(element.val().replaceAll(',', '')) : 0
            profit += amount
        });
        /*
        var other_amount_element = $('#servicemanual-vinwonder-other-amount')
        var commission_element = $('#servicemanual-vinwonder-commission')
        var other_amount = !isNaN(parseFloat(other_amount_element.val().replaceAll(',', ''))) && parseFloat(other_amount_element.val().replaceAll(',', '')) > 0 ? parseFloat(other_amount_element.val().replaceAll(',', '')) : 0
        var commission = !isNaN(parseFloat(commission_element.val().replaceAll(',', ''))) && parseFloat(commission_element.val().replaceAll(',', '')) > 0 ? parseFloat(commission_element.val().replaceAll(',', '')) : 0
        */
        var last_profit = profit  // - other_amount - commission

        var total_element = table_element.find('.service-vinwonder-packages-total-profit')
        total_element.html((last_profit >= 0 ? '' : '-') +_global_function.Comma(last_profit))

    },
    CalucateServiceAmountandProfit: function () {
        var profit = 0;

        $('.service-vinwonder-packages-tbody').find('.service-vinwonder-packages-profit').each(function (index, item) {
            var element = $(this);
            var amount = !isNaN(parseFloat(element.val().replaceAll(',', ''))) && parseFloat(element.val().replaceAll(',', '')) > 0 ? parseFloat(element.val().replaceAll(',', '')) : 0
            profit += amount
        });
        
        var other_amount_element = $('#servicemanual-vinwonder-other-amount')
        var commission_element = $('#servicemanual-vinwonder-commission')
        var other_amount = !isNaN(parseFloat(other_amount_element.val().replaceAll(',', ''))) && parseFloat(other_amount_element.val().replaceAll(',', '')) > 0 ? parseFloat(other_amount_element.val().replaceAll(',', '')) : 0
        var commission = !isNaN(parseFloat(commission_element.val().replaceAll(',', ''))) && parseFloat(commission_element.val().replaceAll(',', '')) > 0 ? parseFloat(commission_element.val().replaceAll(',', '')) : 0
        
        var last_profit = profit   - other_amount - commission


        $('.service-manual-vinwonder-total-service-amount').html($('.service-vinwonder-packages-total-amount').html())

        var total_profit_element = $('.service-manual-vinwonder-total-service-profit')
        total_profit_element.html((last_profit < 0 ? '-' : '') + _global_function.Comma(last_profit))
    },
    CalucateAmount: function (table_element) {
        var total_amount = 0;

        table_element.find('.service-vinwonder-packages-amount').each(function (index, item) {
            var element = $(this);
            var amount = !isNaN(parseFloat(element.val().replaceAll(',', ''))) && parseFloat(element.val().replaceAll(',', '')) > 0 ? parseFloat(element.val().replaceAll(',', '')) : 0
            total_amount += amount
        });
        table_element.find('.service-vinwonder-packages-total-amount').html(_global_function.Comma(total_amount))
    },
    CalucateRowAmountandProfit: function (row_element) {
        var element_base_price = row_element.find('.service-vinwonder-extrapackage-baseprice')
        var element_quanity = row_element.find('.service-vinwonder-extrapackage-quantity')
        var sale_price_element = row_element.find('.service-vinwonder-extrapackage-saleprice')

        var base_price = !isNaN(parseFloat(element_base_price.val().replaceAll(',', ''))) && parseFloat(element_base_price.val().replaceAll(',', '')) > 0 ? parseFloat(element_base_price.val().replaceAll(',', '')) : 0
        var quanity = !isNaN(parseFloat(element_quanity.val().replaceAll(',', ''))) && parseFloat(element_quanity.val().replaceAll(',', '')) > 0 ? parseFloat(element_quanity.val().replaceAll(',', '')) : 0
        var sale_price = !isNaN(parseFloat(sale_price_element.val().replaceAll(',', ''))) && parseFloat(sale_price_element.val().replaceAll(',', '')) > 0 ? parseFloat(sale_price_element.val().replaceAll(',', '')) : 0

        var amount = sale_price * quanity;
        var profit = (sale_price - base_price) * quanity
        row_element.find('.service-vinwonder-packages-amount').val(_global_function.Comma(amount)).change()

        row_element.find('.service-vinwonder-packages-profit').val((profit<0?'-':'')+ _global_function.Comma(profit)).change()
    },
    AddvinwonderGuest: function (element) {
        var table_element = element.closest('.service-vinwonder-guest-tbody')
        var new_position = _order_detail_vinwonder.GetLastestGuestNo(element) + 1;
        table_element.find('.service-vinwonder-guest-summary-row').before(_order_detail_html.html_service_vinwonder_add_guest_tr.replaceAll('@(++index)', '' + new_position).replaceAll('{guest-name}', '').replaceAll('{email}', '').replaceAll('{phone}', '').replaceAll('{note}', ''))
        $('.new_vinwonder_guest_row').each(function (index, item) {
            var row_element = $(this);
            row_element.removeClass('new_vinwonder_guest_row')
        });
        _order_detail_vinwonder.ReIndexGuest(table_element)

    },
    GetLastestGuestNo: function (element) {
        var total = 0;
        element.closest('.service-vinwonder-guest-tbody').find('.service-vinwonder-guest-row').each(function (index, item) {
            total++;
        });
        return total
    },
    DeletevinwonderGuest: function (element) {
        var table_element = element.closest('.service-vinwonder-guest-tbody')
        element.closest('.service-vinwonder-guest-row').remove()
        _order_detail_vinwonder.ReIndexGuest(table_element)

    },
    ReIndexGuest: function (table_element) {
        var total = 0;
        table_element.find('.service-vinwonder-guest-row').each(function (index, item) {
            var row_element = $(this);
            total++;
            row_element.find('.service-vinwonder-guest-order').html('' + total)
        });
    },
    VinWonderLocationSuggestion: function (element) {
        var selected = element.find(':selected').val()
        element.select2();

        $.ajax({
            url: "VinWonderLocationSuggestion",
            type: "post",
            data: { txt_search: "" },
            success: function (result) {
                if (result != undefined && result.data != undefined && result.data.length > 0) {
                    result.data.forEach(function (item) {
                        element.append(_order_detail_html.html_vinwonder_location_option.replaceAll('{id}', item.code).replaceAll('{if_selected}', '').replaceAll('{location_name}', item.name))

                    });
                    element.val(selected).trigger('change');
                }
                else {
                    element.trigger('change');

                }

            }
        });
    },
}


