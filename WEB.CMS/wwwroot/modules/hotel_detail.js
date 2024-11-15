var hotel_contact = {
    Init: function () {
        this.modal = $('#global_modal_popup');
        this.search_params = {
            hotel_id: $('#global_hotel_id').val()
        }
        this.ReloadListing(this.search_params);
    },

    GetFormData: function ($form) {
        var unindexed_array = $form.serializeArray();
        var indexed_array = {};

        $.map(unindexed_array, function (n, i) {
            indexed_array[n['name']] = n['value'];
        });

        return indexed_array;
    },

    OnAddOrUpdate: function (id) {
        let title = `${id > 0 ? "Cập nhật" : "Thêm mới"} liên hệ`;
        let url = '/Hotel/ContactUpsert';

        this.modal.find('.modal-title').html(title);
        this.modal.find('.modal-dialog').css('max-width', '600px');

        _ajax_caller.get(url, { id: id, hotel_id: this.search_params.hotel_id }, function (result) {
            hotel_contact.modal.find('.modal-title').html(title);
            hotel_contact.modal.find('.modal-body').html(result);
            hotel_contact.modal.modal('show');
        });
    },

    Upsert: function () {
        let url = '/Hotel/ContactUpsert';
        let Form = $('#form_hotel_contact');
        Form.validate({
            rules: {
                Name: "required",
                Mobile: {
                    required: true,
                    minlength: 10,
                    maxlength: 11,
                    digits: true
                },
                Email: {
                    email: true
                },
            },
            messages: {
                Name: "Vui lòng nhập tên liên hệ",
                Mobile: {
                    required: "Vui lòng nhập số điện thoại",
                    //exactlength: "Số điện thoại phải nhập đúng 10 kí tự dạng số",
                    digits: "Số điện thoại phải là kí tự dạng số"
                },
                Email: {
                    email: 'Email không đúng định dạng'
                }
            }
        });

        if (!Form.valid()) return;

        let formData = this.GetFormData(Form);

        _ajax_caller.post(url, { model: formData }, function (result) {
            if (result.isSuccess) {
                _msgalert.success(result.message);
                hotel_contact.modal.modal('hide');
                hotel_contact.ReloadListing();
            } else {
                _msgalert.error(result.message);
            }
        });
    },

    Delete: function (id) {
        let url = '/hotel/ContactDelete';
        let title = 'Xác nhận xóa liên hệ';
        let description = 'Bạn có chắc chắn muốn thông tin?';
        _msgconfirm.openDialog(title, description, function () {
            _ajax_caller.post(url, { id: id }, function (result) {
                if (result.isSuccess) {
                    _msgalert.success(result.message);
                    hotel_contact.ReloadListing();
                } else {
                    _msgalert.error(result.message);
                }
            });
        });
    },

    Listing: function (input) {
        _ajax_caller.post("/hotel/ContactListing", input, function (result) {
            $('#grid_hotel_contact').html(result);
        });
    },

    ReloadListing: function () {
        //this.search_params.page_index = 1;
        this.Listing(this.search_params);
    },

    Paging: function (input) {
        this.search_params.page_index = input;
        this.Listing(this.search_params);
    }
}

