let input = $('#order_Id').val();
let type = 7;
$(document).ready(function () {
   
    _orderDetail.LoadClientDetai(input);
    _orderDetail.LoadPackages(input);
    _orderDetail.LoadContractPay(input);
    _orderDetail.LoadBillVAT(input);
    _orderDetail.LoadListPassenger(input);
    _orderDetail.LoadFile(input, type);
    _orderDetail.LoadSingleInformation(input);
    _orderDetail.LoadPersonInCharge(input);
    _orderDetail.LoadSystemInformation(input);
});
var _orderDetail = {
    LoadOeederDetail: function () {
        _orderDetail.LoadClientDetai(input);
        _orderDetail.LoadPackages(input);
        _orderDetail.LoadContractPay(input);
        _orderDetail.LoadBillVAT(input);
        _orderDetail.LoadListPassenger(input);
        _orderDetail.LoadFile(input, type);
        _orderDetail.LoadSingleInformation(input);
        _orderDetail.LoadPersonInCharge(input);
        _orderDetail.LoadSystemInformation(input);
    },
    LoadClientDetai: function (input) {
        $.ajax({
            url: "/Order/ClientDetail",
            type: "Post",
            data: { orderId: input },
            success: function (result) {
                $('#imgLoading_ClientDetai').hide();
                $('#grid_data_ClientDetai').html(result);
            }
        });
    },
    LoadPackages: function (input) {
        $.ajax({
            url: "/Order/Packages",
            type: "Post",
            data: { orderId: input},
            success: function (result) {
                $('#imgLoading_Packages').hide();
                $('#grid_data_Packages').html(result);
            }
        });
    },
    LoadContractPay: function (input) {
        $.ajax({
            url: "/Order/ContractPay",
            type: "Post",
            data: { orderId: input },
            success: function (result) {
                $('#imgLoading_ContractPay').hide();
                $('#grid_data_ContractPay').html(result);
            }
        });
    },
    LoadBillVAT: function (input) {
        $.ajax({
            url: "/Order/BillVAT",
            type: "Post",
            data: { orderId: input },
            success: function (result) {
                $('#imgLoading_BillVAT').hide();
                $('#grid_data_BillVAT').html(result);
            }
        });
    },
    LoadListPassenger: function (input) {
        $.ajax({
            url: "/Order/ListPassenger",
            type: "Post",
            data: { orderId: input },
            success: function (result) {
                $('#imgLoading_ListPassenger').hide();
                $('#grid_data_ListPassenger').html(result);
            }
        });
    },
    LoadFile: function (input, type) {
        _global_function.RenderFileAttachment($('#grid_data_File'), input,type)
        $('#imgLoading_File').hide();
        /*
        $.ajax({
            url: "/Order/File",
            type: "Post",
            data: { orderId: input, type: type },
            success: function (result) {
                $('#imgLoading_File').hide();
                $('#grid_data_File').html(result);
            }
        });
        */
    },
    LoadSingleInformation: function (input) {
        $.ajax({
            url: "/Order/SingleInformation",
            type: "Post",
            data: { orderId: input },
            success: function (result) {
                $('#imgLoading_SingleInformation').hide();
                $('#grid_data_SingleInformation').html(result);
            }
        });
    },
    LoadPersonInCharge: function (input) {
        $.ajax({
            url: "/Order/PersonInCharge",
            type: "Post",
            data: { orderId: input },
            success: function (result) {
                $('#imgLoading_PersonInCharge').hide();
                $('#grid_data_PersonInCharge').html(result);
            }
        });
    },
    LoadSystemInformation: function (input) {
        $.ajax({
            url: "/Order/SystemInformation",
            type: "Post",
            data: { orderId: input },
            success: function (result) {
                $('#imgLoading_SystemInformation').hide();
                $('#grid_data_SystemInformation').html(result);
            }
        });
    },
    UpdateOrderStatus: function (status, Orderid) {
        var ServiceCode = $('#ServiceCode').val();
        var title = 'Xác nhận thay đổi trạng thái';
       
        _msgconfirm.openDialog(title,'Xác nhận thay đổi trạng thái không?', function () {
            $.ajax({
                url: "/Order/UpdateOrderStatus",
                type: "Post",
                data: { status: status, OrderId: Orderid },
                success: function (result) {
                    if (result.sst_status === 0) {
                        _msgalert.success(result.smg);
                        setTimeout(function () {
                            window.location.reload();
                        }, 300);
                    }
                    else {
                        _msgalert.error(result.smg);

                    }
                }
            });
        });
    },
    ChangeOrderSaler: function (order_id, order_no) {
       
        var title = 'Nhận xử lý đơn hàng';
        var description = 'Bạn có chắc chắn muốn nhận xử lý đơn hàng này không?';
        _msgconfirm.openDialog(title, description, function () {
            $.ajax({
                url: "/Order/ChangeOrderSaler",
                type: "Post",
                data: { order_id: order_id, saleid: 0, OrderNo: order_no},
                success: function (result) {
                    if (result.status === 0) {
                        _msgalert.success(result.msg);
                        setTimeout(function () {
                            window.location.reload();
                        }, 1000);
                    }
                    else {
                        _msgalert.error(result.msg);

                    }
                }
            });
        });
    },
    UpdateOrder: function () {
       
        var model = {
            
            OrderId: $('#order_Id').val(),
            SalerId: $('#SalerId').val(),
            SalerGroupId: $('#SalerGroup').val() != null ? $('#SalerGroup').val().toString() : null,
            Label: $('#Label').val(),
            Note: $('#Note').val(),
            ProductService: $('#ProductService').val(),
        }
        $.ajax({
            url: "/Order/UpdateOrder",
            type: "Post",
            data: { model },
            success: function (result) {
                if (result.sst_status === 0) {
                    _msgalert.success(result.smg);
                    setTimeout(function () {
                        window.location.reload();
                    }, 1000);
                }
                else {
                    _msgalert.error(result.smg);

                }
            }
        });
    },
    PopupSendEmail: function () {
        let title = 'Gửi email đơn hàng';
        let url = '/Order/SendEmail';
        let param = {
            Orderid: $('#order_Id').val(),
            type: 0,
        };
        _magnific.OpenSmallPopup(title, url, param);
    },
    PopupSendEmail2: function (Orderid) {
        let title = 'Thông tin chuyến bay';
        let url = '/Order/PopupFly';
        let param = {
            Orderid: Orderid,
            type: 33,
        };
        _magnific.OpenSmallPopup(title, url, param);
    },
    SendEmailOrder: function () {

        var validate = true
        var subject_name = $('#Subject').val()
        var email = $('#Email').val()
        if (subject_name == null || subject_name == undefined || subject_name.trim() == '') {
            $('#Subject-error').html('Tiêu đề không được bỏ trống')
            $('#Subject-error').show()
            validate = false
        }


        if (email == null || email == undefined || email.trim() == '') {
            $('#Email-error').html('Người nhận không được bỏ trống')
            $('#Email-error').show()
            validate = false

        }
        if (!validate) {
            return
        }
        var id = $("#CC_Email").val()
        var _bodyTTNote = tinyMCE.get('TTNote').getContent();
       /* var _bodyTTChuyenKhoan = tinyMCE.get('TTChuyenKhoan').getContent();*/
        var _bodyTTChuyenKhoan = "";
        let OtherEmail = [];
        let TourEmail = [];
        let HotelEmail = [];
        let FyEmail = [];
        $(".Tour-row").each(function (index, item) {
            var element = $(item);
            var obj_package = {
                tourNote: element.find('.tourNote').val(),
                tourAmount: element.find('.tourAmount').val(),
                tourTotalBaby: element.find('.tourTotalBaby').val(),
                tourTotalChildren: element.find('.tourTotalChildren').val(),
                tourTotalAdult: element.find('.tourTotalAdult').val(),
                tourORGANIZINGName: element.find('.tourORGANIZINGName').val(),
                tourEndDate: element.find('.tourEndDate').val(),
                tourStartDate: element.find('.tourStartDate').val(),
                tourGroupEndPoint3: element.find('.tourGroupEndPoint3').val(),
                tourGroupEndPoint2: element.find('.tourGroupEndPoint2').val(),
                tourGroupEndPoint1: element.find('.tourGroupEndPoint1').val(),
                tourStartPoint3: element.find('.tourStartPoint3').val(),
                tourStartPoint2: element.find('.tourStartPoint2').val(),
                tourStartPoint1: element.find('.tourStartPoint1').val(),
                TourProductName: element.find('.TourProductName').val(),
            };
            TourEmail.push(obj_package);
        });
        $(".Other-row").each(function (index, item) {
            var element = $(item);
            var obj_package = {
                OtherStartDate: element.find('#OtherStartDate').val(),
                OtherEndDate: element.find('#OtherEndDate').val(),
                OtherAmount: element.find('#OtherAmount').val(),
                OtherNote: element.find('#OtherNote').val(),
                OtherServiceName: element.find('#OtherServiceName').val(),
            };
            OtherEmail.push(obj_package);
        });
        $(".Hotel-row").each(function (index, item) {
            var element = $(item);
            var obj_package = {
                hotelNote: element.find('#hotelNote').val(),
                hotelAmount: element.find('#hotelAmount').val(),
                hotelTotalDays: element.find('#hotelTotalDays').val(),
                hotelNumberOfAdult: element.find('#hotelNumberOfAdult').val(),
                hotelNumberOfChild: element.find('#hotelNumberOfChild').val(),
                hotelNumberOfInfant: element.find('#hotelNumberOfInfant').val(),
                hotelNumberOfRoom: element.find('#hotelNumberOfRoom').val(),
                hotelDepartureDate: element.find('#hotelDepartureDate').val(),
                hotelArrivalDate: element.find('#hotelArrivalDate').val(),
                hotelArrivalDate: element.find('#hotelArrivalDate').val(),
                HotelName: element.find('#HotelName').val(),
            };
            HotelEmail.push(obj_package);
        });
        $(".Fy-row").each(function (index, item) {
            var element = $(item);
            var obj_package = {
                fyAmount: element.find('#fyAmount').val(),
                fyAdultNumber: element.find('#fyAdultNumber').val(),
                fyChildNumber: element.find('#fyChildNumber').val(),
                fyInfantNumber: element.find('#fyInfantNumber').val(),
                fyBookingCode: element.find('#fyBookingCode').val(),
                fyAirlineName_Vi: element.find('#fyAirlineName_Vi').val(),
                fyBookingCode2: element.find('#fyBookingCode2').val(),
                fyAirlineName_Vi2: element.find('#fyAirlineName_Vi2').val(),
                fyEndDistrict2: element.find('#fyEndDistrict2').val(),
                fyEndDistrict: element.find('#fyEndDistrict').val(),
                fyStartDistrict2: element.find('#fyStartDistrict2').val(),
                fyStartDistrict: element.find('#fyStartDistrict').val(),
                fyEndDate: element.find('#fyEndDate').val(),
                fyStartDate: element.find('#fyStartDate').val(),
                fyNote: element.find('#fyNote').val(),
            };
            FyEmail.push(obj_package);
        });
        var model = {
            Subject: $('#Subject').val(),
            ServiceId: $('#ServiceId').val(),
            Orderid: $('#Orderid').val(),
            OrderNo: $('#orderNo').val(),
            To_Email: $('#To_Email').val() != undefined && $("#To_Email").val().length > 0 ? $("#To_Email").val().toString() : '',
            CC_Email: $("#CC_Email").val() != undefined && $("#CC_Email").val().length > 0 ? $("#CC_Email").val().toString() : '',
            BCC_Email: $("#BCC_Email").val() != undefined && $("#BCC_Email").val().length > 0 ? $("#BCC_Email").val().toString() : '',
            OrderNote: $('#order_note').val(),
            PaymentNotification: $('#payment_notification').val(),
            ServiceType: $('#Type').val(),
            Email: $('#Email').val(),
            group_booking_id: $('#Type').attr('data-group-booking'),

            OrderAmount: $('#OrderAmount').val().replaceAll(',', ''),
            TTChuyenKhoan: _bodyTTChuyenKhoan,
            TTNote: _bodyTTNote,
            NDChuyenKhoan: $('#NDChuyenKhoan').val(),
            saler_Email: $('#saler_Email').val(),
            saler_Name: $('#saler_Name').val(),
            saler_Phone: $('#saler_Phone').val(),
            user_Email: $('#user_Email').val(),
            user_Phone: $('#user_Phone').val(),
            user_Name: $('#user_Name').val(),
            TileEmail: $('#TileEmail').val(),


            fyAmount: $('#fyAmount').val(),
            fyAdultNumber: $('#fyAdultNumber').val(),
            fyChildNumber: $('#fyChildNumber').val(),
            fyInfantNumber: $('#fyInfantNumber').val(),
            fyBookingCode: $('#fyBookingCode').val(),
            fyAirlineName_Vi: $('#fyAirlineName_Vi').val(),
            fyBookingCode2: $('#fyBookingCode2').val(),
            fyAirlineName_Vi2: $('#fyAirlineName_Vi2').val(),
            fyEndDistrict2: $('#fyEndDistrict2').val(),
            fyEndDistrict: $('#fyEndDistrict').val(),
            fyStartDistrict2: $('#fyStartDistrict2').val(),
            fyStartDistrict: $('#fyStartDistrict').val(),
            fyEndDate: $('#fyEndDate').val(),
            fyStartDate: $('#fyStartDate').val(),
            fyNote: $('#fyNote').val(),

            hotelNote: $('#hotelNote').val(),
            hotelAmount: $('#hotelAmount').val(),
            hotelTotalDays: $('#hotelTotalDays').val(),
            hotelNumberOfAdult: $('#hotelNumberOfAdult').val(),
            hotelNumberOfChild: $('#hotelNumberOfChild').val(),
            hotelNumberOfInfant: $('#hotelNumberOfInfant').val(),
            hotelNumberOfRoom: $('#hotelNumberOfRoom').val(),
            hotelDepartureDate: $('#hotelDepartureDate').val(),
            hotelArrivalDate: $('#hotelArrivalDate').val(),

            tourNote: $('#tourNote').val(),
            tourAmount: $('#tourAmount').val(),
            tourTotalBaby: $('#tourTotalBaby').val(),
            tourTotalChildren: $('#tourTotalChildren').val(),
            tourTotalAdult: $('#tourTotalAdult').val(),
            tourORGANIZINGName: $('#tourORGANIZINGName').val(),
            tourEndDate: $('#tourEndDate').val(),
            tourStartDate: $('#tourStartDate').val(),
            tourGroupEndPoint3: $('#tourGroupEndPoint3').val(),
            tourGroupEndPoint2: $('#tourGroupEndPoint2').val(),
            tourGroupEndPoint1: $('#tourGroupEndPoint1').val(),
            tourStartPoint3: $('#tourStartPoint3').val(),
            tourStartPoint2: $('#tourStartPoint2').val(),
            tourStartPoint1: $('#tourStartPoint1').val(),

            OtherStartDate: $('#OtherStartDate').val(),
            OtherEndDate: $('#OtherEndDate').val(),
            OtherAmount: $('#OtherAmount').val(),
            OtherNote: $('#OtherNote').val(),

            OtherEmail: OtherEmail,
            TourEmail: TourEmail,
            HotelEmail: HotelEmail,
            FyEmail: FyEmail,
            Packagesemail: []
        }
        var list_attach_file = _global_function.GetAttachmentFiles($('.attachment-file-block'))
        $('.Order-Packages-row').each(function (index, item) {
            var extra_package_element = $(item);
            var Packages_Type = extra_package_element.find('.Packages_Type').val();
            var date = extra_package_element.find('.Orderdate').val();
            var Packagesdetail = extra_package_element.find('.OrderPackagesdetail').val();
            var order_package = {

                Packages_Type: Packages_Type,
                date: date,
                Packagesdetail: Packagesdetail,

            }
            model.Packagesemail.push(order_package);
        });

        _global_function.AddLoading()
        $.ajax({
            url: "/Order/ConfirmSendEmail",
            type: "Post",
            data: { model: model, attach_file: list_attach_file },
            success: function (result) {
                _global_function.RemoveLoading()
                if (result.status === 0) {
                    _msgalert.success(result.msg);
                    $.magnificPopup.close();
                }
                else {
                    _global_function.RemoveLoading()
                    _msgalert.error(result.msg);

                }
            }
        });

    },
    
    UpdateOrderFinishPayment: function (Orderid,type) {
        var ServiceCode = $('#ServiceCode').val();
        var title = 'Xác nhận đơn hàng được công nợ';

        _msgconfirm.openDialog(title, 'Xác nhận đơn hàng được công nợ?', function () {
            _global_function.AddLoading()
            $.ajax({
                url: "/Order/UpdateOrderFinishPayment",
                type: "Post",
                data: { OrderId: Orderid ,type:type},
                success: function (result) {
                    if (result.sst_status === 0) {
                        _global_function.RemoveLoading()
                        _msgalert.success(result.smg);
                        setTimeout(function () {
                            window.location.reload();
                        }, 300);
                    }
                    else {
                        _global_function.RemoveLoading()
                        _msgalert.error(result.smg);

                    }
                }
            });
        });
    },
    RePushtoOperator: function (order_id) {
        var title = 'Xác nhận chuyển lên điều hành';
        var description = 'Các dịch vụ có trạng thái [Từ chối] trong đơn hàng sẽ được chuyển lại lên điều hành, bạn có chắc chắn không?';
        _msgconfirm.openDialog(title, description, function () {
            _global_function.AddLoading()
            $.ajax({
                url: "/Order/RePushOrderServiceToOperator",
                type: "Post",
                data: { order_id: order_id },
                success: function (result) {
                    if (result.status == 0) {
                        _global_function.RemoveLoading()
                        _msgalert.success(result.msg);
                        setTimeout(function () {
                            window.location.reload();
                        }, 300);
                    }
                    else {
                        _global_function.RemoveLoading()
                        _msgalert.error(result.smg);

                    }
                }
            });
        });
    },
    Edit: function (id) {
        var order_Id = $('#order_Id').val()
        let title = 'Thêm mới/Cập nhật thành viên chính';
        let url = '/Order/ContactClientdetails';
        let param = {
            orderId: order_Id,
        };
        if (id.trim() != "") {
            param = {
                id: id,
            };
        }
        _magnific.OpenSmallPopup(title, url, param);
    },
    SetupContactClient: function () {
        let FromCreate = $('#ContactClient_Detail');
        FromCreate.validate({
            rules: {

                "Name": "required",
                "Mobile": {
                    required: true,
                    number: true,
                },
                "Email": {
                    required: true,
                    email: true,
                },


            },
            messages: {
                "Name": "Tên khách hàng không được bỏ trống",

                "Mobile": {
                    required: "Số điện thoại không được bỏ trống",
                    number: "Nhập đúng định dạng số",
                },
                "Email": {
                    required: "Email không được bỏ trống",
                    email: "Nhạp đúng định dạnh email",
                },
            }
        });
        if (FromCreate.valid()) {

            var model = {
                id: $('#Id').val(),
                client_id: $('#ClientId').val(),
                order_id: $('#OrderId').val(),
                name: $('#Name').val(),
                phone: $('#Mobile').val(),
                email: $('#Email').val(),
            }
            $.ajax({
                url: '/Order/UpdateContactClient',
                type: "post",
                data: { model },
                success: function (result) {
                    if (result.status == 0) {
                        _msgalert.success(result.msg);
                        $.magnificPopup.close();
                        location.reload();
                    }
                    else {
                        _msgalert.error(result.msg);
                    }
                }
            });
        }

    },
    AddInvoiceRequest: function () {
        var order_Id = $('#order_Id').val()
        let title = 'Thêm yêu cầu xuất hóa đơn';
        let url = '/InvoiceRequest/Add';
        let param = {
            orderId: order_Id,
        };
        _magnific.OpenSmallPopup(title, url, param);
    },
    PopupCCTT: function (OrderNo) {
        let title = 'Cập nhật trạng thái đơn hàng ' + OrderNo;
        let url = '/Order/PopupOrderStatus';
        let param = {
            OrderId: $('#order_Id').val(),
        };
        _magnific.OpenSmallPopup(title, url, param);
    },
    UpdateOrderandService: function (OrderNo,id) {
        let title = 'Xác nhận Cập nhật trạng thái đơn hàng '+OrderNo;
        let description = 'Bạn xác nhận Cập nhật trạng thái đơn hàng ' + OrderNo+"?";
      
        
        var model = {
            OrderId: $('#OrderId').val(),
            OrderStatus: $('#OrderStatus').val(),
            PaymentStatus: $('#PaymentStatus').val(),
            ListFly:[],
            ListVin:[],
            ListTour:[],
            ListHotel:[],
            ListOther:[],
        }
       
        $(".SetService-fly-row").each(function (index, item) {
            var element = $(item);
            var obj_package = {
                Status: element.find('.flyStatus').val(),
                GroupBookingId: element.find('.flyServiceid').val(),
            };
            model.ListFly.push(obj_package);
        });
        $(".SetService-Vin-row").each(function (index, item) {
            var element = $(item);
            var obj_package = {
                Status: element.find('.VinStatus').val(),
                Id: element.find('.VinServiceid').val(),
            };
            model.ListVin.push(obj_package);
        });
        $(".SetService-Tour-row").each(function (index, item) {
            var element = $(item);
            var obj_package = {
                Status: element.find('.TourStatus').val(),
                Id: element.find('.TourServiceid').val(),
            };
            model.ListTour.push(obj_package);
        });
        $(".SetService-Hotel-row").each(function (index, item) {
            var element = $(item);
            var obj_package = {
                Status: element.find('.HotelStatus').val(),
                Id: element.find('.HotelServiceid').val(),
            };
            model.ListHotel.push(obj_package);
        });
        $(".SetService-Other-row").each(function (index, item) {
            var element = $(item);
            var obj_package = {
                Status: element.find('.OtherStatus').val(),
                Id: element.find('.OtherServiceid').val(),
            };
            model.ListOther.push(obj_package);
        });
        _msgconfirm.openDialog(title, description, function () {
            $.ajax({
                url: "/Order/UpdateOrderandServiceStatus",
                type: "post",
                data:  model ,
                success: function (result) {
                    if (result.status == 0) {
                        _msgalert.success(result.smg);
                        $.magnificPopup.close();
                        setTimeout(function () {
                            window.location.reload();
                        }, 300);
                    } else {
                        _msgalert.error(result.smg);
                    }
                }
            });

        });
    },
    GetLog(id, orderNo) {

        let title = 'Lịch sử đơn hàng ' + orderNo;
        let url = '/Order/getLog';
        let param = {
            orderid: id,
        };
        _magnific.OpenSmallPopup(title, url, param);
    },
    GetLogOrderIsLock(id, orderNo) {

        let title = 'Lịch sử khóa sổ ';
        let url = '/Order/GetLogOrderIsLock';
        let param = {
            orderid: id,
        };
        _magnific.OpenSmallPopup(title, url, param);
    },
    IsLockOrder: function (orderid, OrderNo, IsLock) {
        var title = 'Xác nhận mở sổ';
        var description = 'Bạn xác nhận mở sổ đơn hàng ' + OrderNo + ' này?';
        if (IsLock == 1) {
            description = 'Bạn xác nhận Khóa sổ đơn hàng ' + OrderNo + ' này?';
            title = 'Xác nhận khóa sổ';
        }
        _msgconfirm.openDialog(title, description, function () {

            $.ajax({
                url: "/Order/UpdateOrderIsLock",
                type: "Post",
                data: { OrderId: orderid, IsLock: IsLock },
                success: function (result) {
                    if (result.status > 0) {
                        _msgalert.error(result.smg);
                    }
                    else {
                        _msgalert.success(result.smg);
                        setTimeout(function () {
                            window.location.reload();
                        }, 300);
                    }
                }
            });
        }
           
        );
    },
}
var _OrderDetail_Sendemail = {
    loadformSenmail: function () {

        _OrderDetail_Sendemail.Select2WithUserSuggesstionEmail1($("#To_Email"))
        _OrderDetail_Sendemail.Select2WithUserSuggesstionEmail($("#CC_Email"))
        _OrderDetail_Sendemail.Select2WithUserSuggesstionEmail($("#BCC_Email"))
        tinymce.remove('#TTChuyenKhoan');
        tinymce.remove('#TTNote');

        _common.tinyMce('#TTChuyenKhoan');
        _common.tinyMce('#TTNote');
        $("body").on('keyup', "input", function () {
            var element = $(this)
            var id = element.attr('id')
            $('#' + id + '-error').hide()
        });
    },
    Select2WithUserSuggesstion: function (element) {
        element.select2({
            theme: 'bootstrap4',
            placeholder: "Người nhận",
            ajax: {
                url: "/Order/UserSuggestion",
                type: "post",
                dataType: 'json',
                delay: 250,
                data: function (params) {
                    var query = {
                        txt_search: params.term,
                    }

                    // Query parameters will be ?search=[term]&type=public
                    return query;
                },
                processResults: function (response) {
                    return {
                        results: $.map(response.data, function (item) {
                            return {
                                text: item.fullname + ' - ' + item.email,
                                id: item.id,
                            }
                        })
                    };
                },
                cache: true
            }
        });
    },
    Select2WithUserSuggesstionEmail: function (element) {
        element.select2({
            theme: 'bootstrap4',
            placeholder: "Chọn Email",
            tags: true,
            ajax: {
                url: "/Order/UserSuggestion",
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
                                text: item.fullname + ' - ' + item.email,
                                id: item.email,
                            }
                        })
                    };
                },
                createTag: function (params) {
                    let term = $.trim(params.term);
                    return {
                        id: term,
                        text: term,
                        newTag: true,
                    }
                },
                cache: true
            }
        });
    },
    Select2WithUserSuggesstionEmail1: function (element) {
        element.select2({
            theme: 'bootstrap4',
            placeholder: "Chọn Email",
          
            ajax: {
                url: "/Order/UserSuggestion",
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
                                text: item.fullname + ' - ' + item.email,
                                id: item.email,
                            }
                        })
                    };
                },
                cache: true
            }
        });
    }, 


}