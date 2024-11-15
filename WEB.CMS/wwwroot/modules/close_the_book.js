
$(document).ready(function () {

    _close_the_book.GetListDate();
    $('body').on('click', '.checkbox', function () {
        var element = $(this)
        var title = 'Khóa sổ';
        var description = '';
        var Month_day = new Date().getMonth();
        var Month = element.closest('.list-row').find('.Month').text();
        var StartDate = element.closest('.list-row').find('.day-StartDate').text();
        var EndDate = element.closest('.list-row').find('.day-EndDate').text();
        if (parseFloat(Month) < parseFloat(Month_day + 1)) {
            if (element.is(":checked") == true) {
                description = 'Bạn muốn khóa sổ Tháng ' + Month + ',từ ngày ' + StartDate + ' đến ngày ' + EndDate
                _close_the_book.openDialogSummit(title, description, function () {
                    var model =  {
                        FromDateStr: StartDate,
                        ToDateStr: EndDate,
                    }
                    $.ajax({
                        url: "/CloseTheBook/OrderBookClosing",
                        type: "Post",
                        data: { model: model },
                        success: function (result) {
                            if (result.status > 0) {
                                _msgalert.error(result.message);
                            }
                            else {
                                _msgalert.success(result.message);
                                setTimeout(function () {
                                    window.location.reload();
                                }, 300);
                            }
                        }
                    });
                  

                },
                    function () {
                        element.removeAttr("checked")
                        _msgalert.error('Khóa sổ tháng ' + Month + ' Không thành công');
                    }
                );

            }
            else {
                //title = 'Mở sổ'
                //description = 'Bạn muốn mở sổ Tháng ' + Month + ',từ ngày ' + StartDate + ' đến ngày ' + EndDate
                //_close_the_book.openDialogSummit(title, description,
                //    function () {
                //        _msgalert.success('Mở sổ tháng ' + Month + ' thành công');

                //    },
                //    function () {
                //        element.prop("checked", true);
                //        _msgalert.error('Mở sổ tháng ' + Month + ' Không thành công');
                //    }
                //);
            }
        }
        else {
            element.removeAttr("checked")
            _msgalert.error('bạn không thể khóa sổ tháng ' + Month + ' vì chưa đến ngày');
        }
     
    });
});
var _close_the_book = {
    GetListDate: function () {
        var date = $('#date_time').val();
        window.scrollTo(0, 0);
        $.ajax({
            url: "/CloseTheBook/GetDateByYear",
            type: "Post",
            data: { date: date },
            success: function (result) {
                $('#grid_data_date').html(result);
            }
        });
    },
    openDialogSummit: function (title, description, callback, callback2) {
        Swal.fire({
            title: title,
            text: description,
            // width: 600,
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#ED5C6A',
            cancelButtonColor: '#A6A4A4',
            confirmButtonText: '<i class="fa fa-check"></i> Đồng ý',
            cancelButtonText: '<i class="fa fa-minus-circle"></i> Bỏ qua'
        }).then((result) => {
            if (result.dismiss == "cancel") {
                callback2();
            } else {
                callback();
            }
        })
    },
}