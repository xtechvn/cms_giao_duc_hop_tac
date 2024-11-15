var programs_package_html = {

    html_programs_package_add_packages_tr: '<tr class="ProgramsPackage-row"><td><input placeholder="Giá nhập" class="form-control currency text-right ProgramsPackage-Amout" id="ProgramsPackage" /></td><td><select style="width:100%;" id="ProgramsPackage"  class="form-control ProgramsPackage-WeekDay " name="ProgramsPackage-WeekDay" ><option value="2">Thứ 2</option><option value="3">Thứ 3</option><option value="4">Thứ 4</option><option value="5">Thứ 5</option><option value="6">Thứ 6</option><option value="7">Thứ 7</option><option value="0">CN</option></select></td><td style="text-align: center;"> <a class="fa fa-trash-o" href="javascript:;" onclick="_ProgramsPackage.DeleteProgramsPackage($(this));"></a></td></tr >',
}
$(document).ready(function () {
    _ProgramsPackage.Init();
});
let isPickerApprove = false;
var _ProgramsPackage = {
    Init: function () {

        objSearch = this.GetParam();
        objSearch.ProgramId = 0;
        objSearch.PageIndex = 1;
        objSearch.PageSize = 20;
        _ProgramsPackage.Search(objSearch);
      
        objSearch2 = {
            ProgramId: 0,
            PageIndex: 1,
            PageSize:10,
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
            ProgramId: 1,
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
    OpenPopup: function (id,type) {
        let title = 'Thêm mới/Cập nhật gói';
        let url = '/ProgramsPackage/DetailProgramsPackage';
        let param = {
        };
        if (id.trim() != "") {
            param = {
                id: id,
                type: type,
            };
        }
        _magnific.OpenSmallPopup(title, url, param);
        _ProgramsPackage.SelectWithFixedOption($(".ProgramsPackage-WeekDay"));
    },
    AddProgramsPackage: function (element) {
        var table_element = element.closest('.ProgramsPackage-tbody')
        table_element.find('.ProgramsPackage-summary-row').before(programs_package_html.html_programs_package_add_packages_tr);
        /*_ProgramsPackage.SelectWithFixedOption($('.programspackage-weekday'))*/

    },
    DeleteProgramsPackage: function (element) {
        var table_element = element.closest('.ProgramsPackage-tbody')
        element.closest('.ProgramsPackage-row').remove()

    },
    loadselect2: function () {
        $('.programspackage-weekday').each(function (index, item) {
            var element = $(item);
            _ProgramsPackage.SelectWithFixedOption(element)
        });
    },
    SelectWithFixedOption: function (element) {
        element.select2({
            theme: 'bootstrap4',
            placeholder: "Thứ ",
            hintText: "Nhập từ khóa tìm kiếm",
            searchingText: "Đang tìm kiếm...",
            maximumSelectionLength: 1,
        });
    },
    SummitProgramsPackage: function (type) {
        var validate_failed = false
        var object_summit = {

            ProgramsPackage: []
        }
        var FromDate; //Ngày bat dau
        var ToDate; //Ngày hết hạn
        if ($('#ApplyDate').data('daterangepicker') !== undefined && $('#ApplyDate').data('daterangepicker') != null && isPickerApprove) {
            FromDate = $('#ApplyDate').data('daterangepicker').startDate._d.toLocaleDateString("en-GB");
            ToDate = $('#ApplyDate').data('daterangepicker').endDate._d.toLocaleDateString("en-GB");
        } else {
            FromDate = null
            ToDate = null
            _msgalert.error("Thời gian áp dụng không được bỏ trống");
            validate_failed = true
            return false;
        }

        $('.ProgramsPackage-row').each(function (index, item) {
            var extra_package_element = $(item);

            var package_amount = extra_package_element.find('.ProgramsPackage-Amout').val();
            var package_WeekDay = extra_package_element.find('.ProgramsPackage-WeekDay').val();


            if (package_amount == null || package_amount.toString() == undefined || package_amount.toString().trim() == '' || package_WeekDay == null || package_WeekDay.toString() == undefined || package_WeekDay.toString().trim() == '') {
                _msgalert.error("Nội dung của giá nhập và theo thứ không được bỏ trống");
                validate_failed = true
                return false;
            }


            var extra_package = {
                ProgramId: $('#ProgramId').val(),
                PackageCode: $('#PackageCode').text(),
                /*ProgramId: $('#ProgramId').val(),*/
                RoomType: $('#RoomType').val(),
                Amount: package_amount,
                FromDateStr: FromDate,
                ToDateStr: ToDate,
                WeekDay: package_WeekDay,
                OpenStatus: $('.optradio').val(),
            }
            if (object_summit.ProgramsPackage.length > 0) {
                object_summit.ProgramsPackage.forEach(function (item, index) {
                    if (item.WeekDay == package_WeekDay) {
                        _msgalert.error("Thứ " + package_WeekDay + " đã tồn tại");
                        validate_failed = true
                        return false;
                    }
                });
            }
            object_summit.ProgramsPackage.push(extra_package);
        });
        if (validate_failed != true) {
            $.ajax({
                url: "/ProgramsPackage/SummitProgramsPackage",
                type: "post",
                data: { data: object_summit.ProgramsPackage, type: type },
                success: function (result) {
                    if (result != undefined && result.status == 0) {
                        _msgalert.success(result.msg);
                        if (type == 0) {
                            window.location.href = '/ProgramsPackage/DetailListProgramsPackage/' + object_summit.ProgramsPackage[0].ProgramId + '/' + object_summit.ProgramsPackage[0].ProgramId.PackageCode.Replace(' / ', '.') + '/0';
                        } else {
                            $.magnificPopup.close();
                            window.location.reload();
                        }
                     
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
    ProgramsDetail: function (id, type) {
        let title = 'Chi tiết gói chương trình';
        let url = '/ProgramsPackage/DetailProgramsPackage';
        let param = {
        };
        if (id.trim() != "") {
            param = {
                id: id,
                type: type,
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
};
