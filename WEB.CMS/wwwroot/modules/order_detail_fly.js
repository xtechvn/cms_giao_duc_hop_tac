var _order_detail_fly = {
    ServiceType:3,
    Initialization: function (order_id, group_fly) {
        $('#flybooking-service').addClass('show')
        _order_detail_common.Select2WithFixedOptionAndNoSearch($('#add-service-fly-select-route'))
        if (group_fly != undefined && group_fly.trim() != '' && group_fly != null) $('.btn-summit-service-fly').text('Sửa')
        var route_count = $('#add-service-fly-select-route').val();
        var select_route = parseInt(route_count) == undefined ? 1 : parseInt(route_count)
        _order_detail_fly.FlyBookingServiceRoutePopup(order_id, group_fly, 0);
       
        if (select_route == 2) {
            _order_detail_fly.FlyBookingServiceRoutePopup(order_id, group_fly, 1);
        }
        _order_detail_fly.DynamicBindInput(order_id, group_fly);
        _order_detail_common.SingleDateTimePicker($('.service-fly-route-from-date'))
        _order_detail_common.SingleDateTimePicker($('.service-fly-route-to-date'))

        _order_detail_common.UserSuggesstion($('#add-service-fly-main-staff'));
        _order_detail_fly.FlyBookingServicePassengerPopup(order_id, group_fly);
        _order_detail_fly.FlyBookingServiceExtraPackagesPopup('service-fly-form', order_id, group_fly);
        _order_detail_fly.AirPortCodeSuggestion();
        _order_detail_common.FileAttachment(group_fly.split(',')[0], _order_detail_fly.ServiceType)
       
    },
    DynamicBindInput: function (order_id, group_fly) {
        $('#add-service-fly-select-route').on('change', function () {
            var data = $("#add-service-fly-select-route option:selected").val()
            switch (data) {
                case '1': {
                    $('#add-service-fly-route-back').html('')
                    $('#add-service-fly-route-back').hide()
                    $('.service-fly-route-to-date').prop('disabled', true);
                    $('.service-fly-route-to-date').css('background-color', 'lightgray');
                   

                } break;
                case '2': {
                    _order_detail_fly.FlyBookingServiceRoutePopup(order_id, group_fly, 1);
                    $('.service-fly-route-to-date').prop('disabled', false);
                    $('.service-fly-route-to-date').css('background-color', '');
                } break;
            }
            
        })
        $("body").on('change','#add-service-fly-route-go .service-fly-select-route-from', function () {
            var data = $('#add-service-fly-route-go .service-fly-select-route-from option:selected').val()
            $('#add-service-fly-route-back .service-fly-select-route-to').val(data).trigger('change');
        })
        $("body").on('change', '#add-service-fly-route-go .service-fly-select-route-to', function () {
            var data = $('#add-service-fly-route-go .service-fly-select-route-to option:selected').val()
            $('#add-service-fly-route-back .service-fly-select-route-from').val(data).trigger('change');
        })
      
       
        $("body").on('keyup', ".service-fly-extrapackage-saleprice, .service-fly-extrapackage-quantity, .service-fly-extrapackage-baseprice", function () {
            var element = $(this)
            var table_element = $(this).closest('.service-fly-extrapackage-tbody')
            _order_detail_fly.CalucateRowAmountandProfit(element);

            _order_detail_fly.CalucateTotalExtraPackages(table_element);
            _order_detail_fly.CalucateTotalAmountOfExtraPackages(table_element);
            _order_detail_fly.CalucateTotalProfit(table_element);


        });
        $("body").on('keyup', " #servicemanual-fly-other-amount, #servicemanual-fly-commission", function () {
            _order_detail_fly.CalucateTotalServiceProfit()
        });
      
        $("body").on('apply.daterangepicker', ".service-fly-route-from-date", function (ev, picker) {
            _order_detail_common.OnApplyStartDateOfBookingRangeDatetime($(this), $('.service-fly-route-to-date'))
        });
        $("body").on('keyup', ".service-fly-passenger-birthday", function (ev, picker) {
            var element = $(this)
            var value = element.val()
            element.val(value)
        });
        $('.service-fly-note').keydown(function (e) {
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
                formData.append("service_type", "3");
               
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
                                var name = _global_function.RemoveUnicode(item.name)
                                var note = _global_function.RemoveUnicode(item.note)
                                if (item.name == null  || _global_function.CheckIfSpecialCharracter(name) || !_global_function.CheckIfStringIsDate(item.birthday)) {
                                    _msgalert.error('Import danh sách thất bại, dữ liệu trong tệp tin không chính xác, vui lòng kiểm tra lại / liên hệ IT')
                                    incorrect_data = true;
                                    return false;
                                }
                            });
                            if (incorrect_data) {
                                $('.img_loading_upload_file').hide()
                                return
                            }
                            $('.service-fly-passenger-row').remove()
                            $(result.data).each(function (index, item) {
                                var row_element = $('.service-fly-passenger-summary-row')
                                var new_position = _order_detail_fly.GetLastestPassengerNo(row_element) + 1;
                                row_element.before(_order_detail_html.html_servicefly_passenger_newpassenger_tr.replaceAll('@p.Id', '0').replaceAll('@(++index)', '' + new_position).replaceAll('@(p.Name)', item.name == null || item.name == undefined ? '' : item.name).replaceAll('@(p.Birthday!=null?((DateTime)p.Birthday).ToString("dd/MM/yyyy"): DateTime.Now.ToString("dd/MM/yyyy"))', item.birthday == null ? '' : item.birthday).replaceAll('@p.Note', item.note == null ? '' : _global_function.RemoveSpecialCharacter(item.note)).replaceAll('{genre_male_if_selected}', item.gender.toLowerCase().includes('nam') ? 'selected=\"selected\"' : '').replaceAll('{genre_female_if_selected}', item.gender.toLowerCase().includes('nữ') || item.gender.toLowerCase().includes('nu') ? 'selected=\"selected\"' : ''))
                                $('.service-fly-passenger-genre-new').select2()
                                $('.service-fly-passenger-genre-new').removeClass('service-fly-passenger-genre-new')
                                _order_detail_common.SingleDatePickerBirthDay($('.service-fly-passenger-birthday'), 'up')
                                $('.service-fly-passenger-birthday-new').removeClass('service-fly-passenger-birthday-new')
                            });
                            _msgalert.success(result.msg)

                        }
                        else {
                           
                            _msgalert.error(result.msg)

                        }
                        $('.img_loading_upload_file').hide()
                    }
                });
                $('.upload-file-guest-btn').removeAttr('disabled')

            }
            element.val('');

        });
        
    },
    AirlineCodeSuggesstion: function (element_id) {
        var element = $('#' + element_id + ' .service-fly-route-airline')
        var value = element.find(":selected").val()
        element.select2();

        $.ajax({
            url: "AirlinesSuggestion",
            type: "post",
            data: { txt_search: "" },
            success: function (result) {
                if (result != undefined && result.data != undefined && result.data.length > 0) {
                    result.data.forEach(function (item) {
                        element.append(_order_detail_html.html_airline_option.replaceAll('{airline_code}', item.code).replace('{airline_name}', item.nameVi).replace('{if_selected}', ''))

                    });
                    element.trigger('change');
                }
                else {
                    element.trigger('change');

                }
                element.val(value).trigger('change');

            }
        });
    },
   
    AirPortCodeSuggestion: function () {
        var element_from = $('.service-fly-select-route-from')
        var element_to = $('.service-fly-select-route-to')
        $('.service-fly-select-route-from').select2();
        $('.service-fly-select-route-to').select2();

        $.ajax({
            url: "AirPortCodeSuggestion",
            type: "post",
            data: { txt_search: "" },
            success: function (result) {
                

                if (result != undefined && result.data != undefined && result.data.length > 0) {
                    result.data.forEach(function (item) {
                        element_from.append(_order_detail_html.html_airport_code_option.replaceAll('{airport_code}', item.code).replaceAll('{airport_name}', item.districtVi).replaceAll('{if_selected}', ''))
                        element_to.append(_order_detail_html.html_airport_code_option.replaceAll('{airport_code}', item.code).replaceAll('{airport_name}', item.districtVi).replaceAll('{if_selected}', ''))

                    });
                   
                    element_from.trigger('change');
                    element_to.trigger('change');
                }
                else {
                    element_from.trigger('change');
                    element_to.trigger('change');
                }

            }
        });
    },
   
    FlyBookingServiceExtraPackagesPopup: function ( element_id,order_id, group_fly) {
        var element = $('#' + element_id)
        var div = element.find('.service-fly-route-extrapackage')
        $.ajax({
            url: "AddFlyBookingServiceExtraPackages",
            type: "post",
            data: {
                order_id: order_id,
                group_fly: group_fly
            },
            success: function (result) {
                div.html(result)
                _order_detail_common.Select2WithFixedOptionAndNoSearch($('.service-fly-extrapackage-packagename-select'))
                _order_detail_fly.CalucateTotalExtraPackages($('.service-fly-extrapackage-tbody'));
                _order_detail_fly.CalucateTotalAmountOfExtraPackages($('.service-fly-extrapackage-tbody'));
                _order_detail_fly.CalucateTotalProfit($('.service-fly-extrapackage-tbody'));
                _order_detail_fly.RemoveInfantPackageProfit(undefined);
                _order_detail_fly.CalucateTotalServiceAmount()
                _order_detail_fly.CalucateTotalServiceProfit()
            }
        });
    },
    FlyBookingServicePassengerPopup: function (order_id, group_fly) {
        var element = $('.service-fly-passenger-div')
        $.ajax({
            url: "AddFlyBookingServicePassenger",
            type: "post",
            data: {
                order_id: order_id,
                group_fly: group_fly
            },
            success: function (result) {
                element.html(result)
                $('.service-fly-passenger-genre').select2();
                _order_detail_common.SingleDatePickerBirthDay($('.service-fly-passenger-birthday'), 'up');
                
            }
        });
    },
    FlyBookingServiceRoutePopup: function (order_id, group_fly,leg) {
        $.ajax({
            url: "AddFlyBookingServiceRoute",
            type: "post",
            data: {
                group_fly: group_fly,
                order_id: order_id,
                leg: leg
            },
            success: function (result) {
                if (leg == '0') {
                    $('#add-service-fly-route-go').html(result)
                    $('#add-service-fly-route-go').show()
                    _order_detail_fly.AirlineCodeSuggesstion('add-service-fly-route-go');

                }
                else if (leg == '1') {
                    $('#add-service-fly-route-back').html(result)
                    $('#add-service-fly-route-back').show()
                    _order_detail_fly.AirlineCodeSuggesstion('add-service-fly-route-back');

                }
            }
        });
    },
    RemoveInfantPackageProfit: function (row_element) {
        if (row_element == undefined) {
            $('.service-fly-extrapackage-row').each(function (index, item) {
                var row_element_2 = $(this);
                var selected_package_id = row_element_2.find('.service-fly-extrapackage-packagename-select option:selected').val()
                if (selected_package_id == 'inf_amount') {
                    row_element_2.find('.service-fly-extrapackage-profit').hide()
                    row_element_2.find('.service-fly-extrapackage-profit').val('0')
                }
                else {
                    row_element_2.find('.service-fly-extrapackage-profit').show()

                }
            });

        }
        else {
            var selected_package_id = row_element.find('.service-fly-extrapackage-packagename-select option:selected').val()
            if (selected_package_id == 'inf_amount') {
                row_element.find('.service-fly-extrapackage-profit').hide()
                row_element.find('.service-fly-extrapackage-profit').val('0')
            }
            else {
                row_element.find('.service-fly-extrapackage-profit').show()
            }

        }
        _order_detail_fly.CalucateTotalProfit($('.service-fly-extrapackage-tbody'));

    },
    DeleteFlyBookingExtraPackage(element) {
        var table_element = element.closest('.service-fly-extrapackage-tbody')
        element.closest('.service-fly-extrapackage-row').remove()
        _order_detail_fly.CalucateTotalAmountOfExtraPackages(table_element)
        _order_detail_fly.CalucateTotalExtraPackages(table_element)
        _order_detail_fly.ReIndexExtraPackages(table_element)
    },
    AddFlyBookingExtraPackage(element) {
        var table_element = element.closest('.service-fly-extrapackage-tbody')
        var new_position = _order_detail_fly.GetLastestExtraPackagesNo(element) + 1;
        table_element.find('.service-fly-extrapackage-summary-row').before(_order_detail_html.html_servicefly_extrapackage_newpackage_tr.replaceAll('@(++index)', new_position))
        _order_detail_fly.CalucateTotalExtraPackages(element.closest('.service-fly-extrapackage-tbody'))
        _order_detail_common.Select2WithFixedOptionAndNoSearch($('.service-fly-extrapackage-packagename-select'))
    },
    AddFlyBookingExtraPackageExtra(element) {
        var table_element = element.closest('.service-fly-extrapackage-tbody')
        var new_position = _order_detail_fly.GetLastestExtraPackagesNo(element) + 1;
        table_element.find('.service-fly-extrapackage-summary-row').before(_order_detail_html.html_service_fly_extrapackage_new_extra_package_tr.replaceAll('@(++index)', new_position))
        _order_detail_fly.CalucateTotalExtraPackages(element.closest('.service-fly-extrapackage-tbody'))
    },
    AddFlyBookingPassenger(element) {
        var table_element = element.closest('.service-fly-passenger-table-content')
        var new_position = _order_detail_fly.GetLastestPassengerNo(element) + 1;
        table_element.find('.service-fly-passenger-summary-row').before(_order_detail_html.html_servicefly_passenger_newpassenger_tr.replaceAll('@p.Id', '0').replaceAll('@(++index)', '' + new_position).replaceAll('@(p.Name)', '').replaceAll('@(p.Birthday!=null?((DateTime)p.Birthday).ToString("dd/MM/yyyy"): DateTime.Now.ToString("dd/MM/yyyy"))', '').replaceAll('@p.Note', '').replaceAll('{genre_male_if_selected}', '').replaceAll('{genre_female_if_selected}', ''))
        $('.service-fly-passenger-genre-new').select2()
        $('.service-fly-passenger-genre-new').removeClass('service-fly-passenger-genre-new')
        _order_detail_common.SingleDatePickerBirthDay($('.service-fly-passenger-birthday'),'up')
        $('.service-fly-passenger-birthday-new').removeClass('service-fly-passenger-birthday-new')
    },
    DeleteFlyBookingPassenger(element) {
        var table_element = element.closest('.service-fly-passenger-table-content')
        element.closest('.service-fly-passenger-row').remove()
        _order_detail_fly.ReIndexPassenger(table_element)

    },
    Close: function () {
        $('#flybooking-service').removeClass('show')
        setTimeout(function () {
            $('#flybooking-service').remove();
            _order_detail_create_service.StartScrollingBody();
            _order_detail_fly.RemoveDynamicBind();
        }, 300);
    },

    RemoveDynamicBind: function () {
        $('#add-service-fly-select-route').off('change', function () {

        });

        $("body").off('change', '#add-service-fly-route-go .service-fly-select-route-from', function () {

        });
           
        $("body").off('change', '#add-service-fly-route-go .service-fly-select-route-to', function () {
          
        })

        $("body").off('keyup', ".service-fly-extrapackage-baseprice", function () {

        });
        $("body").off('keyup', ".service-fly-extrapackage-profit-extra", function () {
           
        });


        $("body").off('keyup', ".service-fly-extrapackage-quantity", function () {
         
        });
        $("body").off('apply.daterangepicker', ".service-fly-route-from-date", function (ev, picker) {
          
        });
        $("body").off('apply.daterangepicker', ".service-fly-route-to-date", function (ev, picker) {
          
        });
        $("body").off('apply.daterangepicker', ".service-fly-passenger-birthday", function (ev, picker) {
          
        });
        $("body").off('keyup', ".service-fly-passenger-birthday", function (ev, picker) {
      
        });
        $('.service-fly-note').keydown(null);
        $("body").off('change', ".import_data_guest", function (ev, picker) {
          
        });
    },
    Summit: function () {
      
        var error_on_collection = false;

        if ($('.add-service-fly-select-route').val() == undefined || $('.add-service-fly-select-route').val().trim() == '') {
            _msgalert.error("Vui lòng chọn loại vé")
            return;
        }
        if ($('.service-fly-select-route-from option:selected').val() == undefined || $('.service-fly-select-route-from option:selected').val().trim() == '') {
            _msgalert.error("Vui lòng chọn điểm đi")
            return;
        }
        if ($('.service-fly-select-route-to option:selected').val() == undefined || $('.service-fly-select-route-to option:selected').val().trim() == '') {
            _msgalert.error("Vui lòng chọn điểm đến")
            return;
        }
        if ($('.add-service-fly-main-staff option:selected').val() == undefined || $('.add-service-fly-main-staff option:selected').val().trim() == '') {
            _msgalert.error("Vui lòng chọn điều hành viên")
            return;
        }
        if ($('.service-fly-select-route-from option:selected').val() == undefined || $('.service-fly-select-route-from option:selected').val().trim() == '' || $('.service-fly-select-route-from option:selected').val() == undefined || $('.service-fly-select-route-to option:selected').val().trim()=='') {
            _msgalert.error("Vui lòng chọn điểm đi và điểm đến")
            return;
        }
        if ($('.service-fly-select-route-from option:selected').val() == $('.service-fly-select-route-to option:selected').val()) {
            _msgalert.error("Điểm đi và điểm đến không được trùng nhau")
            return;
        }
        var from_date = _order_detail_common.GetDateFromNoUpdateDateRangeElement($('.service-fly-route-from-date'))
        var to_date = _order_detail_common.GetDateFromNoUpdateDateRangeElement($('.service-fly-route-to-date'))
        /*
        if ($("#add-service-fly-select-route option:selected").val().trim() == '2' && from_date >= to_date) {
            _msgalert.error("Ngày về không được nhỏ hơn hoặc bằng ngày đi")
            return;
        }*/
        if (error_on_collection) return;
        $('.service-fly-route-airline').each(function (index, item) {
            var element = $(this);
            if (element.find(':selected').val() == undefined || element.find(':selected').val().trim() == '') {
                _msgalert.error("Vui lòng chọn hãng bay cho " + element.closest('.add-service-fly-form-1route').attr('data-route-name'))
                error_on_collection = true
                return false
            }
        });
        if (error_on_collection) return;
        if ($('.service-fly-extrapackage-row').length<=0) {
            _msgalert.error("Vui lòng nhập ít nhất 01 dịch vụ trong bảng kê dịch vụ")
            return
        }
        $('.service-fly-extrapackage-packagename-select').each(function (index, item) {
            var element = $(this);
            if (element.find(':selected').val() == undefined || element.find(':selected').val().trim() == '') {
                _msgalert.error("Vui lòng chọn loại dịch vụ cho dịch vụ thứ " + element.closest('.service-fly-extrapackage-row').find('.service-fly-extrapackage-order').text())
                error_on_collection = true
                return false
            }
        });
        if (error_on_collection) return;
        if ($('.service-fly-extrapackage-packagename-select-input').length>0) {
            $('.service-fly-extrapackage-packagename-select-input').each(function (index, item) {
                var element = $(this);
                if (element.val() == undefined || element.val().trim() == '') {
                    _msgalert.error("Vui lòng nhập nội dung dịch vụ khác cho dịch vụ thứ " + element.closest('.service-fly-extrapackage-row').find('.service-fly-extrapackage-order').text())
                    error_on_collection = true
                    return false
                }
            });
            if (error_on_collection) return;
        }
        $('.service-fly-extrapackage-baseprice').each(function (index, item) {
            var element = $(this);
            if (element.val() == undefined || element.val().trim() == '') {
                _msgalert.error("Vui lòng nhập đơn giá cho dịch vụ thứ " + element.closest('.service-fly-extrapackage-row').find('.service-fly-extrapackage-order').text())
                error_on_collection = true
                return false
            }
        });
        if (error_on_collection) return;
        $('.service-fly-extrapackage-quantity').each(function (index, item) {
            var element = $(this);
            if (element.val() == undefined || element.val().trim() == '') {
                _msgalert.error("Vui lòng nhập số lượng cho dịch vụ thứ " + element.closest('.service-fly-extrapackage-row').find('.service-fly-extrapackage-order').text())
                error_on_collection = true
                return false
            }
        });
        if (error_on_collection) return;
        $('.service-fly-extrapackage-profit-extra').each(function (index, item) {
            var element = $(this);
            if (element.val() == undefined || element.val().trim() == '') {
                _msgalert.error("Vui lòng nhập lợi nhuận cho dịch vụ thứ " + element.closest('.service-fly-extrapackage-row').find('.service-fly-extrapackage-order').text())
                error_on_collection = true
                return false
            }
            else {
                var profit = parseFloat(element.val().replaceAll(',', '')) == undefined || isNaN(parseFloat(element.val().replaceAll(',', ''))) ? 0 : parseFloat(element.val().replaceAll(',', ''))
                var amount = parseFloat(element.closest('.service-fly-extrapackage-row').find('.service-fly-extrapackage-amount').val().replaceAll(',', ''))
                if (amount < profit) {
                    _msgalert.error("Lợi nhuận của dịch vụ thứ" + element.closest('.service-fly-extrapackage-row').find('.service-fly-extrapackage-order').text() +" không thể lớn hơn thành tiền ")
                    error_on_collection = true
                    return false
                }
            }
        });
        if (error_on_collection) return;
        var pathname = window.location.pathname.split('/');
        var arrive_date = _order_detail_common.GetDateFromNoUpdateDateRangeElement($('.service-fly-route-from-date'), false)
        var departure_date = _order_detail_common.GetDateFromNoUpdateDateRangeElement($('.service-fly-route-to-date'), false)
        var object_summit = {
            order_id: pathname[pathname.length - 1],
            route: $("#add-service-fly-select-route option:selected").val(),
            main_staff: $("#add-service-fly-main-staff option:selected").val(),
            start_date: _global_function.GetDayText(arrive_date, true),
            end_date: _global_function.GetDayText(departure_date, true),
            start_point: $('.service-fly-select-route-from').val(),
            end_point: $('.service-fly-select-route-to').val(),
            group_id: $('#add-service-fly-form-select-route').attr('data-group-fly'),
            service_code: $('#add-service-fly-form-select-route').attr('data-service-code'),
            profit: $('.service-manual-fly-total-service-profit').html().replaceAll(',', ''),
            others_amount: (isNaN(parseFloat($('#servicemanual-fly-other-amount').val().replaceAll(',', ''))) ? 0 : parseFloat($('#servicemanual-fly-other-amount').val().replaceAll(',', ''))),
            commission: (isNaN(parseFloat($('#servicemanual-fly-commission').val().replaceAll(',', ''))) ? 0 : parseFloat($('#servicemanual-fly-commission').val().replaceAll(',', ''))),
            note: $('.service-fly-note').val(),
            go: {
                id: $('#add-service-fly-route-go .add-service-fly-form-1route').attr('data-flybooking-id'),
                airline: $("#add-service-fly-route-go .service-fly-route-airline option:selected").val(),
                fly_code: $('#add-service-fly-route-go .service-fly-route-flycode').val(),
                booking_code: $('#add-service-fly-route-go .service-fly-route-bookingcode').val(),
              
            },
            back: {

            },
            passenger: [],
            extra_packages: []
        }
        if (object_summit.route.trim() == '1') {
            object_summit.end_date = _global_function.GetDayText(arrive_date, true)
        }
        $('.service-fly-extrapackage-row').each(function (index, item) {
            var extra_package_element = $(item);
            var package_id = extra_package_element.find('.service-fly-extrapackage-packagename-select option:selected').val();
            var package_name = extra_package_element.find('.service-fly-extrapackage-packagename-select option:selected').text();
            if (package_id == undefined || package_name == undefined) {
                package_id = extra_package_element.find('.service-fly-extrapackage-packagename-select-input').val();
                package_name = extra_package_element.find('.service-fly-extrapackage-packagename-select-input').val();
                if (package_id == undefined || package_id.trim() == '' || package_name == undefined || package_name.trim() == '') {
                    _msgalert.error("Nội dung tại dịch vụ thứ " + extra_package_element.find('.service-fly-extrapackage-order').text() + ' của Bảng kê dịch vụ không được bỏ trống')
                    error_on_collection = true
                    return false;
                }
            }
            if (error_on_collection) return false;
            var base_price = parseFloat(isNaN(extra_package_element.find('.service-fly-extrapackage-baseprice').val().replaceAll(',', '')) ? 0 : extra_package_element.find('.service-fly-extrapackage-baseprice').val().replaceAll(',', ''))
            var quanity = parseFloat(isNaN(extra_package_element.find('.service-fly-extrapackage-quantity').val().replaceAll(',', '')) ? 0 : extra_package_element.find('.service-fly-extrapackage-quantity').val().replaceAll(',', ''))
            var extra_package = {
                id: extra_package_element.attr('data-extra-package-id'),
                package_id: package_id,
                package_code: package_name,
                base_price: base_price * quanity,
                quantity: extra_package_element.find('.service-fly-extrapackage-quantity').val().replaceAll(',', ''),
                amount: extra_package_element.find('.service-fly-extrapackage-amount').val().replaceAll(',', ''),
                profit: extra_package_element.find('.service-fly-extrapackage-profit') != undefined && extra_package_element.find('.service-fly-extrapackage-profit').val() != undefined ? extra_package_element.find('.service-fly-extrapackage-profit').val().replaceAll(',', '') : 0,
            }
            object_summit.extra_packages.push(extra_package);
        });
        if (error_on_collection) return;
        if ($('#add-service-fly-route-back').is(':visible')) {
            object_summit.back = {
                id: $('#add-service-fly-route-back .add-service-fly-form-1route').attr('data-flybooking-id'),
                airline: $("#add-service-fly-route-back .service-fly-route-airline option:selected").val(),
                fly_code: $('#add-service-fly-route-back .service-fly-route-flycode').val(),
                booking_code: $('#add-service-fly-route-back .service-fly-route-bookingcode').val(),
            }
        }

        $('#service-fly-form .service-fly-passenger-row').each(function (index, item) {
            var passenger_element = $(item);
            var passenger = {
                id: passenger_element.attr('data-passenger-id'),
                genre: passenger_element.find('.service-fly-passenger-genre').find(':selected').val(),
                name: passenger_element.find('.service-fly-passenger-name').val(),
                birthday: passenger_element.find('.service-fly-passenger-birthday').val() == undefined || passenger_element.find('.service-fly-passenger-birthday').val().trim()==''?null: _global_function.GetDayText(passenger_element.find('.service-fly-passenger-birthday').data('daterangepicker').startDate._d, true),
                note: passenger_element.find('.service-fly-passenger-note').val(),
            }
            object_summit.passenger.push(passenger);
        });
        var adt_pkg_count=0
        var chd_pkg_count=0
        var inf_pkg_count=0
        $(object_summit.extra_packages).each(function (index, item) {
            switch (item.package_id.trim()) {
                case 'adt_amount': {
                    adt_pkg_count++
                } break;
                case 'chd_amount': {
                    chd_pkg_count++

                } break;
                case 'inf_amount': {
                    inf_pkg_count++

                } break;
            }

        });
        if (adt_pkg_count > 0 && adt_pkg_count > 1) {
            _msgalert.error('Vui lòng chỉ nhập đúng 01 dịch vụ với nội dung là chi phí dịch vụ cho [Người lớn]')
            return;
        }
        if (chd_pkg_count > 0 && chd_pkg_count > 1) {
            _msgalert.error('Vui lòng chỉ nhập đúng 01 dịch vụ với nội dung là chi phí dịch vụ cho [Người lớn]')
            return;
        }
        if (inf_pkg_count > 0 && inf_pkg_count > 1) {
            _msgalert.error('Vui lòng chỉ nhập đúng 01 dịch vụ với nội dung là chi phí dịch vụ cho [Người lớn]')
            return;
        }
        if ((object_summit.go.id != undefined && (!isNaN(parseInt(object_summit.go.id))) && parseInt(object_summit.go.id) > 0)) {
           
            var descriptiion = _order_detail_html.summit_confirmbox_create_fly_booking_service_description.replace('{is_new}', 'đã được sửa')
            _msgconfirm.openDialog(_order_detail_html.summit_confirmbox_title, descriptiion, function () {
                $('.btn-summit-service-fly').attr('disabled', 'disabled')
                $('.btn-summit-service-fly').addClass('disabled')
                $('#service-fly-button-div').append(_order_detail_html.html_loading_gif);

                $.ajax({
                    url: "SummitFlyBookingServiceData",
                    type: "post",
                    data: { data: object_summit },
                    success: function (result) {
                        if (result != undefined && result.status == 0) {
                            _msgalert.success('Dịch vụ vé máy bay đã được sửa thành công');
                            _order_detail_fly.Close();
                            _global_function.ConfirmFileUpload($('.attachment-file-block'), result.data)

                            setTimeout(function () {
                                window.location.reload();
                            }, 1000);
                        }
                        else {
                            _msgalert.error(result.msg);
                            $('.btn-summit-service-fly').removeAttr('disabled')
                            $('.btn-summit-service-fly').removeClass('disabled')
                            $('.img_loading_summit').remove()
                        }

                    }
                });
            });
        }
        else {
            $('.btn-summit-service-fly').attr('disabled', 'disabled')
            $('.btn-summit-service-fly').addClass('disabled')
            $('#service-fly-button-div').append(_order_detail_html.html_loading_gif);
            $.ajax({
                url: "SummitFlyBookingServiceData",
                type: "post",
                data: { data: object_summit },
                success: function (result) {
                    if (result != undefined && result.status == 0) {
                        _msgalert.success(result.msg);
                        _order_detail_fly.Close();
                        _global_function.ConfirmFileUpload($('.attachment-file-block'), result.data)

                        setTimeout(function () {
                            window.location.reload();
                        }, 300);
                    }
                    else {
                        _msgalert.error(result.msg);
                        $('.btn-summit-service-fly').removeAttr('disabled')
                        $('.btn-summit-service-fly').removeClass('disabled')
                        $('.img_loading_summit').remove()
                    }

                }
            });
        }

    },
    CalucateRowAmountandProfit: function (element) {
        var row_element = element.closest('.service-fly-extrapackage-row')

        var base_price_element = row_element.find('.service-fly-extrapackage-baseprice')
        var base_price = isNaN(parseFloat(base_price_element.val().replaceAll(',', ''))) ? 0 : parseFloat(base_price_element.val().replaceAll(',', ''))

        var quanity_element = row_element.find('.service-fly-extrapackage-quantity')
        var quantity = isNaN(parseFloat(quanity_element.val().replaceAll(',', ''))) ? 0 : parseInt(quanity_element.val().replaceAll(',', ''))

        var sale_price_element = row_element.find('.service-fly-extrapackage-saleprice')
        var sale_price = isNaN(parseFloat(sale_price_element.val().replaceAll(',', ''))) ? 0 : parseFloat(sale_price_element.val().replaceAll(',', ''))

        var amount = sale_price * quantity
        row_element.find('.service-fly-extrapackage-amount').val(_global_function.Comma(amount)).change();

        var profit = (sale_price - base_price) * quantity
        row_element.find('.service-fly-extrapackage-profit').val((profit >= 0 ? '' : '-') + _global_function.Comma(profit)).change();
    },
    CalucateTotalAmountOfExtraPackages: function (table_element) {
        var total = 0;
        table_element.find('.service-fly-extrapackage-amount').each(function (index, item) {
            var element_amount = $(item);
            total = total + (isNaN(parseFloat(element_amount.val().replaceAll(',', ''))) ? 0 : parseFloat(element_amount.val().replaceAll(',', '')))
        });
        table_element.find('.service-fly-extrapackage-total-amount').html(_global_function.Comma(total)).change();
        _order_detail_fly.CalucateTotalServiceAmount()
    },
  
    CalucateTotalProfit: function (table_element) {
        var total = 0;
        table_element.find('.service-fly-extrapackage-profit').each(function (index, item) {
            var element_amount = $(item);
            total = total + (isNaN(parseFloat(element_amount.val().replaceAll(',', ''))) ? 0 : parseFloat(element_amount.val().replaceAll(',', '')))
        });
        table_element.find('.service-fly-extrapackage-total-profit').html((total >= 0 ? '' : '-') + _global_function.Comma(total)).change();
        _order_detail_fly.CalucateTotalServiceProfit()

    },
    CalucateTotalExtraPackages: function (table_element) {
        var total = 0;
        table_element.find('.service-fly-extrapackage-quantity').each(function (index, item) {
            var row_element = $(this);
            total = total + parseInt(row_element.val().replaceAll(',', ''))
        });
        table_element.find('.service-fly-extrapackage-total-quantity').html(_global_function.Comma(total)).change();

    },
    CalucateTotalServiceAmount: function () {
        var package_text = $('.service-fly-extrapackage-total-amount').html()
        $('.service-manual-fly-total-service-amount').html(package_text)
    },
    CalucateTotalServiceProfit: function () {
        var profit = 0
        var table_element = $('.service-fly-extrapackage-tbody')
        table_element.find('.service-fly-extrapackage-profit').each(function (index, item) {
            var element_amount = $(item);
            profit = profit + (isNaN(parseFloat(element_amount.val().replaceAll(',', ''))) ? 0 : parseFloat(element_amount.val().replaceAll(',', '')))
        });

        var other_amount_element = $('#servicemanual-fly-other-amount')
        var other_amount = (isNaN(parseFloat(other_amount_element.val().replaceAll(',', ''))) ? 0 : parseFloat(other_amount_element.val().replaceAll(',', '')))

        var discount_element = $('#servicemanual-fly-commission')
        var discount = (isNaN(parseFloat(discount_element.val().replaceAll(',', ''))) ? 0 : parseFloat(discount_element.val().replaceAll(',', '')))

        var total_profit = profit - discount - other_amount
        $('.service-manual-fly-total-service-profit').html((total_profit >= 0 ? '' : '-')+ _global_function.Comma(total_profit)).change();
    },
    ReIndexExtraPackages: function (table_element) {
        var total = 0;
        table_element.find('.service-fly-extrapackage-row').each(function (index, item) {
            var row_element = $(this);
            total++;
            row_element.find('.service-fly-extrapackage-order').html('' + total)
        });
    },
    ReIndexPassenger: function (table_element) {
        var total = 0;
        table_element.find('.service-fly-passenger-row').each(function (index, item) {
            var row_element = $(this);
            total++;
            row_element.find('.service-fly-passenger-number').html('' + total)
        });
    },
    GetLastestExtraPackagesNo: function (element) {
        var total = 0;
        element.closest('.service-fly-extrapackage-tbody').find('.service-fly-extrapackage-row').each(function (index, item) {
            total++;
        });
        return total
    },
    GetLastestPassengerNo: function (element) {
        var total = 0;
        element.closest('.service-fly-passenger-table-content').find('.service-fly-passenger-row').each(function (index, item) {
            total++;
        });
        return total
    },
}


