var _order_detail_hotel = {
    ServiceType:1,
    Initialization: function (hotel_booking_id) {
        $('#AddHotelService').addClass('show')
        _order_detail_common.UserSuggesstion($('#main-hotel-staff'));
        _order_detail_hotel.HotelServiceRoomPopup(hotel_booking_id);
        _order_detail_hotel.HotelServiceRoomPackagesPopup(hotel_booking_id);

        _order_detail_common.SingleDatePicker($('.servicemanual_hotel_checkin'))
        _order_detail_common.SingleDatePicker($('.servicemanual_hotel_checkout'))


        _order_detail_hotel.HotelSuggesstion();
        $("#servicemanual-hotel-contactclient-country").select2()
        _order_detail_common.FileAttachment(hotel_booking_id, _order_detail_hotel.ServiceType)
     

    },
    Close: function () {
        $('#AddHotelService').removeClass('show')
        setTimeout(function () {
            $('#AddHotelService').remove();
            _order_detail_create_service.StartScrollingBody();
            _order_detail_hotel.RemoveDynamicBind()
        }, 300);
    },
    RemoveDynamicBind: function () {
        $('body').off('click', '.modal-order', null);
        $('body').off('click', '.servicemanual_hotel_name_option', null);
        $(document).off('click', null);
        $("body").off('focusout', ".servicemanual-hotel-room-code", null);
        $("body").off('keyup', ".qty_input", null);
        $("body").off('keyup', ".servicemanual-hotel-room-rates-operator-price, .servicemanual-hotel-room-rates-sale-price", null);
        $("body").off('keyup', ".servicemanual-hotel-room-number-of-rooms", null);
        $("body").off('apply.daterangepicker', ".servicemanual_hotel_checkin", null);
        $("body").off('apply.daterangepicker', ".servicemanual_hotel_checkout", null);
        $('#collapseGuest').off('click', '.tang_sl', null);
        $('#collapseGuest').off('click', '.giam_sl', null);
        $("body").off('keyup', ".servicemanual-hotel-room-number-of-rooms", null);
        $("body").off('click', ".servicemanual-hotel-add-new-room", null);
        $("body").off('click', ".add-room-rates-button", null);
        $("body").off('click', ".delete-room-rates-button", null);
        $("body").off('click', ".servicemanual-hotel-room-delete-room", null);
        $("body").off('click', ".servicemanual-hotel-room-clone-room", null);
        $("body").off('focusout', ".servicemanual-hotel-room-type-name", null);
        $("body").off('click', ".servicemanual-hotel-extrapackage-add-new", null);
        $("body").off('click', ".servicemanual-hotel-extrapackage-delete-extrapackage", null);
        $("body").off('keyup', ".servicemanual-hotel-extrapackage-number-of-extrapackages", null);
        $("body").off('apply.daterangepicker', ".servicemanual-hotel-extrapackage-daterange", null);
        $("body").off('keyup', ".servicemanual-hotel-extrapackage-operator-price, .servicemanual-hotel-extrapackage-sale-price", null);
        $("body").off('apply.daterangepicker', ".servicemanual-hotel-room-rates-daterange", null);
        $("body").off('apply.daterangepicker', ".servicemanual-hotel-roomguest-birthday", null);
        $('.service-hotel-note').keydown(null);
        $("body").off('change', ".import_data_guest", null);
    },
    DynamicBindInput: function () {
        $('body').on('click', '.modal-order', function (event) {
            if (!$(event.target).hasClass('modal-dialog')) {
                $(event.target).closest('.modal').removeClass('show');
                setTimeout(function () {
                    $(event.target).closest('.modal').remove();
                }, 300);
            }

        });
        $('body').on('click', '.servicemanual_hotel_name_option', function (e) {
            var target = $(e.target);
            $('#servicemanual_hotel_name').val(target.html());
            $('#servicemanual_hotel_name').attr('hotel-id', target.attr('value'));
            $("#hotel-suggestion").hide();

        });

        $(document).on('click', function (e) {
            if ($(e.target).closest(".number-people").length === 0) {
                $("#collapseGuest").removeClass('show');
            }

        });

        $("body").on('focusout', ".servicemanual-hotel-room-code", function () {
            _order_detail_hotel.OnChangeNumberOfRoomToGuestSelect();
        });

        $("body").on('keyup', ".qty_input", function () {
            _order_detail_hotel.OnchangeSearchRoom();

        });
        $("body").on('keyup', ".servicemanual-hotel-room-rates-operator-price, .servicemanual-hotel-room-rates-sale-price, .servicemanual-hotel-room-rates-others-amount, .servicemanual-hotel-room-rates-discount", function () {
            var element = $(this)
            var row_element = element.closest('.servicemanual-hotel-room-tr')
            var rate_id = element.attr('data-rate-id')
            _order_detail_hotel.CalucateRateTotalAmount(row_element, rate_id)
            _order_detail_hotel.CalucateRateProfit(row_element, rate_id)
            _order_detail_hotel.CalucateRateOperatorAmount(row_element, rate_id)
            _order_detail_hotel.CalucateTotalOperatorAmountOfHotelRoom();

            _order_detail_hotel.CalucateTotalAmountOfHotelRoom();
            _order_detail_hotel.CalucateTotalProfitOfHotelRoom();
            _order_detail_hotel.CalucateTotalDiscountOfHotelRoom();
            _order_detail_hotel.CalucateTotalOthersAmountOfHotelRoom();

        });
        $("body").on('keyup', ".servicemanual-hotel-room-number-of-rooms", function () {
            var element_num_room = $(this)
            var row_element = element_num_room.closest('.servicemanual-hotel-room-tr')
            row_element.find('.servicemanual-hotel-room-rates-operator-price').each(function (index, item) {
                var element = $(item)
                var rate_id = element.attr('data-rate-id')
                _order_detail_hotel.CalucateRateTotalAmount(row_element, rate_id)
                _order_detail_hotel.CalucateRateProfit(row_element, rate_id)
                _order_detail_hotel.CalucateRateOperatorAmount(row_element, rate_id)
                _order_detail_hotel.CalucateTotalOperatorAmountOfHotelRoom();
                _order_detail_hotel.CalucateTotalAmountOfHotelRoom();
                _order_detail_hotel.CalucateTotalProfitOfHotelRoom();
            })

        });
        $("body").on('apply.daterangepicker', ".servicemanual_hotel_checkin", function (ev, picker) {
            var element = $(this)
          //  element.val(_global_function.GetDayText(element.data('daterangepicker').startDate._d).split(' ')[0])
            _order_detail_common.OnApplyStartDateOfBookingRange($(this), $('#servicemanual_hotel_checkout'));
            _order_detail_hotel.OnAddNewRoomRates();
            _order_detail_hotel.OnAddNewRoomExtraPackages();

        });

        $("body").on('apply.daterangepicker', ".servicemanual_hotel_checkout", function (ev, picker) {
            var element = $(this)
          //  element.val(_global_function.GetDayText(element.data('daterangepicker').startDate._d).split(' ')[0])
            _order_detail_hotel.OnAddNewRoomRates();
            _order_detail_hotel.OnAddNewRoomExtraPackages();

        });

        $('#collapseGuest').on('click', '.tang_sl', function () {
            let seft = $(this);
            let inputElement = seft.parent().siblings('input');
            let current_value = parseInt(inputElement.val());
            let is_room = seft.closest('.sl_giohang_room').length > 0;

            if (is_room) {
                if (current_value < 9) {
                    inputElement.val(current_value + 1);
                    _hotel.appendRoomSearch(current_value + 1);
                }
            } else {
                inputElement.val(current_value + 1);
            }

            _order_detail_hotel.OnchangeSearchRoom();
        });

        $('#collapseGuest').on('click', '.giam_sl', function () {
            let seft = $(this);
            let inputElement = seft.parent().siblings('input');
            let current_value = parseInt(inputElement.val());

            let is_room = seft.closest('.sl_giohang_room').length > 0;
            let is_adult = seft.closest('.adult').length > 0;

            if (current_value > 0) {
                if (is_room || is_adult) {
                    if (current_value > 1) {
                        inputElement.val(current_value - 1);
                        if (is_room) $('#block_room_search_content .line-bottom:last').remove();
                    }
                } else {
                    inputElement.val(current_value - 1);
                }
            }

            _order_detail_hotel.OnchangeSearchRoom();
        });

       
        $("body").on('keyup', ".servicemanual-hotel-room-number-of-rooms", function (ev, picker) {
            var element_num_room = $(this);
            var row_element = element_num_room.closest('.servicemanual-hotel-room-tr')
            row_element.find('.servicemanual-hotel-room-rates-operator-price').each(function (index, item) {
                var element = $(item)
                var rate_id = element.attr('data-rate-id')
                _order_detail_hotel.CalucateRateTotalAmount(row_element, rate_id)
                _order_detail_hotel.CalucateRateProfit(row_element, rate_id)
                _order_detail_hotel.CalucateRateOperatorAmount(row_element, rate_id)
                _order_detail_hotel.CalucateTotalOperatorAmountOfHotelRoom();
                _order_detail_hotel.CalucateTotalAmountOfHotelRoom();
                _order_detail_hotel.CalucateTotalProfitOfHotelRoom();
            })
        });
        $("body").on('click', ".servicemanual-hotel-add-new-room", function () {
            _order_detail_hotel.AddNewHotelRoom();
            _order_detail_hotel.FillHotelGuestSelectRoomOption()
            _order_detail_hotel.OnAddNewRoomRates();

        });
        $("body").on('click', ".add-room-rates-button", function () {
            var element = $(this);
            _order_detail_hotel.AddNewHotelRoomRate(element);
            _order_detail_hotel.OnAddNewRoomRates();


        });
        $("body").on('click', ".delete-room-rates-button", function () {
            var element = $(this);
            _order_detail_hotel.DeleteHotelRoomRate(element);
            _order_detail_hotel.CalucateTotalAmountOfHotelRoom();
        });
        $("body").on('click', ".servicemanual-hotel-room-delete-room", function () {
            var element = $(this);
            var row_element = element.closest('.servicemanual-hotel-room-tr')
            row_element.remove()
            _order_detail_hotel.ReIndexRoomOrder()
            _order_detail_hotel.CalucateTotalOperatorAmountOfHotelRoom();
            _order_detail_hotel.CalucateTotalAmountOfHotelRoom();
            _order_detail_hotel.CalucateTotalProfitOfHotelRoom();
            _order_detail_hotel.FillHotelGuestSelectRoomOption()

        });
        $("body").on('click', ".servicemanual-hotel-room-clone-room", function () {
            var element = $(this);
            _order_detail_hotel.CloneHotelRoom(element);
            _order_detail_hotel.FillHotelGuestSelectRoomOption()
            _order_detail_hotel.OnAddNewRoomRates();

        });
        $("body").on('focusout', ".servicemanual-hotel-room-type-name", function () {
            _order_detail_hotel.FillHotelGuestSelectRoomOption()

        });
        $("body").on('click', ".servicemanual-hotel-extrapackage-add-new", function () {
            var element = $(this);
            _order_detail_hotel.AddHotelRoomExtraPackage();
            _order_detail_hotel.OnAddNewRoomExtraPackages();
        });
        $("body").on('click', ".servicemanual-hotel-extrapackage-delete-extrapackage", function () {
            var element = $(this);
            _order_detail_hotel.DeleteHotelRoomExtrapackage(element);
            _order_detail_hotel.ReIndexExtraPackageOrder();
            _order_detail_hotel.CalucateTotalOperatorAmountOfExtraPackage();
            _order_detail_hotel.CalucateTotalProfitOfExtraPacakge();
            _order_detail_hotel.CalucateTotalAmountOfExtraPackage();
        });
        $("body").on('keyup', ".servicemanual-hotel-extrapackage-number-of-extrapackages", function (ev, picker) {
            var element_num_room = $(this);
            var row_element = element_num_room.closest('.servicemanual-hotel-extrapackage-tr')
            _order_detail_hotel.CalucateExtraPackageTotalAmount(row_element)
            _order_detail_hotel.CalucateExtraPackageProfit(row_element)
            _order_detail_hotel.CalucateExtraPackageOperatorAmount(row_element)
            _order_detail_hotel.CalucateTotalProfitOfExtraPacakge();
            _order_detail_hotel.CalucateTotalAmountOfExtraPackage();
            _order_detail_hotel.CalucateTotalOperatorAmountOfExtraPackage();

        });
        $("body").on('apply.daterangepicker', ".servicemanual-hotel-extrapackage-daterange", function (ev, picker) {
            var element_daterange = $(this);
            element_daterange.val(_global_function.GetDayText(element_daterange.data('daterangepicker').startDate._d).split(' ')[0] + ' - ' + _global_function.GetDayText(element_daterange.data('daterangepicker').endDate._d).split(' ')[0])
            var row_element = element_daterange.closest('.servicemanual-hotel-extrapackage-tr')
            _order_detail_hotel.OnApplyDayUsesToExtraPackageDays(element_daterange)
            var row_element = element_daterange.closest('.servicemanual-hotel-extrapackage-tr')
            _order_detail_hotel.CalucateExtraPackageTotalAmount(row_element)
            _order_detail_hotel.CalucateExtraPackageProfit(row_element)
            _order_detail_hotel.CalucateExtraPackageOperatorAmount(row_element)

            _order_detail_hotel.CalucateTotalProfitOfExtraPacakge();
            _order_detail_hotel.CalucateTotalAmountOfExtraPackage();
            _order_detail_hotel.CalucateTotalOperatorAmountOfExtraPackage();

        });
        $("body").on('keyup', ".servicemanual-hotel-extrapackage-operator-price, .servicemanual-hotel-extrapackage-sale-price", function () {
            var element = $(this)
            var row_element = element.closest('.servicemanual-hotel-extrapackage-tr')
            _order_detail_hotel.CalucateExtraPackageTotalAmount(row_element)
            _order_detail_hotel.CalucateExtraPackageProfit(row_element)
            _order_detail_hotel.CalucateExtraPackageOperatorAmount(row_element)

            _order_detail_hotel.CalucateTotalProfitOfExtraPacakge();
            _order_detail_hotel.CalucateTotalAmountOfExtraPackage();
            _order_detail_hotel.CalucateTotalOperatorAmountOfExtraPackage();


        });
        
        $("body").on('apply.daterangepicker', ".servicemanual-hotel-room-rates-daterange", function (ev, picker) {
            var element_daterange = $(this);
            element_daterange.val(_global_function.GetDayText(element_daterange.data('daterangepicker').startDate._d).split(' ')[0] + ' - ' + _global_function.GetDayText(element_daterange.data('daterangepicker').endDate._d).split(' ')[0])

            _order_detail_hotel.OnApplyDayUsesToRoomNight(element_daterange)
            var row_element = element_daterange.closest('.servicemanual-hotel-room-tr')
            row_element.find('.servicemanual-hotel-room-rates-operator-price').each(function (index, item) {
                var element = $(item)
                var rate_id = element.attr('data-rate-id')
                _order_detail_hotel.CalucateRateTotalAmount(row_element, rate_id)
                _order_detail_hotel.CalucateRateProfit(row_element, rate_id)
                _order_detail_hotel.CalucateRateOperatorAmount(row_element, rate_id)
                _order_detail_hotel.CalucateTotalOperatorAmountOfHotelRoom();

                _order_detail_hotel.CalucateTotalAmountOfHotelRoom();
                _order_detail_hotel.CalucateTotalProfitOfHotelRoom();
                _order_detail_hotel.CalucateTotalOperatorAmountOfExtraPackage();

            })
        });

        $("body").on('apply.daterangepicker', ".servicemanual-hotel-roomguest-birthday", function (ev, picker) {
            var element = $(this)
            element.val(_global_function.GetDayText(element.data('daterangepicker').startDate._d).split(' ')[0])
        });
        $("body").on('keydown', ".servicemanual-hotel-room-rates-daterange", function (e) {
            e.stopPropagation();
            e.preventDefault();
            return false

        });
        $('.service-hotel-note').keydown(function (e) {
            e.stopPropagation();
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
                formData.append("service_type", "1");


                $.ajax({
                    url: "GetGuestFromFile",
                    type: "post",
                    processData: false,
                    contentType: false,
                    data: formData,
                    success: function (result) {
                        if (result != undefined && result.data != undefined && result.data.length > 0) {
                            var incorrect_data = false
                            $(result.data).each(function (index, item) {
                                var name = _global_function.RemoveUnicode(item.name)
                                var note = _global_function.RemoveUnicode(item.note)
                                if (item.name == null || _global_function.CheckIfSpecialCharracter(name) || !_global_function.CheckIfStringIsDate(item.birthday)) {
                                    _msgalert.error('Import danh sách thất bại, dữ liệu trong tệp tin không chính xác, vui lòng kiểm tra lại / liên hệ IT')
                                    incorrect_data = true;
                                    return false;
                                }
                            });
                            if (incorrect_data) {
                                $('.img_loading_upload_file').hide()
                                return
                            }
                            $('.servicemanual-hotel-roomguest-row').remove()
                            $(result.data).each(function (index, item) {
                                // var name_non_unicode = _global_function.RemoveUnicode(item.name)
                                if (item.name == 'null' || item.birthday == 'null' || item.note == 'null' || _global_function.CheckIfSpecialCharracter(_global_function.RemoveUnicode(item.name)) || _global_function.CheckIfSpecialCharracter(_global_function.RemoveUnicode(item.note))) {
                                    _msgalert.error('Import danh sách thất bại, dữ liệu trong tệp tin không chính xác, vui lòng kiểm tra lại / liên hệ IT')
                                    incorrect_data = true;
                                    return false;
                                }
                                var lastest_order = _order_detail_hotel.GetLastestOrderHotelRoomGuestTable();
                                lastest_order = lastest_order + 1;
                                var html = _order_detail_html.html_new_room_guest.replaceAll('{order}', lastest_order).replaceAll('{room_guest_name}', item.name == undefined || item.name == null ? '' : item.name).replaceAll('{room_guest_birthday}', item.birthday == null ? '' : item.birthday).replaceAll('{room_guest_note}', item.note == undefined || item.note == null ? '' : _global_function.RemoveSpecialCharacter(item.note))
                                $("#servicemanual-hotelguest-total-summary").before(html);
                            });
                            _order_detail_hotel.OnAddNewGuest()
                            _order_detail_hotel.FillHotelGuestSelectRoomOption()
                            _msgalert.success(result.msg)

                        }
                        else {

                            $('.img_loading_upload_file').hide()
                            _msgalert.error(result.msg)

                        }
                        $('.img_loading_upload_file').hide()
                    }
                });
            }
            $('.upload-file-guest-btn').removeAttr('disabled')
            element.val('');

          

        });
        $("body").on('keyup', ".servicemanual-hotel-other-amount, .servicemanual-hotel-discount", function () {
            var element = $(this)
            _order_detail_hotel.CalucateTotalServiceProfit();

        });
       
    },
    Summit: function () {
        var validate_failed = false
        //---- Null input
        $('.servicemanual-hotel-roomguest-name').each(function (index, item) {
            var element = $(item);
            if (element.val() == undefined || element.val().trim()=='') {
                error = true;
                _msgalert.error('Vui lòng nhập tên cho thành viên thứ' + element.closest('.servicemanual-hotel-roomguest-row').find('.servicemanual-hotel-roomguest-order').text());
                validate_failed=true
                return false;
            }
        });
        if (validate_failed) return
        if ($('#servicemanual-hotel-numberOfRooms').val() == undefined || $('#servicemanual-hotel-numberOfRooms').val().trim() == '' || parseInt($('#servicemanual-hotel-numberOfRooms').val()) < 1) {
            _msgalert.error('Vui lòng chọn số phòng');
            return;
        }
        if ($('#servicemanual-hotel-numberOfRooms').val() == undefined || $('#servicemanual-hotel-numberOfRooms').val().trim() == '' || parseInt($('#servicemanual-hotel-numberOfRooms').val()) < 1) {
            _msgalert.error('Vui lòng chọn số phòng');
            return;
        }
        if ($('#servicemanual-hotel-name').find(':selected').val() == undefined || $('#servicemanual-hotel-name').find(':selected').val().trim() == ''
        || $('#servicemanual-hotel-name').find(':selected').text() == undefined || $('#servicemanual-hotel-name').find(':selected').text().trim() == '') {
            _msgalert.error('Vui lòng chọn khách sạn');
            return;
        }
        if ($('#servicemanual-hotel-numberOfRooms').val() == undefined || parseInt($('#servicemanual-hotel-numberOfRooms').val()) <= 0) {
            _msgalert.error('Vui lòng nhập vào đúng số phòng (lớn hơn 0)');
            return;
        }
        if ($('#servicemanual_hotel_checkin').data('daterangepicker').startDate._d == undefined){
            _msgalert.error('Vui lòng nhập ngày Check-In');
            return;
        }
        if ($('#servicemanual_hotel_checkout').data('daterangepicker').startDate._d == undefined){
            _msgalert.error('Vui lòng nhập ngày Check-Out');
            return;
        }
        /*
        if (_order_detail_common.GetDateFromNoUpdateDateRangeElement($('#servicemanual_hotel_checkout')) <= _order_detail_common.GetDateFromNoUpdateDateRangeElement($('#servicemanual_hotel_checkin'))) {
            _msgalert.error('Ngày check-out không được nhỏ hơn ngày check-in');
            return;
        }*/
        if ($('#main-hotel-staff').find(':selected').val() == undefined || parseInt($('#main-hotel-staff').find(':selected').val())<=0) {
            _msgalert.error('Vui lòng chọn thông tin nhân viên phụ trách chính');
            return;
        }
        var checkin_date_validate = _order_detail_common.GetDateFromNoUpdateDateRangeElement($('#servicemanual_hotel_checkin'));
        var checkout_date_validate = _order_detail_common.GetDateFromNoUpdateDateRangeElement($('#servicemanual_hotel_checkout'));
        if ($(".servicemanual-hotel-room-tr")[0]) {
            var error = false;
            $('.servicemanual-hotel-room-tr').each(function (index, item) {
                var element = $(item);
                if (element.find('.servicemanual-hotel-room-type-name').val() == undefined || element.find('.servicemanual-hotel-room-type-name').val().trim() == '') {
                    error = true;
                    _msgalert.error('Vui lòng nhập tên hạng phòng cho phòng ' + element.find('.servicemanual-hotel-room-td-order').text());
                    return false;
                }
                if (element.find('.servicemanual-hotel-room-number-of-rooms').val() == undefined || parseFloat(element.find('.servicemanual-hotel-room-number-of-rooms').val()) <= 0 || isNaN(parseFloat(element.find('.servicemanual-hotel-room-number-of-rooms').val()))) {
                    error = true;
                    _msgalert.error('Vui lòng nhập số lượng phòng của phòng ' + element.find('.servicemanual-hotel-room-order').text());
                    return false;
                }
                element.find('.servicemanual-hotel-room-rates-code').each(function (index_2, item_2) {
                    var rate_name = $(item_2);
                    if (rate_name.val() == undefined || rate_name.val() <= 0) {
                        error = true;
                        _msgalert.error('Vui lòng nhập tên gói cho phòng ' + element.find('.servicemanual-hotel-room-order').text());
                        return false;
                    }
                    var rate_id = rate_name.attr('data-rate-id')
                    //-- Check Daterange
                    var rate_daterange_element = _order_detail_hotel.GetHotelRatesDaysUse(element, rate_id)
                    if (rate_daterange_element == undefined) {
                        _msgalert.error('Vui lòng nhập khoảng ngày cho gói trong phòng ' + element.find('.servicemanual-hotel-room-order').text() + ' nằm trong khoảng ngày Checkin - Checkout [1]');
                        error = true;
                        return false
                    }
                    else {
                        var package_from = _order_detail_common.GetDateFromNoUpdateDateRangeElementDateRange(rate_daterange_element, false)
                        var package_to = _order_detail_common.GetDateFromNoUpdateDateRangeElementDateRange(rate_daterange_element, true)
                        if (package_from.Date < checkin_date_validate.Date || package_from.Date > checkout_date_validate.Date
                            || package_to.Date < checkin_date_validate.Date || package_to.Date > checkout_date_validate.Date) {
                            error = true;
                            _msgalert.error('Vui lòng nhập khoảng ngày cho gói trong phòng ' + element.find('.servicemanual-hotel-room-order').text() + ' nằm trong khoảng ngày Checkin - Checkout');
                            return false;
                        }
                    }
                    //-- Check Operator Price
                    var operator_price_element = _order_detail_hotel.GetHotelRatesOperatorPrice(element, rate_id)
                    if (operator_price_element == undefined) {
                        _msgalert.error('Vui lòng nhập [Giá nhập] cho gói trong phòng ' + element.find('.servicemanual-hotel-room-order').text() + '[1]');
                        error = true;
                        return false
                    }
                    else if (operator_price_element.val() == undefined || operator_price_element.val().trim()=='') {
                        _msgalert.error('Vui lòng nhập [Giá nhập] cho gói trong phòng ' + element.find('.servicemanual-hotel-room-order').text() + '');
                        error = true;
                        return false
                    }
                    //-- Check Sale Price
                    var saler_price_element = _order_detail_hotel.GetHotelRatesSalePrice(element, rate_id)
                    if (saler_price_element == undefined) {
                        _msgalert.error('Vui lòng nhập [Giá bán] cho gói trong phòng ' + element.find('.servicemanual-hotel-room-order').text() + '[1]');
                        error = true;
                        return false
                    }
                    else if (saler_price_element.val() == undefined || saler_price_element.val().trim() == '') {
                        _msgalert.error('Vui lòng nhập [Giá bán] cho gói trong phòng ' + element.find('.servicemanual-hotel-room-order').text() + '');
                        error = true;
                        return false
                    }
                });
               
            });
            if (error) return;
        }
        else {
            _msgalert.error('Vui lòng nhập vào ít nhất 01 phòng khi tạo mới dịch vụ khách sạn');
            return;
        }

        if ($(".servicemanual-hotel-roompackage-row")[0]) {
            var error = false;
            $('.servicemanual-hotel-roompackage-row').each(function (index, item) {
                var element = $(item);
                if (element.find('.servicemanual-hotel-roompackage-total-amount').val() == undefined || parseInt(element.find('.servicemanual-hotel-roompackage-total-amount').val()) <= 0 || isNaN(parseInt(element.find('.servicemanual-hotel-roompackage-total-amount').val()))) {
                    error = true;
                    _msgalert.error('Vui lòng nhập thành tiền cho phụ thu / dịch vụ ' + element.find('.servicemanual-hotel-roompackage-packagename').text());
                    return false;
                }
            });
            if (error) return;
        }
       
        if ($(".servicemanual-hotel-roomguest-row")[0]) {
            var error = false;
            $('.servicemanual-hotel-roomguest-row').each(function (index, item) {
                var element = $(item)
                /*
                if (element.find('.servicemanual-hotel-roomguest-roomselect').val() == undefined) {
                    _msgalert.error('Vui lòng nhập chọn phòng cho thành viên thứ ' + element.find('.servicemanual-hotel-roomguest-order').text());
                    error = true;
                    return false;
                }*/
                if (element.find('.servicemanual-hotel-roomguest-name').val() == undefined || element.find('.servicemanual-hotel-roomguest-name').val() == '') {
                    _msgalert.error('Vui lòng nhập tên cho thành viên thứ ' + element.find('.servicemanual-hotel-roomguest-order').text());
                    error = true;
                    return false;
                }
            });
            if (error) return;
        }

        if ($('.servicemanual-hotel-roomguest-name').length > parseInt($('#servicemanual-hotel-numberOfPeople').text())) {
            _msgalert.error('Số thành viên trong đoàn không được vượt quá số người tối đa');
            return;
        }
        else if (parseInt($('#servicemanual-hotel-numberOfPeople').text()) == undefined || parseInt($('#servicemanual-hotel-numberOfPeople').text()) <= 0) {
            _msgalert.error('Số người trong phần đặt dịch vụ khách sạn phải lớn hơn 0');
            return;

        }

        //-- Collect & push data:
        var pathname = window.location.pathname.split('/');
        let adult = 0;
        let baby = 0;
        let infant = 0;

        $('#block_room_search_content .row').each(function () {
            let seft = $(this);
            adult = parseInt(seft.find('.adult .qty_input').val()) <= 0 || isNaN(parseInt(seft.find('.adult .qty_input').val())) ? 0 : parseInt(seft.find('.adult .qty_input').val());
            baby = parseInt(seft.find('.baby .qty_input').val()) <= 0 || isNaN(parseInt(seft.find('.baby .qty_input').val())) ? 0 : parseInt(seft.find('.baby .qty_input').val());
            infant = parseInt(seft.find('.infant .qty_input').val()) <= 0 || isNaN(parseInt(seft.find('.infant .qty_input').val())) ? 0 : parseInt(seft.find('.infant .qty_input').val());
        });
        var arrive_date = _order_detail_common.GetDateFromNoUpdateDateRangeElement($('#servicemanual_hotel_checkin'), false)
        var departure_date = _order_detail_common.GetDateFromNoUpdateDateRangeElement($('#servicemanual_hotel_checkout'), false)

        var discount_element = $('#servicemanual-hotel-discount')
        var other_amount_element = $('#servicemanual-hotel-other-amount')
        var discount = discount_element.val() != undefined? discount_element.val().replaceAll(',', ''):'0'
        var other_amount = other_amount_element.val() != undefined ? other_amount_element.val().replaceAll(',', ''):'0'

        var object_summit = {
            order_id: pathname[pathname.length - 1],
            hotel: {
                hotel_id: $('#servicemanual-hotel-name').find(':selected').val(),
                hotel_name: $('#servicemanual-hotel-name').find(':selected').text().split("-")[0],
                number_of_rooms: $('#servicemanual-hotel-numberOfRooms').val(),
                arrive_date: _global_function.GetDayText(arrive_date, true),
                departure_date: _global_function.GetDayText(departure_date,true),
                main_staff_id: $('#main-hotel-staff').find(':selected').val(),
                id: $('#form_add_hotel_service').attr('data-booking-id') == undefined ? "0" : $('#form_add_hotel_service').attr('data-booking-id'),
                number_of_adult: adult,
                number_of_child: baby,
                number_of_infant: infant,
                service_code: $('#form_add_hotel_service').attr('data-service-code'),
                note: $('.service-hotel-note').val(),
                discount: discount,
                other_amount: other_amount
            },
            rooms: [],
            extra_package: [],
            contact_client: {},
            guest: []
        };
        var is_success_room = true;
        $('.servicemanual-hotel-room-tr').each(function (index, item) {
            var element = $(item);
            var obj_hotel_room = {
                id: element.attr('data-room-id') == undefined ? "0" : element.attr('data-room-id'),
                room_no: element.find('.servicemanual-hotel-room-td-order').text(),
                room_type_id: element.attr('data-room-type-id'),
                room_type_code: element.attr('data-room-type-code'),
                room_type_name: element.find('.servicemanual-hotel-room-type-name').val(),
                number_of_rooms: element.find('.servicemanual-hotel-room-number-of-rooms').val().replaceAll(',',''),
                package: []
            };
            element.find('.servicemanual-hotel-room-rates-code').each(function (index_2, item_2) {
                var package_element = $(item_2);
                var rate_id = package_element.attr('data-rate-id').trim()
                //-- Get Daterange element
                var rate_daterange_element = _order_detail_hotel.GetHotelRatesDaysUse(element, rate_id)
               
                //var SD1 = rate_daterange_element.startDate._d.setHours(0, 0, 0, 0);
                //var ED1 = rate_daterange_element.endDate._d.setHours(0, 0, 0, 0);
                var SD1 = _order_detail_common.GetDateFromNoUpdateDateRangeElementDateRange(rate_daterange_element, false)
                var ED1 = _order_detail_common.GetDateFromNoUpdateDateRangeElementDateRange(rate_daterange_element, true)
                var package_from = _global_function.GetDayText(SD1, true);
                var package_to = _global_function.GetDayText(ED1, true);
                if (SD1 >= ED1) {
                    _msgalert.error('Thời gian sử dụng tại Phòng STT: ' + obj_hotel_room.room_no + ' - ' + obj_hotel_room.room_type_id + ' gói ' + package_element.val() + ' không được nhỏ hơn 1 ngày');
                    is_success_room = false;
                    return false;
                }
                var is_success_package = true;
              
                if (element.find('.servicemanual-hotel-room-rates-daterange').length > 1) {
                    element.find('.servicemanual-hotel-room-rates-daterange').each(function (index_compare, item_compare) {
                        var item_compare_element = $(item_compare);
                        if (item_compare_element.attr('data-rate-id').trim() == rate_id) return true
                        var SD2 = item_compare_element.data('daterangepicker').startDate._d.setHours(0, 0, 0, 0);
                        var ED2 = item_compare_element.data('daterangepicker').endDate._d.setHours(0, 0, 0, 0);
                        var compare_date = (SD2 <= SD1 && ED2 <= SD1) || (SD2 >= ED1 && ED2 >= ED1)
                        if (!compare_date)
                        {
                            var rate_id_compare_code = _order_detail_hotel.GetHotelRatesCode(element, item_compare_element.attr('data-rate-id').trim())
                            _msgalert.error('Phòng ' + obj_hotel_room.room_no + ': Gói ' + package_element.val() + ' đang có khoảng ngày trùng với Gói ' + rate_id_compare_code.val());
                            is_success_package = false;
                            return false;
                        }
                    });
                    if (!is_success_package) {
                        is_success_room = false;
                        return false;
                    }
                }
                var operator_price_element = _order_detail_hotel.GetHotelRatesOperatorPrice(element, rate_id)
                var sale_price_element = _order_detail_hotel.GetHotelRatesSalePrice(element, rate_id)
                var amount_element = _order_detail_hotel.GetHotelRatesAmount(element, rate_id)
                var profit_element = _order_detail_hotel.GetHotelRatesProfit(element, rate_id)
                var nights_element = _order_detail_hotel.GetHotelRatesNights(element, rate_id)
                obj_hotel_room.package.push({
                    id: isNaN(parseInt(rate_id)) || parseInt(rate_id) < 0 ? 0 : parseInt(rate_id),
                    package_code: package_element.val(),
                    from: package_from,
                    to: package_to,
                    operator_price: operator_price_element.val().replaceAll(',', ''),
                    sale_price: sale_price_element.val().replaceAll(',', ''),
                    amount: amount_element.val().replaceAll(',', ''),
                    profit: profit_element.val().replaceAll(',', ''),
                    nights: nights_element.val().replaceAll(',', ''),
                });
            });
            if (!is_success_room) return false;
            object_summit.rooms.push(obj_hotel_room);
        });
        if (!is_success_room) return;
        $('.servicemanual-hotel-extrapackage-tr').each(function (index, item) {
            var element = $(item);
            var extra_name = element.find('.servicemanual-hotel-extrapackage-type-name').val()
            var extra_code = element.find('.servicemanual-hotel-extrapackage-code').val()
            if (extra_name == null || extra_name == undefined || extra_name.trim() == '') {
                is_success_room = false
                _msgalert.error('Tên dịch vụ tại Bảng kê phụ thu - vị trí thứ ' + element.find('.servicemanual-hotel-extrapackage-td-order').html() + ' không được để trống');
                return false;
            }
            if (extra_code == null || extra_code == undefined || extra_code.trim() == '') {
                is_success_room = false
                _msgalert.error('Tên gói tại Bảng kê phụ thu - vị trí thứ ' + element.find('.servicemanual-hotel-extrapackage-td-order').html() + ' không được để trống');
                return false;
            }
            var SD1 = _order_detail_common.GetDateFromNoUpdateDateRangeElementDateRange(element.find('.servicemanual-hotel-extrapackage-daterange'), false)
            var ED1 = _order_detail_common.GetDateFromNoUpdateDateRangeElementDateRange(element.find('.servicemanual-hotel-extrapackage-daterange'), true)
            var ex_package_from = _global_function.GetDayText(SD1, true);
            var ex_package_to = _global_function.GetDayText(ED1, true);
            var obj_package = {
                id: element.attr('data-extrapackage-id') == undefined ? "0" : element.attr('data-extrapackage-id'),
                package_id: element.attr('.data-extrapackage-packageid') == undefined ? '' : element.attr('data-extrapackage-packageid'),
                name: element.find('.servicemanual-hotel-extrapackage-type-name').val(),
                code: element.find('.servicemanual-hotel-extrapackage-code').val(),
                start_date: ex_package_from,
                end_date: ex_package_to,
                operator_price: element.find('.servicemanual-hotel-extrapackage-operator-price').val().replaceAll(',', '') == undefined ? '0' : element.find('.servicemanual-hotel-extrapackage-operator-price').val().replaceAll(',', ''),
                sale_price: element.find('.servicemanual-hotel-extrapackage-sale-price').val().replaceAll(',', '') == undefined ? '0' : element.find('.servicemanual-hotel-extrapackage-sale-price').val().replaceAll(',', ''),
                nights: element.find('.servicemanual-hotel-extrapackage-nights').val().replaceAll(',', '') == undefined ? '0' : element.find('.servicemanual-hotel-extrapackage-nights').val().replaceAll(',', ''),
                number_of_extrapackages: element.find('.servicemanual-hotel-extrapackage-number-of-extrapackages').val().replaceAll(',', '') == undefined ? '0' : element.find('.servicemanual-hotel-extrapackage-number-of-extrapackages').val().replaceAll(',', ''),
                amount: element.find('.servicemanual-hotel-extrapackage-total-amount').val().replaceAll(',', '') == undefined ? '0' : element.find('.servicemanual-hotel-extrapackage-total-amount').val().replaceAll(',', ''),
                profit: element.find('.servicemanual-hotel-extrapackage-profit').val().replaceAll(',', '') == undefined ? '0' : element.find('.servicemanual-hotel-extrapackage-profit').val().replaceAll(',', ''),
            };
            object_summit.extra_package.push(obj_package);
        });
        if (!is_success_room) return;
        if ($(".servicemanual-hotel-roomguest-row")[0]) {
            $('.servicemanual-hotel-roomguest-row').each(function (index, item) {
                var element = $(item)
                var guest = {
                    id: element.attr('data-guest-id') == undefined ? "0" : element.attr('data-guest-id'),
                    name: element.find('.servicemanual-hotel-roomguest-name').val(),
                    birthday: element.find('.servicemanual-hotel-roomguest-birthday').val() == undefined || element.find('.servicemanual-hotel-roomguest-birthday').val().trim()==''?null: _global_function.GetDayText(element.find('.servicemanual-hotel-roomguest-birthday').data('daterangepicker').startDate._d, true),
                    room_no: element.find('.servicemanual-hotel-roomguest-roomselect').find(':selected').val(),
                    note: element.find('.servicemanual-hotel-roomguest-note').val(),
                    type: element.find('.servicemanual-hotel-roomguest-type').find(':selected') == undefined ? '0' : element.find('.servicemanual-hotel-roomguest-type').find(':selected').val(),
                }
                object_summit.guest.push(guest);
            });
        }
        
        if ($('#form_add_hotel_service').attr('data-booking-id') != undefined) {
            _msgconfirm.openDialog(_order_detail_html.summit_confirmbox_title, _order_detail_html.summit_confirmbox_create_hotel_service_description, function () {
                $('.btn_summit_service_hotel').attr('disabled', 'disabled')
                $('.btn_summit_service_hotel').addClass('disabled')
                $('#summit-button-div').append(_order_detail_html.html_loading_gif);

                $.ajax({
                    url: "/Order/SummitHotelServiceData",
                    type: "post",
                    data: { data: object_summit},
                    success: function (result) {
                        if (result != undefined && result.status == 0) {
                            _msgalert.success(result.msg);
                            _order_detail_hotel.Close();
                            _global_function.ConfirmFileUpload($('.attachment-file-block'), result.data)
                            setTimeout(function () {
                                window.location.reload();
                            }, 1000);
                        }
                        else {
                            _msgalert.error(result.msg);
                            $('.btn_summit_service_hotel').removeAttr('disabled')
                            $('.btn_summit_service_hotel').removeClass('disabled')
                            $('.img_loading_summit').remove()
                        }

                    }
                });
            });
        }
        else {
            $('.btn_summit_service_hotel').attr('disabled', 'disabled')
            $('.btn_summit_service_hotel').addClass('disabled')
            $('#summit-button-div').append(_order_detail_html.html_loading_gif);

            $.ajax({
                url: "/Order/SummitHotelServiceData",
                type: "post",
                data: { data: object_summit},
                success: function (result) {
                    if (result != undefined && result.status == 0) {
                        _msgalert.success(result.msg);
                        _order_detail_hotel.Close();
                        _global_function.ConfirmFileUpload($('.attachment-file-block'), result.data)
                        setTimeout(function () {
                            window.location.reload();
                        }, 1000);

                    }
                    else {
                        _msgalert.error(result.msg);
                        $('.btn_summit_service_hotel').removeAttr('disabled')
                        $('.btn_summit_service_hotel').removeClass('disabled')
                        $('.img_loading_summit').remove()
                    }

                }
            });
        }
      

    },
    AddNewHotelRoom: function () {
        var room_max_value = $('#servicemanual-hotel-numberOfRooms').val();
        if (room_max_value == undefined || parseInt(room_max_value) == undefined || parseInt(room_max_value) <= 0) {
            _msgalert.error('Vui nhập vào số phòng tại thông tin khách sạn');
            return;
        }
        var room_exists = $('#servicemanual-hotel-rooms').find('.servicemanual-hotel-room-tr').length;
        if (room_exists >= parseInt(room_max_value)) {
            _msgalert.error('Số phòng tại mục [Bảng kê dịch vụ phòng] không được vượt quá số phòng đã nhập bên trên');
            return;
        }
        var lastest_order = _order_detail_hotel.GetLastestHotelRoomOrder()
        var min_range = _order_detail_common.GetDateFromNoUpdateDateRangeElement($('#servicemanual_hotel_checkin'))
        var max_range = _order_detail_common.GetDateFromNoUpdateDateRangeElement($('#servicemanual_hotel_checkout'))
        var min_time = _global_function.GetDayTextDateRangePicker(min_range);
        var max_time = _global_function.GetDayTextDateRangePicker(max_range);
        var html_append = _order_detail_html.html_service_hotel_newroom.replaceAll('{{new_room_id}}', '0').replaceAll('{{room_order}}', '' + (lastest_order + 1)).replaceAll('{{new_rate_id}}', '0').replaceAll('{date_range}', min_time + ' - ' + max_time)
        $('.servicemanual-hotel-room-total-summary').before(html_append)
        _order_detail_hotel.ReIndexRoomOrder()
        $('.servicemanual-hotel-room-tr').each(function (index, item) {
            var element = $(item);
            _order_detail_hotel.CheckIfOnlyRateInRoom(element)
        });
      

    },
    AddNewHotelRoomRate: function (element) {
        var row_element = element.closest('.servicemanual-hotel-room-tr')
        var id = 0;
        while (id > -50 ) {
            var exists = _order_detail_hotel.CheckIfNewHotelRoomRateIdsExists(row_element, id);
            if (!exists) break;
            id--
        }
        var min_range = _order_detail_common.GetDateFromNoUpdateDateRangeElement($('#servicemanual_hotel_checkin'))
        var max_range = _order_detail_common.GetDateFromNoUpdateDateRangeElement($('#servicemanual_hotel_checkout'))
        var min_time = _global_function.GetDayTextDateRangePicker(min_range);
        var max_time = _global_function.GetDayTextDateRangePicker(max_range);

        var html_code = _order_detail_html.html_service_hotel_newrates_code.replaceAll('{{new_rate_id}}', ('' + id).trim())
        row_element.find('.servicemanual-hotel-room-td-rates-code').append(html_code)
        var html_daterange = _order_detail_html.html_service_hotel_newrates_daterange.replaceAll('{{new_rate_id}}', ('' + id).trim()).replaceAll('{date_range}', min_time + ' - ' + max_time)
        row_element.find('.servicemanual-hotel-room-td-rates-daterange').append(html_daterange)
        var html_operatorprice = _order_detail_html.html_service_hotel_newrates_operatorprice.replaceAll('{{new_rate_id}}', ('' + id).trim())
        row_element.find('.servicemanual-hotel-room-td-rates-operator-price').append(html_operatorprice)
        var html_sale = _order_detail_html.html_service_hotel_newrates_saleprice.replaceAll('{{new_rate_id}}', ('' + id).trim())
        row_element.find('.servicemanual-hotel-room-td-rates-sale-price').append(html_sale)
        var html_nights = _order_detail_html.html_service_hotel_newrates_nights.replaceAll('{{new_rate_id}}', ('' + id).trim())
        row_element.find('.servicemanual-hotel-room-td-rates-nights').append(html_nights)
        var html_totalamount = _order_detail_html.html_service_hotel_newrates_totalamount.replaceAll('{{new_rate_id}}', ('' + id).trim())
        row_element.find('.servicemanual-hotel-room-td-rates-total-amount').append(html_totalamount)
        var html_profit = _order_detail_html.html_service_hotel_newrates_profit.replaceAll('{{new_rate_id}}', ('' + id).trim())
        row_element.find('.servicemanual-hotel-room-td-rates-profit').append(html_profit)
        var html_operator_amount = _order_detail_html.html_service_hotel_newrates_operator_amount.replaceAll('{{new_rate_id}}', ('' + id).trim())
        row_element.find('.servicemanual-hotel-room-td-rates-operator-amount').append(html_operator_amount)

        _order_detail_hotel.OnAddNewRoomRates();
        _order_detail_hotel.CheckIfOnlyRateInRoom(row_element)


    },
    CheckIfNewHotelRoomRateIdsExists: function (row_element, id) {
        var exists = false;
        row_element.find('.servicemanual-hotel-room-rates-code').each(function (index, item) {
            var compare_element = $(item)
            if (compare_element.attr('data-rate-id').trim() == ('' + id).trim()) {
                exists = true
                return false
            }
        })
        return exists
    },
    CheckIfOnlyRateInRoom: function (row_element, id) {
        if (!row_element.find('.servicemanual-hotel-room-div-code')[1]) {
            row_element.find('.servicemanual-hotel-room-div-code').find('.delete-room-rates-button').hide()
        }
        else {
            row_element.find('.servicemanual-hotel-room-div-code').find('.delete-room-rates-button').show()
            row_element.find('.servicemanual-hotel-room-div-code').each(function (index, item) {
                var compare_element = $(item)
                compare_element.find('.delete-room-rates-button').show()
            })
        }
        
    },
    DeleteHotelRoomRate: function (element) {
        var rate_id = element.closest('.servicemanual-hotel-room-div-code').find('.servicemanual-hotel-room-rates-code').attr('data-rate-id')
        var row_element = element.closest('.servicemanual-hotel-room-tr')
        var element_code = _order_detail_hotel.GetHotelRatesCode(row_element, rate_id)
        var element_daterange = _order_detail_hotel.GetHotelRatesDaysUse(row_element, rate_id)
        var element_operatorprice = _order_detail_hotel.GetHotelRatesOperatorPrice(row_element, rate_id)
        var element_sale = _order_detail_hotel.GetHotelRatesSalePrice(row_element, rate_id)
        var element_nights = _order_detail_hotel.GetHotelRatesNights(row_element, rate_id)
        var element_totalamount = _order_detail_hotel.GetHotelRatesAmount(row_element, rate_id)
        var element_profit = _order_detail_hotel.GetHotelRatesProfit(row_element, rate_id)
        //var element_others = _order_detail_hotel.GetHotelRatesOtherAmount(row_element, rate_id)
        //var element_discount = _order_detail_hotel.GetHotelRatesDiscount(row_element, rate_id)
        var element_operator_amount = _order_detail_hotel.GetHotelRatesOperatorAmount(row_element, rate_id)
        element_code.closest('.d-flex').remove()
        element_daterange.closest('.d-flex').remove()
        element_operatorprice.closest('.d-flex').remove()
        element_sale.closest('.d-flex').remove()
        element_nights.closest('.d-flex').remove()
        element_totalamount.closest('.d-flex').remove()
        element_profit.closest('.d-flex').remove()
        //element_others.closest('.d-flex').remove()
        //element_discount.closest('.d-flex').remove()
        element_operator_amount.closest('.d-flex').remove()
        _order_detail_hotel.CheckIfOnlyRateInRoom(row_element)
    },
    GetHotelRatesCode: function (row_element, rate_id) {
        var rate_code_element = undefined
        row_element.find('.servicemanual-hotel-room-rates-code').each(function (index_3, item_3) {
            var date_range_element = $(item_3);
            if (date_range_element.attr('data-rate-id').trim() == rate_id.trim()) {
                rate_code_element = date_range_element
                return false
            }
        })
        return rate_code_element
    },
    GetHotelRatesDaysUse: function (row_element,rate_id) {
        var rate_daterange_element = undefined
        row_element.find('.servicemanual-hotel-room-rates-daterange').each(function (index_3, item_3) {
            var date_range_element = $(item_3);
            if (date_range_element.attr('data-rate-id').trim() == rate_id.trim()) {
                rate_daterange_element = date_range_element
                return false
            }
        })
        return rate_daterange_element
    },
    GetHotelRatesOperatorPrice: function (row_element, rate_id) {
        var saler_price_element = undefined
        row_element.find('.servicemanual-hotel-room-rates-operator-price').each(function (index_3, item_3) {
            var date_range_element = $(item_3);
            if (date_range_element.attr('data-rate-id').trim() == rate_id.trim()) {
                saler_price_element = date_range_element
                return false
            }
        })
        return saler_price_element
    },
    GetHotelRatesOperatorAmount: function (row_element, rate_id) {
        var saler_price_element = undefined
        row_element.find('.servicemanual-hotel-room-rates-operator-amount').each(function (index_3, item_3) {
            var date_range_element = $(item_3);
            if (date_range_element.attr('data-rate-id').trim() == rate_id.trim()) {
                saler_price_element = date_range_element
                return false
            }
        })
        return saler_price_element
    },
    GetHotelRatesSalePrice: function (row_element, rate_id) {
        var saler_price_element = undefined
        row_element.find('.servicemanual-hotel-room-rates-sale-price').each(function (index_3, item_3) {
            var date_range_element = $(item_3);
            if (date_range_element.attr('data-rate-id').trim() == rate_id.trim()) {
                saler_price_element = date_range_element
                return false
            }
        })
        return saler_price_element
    },
    GetHotelRatesAmount: function (row_element, rate_id) {
        var amount_element = undefined
        row_element.find('.servicemanual-hotel-room-rates-total-amount').each(function (index_3, item_3) {
            var date_range_element = $(item_3);
            if (date_range_element.attr('data-rate-id').trim() == rate_id.trim()) {
                amount_element = date_range_element
                return false
            }
        })
        return amount_element
    },
    GetHotelRatesProfit: function (row_element, rate_id) {
        var profit_element = undefined
        row_element.find('.servicemanual-hotel-room-rates-profit').each(function (index_3, item_3) {
            var date_range_element = $(item_3);
            if (date_range_element.attr('data-rate-id').trim() == rate_id.trim()) {
                profit_element = date_range_element
                return false
            }
        })
        return profit_element
    },
    GetHotelRatesNights: function (row_element, rate_id) {
        var nights_element = undefined
        row_element.find('.servicemanual-hotel-room-rates-nights').each(function (index_3, item_3) {
            var date_range_element = $(item_3);
            if (date_range_element.attr('data-rate-id').trim() == rate_id.trim()) {
                nights_element = date_range_element
                return false
            }
        })
        return nights_element
    },
    GetHotelRatesOtherAmount: function (row_element, rate_id) {
        var nights_element = undefined
        row_element.find('.servicemanual-hotel-room-rates-others-amount').each(function (index_3, item_3) {
            var date_range_element = $(item_3);
            if (date_range_element.attr('data-rate-id').trim() == rate_id.trim()) {
                nights_element = date_range_element
                return false
            }
        })
        return nights_element
    },
    GetHotelRatesDiscount: function (row_element, rate_id) {
        var nights_element = undefined
        row_element.find('.servicemanual-hotel-room-rates-discount').each(function (index_3, item_3) {
            var date_range_element = $(item_3);
            if (date_range_element.attr('data-rate-id').trim() == rate_id.trim()) {
                nights_element = date_range_element
                return false
            }
        })
        return nights_element
    },
    GetHotelRoomNumberOfRoom: function (row_element) {
        var room_nums_element = undefined
        room_nums_element = row_element.find('.servicemanual-hotel-room-number-of-rooms')
        return room_nums_element
    },
    CalucateRateTotalAmount: function (row_element, rate_id) {
        var sale_element = _order_detail_hotel.GetHotelRatesSalePrice(row_element, rate_id)
        var nights_element = _order_detail_hotel.GetHotelRatesNights(row_element, rate_id)
        var number_of_rooms_element = _order_detail_hotel.GetHotelRoomNumberOfRoom(row_element)
        var amount_element = _order_detail_hotel.GetHotelRatesAmount(row_element, rate_id)


        var sale_price = isNaN(parseFloat(sale_element.val().replaceAll(',', ''))) ? 0 : parseFloat(sale_element.val().replaceAll(',', ''))
        var nights_price = isNaN(parseFloat(nights_element.val().replaceAll(',', ''))) ? 0 : parseFloat(nights_element.val().replaceAll(',', ''))
        var number_of_rooms_price = isNaN(parseFloat(number_of_rooms_element.val().replaceAll(',', ''))) ? 0 : parseFloat(number_of_rooms_element.val().replaceAll(',', ''))
        var amount = sale_price * nights_price * number_of_rooms_price

        amount_element.val(_global_function.Comma(amount))
    },
    CalucateRateOperatorAmount: function (row_element, rate_id) {
        var sale_element = _order_detail_hotel.GetHotelRatesOperatorPrice(row_element, rate_id)
        var nights_element = _order_detail_hotel.GetHotelRatesNights(row_element, rate_id)
        var number_of_rooms_element = _order_detail_hotel.GetHotelRoomNumberOfRoom(row_element)
        var amount_element = _order_detail_hotel.GetHotelRatesOperatorAmount(row_element, rate_id)


        var sale_price = isNaN(parseFloat(sale_element.val().replaceAll(',', ''))) ? 0 : parseFloat(sale_element.val().replaceAll(',', ''))
        var nights_price = isNaN(parseFloat(nights_element.val().replaceAll(',', ''))) ? 0 : parseFloat(nights_element.val().replaceAll(',', ''))
        var number_of_rooms_price = isNaN(parseFloat(number_of_rooms_element.val().replaceAll(',', ''))) ? 0 : parseFloat(number_of_rooms_element.val().replaceAll(',', ''))
        var amount = sale_price * nights_price * number_of_rooms_price

        amount_element.val(_global_function.Comma(amount))
    },
    CalucateRateProfit: function (row_element, rate_id) {
        var operator_element = _order_detail_hotel.GetHotelRatesOperatorPrice(row_element, rate_id)
        var sale_element = _order_detail_hotel.GetHotelRatesSalePrice(row_element, rate_id)
        var nights_element = _order_detail_hotel.GetHotelRatesNights(row_element, rate_id)
        var number_of_rooms_element = _order_detail_hotel.GetHotelRoomNumberOfRoom(row_element)
        var profit_element = _order_detail_hotel.GetHotelRatesProfit(row_element, rate_id)

        var operator_price = isNaN(parseFloat(operator_element.val().replaceAll(',', ''))) ? 0 : parseFloat(operator_element.val().replaceAll(',', ''))
        var sale_price = isNaN(parseFloat(sale_element.val().replaceAll(',', ''))) ? 0 : parseFloat(sale_element.val().replaceAll(',', ''))
        var nights_price = isNaN(parseFloat(nights_element.val().replaceAll(',', ''))) ? 0 : parseFloat(nights_element.val().replaceAll(',', ''))
        var number_of_rooms_price = isNaN(parseFloat(number_of_rooms_element.val().replaceAll(',', ''))) ? 0 : parseFloat(number_of_rooms_element.val().replaceAll(',', ''))

        var amount = (sale_price - operator_price) * nights_price * number_of_rooms_price
       // if (amount < 0) amount = 0
        profit_element.val((amount >=0?'':'-')+ _global_function.Comma(amount))
    },
    CalucateTotalServiceAmount: function () {
        var package_text = $('#servicemanual-hotel-room-total-amount-final').html()
        var extra_text = $('#servicemanual-hotel-extrapackage-total-amount-final').html()

        var amount = 0
        var room_amount = package_text == undefined || isNaN(parseFloat(package_text.replaceAll(',', ''))) ? 0 : parseFloat(package_text.replaceAll(',', ''))
        var extra_amount = extra_text == undefined || isNaN(parseFloat(extra_text.replaceAll(',', ''))) ? 0 : parseFloat(extra_text.replaceAll(',', ''))
        amount = room_amount + extra_amount
        $('.service-manual-hotel-total-service-amount').html(_global_function.Comma(amount))
    },
    CalucateTotalServiceProfit: function () {
        var package_text = $('#servicemanual-hotel-extrapackage-total-profit-final').html()
        var extra_text = $('#servicemanual-hotel-room-total-profit-final').html()
        var amount = 0
        var room_amount = package_text == undefined || isNaN(parseFloat(package_text.replaceAll(',', ''))) ? 0 : parseFloat(package_text.replaceAll(',', ''))
        var extra_amount = extra_text == undefined || isNaN(parseFloat(extra_text.replaceAll(',', ''))) ? 0 : parseFloat(extra_text.replaceAll(',', ''))
        amount = room_amount + extra_amount

        var discount_element = $('#servicemanual-hotel-discount')
        var other_amount_element = $('#servicemanual-hotel-other-amount')
        var discount = isNaN(parseFloat(discount_element.val().replaceAll(',', ''))) ? 0 : parseFloat(discount_element.val().replaceAll(',', ''))
        var other_amount = isNaN(parseFloat(other_amount_element.val().replaceAll(',', ''))) ? 0 : parseFloat(other_amount_element.val().replaceAll(',', ''))
        amount = amount - discount - other_amount

        $('.service-manual-hotel-total-service-profit').html((amount >= 0 ? '' : '-')+ _global_function.Comma(amount))
    },
    OnApplyDayUsesToRoomNight: function (element) {
        var nights = 0
        var element_days = _order_detail_hotel.GetHotelRatesNights(element.closest('.servicemanual-hotel-room-tr'), element.attr('data-rate-id'))
        try {
            var start_date_date = _order_detail_common.GetDateFromNoUpdateDateRangeElementDateRange(element, false)
            var end_date_date = _order_detail_common.GetDateFromNoUpdateDateRangeElementDateRange(element, true)
            var start_date = _global_function.GetDayText(start_date_date).split(' ')[0];
            var end_date = _global_function.GetDayText(end_date_date).split(' ')[0];
            element.val(start_date + ' - ' + end_date).change()
            var diff = Math.floor((end_date_date - start_date_date) / 86400000);
            nights = _global_function.Comma(diff)
        } catch { }
        element_days.val(nights).change()

    },
    OnchangeSearchRoom: function () {
        let adult = 0;
        let baby = 0;
        let infant = 0;

        $('#block_room_search_content .row').each(function () {
            let seft = $(this);
            adult = parseInt(seft.find('.adult .qty_input').val()) <= 0 || isNaN(parseInt(seft.find('.adult .qty_input').val())) ? 0 : parseInt(seft.find('.adult .qty_input').val());
            baby = parseInt(seft.find('.baby .qty_input').val()) <= 0 || isNaN(parseInt(seft.find('.baby .qty_input').val()))? 0 : parseInt(seft.find('.baby .qty_input').val());
            infant = parseInt(seft.find('.infant .qty_input').val()) <= 0 || isNaN(parseInt(seft.find('.infant .qty_input').val()))? 0 : parseInt(seft.find('.infant .qty_input').val());
        });

        $('#servicemanual-hotel-numberOfPeople').text(adult + baby + infant);

    },
    CloneHotelRoom: function (element) {
        var room_max_value = $('#servicemanual-hotel-numberOfRooms').val();
        if (room_max_value == undefined || parseInt(room_max_value) == undefined || parseInt(room_max_value) <= 0) {
            _msgalert.error('Vui nhập vào số phòng tại thông tin khách sạn');
            return;
        }
        var room_exists = $('#servicemanual-hotel-rooms').find('.servicemanual-hotel-room-tr').length;
        if (room_exists >= parseInt(room_max_value)) {
            _msgalert.error('Số phòng tại mục [Bảng kê dịch vụ phòng] không được vượt quá số phòng đã nhập bên trên');
            return;
        }
        var last_order = _order_detail_hotel.GetLastestHotelRoomOrder();
        last_order++;
        //-- Create and Append Div:
        var new_div_id = 'hotel-room-new-room';
        var element_clone = element.closest('.servicemanual-hotel-room-tr').clone().prop('id', new_div_id);
        $('.servicemanual-hotel-room-total-summary').before(element_clone)
        //-- Get new div
        var new_element = $('#' + new_div_id);

        //-- Change Order Text:
        new_element.attr('data-room-id', '0');
        new_element.attr('data-room-type-id', '');
        new_element.attr('data-room-type-code', '');
        new_element.find('.servicemanual-hotel-room-td-order').html('' + last_order);
        //-- Change Package & daterange-indentifer:
        new_element.find('.servicemanual-hotel-room-rates-code').attr('data-rate-id', '0');
        new_element.find('.servicemanual-hotel-room-rates-daterange').attr('data-rate-id', '0');
        new_element.find('.servicemanual-hotel-room-rates-operator-price').attr('data-rate-id', '0');
        new_element.find('.servicemanual-hotel-room-rates-sale-price').attr('data-rate-id', '0');
        new_element.find('.servicemanual-hotel-room-rates-nights').attr('data-rate-id', '0');
        new_element.find('.servicemanual-hotel-room-rates-total-amount').attr('data-rate-id', '0');
        new_element.find('.servicemanual-hotel-room-rates-profit').attr('data-rate-id', '0');
        new_element.attr('id', '')
        _SetService_Detail.ReIndexRoomOrder()
    },
    HotelSuggesstion: function (txt_search) {
        $("#servicemanual-hotel-name").select2({
            theme: 'bootstrap4',
            placeholder: "Tên khách sạn",
            minimumInputLength: 1,
            ajax: {
                url: "/Order/HotelSuggestion",
                type: "post",
                dataType: 'json',
                delay: 250,
                tags: true,
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
                                id: item.hotelid,
                            }
                        })
                    };
                },
                cache: true
            }
        });
    },
    HotelServiceRoomPopup: function (hotel_booking_id) {
        $.ajax({
            url: "/Order/AddHotelServiceRoom",
            type: "post",
            data: { hotel_booking_id: hotel_booking_id },
            success: function (result) {
                if (result != undefined) {
                    $('#servicemanual_hotel_roomdiv').html(result);
                }
                else {
                    $('#servicemanual_hotel_roomdiv').html('');
                }
                _order_detail_hotel.OnAddNewRoomRates();
                _order_detail_hotel.HotelServiceRoomGuestPopup(hotel_booking_id);
                $('.servicemanual-hotel-room-rates-daterange').each(function (index,item) {
                    var element = $(item);
                    _order_detail_hotel.OnApplyDayUsesToRoomNight(element)
                });
                $('.servicemanual-hotel-room-tr').each(function (index, item) {
                    var element = $(item);
                    _order_detail_hotel.CheckIfOnlyRateInRoom(element)
                });
                _order_detail_hotel.DynamicBindInput();
                _order_detail_hotel.CalucateTotalServiceAmount()
                _order_detail_hotel.CalucateTotalServiceProfit()
            }
        });
    },
    HotelServiceRoomPackagesPopup: function (hotel_booking_id) {
        $.ajax({
            url: "/Order/AddHotelServiceRoomPackages",
            type: "post",
            data: { hotel_booking_id: hotel_booking_id },
            success: function (result) {
                if (result != undefined) {
                    $('#servicemanual_hotel_roompackagesdiv').html(result);
                }
                else {
                    $('#servicemanual_hotel_roompackagesdiv').html('');
                }
                _order_detail_hotel.OnAddNewRoomExtraPackages();
                _order_detail_hotel.CalucateTotalServiceAmount()
                _order_detail_hotel.CalucateTotalServiceProfit()
            }
        });
    },
    HotelServiceRoomGuestPopup: function (hotel_booking_id) {
        $.ajax({
            url: "/Order/AddHotelServiceGuest",
            type: "post",
            data: { hotel_booking_id: hotel_booking_id },
            success: function (result) {
                if (result != undefined) {
                    $('#servicemanual_hotel_guestdiv').html(result);
                    _order_detail_hotel.FillHotelGuestSelectRoomOption();
                    _order_detail_hotel.OnAddNewGuest()
                }
                else {
                    $('#servicemanual_hotel_guestdiv').html('');
                }

            }
        });

    },
    CalucateTotalAmountOfHotelRoom: function () {
        var total = 0;
        $('.servicemanual-hotel-room-rates-total-amount').each(function (index, item) {
            var element = $(item);
            if (element.val() != undefined && element.val().trim() != '') {
                total += parseFloat(element.val().replaceAll(',', ''))
            }

        });
        $('#servicemanual-hotel-room-total-amount-final').html('' + _global_function.Comma(total)).change();
        _order_detail_hotel.CalucateTotalServiceAmount()
    },
    CalucateTotalProfitOfHotelRoom: function () {
        var total = 0;
        $('.servicemanual-hotel-room-rates-profit').each(function (index, item) {
            var element = $(item);
            if (element.val() != undefined && element.val().trim() != '') {
                total += parseFloat(element.val().replaceAll(',', ''))
            }

        });
        $('#servicemanual-hotel-room-total-profit-final').html('' + _global_function.Comma(total)).change();
        _order_detail_hotel.CalucateTotalServiceProfit()
    },
    CalucateTotalOperatorAmountOfHotelRoom: function () {
        var total = 0;
        $('.servicemanual-hotel-room-rates-operator-amount').each(function (index, item) {
            var element = $(item);
            if (element.val() != undefined && element.val().trim() != '') {
                total += parseFloat(element.val().replaceAll(',', ''))
            }

        });
        $('#servicemanual-hotel-room-operator-amount-final').html('' + _global_function.Comma(total)).change();
        _order_detail_hotel.CalucateTotalServiceProfit()
    },
    CalucateTotalDiscountOfHotelRoom: function () {
        var total = 0;
        $('.servicemanual-hotel-room-rates-discount').each(function (index, item) {
            var element = $(item);
            if (element.val() != undefined && element.val().trim() != '') {
                total += parseFloat(element.val().replaceAll(',', ''))
            }

        });
        $('#servicemanual-hotel-room-total-discount-final').html('' + _global_function.Comma(total)).change();
        _order_detail_hotel.CalucateTotalServiceProfit()
    },
    CalucateTotalOthersAmountOfHotelRoom: function () {
        var total = 0;
        $('.servicemanual-hotel-room-rates-others-amount').each(function (index, item) {
            var element = $(item);
            if (element.val() != undefined && element.val().trim() != '') {
                total += parseFloat(element.val().replaceAll(',', ''))
            }

        });
        $('#servicemanual-hotel-room-total-others-amount-final').html('' + _global_function.Comma(total)).change();
        _order_detail_hotel.CalucateTotalServiceProfit()
    },
    AddHotelRoomExtraPackage: function () {
        var lastest_order = _order_detail_hotel.GetLastestHotelExtraPackagesOrder();
        lastest_order = lastest_order + 1;
        var min_range = _order_detail_common.GetDateFromNoUpdateDateRangeElement($('#servicemanual_hotel_checkin'))
        var max_range = _order_detail_common.GetDateFromNoUpdateDateRangeElement($('#servicemanual_hotel_checkout'))
        var min_time = _global_function.GetDayTextDateRangePicker(min_range);
        var max_time = _global_function.GetDayTextDateRangePicker(max_range);
        var html = _order_detail_html.html_service_hotel_extrapackage_newextra_package.replaceAll('{{order}}', lastest_order).replaceAll('{date_range}', min_time + ' - ' + max_time)
        $(".servicemanual-hotelpackage-total-summary").before(html);

    },
    GetLastestHotelExtraPackagesOrder: function () {
        var last_order = 0;
        if (!$('.servicemanual-hotel-extrapackage-td-order')[0]) return last_order;
        //-- Get Lastest Order
        $('.servicemanual-hotel-extrapackage-td-order').each(function (index, item) {
            last_order++;
        });
        return last_order;
    },
    OnAddNewRoomRates: function () {
        $('.servicemanual-hotel-room-rates-daterange').each(function (index, item) {
            var element = $(this)
            _order_detail_common.OnApplyPackageDateDateRange(element, $('#servicemanual_hotel_checkin'), $('#servicemanual_hotel_checkout'));
            //_order_detail_hotel.OnApplyDayUsesToRoomNight(element)

        });

    },
    DeleteHotelRoomExtrapackage: function (element) {
        element.closest('.servicemanual-hotel-extrapackage-tr').remove();
    },
    AddHotelRoomGuest: function () {
        if ($('.servicemanual-hotel-roomguest-name').length >= parseInt($('#servicemanual-hotel-numberOfPeople').text())) {
            _msgalert.error('Số lượng thành viên trong đoàn đã đạt mức tối đa. Không thể thêm thành viên mới');
            return;
        }

        var lastest_order = _order_detail_hotel.GetLastestOrderHotelRoomGuestTable();
        lastest_order = lastest_order + 1;
        var html = _order_detail_html.html_new_room_guest.replaceAll('{order}', lastest_order).replaceAll('{room_guest_name}', '').replaceAll('{room_guest_birthday}', '').replaceAll('{room_guest_note}', '')
        $("#servicemanual-hotelguest-total-summary").before(html);
        var row_new;
        $(".servicemanual-hotel-roomguest-order").each(function (index, item) {
            var element = $(this);
            if (parseInt(element.text()) == lastest_order) {
                row_new = element.closest('.servicemanual-hotel-roomguest-row');
                return false;
            }
        });
        if (row_new != undefined) {
            _order_detail_hotel.FillHotelGuestSelectRoomOption()

        }
        _order_detail_hotel.OnAddNewGuest()
       

    },
    DeleteHotelGuest: function (element) {
        element.closest('.servicemanual-hotel-roomguest-row').remove();
        var count = 0;
        $('.servicemanual-hotel-roomguest-order').each(function (index, item) {
            count = count + 1;
            $(item).html(count)
        });
    },
    GetLastestHotelRoomOrder: function () {
        var last_order = 0;
        if (!$('.servicemanual-hotel-room-td-order')[0]) return last_order;
        //-- Get Lastest Order
        $('.servicemanual-hotel-room-td-order').each(function (index, item) {
            last_order++;
        });
        return last_order;
    },
    GetLastestPackageCodeHotelRoomTable: function () {
        var last_package_code = 1;
        if (!$('.servicemanual-hotel-room-roompackage-code')[0]) return 0;

        $('.servicemanual-hotel-room-roompackage-code').each(function (index, item) {
            var text_id = $(item).attr('id').split('-');
            var current_id = parseInt(text_id[text_id.length - 1]);
            if (last_package_code < current_id) {
                last_package_code = current_id
            }

        });

        return last_package_code;
    },
    GetLastestOrderHotelRoomGuestTable: function () {
        var last_order = 1;
        if (!$('.servicemanual-hotel-roomguest-order')[0]) return 0;
        //-- Get Lastest Order
        $('.servicemanual-hotel-roomguest-order').each(function (index, item) {
            if (parseInt($(item).text()) != undefined && last_order < parseInt($(item).text())) {
                last_order = parseInt($(item).html())
            }
        });
        return last_order;
    },
    OnChangeNumberOfRoomToGuestSelect: function () {

        _order_detail_hotel.FillHotelGuestSelectRoomOption();
        $('.servicemanual-hotel-roomguest-roomselect-new').select2();
        $('.servicemanual-hotel-roomguest-roomselect-new').removeClass('servicemanual-hotel-roomguest-roomselect-new');
    },
    OnAddNewGuest: function () {
        _order_detail_common.SingleDatePickerBirthDay($('.servicemanual-hotel-roomguest-birthday'), 'up')
        $('.servicemanual-hotel-roomguest-type-new').each(function (index, item) {
            var ele = $(this)
            _order_detail_common.Select2WithFixedOptionAndNoSearch(ele)
            ele.removeClass('servicemanual-hotel-roomguest-type-new')
        });

    },
    FillHotelGuestSelectRoomOption: function () {
        var html_option = '';
        $('.servicemanual-hotel-room-tr').each(function (index, item) {
            var element = $(this);
            html_option = html_option + _order_detail_html.html_select_guest_to_room_option.replaceAll('{if_selected}', '').replace('{room_id}', element.find('.servicemanual-hotel-room-td-order').text()).replace('{room_order}', element.find('.servicemanual-hotel-room-td-order').text() + ' - ' + element.find('.servicemanual-hotel-room-type-name').val())
        });
        $('.servicemanual-hotel-roomguest-roomselect').each(function (index, item) {

            var value = $(item).find(':selected').val();
            $(item).html(html_option)
            _order_detail_common.Select2WithFixedOptionAndNoSearch($(item))
            $(item).val(value).trigger('change');

        });
    },
    ReIndexRoomOrder: function () {
        var index = 0
        $('.servicemanual-hotel-room-td-order').each(function (index, item) {
            var element = $(item);
            element.html(_global_function.Comma(++index))
        });
    },
    ReIndexExtraPackageOrder: function () {
        var index = 0
        $('.servicemanual-hotel-extrapackage-td-order').each(function (index, item) {
            var element = $(item);
            element.html(_global_function.Comma(++index))
        });
    },
    GetLastestExtraPackageOrder: function () {
        var last_order = 0;
        if (!$('.servicemanual-hotel-extrapackage-td-order')[0]) return last_order;
        //-- Get Lastest Order
        $('.servicemanual-hotel-extrapackage-td-order').each(function (index, item) {
            last_order++;
        });
        return last_order;
    },
    ReIndexExtraPackageOrder: function () {
        var index = 0
        $('.servicemanual-hotel-extrapackage-td-order').each(function (index, item) {
            var element = $(item);
            element.html(_global_function.Comma(++index))
        });
    },
    CalucateTotalAmountOfExtraPackage: function () {
        var total = 0;
        $('.servicemanual-hotel-extrapackage-total-amount').each(function (index, item) {
            var element = $(item);
            if (element.val() != undefined && element.val().trim() != '') {
                total += parseFloat(element.val().replaceAll(',', ''))
            }

        });
        $('#servicemanual-hotel-extrapackage-total-amount-final').html((total >= 0 ? '' : '-') + _global_function.Comma(total)).change();
        _order_detail_hotel.CalucateTotalServiceAmount()
    },
    CalucateTotalProfitOfExtraPacakge: function () {
        var total = 0;
        $('.servicemanual-hotel-extrapackage-profit').each(function (index, item) {
            var element = $(item);
            if (element.val() != undefined && element.val().trim() != '') {
                total += parseFloat(element.val().replaceAll(',', ''))
            }

        });
        $('#servicemanual-hotel-extrapackage-total-profit-final').html((total >= 0 ? '' : '-') + _global_function.Comma(total)).change();
        _order_detail_hotel.CalucateTotalServiceProfit()
    },
    CalucateTotalOperatorAmountOfExtraPackage: function () {
        var total = 0;
        $('.servicemanual-hotel-extrapackage-operator-amount').each(function (index, item) {
            var element = $(item);
            if (element.val() != undefined && element.val().trim() != '') {
                total += parseFloat(element.val().replaceAll(',', ''))
            }

        });
        $('#servicemanual-hotel-extrapackage-operator-amount-final').html((total >= 0 ? '' : '-') + _global_function.Comma(total)).change();
        _order_detail_hotel.CalucateTotalServiceProfit()
    },
    CalucateExtraPackageTotalAmount: function (row_element) {
        var sale_element = row_element.find('.servicemanual-hotel-extrapackage-sale-price')
        var nights_element = row_element.find('.servicemanual-hotel-extrapackage-nights')
        var quanity_element = row_element.find('.servicemanual-hotel-extrapackage-number-of-extrapackages')
        var amount_element = row_element.find('.servicemanual-hotel-extrapackage-total-amount')

        var sale_price = sale_element.val() == undefined || isNaN(parseFloat(sale_element.val().replaceAll(',', ''))) ? 0 : parseFloat(sale_element.val().replaceAll(',', ''))
        var nights_price = nights_element.val() == undefined || isNaN(parseFloat(nights_element.val().replaceAll(',', ''))) ? 0 : parseFloat(nights_element.val().replaceAll(',', ''))
        var quanity = quanity_element.val() == undefined || isNaN(parseFloat(quanity_element.val().replaceAll(',', ''))) ? 0 : parseFloat(quanity_element.val().replaceAll(',', ''))

        var amount = sale_price * nights_price * quanity
        amount_element.val(_global_function.Comma(amount))
    },
    CalucateExtraPackageOperatorAmount: function (row_element) {
        var operator_element = row_element.find('.servicemanual-hotel-extrapackage-operator-price')
        var nights_element = row_element.find('.servicemanual-hotel-extrapackage-nights')
        var quanity_element = row_element.find('.servicemanual-hotel-extrapackage-number-of-extrapackages')
        var element = row_element.find('.servicemanual-hotel-extrapackage-operator-amount')

        var operator_price = operator_element.val() == undefined || isNaN(parseFloat(operator_element.val().replaceAll(',', ''))) ? 0 : parseFloat(operator_element.val().replaceAll(',', ''))
        var nights_price = nights_element.val() == undefined || isNaN(parseFloat(nights_element.val().replaceAll(',', ''))) ? 0 : parseFloat(nights_element.val().replaceAll(',', ''))
        var quanity = quanity_element.val() == undefined || isNaN(parseFloat(quanity_element.val().replaceAll(',', ''))) ? 0 : parseFloat(quanity_element.val().replaceAll(',', ''))

        var amount =  operator_price * nights_price * quanity
        element.val((amount >= 0 ? '' : '-') + _global_function.Comma(amount))
    },
    CalucateExtraPackageProfit: function (row_element) {
        var operator_element = row_element.find('.servicemanual-hotel-extrapackage-operator-price')
        var sale_element = row_element.find('.servicemanual-hotel-extrapackage-sale-price')
        var nights_element = row_element.find('.servicemanual-hotel-extrapackage-nights')
        var quanity_element = row_element.find('.servicemanual-hotel-extrapackage-number-of-extrapackages')
        var profit_element = row_element.find('.servicemanual-hotel-extrapackage-profit')

        var operator_price = operator_element.val() == undefined || isNaN(parseFloat(operator_element.val().replaceAll(',', ''))) ? 0 : parseFloat(operator_element.val().replaceAll(',', ''))
        var sale_price = sale_element.val() == undefined || isNaN(parseFloat(sale_element.val().replaceAll(',', ''))) ? 0 : parseFloat(sale_element.val().replaceAll(',', ''))
        var nights_price = nights_element.val() == undefined || isNaN(parseFloat(nights_element.val().replaceAll(',', ''))) ? 0 : parseFloat(nights_element.val().replaceAll(',', ''))
        var quanity = quanity_element.val() == undefined || isNaN(parseFloat(quanity_element.val().replaceAll(',', ''))) ? 0 : parseFloat(quanity_element.val().replaceAll(',', ''))

        var amount = (sale_price - operator_price) * nights_price * quanity
        profit_element.val((amount >= 0 ? '' : '-') + _global_function.Comma(amount))
    },
    AddNewRoomExtraPackage: function (element) {
        var lastest_order = _order_detail_hotel.GetLastestExtraPackageOrder()
        var html_append = _order_detail_html.html_service_hotel_extrapackage_newextra_package.replaceAll('{{order}}', ('' + (lastest_order+1)))
        $('.servicemanual-hotelpackage-total-summary').before(html_append)
        _order_detail_hotel.ReIndexExtraPackageOrder()
    },
    DeleteRoomExtraPackage: function (element) {
        element.closest('.servicemanual-hotel-extrapackage-tr').remove()
    },
    OnApplyDayUsesToExtraPackageDays: function (element) {
        var start_date_date = _order_detail_common.GetDateFromNoUpdateDateRangeElementDateRange(element, false)
        var end_date_date = _order_detail_common.GetDateFromNoUpdateDateRangeElementDateRange(element, true)
        var start_date = _global_function.GetDayText(start_date_date).split(' ')[0];
        var end_date = _global_function.GetDayText(end_date_date).split(' ')[0];
        element.val(start_date + ' - ' + end_date).change()
        var diff = Math.floor((end_date_date - start_date_date) / 86400000);
        var element_days = element.closest('.servicemanual-hotel-extrapackage-tr').find('.servicemanual-hotel-extrapackage-nights')
        var nights = _global_function.Comma(diff)
        try {
            element_days.val(nights).change()
        } catch { }
    },
    OnAddNewRoomExtraPackages: function () {
        
        $('.servicemanual-hotel-extrapackage-daterange').each(function (index, item) {
            var element = $(this)
            _order_detail_common.OnApplyPackageDateDateRange(element, $('#servicemanual_hotel_checkin'), $('#servicemanual_hotel_checkout'));
        });

    },
   
}


