
$(document).ready(function () {
    _SetService.loadData();
        $("#OrderNo").select2({
            theme: 'bootstrap4',
            placeholder: "Mã đơn hàng",
            minimumInputLength: 1,
            maximumSelectionLength: 1,
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
                            text: item.email,
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
            url: "/SetService/HotelBookingSuggestion",
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
                            text: item.servicecode,
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
                    type: 1,
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
    $(document).click(function (event) {

        var $target = $(event.target);
        if (!$target.closest('#select-btn-StatusBooking-type').length) {//checkbox_trans_type_
            if ($('#list-item-StatusBooking').is(":visible") && !$target[0].id.includes('StatusBooking_type_text') && !$target[0].id.includes('StatusBooking_type')
                && !$target[0].id.includes('list-item-StatusBooking') && !$target[0].id.includes('checkbox_StatusBooking_type')) {
                selectBtnStatusBooking.classList.toggle("open");
            }
        }
       

    });
    selectBtnStatusBooking.addEventListener("click", (e) => {
        e.preventDefault();
        selectBtnStatusBooking.classList.toggle("open");
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
            _SetService.SearchData.StatusBooking = listStatusBooking

            if (checked_list && checked_list.length > 0) {
                btnText.innerText = `${checked_list.length} Selected`;
            } else {
                btnText.innerText = "Tất cả trạng thái";
            }
        });
    })

});
let isPickerApprove = false;
let isPickerApprove2 = false;
let isPickerApprove3 = false;
let isResetTab = false;
let listStatusBooking = [];
var _SetService = {
    loadData: function () {
        let _searchModel = {

            PageIndex: 1,
            PageSize: 20,
        };
        objSearch = _searchModel;
        _SetService.Search(objSearch); 
    },
    Search: function (input) {
        
        window.scrollTo(0, 0);
        $.ajax({
            url: "/SetService/Search",
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
                $('#StatusBookingAll').text('Tất cả (' + $('#TotalHotelBooking').val() + ')');
            }
        });
    },
    SearchData: function () {
        var objSearch = _SetService.getPram(true);
        if (objSearch.PageSize == undefined) { objSearch.PageSize=20}
        _SetService.Search(objSearch);

        this.SetActive(-1);
    },

    getPram: function (isFirstTime = false) {
        var CheckinDateFrom;
        var CheckinDateTo;
        var CheckoutDateFrom;
        var CheckoutDateTo;
        var CreateDateFrom;
        var CreateDateTo;
        var ServiceCode_data = $('#ServiceCode').select2("val");
        var OrderCode_data = $('#OrderNo').select2("val");
        var UserCreate_data = $('#UserCreate').select2("val");
        var SalerId_data = $('#SalerId').select2("val");
        var OperatorId_data = $('#OperatorId').select2("val");
        textNV = $('#CreatedBy').find(':selected').text();
        if ($('#CheckinDate').data('daterangepicker') !== undefined && $('#CheckinDate').data('daterangepicker') != null && isPickerApprove) {
            CheckinDateFrom = $('#CheckinDate').data('daterangepicker').startDate._d.toLocaleDateString("en-GB");
            CheckinDateTo = $('#CheckinDate').data('daterangepicker').endDate._d.toLocaleDateString("en-GB");
        } else {
            CheckinDateFrom = null
            CheckinDateTo = null
        }
        if ($('#CheckoutDate').data('daterangepicker') !== undefined && $('#CheckoutDate').data('daterangepicker') != null && isPickerApprove2) {
            CheckoutDateFrom = $('#CheckoutDate').data('daterangepicker').startDate._d.toLocaleDateString("en-GB");
            CheckoutDateTo = $('#CheckoutDate').data('daterangepicker').endDate._d.toLocaleDateString("en-GB");
        } else {
            CheckoutDateFrom = null
            CheckoutDateTo = null
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
            CheckinDateFrom: CheckinDateFrom,
            CheckinDateTo: CheckinDateTo,
            CheckoutDateFrom: CheckoutDateFrom,
            CheckoutDateTo: CheckoutDateTo,
            UserCreate: null,
            CreateDateFrom: CreateDateFrom ,
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
            var objSearch = _SetService.getPram(true);
            objSearch.PageIndex = value;
            _SetService.Search(objSearch);
        }
    },
    onSelectPageSize: function () {
        _SetService.SearchData();

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
    OnSearchStatus: function (status) {
        isResetTab = false
        var objSearch = _SetService.getPram(true);
       
        if (status == -1) {
            objSearch.StatusBooking = '';

        } else {
            objSearch.StatusBooking = status;
        }
        if (objSearch.PageSize == undefined) { objSearch.PageSize = 20 }
        this.Search(objSearch);
        this.SetActive(status)
        
    },
    Export: function () {
        $('#btnExport').prop('disabled', true);
        $('#icon-export').removeClass('fa-file-excel-o');
        $('#icon-export').addClass('fa-spinner fa-pulse');
        var objSearch = _SetService.getPram(true);
        objSearch.PageIndex = 1;
        objSearch.PageSize = $('#countHotel').val();
        this.searchModel = objSearch
        _global_function.AddLoading()
        $.ajax({
            url: "/SetService/ExportExcel",
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