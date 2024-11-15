let chartRevenu, chartLabel;

var ReportTransactionSMsByYear_html = {
    row_ac: ` <li> <label class="radio"><input type="radio" name="optradio" value="{BankName}/{AccountNumber}" ><span class="checkmark"></span>{BankName} - {AccountNumber}</label> </li>`,
}
$(document).ready(function () {
    _ReportTransactionSMsByYear.ListBankingAccount();
    _ReportTransactionSMsByYear.GetTotalAmountTransactionSMs();
    _ReportTransactionSMsByYear.GetTotalAmountPayment();
    _ReportTransactionSMsByYear.GetPaymentVoucherByDate();
});
var _ReportTransactionSMsByYear = {
    GetListBankingAccount: function (input, callback) {
        _ajax_caller.post('/DashBoard/ListBankingAccountTransactionSMs', { searchModel: input }, function (result) {
            callback(result);
        });
    },
    GetListPaymentVoucherByDate: function (input, callback) {
        _ajax_caller.post('/DashBoard/GetListTransferSmsGroupByDate', { searchModel: input }, function (result) {
            callback(result);
        });
    },
    GetSumAmountTransactionSMs: function (input, callback) {
        _ajax_caller.post('/DashBoard/SumAmountTransactionSMs', { searchModel: input }, function (result) {
            callback(result);
        });
    },
    GetCountPayment: function (input, callback) {
        _ajax_caller.post('/DashBoard/GetCountAmountPaymentVoucherByDate', { searchModel: input }, function (result) {
            callback(result);
        });
    },
    GetAmountPayment: function (input, callback) {
        _ajax_caller.post('/DashBoard/GetTotalAmountPaymentVoucherByDate', { searchModel: input }, function (result) {
            callback(result);
        });
    },
    GetTotalAmountTransactionSMs: function () {
        var newdate = new Date();
        newdate.setDate(01);
        var searchModel = {
            FromDateStr: newdate.toLocaleDateString("en-GB"),
            ToDateStr: new Date().toLocaleDateString("en-GB"),
            type: 1,
        };
        _ReportTransactionSMsByYear.GetSumAmountTransactionSMs(searchModel, function (data) {
            amount2 = data.amount;
            $("#total_Amount_TransactionSMs").html(data.amount.toLocaleString());

        });
    },
    GetTotalAmountPayment: function () {
        var newdate = new Date();
        newdate.setDate(01);
        var searchModel = {
            FromDateStr: newdate.toLocaleDateString("en-GB"),
            ToDateStr: new Date().toLocaleDateString("en-GB"),
            type: 1,
        };
        _ReportTransactionSMsByYear.GetAmountPayment(searchModel, function (data) {
            $("#total_Amount").html(Math.round(data.totalAmountTransactionSMs.amount - data.totalAmountPayment).toLocaleString());
            $("#total_Amount_Payment").html(data.totalAmountPayment.toLocaleString());

        });
    },
    ListBankingAccount: function () {
        var rowAc = '';
        var searchModel = {
            FromDateStr: new Date().toLocaleDateString("en-GB"),
            ToDateStr: new Date().toLocaleDateString("en-GB"),
        };
        _ReportTransactionSMsByYear.GetListBankingAccount(searchModel, function (data) {
         
            data.forEach(function (item) {
                rowAc += ReportTransactionSMsByYear_html.row_ac.replaceAll('{AccountNumber}', item.accountNumber).replaceAll('{BankName}', item.bankId);
            });
            $('#List_AC').html(rowAc);
        });
       
    },
    GetPaymentVoucherByDate: function () {
        $('#revenuChart').hide();
        var newdate = new Date();
        newdate.setDate(01);
        newdate.setMonth(00);
      
        var searchModel = {
            FromDateStr: newdate.toLocaleDateString("en-GB"),
            ToDateStr: new Date().toLocaleDateString("en-GB"),
            AccountNumber: $('input[name=optradio]:checked').val(),
        };
        _ReportTransactionSMsByYear.GetListPaymentVoucherByDate(searchModel, function (data) {

            $('#revenuChart').show();
            var Row_th = '<th>Loại</th>';
            var Row_tr = '<td>Tiền vào</td>';
            var Row_tr2 = '<td>Tiền Ra</td>';
            var Row_tr3 = '<td>Balance</td>';
            var date = [];
            var amount = [];
            var Balance = [];
            var amountTransaction = [];
            data.forEach(function (item) {
                date.push('Tháng ' + item.month);
                Row_th += '<th>Tháng ' + item.month+'</th>';
                amount.push(item.amount);
                Row_tr2 += '<td>' + item.amount.toLocaleString() + ' đ</td>';
                amountTransaction.push(item.amountTransaction);
                Row_tr += '<td>' + item.amountTransaction.toLocaleString() + ' đ</td>';
                Balance.push(item.balance);
                Row_tr3 += '<td>' + item.balance.toLocaleString() + ' đ</td>';
            });
            chart.LoadChartRevenu(date, amountTransaction, amount, Balance);
            $('#Row_th').html(Row_th)
            $('#tbody_Row_tr').html(Row_tr)
            $('#tbody_Row_tr2').html(Row_tr2)
            $('#tbody_Row_tr3').html(Row_tr3)

        });
    },
    Search: function () {
        _global_function.AddLoading()
        $('#form_down_check_radio').attr('style', 'display: none;min-width: 290px;');
        $('#btn_check_radio').removeClass('active');
        var textAc = $('input[name=optradio]:checked').val();
        var List_textAc = textAc != undefined ? textAc.split('/') : null;
        var newdate = new Date();
        newdate.setDate(01);
        newdate.setMonth(00);
        var id = $('#ReceiveTime').val();
        var searchModel = {
            FromDateStr: newdate.toLocaleDateString("en-GB"),
            ToDateStr: new Date().toLocaleDateString("en-GB"),
            AccountNumber: List_textAc != null ? List_textAc[1] : null,
            BankName: List_textAc != null ? List_textAc[0] : null,
        };
        console.log(newdate.getFullYear())
        if (parseFloat(id) != newdate.getFullYear()) {
            var newdate2 = new Date();
            newdate2.setDate(31);
            newdate2.setMonth(11);
            newdate2.setFullYear(parseFloat(id));
            newdate.setFullYear(parseFloat(id));
            searchModel.FromDateStr = newdate.toLocaleDateString("en-GB")
            searchModel.ToDateStr = newdate2.toLocaleDateString("en-GB")
        } else {
            searchModel.FromDateStr = newdate.toLocaleDateString("en-GB")
            searchModel.ToDateStr = new Date().toLocaleDateString("en-GB")
        }
        
        _ReportTransactionSMsByYear.GetListPaymentVoucherByDate(searchModel, function (data) {
            $('#revenuChart').hide();
   
            var Row_th = '<th>Loại</th>';
            var Row_tr = '<td>Tiền vào</td>';
            var Row_tr2 = '<td>Tiền Ra</td>';
            var Row_tr3 = '<td>Balance</td>';
            var date_Year = [];
            var amount_Year = [];
            var Balance_Year = [];
            var amountTransaction_Year = [];
            data.forEach(function (item) {
                date_Year.push('Tháng ' + item.month);
                Row_th += '<th>Tháng ' + item.month + '</th>';
                amount_Year.push(item.amount);
                Row_tr2 += '<td>' + item.amount.toLocaleString() + ' đ</td>';
                amountTransaction_Year.push(item.amountTransaction);
                Row_tr += '<td>' + item.amountTransaction.toLocaleString() + ' đ</td>';
                Balance_Year.push(item.balance);
                Row_tr3 += '<td>' + item.balance.toLocaleString() + ' đ</td>';
            });
            $('#revenuChart').show();
            chart.LoadChartRevenu(date_Year, amountTransaction_Year, amount_Year, Balance_Year);
            $('#Row_th').html(Row_th)
            $('#tbody_Row_tr').html(Row_tr)
            $('#tbody_Row_tr2').html(Row_tr2)
            $('#tbody_Row_tr3').html(Row_tr3)
            _global_function.RemoveLoading()
        });
    },
}

