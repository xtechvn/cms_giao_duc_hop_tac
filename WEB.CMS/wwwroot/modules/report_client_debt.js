var _report_client_debt = {
    Loading: false,
    searchModel: {
        BranchCode: null,
        FromDate: null,
        ToDate: null,
        ClientID: null,
        PageIndex: 1,
        PageSize:30
    },
    Initialization: function () {
        //_report_common.RenderDateRangePicker($('#report_date'))
        var today = new Date();
        var yyyy = today.getFullYear();
        var mm = today.getMonth() + 1; // Months start at 0!
        var dd = today.getDate();
        if (dd < 10) dd = '0' + dd;
        if (mm < 10) mm = '0' + mm;
        var min_range = '01/01/2020';
        var max_range = dd + '/' + mm + '/' + yyyy;
        var start_day_of_month = new Date(yyyy, mm - 1, 1);

        $('#report_date').each(function (index, item) {
            var element = $(item)
            element.daterangepicker({
                autoApply: true,
                autoUpdateInput: false,
                showDropdowns: true,
                drops: 'down',
                minDate: min_range,
                maxDate: max_range,
                locale: {
                    format: 'DD/MM/YYYY'
                }
            });
            element.data('daterangepicker').setStartDate(start_day_of_month);
            element.data('daterangepicker').setEndDate(max_range);
        })
        _report_client_debt.SearchData()
    },
    ExportExcel: function () {
        _report_client_debt.GetFilter(1)
        _global_function.AddLoading()
        $('#operator-export-btn').prop('disabled', true);
        $('#operator-export').removeClass('fa-file-excel-o');
        $('#operator-export').addClass('fa-spinner fa-pulse');

        $.ajax({
            url: "/ReportClientDebt/ExportExcel",
            type: "post",
            data: _report_client_debt.searchModel,
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
    OnPaging: function (value) {
        if (value > 0) {
            _report_client_debt.GetFilter(value);
            _report_client_debt.Search();
        }
    },
    GetFilter: function (pageIndex) {
        var branch_element = $('#report-clientdebt-filter-branch-code')
        var branch = undefined
        if (branch_element != undefined && branch_element != null) {
            branch = branch_element.val()
        }
        var daterange_element = $('#report_date')
        var min_date = null
        var max_date=null
        if (daterange_element != undefined && daterange_element != null) {
            min_date = _global_function.GetDayText(daterange_element.data('daterangepicker').startDate._d,true).split(' ')[0]
            max_date = _global_function.GetDayText(daterange_element.data('daterangepicker').endDate._d, true).split(' ')[0]
        }

        var client_element = $('#report-clientdebt-filter-clientId')
        var client_id = undefined
        if (client_element != undefined && client_element != null) {
            client_id = client_element.find(':selected').val()
        }
        _report_client_debt.searchModel = {
            BranchCode: branch == undefined || branch == null || isNaN(parseInt(branch)) || parseInt(branch) < 0 ? null : branch,
            FromDate: min_date == undefined ? null : min_date,
            ToDate: max_date == undefined ? null : max_date,
            ClientID: client_id == null || client_id.trim() == '' ? null: client_id,
            PageIndex: pageIndex,
            PageSize: $("#report-search-data #selectPaggingOptions").find(':selected').val()
        }
    },
    SearchData: function () {
        _report_client_debt.GetFilter(1)
        _report_client_debt.Search()
    },
    Search: function () {
        $('#imgLoading').show();
        $('#grid-data').hide();

        $.ajax({
            url: "/ReportClientDebt/Search",
            type: "post",
            data: _report_client_debt.searchModel,
            success: function (result) {
                $('#imgLoading').hide();
                $('#grid-data').html(result);
                $('#grid-data').show();
                if (!_report_client_debt.Loading) {
                    _report_client_debt.Loading = true
                    _report_client_debt.DynamicBind()
                }
            },
        });

    },
    onSelectPageSize: function () {
        _report_client_debt.SearchData();
    },
    DynamicBind: function () {
        _report_common.Select2WithFixedOptionAndNoSearch($('#report-clientdebt-filter-branch-code'))
        _report_common.ClientSuggesstion($('#report-clientdebt-filter-clientId'))
        $("body").on('select2:select', ".report-clientdebt-filter-clientId", function () {
            $('.reset-filter-clientId').show()

        });
        $("body").on('click', ".reset-filter-clientId", function () {
            $('#report-clientdebt-filter-clientId').val(null).trigger('change')
            $('.reset-filter-clientId').hide()

        });
    }
}
var _report_client_debt_detail = {
    searchModel: {
        FromDate: null,
        ToDate: null,
        ClientID: null,
        PageIndex: 1,
        PageSize: 30,
        OpeningCredit: 0,
        OpeningCreditValue:0
    },
    Variable: {
        Loading: false,
        Credit: 0,
        Page:1

    },
    Detail: function (ClientID, amount_opening_credit = 0) {
        _report_client_debt_detail.GetFilter()
        _report_client_debt_detail.searchModel.ClientID = ClientID
        _report_client_debt_detail.searchModel.OpeningCredit = amount_opening_credit
        _report_client_debt_detail.searchModel.OpeningCreditValue = amount_opening_credit
        _report_client_debt_detail.searchModel.PageIndex = 1
        _report_client_debt_detail.searchModel.PageSize = 30
        $.ajax({
            url: "/ReportClientDebt/Detail",
            type: "post",
            data: _report_client_debt_detail.searchModel,
            success: function (result) {
                _report_client_debt_detail.StopScrollingBody()
                _report_client_debt_detail.Initialization()
                $('#report-client-debt-detail-block #imgLoading').hide();
                $('#grid-report-clientdebt-detail').html(result);
                $('#report-client-debt-detail-block').show();
                $('#report-client-debt-detail-block').addClass("show");

            },
        });
    },
    DetailSearch: function () {
        $.ajax({
            url: "/ReportClientDebt/DetailSearch",
            type: "post",
            data: _report_client_debt_detail.searchModel,
            success: function (result) {
                $('#view-more-tr').before(result)
            },
        });
    },
    Initialization: function () {
        if (!_report_client_debt_detail.Variable.Loading) {
            _report_client_debt_detail.Variable.Loading = true
            _report_client_debt_detail.DynamicBindEvent()
        }
        _report_client_debt_detail.DetailSearch()

    },
    ExportExcel: function () {
        _report_client_debt_detail.GetFilter()
        _global_function.AddLoading()
        $('#report-client-debt-detail-block #operator-export-btn').prop('disabled', true);
        $('#report-client-debt-detail-block #operator-export').removeClass('fa-file-excel-o');
        $('#report-client-debt-detail-block #operator-export').addClass('fa-spinner fa-pulse');

        $.ajax({
            url: "/ReportClientDebt/ExportExcelDetail",
            type: "post",
            data: _report_client_debt_detail.searchModel,
            success: function (result) {
                _global_function.RemoveLoading()
                $('#report-client-debt-detail-block #operator-export').removeClass('fa-spinner fa-pulse');
                $('#report-client-debt-detail-block #operator-export').addClass('fa-file-excel-o');
                $('#report-client-debt-detail-block #operator-export-btn').prop('disabled', false);
                if (result.status == 0) {
                    _msgalert.success(result.msg);
                    window.location.href = result.path;
                } else {
                    _msgalert.error(result.msg);
                }

            },
        });
    },
    DynamicBindEvent: function () {
        $("body").on('click', "#report-detail-view-more", function () {
            _report_client_debt_detail.searchModel.PageIndex++;
            _report_client_debt_detail.GetFilter();
            _report_client_debt_detail.DetailSearch()
        });
    },
   
    GetFilter: function () {
        var daterange_element = $('#report_date')
        var min_date = null
        var max_date = null
        if (daterange_element != undefined && daterange_element != null) {
            min_date = _global_function.GetDayText(daterange_element.data('daterangepicker').startDate._d, true).split(' ')[0]
            max_date = _global_function.GetDayText(daterange_element.data('daterangepicker').endDate._d, true).split(' ')[0]
        }
        _report_client_debt_detail.searchModel.FromDate = min_date
        _report_client_debt_detail.searchModel.ToDate = max_date

    },
    Close: function () {
        $('#report-client-debt-detail-block').hide()
        $('#grid-report-clientdebt-detail').html('')
        $('#report-client-debt-detail-block').removeClass("show");
        _report_client_debt_detail.StartScrollingBody()

    },
    StopScrollingBody: function () {
        $('body').addClass('stop-scrolling');
    },
    StartScrollingBody: function () {
        $('body').removeClass('stop-scrolling');

    },
}
var _report_common = {
    Select2WithFixedOptionAndNoSearch: function (element) {
        var placeholder = element.attr('placeholder')
        element.select2({
            placeholder: placeholder,
            minimumResultsForSearch: Infinity

        });
    },
    ClientSuggesstion: function (element) {
        element.select2({
            theme: 'bootstrap4',
            placeholder: "Tất cả khách hàng",
            hintText: "Nhập từ khóa tìm kiếm",
            searchingText: "Đang tìm kiếm...",
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
    },
    RenderDateRangePicker: function (element) {
        var today = new Date();
        var yyyy = today.getFullYear();
        var mm = today.getMonth() + 1; // Months start at 0!
        var dd = today.getDate();

        var min_range = '01/01/2020';
        var max_range = dd + '/' + mm + '/' + yyyy;

        element.daterangepicker({
            autoApply: true,
            showDropdowns: true,
            drops: 'down',
            autoUpdateInput: false,
            minDate: min_range,
            maxDate: max_range,
            locale: {
                format: 'DD/MM/YYYY'
            }
        }, function (start, end, label) {


        });
        element.data('daterangepicker').setStartDate(min_range);
        element.data('daterangepicker').setEndDate(max_range);
        element.on('apply.daterangepicker', function (ev, picker) {
            var element = $(this)
            element.val(_global_function.GetDayText(element.data('daterangepicker').startDate._d).split(' ')[0] + ' - ' + _global_function.GetDayText(element.data('daterangepicker').endDate._d).split(' ')[0])

        });
    }
}