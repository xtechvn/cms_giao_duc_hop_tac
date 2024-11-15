$(document).ready(function () {
    _Play_ground.loadplayground();
    var input = document.getElementById("BasicTitle");
    input.addEventListener("keypress", function (event) {
        // If the user presses the "Enter" key on the keyboard
        if (event.key === "Enter") {
            // Cancel the default action, if needed
            event.preventDefault();
            // Trigger the button element with a click
            document.getElementById("myBtn").click();
        }
    });
});
var URL = "https://adavigo.com/tin-tuc/";
var _Play_ground = {
  
    loadplayground: function () {
        let _searchModel = {
            LocationName: null,
            PageIndex: 1,
            PageSize: 20,

        };
        let objSearch = {
            searchModel: _searchModel,
            currentPage: 1,
            pageSize: 20
        };

        this.SearchParam = objSearch;
        this.Search(objSearch);
    },
    Search: function (model) {
        window.scrollTo(0, 0);
        $.ajax({
            url: "/playground/Search",
            type: "Post",
            data: model,
            success: function (result) {
                $('#imgLoading').hide();
                $('#grid-data').html(result);
            }
        });
    },
    OpenPopup: function (id) {

        let title = 'Thêm mới/Sửa địa điểm khu vui chơi';
        let url = '/playground/PopupAdd';
        let param = {

        };
        if (id.trim() != "") {
            param = {
                id: id,
            };
        }
        _magnific.OpenSmallPopup(title, url, param);
    },
    SearchData: function () {

        let objSearch = {
            LocationName: $('#BasicTitle').val(),
            PageIndex: 1,
            PageSize: 20,

        };

        _Play_ground.Search(objSearch)
    },
    
    GetSuggest: function () {
        //$('#Code').select2({
        //    placeholder: "Nhập địa điểm cần tìm ",
        //    hintText: "Nhập địa điểm tìm kiếm",
        //    searchingText: "Đang tìm kiếm...",
        //    theme: 'bootstrap4',
        //    maximumSelectionLength: 1,
        //    ajax: {
        //        url: "/playground/GetProductCategorySuggest",
        //        type: "post",
        //        dataType: 'json',
        //        delay: 250,
        //        data: function (params) {
        //            var query = {
        //                name: params.term,
        //            }
        //            return query;
        //        },
        //        processResults: function (response) {
        //            return {
        //                results: $.map(response, function (item) {
        //                    return {
        //                        text: item.name,
        //                        id: item.code,
        //                    }
        //                })
        //            };
        //        },
        //        cache: true
        //    }
        //});
        $('#NewsId').select2({
            placeholder: "Nhập tiêu đề bài viết cần tìm ",
            hintText: "Nhập tiêu đề tìm kiếm",
            searchingText: "Đang tìm kiếm...",
            theme: 'bootstrap4',
            maximumSelectionLength: 1,
            ajax: {
                url: "/playground/ArticleSuggestion",
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
                                text: item.title,
                                id: item.id,
                            }
                        })
                    };
                },
                cache: true
            }
        });

    },
    LinkDetailShow: function () {
        var NewsId = $('#NewsId').val();
        var NewsIdText = $('#NewsId').text().trim().replaceAll(' ', '-');
        $('#Link-detail').removeAttr('href');
        if (NewsId != null) {
            $('#Link-detail').removeAttr('style');
        } else {
            $('#Link-detail').attr('style', "display:none");
            $('#NewsId option').remove("option")
        }
      
        $('#Link-detail').attr('href', URL + NewsIdText + "-" + NewsId);
    },

    FileAttachment: function () {
        var data_id = $('#id').val();
        var type = 6;
        $.ajax({
            url: "/AttachFile/Widget",
            type: "Post",
            data: { data_id: data_id, type: type, element_name: "attachment-image-choice-list", image_upload_only: true },
            success: function (result) {
                $('.attachment-file-block').html(result);
            }
        });
    },
    Summitplayground: function () {
        let FromSummit = $('#form-playground');
        FromSummit.validate({
            rules: {

                "Code": "required",
                "ServiceType": {
                    required: true,

                },
            },
            messages: {

                "Code": "Tên địa điểm không được bỏ trống",
                "ServiceType": "Dịch vụ không được bỏ trống",
            }
        });
        let other_image = [];
        $('#grid_image_preview .img_other').each(function () {
            let image_item = $(this);
            other_image.push(image_item.attr('src'));
        });
        if (FromSummit.valid()) {
            var object_summit = {
                Code: $('#Code').val(),
                Id: $('#id').val(),
                ServiceType: $('#ServiceType').val(),
                NewsId: $('#NewsId').val(),
                /*LocationName: $('#LocationName').val(),*/
                LocationName: $('#Code').find(':selected').text(),
                Description: $('#Description').val(),
                Status: $('input[name="optradio"]:checked').val(),
            }
          /*  var list_attach_file = _attachment_widget.GetAttachmentFileList()*/
            var list_attach_file = other_image
            $('#Luu').attr("disabled","disabled");
            _global_function.AddLoading()
            $.ajax({
                url: "/playground/AddPlayGround",
                type: "post",
                data: { data: object_summit, attach_file: list_attach_file },
                success: function (result) {
                    if (result != undefined && result.status == 0) {
                        _global_function.RemoveLoading()
                        _msgalert.success(result.msg);
                        _attachment_widget.ConfirmUploadFile(result.id)
                        setTimeout(function () {
                            window.location.reload();
                        }, 1000);
                    }
                    else {

                        _global_function.RemoveLoading()
                        _msgalert.error(result.msg);
                        $('#Luu').removeAttr('disabled');
                        $('.btn_summit_service_hotel').removeAttr('disabled')
                        $('.btn_summit_service_hotel').removeClass('disabled')
                        $('.img_loading_summit').remove()
                    }
                }
            });
          
        }
    },
    OnPaging: function (value) {
        let _searchModel = {
            LocationName: null,
            PageIndex: value,
            PageSize: 20,

        };
        let objSearch = {
            searchModel: _searchModel,
            currentPage: 1,
            pageSize: 20
        };
        _Play_ground.Search(objSearch)
    },
    Deleteplayground: function (id) {
        $.ajax({
            url: "/playground/DeletePlayGround",
            type: "Post",
            data: { id:id },
            success: function (result) {
                if (result != undefined && result.status == 0) {
                    _msgalert.success(result.msg);
                    setTimeout(function () {
                        window.location.reload();
                    }, 1000);
                }
            }
        });
    },
    OnAddImage: function () {
        this.validImageTypes = ['image/gif', 'image/jpeg', 'image/png'];
        this.validImageSize = 5 * 1024 * 1024;
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
                    let html = `<div class="col-md-3 col-6 mb10 image_preview" data-name="${file.name}">
                     <div class="choose-ava">
                     <img class="img_other" src="${reader.result}">
                     <button type="button" class="delete" onclick="this.closest('.image_preview').remove();">×</button>
                     </div>
                     <p>${file.name}</p>
                     </div>`;
                    grid_image_preview.append(html);
                }

            }, false);

            if (file) {
                reader.readAsDataURL(file);
            }
        }
    },
}