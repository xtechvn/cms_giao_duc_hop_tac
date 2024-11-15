using Caching.RedisWorker;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
using Utilities;
using WEB.CMS.Customize;

namespace GDHT.CMS.Controllers.Notify
{
    [CustomAuthorize]
    public class SseController : Controller
    {
        private readonly ISubscriber _subscriber;
        private readonly RedisConn redisService;
        private readonly IConfiguration _Configuration;
        public SseController(RedisConn _redisService, IConfiguration configuration)
        {
            redisService = _redisService;
            _Configuration = configuration;
            var connection = ConnectionMultiplexer.Connect(_Configuration["Redis:Host"] + ":" + _Configuration["Redis:Port"]);
            _subscriber = connection.GetSubscriber();
        }

        //Subscriber
        //Sử dụng Server-Sent Events(SSE) để gửi dữ liệu từ máy chủ tới trình duyệt một cách không đồng bộ
        //[HttpGet]
        // public async Task getNotify()
        // {
        //     try
        //     {
        //         var user_id = -1;
        //         var dataQueue = new ConcurrentQueue<string>();
        //         if (HttpContext.User.FindFirst(ClaimTypes.NameIdentifier) != null)
        //         {
        //             user_id = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
        //             Response.Headers.Add("Content-Type", "text/event-stream");
        //             _subscriber.Subscribe("NOTIFY_" + user_id, (channel, message) =>
        //             {
        //                 dataQueue.Enqueue(message);
        //             });

        //             while (!HttpContext.RequestAborted.IsCancellationRequested)
        //             {
        //                 while (dataQueue.TryDequeue(out var message))
        //                 {
        //                     var data = $"data: {message}\n\n";
        //                     byte[] buffer = System.Text.Encoding.UTF8.GetBytes(data);
        //                     await Response.Body.WriteAsync(buffer, 0, buffer.Length);
        //                     await Response.Body.FlushAsync();
        //                 }

        //                 await Task.Delay(100);
        //             }
        //         }                
        //     }
        //     catch (Exception ex)
        //     {
        //         LogHelper.InsertLogTelegram("SseController - Subscriber" + ex.ToString());               
        //     }
        // }
        // cuonglv update kiểm tra khi nào cần thoát vòng lặp. Điều này giúp tránh việc truy cập một đối tượng đã bị giải phóng khi response đã được gửi.
        [HttpGet]
        public async Task getNotify()
        {
            try
            {
                var user_id = -1;
                var dataQueue = new ConcurrentQueue<string>();
                if (HttpContext.User.FindFirst(ClaimTypes.NameIdentifier) != null)
                {
                    var _company_type="";
                    AppSettings _appconfig = new AppSettings();
                    using (StreamReader r = new StreamReader("appsettings.json"))
                    {
                        string json = r.ReadToEnd();
                        _appconfig = JsonConvert.DeserializeObject<AppSettings>(json);
                        _company_type = _appconfig.CompanyType;
                    }
                    user_id = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                    Response.Headers.Add("Content-Type", "text/event-stream");
                    _subscriber.Subscribe("NOTIFY_" + user_id +(_company_type.Trim() == "0" ? "" : "_" + _company_type.Trim()), (channel, message) =>
                    {
                        dataQueue.Enqueue(message);
                    });

                    while (true)
                    {
                        if (HttpContext.RequestAborted.IsCancellationRequested)
                        {
                            break;
                        }

                        while (dataQueue.TryDequeue(out var message))
                        {
                            var data = $"data: {message}\n\n";
                            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(data);
                            await Response.Body.WriteAsync(buffer, 0, buffer.Length);
                            await Response.Body.FlushAsync();
                        }

                        await Task.Delay(20);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("SseController - Subscriber: " + ex.ToString());
            }
        }


        [HttpGet]
        public async Task<IActionResult> TestPublishMessage()
        {
            _subscriber.Publish("NOTIFY_65", "cuonglv test ");
            return Content("DONE");

        }
    }
}
