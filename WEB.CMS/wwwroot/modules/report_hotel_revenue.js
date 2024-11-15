$(document).ready(function () {

    $("#token-input-hotel").select2({
        theme: 'bootstrap4',
        placeholder: "Tìm kiếm tên khách sạn",
        ajax: {
            url: "/Hotel/Suggest",
            type: "get",
            dataType: 'json',
            delay: 250,
            data: function (params) {
                var query = {
                    text: params.term,
                    size: 20
                }
                return query
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

    $('input[name="check_in_date').on('change.datepicker', function (ev, picker) {
        isPickerCheckInDate = false;
        $(this).val('');
    });

    $('input[name="create_date').on('change.datepicker', function (ev, picker) {
        isPickerCreated = false;
        $(this).val('');
    });

    $('input[name="check_out_date').on('change.datepicker', function (ev, picker) {
        isPickerCheckOutDate = false;
        $(this).val('');
    });

    _report_hotel_revenue.Initialization();
});
var isPickerCreated = false
var isPickerCheckInDate = false
var isPickerCheckOutDate = false
var _report_hotel_revenue = {
    Initialization: function () {
        var SearchParam = _report_hotel_revenue.GetParam()
        _report_hotel_revenue.Search(SearchParam)
    },
    GetParam: function () {
        var objSearch = {
            hotelIds: [],
            currentPage: 1,
            pageSize: 20
        }
        var hotel = $('#token-input-hotel').val()
        if (hotel !== undefined && hotel !== null && hotel !== '') {
            objSearch.hotelIds = $('#token-input-hotel').val()
        }
        if ($('#create_date').data('daterangepicker') !== undefined &&
            $('#create_date').data('daterangepicker') != null && isPickerCreated) {
            objSearch.createDateFromStr = $('#create_date').data('daterangepicker').startDate._d.toLocaleDateString("en-GB");
            objSearch.createDateToStr = $('#create_date').data('daterangepicker').endDate._d.toLocaleDateString("en-GB");
        } else {
            objSearch.createDateFromStr = null
            objSearch.createDateToStr = null
        }
        if ($('#check_in_date').data('daterangepicker') !== undefined &&
            $('#check_in_date').data('daterangepicker') != null && isPickerCheckInDate) {
            objSearch.checkInDateFromStr = $('#check_in_date').data('daterangepicker').startDate._d.toLocaleDateString("en-GB");
            objSearch.checkInDateToStr = $('#check_in_date').data('daterangepicker').endDate._d.toLocaleDateString("en-GB");
        } else {
            objSearch.checkInDateFromStr = null
            objSearch.checkInDateToStr = null
        }
        if ($('#check_out_date').data('daterangepicker') !== undefined &&
            $('#check_out_date').data('daterangepicker') != null && isPickerCheckOutDate) {
            objSearch.checkOutDateFromStr = $('#check_out_date').data('daterangepicker').startDate._d.toLocaleDateString("en-GB");
            objSearch.checkOutDateToStr = $('#check_out_date').data('daterangepicker').endDate._d.toLocaleDateString("en-GB");
        } else {
            objSearch.checkOutDateFromStr = null
            objSearch.checkOutDateToStr = null
        }
        return objSearch
    },
    ExportExcel: function () {
        var objSearch = this.GetParam()
        objSearch.currentPage = 1;
        this.SearchParam = objSearch
        _global_function.AddLoading()
        $.ajax({
            url: "/ReportHotelRevenue/ExportExcel",
            type: "post",
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
            },
        });
    },
    OnPaging: function (value) {
        var objSearch = this.GetParam()
        objSearch.currentPage = value;
        this.SearchParam = objSearch
        this.Search(objSearch);
    },
    Search: function (input) {
        $('#grid-data').hide();
        _global_function.AddLoading()
        $.ajax({
            url: "/ReportHotelRevenue/Search",
            type: "post",
            data: input,
            success: function (result) {
                _global_function.RemoveLoading()
                $('#grid-data').html(result);
                $('#grid-data').show();
            },
        });
    },
    ActionSearch: function () {
        this.OnPaging(1, true)
    },
}