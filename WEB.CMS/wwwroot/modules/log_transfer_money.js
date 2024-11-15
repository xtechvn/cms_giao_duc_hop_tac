
let fields = {
    STT: true,
    MessageContent: true,
    BankName: true,
    Amount: true,
    BookingCode: true,
    ReceiveTime: true,
    UpdateTime: true,
    Status: true
}
let cookieName = 'fields_transactionsms'
var timer;
$(document).ready(function () {
    var today = new Date()
    let yyyy = today.getFullYear();
    let mm = today.getMonth() + 1;
    let dd = today.getDate();
    const toDateFormat = (dd < 10 ? '0' + dd : dd) + '/' + (mm < 10 ? '0' + mm : mm) + '/' + yyyy;
    $('#toDate').val(toDateFormat)
    today.setDate(today.getDate() - 3);
    yyyy = today.getFullYear();
    mm = today.getMonth() + 1;
    dd = today.getDate();
    const fromDateFormat = (dd < 10 ? '0' + dd : dd) + '/' + (mm < 10 ? '0' + mm : mm) + '/' + yyyy;
    $('#fromDate').val(fromDateFormat)

    _log_transfer_money.Init();
    $(".input").on('keyup', function (e) {
        if (e.key === 'Enter' || e.keyCode === 13) {
            _log_transfer_money.OnPaging(1)
        }
    });
    if (_log_transfer_money.getCookie('fields_transactionsms') != null) {
        let cookie = _log_transfer_money.getCookie(cookieName)
        fields = JSON.parse(cookie)
    } else {
        _log_transfer_money.setCookie(cookieName, JSON.stringify(fields), 10)
    }
    _log_transfer_money.checkShowHide();
});
var _log_transfer_money = {
    Init: function () {
        let _searchModel = {
            FromDateStr: $('#fromDate').val(),
            ToDateStr: $('#toDate').val(),
            Amount: $('#amount').val() !== null && $('#amount').val() !== '' ? parseFloat(($('#amount').val()).replaceAll('.', '').replaceAll(',', '')) : -1,
            BankName: $('#bankName').val(),
            BookingCode: $('#orderCode').val(),
            StatusSuccess: $('#successStatus').is(":checked"),
            StatusFail: $('#failStatus').is(":checked")
        };
        let objSearch = {
            searchModel: _searchModel,
            currentPage: 1,
            pageSize: 20
        };
        this.SearchParam = objSearch;
        this.Search(objSearch);
    },
    OnPaging: function (value) {
        let _searchModel = {
            FromDateStr: $('#fromDate').val(),
            ToDateStr: $('#toDate').val(),
            Amount: $('#amount').val() !== null && $('#amount').val() !== '' ? parseFloat(($('#amount').val()).replaceAll('.', '').replaceAll(',', '')) : -1,
            BankName: $('#bankName').val(),
            BookingCode: $('#orderCode').val(),
            StatusSuccess: $('#successStatus').is(":checked"),
            StatusFail: $('#failStatus').is(":checked"),
            AmountSuccess: $('#amountSuccess').is(":checked"),
            AmountFail: $('#amountFail').is(":checked")
        };
        var objSearch = this.SearchParam;
        objSearch.currentPage = value;
        objSearch.searchModel = _searchModel;
        this.Search(objSearch);
    },
    OnChangeInput: function () {
        clearTimeout(timer);
        timer = setTimeout(function () {
            _log_transfer_money.OnPaging(1);
        }, 1500);
    },
    Search: function (input) {
        let amount = $('#amount').val()
        amount = amount.trim()
        if (amount !== null && amount !== '') {
            if (this.IsNumeric(amount.replaceAll('.', '').replaceAll(',', '')) === false) {
                _msgalert.error('Số tiền chuyển không đúng định dạng số')
                return
            }
        }
        let fromDateVal = $('#fromDate').val()
        let toDateVal = $('#toDate').val()
        if (fromDateVal != null && fromDateVal != '' && toDateVal != null && toDateVal != '') {
            const [fromday, frommonth, fromyear] = fromDateVal.split('-');
            const [today, tomonth, toyear] = toDateVal.split('-');
            const fromDate = new Date(+fromyear, frommonth - 1, +fromday, 0, 0);
            const toDate = new Date(+toyear, tomonth - 1, +today, 0, 0);
            if (fromDate > toDate) {
                _msgalert.error('Ngày bắt đầu không được lớn hơn ngày kết thúc');
                return
            }
        }
        $('#imgLoading').show();
       /* _global_function.AddLoading()*/
        $.ajax({
            url: "/TransactionSms/Search",
            type: "Post",
            data: input,
            success: function (result) {
                /*_global_function.RemoveLoading()*/
                $('#imgLoading').hide();
                $('#grid_data').html(result);
                $('.checkbox-tb-column').each(function () {
                    let seft = $(this);
                    let id = seft.data('id');
                    if (seft.is(':checked')) {
                        $('td:nth-child(' + id + '),th:nth-child(' + id + ')').removeClass('mfp-hide');
                    } else {
                        $('td:nth-child(' + id + '),th:nth-child(' + id + ')').addClass('mfp-hide');
                    }
                });
            }
        });
    },
    showColumn: function () {
        _global_function.AddLoading()
        $.ajax({
            type: "POST",
            url: "/TransactionSms/renderColumnChecked",
            contentType: "application/json",
            type: 'POST',
            success: function (data) {
                _global_function.RemoveLoading()
                if (data.error == 0) {
                    $(".show_column").html(data.data);
                }
            },
            error: function (msg) {
                alert('Lỗi xảy ra:' + msg + ' -> Goi 4827 de duoc tro giup')
            }
        });
    },
    showHideColumn: function () {
        $('.checkbox-tb-column').each(function () {
            let seft = $(this);
            let id = seft.data('id');
            if (seft.is(':checked')) {
                $('td:nth-child(' + id + '),th:nth-child(' + id + ')').removeClass('mfp-hide');
            } else {
                $('td:nth-child(' + id + '),th:nth-child(' + id + ')').addClass('mfp-hide');
            }
        });
    },
    checkShowHide: function () {
        if (fields.STT === true) {
            $('#sttDisplay').prop('checked', true);
        } else {
            $('#sttDisplay').prop('checked', false);
        }
        if (fields.MessageContent === true) {
            $('#messageContentDisplay').prop('checked', true);
        } else {
            $('#messageContentDisplay').prop('checked', false);
        }
        if (fields.BankName === true) {
            $('#bankNameDisplay').prop('checked', true);
        } else {
            $('#bankNameDisplay').prop('checked', false);
        }
        if (fields.Amount === true) {
            $('#amountDisplay').prop('checked', true);
        } else {
            $('#amountDisplay').prop('checked', false);
        }
        if (fields.BookingCode === true) {
            $('#orderCodeDisplay').prop('checked', true);
        } else {
            $('#orderCodeDisplay').prop('checked', false);
        }
        if (fields.ReceiveTime === true) {
            $('#receiveDateDisplay').prop('checked', true);
        } else {
            $('#receiveDateDisplay').prop('checked', false);
        }
        if (fields.UpdateTime === true) {
            $('#updateDateDisplay').prop('checked', true);
        } else {
            $('#updateDateDisplay').prop('checked', false);
        }
        if (fields.Status === true) {
            $('#statusDisplay').prop('checked', true);
        } else {
            $('#statusDisplay').prop('checked', false);
        }
    },
    changeSetting: function (position) {
        this.showHideColumn();
        switch (position) {
            case 1:
                if ($('#sttDisplay').is(":checked")) {
                    fields.STT = true
                } else {
                    fields.STT = false
                }
                this.eraseCookie(cookieName);
                this.setCookie(cookieName, JSON.stringify(fields), 10);
                break;
            case 2:
                if ($('#messageContentDisplay').is(":checked")) {
                    fields.MessageContent = true
                } else {
                    fields.MessageContent = false
                }
                this.eraseCookie(cookieName);
                this.setCookie(cookieName, JSON.stringify(fields), 10);
                break;
            case 3:
                if ($('#bankNameDisplay').is(":checked")) {
                    fields.BankName = true
                } else {
                    fields.BankName = false
                }
                this.eraseCookie(cookieName);
                this.setCookie(cookieName, JSON.stringify(fields), 10);
                break;
            case 4:
                if ($('#amountDisplay').is(":checked")) {
                    fields.Amount = true
                } else {
                    fields.Amount = false
                }
                this.eraseCookie(cookieName);
                this.setCookie(cookieName, JSON.stringify(fields), 10);
                break;
            case 5:
                if ($('#orderCodeDisplay').is(":checked")) {
                    fields.BookingCode = true
                } else {
                    fields.BookingCode = false
                }
                this.eraseCookie(cookieName);
                this.setCookie(cookieName, JSON.stringify(fields), 10);
                break;
            case 6:
                if ($('#receiveDateDisplay').is(":checked")) {
                    fields.ReceiveTime = true
                } else {
                    fields.ReceiveTime = false
                }
                this.eraseCookie(cookieName);
                this.setCookie(cookieName, JSON.stringify(fields), 10);
                break;
            case 7:
                if ($('#updateDateDisplay').is(":checked")) {
                    fields.UpdateTime = true
                } else {
                    fields.UpdateTime = false
                }
                this.eraseCookie(cookieName);
                this.setCookie(cookieName, JSON.stringify(fields), 10);
                break;
            case 8:
                if ($('#statusDisplay').is(":checked")) {
                    fields.Status = true
                } else {
                    fields.Status = false
                }
                this.eraseCookie(cookieName);
                this.setCookie(cookieName, JSON.stringify(fields), 10);
                break;
            default:
        }
    },
    setCookie: function (name, value, days) {
        var expires = "";
        if (days) {
            var date = new Date();
            date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
            expires = "; expires=" + date.toUTCString();
        }
        document.cookie = name + "=" + (value || "") + expires + "; path=/";
    },
    getCookie: function (name) {
        var nameEQ = name + "=";
        var ca = document.cookie.split(';');
        for (var i = 0; i < ca.length; i++) {
            var c = ca[i];
            while (c.charAt(0) == ' ') c = c.substring(1, c.length);
            if (c.indexOf(nameEQ) == 0) return c.substring(nameEQ.length, c.length);
        }
        return null;
    },
    eraseCookie: function (name) {
        document.cookie = name + '=; Path=/; Expires=Thu, 01 Jan 1970 00:00:01 GMT;';
    },
    FormatNumber: function () {
        var amount = $('#amount').val()
        $('#amount').val(amount.replaceAll(',', '.'))
        var n = parseFloat($('#amount').val().replace(/\D/g, ''), 10);
        $('#amount').val(isNaN(n) === true ? '' : n.toLocaleString());
    },
    RemoveAccents: function (str) {
        return str.normalize('NFD')
            .replace(/[\u0300-\u036f]/g, '')
            .replace(/đ/g, 'd').replace(/Đ/g, 'D');
    },
    FormatCode: function () {
        var code = $('#campaignCode').val()
        code = this.RemoveAccents(code)
        code = code.replaceAll(' ', '_')
        code = (code.normalize('NFD')).toUpperCase()
        $('#campaignCode').val(code);
    },
    IsNumeric: function (str) {
        if (typeof str != "string") return false // we only process strings!  
        return !isNaN(str) && // use type coercion to parse the _entirety_ of the string (`parseFloat` alone does not do this)...
            !isNaN(parseFloat(str)) // ...and ensure strings of whitespace fail
    },
    Export: function () {
       
        let searchModel = {
            FromDateStr: $('#fromDate').val(),
            ToDateStr: $('#toDate').val(),
            Amount: $('#amount').val() !== null && $('#amount').val() !== '' ? parseFloat(($('#amount').val()).replaceAll('.', '').replaceAll(',', '')) : -1,
            BankName: $('#bankName').val(),
            BookingCode: $('#orderCode').val(),
            StatusSuccess: $('#successStatus').is(":checked"),
            StatusFail: $('#failStatus').is(":checked")
        };
     
        _global_function.AddLoading()
        $.ajax({
            url: "/TransactionSms/ExportExcel",
            type: "Post",
            data: searchModel,
            success: function (result) {
                _global_function.RemoveLoading()
                if (result.isSuccess) {
                    _msgalert.success(result.message);
                    window.location.href = result.path;
                } else {
                    _msgalert.error(result.message);
                }
             
            }
        });
    },
};