var _customer_manager_html = {
    html_Client_option: '<option class="select2-results__option" value="{Client_id}">{Client_name}</option>',
    html_Client_option_checked: '<option checked="checked" selected="selected" class="select2-results__option" value="{Client_id}">{Client_name}</option>',
    html_Client_option_B2C: '<option class="select2-results__option" value="{Client_id}">{Client_name}</option>',

}
let filter = {
    STT: true,
    ContractNo: true,
    ExpireDate: true,
    ClientId: true,
    ServiceType: true,
    DebtType: true,
    AgencyType: true,
    ClientType: true,
    PermisionType: true,
    SalerId: false,
    CreateDate: true,
    UserIdCreate: false,
    UserIdUpdate: false,
    UpdateLast: false,
    ContractStatus: true,
}
let cookieName = 'contract_filter';
let cookieContractFilterName = 'contractFilter_filter';
let cookieContractClient = 'cookie_ContractClient';
let cookieContractSaler = 'cookie_ContractSaler';
let cookieContractUserCreate = 'cookie_ContractUserCreate';
let cookieContractUserVerify = 'cookie_ContractUserVerify';
let cookieContractNo = 'cookie_ContractNo';
let isPickerApprove = false;
let isPickerApprove2 = false;
let isPickerApprove3 = false;
let C_id = 0;
let isResetTab = false;
let isContractStatus = -1;
let listClientType = [];
let listPermissionType= [];
let listContractStatus = [];
let listDebtType = [];
$(document).ready(function () {

    _Contract.Loaddata();
    $("#ContractNo").select2({
        theme: 'bootstrap4',
        placeholder: "Mã hợp đồng",
        minimumInputLength: 1,
        allowClear: true,
        tags: true,
        ajax: {
            url: "/Contract/ContractNoSuggestion",
            
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
                            text: item.contractno,
                            id: item.contractno,
                        }
                    })
                };
            },
            createTag: function (params) {
                let term = $.trim(params.term);
                return {
                    id: term,
                    text: term,
                    newTag: true,
                }
            },
            cache: true
        }
    });
    $("#client").select2({
        theme: 'bootstrap4',
        placeholder: "Tên KH, Điện Thoại, Email",
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

                // Query parameters will be ?search=[term]&type=public
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
    $("#SalerId").select2({
        theme: 'bootstrap4',
        placeholder: "Người phụ trách, Email",
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
    $("#UserIdCreate").select2({
        theme: 'bootstrap4',
        placeholder: "Người tạo,Email",
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
    $("#UpdatedBy").select2({
        theme: 'bootstrap4',
        placeholder: "Người duyệt,Email",
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
   
    const selectBtnContractStatus = document.querySelector(".select-btn-ContractStatus-type")
    const itemsContractStatus = document.querySelectorAll(".item-ContractStatus-type");
    const selectBtnClientType = document.querySelector(".select-btn-ClientType-type")
    const itemsClientType = document.querySelectorAll(".item-ClientType-type");
    const selectBtnPermisionType = document.querySelector(".select-btn-PermisionType-type")
    const itemsPermisionType = document.querySelectorAll(".item-PermisionType-type");
    const selectBtnDebtType = document.querySelector(".select-btn-DebtType-type")
    const itemsDebtType = document.querySelectorAll(".item-DebtType-type");

    $(document).click(function (event) {
       
        var $target = $(event.target);
        if (!$target.closest('#select-btn-ContractStatus-type').length) {//checkbox_trans_type_
            if ($('#list-item-ContractStatus').is(":visible") && !$target[0].id.includes('ContractStatus_type_text') && !$target[0].id.includes('ContractStatus_type')
                && !$target[0].id.includes('list-item-ContractStatus') && !$target[0].id.includes('checkbox_ContractStatus_type')) {
                selectBtnContractStatus.classList.toggle("open");
            }
        }
        if (!$target.closest('#select-btn-ClientType-type').length) {//checkbox_trans_type_
            if ($('#list-item-ClientType').is(":visible") && !$target[0].id.includes('ClientType_type_text') && !$target[0].id.includes('ClientType_type')
                && !$target[0].id.includes('list-item-ClientType') && !$target[0].id.includes('checkbox_ClientType_type')) {
                selectBtnClientType.classList.toggle("open");
            }
        }
        if (!$target.closest('#select-btn-PermisionType-type').length) {//checkbox_trans_type_
            if ($('#list-item-PermisionType').is(":visible") && !$target[0].id.includes('PermisionType_type_text') && !$target[0].id.includes('PermisionType_type')
                && !$target[0].id.includes('list-item-ClientType') && !$target[0].id.includes('checkbox_PermisionType_type')) {
                selectBtnPermisionType.classList.toggle("open");
            }
        }
        if (!$target.closest('#select-btn-DebtType-type').length) {//checkbox_trans_type_
            if ($('#list-item-DebtType').is(":visible") && !$target[0].id.includes('DebtType_type_text') && !$target[0].id.includes('DebtType_type')
                && !$target[0].id.includes('list-item-DebtType') && !$target[0].id.includes('checkbox_DebtType_type')) {
                selectBtnDebtType.classList.toggle("open");
            }
        }

    });
    selectBtnContractStatus.addEventListener("click", (e) => {
        e.preventDefault();
        selectBtnContractStatus.classList.toggle("open");
    });
    selectBtnClientType.addEventListener("click", (e) => {
        e.preventDefault();
        selectBtnClientType.classList.toggle("open");
    });
    selectBtnPermisionType.addEventListener("click", (e) => {
        e.preventDefault();
        selectBtnPermisionType.classList.toggle("open");
    });
    selectBtnDebtType.addEventListener("click", (e) => {
        e.preventDefault();
        selectBtnDebtType.classList.toggle("open");
    });
    itemsContractStatus.forEach(item => {
        item.addEventListener("click", () => {
            item.classList.toggle("checked");
            let checked = document.querySelectorAll(".checked"),
                btnText = document.querySelector(".btn-text-ContractStatus-type");
            listContractStatus = []
            let checked_list = []

            for (var i = 0; i < checked.length; i++) {
                id = checked[i].getAttribute('id')
                if (id.includes('ContractStatus_type_')) {
                    checked_list.push(checked[i]);
                    listContractStatus.push(parseInt(id.replace('ContractStatus_type_', '')))
                }
            }
            _Contract.SearchData.ContractStatus = listContractStatus

            if (checked_list && checked_list.length > 0) {
                btnText.innerText = `${checked_list.length} Selected`;
            } else {
                btnText.innerText = "Tất cả trạng thái";
            }
        });
    })
    itemsClientType.forEach(item => {
        item.addEventListener("click", () => {
            item.classList.toggle("checked");
            let checked = document.querySelectorAll(".checked"),
                btnText = document.querySelector(".btn-text-ClientType-type");
            listClientType = []
            let checked_list = []

            for (var i = 0; i < checked.length; i++) {
                id = checked[i].getAttribute('id')
                if (id.includes('ClientType_type_')) {
                    checked_list.push(checked[i]);
                    listClientType.push(parseInt(id.replace('ClientType_type_', '')))
                }
            }
            _Contract.SearchData.ClientType = listClientType

            if (checked_list && checked_list.length > 0) {
                btnText.innerText = `${checked_list.length} Selected`;
            } else {
                btnText.innerText = "Tất cả loại khách hàng";
            }
        });
    })
    itemsPermisionType.forEach(item => {
        item.addEventListener("click", () => {
            item.classList.toggle("checked");
            let checked = document.querySelectorAll(".checked"),
                btnText = document.querySelector(".btn-text-PermisionType-type");
            listPermissionType = []
            let checked_list = []

            for (var i = 0; i < checked.length; i++) {
                id = checked[i].getAttribute('id')
                if (id.includes('PermisionType_type_')) {
                    checked_list.push(checked[i]);
                    listPermissionType.push(parseInt(id.replace('PermisionType_type_', '')))
                }
            }
            _Contract.SearchData.PermisionType = listPermissionType

            if (checked_list && checked_list.length > 0) {
                btnText.innerText = `${checked_list.length} Selected`;
            } else {
                btnText.innerText = "Tất cả nhóm khách hàng";
            }
        });
    })
    itemsDebtType.forEach(item => {
        item.addEventListener("click", () => {
            item.classList.toggle("checked");
            let checked = document.querySelectorAll(".checked"),
                btnText = document.querySelector(".btn-text-DebtType-type");
            listDebtType = []
            let checked_list = []

            for (var i = 0; i < checked.length; i++) {
                id = checked[i].getAttribute('id')
                if (id.includes('DebtType_type_')) {
                    checked_list.push(checked[i]);
                    listDebtType.push(parseInt(id.replace('DebtType_type_', '')))
                }
            }
            _Contract.SearchData.PermisionType = listDebtType

            if (checked_list && checked_list.length > 0) {
                btnText.innerText = `${checked_list.length} Selected`;
            } else {
                btnText.innerText = "Tất cả loại cong nợ";
            }
        });
    })

    _Contract.checkShowHide();

});

var _Contract = {

    Loaddata: function () {
        let _searchModel = {

            PageIndex: 1,
            PageSize: 20,
        };
        var objSearch = this.GetParam(true);;
        objSearch.PageIndex = 1;
        objSearch.PageSize = 10;
        objSearch = _searchModel;
        if (_Contract.getCookie(cookieContractFilterName) != null)
        {
            let cookieContract = _Contract.getCookie(cookieContractFilterName)
            objSearch = JSON.parse(cookieContract)

            if (objSearch.ClientType.length > 0 ) {
                var btnTextClientType = document.querySelector(".btn-text-ClientType-type");
                btnTextClientType.innerText = `${objSearch.ClientType.length} Selected`;
                for (var i = 0; i < objSearch.ClientType.length; i++) {
                    $('#ClientType_type_' + objSearch.ClientType[i] + '').addClass('checked')
                 
                }
                objSearch.ClientType = objSearch.ClientType.toString();
            }
            if (objSearch.DebtType.length > 0) {
                var btnTextDebtType = document.querySelector(".btn-text-DebtType-type");
                btnTextDebtType.innerText = `${objSearch.DebtType.length} Selected`;
                for (var i = 0; i < objSearch.DebtType.length; i++) {
                    $('#DebtType_type_' + objSearch.DebtType[i] + '').addClass('checked')
                   
                }
               
                objSearch.DebtType = objSearch.DebtType.toString();
            }
            if (objSearch.ContractStatus.length > 0) {
                var btnTextContractStatus = document.querySelector(".btn-text-ContractStatus-type");
                btnTextContractStatus.innerText = `${objSearch.ContractStatus.length} Selected`;
                for (var i = 0; i < objSearch.ContractStatus.length; i++) {
                    $('#ContractStatus_type_' + objSearch.ContractStatus[i] + '').addClass('checked')

                }

                objSearch.ContractStatus = objSearch.ContractStatus.toString();
            }
            if (objSearch.PermissionType.length > 0) {
                var btnTextPermissionType = document.querySelector(".btn-text-PermisionType-type");
                btnTextPermissionType.innerText = `${objSearch.PermissionType.length} Selected`;
                for (var i = 0; i < objSearch.PermissionType.length; i++) {
                    $('#PermisionType_type_' + objSearch.PermissionType[i] + '').addClass('checked')

                }

                objSearch.PermissionType = objSearch.PermissionType.toString();
            }
            if (window.localStorage.getItem(cookieContractNo) != null) {
                var cookie2 = window.localStorage.getItem(cookieContractNo)
                var clientName = JSON.parse(cookie2)
                $('#ContractNo').html('<option selected value = ' + clientName.id + '> ' + clientName.nameselect + '</option>')
            }
            if (window.localStorage.getItem(cookieContractClient) != null) {
                var cookie2 = window.localStorage.getItem(cookieContractClient)
                var clientName = JSON.parse(cookie2)
                $('#client').html('<option selected value = ' + clientName.id + '> ' + clientName.nameselect + '</option>')
            }
            if (window.localStorage.getItem(cookieContractSaler) != null) {
                var cookie2 = window.localStorage.getItem(cookieContractSaler)
                var clientName = JSON.parse(cookie2)
                $('#SalerId').html('<option selected value = ' + clientName.id + '> ' + clientName.nameselect + '</option>')
            }
            if (window.localStorage.getItem(cookieContractUserCreate) != null) {
                var cookie2 = window.localStorage.getItem(cookieContractUserCreate)
                var clientName = JSON.parse(cookie2)
                $('#UserIdCreate').html('<option selected value = ' + clientName.id + '> ' + clientName.nameselect + '</option>')
            }
            if (window.localStorage.getItem(cookieContractUserVerify) != null) {
                var cookie2 = window.localStorage.getItem(cookieContractUserVerify)
                var clientName = JSON.parse(cookie2)
                $('#UpdatedBy').html('<option selected value = ' + clientName.id + '> ' + clientName.nameselect + '</option>')
            }
        }

        this.Search(objSearch);
        this.OnCountStatus(objSearch);


    },
    OpenPopup: function (id) {
        let title = 'Thêm mới/Cập nhật hợp đồng';
        let url = '/Contract/Detail';
        let param = {
        };
        if (id.trim() != "") {
            param = {
                id: id,
            };
        }
        _magnific.OpenSmallPopup(title, url, param);
        
    },
    OpenPopupStatus: function (id, status) {
        let title;
        C_id = id;
        if (status == 4) {
            title = 'Hủy bỏ hợp đồng';
        } else {
            title = 'Từ chối hợp đồng';
        }
   
        let url = '/Contract/ContractStatusDetail';
        let param = {
            id: status
        };
       
        _magnific.OpenSmallPopup(title, url, param);

    },
    Search: function (input) {
        window.scrollTo(0, 0);
        $.ajax({
            url: "/Contract/Search",
            type: "Post",
            data: input,
            success: function (result) {
                $('#imgLoading_Search').hide();
                $('#grid_data_Search').html(result);
                $('.checkbox-tb-column').each(function () {
                    let seft = $(this);
                    let id = seft.data('id');
                    if (seft.is(':checked')) {
                        $('td:nth-child(' + id + '),th:nth-child(' + id + ')').removeClass('mfp-hide');
                    } else {
                        $('td:nth-child(' + id + '),th:nth-child(' + id + ')').addClass('mfp-hide');
                    }
                });
                $('#selectPaggingOptions').val(input.PageSize).attr("selected", "selected");
             
            }
        });
    },
    OnPaging: function (value) {
        let _searchModel = {
            currentPage: value,
        };
        var objSearch = this.SearchParam
        objSearch = this.GetParam(true)
        objSearch.PageIndex = value
        objSearch.currentPage = value
        this.Search(objSearch);
    },
    OnloadPolicy: function () {
        var type = 0;
        var a = $('#ClienType_Name').val();
        var ServiceType = $('#ServiceType').select2("val");
        if (ServiceType != undefined && ServiceType != null) ServiceType = ServiceType.toString();
        var PermisionType = $('#PermisionType_Name').val();
        if (PermisionType == 1 || PermisionType == 2) {
            $.ajax({
                url: "/Contract/loadPolicy",
                type: "Post",
                data: { ClientType: a, PermisionType: PermisionType, type: type },
                success: function (result) {
                    $('#grid_data_loadPolicy').show().html(result);
                }
            });
        
        }
        else {
            $('#grid_data_loadPolicy').hide();
        }
        
    },
    CheckSeverType: function () {
        var a = $('#ClienType_Name').val();
        var ServiceType = $('#ServiceType').select2("val");
      
        var PermisionType = $('#PermisionType_Name').val();
        if (ServiceType != undefined && ServiceType != null && PermisionType == 1) {
            if (ServiceType.includes("1")) {
                $('.HotelDebtAmoutView').removeClass('mfp-hide');

            } else {
                $('.HotelDebtAmoutView').addClass('mfp-hide');;
            }
            if (ServiceType.includes("3")) {
                $('.ProductFlyTicketDebtAmountView').removeClass('mfp-hide');
            } else {
                $('.ProductFlyTicketDebtAmountView').addClass('mfp-hide');;
            }
            if (ServiceType.includes("4") || ServiceType.includes("9")) {
                $('.TouringCarDebtAmountView').removeClass('mfp-hide');

            } else {
                $('.TouringCarDebtAmountView').addClass('mfp-hide');;
            }
            if (ServiceType.includes("5")) {
                $('.TourDebtAmountView').removeClass('mfp-hide');

            } else {
                $('.TourDebtAmountView').addClass('mfp-hide');;
            }
            if (ServiceType.includes("6")) {
                $('.VinWonderDebtAmountView').removeClass('mfp-hide');
            } else {
                $('.VinWonderDebtAmountView').addClass('mfp-hide');;
            }
        }
        if (ServiceType != undefined && ServiceType != null && PermisionType == 2) {
            if (ServiceType.includes("1")) {
                $('.HotelDepositAmoutView').removeClass('mfp-hide');
            } else {
                $('.HotelDepositAmoutView').addClass('mfp-hide');;
            }
            if (ServiceType.includes("3")) {
                $('.ProductFlyTicketDepositAmountView').removeClass('mfp-hide');

            } else {
                $('.ProductFlyTicketDepositAmountView').addClass('mfp-hide');;
            }
            if (ServiceType.includes("4") || ServiceType.includes("9")) {
                $('.TouringCarDepositAmountView').removeClass('mfp-hide');

            } else {
                $('.TouringCarDepositAmountView').addClass('mfp-hide');;
            }
            if (ServiceType.includes("5")) {
                $('.TourDepositAmountView').removeClass('mfp-hide');

            } else {
                $('.TourDepositAmountView').addClass('mfp-hide');;
            }
            if (ServiceType.includes("6")) {
                $('.VinWonderDepositAmountView').removeClass('mfp-hide');
            }
            else {
                $('.VinWonderDepositAmountView').addClass('mfp-hide');;
            }
        }
    },
    CreateContract: function (value) {
        let FromCreate = $('#Contract_form');
        FromCreate.validate({
            rules: {

                "ClientId": "required",
                "ServiceType": {
                    required: true,
                  
                },
                "ClienType_Name": {
                    required: true,
                
                },
                "PermisionType_Name": {
                    required: true,
                   
                },
                "ExpireDate": "required",
                "DebtType_Type": "required",
                "ProductFlyTicketDepositAmount": {
                    required: true,
              
                    maxlength: 13,
                },
                "HotelDepositAmout": {
                    required: true,
                  
                    maxlength: 13,
                },
                "ProductFlyTicketDebtAmount": {
                    required: true,
               
                    maxlength: 13,
                },
                "HotelDebtAmout": {
                    required: true,
                   
                    maxlength: 13,
                },

                "TourDebtAmount": {
                    required: true,

                    maxlength: 13,
                },
                "TourDepositAmount": {
                    required: true,

                    maxlength: 13,
                },
                "TouringCarDebtAmount": {
                    required: true,

                    maxlength: 13,
                },
                "TourDepositAmount": {
                    required: true,

                    maxlength: 13,
                },
                "VinWonderDebtAmount": {
                    required: true,

                    maxlength: 13,
                },
                "VinWonderDepositAmount": {
                    required: true,

                    maxlength: 13,
                },

            },
            messages: {

                "ClientId": "Khách hàng không được bỏ trống",
                "ServiceType": "Dịch vụ không được bỏ trống",
                "ClienType_Name": "Loại khách hàng không được bỏ trống",
                "PermisionType_Name": "Nhóm khách hàng không được bỏ trống",
                "ExpireDate": "Ngày hết hạn không được bỏ trống",
                "DebtType_Type": "Vui lòng chọn loại công nợ",
                "ProductFlyTicketDepositAmount": {
                    required: "Số dư ký quỹ VMB tối thiểu không được bỏ trống",
                    
                    maxlength: "Vui lòng không nhập quá 10 số",
                },
                "HotelDepositAmout": {
                    required: " Số dư ký quỹ KS tối thiểu không được bỏ trống",
                   
                    maxlength: "Vui lòng không nhập quá 10 số",
                },
                "ProductFlyTicketDebtAmount": {
                    required: "Hạn mức công nợ VMB không được bỏ trống",
                    
                    maxlength: "Vui lòng không nhập quá 10 số",
                },
                "HotelDebtAmout": {
                    required: "Hạn mức công nợ KS không được bỏ trống",
                   
                    maxlength: "Vui lòng không nhập quá 10 số",
                },
                "TourDebtAmount": {
                    required: "Hạn mức công nợ Tour không được bỏ trống",

                    maxlength: "Vui lòng không nhập quá 10 số",
                },
                "TourDepositAmount": {
                    required: "Số dư ký quỹ Tour không được bỏ trống",

                    maxlength: "Vui lòng không nhập quá 10 số",
                },
                "TouringCarDebtAmount": {
                    required: "Hạn mức công nợ thuê xe du lịch không được bỏ trống",

                    maxlength: "Vui lòng không nhập quá 10 số",
                },
                "TouringCarDepositAmount": {
                    required: "Số dư ký quỹ thuê xe du lịch không được bỏ trống",

                    maxlength: "Vui lòng không nhập quá 10 số",
                },
                "VinWonderDebtAmount": {
                    required: "Hạn mức công nợ VinWonder không được bỏ trống",

                    maxlength: "Vui lòng không nhập quá 10 số",
                },
                "VinWonderDepositAmount": {
                    required: "HSố dư ký quỹVinWonder không được bỏ trống",

                    maxlength: "Vui lòng không nhập quá 10 số",
                },

            }
        });
        if (FromCreate.valid()) {
            var a = $('#ClientId').select2("val");
            var ServiceType = $('#ServiceType').select2("val").toString();
            var c = $('#ExpireDate').val();
            var ProductFlyTicketDebtAmount = null;
            var HotelDebtAmout = null;
            var ProductFlyTicketDepositAmount = null;
            var HotelDepositAmout = null;
            var Private = 0;
            var VinWonderDepositAmount = null;
            var VinWonderDebtAmount = null;
            var TouringCarDepositAmount = null;
            var TouringCarDebtAmount = null;
            var TourDepositAmount = null;
            var TourDebtAmount = null;
            if ($('#IsPrivate').val() != undefined)
            if (document.getElementById("IsPrivate").checked == true) {
                Private = 1;
            }
            if ($('#HotelDepositAmout').val() != undefined) { HotelDepositAmout = $('#HotelDepositAmout').val().replaceAll(',', '')}
            if ($('#ProductFlyTicketDepositAmount').val() != undefined) { ProductFlyTicketDepositAmount = $('#ProductFlyTicketDepositAmount').val().replaceAll(',', '')}
            if ($('#HotelDebtAmout').val() != undefined) { HotelDebtAmout = $('#HotelDebtAmout').val().replaceAll(',', '')}
            if ($('#ProductFlyTicketDebtAmount').val() != undefined) { ProductFlyTicketDebtAmount = $('#ProductFlyTicketDebtAmount').val().replaceAll(',', '') }

            if ($('#TourDebtAmount').val() != undefined) { TourDebtAmount = $('#TourDebtAmount').val().replaceAll(',', '') }
            if ($('#TourDepositAmount').val() != undefined) { TourDepositAmount = $('#TourDepositAmount').val().replaceAll(',', '') }
            if ($('#TouringCarDebtAmount').val() != undefined) { TouringCarDebtAmount = $('#TouringCarDebtAmount').val().replaceAll(',', '') }
            if ($('#TouringCarDepositAmount').val() != undefined) { TouringCarDepositAmount = $('#TouringCarDepositAmount').val().replaceAll(',', '') }
            if ($('#VinWonderDebtAmount').val() != undefined) { VinWonderDebtAmount = $('#VinWonderDebtAmount').val().replaceAll(',', '') }
            if ($('#VinWonderDepositAmount').val() != undefined) { VinWonderDepositAmount = $('#VinWonderDepositAmount').val().replaceAll(',', '') }

            var model = {
                ContractExpireDate: $('#ExpireDate').val(),
                ServiceType: ServiceType,
                ClientId: a[0],
                Note: $('#Note').val(),
                PolicyId: $('#PolicyId').val(),
                PermisionType_Name: $('#PermisionType_Name').val(),
                ClientId_Name: $('#ClienType_Name').val(),
                ContractId: $('#ContractId').val(),
                ContractStatus: value,
                ProductFlyTicketDebtAmount: ProductFlyTicketDebtAmount ,
                HotelDebtAmout: HotelDebtAmout,
                ProductFlyTicketDepositAmount: ProductFlyTicketDepositAmount ,
                HotelDepositAmout: HotelDepositAmout,
                TourDebtAmount: TourDebtAmount,
                TourDepositAmount: TourDepositAmount,
                TouringCarDebtAmount: TouringCarDebtAmount ,
                TouringCarDepositAmount: TouringCarDepositAmount ,
                VinWonderDebtAmount: VinWonderDebtAmount,
                VinWonderDepositAmount: VinWonderDepositAmount ,
                IsPrivate: Private,
                DebtType: $('#DebtType_Type').val(),
            }
            if (model.IsPrivate == 1) {
                model.PolicyId = 0;
            }
            $('#CreateContract').attr("disabled", false);
            _global_function.AddLoading()
            $.ajax({
                url: "/Contract/Setup",
                type: "Post",
                data: { model },
                success: function (result) {
                    if (result.stt_code == 0) {
                        _global_function.RemoveLoading()
                        _msgalert.success(result.msg);
                        $.magnificPopup.close();
                        _Contract.Loaddata();
                    } else {
                        _global_function.RemoveLoading()
                        _msgalert.error(result.msg);
                        $('#CreateContract').attr("disabled", true);
                    }
                }
            });
        }
       

    },
    UpdataContract: function (value) {
        let FromCreate = $('#Contract_form');
        FromCreate.validate({
            rules: {

                "ClientId": "required",
                "ServiceType": {
                    required: true,

                },
                "ClienType_Name": {
                    required: true,

                },
                "PermisionType_Name": {
                    required: true,

                },
                "ExpireDate": "required",


            },
            messages: {

                "ClientId": "Khách hàng không được bỏ trống",
                "ServiceType": "Dịch vụ không được bỏ trống",
                "ClienType_Name": "Loại khách hàng không được bỏ trống",
                "PermisionType_Name": "Nhóm khách hàng không được bỏ trống",
                "ExpireDate": "Ngày hết hạn không được bỏ trống",
            }
        });
        if (FromCreate.valid()) {
            var a = $('#ClientId').select2("val");
            var ServiceType = $('#ServiceType').select2("val").toString();
            var c = $('#ExpireDate').val();
            var model = {
                ContractExpireDate: $('#ExpireDate').val(),
                ContractNo: $('#ContractNo').val(),
                ServiceType: ServiceType,
                ClientId: a[0],
                Note: $('#Note').val(),
                PolicyId: $('#PolicyId').val(),
                PermisionType_Name: $('#PermisionType_Name').val(),
                ClientId_Name: $('#ClienType_Name').val(),
                ContractId: $('#ContractId').val(),
                ContractStatus: value,
            }

            $.ajax({
                url: "/Contract/Setup",
                type: "Post",
                data: { model },
                success: function (result) {
                    if (result.stt_code == 0) {
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
    OnloadClientSever: function () {
        var a = $('#ClientId').select2("val");
        if (a != null) {
        var client = a[0];
        
        $.ajax({
            url: "/Contract/ClientDetail",
            type: "Post",
            data: { client },
            success: function (data) {
                
                if (data.stt_code == 0) {
                    $("#ClienType_Name option").remove();
                    $("#PermisionType_Name option").remove();
                    var x = document.getElementById("ClienType_Name").options.length;
                    var y = document.getElementById("PermisionType_Name").options.length;
                    
                    $.ajax({
                        url: "/CustomerManager/ListClientType",
                        data: { id:0 },
                        type: "post",
                        success: function (result) {
                            if (result != undefined && result.data != undefined && result.data.length > 0) {
                               
                                if (x < result.data.length) {
                                 
                                    result.data.forEach(function (item) {
                                      
                                        if (item.codeValue != 5 && item.codeValue!=6) {
                                            if (item.codeValue == data.stt_ClientType) {
                                                
                                                $('#ClienType_Name').append(_customer_manager_html.html_Client_option_checked.replaceAll('{Client_id}', item.codeValue).replace('{Client_name}', item.description))
                                               
                                            } else {
                                                $('#ClienType_Name').append(_customer_manager_html.html_Client_option.replaceAll('{Client_id}', item.codeValue).replace('{Client_name}', item.description))

                                            }
                                        }

                                    });
                                    $('#ClienType_Name').append('<option value="">Tất cả loại khách hàng</option>')
                                }
                                _Contract.OnloadPolicy();
                            }
                            else {
                                $("#ClienType_Name").trigger('change');

                            }
                           
                        }
                    });
                    $.ajax({
                        url: "/CustomerManager/ListClientType",
                        type: "post",
                        data: { id: 1 },
                        success: function (result) {
                            if (result != undefined && result.data != undefined && result.data.length > 0) {
                         
                                if (y < result.data.length) {

                                    result.data.forEach(function (item) {

                                        if (item.codeValue != 5) {
                                            if (item.codeValue == data.stt_PermisionType) {

                                                $('#PermisionType_Name').append(_customer_manager_html.html_Client_option_checked.replaceAll('{Client_id}', item.codeValue).replace('{Client_name}', item.description))

                                            } else {
                                                $('#PermisionType_Name').append(_customer_manager_html.html_Client_option.replaceAll('{Client_id}', item.codeValue).replace('{Client_name}', item.description))

                                            }
                                        }

                                    });
                                    $('#PermisionType_Name').append('<option value="">Tất cả nhóm khách hàng</option>')
                                }
                                _Contract.OnloadPolicy();
                            }
                            else {
                                $("#PermisionType_Name").trigger('change');

                            }

                        }
                    });
                }
                if (data.stt_code == 1) {
                    $("#ClienType_Name option").remove();
                    $("#PermisionType_Name option").remove();
                    var x = document.getElementById("ClienType_Name").options.length;
                    var y = document.getElementById("PermisionType_Name").options.length;
                    
                    $.ajax({
                        url: "/CustomerManager/ListClientType",
                        type: "post",
                        data: { id: 0 },
                        success: function (result) {
                            if (result != undefined && result.data != undefined && result.data.length > 0) {

                                if (x < result.data.length) {
                                    
                                    result.data.forEach(function (item) {
                                        
                                        if (item.codeValue != 5 && item.codeValue != 6) {
                                           
                                            if (item.codeValue == data.stt_ClientType) {
                                                $('#ClienType_Name').append(_customer_manager_html.html_Client_option_checked.replaceAll('{Client_id}', item.codeValue).replace('{Client_name}', item.description))
                                               
                                            } else {
                                                $('#ClienType_Name').append(_customer_manager_html.html_Client_option.replaceAll('{Client_id}', item.codeValue).replace('{Client_name}', item.description))
                                            }
                                        } else {
                                        //    $('#ClienType_Name').append(_customer_manager_html.html_Client_option_B2C.replaceAll('{Client_id}', item.codeValue).replace('{Client_name}', item.description))
                                        }

                                    });
                                    $('#ClienType_Name').append('<option class="select2-results__option" value="">Tất cả loại khách hàng</option>')
                                    $("#ClienType_Name").trigger('change');

                                }
                                _Contract.OnloadPolicy();
                            }
                            else {
                                $("#ClienType_Name").trigger('change');

                            }

                        }
                    });
                    $.ajax({
                        url: "/CustomerManager/ListClientType",
                        data: { id: 1 },
                        type: "post",
                        success: function (result) {
                            if (result != undefined && result.data != undefined && result.data.length > 0) {

                                if (y < result.data.length) {

                                    result.data.forEach(function (item) {

                                        if (item.codeValue != 5) {

                                            if (item.codeValue == data.stt_PermisionType) {
                                                $('#PermisionType_Name').append(_customer_manager_html.html_Client_option_checked.replaceAll('{Client_id}', item.codeValue).replace('{Client_name}', item.description))

                                            } else {
                                                $('#PermisionType_Name').append(_customer_manager_html.html_Client_option.replaceAll('{Client_id}', item.codeValue).replace('{Client_name}', item.description))
                                            }
                                        } else {
                                            //    $('#ClienType_Name').append(_customer_manager_html.html_Client_option_B2C.replaceAll('{Client_id}', item.codeValue).replace('{Client_name}', item.description))
                                        }

                                    });
                                    $('#PermisionType_Name').append('<option class="select2-results__option" value="">Tất cả nhóm khách hàng</option>')
                                    $("#PermisionType_Name").trigger('change');

                                }
                                _Contract.OnloadPolicy();
                            }
                            else {
                                $("#PermisionType_Name").trigger('change');

                            }

                        }
                    });
                }
               
            }
        });
        }
      
    },
     SearchData: function () {
         var CreateDateFrom; //Ngày tạo từ--
         var CreateDateTo; //Ngày tạo từ--
         var ExpireDateFrom; //Ngày tạo từ--
         var VerifyDateTo; //Ngày tạo từ--
         var ExpireDateFrom; //Ngày hết hạn 
         var ExpireDateTo; //Ngày hết hạn 
         var ClientId_data = $('#client').select2("val");
         var SalerId_data = $('#SalerId').select2("val");
         var UserCreate_data = $('#UserIdCreate').select2("val");
         var UserVerify_data = $('#UpdatedBy').select2("val");
         textNV = $('#CreatedBy').find(':selected').text();

         if ($('#ExpireDate').data('daterangepicker') !== undefined && $('#ExpireDate').data('daterangepicker') != null && isPickerApprove) {
             ExpireDateFrom = $('#ExpireDate').data('daterangepicker').startDate._d.toLocaleDateString("en-GB");
             ExpireDateTo = $('#ExpireDate').data('daterangepicker').endDate._d.toLocaleDateString("en-GB");
         } else {
             ExpireDateFrom = null
             ExpireDateTo = null
         }

         if ($('#CreateDate').data('daterangepicker') !== undefined &&  $('#CreateDate').data('daterangepicker') != null && isPickerApprove2) {
             CreateDateFrom = $('#CreateDate').data('daterangepicker').startDate._d.toLocaleDateString("en-GB");
             CreateDateTo = $('#CreateDate').data('daterangepicker').endDate._d.toLocaleDateString("en-GB");
        } else {
            CreateDateFrom = null
            CreateDateTo = null
         }

         if ($('#VerifyDate').data('daterangepicker') !== undefined && $('#VerifyDate').data('daterangepicker') != null && isPickerApprove3) {
             VerifyDateFrom = $('#VerifyDate').data('daterangepicker').startDate._d.toLocaleDateString("en-GB");
             VerifyDateTo = $('#VerifyDate').data('daterangepicker').endDate._d.toLocaleDateString("en-GB");
         } else {
             VerifyDateFrom = null
             VerifyDateTo = null
         }
         let _searchModel = {
            ClientId: null,
            SalerId: null,
            UserCreate: null,
             UserVerify: null,
             ContractNo: $('#ContractNo').val(),
            ExpireDateFrom: ExpireDateFrom,
             ExpireDateTo: ExpireDateTo,
             DebtType: listDebtType,
             ClientAgencyType: $('#AgencyType').val(),
             ContractExpire: $('#ContractExpire').val(),
             ClientType: listClientType,
             ContractStatus: listContractStatus,
             PermissionType: listPermissionType,
            VerifyStatus: $('#VerifyStatus').val(),
            CreateDateFrom: CreateDateFrom,
            CreateDateTo: CreateDateTo,
            VerifyDateFrom: VerifyDateFrom,
            VerifyDateTo: VerifyDateTo,
            PageIndex: 1,
            PageSize: $("#selectPaggingOptions").find(':selected').val(),
         };
         if (_searchModel.ContractNo != null) {
             
             var cookieContractname = {
                 id: _searchModel.ContractNo,
                 nameselect: _searchModel.ContractNo
             }
             _Contract.eraseCookie(cookieContractNo)

             window.localStorage.setItem(cookieContractNo, JSON.stringify(cookieContractname))
         } else {
             window.localStorage.removeItem(cookieContractNo)

         }
         if (ClientId_data != null) {
             _searchModel.ClientId = ClientId_data[0]
             var cookieContractname = {
                 id: ClientId_data[0],
                 nameselect: $('#client').select2('data')[0].text
             }
             _Contract.eraseCookie(cookieContractClient)

             window.localStorage.setItem(cookieContractClient, JSON.stringify(cookieContractname))
         } else {
             window.localStorage.removeItem(cookieContractClient)

         }
         if (SalerId_data != null) {
             _searchModel.SalerId = SalerId_data[0]
             var cookieContractname = {
                 id: SalerId_data[0],
                 nameselect: $('#SalerId').select2('data')[0].text
             }
             _Contract.eraseCookie(cookieContractSaler)

             window.localStorage.setItem(cookieContractSaler, JSON.stringify(cookieContractname))
         } else {
             window.localStorage.removeItem(cookieContractSaler)

         }
         if (UserCreate_data != null) {
             _searchModel.UserCreate = UserCreate_data[0]
             var cookieContractname = {
                 id: UserCreate_data[0],
                 nameselect: $('#UserIdCreate').select2('data')[0].text
             }
             _Contract.eraseCookie(cookieContractUserCreate)

             window.localStorage.setItem(cookieContractUserCreate, JSON.stringify(cookieContractname))
         } else {
             window.localStorage.removeItem(cookieContractUserCreate)

         }
         if (UserVerify_data != null) {
             _searchModel.UserVerify = UserVerify_data[0]
             var cookieContractname = {
                 id: UserVerify_data[0],
                 nameselect: $('#UpdatedBy').select2('data')[0].text
             }
             _Contract.eraseCookie(cookieContractUserVerify)

             window.localStorage.setItem(cookieContractUserVerify, JSON.stringify(cookieContractname))
         } else {
             window.localStorage.removeItem(cookieContractUserVerify)

         }
         var objSearch = this.SearchParam;
         if (_searchModel.PageSize == undefined) {
             _searchModel.PageSize = 10;
         }
         objSearch = _searchModel;
         this.SetActive(-1);
         this.saveCookieFilter(objSearch);
         _searchModel.listDebtType = listDebtType.toString()
         _searchModel.ClientType = listClientType.toString()
         _searchModel.ContractStatus = listContractStatus.toString()
         _searchModel.PermissionType = listPermissionType.toString()
         this.Search(objSearch);
         this.OnCountStatus(objSearch);
   

    },
    OnResetStatus: function (id, status) {
        let title = 'Xác nhận duyệt hợp đồng này';
        let description = 'Bạn xác nhận muốn duyệt hợp đồng này?';
        if (id.trim() == 0) {
            let FromCreate = $('#form_Note');
            FromCreate.validate({
                rules: {

                    "Note_txt": "required",
                },
                messages: {

                    "Note_txt": "Không dược bỏ trống lý do",
                }
            });
            if (FromCreate.valid()) {

                id = C_id;
                title = 'Xác nhận từ chối hợp đồng';
                description = 'Bạn xác nhận muốn từ chối hợp đồng này?';
                if (status) {
                    title = 'Xác hủy bỏ hợp đồng';
                    description = 'Bạn xác nhận muốn hủy bỏ hợp đồng này?';
                }
            } else {
                return;
            }
        }
        var Note = $('#Note_txt').val();
        _msgconfirm.openDialog(title, description, function () {
            $.ajax({
                url: "/Contract/ResetStatus",
                type: "post",
                data: { id: id, Status: status, Note },
                success: function (result) {
                    if (result.status == 0) {
                        _msgalert.success(result.msg);
                        window.location.reload();

                    } else {
                        _msgalert.error(result.msg);
                    }
                }
            });
            
        });
    },
    OnDelete: function (id) {
        let title = 'Xác nhận xóa hợp đồng';
        let description = 'Bạn xác nhận muốn xóa hợp đồng này?';
        _msgconfirm.openDialog(title, description, function () {
            $.ajax({
                url: "/Contract/DeleteContract",
                type: "post",
                data: { id: id },
                success: function (result) {
                    if (result.status == 0) {
                        _msgalert.success(result.msg);
                        window.location.href = "/Contract/Index";

                    } else {
                        _msgalert.error(result.msg);
                    }
                }
            });

        });
    },
    GetParam: function (isFirstTime = false) {
        firstTime = false
        var CreateDateFrom; //Ngày tạo từ--
        var CreateDateTo; //Ngày tạo từ--
        var ExpireDateFrom; //Ngày tạo từ--
        var VerifyDateTo; //Ngày tạo từ--
        var ExpireDateFrom; //Ngày hết hạn 
        var ExpireDateTo; //Ngày hết hạn 
        var ClientId_data = $('#client').select2("val");
        var SalerId_data = $('#SalerId').select2("val");
        var UserCreate_data = $('#UserIdCreate').select2("val");
        var UserVerify_data = $('#UpdatedBy').select2("val");
        textNV = $('#CreatedBy').find(':selected').text();

        if ($('#ExpireDate').data('daterangepicker') !== undefined && $('#ExpireDate').data('daterangepicker') != null && isPickerApprove) {
            ExpireDateFrom = $('#ExpireDate').data('daterangepicker').startDate._d.toLocaleDateString("en-GB");
            ExpireDateTo = $('#ExpireDate').data('daterangepicker').endDate._d.toLocaleDateString("en-GB");
        } else {
            ExpireDateFrom = null
            ExpireDateTo = null
        }

        if ($('#CreateDate').data('daterangepicker') !== undefined && $('#CreateDate').data('daterangepicker') != null && isPickerApprove2) {
            CreateDateFrom = $('#CreateDate').data('daterangepicker').startDate._d.toLocaleDateString("en-GB");
            CreateDateTo = $('#CreateDate').data('daterangepicker').endDate._d.toLocaleDateString("en-GB");
        } else {
            CreateDateFrom = null
            CreateDateTo = null
        }

        if ($('#VerifyDate').data('daterangepicker') !== undefined && $('#VerifyDate').data('daterangepicker') != null && isPickerApprove3) {
            VerifyDateFrom = $('#VerifyDate').data('daterangepicker').startDate._d.toLocaleDateString("en-GB");
            VerifyDateTo = $('#VerifyDate').data('daterangepicker').endDate._d.toLocaleDateString("en-GB");
        } else {
            VerifyDateFrom = null
            VerifyDateTo = null
        }
        let _searchModel = {
            ClientId: null,
            SalerId: null,
            UserCreate: null,
            UserVerify: null,
            ContractNo: $('#ContractNo').val(),
            DebtType: listDebtType.toString(),
            ExpireDateFrom: ExpireDateFrom,
            ExpireDateTo: ExpireDateTo,
            ClientAgencyType: $('#AgencyType').val(),
            ContractExpire: $('#ContractExpire').val(),
            ClientType: listClientType.toString(),
            ContractStatus: listContractStatus.toString(),
            PermissionType: listPermissionType.toString(),
            VerifyStatus: $('#VerifyStatus').val(),
            CreateDateFrom: CreateDateFrom,
            CreateDateTo: CreateDateTo,
            VerifyDateFrom: VerifyDateFrom,
            VerifyDateTo: VerifyDateTo,
            PageIndex: 1,
            PageSize: $("#selectPaggingOptions").find(':selected').val(),
        };
        if (ClientId_data != null) { _searchModel.ClientId = ClientId_data[0] }
        if (SalerId_data != null) { _searchModel.SalerId = SalerId_data[0] }
        if (UserCreate_data != null) { _searchModel.UserCreate = UserCreate_data[0] }
        if (UserVerify_data != null) { _searchModel.UserVerify = UserVerify_data[0] }
        if (isResetTab === true) {
            _searchModel.ContractStatus = isContractStatus;
        }
        return _searchModel
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
    checkShowHide: function () {
        if (filter.STT === true) {
            $('#STTfilter').prop('checked', true);
        } else {
            $('#STTfilter').prop('checked', false);
        }
        if (filter.ContractNo === true) {
            $('#ContractNofilter').prop('checked', true);
        } else {
            $('#ContractNofilter').prop('checked', false);
        }
        if (filter.ExpireDate === true) {
            $('#ExpireDatefilter').prop('checked', true);
        } else {
            $('#ExpireDatefilter').prop('checked', false);
        }
        if (filter.ClientId === true) {
            $('#ClientIdfilter').prop('checked', true);
        } else {
            $('#ClientIdfilter').prop('checked', false);
        }
        if (filter.ServiceType === true) {
            $('#ServiceTypefilter').prop('checked', true);
        } else {
            $('#ServiceTypefilter').prop('checked', false);
        }
        if (filter.DebtType === true) {
            $('#DebtTypefilter').prop('checked', true);
        } else {
            $('#DebtTypefilter').prop('checked', false);
        }
        if (filter.AgencyType === true) {
            $('#AgencyTypefilter').prop('checked', true);
        } else {
            $('#AgencyTypefilter').prop('checked', false);
        }
        if (filter.ClientType === true) {
            $('#ClientTypefilter').prop('checked', true);
        } else {
            $('#ClientTypefilter').prop('checked', false);
        }
        if (filter.PermisionType === true) {
            $('#PermisionTypefilter').prop('checked', true);
        } else {
            $('#PermisionTypefilter').prop('checked', false);
        }
        if (filter.SalerId === true) {
            $('#SalerIdfilter').prop('checked', true);
        } else {
            $('#SalerIdfilter').prop('checked', false);
        }
        if (filter.CreateDate === true) {
            $('#CreateDatefilter').prop('checked', true);
        } else {
            $('#CreateDatefilter').prop('checked', false);
        }
        if (filter.UserIdCreate === true) {
            $('#UserIdCreatefilter').prop('checked', true);
        } else {
            $('#UserIdCreatefilter').prop('checked', false);
        }
        if (filter.UserIdUpdate === true) {
            $('#UserIdUpdatefilter').prop('checked', true);
        } else {
            $('#UserIdUpdatefilter').prop('checked', false);
        }
        if (filter.UpdateLast === true) {
            $('#UpdateLastfilter').prop('checked', true);
        } else {
            $('#UpdateLastfilter').prop('checked', false);
        }
        if (filter.ContractStatus === true) {
            $('#ContractStatusfilter').prop('checked', true);
        } else {
            $('#ContractStatusfilter').prop('checked', false);
        }
       

    },
    ChangeSetting: function (position) {
        this.ShowHideColumn();
        switch (position) {
            case 1:
                if ($('#STTfilter').is(":checked")) {
                    filter.STT = true
                } else {
                    filter.STT = false
                }
                this.eraseCookie(cookieName);
                this.setCookie(cookieName, JSON.stringify(filter), 10);
                break;
            case 2:
                if ($('#ContractNofilter').is(":checked")) {
                    filter.ContractNo = true
                } else {
                    filter.ContractNo = false
                }
                this.eraseCookie(cookieName);
                this.setCookie(cookieName, JSON.stringify(filter), 10);
                break;
            case 3:
                if ($('#ExpireDatefilter').is(":checked")) {
                    filter.ExpireDate = true
                } else {
                    filter.ExpireDate = false
                }
                this.eraseCookie(cookieName);
                this.setCookie(cookieName, JSON.stringify(filter), 10);
                break;
            case 4:
                if ($('#ClientIdfilter').is(":checked")) {
                    filter.ClientId = true
                } else {
                    filter.ClientId = false
                }
                this.eraseCookie(cookieName);
                this.setCookie(cookieName, JSON.stringify(filter), 10);
                break;
            case 5:
                if ($('#ServiceTypefilter').is(":checked")) {
                    filter.ServiceType = true
                } else {
                    filter.ServiceType = false
                }
                this.eraseCookie(cookieName);
                this.setCookie(cookieName, JSON.stringify(fifilterelds), 10);
                break;
            case 6:
                if ($('#DebtTypefilter').is(":checked")) {
                    filter.DebtType = true
                } else {
                    filter.DebtType = false
                }
                this.eraseCookie(cookieName);
                this.setCookie(cookieName, JSON.stringify(filter), 10);
                break;
            case 7:
                if ($('#AgencyTypefilter').is(":checked")) {
                    filter.AgencyType = true
                } else {
                    filter.AgencyType = false
                }
                this.eraseCookie(cookieName);
                this.setCookie(cookieName, JSON.stringify(filter), 10);
                break;

            case 8:
                if ($('#ClientTypefilter').is(":checked")) {
                    filter.ClientType = true
                } else {
                    filter.ClientType = false
                }
                this.eraseCookie(cookieName);
                this.setCookie(cookieName, JSON.stringify(filter), 10);
                break;
            case 9:
                if ($('#PermisionTypefilter').is(":checked")) {
                    filter.PermisionType = true
                } else {
                    filter.PermisionType = false
                }
                this.eraseCookie(cookieName);
                this.setCookie(cookieName, JSON.stringify(filter), 10);
                break;

            case 10:
                if ($('#SalerIdfilter').is(":checked")) {
                    filter.SalerId = true
                } else {
                    filter.SalerId = false
                }
                this.eraseCookie(cookieName);
                this.setCookie(cookieName, JSON.stringify(filter), 10);
                break;
            case 11:
                if ($('#CreateDatefilter').is(":checked")) {
                    filter.CreateDate = true
                } else {
                    filter.CreateDate = false
                }
                this.eraseCookie(cookieName);
                this.setCookie(cookieName, JSON.stringify(filter), 10);
                break;

            case 12:
                if ($('#UserIdCreatefilter').is(":checked")) {
                    filter.UserIdCreate = true
                } else {
                    filter.UserIdCreate = false
                }
                this.eraseCookie(cookieName);
                this.setCookie(cookieName, JSON.stringify(filter), 10);
                break;
            case 13:
                if ($('#UserIdUpdatefilter').is(":checked")) {
                    filter.UserIdUpdate = true
                } else {
                    filter.UserIdUpdate = false
                }
                this.eraseCookie(cookieName);
                this.setCookie(cookieName, JSON.stringify(filter), 10);
                break;
            case 14:
                if ($('#UpdateLastfilter').is(":checked")) {
                    filter.UpdateLast = true
                } else {
                    filter.UpdateLast = false
                }
                this.eraseCookie(cookieName);
                this.setCookie(cookieName, JSON.stringify(filter), 10);
                break;
            case 15:
                if ($('#ContractStatusfilter').is(":checked")) {
                    filter.ContractStatus = true
                } else {
                    filter.ContractStatus = false
                }
                this.eraseCookie(cookieName);
                this.setCookie(cookieName, JSON.stringify(filter), 10);
                break;
            default:
        }
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
    eraseCookie: function (name) {
        document.cookie = name + '=; Path=/; Expires=Thu, 01 Jan 1970 00:00:01 GMT;';
    },
    saveCookieFilter: function (input) {
/*        this.setCookie(cookieContractFilterName, JSON.stringify(input), 1)*/
        window.localStorage.setItem(cookieContractFilterName, JSON.stringify(input))

    },
    getsearchModel: function () {
        var CreateDateFrom; //Ngày tạo từ--
        var CreateDateTo; //Ngày tạo từ--
        var ExpireDateFrom; //Ngày tạo từ--
        var VerifyDateTo; //Ngày tạo từ--
        var ExpireDateFrom; //Ngày hết hạn 
        var ExpireDateTo; //Ngày hết hạn 
        var ClientId_data = $('#client').select2("val");
        var SalerId_data = $('#SalerId').select2("val");
        var UserCreate_data = $('#UserIdCreate').select2("val");
        var UserVerify_data = $('#UpdatedBy').select2("val");
        textNV = $('#CreatedBy').find(':selected').text();

        if ($('#ExpireDate').data('daterangepicker') !== undefined && $('#ExpireDate').data('daterangepicker') != null && isPickerApprove) {
            ExpireDateFrom = $('#ExpireDate').data('daterangepicker').startDate._d.toLocaleDateString("en-GB");
            ExpireDateTo = $('#ExpireDate').data('daterangepicker').endDate._d.toLocaleDateString("en-GB");
        } else {
            ExpireDateFrom = null
            ExpireDateTo = null
        }

        if ($('#CreateDate').data('daterangepicker') !== undefined && $('#CreateDate').data('daterangepicker') != null && isPickerApprove2) {
            CreateDateFrom = $('#CreateDate').data('daterangepicker').startDate._d.toLocaleDateString("en-GB");
            CreateDateTo = $('#CreateDate').data('daterangepicker').endDate._d.toLocaleDateString("en-GB");
        } else {
            CreateDateFrom = null
            CreateDateTo = null
        }

        if ($('#VerifyDate').data('daterangepicker') !== undefined && $('#VerifyDate').data('daterangepicker') != null && isPickerApprove3) {
            VerifyDateFrom = $('#VerifyDate').data('daterangepicker').startDate._d.toLocaleDateString("en-GB");
            VerifyDateTo = $('#VerifyDate').data('daterangepicker').endDate._d.toLocaleDateString("en-GB");
        } else {
            VerifyDateFrom = null
            VerifyDateTo = null
        }
        let _searchModel = {
            ClientId: null,
            SalerId: null,
            UserCreate: null,
            UserVerify: null,
            ExpireDateFrom: ExpireDateFrom,
            ExpireDateTo: ExpireDateTo,
            ClientAgencyType: $('#AgencyType').val(),
            ClientType: $('#ClientType').val(),
            ContractStatus: $('#ContractStatus').val(),
            PermissionType: $('#PermissionType').val(),
            VerifyStatus: $('#VerifyStatus').val(),
            CreateDateFrom: CreateDateFrom,
            CreateDateTo: CreateDateTo,
            VerifyDateFrom: VerifyDateFrom,
            VerifyDateTo: VerifyDateTo,
            PageIndex: 1,
            PageSize: $("#selectPaggingOptions").find(':selected').val(),
        };
        if (ClientId_data != null) { _searchModel.ClientId = ClientId_data[0] }
        if (SalerId_data != null) { _searchModel.SalerId = SalerId_data[0] }
        if (UserCreate_data != null) { _searchModel.UserCreate = UserCreate_data[0] }
        if (UserVerify_data != null) { _searchModel.UserVerify = UserVerify_data[0] }
        return _searchModel;
    },

    onSelectPageSize: function () {
        var objSearch = this.SearchParam;
        objSearch = this.GetParam(true);
        this.Search(objSearch);
    },
    OnSearchStatus: function (status) {
        isResetTab = false
        var objSearch = this.GetParam(true);
        filter.ContractStatus = status;
        if (status == -1) {
            objSearch.ContractStatus = '';
        } else {
            objSearch.ContractStatus = status;
        }
    
        this.Search(objSearch);
        this.SetActive(status)
        if (status == -1) {
            this.OnCountStatus(objSearch);
        }
    },
    SetActive: function (status) {
        var objSearch = this.SearchParam

        isContractStatus = status;
        $('#countSttAll').removeClass('active')
        $('#TotalConTract_0').removeClass('active')
        $('#TotalConTract_1').removeClass('active')
        $('#TotalConTract_2').removeClass('active')
        $('#TotalConTract_3').removeClass('active')
        $('#TotalConTract_4').removeClass('active')
        if (status == -1) {
            $('#countSttAll').addClass('active')
            isResetTab = false;
        }
        if (status == 0) {
            $('#TotalConTract_0').addClass('active')
            isResetTab = true;
     
        }
        if (status == 1) {
            $('#TotalConTract_1').addClass('active')
            isResetTab = true;

        }

        if (status == 2) {
            $('#TotalConTract_2').addClass('active')
            isResetTab = true;
            }
        if (status == 3) {
            $('#TotalConTract_3').addClass('active')
            isResetTab = true;
            }
        if (status == 4) {
            $('#TotalConTract_4').addClass('active')
            isResetTab = true;
            }
    },
    OnCountStatus: function (objSearch) {
        $.ajax({
            url: "/Contract/CountStatus",
            type: "Post",
            data: objSearch,
            success: function (result) {
                $('#countSttAll').text('Tất cả (0)')
                if (result.isSuccess == true) {
                    
                    $('#countSttAll').text('Tất cả (' + result.data + ')')
                       
                }
            }
        });
    },
    Ischeck: function () {
        var PermisionType = $('#PermisionType_Name').val();
        if (PermisionType == 1) {
            if (document.getElementById("IsPrivate").checked == true) {
                $('#ProductFlyTicketDebtAmount').removeAttr("readonly")
                $('#ProductFlyTicketDebtAmount').removeAttr("style")
                $('#HotelDebtAmout').removeAttr("readonly");
                $('#HotelDebtAmout').removeAttr("style");
                $('#TourDebtAmount').removeAttr("readonly");
                $('#TourDebtAmount').removeAttr("style");
                $('#TouringCarDebtAmount').removeAttr("readonly");
                $('#TouringCarDebtAmount').removeAttr("style");
                $('#VinWonderDebtAmount').removeAttr("readonly");
                $('#VinWonderDebtAmount').removeAttr("style");
                $('#DebtType_Type').removeAttr("disabled");
                $('#DebtType_Type').removeAttr("style");

              
            }
            else{
                $('#ProductFlyTicketDebtAmount').attr("readonly", "readonly");
                $('#HotelDebtAmout').attr("readonly", "readonly");
                $('#TourDebtAmount').attr("readonly", "readonly");
                $('#TouringCarDebtAmount').attr("readonly", "readonly");
                $('#VinWonderDebtAmount').attr("readonly", "readonly");
                $('#DebtType_Type').attr("disabled", "disabled");
                $('#DebtType_Type').attr("disabled", "disabled");
                this.OnloadPolicy();
            }
        }
        if (PermisionType == 2) {
            if (document.getElementById("IsPrivate").checked == true) {
                $('#ProductFlyTicketDepositAmount').removeAttr("readonly")
                $('#ProductFlyTicketDepositAmount').removeAttr("style")
                $('#TourDepositAmount').removeAttr("readonly")
                $('#TourDepositAmount').removeAttr("style")
                $('#TouringCarDepositAmount').removeAttr("readonly")
                $('#TouringCarDepositAmount').removeAttr("style")
                $('#VinWonderDepositAmount').removeAttr("readonly")
                $('#VinWonderDepositAmount').removeAttr("style")
                $('#HotelDepositAmout').removeAttr("readonly")
                $('#HotelDepositAmout').removeAttr("style")
            }
            else {
                $('#ProductFlyTicketDepositAmount').attr("readonly", "readonly");
                $('#HotelDepositAmout').attr("readonly", "readonly");
                $('#TourDepositAmount').attr("readonly", "readonly");
                $('#TouringCarDepositAmount').attr("readonly", "readonly");
                $('#VinWonderDepositAmount').attr("readonly", "readonly");
                this.OnloadPolicy();
            }
        }
    },
    Ischeck2: function () {
        var PermisionType = $('#PermisionType_Name').val();
        if (PermisionType == 1) {
            
                $('#ProductFlyTicketDebtAmount').removeAttr("readonly")
                $('#HotelDebtAmout').removeAttr("readonly");
                $('#TourDebtAmount').removeAttr("readonly")
                $('#TouringCarDebtAmount').removeAttr("readonly");
                $('#VinWonderDebtAmount').removeAttr("readonly")
                $('#DebtType_Type').removeAttr("disabled");
            
            //else {
            //    $('#ProductFlyTicketDebtAmount').attr("readonly", "readonly");
            //    $('#HotelDebtAmout').attr("readonly", "readonly");
            //    $('#DebtType_Type').attr("disabled", "disabled");
            //}
        }
        if (PermisionType == 2) {
            
            $('#ProductFlyTicketDepositAmount').removeAttr("readonly")
            $('#HotelDepositAmout').removeAttr("readonly")
            $('#TourDepositAmount').removeAttr("readonly")
            $('#TouringCarDepositAmount').removeAttr("readonly")
            $('#VinWonderDepositAmount').removeAttr("readonly")
          
            //else {
            //    $('#ProductFlyTicketDebtAmount').attr("readonly", "readonly");
            //    $('#HotelDebtAmout').attr("readonly", "readonly");
            //}
        }
    },
}
var _Contract_create = {
    Initialization: function () {
        $("#ClientId").select2({
            theme: 'bootstrap4',
            placeholder: "Tên KH, Điện Thoại, Email",
            maximumSelectionLength: 1,
            ajax: {
                url: "/Contract/GetSuggestionClientBySale",
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
                                text: item.clientname + ' - ' + item.email + ' - ' + item.phone ,
                                id: item.id,
                            }
                        })
                    };
                },
                cache: true
            }
        });
        _Contract.OnloadPolicy();
    },
}