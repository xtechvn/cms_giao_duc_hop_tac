let isPickerPayment = false;
var listOrderByClient = []
var isEditView = false
$().ready(function () {
    //setTimeout(function () {
    //    $('input[name="datetimePayment"]').daterangepicker({
    //        autoUpdateInput: true,
    //        autoApply: true,
    //        showDropdowns: true,
    //        drops: 'down',
    //        locale: {
    //            format: 'DD/MM/YYYY'
    //        }
    //    });
    //    var now = new Date()
    //    var fromDate = _global_function.GetFirstDayInPreviousMonth(new Date())
    //    var toDate = _global_function.GetLastDayOfPreviousMonth(now.getFullYear(), now.getMonth())
    //    var fromCreateDateStr = fromDate.getDate() + "/" + (fromDate.getMonth() + 1) + "/" + fromDate.getFullYear()
    //    var toCreateDateStr = toDate.getDate() + "/" + (toDate.getMonth() + 1) + "/" + toDate.getFullYear()
    //    $('input[name="datetimePayment"]').data('daterangepicker').setStartDate(fromCreateDateStr);
    //    $('input[name="datetimePayment"]').data('daterangepicker').setEndDate(toCreateDateStr);
    //    isPickerPayment = true
    //}, 1000)
})
var _add_debt_statistic = {
    Initialization: function (isEditView = false) {
        $("#client-select").select2({
            theme: 'bootstrap4',
            placeholder: "Tên KH, Điện Thoại, Email",
            hintText: "Nhập từ khóa tìm kiếm",
            searchingText: "Đang tìm kiếm...",
            maximumSelectionLength: 1,
            ajax: {
                url: "/Contract/ClientSuggestion",
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
                                text: item.clientname + ' - ' + item.email + ' - ' + item.phone,
                                id: item.id,
                            }
                        })
                    };
                },
                cache: true
            }
        });
        $('input').attr('autocomplete', 'off');
        if (!isEditView) {
            $().ready(function () {
                setTimeout(function () {
                    $('input[name="datetimePayment"]').daterangepicker({
                        autoUpdateInput: true,
                        autoApply: true,
                        showDropdowns: true,
                        drops: 'down',
                        locale: {
                            format: 'DD/MM/YYYY'
                        }
                    });
                    var now = new Date()
                    var fromDate = _global_function.GetFirstDayInPreviousMonth(new Date())
                    var toDate = _global_function.GetLastDayOfPreviousMonth(now.getFullYear(), now.getMonth())
                    var fromCreateDateStr = fromDate.getDate() + "/" + (fromDate.getMonth() + 1) + "/" + fromDate.getFullYear()
                    var toCreateDateStr = toDate.getDate() + "/" + (toDate.getMonth() + 1) + "/" + toDate.getFullYear()
                    $('input[name="datetimePayment"]').data('daterangepicker').setStartDate(fromCreateDateStr);
                    $('input[name="datetimePayment"]').data('daterangepicker').setEndDate(toCreateDateStr);
                    isPickerPayment = true
                }, 1000)
            })
        }
    },
    FormatNumber: function () {
        var amount = $('#amount').val()
        $('#amount').val(amount.replaceAll(',', '.').replaceAll(',', ''))
        var n = parseFloat($('#amount').val().replace(/\D/g, ''), 10);
        $('#amount').val(isNaN(n) === true ? '' : n.toLocaleString().replaceAll('.', ','));
    },
    FormatNumberStr: function (amount) {
        var n = parseFloat(amount, 10);
        return isNaN(n) === true ? '' : n.toLocaleString().replaceAll('.', ',')
    },
    FormatNumberOrder: function (i) {
        var amount = $('#amount_order_' + i).val()
        $('#amount_order_' + i).val(amount.replaceAll(',', '.').replaceAll(',', ''))
        var n = parseFloat($('#amount_order_' + i).val().replace(/\D/g, ''), 10);
        $('#amount_order_' + i).val(isNaN(n) === true ? '' : n.toLocaleString().replaceAll('.', ','));
    },
    GetOrderListByClientId: function () {
        if (($('#filter_date_payment').data('daterangepicker') === undefined ||
            $('#filter_date_payment').data('daterangepicker') == null) && !isPickerPayment) {
            return;
        }
        var fromDateFromStr = $('#filter_date_payment').data('daterangepicker').startDate._d.toLocaleDateString("en-GB");
        var toDateToStr = ($('#filter_date_payment').data('daterangepicker').endDate._d.toLocaleDateString("en-GB"))
        _global_function.AddLoading()
        var obj = {
            'clientId': parseInt(($('#client-select').val())),
            'fromDateStr': fromDateFromStr,
            'toDateStr': toDateToStr,
            'requestId': $('#requestId').val()
        }
        $.ajax({
            url: "/DebtStatistic/GetOrderListByClientId",
            type: "Post",
            data: obj,
            success: function (result) {
                _global_function.RemoveLoading()
                listOrderByClient = result.data
                var totalAmount = 0
                $("#body_order_list").empty();
                for (var i = 0; i < result.data.length; i++) {
                    $('#order-relate-table').find('tbody').append(
                        "<tr id='order_" + i + "'>" +
                        "<td>" +
                        "<label class='check-list number'>" +
                        " <input type='checkbox' id='order_ckb_" + i + "' name='order_ckb' onclick='_add_debt_statistic.OnCheckBox(" + i + ");_add_debt_statistic.AddToListDetail(" + i + ")'>" +
                        " <span class='checkmark'></span>" + (i + 1) +
                        "  </label>"
                        + "</td>" +
                        "<td>" +
                        " <a class='blue' href='/Order/Orderdetails?id=" + result.data[i].orderId + "'> " + result.data[i].orderNo + " </a>"
                        + "</td>" +
                        "<td>" + result.data[i].createDateStr + "</td>" +
                        "<td class='text-right'>" + _add_debt_statistic.FormatNumberStr(result.data[i].amount) + "</td>" +
                        "<td>" + result.data[i].operatorIdName + "</td>" +
                        "</tr>"
                    );
                    totalAmount += result.data[i].amount
                    if (!isEditView) {
                        result.data[i].isChecked = true
                    }
                    if (result.data[i].isChecked) {
                        let index = i
                        setTimeout(function () {
                            $('#order_ckb_' + index).prop('checked', true)
                        }, 800)
                    }
                }
                $('#order-relate-table').find('tbody').append(
                    "<tr style='font-weight:bold !important;'>" +
                    "<td class='text-right' colspan='3'> Tổng </td>" +
                    "<td class='text-right'>" + _add_debt_statistic.FormatNumberStr(totalAmount) + "</td>" +
                    "<td class='text-right'></td>" +
                    "</tr>"
                );
            }
        });
    },
    Validate: function () {
        _add_debt_statistic.ClearError()

        let result = true

        if (($('#client-select').val() == undefined || $('#client-select').val() == null || $('#client-select').val() == '')) {
            _add_debt_statistic.DisplayError('validate-client-select', 'Vui lòng chọn khách hàng')
            result = false;
        }

        if (($('#filter_date_payment').data('daterangepicker') === undefined ||
            $('#filter_date_payment').data('daterangepicker') == null) && !isPickerPayment) {
            _add_debt_statistic.DisplayError('validate-paymentDate', 'Vui lòng chọn thời gian')
            result = false;
        }

        if ($('#filter_date_payment').data('daterangepicker') !== undefined &&
            $('#filter_date_payment').data('daterangepicker') != null && isPickerCreate_payment) {
            var fromDateFromStr = $('#filter_date_payment').data('daterangepicker').startDate._d.toLocaleDateString("en-GB");
            var toDateToStr = ($('#filter_date_payment').data('daterangepicker').endDate._d.toLocaleDateString("en-GB")).split('/')
            var toDate = new Date(toDateToStr[0], toDateToStr[1] - 1, toDateToStr[2]);
            var now = new Date()
            now.setHours(0, 0, 0)
            toDate.setHours(0, 0, 0)
            if (toDate > now) {
                _add_debt_statistic.DisplayError('validate-paymentDate', 'Vui lòng chọn thời gian đến không được lớn hơn ngày hiện tại')
                result = false;
            }
        }

        if ($('#content').val() == undefined || $('#content').val() == null || $('#content').val() == '') {
            _add_debt_statistic.DisplayError('validate-content', 'Vui lòng nhập nội dung')
            result = false;
        }
        if (($('#content').val()).length > 500) {
            _add_debt_statistic.DisplayError('validate-content', 'Vui lòng nhập nội dung không quá 500 kí tự')
            result = false;
        }
        if ($('#description').val() !== undefined && $('#description').val() !== null
            && $('#description').val() !== '' && ($('#description').val()).length > 3000) {
            _add_debt_statistic.DisplayError('validate-description', 'Vui lòng nhập ghi chú không quá 3000 kí tự')
            result = false;
        }
        if (!result) return false

        var flag = false
        for (var i = 0; i < listOrderByClient.length; i++) {
            var checked = $('#order_ckb_' + i).is(":checked")
            if (checked) {
                flag = true
            }
        }
        if (!flag) {
            _msgalert.error('Vui lòng tích chọn đơn hàng');
            return false;
        }
        return result
    },
    AddRequest: function (isSend = 0) {
        let validate = _add_debt_statistic.Validate()
        if (!validate) {
            $('.btn-send-payment-request').removeAttr('disabled')
            return;
        }
        let orderIds = []
        var totalAmount = 0
        for (var i = 0; i < listOrderByClient.length; i++) {
            var checked = $('#order_ckb_' + i).is(":checked")
            if (checked) {
                orderIds.push(listOrderByClient[i].orderId)
                totalAmount += listOrderByClient[i].amount
            }
        }
        var fromDateFromStr = $('#filter_date_payment').data('daterangepicker').startDate._d.toLocaleDateString("en-GB");
        var toDateToStr = ($('#filter_date_payment').data('daterangepicker').endDate._d.toLocaleDateString("en-GB"))
        let obj = {
            'description': $('#description').val(),
            'amount': totalAmount,
            'fromDateStr': fromDateFromStr,
            'toDateStr': toDateToStr,
            'isSend': isSend,
            'note': $('#content').val(),
            'currency': $('#currency').val(),
            'clientId': parseInt(($('#client-select').val())),
            'orderIds': orderIds.toString(),
        }
        _global_function.AddLoading()
        $.ajax({
            url: "/DebtStatistic/AddJson",
            type: "Post",
            data: obj,
            success: function (result) {
                _global_function.RemoveLoading()
                if (result.isSuccess) {
                    _msgalert.success(result.message);
                    $.magnificPopup.close();
                    setTimeout(function () { window.location.reload() }, 1000)
                } else {
                    _msgalert.error(result.message);
                }

            }
        });
    },
    OnChooseRequestTypeEdit: function (client_id, fromDate, toDate) {
        isEditView = true
        $('input[name="datetimePayment"]').daterangepicker({
            autoUpdateInput: true,
            autoApply: true,
            showDropdowns: true,
            drops: 'down',
            locale: {
                format: 'DD/MM/YYYY'
            }
        });
        $('input[name="datetimePayment"]').data('daterangepicker').setStartDate(fromDate);
        $('input[name="datetimePayment"]').data('daterangepicker').setEndDate(toDate);
        isPickerPayment = true
        if ((client_id == undefined || client_id == null || client_id == 0 || client_id == '')) {
            return
        }
        isEdit = true
        if (client_id !== undefined && client_id !== null && client_id !== 0 && client_id !== '') {
            setTimeout(function () {
                var newOption = new Option($('#client_name_hide').val(), client_id, true, true);
                $('#client-select').append(newOption).trigger('change');
            }, 500)
        }
        this.GetOrderListByClientId(true)
      
    },
    Close: function () {
        $('#create_contract_pay').removeClass('show')
        setTimeout(function () {
            $('#create_contract_pay').remove();
        }, 300);
    },
    EditRequest: function (isSend = 0) {
        let validate = _add_debt_statistic.Validate()
        if (!validate)
            return;
        var totalAmount = 0
        let orderIds = []
        for (var i = 0; i < listOrderByClient.length; i++) {
            var checked = $('#order_ckb_' + i).is(":checked")
            if (checked) {
                totalAmount += listOrderByClient[i].amount
                orderIds.push(listOrderByClient[i].orderId)
            }
        }
        var fromDateFromStr = $('#filter_date_payment').data('daterangepicker').startDate._d.toLocaleDateString("en-GB");
        var toDateToStr = ($('#filter_date_payment').data('daterangepicker').endDate._d.toLocaleDateString("en-GB"))
        let obj = {
            'id': parseInt($('#payId').val()),
            'description': $('#description').val(),
            'code': $('#debtStatisticCode').val(),
            'amount': totalAmount,
            'fromDateStr': fromDateFromStr,
            'toDateStr': toDateToStr,
            'isSend': isSend,
            'note': $('#content').val(),
            'currency': $('#currency').val(),
            'clientId': parseInt(($('#client-select').val())),
            'orderIds': orderIds.toString(),
        }
        _global_function.AddLoading()
        $.ajax({
            url: "/DebtStatistic/Update",
            type: "Post",
            data: obj,
            success: function (result) {
                _global_function.RemoveLoading()
                if (result.isSuccess) {
                    _msgalert.success(result.message);
                    $.magnificPopup.close();
                    setTimeout(function () {
                        window.location.reload()
                    }, 1000)
                } else {
                    _msgalert.error(result.message);

                }
            }
        });
    },
    ClearError: function () {
        $(".validate-payment-request-type").find('p').remove();
        $(".validate-supplier-select").find('p').remove();
        $(".validate-client-select").find('p').remove();
        $(".validate-amount").find('p').remove();
        $(".validate-payment-request-pay-type").find('p').remove();
        $(".validate-bankingAccount").find('p').remove();
        $(".validate-bankingName").find('p').remove();
        $(".validate-paymentDate").find('p').remove();
        $(".validate-content").find('p').remove();
        $(".validate-description").find('p').remove();
    },
    DisplayError: function (className, message) {
        $("." + className).find('p').remove();
        $("." + className).append("<p>" + message + "</p>");
        $("." + className).css("color", "red");
        $("." + className).css("font-size", "13px");
        $("." + className).css("margin-top", "3px");
    },
}