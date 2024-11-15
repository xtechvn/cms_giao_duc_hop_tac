let fields = {
    STT: true,
    TransNo: true,
    TransType: true,
    Title: true,
    Price: true,
    ServiceType: true,
    PaymentType: true,
    TransactionDate: true,
    CreateBy: true,
    Approver: true,
    ApproveDate: true,
    Status: true
}
let isPickerCreate = false;
let contractPays = []
let isPickerApprove = false;
let listTransType = [];
let listServiceType = [];
let listPaymentType = [];
let listUserCreate = [];
let listUserApprove = [];
let isResetTab = true;
let cookieName = 'fields_deposit_hisoty'
var timer;
var firstTime = true
$(document).ready(function () {
    $('input').attr('autocomplete', 'off');
    //multi select
    const selectBtnTransType = document.querySelector(".select-btn-trans-type")
    const itemsTransType = document.querySelectorAll(".item-trans-type");
    const selectBtnServiceType = document.querySelector(".select-btn-service-type")
    const itemsServiceType = document.querySelectorAll(".item-service-type");
    const selectBtnPaymentType = document.querySelector(".select-btn-payment-type")
    const itemsPaymentType = document.querySelectorAll(".item-payment-type");
    $(document).click(function (event) {
        var $target = $(event.target);
        if (!$target.closest('#select-btn-trans-type').length) {//checkbox_trans_type_
            if ($('#list-item-trans').is(":visible") && !$target[0].id.includes('trans_type_text') && !$target[0].id.includes('trans_type')
                && !$target[0].id.includes('list-item-trans') && !$target[0].id.includes('checkbox_trans_type')) {
                selectBtnTransType.classList.toggle("open");
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
    selectBtnTransType.addEventListener("click", (e) => {
        e.preventDefault();
        selectBtnTransType.classList.toggle("open");
    });
    selectBtnServiceType.addEventListener("click", (e) => {
        e.preventDefault();
        selectBtnServiceType.classList.toggle("open");
    });
    selectBtnPaymentType.addEventListener("click", (e) => {
        e.preventDefault();
        selectBtnPaymentType.classList.toggle("open");
    });

    itemsTransType.forEach(item => {
        item.addEventListener("click", () => {
            item.classList.toggle("checked");
            let checked = document.querySelectorAll(".checked"),
                btnText = document.querySelector(".btn-text-trans-type");
            listTransType = []
            let checked_list = []

            for (var i = 0; i < checked.length; i++) {
                id = checked[i].getAttribute('id')
                if (id.includes('trans_type_')) {
                    checked_list.push(checked[i]);
                    listTransType.push(parseInt(id.replace('trans_type_', '')))
                }
            }
            _deposit_hisoty_service.SearchParam.transTypes = listTransType

            if (checked_list && checked_list.length > 0) {
                btnText.innerText = `${checked_list.length} Selected`;
            } else {
                btnText.innerText = "Tất cả loại giao dịch";
            }
        });
    })
    itemsServiceType.forEach(item => {
        item.addEventListener("click", () => {
            item.classList.toggle("checked");

            let checked = document.querySelectorAll(".checked"),
                btnText = document.querySelector(".btn-text-service-type");
            let checked_list = []
            listServiceType = []
            for (var i = 0; i < checked.length; i++) {
                id = checked[i].getAttribute('id')
                if (id.includes('service_type_')) {
                    checked_list.push(checked[i]);
                    listServiceType.push(parseInt(id.replace('service_type_', '')))
                }
            }
            _deposit_hisoty_service.SearchParam.serviceTypes = listServiceType

            if (checked_list && checked_list.length > 0) {
                btnText.innerText = `${checked_list.length} Selected`;
            } else {
                btnText.innerText = "Tất cả loại quỹ";
            }
        });
    })
    itemsPaymentType.forEach(item => {
        item.addEventListener("click", () => {
            item.classList.toggle("checked");

            let checked = document.querySelectorAll(".checked"),
                btnText = document.querySelector(".btn-text-payment-type");
            let checked_list = []
            listPaymentType = []
            for (var i = 0; i < checked.length; i++) {
                id = checked[i].getAttribute('id')
                if (id.includes('payment_type_')) {
                    checked_list.push(checked[i]);
                    listServiceType.push(parseInt(id.replace('payment_type_', '')))
                }
            }
            _deposit_hisoty_service.SearchParam.paymentTypes = listPaymentType

            if (checked_list && checked_list.length > 0) {
                btnText.innerText = `${checked_list.length} Selected`;
            } else {
                btnText.innerText = "Tất cả hình thức thanh toán";
            }
        });
    })
    //end multi select
    var SearchParam = _deposit_hisoty_service.GetParam(true)
    $('#transactionDate').val('')
    $('#approveDate').val('')
    _deposit_hisoty_service.Init(SearchParam);
    $(".input").on('keyup', function (e) {
        if (e.key === 'Enter' || e.keyCode === 13) {
            _deposit_hisoty_service.OnPaging(1)
        }
    });

    $("#createdBy").select2({
        theme: 'bootstrap4',
        placeholder: "Người tạo",
        ajax: {
            url: "/Funding/GetUserCreateSuggest",
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
                            text: item.name,
                            id: item.id,
                        }
                    })
                };
            },
            cache: true
        }
    });
    $("#approverBy").select2({
        theme: 'bootstrap4',
        placeholder: "Người duyệt",
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
                            text: item.fullname + ' - ' + item.email,
                            id: item.id,
                        }
                    })
                };
            },
            cache: true
        }
    });
    //if (_deposit_hisoty_service.getCookie('fields_deposit_hisoty') != null) {
    //    let cookie = _deposit_hisoty_service.getCookie(cookieName)
    //    fields = JSON.parse(cookie)
    //} else {
    //    _deposit_hisoty_service.setCookie(cookieName, JSON.stringify(fields), 30)
    //}
    //_deposit_hisoty_service.checkShowHide();
});