var chart = {
    LoadChartRevenu: function (listLabel, listRevenu, listRevenu2, listRevenu3) {
        if (chartRevenu != null && chartRevenu != undefined) chartRevenu.destroy();
        if (chartRevenu != undefined) chartRevenu.destroy();
        var densityCanvas = document.getElementById("revenuChart").getContext('2d');;

        var densityData = {
            label: 'Tiền vào',
            data: listRevenu,
            backgroundColor: 'blue',
            stack: 1,


        };

        var gravityData = {
            label: 'Tiền ra',
            data: listRevenu2,
            backgroundColor: 'aqua',
            stack: 2,


        };
        var BalanceData = {
            label: 'Balance',
            data: listRevenu3,
            backgroundColor: 'yellow',
            stack: 3,


        };
        var barChartData = {
            labels: listLabel,
            datasets: [densityData, gravityData, BalanceData]
        };

        var chartOptions = {
            scales: {
                yAxes: [{
                    ticks: {
                        fontColor: 'black',
                        beginAtZero: true,
                        callback: function (value, index, values) {
                            return (value / 1000000000).toLocaleString() + " Tỷ";
                        }
                    },
                    stacked: true,

                }],
                xAxes: [{

                    categoryPercentage: 0.5
                }],

            },

            tooltips: {
                enabled: true,
                mode: 'single',
                callbacks: {
                    title: function () { },
                    label: function (tooltipItems, data) {
                        var multistringText = [];
                        multistringText.push(" " + tooltipItems.yLabel.toLocaleString() + " đ");
                        return multistringText;
                    }
                }
            },
        };
        chartRevenu =new Chart(densityCanvas, {
            type: 'bar',
            data: barChartData,
            options: chartOptions
        });
        if (chartRevenu != undefined) chartRevenu.update();
        
    },


}