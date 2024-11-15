$(document).ready(function () {
    _SetServiceTour.loadData();
    $("#OrderNo").select2({
        theme: 'bootstrap4',
        placeholder: "Mã đơn hàng",
        minimumInputLength: 1,
        maximumSelectionLength: 1,
        allowClear: true,
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
                            id: item.orderid,
                        }
                    })
                };
            },
            cache: true
        }
    });
    $("#UserCreate").select2({
        theme: 'bootstrap4',
        placeholder: "Người tạo",
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
                            text: item.email,
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
        placeholder: "Nhân viên bán",
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
                            text: item.email,
                            id: item.id,
                        }
                    })
                };
            },
            cache: true
        }
    });
    $("#OperatorId").select2({
        theme: 'bootstrap4',
        placeholder: "Điều hành viên",
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
                            text: item.username + " - " + item.fullname +" - "+ item.email,
                            id: item.id,
                        }
                    })
                };
            },
            cache: true
        }
    });
    $("#ServiceCode").select2({
        theme: 'bootstrap4',
        placeholder: "Mã dịch vụ",
        maximumSelectionLength: 1,
        ajax: {
            url: "/SetService/TourSuggestion",
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
                            text: item.serviceCode,
                            id: item.id,
                        }
                    })
                };
            },
            cache: true
        }
    });
    $("#BookingCode").select2({
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
                    type: 5,
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
    const selectBtnStatusBooking = document.querySelector(".select-btn-StatusBooking-type")
    const itemsStatusBooking = document.querySelectorAll(".item-StatusBooking-type");
    const selectBtnTourType = document.querySelector(".select-btn-TourType-type")
    const itemsTourType = document.querySelectorAll(".item-TourType-type");
    const selectBtnOrganizingType = document.querySelector(".select-btn-OrganizingType-type")
    const itemsOrganizingType = document.querySelectorAll(".item-OrganizingType-type");
    $(document).click(function (event) {

        var $target = $(event.target);
        if (!$target.closest('#select-btn-StatusBooking-type').length) {//checkbox_trans_type_
            if ($('#list-item-StatusBooking').is(":visible") && !$target[0].id.includes('StatusBooking_type_text') && !$target[0].id.includes('StatusBooking_type')
                && !$target[0].id.includes('list-item-StatusBooking') && !$target[0].id.includes('checkbox_StatusBooking_type')) {
                selectBtnStatusBooking.classList.toggle("open");
            }
        }
        if (!$target.closest('#select-btn-TourType-type').length) {//checkbox_trans_type_
            if ($('#list-item-TourType').is(":visible") && !$target[0].id.includes('TourType_type_text') && !$target[0].id.includes('TourType_type')
                && !$target[0].id.includes('list-item-TourType') && !$target[0].id.includes('checkbox_TourType_type')) {
                selectBtnTourType.classList.toggle("open");
            }
        }
        if (!$target.closest('#select-btn-OrganizingType-type').length) {//checkbox_trans_type_
            if ($('#list-item-OrganizingType').is(":visible") && !$target[0].id.includes('OrganizingType_type_text') && !$target[0].id.includes('OrganizingType_type')
                && !$target[0].id.includes('list-item-OrganizingType') && !$target[0].id.includes('checkbox_OrganizingType_type')) {
                selectBtnOrganizingType.classList.toggle("open");
            }
        }
    });
    selectBtnStatusBooking.addEventListener("click", (e) => {
        e.preventDefault();
        selectBtnStatusBooking.classList.toggle("open");
    });
    selectBtnTourType.addEventListener("click", (e) => {
        e.preventDefault();
        selectBtnTourType.classList.toggle("open");
    });
    selectBtnOrganizingType.addEventListener("click", (e) => {
        e.preventDefault();
        selectBtnOrganizingType.classList.toggle("open");
    });
    itemsStatusBooking.forEach(item => {
        item.addEventListener("click", () => {
            item.classList.toggle("checked");
            let checked = document.querySelectorAll(".checked"),
                btnText = document.querySelector(".btn-text-StatusBooking-type");
            listStatusBooking = []
            let checked_list = []

            for (var i = 0; i < checked.length; i++) {
                id = checked[i].getAttribute('id')
                if (id.includes('StatusBooking_type_')) {
                    checked_list.push(checked[i]);
                    listStatusBooking.push(parseInt(id.replace('StatusBooking_type_', '')))
                }
            }
            _SetServiceTour.SearchData.StatusBooking = listStatusBooking

            if (checked_list && checked_list.length > 0) {
                btnText.innerText = `${checked_list.length} Selected`;
            } else {
                btnText.innerText = "Tất cả trạng thái";
            }
        });

    })
    itemsTourType.forEach(item => {
        item.addEventListener("click", () => {
            item.classList.toggle("checked");
            let checked = document.querySelectorAll(".checked"),
                btnText = document.querySelector(".btn-text-TourType-type");
            listTourType = []
            let checked_list = []

            for (var i = 0; i < checked.length; i++) {
                id = checked[i].getAttribute('id')
                if (id.includes('TourType_type_')) {
                    checked_list.push(checked[i]);
                    listTourType.push(parseInt(id.replace('TourType_type_', '')))
                }
            }
            _SetServiceTour.SearchData.TourType = listTourType

            if (checked_list && checked_list.length > 0) {
                btnText.innerText = `${checked_list.length} Selected`;
            } else {
                btnText.innerText = "Tất cả loại tour";
            }
        });

    })
    itemsOrganizingType.forEach(item => {
        item.addEventListener("click", () => {
            item.classList.toggle("checked");
            let checked = document.querySelectorAll(".checked"),
                btnText = document.querySelector(".btn-text-OrganizingType-type");
            listOrganizingType = []
            let checked_list = []

            for (var i = 0; i < checked.length; i++) {
                id = checked[i].getAttribute('id')
                if (id.includes('OrganizingType_type_')) {
                    checked_list.push(checked[i]);
                    listOrganizingType.push(parseInt(id.replace('OrganizingType_type_', '')))
                }
            }
            _SetServiceTour.SearchData.OrganizingType = listOrganizingType

            if (checked_list && checked_list.length > 0) {
                btnText.innerText = `${checked_list.length} Selected`;
            } else {
                btnText.innerText = "Tất cả hình thức tổ chức";
            }
        });

    })
});
let isPickerApprove = false;
let isPickerApprove2 = false;
let isPickerApprove3 = false;
let isResetTab = false;
let listStatusBooking = [];
let listOrganizingType = [];
let listTourType = [];
var _SetServiceTour = {
    loadData: function () {
        let _searchModel = {
            PageIndex: 1,
            PageSize: 20,
        };
        objSearch = _searchModel;
        _SetServiceTour.Search(objSearch);
        _SetServiceTour.TotalTour(objSearch);
    },
    Search: function (input) {

        window.scrollTo(0, 0);
        $.ajax({
            url: "/SetService/TourSearch",
            type: "Post",
            data: input,
            success: function (result) {
                $('#imgLoading_Search').hide();
                $('#grid_data_Search').html(result);
                $('.checkbox-tb-column').each(function () {
                    let seft = $(this);
                    let id = seft.data('id');
                    if (seft.is(':checked')) {
                        $('td:nth-child(' + id + '),th:nth-child(' + id + ')').removeClass('mfp-hide');
                    } else {
                        $('td:nth-child(' + id + '),th:nth-child(' + id + ')').addClass('mfp-hide');
                    }
                });
                $('#selectPaggingOptions').val(input.PageSize).attr("selected", "selected");
            }
        });
    },
    SearchData: function () {
        var objSearch = _SetServiceTour.getPram(true);
        if (objSearch.PageSize == undefined) { objSearch.PageSize = 20 }
        _SetServiceTour.Search(objSearch);
        _SetServiceTour.TotalTour(objSearch);
    },
    getPram: function (isFirstTime = false) {
        var StartDateFrom;
        var StartDateTo;
        var EndDateFrom;
        var EndDateTo;
        var CreateDateFrom;
        var CreateDateTo;
        var ServiceCode_data = $('#ServiceCode').select2("val");
        var OrderCode_data = $('#OrderNo').select2("val");
        var UserCreate_data = $('#UserCreate').select2("val");
        var SalerId_data = $('#SalerId').select2("val");
        var OperatorId_data = $('#OperatorId').select2("val");
        textNV = $('#CreatedBy').find(':selected').text();
        if ($('#StartDate').data('daterangepicker') !== undefined && $('#StartDate').data('daterangepicker') != null && isPickerApprove) {
            StartDateFrom = $('#StartDate').data('daterangepicker').startDate._d.toLocaleDateString("en-GB");
            StartDateTo = $('#StartDate').data('daterangepicker').endDate._d.toLocaleDateString("en-GB");
        } else {
            StartDateFrom = null
            StartDateTo = null
        }
        if ($('#EndDate').data('daterangepicker') !== undefined && $('#EndDate').data('daterangepicker') != null && isPickerApprove2) {
            EndDateFrom = $('#EndDate').data('daterangepicker').startDate._d.toLocaleDateString("en-GB");
            EndDateTo = $('#EndDate').data('daterangepicker').endDate._d.toLocaleDateString("en-GB");
        } else {
            EndDateFrom = null
            EndDateTo = null
        }
        if ($('#CreateDate').data('daterangepicker') !== undefined && $('#CreateDate').data('daterangepicker') != null && isPickerApprove3) {
            CreateDateFrom = $('#CreateDate').data('daterangepicker').startDate._d.toLocaleDateString("en-GB");
            CreateDateTo = $('#CreateDate').data('daterangepicker').endDate._d.toLocaleDateString("en-GB");
        } else {
            CreateDateFrom = null
            CreateDateTo = null
        }
        let _searchModel = {
            ServiceCode: null,
            OrderCode: null,
            StatusBooking: listStatusBooking.toString(),

            TourType: listTourType.toString(),
            OrganizingType: listOrganizingType.toString(),

            StartDateFrom: StartDateFrom,
            StartDateTo: StartDateTo,
            EndDateFrom: EndDateFrom,
            EndDateTo: EndDateTo,
            UserCreate: null,
            CreateDateFrom: CreateDateFrom,
            CreateDateTo: CreateDateTo,
            SalerId: null,
            OperatorId: null,
            BookingCode: $("#BookingCode").val(),
            PageIndex: 1,
            PageSize: $("#selectPaggingOptions").find(':selected').val(),
        };

        if (ServiceCode_data != null) { _searchModel.ServiceCode = ServiceCode_data[0] }
        if (OrderCode_data != null) { _searchModel.OrderCode = OrderCode_data[0] }
        if (SalerId_data != null) { _searchModel.SalerId = SalerId_data[0] }
        if (UserCreate_data != null) { _searchModel.UserCreate = UserCreate_data[0] }
        if (OperatorId_data != null) { _searchModel.OperatorId = OperatorId_data[0] }
        return _searchModel
    },
    OnPaging: function (value) {
        if (value > 0) {
            var objSearch = _SetServiceTour.getPram(true);
            objSearch.PageIndex = value;
            _SetServiceTour.Search(objSearch);
        }
    },
    onSelectPageSize: function () {
        _SetServiceTour.SearchData();
     
    },
    OnSearchStatus: function (status) {
        isResetTab = false
        var objSearch = _SetServiceTour.getPram(true);

        if (status == -1) {
            objSearch.StatusBooking = '';
            
        } else {
            objSearch.StatusBooking = status;
        }
        if (objSearch.PageSize == undefined) { objSearch.PageSize = 20 }
        this.Search(objSearch);
        this.SetActive(status)

    },
    SetActive: function (status) {
        var objSearch = this.SearchParam


        $('#StatusBookingAll').removeClass('active')
        $('#StatusBooking_0').removeClass('active')
        $('#StatusBooking_1').removeClass('active')
        $('#StatusBooking_2').removeClass('active')
        $('#StatusBooking_3').removeClass('active')
        $('#StatusBooking_4').removeClass('active')
        $('#StatusBooking_5').removeClass('active')
        if (status == -1) {
            $('#StatusBookingAll').addClass('active')
            isResetTab = false;
        }
        if (status == 0) {
            $('#StatusBooking_0').addClass('active')
            isResetTab = true;

        }
        if (status == 1) {
            $('#StatusBooking_1').addClass('active')
            isResetTab = true;

        }

        if (status == 2) {
            $('#StatusBooking_2').addClass('active')
            isResetTab = true;
        }
        if (status == 3) {
            $('#StatusBooking_3').addClass('active')
            isResetTab = true;
        }
        if (status == 4) {
            $('#StatusBooking_4').addClass('active')
            isResetTab = true;
        }
        if (status == 5) {
            $('#StatusBooking_5').addClass('active')
            isResetTab = true;
        }
    },
    TotalTour: function (objSearch) {
        $.ajax({
            url: "/SetService/TotalTour",
            type: "Post",
            data: objSearch,
            success: function (result) {
                $('#StatusBookingAll').text('Tất cả (0)')
                if (result.isSuccess == true) {

                    $('#StatusBookingAll').text('Tất cả (' + result.data + ')')

                }
            }
        });
    },
    Export: function () {
        $('#btnExport').prop('disabled', true);
        $('#icon-export').removeClass('fa-file-excel-o');
        $('#icon-export').addClass('fa-spinner fa-pulse');
        var objSearch = _SetServiceTour.getPram(true);
        objSearch.PageIndex = 1;
        objSearch.PageSize = $('#countTour').val();
        this.searchModel = objSearch
        _global_function.AddLoading()
        $.ajax({
            url: "/SetService/TourExportExcel",
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
