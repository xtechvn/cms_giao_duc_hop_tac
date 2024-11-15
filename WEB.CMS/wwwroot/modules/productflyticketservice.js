var serviceFeeType = 2;
$(document).ready(function () {
});
var _fly_ticket_service = {
    OnSaveFlyTicket: function (mode = 0) {
        let campaignCode = $('#campaignCode').val()
        if (campaignCode === undefined || campaignCode === null || campaignCode === '') {
            _msgalert.error('Mã chiến dịch không được để trống');
            return
        }
        //regex campaign code
        campaignCode = campaignCode.trim()
        var regex = new RegExp('^[a-zA-Z0-9\-\_\+]*$')
        if (!regex.test(campaignCode)) {
            _msgalert.error('Mã chiến dịch chỉ chứa kí tự chữ, số, -, _');
            return
        }
        let description = $('#description').val()
        description = description.trim()
        if (description === undefined || description === null || description === '') {
            _msgalert.error('Mô tả không được để trống')
            return
        }
        let serviceFee = $('#serviceFee').val()
        serviceFee = serviceFee.trim()
        if (serviceFee === undefined || serviceFee === null || serviceFee === '') {
            _msgalert.error('Phí dịch vụ không được để trống')
            return
        }
        if (serviceFeeType === 2) {
            if (this.IsNumeric(serviceFee.replaceAll('.', '').replaceAll(',', '')) === false) {
                _msgalert.error('Phí dịch vụ phải là số')
                return
            }
        }
        if (serviceFeeType === 1) {
            if (this.IsNumeric(serviceFee.replaceAll(',','.')) === false) {
                _msgalert.error('Phí dịch vụ phải là số')
                return
            }
        }
        if (serviceFeeType === 1 && parseInt(serviceFee) > 100) {
            _msgalert.error('Phí dịch vụ không được lớn hơn 100%')
            return
        }
        let fromDateVal = $('#fromDate').val()
        if (fromDateVal === undefined || fromDateVal === null || fromDateVal === '') {
            _msgalert.error('Ngày bắt đầu không được để trống')
            return
        }
        let toDateVal = $('#toDate').val()
        if (toDateVal === undefined || toDateVal === null || toDateVal === '') {
            _msgalert.error('Ngày kết thúc không được để trống')
            return
        }
        const [fromDateComponents, fromTimeComponents] = fromDateVal.split(' ');
        const [fromday, frommonth, fromyear] = fromDateComponents.split('/');
        const [fromhours, fromminutes] = fromTimeComponents.split(':');

        const [toDateComponents, toTimeComponents] = toDateVal.split(' ');
        const [today, tomonth, toyear] = toDateComponents.split('/');
        const [tohours, tominutes] = toTimeComponents.split(':');

        const fromDate = new Date(+fromyear, frommonth - 1, +fromday, +fromhours, +fromminutes);
        const toDate = new Date(+toyear, tomonth - 1, +today, +tohours, +tominutes);
        if (typeof fromDate.getMonth !== 'function') {
            _msgalert.error('Ngày bắt đầu không đúng định dạng')
            return
        }
        if (typeof toDate.getMonth !== 'function') {
            _msgalert.error('Ngày kết thúc không đúng định dạng')
            return
        }
        if (fromDate > toDate) {
            _msgalert.error('Ngày bắt đầu không được lớn hơn ngày kết thúc')
            return
        }
        var month = $('[name=month_tags]').val()
        var listMonth = []
        var listDays = []
        if (month !== '') {
            listMonth = JSON.parse($('[name=month_tags]').val())
        }
        var day = $('[name=weekend_day_tags]').val()
        if (day !== '') {
            listDays = JSON.parse($('[name=weekend_day_tags]').val())
        }
        var tagDayArray = []
        var tagMonthArray = []
        for (let i = 0; i < listMonth.length; i++) {
            tagMonthArray.push(listMonth[i].value)
        }
        for (let i = 0; i < listDays.length; i++) {
            tagDayArray.push(listDays[i].value)
        }
        var object_summit = {
            CampaignCode: ($('#campaignCode').val()).trim(),
            ClientTypeId: $('#client_type_id :selected').val(),
            Description: ($('#description').val()).trim(),
            FromDateStr: ($('#fromDate').val()).trim(),
            ToDateStr: ($('#toDate').val()).trim(),
            ServiceFee: serviceFeeType === 2 ? ($('#serviceFee').val()).replace('.', '') : $('#serviceFee').val(),
            ServiceFeeType: serviceFeeType,
            Status: $('input[name="status"]:checked').val(),
            Months: tagMonthArray,
            Days: tagDayArray,
            Mode: mode
        };
        let data = JSON.stringify(object_summit)
        $.ajax({
            url: '/PricePolicy/AddTicketFly',
            type: 'post',
            data: { data: data },
            success: function (result) {
                if (result.isSuccess === true) {
                    _msgalert.success(result.message);
                    $.magnificPopup.close();
                    _pricepolicymanagement.Init()
                } else {
                    _msgalert.error(result.message);
                }
            }
        });
    },
    ChooseType: function (type) {
        if (type === 1) {
            $('#serviceFeeTypeVND').removeClass('active')
            $('#serviceFeeTypePercent').addClass('active')
            serviceFeeType = 1
            var n = parseFloat($('#serviceFee').val().replace(/\D/g, ''), 10);
            if (isNaN(n) === false && n > 100) {
                n = 100
                $('#serviceFee').val(n);
            }
        }
        if (type === 2) {
            serviceFeeType = 2
            $('#serviceFeeTypeVND').addClass('active')
            $('#serviceFeeTypePercent').removeClass('active')
        }
    },
    FormatNumber: function () {
        var serviceFee = $('#serviceFee').val()
        $('#serviceFee').val(serviceFee.replaceAll(',', '.'))
        if (serviceFeeType === 2 || parseInt(serviceFeeType) === 2) {
            var n = parseFloat($('#serviceFee').val().replace(/\D/g, ''), 10);
            $('#serviceFee').val(isNaN(n) === true ? '' : n.toLocaleString());
        }
        if (serviceFeeType === 1 || parseInt(serviceFeeType) === 1) {
            var n = parseFloat($('#serviceFee').val().replace(/\D/g, ''), 10);
            if (isNaN(n) === false && n > 100) {
                n = 100
                $('#serviceFee').val(n);
            }
        }
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
    }
};