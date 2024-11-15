let isPickerCreated = false;
let isPickerCreate_payment = false;
let listUserCreate = []
var listStatusType = []
let DEBT_STATISTIC_STATUS_DRAFT = 0
let DEBT_STATISTIC_STATUS_REJECT = 1
let DEBT_STATISTIC_STATUS_WAIT_ACCOUNTANT_CONFIRM = 2
let DEBT_STATISTIC_STATUS_WAIT_CUSTOMER_CONFIRM = 3
let DEBT_STATISTIC_STATUS_CONFIRM = 4
let DEBT_STATISTIC_STATUS_CANCEL = 5
var isResetTab = false
$(document).ready(function () {
    $('input').attr('autocomplete', 'off');
    $('input').keyup(function (e) {
        if (e.which === 13) {
            _debt_statistic_service.OnPaging(1);
        }
    });
    //end multi select
    var SearchParam = _debt_statistic_service.GetParam()
    _debt_statistic_service.Init(SearchParam);
    $(".input").on('keyup', function (e) {
        if (e.key === 'Enter' || e.keyCode === 13) {
            _debt_statistic_service.OnPaging(1)
        }
    });
    setTimeout(function () {
        $('#token-input-client').css('height', 30)
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
    $("#createdBy").select2({
        theme: 'bootstrap4',
        placeholder: "Người tạo",
        ajax: {
            url: "/User/GetUserSuggestionList",
            type: "post",
            dataType: 'json',
            delay: 250,
            data: function (params) {
                var query = {
                    name: params.term,
                }
                // Query parameters will be ?search=[term]&type=public
                return query;
            },
            processResults: function (response) {
                return {
                    results: $.map(response, function (item) {
                        return {
                            text: item.fullname,
                            id: item.id,
                        }
                    })
                };
            },
            cache: true
        }
    });

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
                selectBtnStatusType.classList.toggle("open");
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
            let checked_list = []
            listStatusType = []
            for (var i = 0; i < checked.length; i++) {
                id = checked[i].getAttribute('id')
                if (id.includes('status_type_')) {
                    checked_list.push(checked[i]);
                    listStatusType.push(parseInt(id.replace('status_type_', '')))
                }
            }
            _debt_statistic_service.SearchParam.statusMulti = listStatusType

            if (checked_list && checked_list.length > 0) {
                btnText.innerText = `${checked_list.length} Selected`;
            } else {
                btnText.innerText = "Tất cả trạng thái";
            }
        });
    })
});
var _debt_statistic_service = {
    Init: function (objSearch) {
        $('#divClient').show()
        this.SearchParam = objSearch;
        this.Search(objSearch);
    },
    GetParam: function () {
        var objSearch = {
            code: $('#billNo').val(),
            statusMulti: listStatusType,
            createByIds: $('#createdBy').select2("val"),
            clientId: $('#token-input-client').val() != null && $('#token-input-client').val() !== undefined &&
                ($('#token-input-client').val()).length > 0 ? ($('#token-input-client').val())[0] : 0,
            currentPage: 1,
            pageSize: 20
        }
        if ($('#filter_date_create_daterangepicker').data('daterangepicker') !== undefined &&
            $('#filter_date_create_daterangepicker').data('daterangepicker') != null && isPickerCreated) {
            objSearch.createdDateFromStr = $('#filter_date_create_daterangepicker').data('daterangepicker').startDate._d.toLocaleDateString("en-GB");
            objSearch.createdDateToStr = $('#filter_date_create_daterangepicker').data('daterangepicker').endDate._d.toLocaleDateString("en-GB");
        } else {
            objSearch.createdDateFromStr = null
            objSearch.createdDateToStr = null
        }
        if ($('#filter_date_payment').data('daterangepicker') !== undefined &&
            $('#filter_date_payment').data('daterangepicker') != null && isPickerCreate_payment) {
            objSearch.fromDateStr = $('#filter_date_payment').data('daterangepicker').startDate._d.toLocaleDateString("en-GB");
            objSearch.toDateStr = $('#filter_date_payment').data('daterangepicker').endDate._d.toLocaleDateString("en-GB");
        } else {
            objSearch.fromDateStr = null
            objSearch.toDateStr = null
        }
        return objSearch
    },
    BackToList: function () {
        window.location.href = '/DebtStatistic/Index'
    },
    Export: function () {
        $('#btnExport').prop('disabled', true);
        $('#icon-export').removeClass('fa-file-excel-o');
        $('#icon-export').addClass('fa-spinner fa-pulse');
        var objSearch = this.GetParam()
        objSearch.currentPage = 1;
        this.SearchParam = objSearch
        _global_function.AddLoading()
        $.ajax({
            url: "/DebtStatistic/ExportExcel",
            type: "Post",
            data: this.SearchParam,
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
    ExportOrderList: function () {
        $('#btnExport').prop('disabled', true);
        $('#icon-export').removeClass('fa-file-excel-o');
        $('#icon-export').addClass('fa-spinner fa-pulse');
        var objSearch = this.GetParam()
        objSearch.currentPage = 1;
        this.SearchParam = objSearch
        _global_function.AddLoading()
        $.ajax({
            url: "/DebtStatistic/ExportExcelOrderList",
            type: "Post",
            data: { id: $('#requestId').val()},
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
    ActionSearch: function (typeSearch) {
        isResetTab = true
        this.OnPaging(1, true)
    },
    Search: function (input, is_count_status = true) {
        window.scrollTo(0, 0);
        //$('#imgLoading').show();
        _global_function.AddLoading()
        $.ajax({
            url: "/DebtStatistic/Search",
            type: "Post",
            data: input,
            success: function (result) {
                _global_function.RemoveLoading()
                //$('#imgLoading').hide();
                $('#grid_data').html(result);
            }
        });
        if (is_count_status) {
            this.OnCountStatus()
            this.SetActive(-1)
        }
    },
    OnPaging: function (value) {
        var objSearch = this.GetParam()
        objSearch.currentPage = value;
        this.SearchParam = objSearch
        this.Search(objSearch, true);
    },
    AddDebtStatistic: function () {
        let title = 'Thêm bảng kê';
        let url = '/DebtStatistic/Add';
        var param = {
        };
        _magnific.OpenSmallPopup(title, url, param);
    },
    Edit: function (id) {
        let title = 'Chỉnh sửa bảng kê';
        let url = '/DebtStatistic/Edit?debtStatisticId=' + id;
        var param = {
        };
        _magnific.OpenSmallPopup(title, url, param);
    },
    OnCountStatus: function () {
        var objSearch = this.GetParam();
        _global_function.AddLoading()
        $.ajax({
            url: "/DebtStatistic/CountStatus",
            type: "Post",
            data: objSearch,
            success: function (result) {
                _global_function.RemoveLoading()
                $('#countSttDraft').text('Lưu nháp (0)')
                $('#countSttWaitAccountantLeadApprove').text('Chờ kế toán xác nhận (0)')
                $('#countSttWaitCustomerApprove').text('Chờ khách hàng xác nhận (0)')
                $('#countSttReject').text('Từ chối (0)')
                $('#countSttConfirm').text('Đã xác nhận (0)')
                $('#countSttCancel').text('Hủy (0)')
                $('#countSttAll').text('Tất cả (0)')
                if (result.data.length > 0) {
                    for (var i = 0; i < result.data.length; i++) {
                        if (result.data[i].status == -1) {
                            $('#countSttAll').text('Tất cả (' + result.data[i].total + ')')
                        }
                        if (result.data[i].status == 0) {
                            $('#countSttDraft').text('Lưu nháp (' + result.data[i].total + ')')
                        }
                        if (result.data[i].status == 1) {
                            $('#countSttReject').text('Từ chối (' + result.data[i].total + ')')
                        }
                        if (result.data[i].status == 2) {
                            $('#countSttWaitAccountantLeadApprove').text('Chờ kế toán xác nhận (' + result.data[i].total + ')')
                        }
                        if (result.data[i].status == 3) {
                            $('#countSttWaitCustomerApprove').text('Chờ khách hàng xác nhận (' + result.data[i].total + ')')
                        }
                        if (result.data[i].status == 4) {
                            $('#countSttConfirm').text('Đã xác nhận (' + result.data[i].total + ')')
                        }
                        if (result.data[i].status == 5) {
                            $('#countSttCancel').text('Hủy (' + result.data[i].total + ')')
                        }
                    }
                }
            }
        });
    },
    OnSearchStatus: function (status) {
        $('#status').val(-1)
        isResetTab = false
        var objSearch = this.SearchParam;
        objSearch.currentPage = 1;
        objSearch.status = status;
        this.Search(objSearch, false);
        this.SetActive(status)
    },
    SetActive: function (status) {
        var objSearch = this.SearchParam
        if (!isResetTab && objSearch.currentPage > 1)
            return
        let status_choose = $('#status').val()
        $('#countSttAll').removeClass('active')
        $('#countSttDraft').removeClass('active')
        $('#countSttWaitAccountantLeadApprove').removeClass('active')
        $('#countSttWaitCustomerApprove').removeClass('active')
        $('#countSttReject').removeClass('active')
        $('#countSttConfirm').removeClass('active')
        $('#countSttCancel').removeClass('active')
        if (status == -1) {
            $('#countSttAll').addClass('active')
        }
        if (status == DEBT_STATISTIC_STATUS_DRAFT) {
            $('#countSttDraft').addClass('active')
            if (status_choose !== undefined && status_choose !== null && status_choose !== '' && status_choose !== '1')
                $('#countSttDraft').addClass('disabled-a-tag')
            else
                $('#countSttDraft').removeClass('disabled-a-tag')
        }
        if (status == DEBT_STATISTIC_STATUS_REJECT) {
            $('#countSttReject').addClass('active')
            if (status_choose !== undefined && status_choose !== null && status_choose !== '' && status_choose !== '1')
                $('#countSttReject').addClass('disabled-a-tag')
            else
                $('#countSttReject').removeClass('disabled-a-tag')
        }
        if (status == DEBT_STATISTIC_STATUS_WAIT_ACCOUNTANT_CONFIRM) {
            $('#countSttWaitAccountantLeadApprove').addClass('active')
            if (status_choose !== undefined && status_choose !== null && status_choose !== '' && status_choose !== '2')
                $('#countSttWaitAccountantLeadApprove').addClass('disabled-a-tag')
            else
                $('#countSttWaitAccountantLeadApprove').removeClass('disabled-a-tag')
        }
        if (status == DEBT_STATISTIC_STATUS_WAIT_CUSTOMER_CONFIRM) {
            $('#countSttWaitCustomerApprove').addClass('active')
            if (status_choose !== undefined && status_choose !== null && status_choose !== '' && status_choose !== '3')
                $('#countSttWaitCustomerApprove').addClass('disabled-a-tag')
            else
                $('#countSttWaitCustomerApprove').removeClass('disabled-a-tag')
        }
        if (status == DEBT_STATISTIC_STATUS_CANCEL) {
            $('#countSttCancel').addClass('active')
            if (status_choose !== undefined && status_choose !== null && status_choose !== '' && status_choose !== '4')
                $('#countSttCancel').addClass('disabled-a-tag')
            else
                $('#countSttCancel').removeClass('disabled-a-tag')
        }
        if (status == DEBT_STATISTIC_STATUS_CONFIRM) {
            $('#countSttConfirm').addClass('active')
            if (status_choose !== undefined && status_choose !== null && status_choose !== '' && status_choose !== '4')
                $('#countSttConfirm').addClass('disabled-a-tag')
            else
                $('#countSttConfirm').removeClass('disabled-a-tag')
        }
    },
    Approve: function (status) {
        _msgconfirm.openDialog("Xác nhận bảng kê", "Bạn có chắc muốn xác nhận bảng kê " + $('#debtStatisticCode').val() + " không?", function () {
            $.ajax({
                url: "/DebtStatistic/Approve",
                type: "Post",
                data: { 'debtStatisticCode': $('#debtStatisticCode').val(), 'requestId': $('#requestId').val(), 'status': status },
                success: function (result) {
                    if (result.isSuccess === true) {
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
        });
    },
    Delete: function (status) {
        _msgconfirm.openDialog("Xóa bảng kê", "Bạn có chắc muốn xóa bảng kê " + $('#debtStatisticCode').val() + " không?", function () {
            $.ajax({
                url: "/DebtStatistic/Delete",
                type: "Post",
                data: { 'debtStatisticCode': $('#debtStatisticCode').val() },
                success: function (result) {
                    if (result.isSuccess === true) {
                        _msgalert.success(result.message);
                        $.magnificPopup.close();
                        setTimeout(function () {
                            window.location.href = '/DebtStatistic/Index'
                        }, 1000)
                    } else {
                        _msgalert.error(result.message);
                    }
                }
            });
        });
    },
    Cancel: function (status) {
        //validate order in list have any payment
        var totalAmountPay = parseFloat($('#totalAmountPayment').html().replaceAll(',', ''))
        _msgconfirm.openDialog("Hủy bảng kê", "Bạn có chắc muốn hủy bảng kê " + $('#debtStatisticCode').val() + " không?", function () {
            $.ajax({
                url: "/DebtStatistic/Cancel",
                type: "Post",
                data: { 'debtStatisticCode': $('#debtStatisticCode').val(), 'requestId': $('#requestId').val(), 'totalAmountPay': totalAmountPay },
                success: function (result) {
                    if (result.isSuccess === true) {
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
        });
    },
    Reject: function () {
        let title = 'Từ chối duyệt yêu cầu';
        let url = '/DebtStatistic/Reject';
        var param = {
        };
        _magnific.OpenSmallPopup(title, url, param);
    },
    RejectDeposit: function () {
        if ($('#note').val() === null || $('#note').val() === undefined || $('#note').val() === '') {
            _msgalert.error('Vui lòng nhập lý do từ chối');
            return
        }
        var note = $('#note').val()
        if (note.length > 300) {
            _msgalert.error('Nội dung từ chối không được quá 300 kí tự');
            return
        }
        _global_function.AddLoading()
        $.ajax({
            url: "/DebtStatistic/RejectRequest",
            type: "Post",
            data: { 'debtStatisticCode': $('#debtStatisticCode').val(), 'requestId': $('#requestId').val(), 'note': $('#note').val() },
            success: function (result) {
                if (result.isSuccess === true) {
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
    History: function (requestId) {
        let title = 'Lịch sử xác nhận bảng kê';
        let url = '/DebtStatistic/History?requestId=' + requestId;
        var param = {
        };
        _magnific.OpenSmallPopup(title, url, param);
    },
}