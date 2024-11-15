var listInvoiceDetail = []
var listDetail = []
var isEdit = false
var isEditView = false
var listInvoiceRequest = []
let isPickerCreateAddInvoice = false;
var listChecked = []
var orderId = 0
var index = 0
var listFileAttach = []
var listAllFile = null
var indexFile = 0
var isSetInput = false
var taxNo = ""
var companyName = ""
var address = ""
var firstInvoiceRequestNo = ''
var requestChooseLast = []
var vat = 0
var _add_invoice = {
    Initialization: function (isEdit = false) {
        this.validImageTypes = ['image/gif', 'image/jpeg', 'image/png', 'image/gif'];
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
        $('#attachFile').hide()
        $('#lblBankAccountRequired').show()
        $('#lblBankAccount').hide()
        $("#invoiceRequestCode").select2({
            theme: 'bootstrap4',
            placeholder: "Mã phiếu",
            hintText: "Nhập từ khóa tìm kiếm",
            searchingText: "Đang tìm kiếm...",
            closeOnSelect: false,
            ajax: {
                url: "/Invoice/GetListFilter",
                type: "post",
                dataType: 'json',
                delay: 250,
                data: function (params) {
                    var query = {
                        jsonData: JSON.stringify(listInvoiceRequest),
                        text: params.term,
                        text: params.term,
                        text: params.term,
                    }
                    return query;
                },
                processResults: function (response) {
                    return {
                        results: $.map(response.data, function (item) {
                            return {
                                text: item.invoiceRequestNo,
                                id: item.id,
                            }
                        })
                    };
                },
                cache: true
            }
        });
        $("#invoiceRequestCreate").select2({
            theme: 'bootstrap4',
            placeholder: "Người tạo",
            ajax: {
                url: "/PaymentRequest/GetUserSuggestionList",
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
                                text: item.fullname,
                                id: item.id,
                            }
                        })
                    };
                },
                cache: true
            }
        });
    },
    FormatNumberStr: function (amount) {
        var n = parseFloat(amount, 10);
        return isNaN(n) === true ? '' : n.toLocaleString().replaceAll('.', ',')
    },
    GetInvoiceRequestsByClientId: function (isEditView = false) {
        var client_id = $('#client-select').val()
        if (client_id === null || client_id === undefined || client_id === '' || client_id === 0) {
            return
        }
        listInvoiceRequest = []
        _global_function.AddLoading()
        $.ajax({
            url: "/Invoice/GetInvoiceRequestsByClientId",
            type: "Post",
            data: { 'clientId': client_id, 'invoiceId': $('#requestId').val() },
            success: function (result) {
                _global_function.RemoveLoading()
                $("#body_invoice_request_list").empty();
                listInvoiceRequest = result.data
                var totalPrice = 0
                var priceExtraExport = 0
                var priceExtra = 0
                $("#invoiceRequestCode").empty()
                for (var i = 0; i < result.data.length; i++) {
                    let planDateStr = ""
                    var now = new Date()
                    if (Date.parse(result.data[i].planDate) < now.setHours(0, 0, 0, 0)) {
                        planDateStr += result.data[i].planDateViewStr + "<div style='color: red'>" + result.data[i].paymentDateRemind + "</div>"
                    } else {
                        planDateStr += result.data[i].planDateViewStr + "<div>" + result.data[i].paymentDateRemind + "</div>"
                    }
                    $('#request-relate-table').find('tbody').append(
                        "<tr id='order_" + i + "'>" +
                        "<td>" +
                        "<label class='check-list number'>" +
                        " <input type='checkbox' name='order_ckb' id='order_ckb_" + result.data[i].id + "' onclick='_add_invoice.OnCheckBox(" + result.data[i].id + ")'>" +
                        " <span class='checkmark'></span>" + (i + 1) +
                        "  </label>" +
                        "</td>" +
                        "<td>" +
                        " <a class='blue' href='/InvoiceRequest/Detail?invoiceRequestId=" + result.data[i].id + "'> " + result.data[i].invoiceRequestNo + " </a>"
                        + "</td>" +
                        "<td>" + planDateStr + "</td>" +
                        "<td>" + result.data[i].userCreateName + "</td>" +
                        "<td class='text-right'>" + _add_invoice.FormatNumberStr(result.data[i].totalPrice) + "</td>" +
                        "<td class='text-right'>" + _add_invoice.FormatNumberStr(result.data[i].priceExtraExport) + "</td>" +
                        "<td class='text-right'>" + _add_invoice.FormatNumberStr(result.data[i].priceExtra) + "</td>" +
                        "</tr>"
                    );
                    totalPrice += result.data[i].totalPrice
                    priceExtraExport += result.data[i].priceExtraExport
                    priceExtra += result.data[i].priceExtra
                    if (result.data[i].isChecked) {
                        let code = result.data[i].id
                        setTimeout(function () {
                            $('#order_ckb_' + code).prop('checked', true)
                        }, 400)
                    }
                }
                $('#request-relate-table').find('tbody').append(
                    "<tr style='font-weight:bold !important;'>" +
                    "<td class='text-right' colspan='4'> Tổng </td>" +
                    "<td class='text-right'>" + _add_invoice.FormatNumberStr(totalPrice) + "</td>" +
                    "<td class='text-right'>" + _add_invoice.FormatNumberStr(priceExtraExport) + "</td>" +
                    "<td class='text-right'>" + _add_invoice.FormatNumberStr(priceExtra) + "</td>" +
                    "</tr>"
                );
                _add_invoice.GetDetailInvoice()
                if (isEditView)
                    setTimeout(function () {
                        _add_invoice.OnCheckBox();
                    }, 1000)
            }
        });
    },
    Validate: function () {
        let result = true
        _add_invoice.ClearError()
        if ($('#client-select').val() == undefined || $('#client-select').val() == null || $('#client-select').val() == '') {
            _add_invoice.DisplayError('validate-client-select', 'Vui lòng chọn khách hàng')
            result = false;
        }
        if ($('#exportDate').val() == undefined || $('#exportDate').val() == null || $('#exportDate').val() == '') {
            _add_invoice.DisplayError('validate-exportDate', 'Vui lòng chọn ngày xuất')
            result = false;
        }
        if ($('#pay-type').val() == undefined || $('#pay-type').val() == null || $('#pay-type').val() == '') {
            _add_invoice.DisplayError('validate-pay_type', 'Vui lòng chọn hình thức thanh toán')
            result = false;
        }
        if ($('#pay-type').val() == '2') {
            if ($('#bankingAccount').val() == undefined || $('#bankingAccount').val() == null || $('#bankingAccount').val() == '') {
                _add_invoice.DisplayError('validate-bankingAccount', 'Vui lòng chọn tài khoản ngân hàng')
                result = false;
            }
        }
        if ($('#number').val() === undefined || $('number').val() === null || $('#number').val() === '') {
            _add_invoice.DisplayError('validate-number', 'Vui lòng nhập số')
            result = false;
        }
        var flag = false
        for (var i = 0; i < listInvoiceRequest.length; i++) {
            var checked = $('#order_ckb_' + listInvoiceRequest[i].id).is(":checked")
            if (checked) {
                flag = true
            }
        }
        if (!flag) {
            _add_invoice.DisplayError('validate-invoice_request', 'Vui lòng tích chọn yêu cầu xuất hóa đơn')
            result = false;
        }
        if ($('#note').val() !== undefined && $('#note').val() !== null
            && $('#note').val() !== '' && ($('#note').val()).length > 3000) {
            _add_invoice.DisplayError('validate-description', 'Vui lòng nhập ghi chú không quá 3000 kí tự')
            result = false;
        }

        if (!result) return false

        return true
    },
    AddInvoice: function (isSend) {
        let validate = _add_invoice.Validate()
        if (!validate)
            return;
        var listDetailInvoice = []
        for (var i = 0; i < listInvoiceRequest.length; i++) {
            var checked = $('#order_ckb_' + listInvoiceRequest[i].id).is(":checked")
            if (checked) {
                listDetailInvoice.push(listInvoiceRequest[i])
            }
        }
        let other_image = [];
        $('#suplier_room_list_image .img_other').each(function () {
            let image_item = $(this);
            other_image.push(image_item.attr('src'));
        });
        let obj = {
            'payType': $('#pay-type').val(),
            'bankingAccountId': $('#bankingAccount').val(),
            'exportDateStr': $('#exportDate').val(),
            'note': $('#note').val(),
            'clientId': parseInt(($('#client-select').val())),
            'OtherImages': other_image,
            'invoiceFromId': $('#denominator').val(),
            'invoiceSignId': $('#symbol').val(),
            'invoiceNo': $('#number').val(),
            'invoiceDetails': listDetailInvoice
        }

        _global_function.AddLoading()
        $.ajax({
            url: "/Invoice/AddJson",
            type: "Post",
            data: obj,
            success: function (result) {
                _global_function.RemoveLoading()
                if (result.isSuccess) {
                    _msgalert.success(result.message);
                    _global_function.ConfirmFileUpload($('.attachment-addnew'), result.id)
                    $.magnificPopup.close();
                    setTimeout(function () { window.location.reload() }, 500)
                } else {
                    _msgalert.error(result.message);
                }
            }
        });
    },
    InitializationEdit: function (client_id, supplier_id) {
        isEditView = true
        if ((client_id == undefined || client_id == null || client_id == 0 || client_id == '')) {
            return
        }
        isEdit = true
        this.GetInvoiceRequestsByClientId(true)
        setTimeout(function () {
            var newOption = new Option($('#client_name_hide').val(), client_id, true, true);
            $('#client-select').append(newOption).trigger('change');
        }, 800)

    },
    GetDetailInvoice: function () {
        _global_function.AddLoading()
        $.ajax({
            url: "/Invoice/GetDetail",
            type: "Post",
            data: { 'invoiceId': $('#requestId').val() },
            success: function (result) {
                _global_function.RemoveLoading()
                var detail = result.data
                var listDetail = detail.invoiceDetails
                if (listDetail == null || listDetail == undefined)
                    listDetail = []
                for (var i = 0; i < listInvoiceRequest.length; i++) {
                    if (listInvoiceRequest[i].invoiceRequestDetails != null && listInvoiceRequest[i].invoiceRequestDetails.length > 0)
                        continue
                    let invoiceRequest = []
                    for (var j = 0; j < listDetail.length; j++) {
                        if (listDetail[j].invoiceRequestId == listInvoiceRequest[i].id) {
                            var obj_request = {
                                index: index,
                                id: listDetail[j].id,
                                productName: listDetail[j].productName,
                                unit: listDetail[j].unit,
                                quantity: listDetail[j].quantity,
                                price: listDetail[j].price,
                                vat: 0,
                                total: listDetail[j].price * listDetail[j].quantity,
                                priceExtraExport: listDetail[j].priceExtraExport,
                                priceExtra: listDetail[j].priceExtra,
                            }
                            invoiceRequest.push(obj_request)
                        }
                    }
                    listInvoiceRequest[i].invoiceRequestDetails = invoiceRequest
                }
                listFileAttach = []
                for (var i = 0; i < listDetail.length; i++) {
                    var obj_request = {
                        index: index,
                        id: listDetail[i].id,
                        productName: listDetail[i].productName,
                        unit: listDetail[i].unit,
                        quantity: listDetail[i].quantity,
                        price: listDetail[i].price,
                        vat: 0,
                        total: listDetail[i].price * listDetail[i].quantity,
                        priceExtraExport: listDetail[i].priceExtraExport,
                        priceExtra: listDetail[i].priceExtra,
                    }
                    index++
                    listInvoiceDetail.push(obj_request)
                    _add_invoice.RenderItemDetail(1)
                }
                if (detail.attachFiles !== null) {
                    for (var i = 0; i < detail.attachFiles.length; i++) {
                        let file = {
                            index: indexFile,
                            name: detail.attachFiles[i],
                        }
                        listFileAttach.push(file)
                        indexFile++
                    }
                    for (var i = 0; i < listFileAttach.length; i++) {
                        $('#divAttachFile').append(
                            "<img src='' alt='' id='attachFile' />" + listFileAttach[i].name +
                            "<a class='blue' style='color: #056BD3 !important; cursor:pointer;margin-left: 20px;' onclick='_add_invoice.OnDeleteImage(" + indexFile + ")'>Xóa</a>"
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
    EditInvoice: function (isSend) {
        let validate = _add_invoice.Validate()
        if (!validate)
            return;
        var listDetailInvoice = []
        for (var i = 0; i < listInvoiceRequest.length; i++) {
            var checked = $('#order_ckb_' + listInvoiceRequest[i].id).is(":checked")
            if (checked) {
                listDetailInvoice.push(listInvoiceRequest[i])
            }
        }
        let other_image = [];
        $('#suplier_room_list_image .img_other').each(function () {
            let image_item = $(this);
            other_image.push(image_item.attr('src'));
        });
        let obj = {
            'id': $('#requestId').val(),
            'payType': $('#pay-type').val(),
            'bankingAccountId': $('#bankingAccount').val(),
            'exportDateStr': $('#exportDate').val(),
            'note': $('#note').val(),
            'clientId': parseInt(($('#client-select').val())),
            'otherImages': other_image,
            'invoiceFromId': $('#denominator').val(),
            'invoiceSignId': $('#symbol').val(),
            'invoiceNo': $('#number').val(),
            'invoiceCode': $('#invoiceCode').val(),
            'invoiceDetails': listDetailInvoice
        }
        _global_function.AddLoading()
        $.ajax({
            url: "/Invoice/Update",
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
    OnCheckBox: function (id) {
        var isChoose = false
        var flag = true
        for (var i = 0; i < listInvoiceRequest.length; i++) {
            if (listInvoiceRequest[i].id != id)
                continue
            var checked = $('#order_ckb_' + listInvoiceRequest[i].id).is(":checked")
            if (checked) {
                if (taxNo === null || taxNo == '') {
                    taxNo = listInvoiceRequest[i].taxNo
                    firstInvoiceRequestNo = listInvoiceRequest[i].invoiceRequestNo
                }
                else
                    if (taxNo !== listInvoiceRequest[i].taxNo) {
                        _msgalert.error('Yêu cầu xuất ' + listInvoiceRequest[i].invoiceRequestNo + ' có thông tin công ty cần xuất hóa đơn không trùng khớp với' +
                            firstInvoiceRequestNo + '. Vui lòng kiểm tra lại.');
                        flag = false
                        break
                    }
                if (companyName === null || companyName == '') {
                    firstInvoiceRequestNo = listInvoiceRequest[i].invoiceRequestNo
                    companyName = listInvoiceRequest[i].companyName
                }
                else
                    if (companyName !== listInvoiceRequest[i].companyName) {
                        _msgalert.error('Yêu cầu xuất ' + listInvoiceRequest[i].invoiceRequestNo + ' có thông tin công ty cần xuất hóa đơn không trùng khớp với ' +
                            firstInvoiceRequestNo + '. Vui lòng kiểm tra lại.');
                        flag = false
                        break
                    }
                if (address === null || address == '') {
                    firstInvoiceRequestNo = listInvoiceRequest[i].invoiceRequestNo
                    address = listInvoiceRequest[i].address
                }
                else
                    if (address !== listInvoiceRequest[i].address) {
                        _msgalert.error('Yêu cầu xuất ' + listInvoiceRequest[i].invoiceRequestNo + ' có thông tin công ty cần xuất hóa đơn không trùng khớp với ' +
                            firstInvoiceRequestNo + '. Vui lòng kiểm tra lại.');
                        break
                        flag = false
                    }
                //if (vat === 0) {
                //    vat = listInvoiceRequest[i].vat
                //    firstInvoiceRequestNo = listInvoiceRequest[i].invoiceRequestNo
                //}
                //else
                //    if (vat !== listInvoiceRequest[i].vat) {
                //        _msgalert.error('Yêu cầu xuất ' + listInvoiceRequest[i].invoiceRequestNo + ' có thuế suất không bằng với ' +
                //            firstInvoiceRequestNo + '. Vui lòng kiểm tra lại.');
                //        flag = false
                //        break
                //    }
            }
        }

        if (flag) {
            $('#taxtNo').val(taxNo)
            $('#companyName').val(companyName)
            $('#address').val(address)
            //$('#VAT').val(vat)
        } else {
            $('#order_ckb_' + id).prop("checked", false)

        }
        listInvoiceDetail = []
        for (var i = 0; i < listInvoiceRequest.length; i++) {
            var checked = $('#order_ckb_' + listInvoiceRequest[i].id).is(":checked")
            if (checked) {
                listInvoiceRequest[i].isChecked = true
                isChoose = true
                for (var j = 0; j < listInvoiceRequest[i].invoiceRequestDetails.length; j++) {
                    var detail = Object.assign({}, listInvoiceRequest[i].invoiceRequestDetails[j])
                    listInvoiceDetail.push(detail)
                }
            } else {
                listInvoiceRequest[i].isChecked = false
            }
        }
        if (!isChoose) {
            taxNo = ''
            companyName = ''
            address = ''
            //vat = 0
        }
        _add_invoice.RenderItemDetail(2)
        setTimeout(_add_invoice.AddItemToInput(), 200)
    },
    CheckOrUnCheckALl: function () {
        var checked = $('#checkAllRequest').is(":checked")
        if (checked)
            _add_invoice.CheckALl()
        else
            _add_invoice.UnCheckALl()
    },
    CheckALl: function () {
        if (listInvoiceRequest.length == 0) {
            $('#checkAllRequest').prop("checked", false)
            return
        }
        var taxNo = ''
        var companyName = ''
        var address = ''
        //var vat = 0
        var flag = true
        //var flagVAT = true
        for (var i = 0; i < listInvoiceRequest.length; i++) {
            if (taxNo == null || taxNo == '') {
                taxNo = listInvoiceRequest[i].taxNo
                companyName = listInvoiceRequest[i].companyName
                address = listInvoiceRequest[i].address
                //vat = listInvoiceRequest[i].vat
                continue
            }
            else {
                if (listInvoiceRequest[i].taxNo !== taxNo || listInvoiceRequest[i].companyName !== companyName
                    || listInvoiceRequest[i].address !== address) {
                    flag = false
                }
                //if (listInvoiceRequest[i].vat !== vat) {
                //    flagVAT = false
                //}
            }
        }
        if (!flag) {
            _msgalert.error('Tất cả yêu cầu xuất có thông tin công ty cần xuất hóa đơn không trùng khớp nhau. Vui lòng kiểm tra lại.');
            $('#checkAllRequest').prop("checked", false)
            return
        }
        //if (!flagVAT) {
        //    _msgalert.error('Tất cả yêu cầu xuất có thuế suất không bằng nhau. Vui lòng kiểm tra lại.');
        //    $('#checkAllRequest').prop("checked", false)
        //    return
        //}
        $("#body_invoice_request_detail_list").empty();
        for (var i = 0; i < listInvoiceRequest.length; i++) {
            $('#order_ckb_' + listInvoiceRequest[i].id).prop("checked", true)

        }
        $('#taxtNo').val(taxNo)
        $('#companyName').val(companyName)
        $('#address').val(address)
        //$('#VAT').val(vat)
        _add_invoice.OnCheckBox(3)
        setTimeout(_add_invoice.AddItemToInput(), 500)
    },
    UnCheckALl: function () {
        $("#body_invoice_request_detail_list").empty();
        for (var i = 0; i < listInvoiceRequest.length; i++) {
            $('#order_ckb_' + listInvoiceRequest[i].id).prop("checked", false)
        }
        $('#taxtNo').val('')
        $('#companyName').val('')
        $('#address').val('')
        //$('#VAT').val(0)
        $('#body_contenxt_request').empty()
        setTimeout(_add_invoice.AddItemToInput(), 500)
    },
    AddToListDetail: function (index) {
    },
    ClearError: function () {
        $(".validate-pay_type").find('p').remove();
        $(".validate-bankingAccount").find('p').remove();
        $(".validate-client-select").find('p').remove();
        $(".validate-denominator").find('p').remove();
        $(".validate-exportDate").find('p').remove();
        $(".validate-symbol").find('p').remove();
        $(".validate-number").find('p').remove();
        $(".validate-note").find('p').remove();
        $(".validate-detail-info").find('p').remove();
        $(".validate-imagefile").find('p').remove();
        $(".validate-invoice_request").find('p').remove();
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
                _add_invoice.DisplayError('validate-imagefile', 'File tải lên không đúng định dạng ảnh (gif, jpeg, png,...)')
                break;
            }

            if (this.validImageSize < file.size) {
                _add_invoice.DisplayError('validate-imagefile', 'Ảnh tải lên vượt quá dung lượng cho phép (10MB).')
                break;
            }

            const reader = new FileReader();
            reader.addEventListener("load", function () {
                let is_exist = grid_image_preview.find(`.image_preview[data-name="${file.name}"]`).length > 0 ? true : false;
                if (!is_exist) {
                    let html = `<div class="col-md-3 col-3 mb10 image_preview" style="min-width: 200px !important;" data-name="${file.name}">
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
                _add_invoice.DisplayError('validate-imagefile',
                    'File đính kèm không đúng định dạng ảnh .png, .jpg, .jpeg, gif. Vui lòng kiểm tra lại')
                break
            }

            if (file.size > (10 * 1024 * 1024)) {
                _add_invoice.DisplayError('validate-imagefile',
                    'File đính kèm ' + fileName + ' không được quá 10MB. Vui lòng kiểm tra lại')
                break
            }
            $('#divAttachFile').append(
                "<img src='' alt='' id='attachFile' />" + fileName +
                "<a class='blue' style='color: #056BD3 !important; cursor:pointer;margin-left: 20px;' onclick='_add_invoice.OnDeleteImage(" + indexFile + ")'>Xóa</a>"
            );
            const reader = new FileReader();
            reader.addEventListener("load", function () {

                let is_exist = grid_image_preview.find(`.image_preview[data-name="${file.name}"]`).length > 0 ? true : false;
                if (!is_exist) {
                    let html = `<div class="col-md-3 col-6 mb10 image_preview" style="min-width: 200px !important; " data-name="${file.name}">
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
                "<a class='blue' style='color: #056BD3 !important; cursor:pointer;' onclick='_add_invoice.OnDeleteImage(" + listFileAttach[i].index + ")'>Xóa</a>"
            );
        }
    },
    RenderItemDetail: function (from) {
        $('#body_contenxt_request').empty()
        var total = 0
        var priceExtraExport = 0
        var priceExtra = 0
        for (var i = 0; i < listInvoiceDetail.length; i++) {
            let id = listInvoiceDetail[i].id
            var disabled = i == 0 ? "disabled" : ""
            $('#table_contenxt_request').find('tbody').append(
                "<tr>" +
                " <td>" + (i + 1) + "</td>" +
                " <td><input type='text' class='form-control ' disabled id='productName_" + id + "'></td>" +
                " <td><input type='text' class='form-control ' disabled id='unit_" + id + "' ></td>" +
                " <td><input type='text' class='form-control text-right' disabled id='quantity_" + id + "' ></td>" +
                " <td><input type='text' class='form-control text-right' disabled id='price_" + id + "' ></td>" +
                " <td><input type='text' class='form-control text-right ' disabled id='total_" + id + "'></td>" +
                "<td><input type='text' class='form-control text-right' disabled id='priceExtraExport_" + id + "'></td>" +
                " <td><input type='text' class='form-control text-right ' disabled id='priceExtra_" + id + "'></td>" +
                " </tr>"
            );
            total += listInvoiceDetail[i].quantity * listInvoiceDetail[i].price
            priceExtraExport += listInvoiceDetail[i].priceExtraExport
            priceExtra += listInvoiceDetail[i].priceExtra
            $('#productName_' + id).val(listInvoiceDetail[i].productName)
            $('#unit_' + id).val(listInvoiceDetail[i].unit)
            $('#quantity_' + id).val(listInvoiceDetail[i].quantity)
            $('#price_' + id).val(_add_invoice.FormatNumberStr(listInvoiceDetail[i].price))
            $('#total_' + id).val(_add_invoice.FormatNumberStr(listInvoiceDetail[i].price * listInvoiceDetail[i].quantity))
            $('#priceExtraExport_' + id).val(_add_invoice.FormatNumberStr(listInvoiceDetail[i].priceExtraExport))
            $('#priceExtra_' + id).val(_add_invoice.FormatNumberStr(listInvoiceDetail[i].priceExtra))
        }
        $('#table_contenxt_request').find('tbody').append(
            "<tr>" +
            "<td></td>" +
            " <td>" +
            " <div class='flex space-between'>" +
            " <strong>Tổng cộng</strong>" +
            " </div>" +
            " </td>" +
            "  <td></td>" +
            "  <td></td>" +
            "  <td></td>" +
            "  <td class='text-right' > <input type='text' class='form-control  text-right' id='totalPrice' value='" + _add_invoice.FormatNumberStr(total) + "' disabled> </td>" +
            "  <td class='text-right' id=''><input type='text' class='form-control  text-right' id='totalPriceExport' value='" + _add_invoice.FormatNumberStr(priceExtraExport) + "' disabled> </td>" +
            "  <td class='text-right' id=''><input type='text' class='form-control  text-right' id='totalPriceExtra' value=' " + _add_invoice.FormatNumberStr(priceExtra) + "' disabled></td>" +
            " </tr>"
        );
        $('#totalPriceBeforeVAT').html(_add_invoice.FormatNumberStr(total + priceExtraExport))
        //var vat = 0
        //if ($('#VAT').val() !== undefined && $('#VAT').val() !== null && $('#VAT').val() !== ''
        //    && parseInt($('#VAT').val()) > 0) {
        //    vat = parseInt($('#VAT').val())
        //}
        //var totalPriceAfterVAT = total + ((vat / 100) * total)
        //$('#totalPriceAfterVAT').html(_add_invoice.FormatNumberStr(totalPriceAfterVAT))
        $('#totalPriceExportAfter').html(_add_invoice.FormatNumberStr(priceExtra))
    },
    OnChangeDetail: function (index) {
        var total = 0
        var totalPriceExtraExport = 0
        var totalPriceExtra = 0
        for (var i = 0; i < listInvoiceDetail.length; i++) {
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
                var vat = 0
                if ($('#VAT').val() !== undefined && $('#VAT').val() !== null && $('#VAT').val() !== ''
                    && parseInt($('#VAT').val()) > 0) {
                    vat = parseInt($('#VAT').val())
                }
                listInvoiceDetail[i].vat = vat
                $('#price_' + index).val(_add_invoice.FormatNumberStr(price))
                $('#total_' + index).val(_add_invoice.FormatNumberStr(price * quantity))
                $('#priceExtraExport_' + index).val(_add_invoice.FormatNumberStr(priceExtraExport))
                $('#priceExtra_' + index).val(_add_invoice.FormatNumberStr(priceExtra))
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
        $('#totalPrice').val(_add_invoice.FormatNumberStr(total))
        $('#totalPriceExport').val(_add_invoice.FormatNumberStr(totalPriceExtraExport))
        $('#totalPriceExtra').val(_add_invoice.FormatNumberStr(totalPriceExtra))
        $('#totalPriceBeforeVAT').html(_add_invoice.FormatNumberStr(total + totalPriceExtraExport))
        //var vat = 0
        //if ($('#VAT').val() !== undefined && $('#VAT').val() !== null && $('#VAT').val() !== ''
        //    && parseInt($('#VAT').val()) > 0) {
        //    vat = parseInt($('#VAT').val())
        //}
        //var totalPriceAfterVAT = total + ((vat / 100) * total)
        //$('#totalPriceAfterVAT').html(_add_invoice.FormatNumberStr(totalPriceAfterVAT))
        $('#totalPriceExportAfter').html(_add_invoice.FormatNumberStr(totalPriceExtra))
    },
    OnChoosePaymentType: function () {
        var pay_type = $('#pay-type').val()
        if (parseInt(pay_type) != 2) { //thanh toán tiền mặt
            $('#bankingAccount').attr('disabled', true)
            $('#lblBankAccountRequired').hide()
            $('#lblBankAccount').show()
            $('#bankingAccount').val(-1)
        } else {
            $('#bankingAccount').attr('disabled', false)
            $('#lblBankAccountRequired').show()
            $('#lblBankAccount').hide()
        }
    },
    FormatDate: function (date) {
        var d = new Date(date),
            month = '' + (d.getMonth() + 1),
            day = '' + d.getDate(),
            year = d.getFullYear();

        if (month.length < 2)
            month = '0' + month;
        if (day.length < 2)
            day = '0' + day;

        return [day, month, year].join('/');
    },
    OnCheckedRequest: function (isClearDate = false) {
        if (listInvoiceRequest.length == 0) return
        if (isClearDate) {
            $('#filter_date_create_daterangepicker').data = null
        }
        var requestChoose = $('#invoiceRequestCode').val()
        let new_id = 0
        if (requestChoose != null) {
            for (var i = 0; i < requestChoose.length; i++) {
                var filter = requestChooseLast.filter(n => n == requestChoose[i])
                if (filter.length == 0)
                    new_id = requestChoose[i]
            }
        }

        requestChooseLast = requestChoose
        let fromCreateDateStr = null
        let toCreateDateStr = null
        if ($('#filter_date_create_daterangepicker').data('daterangepicker') !== undefined &&
            $('#filter_date_create_daterangepicker').data('daterangepicker') != null && !isClearDate) {
            fromCreateDateStr = $('#filter_date_create_daterangepicker').data('daterangepicker').startDate._d.toLocaleDateString("en-GB");
            toCreateDateStr = $('#filter_date_create_daterangepicker').data('daterangepicker').endDate._d.toLocaleDateString("en-GB");
        } else {
            fromCreateDateStr = null
            toCreateDateStr = null
        }
        var listDate = []
        if (fromCreateDateStr != null && toCreateDateStr != null) {
            const [fromday, frommonth, fromyear] = fromCreateDateStr.split('/');
            const [today, tomonth, toyear] = toCreateDateStr.split('/');
            const fromDate = new Date(+fromyear, frommonth - 1, +fromday, 0, 0);
            const toDate = new Date(+toyear, tomonth - 1, +today, 0, 0);
            for (var d = fromDate; d <= toDate; d.setDate(d.getDate() + 1)) {
                listDate.push(_add_invoice.FormatDate(d));
            }
        }
        var invoiceRequestCreate = $('#invoiceRequestCreate').select2("val")
        for (var i = 0; i < listInvoiceRequest.length; i++) {
            if ((requestChoose !== undefined && requestChoose !== null && requestChoose !== '' && requestChoose.includes(listInvoiceRequest[i].id + ''))
                || (listDate.includes(listInvoiceRequest[i].planDateViewStr))
                || (invoiceRequestCreate !== undefined && invoiceRequestCreate !== null && invoiceRequestCreate !== '' && invoiceRequestCreate.includes(listInvoiceRequest[i].createdBy + ''))) {
                listInvoiceRequest[i].isChecked = true
                $('#order_ckb_' + listInvoiceRequest[i].id).prop('checked', true)
            } else {
                listInvoiceRequest[i].isChecked = false
                $('#order_ckb_' + listInvoiceRequest[i].id).prop('checked', false)
            }
        }
        setTimeout(_add_invoice.OnCheckBox(new_id), 300)
    },
    RenderItemChecked: function () {
        $("#body_contenxt_request").empty();
        var totalAmount = 0
        var listChecked = listInvoiceRequest.filter(n => n.isChecked)
        var listUnChecked = listInvoiceRequest.filter(n => !n.isChecked)
        var requestChoose = $('#invoiceRequestCode').val()
        var index = 1
        for (var i = 0; i < listChecked.length; i++) {
            let planDateStr = ""
            var now = new Date()
            if (Date.parse(result.data[i].planDate) < now.setHours(0, 0, 0, 0)) {
                planDateStr += result.data[i].planDateViewStr + "<div style='color: red'>" + result.data[i].paymentDateRemind + "</div>"
            } else {
                planDateStr += result.data[i].planDateViewStr + "<div>" + result.data[i].paymentDateRemind + "</div>"
            }
            $('#request-relate-table').find('tbody').append(
                "<tr id='order_" + i + "'>" +
                "<td>" +
                "<label class='check-list number'>" +
                " <input type='checkbox' name='order_ckb' id='order_ckb_" + result.data[i].id + "' onclick='_add_invoice.OnCheckBox(" + result.data[i].id + ")'>" +
                " <span class='checkmark'></span>" + (i + 1) +
                "  </label>" +
                "</td>" +
                "<td>" +
                " <a class='blue' href='/InvoiceRequest/Detail?invoiceRequestId=" + result.data[i].id + "'> " + result.data[i].invoiceRequestNo + " </a>"
                + "</td>" +
                "<td>" + planDateStr + "</td>" +
                "<td>" + result.data[i].userCreateName + "</td>" +
                "<td class='text-right'>" + _add_invoice.FormatNumberStr(result.data[i].totalPrice) + "</td>" +
                "<td class='text-right'>" + _add_invoice.FormatNumberStr(result.data[i].vatAmount) + "</td>" +
                "<td class='text-right'>" + _add_invoice.FormatNumberStr(result.data[i].totalPriceVAT) + "</td>" +
                "<td class='text-right'>" + _add_invoice.FormatNumberStr(result.data[i].priceExtraExport) + "</td>" +
                "<td class='text-right'>" + _add_invoice.FormatNumberStr(result.data[i].priceExtra) + "</td>" +
                "</tr>"
            );
            totalPrice += result.data[i].totalPrice
            vATAmount += result.data[i].vatAmount
            totalPriceVAT += result.data[i].totalPriceVAT
            priceExtraExport += result.data[i].priceExtraExport
            priceExtra += result.data[i].priceExtra
            let code = result.data[i].id
            $('#order_ckb_' + code).prop('checked', true)
            totalAmount += listChecked[i].amount
            if (listChecked[i].isChecked) {
                let index = i
                let code = listChecked[i].invoiceRequestNo
                setTimeout(function () {
                    $('#order_ckb_' + code).prop('checked', true)
                }, 300)
            }
            index++
        }
        for (var i = 0; i < listChecked.length; i++) {
            if (requestChoose !== undefined && requestChoose !== null && requestChoose !== ''
                && requestChoose.includes(listInvoiceRequest[i].id + '')) {
                continue
            } else {
                if (listChecked[i].isChecked) {
                    let indexCount = i
                    var newOption = new Option(listChecked[indexCount].invoiceRequestNo, listChecked[indexCount].id, true, true);
                    $('#invoiceRequestCode').append(newOption);
                }
            }
        }
        for (var i = 0; i < listUnChecked.length; i++) {
            $('#request-relate-table').find('tbody').append(
                "<tr id='order_" + index + "'>" +
                "<td>" +
                "<label class='check-list number'>" +
                " <input type='checkbox'  id='order_ckb_" + listUnChecked[i].invoiceRequestNo + "' name='order_ckb' onclick='_add_payment_voucher.OnCheckBox(" + i + ");_add_payment_voucher.AddToListDetail(" + i + ")'>" +
                " <span class='checkmark'></span>" + (index
                ) +
                "  </label>"
                + "</td>" +
                "<td>" +
                " <a class='blue' href='/PaymentRequest/Detail?paymentRequestId=" + listUnChecked[i].id + "'> " + listUnChecked[i].invoiceRequestNo + " </a>"
                + "</td>" +
                "<td>" + listUnChecked[i].paymentDateViewStr
                + "</td>" +
                "<td>" + listUnChecked[i].userName + "</td>" +
                "<td>" + _add_payment_voucher.FormatNumberStr(listUnChecked[i].amount) + "</td>" +
                "</tr>"
            );
            index++
        }
        $('#request-relate-table').find('tbody').append(
            "<tr style='font-weight:bold !important;'>" +
            "<td class='text-right' colspan='4'> Tổng </td>" +
            "<td>" + _add_payment_voucher.FormatNumberStr(totalAmount) + "</td>" +
            "</tr>"
        );
        _add_invoice.AddItemToInput()
    },
    AddItemToInput: function () {
        var requestChoose = $('#invoiceRequestCode').val()
        $('#invoiceRequestCode').empty()
        for (var i = 0; i < listInvoiceRequest.length; i++) {
            if (listInvoiceRequest[i].isChecked) {
                requestChoose = $('#invoiceRequestCode').val()
                if (requestChoose !== undefined && requestChoose !== null && requestChoose !== ''
                    && requestChoose.includes(listInvoiceRequest[i].id + '')) {
                    continue
                } else {
                    var newOption = new Option(listInvoiceRequest[i].invoiceRequestNo, listInvoiceRequest[i].id, true, true);
                    $('#invoiceRequestCode').append(newOption)
                }
            }
        }
        //$('#invoiceRequestCode').trigger('change');
    },

}