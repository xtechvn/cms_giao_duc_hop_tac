using Entities.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ViewModels
{
    public class TelegramviewModel
    {
        public Boolean ok { get; set; }
        public TelegramGroupModel result { get; set; }
    }
    public class TelegramGroupModel
    {
        public long id { get; set; }
        public string title { get; set; }

    }
    public class Telegramapi
    {
        public string token { get; set; }
        public string groupid { get; set; }
    }
    public class TelegramDetailmodel: TelegramDetail
    {
        public int STT { get; set; }
    }

}