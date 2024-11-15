$(document).ready(function () {
    var date = new Date();
    var dd = date.getDate();
    var MM = date.getMonth() + 1;
    var yyyy = date.getFullYear();


    _report_supplier.RenderDateRangePicker($('#Date'))
   
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
    _report_supplier.Init();
});
let isPickerApprove = true;
let PageSize = 20;
var _report_supplier = {
    Init: function () {
     
        objSearch = this.GetParam();
        objSearch.PageSize = 20;
        _report_supplier.Search(objSearch);
    },
    Export: function () {
        objSearch = _report_supplier.GetParam();
        objSearch.PageIndex = -1;
      
        this.searchModel = objSearch
        _global_function.AddLoading()
        $.ajax({
            url: "/ReportSupplier/ExportExcel",
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
                    _global_function.RemoveLoading()
                }
               
            }
        });
    },
    SearchData: function () {
        objSearch = this.GetParam();
        if (objSearch.PageSize == undefined) {
            objSearch.PageSize = 20;
        }
        _report_supplier.Search(objSearch);
    },
    Search: function (input) {
        $.ajax({
            url: "/ReportSupplier/SearchReportSupplier",
            type: "post",
            data: input,
            success: function (result) {
                $('#imgLoading').hide();
                $('#grid_data').html(result);
                $('#selectPaggingOptions').val(input.PageSize).attr("selected", "selected");
            },
        });
    },
    GetParam: function () {
        var FromDate; //Ngày bat dau
        var ToDate; //Ngày hết hạn

        if ($('#Date').data('daterangepicker') !== undefined && $('#Date').data('daterangepicker') != null && isPickerApprove) {
            FromDate = $('#Date').data('daterangepicker').startDate._d.toLocaleDateString("en-GB");
            ToDate = $('#Date').data('daterangepicker').endDate._d.toLocaleDateString("en-GB");
        } else {
            FromDate = null
            ToDate = null
        }
    
        let _searchModel = {
            Branch: $("#Branch").val(),
            SupplierId: $("#SupplierId").val(),
            FromDate: FromDate,
            ToDate: ToDate,
            PageIndex: 1,
            PageSize: $("#selectPaggingOptions").find(':selected').val(),
        };
        return _searchModel;
    },
    OnPaging: function (values) {
        objSearch = this.GetParam();
        objSearch.PageIndex = values;
        if (objSearch.PageSize == undefined) {
            objSearch.PageSize = 20;
        }
        
        _report_supplier.Search(objSearch);
    },
    onSelectPageSize: function () {
        objSearch = this.GetParam();
        _report_supplier.Search(objSearch);
    },
    OpenPopup: function (id, name, Amount, Amount2, Amount3, Amount4) {
        objSearch = this.GetParam();
        var tile = "Chi tiết công nợ phải trả NCC: " + name + " - Ngày báo cáo:" + objSearch.FromDate + " - " + objSearch.ToDate +"";
        let url = '/ReportSupplier/DetailReportSupplier';
        let param = {
        };
        if (id.trim() != "") {
            param = {
                id: id,
                Name: name,
                FromDate: objSearch.FromDate,
                ToDate: objSearch.ToDate,
                Amount: Amount,
                Amount2: Amount2,
                Amount3: Amount3,
                Amount4: Amount4,
            };
        }
        _magnific.OpenSmallPopup(tile, url, param);
        //objSearch.SupplierId = id;
        //objSearch.PageSize = 10;
        //detail_report_supplier.Searchsupplier(objSearch);
   
    },
    RenderDateRangePicker: function (element) {
        var today = new Date();
        var yyyy = today.getFullYear();
        var mm = today.getMonth() + 1; // Months start at 0!
        var dd = today.getDate();

        var min_range = '01/01/2020';
        var max_range = dd + '/' + mm + '/' + yyyy;
        var start_day_of_month = new Date(yyyy, mm - 1, 1);
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
        element.data('daterangepicker').setStartDate(start_day_of_month);
        element.data('daterangepicker').setEndDate(max_range);
        element.on('apply.daterangepicker', function (ev, picker) {
            var element = $(this)
            element.val(_global_function.GetDayText(element.data('daterangepicker').startDate._d).split(' ')[0] + ' - ' + _global_function.GetDayText(element.data('daterangepicker').endDate._d).split(' ')[0])

        });
    }
}
var detail_report_supplier = {
    Searchsupplier: function (input) {
        _global_function.AddLoading()
        var Amount = $('#Amount').val()
        var AmountDebit = $('#AmountDebit').val()
        var AmountCredit = $('#AmountCredit').val()
        var AmountClosingBalanceCredit = $('#AmountClosingBalanceCredit').val()
        $.ajax({
            url: "/ReportSupplier/ListDetailReportSupplier",
            type: "post",
            data: {
                searchModel: input,
                Amount: Amount,
            },
            success: function (result) {
                _global_function.RemoveLoading()
                $('.grid_data_supplier').html(result);
              
                
                if (Amount >= 0) {
                    $('#td_Amount').html(_global_function.Comma(Amount));
                } else {
                    $('#td_Amount').html("-"+ _global_function.Comma(Amount));
                }
                if (AmountDebit >= 0) {
                    $('#td_AmountDebit').html(_global_function.Comma(AmountDebit));
                } else {
                    $('#td_AmountDebit').html("-" +_global_function.Comma(AmountDebit));
                }
                if (AmountCredit >= 0) {
                    $('#td_AmountCredit').html(_global_function.Comma(AmountCredit));
                } else {
                    $('#td_AmountCredit').html("-" +_global_function.Comma(AmountCredit));
                }
                if (AmountClosingBalanceCredit >= 0) {
                    $('#td_SumAmountCredit').html(_global_function.Comma(AmountClosingBalanceCredit));
                } else {
                    $('#td_SumAmountCredit').html("-" +_global_function.Comma(AmountClosingBalanceCredit));
                }
                $('#selectPaggingOptions').val(input.PageSize).attr("selected", "selected");
            },
        });
        _global_function.RemoveLoading()
    },
    OnPaging: function (values) {
        objSearch = detail_report_supplier.GetParam();
        objSearch.PageIndex = values;
        if (objSearch.PageSize == undefined) {
            objSearch.PageSize = 20;
        }
        detail_report_supplier.Searchsupplier(objSearch);
    },
    onSelectPageSize: function () {
      
        objSearch = detail_report_supplier.GetParam();
        PageSize = PageSize + 20;
        objSearch.PageSize = PageSize;
        detail_report_supplier.Searchsupplier(objSearch);
    },
    GetParam: function () {
      
        let _searchModel = {
        
            SupplierId: $("#idSupplier").val().toString(),
            FromDate: $("#FromDateSupplier").val(),
            ToDate: $("#ToDateSupplier").val(),
            PageIndex: 1,
            PageSize: 20,
        };
        return _searchModel;
    },
    Export: function () {
        objSearch = _report_supplier.GetParam();
        objSearch.PageIndex = -1;

        this.searchModel = objSearch
        _global_function.AddLoading()
        $.ajax({
            url: "/ReportSupplier/ExportExcel",
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
                    _global_function.RemoveLoading()
                }

            }
        });
    },
    Export2: function () {
        objSearch = _report_supplier.GetParam();
        objSearch.PageIndex = -1;
        objSearch.SupplierId = $('#idSupplier').val();
        var amount = $('#Amount').val();
        this.searchModel = objSearch
        _global_function.AddLoading()
        $.ajax({
            url: "/ReportSupplier/ExportExcelReportSupplier",
            type: "Post",
            data: { searchModel: this.searchModel, amount: amount },
            success: function (result) {
                _global_function.RemoveLoading()
                $('#btnExport').prop('disabled', false);
                if (result.isSuccess) {
                    _msgalert.success(result.message);
                    window.location.href = result.path;
                } else {
                    _msgalert.error(result.message);
                    _global_function.RemoveLoading()
                }

            }
        });
    },

}