using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Repositories.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utilities;
using Entities.Models;
using Utilities.Contants;
using System.Security.Claims;
using WEB.CMS.Customize;

namespace GDHT.CMS.Controllers
{
    [CustomAuthorize]

    public class PaymentAccountController : Controller
    {
        private readonly IPaymentAccountRepository _paymentAccountRepository;
        private readonly ISupplierRepository _supplierRepository;
   
        public PaymentAccountController(IPaymentAccountRepository paymentAccountRepository, ISupplierRepository supplierRepository)
        {

            _paymentAccountRepository = paymentAccountRepository;
            _supplierRepository = supplierRepository;
        }
        [HttpPost]
        public IActionResult Setup(BankingAccount DataModel)
        {
            int stt_code = (int)ResponseType.FAILED;
            string msg = "Error On Excution";
            try
            {
               
                 var Result = _supplierRepository.UpsertSupplierPayment(DataModel);
                if (Result > 0)
                {
                    stt_code = (int)ResponseType.SUCCESS;
                    msg = "Thêm mới/Cập nhật tài khoản thanh toán thành công";
                }
                else
                {
                    stt_code = (int)ResponseType.FAILED;
                    msg = "Thêm mới/Cập nhật tài khoản thanh toán không thành công";
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("Setup - PaymentAccountController: " + ex);
                stt_code = (int)ResponseType.ERROR;
                msg = "Lỗi kỹ thuật vui lòng liên hệ bộ phận IT";
            }
            return Ok(new
            {
                stt_code = stt_code,
                msg = msg,
            });
        }
        
        public async Task<IActionResult> Detail(int id,int user_Id)
        {
            
            try
            {
                ViewBag.ClientId = user_Id;
                if (id != 0)
                {
                    var model = _supplierRepository.GetSupplierPaymentById(id);
                    return PartialView(model);
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("Detail - PaymentAccountController: " + ex);
            }
            return PartialView();
        }
        [HttpPost]
        public IActionResult deleteById(int id)
        {
            try
            {
                var a = _paymentAccountRepository.Delete(id);
                if (a == 1)
                {
                    return Ok(new
                    {
                        stt_code = (int)ResponseType.SUCCESS,
                        msg = "Xóa thành công",

                    });
                }
                else
                {

                    return Ok(new
                    {
                        stt_code = (int)ResponseType.FAILED,
                        msg = "Xóa không thành công",

                    });
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("deleteById - PaymentAccountController: " + ex);
            }
            return PartialView();
        }
        public async Task<IActionResult> DetailUserAgent(int user_Id)
        {

            try
            {
                if (user_Id != 0)
                {
                    var model = _paymentAccountRepository.UserAgentByClient(user_Id);
                    return PartialView(model);
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("DetailUserAgent - PaymentAccountController: " + ex);
            }
            return PartialView();
        }
        [HttpPost]
        public IActionResult UpdatalUserAgent(int id,int userId)
        {

            try
            {
                if (id != 0)
                {
                    var create_id = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                    var model = _paymentAccountRepository.UpdataUserAgent(id, userId, create_id);
                    if (model == 1)
                    {
                        return Ok(new
                        {
                            stt_code = (int)ResponseType.SUCCESS,
                            msg = "Đổi nhân viên thành công",

                        });
                    }
                    else
                    {

                        return Ok(new
                        {
                            stt_code = (int)ResponseType.FAILED,
                            msg = "Đổi nhân viên không thành công",

                        });
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdatalUserAgent - PaymentAccountController: " + ex);
            }
            return Ok(new
            {
                stt_code = (int)ResponseType.FAILED,
                msg = "Đổi nhân viên không thành công",

            });
        }
    }
}