var _deposit_hisoty_service = {
    Init: function (objSearch) {
        this.SearchParam = objSearch;
        this.Search(objSearch);
    },
    GetParam: function (isFirstTime = false) {
        firstTime = false
        var objSearch = {
            transNo: $('#transactionNo').val(),
            transTypes: listTransType,
            serviceTypes: listServiceType,
            paymentTypes: listPaymentType,
            status: $('#status').val(),
            transactionDate: null,
            createBy: $('#createBy').val(),
            approver: $('#approver').val(),
            approveDate: null,
            createByIds: $('#createdBy').select2("val"),
            approverIds: $('#approverBy').select2("val"),
            statusChoose: -1,
            currentPage: 1,
            pageSize: 20
        }
        if ($('#filter_date_create_daterangepicker').data('daterangepicker') !== undefined &&
            $('#filter_date_create_daterangepicker').data('daterangepicker') != null && isPickerCreate) {
            objSearch.fromCreateDateStr = $('#filter_date_create_daterangepicker').data('daterangepicker').startDate._d.toLocaleDateString("en-GB");
            objSearch.toCreateDateStr = $('#filter_date_create_daterangepicker').data('daterangepicker').endDate._d.toLocaleDateString("en-GB");
        } else {
            objSearch.fromCreateDateStr = null
            objSearch.toCreateDateStr = null
        }
        if ($('#filter_date_aprrove_daterangepicker').data('daterangepicker') !== undefined &&
            $('#filter_date_aprrove_daterangepicker').data('daterangepicker') != null && isPickerApprove) {
            objSearch.fromApproveDateStr = $('#filter_date_aprrove_daterangepicker').data('daterangepicker').startDate._d.toLocaleDateString("en-GB");
            objSearch.toApproveDateStr = $('#filter_date_aprrove_daterangepicker').data('daterangepicker').endDate._d.toLocaleDateString("en-GB");
        } else {
            objSearch.fromApproveDateStr = null
            objSearch.toApproveDateStr = null
        }
        return objSearch
    },
    BackToList: function () {
        window.location.href = '/Funding/Index'
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
            url: "/Funding/ExportExcel",
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
    ActionSearch: function () {
        isResetTab = true
        this.OnPaging(1)
    },
    OnPaging: function (value) {
        //var objSearch = this.GetParam()
        //this.SearchParam = objSearch;
        var objSearch = this.GetParam()
        objSearch.currentPage = value;
        this.SearchParam = objSearch
        this.Search(objSearch);
    },
    OnCountStatus: function () {
        var objSearch = this.GetParam();
        _global_function.AddLoading()
        $.ajax({
            url: "/Funding/CountStatus",
            type: "Post",
            data: objSearch,
            success: function (result) {
                _global_function.RemoveLoading()
                $('#countSttWaitPayment').text('Chờ thanh toán (0)')
                $('#countSttWaitApprove').text('Chờ duyệt (0)')
                $('#countSttNew').text('Tạo mới (0)')
                $('#countSttReject').text('Từ chối (0)')
                $('#countSttApproved').text('Đã duyệt (0)')
                $('#countSttExpired').text('Hết hạn (0)')
                $('#countSttAll').text('Tất cả (0)')
                if (result.data.length > 0) {
                    for (var i = 0; i < result.data.length; i++) {
                        if (result.data[i].status == -1) {
                            $('#countSttAll').text('Tất cả (' + result.data[i].count + ')')
                        }
                        if (result.data[i].status == 0) {
                            $('#countSttNew').text('Tạo mới (' + result.data[i].count + ')')
                        }
                        if (result.data[i].status == 1) {
                            $('#countSttWaitPayment').text('Chờ thanh toán (' + result.data[i].count + ')')
                        }
                        if (result.data[i].status == 2) {
                            $('#countSttWaitApprove').text('Chờ duyệt (' + result.data[i].count + ')')
                        }
                        if (result.data[i].status == 3) {
                            $('#countSttApproved').text('Đã duyệt (' + result.data[i].count + ')')
                        }
                        if (result.data[i].status == 4) {
                            $('#countSttReject').text('Từ chối (' + result.data[i].count + ')')
                        }
                        if (result.data[i].status == 5) {
                            $('#countSttExpired').text('Hết hạn (' + result.data[i].count + ')')
                        }
                    }
                }
            }
        });
    },
    OnChangeInput: function () {
        clearTimeout(timer);
        timer = setTimeout(function () {
            _log_transfer_money.OnPaging(1);
        }, 1500);
    },
    OnChangeUserCreate: function (value) {
        var searchobj = this.SearchParam;
        var listId = value.split(',')
        searchobj.createByIds = []
        listUserCreate = []
        for (var i = 0; i < listId.length; i++) {
            searchobj.createByIds.push(parseInt(listId[i]))
        }
        listUserCreate = searchobj.createByIds
        searchobj.currentPage = 1;
        this.SearchParam = searchobj;
        this.Search(searchobj);
    },
    OnChangeUserApprove: function (value) {
        var searchobj = this.SearchParam;
        listUserApprove = []
        searchobj.approverIds = []
        //var listId = value.split(',')
        //for (var i = 0; i < listId.length; i++) {
        //    searchobj.approverIds.push(parseInt(listId[i]))
        //}
        searchobj.approverIds = $('#createdBy').select2("val")
        listUserApprove = searchobj.approverIds
        searchobj.currentPage = 1;
        this.SearchParam = searchobj;
        this.Search(searchobj);
    },
    OnSearchStatus: function (status) {
        isResetTab = false
        var objSearch = this.SearchParam;
        objSearch.currentPage = 1;
        objSearch.statusChoose = status;
        this.Search(objSearch, false);
        this.SetActive(status)
    },
    Search: function (input, is_count_status = true) {
        window.scrollTo(0, 0);
        $('#imgLoading').show();
        _global_function.AddLoading()
        $.ajax({
            url: "/Funding/Search",
            type: "Post",
            data: input,
            success: function (result) {
                _global_function.RemoveLoading()
                $('#imgLoading').hide();
                $('#grid_data').html(result);
            }
        });
        if (is_count_status) {
            this.OnCountStatus()
            this.SetActive(-1)
        }
    },
    Approve: function () {
        _msgconfirm.openDialog("Phê duyệt nạp quỹ", "Bạn có chắc muốn phê duyệt mã giao dịch " + $('#transNo').val() + " không?", function () {
            $.ajax({
                url: "/Funding/Approve",
                type: "Post",
                data: { 'transNo': $('#transNo').val(), 'contractPayId': $('#contractPayId').val() },
                success: function (result) {
                    if (result.isSuccess === true) {
                        _msgalert.success(result.message);
                        $.magnificPopup.close();
                        setTimeout(function () {
                            window.location.href = "/Funding/Index"
                        }, 1000)
                    } else {
                        _msgalert.error(result.message);
                    }
                }
            });
        });
    },
    DeleteContactPay: function (contractPayId, billNo) {
        _msgconfirm.openDialog("Xác nhận xóa phiếu thu", "Bạn có chắc muốn xóa phiếu thu " + billNo + " không?", function () {
            $.ajax({
                url: "/Funding/DeleteContactPay",
                type: "Post",
                data: { 'contractPayId': contractPayId },
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
    AddContract: function (depositId) {
        let title = 'Thêm phiếu thu';
        let url = '/Funding/AddContractPay?depositHistotyId=' + depositId;
        var param = {
        };
        _magnific.OpenSmallPopup(title, url, param);
    },
    Reject: function (depositId) {
        let title = 'Từ chối duyệt yêu cầu';
        let url = '/Funding/Reject?depositHistotyId=' + depositId;
        var param = {
        };
        _magnific.OpenSmallPopup(title, url, param);
    },
    RejectDeposit: function () {
        if ($('#note').val() === null || $('#note').val() === undefined || $('#note').val() === '') {
            _msgalert.error('Vui lòng nhập lý do từ chối');
            return
        }
        _global_function.AddLoading()
        $.ajax({
            url: "/Funding/RejectDeposit",
            type: "Post",
            data: { 'transNo': $('#transNo').val(), 'note': $('#note').val() },
            success: function (result) {
                _global_function.RemoveLoading()
                if (result.isSuccess === true) {
                    _msgalert.success(result.message);
                    $.magnificPopup.close();
                    setTimeout(function () {
                        window.location.href = "/Funding/Index"
                    }, 1000)
                } else {
                    _msgalert.error(result.message);
                }
            }
        });
    },
    SetActive: function (status) {
        var objSearch = this.SearchParam
        if (!isResetTab && objSearch.currentPage > 1)
            return
        let status_choose = $('#status').val()
        $('#countSttAll').removeClass('active')
        $('#countSttNew').removeClass('active')
        $('#countSttWaitPayment').removeClass('active')
        $('#countSttWaitApprove').removeClass('active')
        $('#countSttApproved').removeClass('active')
        $('#countSttReject').removeClass('active')
        if (status == -1) {
            $('#countSttAll').addClass('active')
        }
        if (status == 0) {
            $('#countSttNew').addClass('active')
        }
        if (status == 1) {
            $('#countSttWaitPayment').addClass('active')
            if (status_choose !== undefined && status_choose !== null && status_choose !== '' && status_choose !== '1')
                $('#countSttWaitPayment').addClass('disabled-a-tag')
            else
                $('#countSttWaitPayment').removeClass('disabled-a-tag')
        }
        if (status == 2) {
            $('#countSttWaitApprove').addClass('active')
            if (status_choose !== undefined && status_choose !== null && status_choose !== '' && status_choose !== '2')
                $('#countSttWaitPayment').addClass('disabled-a-tag')
            else
                $('#countSttWaitPayment').removeClass('disabled-a-tag')
        }
        if (status == 3) {
            $('#countSttApproved').addClass('active')
            if (status_choose !== undefined && status_choose !== null && status_choose !== '' && status_choose !== '3')
                $('#countSttWaitPayment').addClass('disabled-a-tag')
            else
                $('#countSttWaitPayment').removeClass('disabled-a-tag')
        }
        if (status == 4) {
            $('#countSttReject').addClass('active')
            if (status_choose !== undefined && status_choose !== null && status_choose !== '' && status_choose !== '4')
                $('#countSttWaitPayment').addClass('disabled-a-tag')
            else
                $('#countSttWaitPayment').removeClass('disabled-a-tag')
        }
    },
    SelectContractPay: function () {
        let contractId = $('#contractPay_id').val()
        if (contractPays !== null && contractPays !== undefined) {
            for (var i = 0; i < contractPays.length; i++) {
                if (contractPays[i].Id === parseInt(contractId)) {
                    $('#content_add_contract').text(': ' + contractPays[i].Note)
                    var number = parseFloat(contractPays[i].Amount, 10);
                    $('#amount_contract').text(': ' + number.toLocaleString())
                    if (contractPays[i].PayStatus === 0)
                        $('#paymentStatus_add_contract').text(': ' + 'Bot duyệt')
                    if (contractPays[i].PayStatus === 1)
                        $('#paymentStatus_add_contract').text(': ' + 'Kế toán duyệt')
                    if (contractPays[i].PayStatus === 2)
                        $('#paymentStatus_add_contract').text(': ' + 'Tạo mới')
                    break;
                }
            }
        }
    },
    CreateContractPay: function (depositId) {
        _global_function.AddLoading()
        $.ajax({
            url: "/Funding/AddContractPayJson",
            type: "Post",
            data: {
                'depositId': depositId, 'contractPayId': $('#contractPay_id').val(), 'totalAmount': $('#amount_add_contract_get').val()
            },
            success: function (result) {
                _global_function.RemoveLoading()
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
    showHideColumn: function () {
        $('.checkbox-tb-column').each(function () {
            let seft = $(this);
            let id = seft.data('id');
            if (seft.is(':checked')) {
                $('td:nth-child(' + id + '),th:nth-child(' + id + ')').removeClass('mfp-hide');
            } else {
                $('td:nth-child(' + id + '),th:nth-child(' + id + ')').addClass('mfp-hide');
            }
        });
    },
    checkShowHide: function () {
        if (fields.STT === true) {
            $('#sttDisplay').prop('checked', true);
        } else {
            $('#sttDisplay').prop('checked', false);
        }
        if (fields.TransNo === true) {
            $('#transNoDisplay').prop('checked', true);
        } else {
            $('#transNoDisplay').prop('checked', false);
        }
        if (fields.Title === true) {
            $('#titleDisplay').prop('checked', true);
        } else {
            $('#titleDisplay').prop('checked', false);
        }
        if (fields.TransType === true) {
            $('#transTypeDisplay').prop('checked', true);
        } else {
            $('#transTypeDisplay').prop('checked', false);
        }
        if (fields.Price === true) {
            $('#priceDisplay').prop('checked', true);
        } else {
            $('#priceDisplay').prop('checked', false);
        }
        if (fields.ServiceType === true) {
            $('#serviceTypeDisplay').prop('checked', true);
        } else {
            $('#serviceTypeDisplay').prop('checked', false);
        }
        if (fields.PaymentType === true) {
            $('#paymentTypeDisplay').prop('checked', true);
        } else {
            $('#paymentTypeDisplay').prop('checked', false);
        }
        if (fields.TransactionDate === true) {
            $('#transactionDateDisplay').prop('checked', true);
        } else {
            $('#transactionDateDisplay').prop('checked', false);
        }
        if (fields.CreateBy === true) {
            $('#createdByDisplay').prop('checked', true);
        } else {
            $('#createdByDisplay').prop('checked', false);
        }
        if (fields.Approver === true) {
            $('#approverDisplay').prop('checked', true);
        } else {
            $('#approverDisplay').prop('checked', false);
        }
        if (fields.ApproveDate === true) {
            $('#verifyDateDisplay').prop('checked', true);
        } else {
            $('#verifyDateDisplay').prop('checked', false);
        }
        if (fields.Status === true) {
            $('#statusDisplay').prop('checked', true);
        } else {
            $('#statusDisplay').prop('checked', false);
        }
    },
    changeSetting: function (position) {
        this.showHideColumn();
        switch (position) {
            case 1:
                if ($('#sttDisplay').is(":checked")) {
                    fields.STT = true
                } else {
                    fields.STT = false
                }
                this.eraseCookie(cookieName);
                this.setCookie(cookieName, JSON.stringify(fields), 10);
                break;
            case 2:
                if ($('#transNoDisplay').is(":checked")) {
                    fields.TransNo = true
                } else {
                    fields.TransNo = false
                }
                this.eraseCookie(cookieName);
                this.setCookie(cookieName, JSON.stringify(fields), 10);
                break;
            case 3:
                if ($('#titleDisplay').is(":checked")) {
                    fields.Title = true
                } else {
                    fields.Title = false
                }
                this.eraseCookie(cookieName);
                this.setCookie(cookieName, JSON.stringify(fields), 10);
                break;
            case 4:
                if ($('#transTypeDisplay').is(":checked")) {
                    fields.TransType = true
                } else {
                    fields.TransType = false
                }
                this.eraseCookie(cookieName);
                this.setCookie(cookieName, JSON.stringify(fields), 10);
                break;
            case 5:
                if ($('#priceDisplay').is(":checked")) {
                    fields.Price = true
                } else {
                    fields.Price = false
                }
                this.eraseCookie(cookieName);
                this.setCookie(cookieName, JSON.stringify(fields), 10);
                break;
            case 6:
                if ($('#serviceTypeDisplay').is(":checked")) {
                    fields.ServiceType = true
                } else {
                    fields.ServiceType = false
                }
                this.eraseCookie(cookieName);
                this.setCookie(cookieName, JSON.stringify(fields), 10);
                break;
            case 7:
                if ($('#paymentTypeDisplay').is(":checked")) {
                    fields.PaymentType = true
                } else {
                    fields.PaymentType = false
                }
                this.eraseCookie(cookieName);
                this.setCookie(cookieName, JSON.stringify(fields), 10);
                break;
            case 8:
                if ($('#transactionDateDisplay').is(":checked")) {
                    fields.TransactionDate = true
                } else {
                    fields.TransactionDate = false
                }
                this.eraseCookie(cookieName);
                this.setCookie(cookieName, JSON.stringify(fields), 10);
                break;
            case 9:
                if ($('#createdByDisplay').is(":checked")) {
                    fields.CreateBy = true
                } else {
                    fields.CreateBy = false
                }
                this.eraseCookie(cookieName);
                this.setCookie(cookieName, JSON.stringify(fields), 10);
                break;
            case 10:
                if ($('#verifyDateDisplay').is(":checked")) {
                    fields.ApproveDate = true
                } else {
                    fields.ApproveDate = false
                }
                this.eraseCookie(cookieName);
                this.setCookie(cookieName, JSON.stringify(fields), 10);
                break;
            case 11:
                if ($('#approverDisplay').is(":checked")) {
                    fields.Approver = true
                } else {
                    fields.Approver = false
                }
                this.eraseCookie(cookieName);
                this.setCookie(cookieName, JSON.stringify(fields), 10);
                break;
            case 12:
                if ($('#statusDisplay').is(":checked")) {
                    fields.Status = true
                } else {
                    fields.Status = false
                }
                this.eraseCookie(cookieName);
                this.setCookie(cookieName, JSON.stringify(fields), 10);
                break;
            default:
        }
    },
    setCookie: function (name, value, days) {
        var expires = "";
        if (days) {
            var date = new Date();
            date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
            expires = "; expires=" + date.toUTCString();
        }
        document.cookie = name + "=" + (value || "") + expires + "; path=/";
    },
    getCookie: function (name) {
        var nameEQ = name + "=";
        var ca = document.cookie.split(';');
        for (var i = 0; i < ca.length; i++) {
            var c = ca[i];
            while (c.charAt(0) == ' ') c = c.substring(1, c.length);
            if (c.indexOf(nameEQ) == 0) return c.substring(nameEQ.length, c.length);
        }
        return null;
    },
    eraseCookie: function (name) {
        document.cookie = name + '=; Path=/; Expires=Thu, 01 Jan 1970 00:00:01 GMT;';
    },
};



















