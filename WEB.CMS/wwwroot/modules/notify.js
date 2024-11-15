var _menu_html = {
    html_menu_notify: '<li class= "active"  onclick="_noti.UpdateNotify($(this))" data="{id}"> <a href="{link}" > <span class="ava"><img src="/images/icons/user.png"></span> {note}  <div class="time">{date}</div></a></li> ',
    html_menu_notify_active: '<li class="ava"  onclick="_noti.UpdateNotify($(this))" data="{id}"> <a href="{link}" > <span class="ava"><img src="/images/icons/user.png"></span> {note}  <div class="time">{date}</div></a></li> ',
    html_menu_notify_coutn: '<span class="coutn">{coutn}<input id="lst_id_not_seen" value="{value}" style="display:none;"/><input id="total_not" value="{total_not}" style="display:none;"/>',
    html_menu_notify_coutn2: '<input id="lst_id_not_seen" value="{value}" style="display:none;"/><input id="total_not" value="{coutn}" style="display:none;"/>',
    html_menu_notify_thongbao: ' <span class="note">{note}</span> <ul id="Notify" class="scroll-inner"></ul >    <a class="view-all" href="javascript:;">Xem tất cả</a>',
}

$(document).ready(function () {
   // _noti.loadNotify();
    $("body").on('click', ".ava", function () {
        _noti.UpdateNotify($(this))
    });

    _noti.loadNotify();

});

var _noti = {
    loadNotify: function () {
    

        $.ajax({
            url: "/menu/Notify",
            type: "POST",
            data: {},
            success: function (result) {
                if (result.status == 0 && result.data != null) {
                    if (total_not != result.data.total_not_seen) {

                        $('#note-tt').html(_menu_html.html_menu_notify_thongbao.replace('{note}', "Thông báo từ hệ thống"))
                     
                        if (result.data.total_not_seen > 0) {
                            $('#coutn-noti').html(_menu_html.html_menu_notify_coutn.replace('{coutn}', result.data.total_not_seen).replace('{value}', result.data.lst_id_not_seen).replace('{total_not}', result.data.total_not_seen))
                        } else {
                            $('#coutn-noti').html(_menu_html.html_menu_notify_coutn2.replace('{coutn}', 0).replace('{value}', result.data.lst_id_not_seen))
                        }

                        result.data.lst_not_seen_detail.forEach(function (item) {

                            var base_date = (new Date(1970, 0, 1, 7, 0, 0)).getTime();
                            var date = new Date(base_date + item.seen_date * 1000);
                            if (item.seen_status == 0) {
                                $('#Notify').append(_menu_html.html_menu_notify_active.replace('{link}', item.link_redirect).replaceAll('{id}', item.notify_id).replace('{note}', item.content).replace('{date}', _global_function.GetDayText(date)))

                            } else {
                                $('#Notify').append(_menu_html.html_menu_notify.replace('{link}', item.link_redirect).replaceAll('{id}', item.notify_id).replace('{note}', item.content).replace('{date}', _global_function.GetDayText(date)))

                            }

                        });
                    }
                } 
                $("body").on('click', ".ava", function () {
                    _noti.UpdateNotify($(this))
                });
            }
        });
    },

    UpdateNotify: function (li_id) {

        var id = li_id.attr('data')
        $.ajax({
            url: "/menu/updateNotify",
            type: "POST",
            data: { id: id, seen_status: 2 },
            success: function (result) {
                if (result.status == 0) {
                    _noti.loadNotify();
                }
            }
        });
    },

    UpdateNotifyAll: function () {
        var id = $('#lst_id_not_seen').val()
        var list_id = id.split(',');
        var slice_id = list_id.slice(0, 20);
        id = slice_id.toString();
        if (id != "" && id != undefined) {
            $.ajax({
                url: "/menu/updateNotify",
                type: "POST",
                data: { id: id, seen_status: 1 },
                success: function (result) {
                    if (result.status == 0) {
                        _noti.loadNotify();
                    }
                }
            });
        }
    },
  
   
}