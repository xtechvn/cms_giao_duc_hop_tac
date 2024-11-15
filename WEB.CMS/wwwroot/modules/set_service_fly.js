var _set_service_fly_html = {
    html_user_option: '<option class="select2-results__option" value="{user_id}">{user_name} - {user_email}{user_phone}</option>',
    confirm_box_description_grant_order: 'Bạn có chắc muốn nhận đặt dịch vụ {Mã dịch vụ} không?',
    confirm_box_description_send_service_code: 'Bạn có chắc muốn trả code dịch vụ {Mã dịch vụ} không?',
    confirm_box_description_change_to_payment: 'Bạn có chắc muốn quyết toán dịch vụ {Mã dịch vụ} không?',
    confirm_box_description_operator_order_price: 'Thông tin giá đặt dịch vụ sẽ được lưu lại, bạn có chắc chắn không?',
    confirm_box_description_decline: 'Bạn có chắc muốn từ chối dịch vụ {Mã dịch vụ} không?',
    confirm_box_title_grant_order: 'Confirm nhận đặt dịch vụ',
    confirm_box_title_send_service_code: 'Confirm trả Code dịch vụ',
    confirm_box_title_change_to_payment: 'Confirm quyết toán',
    confirm_box_title_operator_order_price: 'Thay đổi giá đặt dịch vụ',
    confirm_box_title_decline: 'Từ chối đặt dịch vụ',
    html_ordered_new_packages: '<tr class="service-fly-ordered-row" data-id="0"> <td class="service-fly-ordered-order">@(++index)</td> <td><p class="form-control service-fly-ordered-package-name ">@item.PackageName</p></td> <td><select class="select2 service-fly-ordered-suplier " style="width:100% !important" > </select> <td ><select  class="form-control supplier-select  service-fly-status" name="service-fly-status"><option value="0">Sử dụng </option><option value="1">Hủy </option></select></td ></td > <td><input type="text" class="form-control currency service-fly-ordered-amount text-right"  value="0" /></td> <td><input type="text" class="form-control service-fly-ordered-note" value="" /></td> <td> <a class="fa fa-trash-o service-fly-ordered-delete-row-disabled service-fly-ordered-delete-row" href="javascript:;"></a></td> </tr > '
}


