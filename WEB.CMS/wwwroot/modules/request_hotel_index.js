$(document).ready(function () {
    _request_hotel.Init();
    $("#ClientId").select2({
        theme: 'bootstrap4',
        placeholder: "Thông tin khách hàng",
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
                            text: item.fullname + ' - ' + item.email,
                            id: item.id,
                        }
                    })
                };
            },
            cache: true
        }
    });
})
var _request_hotel = {
    Init: function () {

        objSearch = this.GetParam();
        objSearch.PageSize = 20;
        _request_hotel.Search(objSearch);
    },
    Search: function (input) {
        $.ajax({
            url: "/RequestHotelBooking/Search",
            type: "Post",
            data: { searchModel: input},
            success: function (result) {
                $('#imgLoading').hide();
                $('#grid_data').html(result);
                $('#selectPaggingOptions').val(input.PageSize).attr("selected", "selected");
            }
        });
    },
    GetParam: function () {
        var RequestId_data = $('#RequestId').select2("val");
        var ClientId_data = $('#ClientId').select2("val");
        var SalerId_data = $('#SalerId').select2("val");

        let _searchModel = {
            RequestId: null,
            SalerId: null,
            ClientId: null,
            PageIndex: 1,
            PageSize: $("#selectPaggingOptions").find(':selected').val(),
        };
        if (RequestId_data != null) {
            _searchModel.RequestId = RequestId_data[0]
        }
        if (ClientId_data != null) {
            _searchModel.ClientId = ClientId_data[0]
        }
        if (SalerId_data != null) {
            _searchModel.SalerId = SalerId_data[0]
        }
        return _searchModel;
    },
    OnPaging: function (values) {
        objSearch = this.GetParam();
        objSearch.PageIndex = values;
        if (objSearch.PageSize == undefined) {
            objSearch.PageSize = 20;
        }
        _request_hotel.Search(objSearch);
    },
    onSelectPageSize: function () {
        objSearch = this.GetParam();
        _request_hotel.Search(objSearch);
    },
}