using Entities.Models;
using Entities.ViewModels;
using Entities.ViewModels.TransferSms;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Utilities;
using GDHT.CMS.Service;
using WEB.CMS.Customize;

namespace GDHT.CMS.Controllers
{
    [CustomAuthorize]

    public class TransactionSmsController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _WebHostEnvironment;
        public TransactionSmsController(IConfiguration configuration, IWebHostEnvironment WebHostEnvironment)
        {
            _configuration = configuration;
            _WebHostEnvironment = WebHostEnvironment;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Search(TransferSmsSearchModel searchModel, int currentPage = 1, int pageSize = 20)
        {
            var model = new GenericViewModel<TransactionSMSViewModel>();
            try
            {
                TransferSmsService transferSmsService = new TransferSmsService();
                long total = 0;
                var listTransactionSms = transferSmsService.SearchTransactionSMs(searchModel, out total, currentPage, pageSize);
                model.CurrentPage = currentPage;
                model.ListData = listTransactionSms;
                model.PageSize = pageSize;
                model.TotalRecord = total;
                model.TotalPage = (int)Math.Ceiling((double)total / pageSize);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("Search - TransactionSmsController. " + JsonConvert.SerializeObject(ex));
            }
            return PartialView(model);
        }
        public IActionResult DashBoardKeToan()
        {
       
            return View();
        }
      
        public IActionResult ReportTransactionSMs()
        {

            return View();
        }
        [HttpPost]
        public IActionResult GetListReportTransactionSMs(TransferSmsSearchModel searchModel)
        {
            try
            {
                TransferSmsService transferSmsService = new TransferSmsService();
                var ListBankingAccountTransactionSMs = transferSmsService.GetLisstBankingAccountTransactionSMs();
                foreach(var item in ListBankingAccountTransactionSMs)
                {
                    searchModel.AccountNumber = item.AccountNumber;
                    searchModel.BankName = item.BankId;
                    var ListTransactionSMs = transferSmsService.ListTransactionSMs(searchModel);
                    item.SumAmount = ListTransactionSMs.Where(s=>s.Amount>0).Sum(s => s.Amount);
                    item.SumAmountTR = ListTransactionSMs.Where(s=>s.Amount<0).Sum(s => s.Amount);

                    var date_new = searchModel.FromDate.Value.AddDays(-1);
                    var SumTotalAmountTransactionSMs = transferSmsService.SumTotalAmountTransactionSMs(searchModel.AccountNumber, searchModel.BankName, date_new);

                    item.Amount = item.Amount + SumTotalAmountTransactionSMs;
                }
                
                return PartialView(ListBankingAccountTransactionSMs);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetListTransferSmsAccountNumber - DashBoardController: " + ex.ToString());
               
            }
            return PartialView();
        }
        public IActionResult ReportTransactionSMsByYear()
        {
           
            return View();
        }
        public async Task<IActionResult> ExportExcel(TransferSmsSearchModel searchModel)
        {
            try
            {

                int _UserId = 0;
                if (HttpContext.User.FindFirst(ClaimTypes.NameIdentifier) != null)
                {
                    _UserId = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                }
                string _FileName = StringHelpers.GenFileName("Danh sách GDTN chuyển khoản", _UserId, "xlsx");
                string _UploadFolder = @"Template\Export";
                string _UploadDirectory = Path.Combine(_WebHostEnvironment.WebRootPath, _UploadFolder);

                if (!Directory.Exists(_UploadDirectory))
                {
                    Directory.CreateDirectory(_UploadDirectory);
                }
                //delete all file in folder before export
                try
                {
                    System.IO.DirectoryInfo di = new DirectoryInfo(_UploadDirectory);
                    foreach (FileInfo file in di.GetFiles())
                    {
                        file.Delete();
                    }
                }
                catch
                {
                }
                string FilePath = Path.Combine(_UploadDirectory, _FileName);

                TransferSmsService transferSmsService = new TransferSmsService();
                long total = 0;
               
               
                var rsPath = await transferSmsService.ExportDeposit(searchModel, FilePath);

                if (!string.IsNullOrEmpty(rsPath))
                {
                    return new JsonResult(new
                    {
                        isSuccess = true,
                        message = "Xuất dữ liệu thành công",
                        path = "/" + _UploadFolder + "/" + _FileName
                    });
                }
                else
                {
                    return new JsonResult(new
                    {
                        isSuccess = false,
                        message = "Xuất dữ liệu thất bại"
                    });
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("ExportExcel - OrderController: " + ex);
                return new JsonResult(new
                {
                    isSuccess = false,
                    message = ex.Message.ToString()
                });
            }
        }
    }
}
