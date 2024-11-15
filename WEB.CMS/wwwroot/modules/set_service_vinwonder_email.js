var _set_service_vinwonder_email_html = {
    html_loading_email_vinwonder_text: 'Đang thực hiện gửi Email vé VinWonder, vui lòng không đóng cửa sổ này',
    dot_text: '',
    email_success: false
}

var _set_service_vinwonder_email = {
    Initialization: function () {
        _common_function_vinwonder.Select2FixedOptionWithAddNew($('#to_email'))
        _common_function_vinwonder.Select2FixedOptionWithAddNew($('#cc_email'))
        _common_function_vinwonder.Select2FixedOptionWithAddNew($('#bcc_email'))
        try {
            var _wrapperImage = $("#lightgallery-file");
            _wrapperImage.data('lightGallery').destroy(true);
        } catch {

        }
        _wrapperImage.lightGallery();

        setTimeout(function () {
            _set_service_vinwonder_email.StopScrollingBody();
            $('#vinwonder-email').addClass('show')
            $('#vinwonder-email').find('style').remove()

        }, 300);

    },
    Close: function () {
        $('#vinwonder-email').removeClass('show')
        setTimeout(function () {
            $('#vinwonder-email').remove();
            _set_service_vinwonder_email.StartScrollingBody();
            _set_service_vinwonder_email.RemoveDynamicBind();
        }, 300);
    },
    StopScrollingBody: function () {
        $('body').addClass('stop-scrolling');
    },
    StartScrollingBody: function () {
        $('body').removeClass('stop-scrolling');

    },

    RemoveDynamicBind: function () {

    },
    ConfirmSendEmail: function () {
        $('#send_email_vinwonder_ticket').attr('disabled', 'disabled')
        $('#sendemail-loading').show();
        _set_service_vinwonder_email.LoopDisplayLoading()
        $('#send_email_vinwonder_ticket').closest('.text-right').hide()
        var subject = $('#vinwonder-email').find('#subject').val()
        var order_id = $('#vinwonder-email').attr('data-orderid')
        var booking_id = $('#vinwonder-email').attr('data-bookingid')

        var to_email = $('#vinwonder-email').find('#to_email').select2('data').map(a => a.id)
        var cc_email = $('#vinwonder-email').find('#cc_email').select2('data').map(a => a.id)
        var bcc_email = $('#vinwonder-email').find('#bcc_email').select2('data').map(a => a.id)
        $.ajax({
            url: "/SetService/SendEmailVinWonderTicket",
            type: "post",
            data: {
                order_id: order_id,
                booking_id: booking_id,
                subject: subject,
                to_email: to_email,
                cc_email: cc_email,
                bcc_email: bcc_email,
            },
            success: function (result) {
                $('#sendemail-loading').hide();

                if (result.status == 0) {
                    _msgalert.success(result.msg)
                    $('#loading_exportvw').html(result.msg)
                    setTimeout(function () {
                        _set_service_vinwonder_email.Close();

                    }, 1000);
                } else {
                    $('#send_email_vinwonder_ticket').removeAttr('disabled');
                    _msgalert.error(result.msg)
                    $('#send_email_vinwonder_ticket').closest('.text-right').show()

                }

            }
        });
    },
    LoopDisplayLoading: function () {
        setTimeout(function () {
            _set_service_vinwonder_email_html.dot_text = _set_service_vinwonder_email_html.dot_text + '.'
            $('#loading_exportvw').html(_set_service_vinwonder_email_html.html_loading_email_vinwonder_text + _set_service_vinwonder_email_html.dot_text)
            if (_set_service_vinwonder_email_html.dot_text.trim() == '....') _set_service_vinwonder_email_html.dot_text = ''
            if (!_set_service_vinwonder_email_html.email_success) _set_service_vinwonder_email.LoopDisplayLoading()
        }, 1000)
    },
}