var hotel_banking_account = {
    Init: function () {
        this.modal = $('#global_modal_popup');
        this.search_params = {
            hotel_id: $('#global_hotel_id').val()
        }
        this.ReloadListing(this.search_params);
    },

    GetFormData: function ($form) {
        var unindexed_array = $form.serializeArray();
        var indexed_array = {};

        $.map(unindexed_array, function (n, i) {
            indexed_array[n['name']] = n['value'];
        });

        return indexed_array;
    },

    OnAddOrUpdate: function (id) {
        let title = `${id > 0 ? "Cập nhật" : "Thêm mới"} thông tin thanh toán`;
        let url = '/Hotel/BankingAccountUpsert';

        this.modal.find('.modal-title').html(title);
        this.modal.find('.modal-dialog').css('max-width', '600px');

        _ajax_caller.get(url, { id: id, hotel_id: this.search_params.hotel_id }, function (result) {
            hotel_banking_account.modal.find('.modal-title').html(title);
            hotel_banking_account.modal.find('.modal-body').html(result);
            hotel_banking_account.modal.modal('show');
        });
    },

    Upsert: function () {
        let url = '/Hotel/BankingAccountUpsert';
        let Form = $('#form_hotel_banking_account');
        Form.validate({
            rules: {
                AccountName: "required",
                AccountNumber: {
                    required: true,
                    maxlength: 20,
                    digits: true
                },
                BankId: "required"
            },
            messages: {
                AccountName: "Vui lòng nhập chủ tài khoản ngân hàng",
                AccountNumber: {
                    required: "Vui lòng nhập số tài khoản",
                    maxlength: "Số tài khoản chỉ chứa tối đa 20 kí tự",
                    digits: "Số tài khoản phải là kí tự dạng số"
                },
                BankId: "Vui lòng nhập tên ngân hàng"
            }
        });

        if (!Form.valid()) return;

        let formData = this.GetFormData(Form);

        _ajax_caller.post(url, { model: formData }, function (result) {
            if (result.isSuccess) {
                _msgalert.success(result.message);
                hotel_banking_account.modal.modal('hide');
                hotel_banking_account.ReloadListing();
            } else {
                _msgalert.error(result.message);
            }
        });
    },

    Delete: function (id) {
        let url = '/Hotel/BankingAccountDelete';
        let title = 'Xác nhận xóa liên hệ';
        let description = 'Bạn có chắc chắn muốn thông tin?';
        _msgconfirm.openDialog(title, description, function () {
            _ajax_caller.post(url, { id: id }, function (result) {
                if (result.isSuccess) {
                    _msgalert.success(result.message);
                    hotel_banking_account.ReloadListing();
                } else {
                    _msgalert.error(result.message);
                }
            });
        });
    },

    Listing: function (input) {
        _ajax_caller.post("/Hotel/BankingAccountListing", input, function (result) {
            $('#grid_hotel_banking_account').html(result);
        });
    },

    ReloadListing: function () {
        //this.search_params.page_index = 1;
        this.Listing(this.search_params);
    },

    Paging: function (input) {
        this.search_params.page_index = input;
        this.Listing(this.search_params);
    }
}

var supplier_program = {
    Init: function () {
        this.modal = $('#global_modal_popup');
        this._ChangeIntervalName = null;
        this.search_params = {
            SupplierID: $('#global_supplier_id').val(),
            ServiceName: $('#global_ServiceName').val(),
            ProgramCode: null,
            ServiceType: "1",
            PageIndex: 1,
            PageSize: 10
        }
        this.ReloadListing(this.search_params);
    },

    Listing: function (input) {
        _ajax_caller.post("/Programs/SearchProgramsBySupplierId", input, function (result) {
            $('#grid_supplier_program').html(result);
        });
    },

    ReloadListing: function () {
        this.search_params.PageIndex = 1;
        this.Listing(this.search_params);
    },

    Paging: function (input) {
        this.search_params.PageIndex = input;
        this.Listing(this.search_params);
    }
}

