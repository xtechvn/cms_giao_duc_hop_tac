var listInvoiceDetail = []
var listDetail = []
var amountEdit = 0
var isRender = false
var clientIdSearch = 0
var isEdit = false
var isEditView = false
var isSetInput = false
var isSetClient = false
var isSetSupplier = false
var listOrder = []
var orderId = 0
var clientId = 0
var clientName = ''
var taxNo = ''
var address = ''
var orderId = 0
var index = 0
var listFileAttach = []
var listAllFile = null
var indexFile = 0
var obj_request = {
    index: index,
    productName: "",
    unit: "",
    quantity: 1,
    price: 0,
    total: 0,
    priceExtraExport: 0,
    priceExtra: 0,
}
var _add_invoice_request = {
    Initialization: function (isEdit = false) {
        this.validImageTypes = ['image/gif', 'image/jpeg', 'image/png', 'image/gif',
            'application/vnd.openxmlformats-officedocument.wordprocessingml.document', 'application/pdf'];
        this.validImageSize = 10 * 1024 * 1024;
        $("#client-select").select2({
            theme: 'bootstrap4',
            placeholder: "Tên KH, Điện Thoại, Email",
            hintText: "Nhập từ khóa tìm kiếm",
            searchingText: "Đang tìm kiếm...",
            maximumSelectionLength: 1,
            ajax: {
                url: "/Contract/ClientSuggestion",
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
                                text: item.clientname + ' - ' + item.email + ' - ' + item.phone,
                                id: item.id,
                            }
                        })
                    };
                },
                cache: true
            }
        });
        $('input').attr('autocomplete', 'off');
        //$('#btnDeleteImage').hide()
        $('#attachFile').hide()
        if (!isEdit)
            _add_invoice_request.InitItemDetail()
        setTimeout(function () {
            if (!isEdit && $('#orderId').val() !== '0' && $('#orderId').val() !== null && $('#orderId').val() !== '' && $('#orderId').val() !== undefined)
                _add_invoice_request.SetServiceData()
        }, 800)
        $("#orderCode").select2({
            theme: 'bootstrap4',
            placeholder: "Tìm kiếm mã hóa đơn",
            hintText: "Nhập từ khóa tìm kiếm",
            searchingText: "Đang tìm kiếm...",
            maximumSelectionLength: 1,
            closeOnSelect: false,
            ajax: {
                url: "/InvoiceRequest/GetListFilter",
                type: "post",
                dataType: 'json',
                delay: 250,
                data: function (params) {
                    var query = {
                        jsonData: JSON.stringify(listOrder),
                        text: params.term,
                    }
                    return query;
                },
                processResults: function (response) {
                    return {
                        results: $.map(response.data, function (item) {
                            return {
                                text: item.orderCode,
                                id: item.orderId,
                            }
                        })
                    };
                },
                cache: true
            }
        });
    },
    SetServiceData: function () {
        orderId = $('#orderId').val()
        clientId = $('#clientId').val()
        clientName = $('#clientName').val()
        taxNo = $('#taxNoClient').val()
        $('#taxtNo').val(taxNo)
        address = $('#businessAddress').val()
        $('#address').val(address)
        setTimeout(function () {
            var newOption = new Option(clientName, clientId, true, true);
            $('#client-select').append(newOption).trigger('change');
        }, 800)
    },
    FormatNumberStr: function (amount) {
        var n = parseFloat(amount, 10);
        return isNaN(n) === true ? '' : n.toLocaleString().replaceAll('.', ',')
    },
    GetOrderListByClientId: function () {
        var client_id = $('#client-select').val()
        if (client_id === null || client_id === undefined || client_id === '' || client_id === 0) {
            return
        }
        listOrder = []
        _global_function.AddLoading()
        $("#orderCode").empty()
        $.ajax({
            url: "/InvoiceRequest/GetOrderListByClientId",
            type: "Post",
            data: { 'clientId': client_id, 'invoiceRequestId': $('#requestId').val() },
            success: function (result) {
                _global_function.RemoveLoading()
                $("#body_orderList").empty();
                var client = result.client
                if (client != null && !isEditView) {
                    $('#taxtNo').val(client.taxNo)
                    $('#address').val(client.businessAddress)
                }
                listOrder = result.data
                var totalAmount = 0
                for (var i = 0; i < result.data.length; i++) {
                    $('#request-relate-table').find('tbody').append(
                        "<tr id='order_" + i + "'>" +
                        "<td>" +
                        " <label class='radio mr-2'>" +
                        "  <input type='radio' name='optradio' id='order_radio_" + result.data[i].orderId + "' onclick='_add_invoice_request.OnCheckBox(" + i + ")'>" +
                        " <span class='checkmark'></span> " + (i + 1) +
                        " </label>" +
                        "</td>" +
                        "<td>" +
                        " <a class='blue' href='/Order/Orderdetails?id=" + result.data[i].orderId + "'> " + result.data[i].orderCode + " </a>"
                        + "</td>" +
                        "<td>" + result.data[i].startDate + " - " + result.data[i].endDate + "</td>" +
                        "<td>" + result.data[i].salerName + "</td>" +
                        "<td class='text-right'>" + _add_invoice_request.FormatNumberStr(result.data[i].amount) + "</td>" +
                        "</tr>"
                    );
                    totalAmount += result.data[i].amount
                    if (result.data[i].isChecked) {
                        let code = result.data[i].orderId
                        setTimeout(function () {
                            $('#order_radio_' + code).prop('checked', true)
                        }, 800)
                    }
                }
                $('#request-relate-table').find('tbody').append(
                    "<tr style='font-weight:bold !important;'>" +
                    "<td class='text-right' colspan='4'> Tổng </td>" +
                    "<td class='text-right'>" + _add_invoice_request.FormatNumberStr(totalAmount) + "</td>" +
                    "</tr>"
                );
                setTimeout(function () {
                    if (orderId !== 0 && orderId !== null && orderId != undefined)
                        $('#order_radio_' + orderId).prop('checked', true)
                    _add_invoice_request.OnCheckBox();
                }, 1000)
            }
        });
    },
    Validate: function () {
        let result = true
        _add_invoice_request.ClearError()
        if ($('#client-select').val() == undefined || $('#client-select').val() == null || $('#client-select').val() == '') {
            _add_invoice_request.DisplayError('validate-client-select', 'Vui lòng chọn khách hàng')
            result = false;
        }
        if ($('#planDate').val() == undefined || $('#planDate').val() == null || $('#planDate').val() == '') {
            _add_invoice_request.DisplayError('validate-planDate', 'Vui lòng chọn ngày dự kiến xuất')
            result = false;
        }
        if ($('#taxtNo').val() == undefined || $('#taxtNo').val() == null || $('#taxtNo').val() == '') {
            _add_invoice_request.DisplayError('validate-taxtNo', 'Vui lòng nhập mã số thuế')
            result = false;
        }
        if ($('#companyName').val() == undefined || $('#companyName').val() == null || $('#companyName').val() == '') {
            _add_invoice_request.DisplayError('validate-companyName', 'Vui lòng nhập tên công ty')
            result = false;
        }
        if ($('#address').val() == undefined || $('#address').val() == null || $('#address').val() == '') {
            _add_invoice_request.DisplayError('validate-address', 'Vui lòng nhập địa chỉ')
            result = false;
        }
        var flag = false
        for (var i = 0; i < listOrder.length; i++) {
            var checked = $('#order_radio_' + listOrder[i].orderId).is(":checked")
            if (checked) {
                flag = true
            }
        }
        if (!flag) {
            _add_invoice_request.DisplayError('validate-orderId', 'Vui lòng tích chọn đơn hàng ')
            result = false;
        }
        if ($('#note').val() !== undefined && $('#note').val() !== null
            && $('#note').val() !== '' && ($('#note').val()).length > 3000) {
            _add_invoice_request.DisplayError('validate-description', 'Vui lòng nhập ghi chú không quá 3000 kí tự')
            result = false;
        }

        for (var i = 0; i < listInvoiceDetail.length; i++) {
            if (listInvoiceDetail[i].productName == null || listInvoiceDetail[i].productName == ''
                || listInvoiceDetail[i].productName == undefined) {
                _add_invoice_request.DisplayError('validate-detail-info', 'Vui lòng nhập các nội dung bắt buộc trong Nội dung dịch vụ cần xuất')
                result = false;
            }
            if (listInvoiceDetail[i].unit == null || listInvoiceDetail[i].unit == ''
                || listInvoiceDetail[i].unit == undefined) {
                _add_invoice_request.DisplayError('validate-detail-info', 'Vui lòng nhập các nội dung bắt buộc trong Nội dung dịch vụ cần xuất')
                result = false;
            }
            if (listInvoiceDetail[i].quantity == null || listInvoiceDetail[i].quantity == ''
                || listInvoiceDetail[i].quantity == undefined) {
                _add_invoice_request.DisplayError('validate-detail-info', 'Vui lòng nhập các nội dung bắt buộc trong Nội dung dịch vụ cần xuất')
                result = false;
            }
            if (listInvoiceDetail[i].price == null || listInvoiceDetail[i].price == ''
                || listInvoiceDetail[i].price == undefined) {
                _add_invoice_request.DisplayError('validate-detail-info', 'Vui lòng nhập các nội dung bắt buộc trong Nội dung dịch vụ cần xuất')
                result = false;
            }
        }

        if (!result) return false

        return true
    },
    AddInvoiceRequest: function (isSend) {
        let validate = _add_invoice_request.Validate()
        if (!validate)
            return;
        var orderId = 0
        for (var i = 0; i < listOrder.length; i++) {
            var checked = $('#order_radio_' + listOrder[i].orderId).is(":checked")
            if (checked) {
                orderId = listOrder[i].orderId
            }
        }
        let other_image = [];
        $('#suplier_room_list_image .img_other').each(function () {
            let image_item = $(this);
            other_image.push(image_item.attr('src'));
        });
        let obj = {
            'taxNo': $('#taxtNo').val(),
            'companyName': $('#companyName').val(),
            'planDateStr': $('#planDate').val(),
            'address': $('#address').val(),
            'orderId': orderId,
            'note': $('#note').val(),
            'isSend': isSend,
            'clientId': parseInt(($('#client-select').val())),
            'totalPrice': parseFloat($('#totalPriceBeforeVAT').html().replaceAll('.', '').replaceAll(',', '')),
            'vAT': 0,
            'totalPriceVAT': 0,
            'invoiceRequestDetails': listInvoiceDetail,
            'OtherImages': other_image
        }

        _global_function.AddLoading()
        $.ajax({
            url: "/InvoiceRequest/AddJson",
            type: "Post",
            data: obj,
            success: function (result) {
                _global_function.RemoveLoading()
                if (result.isSuccess) {
                    _msgalert.success(result.message);
                    _global_function.ConfirmFileUpload($('.attachment-addnew'),result.id)
                    $.magnificPopup.close();
                    setTimeout(function () { window.location.reload() }, 500)
                } else {
                    _msgalert.error(result.message);
                }
            }
        });
    },
    InitializationEdit: function (client_id, supplier_id) {
        this.GetDetailRequest()
        isEditView = true
        if ((client_id == undefined || client_id == null || client_id == 0 || client_id == '')) {
            return
        }
        isEdit = true
        setTimeout(function () {
            var newOption = new Option($('#client_name_hide').val(), client_id, true, true);
            $('#client-select').append(newOption).trigger('change');
        }, 500)
        this.GetOrderListByClientId(true)
    },
    GetDetailRequest: function () {
        _global_function.AddLoading()
        $.ajax({
            url: "/InvoiceRequest/GetDetail",
            type: "Post",
            data: { 'invoiceRequestId': $('#requestId').val() },
            success: function (result) {
                _global_function.RemoveLoading()
                var detail = result.data
                var listDetail = detail.invoiceRequestDetails
                listFileAttach = []
                for (var i = 0; i < listDetail.length; i++) {
                    var obj_request = {
                        index: index,
                        id: listDetail[i].id,
                        productName: listDetail[i].productName,
                        unit: listDetail[i].unit,
                        quantity: listDetail[i].quantity,
                        price: listDetail[i].price,
                        vat: listDetail[i].vat,
                        total: listDetail[i].price * listDetail[i].quantity,
                        priceExtraExport: listDetail[i].priceExtraExport,
                        priceExtra: listDetail[i].priceExtra,
                    }
                    index++
                    listInvoiceDetail.push(obj_request)
                    _add_invoice_request.RenderItemDetail()
                }
                if (detail.attachFiles !== null) {
                    for (var i = 0; i < detail.attachFiles.length; i++) {
                        let file = {
                            index: indexFile,
                            name: detail.attachFiles[i],
                            isExists: true,
                            isDelete: false
                        }
                        listFileAttach.push(file)
                        indexFile++
                    }
                    for (var i = 0; i < listFileAttach.length; i++) {
                        $('#divAttachFile').append(
                            "<img src='' alt='' id='attachFile' />" + listFileAttach[i].name +
                            "<a class='blue' style='color: #056BD3 !important; cursor:pointer;margin-left: 20px;' onclick='_add_invoice_request.OnDeleteImage(" + indexFile + ")'>Xóa</a>"
                        );
                    }
                }
            }
        });
    },
    Close: function () {
        $('#create_contract_pay').removeClass('show')
        setTimeout(function () {
            $('#create_contract_pay').remove();
        }, 300);
    },
    EditInvoiceRequest: function (isSend) {
        let validate = _add_invoice_request.Validate()
        if (!validate)
            return;
        var orderId = 0
        for (var i = 0; i < listOrder.length; i++) {
            var checked = $('#order_radio_' + listOrder[i].orderId).is(":checked")
            if (checked) {
                orderId = listOrder[i].orderId
            }
        }
        let other_image = [];
        $('#suplier_room_list_image .img_other').each(function () {
            let image_item = $(this);
            other_image.push(image_item.attr('src'));
        });
        let obj = {
            'id': $('#requestId').val(),
            'invoiceRequestNo': $('#invoiceRequestNo').val(),
            'taxNo': $('#taxtNo').val(),
            'companyName': $('#companyName').val(),
            'planDateStr': $('#planDate').val(),
            'address': $('#address').val(),
            'orderId': orderId,
            'note': $('#note').val(),
            'isSend': isSend,
            'clientId': parseInt(($('#client-select').val())),
            'totalPrice': parseFloat($('#totalPriceBeforeVAT').html().replaceAll('.', '').replaceAll(',', '')),
            'vAT': 0,
            'totalPriceVAT': 0,
            'invoiceRequestDetails': listInvoiceDetail,
            'OtherImages': other_image
        }
        _global_function.AddLoading()
        $.ajax({
            url: "/InvoiceRequest/Update",
            type: "Post",
            data: obj,
            success: function (result) {
                _global_function.RemoveLoading()
                if (result.isSuccess) {
                    _msgalert.success(result.message);
                    _global_function.ConfirmFileUpload($('.attachment-edit'), $('#requestId').val())
                    $.magnificPopup.close();
                    setTimeout(function () { window.location.reload() }, 500)
                } else {
                    _msgalert.error(result.message);
                }
            }
        });
    },
    OnCheckBox: function (index) {
        for (var i = 0; i < listOrder.length; i++) {
            var checked = $('#order_radio_' + listOrder[i].orderId).is(":checked")
            if (checked) {
                orderId = listOrder[i].orderId
                var newOption1 = new Option(listOrder[i].orderCode, listOrder[i].orderId, true, true);
                $('#orderCode').append(newOption1)
            }
        }
        _add_invoice_request.SetCheckedRadio()
    },
    OnCheckedRequest: function () {
        for (var i = 0; i < listOrder.length; i++) {
            $('#order_radio_' + listOrder[i].orderId).prop('checked', false)
        }

        var requestChoose = $('#orderCode').val()
        $("#orderCode").empty()
        if (requestChoose === undefined || requestChoose === null || requestChoose === '') return
        for (var i = 0; i < listOrder.length; i++) {
            if (requestChoose !== undefined && requestChoose !== null && requestChoose !== ''
                && requestChoose.includes(listOrder[i].orderId + '')) {
                $('#order_radio_' + listOrder[i].orderId).prop('checked', true)
            }
        }
        setTimeout(_add_invoice_request.OnCheckBox(), 300)
    },
    SetCheckedRadio: function () {
        $("#orderCode").empty()
        for (var i = 0; i < listOrder.length; i++) {
            var checked = $('#order_radio_' + listOrder[i].orderId).is(":checked")
            if (checked) {
                orderId = listOrder[i].orderId
                var newOption1 = new Option(listOrder[i].orderCode, listOrder[i].orderId, true, true);
                $('#orderCode').append(newOption1)
            }
        }
    },
    AddToListDetail: function (index) {
        var payment_request_type = $('#payment-voucher-type').val()
        if (payment_request_type !== null && payment_request_type !== '' && (parseInt(payment_request_type) == 1 || parseInt(payment_request_type) == 3)) { // thanh toán dịch vụ
            for (var i = 0; i < listInvoiceDetail.length; i++) {
                if (i === index) {
                    listDetail.push(listInvoiceDetail[i])
                }
            }
        }
    },
    ClearError: function () {
        $(".validate-planDate").find('p').remove();
        $(".validate-taxtNo").find('p').remove();
        $(".validate-client-select").find('p').remove();
        $(".validate-companyName").find('p').remove();
        $(".validate-address").find('p').remove();
        $(".validate-note").find('p').remove();
        $(".validate-detail-info").find('p').remove();
        $(".validate-imagefile").find('p').remove();
        $(".validate-orderId").find('p').remove();
    },
    DisplayError: function (className, message) {
        $("." + className).find('p').remove();
        $("." + className).append("<p>" + message + "</p>");
        $("." + className).css("color", "red");
        $("." + className).css("font-size", "13px");
        $("." + className).css("margin-top", "3px");
    },
    OnAddRoomImage: function () {
        const files = document.querySelector('input[name=ImageRoom]').files;
        let grid_image_preview = $('#suplier_room_list_image');

        for (let file of files) {
            if (!this.validImageTypes.includes(file['type'])) {
                _msgalert.error("File tải lên không đúng định dạng(.jpg,.png, .gif,.pdf, .doc & docx)");
                break;
            }

            if (this.validImageSize < file.size) {
                _msgalert.error("Ảnh tải lên vượt quá dung lượng cho phép (10MB).");
                break;
            }

            const reader = new FileReader();
            reader.addEventListener("load", function () {
                let is_exist = grid_image_preview.find(`.image_preview[data-name="${file.name}"]`).length > 0 ? true : false;
                if (!is_exist) {
                    let html = `<div class="col-md-3 col-3 mb10 image_preview" style="min-width: 200px !important; " data-name="${file.name}">
                     <div class="choose-ava" >
                     <img class="img_other" src="${reader.result}" >
                     <button type="button" class="delete" onclick="this.closest('.image_preview').remove();">×</button>
                     </div>
                     </div>`;
                    grid_image_preview.append(html);
                }

            }, false);

            if (file) {
                reader.readAsDataURL(file);
            }
        }
    },
    OnChangeImage: function () {
        var files = document.querySelector('input[name=imagefile]').files
        for (let file of files) {
            var fileName = file.name
            if (!this.validImageTypes.includes(file['type'])) {
                _add_invoice_request.DisplayError('validate-imagefile',
                    'File đính kèm không đúng định dạng ảnh .png, .jpg, .jpeg, gif. Vui lòng kiểm tra lại')
                break
            }

            if (file.size > (10 * 1024 * 1024)) {
                _add_invoice_request.DisplayError('validate-imagefile',
                    'File đính kèm ' + fileName + ' không được quá 10MB. Vui lòng kiểm tra lại')
                break
            }
            $('#divAttachFile').append(
                "<img src='' alt='' id='attachFile' />" + fileName +
                "<a class='blue' style='color: #056BD3 !important; cursor:pointer;margin-left: 20px;' onclick='_add_invoice_request.OnDeleteImage(" + indexFile + ")'>Xóa</a>"
            );
            const reader = new FileReader();
            reader.addEventListener("load", function () {

                let is_exist = grid_image_preview.find(`.image_preview[data-name="${file.name}"]`).length > 0 ? true : false;
                if (!is_exist) {
                    let html = `<div class="col-md-3 col-6 mb10 image_preview"  style="min-width: 200px !important; " data-name="${file.name}">
                     <div class="choose-ava">
                     <img class="img_other" src="${reader.result}">
                     <button type="button" class="delete" onclick="this.closest('.image_preview').remove();">×</button>
                     </div>
                     </div>`;
                    grid_image_preview.append(html);
                }

            }, false);

            if (file) {
                reader.readAsDataURL(file);
            }
            file.index = indexFile
            listFileAttach.push(file)
            indexFile++
        }

    },
    OnDeleteImage: function (index) {
        $('#divAttachFile').empty()
        listFileAttach = listFileAttach.filter(n => n.index !== index)
        for (var i = 0; i < listFileAttach.length; i++) {
            $('#divAttachFile').append(
                "<img src='' alt='' id='attachFile' />" + listFileAttach[i].name +
                "<a class='blue' style='color: #056BD3 !important; cursor:pointer;' onclick='_add_invoice_request.OnDeleteImage(" + listFileAttach[i].index + ")'>Xóa</a>"
            );
        }
    },
    InitItemDetail: function () {
        var obj = Object.assign({}, obj_request)
        obj.index = index
        listInvoiceDetail.push(obj)
        index++
        _add_invoice_request.RenderItemDetail()
    },
    AddItemDetail: function () {
        var obj = Object.assign({}, obj_request)
        obj.index = index
        listInvoiceDetail.push(obj)
        index++
        _add_invoice_request.RenderItemDetail()
    },
    DeleteItemDetail: function (index) {
        if (listInvoiceDetail.length == 1) {
            _add_invoice_request.DisplayError('validate-detail-info', 'Vui lòng không thể xóa hết các nội dung dịch vụ cần xuất')
            return
        }

        var listRemain = listInvoiceDetail.filter(n => n.index != index)
        listInvoiceDetail = listRemain
        _add_invoice_request.RenderItemDetail()
    },
    RenderItemDetail: function () {
        $('#body_contenxt_request').empty()
        var total = 0
        var priceExtraExport = 0
        var priceExtra = 0
        for (var i = 0; i < listInvoiceDetail.length; i++) {
            var disabled = i == 0 ? "disabled" : ""
            $('#table_contenxt_request').find('tbody').append(
                "<tr>" +
                " <td>" + (i + 1) + "</td>" +
                " <td><input type='text' class='form-control' id='productName_" + listInvoiceDetail[i].index + "' onchange='_add_invoice_request.OnChangeDetail(" + listInvoiceDetail[i].index + ")'></td>" +
                " <td><input type='text' class='form-control' id='unit_" + listInvoiceDetail[i].index + "' onchange='_add_invoice_request.OnChangeDetail(" + listInvoiceDetail[i].index + ")'></td>" +
                " <td><input type='text' class='form-control text-right' id='quantity_" + listInvoiceDetail[i].index + "'  onchange='_add_invoice_request.OnChangeDetail(" + listInvoiceDetail[i].index + ")'></td>" +
                " <td><input type='text' class='form-control text-right' id='price_" + listInvoiceDetail[i].index + "'  onkeyup='_add_invoice_request.OnChangeDetail(" + listInvoiceDetail[i].index + ")'></td>" +
                " <td><input type='text' class='form-control text-right background-disabled' disabled id='total_" + listInvoiceDetail[i].index + "' ></td>" +
                "<td><input type='text' class='form-control text-right' id='priceExtraExport_" + listInvoiceDetail[i].index + "' onkeyup='_add_invoice_request.OnChangeDetail(" + listInvoiceDetail[i].index + ")'></td>" +
                " <td><input type='text' class='form-control text-right' id='priceExtra_" + listInvoiceDetail[i].index + "' onkeyup='_add_invoice_request.OnChangeDetail(" + listInvoiceDetail[i].index + ")'></td>" +
                " <td>" +
                " <button class='delete btn btn-danger' onclick='_add_invoice_request.DeleteItemDetail(" + listInvoiceDetail[i].index + ")'>" +
                " <i class='fa fa-trash-o'></i>" +
                " </button>" +
                " </td>" +
                " </tr>"
            );
            total += listInvoiceDetail[i].quantity * listInvoiceDetail[i].price
            priceExtraExport += listInvoiceDetail[i].priceExtraExport
            priceExtra += listInvoiceDetail[i].priceExtra
            $('#productName_' + listInvoiceDetail[i].index).val(listInvoiceDetail[i].productName)
            $('#unit_' + listInvoiceDetail[i].index).val(listInvoiceDetail[i].unit)
            $('#quantity_' + listInvoiceDetail[i].index).val(listInvoiceDetail[i].quantity)
            $('#price_' + listInvoiceDetail[i].index).val(_add_invoice_request.FormatNumberStr(listInvoiceDetail[i].price))
            $('#total_' + listInvoiceDetail[i].index).val(_add_invoice_request.FormatNumberStr(listInvoiceDetail[i].price * listInvoiceDetail[i].quantity))
            $('#priceExtraExport_' + listInvoiceDetail[i].index).val(_add_invoice_request.FormatNumberStr(listInvoiceDetail[i].priceExtraExport))
            $('#priceExtra_' + listInvoiceDetail[i].index).val(_add_invoice_request.FormatNumberStr(listInvoiceDetail[i].priceExtra))
        }
        $('#table_contenxt_request').find('tbody').append(
            "<tr>" +
            "<td></td>" +
            " <td>" +
            " <div class='flex space-between'>" +
            " <a class='blue' onclick='_add_invoice_request.AddItemDetail()' style='color: #056BD3 !important;'>" +
            " <i class='fa fa-plus'></i> Thêm mới" +
            " </a>" +
            " <strong>Tổng cộng</strong>" +
            " </div>" +
            " </td>" +
            "  <td></td>" +
            "  <td></td>" +
            "  <td></td>" +
            "  <td class='text-right' > <input type='text' class='form-control  text-right' id='totalPrice' value='" + _add_invoice_request.FormatNumberStr(total) + "' disabled> </td>" +
            "  <td class='text-right' id=''><input type='text' class='form-control  text-right' id='totalPriceExport' value='" + _add_invoice_request.FormatNumberStr(priceExtraExport) + "' disabled> </td>" +
            "  <td class='text-right' id=''><input type='text' class='form-control  text-right' id='totalPriceExtra' value=' " + _add_invoice_request.FormatNumberStr(priceExtra) + "' disabled></td>" +
            "  <td></td>" +
            " </tr>"
        );
        $('#totalPriceBeforeVAT').html(_add_invoice_request.FormatNumberStr(total + priceExtraExport))
        $('#totalPriceExportAfter').html(_add_invoice_request.FormatNumberStr(priceExtra))
    },
    OnChangeDetail: function (index) {
        var indexBefor = index
        var total = 0
        var totalPriceExtraExport = 0
        var totalPriceExtra = 0
        for (var i = 0; i < listInvoiceDetail.length; i++) {
            if (indexBefor == 0)
                index = listInvoiceDetail[i].index
            if (listInvoiceDetail[i].index == index) {
                var price = 0
                if ($('#price_' + index).val() !== undefined && $('#price_' + index).val() !== null && $('#price_' + index).val() !== ''
                    && parseInt($('#price_' + index).val()) > 0) {
                    price = parseInt($('#price_' + index).val().replaceAll('.', '').replaceAll(',', ''))
                }
                var quantity = 0
                if ($('#quantity_' + index).val() !== undefined && $('#quantity_' + index).val() !== null && $('#quantity_' + index).val() !== ''
                    && parseInt($('#quantity_' + index).val()) > 0) {
                    quantity = parseInt($('#quantity_' + index).val().replaceAll('.', '').replaceAll(',', ''))
                }
                var priceExtraExport = 0
                if ($('#priceExtraExport_' + index).val() !== undefined && $('#priceExtraExport_' + index).val() !== null && $('#priceExtraExport_' + index).val() !== ''
                    && parseInt($('#priceExtraExport_' + index).val()) > 0) {
                    priceExtraExport = parseFloat($('#priceExtraExport_' + index).val().replaceAll('.', '').replaceAll(',', ''))
                }
                var priceExtra = 0
                if ($('#priceExtra_' + index).val() !== undefined && $('#priceExtra_' + index).val() !== null && $('#priceExtra_' + index).val() !== ''
                    && parseInt($('#priceExtra_' + index).val()) > 0) {
                    priceExtra = parseFloat($('#priceExtra_' + index).val().replaceAll('.', '').replaceAll(',', ''))
                }
                listInvoiceDetail[i].productName = $('#productName_' + index).val()
                listInvoiceDetail[i].unit = $('#unit_' + index).val()
                listInvoiceDetail[i].quantity = $('#quantity_' + index).val() !== undefined &&
                    $('#quantity_' + index).val() !== null && $('#quantity_' + index).val() !== '' ? parseInt($('#quantity_' + index).val()) : 1
                listInvoiceDetail[i].price = $('#price_' + index).val() !== undefined &&
                    $('#price_' + index).val() !== null && $('#price_' + index).val() !== '' ? parseFloat($('#price_' + index).val().replaceAll('.', '').replaceAll(',', '')) : 0
                listInvoiceDetail[i].total = price * quantity
                listInvoiceDetail[i].priceExtraExport = priceExtraExport
                listInvoiceDetail[i].priceExtra = priceExtra
                listInvoiceDetail[i].vat = 0
                $('#price_' + index).val(_add_invoice_request.FormatNumberStr(price))
                $('#total_' + index).val(_add_invoice_request.FormatNumberStr(price * quantity))
                $('#priceExtraExport_' + index).val(_add_invoice_request.FormatNumberStr(priceExtraExport))
                $('#priceExtra_' + index).val(_add_invoice_request.FormatNumberStr(priceExtra))
            }
        }

        for (var i = 0; i < listInvoiceDetail.length; i++) {
            let index = listInvoiceDetail[i].index
            var price = 0
            if ($('#price_' + index).val() !== undefined && $('#price_' + index).val() !== null && $('#price_' + index).val() !== ''
                && parseInt($('#price_' + index).val()) > 0) {
                price = parseInt($('#price_' + index).val().replaceAll('.', '').replaceAll(',', ''))
            }
            var quantity = 0
            if ($('#quantity_' + index).val() !== undefined && $('#quantity_' + index).val() !== null && $('#quantity_' + index).val() !== ''
                && parseInt($('#quantity_' + index).val()) > 0) {
                quantity = parseInt($('#quantity_' + index).val().replaceAll('.', '').replaceAll(',', ''))
            }
            var priceExtraExport = 0
            if ($('#priceExtraExport_' + index).val() !== undefined && $('#priceExtraExport_' + index).val() !== null && $('#priceExtraExport_' + index).val() !== ''
                && parseInt($('#priceExtraExport_' + index).val()) > 0) {
                priceExtraExport = parseFloat($('#priceExtraExport_' + index).val().replaceAll('.', '').replaceAll(',', ''))
            }
            var priceExtra = 0
            if ($('#priceExtra_' + index).val() !== undefined && $('#priceExtra_' + index).val() !== null && $('#priceExtra_' + index).val() !== ''
                && parseInt($('#priceExtra_' + index).val()) > 0) {
                priceExtra = parseFloat($('#priceExtra_' + index).val().replaceAll('.', '').replaceAll(',', ''))
            }
            total += (price * quantity)
            totalPriceExtraExport += priceExtraExport
            totalPriceExtra += priceExtra
        }
        $('#totalPrice').val(_add_invoice_request.FormatNumberStr(total))
        $('#totalPriceExport').val(_add_invoice_request.FormatNumberStr(totalPriceExtraExport))
        $('#totalPriceExtra').val(_add_invoice_request.FormatNumberStr(totalPriceExtra))
        $('#totalPriceBeforeVAT').html(_add_invoice_request.FormatNumberStr(total + totalPriceExtraExport))
        $('#totalPriceExportAfter').html(_add_invoice_request.FormatNumberStr(totalPriceExtra))
    },
}