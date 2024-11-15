let ProgramsFields = {
    ProgramCode: true,
    ProgramNamef: true,
    ServiceType: false,
    ServiceNamef: true,
    SupplierNamef: true,
    ApplyDate: true,
    UserCreate: true,
    Descriptionf: true,
    CreateDatef: true,
    UserVerify: true,
    VerifyDate: true,

}
let cookieName = 'ProgramsFields_transactionsms';
let cookieFilterName = 'ProgramsFields_filter';
$(document).ready(function () {
    _Programs.Init();
    $("#UserCreate").select2({
        theme: 'bootstrap4',
        placeholder: "Người tạo",
        maximumSelectionLength: 1,
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
    $("#UserVerify").select2({
        theme: 'bootstrap4',
        placeholder: "Người duyệt",
        maximumSelectionLength: 1,
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

    $("#SupplierID").select2({
        theme: 'bootstrap4',
        placeholder: "Tên NCC, Điện Thoại, Email",
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
                            text: item.name,
                            id: item.id,
                        }
                    })
                };
            },
            cache: true
        }
    });
    $("#ProgramCode").select2({
        theme: 'bootstrap4',
        placeholder: "Id,Mã,Tên chương trình",
        searchingText: "Đang tìm kiếm...",
        maximumSelectionLength: 1,
        ajax: {
            url: "/Programs/ProgramSuggestion",
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
                            text: item.programname + ' - ' + item.programcode,
                            id: item.programcode,
                        }
                    })
                };
            },
            cache: true
        }
    });
    _Programs.checkShowHide();
});
let isPickerApprove = false;
let isPickerApprove2 = false;
let isPickerApprove3 = false;
let isPickerApprove4 = false;
let isPickerApprove5 = false;
let isPickerApprove6 = false;
let isPickerApprove7 = false;

