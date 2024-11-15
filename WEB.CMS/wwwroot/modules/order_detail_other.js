var _order_detail_other = {
    ServiceType: 20,
    Initialization: function (order_id, booking_id) {
        $('#otherbooking-service').addClass('show')
        _order_detail_other.AddOtherServicePackage(order_id, booking_id)
        _order_detail_common.Select2WithFixedOptionAndSearch($('.add-service-other-select-service'))
        _order_detail_common.SingleDateTimePicker($('.service-other-from-date'))
        _order_detail_common.SingleDateTimePicker($('.service-other-to-date'))
        _order_detail_common.UserSuggesstion($('.add-service-other-main-staff'))
        _order_detail_common.FileAttachment(booking_id, _order_detail_other.ServiceType)
        _order_detail_other.DynamicBind()
    },
    DynamicBind: function () {
        $('body').on('keyup', '.service-other-packages-baseprice, .service-other-packages-quantity, .service-other-packages-saleprice', function () {
            var element = $(this)
            var row_element = element.closest('.service-other-packages-row')
            var table_element = element.closest('.service-other-packages-tbody')
            _order_detail_other.CalucateRowAmount(row_element)
            _order_detail_other.CalucateAmount(table_element)
            _order_detail_other.CalucateProfit(table_element)
        });
        $('body').on('apply.daterangepicker', '.service-other-from-date', function () {
            _order_detail_common.OnApplyStartDateOfBookingRangeDatetime($(this), $('.service-other-to-date'))
        });
       
        $('.service-other-note').keydown(function (e) {
            e.stopPropagation();
        });
    },
    RemoveDynamicBind: function () {
        $('body').off('keyup', '.service-other-packages-baseprice, .service-other-packages-quantity, .service-other-packages-profit', function () {
            
        });
        $('body').off('apply.daterangepicker', '.service-other-from-date', function () {
           
        });
        $('.service-other-note').keydown(null);
    },
    AddOtherServicePackage: function (order_id,booking_id) {
        if (booking_id != undefined && !isNaN(parseInt(booking_id))) {
            $.ajax({
                url: "AddOtherServicePackages",
                type: "post",
                data: { order_id: order_id, other_booking_id: booking_id },
                success: function (result) {
                    $('.service-other-packages').html(result)
                    
                }
            });
        }

    },
    DeleteOtherBookingpackages: function (element) {
        var row_element = element.closest('.service-other-packages-row')
        var table_element = element.closest('.service-other-packages-tbody')

        row_element.remove()

        _order_detail_other.CalucateProfit(table_element)
        _order_detail_other.CalucateAmount(table_element)
        _order_detail_other.ReIndexPackages(table_element)

    },
    Summit: function () {
        var service_code = $('#add-service-other-form-select').attr('data-service-code')
        var service_type = $('.add-service-other-select-service').find(':selected').val()
        var from_date_value = $('.service-other-from-date').data('daterangepicker').startDate._d
        var to_date_value  = $('.service-other-to-date').data('daterangepicker').startDate._d
        var from_date = _global_function.GetDayText($('.service-other-from-date').data('daterangepicker').startDate._d,true)
        var to_date = _global_function.GetDayText($('.service-other-to-date').data('daterangepicker').startDate._d,true)
        var operator_id = $('.add-service-other-main-staff').find(':selected').val()
        var note = $('.service-other-note').val()
        if (service_type == undefined || service_type == null || service_type.trim() =='') {
            _msgalert.error("Vui lòng chọn loại dịch vụ")
            return;
        }
        /*
        if (from_date_value >= to_date_value) {
            _msgalert.error("Ngày bắt đầu không được lớn hơn hoặc bằng ngày kết thúc")
            return;
        }*/
        if (operator_id == undefined || operator_id == null || operator_id.trim() == '') {
            _msgalert.error("Vui lòng chọn điều hành viên")
            return;
        }
        var pathname = window.location.pathname.split('/');

        var other_amount_element = $('#servicemanual-other-other-amount')
        var other_amount = other_amount_element.val() == undefined || isNaN(parseFloat(other_amount_element.val().replaceAll(',', ''))) ? 0 : parseFloat(other_amount_element.val().replaceAll(',', ''))

        var discount_element = $('#servicemanual-other-commission')
        var discount = discount_element.val() == undefined || isNaN(parseFloat(discount_element.val().replaceAll(',', ''))) ? 0 : parseFloat(discount_element.val().replaceAll(',', ''))

        var object_summit = {
            order_id: pathname[pathname.length - 1],
            id: $('#add-service-other-form-select').attr('data-id'),
            service_type: service_type,
            from_date: from_date,
            to_date: to_date,
            operator_id: operator_id,
            note: note,
            service_code:service_code,
            packages: [],
            others_amount: other_amount,
            commission: discount
        }
        var validate_failed = false
        $('.service-other-packages-row').each(function (index, item) {
            var extra_package_element = $(item);
            var package_name = extra_package_element.find('.service-other-packages-packagename').val();
            if ( package_name == undefined || package_name == null || package_name.trim() == '') {
                _msgalert.error("Nội dung tại dịch vụ thứ " + extra_package_element.find('.service-other-packages-order').text() + ' của Bảng kê dịch vụ không được bỏ trống')
                validate_failed = true
                return false;
            }

            var extra_package = {
                id: extra_package_element.attr('data-extra-package-id'),
                package_name: package_name,
                base_price: _global_function.GetAmountFromCurrencyInput(extra_package_element.find('.service-other-packages-baseprice')),
                quantity: _global_function.GetAmountFromCurrencyInput(extra_package_element.find('.service-other-packages-quantity')),
                amount:  _global_function.GetAmountFromCurrencyInput(extra_package_element.find('.service-other-packages-amount')),
                profit:  _global_function.GetAmountFromCurrencyInput(extra_package_element.find('.service-other-packages-profit')),
                sale_price: _global_function.GetAmountFromCurrencyInput(extra_package_element.find('.service-other-packages-saleprice')),
            }
            object_summit.packages.push(extra_package);
        });
        if (validate_failed) {
            return
        }
        var descriptiion = _order_detail_html.summit_confirmbox_create_other_service_description
        _msgconfirm.openDialog(_order_detail_html.summit_confirmbox_title, descriptiion, function () {
            $('.btn-summit-service-other').attr('disabled', 'disabled')
            $('.btn-summit-service-other').addClass('disabled')
            $.ajax({
                url: "SummitOtherServicePackages",
                type: "post",
                data: { data: object_summit},
                success: function (result) {
                    if (result != undefined && result.status == 0) {
                        _msgalert.success(result.msg);
                        _order_detail_other.Close();
                        _global_function.ConfirmFileUpload($('.attachment-file-block'),result.data)
                        setTimeout(function () {
                            window.location.reload();
                        }, 300);
                    }
                    else {
                        _msgalert.error(result.msg);
                        $('.btn-summit-service-other').removeAttr('disabled')
                        $('.btn-summit-service-other').removeClass('disabled')
                    }
                }
            });
        });
    },
    Close: function () {
        $('#otherbooking-service').removeClass('show')
        setTimeout(function () {
            $('#otherbooking-service').remove();
            _order_detail_create_service.StartScrollingBody();
            _order_detail_other.RemoveDynamicBind();
        }, 300);
    },
    AddOtherBookingpackages: function () {
        var table_element = $('.service-other-packages-tbody')
        var new_position = _order_detail_other.GetLastestPackagesNo() + 1;
        table_element.find('.service-other-packages-summary-row').before(_order_detail_html.html_service_other_new_packages.replaceAll('@(++index)', new_position))
    },
    GetLastestPackagesNo: function () {
        var total = 0;
        $('.service-other-packages-row').each(function (index, item) {
            total++;
        });
        return total
    },
    ReIndexPackages: function (table_element) {
        var total = 0;
        if (!table_element.find('.service-other-packages-row')[0]) return;
        table_element.find('.service-other-packages-row').each(function (index, item) {
            var row_element = $(this);
            total++;
            row_element.find('.service-other-packages-order').html('' + total)
        });
    },
    CalucateProfit: function (table_element) {
        var profit = 0;
        
        table_element.find('.service-other-packages-profit').each(function (index, item) {
            var element = $(this);
            var amount = !isNaN(parseFloat(element.val().replaceAll(',', ''))) ? parseFloat(element.val().replaceAll(',', '')) : 0
            profit += amount
        });
        var total_element = table_element.find('.service-other-packages-total-profit')
        total_element.html((profit >= 0 ? '' : '-') +_global_function.Comma(profit))

    },
    CalucateAmount: function (table_element) {
        var total_amount = 0;
      
        table_element.find('.service-other-packages-amount').each(function (index, item) {
            var element = $(this);
            var amount = !isNaN(parseFloat(element.val().replaceAll(',', ''))) ? parseFloat(element.val().replaceAll(',', '')) : 0
            total_amount += amount
        });
        table_element.find('.service-other-packages-total-amount').html((total_amount >= 0 ? '' : '-') +_global_function.Comma(total_amount))
    },
    CalucateRowAmount: function (row_element) {
        var element_base_price = row_element.find('.service-other-packages-baseprice')
        var element_quanity = row_element.find('.service-other-packages-quantity')
        var element_sale_price = row_element.find('.service-other-packages-saleprice')

        var base_price = !isNaN(parseFloat(element_base_price.val().replaceAll(',', ''))) && parseFloat(element_base_price.val().replaceAll(',', '')) > 0 ? parseFloat(element_base_price.val().replaceAll(',', '')) : 0
        var quanity = !isNaN(parseFloat(element_quanity.val().replaceAll(',', ''))) && parseFloat(element_quanity.val().replaceAll(',', '')) > 0 ? parseFloat(element_quanity.val().replaceAll(',', '')) : 0
        var sale_price = !isNaN(parseFloat(element_sale_price.val().replaceAll(',', ''))) && parseFloat(element_sale_price.val().replaceAll(',', '')) > 0 ? parseFloat(element_sale_price.val().replaceAll(',', '')) : 0

        var element_profit = row_element.find('.service-other-packages-profit')
        var element_amount = row_element.find('.service-other-packages-amount')

        var amount = sale_price * quanity;
        var price = base_price * quanity;
        var profit = amount - price;

        var element_amount = row_element.find('.service-other-packages-amount')
        var element_profit = row_element.find('.service-other-packages-profit')


        element_amount.val((amount >= 0 ? '' : '-')+ _global_function.Comma(amount.toFixed(0))).change()
        element_profit.val((profit >= 0 ? '' : '-') +_global_function.Comma(profit.toFixed(0))).change()
    },
}