var hotel_surcharge = {
    Init: function () {
        this.modal = $('#global_modal_popup');
        this.search_params = {
            hotel_id: $('#global_hotel_id').val(),
            page_index: 1,
            page_size: 100
        }
        this.ReloadListing(this.search_params);
        _common.tinyMce('#text-editor');

    },

    GetFormData: function ($form) {
        var unindexed_array = $form.serializeArray();
        var indexed_array = {};

        $.map(unindexed_array, function (n, i) {
            indexed_array[n['name']] = n['value'];
        });

        return indexed_array;
    },

    OnAddOrUpdate: function (id) {
        let title = `${id > 0 ? "Cập nhật" : "Thêm mới"} giá phụ thu`;
        let url = '/Hotel/SurchargeUpsert';

        this.modal.find('.modal-title').html(title);
        this.modal.find('.modal-dialog').css('max-width', '600px');

        _ajax_caller.get(url, { id: id, hotel_id: this.search_params.hotel_id }, function (result) {
            hotel_surcharge.modal.find('.modal-title').html(title);
            hotel_surcharge.modal.find('.modal-body').html(result);
            hotel_surcharge.modal.modal('show');
            hotel_surcharge.SingleDatePickerFromNow($('#hotel-surcharge-from-date'))
            hotel_surcharge.SingleDatePickerFromNow($('#hotel-surcharge-to-date'))
        });
    },

    Upsert: function () {
        let url = '/Hotel/SurchargeUpsert';
        let Form = $('#form_hotel_surcharge');
        Form.validate({
            rules: {
                Name: "required",
                //Code: "required"
            },
            messages: {
                Name: "Vui lòng nhập tên phụ thu",
                //Code: "Vui lòng nhập mã phụ thu"
            }
        });

        if (!Form.valid()) return;

        let formData = this.GetFormData(Form);

        console.log(formData);
        formData.FromDate = hotel_surcharge.GetDayText($('#hotel-surcharge-from-date').data('daterangepicker').startDate._d,true);
        formData.ToDate = hotel_surcharge.GetDayText($('#hotel-surcharge-to-date').data('daterangepicker').startDate._d, true);

        _ajax_caller.post(url, { model: formData }, function (result) {
            if (result.isSuccess) {
                _msgalert.success(result.message);
                hotel_surcharge.modal.modal('hide');
                hotel_surcharge.ReloadListing();
            } else {
                _msgalert.error(result.message);
            }
        });
    },

    Delete: function (id) {
        let url = '/Hotel/SurchargeDelete';
        let title = 'Xác nhận xóa giá phụ thu';
        let description = 'Bạn có chắc chắn muốn thông tin?';
        _msgconfirm.openDialog(title, description, function () {
            _ajax_caller.post(url, { id: id }, function (result) {
                if (result.isSuccess) {
                    _msgalert.success(result.message);
                    hotel_surcharge.ReloadListing();
                } else {
                    _msgalert.error(result.message);
                }
            });
        });
    },

    Listing: function (input) {
        _ajax_caller.post("/hotel/SurchargeListing", input, function (result) {
            $('#grid_hotel_surcharge').html(result);
        });
    },

    ReloadListing: function () {
        this.search_params.page_index = 1;
        this.Listing(this.search_params);
    },

    Paging: function (input) {
        this.search_params.page_index = input;
        this.Listing(this.search_params);
    },
    UpdateSurchageNote: function () {
        var hotel_id = $('#global_hotel_id').val()
        let url = '/Hotel/UpdateSurchargeNote';
        var body = tinyMCE.get('text-editor').getContent()

        _ajax_caller.post(url, { hotel_id: hotel_id, body: body }, function (result) {
            if (result.isSuccess) {
                _msgalert.success(result.message);
                hotel_surcharge.ReloadListing();
            } else {
                _msgalert.error(result.message);
            }
        });
    },
    CancelSurchageNote: function () {
        $(tinymce.get('text-editor').getBody()).html($('#hotel-surcharge-note-orginal').val());

    },
    SingleDatePickerFromNow: function (element, dropdown_position = 'down') {

        var today = new Date();
        var yyyy = today.getFullYear();
        var mm = today.getMonth() + 1; // Months start at 0!
        var dd = today.getDate();
        var yyyy_max = yyyy + 10;
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
    GetDayText: function (date, donetdate = false) {
        var text = ("0" + date.getDate()).slice(-2) + '/' + ("0" + (date.getMonth() + 1)).slice(-2) + '/' + date.getFullYear() + ' ' + ("0" + date.getHours()).slice(-2) + ':' + ("0" + date.getMinutes()).slice(-2);
        if (donetdate) {
            text = ("0" + (date.getMonth() + 1)).slice(-2) + '/' + ("0" + date.getDate()).slice(-2) + '/' + date.getFullYear() + ' ' + ("0" + date.getHours()).slice(-2) + ':' + ("0" + date.getMinutes()).slice(-2);
        }
        return text;
    },
}

var hotel_room = {
    Init: function () {
        hotel_room.modal = $('#global_modal_popup');
        hotel_room.search_params = {
            hotel_id: $('#global_hotel_id').val(),
            page_index: 1,
            page_size: 20
        }
        hotel_room.validImageTypes = ['image/gif', 'image/jpeg', 'image/png'];
        hotel_room.validImageSize = 5 * 1024 * 1024;
        hotel_room.noImageSrc = "/images/icons/noimage.png";
        hotel_room.ReloadListing(this.search_params);
    },

    GetFormData: function ($form) {
        var unindexed_array = $form.serializeArray();
        var indexed_array = {};

        $.map(unindexed_array, function (n, i) {
            indexed_array[n['name']] = n['value'];
        });

        return indexed_array;
    },

    OnAddOrUpdate: function (id) {
        let title = `${id > 0 ? "Cập nhật" : "Thêm mới"} phòng khách sạn`;
        let url = '/Hotel/RoomUpsert';

        this.modal.find('.modal-title').html(title);
        this.modal.find('.modal-dialog').css('max-width', '800px');

        _ajax_caller.get(url, { id: id, hotel_id: this.search_params.hotel_id }, function (result) {
            hotel_room.modal.find('.modal-title').html(title);
            hotel_room.modal.find('.modal-body').html(result);
            hotel_room.modal.modal('show');
        });
    },

    OnCopyRoom: function (id) {

        let title = `Thêm mới phòng`;
        let url = '/Hotel/RoomUpsert';

        hotel_room.modal.find('.modal-title').html(title);
        hotel_room.modal.find('.modal-dialog').css('max-width', '800px');

        _ajax_caller.get(url, { id: id, hotel_id: this.search_params.hotel_id, is_copy: true }, function (result) {
            hotel_room.modal.find('.modal-title').html(title);
            hotel_room.modal.find('.modal-body').html(result);
            hotel_room.modal.modal('show');
        });
    },

    OnAddRoomImage: function () {
        const files = document.querySelector('input[name=ImageRoom]').files;
        let grid_image_preview = $('#suplier_room_list_image');

        for (let file of files) {

            if (!hotel_room.validImageTypes.includes(file['type'])) {
                _msgalert.error("File tải lên không đúng định dạng ảnh (gif, jpeg, png,...)");
                break;
            }

            if (hotel_room.validImageSize < file.size) {
                _msgalert.error("Ảnh tải lên vượt quá dung lượng cho phép (5MB).");
                break;
            }

            const reader = new FileReader();
            reader.addEventListener("load", function () {

                let is_exist = grid_image_preview.find(`.image_preview[data-name="${file.name}"]`).length > 0 ? true : false;
                if (!is_exist) {
                    let html = `<div class="col-md-3 col-6 mb10 image_preview" data-name="${file.name}">
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

    OnChangeRoomAvartar: function () {
        const preview = document.querySelector('.img-preview');
        const file = document.querySelector('input[name=ImgAvatar]').files[0];
        const fileType = file['type'];

        if (!hotel_room.validImageTypes.includes(fileType)) {
            _msgalert.error("File tải lên không đúng định dạng ảnh(gif, jpeg, png)");
            return false;
        }

        if (hotel_room.validImageSize < file.size) {
            _msgalert.error("Ảnh tải lên phải có dung lượng bé hơn hoặc bằng 5MB.");
            return false;
        }

        const reader = new FileReader();
        reader.addEventListener("load", function () {
            preview.src = reader.result;
        }, false);
        if (file) {
            reader.readAsDataURL(file);
        }
    },

    Upsert: function () {
        let url = '/Hotel/RoomUpsert';
        let Form = $('#form_hotel_room');
        Form.validate({
            rules: {
                Name: "required",
                BedRoomType: "required",
                TypeOfRoom: "required",
                NumberOfAdult: {
                    required: true,
                    digits: true
                },
                NumberOfChild: {
                    digits: true
                },
                NumberOfRoom: {
                    digits: true
                },
                RoomArea: {
                    digits: true
                }
            },
            messages: {
                Name: "Vui lòng nhập tên phòng",
                TypeOfRoom: "Vui lòng chọn hạng phòng",
                BedRoomType: "Vui lòng chọn loại giường",
                NumberOfAdult: {
                    required: "Bạn phải nhập số người lớn/phòng",
                    digits: "Số người lớn/phòng phải là kí tự dạng số"
                },
                NumberOfChild: {
                    digits: "Số trẻ em/phòng phải là kí tự dạng số"
                },
                NumberOfRoom: {
                    digits: "Số lượng phòng phải là kí tự dạng số"
                },
                RoomArea: {
                    digits: "Diện tích phòng phải là kí tự dạng số"
                }
            }
        });

        if (!Form.valid()) return;

        let formData = this.GetFormData(Form);

        let extend_type = [];
        $('#block_room_extend_ckb .ckb_extend_type:checked').each(function () {
            let seft = $(this);
            extend_type.push(seft.val());
        });

        if (extend_type.length <= 0) {
            _msgalert.error("Vui lòng check chọn ít nhất một loại tiện ích ");
            return;
        }

        formData.Extends = extend_type.join(',');

        let avatar = $('#avatar_room_image').attr('src');
        formData.Avatar = avatar != hotel_room.noImageSrc ? avatar : "";

        let other_image = [];
        $('#suplier_room_list_image .img_other').each(function () {
            let image_item = $(this);
            other_image.push(image_item.attr('src'));
        });

        formData.OtherImages = other_image;

        _ajax_caller.post(url, { model: formData }, function (result) {
            if (result.isSuccess) {
                _msgalert.success(result.message);
                hotel_room.modal.modal('hide');
                hotel_room.ReloadListing();
            } else {
                _msgalert.error(result.message);
            }
        });
    },

    Delete: function (id) {
        let url = '/Hotel/RoomDelete';
        let title = 'Xác nhận xóa phòng';
        let description = 'Bạn có chắc chắn muốn thông tin?';
        _msgconfirm.openDialog(title, description, function () {
            _ajax_caller.post(url, { id: id }, function (result) {
                if (result.isSuccess) {
                    _msgalert.success(result.message);
                    hotel_room.ReloadListing();
                } else {
                    _msgalert.error(result.message);
                }
            });
        });
    },

    Listing: function (input) {
        _ajax_caller.post("/Hotel/RoomListing", input, function (result) {
            $('#grid_hotel_room').html(result);
        });
    },

    ReloadListing: function () {
        this.search_params.page_index = 1;
        this.Listing(this.search_params);
    },

    Paging: function (input) {
        this.search_params.page_index = input;
        this.Listing(this.search_params);
    },

    PagingRoom: function (input) {
        this.search_params.page_index = input;
        this.Listing(this.search_params);
    }
}

var hotel_ultility = {
    Upsert: function () {
        let url = '/Hotel/UltilityUpsert';
        let extend_type = [];
        $('#grid_hotel_benefit .ckb_hotel_extend_type:checked').each(function () {
            let seft = $(this);
            extend_type.push(seft.val());
        });

        let strExtends = extend_type.join(',');

        _ajax_caller.post(url, { hotel_id: $('#global_hotel_id').val(), extends: strExtends }, function (result) {
            if (result.isSuccess) {
                _msgalert.success(result.message);
            } else {
                _msgalert.error(result.message);
            }
        });
    },

    Listing: function (input) {
        _ajax_caller.post("/Hotel/RoomListing", input, function (result) {
            $('#grid_hotel_room').html(result);
        });
    },

    ReloadListing: function () {
        this.Listing(this.search_params);
    }
}

$(document).on('click', '#btn_room_extend_ckb_all', function () {
    let seft = $(this);
    if (seft.is(":checked")) {
        $('#block_room_extend_ckb .ckb_extend_type').prop('checked', true);
    } else {
        $('#block_room_extend_ckb .ckb_extend_type').prop('checked', false);
    }
});

$(document).on('click', '#btn_hotel_extend_ckb_all', function () {
    let seft = $(this);
    if (seft.is(":checked")) {
        $('#grid_hotel_benefit .ckb_hotel_extend_type').prop('checked', true);
    } else {
        $('#grid_hotel_benefit .ckb_hotel_extend_type').prop('checked', false);
    }
});

$(document).ready(function () {
    $('input').attr('autocomplete', 'off');
    hotel_contact.Init();
    hotel_banking_account.Init();
    supplier_program.Init();
    hotel_surcharge.Init();
    hotel_room.Init();
});

var hotel_management = {
    Init: function () {
        this.modal = $('#global_modal_popup');
        this.search_params = {
            FullName: null,
            ProvinceId: null,
            RatingStar: null,
            ChainBrands: null,
            SalerId: null,
            PageIndex: 1,
            PageSize: 20
        }
    },

    GetFormData: function ($form) {
        var unindexed_array = $form.serializeArray();
        var indexed_array = {};

        $.map(unindexed_array, function (n, i) {
            indexed_array[n['name']] = n['value'];
        });

        return indexed_array;
    },

    OnAddOrUpdate: function (id) {
        hotel_management.Init()
        let title = `${id > 0 ? "Cập nhật" : "Thêm mới"} khách sạn`;
        let url = '/Hotel/AddOrUpdate';

        this.modal.find('.modal-title').html(title);
        this.modal.find('.modal-dialog').css('max-width', '900px');

        _ajax_caller.get(url, { id: id }, function (result) {
            hotel_management.modal.find('.modal-title').html(title);
            hotel_management.modal.find('.modal-body').html(result);

            $('.select2-modal').select2();

            //$('.datepicker-input').daterangepicker({
            //    autoUpdateInput: false,
            //    singleDatePicker: true,
            //    showDropdowns: true,
            //    timePicker: true,
            //    minYear: 1901,
            //    maxYear: parseInt(moment().format('YYYY'), 10),
            //    locale: {
            //        format: 'DD/MM/YYYY HH:mm'
            //    }
            //}).on('apply.daterangepicker', function (ev, picker) {
            //    $(this).val(picker.startDate.format('MM/DD/YYYY HH:mm'));
            //}).on('cancel.daterangepicker', function (ev, picker) {
            //    $(this).val('');
            //});

            hotel_management.modal.modal('show');
        });
    },

    Upsert: function () {
        let url = '/Hotel/Save';
        let Form = $('#form_hotel');
        Form.validate({
            rules: {
                Name: "required",
                ProvinceId: "required",
                Street: "required",
                Email: {
                    email: true
                },
                EstablishedYear: {
                    exactlength: 4,
                    digits: true
                },
                Telephone: {
                    minlength: 10,
                    maxlength: 16,
                    digits: true
                }
            },
            messages: {
                Name: "Vui lòng nhập tên khách sạn",
                Street: "Vui lòng nhập địa chỉ",
                ProvinceId: "Vui lòng chọn địa điểm",
                Email: {
                    email: 'Email không đúng định dạng'
                },
                EstablishedYear: {
                    exactlength: "Năm thành lập phải nhập đúng 4 kí tự",
                    digits: "Năm thành lập phải là kí tự dạng số"
                },
                Telephone: {
                    exactlength: "Số điện thoại phải nhập đúng 10 kí tự",
                    digits: "Số điện thoại phải là kí tự dạng số"
                }
            }
        });

        if (!Form.valid()) return;
        let formData = hotel_management.GetFormData(Form);
        let avatar = $('#avatar_image').attr('src');
        formData.HotelId = null;
        formData.ImageThumb = (avatar != null && avatar != "") ? avatar : "";
        formData.IsCommitFund = $('#IsCommitFund').is(":checked")
        debugger
        if ((formData.ImageThumb == null || formData.ImageThumb == "") && status == 0) {
            _msgalert.error("Vui lòng chọn và tải ảnh đại diện");
            return;
        }
        _ajax_caller.post(url, { model: formData }, function (result) {
            if (result.isSuccess) {
                _msgalert.success(result.message);
                hotel_management.modal.modal('hide');
                window.location.reload()
            } else {
                _msgalert.error(result.message);
            }
        });
    },

  
    OnSearch: function () {
        let provinces = $('#sl_search_province').val();
        let stars = $('#sl_search_star').val();
        let branchs = $('#sl_search_branch').val();
        let users = $('#sl_search_suggest_user').val();

        this.search_params.FullName = $('#ip_search_fullname').val();
        this.search_params.ProvinceId = provinces != null ? provinces.join(',') : null;
        this.search_params.RatingStar = stars != null ? stars.join(',') : null;
        this.search_params.ChainBrands = branchs != null ? branchs.join(',') : null;
        this.search_params.SalerId = users != null ? users.join(',') : null;
        this.search_params.PageIndex = 1;
        this.Listing(this.search_params);
    },

    Listing: function (input) {
        _ajax_caller.post("/Hotel/Search", input, function (result) {
            $('#grid_data').html(result);
        });
    },

    ReloadListing: function () {
        this.search_params.PageIndex = 1;
        this.Listing(this.search_params);
    },

    OnPaging: function (input) {
        this.search_params.PageIndex = input;
        this.Listing(this.search_params);
    },

    OnChangeAvartar: function () {
        const preview = document.querySelector('.img-preview');
        const file = document.querySelector('input[name=ImgAvatar]').files[0];
        var fileName = file.name;

        if (!(fileName.includes('.jpg') || fileName.includes('.png') || fileName.includes('.jpeg') || fileName.includes('.gif '))) {
            _msgalert.error('File đính kèm không đúng định dạng ảnh .png, .jpg, .jpeg, gif. Vui lòng kiểm tra lại');
            return false;
        }

        if (file.size > (5 * 1024 * 1024)) {
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
    }
}