var _Programs = {
    Init: function () {

        objSearch = this.GetParam();
        objSearch.PageSize = 20;
        _Programs.Search(objSearch);
    },
    Search: function (input) {
        window.scrollTo(0, 0);
        $.ajax({
            url: "/Programs/SearchPrograms",
            type: "Post",
            data: input,
            success: function (result) {
                $('#imgLoading').hide();
                $('#grid_data').html(result);
                $('#selectPaggingOptions').val(input.PageSize).attr("selected", "selected");

            }
        });
    },
    SearchData: function () {
        _global_function.AddLoading()
        var objSearch = this.GetParam();

        _Programs.Search(objSearch);
        _global_function.RemoveLoading()
        $(document).load().scrollTop(0);
    },
    GetParam: function () {
        var StartDateFrom = null; //Ngày bat dau
        var StartDateTo = null; //Ngày hết hạn
        var EndDateFrom = null; //Ngày hết hạn
        var EndDateTo = null; //Ngày hết hạn
        var CreateDateFrom = null; //Ngày hết hạn
        var CreateDateTo = null; //Ngày hết hạn
        var VerifyDateFrom = null; //Ngày hết hạn
        var VerifyDateTo = null; //Ngày hết hạn
        if ($('#ApplyDate').data('daterangepicker') !== undefined && $('#ApplyDate').data('daterangepicker') != null && isPickerApprove2) {
            StartDateFrom = $('#ApplyDate').data('daterangepicker').startDate._d.toLocaleDateString("en-GB");
            StartDateTo = $('#ApplyDate').data('daterangepicker').endDate._d.toLocaleDateString("en-GB");
        } else {
            StartDateFrom = null
            StartDateTo = null
        }
        if ($('#ApplyDate2').data('daterangepicker') !== undefined && $('#ApplyDate2').data('daterangepicker') != null && isPickerApprove6) {
            EndDateFrom = $('#ApplyDate2').data('daterangepicker').startDate._d.toLocaleDateString("en-GB");
            EndDateTo = $('#ApplyDate2').data('daterangepicker').endDate._d.toLocaleDateString("en-GB");
        } else {
            EndDateFrom = null
            EndDateTo = null
        }
        if ($('#CreatDate').data('daterangepicker') !== undefined && $('#CreatDate').data('daterangepicker') != null && isPickerApprove3) {
            CreateDateFrom = $('#CreatDate').data('daterangepicker').startDate._d.toLocaleDateString("en-GB");
            CreateDateTo = $('#CreatDate').data('daterangepicker').endDate._d.toLocaleDateString("en-GB");
        } else {
            CreateDateFrom = null
            CreateDateTo = null
        }
        if ($('#VerifyDate').data('daterangepicker') !== undefined && $('#VerifyDate').data('daterangepicker') != null && isPickerApprove4) {
            VerifyDateFrom = $('#VerifyDate').data('daterangepicker').startDate._d.toLocaleDateString("en-GB");
            VerifyDateTo = $('#VerifyDate').data('daterangepicker').endDate._d.toLocaleDateString("en-GB");
        } else {
            VerifyDateFrom = null
            VerifyDateTo = null
        }

        var _searchModel = {
            ProgramCode: $("#ProgramCode").select2("val"),
            Description: null,
            ProgramStatus: $("#ProgramStatus").val(),
           /* ServiceType: $("#ServiceType").val(),*/
            SupplierID: $("#SupplierID").select2("val"),
            ClientId: null,
            StartDateFrom: StartDateFrom,
            StartDateTo: StartDateTo,
            EndDateFrom: EndDateFrom,
            EndDateTo: EndDateTo,
            UserCreate: $("#UserCreate").select2("val"),
            CreateDateFrom: CreateDateFrom,
            CreateDateTo: CreateDateTo,
            UserVerify: $("#UserVerify").select2("val"),
            VerifyDateFrom: VerifyDateFrom,
            VerifyDateTo: VerifyDateTo,
            PageIndex: 1,
            PageSize: $("#selectPaggingOptions").find(':selected').val(),
        };
        return _searchModel;
    },
    OnPaging: function (value) {
        var objSearch = this.GetParam();
        objSearch.PageIndex = value;
        _Programs.Search(objSearch);
        $(document).load().scrollTop(0);
    },
    onSelectPageSize: function () {
        var objSearch = this.GetParam();
        _Programs.Search(objSearch);
        $(document).load().scrollTop(0);
    },
    OpenPopup: function (id) {
        let title = 'Thêm mới chương trình';
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
            }
        });
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
                                text: item.name + "(id: "+item.hotelid+")",
                                id: item.hotelid,
                            }
                        })
                    };
                },
                cache: true
            }
        });
    },
    InsertPrograms: function (status) {
        let FromProgramsCreate = $('#Programs_form');
        FromProgramsCreate.validate({
            rules: {

                ProgramName: {
                    required: true,
                },
                "Suppliersid": {
                    required: true,
                },
                //"ServiceType": {
                //    required: true,
                //},
                "StartDate_EndDate": {
                    required: true,
                },
                "StayDate": {
                    required: true,
                },
                "ServiceName": {
                    required: true,
                },
                ProgramCode: {
                    required: true,
                    maxlength: 15,
                },

            },
            messages: {

                ProgramName: {
                    required: "Tên không được bỏ trống",
                },

                "Suppliersid": "Nhà cung cấp không được bỏ trống",
                /*"ServiceType": "Dịch vụ không được bỏ trống",*/
                "StartDate_EndDate": "Thời gian áp dụng không được bỏ trống",
                "StayDate": "Thời gian lưu trú không được bỏ trống",
                "ServiceName": "Tên dịch vụ không được bỏ trống",
                ProgramCode: {
                    required: "Mã không được bỏ trống",
                    maxlength: "Vui lòng không nhập quá 15 ký tự",
                },
            }
        });
        if (FromProgramsCreate.valid()) {
            var StartDate;
            var EndDate;
            var StayStartDate;
            var StayEndDate;
            if ($('#StartDate_EndDate').data('daterangepicker') !== undefined && $('#StartDate_EndDate').data('daterangepicker') != null && isPickerApprove5) {
                StartDate = $('#StartDate_EndDate').data('daterangepicker').startDate._d.toLocaleDateString("en-GB");
                EndDate = $('#StartDate_EndDate').data('daterangepicker').endDate._d.toLocaleDateString("en-GB");
            } else {
                StartDate = null
                EndDate = null
            }
            if ($('#StayDate').data('daterangepicker') !== undefined && $('#StayDate').data('daterangepicker') != null && isPickerApprove7) {
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
                /*ServiceType: $('#ServiceType').val(),*/
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
                        setTimeout(function () {
                            $.magnificPopup.close();
                            window.location.reload();
                        }, 1000)

                    } else {
                        _msgalert.error(result.msg);

                    }
                }
            });
        }
    },
    OnloadHotelName: function () {
        $("#Suppliersid-error").hide()
        var id = $('#ServiceName').select2().val();
        _Programs.HotelSuggestion($('#ServiceName'));
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
                        $("#Suppliersid-error").hide()
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
    Export: function () {
        var searchModel = this.GetParam();
        _global_function.AddLoading()
        $.ajax({
            url: "/Programs/ExportExcel",
            type: "Post",
            data: { searchModel: searchModel, field: ProgramsFields },
            success: function (result) {
                _global_function.RemoveLoading()
                if (result.isSuccess) {
                    _msgalert.success(result.message);
                    window.location.href = result.path;
                } else {
                    _msgalert.error(result.message);
                }
            }
        });
    },

    ShowHideColumn: function () {
        $('.checkbox-tb-column').each(function () {
            let seft = $(this);
            let id = seft.data('id');
            if (seft.is(':checked')) {
                $('td:nth-child(' + id + '),th:nth-child(' + id + ')').removeClass('mfp-hide');
            } else {
                $('td:nth-child(' + id + '),th:nth-child(' + id + ')').addClass('mfp-hide');
            }
        });
    },
    changeSetting: function (position) {
        _Programs.checkShowHide();
        this.ShowHideColumn();
        switch (position) {
            case 1:
                if ($('#ProgramCode').is(":checked")) {
                    ProgramsFields.ProgramCode = true
                } else {
                    ProgramsFields.ProgramCode = false
                }
                this.eraseCookie(cookieName);
                this.setCookie(cookieName, JSON.stringify(ProgramsFields), 12);
                break;
            case 2:
                if ($('#ProgramNamef').is(":checked")) {
                    ProgramsFields.ProgramNamef = true
                } else {
                    ProgramsFields.ProgramNamef = false
                }
                this.eraseCookie(cookieName);
                this.setCookie(cookieName, JSON.stringify(ProgramsFields), 12);
                break;
            //case 3:
            //    if ($('#ServiceType').is(":checked")) {
            //        ProgramsFields.ServiceType = true
            //    } else {
            //        ProgramsFields.ServiceType = false
            //    }
            //    this.eraseCookie(cookieName);
            //    this.setCookie(cookieName, JSON.stringify(ProgramsFields), 12);
            //    break;
            case 3:
                if ($('#ServiceNamef').is(":checked")) {
                    ProgramsFields.ServiceNamef = true
                } else {
                    ProgramsFields.ServiceNamef = false
                }
                this.eraseCookie(cookieName);
                this.setCookie(cookieName, JSON.stringify(ProgramsFields), 12);
                break;
            case 4:
                if ($('#SupplierNamef').is(":checked")) {
                    ProgramsFields.SupplierNamef = true
                } else {
                    ProgramsFields.SupplierNamef = false
                }
                this.eraseCookie(cookieName);
                this.setCookie(cookieName, JSON.stringify(ProgramsFields), 12);
                break;
            case 5:
                if ($('#ApplyDate').is(":checked")) {
                    ProgramsFields.ApplyDate = true
                } else {
                    ProgramsFields.ApplyDate = false
                }
                this.eraseCookie(cookieName);
                this.setCookie(cookieName, JSON.stringify(ProgramsFields), 12);
                break;
            case 6:
                if ($('#UserCreate').is(":checked")) {
                    ProgramsFields.UserCreate = true
                } else {
                    ProgramsFields.UserCreate = false
                }
                this.eraseCookie(cookieName);
                this.setCookie(cookieName, JSON.stringify(ProgramsFields), 12);
                break;
            case 7:
                if ($('#Descriptionf').is(":checked")) {
                    ProgramsFields.Descriptionf = true
                } else {
                    ProgramsFields.Descriptionf = false
                }
                this.eraseCookie(cookieName);
                this.setCookie(cookieName, JSON.stringify(ProgramsFields), 12);
                break;

            case 8:
                if ($('#CreateDatef').is(":checked")) {
                    ProgramsFields.CreateDatef = true
                } else {
                    ProgramsFields.CreateDatef = false
                }
                this.eraseCookie(cookieName);
                this.setCookie(cookieName, JSON.stringify(ProgramsFields), 12);
                break;
            case 9:
                if ($('#UserVerify').is(":checked")) {
                    ProgramsFields.UserVerify = true
                } else {
                    ProgramsFields.UserVerify = false
                }
                this.eraseCookie(cookieName);
                this.setCookie(cookieName, JSON.stringify(ProgramsFields), 12);
                break;
            case 10:
                if ($('#VerifyDate').is(":checked")) {
                    ProgramsFields.VerifyDate = true
                } else {
                    ProgramsFields.VerifyDate = false
                }
                this.eraseCookie(cookieName);
                this.setCookie(cookieName, JSON.stringify(ProgramsFields), 12);
                break;

        }
    },
    checkShowHide: function () {

        if (ProgramsFields.STT === true) {
            $('#STT').prop('checked', true);
        } else {
            $('#STT').prop('checked', false);
        }
        if (ProgramsFields.ProgramCode === true) {
            $('#ProgramCode').prop('checked', true);
        } else {
            $('#ProgramCode').prop('checked', false);
        }
        if (ProgramsFields.ProgramNamef === true) {
            $('#ProgramNamef').prop('checked', true);
        } else {
            $('#ProgramNamef').prop('checked', false);
        }
        if (ProgramsFields.ServiceType === true) {
            $('#ServiceType').prop('checked', true);
        } else {
            $('#ServiceType').prop('checked', false);
        }
        if (ProgramsFields.ServiceNamef === true) {
            $('#ServiceNamef').prop('checked', true);
        } else {
            $('#ServiceNamef').prop('checked', false);
        }
        if (ProgramsFields.SupplierNamef === true) {
            $('#SupplierNamef').prop('checked', true);
        } else {
            $('#SupplierNamef').prop('checked', false);
        }
        if (ProgramsFields.ApplyDate === true) {
            $('#ApplyDate').prop('checked', true);
        } else {
            $('#ApplyDate').prop('checked', false);
        }
        if (ProgramsFields.Descriptionf === true) {
            $('#Descriptionf').prop('checked', true);
        } else {
            $('#Descriptionf').prop('checked', false);
        }
        if (ProgramsFields.UserCreate === true) {
            $('#UserCreate').prop('checked', true);
        } else {
            $('#UserCreate').prop('checked', false);
        }
        if (ProgramsFields.CreateDatef === true) {
            $('#CreateDatef').prop('checked', true);
        } else {
            $('#CreateDatef').prop('checked', false);
        }
        if (ProgramsFields.UserVerify === true) {
            $('#UserVerify').prop('checked', true);
        } else {
            $('#UserVerify').prop('checked', false);
        }
        if (ProgramsFields.VerifyDate === true) {
            $('#VerifyDate').prop('checked', true);
        } else {
            $('#VerifyDate').prop('checked', false);
        }
    },
    eraseCookie: function (name) {
        document.cookie = name + '=; Path=/; Expires=Thu, 01 Jan 1970 00:00:01 GMT;';
    },
    getCookie: function (name) {
        var nameEQ = name + "=";
        var ca = document.cookie.split(';');
        for (var i = 0; i < ca.length; i++) {
            var c = ca[i];
            while (c.charAt(0) == ' ') c = c.substring(1, c.length);
            if (c.indexOf(nameEQ) == 0) return c.substring(nameEQ.length, c.length);
        }
        return null;
    },
    setCookie: function (name, value, days) {
        var expires = "";
        if (days) {
            var date = new Date();
            date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
            expires = "; expires=" + date.toUTCString();
        }
        document.cookie = name + "=" + (value || "") + expires + "; path=/";
    },
};