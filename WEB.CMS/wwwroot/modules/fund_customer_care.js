let isResetTab = false;
$(document).ready(function () {
    _fund_customer_care.Init();
});
var _fund_customer_care = {
    Init: function () {
        var input = _fund_customer_care.GetList();
        _fund_customer_care.Search(input);
    },
    GetList: function () {
        var objSearch = _fund_customer_care.GetParam();
        return objSearch;
    },
    Search: function (input, is_count_status = true, isClickSearch = false) {
        window.scrollTo(0, 0);
        _global_function.AddLoading()
        $.ajax({
            url: "/FundCustomerCare/Search",
            type: "Post",
            data: input,
            success: function (result) {
                _global_function.RemoveLoading()
                $('#grid_data').html(result);
                if (isClickSearch) {
                    var modelCache = _payment_request_service.GetParam()
                    _payment_request_service.SetCacheFilter(modelCache)
                } else {

                }

            }
        });
        if (is_count_status) {
            this.OnCountStatus()
            this.SetActive(-1)
        }
    },
    Add: function (serviceId, serviceType, supplierId, amount, orderId, serviceCode, clientId, amount_supplier_refund, payment_request_type) {
        let title = 'Thêm phiếu yêu cầu chi';
        let url = '/PaymentRequest/AddNew';
        var param = {
            'serviceId': serviceId,
            'serviceType': serviceType,
            'serviceCode': serviceCode,
            'supplierId': supplierId,
            'amount_supplier_refund': amount_supplier_refund,
            'amount': amount,
            'orderId': orderId,
            'clientId': clientId,
            'payment_request_type': 5,
        };
        _magnific.OpenSmallPopup(title, url, param);
    },
    Edit: function (paymentRequestId, serviceId, serviceType, supplierId, amount, orderId, clientId, amount_supplier_refund) {
        let title = 'Chỉnh sửa phiếu yêu cầu chi';
        let url = '/PaymentRequest/Edit?paymentRequestId=' + paymentRequestId + "&service_type=";
        var param = {
            'serviceId': serviceId,
            'serviceType': serviceType,
            'amount_supplier_refund': amount_supplier_refund,
            'supplierId': supplierId,
            'amount': amount,
            'orderId': orderId,
            'clientId': clientId,
        };
        _magnific.OpenSmallPopup(title, url, param);
    },
    EditAdmin: function (paymentRequestId, serviceId, serviceType, supplierId, amount, orderId, clientId, amount_supplier_refund) {
        let title = 'Admin chỉnh sửa phiếu yêu cầu chi';
        let url = '/PaymentRequest/EditAdmin?paymentRequestId=' + paymentRequestId + "&service_type=";
        var param = {
            'serviceId': serviceId,
            'serviceType': serviceType,
            'supplierId': supplierId,
            'amount_supplier_refund': amount_supplier_refund,
            'amount': amount,
            'orderId': orderId,
            'clientId': clientId,
        };
        _magnific.OpenSmallPopup(title, url, param);
    },
    OnSearchStatus: function (status) {
        $('#status').val(-1)
        isResetTab = false
        var objSearch = _fund_customer_care.GetParam()
        objSearch.currentPage = 1;
        objSearch.status = status;
        _fund_customer_care.Search(objSearch, false);
        _fund_customer_care.SetActive(status)
    },
    OnCountStatus: function () {
        var objSearch = _fund_customer_care.GetParam();
        _global_function.AddLoading()
        $.ajax({
            url: "/PaymentRequest/CountStatus",
            type: "Post",
            data: objSearch,
            success: function (result) {
                _global_function.RemoveLoading()
                $('#countSttDraft').text('Lưu nháp (0)')
                $('#countSttWaitDepartmentLeadApprove').text('Chờ TBP duyệt (0)')
                $('#countSttWaitDirectorApprove').text('Chờ KTT duyệt (0)')
                $('#countSttReject').text('Từ chối (0)')
                $('#countSttApprove').text('Chờ chi (0)')
                $('#countSttPayment').text('Đã chi (0)')
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
                            $('#countSttWaitDepartmentLeadApprove').text('Chờ TBP duyệt (' + result.data[i].total + ')')
                        }
                        if (result.data[i].status == 3) {
                            $('#countSttWaitDirectorApprove').text('Chờ KTT duyệt (' + result.data[i].total + ')')
                        }
                        if (result.data[i].status == 4) {
                            $('#countSttApprove').text('Chờ chi (' + result.data[i].total + ')')
                        }
                        if (result.data[i].status == 5) {
                            $('#countSttPayment').text('Đã chi (' + result.data[i].total + ')')
                        }
                    }
                }
            }
        });
    },
    OnPaging: function (value, isClickSearch) {
        var objSearch = _fund_customer_care.GetParam()
        objSearch.currentPage = value;
        this.SearchParam = objSearch
        _fund_customer_care.Search(objSearch, true, isClickSearch);
    },
    GetParam: function () {
     
        var objSearch = {
            paymentCode: null,
            content: null,
            serviceCode: null,
            orderNo: null,
            type: null,
            status: -1,
            statusMulti: null,
            typeMulti: [5],
            paymentTypeMulti: null,
            paymentType: null,
            createByIds: null,
            verifyByIds: null,
            clientId: null,
            supplierId: null,
            currentPage: 1,
            pageSize: 20
        }
      
        return objSearch
    },
    SetActive: function (status) {
       
        let status_choose = $('#status').val()
        $('#countSttAll').removeClass('active')
        $('#countSttDraft').removeClass('active')
        $('#countSttWaitDepartmentLeadApprove').removeClass('active')
        $('#countSttWaitDirectorApprove').removeClass('active')
        $('#countSttApprove').removeClass('active')
        $('#countSttReject').removeClass('active')
        $('#countSttPayment').removeClass('active')
        if (status == -1) {
            $('#countSttAll').addClass('active')
        }
        if (status == 0) {
            $('#countSttDraft').addClass('active')
            if (status_choose !== undefined && status_choose !== null && status_choose !== '' && status_choose !== '1')
                $('#countSttDraft').addClass('disabled-a-tag')
            else
                $('#countSttDraft').removeClass('disabled-a-tag')
        }
        if (status == 2) {
            $('#countSttWaitDepartmentLeadApprove').addClass('active')
            if (status_choose !== undefined && status_choose !== null && status_choose !== '' && status_choose !== '2')
                $('#countSttWaitDepartmentLeadApprove').addClass('disabled-a-tag')
            else
                $('#countSttWaitDepartmentLeadApprove').removeClass('disabled-a-tag')
        }
        if (status == 3) {
            $('#countSttWaitDirectorApprove').addClass('active')
            if (status_choose !== undefined && status_choose !== null && status_choose !== '' && status_choose !== '3')
                $('#countSttWaitDirectorApprove').addClass('disabled-a-tag')
            else
                $('#countSttWaitDirectorApprove').removeClass('disabled-a-tag')
        }
        if (status == 1) {
            $('#countSttReject').addClass('active')
            if (status_choose !== undefined && status_choose !== null && status_choose !== '' && status_choose !== '1')
                $('#countSttReject').addClass('disabled-a-tag')
            else
                $('#countSttReject').removeClass('disabled-a-tag')
        }
        if (status == 4) {
            $('#countSttApprove').addClass('active')
            if (status_choose !== undefined && status_choose !== null && status_choose !== '' && status_choose !== '4')
                $('#countSttApprove').addClass('disabled-a-tag')
            else
                $('#countSttApprove').removeClass('disabled-a-tag')
        }
        if (status == 5) {
            $('#countSttPayment').addClass('active')
            if (status_choose !== undefined && status_choose !== null && status_choose !== '' && status_choose !== '4')
                $('#countSttPayment').addClass('disabled-a-tag')
            else
                $('#countSttPayment').removeClass('disabled-a-tag')
        }
    },
    PopupYCChi: function (id, type) {
        let title = 'Yêu cầu chi';
        if (type == 1) {
            title = 'Yêu cầu chi hoàn trả khách hàng'
        }
        let url = '/SetService/SendYCChi';
        var profit = $('#operator-order-profit').attr('data-profit');
        if (profit == undefined) {
            profit = $('#operator-total-profit').attr('data-profit');

        }

        let param = {
            id: id,
            profit: profit,
            type: type,
        };
        _magnific.OpenSmallPopup(title, url, param);
    },
    ClientOnPaging: function (value) {

        var MaKH_data = $('#client').select2("val");

        let _searchModel = {
            MaKH: null,
            CreatedBy: null,
            UserId: null,
            TenKH: null,
            Email: null,
            Phone: null,
            AgencyType: null,
            ClientType: null,
            PermissionType: null,
            CreateDate: null,
            EndDate: null,
            MinAmount: null,
            MaxAmount: null,
            PageIndex: value,
            PageSize: $("#selectPaggingOptions").find(':selected').val(),
        };
        if (MaKH_data != null) { _searchModel.MaKH = MaKH_data[0] }
      
        _searchModel.currentPage = value;

        _fund_customer_care.SearchClient(_searchModel);
    },
    onSelectPageSize: function (value) {


        var MaKH_data = $('#client').select2("val");
    
        let _searchModel = {
            MaKH: null,
            UserId: null,
            TenKH: null,
            Email: null,
            Phone: null,
            AgencyType: null,
            ClientType: null,
            PermissionType: null,
            CreateDate: null,
            EndDate: null,
            MinAmount: null,
            MaxAmount: null,
            PageIndex: 1,
            PageSize: value,
        };
        if (MaKH_data != null) { _searchModel.MaKH = MaKH_data[0] }

        _searchModel.PageSize = $("#selectPaggingOptions").find(':selected').val()
        _fund_customer_care.SearchClient(_searchModel);
    },
    SearchClient: function (input) {
        $.ajax({
            url: "/FundCustomerCare/ListClient",
            type: "Post",
            data: input,
            success: function (result) {
                $('#imgLoading_client').hide();
                $('#grid_data_client').html(result);
         
                $('#selectPaggingOptions').val(input.PageSize).attr("selected", "selected");

            }
        });
    },
    SearchDataClient: function () {

        var MaKH_data = $('#client').select2("val");

        let _searchModel = {
            MaKH: null,
            UserId: null,
            TenKH: null,
            Email: null,
            Phone: null,
            AgencyType: null,
            ClientType: null,
            PermissionType: null,
            CreateDate: null,
            EndDate: null,
            MinAmount: null,
            MaxAmount: null,
            PageIndex: 1,
            PageSize: $("#selectPaggingOptions").find(':selected').val(),
        };
        if (MaKH_data != null) { _searchModel.MaKH = MaKH_data[0] }
        if (_searchModel.PageSize == "" || _searchModel.PageSize == undefined) _searchModel.PageSize=20
        this.SearchClient(_searchModel);
    },
    PopupClient: function () {
        let title = 'Danh sách quỹ';
       
        let url = '/FundCustomerCare/FundCustomerCareClient';
        let param = {

        };
        _magnific.OpenSmallPopup(title, url, param);
    },
    clientSuggestion: function () {
      
    },
};