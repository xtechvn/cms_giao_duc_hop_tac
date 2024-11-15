$(document).ready(function () {
    var id = $('#form_add_hotel_service').attr('data-booking-id')
    _request_hotel_detail.Initialization(parseFloat(id));
  
})
var _request_hotel_detail = {
    ServiceType: 1,
    Initialization: function (hotel_booking_id) {

        _order_detail_common.UserSuggesstion($('.main-hotel-staff-rq-detail'));
        _order_detail_hotel.HotelServiceRoomPopup(hotel_booking_id);
        _order_detail_hotel.HotelServiceRoomPackagesPopup(hotel_booking_id);

        _order_detail_common.SingleDatePicker($('.servicemanual_hotel_checkin'))
        _order_detail_common.SingleDatePicker($('.servicemanual_hotel_checkout'))


        _order_detail_hotel.HotelSuggesstion();
        $("#servicemanual-hotel-contactclient-country").select2()

    },
    Summit: function () {
        var validate_failed = false
        //---- Null input
        $('.servicemanual-hotel-roomguest-name').each(function (index, item) {
            var element = $(item);
            if (element.val() == undefined || element.val().trim() == '') {
                error = true;
                _msgalert.error('Vui lòng nhập tên cho thành viên thứ' + element.closest('.servicemanual-hotel-roomguest-row').find('.servicemanual-hotel-roomguest-order').text());
                validate_failed = true
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
        if ($('#servicemanual_hotel_checkin').data('daterangepicker').startDate._d == undefined) {
            _msgalert.error('Vui lòng nhập ngày Check-In');
            return;
        }
        if ($('#servicemanual_hotel_checkout').data('daterangepicker').startDate._d == undefined) {
            _msgalert.error('Vui lòng nhập ngày Check-Out');
            return;
        }
        /*
        if (_order_detail_common.GetDateFromNoUpdateDateRangeElement($('#servicemanual_hotel_checkout')) <= _order_detail_common.GetDateFromNoUpdateDateRangeElement($('#servicemanual_hotel_checkin'))) {
            _msgalert.error('Ngày check-out không được nhỏ hơn ngày check-in');
            return;
        }*/
        if ($('#main-hotel-staff').find(':selected').val() == undefined || parseInt($('#main-hotel-staff').find(':selected').val()) <= 0) {
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
                    else if (operator_price_element.val() == undefined || operator_price_element.val().trim() == '') {
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
        var discount = discount_element.val() != undefined ? discount_element.val().replaceAll(',', '') : '0'
        var other_amount = other_amount_element.val() != undefined ? other_amount_element.val().replaceAll(',', '') : '0'

        var object_summit = {
            order_id: $('#OrderId').val(),
            hotel: {
                hotel_id: $('#servicemanual-hotel-name').find(':selected').val(),
                hotel_name: $('#servicemanual-hotel-name').find(':selected').text().split("-")[0],
                number_of_rooms: $('#servicemanual-hotel-numberOfRooms').val(),
                arrive_date: _global_function.GetDayText(arrive_date, true),
                departure_date: _global_function.GetDayText(departure_date, true),
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
                number_of_rooms: element.find('.servicemanual-hotel-room-number-of-rooms').val().replaceAll(',', ''),
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
                        if (!compare_date) {
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
                    birthday: element.find('.servicemanual-hotel-roomguest-birthday').val() == undefined || element.find('.servicemanual-hotel-roomguest-birthday').val().trim() == '' ? null : _global_function.GetDayText(element.find('.servicemanual-hotel-roomguest-birthday').data('daterangepicker').startDate._d, true),
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
                    url: "/RequestHotelBooking/SummitHotelServiceData",
                    type: "post",
                    data: { data: object_summit },
                    success: function (result) {
                        if (result != undefined && result.status == 0) {
                            _msgalert.success(result.msg);
               
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
                url: "/RequestHotelBooking/SummitHotelServiceData",
                type: "post",
                data: { data: object_summit },
                success: function (result) {
                    if (result != undefined && result.status == 0) {
                        _msgalert.success(result.msg);
             
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
    UpdateStatus: function (type, id) {
        var title = "Xác nhận";
        var description = "Xác nhận nhận xử lý đơn này!";
        if (type == 2) {
            description = "Xác nhận đã xử lý xong đơn này!";
        }
        if (type == 4) {
            description = "Xác nhận hủy đơn này!";
        }
        _msgconfirm.openDialog(title, description, function () {
            $.ajax({
                url: "/RequestHotelBooking/UpdateStatus",
                type: "post",
                data: { type: type, id: id },
                success: function (result) {
                    if (result != undefined && result.status == 0) {
                        _msgalert.success(result.msg);
                        setTimeout(function () {
                            window.location.reload();
                        }, 1000);

                    }
                    else {
                        _msgalert.error(result.msg);

                    }

                }
            });
        })
     
    },
}