var hotel_management = {
    Init: function () {
        this.modal = $('#global_modal_popup');
        this.search_params = {
            FullName: null,
            ProvinceId: null,
            RatingStar: null,
            ChainBrands: null,
            SalerId: null,
            TypeFilter: null,
            PageIndex: 1,
            PageSize: 20
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
                    exactlength: "Số điện thoại phải nhập đúng 10-16 kí tự",
                    digits: "Số điện thoại phải là kí tự dạng số"
                }
            }
        });

        if (!Form.valid()) return;

        let formData = hotel_management.GetFormData(Form);
        var b2c = $('#PositionB2C :checked').text();
        var b2B = $('#PositionB2B :checked').text();
        if (parseFloat(formData.PositionB2B) != 0 && parseFloat(formData.PositionB2B) != parseFloat(b2B)) {
            formData.PositionB2B = b2B
        }
        if (parseFloat(formData.PositionB2C) != 0 && parseFloat(formData.PositionB2C) != parseFloat(b2c)) {
            formData.PositionB2C = b2c
        }
        
        let avatar = $('#avatar_image').attr('src');
        formData.ImageThumb = (avatar != null && avatar != "") ? avatar : "";
        formData.HotelId = null;
        formData.IsCommitFund = $('#IsCommitFund').is(":checked")


        if ((formData.ImageThumb == null || formData.ImageThumb == "") && status == 0) {
            _msgalert.error("Vui lòng chọn và tải ảnh đại diện");
            return;
        }
        _global_function.AddLoading()

        _ajax_caller.post(url, { model: formData }, function (result) {
            _global_function.RemoveLoading()
            if (result.isSuccess) {
                _msgalert.success(result.message);
                hotel_management.modal.modal('hide');
                hotel_management.ReloadListing();
            } else {
                _msgalert.error(result.message);
            }
        });
    },

    Delete: function (id) {
        let url = '/Supplier/ContactDelete';
        let title = 'Xác nhận xóa liên hệ';
        let description = 'Bạn có chắc chắn muốn thông tin?';
        _msgconfirm.openDialog(title, description, function () {
            _ajax_caller.post(url, { id: id }, function (result) {
                if (result.isSuccess) {
                    _msgalert.success(result.message);
                    _supplier_contact.ReloadListing();
                } else {
                    _msgalert.error(result.message);
                }
            });
        });
    },

    OnSearch: function (type) {
        let provinces = $('#sl_search_province').val();
        let stars = $('#sl_search_star').val();
        let branchs = $('#sl_search_branch').val();
        let users = $('#sl_search_suggest_user').val();
        let TypeFilter = null;
        if (type == 1) {
            $('#sl_search_type_filter_2').removeAttr('checked')
        } else {
            $('#sl_search_type_filter_1').removeAttr('checked')
        }       
        if ($('#sl_search_type_filter_1').is(":checked")) {
            TypeFilter = parseFloat($('#sl_search_type_filter_1').attr('data-id'));
            $('#sl_search_type_filter_2').removeAttr('checked')
        }
        if ($('#sl_search_type_filter_2').is(":checked")) {
            TypeFilter = parseFloat($('#sl_search_type_filter_2').attr('data-id'));
            $('#sl_search_type_filter_1').removeAttr('checked')
        }
     
        this.search_params.FullName = $('#ip_search_fullname').val();
        this.search_params.ProvinceId = provinces != null ? provinces.join(',') : null;
        this.search_params.RatingStar = stars != null ? stars.join(',') : null;
        this.search_params.ChainBrands = branchs != null ? branchs.join(',') : null;
        this.search_params.SalerId = users != null ? users.join(',') : null;
        this.search_params.TypeFilter = TypeFilter != null ? TypeFilter : null;
        this.search_params.PageIndex = 1;
        this.Listing(this.search_params);
    },

    Listing: function (input) {
        _global_function.AddLoading()
        _ajax_caller.post("/Hotel/Search", input, function (result) {
            $('#grid_data').html(result);
            _global_function.RemoveLoading()
        });
        _global_function.RemoveLoading()
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
    },
    loadingDistrict: function () {
        var id = $('#ProvinceId').val();
        var row = '';
        $("#City").select2({
            //theme: 'bootstrap4',
            placeholder: "Chọn địa điểm",
            ajax: {
                url: "/Hotel/SuggestDistrict",
                type: "get",
                dataType: 'json',
                delay: 250,
                data: function (params) {
                    var query = {
                        id: id,

                    }
                    return query;
                },
                processResults: function (data) {
                    var data = {
                        results: $.map(data, function (item) {
                            return {
                                text: item.name,
                                id: item.name,
                            }
                        })
                    }
                    return data;
                }
            }
        });

    },
    loadingprogram: function () {
        $('.hotel-data-id-tr').each(function (index, item) {
            var element = $(item);
            var hotel_data_id = element.find('.hotel-data-id');
           
            var hotelid = hotel_data_id.attr('data-id');

            var html_result = '';
            $('.div-program-hotel-id-' + hotelid).html('');
            $('.program-hotel-id-' + hotelid).html('');
            //$.ajax({
            //    url: "/Hotel/ProgramSuggestionByHotelid",
            //    type: "post",
            //    data: { txt_search: hotelid},
            //    success: function (result) {
            //        var html = '<div><a href="{link}" class="blue txt_14" style=" margin-left: 5px;"></i> {name}</a></div>'
            //        var html2 = '<div class="{classid} blue txt_14" style="display: none;"><a href="{link}" class="blue txt_14" style=" margin-left: 5px;"></i> {name}</a></div>'
            //        if (result.status == 0 && result != undefined && result.data != undefined && result.data.length > 0) {
            //            $(result.data).each(function (index, item) {
            //                if (index > 1 ){
            //                    html_result += html2.replace('{link}', "/Programs/DetailPrograms/" + item.id).replace('{name}', item.programname).replace('{classid}', "program_v2_" + item.hotelid);
            //                } else {
            //                    html_result += html.replace('{link}', "/Programs/DetailPrograms/" + item.id).replace('{name}', item.programname);
            //                }
                            
            //            });
            //            if (result.data.length > 2) {
            //                $('.div-program-hotel-id-' + hotelid).html('Chương trình');
            //                $('#show-Program-' + hotelid).html('<div>  <a class="showProgram_1_' + hotelid + ' blue txt_14" onclick="hotel_management.showProgram(1,' + hotelid +')" style="margin-left: 10%;"> Xem thêm</a></div> ');
            //            } else {
            //                $('.div-program-hotel-id-' + hotelid).html('Chương trình');
            //            }
                       
            //            $('.program-hotel-id-' + hotelid).html(html_result);
            //        } else {
            //            $('.program-hotel-id-' + hotelid).html(html_result);
            //            $('.div-program-hotel-id-' + hotelid).html('');
            //        }
                   
            //    }
            //});
        })
    },
    showProgram: function (type,id) {
        if (type > 0) {
            $('#show-Program-' + id).html('<div>  <a class="showProgram_0_' + id + ' blue txt_14" onclick="hotel_management.showProgram(0,' + id + ')" style="margin-left: 10%;"> Đóng</a></div>');
            $('.program_v2_' + id).show();
           
        } else {
            $('#show-Program-' + id).html(' <div>  <a class="showProgram_1_' + id + ' blue txt_14" onclick="hotel_management.showProgram(1,' + id + ')" style="margin-left: 10%;"> Xem thêm</a></div>');
            $('.program_v2_' + id).hide();
           
        }
      
    },
}


$(document).ready(function () {
    $('input').attr('autocomplete', 'off');
    hotel_management.Init();

    $("#sl_search_province").select2({
        placeholder: "Tất cả địa điểm",
        multiple: true
    });

    $("#sl_search_star").select2({
        placeholder: "Tất cả hạng sao",
        multiple: true
    });

    $("#sl_search_branch").select2({
        placeholder: "Tất chuỗi thương hiệu",
        multiple: true
    });

    $("#sl_search_suggest_user").select2({
        //theme: 'bootstrap4',
        placeholder: "Người tạo",
        multiple: true,
        maximumSelectionLength: 3,
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
                            text: `${item.fullname} - ${item.username}`,
                            id: item.id,
                        }
                    })
                };
            }
        }
    });
});
