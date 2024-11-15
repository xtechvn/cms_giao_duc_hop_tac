let listStatusType = [];
let isResetTab = true;
$(document).ready(function () {
    $('input').attr('autocomplete', 'off');
    $('input').keyup(function (e) {
        if (e.which === 13) {
            _debt_brick_service.OnPaging(1);
        }
    });
    $('input').attr('autocomplete', 'off');
    //multi select
    const selectBtnStatusType = document.querySelector(".select-btn-status-type")
    const itemsStatusType = document.querySelectorAll(".item-status-type");
    $(document).click(function (event) {
        var $target = $(event.target);
        if (!$target.closest('#select-btn-status-type').length) {//checkbox_trans_type_
            if ($('#list-item-status').is(":visible") && !$target[0].id.includes('status_type_text') && !$target[0].id.includes('status_type')
                && !$target[0].id.includes('list-item-status') && !$target[0].id.includes('checkbox_status_type')) {
                selectBtnStatusType.classList.toggle("open");
            }
        }
        if (!$target.closest('#select-btn-service-type').length) {
            if ($('#list-item-service').is(":visible") && !$target[0].id.includes('service_type_text') && !$target[0].id.includes('service_type')
                && !$target[0].id.includes('list-item-service') && !$target[0].id.includes('checkbox_service_type')) {
                selectBtnServiceType.classList.toggle("open");
            }
        }
        if (!$target.closest('#select-btn-payment-type').length) {
            if ($('#list-item-payment').is(":visible") && !$target[0].id.includes('payment_type_text') && !$target[0].id.includes('payment_type')
                && !$target[0].id.includes('list-item-payment') && !$target[0].id.includes('checkbox_payment_type')) {
                selectBtnPaymentType.classList.toggle("open");
            }
        }
    });
    if (selectBtnStatusType !== null && selectBtnStatusType !== undefined)
        selectBtnStatusType.addEventListener("click", (e) => {
            e.preventDefault();
            selectBtnStatusType.classList.toggle("open");
        });

    itemsStatusType.forEach(item => {
        item.addEventListener("click", () => {
            item.classList.toggle("checked");
            let checked = document.querySelectorAll(".checked"),
                btnText = document.querySelector(".btn-text-status-type");
            listStatusType = []
            let checked_list = []

            for (var i = 0; i < checked.length; i++) {
                id = checked[i].getAttribute('id')
                if (id.includes('status_type_')) {
                    checked_list.push(checked[i]);
                    listStatusType.push(parseInt(id.replace('status_type_', '')))
                }
            }
            _debt_brick_service.SearchParam.statusMulti = listStatusType

            if (checked_list && checked_list.length > 0) {
                btnText.innerText = `${checked_list.length} Selected`;
            } else {
                btnText.innerText = "Tất cả trạng thái";
            }
        });
    })
    //end multi select
    var SearchParam = _debt_brick_service.GetParam()
    _debt_brick_service.Init(SearchParam);
    $(".input").on('keyup', function (e) {
        if (e.key === 'Enter' || e.keyCode === 13) {
            _debt_brick_service.OnPaging(1)
        }
    });
    setTimeout(function () {
        $('#token-input-client').css('height', 30)
        $('#token-input-supplier').css('height', 30)
    }, 800)

    $("#token-input-client").select2({
        theme: 'bootstrap4',
        placeholder: "Tên KH, Điện Thoại, Email",
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
});
var _debt_brick_service = {
    Init: function (objSearch) {
        $('#divClient').show()
        $('#divSupplier').show()
        this.SearchParam = objSearch;
        this.Search(objSearch);
    },
    ActionSearch: function () {
        isResetTab = true
        this.OnPaging(1)
    },
    GetParam: function () {
        var objSearch = {
            orderNo: $('#orderNo').val(),
            labelName: $('#content').val(),
            type: $('#request_type').val(),
            status: -1,
            statusMulti: listStatusType,
            clientId: $('#token-input-client').val(),
            currentPage: 1,
            pageSize: 20
        }
        objSearch.debtStatus = $('#debtStatus').val()
        return objSearch
    },
    Export: function () {
        var objSearch = this.GetParam()
        objSearch.currentPage = 1;
        this.SearchParam = objSearch
        _global_function.AddLoading()
        $.ajax({
            url: "/DebtBrick/ExportExcel",
            type: "Post",
            data: this.SearchParam,
            success: function (result) {
                _global_function.RemoveLoading()
                if (result.isSuccess) {
                    _msgalert.success(result.message);
                    window.location.href = result.path;
                } else {
                    _msgalert.error(result.message);
                }
            }
        });
    },
    Search: function (input, is_count_status = true) {
        window.scrollTo(0, 0);
        _global_function.AddLoading()
        $.ajax({
            url: "/DebtBrick/Search",
            type: "Post",
            data: input,
            success: function (result) {
                _global_function.RemoveLoading()
                //$('#imgLoading').hide();
                $('#grid_data').html(result);
            }
        });
    },
    OnPaging: function (value) {
        var objSearch = this.GetParam()
        objSearch.currentPage = value;
        this.SearchParam = objSearch
        this.Search(objSearch);
    },
    Add: function (orderId, clientId, payment, amount, orderNo, debtNote) {
        let title = 'Gạch nợ';
        let url = '/DebtBrick/Add';
        var param = {
            'clientId': clientId,
            'orderId': orderId,
            'orderNo': orderNo,
            'amount': amount,
            'payment': payment,
            'debtNote': debtNote,
        };
        _magnific.OpenSmallPopup(title, url, param);
    },
    UndoDebtBrick: function (orderId, clientId, payment, amount, orderNo) {
        let title = 'Bỏ gạch nợ đơn hàng';
        let url = '/DebtBrick/UndoDebtBrick';
        var param = {
            'clientId': clientId,
            'orderId': orderId,
            'orderNo': orderNo,
            'amount': amount,
            'payment': payment,
        };
        _magnific.OpenSmallPopup(title, url, param);
    },
    History: function (orderId, orderNo) {
        let title = 'Lịch sử gạch nợ';
        let url = '/DebtBrick/History';
        var param = {
            'orderId': orderId,
            'orderNo': orderNo,
        };
        _magnific.OpenSmallPopup(title, url, param);
    },
}