var _set_service_fly = {
    Initialization: function () {
        _common_function_fly.SingleDateRangePicker($('.set-service-fly-search-startdate'))
        _common_function_fly.SingleDateRangePicker($('.set-service-fly-search-enddate'))
        _common_function_fly.SingleDateRangePicker($('.set-service-fly-search-createddate'))

        _common_function_fly.UserSuggesstionMultiple($('.set-service-fly-search-saler'))
        _common_function_fly.UserSuggesstionMultiple($('.set-service-fly-search-main-staff'))
        _common_function_fly.UserSuggesstionMultiple($('.set-service-fly-search-usercreate'))
    
        _set_service_fly.DynamicBindInput()
        _set_service_fly.SearchData()
        setTimeout(function () {


            $('.set-service-fly-search-status').select2({
                placeholder: "Tất cả trạng thái dịch vụ"
            })
        }, 500)
    },
    DynamicBindInput: function () {
        $("body").on('click', ".fly-status", function (ev, picker) {
            var element = $(this)
            if (!element.hasClass('open')) {
                element.addClass('open')
            }
            else {
                element.removeClass('open')
            }
        });
        $("body").on('click', ".fly-status-option", function (ev, picker) {
            var element = $(this)
            if (!ev.target.closest(".checkbox")) {
                if (element.find('.checkbox').prop('checked') == true) {
                    element.find('.checkbox').prop('checked', false);
                }
                else {
                    element.find('.checkbox').prop('checked', true);
                }
            }
            var option_selected = false
            var count=0
            $("input[name='fly-status-value']:checked").each(function (index, item) {
                if (!option_selected) option_selected = true
                count++;
            });

            if (!option_selected) {
                $('.fly-status-lbl').html('Tất cả trạng thái')
            }
            else {
                $('.fly-status-lbl').html('0' + count +' trạng thái')
            }
        });
        $("body").on('click', function (ev, picker) {
            if (!ev.target.closest(".fly-select-box")) {
                $('.fly-status').removeClass('open')
            }
        });
        $("body").on('apply.daterangepicker', ".set-service-fly-search-startdate", function (ev, picker) {
            _common_function_fly.OnApplyStartDateTimeOfBookingRange($(this), $('.set-service-fly-search-enddate'))

        });
        $("body").on('apply.daterangepicker', ".set-service-fly-search-daterange", function (ev, picker) {
            $(this).val(picker.startDate.format('DD/MM/YYYY') + ' - ' + picker.endDate.format('DD/MM/YYYY'));
        });
        $("body").on('focusout', ".set-service-fly-search-startdate", function (ev, picker) {
            if ($(this).val() == undefined || $(this).val().trim() == '') {
                _common_function_fly.SingleDateRangePicker($('.set-service-fly-search-enddate'))
            }
        });
        $("body").on('click', ".service-fly-filter-by-status", function (ev, picker) {
            var element = $(this)

            $('.service-fly-filter-by-status').removeClass('active')
            element.addClass('active')

            var status = [];

            $('input[name="fly-status-value"]').each(function (index, item) {
                var element_status = $(item);
                if (element_status.val().trim() == element.attr('data-status').trim()) {
                    element_status.prop("checked", true);
                }
                else {
                    element_status.prop("checked", false);

                }

            });
            $('.fly-status-lbl').html('01 trạng thái')

            //if (element.attr('data-status').trim() != '-1') status = [element.attr('data-status')]
            //{
            //    $('input[name="fly-status-value"][value="' + element.attr('data-status').trim() + '"]').prop("checked", true);
            //}

            //$('.set-service-fly-search-status').val(status).change();
            _set_service_fly.SearchData()


        });



    },
    Search: function (input) {
        window.scrollTo(0, 0);
        $.ajax({
            url: "/SetService/FlySearch",
            type: "Post",
            data: input,
            success: function (result) {
                $('#search_data_grid').html(result);
                _common_function_fly.OrderNoSuggesstion($('.set-service-fly-search-orderno'))
                _common_function_fly.ServiceCodeSuggesstion($('.set-service-fly-search-servicecode'))
                _common_function_fly.Select2BookingCode($("#BookingCode"))
            }
        });
    },
    OnPaging: function (value) {
        if (value > 0) {
            var objSearch = _set_service_fly.GetParam(value);
            _set_service_fly.Search(objSearch);
        }
    },
    onSelectPageSize: function () {
        _set_service_fly.SearchData();

    },
    SearchData: function () {
        var objSearch = _set_service_fly.GetParam(1);
        _set_service_fly.Search(objSearch);
    },
    GetParam: function (PageIndex) {
        var object_search_summit = {
            searchModel: {
                ServiceCode: $('.set-service-fly-search-servicecode').find(':selected').val() == undefined || $('.set-service-fly-search-servicecode').find(':selected').val().trim() == '' ? '' : $('.set-service-fly-search-servicecode').find(':selected').val().trim(),
                OrderCode: $('.set-service-fly-search-orderno').find(':selected').val() == undefined || $('.set-service-fly-search-orderno').find(':selected').val().trim() == '' ? '' : $('.set-service-fly-search-orderno').find(':selected').val().trim(),
            }

        }
        var status = '';
        var is_more_than1 = false;
        $("input[name='fly-status-value']:checked").each(function (index, item) {
            var element = $(item);
            if (status == '') status = '' + element.val()
            else {
                status = status + ',' + element.val()
                is_more_than1=true
            }
        });
        if (is_more_than1) {
            $('.service-fly-filter-by-status').each(function (index, item) {
                var element = $(item);
                if (element.attr('data-status') == "-1") {
                    element.addClass('active')
                }
                else if (element.hasClass('active')) {
                    element.removeClass('active')
                }

            });
        }
       
        object_search_summit.searchModel.StatusBooking = status
        object_search_summit.searchModel.BookingCode = $('#BookingCode').val()

        object_search_summit.searchModel.UserCreate = $('.set-service-fly-search-usercreate').find(':selected').val() == undefined || $('.set-service-fly-search-usercreate').find(':selected').val().trim() == '' ? null : $('.set-service-fly-search-usercreate').find(':selected').val()
        object_search_summit.searchModel.SalerId = $('.set-service-fly-search-saler').find(':selected').val() == undefined || $('.set-service-fly-search-saler').find(':selected').val().trim() == '' ? null : $('.set-service-fly-search-saler').find(':selected').val()
        object_search_summit.searchModel.OperatorId = $('.set-service-fly-search-main-staff').find(':selected').val() == undefined || $('.set-service-fly-search-main-staff').find(':selected').val().trim() == '' ? null : $('.set-service-fly-search-main-staff').find(':selected').val()
        object_search_summit.searchModel.PageIndex = PageIndex
        object_search_summit.searchModel.pageSize = isNaN(parseInt($('#selectPaggingOptions').find(':selected').val())) || parseInt($('#selectPaggingOptions').find(':selected').val()) == undefined ? 30 : parseInt($('#selectPaggingOptions').find(':selected').val())
        var compare_from = _global_function.GetDayText($('.set-service-fly-search-startdate').data('daterangepicker').startDate._d, true).trim().split(' ')[0]
        var compare_to = _global_function.GetDayText($('.set-service-fly-search-startdate').data('daterangepicker').endDate._d, true).trim().split(' ')[0]

        if ($('.set-service-fly-search-startdate').val() == undefined || $('.set-service-fly-search-startdate').val().trim() == '') {
            object_search_summit.searchModel.StartDateFrom = undefined;
            object_search_summit.searchModel.StartDateTo = undefined;

        }
        else {
            object_search_summit.searchModel.StartDateFrom = _global_function.GetDayText($('.set-service-fly-search-startdate').data('daterangepicker').startDate._d, true);
            object_search_summit.searchModel.StartDateTo = _global_function.GetDayText($('.set-service-fly-search-startdate').data('daterangepicker').endDate._d, true);
        }
        compare_from = _global_function.GetDayText($('.set-service-fly-search-enddate').data('daterangepicker').startDate._d, true).trim().split(' ')[0]
        compare_to = _global_function.GetDayText($('.set-service-fly-search-enddate').data('daterangepicker').endDate._d, true).split(' ')[0]
        if ($('.set-service-fly-search-enddate').val() == undefined || $('.set-service-fly-search-enddate').val().trim() == '') {
            object_search_summit.searchModel.EndDateFrom = undefined;
            object_search_summit.searchModel.EndDateTo = undefined;
        }
        else {
            object_search_summit.searchModel.EndDateFrom = _global_function.GetDayText($('.set-service-fly-search-enddate').data('daterangepicker').startDate._d, true);
            object_search_summit.searchModel.EndDateTo = _global_function.GetDayText($('.set-service-fly-search-enddate').data('daterangepicker').endDate._d, true);
        }
        compare_from = _global_function.GetDayText($('.set-service-fly-search-createddate').data('daterangepicker').startDate._d, true).trim().split(' ')[0]
        compare_to = _global_function.GetDayText($('.set-service-fly-search-createddate').data('daterangepicker').endDate._d, true).split(' ')[0]
        if ($('.set-service-fly-search-createddate').val() == undefined || $('.set-service-fly-search-createddate').val().trim() == '') {
            object_search_summit.searchModel.CreateDateFrom = undefined;
            object_search_summit.searchModel.CreateDateTo = undefined;

        }
        else {
            object_search_summit.searchModel.CreateDateFrom = _global_function.GetDayText($('.set-service-fly-search-createddate').data('daterangepicker').startDate._d, true);
            object_search_summit.searchModel.CreateDateTo = _global_function.GetDayText($('.set-service-fly-search-createddate').data('daterangepicker').endDate._d, true);
        }

        return object_search_summit;
    },
    ChangeTotalFlyTicketTotalCount: function (total_count) {
        status_type = -1;
        $('.service-fly-filter-by-status').each(function (index, item) {
            var element = $(item);
            if (element.hasClass('active')) {
                status_type = parseInt(element.attr('data-status').trim())
            }

        });
        
        switch (status_type) {
            case -1: {
                if ($('.set-service-fly-total-count').is(":hidden")) $('.set-service-fly-total-count').show()
                $('.set-service-fly-total-count').html('(' + total_count + ')')
            } break;
            /*
            case 0: {
                if ($('.set-service-fly-new-order-total-count').is(":hidden")) $('.set-service-fly-new-order-total-count').show()

                $('.set-service-fly-new-order-total-count').html('(' + total_count + ')')
            } break;*/
            case 1: {
                if ($('.set-service-fly-waiting-total-count').is(":hidden")) $('.set-service-fly-waiting-total-count').show()

                $('.set-service-fly-waiting-total-count').html('(' + total_count + ')')
            } break;
            case 2: {
                if ($('.set-service-fly-on-progress-total-count').is(":hidden")) $('.set-service-fly-on-progress-total-count').show()

                $('.set-service-fly-on-progress-total-count').html('(' + total_count + ')')
            } break;
            case 3: {
                if ($('.set-service-fly-return-code-total-count').is(":hidden")) $('.set-service-fly-return-code-total-count').show()

                $('.set-service-fly-return-code-total-count').html('(' + total_count + ')')
            } break;
            case 4: {
                if ($('.set-service-fly-finished-payment-total-count').is(":hidden")) $('.set-service-fly-finished-payment-total-count').show()

                $('.set-service-fly-finished-payment-total-count').html('(' + total_count + ')')

            } break;
            case 5: {
                if ($('.set-service-fly-decline-total-count').is(":hidden")) $('.set-service-fly-decline-total-count').show()

                $('.set-service-fly-decline-total-count').html('(' + total_count + ')')
            } break;
             case 5: {
                if ($('.set-service-fly-decline-total-count').is(":hidden")) $('.set-service-fly-decline-total-count').show()

                $('.set-service-fly-decline-total-count').html('(' + total_count + ')')
            } break;
            default: break;
        }
    },
    Export: function () {
        $('#btnExport').prop('disabled', true);
        $('#icon-export').removeClass('fa-file-excel-o');
        $('#icon-export').addClass('fa-spinner fa-pulse');
        var objSearch = _set_service_fly.GetParam(1);
        objSearch.PageIndex = 1;
        objSearch.searchModel.pageSize = $('#countFy').val();
        this.searchModel = objSearch;
  
        _global_function.AddLoading()
        $.ajax({
            url: "/SetService/FlyExportExcel",
            type: "Post",
            data: this.searchModel,
            success: function (result) {
                _global_function.RemoveLoading()
                $('#btnExport').prop('disabled', false);
                if (result.isSuccess) {
                    _msgalert.success(result.message);
                    window.location.href = result.path;
                } else {
                    _msgalert.error(result.message);
                }
                $('#icon-export').removeClass('fa-spinner fa-pulse');
                $('#icon-export').addClass('fa-file-excel-o');
            }
        });
    },
}


