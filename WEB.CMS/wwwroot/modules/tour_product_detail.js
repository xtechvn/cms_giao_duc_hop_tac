var _tour_product_detail = {
    Init: function () {
        this.validImageTypes = ['image/gif', 'image/jpeg', 'image/png'];
        this.validImageSize = 5 * 1024 * 1024;
        this.noImageSrc = "/images/icons/noimage.png";
        _tour_product_detail.GetTourProductPrices()
    },

    GetFormData: function ($form) {
        var unindexed_array = $form.serializeArray();
        var indexed_array = {};

        $.map(unindexed_array, function (n, i) {
            indexed_array[n['name']] = n['value'];
        });

        return indexed_array;
    },

    OnSave: function (status = 0) {
        let Form = $('#form_tour_product');

        if (status == 7) {
            Form.validate({
                rules: {
                    TourName: "required",
                },
                messages: {
                    TourName: "Vui lòng nhập thông tin",
                }
            });
        } else {
            Form.validate({
                rules: {
                    TourName: "required",
                    Days: "required",
                    SupplierId: "required",
                    TourType: "required",
                    OrganizingType: "required",
                    Price: "required",
                    Star: "required",
                    StartPoint: "required",
                    DateDeparture:"required"
                },
                messages: {
                    TourName: "Vui lòng nhập thông tin",
                    Days: "Vui lòng chọn số ngày",
                    SupplierId: "Vui lòng chọn nhà cung cấp",
                    TourType: "Vui lòng chọn loại tour",
                    OrganizingType: "Vui lòng chọn hình thức tour",
                    Price: "Vui lòng nhập giá gốc",
                    Star: "Vui lòng chọn hạng sao",
                    StartPoint: "Vui lòng chọn điểm đi",
                    DateDeparture: "Vui lòng nhập lịch khởi hành",
                }
            });
        }

        if (!Form.valid()) {
            _msgalert.error("Vui lòng nhập chính xác thông tin tour");
            return;
        }

        let formData = this.GetFormData(Form);

        let avatar = $('#avatar_image').attr('src');
        formData.Avatar = (avatar != null && avatar != "") ? avatar : "";

        if ((formData.Avatar == null || formData.Avatar == "") && status == 0) {
            _msgalert.error("Vui lòng chọn và tải ảnh đại diện");
            return;
        }

        let other_image = [];
        $('#grid_image_preview .img_other').each(function () {
            let image_item = $(this);
            other_image.push(image_item.attr('src'));
        });
        formData.OtherImages = other_image;

        if (other_image.length <= 0 && status == 0) {
            _msgalert.error("Vui lòng chọn và tải ảnh mô tả chi tiết ở tab ảnh để đăng bài");
            return;
        }

        let transport_type = [];
        $('.ckb_transport_type:checked').each(function () {
            let seft = $(this);
            transport_type.push(seft.val());
        });
        if (transport_type.length <= 0 && status == 0) {
            _msgalert.error("Vui lòng check chọn ít nhất một loại phương tiện di chuyển ");
            return;
        }
        formData.Transportation = transport_type.join(',');

        let schedule = []
        let schedule_element = $('#block_insert_day_description .form-group');
        let valid_day_description = true;
        if (schedule_element.length > 0) {
            let day = 1;
            schedule_element.each(function () {
                let item_schedule = $(this);
                let title = item_schedule.find('.day_title').val();
                let description = tinyMCE.get(`day_${day}_description`).getContent();

                if (!title || title == null || title == "") {
                    valid_day_description = false;
                }

                schedule.push({
                    day_num: day,
                    day_title: title,
                    day_description: description
                });

                day++;
            });
        }

        if (valid_day_description) {
            formData.Schedule = JSON.stringify(schedule);
        } else {
            if (status == 0) {
                _msgalert.error("Vui lòng nhập đẩy đủ thông tin lịch trình các ngày");
                return;
            }
        }

        let endPoints = $('#EndPoints').val();
        if ((!endPoints || endPoints == null) && status == 0) {
            _msgalert.error("Vui lòng chọn điểm đến");
            return;
        }

        formData.EndPoints = endPoints;
        formData.Description = tinyMCE.get(`Description`).getContent();
        formData.Include = tinyMCE.get(`Include`).getContent();
        formData.Exclude = tinyMCE.get(`Exclude`).getContent();
        formData.Refund = tinyMCE.get(`Refund`).getContent();
        formData.Surcharge = tinyMCE.get(`Surcharge`).getContent();
        formData.Note = tinyMCE.get(`Note`).getContent();
        formData.OldPrice = isNaN(formData.OldPrice) ? ConvertMoneyToNumber(formData.OldPrice) : null;
        formData.Price = isNaN(formData.Price) ? ConvertMoneyToNumber(formData.Price) : 0;
        formData.Status = status;

        var b2c = $('#PositionB2C :checked').text();
        var b2B = $('#PositionB2B :checked').text();
        if (parseFloat(formData.PositionB2B) != 0 && parseFloat(formData.PositionB2B) != parseFloat(b2B)) {
            formData.PositionB2B = b2B
        }
        if (parseFloat(formData.PositionB2C) != 0 && parseFloat(formData.PositionB2C) != parseFloat(b2c)) {
            formData.PositionB2C = b2c
        }

        //if (formData.Price <= 0 && status == 0) {
        //    _msgalert.error("Giá gốc Tour phải lớn hơn 0");
        //    return;
        //}

        let url = '/TourProduct/UpsertTourProduct';

        console.log(formData);
        _ajax_caller.post(url, formData, function (result) {
            if (result.isSuccess) {
                _msgalert.success(result.message);
                _tour_product_detail.UpSertProductPrices(result.productID)

                window.location.href = "/TourProduct";
            } else {
                _msgalert.error(result.message);
            }
        });
    },

    OnChangeAvartar: function () {
        const preview = document.querySelector('.img-preview');
        const file = document.querySelector('input[name=ImgAvatar]').files[0];
        const fileType = file['type'];

        if (!this.validImageTypes.includes(fileType)) {
            _msgalert.error("File tải lên không đúng định dạng ảnh(gif, jpeg, png)");
            return false;
        }

        if (this.validImageSize < file.size) {
            _msgalert.error("Ảnh tải lên phải có dung lượng bé hơn hoặc bằng 5MB.");
            return false;
        }

        const reader = new FileReader();
        reader.addEventListener("load", function () {
            preview.src = reader.result;
            $('#avatar_image').removeClass('mfp-hide');
            $('#ava_upload').addClass('mfp-hide');
        }, false);
        if (file) {
            reader.readAsDataURL(file);
        }
    },

    OnChangeDateCount: function (num) {
        let grid_appending = $('#block_insert_day_description');
        let html = '';
        if (num > 0) {
            for (let i = 1; i <= num; i++) {
                let ele_id = `day_${i}_description`;
                html += `
                <div class="form-group col-12">
                    <label class="lbl">Ngày ${i} <sup class="red">*</sup></label>
                    <div class="wrap_input mb10">
                        <input class="form-control day_title" placeholder="Tiêu đề" />
                    </div>
                    <div class="wrap_input">
                        <textarea class="form-control day_description" id="${ele_id}" style="resize:none;"></textarea>
                    </div>
                </div>`;
            }
        }

        grid_appending.html(html);

        if ($('.day_description').length > 0) {
            tinymce.remove(`.day_description`);
            _common.tinyMce(`.day_description`, 300);
        }

    },

    OnAddImage: function () {
        const files = document.querySelector('input[name=ImageData]').files;
        let grid_image_preview = $('#grid_image_preview');

        for (let file of files) {

            if (!this.validImageTypes.includes(file['type'])) {
                _msgalert.error("File tải lên không đúng định dạng ảnh (gif, jpeg, png,...)");
                break;
            }

            if (this.validImageSize < file.size) {
                _msgalert.error("Ảnh tải lên vượt quá dung lượng cho phép (5MB).");
                break;
            }

            const reader = new FileReader();
            reader.addEventListener("load", function () {

                let is_exist = grid_image_preview.find(`.image_preview[data-name="${file.name}"]`).length > 0 ? true : false;
                if (!is_exist) {
                    let html = `<div class="col-md-2 col-6 mb10 image_preview" data-name="${file.name}">
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
        }
    },
    HTML: {
        HTMLOptions:``,
        PriceItem:`  <tr class="tour-product-price-tr" data-id="{data-id}">
                                            <td>
                                                <div class="d-flex align-center td-d-flex tour-product-price-id">
                                                    {id}
                                                </div>
                                            </td>
                                            <td>
                                                <div class="d-flex align-center td-d-flex">
                                                    <select class="select2 tour-product-price-item-clienttypes tour-product-price-item-clienttypes-new"  style="width: 100%;">
                                                        {client_type}
                                                    </select>
                                                </div>

                                            </td>
                                            <td>
                                                <div class="d-flex align-center td-d-flex">
                                                    <input class="form-control tour-product-price-item-fromdate tour-product-price-item-fromdate-new"  type="text" name="tour-product-price-item-fromdate" value="{fromdate}">
                                                </div>
                                            </td>
                                          <td>
                                                <div class="d-flex align-center td-d-flex">
                                                    <input class="form-control tour-product-price-item-todate tour-product-price-item-todate-new"  type="text" name="tour-product-price-item-todate" value="{todate}">
                                                </div>
                                            </td>

                                            <td class="txt_14">
                                                <div class="d-flex align-center td-d-flex">
                                                    <input type="checkbox" class="tour-product-price-item-isdaily" name="isdaily" {is_daily}  value="0">
                                                </div>
                                            </td>
                                            <td>
                                                <div class="d-flex align-center td-d-flex">
                                                    <input class="form-control currency tour-product-price-item-adult-price" type="text" name="tour-product-price-item-adult-price"  value="{adult-price}">
                                                </div>
                                            </td>
                                            <td>
                                                <div class="d-flex align-center td-d-flex">
                                                    <input class="form-control currency tour-product-price-item-child-price" type="text" name="tour-product-price-item-child-price"  value="{child-price}">
                                                </div>
                                            </td>
                                            <td>
                                                <div class="d-flex align-center td-d-flex">
                                                    {create_date}

                                                </div>
                                            </td>
                                            <td>
                                                <div class="d-flex align-center td-d-flex">

                                                    {update_date}
                                                </div>
                                            </td>
                                            <td>
                                                <div class="d-flex align-center td-d-flex" style=" font-size: 25px; ">
                                                   <a href="javascript:;" class="fa fa-plus-circle green tour-product-price-item-add" style="{display_readonly}"></a>
                                                    <a href="javascript:;" class="fa fa-pencil ml-2 green tour-product-price-item-edit"style="{display_readonly}"></a>
                                                    <a href="javascript:;" class="fa fa-check ml-2 green tour-product-price-item-confirm"style="{display_editmode}"></a>
                                                    <a href="javascript:;" class="fa fa-trash ml-1 red tour-product-price-item-delete"></a>
                                                    <img src="/images/icons/loading.gif" style="width:25px;height:25px; display:none;" class="coll tour-product-price-item-loading">

                                                </div>
                                            </td>`,
        PriceItemDisabled: `  <tr class="tour-product-price-tr" data-id="{data-id}">
                                            <td>
                                                <div class="d-flex align-center td-d-flex tour-product-price-id">
                                                    {id}
                                                </div>
                                            </td>
                                            <td>
                                                <div class="d-flex align-center td-d-flex">
                                                    <select disabled class="tour-product-price-item-clienttypes tour-product-price-item-clienttypes-new"  style="width: 100%;">
                                                        {client_type}
                                                    </select>
                                                </div>

                                            </td>
                                            <td>
                                                <div class="d-flex align-center td-d-flex">
                                                    <input class="form-control tour-product-price-item-fromdate tour-product-price-item-fromdate-new" disabled  type="text" name="tour-product-price-item-fromdate" value="{fromdate}">
                                                </div>
                                            </td>
                                          <td>
                                                <div class="d-flex align-center td-d-flex">
                                                    <input class="form-control tour-product-price-item-todate tour-product-price-item-todate-new" disabled type="text" name="tour-product-price-item-todate" value="{todate}">
                                                </div>
                                            </td>

                                            <td class="txt_14">
                                                <div class="d-flex align-center td-d-flex">
                                                    <input type="checkbox" class="tour-product-price-item-isdaily" disabled name="isdaily" {is_daily}  value="0">
                                                </div>
                                            </td>
                                            <td>
                                                <div class="d-flex align-center td-d-flex">
                                                    <input class="form-control currency tour-product-price-item-adult-price" disabled type="text" name="tour-product-price-item-adult-price"  value="{adult-price}">
                                                </div>
                                            </td>
                                            <td>
                                                <div class="d-flex align-center td-d-flex">
                                                    <input class="form-control currency tour-product-price-item-child-price" disabled type="text" name="tour-product-price-item-child-price"  value="{child-price}">
                                                </div>
                                            </td>
                                            <td>
                                                <div class="d-flex align-center td-d-flex">
                                                    {create_date}

                                                </div>
                                            </td>
                                            <td>
                                                <div class="d-flex align-center td-d-flex">

                                                    {update_date}
                                                </div>
                                            </td>
                                            <td>
                                                <div class="d-flex align-center td-d-flex" style=" font-size: 25px; ">
                                                   <a href="javascript:;" class="fa fa-plus-circle green tour-product-price-item-add" style="{display_readonly}"></a>
                                                    <a href="javascript:;" class="fa fa-pencil ml-2 green tour-product-price-item-edit"style="{display_readonly}"></a>
                                                    <a href="javascript:;" class="fa fa-check ml-2 green tour-product-price-item-confirm"style="{display_editmode}"></a>
                                                    <a href="javascript:;" class="fa fa-trash ml-1 red tour-product-price-item-delete"></a>
                                                    <img src="/images/icons/loading.gif" style="width:25px;height:25px; display:none;" class="coll tour-product-price-item-loading">
                                                </div>
                                            </td>`,
    },
    GetTourProductPrices: function () {
        $('#tour-product-price').html('');

        _ajax_caller.post('/TourProduct/GetTourProgramPackages', { tour_product_id: $('#Id').val() }, function (result) {
            let html_result = ''
            if (result) {
                if (result.client_types && result.client_types.length > 0) {
                    result.client_types.map((item) => {
                        _tour_product_detail.HTML.HTMLOptions += `<option value="${item.codeValue}" ${item.codeValue==5?'selected':''} >${item.description}</option>`
                    });
                }
                if (result.data && result.data.length > 0) {
                    result.data.map((item) => {
                        
                        html_result += _tour_product_detail.HTML.PriceItemDisabled.replaceAll('{id}', (_tour_product_detail.GetTourPriceID() + 1))
                            .replaceAll('{data-id}', item.id)
                            .replaceAll('{client_type}', _tour_product_detail.HTML.HTMLOptions)
                            .replaceAll('{fromdate}', _global_function.DateDotNETToDatePicker(new Date(item.fromDate)))
                            .replaceAll('{todate}', _global_function.DateDotNETToDatePicker(new Date(item.toDate)))
                            .replaceAll('{is_daily}', item.isDaily? 'checked="checked"':'')
                            .replaceAll('{adult-price}', _global_function.Comma(item.adultPrice))
                            .replaceAll('{child-price}', _global_function.Comma(item.childPrice))
                            .replaceAll('{create_date}', _global_function.DateDotNETToDatePicker(new Date(item.createdDate)))
                            .replaceAll('{update_date}', _global_function.DateDotNETToDatePicker(new Date(item.updatedDate)))
                            .replaceAll('{display_readonly}', '')
                            .replaceAll('{display_editmode}', 'display:none;')
                    });

                }
            }
           
            $('#tour-product-price').html(html_result);
            $('.tour-product-price-item-clienttypes-new').each(function (index, item) {
                var element = $(this)
                _tour_product_detail.Select2WithFixedOptionAndNoSearch(element)
                element.removeClass('tour-product-price-item-clienttypes-new')
            });
            $('.tour-product-price-item-fromdate-new').each(function (index, item) {
                var element = $(this)
                _tour_product_detail.SingleDatePickerFromNow(element)
                element.removeClass('tour-product-price-item-fromdate-new')

            });
            $('.tour-product-price-item-todate-new').each(function (index, item) {
                var element = $(this)
                _tour_product_detail.SingleDatePickerFromNow(element)
                element.removeClass('tour-product-price-item-todate-new')

            });
        });
    },
    GetTourPriceID: function () {
        var id = 0;
        $('.tour-product-price-id').each(function (index, item) {
           id++
        });
        return id
    },
    AddNewProductPrice: function () {
        var new_id = _tour_product_detail.GetTourPriceID() + 1
        var html = _tour_product_detail.HTML.PriceItem.replaceAll('{id}', new_id)
            .replaceAll('{data-id}', '0')
            .replaceAll('{client_type}', _tour_product_detail.HTML.HTMLOptions)
            .replaceAll('{fromdate}', '')
            .replaceAll('{todate}', '')
            .replaceAll('{is_daily}',  '')
            .replaceAll('{adult-price}','0')
            .replaceAll('{child-price}', '0')
            .replaceAll('{create_date}', '')
            .replaceAll('{update_date}', '')
            .replaceAll('{display_readonly}', 'display:none;' )
            .replaceAll('{display_editmode}', '')
        $('#tour-product-price').append(html)
      
        $('.tour-product-price-item-clienttypes-new').each(function (index, item) {
            var element = $(this)
            _tour_product_detail.Select2WithFixedOptionAndNoSearch(element)
            element.removeClass('tour-product-price-item-clienttypes-new')
        });
        $('.tour-product-price-item-fromdate-new').each(function (index, item) {
            var element = $(this)
            _tour_product_detail.SingleDatePickerFromNow(element)
            element.removeClass('tour-product-price-item-fromdate-new')

        });
        $('.tour-product-price-item-todate-new').each(function (index, item) {
            var element = $(this)
            _tour_product_detail.SingleDatePickerFromNow(element)
            element.removeClass('tour-product-price-item-todate-new')

        });
    },
    EditProductPrice: function (tr) {
        tr.find('.tour-product-price-item-clienttypes').prop("disabled", false)
        tr.find('.tour-product-price-item-isdaily').prop("disabled", false)
        tr.find('.tour-product-price-item-fromdate').prop("disabled", false)
        tr.find('.tour-product-price-item-todate').prop("disabled", false)
        tr.find('.tour-product-price-item-adult-price').prop("disabled", false)
        tr.find('.tour-product-price-item-child-price').prop("disabled", false)
        tr.find('.tour-product-price-item-add').hide()
        tr.find('.tour-product-price-item-edit').hide()
        tr.find('.tour-product-price-item-confirm').show()
        tr.find('.tour-product-price-item-loading').hide()
    },
    ConfirmProductPrice: function (tr) {
        tr.find('.tour-product-price-item-clienttypes').prop("disabled", true)
        tr.find('.tour-product-price-item-isdaily').prop("disabled", true)
        tr.find('.tour-product-price-item-fromdate').prop("disabled", true)
        tr.find('.tour-product-price-item-todate').prop("disabled", true)
        tr.find('.tour-product-price-item-adult-price').prop("disabled", true)
        tr.find('.tour-product-price-item-child-price').prop("disabled", true)
        tr.find('.tour-product-price-item-add').show()
        tr.find('.tour-product-price-item-edit').show()
        tr.find('.tour-product-price-item-confirm').hide()
        tr.find('.tour-product-price-item-loading').hide()
    },
   
    UpSertProductPrices: function (ProductID, notification = false, parent = $('#tour-product-price').find('.tour-product-price-tr')) {
        var obj = []
        var tour_product_id = ProductID
        parent.each(function (index, item) {
            var element=$(this)
            var item = {
                Id: element.attr('data-id'),
                TourProductId: tour_product_id,
                FromDate: _global_function.GetDayText(element.find('.tour-product-price-item-fromdate').data('daterangepicker').startDate._d,true),
                ToDate: _global_function.GetDayText(element.find('.tour-product-price-item-todate').data('daterangepicker').startDate._d, true),
                IsDaily: element.find('.tour-product-price-item-isdaily').is(":checked") ? true : false,
                AdultPrice: element.find('.tour-product-price-item-adult-price').val().replaceAll(',',''),
                ChildPrice: element.find('.tour-product-price-item-child-price').val().replaceAll(',', ''),
                ClientType: element.find('.tour-product-price-item-clienttypes').find(':selected').val(),

            }
            obj.push(item)
        });
        _ajax_caller.post('/TourProduct/UpSertProductPrices', { model_upload: obj, tour_product_id: tour_product_id }, function (result) {
            if (notification && result) {
                if (result.isSuccess) {
                    _msgalert.success(result.message)

                }
                else {
                    _msgalert.error(result.message)
                }
            }

        })
    },
    DeleteProductPrices: function (tr) {
        if (tr.attr('data-id') != undefined && !isNaN(parseInt(tr.attr('data-id'))) && parseInt(tr.attr('data-id')) > 0) {
            _ajax_caller.post('/TourProduct/DeleteProductPrice', { price_id:  tr.attr('data-id') }, function (result) {
                 if (result.isSuccess) {
                        _msgalert.success(result.message)

                    }
                    else {
                        _msgalert.error(result.message)
                    }
            })
        }
        else {
            tr.remove()
        }
       
    },
    Select2WithFixedOptionAndNoSearch: function (element) {
        var placeholder = element.attr('placeholder')
        element.select2({
            placeholder: placeholder,
            minimumResultsForSearch: Infinity
        });
    },
    SingleDatePickerFromNow: function (element, dropdown_position = 'down') {

        var today = new Date();
        var yyyy = today.getFullYear();
        var mm = today.getMonth() + 1; // Months start at 0!
        var dd = today.getDate();
        var yyyy_max = yyyy + 5;
        if (dd < 10) dd = '0' + dd;
        if (mm < 10) mm = '0' + mm;
        var time_now = dd + '/' + mm + '/' + yyyy /*+ ' ' + ("0" + today.getHours()).slice(-2) + ':' + ("0" + today.getMinutes()).slice(-2);*/
        var max_range = '31/12/' + yyyy_max /*+ ' 23:59'*/;

        element.daterangepicker({
            singleDatePicker: true,
            autoApply: true,
            showDropdowns: true,
            drops: dropdown_position,
            minDate: time_now,
            maxDate: max_range,
            drops: 'up',
            locale: {
                format: 'DD/MM/YYYY '/*HH:mm*/
            }
        }, function (start, end, label) {


        });
    },
}
$('body').on('click', '.tour-product-price-item-add', function () {
    let seft = $(this);
    _tour_product_detail.AddNewProductPrice()
});
$('body').on('click', '.tour-product-price-item-edit', function () {
    let seft = $(this);
    var tr = seft.closest('tr')
    _tour_product_detail.EditProductPrice(tr)
});
$('body').on('click', '.tour-product-price-item-confirm', function () {
    let seft = $(this);
    var tr = seft.closest('tr')
    _tour_product_detail.ConfirmProductPrice(tr)
    if ($('#Id').val() != undefined && !isNaN(parseInt($('#Id').val())) && parseInt($('#Id').val())>0) {
        _tour_product_detail.UpSertProductPrices($('#Id').val(),true, tr)
    }
});
$('body').on('click', '.tour-product-price-item-delete', function () {
    let seft = $(this);
    var tr = seft.closest('tr')
    if ($('#Id').val() != undefined && !isNaN(parseInt($('#Id').val())) && parseInt($('#Id').val()) > 0) {
        _msgconfirm.openDialog('Xóa lịch trình Tour', 'Lịch trình Tour này sẽ bị xóa, bạn có chắc chắn không', function () {
            _tour_product_detail.DeleteProductPrices(tr)

        });
    } else {
        _tour_product_detail.DeleteProductPrices(tr)

    }
});
$('body').on('change', '#form_tour_product #Days', function () {
    let seft = $(this);
    let value = seft.val() != "" ? seft.val() : 0;
    _tour_product_detail.OnChangeDateCount(value);
});

$('#tab-thongtin #TourType').change(function () {
    let type = $(this).val();
    let endpoint_element = $('#tab-thongtin #EndPoints');

    let url = '/TourProduct/GetEnpointListByType';
    _ajax_caller.get(url, { type: type }, function (result) {

        if (result && result.length > 0) {
            let html_result = "";
            result.map((item) => {
                html_result += `<option value="${item.id}">${item.name}</option>`
            });

            endpoint_element.html(html_result);
            endpoint_element.select2();
        }
    });
});

$(document).ready(function () {
    $('input').attr('autocomplete', 'off');
    _tour_product_detail.Init();

    $('.select2_modal').select2();

    if ($('.day_description').length > 0) {
        _common.tinyMce(`.day_description`, 300);
    }

    _common.tinyMce(`#Description`, 300);
    _common.tinyMce(`#Description`, 300);
    _common.tinyMce(`#Include`, 300);
    _common.tinyMce(`#Exclude`, 300);
    _common.tinyMce(`#Refund`, 300);
    _common.tinyMce(`#Surcharge`, 300);
    _common.tinyMce(`#Note`, 300);

    $("#SupplierId").select2({
        //theme: 'bootstrap4',
        placeholder: "Nhà cung cấp",
        ajax: {
            url: "/Supplier/Suggest",
            type: "get",
            dataType: 'json',
            delay: 250,
            data: function (params) {
                var query = {
                    text: params.term,
                    size: 10
                }
                return query;
            },
            processResults: function (data) {
                var data = {
                    results: $.map(data, function (item) {
                        return {
                            text: item.name,
                            id: item.id,
                        }
                    })
                }
                return data;
            }
        }
    });
});
