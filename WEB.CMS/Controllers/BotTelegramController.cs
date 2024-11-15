using Caching.RedisWorker;
using Entities.Models;
using Entities.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Repositories.IRepositories;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;
using WEB.CMS.Customize;

namespace GDHT.CMS.Controllers
{
    [CustomAuthorize]

    public class BotTelegramController : Controller
    {
        private readonly ITelegramRepository _telegramRepository;
        private readonly IConfiguration _configuration;
        private RedisConn _redisConn;

        public BotTelegramController(ITelegramRepository telegramRepository, IConfiguration configuration)
        {
            _telegramRepository = telegramRepository;
            _configuration = configuration;
            _redisConn = new RedisConn(configuration);
            _redisConn.Connect();
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult BotDetail(int id)
        {
            if (id != 0)
            {
                var listdata = _telegramRepository.GetTelegrambyid(id);
                return PartialView(listdata);
            }
            return PartialView();
        }

        [HttpPost]
        public async Task<IActionResult> AddBot(string data)
        {
            int stt_code = (int)ResponseType.FAILED;
            string msg = "Error On Excution";

            try
            {
                var botlogModel = JsonConvert.DeserializeObject<TelegramDetail>(data);
                if (botlogModel.Token != null && botlogModel.Token != "" || botlogModel.GroupChatId != null && botlogModel.GroupChatId != "" || botlogModel.GroupLog != "" && botlogModel.GroupLog != "")
                {
                    var a = _telegramRepository.AddTelegram(botlogModel);
                    if (a.Result == 0)
                    {
                        try
                        {
                            var cache_name = CacheName.CACHE_TELEGRAM_LIST;

                            _redisConn.clear(cache_name,  Convert.ToInt32(_configuration["DataBaseConfig:Redis:Database:db_common"]));
                            var list = _telegramRepository.GetAllcodeTelegram();
                            _redisConn.Set(cache_name, JsonConvert.SerializeObject(list), Convert.ToInt32(_configuration["DataBaseConfig:Redis:Database:db_common"]));
                        }
                         catch (Exception ex)
                        {
                            LogHelper.InsertLogTelegram("AddBot - BotTelegramController - Set Redis: " + ex.ToString());
                        }
                        stt_code = (int)ResponseType.SUCCESS;
                        msg = "Thêm mới/Cập nhật thông tin thành công";
                    }
                    else
                    {
                        stt_code = (int)ResponseType.FAILED;
                        msg = "Token đã tồn tại, vui lòng kiểm tra lại";
                    }
                }
                else
                {
                    stt_code = (int)ResponseType.FAILED;
                    msg = "Dữ liệu gửi lên không chính xác, vui lòng kiểm tra lại";
                }

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("AddBot - BotTelegramController: " + ex);
                stt_code = (int)ResponseType.ERROR;
                msg = "Lỗi kỹ thuật vui lòng liên hệ bộ phận IT";
            }
            return Ok(new
            {
                stt_code = stt_code,
                msg = msg,
                data = data
            });
        }
        [HttpPost]
        public async Task<string> GetGrouplogname(string dataapi)
        {
            TelegramviewModel list = new TelegramviewModel();
            try
            {
                var conten = JsonConvert.DeserializeObject<Telegramapi>(dataapi);
                HttpClient httpClient = new HttpClient();
                var dommain = _configuration["BotSetting:domain"];
                var apiPrefix = dommain + conten.token + "/getChat?chat_id=" + conten.groupid + "";
                var rs = await httpClient.GetAsync(apiPrefix);
                var rs_content = JsonConvert.DeserializeObject<TelegramviewModel>(rs.Content.ReadAsStringAsync().Result);
                list = rs_content;
                if (list.result != null)
                {
                    string groupname = list.result.title;
                    return groupname;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetGrouplogname - BotTelegramController: " + ex);
            }
            return null;
        }

        [HttpPost]
        public IActionResult Search(string TokenName, int Projectmodel = -1, int statusmodel = -1, int currentPage = 1, int pageSize = 20)
        {
            var model = new GenericViewModel<TelegramDetail>();
            try
            {
                model = _telegramRepository.GetTelegramPagingList(TokenName, Projectmodel, statusmodel, currentPage, pageSize);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("Search - BotTelegramController: " + ex);
            }
            return PartialView(model);
        }
    }

}
