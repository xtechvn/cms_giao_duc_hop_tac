let listOrderStatus = [];
let listServiceBC = [];
let PageSize = 20;
$(document).ready(function () {
    _ReportDepartment.Init();

    var date = new Date();
    var dd = date.getDate();
    var MM = date.getMonth() + 1;
    var yyyy = date.getFullYear();

    $("body").on('click', ".ks-open", function (ev, picker) {
        $('.ks-item').show();
        $('.ks-open').addClass('ks-edit');
        $('.ks-open').removeClass('ks-open');
    });
    $("body").on('click', ".ks-edit", function (ev, picker) {
        $('.ks-item').hide();
        $('.ks-edit').addClass('ks-open');
        $('.ks-edit').removeClass('ks-edit');
    });
    /* $("#filter_date_daterangepicker").val(('01/' + MM + '/' + yyyy) + ' - ' + (dd + '/' + MM + '/' + yyyy));*/
    //$('#CreateDate').daterangepicker({
    //    "autoApply": true,
    //    "startDate": ('01/' + MM + '/' + yyyy),
    //    "endDate": (dd + '/' + MM + '/' + yyyy),
    //    locale: {
    //        format: 'DD/MM/YYYY',
    //        cancelLabel: 'Clear'
    //    }
    //});
    $("#ClientId").select2({
        theme: 'bootstrap4',
        placeholder: "Thông tin KH",
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

                // Query parameters will be ?search=[term]&type=public
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
    $("#SalerId").select2({
        theme: 'bootstrap4',
        placeholder: "Thông tin NV",
        hintText: "Nhập từ khóa tìm kiếm",
        searchingText: "Đang tìm kiếm...",
        maximumSelectionLength: 1,
        ajax: {
            url: "/Order/UserSuggestion",
            type: "post",
            dataType: 'json',
            delay: 250,
            data: function (params) {
                var query = {
                    txt_search: params.term,
                }

                // Query parameters will be ?search=[term]&type=public
                return query;
            },
            processResults: function (response) {
                return {
                    results: $.map(response.data, function (item) {
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
    $("#SupplierId").select2({
        theme: 'bootstrap4',
        placeholder: "Thông tin NCC",
        hintText: "Nhập từ khóa tìm kiếm",
        searchingText: "Đang tìm kiếm...",
        maximumSelectionLength: 1,
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
                            text: item.fullname,
                            id: item.id,
                        }
                    })
                };
            },
            cache: true
        }
    });
    $("#SalerId_sale").select2({
        theme: 'bootstrap4',
        placeholder: "Thông tin NV",
        hintText: "Nhập từ khóa tìm kiếm",
        searchingText: "Đang tìm kiếm...",
        maximumSelectionLength: 1,
        ajax: {
            url: "/Order/UserSuggestion",
            type: "post",
            dataType: 'json',
            delay: 250,
            data: function (params) {
                var query = {
                    txt_search: params.term,
                }

                // Query parameters will be ?search=[term]&type=public
                return query;
            },
            processResults: function (response) {
                return {
                    results: $.map(response.data, function (item) {
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
    $('#Type').on ("select2:select", function (e) {
        var element = $(this)
        var value = element.find(':selected').val()
        switch (parseInt(value)) {
            case 1: {
                _report_operator.ShowFilterGeneral()
                $('#ServiceType_BC').hide();
                $('#HINHTHUCTT_BC').show();
                $('#DepartmentType').closest('.form-group').show()
                $('#filter-departmentSale').hide();
                _ReportDepartment.SearchData()
            } break;
            case 2: {
                _report_operator.ShowFilterGeneral()
                $('#ServiceType_BC').show();
                $('#HINHTHUCTT_BC').hide();
                $('#DepartmentType').closest('.form-group').show()
                $('#filter-departmentSale').hide();
                _ReportDepartment.SearchData()
             

            } break;
            case 3: {
                $('#filter-departmentSale').hide();
                _report_operator.ShowFilterOperator()
                _report_operator.Initialization()
            } break;
            case 4: {
                $('#ServiceType_BC').hide();
                $('#HINHTHUCTT_BC').hide();
                $('#filter-sale').hide();
                $('#filter-operator').hide();
                $('#DepartmentType').hide();
                $('#filter-departmentSale').show();
                $('#DepartmentType').closest('.form-group').hide()
                _ReportDepartment.SearchData()
               
            } break;
        }


    });

    const selectBtnOrderStatus = document.querySelector(".select-btn-OrderStatus-type")
    const itemsOrderStatus = document.querySelectorAll(".item-OrderStatus-type");

    const selectBtnOrderStatussale = document.querySelector(".select-btn-OrderStatus-sale-type")
    const itemsOrderStatussale = document.querySelectorAll(".item-OrderStatus-sale-type");

    const selectBtnServiceBC = document.querySelector(".select-service-type-BC");
    const itemsServiceBC = document.querySelectorAll(".item-services-BC");

    $(document).click(function (event) {

        var $target = $(event.target);
        if (!$target.closest('#select-btn-OrderStatus-type').length) {//checkbox_trans_type_
            if ($('#list-item-OrderStatus').is(":visible") && !$target[0].id.includes('OrderStatus_type_text') && !$target[0].id.includes('OrderStatus_type')
                && !$target[0].id.includes('list-item-OrderStatus') && !$target[0].id.includes('checkbox_OrderStatus_type')) {
                selectBtnOrderStatus.classList.toggle("open");
            }
        }
        if (!$target.closest('#select-btn-OrderStatus-sale-type').length) {//checkbox_trans_type_
            if ($('#list-item-OrderStatus-sale').is(":visible") && !$target[0].id.includes('OrderStatus_sale_type_text') && !$target[0].id.includes('OrderStatus_sale_type')
                && !$target[0].id.includes('list-item-OrderStatus-sale') && !$target[0].id.includes('checkbox_OrderStatus_sale_type')) {
                selectBtnOrderStatussale.classList.toggle("open");
            }
        }
        if (!$target.closest('#ServiceTypeBC').length) {
            if ($('#list-item-service-type-BC').is(":visible") && !$target[0].id.includes('service_data_BC_') && !$target[0].id.includes('service_type_BC_')
                && !$target[0].id.includes('list-item-service-type-BC') && !$target[0].id.includes('checkbox_service_type_BC_')) {
                selectBtnServiceBC.classList.toggle("open");
            }
        }
    });
    selectBtnOrderStatus.addEventListener("click", (e) => {
        e.preventDefault();
        selectBtnOrderStatus.classList.toggle("open");
    });
    selectBtnOrderStatussale.addEventListener("click", (e) => {
        e.preventDefault();
        selectBtnOrderStatussale.classList.toggle("open");
    });
    selectBtnServiceBC.addEventListener("click", (e) => {
        e.preventDefault();
        selectBtnServiceBC.classList.toggle("open");
    });
    itemsOrderStatus.forEach(item => {
        item.addEventListener("click", () => {
            item.classList.toggle("checked");
            let checked = document.querySelectorAll(".checked"),
                btnText = document.querySelector(".btn-text-OrderStatus-type");
            listOrderStatus = []
            let checked_list = []

            for (var i = 0; i < checked.length; i++) {
                id = checked[i].getAttribute('id')
                if (id.includes('OrderStatus_type_')) {
                    checked_list.push(checked[i]);
                    listOrderStatus.push(parseInt(id.replace('OrderStatus_type_', '')))
                }
            }

            if (checked_list && checked_list.length > 0) {
                btnText.innerText = `${checked_list.length} Selected`;
            } else {
                btnText.innerText = "Tất cả trạng thái";
            }
        });
    });
    itemsServiceBC.forEach(item => {
        item.addEventListener("click", () => {
            item.classList.toggle("checked");

            let checked = document.querySelectorAll("#list-item-service-type-BC .checked"),
                btnText = document.querySelector(".btn-text-service-BC");
            let checked_list = []
            listServiceBC = []
            for (var i = 0; i < checked.length; i++) {
                id = checked[i].getAttribute('id')
                if (listServiceBC && id.includes('service_data_BC_')) {
                    checked_list.push(checked[i]);
                    listServiceBC.push(parseInt(id.replace('service_data_BC_', '')))
                }


            }
            if (listServiceBC.length > 0) {
                btnText.innerText = `${listServiceBC.length} Selected`;
            } else {
                btnText.innerText = "Tất cả dịch vụ";
            }


        });
    })
    itemsOrderStatussale.forEach(item => {
        item.addEventListener("click", () => {
            item.classList.toggle("checked");
            let checked = document.querySelectorAll(".checked"),
                btnText = document.querySelector(".btn-text-OrderStatus-sale-type");
            listOrderStatus = []
            let checked_list = []

            for (var i = 0; i < checked.length; i++) {
                id = checked[i].getAttribute('id')
                if (id.includes('OrderStatus_sale_type_')) {
                    checked_list.push(checked[i]);
                    listOrderStatus.push(parseInt(id.replace('OrderStatus_sale_type_', '')))
                }
            }

            if (checked_list && checked_list.length > 0) {
                btnText.innerText = `${checked_list.length} Selected`;
            } else {
                btnText.innerText = "Tất cả trạng thái";
            }
        });
    });
});
let isPickerApprove = false;
let isPickerApprove2 = false;
let isPickerApprove3 = false;
let isPickerApprove4 = false;

var _ReportDepartment = {
    Init: function () {
        var date = new Date();
        var dd = date.getUTCDate();
        var MM = date.getMonth() + 1;
        var yyyy = date.getFullYear();
        objSearch = this.GetParam();
        objSearch.PageSize = 20;
 
        _ReportDepartment.Search(objSearch);
    },
    Search: function (input) {
        $('#imgLoading').show();
        $('#grid-data').html("");
        if (input.Type == 1) {
       
            if (input.DepartmentType == 1) {
                $('.SalerId').addClass('mfp-hide');
                $('.ClientId').addClass('mfp-hide');
                $('.SupplierId').addClass('mfp-hide');
                $.ajax({
                    url: "/ReportDepartment/SearchReportDepartment",
                    type: "post",
                    data: input,
                    success: function (result) {
                        $('#imgLoading').hide();
                        $('#grid-data').html(result);
                        $('#selectPaggingOptions').val(input.PageSize).attr("selected", "selected");
                    },
                });
            }
            if (input.DepartmentType == 2) {
         
                $('.SalerId').removeClass('mfp-hide');
                $('.ClientId').addClass('mfp-hide');
                $('.SupplierId').addClass('mfp-hide');
                $.ajax({
                    url: "/ReportDepartment/SearchReportDepartmentsaler",
                    type: "post",
                    data: input,
                    success: function (result) {
                        $('#imgLoading').hide();
                        $('#grid-data').html(result);
                        $('#selectPaggingOptions').val(input.PageSize).attr("selected", "selected");
                    },
                });
            }
            if (input.DepartmentType == 3) {
                $('.SalerId').addClass('mfp-hide');
                $('.ClientId').addClass('mfp-hide');
                $('.SupplierId').removeClass('mfp-hide');
                $.ajax({
                    url: "/ReportDepartment/SearchReportDepartmentBySupplier",
                    type: "post",
                    data: input,
                    success: function (result) {
                        $('#imgLoading').hide();
                        $('#grid-data').html(result);
                        $('#selectPaggingOptions').val(input.PageSize).attr("selected", "selected");
                    },
                });
            }
            if (input.DepartmentType == 4) {
                $('.SalerId').addClass('mfp-hide');
                $('.ClientId').removeClass('mfp-hide');
                $('.SupplierId').addClass('mfp-hide');
                $.ajax({
                    url: "/ReportDepartment/SearchReportDepartmentClient",
                    type: "post",
                    data: input,
                    success: function (result) {
                        $('#imgLoading').hide();
                        $('#grid-data').html(result);
                        $('#selectPaggingOptions').val(input.PageSize).attr("selected", "selected");
                    },
                });
            }
        }
        if (input.Type == 2) {
            if (input.DepartmentType == 1) {
                $('.SalerId').addClass('mfp-hide');
                $('.ClientId').addClass('mfp-hide');
                $('.SupplierId').addClass('mfp-hide');
                $.ajax({
                    url: "/ReportDepartment/GetListDetailRevenueByDepartment",
                    type: "post",
                    data: input,
                    success: function (result) {
                        $('#imgLoading').hide();
                        $('#grid-data').html(result);
                        $('#selectPaggingOptions').val(input.PageSize).attr("selected", "selected");
                    },
                });
            }
            if (input.DepartmentType == 2) {
                $('.SalerId').removeClass('mfp-hide');
                $('.ClientId').addClass('mfp-hide');
                $('.SupplierId').addClass('mfp-hide');
                $.ajax({
                    url: "/ReportDepartment/GetListDetailRevenueByDepartmentsaler",
                    type: "post",
                    data: input,
                    success: function (result) {
                        $('#imgLoading').hide();
                        $('#grid-data').html(result);
                        $('#selectPaggingOptions').val(input.PageSize).attr("selected", "selected");
                    },
                });
            }
            if (input.DepartmentType == 3) {
                $('.SalerId').removeClass('mfp-hide');
                $('.ClientId').addClass('mfp-hide');
                $('.SupplierId').addClass('mfp-hide');
                $.ajax({
                    url: "/ReportDepartment/GetListDetailRevenueByDepartmentSupplier",
                    type: "post",
                    data: input,
                    success: function (result) {
                        $('#imgLoading').hide();
                        $('#grid-data').html(result);
                        $('#selectPaggingOptions').val(input.PageSize).attr("selected", "selected");
                    },
                });
            }
            if (input.DepartmentType == 4) {
                $('.SalerId').removeClass('mfp-hide');
                $('.ClientId').addClass('mfp-hide');
                $('.SupplierId').addClass('mfp-hide');
                $.ajax({
                    url: "/ReportDepartment/GetListDetailRevenueByDepartmentClient",
                    type: "post",
                    data: input,
                    success: function (result) {
                        $('#imgLoading').hide();
                        $('#grid-data').html(result);
                        $('#selectPaggingOptions').val(input.PageSize).attr("selected", "selected");
                    },
                });
            }
        }
        if (input.Type == 4) {
            var input2 = _ReportDepartment.GetParamBysaler();
            _ReportDepartment.SearchDepartmentBysaler(input2);
        }
       
    },
    SearchData: function () {
        var input = this.GetParam();
        if (input.PageSize == undefined) {
            input.PageSize = 20;
        }
        if (input.DepartmentType == 1 || input.DepartmentType == 2) {
            $('#DepartmentId').closest('.form-group').show()
        } else {
            $('#DepartmentId').closest('.form-group').hide()
        }
        this.Search(input);
    },
    GetParam: function () {
        var FromDate; //Ngày bat dau
        var ToDate; //Ngày hết hạn

        var FromDate; //Ngày bat dau
        var ToDate; //Ngày hết hạn

        var CreateDateFrom; //Ngày bat dau
        var CreateDateTo; //Ngày hết hạn

        var EndDateFrom; //Ngày bat dau
        var EndDateTo; //Ngày hết hạn

        if ($('#filter_date_daterangepicker').data('daterangepicker') !== undefined && $('#filter_date_daterangepicker').data('daterangepicker') != null && isPickerApprove) {
            FromDate = $('#filter_date_daterangepicker').data('daterangepicker').startDate._d.toLocaleDateString("en-GB");
            ToDate = $('#filter_date_daterangepicker').data('daterangepicker').endDate._d.toLocaleDateString("en-GB");
        } else {
            FromDate = null
            ToDate = null
        }
        if ($('#StartDate').data('daterangepicker') !== undefined && $('#StartDate').data('daterangepicker') != null && isPickerApprove2) {
            StartDateFrom = $('#StartDate').data('daterangepicker').startDate._d.toLocaleDateString("en-GB");
            StartDateTo = $('#StartDate').data('daterangepicker').endDate._d.toLocaleDateString("en-GB");
        } else {
            StartDateFrom = null
            StartDateTo = null
        }
        if ($('#EndDate').data('daterangepicker') !== undefined && $('#EndDate').data('daterangepicker') != null && isPickerApprove3) {
            EndDateFrom = $('#EndDate').data('daterangepicker').startDate._d.toLocaleDateString("en-GB");
            EndDateTo = $('#EndDate').data('daterangepicker').endDate._d.toLocaleDateString("en-GB");
        } else {
            EndDateFrom = null
            EndDateTo = null
        }
        if ($('#CreateDate').data('daterangepicker') !== undefined && $('#CreateDate').data('daterangepicker') != null && isPickerApprove4) {
            CreateDateFrom = $('#CreateDate').data('daterangepicker').startDate._d.toLocaleDateString("en-GB");
            CreateDateTo = $('#CreateDate').data('daterangepicker').endDate._d.toLocaleDateString("en-GB");
        } else {
            CreateDateFrom = null
            CreateDateTo = null
        }
        let _searchModel = {
            DepartmentType: $("#DepartmentType").val(),
            DepartmentId: $("#DepartmentId").val(),
            SupplierId: $("#SupplierId").val(),
            SalerId: $("#SalerId").val(),
            ClientId: $("#ClientId").val(),
            Type: $("#Type").val(),
            Vat: $("#Vat").val(),
            HINHTHUCTT: $("#HINHTHUCTT").val(),
            FromDate: FromDate,
            OrderStatus: listOrderStatus.toString(),
            ToDate: ToDate,
            ServiceType: listServiceBC.toString(),
            StartDateFromStr: StartDateFrom,
            StartDateToStr: StartDateTo,
            EndDateFromStr: EndDateFrom,
            EndDateToStr: EndDateTo,
            CreateDateFrom: CreateDateFrom,
            CreateDateTo: CreateDateTo,
            PageIndex: 1,
            PageSize: $("#selectPaggingOptions").find(':selected').val(),
        };
        return _searchModel;
    },
    OnPaging: function (value) {

        var objSearch = this.SearchParam
        objSearch = this.GetParam()
        objSearch.PageIndex = value

        this.Search(objSearch);
        $(document).load().scrollTop(0);
    },
    onSelectPageSize: function () {
        var objSearch = this.SearchParam;
        objSearch = this.GetParam();
        this.Search(objSearch);
        $(document).load().scrollTop(0);
    },

    Export: function () {
        $('#btnExport').prop('disabled', true);
        $('#icon-export').removeClass('fa-file-excel-o');
        $('#icon-export').addClass('fa-spinner fa-pulse');
        var searchModel = this.GetParam();
        _global_function.AddLoading()
        $.ajax({
            url: "/ReportDepartment/ExportExcel",
            type: "Post",
            data: searchModel,
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
    ExportListOrder: function () {
  
        objSearch = this.GetParam()

        objSearch.DepartmentId = $('#DepartmentIdOrder').val();
        objSearch.SalerId = $('#SalerIdOrder').val();
        _global_function.AddLoading()
        $.ajax({
            url: "/ReportDepartment/ExportExcelListOrrder",
            type: "Post",
            data: objSearch,
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

    OpenPopup: function (id,userid) {
        var objSearch = _ReportDepartment.GetParam();
        objSearch.DepartmentId = id;
        objSearch.SalerId = userid;
        let title = 'Danh sách đơn hàng';
        let url = '/ReportDepartment/SearchReportDepartmentListOrder';

        _magnific.OpenSmallPopup(title, url, objSearch);
    },
    OnPagingOrder: function (value) {

        var objSearch = this.SearchParam
        objSearch = this.GetParam()
        objSearch.PageIndex = value
        objSearch.DepartmentId = $('#DepartmentIdOrder').val();
        objSearch.SalerId = $('#SalerIdOrder').val();
        this.SearchDataListOrder(objSearch);
        $(document).load().scrollTop(0);
    },
    onSelectPageSizeOrder: function () {
        var objSearch = this.GetParam();
        objSearch.DepartmentId = $('#DepartmentIdOrder').val();
        objSearch.SalerId = $('#SalerIdOrder').val();
        this.SearchDataListOrder(objSearch);
        $(document).load().scrollTop(0);
    },
    SearchDataListOrder: function (input) {
        $.ajax({
            url: "/ReportDepartment/SearchReportDepartmentListOrder",
            type: "post",
            data: input,
            success: function (result) {
            
                $('#data-listOrder').html(result);
                $('#selectPaggingOptions').val(input.PageSize).attr("selected", "selected");
            },
        });
    },
    SearchDepartmentBysaler: function (input) {
        
        if (input.PageSize == undefined) {
            input.PageSize = 20;
        }
        $.ajax({
            url: "/ReportDepartment/SearchReportDepartmentBysaler",
            type: "post",
            data: input,
            success: function (result) {
               
                $('#imgLoading').hide();
                $('#grid-data').html(result);
                $('#selectPaggingOptions').val(input.PageSize).attr("selected", "selected");
            },
        });
    },
     ExportDepartmentBysaler: function () {

         objSearch = this.GetParamBysaler()

         objSearch.SalerId = $("#SalerId_sale").val(),
        _global_function.AddLoading()
        $.ajax({
            url: "/ReportDepartment/ExportExcelReportDepartmentBysaler",
            type: "Post",
            data: objSearch,
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
    ExportDepartmentBysaler2: function (id) {

        objSearch = this.GetParamBysaler()

        objSearch.SalerId = id,
            _global_function.AddLoading()
        $.ajax({
            url: "/ReportDepartment/ExportExcelDetailDepartmentBysaler",
            type: "Post",
            data: objSearch,
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
    GetParamBysaler: function () {
       
     
        let _searchModel = {
            DepartmentType: $("#DepartmentType").val(),
            DepartmentId: $("#sale-DepartmentId").val(),
            SalerId: $("#SalerId_sale").val(),
            StartDateFromStr: $("#filter_date_daterangepicker_DateFrom").val(),
            StartDateToStr: $("#filter_date_daterangepicker_DateTo").val(),
            OrderStatus: listOrderStatus.toString(),
            PageIndex: 1,
            PageSize: $("#selectPaggingOptions").find(':selected').val(),
        };
        return _searchModel;
    },
    OpenPopupDetailDepartmentBysaler: function (id, name,email) {
        objSearch = this.GetParamBysaler();
        objSearch.SalerId = id;
        var tile = "Chi tiết báo cáo " + name + " - " + email;
        let url = '/ReportDepartment/DetailReportDepartmentBysaler';
        let param = objSearch;
     
        _magnific.OpenSmallPopup(tile, url, objSearch);
     
    },
    onloadSelectPageSize: function (id) {
        objSearch = this.GetParamBysaler();
        objSearch.SalerId = id;
        PageSize = PageSize + 20;
        objSearch.PageSize = PageSize;
        $.ajax({
            url: "/ReportDepartment/DetailReportDepartmentBysaler",
            type: "post",
            data: objSearch,
            success: function (result) {
               
                $('#list_data_sale').html(result);
              
            },
        });
    },
    onSelectPageSize2: function () {
        var objSearch = this.SearchParam;
        objSearch = this.GetParamBysaler();
        this.SearchDepartmentBysaler(objSearch);
        $(document).load().scrollTop(0);
    },
    OnPaging2: function (value) {

        var objSearch = this.SearchParam
        objSearch = this.GetParamBysaler()
        objSearch.PageIndex = value

        this.SearchDepartmentBysaler(objSearch);
        $(document).load().scrollTop(0);
    },
};