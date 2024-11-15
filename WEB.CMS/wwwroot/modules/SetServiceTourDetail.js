var _manual_tour_html = {
  
    html_service_tour_packages_add_packages_tr: '<tr class="service-tour-extrapackage-row" data-extra-package-id="@item.Id"><td style="width: 80px; text-align: center;" class="service-tour-stt">@(++index)<input name="service-tour-package-stt" style="display:none" class="form-control service-tour-package-stt"  value="@(++index)"> </td><td></td><td style="max-width:290px"><select  class="form-control service-tour-extrapackage-SupplierId-select"  name="service-tour-extrapackage-SupplierId-select" style="width:100% !important"></select></td>  <td> <select class="form-control service-tour-extrapackage-Packageid-select" style="width:100% !important"> </select> </td><td><select class="form-control supplier-select  service-tour-extrapackage-status-select" name="service-tour-extrapackage-status-select"><option value="0">Sử dụng </option><option value="1">Hủy </option></select></td >  <td class= "text-right" ><input class= "form-control text-right currency service-tour-extrapackage-Price"value=""></td><td> <input   class= "form-control style-width text-right currency service-tour-extrapackage-Quantity" value=""></td> <td class="text-right" ><input class= " form-control style-width text-right currency service-tour-extrapackage-Times"value=""></td > <td class="text-right"> <input disabled style="background: #f0f0f0;" class="form-control text-right currency service-tour-extrapackage-amount"  value=""></td> <td ><textarea class="service-tour-extrapackage-Note" style="min-height:70px"></textarea></td><td style="text-align: center;"> <a class="fa fa-trash-o" href="javascript:;" onclick="_SetService_Tour_Detail.DeleteTourPackage($(this));"></a> </td></tr> ',
    html_TourPackageOptional_option: '<option class="select2-results__option" value="{CodeValue}">{Description}</option>',
}
let Touramount = 0;
var _SetService_Tour_Detail = {
    ServiceType: 17,
    ServiceTypeSale: 5,
    loaddata: function () {
        var id = $('#HotelBookingID').val();
        var Orderid = $('#Orderid').val();
        var HotelBookingstatus = $('#service-fly-detail-data').attr('data-status');
        _SetService_Tour_Detail.getListTourPackages(id);
        _SetService_Tour_Detail.ListTourPackagesOrdered(id, Orderid, HotelBookingstatus);
        _SetService_Tour_Detail.ShowTourCodeTab();
        _SetService_Tour_Detail.TourServiceDetail();
        _SetService_Tour_Detail.ShowTourPaymentTab();
        _SetService_Tour_Detail.TourServicesOrder();
        _SetService_Tour_Detail.TourServiceRefund();
        _SetService_Tour_Detail.MarketingPaymentRequest();


        switch ($('#service-fly-detail-data').attr('data-status')) {
            case '1':
            case '2': {
                if (!$('.service-fly-detail-button-ordered').hasClass('active')) {
                    _SetService_Tour_Detail.OnStatuse(2); _SetService_Tour_Detail.SetActive(2)
                }
            } break;
            case '3':
            case '4': {
                if (!$('.service-fly-detail-button-payment').hasClass('active')) {
                    _SetService_Tour_Detail.OnStatuse(4); _SetService_Tour_Detail.SetActive(4)
                }
            } break;
            default: {
                if (!$('.service-fly-detail-button-order').hasClass('active')) {
                    _SetService_Tour_Detail.OnStatuse(1); _SetService_Tour_Detail.SetActive(1)
                }
            } break;
        }
    },
    DynamicBindInput: function () {
        $('body').on('keyup', '.service-tour-extrapackage-Price, .service-tour-extrapackage-Times, .service-tour-extrapackage-Quantity', function () {
            var row_element = $(this).closest('.service-tour-extrapackage-row')
            _SetService_Tour_Detail.OnChangeBasePrice(row_element)
        });
        $('body').on('change', '.service-tour-extrapackage-status-select', function () {
            var row_element = $(this).closest('.service-tour-extrapackage-row')
            _SetService_Tour_Detail.OnChangeBasePrice(row_element)
        });
    },

    TourPackagesOrderedDynamicBindEvent: function () {
        $("body").on('keyup', ".service-tour-extrapackage-Price, .service-tour-extrapackage-Times, .service-tour-extrapackage-Quantity", function () {
            _SetService_Tour_Detail.OnChangeBasePrice($(this))
        });
    },

    OnChangeBasePrice: function (row_element) {
        var element_price = row_element.find('.service-tour-extrapackage-Price')
        var element_quanity = row_element.find('.service-tour-extrapackage-Quantity')
        var element_times = row_element.find('.service-tour-extrapackage-Times')



        var amount = !isNaN(parseFloat(element_price.val().replaceAll(',', ''))) && parseFloat(element_price.val().replaceAll(',', '')) > 0 ? parseFloat(element_price.val().replaceAll(',', '')) : 0
        var quantity = !isNaN(parseFloat(element_quanity.val().replaceAll(',', ''))) && parseFloat(element_quanity.val().replaceAll(',', '')) > 0 ? parseFloat(element_quanity.val().replaceAll(',', '')) : 0
        var times = !isNaN(parseFloat(element_times.val().replaceAll(',', ''))) && parseFloat(element_times.val().replaceAll(',', '')) > 0 ? parseFloat(element_times.val().replaceAll(',', '')) : 0

        var base_price = (amount * times * quantity)
        row_element.find('.service-tour-extrapackage-amount').val(_global_function.Comma(base_price.toFixed(0))).change();

        var sumamount = parseFloat($('.service-tour-total-extrapackage-amount-after-vat').attr('data-extra-package-amount'));
        $('.service-tour-extrapackage-row').each(function (index, item) {
            var extra_package_element = $(item);
            var package_amount = extra_package_element.find('.service-tour-extrapackage-amount').val().replaceAll(',', '');
            var status = extra_package_element.find('.service-tour-extrapackage-status-select').val();
            if (status != 1)
            sumamount += parseFloat(package_amount);

        });
        $('.service-tour-total-extrapackage-amount-after-vat').html(_common_function_set_tour.Comma(sumamount))
    },
    ChangeToPayment: function (valuse) {
        var ServiceCode = $('#ServiceCode').val();
        var title = 'Nhận đặt dịch vụ';
        var title2 = 'Nhận đặt dịch vụ';
        var title3 = 'Quyết toán';
        var title4 = 'Trả code dịch vụ tour';
        var title5 = 'Từ chối';
        _msgconfirm.openDialog(title, title2 + ServiceCode + ' không?', function () {
            $.ajax({
                url: "/SetService/ChangeToConfirmPaymentStatus",
                type: "post",
                data: { group_booking_id: group_booking_id },
                success: function (result) {
                    if (result != undefined && result.status == 0) {
                        _msgalert.success(result.msg);
                    }
                    else {
                        _msgalert.error(result.msg);
                    }

                }
            });
        });
    },
    getListTourPackages: function (input) {
        $.ajax({
            url: "/SetService/ListTourPackages",
            type: "Post",
            data: { id: input },
            success: function (result) {
                $('#imgLoading_ListTourPackagesOrder').hide();
                $('#grid_data_ListTourPackagesOrder').html(result);
                _global_function.RenderFileAttachment($('.attachment-sale'), input, _SetService_Tour_Detail.ServiceTypeSale, false, false)

            }
        });
    },
    ListTourPackagesOrdered: function (input, order, HotelBookingstatus) {
        var id = $('#HotelBookingID').val();
        $.ajax({
            url: "/SetService/ListTourPackagesOrdered",
            type: "Post",
            data: { id: input, order: order, HotelBookingstatus: HotelBookingstatus },
            success: function (result) {
                $('#imgLoading_ListTourPackagesOrdered').hide();
                $('#grid_data_ListTourPackagesOrdered').html(result);
                _SetService_Tour_Detail.DynamicBindInput()
                _global_function.RenderFileAttachment($('.attachment-operator'), input, _SetService_Tour_Detail.ServiceType, true, false, true)

            }
        });
    },
    UpdateTourStatus: function (tourId, status, OrderId) {
        var ServiceCode = $('#ServiceCode').val();
        var amount = $('#TourAmount').val();
        var user = $('#user').val();
        var Saler = $('#Saler-Name').attr('data-sale');
        var title = 'Nhận đặt dịch vụ';
        var title2 = 'Nhận đặt dịch vụ ';
        if (status == 4) {
            title = 'Quyết toán';
            title2 = 'Quyết toán dịch vụ tour ';
        }
        if (status == 3) {
            title = 'Trả code dịch vụ tour';
            title2 = 'Trả code dịch vụ tour ';
        }
        if (status == 5) {
            title = 'Từ chối';
            title2 = 'Từ chối dịch vụ tour ';
        }
        _msgconfirm.openDialog(title, title2 + ServiceCode + ' không?', function () {
            $.ajax({
                url: "/SetService/UpdateTourStatus",
                type: "Post",
                data: { tourId: tourId, tour_status: status, OrderId: OrderId, amount: amount },
                success: function (result) {
                    if (result.status === 0) {
                        if (Saler == "") {
                            $.ajax({
                                url: "/SetService/UpdateHotelBooking",
                                type: "Post",
                                data: { id: tourId, OrderId: OrderId, salerId: user, type: 5 },
                                success: function (result) {
                                }
                            });
                        }
                        _msgalert.success(result.msg);
                        setTimeout(function () { window.location.reload() }, 1000)
                    }
                    else {
                        _msgalert.error(result.msg);

                    }
                }
            });
        });

    },
    SetActive: function (status) {
        Isstatus = status;
        $('#data_order').removeClass('active')
        $('#data_ListTourPackagesOrder').removeClass('active')
        $('#data_ListTourPackagesOrdered').removeClass('active')
        $('#data_HotelBookingCode').removeClass('active')
        $('#data_payment_account').removeClass('active')
        $('#data_HotelRefund').removeClass('active')
        $('#data_HotelRefund_Ncc').removeClass('active')

        if (status == 1)
            $('#data_order').addClass('active')
        if (status == 2)
            $('#data_ListTourPackagesOrder').addClass('active');
        if (status == 3)
            $('#data_ListTourPackagesOrdered').addClass('active')
        if (status == 4)
            $('#data_payment_account').addClass('active')
        if (status == 5)
            $('#data_HotelBookingCode').addClass('active')
        if (status == 6)
            $('#data_HotelRefund').addClass('active')
        if (status == 7)
            $('#data_Marketing').addClass('active')
    },
    OnStatuse: function (value) {
        if (value == 1) {
            $('#grid_data_TourServicesOrder').show();
            $('#grid_data_ListTourPackagesOrder').hide();
            $('#grid_data_ListTourPackagesOrdered').hide();
            $('#grid_data_TourCode').hide();
            $('#grid_data_Tourpayment').hide();
            $('#grid_data_TourRefund').hide();
            $('#grid_data_Marketing').hide();

        }
        if (value == 2) {
            $('#grid_data_TourServicesOrder').hide();
            $('#grid_data_ListTourPackagesOrder').show();
            $('#grid_data_ListTourPackagesOrdered').hide();
            $('#grid_data_TourCode').hide();
            $('#grid_data_Tourpayment').hide();
            $('#grid_data_TourRefund').hide();
            $('#grid_data_Marketing').hide();

        }
        if (value == 3) {
            $('#grid_data_TourServicesOrder').hide();
            $('#grid_data_ListTourPackagesOrder').hide();
            $('#grid_data_ListTourPackagesOrdered').show();
            $('#grid_data_TourCode').hide();
            $('#grid_data_Tourpayment').hide();
            $('#grid_data_TourRefund').hide();
            $('#grid_data_Marketing').hide();

        }
        if (value == 4) {
            $('#grid_data_TourServicesOrder').hide();
            $('#grid_data_ListTourPackagesOrder').hide();
            $('#grid_data_ListTourPackagesOrdered').hide();
            $('#grid_data_TourCode').hide();
            $('#grid_data_Tourpayment').show();
            $('#grid_data_TourRefund').hide();
            $('#grid_data_Marketing').hide();

        }
        if (value == 5) {
            $('#grid_data_TourServicesOrder').hide();
            $('#grid_data_ListTourPackagesOrder').hide();
            $('#grid_data_ListTourPackagesOrdered').hide();
            $('#grid_data_Tourpayment').hide();
            $('#grid_data_TourCode').show();
            $('#grid_data_TourRefund').hide();
            $('#grid_data_Marketing').hide();

        }
        if (value == 6) {
            $('#grid_data_TourServicesOrder').hide();
            $('#grid_data_ListTourPackagesOrder').hide();
            $('#grid_data_ListTourPackagesOrdered').hide();
            $('#grid_data_Tourpayment').hide();
            $('#grid_data_TourCode').hide();
            $('#grid_data_TourRefund').show();
            $('#grid_data_Marketing').hide();

        }
        if (value == 7) {
            $('#grid_data_TourServicesOrder').hide();
            $('#grid_data_ListTourPackagesOrder').hide();
            $('#grid_data_ListTourPackagesOrdered').hide();
            $('#grid_data_Tourpayment').hide();
            $('#grid_data_TourCode').hide();
            $('#grid_data_TourRefund').hide();
            $('#grid_data_Marketing').show();

        }
    },
    TTStatus: function () {
        $('#CCthanhtien').hide();
        $('#Boqua').show();
        $('#luu').show();

        $('.service-tour-extrapackage-SupplierId-select').removeAttr("disabled");
        $('.service-tour-extrapackage-SupplierId-select').removeAttr("style");

        $('.service-tour-extrapackage-Packageid-select').removeAttr("disabled");
        $('.service-tour-extrapackage-Packageid-select').removeAttr("style");

        $('.service-tour-extrapackage-status-select').removeAttr("disabled");
        $('.service-tour-extrapackage-status-select').removeAttr("style");
        //$('.service-tour-extrapackage-amount').removeAttr("disabled");
        //$('.service-tour-extrapackage-amount').removeAttr("style");

        $('.service-tour-extrapackage-Quantity').removeAttr("disabled");
        $('.service-tour-extrapackage-Quantity').removeAttr("style");
        $('.service-tour-extrapackage-Times').removeAttr("disabled");
        $('.service-tour-extrapackage-Times').removeAttr("style");
        $('.service-tour-extrapackage-Price').removeAttr("disabled");
        $('.service-tour-extrapackage-Price').removeAttr("style");
        $('.service-tour-extrapackage-Note').removeAttr("disabled");
        $('.service-tour-extrapackage-Note').removeAttr("style");
        $('.service-tour-delete').show();
        $('.service-tour-buttun').show();
    },
    TTStatusTourGuests: function () {
        $('#CCTourGuests').hide();
        $('#BoquaTourGuests').show();
        $('#luuTourGuests').show();

        $('.TourGuests-RoomNumber').removeAttr("disabled");
        $('.TourGuests-RoomNumber').removeAttr("style");

        $('.TourGuests-CCCD').removeAttr("disabled");
        $('.TourGuests-CCCD').removeAttr("style");


    },
    BQStatusTourGuests: function () {
        $('#CCTourGuests').show();
        $('#BoquaTourGuests').hide();
        $('#luuTourGuests').hide();

        _SetService_Tour_Detail.loaddata();
        _SetService_Tour_Detail.SetActive(3);
        _SetService_Tour_Detail.OnStatuse(3);

    },
    BQStatus: function () {
        $('#CCthanhtien').show();
        $('#Boqua').hide();
        $('#luu').hide();

        _SetService_Tour_Detail.loaddata();
        _SetService_Tour_Detail.SetActive(3);
        _SetService_Tour_Detail.OnStatuse(3);
        $('.service-tour-extrapackage-SupplierId-select').addClass("disabled");
        $('.service-tour-extrapackage-SupplierId-select').attr("style", "width: 100 % !important");
    },
    onEdit: function (value) {
        $('.SetService-Tour-row').eq(value).addClass('SetService-Tour-row-Edit');
    },
    UpdateHotelBookingRooms: function () {
        var object_summit = {
            extra_SetService: [],
        }
        $(".SetService-Tour-row-Edit").each(function (index, item) {
            var element = $(item);
            var obj_package = {
                Id: element.find('.SetService-Tour-Id').val(),
                UnitPrice: element.find('.SetService-UnitPrice').val().replaceAll(',', ''),
                RatePlanCode: element.find('.servicemanual-HotelBookingRoom-RatePlanCode').val(),

            };
            object_summit.extra_SetService.push(obj_package);
        });
        $.ajax({
            url: '/SetService/UpdateHotelBookingRoomsUnitPrice',
            type: "post",
            data: { data: object_summit.extra_SetService },
            success: function (result) {
                if (result.status === 0) {
                    _msgalert.success(result.msg);
                    _SetService_Detail.loadDataDetail();
                    _SetService_Detail.SetActive(Isstatus);
                }
                else {
                    _msgalert.error(result.msg);

                }

            }
        });
    },
    UpdateTourPice: function () {
        var object_summit = {
            extra_SetService: [],

        }
        let FromCreate = $('#Edit-Tour');
        FromCreate.validate({
            rules: {
                "SetService-Tour-UnitPrice": {
                    required: true,
                },

            },
            messages: {
                "SetService-Tour-UnitPrice": {
                    required: "Đơn giá không được bỏ trống",
                },

            }
        });
        if (FromCreate.valid()) {

            if ($(".SetService-Tour-row-Edit")[0]) {
                var error = false;
                $('.SetService-Tour-row-Edit').each(function (index, item) {
                    var element = $(item)
                    if (element.find('.SetService-Tour-UnitPrice').val() == undefined || element.find('.SetService-Tour-UnitPrice').val() == '') {
                        _msgalert.error('Vui lòng nhập đơn giá');
                        error = true;
                        return false;
                    }
                });
                if (error) return;
            }
            $(".SetService-Tour-row-Edit").each(function (index, item) {
                var element = $(item);
                var obj_package = {
                    Id: element.find('.SetService-Tour-Id').val(),
                    UnitPrice: element.find('.SetService-Tour-UnitPrice').val().replaceAll(',', ''),
                    TourId: element.find('.SetService-Tour-TourId').val(),
                };
                object_summit.extra_SetService.push(obj_package);
            });
            var tour_id = $('#HotelBookingID').val()
            if (object_summit.extra_SetService.length > 0) {
                $.ajax({
                    url: '/SetService/UpdateTourPackagesUnitPrice',
                    type: "post",
                    data: {
                        data: object_summit.extra_SetService,
                        tour_id: tour_id
                    },
                    success: function (result) {
                        if (result.status === 0) {
                            _msgalert.success(result.msg);
                            setTimeout(function () {
                                _SetService_Tour_Detail.loaddata();
                                _SetService_Tour_Detail.SetActive(3);
                                _SetService_Tour_Detail.OnStatuse(3);

                            }, 1000)

                        }
                        else {
                            _msgalert.error(result.msg);
                            setTimeout(function () {
                                _SetService_Tour_Detail.loaddata();
                                _SetService_Tour_Detail.SetActive(3);
                                _SetService_Tour_Detail.OnStatuse(3);

                            }, 1000)
                        }

                    }
                });
            }
            else {
                _msgalert.error("Không có sự thay đổi về giá của dịch vụ");
                _SetService_Tour_Detail.loaddata();
                _SetService_Tour_Detail.SetActive(3);
                _SetService_Tour_Detail.OnStatuse(3);
            }


        }

    },
    ShowTourCodeTab: function () {

        let input = {
            HotelBookingstatus: $('#service-fly-detail-data').attr('data-status'),
            HotelBookingID: $('#HotelBookingID').val(),
            Type: 5
        };
        $.ajax({
            url: "/SetService/ListHotelBookingCode",
            type: "Post",
            data: { HotelBookingstatus: input.HotelBookingstatus, HotelBookingID: input.HotelBookingID, type: input.Type },
            success: function (result) {
                $('#imgLoading_TourCode').hide();
                $('#grid_data_TourCode').html(result);
                $("body").on('click', ".service-code-send-email", function (ev, picker) {
                    _SetService_Tour_Detail.PopupSendEmailTour()

                });

            }
        });
    },

    ShowTourPaymentTab: function () {
        let input = {
            id: $('#Orderid').val(),
            HotelBookingID: $('#HotelBookingID').val(),
            ContactClientId: $('#ContactClientId').val(),
            serviceType: 5,
            HotelBookingstatus: $('#service-fly-detail-data').attr('data-status'),
            type: 5,
            supplierId: $('#suplier-detail').attr('data-suplier-id'),
            supplierName: $('#suplier-detail').attr('data-suplier-name'),
            orderId: $('#Orderid').val(),
            serviceCode: $('#ServiceCode').val(),
          /*  amount: $('#operator-order-amount').attr('data-amount'),*/
            amount: Touramount,
        };
        $.ajax({
            url: "/SetService/ListHotelBookingpayment",
            type: "Post",
            data: input,
            success: function (result) {
                $('#imgLoading_Tourpayment').hide();
                $('#grid_data_Tourpayment').html(result);
            }
        });
    },
    TourServicesOrder: function () {
        let input = {
            id: $('#Orderid').val(),
            HotelBookingID: $('#HotelBookingID').val(),
            ContactClientId: $('#ContactClientId').val(),
            serviceType: 5,
            HotelBookingstatus: $('#service-fly-detail-data').attr('data-status'),
            type: 5,
            supplierId: $('#suplier-detail').attr('data-suplier-id'),
            supplierName: $('#suplier-detail').attr('data-suplier-name'),
            orderId: $('#Orderid').val(),
            amount: $('#operator-order-amount').attr('data-amount'),
        };
        $.ajax({
            url: "/SetService/ListOrder",
            type: "Post",
            data: input,
            success: function (result) {
                $('#imgLoading_TourServicesOrder').hide();
                $('#grid_data_TourServicesOrder').html(result);
            }
        });
    },
    TourServiceDetail: function () {

        let input = {
            HotelBookingstatus: $('#service-fly-detail-data').attr('data-status'),
            HotelBookingID: $('#HotelBookingID').val(),
            Type: 5
        };
        $.ajax({
            url: "/SetService/TourServiceDetail",
            type: "Post",
            data: { id: input.HotelBookingID },
            success: function (result) {
                $('#imgLoading_TourServiceDetail').hide();
                $('#grid_data_TourServiceDetail').html(result);
                Touramount = $('#operator-order-amount').attr('data-amount');
                _SetService_Tour_Detail.ShowTourPaymentTab();
            }
        });
    },
    TourServiceRefund: function (type) {

        let input = {
            orderId: $('#Orderid').val(),
            amount: $('#TourTotalAmount').val(),
            clientId: $('#client_id').val(),
            bookingId: $('#HotelBookingID').val(),
            bookingstatus: $('#service-fly-detail-data').attr('data-status'),
            serviceType: 5
        };
        $.ajax({
            url: "/SetService/ListPaymentRequestByClient",
            type: "Post",
            data: input,
            success: function (result) {
                $('#grid_data_TourRefund').hide();
                $('#grid_data_TourRefund').html(result);
                if (type == 1) {
                    $('#grid_data_TourRefund').show();
                }
            }
        });
    },
    MarketingPaymentRequest: function (type) {

        let input = {
            orderId: $('#Orderid').val(),
            amount: $('#TourTotalAmount').val(),
            clientId: $('#client_id').val(),
            bookingId: $('#HotelBookingID').val(),
            bookingstatus: $('#service-fly-detail-data').attr('data-status'),
            serviceType: 5,
            serviceId: $('#HotelBookingID').val(),
        };
        $.ajax({
            url: "/SetService/MarketingPaymentRequest",
            type: "Post",
            data: input,
            success: function (result) {
                $('#grid_data_Marketing').hide();
                $('#grid_data_Marketing').html(result);
                if (type == 1) {
                    $('#grid_data_Marketing').show();
                }
            }
        });
    },

    PopupSendEmailTour: function () {
        let title = 'Gửi Code dịch vụ Tour';
        let url = '/SetService/SendEmail';
        let param = {
            id: $('.service-code-send-email').attr('data-booking-id').trim(),
            Orderid: $('#service-fly-detail-data').attr('data-order-id').trim(),
            type: 5,
        };
        _magnific.OpenSmallPopup(title, url, param);
    },
    OpenPopupUserAgent: function (id) {

        let title = 'Đổi điều hành viên';
        let url = '/SetService/DetailUserHotel';
        let param = {
            id: id,
            type: 5
        };
        _magnific.OpenSmallPopup(title, url, param);
    },
    AddTourServicePackagesPopup: function () {
        var tour_id = $('#HotelBookingID').val();
        $.ajax({
            url: "/Order/AddTourServicePackages",
            type: "post",
            data: { tour_id: tour_id },
            success: function (result) {
                $('#service-tour-packages').html(result)
                $('#img-loading-service-tour-packages').hide()

                _common_function.Select2WithFixedOptionAndNoSearch($('.service-tour-extrapackage-packagename-select'))

            }
        });
    },
    AddTourPackage: function (element) {
        var table_element = element.closest('.service-tour-extrapackage-tbody')
        var new_position = _SetService_Tour_Detail.GetLastestExtraPackagesNo(element) ;
        table_element.find('.service-tour-extrapackage-summary-row').before(_manual_tour_html.html_service_tour_packages_add_packages_tr.replaceAll('@item.Id', '0').replaceAll('@(++index)', '' + new_position));
        _SetService_Tour_Detail.Select2WithFixedOptionAndNoSearch($(".service-tour-extrapackage-SupplierId-select"));
        _SetService_Tour_Detail.TourPackageOptionalMultiple($(".service-tour-extrapackage-Packageid-select"));
        $('.select2-container').attr("style", "width: 100% !important");

    },
    AddTourExtraPackage: function (element) {
        var table_element = element.closest('.service-tour-extrapackage-tbody')
        var new_position = _SetService_Tour_Detail.GetLastestExtraPackagesNo(element) + 1;
        table_element.find('.service-tour-extrapackage-summary-row').before(_order_detail_html.html_service_tour_packages_add_extra_package_tr.replaceAll('@item.Id', '0').replaceAll('@(++index)', '' + new_position).replaceAll('@(((double)item.BasePrice).ToString("N0"))', '').replaceAll('@(((int)item.Quantity).ToString("N0"))', '').replaceAll('@(((double)item.AmountBeforeVat).ToString("N0"))', '').replaceAll('@(((double)item.Vat).ToString("N0"))', '').replaceAll('@(((double)item.Amount).ToString("N0"))', '').replaceAll('@item.PackageName', ''))
    },
    GetLastestExtraPackagesNo: function (element) {
        var total = 0;
        element.closest('.service-tour-extrapackage-tbody').find('.service-tour-extrapackage-row').each(function (index, item) {
            total++;
        });
        return total
    },
    DeleteTourPackage: function (element) {
        var row_element = element.closest('.service-tour-extrapackage-row')
        row_element.remove()
    },

    Select2WithFixedOptionAndNoSearch: function (element) {
        element.select2({
            theme: 'bootstrap4',
            placeholder: "Tên NCC ",
            hintText: "Nhập từ khóa tìm kiếm",
            searchingText: "Đang tìm kiếm...",
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
                                text: item.id + ' - ' + item.fullname,
                                id: item.id,
                            }
                        })
                    };
                },
                cache: true
            }
        });
    },
    TourPackageOptionalMultiple: function (element) {

        element.select2({
            theme: 'bootstrap4',
            placeholder: "Chọn dịch vụ",
            hintText: "Nhập từ khóa tìm kiếm",
            searchingText: "Đang tìm kiếm...",
            ajax: {
                url: "/SetService/TourPackageOptionalMultiple",
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
                        results: $.map(response.data, function (item) {
                            return {
                                text: item.description,
                                id: item.codeValue,
                            }
                        })
                    };
                },
                cache: true
            }
        });

    },
    Summit: function () {
        $('#luu').removeAttr("onclick");
        var ServiceCode = $('#ServiceCode').val();
        var validate_failed = false
        var object_summit = {

            extra_packages: []
        }
        $('.service-tour-extrapackage-row').each(function (index, item) {
            var extra_package_element = $(item);
            var package_id = extra_package_element.find('.service-tour-extrapackage-SupplierId-select').select2("val");
            var package_Packageid = extra_package_element.find('.service-tour-extrapackage-Packageid-select').select2("val");
            var package_amount = extra_package_element.find('.service-tour-extrapackage-amount').val();
            var package_Quantity = extra_package_element.find('.service-tour-extrapackage-Quantity').val();
            var package_Times = extra_package_element.find('.service-tour-extrapackage-Times').val();
            var package_Price = extra_package_element.find('.service-tour-extrapackage-Price').val();
            var package_Note = extra_package_element.find('.service-tour-extrapackage-Note').val();
            var package_stt = extra_package_element.find('.service-tour-package-stt').val();
            var package_status = extra_package_element.find('.service-tour-extrapackage-status-select').val();

            if (package_id == null || package_id.toString() == undefined || package_id.toString().trim() == '' || package_Packageid == null || package_Packageid.toString() == undefined || package_Packageid.toString().trim() == '' || package_amount == undefined || package_amount.trim() == '' || package_Quantity == undefined || package_Quantity.trim() == '' || package_Times == undefined || package_Times.trim() == '' || package_Price == undefined || package_Price.trim() == '') {
                _msgalert.error("Nội dung tại dịch vụ thứ " + package_stt + ' của Bảng kê dịch vụ không được bỏ trống')
                validate_failed = true
                $('#luu').attr("onclick", "_SetService_Tour_Detail.Summit()");
                return false;
            }


            var extra_package = {
                Id: extra_package_element.attr('data-extra-package-id'),
                TourId: $('#HotelBookingID').val(),
                SupplierId: package_id.toString(),
                Packageid: package_Packageid.toString(),
                Amount: extra_package_element.find('.service-tour-extrapackage-amount').val().replaceAll(',', ''),
                PackageName: (ServiceCode + "-" + package_stt),
                Quantity: package_Quantity,
                Times: package_Times,
                Price: package_Price,
                Note: package_Note,
                Status: package_status,

            }
            object_summit.extra_packages.push(extra_package);
        });
        if (validate_failed) return;
        if (object_summit.extra_packages.length > 0){
            $.ajax({
                url: "/SetService/SummitTourService",
                type: "post",
                data: { data: object_summit.extra_packages },
                success: function (result) {
                    if (result != undefined && result.status == 0) {
                        _msgalert.success(result.msg);

                        setTimeout(function () {
                            _SetService_Tour_Detail.loaddata();
                          
                            _SetService_Tour_Detail.SetActive(3);
                            _SetService_Tour_Detail.OnStatuse(3);
                        }, 300);
                    } else {
                        $('#luu').attr("onclick", "_SetService_Tour_Detail.Summit()");
                        _msgalert.error(result.msg);

                    }

                }
            });
        } else {
            $('#luu').attr("onclick", "_SetService_Tour_Detail.Summit()");
        _msgalert.error("Thêm ít nhất 1 dịch vụ");
        }
     
    },
    TourOrderedInitialization: function () {
        _SetService_Tour_Detail.Select2WithFixedOptionAndNoSearch($(".service-tour-extrapackage-SupplierId-select"));
        _SetService_Tour_Detail.TourPackageOptionalMultiple($(".service-tour-extrapackage-Packageid-select"));
    },
    DeleteTourPackageOptional: function (id, tourId) {
        $.ajax({
            url: "/SetService/DeleteTourPackageOptional",
            type: "post",
            data: { id: id, tourId: tourId },
            success: function (result) {
                if (result.status === 0) {
                    _msgalert.success(result.msg);
                    setTimeout(function () {
                        _SetService_Tour_Detail.loaddata();
                        _SetService_Tour_Detail.SetActive(3);
                        _SetService_Tour_Detail.OnStatuse(3);

                    }, 1000)

                }
                else {
                    _msgalert.error(result.msg);
                   
                }
            }
        });
    },
 
    SummitTourGuests: function () {
        $('#luuTourGuests').removeAttr("onclick");
      
        var validate_failed = false
        var object_summit = {

            extra_packages: []
        }
        $('.service-TourGuests-extrapackage-row').each(function (index, item) {
            var extra_package_element = $(item);
           
            var RoomNumber = extra_package_element.find('.TourGuests-RoomNumber').val();
            var CCCD = extra_package_element.find('.TourGuests-CCCD').val();
/*
            if (package_id.toString() == undefined || package_id.toString().trim() == '' || package_Packageid.toString() == undefined || package_Packageid.toString().trim() == '' || package_amount == undefined || package_amount.trim() == '' || package_Quantity == undefined || package_Quantity.trim() == '' || package_Times == undefined || package_Times.trim() == '' || package_Price == undefined || package_Price.trim() == '') {
                _msgalert.error("Nội dung tại dịch vụ thứ " + extra_package_element.find('.service-tour-extrapackage-order').text() + ' của Bảng kê dịch vụ không được bỏ trống')
                validate_failed = true
                $('#luu').attr("onclick", "_SetService_Tour_Detail.Summit()");
                return false;
            }*/

            var extra_package = {
                Id: extra_package_element.attr('data-TourGuests-id'),
                RoomNumber: RoomNumber,
                Cccd: CCCD,

            }
            object_summit.extra_packages.push(extra_package);
        });
        if (validate_failed) return;
        $.ajax({
            url: "/SetService/UpdateTourGuests",
            type: "post",
            data: { data: object_summit.extra_packages },
            success: function (result) {
                if (result != undefined && result.status == 0) {
                    _msgalert.success(result.msg);

                    setTimeout(function () {
                        _SetService_Tour_Detail.loaddata();
                        _SetService_Tour_Detail.SetActive(3);
                        _SetService_Tour_Detail.OnStatuse(3);
                    }, 300);
                } else {
                    $('#luuTourGuests').attr("onclick", "_SetService_Tour_Detail.SummitTourGuests()");
                    _msgalert.error(result.msg);

                }

            }
        });
    },
    OpenPopupFundCustomer: function (id) {
        var OrderId = $('#Orderid').val();
        if ($('#checkbox_Tour').is(":checked") == true) {
         
            var title = "Xác nhận"
            var description = "Xác nhận áp dụng quỹ chăm sóc khách hàng ?"
            _SetService_Tour_Detail.openDialogSummit(title, description, function () {
                _global_function.AddLoading()
                $.ajax({
                    url: "/SetService/UpdateTourFundCustomer",
                    type: "post",
                    data: { id: id, OrderId: OrderId,type: 1 },
                    success: function (result) {
                        _global_function.RemoveLoading()
                        if (result != undefined && result.status == 0) {
                            _msgalert.success(result.msg);
                            setTimeout(function () {
                                _SetService_Tour_Detail.TourServiceDetail();
                            }, 300);
                        }
                        else {
                            _msgalert.error(result.msg);
                            $('#checkbox_Tour').removeAttr("checked")
                        }
                    
                    }
                })
            },
                function () {
                    $('#checkbox_Tour').removeAttr("checked")
                }
            )
        }
        else
        {
            var title = "Xác nhận"
            var description = "Xác nhận hủy áp dụng quỹ chăm sóc khách hàng ?"
            _SetService_Tour_Detail.openDialogSummit(title, description, function () {
                _global_function.AddLoading()
                $.ajax({
                    url: "/SetService/UpdateTourFundCustomer",
                    type: "post",
                    data: { id: id, OrderId: OrderId, type: 0 },
                    success: function (result) {
                        _global_function.RemoveLoading()
                        if (result != undefined && result.status == 0) {
                            _msgalert.success(result.msg);
                            setTimeout(function () {
                                _SetService_Tour_Detail.TourServiceDetail();
                            }, 300);
                        }
                        else {
                            _msgalert.error(result.msg);
                            $('#checkbox_Tour').prop("checked", true);
                        }
                    }
                })
            },
                function () {
                    $('#checkbox_Tour').prop("checked", true);
                }            )
        }
      
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
var _common_function_set_tour = {

    Comma: function (number) { //function to add commas to textboxes
        number = ('' + number).replace(/[^0-9.,]+/g, '');
        number += '';
        number = number.replace(',', ''); number = number.replace(',', ''); number = number.replace(',', '');
        number = number.replace(',', ''); number = number.replace(',', ''); number = number.replace(',', '');
        x = number.split('.');
        x1 = x[0];
        x2 = x.length > 1 ? '.' + x[1] : '';
        var rgx = /(\d+)(\d{3})/;
        while (rgx.test(x1))
            x1 = x1.replace(rgx, '$1' + ',' + '$2');
        return x1 + x2;
    }
}