var _set_service_fly_detail = {
    ServiceType: 15,
    ServiceTypeSale:3,
    Initialization: function () {
        _set_service_fly_detail.DynamicBindInput()
        $('.service-fly-detail-change-tab-button').click()
        switch ($('#service-fly-detail-data').attr('data-status')) {
            case '1':
            case '2': {
                _set_service_fly_detail.ShowNeedToOrderTab()

            } break;
            case '3':
            case '4': {
                _set_service_fly_detail.ShowPaymentTab()

            } break;
           
            default: {
                _set_service_fly_detail.ShowOrderTab()

            } break;
        }
    },
    DynamicBindInput: function () {
        $('body').on('click', '.service-fly-detail-change-tab-button', function (event) {
            var element = $(this)
            switch (element.attr('data-tab-id')) {
                case '0': {
                    _set_service_fly_detail.ShowOrderTab()

                } break;
                case '1': {
                    _set_service_fly_detail.ShowNeedToOrderTab()

                } break;
                case '2': {
                    _set_service_fly_detail.ShowOrderedTab()

                } break;
                case '3': {
                    _set_service_fly_detail.ShowPaymentTab()

                } break;
                case '4': {
                    _set_service_fly_detail.ShowServiceCodeTab()

                } break;
                case '5': {
                    _set_service_fly_detail.ShowRefundTab()

                } break;
                case '6': {
                    _set_service_fly_detail.MarketingTab()

                } break;
            }

        });
        $('body').on('change', '.service-fly-status', function () {
            _set_service_fly_detail.TabReCalucateTotalAmount()
        });
    },
    Back: function () {
        window.location.href = '/SetService/Fly';
    },
    PopupSendEmail: function () {
        let title = 'Gửi Code dịch vụ vé máy bay';
        let url = '/SetService/SendEmail';
        let param = {
            id: $('#service-fly-detail-data').attr('data-goserviceid').trim(),
            Orderid: $('#service-fly-detail-data').attr('data-order-id').trim(),
            type: 3,
            group_booking_id: $('#service-fly-detail-data').attr('data-group-booking-id').trim()
        };
        _magnific.OpenSmallPopup(title, url, param);

    },
    GrantOrderPermission: function (element) {
        element.attr('disabled', true)
        var group_booking_id = $('#service-fly-detail-data').attr('data-group-booking-id')
        var service_code = $('#service-fly-detail-data').attr('data-servicecode')
        _msgconfirm.openDialog(_set_service_fly_html.confirm_box_title_grant_order, _set_service_fly_html.confirm_box_description_grant_order.replace('{Mã dịch vụ}', service_code), function () {
            $.ajax({
                url: "/SetService/GrantOrderPermission",
                type: "post",
                data: { group_booking_id: group_booking_id },
                success: function (result) {
                    if (result != undefined && result.status == 0) {
                        _msgalert.success(result.msg);
                        setTimeout(function () { window.location.reload() }, 500)
                    }
                    else {
                        _msgalert.error(result.msg);
                    }

                }
            });

        });
        element.attr('disabled', false)
    },
    SendServiceCode: function (element) {
        element.attr('disabled', true)
        var group_booking_id = $('#service-fly-detail-data').attr('data-group-booking-id')
        var service_code = $('#service-fly-detail-data').attr('data-servicecode')
        _msgconfirm.openDialog(_set_service_fly_html.confirm_box_title_send_service_code, _set_service_fly_html.confirm_box_description_send_service_code.replace('{Mã dịch vụ}', service_code), function () {
            $.ajax({
                url: "/SetService/SendServiceCode",
                type: "post",
                data: { group_booking_id: group_booking_id },
                success: function (result) {
                    if (result != undefined && result.status == 0) {
                        _msgalert.success(result.msg);
                        setTimeout(function () { window.location.reload() }, 500)

                    }
                    else {
                        _msgalert.error(result.msg);
                        element.removeAttr('disabled');

                    }
                }
            });
        });
        element.removeAttr('disabled');

    },
    ChangeToPayment: function (element) {
        element.attr('disabled', true)
        var group_booking_id = $('#service-fly-detail-data').attr('data-group-booking-id')
        var service_code = $('#service-fly-detail-data').attr('data-servicecode')
        var Orderid = $('#service-fly-detail-data').attr('data-order-id')
        var list = group_booking_id.split(",");
        var list_group = [];
        $(list).each(function (index, item) {
            list_group.push(parseInt(item))

        });
        _msgconfirm.openDialog(_set_service_fly_html.confirm_box_title_change_to_payment, _set_service_fly_html.confirm_box_description_change_to_payment.replace('{Mã dịch vụ}', service_code), function () {
            $.ajax({
                url: "/SetService/ChangeToConfirmPaymentStatus",
                type: "post",
                data: { group_booking_id: group_booking_id, group_list: list },
                success: function (result) {
                    if (result != undefined && result.status == 0) {
                        _msgalert.success(result.msg);
                        setTimeout(function () { window.location.reload() }, 500)

                    }
                    else {
                        _msgalert.error(result.msg);
                        element.removeAttr('disabled');

                    }
                }
            });
        });
        element.removeAttr('disabled');

    },
    ShowOrderTab: function () {
        $('.service-fly-detail-change-tab-button').removeClass('active')
        $('.service-fly-detail-button-order').addClass('active')
        $('.service-fly-detail-tab').hide()
        $('.service-fly-detail-tab-order').show()
        if ($('.service-fly-detail-button-order').attr('data-loaded').trim() == '0') {
            $.ajax({
                url: "/SetService/ListOrder",
                type: "Post",
                data: { id: $('#service-fly-detail-data').attr('data-order-id') },
                success: function (result) {
                    $('.service-fly-detail-tab-order').html(result);
                    $('.service-fly-detail-button-order').attr('data-loaded', '1')

                }
            });
        }
    },
    ShowNeedToOrderTab: function () {
        $('.service-fly-detail-change-tab-button').removeClass('active')
        $('.service-fly-detail-button-need-to-order').addClass('active')
        $('.service-fly-detail-tab').hide()
        $('.service-fly-detail-tab-need-to-order').show()
        var group_fly = $('#service-fly-detail-data').attr('data-group-booking-id')
        var go_id = $('#service-fly-detail-data').attr('data-goserviceid').trim()
        if ($('.service-fly-detail-button-need-to-order').attr('data-loaded').trim() == '0') {
            $.ajax({
                url: "/SetService/FlyDetailBookingCode",
                type: "Post",
                data: {
                    order_id: $('#service-fly-detail-data').attr('data-order-id'),
                    group_fly: group_fly
                },
                success: function (result) {
                    $('.service-fly-detail-tab-need-to-order').html(result);
                    $('.service-fly-detail-button-need-to-order').attr('data-loaded', '1')
                    _global_function.RenderFileAttachment($('.attachment-sale'), go_id, _set_service_fly_detail.ServiceTypeSale, false, false)
                }
            });
        }
    },
    ShowOrderedTab: function () {
        $('.service-fly-detail-change-tab-button').removeClass('active')
        $('.service-fly-detail-button-ordered').addClass('active')
        $('.service-fly-detail-tab').hide()
        $('.service-fly-detail-tab-ordered').show()
        var group_fly = $('#service-fly-detail-data').attr('data-group-booking-id')
        var go_id = $('#service-fly-detail-data').attr('data-goserviceid').trim()
        if ($('.service-fly-detail-button-ordered').attr('data-loaded').trim() == '0') {
            $.ajax({
                url: "/SetService/FlyDetailBookingOrdered",
                type: "Post",
                data: {
                    order_id: $('#service-fly-detail-data').attr('data-order-id'),
                    group_fly: group_fly
                },
                success: function (result) {
                    $('.service-fly-detail-tab-ordered').html(result);
                    $('.service-fly-detail-button-ordered').attr('data-loaded', '1')
                    _global_function.RenderFileAttachment($('.attachment-operator'), go_id, _set_service_fly_detail.ServiceType, true, false, true)

                }
            });
        }
    
       
    },
    ShowPaymentTab: function () {
        $('.service-fly-detail-change-tab-button').removeClass('active')
        $('.service-fly-detail-button-payment').addClass('active')
        $('.service-fly-detail-tab').hide()
        $('.service-fly-detail-tab-payment').show()

        $.ajax({
            url: "/SetService/ListHotelBookingpayment",
            type: "Post",
            data: {
                HotelBookingID: $('#service-fly-detail-data').attr('data-goserviceid').trim(),
                serviceType: 3,
                supplierId: $('#suplier-detail').attr('data-suplier-id'),
                supplierName: $('#suplier-detail').attr('data-suplier-name'),
                orderId: $('#service-fly-detail-data').attr('data-order-id'),
                amount: $('#operator-order-amount').attr('data-amount'),
                HotelBookingstatus: $('#service-fly-detail-data').attr('data-status'),
                serviceCode: $('#service-fly-detail-data').attr('data-servicecode')
            },
            success: function (result) {
                $('.service-fly-detail-tab-payment').html(result);
            }
        });
    },
    ReloadPaymentTab: function () {
        $.ajax({
            url: "/SetService/ListHotelBookingpayment",
            type: "Post",
            data: {
                HotelBookingID: $('#service-fly-detail-data').attr('data-goserviceid').trim(),
                serviceType: 3,
                supplierId: $('#suplier-detail').attr('data-suplier-id'),
                supplierName: $('#suplier-detail').attr('data-suplier-name'),
                orderId: $('#service-fly-detail-data').attr('data-order-id'),
                amount: $('#operator-order-amount').attr('data-amount'),
                HotelBookingstatus: $('#service-fly-detail-data').attr('data-status'),
                serviceCode: $('#service-fly-detail-data').attr('data-servicecode')
            },
            success: function (result) {
                $('.service-fly-detail-tab-payment').html(result);
            }
        });
    },
    ShowServiceCodeTab: function () {
        $('.service-fly-detail-change-tab-button').removeClass('active')
        $('.service-fly-detail-button-servicecode').addClass('active')
        $('.service-fly-detail-tab').hide()
        $('.service-fly-detail-tab-servicecode').show()

        $.ajax({
            url: "/SetService/ListHotelBookingCode",
            type: "Post",
            data: {
                HotelBookingstatus: $('#service-fly-detail-data').attr('data-status'),
                HotelBookingID: $('#service-fly-detail-data').attr('data-goserviceid').trim(),
                Type: 3
            },
            success: function (result) {
                $('.service-fly-detail-tab-servicecode').html(result);
                $("body").on('click', ".service-code-send-email", function (ev, picker) {
                    _set_service_fly_detail.PopupSendEmail()

                });

            }
        });
    },
    ShowRefundTab: function () {
        $('.service-fly-detail-change-tab-button').removeClass('active')
        $('.service-fly-detail-button-refund').addClass('active')
        $('.service-fly-detail-tab').hide()
        $('.service-fly-detail-tab-refund').show()

        $.ajax({
            url: "/SetService/ListPaymentRequestByClient",
            type: "Post",
            data: {
                clientId: $('#service-fly-detail-data').attr('data-clientid'),
                bookingId: $('#service-fly-detail-data').attr('data-goserviceid'),
                amount: $('#saler-order-amount').attr('data-amount'),
                orderId: $('#service-fly-detail-data').attr('data-orderid'),
                bookingstatus: $('#service-fly-detail-data').attr('data-status'),
                serviceType:3
            },
            success: function (result) {
                $('.service-fly-detail-tab-refund').html(result);
              
            }
        });
    },
    MarketingTab: function () {
        $('.service-fly-detail-change-tab-button').removeClass('active')
        $('.service-fly-detail-button-marketing').addClass('active')
        $('.service-fly-detail-tab').hide()
        $('.service-fly-detail-tab-marketing').show()

        $.ajax({
            url: "/SetService/MarketingPaymentRequest",
            type: "Post",
            data: {
                clientId: $('#service-fly-detail-data').attr('data-clientid'),
                bookingId: $('#service-fly-detail-data').attr('data-goserviceid'),
                amount: $('#saler-order-amount').attr('data-amount'),
                orderId: $('#service-fly-detail-data').attr('data-orderid'),
                bookingstatus: $('#service-fly-detail-data').attr('data-status'),
                serviceType: 3,
                serviceId: $('#service-fly-detail-data').attr('data-goserviceid'),
                serviceCode: $('#service-other-detail-data').attr('data-servicecode'),
            },
            success: function (result) {
                $('.service-fly-detail-tab-marketing').html(result);

            }
        });
    },
    FlyDetailBookingCodeInitialization: function () {
        var total = 0;
        $('.set-service-fly-readonly-amount').each(function (index, item) {
            var element = $(item);
            if (element.html() != undefined && element.html().trim() != '') {
                total += parseFloat(element.html().replaceAll(',', ''))
            }

        });
        $('.set-service-fly-extrapackage-readonly-total-amount').html('' + _global_function.Comma(total)).change();

    },
    FlyDetailBookingOrderedInitialization: function () {
        _set_service_fly_detail.TabReCalucateTotalAmount()
        _set_service_fly_detail.TabServiceOrderedDynamicBind()
        $('.service-fly-ordered-suplier').each(function (index, item) {
            var element = $(item);
            _common_function_fly.Select2SupplierWithSearch(element)


        });

    },
    TabServiceOrderedDynamicBind: function () {
        $("body").on('keyup', ".service-fly-ordered-amount", function () {
            _set_service_fly_detail.TabReCalucateTotalAmount()

        });
       
        $("body").on('click', ".service-fly-ordered-delete-row", function () {
            var element = $(this)
            var row_element = element.closest('.service-fly-ordered-row')
            row_element.remove()
            _set_service_fly_detail.TabReCalucateTotalAmount()
            _set_service_fly_detail.ReIndexPackages()

        });
        $("body").on('click', ".service-fly-ordered-add-new", function () {
            _set_service_fly_detail.AddNewOrderedExtraPackage()
        });
        $("body").on('click', ".update-operator-order-amount", function () {
            $('.service-fly-ordered-package-name').removeAttr('disabled')
            $('.service-fly-ordered-package-name').removeClass('input-disabled-background')
            $('.service-fly-ordered-suplier').removeAttr('disabled')
            $('.service-fly-ordered-suplier').removeClass('input-disabled-background')
            $('.service-fly-status').removeAttr('disabled')
            $('.service-fly-status').removeClass('input-disabled-background')
            $('.service-fly-ordered-amount').removeAttr('disabled')
            $('.service-fly-ordered-amount').removeClass('input-disabled-background')
            $('.service-fly-ordered-note').removeAttr('disabled')
            $('.service-fly-ordered-note').removeClass('input-disabled-background')
            $('.service-fly-ordered-delete-row').show()
            $('.service-fly-ordered-add-new').show()
            $('.update-operator-order-amount-save').show()
            $('.update-operator-order-amount-cancel').show()
            $('.update-operator-order-amount').hide()
        });
        $("body").on('click', ".update-operator-order-amount-save", function () {

            _set_service_fly_detail.UpdateOperatorPrice();
            

        });
        $("body").on('click', ".update-operator-order-amount-cancel", function () {
            $('.service-fly-detail-button-ordered').attr('data-loaded','0')
            _set_service_fly_detail.ReloadOrderedTab();
            
        });
    },
    ReloadOrderedTab: function () {
        $("body").off('keyup', ".service-fly-extrapackage-amount", null);
        $("body").off('keyup', ".service-fly-ordered-delete-add-new", null);
        $("body").off('keyup', ".service-fly-ordered-delete-row", null);
        $("body").off('click', ".service-fly-ordered-add-new", null);
        $("body").off('click', ".update-operator-order-amount", null);
        $("body").off('click', ".update-operator-order-amount-save", null);
        $("body").off('click', ".update-operator-order-amount-cancel", null);
        _set_service_fly_detail.ShowOrderedTab()
        _set_service_fly_detail.ReloadPaymentTab()

    },
    ReIndexPackages: function () {
        var index = 0
        $('.service-fly-ordered-order').each(function (index, item) {
            var element = $(item);
            element.html(_global_function.Comma(++index))
        });
    },
    GetLastestSetServiceOrderedTable: function () {
        var last_order = 0;
        if (!$('.service-fly-ordered-order')[0]) return 0;
        //-- Get Lastest Order
        $('.service-fly-ordered-order').each(function (index, item) {
            last_order++
        });
        return last_order;
    },
    TabReCalucateTotalAmount: function () {
        var total = 0;
        $('.service-fly-ordered-amount').each(function (index, item) {
            var element = $(this);
            var element2 = element.closest('.service-fly-ordered-row').find('.service-fly-status');
            if (element.val() != undefined && element.val().trim() != '') {
                if (parseFloat(element2.val()) != 1)
                total += parseFloat(element.val().replaceAll(',', ''))
            }

        });
        $('.service-fly-ordered-total-amount').html('' + _global_function.Comma(total)).change();
    },
    AddNewOrderedExtraPackage: function (element) {
        var lastest_order = _set_service_fly_detail.GetLastestSetServiceOrderedTable()
        var code = _set_service_fly_detail.GenerateNewPackageOptionalCode()
        var html_append = _set_service_fly_html.html_ordered_new_packages.replaceAll('@(++index)', ('' + (lastest_order + 1))).replaceAll('@item.PackageName', code)
        $('.service-fly-ordered-summary-row').before(html_append)
        _set_service_fly_detail.ReIndexPackages()
        _common_function_fly.Select2SupplierWithSearch($('.service-fly-ordered-suplier'))
    },
    ChangeServiceOperator: function () {
        var operator_id = $('.service-fly-change-operator option:selected').val()
        if (operator_id == undefined || operator_id.trim() == '') {
            _msgalert.error('Vui lòng chọn điều hành viên mới')
            return
        }
        $.ajax({
            url: "/SetService/SummitFlyDetailChangeOperator",
            type: "Post",
            data: {
                group_booking_id: $('.form-operator-change').attr('data-group-booking'),
                user_id: operator_id
            },
            success: function (result) {
                if (result.status === 0) {
                    _msgalert.success(result.msg);
                    _set_service_fly_detail.PopupChangeOperatorClose()
                    setTimeout(function () {
                        window.location.reload();
                    }, 500)
                }
                else {
                    _msgalert.error(result.smg);

                }

            }
        });
    },
    UpdateOperatorPrice: function () {
        var obj_summit = []
        if (!$('.service-fly-ordered-row')[0]) {
            _msgalert.error('Vui lòng nhập ít nhất bản ghi dịch vụ tại bảng kê dịch vụ')
            return
        }
        var is_success = true
        $('.service-fly-ordered-row').each(function (index, item) {
            var element = $(item)
            var id = element.attr('data-id')
            var package_name = element.find('.service-fly-ordered-package-name').html()
            var suplier_id = element.find('.service-fly-ordered-suplier').find(':selected').val()
            var amount = element.find('.service-fly-ordered-amount').val().replaceAll(',', '')
            var status = element.find('.service-fly-status').val()
            if (isNaN(parseFloat(element.find('.service-fly-ordered-amount').val().replaceAll(',', '')))) {
                _msgalert.error('Tổng thành tiền cho nhà cung cấp phải là số')
                is_success = false
                return false
            }
            if (suplier_id == undefined || suplier_id.trim()=='') {
                _msgalert.error('Vui lòng nhập & chọn thông tin nhà cung cấp')
                is_success = false

                return false
            }
            var note = element.find('.service-fly-ordered-note').val()
            var go_id = $('.set-service-fly-packages-optional-tbody').attr('data-go-id')
            var item_push = {
                Id: id,
                BookingId: go_id,
                SuplierId: suplier_id,
                Amount: amount,
                Note: note,
                PackageName: package_name,
                Status: status
            }
            obj_summit.push(item_push)
        });
        if (!is_success) return;
        $.ajax({
            url: "/SetService/UpdateFlyOperatorOrderPrice",
            type: "post",
            data: { data: obj_summit },
            success: function (result) {
                if (result != undefined && result.status == 0) {
                    _msgalert.success(result.msg)
                    _set_service_fly_detail.ReloadOrderedTab();
                    $('#operator-order-amount').html(':  ' + _global_function.Comma(result.amount))
                    $('#operator-order-amount').attr('data-amount', result.amount)
                    $('#operator-total-profit').html(':  ' + _global_function.Comma(result.profit))

                    $('.service-fly-ordered-package-name').attr('disabled', 'disabled')
                    $('.service-fly-ordered-package-name').addClass('input-disabled-background')
                    $('.service-fly-ordered-suplier').attr('disabled', 'disabled')
                    $('.service-fly-ordered-suplier').addClass('input-disabled-background')
                    $('.service-fly-ordered-amount').attr('disabled', 'disabled')
                    $('.service-fly-ordered-amount').addClass('input-disabled-background')
                    $('.service-fly-ordered-note').attr('disabled', 'disabled')
                    $('.service-fly-ordered-note').addClass('input-disabled-background')
                    $('.service-fly-status').attr('disabled', 'disabled')
                    $('.service-fly-status').addClass('input-disabled-background')
                    $('.service-fly-ordered-delete-row').hide()
                    $('.service-fly-ordered-add-new').hide()
                    $('.update-operator-order-amount-save').hide()
                    $('.update-operator-order-amount-cancel').hide()
                    $('.update-operator-order-amount').show()
                    $('.service-fly-detail-button-ordered').attr('data-loaded', '0')
                    _set_service_fly_detail.ReloadOrderedTab();
                }
                else {
                    _msgalert.error(result.msg)

                }

            }
        });
    },
    PopupChangeOperator: function (operator_name) {
        
        if ($('#change_operator_popup').length) {
            $('#change_operator_popup').removeClass('show')
            setTimeout(function () {
                $('#change_operator_popup').remove();
            }, 300);

        }
        $.ajax({
            url: "/SetService/FlyDetailChangeOperator",
            type: "post",
            data: {
                operator_name: operator_name,
                group_booking_id: $('#service-fly-detail-data').attr('data-group-booking-id').trim()
            },
            success: function (result) {
                $('body').append(result);
                setTimeout(function () {
                    $('#change_operator_popup').addClass('show')
                }, 300);

            }
        });
    },
    PopupChangeOperatorInit: function () {
        _common_function_fly.UserSuggesstionMultiple($('.service-fly-change-operator'))
    },
    PopupChangeOperatorClose: function () {
        $('#change_operator_popup').removeClass('show')
        setTimeout(function () {
            $('#change_operator_popup').remove();
        }, 300);
    },
    GenerateNewPackageOptionalCode: function () {
        var code = ''
        var list_exists = []
        var count=0
        $('.service-fly-ordered-package-name').each(function (index, item) {
            var element = $(item)
            count++
            list_exists.push(element.text().trim())
        });
        var service_code = $('.set-service-fly-packages-optional-tbody').attr('data-code')
        var no_duplicate = true
        for (var i = (count + 1); i < 300; i++) {
            code = service_code + '-' + i
            no_duplicate = true
            $('.service-fly-ordered-package-name').each(function (index, item) {
                var element = $(item)
                if (element.text().trim() == code.trim()) {
                    no_duplicate = false
                    return false
                }
            });
            if (no_duplicate) break;
        }
        if (no_duplicate) return code
        else {
            return ''
        }
    },
    
}
var _common_function_fly = {

    OrderNoSuggesstion: function (element) {

        element.select2({
            theme: 'bootstrap4',
            placeholder: "Mã đơn hàng",
            minimumInputLength: 1,
            allowClear: true,
            tags: true,
            ajax: {
                url: "/SetService/OrderSuggestion",
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
                                text: item.orderno,
                                id: item.orderno,
                            }
                        })
                    };
                },
                createTag: function (params) {
                    let term = $.trim(params.term);
                    return {
                        id: term,
                        text: term,
                        newTag: true,
                    }
                },
                cache: true
            }
        });
    },

    ServiceCodeSuggesstion: function (element) {
        element.select2({
            theme: 'bootstrap4',
            placeholder: "Mã dịch vụ",
            minimumInputLength: 1,
            allowClear: true,
            ajax: {
                url: "/SetService/FlyBookingSuggestion",
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
                                text: item.servicecode,
                                id: item.servicecode,
                            }
                        })
                    };
                },

                createTag: function (params) {
                    let term = $.trim(params.term);
                    return {
                        id: term,
                        text: term,
                        newTag: true,
                    }
                },
                cache: true
            }
        });

    },
    UserSuggesstionMultiple: function (element) {
        var selected = element.find(':selected').val()
        var placeholder = element.attr('placeholder')
        $.ajax({
            url: "/Order/UserSuggestion",
            type: "post",
            data: { txt_search: "" },
            success: function (result) {
                if (result != undefined && result.data != undefined && result.data.length > 0) {
                    result.data.forEach(function (item) {
                        element.append(_set_service_fly_html.html_user_option.replaceAll('{user_id}', item.id).replace('{user_email}', item.email).replace('{user_name}', item.username).replace('{user_phone}', item.phone == undefined ? "" : ' - ' + item.phone))

                    });
                    element.val(selected).trigger('change');
                }
                else {
                    element.trigger('change');

                }
                element.select2({
                    placeholder: placeholder,
                    allowClear: true
                });
            }
        });
    },
    Select2FixedOptionWithAddNew: function (element) {
        element.select2({
            tags: true,

            createTag: function (params) {
                var term = $.trim(params.term);

                if (term === '') {
                    return null;
                }

                return {
                    id: term,
                    text: term,
                }
            }
        });
    },
    Select2WithFixedOptionAndNoSearch: function (element) {
        var placeholder = element.attr('placeholder')
        element.select2({
            placeholder: placeholder,
            allowClear: true,
            minimumResultsForSearch: Infinity
        });
    },
    SingleDateRangePicker: function (element) {

        var today = new Date();
        var yyyy = today.getFullYear();
        var yyyy_max = yyyy + 10;
        var current_date = element.val();
        var min_range = '01/01/2020';
        var max_range = '31/12/' + yyyy_max;

        element.daterangepicker({
            autoApply: true,
            autoUpdateInput: false,
            showDropdowns: true,
            drops: 'down',
            minDate: min_range,
            maxDate: max_range,
            locale: {
                format: 'DD/MM/YYYY',
                cancelLabel: 'Clear'

            }
        }, function (start, end, label) {


        });

        //.data('daterangepicker').setStartDate(current_date);
    },
    OnApplyStartDateTimeOfBookingRange: function (start_date_element, end_date_element) {
        var today = new Date();

        var min_range = _global_function.GetDayTextDateRangePicker(start_date_element.data('daterangepicker').startDate._d);
        var yyyy = today.getFullYear();
        var yyyy_max = yyyy + 10;
        var max_range = '31/12/' + yyyy_max;
        end_date_element.daterangepicker({
            autoApply: true,
            autoUpdateInput: false,
            showDropdowns: true,
            drops: 'down',
            minDate: min_range,
            maxDate: max_range,
            locale: {
                format: 'DD/MM/YYYY',
                cancelLabel: 'Clear'

            }
        }, function (start, end, label) {


        });
    },
   
    Select2SupplierWithSearch: function (element) {
        var selected = element.find(':selected').val()
        element.select2({
            theme: 'bootstrap4',
            placeholder: "Tên NCC ",
            hintText: "Nhập từ khóa tìm kiếm",
            searchingText: "Đang tìm kiếm...",
            ajax: {
                url: "/PaymentRequest/GetSuppliersSuggest",
                type: "post",
                dataType: 'json',
                delay: 250,
                data: function (params) {
                    var query = {
                        name: params.term,
                    }
                    return query;
                },
                processResults: function (response) {
                    return {
                        results: $.map(response, function (item) {
                            return {
                                text: item.id+' - '+item.fullname,
                                id: item.id,
                            }
                        })
                    };
                },
                cache: true
            }
        });
      /*  element.val(selected).change()*/
    },
    Select2BookingCode: function (element) {
        element.select2({
            theme: 'bootstrap4',
            placeholder: "Mã Code",

            allowClear: true,
            tags: true,
            ajax: {
                url: "/SetService/BookingCodeSuggestion",
                type: "post",
                dataType: 'json',
                delay: 250,
                data: function (params) {
                    var query = {
                        txt_search: params.term,
                        type: 3,
                    }
                    return query;
                },
                processResults: function (response) {
                    return {
                        results: $.map(response.data, function (item) {
                            return {
                                text: item.bookingcode,
                                id: item.bookingcode,
                            }
                        })
                    };
                },
                createTag: function (params) {
                    let term = $.trim(params.term);
                    return {
                        id: term,
                        text: term,
                        newTag: true,
                    }
                },
                cache: true
            }
        });
    },
  
   
}