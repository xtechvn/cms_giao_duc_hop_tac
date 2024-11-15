
$(document).ready(function () {
    _pricepolicymanagement.Init();
});


var _pricepolicymanagement = {
    LoadingDetail: false,
    Init: function () {
        let _searchModel = {
            ServiceType: null,
            CampaignCode: null,
            CampaignDescription: null,
            FromDate: '01/01/2000 00:00:00',
            ToDate: '12/31/2100 23:59:59',
            CampaginStatus: '0,1,2,3',
            ClientType: '1,2,3,4,5,6,7,8',
            ServiceType: -1
        };
        let objSearch = {
            searchModel: _searchModel,
            currentPage: 1,
            pageSize: 20
        };
        _pricepolicymanagement.SearchParam = objSearch;
        _pricepolicymanagement.Search(objSearch);
        $('.filter_service_type').select2({
            minimumResultsForSearch: -1
        });
        $('body').on('click', '.new_pricepolicy', function (event) {
            var type = $(this).attr("data-servicetype");
            switch (parseInt(type)) {
                case 1:
                case 2:
                    {

                        let title = 'Chính sách giá phòng khách sạn';
                        let url = 'PricePolicy/RoomPricePolicy';
                        var param = {
                            campaign_code: '',
                            campaign_id: -1

                        };
                        _magnific.OpenSmallPopup(title, url, param);
                        $('body').addClass('stop-scrolling');
                    }
                    break;
                case 3:
                    {
                        let title = 'Chính sách giá vé máy bay';
                        let url = 'PricePolicy/FlyTicketPolicy';
                        var param = {
                        };
                        _magnific.OpenSmallPopup(title, url, param);
                    }
                    break;
                case 6:
                    _pricepolicymanagement.UpdateVinWonderCampaign(0);
                    break;
                default:
                    break;
            };

        });
    },

    OnPaging: function (value) {
        var objSearch = _pricepolicymanagement.SearchParam;
        objSearch.currentPage = value;
        _pricepolicymanagement.Search(objSearch);
    },

    Search: function (input) {
        $.ajax({
            url: "/PricePolicy/search",
            type: "post",
            data: input,
            success: function (result) {
                $('#grid_data').html(result);
            }
        });
    },

    AdvanceSearch: function () {
        var objSearch = _pricepolicymanagement.SearchParam;

        objSearch.searchModel.ServiceType = $('#filter_service_type :selected').val();
        objSearch.searchModel.CampaignCode = $('#filter_campaign_code').val();;
        objSearch.searchModel.CampaignDescription = $('#filter_text').val();
        objSearch.searchModel.ProviderName = $('#filter_campaign_hotel_name').val();
        var choose_form_date_value = $('input[name="search_date_by_form"]:checked').val();
        switch (choose_form_date_value) {
            case '0': {
                objSearch.searchModel.FromDate = $('#filer_date_selected').attr('data-from');
                objSearch.searchModel.ToDate = $('#filer_date_selected').attr('data-to');
            } break;
            case '1': {
                objSearch.searchModel.FromDate = $('#filter_date_daterangepicker').data('daterangepicker').startDate._d.toLocaleDateString('en-US') + ' ' + $('#filter_date_daterangepicker').data('daterangepicker').startDate._d.toLocaleTimeString('en-US');
                objSearch.searchModel.ToDate = $('#filter_date_daterangepicker').data('daterangepicker').endDate._d.toLocaleDateString('en-US') + ' ' + $('#filter_date_daterangepicker').data('daterangepicker').endDate._d.toLocaleTimeString('en-US');
            } break;
        }
        var array_status = [];
        $('.filter_status_checkmark').each(function () {
            if ($(this).is(":checked")) {
                array_status.push($(this).attr('data-value'));
            }
        });
        var array_client_type = [];
        $('.filter_client_type_checkmark').each(function () {
            if ($(this).is(":checked")) {
                array_client_type.push($(this).val());
            }
        });
        if (array_client_type.length > 0) {
            objSearch.searchModel.ClientType = array_client_type.join(",");
        }
        else {
            objSearch.searchModel.ClientType ='1,2,3,4,5,6,7,8'

        }
        objSearch.searchModel.CampaginStatus = array_status.join(",");
        if (objSearch.searchModel.CampaginStatus.trim() == '') objSearch.searchModel.CampaginStatus = '0,1,2,3';
      
        objSearch.currentPage = 1;
        this.Search(objSearch);
    },

    NewPricePolicy: function () {
        let title = 'Thêm chính sách giá mới';
        let url = '/PricePolicy/AddNew';
        let param = {};
        _magnific.OpenSmallPopup(title, url, param);
    },
    NewHotel: function () {
        let title = 'Thêm Khách sạn';
        let url = '/Hotel/UpsertHotel';
        let param = {};
        _magnific.OpenSmallPopup(title, url, param);
    },

    UpdateCampaign: function (campaignCode) {
        let title = 'Cập nhật chính sách giá vé máy bay';
        let url = 'PricePolicy/FlyTicketPolicyUpdate?campaignCode=' + campaignCode;
        var param = {
        };
        _magnific.OpenSmallPopup(title, url, param);
    },

    UpdateHotelCampaign: function (campaignCode, campaign_id) {
        _global_function.AddLoading()
        let title = 'Cập nhật chính sách giá khách sạn';
        let url = 'PricePolicy/RoomPricePolicy'
        var param = {
            campaign_id: campaign_id,
            campaign_code: campaignCode,
        };

        _magnific.OpenSmallPopup(title, url, param);
        $('body').addClass('stop-scrolling');
    },

    UpdateVinWonderCampaign: function (campaign_id) {
        let elModal = $('#global_modal_popup');
        let title = `Cấu hình giá vé Vinwonder`;
        let url = '/VinWonderPolicy/Upsert';

        elModal.find('.modal-title').html(title);
        elModal.find('.modal-dialog').css('max-width', '1200px');

        _ajax_caller.get(url, { id: campaign_id }, function (result) {
            $.magnificPopup.close();
            elModal.find('.modal-title').html(title);
            elModal.find('.modal-body').html(result);
            elModal.modal('show');
        });
    },
   
    SearchInit: function () {

    },

    delay_callback: function (callback, ms) {
        var timer = 0;
        return function () {
            var context = this, args = arguments;
            clearTimeout(timer);
            timer = setTimeout(function () {
                callback.apply(context, args);
            }, ms || 0);
        };
    },
    Export: function () {
        _global_function.AddLoading()
        $('#operator-export-btn').prop('disabled', true);
        $('#operator-export').removeClass('fa-file-excel-o');
        $('#operator-export').addClass('fa-spinner fa-pulse');
        var param = _pricepolicymanagement.GetParam()

        $.ajax({
            url: "/PricePolicy/ExportExcel",
            type: "post",
            data: { searchModel: param },
            success: function (result) {
                _global_function.RemoveLoading()
                $('#operator-export').removeClass('fa-spinner fa-pulse');
                $('#operator-export').addClass('fa-file-excel-o');
                $('#operator-export-btn').prop('disabled', false);
                if (result.status == 0) {
                    _msgalert.success(result.msg);
                    window.location.href = result.path;
                } else {
                    _msgalert.error(result.msg);
                }

            },
        });
    },
    GetParam: function () {
        var objSearch = _pricepolicymanagement.SearchParam;

        objSearch.searchModel.ServiceType = $('#filter_service_type :selected').val();
        objSearch.searchModel.CampaignCode = $('#filter_campaign_code').val();;
        objSearch.searchModel.CampaignDescription = $('#filter_campaign_description').val();
        var choose_form_date_value = $('input[name="search_date_by_form"]:checked').val();
        switch (choose_form_date_value) {
            case '0': {
                objSearch.searchModel.FromDate = $('#filer_date_selected').attr('data-from');
                objSearch.searchModel.ToDate = $('#filer_date_selected').attr('data-to');
            } break;
            case '1': {
                objSearch.searchModel.FromDate = $('#filter_date_daterangepicker').data('daterangepicker').startDate._d.toLocaleDateString('en-US') + ' ' + $('#filter_date_daterangepicker').data('daterangepicker').startDate._d.toLocaleTimeString('en-US');
                objSearch.searchModel.ToDate = $('#filter_date_daterangepicker').data('daterangepicker').endDate._d.toLocaleDateString('en-US') + ' ' + $('#filter_date_daterangepicker').data('daterangepicker').endDate._d.toLocaleTimeString('en-US');
            } break;
        }
        var array_status = [];
        $('.filter_status_checkmark').each(function () {
            if ($(this).is(":checked")) {
                array_status.push($(this).attr('data-value'));
            }
        });
        var array_client_type = [];
        $('.filter_client_type_checkmark').each(function () {
            if ($(this).is(":checked")) {
                array_client_type.push($(this).val());
            }
        });
        if (array_client_type.length > 0) {
            objSearch.searchModel.ClientType = array_client_type.join(",");
        }
        else {
            objSearch.searchModel.ClientType = '1,2,3,4,5,6,7,8'

        }
        objSearch.searchModel.CampaginStatus = array_status.join(",");
        if (objSearch.searchModel.CampaginStatus.trim() == '') objSearch.searchModel.CampaginStatus = '0,1,2,3';

        objSearch.currentPage = 1;
        return objSearch;
    }
}

