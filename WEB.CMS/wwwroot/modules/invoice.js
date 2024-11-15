let isPickerCreatedInvoiceRequest = false
let isPickerPayment = false
let isPickerApproved = false
let listStatusType = []
let isResetTab = true
let listUserCreate = []
$(document).ready(function () {
    $('input').attr('autocomplete', 'off');
    $('input').keyup(function (e) {
        if (e.which === 13) {
            _invoice_service.OnPaging(1);
        }
    });
    $('input').attr('autocomplete', 'off');
    var SearchParam = _invoice_service.GetParam()
    _invoice_service.Init(SearchParam);
    $(".input").on('keyup', function (e) {
        if (e.key === 'Enter' || e.keyCode === 13) {
            _invoice_service.OnPaging(1)
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
    $("#createdBy").select2({
        theme: 'bootstrap4',
        placeholder: "Người tạo",
        ajax: {
            url: "/PaymentRequest/GetUserSuggestionList",
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
    $("#approveBy").select2({
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
                            text: item.fullname,
                            id: item.id,
                        }
                    })
                };
            },
            cache: true
        }
    });
})
var _invoice_service = {
    Init: function (objSearch) {
        $('#divClient').show()
        this.SearchParam = objSearch;
        this.Search(objSearch);
    },
    ActionSearch: function () {
        isResetTab = true
        this.OnPaging(1)
    },
    GetParam: function () {
        var objSearch = {
            invoiceCode: $('#billNo').val(),
            invoiceNo: $('#invoiceNo').val(),
            orderNo: $('#orderNo').val(),
            invoiceRequestNo: $('#content').val(),
            createByIds: $('#createdBy').select2("val"),
            clientId: $('#token-input-client').val(),
            currentPage: 1,
            pageSize: 20
        }
        if ($('#filter_date_create_daterangepicker').data('daterangepicker') !== undefined &&
            $('#filter_date_create_daterangepicker').data('daterangepicker') != null && isPickerCreatedInvoiceRequest) {
            objSearch.createDateFromStr = $('#filter_date_create_daterangepicker').data('daterangepicker').startDate._d.toLocaleDateString("en-GB");
            objSearch.createDateToStr = $('#filter_date_create_daterangepicker').data('daterangepicker').endDate._d.toLocaleDateString("en-GB");
        } else {
            objSearch.createDateFromStr = null
            objSearch.createDateToStr = null
        }
        if ($('#filter_export_date').data('daterangepicker') !== undefined &&
            $('#filter_export_date').data('daterangepicker') != null && isPickerPayment) {
            objSearch.exportDateFromStr = $('#filter_export_date').data('daterangepicker').startDate._d.toLocaleDateString("en-GB");
            objSearch.exportDateToStr = $('#filter_export_date').data('daterangepicker').endDate._d.toLocaleDateString("en-GB");
        } else {
            objSearch.exportDateFromStr = null
            objSearch.exportDateToStr = null
        }
        return objSearch
    },
    BackToList: function () {
        window.location.href = '/Invoice/Index'
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
            url: "/Invoice/ExportExcel",
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
            }
        });
    },
    Search: function (input, is_count_status = true) {
        window.scrollTo(0, 0);
        _global_function.AddLoading()
        $.ajax({
            url: "/Invoice/Search",
            type: "Post",
            data: input,
            success: function (result) {
                _global_function.RemoveLoading()
                //$('#imgLoading').hide();
                $('#grid_data_invoice').html(result);
            }
        });
    },
    OnPaging: function (value) {
        var objSearch = this.GetParam()
        objSearch.currentPage = value;
        this.SearchParam = objSearch
        this.Search(objSearch);
    },
    OnChangeAccountClientId: function (value) {
        listUserCreate = []
        var searchobj = this.SearchParam;
        var listId = value.split(',')
        searchobj.createByIds = []
        for (var i = 0; i < listId.length; i++) {
            searchobj.createByIds.push(parseInt(listId[i]))
        }
        searchobj.currentPage = 1;
        listUserCreate = searchobj.createByIds
        this.SearchParam = searchobj;
        this.Search(searchobj);
    },
    OnChangeClient: function (value) {
        var searchobj = this.SearchParam;
        var listId = value.split(',')
        searchobj.clientId = null
        for (var i = 0; i < listId.length; i++) {
            searchobj.createByIds.push(parseInt(listId[i]))
        }
        searchobj.currentPage = 1;
        this.SearchParam = searchobj;
        this.Search(searchobj);
    },
    Add: function () {
        let title = 'Thêm hóa đơn';
        let url = '/Invoice/Add';
        var param = {
        };
        _magnific.OpenSmallPopup(title, url, param);
    },
    Edit: function (invoiceId) {
        let title = 'Chỉnh sửa hóa đơn';
        let url = '/Invoice/Edit?invoiceId=' + invoiceId;
        var param = {
        };
        _magnific.OpenSmallPopup(title, url, param);
    },
    Delete: function (invoiceId) {
        let invoiceCode = $('#invoiceCode').val()
        _msgconfirm.openDialog("Xác nhận xóa hóa đơn",
            "Bạn có chắc muốn xóa hóa đơn " + invoiceCode + " không?",
            function () {
                $.ajax({
                    url: "/Invoice/Delete",
                    type: "Post",
                    data: { 'invoiceId': invoiceId },
                    success: function (result) {
                        if (result.isSuccess === true) {
                            _msgalert.success(result.message);
                            $.magnificPopup.close();
                            setTimeout(function () {
                                window.location.href = "/Invoice/Index"
                            }, 1000)
                        } else {
                            _msgalert.error(result.message);
                        }
                    }
                });
            });
    },
    FileAttachment: function (data_id, type) {
        $.ajax({
            url: "/AttachFile/Widget",
            type: "Post",
            data: { data_id: data_id, type: type, read_only: true },
            success: function (result) {
                $('.attachment-file-block').html(result);
            }
        });
    },
}