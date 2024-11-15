$(document).ready(function () {
    _ReportTransactionSMs.LoadReportTransactionSMs();
});
var _ReportTransactionSMs = {
    LoadReportTransactionSMs: function () {
        var searchModel = {
            FromDateStr: new Date().toLocaleDateString("en-GB"),
            ToDateStr: new Date().toLocaleDateString("en-GB"),
        };
        _ReportTransactionSMs.Search(searchModel);
    },
    Search: function (input) {
        _global_function.AddLoading()
        $.ajax({
            url: "/TransactionSms/GetListReportTransactionSMs",
            type: "Post",
            data: { searchModel: input },
            success: function (result) {
                _global_function.RemoveLoading()
                $('#grid_data_Search').html(result);
            }
        });
    },
    getList: function () {
        var type = $('input[name=optradio]:checked').val();
        $('#form_down_check_radio').attr('style','display: none');
        $('#btn_check_radio').removeClass('active');

        var searchModel = {
            FromDateStr: null,
            ToDateStr: null,
        };
        switch (type) {
            case "1": {
                searchModel.FromDateStr = new Date().toLocaleDateString("en-GB");
                searchModel.ToDateStr = new Date().toLocaleDateString("en-GB");
                $('#check_radio_name').text("Hôm nay")
            }
                break;
            case "2": {
                var newDate = new Date(_global_function.ParseDateTostring(new Date().toLocaleDateString("en-GB")));

                newDate.setDate(newDate.getDate() - 1);
                searchModel.FromDateStr = newDate.toLocaleDateString("en-GB");
                searchModel.ToDateStr = newDate.toLocaleDateString("en-GB");
                $('#check_radio_name').text("Hôm qua")
            }
                break;
            case "3": {
                var newDate = new Date(_global_function.ParseDateTostring(new Date().toLocaleDateString("en-GB")));

                newDate.setDate(newDate.getDate() - 7);
                searchModel.FromDateStr = newDate.toLocaleDateString("en-GB");
                searchModel.ToDateStr = new Date().toLocaleDateString("en-GB");
                $('#check_radio_name').text("Tuần trước")
            }
                break;
            case "4": {
                var newdate = new Date();
                newdate.setDate(01);
                searchModel.FromDateStr = newdate.toLocaleDateString("en-GB");
                searchModel.ToDateStr = new Date().toLocaleDateString("en-GB");
                $('#check_radio_name').text("Tháng này")
            }
                break;
            case "5": {
                var newDate = new Date(_global_function.ParseDateTostring(new Date().toLocaleDateString("en-GB")));
                newDate.setDate(01);
                newDate.setMonth(newDate.getMonth() - 1);
                var newDate2 = new Date(_global_function.ParseDateTostring(new Date().toLocaleDateString("en-GB")));
                newDate2.setDate(30);
                newDate2.setMonth(newDate2.getMonth() - 1);

                searchModel.FromDateStr = newDate.toLocaleDateString("en-GB");
                searchModel.ToDateStr = newDate2.toLocaleDateString("en-GB");
                $('#check_radio_name').text("Tháng trước")
            }
                break;
            case "6": {
                $('#check_radio_name').text("Quý này")
                var newDate = new Date(_global_function.ParseDateTostring(new Date().toLocaleDateString("en-GB")));
                var month = newDate.getMonth() + 1;
                var quarter = Math.ceil(month / 3);
                switch (quarter) {
                    case 1: {
                        var newDate = new Date(_global_function.ParseDateTostring(new Date().toLocaleDateString("en-GB")));
                        newDate.setDate(01);
                        newDate.setMonth(00);
                        var newDate2 = new Date(_global_function.ParseDateTostring(new Date().toLocaleDateString("en-GB")));
                        newDate2.setDate(31);
                        newDate2.setMonth(02);

                        searchModel.FromDateStr = newDate.toLocaleDateString("en-GB");
                        searchModel.ToDateStr = newDate2.toLocaleDateString("en-GB");
                    } break;
                    case 2: {
                        var newDate = new Date(_global_function.ParseDateTostring(new Date().toLocaleDateString("en-GB")));
                        newDate.setDate(01);
                        newDate.setMonth(03);
                        var newDate2 = new Date(_global_function.ParseDateTostring(new Date().toLocaleDateString("en-GB")));
                        newDate2.setDate(30);
                        newDate2.setMonth(05);

                        searchModel.FromDateStr = newDate.toLocaleDateString("en-GB");
                        searchModel.ToDateStr = newDate2.toLocaleDateString("en-GB");
                    } break;
                    case 3: {
                        var newDate = new Date(_global_function.ParseDateTostring(new Date().toLocaleDateString("en-GB")));
                        newDate.setDate(01);
                        newDate.setMonth(06);
                        var newDate2 = new Date(_global_function.ParseDateTostring(new Date().toLocaleDateString("en-GB")));
                        newDate2.setDate(30);
                        newDate2.setMonth(08);

                        searchModel.FromDateStr = newDate.toLocaleDateString("en-GB");
                        searchModel.ToDateStr = newDate2.toLocaleDateString("en-GB");
                    } break;
                    case 4: {
                        var newDate = new Date(_global_function.ParseDateTostring(new Date().toLocaleDateString("en-GB")));
                        newDate.setDate(01);
                        newDate.setMonth(09);
                        var newDate2 = new Date(_global_function.ParseDateTostring(new Date().toLocaleDateString("en-GB")));
                        newDate2.setDate(31);
                        newDate2.setMonth(11);

                        searchModel.FromDateStr = newDate.toLocaleDateString("en-GB");
                        searchModel.ToDateStr = newDate2.toLocaleDateString("en-GB");
                    } break;
                }
            }
                break;
            case "7": {
                $('#check_radio_name').text("Quý trước")
                var newDate = new Date(_global_function.ParseDateTostring(new Date().toLocaleDateString("en-GB")));
                var month = newDate.getMonth() + 1;
                var quarter = Math.ceil(month / 3) - 1;
                switch (quarter) {
                    case 1: {
                        var newDate = new Date(_global_function.ParseDateTostring(new Date().toLocaleDateString("en-GB")));
                        newDate.setDate(01);
                        newDate.setMonth(00);
                        var newDate2 = new Date(_global_function.ParseDateTostring(new Date().toLocaleDateString("en-GB")));
                        newDate2.setDate(31);
                        newDate2.setMonth(02);

                        searchModel.FromDateStr = newDate.toLocaleDateString("en-GB");
                        searchModel.ToDateStr = newDate2.toLocaleDateString("en-GB");
                    } break;
                    case 2: {
                        var newDate = new Date(_global_function.ParseDateTostring(new Date().toLocaleDateString("en-GB")));
                        newDate.setDate(01);
                        newDate.setMonth(03);
                        var newDate2 = new Date(_global_function.ParseDateTostring(new Date().toLocaleDateString("en-GB")));
                        newDate2.setDate(30);
                        newDate2.setMonth(05);

                        searchModel.FromDateStr = newDate.toLocaleDateString("en-GB");
                        searchModel.ToDateStr = newDate2.toLocaleDateString("en-GB");
                    } break;
                    case 3: {
                        var newDate = new Date(_global_function.ParseDateTostring(new Date().toLocaleDateString("en-GB")));
                        newDate.setDate(01);
                        newDate.setMonth(06);
                        var newDate2 = new Date(_global_function.ParseDateTostring(new Date().toLocaleDateString("en-GB")));
                        newDate2.setDate(30);
                        newDate2.setMonth(08);

                        searchModel.FromDateStr = newDate.toLocaleDateString("en-GB");
                        searchModel.ToDateStr = newDate2.toLocaleDateString("en-GB");
                    } break;
                    case 4: {
                        var newDate = new Date(_global_function.ParseDateTostring(new Date().toLocaleDateString("en-GB")));
                        newDate.setDate(01);
                        newDate.setMonth(09);
                        var newDate2 = new Date(_global_function.ParseDateTostring(new Date().toLocaleDateString("en-GB")));
                        newDate2.setDate(31);
                        newDate2.setMonth(11);

                        searchModel.FromDateStr = newDate.toLocaleDateString("en-GB");
                        searchModel.ToDateStr = newDate2.toLocaleDateString("en-GB");
                    } break;
                }
            }
                break;
        }
        console.log(searchModel.FromDateStr)
        console.log(searchModel.ToDateStr)
        _ReportTransactionSMs.Search(searchModel);
    }
}