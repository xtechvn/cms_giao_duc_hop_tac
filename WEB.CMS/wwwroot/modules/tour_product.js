
var _tour_product = {
    Init: function () {
        this.modal_element = $('#global_modal_popup');
        this.validImageTypes = ['image/gif', 'image/jpeg', 'image/png'];
        this.OnSearch();
    },

    GetFormData: function ($form) {
        var unindexed_array = $form.serializeArray();
        var indexed_array = {};

        $.map(unindexed_array, function (n, i) {
            indexed_array[n['name']] = n['value'];
        });

        return indexed_array;
    },

    GetSeachParam: function () {
        let tour_type = $('#sl_search_tour_type').val();
        let organize_type = $('#sl_search_organize_type').val();
        let display_website = $('#sl_search_display_website').val();
        let stars = $('#sl_search_star').val();

        let start_point = $('#sl_search_start_point').val();
        let end_point = $('#sl_search_end_point').val();
        let suppliers = $('#sl_search_supplier').val();

        let objSearch = {
            ServiceCode: "",
            TourName: $('#ip_search_tour_name').val(),
            TourType: tour_type != null ? tour_type.join(',') : "",
            OrganizingType: organize_type != null ? organize_type.join(',') : "",
            SupplierIds: suppliers != null ? suppliers.join(',') : "",
            Days: "",
            Star: stars != null ? stars.join(',') : "",
            IsDisplayWeb: display_website,
            StartPoint: start_point != null ? start_point.join(',') : "",
            Endpoint: end_point != null ? end_point.join(',') : "",
            IsSelfDesign: "",
            PageIndex: 1,
            PageSize: 10
        }

        return objSearch;
    },

    Search: function (input) {
        window.scrollTo(0, 0);
        _ajax_caller.post("/TourProduct/Search", input, function (result) {
            $('#grid_data').html(result);
        });
    },

    OnSearch: function () {
        let objSearch = this.GetSeachParam();
        this.SearchParam = objSearch;
        this.Search(objSearch);
    },

    OnPaging: function (value) {
        var objSearch = this.SearchParam;
        objSearch.PageIndex = value;
        this.SearchParam = objSearch
        this.Search(objSearch);
    },

    ReLoad: function () {
        this.Search(this.SearchParam);
    },

    OnChangePropertySearch: function (property, value) {
        var searchobj = this.SearchParam;
        searchobj[property] = value;
        searchobj.currentPage = 1;
        this.SearchParam = searchobj;
        this.Search(searchobj);
    },

    OnDelete: function (id) {
        let title = 'Xác nhận xóa sản phẩm tour';
        let description = 'Bạn xác nhận muốn xóa bản ghi này?';
        _msgconfirm.openDialog(title, description, function () {
            let url = "/TourProduct/DeleteTourProduct";
            _ajax_caller.post(url, { Id: id }, function (result) {
                if (result.isSuccess) {
                    _msgalert.success(result.message);
                    _tour_product.ReLoad();
                } else {
                    _msgalert.error(result.message);
                }
            });
        });
    }
}

var _changeInterval = null;
$("#ip_search_tour_name").keyup(function (e) {
    if (e.which === 13) {
        _tour_product.OnChangePropertySearch("TourName", e.target.value);
    } else {
        clearInterval(_changeInterval);
        _changeInterval = setInterval(function () {
            _tour_product.OnChangePropertySearch("TourName", e.target.value);
            clearInterval(_changeInterval);
        }, 1000);
    }
});

$(document).ready(function () {
    $('input').attr('autocomplete', 'off');
    _tour_product.Init();

    $("#sl_search_star").select2({
        placeholder: "Tất cả hạng sao",
        multiple: true
    });

    $("#sl_search_tour_type").select2({
        placeholder: "Tất cả loại tour",
        multiple: true
    });

    $("#sl_search_organize_type").select2({
        placeholder: "Tất cả hình thức",
        multiple: true
    });

    $("#sl_search_start_point").select2({
        placeholder: "Điểm đi",
        multiple: true,
        maximumSelectionLength: 3
    });

    $("#sl_search_end_point").select2({
        placeholder: "Điểm đến",
        multiple: true,
        maximumSelectionLength: 3
    });

    $("#sl_search_supplier").select2({
        //theme: 'bootstrap4',
        placeholder: "Nhà cung cấp",
        multiple: true,
        ajax: {
            url: "/Supplier/Suggest",
            type: "get",
            dataType: 'json',
            delay: 250,
            data: function (params) {
                var query = {
                    text: params.term,
                    size: 10
                }
                return query;
            },
            processResults: function (data) {
                var data = {
                    results: $.map(data, function (item) {
                        return {
                            text: item.name,
                            id: item.id,
                        }
                    })
                }
                return data;
            }
        }
    });
});
