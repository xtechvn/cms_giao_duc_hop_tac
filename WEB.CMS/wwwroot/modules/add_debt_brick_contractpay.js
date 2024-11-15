var listOrderDebt = []
var listDetail = []
var amountEdit = 0
var isSetInput = false
var isEdit = false
let isPickerCreateAddContract = false;
var _add_debt_brick_contract_pay = {
    Initialization: function (clientId, payId, amount, debtNote) {
        $('input').attr('autocomplete', 'off');
        $('#note').val(debtNote)
        _add_debt_brick_contract_pay.GetListOrderDebtByClientId(clientId, payId, amount);
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
    GetListOrderDebtByClientId: function (clientId, payId, amount) {
        _global_function.AddLoading()
        $.ajax({
            url: "/DebtBrick/GetListOrderDebtByClientId",
            type: "Post",
            data: {
                'clientId': clientId,
                'payId': payId,
                'amountOrder': amount,
            },
            success: function (result) {
                _global_function.RemoveLoading()
                listOrderDebt = result.data
                var totalAmount = 0
                var totalDisarmed = 0
                var totalNeedPayment = 0
                $("#body_order_list").empty();
                for (var i = 0; i < result.data.length; i++) {
                    $('#order-relate-table').find('tbody').append(
                        "<tr id='order_" + i + "'>" +
                        "<td>" +
                        "<label class='check-list number'>" +
                        " <input type='checkbox' id='order_ckb_" + result.data[i].orderNo + "' name='order_ckb' onclick='_add_debt_brick_contract_pay.OnCheckBox(" + i + ");'>" +
                        " <span class='checkmark'></span>" + (i + 1) +
                        "  </label>" +
                        "<td>" +
                        " <a class='blue' target='blank' href='/Order/Orderdetails?id=" + result.data[i].orderId + "'> " + result.data[i].orderNo + " </a>"
                        + "</td>" +
                        "<td>" + result.data[i].createName + "</td>" +
                        "<td class='text-right' >" + _add_debt_brick_contract_pay.FormatNumberStr(result.data[i].amount) + "</td>" +
                        "<td class='text-right' >" + _add_debt_brick_contract_pay.FormatNumberStr(result.data[i].payment) + "</td>" +
                        "<td class='text-right'>" + _add_debt_brick_contract_pay.FormatNumberStr(result.data[i].amountRemain) + "</td>" +
                        "<td class='text-right' >" + "<input type='text' id='amount_order_" + result.data[i].orderId + "' class='background-disabled text-right' maxlength='15'  autocomplete='off' style='min-width: 100px;' disabled  onkeyup='_add_debt_brick_contract_pay.FormatNumberOrder(" + result.data[i].orderId + ");' onchange='_add_debt_brick_contract_pay.UpdateAmount(" + i + "," + result.data[i].orderId + ")' >" + "</td>" +
                        "</tr>"
                    );
                    totalAmount += result.data[i].amount
                    totalDisarmed += result.data[i].payment
                    totalNeedPayment += result.data[i].amountRemain
                }
                $('#order-relate-table').find('tbody').append(
                    "<tr style='font-weight:bold !important;'>" +
                    "<td class='text-right' colspan='3'> Tổng </td>" +
                    "<td class='text-right' >" + _add_debt_brick_contract_pay.FormatNumberStr(totalAmount) + "</td>" +
                    "<td class='text-right'>" + _add_debt_brick_contract_pay.FormatNumberStr(totalDisarmed) + "</td>" +
                    "<td class='text-right' >" + _add_debt_brick_contract_pay.FormatNumberStr(totalNeedPayment) + "</td>" +
                    "<td class='text-right' id='total_amount_need_pay'>0</td>" +
                    "</tr>"
                );
            }
        });
    },
    UpdateAmount: function (index, payId) {
        var totalAmount = 0
        for (var i = 0; i < listOrderDebt.length; i++) {
            if (listOrderDebt[i].orderId == payId) {
                var amount_input = parseFloat($('#amount_order_' + listOrderDebt[i].orderId).val().replaceAll('.', '').replaceAll(',', ''))
                listOrderDebt[i].amount = amount_input
                if (amount_input > listOrderDebt[i].amountRemain)
                    listOrderDebt[i].amount = listOrderDebt[i].amountRemain
            }
            if (listOrderDebt[i].amount !== undefined && listOrderDebt[i].amount !== null
                && listOrderDebt[i].amount) {
                var checked = $('#order_ckb_' + listOrderDebt[i].orderNo).is(":checked")
                if (checked) {
                    totalAmount += listOrderDebt[i].amount
                }
                if (i == index) {
                    $('#amount_order_' + listOrderDebt[i].orderId).val(_add_debt_brick_contract_pay.FormatNumberStr(listOrderDebt[i].amount))
                }
            }
        }
        $('#total_amount_need_pay').html(_add_debt_brick_contract_pay.FormatNumberStr(totalAmount))
        if (totalAmount > parseFloat($('#amount').val().replaceAll('.', '').replaceAll(',', ''))) {
            _msgalert.error(' Tổng tiền cần giải trừ không được lớn hơn số tiền còn lại của đơn hàng');
        }
    },
    Validate: function () {
        var flag = false
        var totalInputAmount = 0
        for (var i = 0; i < listOrderDebt.length; i++) {
            var checked = $('#order_ckb_' + listOrderDebt[i].orderNo).is(":checked")
            if (checked) {
                totalInputAmount += listOrderDebt[i].amount
                if (listOrderDebt[i].amount === 0) {
                    _msgalert.error('Vui lòng nhập số tiền giải trừ lớn hơn 0');
                    return false;
                }
                flag = true
            }
        }
        if (!flag) {
            _msgalert.error('Vui lòng tích chọn phiếu thu cần giải trừ');
            return false;
        }
        if (totalInputAmount === 0) {
            _msgalert.error('Vui lòng nhập số tiền giải trừ lớn hơn 0');
            return false;
        }
        var amountContract = parseFloat($('#amount').val().replaceAll('.', '').replaceAll(',', ''))
        if (totalInputAmount > amountContract) {
            _msgalert.error(' Tổng tiền cần giải trừ không được lớn hơn số tiền còn lại của đơn hàng');
            return false;
        }
        //if ($('#note').val() === null || $('#note').val() === undefined || $('#note').val() === '') {
        //    _msgalert.error('Vui lòng nhập ghi chú');
        //    return false;
        //}
        return true
    },
    AddNewContractPayDetail: function () {
        let validate = _add_debt_brick_contract_pay.Validate()
        if (!validate)
            return;
        let orderDebts = listOrderDebt
        orderDebts = []
        for (var i = 0; i < listOrderDebt.length; i++) {
            var checked = $('#order_ckb_' + listOrderDebt[i].orderNo).is(":checked")
            if (checked) {
                orderDebts.push(listOrderDebt[i])
            }
        }
        let obj = {
            'orderDebtViewModels': orderDebts,
            'note': $('#note').val(),
            'clientName': $('#clientName').val(),
            'billNo': $('#billNo').val(),
            'clientId': $('#clientId').val(),
            'contractPayId': $('#payId').val(),
            'orderId': $('#orderId').val(),
            'orderNo': $('#orderNo').val(),
            'amountRemain': $('#amountRemain').val().replaceAll('.', '').replaceAll(',', ''),
            'payment': $('#payment').val().replaceAll('.', '').replaceAll(',', ''),
            'amountOrder': $('#amountOrder').val().replaceAll('.', '').replaceAll(',', ''),
        }
        _global_function.AddLoading()
        $.ajax({
            url: "/DebtBrick/AddWithContractPayJson",
            type: "Post",
            data: obj,
            success: function (result) {
                _global_function.RemoveLoading()
                if (result.isSuccess) {
                    _msgalert.success(result.message);
                    $.magnificPopup.close();
                    setTimeout(function () { window.location.reload() }, 500)
                } else {
                    _msgalert.error(result.message);
                }
            }
        });
    },
    Close: function () {
        $('#create_contract_pay').removeClass('show')
        setTimeout(function () {
            $('#create_contract_pay').remove();
        }, 300);
    },
    OnCheckBox: function (index) {
        //calculate automatic amount
        var amount_contract = parseFloat($('#amount').val().replaceAll('.', '').replaceAll(',', ''))
        var totalAmount = 0
        for (var i = 0; i < listOrderDebt.length; i++) {
            var checked = $('#order_ckb_' + listOrderDebt[i].orderNo).is(":checked")
            if (checked && $('#amount_order_' + listOrderDebt[i].orderId).val() !== undefined && $('#amount_order_' + listOrderDebt[i].orderId).val() !== null
                && $('#amount_order_' + listOrderDebt[i].orderId).val() !== '') {
                totalAmount += parseFloat($('#amount_order_' + listOrderDebt[i].orderId).val().replaceAll('.', '').replaceAll(',', ''))
            }
        }
        if (isNaN(totalAmount)) totalAmount = 0
        for (var i = 0; i < listOrderDebt.length; i++) {
            var checked = $('#order_ckb_' + listOrderDebt[i].orderNo).is(":checked")
            if (checked) {
                let isSuccess = true
                $('#amount_order_' + listOrderDebt[i].orderId).attr('disabled', false)
                $('#amount_order_' + listOrderDebt[i].orderId).removeClass('background-disabled')
                var amount = $('#amount_order_' + listOrderDebt[i].orderId).val()
                if (amount === null || amount === undefined || amount === '' || parseInt(amount) === 0) {
                    var totalNeedPayment = Math.min((amount_contract - totalAmount), listOrderDebt[i].totalNeedPayment)
                    if (totalNeedPayment <= 0) {
                        $('#order_ckb_' + listOrderDebt[i].orderNo).attr("checked", false)
                        $('#amount_order_' + listOrderDebt[i].orderId).attr('disabled', true)
                        $('#amount_order_' + listOrderDebt[i].orderId).addClass('background-disabled')
                        $('#amount_order_' + listOrderDebt[i].orderId).val('')
                        _msgalert.error('Tổng tiền cần giải trừ đã bằng số tiền cần thanh toán của đơn hàng');
                        isSuccess = false
                    } else {
                        $('#amount_order_' + listOrderDebt[i].orderId).val(_add_debt_brick_contract_pay.FormatNumberStr(totalNeedPayment))
                        _add_debt_brick_contract_pay.UpdateAmount(i, listOrderDebt[i].orderId)
                    }
                }
                if (isSuccess)
                    listOrderDebt[i].isChecked = true
            } else {
                $('#amount_order_' + listOrderDebt[i].orderId).attr('disabled', true)
                $('#amount_order_' + listOrderDebt[i].orderId).addClass('background-disabled')
                $('#amount_order_' + listOrderDebt[i].orderId).val('')
            }
        }
        _add_debt_brick_contract_pay.GetTotalAmount(index)
    },
    GetTotalAmount: function (index) {
        var totalAmount = 0
        for (var i = 0; i < listOrderDebt.length; i++) {
            if (listOrderDebt[i].amount !== undefined && listOrderDebt[i].amount !== null
                && listOrderDebt[i].amount) {
                var checked = $('#order_ckb_' + listOrderDebt[i].orderNo).is(":checked")
                if (checked) {
                    totalAmount += parseFloat($('#amount_order_' + listOrderDebt[i].orderId).val().replaceAll('.', '').replaceAll(',', ''))
                }
            }
        }
        $('#total_amount_need_pay').html(_add_debt_brick_contract_pay.FormatNumberStr(totalAmount))
        if (totalAmount > parseFloat($('#amount').val().replaceAll('.', '').replaceAll(',', ''))) {
            _msgalert.error(' Tổng tiền cần giải trừ không được lớn hơn số tiền cần thanh toán của đơn hàng');
        }
    },
    OnChangeAmount: function () {
        var contract_type = $('#contract-type').val()
        if (contract_type !== null && contract_type !== '' && parseInt(contract_type) == 1) { // thu tiền đơn hàng
            var currentAmomunt = parseFloat($('#amount').val().replaceAll('.', '').replaceAll(',', ''))
            if (currentAmomunt < amountEdit) {
                _msgalert.error('Vui lòng nhập số tiền lớn hơn hoặc bằng số tiền ban đầu');
                $('#amount').val(_add_debt_brick_contract_pay.FormatNumberStr(amountEdit))
                return false;
            }
        }
    },
    InitializationUnDebt: function (clientId, payId, amount) {
        $('input').attr('autocomplete', 'off');
        _add_debt_brick_contract_pay.GetListContractPayByPayId(payId);
    },
    GetListContractPayByPayId: function (payId) {
        _global_function.AddLoading()
        $.ajax({
            url: "/DebtBrick/GetListContractPayByPayId",
            type: "Post",
            data: {
                'payId': payId,
            },
            success: function (result) {
                _global_function.RemoveLoading()
                listContractPayDetail = result.data
                var totalAmount = 0
                var totalDisarmed = 0
                var totalNeedPayment = 0
                var totalPayment = 0
                $("#body_contract_pay_list").empty();
                for (var i = 0; i < result.data.length; i++) {
                    $('#order-relate-table').find('tbody').append(
                        "<tr id='order_" + i + "'>" +
                        "<td>" +
                        "<label class='check-list number'>" +
                        " <input type='checkbox' id='order_ckb_" + result.data[i].orderNo + "' name='order_ckb' checked='true'>" +
                        " <span class='checkmark'></span>" + (i + 1) +
                        "  </label>" +
                        "<td>" +
                        " <a class='blue' href='/Order/Orderdetails?id=" + result.data[i].orderId + "'> " + result.data[i].orderNo + " </a>"
                        + "</td>" +
                        "<td>" + result.data[i].orderCreateName + "</td>" +
                        "<td class='text-right' >" + _add_debt_brick_contract_pay.FormatNumberStr(result.data[i].amountOrder) + "</td>" +
                        "<td class='text-right' >" + _add_debt_brick_contract_pay.FormatNumberStr(result.data[i].totalAmountPayDetail) + "</td>" +
                        "<td class='text-right'>" + _add_debt_brick_contract_pay.FormatNumberStr(result.data[i].totalAmountRemain) + "</td>" +
                        "<td class='text-right'>" + _add_debt_brick_contract_pay.FormatNumberStr(result.data[i].amountPayDetail) + "</td>" +
                        "</tr>"
                    );
                    totalAmount += result.data[i].amountOrder
                    totalDisarmed += result.data[i].totalAmountPayDetail
                    totalNeedPayment += result.data[i].totalAmountRemain
                    totalPayment += result.data[i].amountPayDetail
                }
                $('#order-relate-table').find('tbody').append(
                    "<tr style='font-weight:bold !important;'>" +
                    "<td class='text-right' colspan='3'> Tổng </td>" +
                    "<td class='text-right' >" + _add_debt_brick_contract_pay.FormatNumberStr(totalAmount) + "</td>" +
                    "<td class='text-right'>" + _add_debt_brick_contract_pay.FormatNumberStr(totalDisarmed) + "</td>" +
                    "<td class='text-right' >" + _add_debt_brick_contract_pay.FormatNumberStr(totalNeedPayment) + "</td>" +
                    "<td class='text-right' >" + _add_debt_brick_contract_pay.FormatNumberStr(totalPayment) + "</td>" +
                    "</tr>"
                );
            }
        });
    },
    UndoDebtBrick: function () {
        let listDetail = listContractPayDetail
        listDetail = []
        for (var i = 0; i < listContractPayDetail.length; i++) {
            var checked = $('#order_ckb_' + listContractPayDetail[i].orderNo).is(":checked")
            if (!checked) {
                listDetail.push(listContractPayDetail[i])
            }
        }
        if (listDetail.length == 0) {
            _msgalert.error("Bạn cần bỏ chọn ít nhất 1 đơn hàng");
            return
        }
        let obj = {
            'model': listDetail,
        }
        _global_function.AddLoading()
        $.ajax({
            url: "/DebtBrick/UndoDebtBrickContractPayJson",
            type: "Post",
            data: obj,
            success: function (result) {
                _global_function.RemoveLoading()
                if (result.isSuccess) {
                    _msgalert.success(result.message);
                    $.magnificPopup.close();
                    setTimeout(function () { window.location.reload() }, 500)
                } else {
                    _msgalert.error(result.message);
                }
            }
        });
    }
}