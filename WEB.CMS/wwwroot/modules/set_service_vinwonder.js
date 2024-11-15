var _set_service_vinwonder_html = {
    html_user_option: '<option value="{user_id}">{user_name} - {user_email}{user_phone}</option>',
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
    html_ordered_new_packages: '<tr class="service-vinwonder-ordered-row" data-id="0"> <td class="service-vinwonder-ordered-order">@(++index)</td> <td><p class="form-control service-vinwonder-ordered-package-name ">@item.PackageName</p></td> <td><select class="select2 service-vinwonder-ordered-suplier service-vinwonder-ordered-suplier-new" style="width:100% !important" > </select></td> <td><input type="text" class="form-control currency service-vinwonder-ordered-amount text-right"  value="0" /></td> <td><input type="text" class="form-control service-vinwonder-ordered-note" value="" /></td> <td> <a class="fa fa-trash-o service-vinwonder-ordered-delete-row-disabled service-vinwonder-ordered-delete-row" href="javascript:;"></a></td> </tr>',
    html_btn_vinwonder: '<a href="javascript:;" id="export-vinwonder-code" class="btn btn-default min ml-2 export-vinwonder-code">Xuất code vé VinWonder</a>',
    html_sendemail_btn: '<a href="javascript:;" id="send-email-vinwonder-code" class="btn btn-default min ml-2 send-email-vinwonder-code">Gửi email Vé Vinwonder</a>',
    html_loading_export_vinwonder: '<a class="loading_export_vinwonder" href="javascript:;"><img src="/images/icons/loading.gif" style="width: 30px;height: 30px; margin-left:15px;" class="img_loading_summit coll"><nw id="loading_exportvw" style="color:red;">Đang xuất vé dịch vụ, vui lòng không đóng cửa sổ này </nw> </a>',
    html_loading_export_vinwonder_text:'Đang xuất vé dịch vụ, vui lòng không đóng cửa sổ này'

}
var vinwonder_loading= {
    ServiceCode: false,
    export_success: false,
    dot_text : ''
}
var _set_service_vinwonder = {
    Initialization: function () {
        _common_function_vinwonder.SingleDateRangePicker($('.set-service-vinwonder-search-startdate'))
        _common_function_vinwonder.SingleDateRangePicker($('.set-service-vinwonder-search-enddate'))
        _common_function_vinwonder.SingleDateRangePicker($('.set-service-vinwonder-search-createddate'))

        _common_function_vinwonder.UserSuggesstionMultiple($('.set-service-vinwonder-search-saler'))
        _common_function_vinwonder.UserSuggesstionMultiple($('.set-service-vinwonder-search-main-staff'))
        _common_function_vinwonder.UserSuggesstionMultiple($('.set-service-vinwonder-search-usercreate'))

        _set_service_vinwonder.DynamicBindInput()
        _set_service_vinwonder.SearchData()
        setTimeout(function () {

            $('.set-service-vinwonder-search-status').select2({
                placeholder: "Tất cả trạng thái dịch vụ"
            })
        }, 500)
    },
    DynamicBindInput: function () {
        $("body").on('apply.daterangepicker', ".set-service-vinwonder-search-startdate", function (ev, picker) {
            _common_function_vinwonder.OnApplyStartDateTimeOfBookingRange($(this), $('.set-service-vinwonder-search-enddate'))

        });
        $("body").on('apply.daterangepicker', ".set-service-vinwonder-search-daterange", function (ev, picker) {
            $(this).val(picker.startDate.format('DD/MM/YYYY') + ' - ' + picker.endDate.format('DD/MM/YYYY'));
        });
        $("body").on('focusout', ".set-service-vinwonder-search-startdate", function (ev, picker) {
            if ($(this).val() == undefined || $(this).val().trim() == '') {
                _common_function_vinwonder.SingleDateRangePicker($('.set-service-vinwonder-search-enddate'))
            }
        });
        $("body").on('click', ".service-vinwonder-filter-by-status", function (ev, picker) {
            var element = $(this)

            $('.service-vinwonder-filter-by-status').removeClass('active')
            element.addClass('active')

            var status = [];
            if (element.attr('data-status').trim() != '-1') status = [element.attr('data-status')]
            $('.set-service-vinwonder-search-status').val(status).change();
            _set_service_vinwonder.SearchData()


        });



    },
    Search: function (input) {
        window.scrollTo(0, 0);
        $.ajax({
            url: "/SetService/vinwonderSearch",
            type: "Post",
            data: input,
            success: function (result) {
                $('#search_data_grid').html(result);
                _common_function_vinwonder.OrderNoSuggesstion($('.set-service-vinwonder-search-orderno'))
                _common_function_vinwonder.ServiceCodeSuggesstion($('.set-service-vinwonder-search-servicecode'))
                _common_function_vinwonder.Select2BookingCode($("#BookingCode"))
            }
        });
    },
    OnPaging: function (value) {
        if (value > 0) {
            var objSearch = _set_service_vinwonder.GetParam(value);
            _set_service_vinwonder.Search(objSearch);
        }
    },
    onSelectPageSize: function () {
        _set_service_vinwonder.SearchData();

    },
    SearchData: function () {
        var objSearch = _set_service_vinwonder.GetParam(1);
        _set_service_vinwonder.Search(objSearch);
    },
    GetParam: function (PageIndex) {
        var object_search_summit = {
            searchModel: {
                ServiceCode: $('.set-service-vinwonder-search-servicecode').find(':selected').val() == undefined || $('.set-service-vinwonder-search-servicecode').find(':selected').val().trim() == '' ? '' : $('.set-service-vinwonder-search-servicecode').find(':selected').val().trim(),
                OrderCode: $('.set-service-vinwonder-search-orderno').find(':selected').val() == undefined || $('.set-service-vinwonder-search-orderno').find(':selected').val().trim() == '' ? '' : $('.set-service-vinwonder-search-orderno').find(':selected').val().trim(),
            }

        }
        var status = '';
        $('.set-service-vinwonder-search-status').find(':selected').each(function (index, item) {
            var element = $(item);
            if (element.val().includes(',')) {
                status = '' + element.val()
                return false;
            }
            if (status == '') status = '' + element.val()
            else status = status + ',' + element.val()
        });
        object_search_summit.searchModel.StatusBooking = status
        object_search_summit.searchModel.BookingCode = $('#BookingCode').val()

        object_search_summit.searchModel.UserCreate = $('.set-service-vinwonder-search-usercreate').find(':selected').val() == undefined || $('.set-service-vinwonder-search-usercreate').find(':selected').val().trim() == '' ? null : $('.set-service-vinwonder-search-usercreate').find(':selected').val()
        object_search_summit.searchModel.SalerId = $('.set-service-vinwonder-search-saler').find(':selected').val() == undefined || $('.set-service-vinwonder-search-saler').find(':selected').val().trim() == '' ? null : $('.set-service-vinwonder-search-saler').find(':selected').val()
        object_search_summit.searchModel.OperatorId = $('.set-service-vinwonder-search-main-staff').find(':selected').val() == undefined || $('.set-service-vinwonder-search-main-staff').find(':selected').val().trim() == '' ? null : $('.set-service-vinwonder-search-main-staff').find(':selected').val()
        object_search_summit.searchModel.PageIndex = PageIndex
        object_search_summit.searchModel.pageSize = isNaN(parseInt($('#selectPaggingOptions').find(':selected').val())) || parseInt($('#selectPaggingOptions').find(':selected').val()) == undefined ? 30 : parseInt($('#selectPaggingOptions').find(':selected').val())
        var compare_from = _global_function.GetDayText($('.set-service-vinwonder-search-startdate').data('daterangepicker').startDate._d, true).trim().split(' ')[0]
        var compare_to = _global_function.GetDayText($('.set-service-vinwonder-search-startdate').data('daterangepicker').endDate._d, true).trim().split(' ')[0]

        if ($('.set-service-vinwonder-search-startdate').val() == undefined || $('.set-service-vinwonder-search-startdate').val().trim() == '') {
            object_search_summit.searchModel.StartDateFrom = undefined;
            object_search_summit.searchModel.StartDateTo = undefined;

        }
        else {
            object_search_summit.searchModel.StartDateFrom = _global_function.GetDayText($('.set-service-vinwonder-search-startdate').data('daterangepicker').startDate._d, true);
            object_search_summit.searchModel.StartDateTo = _global_function.GetDayText($('.set-service-vinwonder-search-startdate').data('daterangepicker').endDate._d, true);
        }
        compare_from = _global_function.GetDayText($('.set-service-vinwonder-search-enddate').data('daterangepicker').startDate._d, true).trim().split(' ')[0]
        compare_to = _global_function.GetDayText($('.set-service-vinwonder-search-enddate').data('daterangepicker').endDate._d, true).split(' ')[0]
        if ($('.set-service-vinwonder-search-enddate').val() == undefined || $('.set-service-vinwonder-search-enddate').val().trim() == '') {
            object_search_summit.searchModel.EndDateFrom = undefined;
            object_search_summit.searchModel.EndDateTo = undefined;
        }
        else {
            object_search_summit.searchModel.EndDateFrom = _global_function.GetDayText($('.set-service-vinwonder-search-enddate').data('daterangepicker').startDate._d, true);
            object_search_summit.searchModel.EndDateTo = _global_function.GetDayText($('.set-service-vinwonder-search-enddate').data('daterangepicker').endDate._d, true);
        }
        compare_from = _global_function.GetDayText($('.set-service-vinwonder-search-createddate').data('daterangepicker').startDate._d, true).trim().split(' ')[0]
        compare_to = _global_function.GetDayText($('.set-service-vinwonder-search-createddate').data('daterangepicker').endDate._d, true).split(' ')[0]
        if ($('.set-service-vinwonder-search-createddate').val() == undefined || $('.set-service-vinwonder-search-createddate').val().trim() == '') {
            object_search_summit.searchModel.CreateDateFrom = undefined;
            object_search_summit.searchModel.CreateDateTo = undefined;

        }
        else {
            object_search_summit.searchModel.CreateDateFrom = _global_function.GetDayText($('.set-service-vinwonder-search-createddate').data('daterangepicker').startDate._d, true);
            object_search_summit.searchModel.CreateDateTo = _global_function.GetDayText($('.set-service-vinwonder-search-createddate').data('daterangepicker').endDate._d, true);
        }

        return object_search_summit;
    },
    
    Export: function () {
        $('#btnExport').prop('disabled', true);
        $('#icon-export').removeClass('fa-file-excel-o');
        $('#icon-export').addClass('fa-spinner fa-pulse');
        var objSearch = _set_service_vinwonder.GetParam(1);
        objSearch.PageIndex = 1;
        this.searchModel = objSearch
        _global_function.AddLoading()
        $.ajax({
            url: "/SetService/vinwonderExportExcel",
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

var _set_service_vinwonder_detail = {
    ServiceType: 18,
    ServiceTypeSale: 6,
    Initialization: function () {
        _set_service_vinwonder_detail.DynamicBindInput()
        _set_service_vinwonder_detail.vinwonderDetailBookingOrderedInitialization()

        $('.service-vinwonder-detail-change-tab-button').click()
        switch ($('#service-vinwonder-detail-data').attr('data-status')) {
            case '1':
            case '2': {
                _set_service_vinwonder_detail.ShowNeedToOrderTab()

            } break;
            case '3':
            case '4': {
                _set_service_vinwonder_detail.ShowPaymentTab()

            } break;
            default: {
                _set_service_vinwonder_detail.ShowOrderTab()

            } break;
        }
    },
    DynamicBindInput: function () {
        $('body').on('click', '.service-vinwonder-detail-change-tab-button', function (event) {
            var element = $(this)
            switch (element.attr('data-tab-id')) {
                case '0': {
                    _set_service_vinwonder_detail.ShowOrderTab()

                } break;
                case '1': {
                    _set_service_vinwonder_detail.ShowNeedToOrderTab()

                } break;
                case '2': {
                    _set_service_vinwonder_detail.ShowOrderedTab()

                } break;
                case '3': {
                    _set_service_vinwonder_detail.ShowPaymentTab()

                } break;
                case '4': {
                    _set_service_vinwonder_detail.ShowServiceCodeTab()

                } break;
                case '5': {
                    _set_service_vinwonder_detail.ShowRefundTab()

                } break;
                case '6': {
                    _set_service_vinwonder_detail.MarketingTab()

                } break;
            }

        });
    },
    Back: function () {
        window.location.href = '/SetService/vinwonder';
    },
    PopupSendEmail: function () {
        let title = 'Gửi Code dịch vụ vé VinWonder';
        let url = '/SetService/SendEmail';
        let param = {
            id: $('#service-vinwonder-detail-data').attr('data-booking-id').trim(),
            Orderid: $('#service-vinwonder-detail-data').attr('data-order-id').trim(),
            type: 6,
        };
        _magnific.OpenSmallPopup(title, url, param);
    },
    ShowOrderTab: function () {
        $('.service-vinwonder-detail-change-tab-button').removeClass('active')
        $('.service-vinwonder-detail-button-order').addClass('active')
        $('.service-vinwonder-detail-tab').hide()
        $('.service-vinwonder-detail-tab-order').show()
        if ($('.service-vinwonder-detail-button-order').attr('data-loaded').trim() == '0') {
            $.ajax({
                url: "/SetService/ListOrder",
                type: "Post",
                data: { id: $('#service-vinwonder-detail-data').attr('data-order-id') },
                success: function (result) {
                    $('.service-vinwonder-detail-tab-order').html(result);
                    $('.service-vinwonder-detail-button-order').attr('data-loaded', '1')

                }
            });
        }
    },
    ShowNeedToOrderTab: function () {
        $('.service-vinwonder-detail-change-tab-button').removeClass('active')
        $('.service-vinwonder-detail-button-need-to-order').addClass('active')
        $('.service-vinwonder-detail-tab').hide()
        $('.service-vinwonder-detail-tab-need-to-order').show()
        if ($('.service-vinwonder-detail-button-need-to-order').attr('data-loaded').trim() == '0') {
            $.ajax({
                url: "/SetService/VinwonderDetailBookingSale",
                type: "Post",
                data: {
                    id: $('#service-vinwonder-detail-data').attr('data-booking-id')
                },
                success: function (result) {
                    $('.service-vinwonder-detail-tab-need-to-order').html(result);
                    $('.service-vinwonder-detail-button-need-to-order').attr('data-loaded', '1')
                    _global_function.RenderFileAttachment($('#service-vinwonder-detail-data').find('.attachment-sale'), $('#service-vinwonder-detail-data').attr('data-booking-id'), _set_service_vinwonder_detail.ServiceTypeSale, false, false)
                    $('.add-service-vinwonder-readonly-select-location').select2()
                    $(".add-service-vinwonder-readonly-select-location").prop("disabled", true);
                    $('.service-vinwonder-readonly').prop("disabled", true);
                    $('.service-vinwonder-readonly').addClass('input-disabled-background');
                    _global_function.RenderFileAttachment($('.attachment-sale'), $('#service-vinwonder-detail-data').attr('data-booking-id'), _set_service_vinwonder_detail.ServiceTypeSale, false, false)

                }
            });
        }
    },
    ShowOrderedTab: function () {
        $('.service-vinwonder-detail-change-tab-button').removeClass('active')
        $('.service-vinwonder-detail-button-ordered').addClass('active')
        $('.service-vinwonder-detail-tab').hide()
        $('.service-vinwonder-detail-tab-ordered').show()
        if ($('.service-vinwonder-detail-button-ordered').attr('data-loaded').trim() == '0') {
            $.ajax({
                url: "/SetService/VinwonderDetailBookingOperator",
                type: "Post",
                data: {
                    id: $('#service-vinwonder-detail-data').attr('data-booking-id')
                },
                success: function (result) {
                    $('.service-vinwonder-detail-tab-ordered').html(result);
                    $('.service-vinwonder-detail-button-ordered').attr('data-loaded', '1')
                    _global_function.RenderFileAttachment($('#service-vinwonder-detail-data').find('.attachment-operator'), $('#service-vinwonder-detail-data').attr('data-booking-id'), _set_service_vinwonder_detail.ServiceType, true, false, true)
                    $('.add-service-vinwonder-select-location').select2()
                    $(".add-service-vinwonder-select-location").prop("disabled", true);
                    _global_function.RenderFileAttachment($('.attachment-operator'), $('#service-vinwonder-detail-data').attr('data-booking-id'), _set_service_vinwonder_detail.ServiceType, true, false, true)

                }
            });
        }


    },
    ShowPaymentTab: function () {
        $('.service-vinwonder-detail-change-tab-button').removeClass('active')
        $('.service-vinwonder-detail-button-payment').addClass('active')
        $('.service-vinwonder-detail-tab').hide()
        $('.service-vinwonder-detail-tab-payment').show()

        $.ajax({
            url: "/SetService/ListHotelBookingpayment",
            type: "Post",
            data: {
                HotelBookingID: $('#service-vinwonder-detail-data').attr('data-booking-id'),
                serviceType:6,
                supplierId: $('#suplier-detail').attr('data-suplier-id'),
                supplierName: $('#suplier-detail').attr('data-suplier-name'),
                orderId: $('#service-vinwonder-detail-data').attr('data-order-id'),
                amount: $('#operator-order-amount').attr('data-amount'),
                HotelBookingstatus: $('#service-vinwonder-detail-data').attr('data-status'),
                serviceCode: $('#service-vinwonder-detail-data').attr('data-servicecode')

            },
            success: function (result) {
                $('.service-vinwonder-detail-tab-payment').html(result);
            }
        });
    },
    ReloadPaymentTab: function () {
        $.ajax({
            url: "/SetService/ListHotelBookingpayment",
            type: "Post",
            data: {
                HotelBookingID: $('#service-vinwonder-detail-data').attr('data-booking-id'),
                serviceType: 6,
                supplierId: $('#suplier-detail').attr('data-suplier-id'),
                supplierName: $('#suplier-detail').attr('data-suplier-name'),
                orderId: $('#service-vinwonder-detail-data').attr('data-order-id'),
                amount: $('#operator-order-amount').attr('data-amount'),
                HotelBookingstatus: $('#service-vinwonder-detail-data').attr('data-status'),
                serviceCode: $('#service-vinwonder-detail-data').attr('data-servicecode')

            },
            success: function (result) {
                $('.service-vinwonder-detail-tab-payment').html(result);
            }
        });
    },
    ShowServiceCodeTab: function () {
        $('.service-vinwonder-detail-change-tab-button').removeClass('active')
        $('.service-vinwonder-detail-button-servicecode').addClass('active')
        $('.service-vinwonder-detail-tab').hide()
        $('.service-vinwonder-detail-tab-servicecode').show()

        $.ajax({
            url: "/SetService/ListHotelBookingCode",
            type: "Post",
            data: {
                HotelBookingstatus: $('#service-vinwonder-detail-data').attr('data-status'),
                HotelBookingID: $('#service-vinwonder-detail-data').attr('data-booking-id'),
                Type: 6
            },
            success: function (result) {
                $('.service-vinwonder-detail-tab-servicecode').html(result);
                if ($('.service-vinwonder-detail-tab-servicecode').find('tbody').find('tr') && $('.service-vinwonder-detail-tab-servicecode').find('tbody').find('tr')[0]) {
                    $('#service-code').find('.service-code-send-email2').after(_set_service_vinwonder_html.html_sendemail_btn);
                }
                else {
                    $('#service-code').find('#add-service-code').before(_set_service_vinwonder_html.html_btn_vinwonder);

                }
               // $('#service-code').find('#add-service-code').remove()
                if (!vinwonder_loading.ServiceCode) {
                    $("body").on('click', ".service-code-send-email", function (ev, picker) {
                        _set_service_vinwonder_detail.PopupSendEmail()

                    });

                    $("body").on('click', ".export-vinwonder-code", function (ev, picker) {
                        _set_service_vinwonder_detail.ExportVinWonderTicket()
                    });
                    $("body").on('click', ".send-email-vinwonder-code", function (ev, picker) {
                        _set_service_vinwonder_detail.SendEmailVinWonderTicket($('#service-vinwonder-detail-data').attr('data-booking-id'))
                    });
                    vinwonder_loading.ServiceCode=true
                }
            }
        });
    },
    ShowRefundTab: function () {
        $('.service-vinwonder-detail-change-tab-button').removeClass('active')
        $('.service-vinwonder-detail-button-refund').addClass('active')
        $('.service-vinwonder-detail-tab').hide()
        $('.service-vinwonder-detail-tab-refund').show()

        $.ajax({
            url: "/SetService/ListPaymentRequestByClient",
            type: "Post",
            data: {
                clientId: $('#service-vinwonder-detail-data').attr('data-clientid'),
                bookingId: $('#service-vinwonder-detail-data').attr('data-booking-id'),
                amount: $('#service-vinwonder-detail-data').attr('data-amount'),
                orderId: $('#service-vinwonder-detail-data').attr('data-orderid'),
                bookingstatus: $('#service-vinwonder-detail-data').attr('data-status'),
                serviceType: 6
            },
            success: function (result) {
                $('.service-vinwonder-detail-tab-refund').html(result);
               
            }
        });
    },
    MarketingTab: function () {
        $('.service-vinwonder-detail-change-tab-button').removeClass('active')
        $('.service-vinwonder-detail-button-marketing').addClass('active')
        $('.service-vinwonder-detail-tab').hide()
        $('.service-vinwonder-detail-tab-marketing').show()

        $.ajax({
            url: "/SetService/MarketingPaymentRequest",
            type: "Post",
            data: {
                clientId: $('#service-vinwonder-detail-data').attr('data-clientid'),
                bookingId: $('#service-vinwonder-detail-data').attr('data-booking-id'),
                amount: $('#service-vinwonder-detail-data').attr('data-amount'),
                orderId: $('#service-vinwonder-detail-data').attr('data-orderid'),
                bookingstatus: $('#service-vinwonder-detail-data').attr('data-status'),
                serviceType: 6,
                serviceId: $('#service-vinwonder-detail-data').attr('data-booking-id'),
                serviceCode: $('#service-other-detail-data').attr('data-servicecode'),
            },
            success: function (result) {
                $('.service-vinwonder-detail-tab-marketing').html(result);

            }
        });
    },
    vinwonderDetailBookingOrderedInitialization: function () {
        _set_service_vinwonder_detail.TabReCalucateTotalAmount()
        _set_service_vinwonder_detail.TabServiceOrderedDynamicBind()
        _common_function_vinwonder.Select2SupplierWithSearch($('.service-vinwonder-ordered-suplier'))

    },
    TabServiceOrderedDynamicBind: function () {
        $("body").on('keyup', ".service-vinwonder-packages-baseprice-operator", function () {
            var element=$(this)
            _set_service_vinwonder_detail.CalucateRowAmount(element)
            _set_service_vinwonder_detail.CalucateRowProfit(element)
            _set_service_vinwonder_detail.TabReCalucateTotalAmount()
            _set_service_vinwonder_detail.TabReCalucateProfit()
            _set_service_vinwonder_detail.TabReCalucateUnitPrice()

        });
        $("body").on('click', ".update-operator-order-amount", function () {
            $('.service-vinwonder-packages-baseprice-operator').removeAttr('disabled')
            $('.service-vinwonder-packages-baseprice-operator').removeClass('input-disabled-background')
            $('.update-operator-order-amount-save').show()
            $('.update-operator-order-amount-cancel').show()
            $('.update-operator-order-amount').hide()
        });
        $("body").on('click', ".update-operator-order-amount-save", function () {
            _set_service_vinwonder_detail.UpdateOperatorPrice();

        });
        $("body").on('click', ".update-operator-order-amount-cancel", function () {
            $('.service-vinwonder-detail-button-ordered').attr('data-loaded', '0')
            _set_service_vinwonder_detail.ReloadOrderedTab();

        });
    },
    ReloadOrderedTab: function () {
        $("body").off('keyup', ".service-vinwonder-extrapackage-amount", function () {

        });
        $("body").off('keyup', ".service-vinwonder-ordered-delete-add-new", function () {

        });
        $("body").off('keyup', ".service-vinwonder-ordered-delete-row", function () {


        });
        $("body").off('click', ".service-vinwonder-ordered-add-new", function () {

        });
        $("body").off('click', ".update-operator-order-amount", function () {


        });
        $("body").off('click', ".update-operator-order-amount-save", function () {


        });
        $("body").off('click', ".update-operator-order-amount-cancel", function () {


        });
        _set_service_vinwonder_detail.ShowOrderedTab()
        _set_service_vinwonder_detail.ReloadPaymentTab()

    },
    ExportVinWonderTicket: function () {
        var title = 'Xác nhận xuất vé tự động Vinwonder'
        var description = 'Đơn hàng tự động Vinwonder sẽ được xuất vé tự động, bạn có chắc chắn không?'

        _set_service_vinwonder_detail.LoopDisplayLoading()

        _msgconfirm.openDialog(title, description, function () {
            $('.export-vinwonder-code').after(_set_service_vinwonder_html.html_loading_export_vinwonder)
            $('#add-service-code').hide()
            $('.service-code-send-email2').hide()
            $('.service-code-send-email').hide()
            $('#export-vinwonder-code').hide()
            $('#send-email-vinwonder-code').hide()
            $.ajax({
                url: "/SetService/ExportVinWonderTicket",
                type: "post",
                data: {
                    booking_id: $('#service-vinwonder-detail-data').attr('data-booking-id')
                },
                success: function (result) {
                    if (result.status === 0) {
                        _msgalert.success(result.msg);
                        setTimeout(function () {
                            _set_service_vinwonder_detail.ShowServiceCodeTab()
                        }, 1000)
                    }
                    else {
                        _msgalert.error(result.msg);
                    }
                    vinwonder_loading.export_success = true
                    $('.loading_export_vinwonder').remove()
                    $('#add-service-code').show()
                    $('.service-code-send-email2').show()
                    $('.service-code-send-email').show()
                    $('#export-vinwonder-code').show()
                    $('#send-email-vinwonder-code').show()
                }
            });
        })
       
    },
    SendEmailVinWonderTicket: function (booking_id) {
        /*
        let title = 'Gửi Email vé VinWonder';
        let url = '/SetService/EmailVinWonderTicket';
        let param = {
            booking_id: booking_id
        };
        _magnific.OpenSmallPopup(title, url, param);
        */
        $.ajax({
            url: "/SetService/EmailVinWonderTicket",
            type: "post",
            data: {
                booking_id: booking_id
            },
            success: function (result) {
                $('body').append(result)
                _set_service_vinwonder_email.Initialization()
            }
        });
    },
    LoopDisplayLoading: function () {     
        setTimeout(function () { 
            vinwonder_loading.dot_text = vinwonder_loading.dot_text+ '.'
            $('#loading_exportvw').html(_set_service_vinwonder_html.html_loading_export_vinwonder_text + vinwonder_loading.dot_text)
            if (vinwonder_loading.dot_text.trim() == '....') vinwonder_loading.dot_text = ''
            if (!vinwonder_loading.export_success) _set_service_vinwonder_detail.LoopDisplayLoading()
        }, 1000)
    },
    CalucateRowAmount: function (element) {
        var row_element = element.closest('.service-vinwonder-packages-row')
        var base_price_unit = _global_function.GetAmountFromCurrencyInput(row_element.find('.service-vinwonder-packages-baseprice-operator'))
        var quanity = _global_function.GetAmountFromCurrencyInput(row_element.find('.service-vinwonder-packages-quantity'))

        var base_price_unit_value = isNaN(base_price_unit) ? 0 : parseFloat(base_price_unit)
        var quanity_value = isNaN(quanity) ? 0 : parseFloat(quanity)

        var unit_price = base_price_unit_value * quanity_value
        row_element.find('.service-vinwonder-packages-unit-price').val(_global_function.Comma(unit_price))
    },
    CalucateRowProfit: function (element) {
        var row_element = element.closest('.service-vinwonder-packages-row')
        var base_price_unit = _global_function.GetAmountFromCurrencyInput(row_element.find('.service-vinwonder-packages-baseprice-operator'))
        var amount = _global_function.GetAmountFromCurrencyInput(row_element.find('.service-vinwonder-packages-amount'))
        var quanity = _global_function.GetAmountFromCurrencyInput(row_element.find('.service-vinwonder-packages-quantity'))

        var base_price_unit_value = isNaN(base_price_unit) ? 0 : parseFloat(base_price_unit)
        var amount_value = isNaN(amount) ? 0 : parseFloat(amount)
        var quanity_value = isNaN(quanity) ? 0 : parseFloat(quanity)
        var unit_price = base_price_unit_value * quanity_value
        var profit = amount_value - unit_price
        row_element.find('.service-vinwonder-packages-profit').val(_global_function.Comma(profit))
    },
    TabReCalucateTotalAmount: function () {
        var total = 0;
        $('.service-vinwonder-packages-amount').each(function (index, item) {
            var element = $(this);
            if (element.val() != undefined && element.val().trim() != '') {
                total += parseFloat(element.val().replaceAll(',', ''))
            }

        });
        $('.service-vinwonder-packages-total-amount').html('' + _global_function.Comma(total)).change();
    },
    TabReCalucateUnitPrice: function () {
        var total = 0;
        $('.service-vinwonder-packages-unit-price').each(function (index, item) {
            var element = $(this);
            if (element.val() != undefined && element.val().trim() != '') {
                total += parseFloat(element.val().replaceAll(',', ''))
            }

        });
        $('.service-vinwonder-packages-total-total-unit-price').html('' + _global_function.Comma(total)).change();
    },
    TabReCalucateProfit: function () {
        var total = 0;
        $('.service-vinwonder-packages-profit').each(function (index, item) {
            var element = $(this);
            if (element.val() != undefined && element.val().trim() != '') {
                total += parseFloat(element.val().replaceAll(',', ''))
            }

        });
        $('.service-vinwonder-packages-total-profit').html('' + _global_function.Comma(total)).change();
    },
    PopupChangeOperator: function (operator_name) {
        if ($('#change_operator_popup').length) {
            $('#change_operator_popup').removeClass('show')
            setTimeout(function () {
                $('#change_operator_popup').remove();
            }, 300);

        }
        $.ajax({
            url: "/SetService/VinWonderDetailBookingChangeOperator",
            type: "post",
            data: {
                operator_name: operator_name,
                booking_id: $('#service-vinwonder-detail-data').attr('data-booking-id')
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
        _set_service_vinwonder_detail.UserSuggesstionMultiple($('.service-vinwonder-change-operator'))
    },
    PopupChangeOperatorClose: function () {
        $('#change_operator_popup').removeClass('show')
        setTimeout(function () {
            $('#change_operator_popup').remove();
        }, 300);
    },
    ChangeServiceOperator: function () {
        var operator_id = $('.service-vinwonder-change-operator option:selected').val()
        if (operator_id == undefined || operator_id.trim() == '') {
            _msgalert.error('Vui lòng chọn điều hành viên mới')
            return
        }
        $.ajax({
            url: "/SetService/SummitVinWonderDetailChangeOperator",
            type: "Post",
            data: {
                booking_id: $('.form-operator-change').attr('data-id'),
                user_id: operator_id
            },
            success: function (result) {
                if (result.status === 0) {
                    _msgalert.success(result.msg);
                    _set_service_vinwonder_detail.PopupChangeOperatorClose()
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
        var is_success = true
        $('.service-vinwonder-packages-row').each(function (index, item) {
            var extra_package_element = $(item);
            var base_price_unit = _global_function.GetAmountFromCurrencyInput(extra_package_element.find('.service-vinwonder-packages-baseprice-operator'))
            var unit_price = _global_function.GetAmountFromCurrencyInput(extra_package_element.find('.service-vinwonder-packages-unit-price'))
            if (base_price_unit == null || base_price_unit == undefined || isNaN(parseFloat(base_price_unit)) || unit_price == null || unit_price == undefined || isNaN(parseFloat(unit_price))) {
                _msgalert.error('Vui lòng nhập giá nhập tại bản bảng kê dịch vụ dòng thứ ' + extra_package_element.find('.service-vinwonder-packages-order').html())
            }
            var extra_package = {
                Id: extra_package_element.attr('data-extra-package-id'),
                BookingId: $('#service-vinwonder-detail-data').attr('data-booking-id'),
                UnitPrice: _global_function.GetAmountFromCurrencyInput(extra_package_element.find('.service-vinwonder-packages-unit-price')),
            }
            obj_summit.push(extra_package);
        });
        if (!is_success) return;
        $.ajax({
            url: "/SetService/UpdateVinWonderOperatorOrderPrice",
            type: "post",
            data: { data: obj_summit },
            success: function (result) {
                if (result != undefined && result.status == 0) {
                    _msgalert.success(result.msg)
                    _set_service_vinwonder_detail.ReloadOrderedTab();

                    $('#operator-order-amount').html(':  ' + _global_function.Comma(result.amount))
                    $('#operator-order-amount').attr('data-amount', result.amount)
                    $('#operator-total-profit').html(':  ' + _global_function.Comma(result.profit))


                    $('.service-vinwonder-packages-baseprice-operator').attr('disabled', 'disabled')
                    $('.service-vinwonder-packages-baseprice-operator').addClass('input-disabled-background')

                    $('.update-operator-order-amount-save').hide()
                    $('.update-operator-order-amount-cancel').hide()
                    $('.update-operator-order-amount').show()

                }
                else {
                    _msgalert.error(result.msg)

                }

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
                        element.append(_set_service_vinwonder_html.html_user_option.replaceAll('{user_id}', item.id).replace('{user_email}', item.email).replace('{user_name}', item.username).replace('{user_phone}', item.phone == undefined ? "" : ' - ' + item.phone))

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
    GrantOrderPermission: function (element) {
        element.attr('disabled', true)
        var booking_id = $('#service-vinwonder-detail-data').attr('data-booking-id')
        var service_code = $('#service-vinwonder-detail-data').attr('data-servicecode')
        _msgconfirm.openDialog(_set_service_vinwonder_html.confirm_box_title_grant_order, _set_service_vinwonder_html.confirm_box_description_grant_order.replace('{Mã dịch vụ}', service_code), function () {
            $.ajax({
                url: "/SetService/GrantvinwonderServiceOrderPermission",
                type: "post",
                data: { booking_id: booking_id },
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
        var booking_id = $('#service-vinwonder-detail-data').attr('data-booking-id')
        var service_code = $('#service-vinwonder-detail-data').attr('data-servicecode')
        _msgconfirm.openDialog(_set_service_vinwonder_html.confirm_box_title_send_service_code, _set_service_vinwonder_html.confirm_box_description_send_service_code.replace('{Mã dịch vụ}', service_code), function () {
            $.ajax({
                url: "/SetService/vinwonderServiceSendServiceCode",
                type: "post",
                data: { booking_id: booking_id },
                success: function (result) {
                    if (result != undefined && result.status == 0) {
                        _msgalert.success(result.msg);
                        setTimeout(function () { window.location.reload() }, 500)

                    }
                    else {
                        _msgalert.error(result.msg);
                    }
                    element.removeAttr('disabled');
                }
            });
        });
    },
    ChangeToPayment: function (element) {
        element.attr('disabled', true)
        var booking_id = $('#service-vinwonder-detail-data').attr('data-booking-id')
        var service_code = $('#service-vinwonder-detail-data').attr('data-servicecode')
        _msgconfirm.openDialog(_set_service_vinwonder_html.confirm_box_title_change_to_payment, _set_service_vinwonder_html.confirm_box_description_change_to_payment.replace('{Mã dịch vụ}', service_code), function () {
            $.ajax({
                url: "/SetService/vinwonderServiceChangeToConfirmPaymentStatus",
                type: "post",
                data: { booking_id: booking_id },
                success: function (result) {
                    if (result != undefined && result.status == 0) {
                        _msgalert.success(result.msg);
                        setTimeout(function () { window.location.reload() }, 500)

                    }
                    else {
                        _msgalert.error(result.msg);
                    }
                    element.removeAttr('disabled');
                }
            });
        });
    },
    ReloadServiceCodeTab: function () {
        _set_service_vinwonder_detail.ShowServiceCodeTab();
    }
}
var _common_function_vinwonder = {


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
                url: "/SetService/vinwonderBookingSuggestion",
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
                        element.append(_set_service_vinwonder_html.html_user_option.replaceAll('{user_id}', item.id).replace('{user_email}', item.email).replace('{user_name}', item.username).replace('{user_phone}', item.phone == undefined ? "" : ' - ' + item.phone))

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
            minimumInputLength: 0,
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
                                text: item.id + ' - ' + item.fullname,
                                id: item.id,
                            }
                        })
                    };
                },
                cache: true
            }
        });
        element.val(selected).change()
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
                        type: 6,
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