$('body').on('click', '.mfp-close', function (event) {
    $('body').removeClass('stop-scrolling');

});

$('body').on('click', '.search_change_date_form', function (event) {
    var element = $(this);
    $('#filer_date_selected').val(element.text()).change();
    $('.search_change_date_form').removeClass('selected_date');
    element.addClass('selected_date');
    switch (element.attr('data-date')) {
        case 'today': {
            var now = new Date;
            $('#filer_date_selected').attr('data-from', now.getDate() + '/' + (now.getMonth() + 1) + '/' + now.getFullYear() + ' 00:00:00');
            $('#filer_date_selected').attr('data-to', now.getDate() + '/' + (now.getMonth() + 1) + '/' + now.getFullYear() + ' 23:59:59');
        } break;
        case 'yesterday': {
            var now = new Date;
            $('#filer_date_selected').attr('data-from', (now.getDate() - 1) + '/' + (now.getMonth() + 1) + '/' + now.getFullYear() + ' 00:00:00');
            $('#filer_date_selected').attr('data-to', (now.getDate() - 1) + '/' + (now.getMonth() + 1) + '/' + now.getFullYear() + ' 23:59:59');
        } break;
        case 'this_week': {
            var curr = new Date; // get current date
            var first = curr.getDate() - curr.getDay() + 1; // First day is the day of the month - the day of the week - monday
            var last = first + 7; // last day is the first day + 7 = sunday

            var firstday = new Date(curr.setDate(first));
            var lastday = new Date(curr.setDate(last));

            $('#filer_date_selected').attr('data-from', (firstday.getDate() - 1) + '/' + (firstday.getMonth() + 1) + '/' + firstday.getFullYear() + ' 00:00:00');
            $('#filer_date_selected').attr('data-to', (lastday.getDate() - 1) + '/' + (lastday.getMonth() + 1) + '/' + lastday.getFullYear() + ' 23:59:59');
        } break;
        case 'last_week': {
            var curr = new Date; // get current date
            var first = (curr.getDate() - 7) - (curr.getDay() - 7) + 1; // First day is the day of the month - the day of last week - monday
            var last = first + 7; // last day is the first day + 7 = sunday

            var firstday = new Date(curr.setDate(first));
            var lastday = new Date(curr.setDate(last));

            $('#filer_date_selected').attr('data-from', (firstday.getDate() - 1) + '/' + (firstday.getMonth() + 1) + '/' + firstday.getFullYear() + ' 00:00:00');
            $('#filer_date_selected').attr('data-to', (lastday.getDate() - 1) + '/' + (lastday.getMonth() + 1) + '/' + lastday.getFullYear() + ' 23:59:59');
        } break;
        case 'last_7_day': {
            var curr = new Date; // get current date
            var first = curr.getDate() - 7
            var last = curr // last day is the first day -7

            var firstday = new Date(curr.setDate(first));
            var lastday = new Date(curr.setDate(last));

            $('#filer_date_selected').attr('data-from', (firstday.getDate() - 1) + '/' + (firstday.getMonth() + 1) + '/' + firstday.getFullYear() + ' 00:00:00');
            $('#filer_date_selected').attr('data-to', (lastday.getDate() - 1) + '/' + (lastday.getMonth() + 1) + '/' + lastday.getFullYear() + ' 23:59:59');
        } break;
        case 'this_month': {
            var curr = new Date; // get current date
            var firstday = new Date(curr.getFullYear(), curr.getMonth(), 1);
            var lastday = new Date(curr.getFullYear(), curr.getMonth() + 1, 0);

            $('#filer_date_selected').attr('data-from', (firstday.getDate() - 1) + '/' + (firstday.getMonth() + 1) + '/' + firstday.getFullYear() + ' 00:00:00');
            $('#filer_date_selected').attr('data-to', (lastday.getDate() - 1) + '/' + (lastday.getMonth() + 1) + '/' + lastday.getFullYear() + ' 23:59:59');
        } break;
        case 'last_month': {
            var curr = new Date; // get current date
            var firstday = new Date(curr.getFullYear(), curr.getMonth() - 1, 1);
            var lastday = new Date(curr.getFullYear(), curr.getMonth(), 0);

            $('#filer_date_selected').attr('data-from', (firstday.getDate() - 1) + '/' + (firstday.getMonth() + 1) + '/' + firstday.getFullYear() + ' 00:00:00');
            $('#filer_date_selected').attr('data-to', (lastday.getDate() - 1) + '/' + (lastday.getMonth() + 1) + '/' + lastday.getFullYear() + ' 23:59:59');
        } break;
        case 'last_30_days': {
            var curr = new Date; // get current date
            var first = curr.getDate() - 30
            var last = curr // last day is the first day -7

            var firstday = new Date(curr.setDate(first));
            var lastday = new Date(curr.setDate(last));

            $('#filer_date_selected').attr('data-from', (firstday.getDate() - 1) + '/' + (firstday.getMonth() + 1) + '/' + firstday.getFullYear() + ' 00:00:00');
            $('#filer_date_selected').attr('data-to', (lastday.getDate() - 1) + '/' + (lastday.getMonth() + 1) + '/' + lastday.getFullYear() + ' 23:59:59');
        } break;
        case 'this_quarter': {
            var curr = new Date; // get current date
            const quarter = Math.floor((curr.getMonth() / 3));


            const firstday = new Date(curr.getFullYear(), quarter * 3, 1);
            const lastday = new Date(firstday.getFullYear(), firstday.getMonth() + 3, 0);

            $('#filer_date_selected').attr('data-from', (firstday.getDate() - 1) + '/' + (firstday.getMonth() + 1) + '/' + firstday.getFullYear() + ' 00:00:00');
            $('#filer_date_selected').attr('data-to', (lastday.getDate() - 1) + '/' + (lastday.getMonth() + 1) + '/' + lastday.getFullYear() + ' 23:59:59');
        } break;
        case 'last_quarter': {
            var curr = new Date; // get current date
            const quarter = Math.floor((curr.getMonth() / 3));

            const firstday = new Date(curr.getFullYear(), quarter * 3 - 3, 1);
            const lastday = new Date(firstday.getFullYear(), firstday.getMonth() + 3, 0);

            $('#filer_date_selected').attr('data-from', (firstday.getDate() - 1) + '/' + (firstday.getMonth() + 1) + '/' + firstday.getFullYear() + ' 00:00:00');
            $('#filer_date_selected').attr('data-to', (lastday.getDate() - 1) + '/' + (lastday.getMonth() + 1) + '/' + lastday.getFullYear() + ' 23:59:59');
        } break;
        case 'this_year': {
            var curr = new Date; // get current date

            const firstday = new Date(curr.getFullYear(), 01, 01);
            const lastday = new Date(curr.getFullYear(), 11, 31);

            $('#filer_date_selected').attr('data-from', (firstday.getDate() - 1) + '/' + (firstday.getMonth() + 1) + '/' + firstday.getFullYear() + ' 00:00:00');
            $('#filer_date_selected').attr('data-to', (lastday.getDate() - 1) + '/' + (lastday.getMonth() + 1) + '/' + lastday.getFullYear() + ' 23:59:59');
        } break;
        case 'last_year': {
            var curr = new Date; // get current date

            const firstday = new Date(curr.getFullYear() - 1, 01, 01);
            const lastday = new Date(curr.getFullYear() - 1, 11, 31);

            $('#filer_date_selected').attr('data-from', (firstday.getDate() - 1) + '/' + (firstday.getMonth() + 1) + '/' + firstday.getFullYear() + ' 00:00:00');
            $('#filer_date_selected').attr('data-to', (lastday.getDate() - 1) + '/' + (lastday.getMonth() + 1) + '/' + lastday.getFullYear() + ' 23:59:59');
        } break;
        case 'all': {
            var curr = new Date; // get current date

            const firstday = new Date(2022, 01, 01);
            const lastday = new Date(2059, 11, 31);

            $('#filer_date_selected').attr('data-from', (firstday.getDate() - 1) + '/' + (firstday.getMonth() + 1) + '/' + firstday.getFullYear() + ' 00:00:00');
            $('#filer_date_selected').attr('data-to', (lastday.getDate() - 1) + '/' + (lastday.getMonth() + 1) + '/' + lastday.getFullYear() + ' 23:59:59');
        } break;
    }
    _pricepolicymanagement.AdvanceSearch();
    $('input:radio[name=search_date_by_form][value=0]').click();

});

$(".select_all_b2b").change(function () {
    if (this.checked) {
        var myarr = ["2", "3", "4"];
        $('.filter_client_type_checkmark').each(function (i, obj) {
            var ele = $(this);
            if (myarr.indexOf(ele.attr('value')) > -1) {
                ele.prop('checked', true);
            }
        });
    }
});

//-- Dynamic bind change search filter:
//---- Dynamic Bind
$('#filter_service_type').on('change', function (event) {
    
    _pricepolicymanagement.AdvanceSearch();
});

$('body').on('keyup', '.filter_text', _pricepolicymanagement.delay_callback(function (e) {
    
    _pricepolicymanagement.AdvanceSearch();
}, 500));

$('body').on('apply.daterangepicker', '.filter_date_daterangepicker', function (ev, picker) {
    
    $('input:radio[name=search_date_by_form][value=1]').click();
    _pricepolicymanagement.AdvanceSearch();
});

$('body').on('change', '.filter_checkmark', _pricepolicymanagement.delay_callback(function (e) {
    
    _pricepolicymanagement.AdvanceSearch();
}, 500));

