
$(document).ready(function () {
    $("#SupplierID").select2({
        theme: 'bootstrap4',
        placeholder: "Tên NCC, Điện Thoại, Email",
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
                            text: item.name,
                            id: item.id,
                        }
                    })
                };
            },
            cache: true
        }
    });
    $("#Hotel").select2({
        theme: 'bootstrap4',
        placeholder: "Tên khách sạn",
        minimumInputLength: 1,
        ajax: {
            url: "/Order/HotelSuggestion",
            type: "post",
            dataType: 'json',
            delay: 250,
            tags: true,
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
                            text: item.name,
                            id: item.hotelid,
                        }
                    })
                };
            },
            cache: true
        }
    });
    _ProgramsPriceHotel.Init();
});
let isPickerApprove = false;
let isPickerApprove2 = false;
let isPickerApprove3 = false;
var _ProgramsPriceHotel = {
    Init: function () {

        objSearch = this.GetParam();
      
        _ProgramsPriceHotel.SearchProgramsPriceHotel(objSearch);
    },
    SearchData: function () {
        objSearch = this.GetParam();
        _ProgramsPriceHotel.SearchProgramsPriceHotel(objSearch);
    },
    SearchProgramsPriceHotel: function (input) {
        $('#grid_data').html('<img src="/images/icons/loading.gif" style=" width: 100px; height: 100px; margin-left:46%;" id="imgLoading" />');
        $('#imgLoading').show()
        $.ajax({
            url: "/ProgramsPackage/ListProgramsPriceHotel",
            type: "post",
            data: input,
            success: function (result) {
              
                $('#imgLoading').hide();
                $('#grid_data').html(result);
            },
        });
    },
    GetParam: function () {
    
        var StayStartDateFrom; //Ngày bat dau
        var StayStartDateTo; //Ngày hết hạn
        if ($('#ApplyDate').data('daterangepicker') !== undefined && $('#ApplyDate').data('daterangepicker') != null && isPickerApprove) {
            StayStartDateFrom = $('#ApplyDate').data('daterangepicker').startDate._d.toLocaleDateString("en-GB");
            StayStartDateTo = $('#ApplyDate').data('daterangepicker').endDate._d.toLocaleDateString("en-GB");
           
        } else {
            StayStartDateFrom = null
            StayStartDateTo = null
        }
        var StayEndDateFrom; //Ngày bat dau
        var StayEndDateTo; //Ngày hết hạn
        if ($('#ApplyDate2').data('daterangepicker') !== undefined && $('#ApplyDate2').data('daterangepicker') != null && isPickerApprove2) {
            StayEndDateFrom = $('#ApplyDate2').data('daterangepicker').startDate._d.toLocaleDateString("en-GB");
            StayEndDateTo = $('#ApplyDate2').data('daterangepicker').endDate._d.toLocaleDateString("en-GB");
        } else {
            StayEndDateFrom = null
            StayEndDateTo = null
        }
        let _searchModel = {
            /*   ProgramId: $("#ProgramId").val(),*/
            ProgramId: null,
            SupplierID: $("#SupplierID").select2("val"),
            HotelId: $("#Hotel").select2("val"),
            ClientType: $("#ClientType").val(),
            FromDate: StayStartDateFrom,
            ToDate: StayStartDateTo,
            StayStartDateFrom: StayStartDateFrom,
            StayStartDateTo: StayStartDateTo,
            StayEndDateFrom: StayEndDateFrom,
            StayEndDateTo: StayEndDateTo,
            Type: $("#Type").val(),
            Status: 2,
            PageIndex: -1,
            PageSize: 100,
        };
        return _searchModel;
    },
    Search2: function (id,roomtype,pkCode) {
        objSearch = this.GetParam();
        objSearch.ProgramId = id;
        objSearch.PageIndex = 1;
        objSearch.RoomType = roomtype;
        objSearch.PackageCode = pkCode;
        objSearch.FromDate = objSearch.StayStartDateFrom != null ? objSearch.StayStartDateFrom : objSearch.StayEndDateFrom;
        objSearch.ToDate = objSearch.StayStartDateTo != null ? objSearch.StayStartDateTo : objSearch.StayEndDateTo;
        objSearch.StayStartDateFrom = null;
        objSearch.StayStartDateTo = null;
        objSearch.StayEndDateFrom = null;
        objSearch.StayEndDateTo = null;
        var newDate = new Date(_global_function.ParseDateTostring(objSearch.ToDate));
        
        newDate.setDate(newDate.getDate() - 1);
        objSearch.ToDate = newDate.toLocaleDateString("en-GB");
        
        let title = 'Bảng giá chi tiết';
        let url = "/ProgramsPackage/ListPricePolicyHotel";
        let param = objSearch;
        let elPopup = $('#magnific-popup-small');
        let elBody = elPopup.find('.magnific-body');
        elBody.html(`<img src="/images/icons/loading.gif" style="margin-left:45%; width: 100px; height: 100px; display:none" class="loading" id="imgLoading" />`)
        _magnific.OpenSmallPopup(title, url, param);
   
    },

};