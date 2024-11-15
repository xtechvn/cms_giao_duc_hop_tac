
var programs_package_html2 = {

    html_programs_package_add_packages_tr2: '<tr class="ProgramsPackage-Add-row"><td><input maxlength="12"  placeholder="Giá nhập" class="form-control currency text-right ProgramsPackage-Add-Amout" id="ProgramsPackage" /></td><td><select style="width:100%;" multiple  class="form-control ProgramsPackage-Add-WeekDay " ><option value="2">Thứ 2</option><option value="3">Thứ 3</option><option value="4">Thứ 4</option><option value="5">Thứ 5</option><option value="6">Thứ 6</option><option value="7">Thứ 7</option><option value="0">CN</option></select></td><td><select style="width:100%;"  class= "form-control ProgramsPackage-Add-WeekDay_2" ></select ></td ><td> <a href="javascript:;" class="blue ml-2 mb10 buttun_add" onclick="_ProgramsPackage.AddProgramsPackage($(this));"><i class="fa fa-plus-circle green"></i> </a> <a class="fa fa-trash-o" href="javascript:;" onclick="_ProgramsPackage.DeleteProgramsPackage($(this));"></a></td></tr >',
    html_programs_package_add_packages_tr3: '<tr class="ProgramsPackage-Edit-row"><td><input maxlength="12"  placeholder="Giá nhập" class="form-control currency text-right ProgramsPackage-Edit-Amout" id="ProgramsPackage" /></td><td><select style="width:100%;" multiple  class="form-control ProgramsPackage-Edit-WeekDay " ><option value="2">Thứ 2</option><option value="3">Thứ 3</option><option value="4">Thứ 4</option><option value="5">Thứ 5</option><option value="6">Thứ 6</option><option value="7">Thứ 7</option><option value="0">CN</option></select></td><td><select style="width:100%;"  class= "form-control ProgramsPackage-Add-WeekDay_2" ></select ></td ><td > <a href="javascript:;" class="blue ml-2 mb10 buttun_add" onclick="_ProgramsPackage.AddProgramsPackage2($(this));"><i class="fa fa-plus-circle green"></i> </a> <a class="fa fa-trash-o" href="javascript:;" onclick="_ProgramsPackage.DeleteProgramsPackage($(this));"></a></td></tr >',
    html_option: '<option value="2">Thứ 2</option><option value="3">Thứ 3</option><option value="4">Thứ 4</option><option value="5">Thứ 5</option><option value="6">Thứ 6</option><option value="7">Thứ 7</option><option value="0">CN</option>',
    html_programs_package_add_packages_tr_Date: '<tr class="ProgramsPackage-Date-row-add "><td><input class="ProgramsPackage-Date-index" value="@index" style="display:none" /><input class="ProgramsPackage-Date-Id" value="" style="display:none" /><input placeholder="Giá nhập" maxlength="12" class="form-control currency text-right ProgramsPackage-Date-Amout"  /></td><td><input class="form-control ProgramsPackage-Date-ApplyDate" type="text"name="@name" style="min-width: 180px !important" placeholder="Thời gian áp dụng" /></td><td class="action "><a href="javascript:;" class="blue ml-2 mb10 buttun_add_date" onclick="_ProgramsPackage.AddProgramsPackageDate($(this));"><i class="fa fa-plus-circle green"></i> </a><a class="fa fa-trash-o" href="javascript:;" onclick="_ProgramsPackage.DeleteProgramsPackageDate($(this));"></a></td></tr >',
    html_programs_package_add_packages_tr_Date2: '<tr class="ProgramsPackage-Date-row "><td><input class="ProgramsPackage-Date-index" value="@index" style="display:none" /><input class="ProgramsPackage-Date-Id" value="" style="display:none" /><input placeholder="Giá nhập" maxlength="12" class="form-control currency text-right ProgramsPackage-Date-Amout"  /></td><td><input class="form-control ProgramsPackage-Date-ApplyDate" type="text"name="@name" style="min-width: 180px !important" placeholder="Thời gian áp dụng" /></td><td class="action "><a href="javascript:;" class="blue ml-2 mb10 buttun_add_date" onclick="_ProgramsPackage.AddProgramsPackageDate2($(this));"><i class="fa fa-plus-circle green"></i> </a><a class="fa fa-trash-o" href="javascript:;" onclick="_ProgramsPackage.DeleteProgramsPackageDate($(this));"></a></td></tr >',
}
var isPickerApprove5 = false;
var isPickerApprove7 = false;
var isPickerApprove = false;
var isPickerApprovePackagePrice = false;
var isPickerApproveProgramsPackage = false;
let dontshow = false;
var _DetailPrograms = {
    OpenPopup: function (id) {
        let title = 'Cập nhật chương trình';
        let url = '/Programs/AddProgramsView';
        let param = {
        };
        if (id.trim() != "") {
            param = {
                id: id,
            };
        }
        _magnific.OpenSmallPopup(title, url, param);

    },
    UpdatePrograms: function (status) {
        let FromProgramsCreate = $('#Programs_form');
        FromProgramsCreate.validate({
            rules: {


                "ProgramCode": {
                    required: true,
                },
                "ProgramName": {
                    required: true,
                },
                "Suppliersid": {
                    required: true,
                },
                "ServiceType": {
                    required: true,
                },
                "StartDate_EndDate": {
                    required: true,
                },
                "StayDate": {
                    required: true,
                },
                "ServiceName": {
                    required: true,
                },


            },
            messages: {

                "ProgramCode": "Mã không được bỏ trống",
                "ProgramName": "Tên không được bỏ trống",
                "Suppliersid": "Nhà cung cấp không được bỏ trống",
                "ServiceType": "Dịch vụ không được bỏ trống",
                "StartDate_EndDate": "Thời gian áp dụng không được bỏ trống",
                "StayDate": "Thời gian lưu trú không được bỏ trống",
                "ServiceName": "Tên dịch vụ không được bỏ trống",
            }
        });
        if (FromProgramsCreate.valid()) {
            var StartDate;
            var EndDate;
            var StayStartDate;
            var StayEndDate;
            if ($('#StartDate_EndDate').data('daterangepicker') !== undefined && $('#StartDate_EndDate').data('daterangepicker') ) {
                StartDate = $('#StartDate_EndDate').data('daterangepicker').startDate._d.toLocaleDateString("en-GB");
                EndDate = $('#StartDate_EndDate').data('daterangepicker').endDate._d.toLocaleDateString("en-GB");
            } else {
                StartDate = null
                EndDate = null
            }
            if ($('#StayDate').data('daterangepicker') !== undefined && $('#StayDate').data('daterangepicker') ) {
                StayStartDate = $('#StayDate').data('daterangepicker').startDate._d.toLocaleDateString("en-GB");
                StayEndDate = $('#StayDate').data('daterangepicker').endDate._d.toLocaleDateString("en-GB");
            } else {
                StayStartDate = null
                StayEndDate = null
            }
            var model = {
                Id: $('#Id').val(),
                ProgramCode: $('#ProgramCode').val(),
                ProgramName: $('#ProgramName').val(),
                SupplierId: $('#Suppliersid').select2().val(),
                ServiceName: $('#ServiceName').select2('data')[0].text,
                HotelId: $('#ServiceName').select2().val(),
                ServiceType: $('#ServiceType').val(),
                StartDateStr: StartDate,
                EndDateStr: EndDate,
                StayStartDateStr: StayStartDate,
                StayEndDateStr: StayEndDate,
                Description: $('#Description').val(),
                Status: status,
            };
            $.ajax({
                url: "/Programs/SummitPrograms",
                type: "Post",
                data: { model },
                success: function (result) {
                    if (result.status == 0) {
                        _msgalert.success(result.msg);
                        $.magnificPopup.close();
                        window.location.reload();
                    } else {
                        _msgalert.error(result.msg);

                    }
                }
            });
        }
    },
    Suppliers: function (element) {
        element.select2({
            theme: 'bootstrap4',
            placeholder: "Tên NCC ",
            hintText: "Nhập từ khóa tìm kiếm",
            searchingText: "Đang tìm kiếm...",
            maximumSelectionLength: 1,
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
    UpdateProgramsStatus: function (status, id) {
        var title = ' ';
        var title2 = ' ';
        if (status == 0) {
            title = 'Xác nhận lưu nháp';
            title2 = 'Xác nhận lưu nháp chương trình ';
        }
        if (status == 3) {
            title = 'Xác nhận từ chối';
            title2 = 'Xác nhận từ chối chương trình';
        }
        if (status == 1) {
            title = 'Xác nhận gửi duyệt';
            title2 = 'Xác nhận gửi duyệt chương trình ';
        }
        if (status == 2) {
            title = 'Xác nhận duyệt';
            title2 = 'Xác nhận duyệt chương trình';
        }
        if (status == 5) {
            title = 'Xác nhận xóa';
            title2 = 'Xác nhận xóa chương trình';
        }
        _msgconfirm.openDialog(title, title2 + ' không?', function () {
            $.ajax({
                url: "/Programs/UpdateProgramsStatus",
                type: "post",
                data: { statustype: status, id: id },
                success: function (result) {
                    if (result != undefined && result.status == 0) {
                        _msgalert.success(result.msg);
                        if (status != 5) {
                            setTimeout(function () {
                                $.magnificPopup.close();
                                window.location.reload();
                            }, 1000)
                        }
                        else {
                            setTimeout(function () {
                                window.location.href = '/Programs/Index';
                            }, 1000)

                        }
                    }

                }
            });
        });

    },
    OnloadHotelName: function () {
        $("#Suppliersid-error").hide()
        var id = $('#ServiceName').select2().val();
        _DetailPrograms.HotelSuggestion($('#ServiceName'));
        $("#Suppliersid").select2({
            theme: 'bootstrap4',
            placeholder: "Nhà cung cấp",
            hintText: "Nhập từ khóa tìm kiếm",
            searchingText: "Đang tìm kiếm...",
            maximumSelectionLength: 1,
            ajax: {
                url: "/Programs/GetSupplierSuggest",
                type: "post",
                dataType: 'json',
                delay: 250,
                data: function () {
                    var query = {
                        id: id,
                    }
                    return query;
                },
                processResults: function (response) {
                    if (response == undefined) {

                        $("#Suppliersid-error").show()
                        $("#Suppliersid-error").text("Dịch vụ khách sạn này chưa có nhà cung cấp")
                        return null;
                    } else {
                        return {
                            results: $.map(response, function (item) {
                                return {
                                    text: item.FullName,
                                    id: item.SupplierId,
                                }
                            })
                        };
                    }


                },

            }
        });
    },
    loadDetailProgramsPackage: function (id, type, SupplierId, ProgramsPackageid) {
        $.magnificPopup.close();
        $('#view_btnT').hide();
        /*$('#view_summit').remove();*/
        $('#view_summit').show();
        if (type != 4) {
            $.ajax({
                url: "/ProgramsPackage/DetailProgramsPackage",
                type: "post",
                data: {
                    id: id, type: type, RoomType: SupplierId, ProgramName: ProgramsPackageid
                },
                success: function (result) {
                    $('#view_summit').html(result);
                    if (type != 1 && type != 2) {
                        $('#PackageCode').removeAttr('disabled')
                    }
                    $(document).load().scrollTop(0);
                }
            });
        }
        else {
            $('#view_summit').hide();
            if (type != 1 ) {
                $('#PackageCode').removeAttr('disabled')
            }
            $(document).load().scrollTop(0);
        }


    },
    showbtnThem: function () {
        $('#view_btnT').show();
        $('#view_summit').hide();
        $('#view_summit_goi').hide();
        $('#btnthemgoi').show()
    },
    OpenviewAdd: function (id, type) {
        let title = 'Thêm mới/Cập nhật gói';

        $('#view_summit_goi').show()
        $('#btnthemgoi').hide()
        $.ajax({
            url: "/ProgramsPackage/DetailProgramsPackage",
            type: "post",
            data: {
                id: id, type: type
            },
            success: function (result) {
                $('#view_summit_goi').html(result);

            }
        });

    },
    SingleDatePickerFromNow: function (dropdown_position = 'down') {
        var date = $('#FromDateAppLy').val();
        $('#ApplyDate').daterangepicker({
            singleDatePicker: true,
            autoApply: true,
            showDropdowns: true,
            minDate: date,
            locale: {
                format: 'DD/MM/YYYY'
            }
        }, function (start, end, label) {


        });
    },
    CheckAppLy: function (element, FromDate, ToDate, Status) {
        $('.buttun_add').each(function (index, item) {
            var element3 = $(item);
            element3.show();
        });
        $('.buttun_add').removeClass("mfp-hide")
        $('.buttun_add_date').each(function (index, item) {
            var element = $(item);
            element.show();
        });
        $('.btn_ApplyDate').removeAttr("disabled", "disabled")
        $('.btn_ApplyDate').removeAttr("style", "cursor: not-allowed;")

       
        $('.btn_' + element).attr("style", "cursor: not-allowed;")
        $('.btn_' + element).attr("disabled", "disabled")
        
        
        $('.ProgramsPackage-Date-row').addClass("ProgramsPackage-Date-row2 mfp-hide");
        $('.ProgramsPackage-Date-row').removeClass("ProgramsPackage-Date-row");
        $('.Date_' + element).addClass("ProgramsPackage-Date-row");
        $('.Date_' + element).removeClass("mfp-hide");

        $('.ProgramsPackage-Edit-row').addClass("ProgramsPackage-Edit-row-2 mfp-hide");
        $('.ProgramsPackage-Edit-row').removeClass("ProgramsPackage-Edit-row");

        $('.' + element).removeClass("mfp-hide");
        $('.' + element).removeClass("ProgramsPackage-Edit-row-2");
        $('.' + element).addClass("ProgramsPackage-Edit-row");

        $('.ProgramsPackage-Edit-WeekDay').addClass("ProgramsPackage-Edit-WeekDay-2");
        $('.ProgramsPackage-Edit-WeekDay').removeClass("ProgramsPackage-Edit-WeekDay");
        $('.WeekDay-' + element).removeClass("mfp-hide");
        $('.WeekDay-' + element).removeClass("ProgramsPackage-Edit-WeekDay-2");
        $('.WeekDay-' + element).addClass("ProgramsPackage-Edit-WeekDay");

        $('input[name="datetimeApprove"]').daterangepicker({
            autoUpdateInput: true,
            autoApply: true,
            showDropdowns: true,
            drops: 'down',
            minDate: FromDate,
            maxDate: ToDate,
            locale: {
                format: 'DD/MM/YYYY'
            }
        });
        $('#ApplyDate').data('daterangepicker').setStartDate(FromDate);
        $('#ApplyDate').data('daterangepicker').setEndDate(ToDate);
        var FromDateDB = _global_function.ParseDateTostring(FromDate);
        var ToDateDB = _global_function.ParseDateTostring(ToDate);
        $('#FromDateDB').val(FromDateDB);
        $('#ToDateDB').val(ToDateDB);
      
        $('.ProgramsPackage-Date-ApplyDate').each(function (index, item) {
            $('input[name="ProgramsPackage-Date-ApplyDate_' + index + '"]').Zebra_DatePicker({
                format: 'd/m/Y',
                direction: [FromDateDB,ToDateDB],
                onClear: function () {

                }
            });
            
        });
        if (Status == 0) {
            $('input[name=optradio][value=0]').prop('checked', true);
        } else {
            $('input[name=optradio][value=1]').prop('checked', true);
        }
        _ProgramsPackage.Select2WithFixedOption($('.ProgramsPackage-Edit-row .ProgramsPackage-Edit-WeekDay'))
    },
 
    Edit: function (id, date, Price, type, ProgramId, roomtype, PackageName) {
        let title = 'Sửa giá';
        let url = '/ProgramsPackage/Edit';
        let param = {
            id: id,
            type: type,
            date: date,
            amount: Price,
            ProgramId: ProgramId,
            roomtype: roomtype,
            PackageName: PackageName,
        };
        _magnific.OpenSmallPopup(title, url, param);
    },
    Edit2: function (id) {
        let title = 'Sửa giá';
        let url = '/ProgramsPackage/Edit';
        let param = {
            id: id,
            type: 1
        };
        _magnific.OpenSmallPopup(title, url, param);
    },
    EditProgramsPackage: function () {
        var type = $("#type").val();
        var ProgramId = $("#ProgramId").val();
        var roomtype = $("#roomtype").val();
        var date = $("#date").val();
        var PackageName = $("#PackageName").val();
        let FromAmountProgramPackage = $("#form-amount-ProgramPackage");
        FromAmountProgramPackage.validate({
            rules: {

                Amount: "required",
            },
            messages: {
                Amount: "Giá không được bỏ trống",
            }
        });
        if (FromAmountProgramPackage.valid()) {
            var id = $('#Id').val();
            var amount = $('#Price').val().replaceAll(',', '');


            $.ajax({
                url: "/ProgramsPackage/EditProgramsPackage",
                type: "POST",
                data: {
                    id: id,
                    amount: amount,
                    date: date,
                    type: type,
                    ProgramId: ProgramId,
                    roomtype: roomtype,
                    PackageName: PackageName,
                },
                success: function (result) {

                    if (result.status === 1) {
                        _msgalert.error(result.msg);
                    }

                    if (result.status === 0) {
                        _msgalert.success(result.msg);
                        if (type == 1) {
                            setTimeout(function () {
                                $.magnificPopup.close();
                                window.location.reload();
                            }, 1000)
                        } else {
                            setTimeout(function () {
                                $.magnificPopup.close();
                                location.reload();
                            }, 1000)
                        }



                    }
                }
            });
        }
    },
    HotelSuggestion: function (element) {
        element.select2({
            theme: 'bootstrap4',
            placeholder: "Tên khách sạn",
            minimumInputLength: 1,
            ajax: {
                url: "/Order/HotelSuggestion",
                type: "post",
                dataType: 'json',
                delay: 250,
                tags: true,
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
                                text: item.name,
                                id: item.hotelid,
                            }
                        })
                    };
                },
                cache: true
            }
        });
    },
    SearchDetailListProgramsPackage: function () {
        var date = $('#filter_date_ListProgramsPackagePrice').val()
        var ProgramName = $('#ProgramName').val()
        var FromDatePrice; //Ngày bat dau
        var ToDatePrice; //Ngày hết hạn
        if ($('#filter_date_ListProgramsPackagePrice').data('daterangepicker') !== undefined && $('#filter_date_ListProgramsPackagePrice').data('daterangepicker') != null && isPickerApprovePackagePrice) {
            FromDatePrice = $('#filter_date_ListProgramsPackagePrice').data('daterangepicker').startDate._d.toLocaleDateString("en-GB");
            ToDatePrice = $('#filter_date_ListProgramsPackagePrice').data('daterangepicker').endDate._d.toLocaleDateString("en-GB");
        } else {
            FromDatePrice = null
            ToDatePrice = null
        }
        if (date == "") {
            FromDatePrice = null
            ToDatePrice = null
        }
        let input = {
            ProgramId: $("#ProgramId").val(),
            Packageid: $("#Packageid").val(),
            ProgramName: ProgramName,
            FromDate: FromDatePrice,
            ToDate: ToDatePrice,
            PageIndex: 1,
            PageSize: 20,
        };
        _global_function.AddLoading()
        $.ajax({
            url: "/ProgramsPackage/ListProgramsPackagePrice",
            type: "post",
            data: input,
            success: function (result) {
                _global_function.RemoveLoading()
                $('#table-ListProgramsPackagePrice').html(result);
            },
        });
       
    },
};
var _ProgramsPackage = {
    Init: function () {
        var id = $('#ProgramId').val();
        objSearch = this.GetParam();
        objSearch.ProgramId = id;
        objSearch.PageIndex = 1;
        objSearch.PageSize = 20;
        _ProgramsPackage.Search(objSearch);

        objSearch2 = {
            ProgramId: id,
            PageIndex: 1,
            PageSize: 10,
        }
        _ProgramsPackage.Search2(objSearch2);
    },
    Search: function (input) {
        $.ajax({
            url: "/ProgramsPackage/ListProgramsPackage",
            type: "post",
            data: input,
            success: function (result) {
                $('#imgLoading').hide();
                $('#grid-data').html(result);
                $('#selectPaggingOptions').val(input.PageSize).attr("selected", "selected");
            },
        });
    },
    Search2: function (input) {
        $.ajax({
            url: "/ProgramsPackage/ListProgramsPrice",
            type: "post",
            data: input,
            success: function (result) {
                $('#imgLoading').hide();
                $('#grid-data2').html(result);
                $('#selectPaggingOptions').val(input.PageSize).attr("selected", "selected");
            },
        });
    },

    GetParam: function () {
        var FromDate; //Ngày bat dau
        var ToDate; //Ngày hết hạn
        if ($('#filter_date_daterangepicker').data('daterangepicker') !== undefined && $('#filter_date_daterangepicker').data('daterangepicker') != null && isPickerApprove) {
            FromDate = $('#filter_date_daterangepicker').data('daterangepicker').startDate._d.toLocaleDateString("en-GB");
            ToDate = $('#filter_date_daterangepicker').data('daterangepicker').endDate._d.toLocaleDateString("en-GB");
        } else {
            FromDate = null
            ToDate = null
        }
        let _searchModel = {
            /*   ProgramId: $("#ProgramId").val(),*/
            ProgramId: $('#ProgramId').val(),
            FromDate: FromDate,
            ToDate: ToDate,
            PageIndex: 1,
            PageSize: $("#selectPaggingOptions").find(':selected').val(),
        };
        return _searchModel;
    },
    OnPaging: function (value) {
        objSearch = this.GetParam();
        objSearch.PageIndex = value;
        _ProgramsPackage.Search(objSearch);
    },
    onSelectPageSize: function () {
        _ProgramsPackage.SearchData();

    },
    OpenPopup: function (id, type, SupplierId) {
        let title = 'Thêm mới gói';
        let url = '/ProgramsPackage/DetailProgramsPackage';
        let param = {
        };
        if (id.trim() != "") {
            param = {
                id: id,
                type: type,
                SupplierId: SupplierId,
            };
        }
        _magnific.OpenSmallPopup(title, url, param);
        _ProgramsPackage.Select2WithFixedOption($('.ProgramsPackage-Add-WeekDay'))
    },
    AddProgramsPackage: function (element) {
        $('.buttun_add').each(function (index, item) {
            var element3 = $(item);
            element3.hide();
        });
        var table_element = element.closest('.ProgramsPackage-tbody')
        table_element.find('.ProgramsPackage-summary-row').before(programs_package_html2.html_programs_package_add_packages_tr2);

        _ProgramsPackage.Select2WithFixedOption($('.ProgramsPackage-Add-WeekDay'))


    },
    AddProgramsPackage2: function (element) {

        $('.buttun_add').each(function (index, item) {
            var element3 = $(item);
            element3.hide();
        });
        var table_element = element.closest('.ProgramsPackage-tbody')
        table_element.find('.ProgramsPackage-summary-row').before(programs_package_html2.html_programs_package_add_packages_tr3);

        _ProgramsPackage.Select2WithFixedOption($('.ProgramsPackage-Edit-WeekDay'))
    },
    DeleteProgramsPackage: function (element) {

        var table_element = element.closest('.ProgramsPackage-tbody')
        var data1 = [];
        var data2 = [];
        $('.ProgramsPackage-Add-WeekDay').each(function (index, item) {
            var element2 = $(item);
            var valus = element2.select2().val();
            data1.push(valus)
        });
        $('.ProgramsPackage-Edit-WeekDay').each(function (index, item) {
            var element3 = $(item);
            var valus = element3.select2().val();
            data2.push(valus)
        });
        if (data1.length > 1) {
            element.closest('.ProgramsPackage-Add-row').remove()
            _ProgramsPackage.Select2WithFixedOption($('.ProgramsPackage-Add-WeekDay'))
        } else {
            if (data1.length == 1) {
                _msgalert.error("Không thể xóa khi còn 1 dòng");
            }

        }
        if (data2.length > 1) {
            element.closest('.ProgramsPackage-Edit-row').remove()
            _ProgramsPackage.Select2WithFixedOption($('.ProgramsPackage-Edit-WeekDay'))
        } else {
            if (data2.length == 1) {
                _msgalert.error("Không thể xóa khi còn 1 dòng");
            }
        }

        $('.buttun_add').each(function (index, item) {
            var element3 = $(item);
            element3.show();
        });

    },
    loadselect2: function () {
        $('.ProgramsPackage-WeekDay').each(function (index, item) {
            var element = $(item);
            _ProgramsPackage.Select2WithFixedOption(element)
        });
    },
    Select2WithFixedOption: function (element) {
     
        let dataOption = [];
        var data = [];
        $('.ProgramsPackage-Add-WeekDay_2').each(function (index, item) {
            var element2 = $(item);
            var valus = element2.select2().val();
            dataOption.push(valus)
        
        });
        $('.ProgramsPackage-Add-WeekDay').each(function (index, item) {
            var element2 = $(item);
            var valus = element2.select2().val();
            data.push(valus)
        
        });
        $('.ProgramsPackage-Edit-row .ProgramsPackage-Edit-WeekDay').each(function (index, item) {
            var element3 = $(item);
            var valus = element3.select2().val();
            data.push(valus)
            
        });
        function formatSelection(state) {
            return state.text;
        }

        function formatResult(state) {
         
            if (!state.id) return state.text; // optgroup
            var id = 'state' + state.id.toLowerCase();
            var label = $('<label></label>', { for: id })
                .text(state.text);
            var checkbox = $('<input type="checkbox">', { id: id });

            return checkbox.add(label);
        }
        element.select2({
            theme: 'bootstrap4',
            placeholder: "Thứ ",
            hintText: "Nhập từ khóa tìm kiếm",
            searchingText: "Đang tìm kiếm...",
            maximumSelectionLength: 7,
            closeOnSelect: false,
            formatResult: formatResult,
            formatSelection: formatSelection,
            ajax: {
                url: "/ProgramsPackage/WeekDaySuggestion",
                type: "post",
                dataType: 'json',
                delay: 250,
                data: function (params) {
                    var query = {
                        txt_search: data.toString(),
                    }

                    return query;
                },
                processResults: function (response) {
                    return {
                        results: $.map(response.data, function (item) {
                            return {
                                text: item.name,
                                id: item.id,
                            }
                        })
                    };
                },
            }
        });
        $('.ProgramsPackage-Add-WeekDay_2').select2({
            theme: 'bootstrap4',
            placeholder: "Chọn thứ nhanh ",
            hintText: "Nhập từ khóa tìm kiếm",
            searchingText: "Đang tìm kiếm...",
            maximumSelectionLength: 7,
            closeOnSelect: false,
            formatResult: formatResult,
            formatSelection: formatSelection,
            ajax: {
                url: "/ProgramsPackage/WeekDaySuggestion2",
                type: "post",
                dataType: 'json',
                delay: 250,
                data: function (params) {
                    var query = {
                        txt_search: dataOption.toString(),
                    }

                    return query;
                },
                processResults: function (response) {
                    return {
                       
                        results: $.map(response.data, function (item) {
                            return {
                                text: item.name,
                                id: item.id,
                            }
                        })
                    };
                },
            }
        });
        element.on('select2:unselect', function (e) {
        
            _ProgramsPackage.Select2WithFixedOption(element)
        });
        $('.ProgramsPackage-WeekDay').each(this.options, function (i, item) {
            if (item.selected) {
                $(item).prop("disabled", true);
            }
        });
        function iformat(icon, badge,) {
            var originalOption = icon.element;
            var originalOptionBadge = $(originalOption).data('badge');

            return $('<span><i class="fa ' + $(originalOption).data('icon') + '"></i> ' + icon.text + '<span class="badge">' + originalOptionBadge + '</span></span>');
        }
     
        $('.ProgramsPackage-Add-row').each(function (index, item) {
            var element2 = $(item);
            element2.find('.ProgramsPackage-Add-WeekDay_2').on("change", function (e) {
                var dataWeekDay_2 = element2.find('.ProgramsPackage-Add-WeekDay_2').val();
                var dataWeekDay = ["0", "2", "3", "4", "5"];
                switch (parseFloat(dataWeekDay_2)) {
                    case 1: {
                        dataWeekDay = null;
                    } break;
                    case 2: {
                        dataWeekDay = ["0", "2", "3", "4", "5"];
                    } break;
                    case 3: {
                        dataWeekDay = ["0", "2", "3", "4", "5", "6"];
                    } break;
                    case 4: {
                        dataWeekDay = ["6", "7"];
                    } break;
                    case 5: {
                        dataWeekDay = ["7"];
                    } break;
                    case 6: {
                        dataWeekDay = ["0", "2", "3", "4", "5", "6", "7"];
                    } break;
                }
              
                element2.find('.ProgramsPackage-Add-WeekDay').select2({ theme: 'bootstrap4' }).val(dataWeekDay).trigger("change");
                _ProgramsPackage.Select2Option2();
            });

        });
      
       
        $('.ProgramsPackage-Edit-row').each(function (index, item) {
            var element3 = $(item);
            
            element3.find('.ProgramsPackage-Add-WeekDay_2').on("change", function (e) {
                var dataWeekDay_2 = element3.find('.ProgramsPackage-Add-WeekDay_2').val();
                var dataWeekDay = ["0", "2", "3", "4", "5"];
                switch (parseFloat(dataWeekDay_2)) {
                    case 1: {
                        dataWeekDay = null;
                    } break;
                    case 2: {
                        dataWeekDay = ["0", "2", "3", "4", "5"];
                    } break;
                    case 3: {
                        dataWeekDay = ["0", "2", "3", "4", "5", "6"];
                    } break;
                    case 4: {
                        dataWeekDay = ["6", "7"];
                    } break;
                    case 5: {
                        dataWeekDay = ["7"];
                    } break;
                    case 6: {
                        dataWeekDay = ["0", "2", "3", "4", "5", "6", "7"];
                    } break;
                }
           
                element3.find('.ProgramsPackage-Edit-WeekDay').select2({ theme: 'bootstrap4' }).val(dataWeekDay).trigger("change");
                _ProgramsPackage.Select2Option2();
            });

        });
    },
    Select2Option2: function () {
        let dataOption2 = [];
   
        $('.ProgramsPackage-Add-WeekDay_2').each(function (index, item) {
            var element2 = $(item);
            var valus = element2.select2().val();
            dataOption2.push(valus)

        });
  
        $('.ProgramsPackage-Add-WeekDay_2').select2({
            theme: 'bootstrap4',
            placeholder: "Chọn thứ nhanh ",
            hintText: "Nhập từ khóa tìm kiếm",
            searchingText: "Đang tìm kiếm...",
            closeOnSelect: false,
            ajax: {
                url: "/ProgramsPackage/WeekDaySuggestion2",
                type: "post",
                dataType: 'json',
                delay: 250,
                data: function (params) {
                    var query = {
                        txt_search: dataOption2.toString(),
                    }

                    return query;
                },
                processResults: function (response) {
                    return {

                        results: $.map(response.data, function (item) {
                            return {
                                text: item.name,
                                id: item.id,
                            }
                        })
                    };
                },
            }
        });
    },
    Select2HotelRoomSuggest: function (element) {
        var id = $("#SupplierID").val();
        var ServiceName = $("#ServiceName").val();
        var HotelId = $("#HotelId").val();
        element.select2({
            theme: 'bootstrap4',
            placeholder: "Hạng phòng ",
            hintText: "Nhập từ khóa tìm kiếm",
            searchingText: "Đang tìm kiếm...",
            maximumSelectionLength: 1,
            ajax: {
                url: "/Programs/GetHotelRoomSuggest",
                type: "post",
                dataType: 'json',
                delay: 250,
                data: function (params) {
                    var query = {
                        id: id,
                        ServiceName: ServiceName,
                        HotelId: HotelId,
                    }
                    return query;
                },
                processResults: function (response) {
                    return {
                        results: $.map(response, function (item) {
                            return {
                                text: item.Name,
                                id: item.Id,
                            }
                        })
                    };
                },
            }
        });
    },
    SummitProgramsPackage: function (type, type2) {
        dontshow = false;
        var FromProgramsPackageCreate = $('#ProgramsPackage_form');
        //FromProgramsPackageCreate.validate({

        //    messages: {
        //        "RoomType": "Hạng  phòng không được bỏ trống",
        //        "ApplyDate": "Thời gian áp dụng không được bỏ trống",

        //        "PackageCode": {
        //            required: "Mã không được bỏ trống",
        //        },
        //    }
        //});

        if ($('#PackageCode').val() == undefined || $('#PackageCode').val() == "") {
            _msgalert.error("Mã không được bỏ trống");
            validate_failed = true
            return false;
        }
        if ($('#RoomType').val() == undefined || $('#RoomType').val() == "") {
            _msgalert.error("Hạng  phòng không được bỏ trống");
            validate_failed = true
            return false;
        }
        var validate_failed = false
        var object_summit = {

            ProgramsPackage: [],
            ProgramsPackageDate: []
        }
        if (type == 1) {
            isPickerApproveProgramsPackage = true;
        }
        var FromDate; //Ngày bat dau
        var ToDate; //Ngày hết hạn
        if ($('#ApplyDate').data('daterangepicker') !== undefined && $('#ApplyDate').data('daterangepicker') != null) {
            FromDate = $('#ApplyDate').data('daterangepicker').startDate._d.toLocaleDateString("en-GB");
            ToDate = $('#ApplyDate').data('daterangepicker').endDate._d.toLocaleDateString("en-GB");

        } else {
            FromDate = null
            ToDate = null
            _msgalert.error("Thời gian áp dụng không được bỏ trống");
            validate_failed = true
            return false;
        }

        $('.ProgramsPackage-Add-row').each(function (index, item) {
            var extra_package_element = $(item);

            var package_amount = extra_package_element.find('.ProgramsPackage-Add-Amout').val();
            var package_WeekDay = extra_package_element.find('.ProgramsPackage-Add-WeekDay').select2("val");


            if (package_amount == null || package_amount.toString() == undefined || package_amount.toString().trim() == '' || package_WeekDay == null || package_WeekDay.toString() == undefined || package_WeekDay.toString().trim() == '') {
                _msgalert.error("Nội dung của giá nhập và theo thứ không được bỏ trống");
                validate_failed = true
                return false;
            }


            var extra_package = {

                PackageCode: $('#PackageCode').val(),
                PackageName: $('#PackageName').val(),
                ProgramId: $('#ProgramId').val(),
                RoomType: $('#RoomType').select2('data')[0].text,
                RoomTypeId: $('#RoomType').val(),
                Price: package_amount,
                FromDateStr: FromDate,
                ToDateStr: ToDate,
                WeekDay: package_WeekDay.toString(),
                OpenStatus: $('.optradio:checked').val(),
            }

            object_summit.ProgramsPackage.push(extra_package);
        });
        $('.ProgramsPackage-Date-row').each(function (index, item) {
            var extra_package_element = $(item);

            var package_amount_date = extra_package_element.find('.ProgramsPackage-Date-Amout').val();
            var package_WeekDay_date = extra_package_element.find('.ProgramsPackage-Date-ApplyDate').val();


            if (package_amount_date == null || package_amount_date.toString() == undefined || package_amount_date.toString().trim() == '' || package_WeekDay_date == null || package_WeekDay_date.toString() == undefined || package_WeekDay_date.toString().trim() == '') {
                _msgalert.error("Nội dung của giá nhập và theo thứ không được bỏ trống");
                validate_failed = true
                return false;
            }


            var extra_package_date = {

                PackageCode: $('#PackageCode').val(),
                PackageName: $('#PackageName').val(),
                ProgramId: $('#ProgramId').val(),
                RoomType: $('#RoomType').select2('data')[0].text,
                RoomTypeId: $('#RoomType').val(),
                Price: package_amount_date,
                FromDateStr: FromDate,
                ToDateStr: ToDate,
                ApplyDateStr: package_WeekDay_date.toString(),
                OpenStatus: $('.optradio:checked').val(),
            }

            object_summit.ProgramsPackageDate.push(extra_package_date);
        });
        if (validate_failed != true) {
            _global_function.AddLoading()
            $.ajax({
                url: "/ProgramsPackage/SummitProgramsPackage",
                type: "post",
                data: { data: object_summit.ProgramsPackage, datadate: object_summit.ProgramsPackageDate, type: type, type2: type2 },
                success: function (result) {
                    _global_function.RemoveLoading()
                    if (result != undefined && result.status == 0) {
                        _msgalert.success(result.msg);
                        setTimeout(function () {
                            if (type == 0) {
                      
                                window.location.href = '/ProgramsPackage/DetailListProgramsPackage/' + object_summit.ProgramsPackage[0].ProgramId + '/' + result.id + '/' + object_summit.ProgramsPackage[0].PackageCode + '/' + object_summit.ProgramsPackage[0].RoomTypeId  +'';
                            } else {
                                $.magnificPopup.close();
                                window.location.reload();
                                dontshow = true;
                            }
                        }, 1000)

                    } else {
                        _msgalert.error(result.msg);
                    }

                }
            });
        }

    },
    SummitupdateProgramsPackage: function (type, type2) {
        dontshow = false;
        var typeSummit = $('#typeSummit').val();
        if ($('#PackageCode').val() == undefined || $('#PackageCode').val() == "") {
            _msgalert.error("Mã không được bỏ trống");
            validate_failed = true
        }
        if ($('#RoomType').val() == undefined || $('#RoomType').val() == "") {
            _msgalert.error("Hạng  phòng không được bỏ trống");
            validate_failed = true
        }
        var validate_failed = false
        var object_summit = {

            ProgramsPackage: [],
            ProgramsPackageDate: []
        }
        if (type == 1 ) {
            isPickerApproveProgramsPackage = true;
        }
        var FromDate; //Ngày bat dau
        var ToDate; //Ngày hết hạn
        if ($('#ApplyDate').data('daterangepicker') !== undefined && $('#ApplyDate').data('daterangepicker') != null && isPickerApproveProgramsPackage) {
            FromDate = $('#ApplyDate').data('daterangepicker').startDate._d.toLocaleDateString("en-GB");
            ToDate = $('#ApplyDate').data('daterangepicker').endDate._d.toLocaleDateString("en-GB");

        } else {
            FromDate = null
            ToDate = null
            _msgalert.error("Thời gian áp dụng không được bỏ trống");
            validate_failed = true
            return false;
        }
        $('.ProgramsPackage-Edit-row').each(function (index, item) {
            var extra_package_element = $(item);
          
            var package_amount = extra_package_element.find('.ProgramsPackage-Edit-Amout').val();
        
            var id = extra_package_element.find('.ProgramsPackage-Edit-Id').val();
            var package_WeekDay = extra_package_element.find('.ProgramsPackage-Edit-WeekDay').select2("val");

            if (package_WeekDay != null) {
                package_WeekDay = package_WeekDay.toString()
            }
            if (package_amount == null || package_amount.toString() == undefined || package_amount.toString().trim() == '' || package_WeekDay == null || package_WeekDay.toString() == undefined || package_WeekDay.toString().trim() == '') {
                _msgalert.error("Nội dung của giá nhập và theo thứ không được bỏ trống");
                validate_failed = true
                return false;
            }

            var extra_package = {
                id: id,
                PackageCode: $('#PackageCode').val(),
                PackageName: $('#PackageName').val(),
                ProgramId: $('#ProgramId').val(),
                RoomType: $('#RoomType').select2('data')[0].text,
                RoomTypeId: $('#RoomType').val(),
                Price: package_amount,
                FromDateStr: FromDate,
                ToDateStr: ToDate,
                WeekDay: package_WeekDay,
                OpenStatus: $('.optradio:checked').val(),
                RoomTypeIdV1: $('#RoomTypeIdv1').val()
            }

            object_summit.ProgramsPackage.push(extra_package);
        });
        $('.ProgramsPackage-Date-row').each(function (index, item) {
            var extra_package_element = $(item);

            var package_amount_date = extra_package_element.find('.ProgramsPackage-Date-Amout').val();
            var package_WeekDay_date = extra_package_element.find('.ProgramsPackage-Date-ApplyDate').val();
            var ToDate_date = extra_package_element.find('.ToDate_date').val();
            var FromDate_date = extra_package_element.find('.FromDate_date').val();
            if (typeSummit == 3) {
                ToDate_date = ToDate;
                FromDate_date = FromDate;
            }


            if (package_amount_date == null || package_amount_date.toString() == undefined || package_amount_date.toString().trim() == '' || package_WeekDay_date == null || package_WeekDay_date.toString() == undefined || package_WeekDay_date.toString().trim() == '') {
                _msgalert.error("Nội dung của giá nhập và theo thứ không được bỏ trống");
                validate_failed = true
                return false;
            }


            var extra_package_date = {

                PackageCode: $('#PackageCode').val(),
                PackageName: $('#PackageName').val(),
                ProgramId: $('#ProgramId').val(),
                RoomType: $('#RoomType').select2('data')[0].text,
                RoomTypeId: $('#RoomType').val(),
                Price: package_amount_date,
                FromDateStr: FromDate_date != undefined ? FromDate_date : FromDate,
                ToDateStr: ToDate_date != undefined ? ToDate_date : ToDate,
                ApplyDateStr: package_WeekDay_date.toString(),
                OpenStatus: $('.optradio:checked').val(),
            }

            object_summit.ProgramsPackageDate.push(extra_package_date);
        });
        $('.ProgramsPackage-Date-row-add ').each(function (index, item) {
            var extra_package_element = $(item);

            var package_amount_date = extra_package_element.find('.ProgramsPackage-Date-Amout').val();
            var package_WeekDay_date = extra_package_element.find('.ProgramsPackage-Date-ApplyDate').val();
            var ToDate_date = extra_package_element.find('.ToDate_date').val();
            var FromDate_date = extra_package_element.find('.FromDate_date').val();

            if (typeSummit == 3) {
                ToDate_date = ToDate;
                FromDate_date = FromDate;
            }
            if (package_amount_date == null || package_amount_date.toString() == undefined || package_amount_date.toString().trim() == '' || package_WeekDay_date == null || package_WeekDay_date.toString() == undefined || package_WeekDay_date.toString().trim() == '') {
                _msgalert.error("Nội dung của giá nhập và theo thứ không được bỏ trống");
                validate_failed = true
                return false;
            }


            var extra_package_date = {

                PackageCode: $('#PackageCode').val(),
                PackageName: $('#PackageName').val(),
                ProgramId: $('#ProgramId').val(),
                RoomType: $('#RoomType').select2('data')[0].text,
                RoomTypeId: $('#RoomType').val(),
                Price: package_amount_date,
                FromDateStr: FromDate_date != undefined ? FromDate_date : FromDate,
                ToDateStr: ToDate_date != undefined ? ToDate_date : ToDate,
                ApplyDateStr: package_WeekDay_date.toString(),
                OpenStatus: $('.optradio:checked').val(),
            }

            object_summit.ProgramsPackageDate.push(extra_package_date);
        });
        if (validate_failed != true) {
            _global_function.AddLoading()
            $.ajax({
                url: "/ProgramsPackage/SummitProgramsPackage",
                type: "post",
                data: { data: object_summit.ProgramsPackage, datadate: object_summit.ProgramsPackageDate,  type: type, type2: type2 },
                success: function (result) {
                    _global_function.RemoveLoading()
                    if (result != undefined && result.status == 0) {
                        _msgalert.success(result.msg);
                        dontshow = true;
                        if (typeSummit > 0) {
                            $('#text_Addpk').html("Chi tiết gói :" + object_summit.ProgramsPackage[0].PackageCode);
                            $('#view_summit').hide();
                            $('#List_data').show();
                            console.log($('#ProgramId').val(), result.id, object_summit.ProgramsPackage[0].PackageCode, object_summit.ProgramsPackage[0].RoomTypeId, $('#typeSummit').val())
                            _ProgramsPackage.loaddataAddnhanhProgramsPackage($('#ProgramId').val(), result.id, object_summit.ProgramsPackage[0].PackageCode, object_summit.ProgramsPackage[0].RoomTypeId, $('#typeSummit').val())
                        } else {
                            setTimeout(function () {
                                $.magnificPopup.close();
                                window.location.reload();
                            }, 1000)
                        }
                     

                    } else {
                        _msgalert.error(result.msg);
                    }

                }
            });

        }
    },
    loadgrid: function (valus) {
        if (valus == 1) {
            $('#grid-data2').attr("style", "display: none;")
            $('#grid-data').removeAttr("style", "display: none;")

            $('#DSgoi').addClass("active")
            $('#Bgia').removeClass("active")
        }
        else {
            $('#grid-data').attr("style", "display: none;")
            $('#grid-data2').removeAttr("style", "display: none;")
            $('#Bgia').addClass("active")
            $('#DSgoi').removeClass("active")
        }
    },
    ProgramsDetail: function (id, type, SupplierId, ProgramsPackageid) {
        let title = 'Chi tiết gói chương trình';
        let url = '/ProgramsPackage/DetailProgramsPackage';
        let param = {
        };
        if (id.trim() != "") {
            param = {
                id: id,
                type: type,
                SupplierId: SupplierId,
                ProgramsPackageid: ProgramsPackageid,
            };
        }
        _magnific.OpenSmallPopup(title, url, param);

    },
    ProgramsDetail2: function (id, type, SupplierId, ProgramName) {
        let title = 'Thông tin hạng phòng';
        let url = '/ProgramsPackage/DetailProgramsPackage2';
        let param = {
        };
        if (id.trim() != "") {
            param = {
                id: id,
                type: type,
                RoomType: SupplierId,
                ProgramName: ProgramName,
            };
        }
        _magnific.OpenSmallPopup(title, url, param);

    },
    SearchProgramsPackagePrice: function () {

        var FromDate; //Ngày bat dau
        var ToDate; //Ngày hết hạn
        if ($('#filter_date_ProgramsPackagePrice').data('daterangepicker') !== undefined && $('#filter_date_ProgramsPackagePrice').data('daterangepicker') != null && isPickerApprove) {
            FromDate = $('#filter_date_ProgramsPackagePrice').data('daterangepicker').startDate._d.toLocaleDateString("en-GB");
            ToDate = $('#filter_date_ProgramsPackagePrice').data('daterangepicker').endDate._d.toLocaleDateString("en-GB");
        } else {
            FromDate = null
            ToDate = null
        }
        let input = {
            ProgramId: $("#ProgramId").val(),
            FromDate: FromDate,
            ToDate: ToDate,
            PageIndex: 1,
            PageSize: $("#selectPaggingOptions").find(':selected').val(),
        };

        _ProgramsPackage.Search2(input);
    },
    ShowEditView: function () {


        $('#editview').addClass('mfp-hide');

        $('#RoomType').prop('disabled', false);
        /* $('#PackageCode').prop('disabled', false);*/
        $('#ApplyDate').prop('disabled', false);
        $('.optradio').prop('disabled', false);

        $('.ProgramsPackage-RoomType').prop('disabled', false);
        $('.ProgramsPackage-Amout').prop('disabled', false);
        $('.ProgramsPackage-WeekDay').prop('disabled', false);

        $('.ProgramsPackage-Amout').removeClass('input-disabled-background')
        $('.ProgramsPackage-WeekDay').removeClass('input-disabled-background')
        $('.ProgramsPackage-RoomType').removeClass('input-disabled-background')
        $('.ProgramsPackage-summary-row').removeClass('mfp-hide')
        $('.action').removeClass('mfp-hide')
        $('#btnLuu').removeClass('mfp-hide')

    },
    HineAddView: function () {
        $('.buttun_add').each(function (index, item) {
            var element3 = $(item);
            element3.hide();
        });
    },
    HotelRoomSuggest: function () {
        var id = $("#SupplierID").val();
        var ServiceName = $("#ServiceName").val();
        var HotelId = $("#HotelId").val();

        $('.RoomType').select2({
            theme: 'bootstrap4',
            placeholder: "Hạng phòng ",
            hintText: "Nhập từ khóa tìm kiếm",
            searchingText: "Đang tìm kiếm...",

            ajax: {
                url: "/Programs/GetHotelRoomSuggest",
                type: "post",
                dataType: 'json',
                delay: 250,
                data: function (params) {
                    var query = {
                        id: id,
                        ServiceName: ServiceName,
                        HotelId: HotelId,
                    }
                    return query;
                },
                processResults: function (response) {
                    return {
                        results: $.map(response, function (item) {
                            return {
                                text: item.Name,
                                id: item.Id,
                            }
                        })
                    };
                },
            }
        });
    },

    HotelRoomSuggest2: function () {

        var id = $("#SupplierID").val();
        var ServiceName = $("#ServiceName").val();
        var HotelId = $("#HotelId").val();
        $('#RoomType').select2({
            theme: 'bootstrap4',
            placeholder: "Hạng phòng ",
            hintText: "Nhập từ khóa tìm kiếm",
            searchingText: "Đang tìm kiếm...",

            ajax: {
                url: "/Programs/GetHotelRoomSuggest",
                type: "post",
                dataType: 'json',
                delay: 250,
                data: function (params) {
                    var query = {
                        id: id,
                        ServiceName: ServiceName,
                        HotelId: HotelId,
                    }
                    return query;
                },
                processResults: function (response) {
                    return {
                        results: $.map(response, function (item) {
                            return {
                                text: item.Name,
                                id: item.Id,
                            }
                        })
                    };
                },
            }
        });
    },
    DeleteProgramsPackagedb: function (id, packagecode, roomtype, amount, date, type, WeekDay,date2) {
        _msgconfirm.openDialog('Xác nhận xóa hạng phòng', 'Xác nhận xóa hạng phòng ' + roomtype + ' của gói ' + packagecode + ' không?', function () {
            $.ajax({
                url: "/ProgramsPackage/DeleteProgramsPackage",
                type: "post",
                data: { id: id, packagecode: packagecode, roomtype: roomtype, amount: amount, date: date, type: type, WeekDay: WeekDay, date2: date2 },
                success: function (result) {
                    if (result != undefined && result.status == 0) {
                        _msgalert.success(result.msg);
                        setTimeout(function () {
                            $.magnificPopup.close();
                            window.location.reload();
                        }, 1000)

                    } else {
                        _msgalert.error(result.msg);
                    }

                }
            });
        });

    },

    AddProgramsPackageDate: function (element) {
        var FromDateDB = $('#FromDateDB').val()
        var ToDateDB = $('#ToDateDB').val()

        $('.buttun_add_date').each(function (index, item) {
            var element3 = $(item);
            element3.hide();
        });
        let count = 0
        $('.ProgramsPackage-Date-row-add').each(function (index, item) {
            var element = $(item)
            var countindex = element.find('.ProgramsPackage-Date-index').val()
   
            if (countindex != undefined) {
                count = parseFloat(countindex) + 1

            }
            else {
                count++
            }

        });
        var table_element = element.closest('.ProgramsPackage-Date-tbody')
        table_element.find('.ProgramsPackage-Date-summary-row').before(programs_package_html2.html_programs_package_add_packages_tr_Date.replaceAll('@name', 'ProgramsPackage-Date-ApplyDate_' + count + '_' + count).replaceAll('@index', count));
      
        $('input[name="ProgramsPackage-Date-ApplyDate_' + count + '_' + count + '"]').Zebra_DatePicker({
            direction: [FromDateDB, ToDateDB],
            onSelect: function () {
                var fromDate = $('input[name="ProgramsPackage-Date-ApplyDate_' + count + '_' + count+'"]').val()
                var lstFromDate = fromDate.split('-')
                $('input[name="ProgramsPackage-Date-ApplyDate_' + count + '_'+ count +'"]').val(lstFromDate[2] + '/' + lstFromDate[1] + '/' + lstFromDate[0])

            },
            onClear: function () {

            }
        });
    },
    AddProgramsPackageDate2: function (element) {
        var FromDate; //Ngày bat dau
        var ToDate; //Ngày hết hạn
        if ($('#ApplyDate').data('daterangepicker') !== undefined && $('#ApplyDate').data('daterangepicker') != null) {
            FromDate = $('#ApplyDate').data('daterangepicker').startDate._d.toLocaleDateString("en-GB");
            ToDate = $('#ApplyDate').data('daterangepicker').endDate._d.toLocaleDateString("en-GB");

        } else {
            FromDate = null
            ToDate = null
        }
        var FromDateDB = FromDate == null ? $('#FromDateDB').val() : _global_function.ParseDateTostring(FromDate)
        var ToDateDB = ToDate == null ? $('#ToDateDB').val() : _global_function.ParseDateTostring(ToDate)
    
        $('.buttun_add_date').each(function (index, item) {
            var element3 = $(item);
            element3.hide();
        });
        let count2 = 0
        $('.ProgramsPackage-Date-row').each(function (index, item) {
            var element = $(item)
            var countindex = element.find('.ProgramsPackage-Date-index').val()
         
            if (countindex != undefined) {
                count2 = parseFloat(countindex) + 1

            }
            else {
                count2++
            }
            count2++

        });
        var table_element = element.closest('.ProgramsPackage-Date-tbody')
        table_element.find('.ProgramsPackage-Date-summary-row').before(programs_package_html2.html_programs_package_add_packages_tr_Date2.replaceAll('@name', 'ProgramsPackage-Date-ApplyDate_' + count2 + '_' + count2).replaceAll('@index', count2));

        $('input[name="ProgramsPackage-Date-ApplyDate_' + count2 + '_' + count2 + '"]').Zebra_DatePicker({
            direction: [FromDateDB, ToDateDB],
            onSelect: function () {
                var fromDate = $('input[name="ProgramsPackage-Date-ApplyDate_' + count2 + '_' + count2 + '"]').val()
                var lstFromDate = fromDate.split('-')
                $('input[name="ProgramsPackage-Date-ApplyDate_' + count2 + '_' + count2 + '"]').val(lstFromDate[2] + '/' + lstFromDate[1] + '/' + lstFromDate[0])

            },
            onClear: function () {

            }
        });
    },
    DeleteProgramsPackageDate: function (element) {
      
        element.closest('.ProgramsPackage-Date-row').remove()
        element.closest('.ProgramsPackage-Date-row-add').remove()

        $('.buttun_add_date').each(function (index, item) {
            var element3 = $(item);
            element3.show();
        });

    },
    loadDatePicker: function () {
        var count = 0;
        var FromDateDB = $('#FromDateDB').val()
        var ToDateDB = $('#ToDateDB').val()
        
        $('.ProgramsPackage-Date-ApplyDate').each(function (index, item) {
            var element = $(item);

            $('input[name="ProgramsPackage-Date-ApplyDate_' + index + '"]').Zebra_DatePicker({
                direction: [FromDateDB, ToDateDB],
                onSelect: function () {
                    var fromDate = $('input[name="ProgramsPackage-Date-ApplyDate_' + index + '"]').val()
                    var lstFromDate = fromDate.split('-')
                    $('input[name="ProgramsPackage-Date-ApplyDate_' + index + '"]').val(lstFromDate[2] + '/' + lstFromDate[1] + '/' + lstFromDate[0])

                },
            });
         
        });
 
        //$('input[name="ProgramsPackage-Date-ApplyDate_0"]').Zebra_DatePicker({
        //    format: 'd/m/Y',
        //    direction: [FromDateDB, ToDateDB],
        //    onClear: function () {

        //    }
        //});
    },
 
    loaddataAddnhanhProgramsPackage: function (id, Packageid, ProgramName, RoomTypeid, type) {
        $.ajax({
            url: "/ProgramsPackage/ListDataAddProgramsPackage",
            type: "post",
            data: { id: id, Packageid: Packageid, ProgramName: ProgramName, RoomTypeid: RoomTypeid, type: type },
            success: function (result) {
                $('#List_data').html(result);
                if (type != 1) {
                    $('#PackageCode').removeAttr('disabled')
                }
     
                $(document).load().scrollTop(0);
            }
        });
    },

};
