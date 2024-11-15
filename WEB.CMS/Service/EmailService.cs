using Entities.Models;
using Entities.ViewModels.Attachment;
using Entities.ViewModels.Funding;
using Entities.ViewModels.HotelBookingCode;
using Entities.ViewModels.HotelBookingRoom;
using Entities.ViewModels.VinWonder;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PuppeteerSharp;
using Repositories.IRepositories;
using System.Net;
using System.Net.Mail;
using System.Text;
using Utilities;
using Utilities.Contants;
using GDHT.CMS.Service.ServiceInterface;
using WEB.CMS.Models;

namespace GDHT.CMS.Service
{
    public class EmailService : IEmailService
    {

        private readonly IConfiguration _configuration;
        private readonly IHotelBookingCodeRepository _hotelBookingCodeRepository;
        private readonly IHotelBookingRepositories _hotelBookingRepositories;
        private readonly IHotelBookingRoomExtraPackageRepository _hotelBookingRoomExtraPackageRepository;
        private readonly IFlyBookingDetailRepository _flyBookingDetailRepository;
        private readonly IBagageRepository _bagageRepository;
        private readonly IPassengerRepository _passengerRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IContactClientRepository _contactClientRepository;
        private readonly IUserRepository _userRepository;
        private readonly IClientRepository _clientRepository;
        private readonly ITourRepository _tourRepository;
        private readonly IFlightSegmentRepository _flightSegmentRepository;
        private readonly IAirlinesRepository _airlinesRepository;
        private readonly ISupplierRepository _supplierRepository;
        private readonly IHotelBookingRoomRepository _hotelBookingRoomRepository;
        private readonly IHotelBookingRoomRatesRepository _hotelBookingRoomRatesRepository;
        private readonly IOtherBookingRepository _otherBookingRepository;
        private readonly IPaymentRequestRepository _paymentRequestRepository;
        private readonly IVinWonderBookingRepository _vinWonderBookingRepository;
        private readonly IContractPayRepository _contractPayRepository;
        private readonly IAttachFileRepository _AttachFileRepository;
        private readonly IBankingAccountRepository _bankingAccountRepository;
        private readonly APIService _APIService;

        public EmailService(IConfiguration configuration, IHotelBookingCodeRepository hotelBookingCodeRepository, IHotelBookingRepositories hotelBookingRepositories, IHotelBookingRoomExtraPackageRepository hotelBookingRoomExtraPackageRepository, IHotelBookingRoomRatesRepository hotelBookingRoomRatesRepository,
        IFlyBookingDetailRepository flyBookingDetailRepository, IBagageRepository bagageRepository, IPassengerRepository passengerRepository, IOrderRepository orderRepository, IContactClientRepository contactClientRepository, IHotelBookingRoomRepository hotelBookingRoomRepository, IOtherBookingRepository otherBookingRepository,
             IUserRepository userRepository, IClientRepository clientRepository, ITourRepository tourRepository, IFlightSegmentRepository flightSegmentRepository, IAirlinesRepository airlinesRepository, ISupplierRepository supplierRepository, IPaymentRequestRepository paymentRequestRepository,
             IVinWonderBookingRepository vinWonderBookingRepository, IContractPayRepository contractPayRepository, IAttachFileRepository AttachFileRepository, IBankingAccountRepository bankingAccountRepository)
        {

            _configuration = configuration;
            _hotelBookingCodeRepository = hotelBookingCodeRepository;
            _hotelBookingRepositories = hotelBookingRepositories;
            _hotelBookingRoomExtraPackageRepository = hotelBookingRoomExtraPackageRepository;
            _flyBookingDetailRepository = flyBookingDetailRepository;
            _bagageRepository = bagageRepository;
            _passengerRepository = passengerRepository;
            _orderRepository = orderRepository;
            _contactClientRepository = contactClientRepository;
            _userRepository = userRepository;
            _clientRepository = clientRepository;
            _tourRepository = tourRepository;
            _flightSegmentRepository = flightSegmentRepository;
            _airlinesRepository = airlinesRepository;
            _supplierRepository = supplierRepository;
            _hotelBookingRoomRepository = hotelBookingRoomRepository;
            _hotelBookingRoomRatesRepository = hotelBookingRoomRatesRepository;
            _otherBookingRepository = otherBookingRepository;
            _paymentRequestRepository = paymentRequestRepository;
            _vinWonderBookingRepository = vinWonderBookingRepository;
            _AttachFileRepository = AttachFileRepository;
            _bankingAccountRepository = bankingAccountRepository;
            _contractPayRepository = contractPayRepository;
            _APIService = new APIService(configuration, userRepository);

        }
        public async Task<bool> SendEmail(SendEmailViewModel model, List<AttachfileViewModel> attach_file)
        {
            bool ressult = true;
            try
            {
                //AccountClient orderInfo = JsonConvert.DeserializeObject<AccountClient>(objectStr);

                MailMessage message = new MailMessage();
                if (string.IsNullOrEmpty(model.Subject))
                    model.Subject = "Xác nhận đơn hàng ";
                message.Subject = model.Subject;
                //config send email
                string from_mail = new ConfigurationBuilder().AddJsonFile("appsettings.json")
                    .Build().GetSection("MAIL_CONFIG")["FROM_MAIL"];
                string account = new ConfigurationBuilder().AddJsonFile("appsettings.json")
                    .Build().GetSection("MAIL_CONFIG")["USERNAME"];
                string password = new ConfigurationBuilder().AddJsonFile("appsettings.json")
                    .Build().GetSection("MAIL_CONFIG")["PASSWORD"];
                string host = new ConfigurationBuilder().AddJsonFile("appsettings.json")
                    .Build().GetSection("MAIL_CONFIG")["HOST"];
                string port = new ConfigurationBuilder().AddJsonFile("appsettings.json")
                    .Build().GetSection("MAIL_CONFIG")["PORT"];
                string Email_KIEMSOAT = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("MAIL_CONFIG")["Email_KIEMSOAT"];
                string Email_KETOAN = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("MAIL_CONFIG")["Email_KETOAN"];
                message.IsBodyHtml = true;
                message.From = new MailAddress(from_mail, new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("MAIL_CONFIG")["STMP_USERNAME_Email"] );
                switch (model.ServiceType)
                {
                    case (int)ServicesType.VINHotelRent:
                    case (int)ServicesType.OthersHotelRent:
                        {
                            message.Body = await GetTemplateinsertUser(model, model.ServiceId, model.OrderNote, model.PaymentNotification);

                        }
                        break;
                    case (int)ServicesType.FlyingTicket:
                        {
                            message.Body = await GetFlyBookingTemplateBody(model, model.group_booking_id, model.OrderNote, model.PaymentNotification);
                        }
                        break;
                    case (int)ServicesType.Tourist:
                        {
                            message.Body = await TourTemplateBody(model, model.ServiceId, model.OrderNote, model.PaymentNotification);
                        }
                        break;
                    case (int)EmailType.DON_HANG:
                        {

                            message.Body = await OrderTemplateBody(model, model.Orderid, model.PaymentNotification, true);
                            message.To.Add(model.Email);
                            if (model.To_Email != null)
                                message.ReplyToList.Add(model.To_Email);
                            var order = await _orderRepository.GetOrderByID(model.Orderid);
                            var userId = await _userRepository.GetManagerByUserId((long)order.SalerId);
                            var Leaderid = await _userRepository.GetLeaderByUserId((long)order.SalerId);
                            if (userId != 0)
                            {
                                var saler = await _userRepository.GetById((long)userId);
                                if (saler != null)
                                    message.ReplyToList.Add(saler.Email);
                            }
                            if (Leaderid != 0)
                            {
                                var Leader = await _userRepository.GetById(Leaderid);
                                if (Leader != null)
                                {
                                    message.To.Add(Leader.Email);
                                }
                            }
                            if (model.To_Email != null && model.To_Email.Trim() != "")
                            {
                                var to_split = model.To_Email.Split(",");
                                foreach (var to in to_split)
                                {
                                    message.To.Add(to);
                                    message.ReplyToList.Add(to);
                                }
                            }
                            if (model.CC_Email != null && model.CC_Email.Trim() != "")
                            {
                                var cc_split = model.CC_Email.Split(",");
                                foreach (var cc in cc_split)
                                {
                                    message.ReplyToList.Add(cc);
                                }
                            }
                            if (model.BCC_Email != null && model.BCC_Email.Trim() != "")
                            {
                                var bcc_split = model.BCC_Email.Split(",");
                                foreach (var bcc in bcc_split)
                                {
                                    message.ReplyToList.Add(bcc);
                                }
                            }
                            message.ReplyToList.Add(account);
                        }
                        break;
                    case (int)EmailType.Supplier:
                        {

                            message.Body = await GetTemplateSupplier(model, model.Orderid, model.SupplierId, model.type, "", model.PaymentNotification, true);
                            var data = await _orderRepository.GetAllServiceByOrderId(model.Orderid);

                            if (data != null)
                                foreach (var item in data)
                                {
                                    item.Price += item.Profit;
                                    if (item.Type.Equals("Tour"))
                                    {
                                        item.tour = await _tourRepository.GetDetailTourByID(Convert.ToInt32(item.ServiceId));
                                        var saler = await _userRepository.GetById((long)item.tour.SalerId);

                                    }
                                    if (item.Type.Equals("Khách sạn"))
                                    {
                                        item.Hotel = await _hotelBookingRepositories.GetDetailHotelBookingByID(Convert.ToInt32(item.ServiceId));
                                        var saler = await _userRepository.GetById((long)item.Hotel[0].SalerId);
                                        message.To.Add(saler.Email);

                                    }
                                    if (item.Type.Equals("Vé máy bay"))
                                    {
                                        item.Flight = await _flyBookingDetailRepository.GetDetailFlyBookingDetailById(Convert.ToInt32(item.ServiceId));
                                        var saler = await _userRepository.GetById((long)item.Flight.SalerId);

                                        message.To.Add(saler.Email);


                                    }


                                }
                            message.To.Add(model.Email);

                            message.To.Add(Email_KIEMSOAT);
                            message.To.Add(Email_KETOAN);

                        }
                        break;
                    case (int)EmailType.SaleDH:
                        {
                            message.Body = await OrderTemplateSaleDH(model.Orderid, "", true);
                            var order = await _orderRepository.GetOrderByID(model.Orderid);
                            try
                            {

                                if (order != null)
                                {
                                    model.Subject = "Xác nhận đơn hàng " + order.OrderNo + " " + order.Label;
                                    message.Subject = model.Subject.Replace('\n', ' ');
                                    var listService = await _orderRepository.GetAllServiceByOrderId(order.OrderId);
                                    var saler = await _userRepository.GetById(Convert.ToInt64(order.SalerId));
                                    var Tpsalerid = await _userRepository.GetManagerByUserId(Convert.ToInt64(order.SalerId));
                                    var Leaderid = await _userRepository.GetLeaderByUserId(Convert.ToInt64(order.SalerId));

                                    if (listService != null && saler != null && ReadFile.LoadConfig().List_Department_ks.Contains(saler.DepartmentId.ToString()) == true)
                                    {
                                        var ListId = _userRepository.GetHeadOfAccountantUser2();
                                        if (ListId != null && ListId.Count > 0)
                                        {
                                            ListId = ListId.Where(s => s.UserPositionId == UserPositionType.TP && s.DepartmentId == DepartmentContractType.PKDKSHN).ToList();
                                            if (ListId.Count > 0)
                                                foreach (var i in ListId)
                                                {
                                                    message.To.Add(i.Email);
                                                }
                                        }
                                    }
                                    if (Tpsalerid != 0)
                                    {
                                        var Tpsaler = await _userRepository.GetById(Tpsalerid);
                                        if (Tpsaler != null)
                                        {
                                            message.To.Add(Tpsaler.Email);
                                        }
                                    }
                                    if (Leaderid != 0)
                                    {
                                        var Leader = await _userRepository.GetById(Leaderid);
                                        if (Leader != null)
                                        {
                                            message.To.Add(Leader.Email);
                                        }
                                    }
                                    if (saler != null)
                                    {
                                        model.Subject = "Xác nhận đơn hàng " + order.OrderNo + " " + order.Label;
                                        message.To.Add(saler.Email);
                                    }

                                    if (order.OperatorId != null)
                                    {
                                        var cc_split = order.OperatorId.Split(",");
                                        foreach (var item in cc_split)
                                        {
                                            var TpDHid = await _userRepository.GetManagerByUserId(Convert.ToInt32(item));

                                            var salerdh = await _userRepository.GetById(Convert.ToInt32(item));
                                            if (salerdh != null) { message.To.Add(salerdh.Email); }
                                            if (TpDHid != 0 && order.ProductService.Contains(((int)ServicesType.Tourist).ToString()))
                                            {
                                                var TpDH = await _userRepository.GetById(Convert.ToInt32(TpDHid));
                                                if (TpDH != null) { message.To.Add(TpDH.Email); }
                                            }

                                        }
                                    }
                                }

                                message.To.Add(Email_KIEMSOAT);
                            }
                            catch (Exception ex)
                            {
                                LogHelper.InsertLogTelegram("sendMail - Base.MailService - SaleDH: " + ex + "Orderid:" + order.OrderId + ",SalerId:" + order.SalerId);
                            }

                        }
                        break;
                }
                //attachment 

                string sendEmailsFrom = account;
                string sendEmailsFromPassword = password;
                SmtpClient smtp = new SmtpClient(host, Convert.ToInt32(port));
                smtp.EnableSsl = true;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Credentials = new NetworkCredential(sendEmailsFrom, sendEmailsFromPassword);
                smtp.Timeout = 50000;
                if (model.To_Email != null && model.ServiceType != (int)EmailType.DON_HANG)
                    message.To.Add(model.To_Email);
                //message.To.Add(model.Email);
                message.Bcc.Add("anhhieuk51@gmail.com");
                string workingDirectory = Directory.GetCurrentDirectory();
                if (attach_file != null)
                    foreach (var item in attach_file)
                    {
                        string url = workingDirectory + "\\wwwroot" + item.path.Replace(@"/", "\\");
                        /* LogHelper.InsertLogTelegram("sendMail - Base.MailService: " + url);*/

                        message.Attachments.Add(new Attachment(url));
                    }

                if (model.CC_Email != null && model.CC_Email.Trim() != "")
                {
                    var cc_split = model.CC_Email.Split(",");
                    foreach (var cc in cc_split)
                    {
                        message.CC.Add(cc);
                    }
                }
                if (model.BCC_Email != null && model.BCC_Email.Trim() != "")
                {
                    var bcc_split = model.BCC_Email.Split(",");
                    foreach (var bcc in bcc_split)
                    {
                        message.Bcc.Add(bcc);
                    }
                }
                smtp.Send(message);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("sendMail - Base.MailService: " + ex + "Orderid:" + model.Orderid);
                ressult = false;
            }
            return ressult;
        }
        public async Task<string> GetTemplateSupplier(SendEmailViewModel modelEmail, long id, long SupplierId, long type, string order_note = "", string payment_notification = "", bool is_edit_form = false)
        {
            try
            {
                if (modelEmail == null)
                {

                    //string workingDirectory = Directory.GetCurrentDirectory();

                    string passenger = String.Empty;
                    string datatabledv = String.Empty;
                    string datatabledvkhac = String.Empty;
                    string chitietdichvu = String.Empty;
                    string chitietdichvukhac = String.Empty;
                    switch (type)
                    {
                        case (int)ServicesType.VINHotelRent:
                            {
                                string workingDirectory = AppDomain.CurrentDomain.BaseDirectory;
                                var template = workingDirectory + @"\EmailTemplate\SupplierTemplate.html";

                                string body = File.ReadAllText(template);
                                var DetailHotelBooking = await _hotelBookingRepositories.GetDetailHotelBookingByID(Convert.ToInt32(id));

                                //if (DetailHotelBooking[0].SupplierId == null) return null;
                                //var SupplierDetail = _supplierRepository.GetDetailById((int)DetailHotelBooking[0].SupplierId);
                                var datahotelbookingroomextrapackage = await _hotelBookingRoomExtraPackageRepository.Gethotelbookingroomextrapackagebyhotelbookingid(Convert.ToInt32(id));

                                var model = await _hotelBookingRepositories.GetHotelBookingById(id);
                                var hotel = await _hotelBookingRepositories.GetHotelBookingByID(id);
                                var order = await _orderRepository.GetOrderByID((long)hotel.OrderId);
                                var Dh = await _userRepository.GetById((long)hotel.SalerId);

                                foreach (var item in datahotelbookingroomextrapackage)
                                {
                                    passenger += "" + item.PackageCode + "&#10 ";

                                }

                                var rooms = await _hotelBookingRepositories.GetHotelBookingOptionalListByHotelBookingId(id);
                                var packages = await _hotelBookingRoomRepository.GetHotelBookingRoomRatesOptionalByBookingId(id);
                                var extra_package = await _hotelBookingRoomExtraPackageRepository.GetByBookingID(id);
                                List<HotelBookingRoomRatesOptionalViewModel> package_daterange = new List<HotelBookingRoomRatesOptionalViewModel>();
                                rooms = rooms.Where(s => s.SupplierId == SupplierId).ToList();
                                var NumberOfAdult = rooms.Sum(x => x.NumberOfAdult);
                                var NumberOfChild = rooms.Sum(x => x.NumberOfChild);
                                var NumberOfInfant = rooms.Sum(x => x.NumberOfInfant);
                                var NumberOfRoom = rooms.Sum(x => x.NumberOfRooms);
                                var sumtoday = 0;
                                var Amount = rooms.Sum(x => x.TotalAmount);
                                double AmountDVK = 0;
                                double number_of_people = (double)rooms.Sum(x => x.NumberOfAdult) + (double)rooms.Sum(x => x.NumberOfChild) + (double)rooms.Sum(x => x.NumberOfInfant);
                                if (packages != null && packages.Count > 0)
                                {
                                    foreach (var p in packages)
                                    {
                                        if (p.StartDate == null && p.EndDate == null)
                                        {
                                            if (packages.Count < 1 || !package_daterange.Any(x => x.HotelBookingRoomId == p.HotelBookingRoomId && x.RatePlanId == p.RatePlanId))
                                            {
                                                var add_value = p;
                                                add_value.StartDate = add_value.StayDate;
                                                add_value.EndDate = add_value.StayDate.AddDays(1);
                                                p.StayDate = (DateTime)add_value.StartDate;
                                                p.SalePrice = p.TotalAmount;
                                                p.OperatorPrice = p.Price;
                                                package_daterange.Add(add_value);
                                            }
                                            else
                                            {
                                                var p_d = package_daterange.FirstOrDefault(x => x.HotelBookingRoomId == p.HotelBookingRoomId && x.RatePlanId == p.RatePlanId && ((DateTime)x.EndDate).Date == p.StayDate.Date);
                                                if (p_d != null)
                                                {

                                                    if (p_d.StartDate == null || p_d.StartDate > p.StayDate)
                                                        p_d.StartDate = p.StayDate;
                                                    p_d.EndDate = p.StayDate.AddDays(1);
                                                }
                                                else
                                                {
                                                    var add_value = p;
                                                    add_value.StartDate = add_value.StayDate;
                                                    add_value.EndDate = add_value.StayDate.AddDays(1);
                                                    p.StayDate = (DateTime)add_value.StartDate;
                                                    p.SalePrice = p.TotalAmount;
                                                    p.OperatorPrice = p.Price;
                                                    package_daterange.Add(add_value);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            package_daterange.Add(p);
                                        }
                                    }
                                }
                                if (rooms != null && rooms.Count > 0)
                                    foreach (var item in rooms)
                                    {
                                        string RatePlanCode = String.Empty;
                                        string date = String.Empty;
                                        double Nights = 0;
                                        string TotalAmount = String.Empty;
                                        string operatorprice = String.Empty;
                                        string Goi = String.Empty;
                                        string TgSD = String.Empty;
                                        string GiaN = String.Empty;
                                        string SDem = String.Empty;
                                        string SP = String.Empty;
                                        string TTien = String.Empty;
                                        double NumberOfRooms = 0;
                                        var package_by_room_id = packages.Where(x => x.HotelBookingRoomOptionalId == item.Id);
                                        if (package_by_room_id != null && package_by_room_id.Count() > 0)
                                        {
                                            sumtoday += (int)package_by_room_id.Sum(s => s.Nights);
                                            foreach (var p in package_by_room_id)
                                            {
                                                double operator_price = 0;
                                                if (p.Price != null) operator_price = Math.Round(((double)p.Price / (double)p.Nights / (double)item.NumberOfRooms), 0);
                                                if (operator_price <= 0) operator_price = p.OperatorPrice != null ? (double)p.OperatorPrice : 0;

                                                RatePlanCode = p.RatePlanCode;
                                                date = (p.StartDate == null ? "" : ((DateTime)p.StartDate).ToString("dd/MM/yyyy")) + " - " + (p.EndDate == null ? "" : ((DateTime)p.EndDate).ToString("dd/MM/yyyy"));
                                                operatorprice = operator_price.ToString("N0");
                                                Nights = (double)p.Nights;
                                                TotalAmount = p.Price == null ? ((double)p.TotalAmount - (double)p.Profit).ToString("N0") : ((double)p.Price).ToString("N0");
                                                NumberOfRooms = item.NumberOfRooms == null ? 1 : (double)item.NumberOfRooms;
                                                Goi += "<div style='border: 1px solid #999; padding: 2px; text-align: center;'>" + RatePlanCode + "</div>";
                                                TgSD += "<div style='border: 1px solid #999; padding:2px; text-align: center;'>" + date + "</div>";
                                                GiaN += "<div style='border: 1px solid #999; padding: 2px; text-align: center;'>" + operatorprice + "</div>";
                                                SDem += "<div style='border: 1px solid #999; padding: 2px; text-align: center;'>" + Nights + "</div>";
                                                SP = "<div style='border: 1px solid #999; padding: 2px; text-align: center;'>" + NumberOfRooms + "</div>";
                                                TTien += "<div style='border: 1px solid #999; padding: 2px; text-align: center;'>" + TotalAmount + "</div>";
                                            }

                                        }
                                        chitietdichvu += "<tr><td  style='border: 1px solid #999; padding: 2px; text-align: center;'>" + item.RoomTypeName + "</td>" +
                                                                    "<td style='border: 1px solid #999; padding: 2px; text-align: center;'>" + Goi + "</td>" +
                                                                      "<td style='border: 1px solid #999; padding:2px; text-align: center;'>" + TgSD + "</td>" +
                                                                     "<td style='border: 1px solid #999; padding: 2px; text-align: center;'>" + GiaN + "</td>" +
                                                                     "<td style='border: 1px solid #999; padding: 2px; text-align: center;'>" + SDem + "</td>" +
                                                                      "<td style='border: 1px solid #999; padding: 2px; text-align: center;'>" + SP + "</td>" +
                                                                      "<td style='border: 1px solid #999; padding: 2px; text-align: center;'>" + TTien + "</td>"
                                                                      + "</tr>";

                                    }
                                if (extra_package != null && extra_package.Count > 0)
                                    foreach (var item in extra_package)
                                    {
                                        double operator_price = 0;
                                        if (item.UnitPrice == null)
                                        {
                                            AmountDVK += (double)(item.Amount - item.Profit);
                                        }
                                        else
                                        {
                                            AmountDVK += (double)item.UnitPrice;
                                        };

                                        if (item.UnitPrice != null) operator_price = Math.Round(((double)item.UnitPrice / (double)item.Nights / (double)item.Quantity), 0);
                                        if (operator_price <= 0) operator_price = item.OperatorPrice != null ? (double)item.OperatorPrice : 0;
                                        chitietdichvukhac += "<tr><td style='border: 1px solid #999; padding: 2px; text-align: center;'>" + item.PackageCode + "</td>" +
                                                                          "<td style='border: 1px solid #999; padding: 2px; text-align: center;'>" + item.PackageId + "</td>" +
                                                                          "<td style='border: 1px solid #999; padding: 2px; text-align: center;'>" + (item.StartDate == null ? "" : ((DateTime)item.StartDate).ToString("dd/MM/yyyy")) + " - " + (item.EndDate == null ? "" : ((DateTime)item.EndDate).ToString("dd/MM/yyyy")) + "</td>" +
                                                                          "<td style='border: 1px solid #999; padding: 2px; text-align: center;'>" + operator_price.ToString("N0") + "</td>" +
                                                                          "<td style='border: 1px solid #999; padding: 2px; text-align: center;'>" + (item.Nights != null ? ((double)item.Nights).ToString("N0") : "1") + "</td>" +
                                                                          "<td style='border: 1px solid #999; padding: 2px; text-align: center;'>" + (item.Quantity != null ? ((double)item.Quantity).ToString("N0") : "1") + "</td>" +
                                                                          "<td style='border: 1px solid #999; padding: 2px; text-align: center;'>" + (item.UnitPrice == null ? ((double)item.Amount - (double)item.Profit).ToString("N0") : ((double)item.UnitPrice).ToString("N0")) + "</td>" +
                                                                          "</tr>";


                                    }

                                var code = await _hotelBookingCodeRepository.GetListlBookingCodeByHotelBookingId(id, (int)ServiceType.BOOK_HOTEL_ROOM_VIN);
                                string passenger2 = "";
                                if (code != null && code.Count > 0)
                                {
                                    foreach (var dv in code)
                                    {
                                        passenger2 += "Mã : " + dv.BookingCode + ", nội dung: " + dv.Description + " &#10";
                                    }
                                }
                                if (chitietdichvu != string.Empty)
                                {
                                    datatabledv = "<table style='border-collapse: collapse;width:100%;'>" +
                                                                "<thead>" +
                                                                    "<tr>" +
                                                                        "<th style='border: 1px solid #999; padding: 2px; text-align: center;'>Hạng phòng</th>" +
                                                                        "<th style='border: 1px solid #999; padding: 2px; text-align: center;'>Gói</th>" +
                                                                        "<th style='border: 1px solid #999; padding: 2px; text-align: center;'>Thời gian sử dụng</th>" +
                                                                        "<th style='border: 1px solid #999; padding: 2px; text-align: center;'>Giá nhập</th>" +
                                                                        "<th style='border: 1px solid #999; padding: 2px; text-align: center;'>Số đêm</th>" +
                                                                        "<th style='border: 1px solid #999; padding: 2px; text-align: center;'>Số phòng</th>" +
                                                                        "<th style='border: 1px solid #999; padding: 2px; text-align: center;'>Thành tiền</th>" +
                                                                    "</tr> " +
                                                                "</thead>" +
                                                                "<tbody>" +
                                                                    chitietdichvu +
                                                               "</tbody>" +
                                                           "</table>";
                                }
                                else
                                {
                                    datatabledv = "";
                                }
                                if (extra_package != null && extra_package.Count > 0)
                                {
                                    datatabledvkhac = "<table style='border-collapse: collapse;width:100%;'>" +
                                                            "<thead>" +
                                                                "<tr>" +
                                                                    "<th style='border: 1px solid #999; padding: 2px; text-align: center;'>Tên dịch vụ</th>" +
                                                                    "<th style='border: 1px solid #999; padding: 2px; text-align: center;'>Gói</th>" +
                                                                    "<th style='border: 1px solid #999; padding: 2px; text-align: center;'>Thời gian sử dụng</th>" +
                                                                    "<th style='border: 1px solid #999; padding: 2px; text-align: center;'>Giá nhập</th>" +
                                                                    "<th style='border: 1px solid #999; padding: 2px; text-align: center;'>Số ngày	</th>" +
                                                                    "<th style='border: 1px solid #999; padding: 2px; text-align: center;'>Số lượng</th>" +
                                                                    "<th style='border: 1px solid #999; padding: 2px; text-align: center;'>Thành tiền</th>" +
                                                                "</tr> " +
                                                            "</thead>" +
                                                            "<tbody>" +
                                                                chitietdichvukhac +
                                                           "</tbody>" +
                                                       "</table>";
                                }
                                else
                                {
                                    datatabledvkhac = "";
                                }
                                if (datatabledv == "")
                                {
                                    Amount = 0;
                                    body = body.Replace("{{styledv}}", "style=\"display:none;\"");
                                    body = body.Replace("{{datatabledv}}", datatabledv);
                                }
                                else
                                {
                                    body = body.Replace("{{datatabledv}}", datatabledv);
                                }

                                if (hotel.SupplierId == (int)SupplierId)
                                {
                                    body = body.Replace("{{datatabledvkhac}}", datatabledvkhac);
                                }
                                else
                                {
                                    AmountDVK = 0;
                                    body = body.Replace("{{styledvkhac}}", "style =\"display:none;\"");
                                    body = body.Replace("{{datatabledvkhac}}", "");
                                }

                                body = body.Replace("{{datatable}}", "<textarea id=\"datatable\" style=\"height: 200px !important;\">" + model[0].Note + "</textarea>");
                                body = body.Replace("{{datatableCode}}", "<textarea id=\"datatableCode\" style=\"height: 200px !important;\">" + passenger2 + "</textarea>");
                                body = body.Replace("{{userName}}", "<input type =\"text\" id=\"user_Name\" value=\"\" />");
                                if (DetailHotelBooking[0].HotelName != null)
                                {
                                    body = body.Replace("{{HotelName}}", "<input type =\"text\" id=\"TileEmail\"style=\"text-align: center;font-weight: bold;\" value=\"PHIẾU XÁC NHẬN DỊCH VỤ: " + DetailHotelBooking[0].HotelName + "\"/>");
                                }
                                else
                                {
                                    body = body.Replace("{{HotelName}}", "<input type =\"text\" id=\"TileEmail\"style=\"text-align: center;font-weight: bold;\" value=\"PHIẾU XÁC NHẬN DỊCH VỤ: \"/>");
                                }
                                body = body.Replace("{{userPhone}}", "<input type =\"text\" id=\"user_Phone\" value=\"\" />");
                                body = body.Replace("{{userEmail}}", "<input type =\"text\" id=\"user_Email\" value=\"\" />");
                                body = body.Replace("{{orderNo}}", "<input type =\"text\" id=\"orderNo\" value=\"" + model[0].OrderNo + "\" />");
                                body = body.Replace("{{ArrivalDate}}", "<input type =\"text\"style=\"min-width: 100px;\" id=\"go_startdate\" value=\"" + model[0].ArrivalDate.ToString("dd/MM/yyyy") + "\" />");
                                body = body.Replace("{{DepartureDate}}", "<input type =\"text\" id=\"go_enddate\" value=\"" + model[0].DepartureDate.ToString("dd/MM/yyyy") + "\" />");

                                body = body.Replace("{{salerName}}", "<input type =\"text\" id=\"saler_Name\" value=\"" + Dh.FullName + "\" />");
                                body = body.Replace("{{salerPhone}}", "<input type =\"text\" id=\"saler_Phone\" value=\"" + Dh.Phone + "\" />");
                                body = body.Replace("{{salerEmail}}", "<input type =\"text\" id=\"saler_Email\" value=\"" + Dh.Email + "\" />");

                                body = body.Replace("{{NumberOfRoom}}", "<input type =\"text\" id=\"NumberOfRoom\" value=\"" + NumberOfRoom.ToString() + "\" />");
                                body = body.Replace("{{numberOfAdult}}", "<input style=\"width:30% !important;\" type =\"text\" id=\"go_numberOfAdult\" value=\"" + NumberOfAdult.ToString() + "\" />");
                                body = body.Replace("{{numberOfChild}}", "<input style=\"width:30% !important;\" type =\"text\" id=\"go_numberOfChild\" value=\"" + NumberOfChild.ToString() + "\" />");
                                body = body.Replace("{{numberOfInfant}}", "<input style=\"width:30% !important;\" type =\"text\" id=\"go_numberOfInfant\" value=\"" + NumberOfInfant.ToString() + "\" />");

                                body = body.Replace("{{totalAmount}}", "<input type =\"text\" class=\"currency\" id=\"totalAmount\" value=\"" + model[0].TotalAmount.ToString("###,###") + "\" />");
                                body = body.Replace("{{OrderAmount}}", "<input type =\"text\" class=\"currency\" id=\"OrderAmount\" value=\"" + (Amount + AmountDVK).ToString("N0") + "\" />");

                                body = body.Replace("{{totalToday}}", "<input type=\"text\" id=\"totalToday\" value=\"" + sumtoday.ToString() + "\" />");


                                body = body.Replace("{{Note}}", "<textarea id=\"order_note\" style=\"height: 100px !important;\">" + order_note + "</textarea>");
                                body = body.Replace("{{payment_notification}}", "<textarea id=\"payment_notification\" style=\"height: 200px !important;\">" + payment_notification + "</textarea>");
                                return body;
                            }
                            break;
                        case (int)ServicesType.Other:
                            {
                                string workingDirectory = AppDomain.CurrentDomain.BaseDirectory;
                                var template = workingDirectory + @"\EmailTemplate\OtherSupplierTemplate.html";

                                string body = File.ReadAllText(template);
                                var model = await _otherBookingRepository.GetDetailOtherBookingById(Convert.ToInt32(id));
                                var extra_package = await _otherBookingRepository.GetOtherBookingPackagesOptionalByBookingId(Convert.ToInt32(id));

                                if (model == null) return null;

                                var order = await _orderRepository.GetOrderByID(model[0].OrderId);
                                var Dh = await _userRepository.GetById((long)model[0].OperatorId);
                                if (extra_package != null && extra_package.Count() > 0)
                                {
                                    extra_package = extra_package.Where(s => s.SuplierId == SupplierId).ToList();
                                    foreach (var item in extra_package)
                                    {

                                        chitietdichvu += "<tr><td  style='border: 1px solid #999; padding: 2px; text-align: center;'>" + model[0].ServiceName + "</td>" +

                                                                     "<td style='border: 1px solid #999; padding: 2px; text-align: center;'>" + (item.BasePrice != null ? (double)item.BasePrice : 0).ToString("N0") + "</td>" +
                                                                    "<td style='border: 1px solid #999; padding: 2px; text-align: center;'>" + item.Quantity + "</td>" +
                                                                    "<td style='border: 1px solid #999; padding: 2px; text-align: center;'>" + item.Amount.ToString("N0") + "</td>"
                                                                    + "</tr>";
                                    }

                                }


                                if (chitietdichvu != string.Empty)
                                {
                                    datatabledv = "<table style='border-collapse: collapse;width:100%;'>" +
                                                                "<thead>" +
                                                                    "<tr>" +

                                                                        "<th style='border: 1px solid #999; padding: 2px; text-align: center;'>Loại dịch vụ</th>" +

                                                                        "<th style='border: 1px solid #999; padding: 2px; text-align: center;'>Giá nhập</th>" +

                                                                        "<th style='border: 1px solid #999; padding: 2px; text-align: center;'>Số lượng</th>" +
                                                                        "<th style='border: 1px solid #999; padding: 2px; text-align: center;'>Thành tiền</th>" +
                                                                    "</tr> " +
                                                                "</thead>" +
                                                                "<tbody>" +
                                                                    chitietdichvu +
                                                               "</tbody>" +
                                                           "</table>";
                                }
                                else
                                {
                                    datatabledv = "";
                                }
                                if (datatabledv == "")
                                {

                                    body = body.Replace("{{styledv}}", "style=\"display:none;\"");
                                    body = body.Replace("{{datatabledv}}", datatabledv);
                                }
                                else
                                {
                                    body = body.Replace("{{datatabledv}}", datatabledv);
                                }
                                if (extra_package != null && extra_package.Count() > 0)
                                {
                                    var Supplier = _supplierRepository.GetDetailById((int)extra_package[0].SuplierId);
                                    body = body.Replace("{{HotelName}}", "<input type =\"text\" id=\"TileEmail\"style=\"text-align: center;font-weight: bold;\" value=\"PHIẾU XÁC NHẬN DỊCH VỤ: " + Supplier.FullName + "\"/>");

                                }
                                else
                                {
                                    body = body.Replace("{{HotelName}}", "<input type =\"text\" id=\"TileEmail\"style=\"text-align: center;font-weight: bold;\" value=\"PHIẾU XÁC NHẬN DỊCH VỤ: \"/>");

                                }
                                body = body.Replace("{{styledvkhac}}", "style =\"display:none;\"");
                                body = body.Replace("{{datatable}}", "<textarea id=\"datatable\" style=\"height: 200px !important;\">" + model[0].Note + "</textarea>");
                                body = body.Replace("{{datatableCode}}", "<textarea id=\"datatableCode\" style=\"height: 200px !important;\">" + model[0].ServiceCode + "</textarea>");
                                body = body.Replace("{{userName}}", "<input type =\"text\" id=\"user_Name\" value=\"\" />");

                                body = body.Replace("{{userPhone}}", "<input type =\"text\" id=\"user_Phone\" value=\"\" />");
                                body = body.Replace("{{userEmail}}", "<input type =\"text\" id=\"user_Email\" value=\"\" />");
                                body = body.Replace("{{orderNo}}", "<input type =\"text\" id=\"orderNo\" value=\"" + order.OrderNo + "\" />");
                                body = body.Replace("{{ArrivalDate}}", "<input type =\"text\"style=\"min-width: 100px;\" id=\"go_startdate\" value=\"" + model[0].StartDate.ToString("dd/MM/yyyy") + "\" />");
                                body = body.Replace("{{DepartureDate}}", "<input type =\"text\" id=\"go_enddate\" value=\"" + model[0].EndDate.ToString("dd/MM/yyyy") + "\" />");

                                body = body.Replace("{{salerName}}", "<input type =\"text\" id=\"saler_Name\" value=\"" + Dh.FullName + "\" />");
                                body = body.Replace("{{salerPhone}}", "<input type =\"text\" id=\"saler_Phone\" value=\"" + Dh.Phone + "\" />");
                                body = body.Replace("{{salerEmail}}", "<input type =\"text\" id=\"saler_Email\" value=\"" + Dh.Email + "\" />");

                                //body = body.Replace("{{NumberOfRoom}}", "<input type =\"text\" id=\"NumberOfRoom\" value=\"" + NumberOfRoom.ToString() + "\" />");
                                //body = body.Replace("{{numberOfAdult}}", "<input style=\"width:30% !important;\" type =\"text\" id=\"go_numberOfAdult\" value=\"" + NumberOfAdult.ToString() + "\" />");
                                //body = body.Replace("{{numberOfChild}}", "<input style=\"width:30% !important;\" type =\"text\" id=\"go_numberOfChild\" value=\"" + NumberOfChild.ToString() + "\" />");
                                //body = body.Replace("{{numberOfInfant}}", "<input style=\"width:30% !important;\" type =\"text\" id=\"go_numberOfInfant\" value=\"" + NumberOfInfant.ToString() + "\" />");

                                body = body.Replace("{{totalAmount}}", "<input type =\"text\" class=\"currency\" id=\"totalAmount\" value=\"" + (model[0].OthersAmount != null ? (double)model[0].OthersAmount : 0).ToString("N0") + "\" />");
                                body = body.Replace("{{OrderAmount}}", "<input type =\"text\" class=\"currency\" id=\"OrderAmount\" value=\"" + (model[0].Price != null ? (double)model[0].Price : 0).ToString("N0") + "\" />");

                                body = body.Replace("{{totalToday}}", "<input type=\"text\" id=\"totalToday\" value=\"" + 1 + "\" />");


                                body = body.Replace("{{Note}}", "<textarea id=\"order_note\" style=\"height: 100px !important;\">" + order_note + "</textarea>");
                                body = body.Replace("{{payment_notification}}", "<textarea id=\"payment_notification\" style=\"height: 200px !important;\">" + payment_notification + "</textarea>");
                                return body;

                            }
                            break;
                    }

                    return null;
                }
                else
                {
                    string workingDirectory = AppDomain.CurrentDomain.BaseDirectory;
                    switch (type)
                    {
                        case (int)ServicesType.VINHotelRent:
                            {
                                var template = workingDirectory + @"\EmailTemplate\SupplierTemplate.html";
                                string body = File.ReadAllText(template);

                                string datatable = string.Empty;
                                string OrderNote = string.Empty;
                                string PaymentNotification = string.Empty;

                                string datatabledv = String.Empty;
                                string datatabledvkhac = String.Empty;
                                string chitietdichvu = String.Empty;
                                string chitietdichvukhac = String.Empty;

                                if (modelEmail.PaymentNotification != null)
                                {
                                    var listPaymentNotification = Array.ConvertAll(modelEmail.PaymentNotification.Split('\n'), s => (s).ToString());

                                    foreach (var item in listPaymentNotification)
                                    {
                                        PaymentNotification += "<div>" + item + "</div>";
                                    }
                                    body = body.Replace("{{payment_notification}}", PaymentNotification);
                                }
                                else
                                {
                                    body = body.Replace("{{payment_notification}}", PaymentNotification);
                                }

                                if (modelEmail.OrderNote != null)
                                {
                                    var listOrderNote = Array.ConvertAll(modelEmail.OrderNote.Split('\n'), s => (s).ToString());

                                    foreach (var item in listOrderNote)
                                    {
                                        OrderNote += "<div>" + item + "</div>";
                                    }
                                    body = body.Replace("{{Note}}", OrderNote);
                                }
                                else
                                {
                                    body = body.Replace("{{Note}}", OrderNote);
                                }

                                if (modelEmail.datatable != null)
                                {
                                    var listdatatable = Array.ConvertAll(modelEmail.datatable.Split('\n'), s => (s).ToString());

                                    foreach (var item in listdatatable)
                                    {
                                        datatable += "<div>" + item + "</div>";
                                    }
                                    body = body.Replace("{{datatable}}", datatable);
                                }
                                else
                                {
                                    body = body.Replace("{{datatable}}", modelEmail.datatable);
                                }


                                var rooms = await _hotelBookingRepositories.GetHotelBookingOptionalListByHotelBookingId(modelEmail.ServiceId);
                                var packages = await _hotelBookingRoomRepository.GetHotelBookingRoomRatesOptionalByBookingId(modelEmail.ServiceId);
                                var extra_package = await _hotelBookingRoomExtraPackageRepository.GetByBookingID(modelEmail.ServiceId);
                                List<HotelBookingRoomRatesOptionalViewModel> package_daterange = new List<HotelBookingRoomRatesOptionalViewModel>();
                                rooms = rooms.Where(s => s.SupplierId == modelEmail.SupplierId).ToList();
                                double number_of_people = (double)rooms.Sum(x => x.NumberOfAdult) + (double)rooms.Sum(x => x.NumberOfChild) + (double)rooms.Sum(x => x.NumberOfInfant);
                                if (packages != null && packages.Count > 0)
                                {
                                    foreach (var p in packages)
                                    {
                                        if (p.StartDate == null && p.EndDate == null)
                                        {
                                            if (packages.Count < 1 || !package_daterange.Any(x => x.HotelBookingRoomId == p.HotelBookingRoomId && x.RatePlanId == p.RatePlanId))
                                            {
                                                var add_value = p;
                                                add_value.StartDate = add_value.StayDate;
                                                add_value.EndDate = add_value.StayDate.AddDays(1);
                                                p.StayDate = (DateTime)add_value.StartDate;
                                                p.SalePrice = p.TotalAmount;
                                                p.OperatorPrice = p.Price;
                                                package_daterange.Add(add_value);
                                            }
                                            else
                                            {
                                                var p_d = package_daterange.FirstOrDefault(x => x.HotelBookingRoomId == p.HotelBookingRoomId && x.RatePlanId == p.RatePlanId && ((DateTime)x.EndDate).Date == p.StayDate.Date);
                                                if (p_d != null)
                                                {

                                                    if (p_d.StartDate == null || p_d.StartDate > p.StayDate)
                                                        p_d.StartDate = p.StayDate;
                                                    p_d.EndDate = p.StayDate.AddDays(1);
                                                }
                                                else
                                                {
                                                    var add_value = p;
                                                    add_value.StartDate = add_value.StayDate;
                                                    add_value.EndDate = add_value.StayDate.AddDays(1);
                                                    p.StayDate = (DateTime)add_value.StartDate;
                                                    p.SalePrice = p.TotalAmount;
                                                    p.OperatorPrice = p.Price;
                                                    package_daterange.Add(add_value);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            package_daterange.Add(p);
                                        }
                                    }
                                }
                                if (rooms != null && rooms.Count > 0)
                                    foreach (var item in rooms)
                                    {
                                        string RatePlanCode = String.Empty;
                                        string date = String.Empty;
                                        double Nights = 0;
                                        string TotalAmount = String.Empty;
                                        string operatorprice = String.Empty;
                                        string Goi = String.Empty;
                                        string TgSD = String.Empty;
                                        string GiaN = String.Empty;
                                        string SDem = String.Empty;
                                        string SP = String.Empty;
                                        string TTien = String.Empty;
                                        double NumberOfRooms = 0;
                                        var package_by_room_id = packages.Where(x => x.HotelBookingRoomOptionalId == item.Id);
                                        if (package_by_room_id != null && package_by_room_id.Count() > 0)
                                        {
                                            foreach (var p in package_by_room_id)
                                            {
                                                double operator_price = 0;
                                                if (p.Price != null) operator_price = Math.Round(((double)p.Price / (double)p.Nights / (double)item.NumberOfRooms), 0);
                                                if (operator_price <= 0) operator_price = p.OperatorPrice != null ? (double)p.OperatorPrice : 0;

                                                RatePlanCode = p.RatePlanCode;
                                                date = (p.StartDate == null ? "" : ((DateTime)p.StartDate).ToString("dd/MM/yyyy")) + " - " + (p.EndDate == null ? "" : ((DateTime)p.EndDate).ToString("dd/MM/yyyy"));
                                                operatorprice = operator_price.ToString("N0");
                                                Nights = (double)p.Nights;
                                                TotalAmount = p.Price == null ? ((double)p.TotalAmount - (double)p.Profit).ToString("N0") : ((double)p.Price).ToString("N0");
                                                NumberOfRooms = item.NumberOfRooms == null ? 1 : (double)item.NumberOfRooms;
                                                Goi += "<div style='border: 1px solid #999; padding: 2px; text-align: center;'>" + RatePlanCode + "</div>";
                                                TgSD += "<div style='border: 1px solid #999; padding:2px; text-align: center;'>" + date + "</div>";
                                                GiaN += "<div style='border: 1px solid #999; padding: 2px; text-align: center;'>" + operatorprice + "</div>";
                                                SDem += "<div style='border: 1px solid #999; padding: 2px; text-align: center;'>" + Nights + "</div>";
                                                SP = "<div style='border: 1px solid #999; padding: 2px; text-align: center;'>" + NumberOfRooms + "</div>";
                                                TTien += "<div style='border: 1px solid #999; padding: 2px; text-align: center;'>" + TotalAmount + "</div>";
                                            }

                                        }
                                        chitietdichvu += "<tr><td  style='border: 1px solid #999; padding: 2px;'>" + item.RoomTypeName + "</td>" +
                                                                    "<td style='border: 1px solid #999; padding: 2px; text-align: center;'>" + Goi + "</td>" +
                                                                      "<td style='border: 1px solid #999; padding:2px; text-align: center;'>" + TgSD + "</td>" +
                                                                     "<td style='border: 1px solid #999; padding: 2px; text-align: center;'>" + GiaN + "</td>" +
                                                                     "<td style='border: 1px solid #999; padding: 2px; text-align: center;'>" + SDem + "</td>" +
                                                                      "<td style='border: 1px solid #999; padding: 2px; text-align: center;'>" + SP + "</td>" +
                                                                      "<td style='border: 1px solid #999; padding: 2px; text-align: center;'>" + TTien + "</td>"
                                                                      + "</tr>";

                                    }
                                if (extra_package != null && extra_package.Count > 0)
                                    foreach (var item in extra_package)
                                    {
                                        double operator_price = 0;
                                        if (item.UnitPrice != null) operator_price = Math.Round(((double)item.UnitPrice / (double)item.Nights / (double)item.Quantity), 0);
                                        if (operator_price <= 0) operator_price = item.OperatorPrice != null ? (double)item.OperatorPrice : 0;
                                        chitietdichvukhac += "<tr><td style='border: 1px solid #999; padding: 2px;'>" + item.PackageCode + "</td>" +
                                                                          "<td style='border: 1px solid #999; padding: 2px; text-align: center;'>" + item.PackageId + "</td>" +
                                                                          "<td style='border: 1px solid #999; padding: 2px; text-align: center;'>" + (item.StartDate == null ? "" : ((DateTime)item.StartDate).ToString("dd/MM/yyyy")) + " - " + (item.EndDate == null ? "" : ((DateTime)item.EndDate).ToString("dd/MM/yyyy")) + "</td>" +
                                                                          "<td style='border: 1px solid #999; padding: 2px; text-align: center;'>" + operator_price.ToString("N0") + "</td>" +
                                                                          "<td style='border: 1px solid #999; padding: 2px; text-align: center;'>" + (item.Nights != null ? ((double)item.Nights).ToString("N0") : "1") + "</td>" +
                                                                          "<td style='border: 1px solid #999; padding: 2px; text-align: center;'>" + (item.Quantity != null ? ((double)item.Quantity).ToString("N0") : "1") + "</td>" +
                                                                          "<td style='border: 1px solid #999; padding: 2px; text-align: center;'>" + (item.UnitPrice == null ? ((double)item.Amount - (double)item.Profit).ToString("N0") : ((double)item.UnitPrice).ToString("N0")) + "</td>" +
                                                                          "</tr>";


                                    }

                                datatabledv = "<table style='border-collapse: collapse;width:100%;'>" +
                                                              "<thead>" +
                                                                  "<tr>" +
                                                                      "<th style='border: 1px solid #999; padding: 2px; text-align: center;'>Hạng phòng</th>" +
                                                                      "<th style='border: 1px solid #999; padding: 2px; text-align: center;'>Gói</th>" +
                                                                      "<th style='border: 1px solid #999; padding: 2px; text-align: center;'>Thời gian sử dụng</th>" +
                                                                      "<th style='border: 1px solid #999; padding: 2px; text-align: center;'>Giá nhập</th>" +
                                                                      "<th style='border: 1px solid #999; padding: 2px; text-align: center;'>Số đêm</th>" +
                                                                      "<th style='border: 1px solid #999; padding: 2px; text-align: center;'>Số phòng</th>" +
                                                                      "<th style='border: 1px solid #999; padding: 2px; text-align: center;'>Thành tiền</th>" +
                                                                  "</tr> " +
                                                              "</thead>" +
                                                              "<tbody>" +
                                                                  chitietdichvu +

                                                             "</tbody>" +
                                                         "</table>";
                                if (extra_package != null && extra_package.Count > 0)
                                {
                                    datatabledvkhac = "<table style='border-collapse: collapse;width:100%;'>" +
                                                         "<thead>" +
                                                             "<tr>" +
                                                                 "<th style='border: 1px solid #999; padding: 2px; text-align: center;'>Tên dịch vụ</th>" +
                                                                 "<th style='border: 1px solid #999; padding: 2px; text-align: center;'>Gói</th>" +
                                                                 "<th style='border: 1px solid #999; padding: 2px; text-align: center;'>Thời gian sử dụng</th>" +
                                                                 "<th style='border: 1px solid #999; padding: 2px; text-align: center;'>Giá nhập</th>" +
                                                                 "<th style='border: 1px solid #999; padding: 2px; text-align: center;'>Số ngày	</th>" +
                                                                 "<th style='border: 1px solid #999; padding: 2px; text-align: center;'>Số lượng</th>" +
                                                                 "<th style='border: 1px solid #999; padding: 2px; text-align: center;'>Thành tiền</th>" +
                                                             "</tr> " +
                                                         "</thead>" +
                                                         "<tbody>" +
                                                             chitietdichvukhac +

                                                        "</tbody>" +
                                                    "</table>";
                                }
                                else
                                {
                                    datatabledvkhac = "";
                                }


                                if (chitietdichvu == "")
                                {

                                    body = body.Replace("{{styledv}}", "style=\"display:none;\"");
                                    body = body.Replace("{{datatabledv}}", datatabledv);
                                }
                                else
                                {
                                    body = body.Replace("{{datatabledv}}", datatabledv);
                                }
                                var hotel = await _hotelBookingRepositories.GetHotelBookingByID(modelEmail.ServiceId);
                                if (hotel.SupplierId == (int)modelEmail.SupplierId)
                                {
                                    body = body.Replace("{{datatabledvkhac}}", datatabledvkhac);
                                }
                                else
                                {

                                    body = body.Replace("{{styledvkhac}}", "style =\"display:none;\"");
                                    body = body.Replace("{{datatabledvkhac}}", "");
                                }




                                body = body.Replace("{{datatable}}", datatable);
                                body = body.Replace("{{userName}}", modelEmail.user_Name);
                                body = body.Replace("{{HotelName}}", modelEmail.TileEmail);
                                body = body.Replace("{{userPhone}}", modelEmail.user_Phone);
                                body = body.Replace("{{userEmail}}", modelEmail.user_Email);
                                body = body.Replace("{{orderNo}}", modelEmail.OrderNo);
                                body = body.Replace("{{ArrivalDate}}", modelEmail.go_startdate);
                                body = body.Replace("{{DepartureDate}}", modelEmail.go_enddate);
                                body = body.Replace("{{NumberOfRoom}}", modelEmail.NumberOfRoom);
                                body = body.Replace("{{numberOfAdult}}", modelEmail.go_numberOfAdult);
                                body = body.Replace("{{numberOfChild}}", modelEmail.go_numberOfChild);
                                body = body.Replace("{{numberOfInfant}}", modelEmail.go_numberOfInfant);
                                body = body.Replace("{{totalAmount}}", modelEmail.totalAmount.ToString("N0"));
                                body = body.Replace("{{OrderAmount}}", modelEmail.OrderAmount.ToString("N0"));

                                body = body.Replace("{{salerName}}", modelEmail.saler_Name);
                                body = body.Replace("{{salerPhone}}", modelEmail.saler_Phone);
                                body = body.Replace("{{salerEmail}}", modelEmail.saler_Email);
                                body = body.Replace("{{totalToday}}", modelEmail.totalToday);
                                /* body = body.Replace("{{Note}}", modelEmail.OrderNote);*/
                                /*    body = body.Replace("{{payment_notification}}", modelEmail.PaymentNotification);*/

                                return body;
                            }
                            break;
                        case (int)ServicesType.Other:
                            {
                                string datatabledv = String.Empty;
                                string datatabledvkhac = String.Empty;
                                string chitietdichvu = String.Empty;
                                var template = workingDirectory + @"\EmailTemplate\OtherSupplierTemplate.html";

                                string body = File.ReadAllText(template);
                                var model = await _otherBookingRepository.GetDetailOtherBookingById(Convert.ToInt32(modelEmail.ServiceId));
                                var extra_package = await _otherBookingRepository.GetOtherBookingPackagesOptionalByBookingId(Convert.ToInt32(modelEmail.ServiceId));

                                if (model == null) return null;

                                var order = await _orderRepository.GetOrderByID(model[0].OrderId);
                                var Dh = await _userRepository.GetById((long)model[0].OperatorId);
                                if (extra_package != null && extra_package.Count() > 0)
                                {
                                    extra_package = extra_package.Where(s => s.SuplierId == SupplierId).ToList();
                                    foreach (var item in extra_package)
                                    {

                                        chitietdichvu += "<tr><td  style='border: 1px solid #999; padding: 2px; text-align: center;'>" + model[0].ServiceName + "</td>" +

                                                                     "<td style='border: 1px solid #999; padding: 2px; text-align: center;'>" + (item.BasePrice != null ? (double)item.BasePrice : 0).ToString("N0") + "</td>" +
                                                                    "<td style='border: 1px solid #999; padding: 2px; text-align: center;'>" + item.Quantity + "</td>" +
                                                                    "<td style='border: 1px solid #999; padding: 2px; text-align: center;'>" + item.Amount.ToString("N0") + "</td>"
                                                                    + "</tr>";
                                    }

                                }


                                if (chitietdichvu != string.Empty)
                                {
                                    datatabledv = "<table style='border-collapse: collapse;width:100%;'>" +
                                                                "<thead>" +
                                                                    "<tr>" +

                                                                        "<th style='border: 1px solid #999; padding: 2px; text-align: center;'>Loại dịch vụ</th>" +

                                                                        "<th style='border: 1px solid #999; padding: 2px; text-align: center;'>Giá nhập</th>" +

                                                                        "<th style='border: 1px solid #999; padding: 2px; text-align: center;'>Số lượng</th>" +
                                                                        "<th style='border: 1px solid #999; padding: 2px; text-align: center;'>Thành tiền</th>" +
                                                                    "</tr> " +
                                                                "</thead>" +
                                                                "<tbody>" +
                                                                    chitietdichvu +
                                                               "</tbody>" +
                                                           "</table>";
                                }
                                else
                                {
                                    datatabledv = "";
                                }
                                if (datatabledv == "")
                                {

                                    body = body.Replace("{{styledv}}", "style=\"display:none;\"");
                                    body = body.Replace("{{datatabledv}}", datatabledv);
                                }
                                else
                                {
                                    body = body.Replace("{{datatabledv}}", datatabledv);
                                }



                                body = body.Replace("{{datatable}}", modelEmail.datatable);
                                body = body.Replace("{{userName}}", modelEmail.user_Name);
                                body = body.Replace("{{HotelName}}", modelEmail.TileEmail);
                                body = body.Replace("{{userPhone}}", modelEmail.user_Phone);
                                body = body.Replace("{{userEmail}}", modelEmail.user_Email);
                                body = body.Replace("{{orderNo}}", modelEmail.OrderNo);
                                body = body.Replace("{{ArrivalDate}}", modelEmail.go_startdate);
                                body = body.Replace("{{DepartureDate}}", modelEmail.go_enddate);
                                body = body.Replace("{{NumberOfRoom}}", modelEmail.NumberOfRoom);
                                body = body.Replace("{{numberOfAdult}}", modelEmail.go_numberOfAdult);
                                body = body.Replace("{{numberOfChild}}", modelEmail.go_numberOfChild);
                                body = body.Replace("{{numberOfInfant}}", modelEmail.go_numberOfInfant);
                                body = body.Replace("{{totalAmount}}", modelEmail.totalAmount.ToString("N0"));
                                body = body.Replace("{{OrderAmount}}", modelEmail.OrderAmount.ToString("N0"));

                                body = body.Replace("{{salerName}}", modelEmail.saler_Name);
                                body = body.Replace("{{salerPhone}}", modelEmail.saler_Phone);
                                body = body.Replace("{{salerEmail}}", modelEmail.saler_Email);
                                body = body.Replace("{{totalToday}}", modelEmail.totalToday);

                                body = body.Replace("{{Note}}", modelEmail.OrderNote);
                                body = body.Replace("{{payment_notification}}", modelEmail.PaymentNotification);

                                return body;
                            }
                            break;
                    }

                    return null;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetTemplateSupplier - MailService: " + ex.ToString());
                return null;
            }
        }
        public async Task<string> GetTemplateinsertUser(SendEmailViewModel modelEmail, long id, string order_note = "", string payment_notification = "", bool is_edit_form = false)
        {
            try
            {
                if (modelEmail == null)
                {

                    //string workingDirectory = Directory.GetCurrentDirectory();
                    string workingDirectory = AppDomain.CurrentDomain.BaseDirectory;
                    var template = workingDirectory + @"\EmailTemplate\HotelBookingTemplate.html";
                    string body = File.ReadAllText(template);
                    string passenger = String.Empty;
                    int Type = 1;
                    var DetailHotelBooking = await _hotelBookingRepositories.GetDetailHotelBookingByID(Convert.ToInt32(id));
                    var sumtoday = DetailHotelBooking.Sum(s => s.TotalDays);
                    var datahotelbookingroomextrapackage = await _hotelBookingRoomExtraPackageRepository.Gethotelbookingroomextrapackagebyhotelbookingid(Convert.ToInt32(id));

                    var model = await _hotelBookingRepositories.GetHotelBookingById(id);
                    var hotel = await _hotelBookingRepositories.GetHotelBookingByID(id);
                    var order = await _orderRepository.GetOrderByID((long)hotel.OrderId);
                    var saler = await _userRepository.GetById((long)order.SalerId);

                    foreach (var item in datahotelbookingroomextrapackage)
                    {
                        passenger += "" + item.PackageCode + "&#10 ";

                    }

                    var code = await _hotelBookingCodeRepository.GetListlBookingCodeByHotelBookingId(id, (int)ServiceType.BOOK_HOTEL_ROOM_VIN);
                    string passenger2 = "";
                    if (code != null && code.Count > 0)
                    {
                        foreach (var dv in code)
                        {
                            passenger2 += "Mã : " + dv.BookingCode + ", nội dung: " + dv.Description + " &#10";
                        }
                    }
                    //body = body.Replace("{{datatable}}", passenger);
                    body = body.Replace("{{datatable}}", "<textarea id=\"datatable\" style=\"height: 200px !important;\">" + passenger + "</textarea>");
                    body = body.Replace("{{datatableCode}}", "<textarea id=\"datatableCode\" style=\"height: 200px !important;\">" + passenger2 + "</textarea>");
                    body = body.Replace("{{userName}}", "<input type =\"text\" id=\"user_Name\" value=\"" + model[0].ContactClientName + "\" />");
                    body = body.Replace("{{HotelName}}", "<input type =\"text\" id=\"TileEmail\" style=\"text-align: center;font-weight: bold;\"value=\"PHIẾU XÁC NHẬN DỊCH VỤ: " + model[0].HotelName + "\" />");
                    body = body.Replace("{{userPhone}}", "<input type =\"text\" id=\"user_Phone\" value=\"" + model[0].ContactClientPhone + "\" />");
                    body = body.Replace("{{userEmail}}", "<input type =\"text\" id=\"user_Email\" value=\"" + model[0].ContactClientEmail + "\" />");
                    body = body.Replace("{{orderNo}}", "<input type =\"text\" id=\"orderNo\" value=\"" + model[0].OrderNo + "\" />");
                    body = body.Replace("{{ArrivalDate}}", "<input type =\"text\"style=\"min-width: 100px;\" id=\"go_startdate\" value=\"" + model[0].ArrivalDate.ToString("dd/MM/yyyy") + "\" />");
                    body = body.Replace("{{DepartureDate}}", "<input type =\"text\" id=\"go_enddate\" value=\"" + model[0].DepartureDate.ToString("dd/MM/yyyy") + "\" />");

                    body = body.Replace("{{NumberOfRoom}}", "<input type =\"text\" id=\"NumberOfRoom\" value=\"" + model[0].NumberOfRoom.ToString() + "\" />");
                    body = body.Replace("{{numberOfAdult}}", "<input style=\"width:30% !important;\" type =\"text\" id=\"go_numberOfAdult\" value=\"" + model[0].NumberOfAdult.ToString() + "\" />");
                    body = body.Replace("{{numberOfChild}}", "<input style=\"width:30% !important;\" type =\"text\" id=\"go_numberOfChild\" value=\"" + model[0].NumberOfChild.ToString() + "\" />");
                    body = body.Replace("{{numberOfInfant}}", "<input style=\"width:30% !important;\" type =\"text\" id=\"go_numberOfInfant\" value=\"" + model[0].NumberOfInfant.ToString() + "\" />");

                    body = body.Replace("{{totalAmount}}", "<input type =\"text\" class=\"currency\" id=\"totalAmount\" value=\"" + model[0].TotalAmount.ToString("###,###") + "\" />");
                    body = body.Replace("{{OrderAmount}}", "<input type =\"text\" class=\"currency\" id=\"OrderAmount\" value=\"" + model[0].Amount.ToString("###,###") + "\" />");
                    body = body.Replace("{{SalerName}}", "<input type =\"text\" id=\"saler_Name\" value=\"" + saler.FullName + "\" />");
                    body = body.Replace("{{SalerPhone}}", "<input type =\"text\" id=\"saler_Phone\" value=\"" + saler.Phone + "\" />");
                    body = body.Replace("{{SalerEmail}}", "<input type =\"text\" id=\"saler_Email\" value=\"" + saler.Email + "\" />");
                    body = body.Replace("{{totalToday}}", "<input type=\"text\" id=\"totalToday\" value=\"" + sumtoday.ToString() + "\" />");


                    body = body.Replace("{{Note}}", "<input type=\"text\" id=\"order_note\" value=\"" + order_note + "\" />");
                    body = body.Replace("{{payment_notification}}", "<input type=\"text\" id=\"payment_notification\" value=\"" + payment_notification + "\" />");

                    body = body.Replace("{{DKHuy}}", "<input type=\"text\" id=\"DKHuy\" value=\"Không hoàn hủy\" />");
                    body = body.Replace("{{NDChuyenKhoan}}", "<input type=\"text\" id=\"NDChuyenKhoan\" value=\"" + order.OrderNo + " CHUYEN KHOAN\" />");
                    //body = body.Replace("{{TileEmail}}", "<input type=\"text\"style=\"text-align: center;font-weight: bold;\" id=\"TileEmail\" value=\"PHIẾU XÁC NHẬN ĐƠN HÀNG\" />");
                    string TTChuyenKhoan = "1. VP bank STK: 9698888 &#10 " +
                                   "CTK: Công ty Cổ phần Thương mại và Dịch vụ Quốc tế Đại Việt &#10 " +
                                   "Chi nhánh: Thăng Long &#10 ";
                    body = body.Replace("{{TTChuyenKhoan}}", "<textarea id=\"TTChuyenKhoan\" style=\"height: 425px !important;\">" + TTChuyenKhoan + "</textarea>");
                    return body;
                }
                else
                {
                    string workingDirectory = AppDomain.CurrentDomain.BaseDirectory;
                    var template = workingDirectory + @"\EmailTemplate\HotelBookingTemplate.html";
                    string body = File.ReadAllText(template);

                    string datatable = string.Empty;
                    string datatableCode = string.Empty;
                    string TTChuyenKhoan = string.Empty;






                    if (modelEmail.TTChuyenKhoan != null)
                    {
                        var list = Array.ConvertAll(modelEmail.TTChuyenKhoan.Split('\n'), s => (s).ToString());
                        foreach (var item in list)
                        {
                            TTChuyenKhoan += "<div>" + item + "</div>";
                        }
                        body = body.Replace("{{TTChuyenKhoan}}", TTChuyenKhoan);
                    }
                    else
                    {
                        body = body.Replace("{{TTChuyenKhoan}}", modelEmail.TTChuyenKhoan);
                    }
                    if (modelEmail.datatable != null)
                    {
                        var listdatatable = Array.ConvertAll(modelEmail.datatable.Split('\n'), s => (s).ToString());

                        foreach (var item in listdatatable)
                        {
                            datatable += "<div>" + item + "</div>";
                        }
                        body = body.Replace("{{datatable}}", datatable);
                    }
                    else
                    {
                        body = body.Replace("{{datatable}}", modelEmail.datatable);
                    }

                    if (modelEmail.datatableCode != null)
                    {
                        var listdatatableCode = Array.ConvertAll(modelEmail.datatableCode.Split('\n'), s => (s).ToString());
                        foreach (var item in listdatatableCode)
                        {
                            datatableCode += "<div>" + item + "</div>";
                        }
                        body = body.Replace("{{datatableCode}}", datatableCode);
                    }
                    else
                    {
                        body = body.Replace("{{datatableCode}}", modelEmail.datatableCode);
                    }


                    body = body.Replace("{{datatable}}", datatable);


                    body = body.Replace("{{NDChuyenKhoan}}", modelEmail.NDChuyenKhoan);
                    body = body.Replace("{{DKHuy}}", modelEmail.DKHuy);
                    body = body.Replace("{{userName}}", modelEmail.user_Name);
                    body = body.Replace("{{HotelName}}", modelEmail.TileEmail);
                    body = body.Replace("{{userPhone}}", modelEmail.user_Phone);
                    body = body.Replace("{{userEmail}}", modelEmail.user_Email);
                    body = body.Replace("{{orderNo}}", modelEmail.OrderNo);
                    body = body.Replace("{{ArrivalDate}}", modelEmail.go_startdate);
                    body = body.Replace("{{DepartureDate}}", modelEmail.go_enddate);
                    body = body.Replace("{{NumberOfRoom}}", modelEmail.NumberOfRoom);
                    body = body.Replace("{{numberOfAdult}}", modelEmail.go_numberOfAdult);
                    body = body.Replace("{{numberOfChild}}", modelEmail.go_numberOfChild);
                    body = body.Replace("{{numberOfInfant}}", modelEmail.go_numberOfInfant);
                    body = body.Replace("{{totalAmount}}", modelEmail.totalAmount.ToString("N0"));
                    body = body.Replace("{{OrderAmount}}", modelEmail.OrderAmount.ToString("N0"));
                    body = body.Replace("{{SalerName}}", modelEmail.saler_Name);
                    body = body.Replace("{{SalerPhone}}", modelEmail.saler_Phone);
                    body = body.Replace("{{SalerEmail}}", "<a href='#'>" + modelEmail.saler_Email + "");
                    body = body.Replace("{{totalToday}}", modelEmail.totalToday);
                    body = body.Replace("{{Note}}", modelEmail.OrderNote);
                    body = body.Replace("{{payment_notification}}", modelEmail.PaymentNotification);

                    return body;

                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetTemplateinsertUser - MailService: " + ex.ToString());
                return null;
            }
        }
        public async Task<string> GetFlyBookingTemplateBody(SendEmailViewModel model, string group_booking_id, string order_note = "", string payment_notification = "", bool is_edit_form = false)
        {
            try
            {

                if (model == null)
                {


                    var fly_list = await _flyBookingDetailRepository.GetListByGroupFlyID(group_booking_id);
                    if (fly_list == null || fly_list.Count < 1)
                    {
                        LogHelper.InsertLogTelegram("GetFlyBookingTemplateBody - MailService: NULL DATA with [" + group_booking_id + "]");
                        return null;
                    }
                    var extra = await _flyBookingDetailRepository.GetExtraPackageByFlyBookingId(group_booking_id);
                    var passenger = await _passengerRepository.GetByOrderID(fly_list[0].OrderId, fly_list[0].GroupBookingId);
                    var baggage = _bagageRepository.GetBaggages(passenger.Select(x => x.Id).ToList());
                    var order = await _orderRepository.GetOrderByID(fly_list[0].OrderId);


                    var saler = await _userRepository.GetById((long)order.SalerId);
                    var client = await _clientRepository.GetClientDetailAsync((long)order.ClientId);
                    //string workingDirectory = Directory.GetCurrentDirectory();
                    string workingDirectory = AppDomain.CurrentDomain.BaseDirectory;
                    var template = workingDirectory + @"\EmailTemplate\FlyBookingServiceCodeTemplate.html";
                    string body = File.ReadAllText(template);


                    if (order == null || order.ContactClientId != null)
                    {
                        var contact = _contactClientRepository.GetByContactClientId((long)order.ContactClientId);
                        body = body.Replace("{{userName}}", "<input type=\"text\" id=\"user_Name\" value=\"" + contact.Name + "\" />");
                        body = body.Replace("{{userPhone}}", "<input type=\"text\" id=\"user_Phone\" value=\"" + contact.Mobile + "\" />");
                        body = body.Replace("{{userEmail}}", "<input type=\"text\" id=\"user_Email\" value=\"" + contact.Email + "\" />");
                    }
                    else
                    {
                        body = body.Replace("{{userName}}", "<input type=\"text\" id=\"user_Name\" value=\"\" />");
                        body = body.Replace("{{userPhone}}", "<input type=\"text\" id=\"user_Phone\" value=\"\" />");
                        body = body.Replace("{{userEmail}}", "<input type=\"text\" id=\"user_Email\" value=\"\" />");
                    }

                    body = body.Replace("{{HotelName}}", "<input type=\"text\" id=\"TileEmail\" style=\"text-align: center;font-weight: bold;\"value=\"PHIẾU XÁC NHẬN DỊCH VỤ: Vé máy bay\" />");

                    body = body.Replace("{{orderNo}}", "<input type=\"text\" id=\"orderNo\" value=\"" + order.OrderNo + "\" />");
                    var go = fly_list.First(x => x.Leg == 0);
                    body = body.Replace("{{go_startdate}}", "<input type=\"text\" style=\"min-width: 100px;\" id=\"go_startdate\" value=\"" + ((DateTime)go.StartDate).ToString("dd/MM/yyyy HH:mm") + "\" />");
                    body = body.Replace("{{go_enddate}}", "<input type=\"text\" id=\"go_enddate\" value=\"" + ((DateTime)go.EndDate).ToString("dd/MM/yyyy HH:mm") + "\" />");
                    body = body.Replace("{{go_startpoint}}", "<input type=\"text\" id=\"go_startpoint\" value=\"" + go.StartPoint + "\" />");
                    body = body.Replace("{{go_endpoint}}", "<input type=\"text\" id=\"go_endpoint\" value=\"" + go.EndPoint + "\" />");
                    body = body.Replace("{{go_airline}}", "<input type=\"text\" id=\"go_airline\" value=\"" + go.Airline + "\" />");
                    body = body.Replace("{{go_numberOfAdult}}", "<input  style=\"width:30% !important;\"type=\"text\" id=\"go_numberOfAdult\" value=\"" + go.AdultNumber.ToString() + "\" />");
                    body = body.Replace("{{go_numberOfChild}}", "<input  style=\"width:30% !important;\"type=\"text\" id=\"go_numberOfChild\" value=\"" + go.ChildNumber.ToString() + "\" />");
                    body = body.Replace("{{go_numberOfInfant}}", "<input  style=\"width:30% !important;\"type=\"text\" id=\"go_numberOfInfant\" value=\"" + go.InfantNumber.ToString() + "\" />");
                    body = body.Replace("{{totalamount}}", "<input type=\"text\"  class=\"currency\" id=\"totalAmount\" value=\"" + ((double)order.Amount).ToString("N0") + "\" />");
                    string extra_list = "";
                    string template_extra = @"<div> item.PackageCode </div>";
                    string template_baggage = @"<div > Hành lý chiều {leg}: item.PackageCode </div>";
                    if (extra != null && extra.Count > 0)
                    {
                        foreach (var package in extra)
                        {
                            extra_list += template_extra.Replace("item.PackageCode", package.PackageCode);
                        }
                    }
                    if (baggage != null && baggage.Count > 0)
                    {
                        var go_baggage = baggage.Where(x => x.Leg == 0).ToList();
                        if (go_baggage != null && go_baggage.Count > 0)
                        {
                            foreach (var b in go_baggage)
                            {
                                extra_list += template_baggage.Replace("{leg}", "Chiều đi").Replace("item.PackageCode", b.Code);
                            }
                        }
                    }
                    var back = fly_list.FirstOrDefault(x => x.Leg == 1);
                    var code = await _hotelBookingCodeRepository.GetListlBookingCodeByHotelBookingId(go.Id, (int)ServiceType.PRODUCT_FLY_TICKET);
                    string passenger2 = "";
                    if (code != null && code.Count > 0)
                    {
                        foreach (var dv in code)
                        {
                            passenger2 += "Mã : " + dv.BookingCode + ", nội dung: " + dv.Description + " &#10";
                        }
                    }
                    //body = body.Replace("{{datatable}}", passenger);

                    body = body.Replace("{{datatableCode}}", "<textarea id=\"datatableCode\" style=\"height: 200px !important;\">" + passenger2 + "</textarea>");
                    //if (code == null || code.Count <= 0) return null;
                    if (back != null && back.Id > 0)
                    {
                        body = body.Replace("display:none;", "");

                        body = body.Replace("{{back_startdate}}", "<input type=\"text\" id=\"back_startdate\" value=\"" + ((DateTime)back.StartDate).ToString("dd/MM/yyyy HH:mm") + "\" />");
                        body = body.Replace("{{back_enddate}}", "<input type=\"text\" id=\"back_enddate\" value=\"" + ((DateTime)back.EndDate).ToString("dd/MM/yyyy HH:mm") + "\" />");
                        body = body.Replace("{{back_startpoint}}", "<input type=\"text\" id=\"back_startpoint\" value=\"" + back.StartPoint + "\" />");
                        body = body.Replace("{{back_endpoint}}", "<input type=\"text\" id=\"back_endpoint\" value=\"" + back.EndPoint + "\" />");
                        body = body.Replace("{{back_airline}}", "<input type=\"text\" id=\"back_airline\" value=\"" + back.Airline + "\" />");
                        body = body.Replace("{{back_numberOfAdult}}", "<input  style=\"width:30% !important;\" type=\"text\" id=\"back_numberOfAdult\" value=\"" + back.AdultNumber.ToString() + "\" />");
                        body = body.Replace("{{back_numberOfChild}}", "<input  style=\"width:30% !important;\"type=\"text\" id=\"back_numberOfChild\" value=\"" + back.ChildNumber.ToString() + "\" />");
                        body = body.Replace("{{back_numberOfInfant}}", "<input  style=\"width:30% !important;\"type=\"text\" id=\"back_numberOfInfant\" value=\"" + back.InfantNumber.ToString() + "\" />");





                        if (baggage != null && baggage.Count > 0)
                        {
                            var back_baggage = baggage.Where(x => x.Leg == 1).ToList();
                            if (back_baggage != null && back_baggage.Count > 0)
                            {
                                foreach (var b in back_baggage)
                                {
                                    extra_list += template_baggage.Replace("{leg}", "Chiều về").Replace("item.PackageCode", b.Code);
                                }
                            }
                        }
                    }
                    body = body.Replace("{{datatable}}", "<textarea id=\"datatable\" style='min-height: 200px;'>" + extra_list + "</textarea>");
                    if (code != null && code.Count > 0)
                    {


                        body = body.Replace("{{code_1_code}}", "<input type=\"text\" id=\"saler_Name\" value=\"" + code[0].BookingCode + "\" />");
                        body = body.Replace("{{code_1_description}}", "<input type=\"text\" id=\"saler_Name\" value=\"" + code[0].Description + "\" />");
                    }
                    else
                    {
                        body = body.Replace("{{code_1_code}}", "<input type=\"text\" id=\"code_1_code\" value=\"\" />");
                        body = body.Replace("{{code_1_description}}", "<input type=\"text\" id=\"code_1_description\" value=\"\" />");
                    }
                    if (saler != null && saler.Id > 0)
                    {
                        body = body.Replace("{{SalerName}}", "<input type=\"text\" id=\"saler_Name\" value=\"" + saler.FullName + "\" />");
                        body = body.Replace("{{SalerPhone}}", "<input type=\"text\" id=\"saler_Phone\" value=\"" + saler.Phone + "\" />");
                        body = body.Replace("{{SalerEmail}}", "<input type=\"text\" id=\"saler_Email\" value=\"" + saler.Email + "\" />");
                    }
                    else
                    {
                        body = body.Replace("{{SalerName}}", "<input type=\"text\" id=\"saler_Name\" value=\"Phòng hỗ trợ khách hàng\" />");
                        body = body.Replace("{{SalerPhone}}", "<input type=\"text\" id=\"saler_Phone\" value=\"0936.191.192\" />");
                        body = body.Replace("{{SalerEmail}}", "<input type=\"text\" id=\"saler_Email\" value=\"\" />");
                    }
                    if (is_edit_form)
                    {
                        body = body.Replace("{{Note}}", "<input type=\"text\" id=\"order_note\" value=\"" + order_note + "\" />");
                        body = body.Replace("{{payment_notification}}", "<input type=\"text\" id=\"payment_notification\" value=\"" + payment_notification + "\" />");
                        body = body.Split("</head>")[1];
                        body = body.Split("</html>")[0];
                    }
                    else
                    {
                        body = body.Replace("{{Note}}", order_note);
                        body = body.Replace("{{payment_notification}}", payment_notification);
                    }
                    body = body.Replace("{{DKHuy}}", "<input type=\"text\" id=\"DKHuy\" value=\"Không hoàn hủy\" />");
                    body = body.Replace("{{payment_notification}}", payment_notification);



                    return body;
                }
                else
                {
                    var fly_list = await _flyBookingDetailRepository.GetListByGroupFlyID(group_booking_id);
                    if (fly_list == null || fly_list.Count < 1)
                    {
                        LogHelper.InsertLogTelegram("GetFlyBookingTemplateBody - MailService: NULL DATA with [" + group_booking_id + "]");
                        return null;
                    }
                    var extra = await _flyBookingDetailRepository.GetExtraPackageByFlyBookingId(group_booking_id);
                    var passenger = await _passengerRepository.GetByOrderID(fly_list[0].OrderId, fly_list[0].GroupBookingId);
                    var baggage = _bagageRepository.GetBaggages(passenger.Select(x => x.Id).ToList());
                    var order = await _orderRepository.GetOrderByID(fly_list[0].OrderId);

                    var saler = await _userRepository.GetById((long)order.SalerId);
                    var client = await _clientRepository.GetClientDetailAsync((long)order.ClientId);
                    //string workingDirectory = Directory.GetCurrentDirectory();
                    string workingDirectory = AppDomain.CurrentDomain.BaseDirectory;
                    var template = workingDirectory + @"\EmailTemplate\FlyBookingServiceCodeTemplate.html";
                    string body = File.ReadAllText(template);

                    body = body.Replace("{{userName}}", model.user_Name);
                    body = body.Replace("{{HotelName}}", model.TileEmail);
                    body = body.Replace("{{userPhone}}", model.user_Phone);
                    body = body.Replace("{{userEmail}}", model.user_Email);
                    body = body.Replace("{{orderNo}}", model.OrderNo);
                    var go = fly_list.First(x => x.Leg == 0);
                    body = body.Replace("{{go_startdate}}", model.go_startdate);
                    body = body.Replace("{{go_enddate}}", model.go_enddate);
                    body = body.Replace("{{go_startpoint}}", model.go_startpoint);
                    body = body.Replace("{{go_endpoint}}", model.go_endpoint);
                    body = body.Replace("{{go_airline}}", model.go_airline);
                    body = body.Replace("{{go_numberOfAdult}}", model.go_numberOfAdult);
                    body = body.Replace("{{go_numberOfChild}}", model.go_numberOfChild);
                    body = body.Replace("{{go_numberOfInfant}}", model.go_numberOfInfant);
                    body = body.Replace("{{totalamount}}", model.totalAmount.ToString("N0"));
                    string extra_list = "";
                    string template_extra = @"<div> item.PackageCode </div>";
                    string template_baggage = @"<div > Hành lý chiều {leg}: item.PackageCode </div>";
                    if (extra != null && extra.Count > 0)
                    {
                        foreach (var package in extra)
                        {
                            extra_list += template_extra.Replace("item.PackageCode", package.PackageCode);
                        }
                    }
                    if (baggage != null && baggage.Count > 0)
                    {
                        var go_baggage = baggage.Where(x => x.Leg == 0).ToList();
                        if (go_baggage != null && go_baggage.Count > 0)
                        {
                            foreach (var b in go_baggage)
                            {
                                extra_list += template_baggage.Replace("{leg}", "Chiều đi").Replace("item.PackageCode", b.Code);
                            }
                        }
                    }
                    var back = fly_list.FirstOrDefault(x => x.Leg == 1);
                    var code = await _hotelBookingCodeRepository.GetListlBookingCodeByHotelBookingId(go.Id, (int)ServiceType.PRODUCT_FLY_TICKET);
                    if (code == null || code.Count <= 0) return null;
                    if (back != null && back.Id > 0)
                    {
                        body = body.Replace("display:none;", "");

                        body = body.Replace("{{back_startdate}}", model.back_startdate);
                        body = body.Replace("{{back_enddate}}", model.back_enddate);
                        body = body.Replace("{{back_startpoint}}", model.back_startpoint);
                        body = body.Replace("{{back_endpoint}}", model.back_endpoint);
                        body = body.Replace("{{back_airline}}", model.back_airline);
                        body = body.Replace("{{back_numberOfAdult}}", model.back_numberOfAdult);
                        body = body.Replace("{{back_numberOfChild}}", model.back_numberOfChild);
                        body = body.Replace("{{back_numberOfInfant}}", model.back_numberOfInfant);



                        body = body.Replace("{{code_2_code}}", model.code_2_code);
                        body = body.Replace("{{code_2_description}}", model.code_2_description);




                        if (baggage != null && baggage.Count > 0)
                        {
                            var back_baggage = baggage.Where(x => x.Leg == 1).ToList();
                            if (back_baggage != null && back_baggage.Count > 0)
                            {
                                foreach (var b in back_baggage)
                                {
                                    extra_list += template_baggage.Replace("{leg}", "Chiều về").Replace("item.PackageCode", b.Code);
                                }
                            }
                        }
                    }


                    if (code.Count > 0)
                    {
                        body = body.Replace("{{code_1_code}}", model.code_1_code);
                        body = body.Replace("{{code_1_description}}", model.code_1_description);
                    }

                    string TTChuyenKhoan = string.Empty;
                    string datatable = string.Empty;
                    string datatableCode = string.Empty;
                    if (model.datatableCode != null)
                    {
                        var listdatatableCode = Array.ConvertAll(model.datatableCode.Split('\n'), s => (s).ToString());

                        foreach (var item in listdatatableCode)
                        {
                            datatableCode += "<div>" + item + "</div>";
                        }
                        body = body.Replace("{{datatableCode}}", datatableCode);
                    }
                    else
                    {
                        body = body.Replace("{{datatableCode}}", model.datatableCode);
                    }
                    if (model.datatable != null)
                    {
                        var listdatatable = Array.ConvertAll(model.datatable.Split('\n'), s => (s).ToString());


                        foreach (var item in listdatatable)
                        {
                            datatable += "<div>" + item + "</div>";
                        }
                        body = body.Replace("{{datatable}}", datatable);
                    }
                    else
                    {
                        body = body.Replace("{{datatable}}", model.datatable);
                    }



                    body = body.Replace("{{datatableCode}}", datatableCode);

                    body = body.Replace("{{SalerName}}", model.saler_Name);
                    body = body.Replace("{{SalerPhone}}", model.saler_Phone);
                    body = body.Replace("{{SalerEmail}}", model.saler_Email);
                    body = body.Replace("{{Note}}", model.OrderNote);

                    body = body.Replace("{{DKHuy}}", model.DKHuy);
                    body = body.Replace("{{payment_notification}}", model.PaymentNotification);

                    return body;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetFlyBookingTemplateBody - MailService: " + ex);
                return null;
            }
        }
        public async Task<string> TourTemplateBody(SendEmailViewModel model, long id, string order_note = "", string payment_notification = "", bool is_edit_form = false)
        {
            try
            {
                if (model == null)
                {

                    var Tour_list = await _tourRepository.GetDetailTourByID(Convert.ToInt32(id));
                    if (Tour_list == null)
                    {
                        LogHelper.InsertLogTelegram("TourTemplateBody - MailService: NULL DATA with [" + id + "]");
                        return null;
                    }
                    var order = await _orderRepository.GetOrderByID(Convert.ToInt32(Tour_list.OrderId));
                    //string workingDirectory = Directory.GetCurrentDirectory();
                    string workingDirectory = AppDomain.CurrentDomain.BaseDirectory;

                    var tourdetail = await _tourRepository.ListTourPackagesByTourId(id);
                    var template = workingDirectory + @"\EmailTemplate\TourServiceCodeTemplate.html";
                    string body = File.ReadAllText(template);
                    if (order.ContactClientId != null && order.ContactClientId != 0)
                    {
                        var contact = _contactClientRepository.GetByContactClientId((long)order.ContactClientId);
                        body = body.Replace("{{userName}}", "<input type=\"text\" id=\"user_Name\" value=\"" + contact.Name + "\" />");
                        body = body.Replace("{{userPhone}}", "<input type=\"text\" id=\"user_Phone\" value=\"" + contact.Mobile + "\" />");
                        body = body.Replace("{{userEmail}}", "<input type=\"text\" id=\"user_Email\" value=\"" + contact.Email + "\" />");

                    }
                    else
                    {
                        body = body.Replace("{{userName}}", "<input type=\"text\" id=\"user_Name\" value=\"\" />");
                        body = body.Replace("{{userPhone}}", "<input type=\"text\" id=\"user_Phone\" value=\"\" />");
                        body = body.Replace("{{userEmail}}", "<input type=\"text\" id=\"user_Email\" value=\"\" />");
                    }

                    body = body.Replace("{{HotelName}}", "<input type=\"text\" id=\"TileEmail\"style=\"text-align: center;font-weight: bold;\" value=\"PHIẾU XÁC NHẬN DỊCH VỤ: " + Tour_list.TourProductName + "\" />");

                    body = body.Replace("{{orderNo}}", "<input type=\"text\" id=\"orderNo\" value=\"" + order.OrderNo + "\" />");

                    body = body.Replace("{{go_startdate}}", "<input type=\"text\" style=\"min-width: 100px;\" id=\"go_startdate\" value=\"" + ((DateTime)Tour_list.StartDate).ToString("dd/MM/yyyy HH:mm") + "\" />");
                    body = body.Replace("{{go_enddate}}", "<input type=\"text\" id=\"go_enddate\" value=\"" + ((DateTime)Tour_list.EndDate).ToString("dd/MM/yyyy HH:mm") + "\" />");
                    if (Tour_list.TourType == 1)
                    {
                        body = body.Replace("{{go_startpoint}}", "<input type=\"text\" id=\"go_startpoint\" value=\"" + Tour_list.StartPoint1 + "\" />");
                        body = body.Replace("{{go_endpoint}}", "<input type=\"text\" id=\"go_endpoint\" value=\"" + Tour_list.GroupEndPoint1 + "\" />");
                    }
                    if (Tour_list.TourType == 2)
                    {
                        body = body.Replace("{{go_startpoint}}", "<input type=\"text\" id=\"go_startpoint\" value=\"" + Tour_list.StartPoint2 + "\" />");
                        body = body.Replace("{{go_endpoint}}", "<input type=\"text\" id=\"go_endpoint\" value=\"" + Tour_list.GroupEndPoint2 + "\" />");
                    }
                    if (Tour_list.TourType == 3)
                    {
                        body = body.Replace("{{go_startpoint}}", "<input type=\"text\" id=\"go_startpoint\" value=\"" + Tour_list.StartPoint3 + "\" />");
                        body = body.Replace("{{go_endpoint}}", "<input type=\"text\" id=\"go_endpoint\" value=\"" + Tour_list.GroupEndPoint3 + "\" />");
                    }

                    body = body.Replace("{{go_airline}}", "<input type=\"text\" id=\"go_airline\" value=\"" + Tour_list.ORGANIZINGName + "\" />");

                    body = body.Replace("{{totalamount}}", "<input type=\"text\"class=\"currency\" id=\"OrderAmount\" value=\"" + ((double)order.Amount).ToString("N0") + "\" />");
                    if (order.SalerId != null && order.SalerId != 0)
                    {
                        var saler = await _userRepository.GetById((long)order.SalerId);
                        body = body.Replace("{{SalerName}}", "<input type=\"text\" id=\"saler_Name\" value=\"" + saler.FullName + "\" />");
                        body = body.Replace("{{SalerPhone}}", "<input type=\"text\" id=\"saler_Phone\" value=\"" + saler.Phone + "\" />");
                        body = body.Replace("{{SalerEmail}}", "<input type=\"text\" id=\"saler_Email\" value=\"" + saler.Email + "\" />");

                    }
                    else
                    {
                        body = body.Replace("{{SalerName}}", "<input type=\"text\" id=\"saler_Name\" value=\"\" />");
                        body = body.Replace("{{SalerPhone}}", "<input type=\"text\" id=\"saler_Phone\" value=\"\" />");
                        body = body.Replace("{{SalerEmail}}", "<input type=\"text\" id=\"saler_Email\" value=\"\" />");

                    }

                    if (is_edit_form)
                    {
                        body = body.Replace("{{Note}}", "<input type=\"text\" id=\"order_note\" value=\"" + order_note + "\" />");
                        body = body.Replace("{{payment_notification}}", "<input  type=\"text\" id=\"payment_notification\" value=\"" + payment_notification + "\" />");
                        body = body.Split("</head>")[1];
                        body = body.Split("</html>")[0];
                    }
                    else
                    {
                        body = body.Replace("{{Note}}", order_note);
                        body = body.Replace("{{payment_notification}}", payment_notification);
                    }

                    var passenger = "";
                    if (tourdetail != null && tourdetail.Count > 0)
                        foreach (var item in tourdetail)
                        {
                            if (item.PackageCode == "adt_amount") { body = body.Replace("{{go_numberOfAdult}}", "<input style=\"width:30% !important;\" type=\"text\" id=\"go_numberOfAdult\" value=\"" + item.Quantity.ToString() + "\" />"); }
                            else { body = body.Replace("{{go_numberOfAdult}}", "<input style=\"width:30% !important;\" type=\"text\" id=\"go_numberOfAdult\" value=\"0\" />"); }
                            if (item.PackageCode == "chd_amount")
                            {
                                body = body.Replace("{{go_numberOfChild}}", "<input style=\"width:30% !important;\" type=\"text\" id=\"go_numberOfChild\" value=\"" + item.Quantity.ToString() + "\" />");
                            }
                            else { body = body.Replace("{{go_numberOfChild}}", "<input style=\"width:30% !important;\" type=\"text\" id=\"go_numberOfChild\" value=\"0\" />"); }
                            if (item.PackageCode == "inf_amount")
                            {
                                body = body.Replace("{{go_numberOfInfant}}", "<input style=\"width:30% !important;\" type=\"text\" id=\"go_numberOfInfant\" value=\"" + item.Quantity.ToString() + "\" />");
                            }
                            else { body = body.Replace("{{go_numberOfInfant}}", "<input style=\"width:30% !important;\" type=\"text\" id=\"go_numberOfInfant\" value=\"0\" />"); }
                            passenger += "" + item.PackageName + ": Số lượng " + item.Quantity + "&#10 ";

                        }
                    body = body.Replace("{{datatable}}", "<textarea  type=\"text\" id=\"datatable\"style=\"height: 200px !important;\"> " + passenger + "</textarea>");

                    var code = await _hotelBookingCodeRepository.GetListlBookingCodeByHotelBookingId(Tour_list.Id, (int)ServiceType.Tour);
                    string passenger2 = "";
                    if (code != null && code.Count > 0)
                    {
                        foreach (var dv in code)
                        {
                            passenger2 += "Mã : " + dv.BookingCode + ", nội dung: " + dv.Description + " &#10";
                        }
                    }
                    body = body.Replace("{{datatableCode}}", "<textarea type=\"text\" id=\"datatableCode\"style=\"height: 200px !important;\"> " + passenger2 + "</textarea>");
                    body = body.Replace("{{DKHuy}}", "<input type=\"text\" id=\"DKHuy\" value=\"Không hoàn hủy\" />");
                    body = body.Replace("{{NDChuyenKhoan}}", "<input type=\"text\" id=\"NDChuyenKhoan\" value=\"" + order.OrderNo + " CHUYEN KHOAN\" />");
                    body = body.Replace("{{TileEmail}}", "<input type=\"text\" id=\"TileEmail\"style=\"text-align: center;font-weight: bold;\" value=\"PHIẾU XÁC NHẬN ĐƠN HÀNG\" />");
                    string TTChuyenKhoan = "1. VP bank STK: 9698888 &#10 " +
                                        "CTK: Công ty Cổ phần Thương mại và Dịch vụ Quốc tế Đại Việt &#10 " +
                                        "Chi nhánh: Thăng Long &#10 ";
                    body = body.Replace("{{TTChuyenKhoan}}", "<textarea id=\"TTChuyenKhoan\" style=\"height: 425px !important;\">" + TTChuyenKhoan + "</textarea>");
                    return body;
                }
                else
                {
                    var Tour_list = await _tourRepository.GetDetailTourByID(Convert.ToInt32(id));
                    if (Tour_list == null)
                    {
                        LogHelper.InsertLogTelegram("TourTemplateBody - MailService: NULL DATA with [" + id + "]");
                        return null;
                    }
                    var order = await _orderRepository.GetOrderByID(Convert.ToInt32(Tour_list.OrderId));
                    //string workingDirectory = Directory.GetCurrentDirectory();
                    string workingDirectory = AppDomain.CurrentDomain.BaseDirectory;

                    var tourdetail = await _tourRepository.ListTourPackagesByTourId(id);
                    var template = workingDirectory + @"\EmailTemplate\TourServiceCodeTemplate.html";
                    string body = File.ReadAllText(template);

                    string Packagesdata = string.Empty;
                    string Packages = string.Empty;


                    string TTChuyenKhoan = string.Empty;
                    string datatable = string.Empty;
                    string datatableCode = string.Empty;
                    if (model.TTChuyenKhoan != null)
                    {
                        var list = Array.ConvertAll(model.TTChuyenKhoan.Split('\n'), s => (s).ToString());
                        foreach (var item in list)
                        {
                            TTChuyenKhoan += "<div>" + item + "</div>";
                        }
                    }
                    if (model.datatable != null)
                    {
                        var listdatatable = Array.ConvertAll(model.datatable.Split('\n'), s => (s).ToString());

                        foreach (var item in listdatatable)
                        {
                            datatable += "<div>" + item + "</div>";
                        }
                    }

                    if (model.datatableCode != null)
                    {
                        var listdatatableCode = Array.ConvertAll(model.datatableCode.Split('\n'), s => (s).ToString());

                        foreach (var item in listdatatableCode)
                        {
                            datatableCode += "<div>" + item + "</div>";
                        }

                    }

                    body = body.Replace("{{HotelName}}", model.TileEmail);

                    body = body.Replace("{{userName}}", model.user_Name);
                    body = body.Replace("{{userPhone}}", model.user_Phone);
                    body = body.Replace("{{userEmail}}", model.user_Email);
                    body = body.Replace("{{orderNo}}", model.OrderNo);
                    body = body.Replace("{{OrderPackages}}", Packages);
                    body = body.Replace("{{SalerName}}", model.saler_Name);
                    body = body.Replace("{{SalerPhone}}", model.saler_Phone);
                    body = body.Replace("{{SalerEmail}}", model.saler_Email);
                    body = body.Replace("{{NDChuyenKhoan}}", model.NDChuyenKhoan);
                    body = body.Replace("{{TTChuyenKhoan}}", TTChuyenKhoan);
                    body = body.Replace("{{payment_notification}}", model.PaymentNotification);

                    body = body.Replace("{{go_numberOfChild}}", model.go_numberOfChild);
                    body = body.Replace("{{go_numberOfInfant}}", model.go_numberOfInfant);
                    body = body.Replace("{{go_numberOfAdult}}", model.go_numberOfAdult);
                    body = body.Replace("{{go_startpoint}}", model.go_startpoint);
                    body = body.Replace("{{go_endpoint}}", model.go_endpoint);
                    body = body.Replace("{{DKHuy}}", model.DKHuy);
                    body = body.Replace("{{go_airline}}", model.go_airline);
                    body = body.Replace("{{go_startdate}}", model.go_startdate);
                    body = body.Replace("{{go_enddate}}", model.go_enddate);
                    body = body.Replace("{{totalamount}}", model.OrderAmount.ToString("N0"));
                    body = body.Replace("{{datatable}}", datatable);
                    body = body.Replace("{{datatableCode}}", datatableCode);
                    body = body.Replace("{{Note}}", model.OrderNote);

                    return body;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("TourTemplateBody - MailService: " + ex.ToString());
                return null;
            }
        }
        public async Task<string> OrderTemplateBody(SendEmailViewModel model, long id, string payment_notification = "", bool is_edit_form = false)
        {
            try
            {
                if (model == null)
                {
                    var order = await _orderRepository.GetOrderByID(id);
                    var data = await _orderRepository.GetAllServiceByOrderId(id);
                    if (order == null)
                    {
                        LogHelper.InsertLogTelegram("TourTemplateBody - MailService: NULL DATA with [" + id + "]");
                        return null;
                    }
                    if (order != null)
                    {
                        if (data != null)
                            foreach (var item in data)
                            {
                                item.Price += item.Profit;
                                if (item.Type.Equals("Tour"))
                                {
                                    item.tour = await _tourRepository.GetDetailTourByID(Convert.ToInt32(item.ServiceId));

                                    var note = await _hotelBookingRepositories.GetServiceDeclinesByServiceId(item.ServiceId, 5);
                                    if (note != null)
                                        item.Note = note.UserName + " đã từ chối lý do: " + note.Note;
                                }
                                if (item.Type.Equals("Khách sạn"))
                                {
                                    item.Hotel = await _hotelBookingRepositories.GetDetailHotelBookingByID(Convert.ToInt32(item.ServiceId));
                                    var note = await _hotelBookingRepositories.GetServiceDeclinesByServiceId(item.ServiceId, 1);
                                    if (note != null)
                                        item.Note = note.UserName + " đã từ chối lý do: " + note.Note;
                                }
                                if (item.Type.Equals("Vé máy bay"))
                                {
                                    item.Flight = await _flyBookingDetailRepository.GetDetailFlyBookingDetailById(Convert.ToInt32(item.ServiceId));

                                    if (item.Flight.GroupBookingId != null)
                                    {
                                        var note = await _hotelBookingRepositories.GetServiceDeclinesByServiceId(item.Flight.GroupBookingId, 3);
                                        if (note != null)
                                            item.Note = note.UserName + " đã từ chối lý do: " + note.Note;
                                    }

                                }
                                if (item.Type.Equals("Dịch vụ khác"))
                                {
                                    item.OtherBooking = await _otherBookingRepository.GetDetailOtherBookingById(Convert.ToInt32(item.ServiceId));
                                    var note = await _hotelBookingRepositories.GetServiceDeclinesByServiceId(item.ServiceId, (int)ServicesType.Other);
                                    if (note != null)
                                        item.Note = note.UserName + " đã từ chối lý do: " + note.Note;
                                }
                            }
                        if (data != null && data.Count > 1)
                        {
                            for (int o = 0; o < data.Count - 1; o++)
                            {
                                if (data[o].Flight != null && data[o + 1].Flight != null)
                                {
                                    if (data[o].Flight.GroupBookingId == data[o + 1].Flight.GroupBookingId && data[o].Flight.Leg != data[o + 1].Flight.Leg)
                                    {
                                        data[o].Flight.StartDistrict2 = data[o + 1].Flight.StartDistrict;
                                        data[o].Flight.EndDistrict2 = data[o + 1].Flight.EndDistrict;
                                        data[o].Flight.AirlineName_Vi2 = data[o + 1].Flight.AirlineName_Vi;
                                        data[o].Flight.Leg2 = 3;
                                        data[o].Flight.BookingCode2 = data[o + 1].Flight.BookingCode;
                                        data[o].Amount = data[o].Flight.Amount + data[o + 1].Flight.Amount;
                                        data[o].EndDate = data[o + 1].EndDate;


                                        data.Remove(data[o + 1]);

                                    }
                                }

                            }
                        }

                    }
                    string Packagesdata = string.Empty;

                    if (data != null)
                    {
                        foreach (var item in data)
                        {
                            string date = string.Empty;
                            string Packagesdetail = string.Empty;
                            string PackagesOrder = string.Empty;
                            if (item.Type.Equals("Vé máy bay"))
                            {
                                if (item.Flight != null)
                                {
                                    if (item.Flight.Leg2 != 3)
                                    {
                                        if (item.Flight.Leg == 0)
                                        {
                                            date = item.StartDate.ToString("dd/MM/yyyy");
                                        }
                                        if (item.Flight.Leg == 1)
                                        {
                                            date = item.StartDate.ToString("dd/MM/yyyy");
                                        }
                                    }
                                    else
                                    {
                                        date = item.StartDate.ToString("dd/MM/yyyy") + "-" + item.EndDate.ToString("dd/MM/yyyy");
                                    }

                                }
                            }
                            else
                            {
                                date = item.StartDate.ToString("dd/MM/yyyy") + "-" + item.EndDate.ToString("dd/MM/yyyy");
                            }
                            if (item.Type.Equals("Tour"))
                            {
                                string note = string.Empty;
                                if (item.tour != null)
                                {
                                    var Tour_list = await _tourRepository.GetDetailTourByID(Convert.ToInt32(item.ServiceId));
                                    string Point = string.Empty;
                                    if (item.tour.TourType == 1)
                                    {
                                        Point =
                                          "<tr >" +
                                              "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;'>Điểm đi:</td>" +
                                              "<td style='border: 1px solid #999; padding: 5px;'><input class='tourStartPoint1' id='tourStartPoint1' type='text'value=\"" + item.tour.StartPoint1 + "\"></td>" +
                                              "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;'>Điểm đến:</td>" +
                                              "<td style='border: 1px solid #999; padding: 5px;'><input class='tourGroupEndPoint1' id='tourGroupEndPoint1' type='text'value=\"" + item.tour.GroupEndPoint1 + "\"></td>" +
                                          "</tr '>";
                                    }
                                    if (item.tour.TourType == 2)
                                    {
                                        Point =
                                          "<tr >" +
                                              "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;'>Điểm đi:</td>" +
                                              "<td style='border: 1px solid #999; padding: 5px;'><input class='tourStartPoint2' id='tourStartPoint2' type='text'value=\"" + item.tour.StartPoint2 + "\"></td>" +
                                              "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;'>Điểm đến:</td>" +
                                              "<td style='border: 1px solid #999; padding: 5px;'><input class='tourGroupEndPoint2'id='tourGroupEndPoint2' type='text'value=\"" + item.tour.GroupEndPoint2 + "\"></td>" +
                                          "</tr>";
                                    }
                                    if (item.tour.TourType == 3)
                                    {
                                        Point =
                                          "<tr >" +
                                              "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;'>Điểm đi:</td>" +
                                              "<td style='border: 1px solid #999; padding: 5px;'><input class='tourStartPoint3' id='tourStartPoint3' type='text'value=\"" + item.tour.StartPoint3 + " \"></td>" +
                                              "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;'>Điểm đến:</td>" +
                                              "<td style='border: 1px solid #999; padding: 5px;'><input class='tourGroupEndPoint3' id='tourGroupEndPoint3' type='text'value=\"" + item.tour.GroupEndPoint3 + "\"></td>" +
                                          "</tr>";
                                    }

                                    note += "<tr >" +
                                            "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;'>Ngày khởi hành:</td>" +
                                            "<td style='border: 1px solid #999; padding: 5px;' > <input class='tourStartDate' id='tourStartDate'type='text'value=" + ((DateTime)item.tour.StartDate).ToString("dd/MM/yyyy") + " ></td> " +
                                           "<td style= 'border: 1px solid #999; padding: 5px; font-weight: bold;'>Ngày đến:</td>" +
                                           " <td style= 'border: 1px solid #999; padding: 5px;'> <input class='tourEndDate' id='tourEndDate' type='text'value=" + ((DateTime)item.tour.EndDate).ToString("dd/MM/yyyy") + "></td>" +
                                        "</tr>" +

                                        Point +
                                          "<tr >" +
                                            "<td style= 'border: 1px solid #999; padding: 5px; font-weight: bold;' > Loại tour:</td>" +
                                           " <td style= 'border: 1px solid #999; padding: 5px;' ><input class='tourORGANIZINGName' id='tourORGANIZINGName' type='text'value=" + item.tour.ORGANIZINGName + "></td>" +
                                            "<td style= 'border: 1px solid #999; padding: 5px; font-weight: bold;' > Số lượng khách (NL/TE/EB):</td>" +
                                            "<td style= 'border: 1px solid #999; padding: 5px;' ><input class='tourTotalAdult' id='tourTotalAdult' style='width:30%;'type='text' value=" + item.tour.TotalAdult + ">/<input class='tourTotalChildren' id='tourTotalChildren' style='width:30%;'type='text' value=" + item.tour.TotalChildren + ">/<input class='tourTotalBaby' id='tourTotalBaby' style='width:30%;' type='text'value=" + item.tour.TotalBaby + "></td>" +
                                        "</tr>" +
                                        "<tr >" +
                                            "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;'>Tổng tiền tour:</td>" +
                                            "<td colspan= '3' style = 'border: 1px solid #999; padding: 5px;' ><input id='tourAmount' class='currency tourAmount'type='text' value=" + ((double)item.tour.Amount).ToString("N0") + "></td>" +
                                       "</tr>";

                                    Packagesdetail = "<table class='Tour-row' role='presentation' border='0' width='100%' style='border: 0; border-spacing: 0; text-indent: 0; border-collapse: collapse; font-size: 13px; width: 100%;'><tr>" +
                                                    "<td colspan='4' style = 'border: 1px solid #999; padding: 5px; font-weight: bold;text-align: center;' > <input class='TourProductName' id='TourProductName'style = 'font-weight: bold;text-align: center;'  type='text' value=\"Dịch vụ tour : " + Tour_list.TourProductName + "\"> </ td > </tr>" +
                                                    "" + note + "</table>";

                                }
                            }
                            if (item.Type.Equals("Khách sạn"))
                            {
                                if (item.Hotel != null)
                                {
                                    string note = string.Empty;
                                    var hotedetail = await _hotelBookingRepositories.GetHotelBookingById(Convert.ToInt32(item.ServiceId));


                                    note += "<tr>" +
                                        "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;'>Ngày nhận phòng:</td>" +
                                        "<td style='border: 1px solid #999; padding: 5px;' ><input id='hotelArrivalDate'type='text' value=" + item.Hotel[0].ArrivalDate.ToString("dd/MM/yyyy") + " ></td> " +
                                       "<td style= 'border: 1px solid #999; padding: 5px; font-weight: bold;'>Ngày trả phòng:</td>" +
                                       " <td style= 'border: 1px solid #999; padding: 5px;'><input id='hotelDepartureDate' type='text' value=" + item.Hotel[0].DepartureDate.ToString("dd/MM/yyyy") + "></td>" +
                                    "</tr>" +
                                    "<tr>" +
                                        "<td style= 'border: 1px solid #999; padding: 5px; font-weight: bold;' > Số lượng phòng:</td>" +
                                       " <td style= 'border: 1px solid #999; padding: 5px;' ><input id='hotelNumberOfRoom'type='text'value=" + item.Hotel[0].TotalRooms + "></td>" +
                                        "<td rowspan='2' style= 'border: 1px solid #999; padding: 5px; font-weight: bold;' > Số lượng khách (NL/TE/EB):</td>" +
                                        "<td rowspan='2' style= 'border: 1px solid #999; padding: 5px;' ><input id='hotelNumberOfAdult'type='text'style='width:30%;' value=" + item.Hotel[0].NumberOfAdult + ">/<input id='hotelNumberOfChild' type='text'style='width:30%;' value=" + item.Hotel[0].NumberOfChild + ">/<input id='hotelNumberOfInfant' type='text'style='width:30%;' value=" + item.Hotel[0].NumberOfInfant + "></td>" +
                                    "</tr>" +
                                    "<tr>" +
                                        "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;'>Số đêm</td>" +
                                        "<td style='border: 1px solid #999; padding: 5px;'><input id='hotelTotalDays'type='text'value=" + item.Hotel[0].TotalDays + "></td>" +
                                    "</tr>" +

                                    "<tr>" +
                                        "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;'>Tổng tiền phòng:</td>" +
                                        "<td colspan= '3' style = 'border: 1px solid #999; padding: 5px;' ><input id='hotelAmount' class='currency'type='text' value=" + item.Hotel[0].TotalAmount.ToString("N0") + "></td>" +
                                   "</tr>";



                                    Packagesdetail = "<table class='Hotel-row' role='presentation' border='0' width='100%' style='border: 0; border-spacing: 0; text-indent: 0; border-collapse: collapse; font-size: 13px; width: 100%;'><tr>" +
                                        "<td colspan='4' style = 'border: 1px solid #999; padding: 5px; font-weight: bold;text-align: center;' > <input id='HotelName'style = 'font-weight: bold;text-align: center;' type='text'value=\"Dịch vụ khách sạn : " + hotedetail[0].HotelName + "\"></ td ></tr> " +
                                                    "" + note + "</table>";

                                }
                            }
                            if (item.Type.Equals("Vé máy bay"))
                            {
                                string note = string.Empty;
                                if (item.Flight != null)
                                {
                                    if (item.Flight.Leg2 == 3)
                                    {
                                        note += "<tr>" +
                                           "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;'>Ngày khởi hành:</td>" +
                                          "<td style='border: 1px solid #999; padding: 5px;' ><input id='fyStartDate'type='text'value=" + item.Flight.StartDate.ToString("dd/MM/yyyy") + "></td>" +
                                          "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;'>Ngày hạ cánh</td>" +
                                          "<td style='border: 1px solid #999; padding: 5px;' ><input id='fyEndDate'type='text'value=" + item.Flight.EndDate.ToString("dd/MM/yyyy") + "></td>" +
                                     "</tr>" +
                                     "<tr>" +
                                         "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;' >Điểm đi:</td>" +
                                         "<td style='border: 1px solid #999; padding: 5px;' ><input id='fyStartDistrict' type='text'value=\"" + item.Flight.StartDistrict + "\"></td>" +
                                         "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;' >Điểm đến:</td>" +
                                         "<td style='border: 1px solid #999; padding: 5px;' ><input id='fyEndDistrict' type='text'value=\"" + item.Flight.EndDistrict + "\"></td>" +
                                      "</tr>" +
                                    "<tr>" +
                                          "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;' > Hãng bay:</td>" +
                                          "<td style='border: 1px solid #999; padding: 5px;' ><input id='fyAirlineName_Vi'type='text'value=\"" + item.Flight.AirlineName_Vi + "\"></td>" +
                                           "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;' >Mã giữ chỗ :</td> " +
                                          "<td style='border: 1px solid #999; padding: 5px;' ><input id='fyBookingCode' type='text'value=\"" + item.Flight.BookingCode + "\"></td>" +

                                       "</tr>" +
                                     "<tr>" +
                                          "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;' > Điểm đi:</td>" +
                                          "<td style='border: 1px solid #999; padding: 5px;' ><input id='fyStartDistrict2'type='text'value=\"" + item.Flight.StartDistrict2 + "\"></td>" +
                                          "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;' > Điểm đến:</td>" +
                                          "<td style='border: 1px solid #999; padding: 5px;' ><input id='fyEndDistrict2' type='text'value=\"" + item.Flight.EndDistrict2 + "\"></td>" +
                                       "</tr>" +
                                         "<tr>" +
                                          "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;' > Hãng bay:</td>" +
                                          "<td style='border: 1px solid #999; padding: 5px;' ><input id='fyAirlineName_Vi2'type='text'value=\"" + item.Flight.AirlineName_Vi2 + "\"></td>" +
                                           "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;' >Mã giữ chỗ :</td> " +
                                          "<td style='border: 1px solid #999; padding: 5px;' ><input id='fyBookingCode2' type='text'value=\"" + item.Flight.BookingCode2 + "\"></td>" +

                                       "</tr>" +
                                      "<tr>" +
                                         "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;' >Hành trình:</td>" +
                                         "<td style='border: 1px solid #999; padding: 5px;' >Khứ hồi</td>" +
                                        "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;' > Số lượng khách (NL/TE/EB):</td> " +
                                           "<td style='border: 1px solid #999; padding: 5px;' ><input id='fyAdultNumber' type='text'style='width:30%;'value=" + item.Flight.AdultNumber + ">/<input id='fyChildNumber'type='text'style='width:30%;'value=" + item.Flight.ChildNumber + ">/<input id='fyInfantNumber' type='text'style='width:30%;' value=" + item.Flight.InfantNumber + "></td>" +
                                      "</tr>" +
                                       "<tr>" +
                                            "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;'>Tổng tiền vé:</td>" +
                                            "<td colspan= '3' style = 'border: 1px solid #999; padding: 5px;' ><input class='currency' id='fyAmount'type='text'value=" + item.Amount.ToString("N0") + "></td>" +
                                       "</tr>";

                                    }
                                    else
                                    {

                                        if (item.Flight.Leg == 0)
                                        {
                                            note += "<tr>" +
                                          "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;'>Ngày khởi hành:</td>" +
                                          "<td style='border: 1px solid #999; padding: 5px;' ><input id='fyStartDate'type='text'value=" + item.Flight.StartDate.ToString("dd/MM/yyyy") + "></td>" +
                                          "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;'>Ngày hạ cánh</td>" +
                                          "<td style='border: 1px solid #999; padding: 5px;' ><input id='fyEndDate'type='text'value=" + item.Flight.EndDate.ToString("dd/MM/yyyy") + "></td>" +
                                     "</tr>" +
                                      "<tr>" +
                                          "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;' > Điểm đi:</td>" +
                                          "<td style='border: 1px solid #999; padding: 5px;' ><input id='fyStartDistrict'type='text'value=\"" + item.Flight.StartDistrict + "\"></td>" +
                                          "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;' > Điểm đến:</td>" +
                                          "<td style='border: 1px solid #999; padding: 5px;' ><input id='fyEndDistrict' type='text'value=\"" + item.Flight.EndDistrict + "\"></td>" +
                                       "</tr>" +
                                      "<tr>" +
                                          "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;' > Hãng bay:</td>" +
                                          "<td style='border: 1px solid #999; padding: 5px;' ><input id='fyAirlineName_Vi'type='text'value=\"" + item.Flight.AirlineName_Vi + "\"></td>" +
                                           "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;' >Mã giữ chỗ :</td> " +
                                          "<td style='border: 1px solid #999; padding: 5px;' ><input id='fyBookingCode' type='text'value=\"" + item.Flight.BookingCode + "\"></td>" +

                                       "</tr>" +
                                      "<tr>" +
                                         "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;' >Hành trình:</td>" +
                                         "<td style='border: 1px solid #999; padding: 5px;' >1 chiều</td>" +
                                        "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;' > Số lượng khách (NL/TE/EB):</td> " +
                                           "<td style='border: 1px solid #999; padding: 5px;' ><input id='fyAdultNumber' type='text'style='width:30%;'value=" + item.Flight.AdultNumber + ">/<input id='fyChildNumber'type='text'style='width:30%;'value=" + item.Flight.ChildNumber + ">/<input id='fyInfantNumber' type='text'style='width:30%;' value=" + item.Flight.InfantNumber + "></td>" +
                                      "</tr>" +
                                       "<tr>" +
                                            "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;'>Tổng tiền vé:</td>" +
                                            "<td colspan= '3' style = 'border: 1px solid #999; padding: 5px;' ><input id='fyAmount'type='text'value=" + item.Amount.ToString("N0") + "></td>" +
                                       "</tr>";
                                            /*note += " Chiều đi:" + item.Flight.StartDistrict + "-" + item.Flight.EndDistrict + " - Mã đặt chỗ:" + item.Flight.BookingCode + "&#10" +

                                                            "Chiều về:" + item.Flight.StartDistrict2 + "-" + item.Flight.EndDistrict2 + "-  Mã đặt chỗ: " + item.Flight.BookingCode2 + "&#10";*/
                                        }
                                        if (item.Flight.Leg == 1)
                                        {
                                            note += "<tr>" +
                                          "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;'>Ngày khởi hành:</td>" +
                                          "<td style='border: 1px solid #999; padding: 5px;' ><input id='fyStartDate'type='text'value=" + item.Flight.StartDate.ToString("dd/MM/yyyy") + "></td>" +
                                          "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;'>Ngày hạ cánh</td>" +
                                          "<td style='border: 1px solid #999; padding: 5px;' ><input id='fyEndDate'type='text'value=" + item.Flight.EndDate.ToString("dd/MM/yyyy") + "></td>" +
                                     "</tr>" +
                                      "<tr>" +
                                          "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;' > Điểm đi:</td>" +
                                          "<td style='border: 1px solid #999; padding: 5px;' ><input id='fyStartDistrict2'type='text'value=\"" + item.Flight.StartDistrict2 + "\"></td>" +
                                          "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;' > Điểm đến:</td>" +
                                          "<td style='border: 1px solid #999; padding: 5px;' ><input id='fyEndDistrict2' type='text'value=\"" + item.Flight.EndDistrict2 + "\"></td>" +
                                       "</tr>" +
                                      "<tr>" +
                                          "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;' > Hãng bay:</td>" +
                                          "<td style='border: 1px solid #999; padding: 5px;' ><input id='fyAirlineName_Vi2'type='text'value=\"" + item.Flight.AirlineName_Vi2 + "\"></td>" +
                                           "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;' >Mã giữ chỗ :</td> " +
                                          "<td style='border: 1px solid #999; padding: 5px;' ><input id='fyBookingCode2' type='text'value=\"" + item.Flight.BookingCode2 + "\"></td>" +

                                       "</tr>" +
                                      "<tr>" +
                                         "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;' >Hành trình:</td>" +
                                         "<td style='border: 1px solid #999; padding: 5px;' >1 chiều</td>" +
                                        "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;' > Số lượng khách (NL/TE/EB):</td> " +
                                           "<td style='border: 1px solid #999; padding: 5px;' ><input id='fyAdultNumber' type='text'style='width:30%;'value=" + item.Flight.AdultNumber + ">/<input id='fyChildNumber'type='text'style='width:30%;'value=" + item.Flight.ChildNumber + ">/<input id='fyInfantNumber' type='text'style='width:30%;' value=" + item.Flight.InfantNumber + "></td>" +
                                      "</tr>" +
                                       "<tr>" +
                                            "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;'>Tổng tiền vé:</td>" +
                                            "<td colspan= '3' style = 'border: 1px solid #999; padding: 5px;' ><input type='text' id='fyAmount'value=" + item.Amount.ToString("N0") + "></td>" +
                                       "</tr>";

                                        }


                                    }





                                    Packagesdetail = "<table class='Fy-row' role='presentation' border='0' width='100%' style='border: 0; border-spacing: 0; text-indent: 0; border-collapse: collapse; font-size: 13px; width: 100%;'><tr>" +
                                        "<td colspan='4' style = 'border: 1px solid #999; padding: 5px; font-weight: bold;text-align: center;' > Dịch vụ " + item.Type + "</td ></tr>" +
                                                    "" + note + "</table>";

                                }



                            }
                            if (item.Type.Equals("Dịch vụ khác"))
                            {
                                if (item.OtherBooking != null)
                                {
                                    string note = string.Empty;

                                    note += "<tr>" +
                                        "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;'>Ngày bắt đầu:</td>" +
                                        "<td style='border: 1px solid #999; padding: 5px;' ><input id='OtherStartDate'type='text' value=" + item.OtherBooking[0].StartDate.ToString("dd/MM/yyyy") + " ></td> " +
                                       "<td style= 'border: 1px solid #999; padding: 5px; font-weight: bold;'>Ngày kết thúc:</td>" +
                                       " <td style= 'border: 1px solid #999; padding: 5px;'><input id='OtherEndDate' type='text' value=" + item.OtherBooking[0].EndDate.ToString("dd/MM/yyyy") + "></td>" +
                                    "</tr>" +
                                    "<tr>" +
                                        "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;'>Tổng tiền dịch vụ:</td>" +
                                        "<td colspan= '3' style = 'border: 1px solid #999; padding: 5px;' ><input id='OtherAmount' class='currency'type='text' value=" + item.OtherBooking[0].Amount.ToString("N0") + "></td>" +
                                   "</tr>";



                                    Packagesdetail = "<table class='Other-row' role='presentation' border='0' width='100%' style='border: 0; border-spacing: 0; text-indent: 0; border-collapse: collapse; font-size: 13px; width: 100%;'><tr>" +
                                        "<td colspan='4' style = 'border: 1px solid #999; padding: 5px; font-weight: bold;text-align: center;' > <input id='OtherServiceName' style=\"font-weight: bold; text-align: center; \"type='text'value=\"Dịch vụ : " + item.OtherBooking[0].ServiceName + "\"</ td ></tr> " +
                                                    "" + note + "</table>";

                                }
                            }
                            Packagesdata += Packagesdetail;
                        }
                    }
                    string Packages = string.Empty;

                    Packages = Packagesdata;

                    string workingDirectory = AppDomain.CurrentDomain.BaseDirectory;


                    var template = workingDirectory + @"\EmailTemplate\OrderTemplate.html";
                    string body = File.ReadAllText(template);

                    if (order.ClientId != null && order.ClientId != 0)
                    {
                        var contact = await _clientRepository.GetClientDetailByClientId((long)order.ClientId);
                        body = body.Replace("{{userName}}", "<input type=\"text\" id=\"user_Name\" value=\"" + contact.ClientName + "\" />");
                        body = body.Replace("{{userPhone}}", "<input type=\"text\" id=\"user_Phone\" value=\"" + contact.Phone + "\" />");
                        body = body.Replace("{{userEmail}}", "<input type=\"text\" id=\"user_Email\" value=\"" + contact.Email + "\" />");
                    }
                    else
                    {

                        body = body.Replace("{{userName}}", "<input type=\"text\" id=\"user_Name\" value='' />");
                        body = body.Replace("{{userPhone}}", "<input type=\"text\" id=\"user_Phone\" value='' />");
                        body = body.Replace("{{userEmail}}", "<input type=\"text\" id=\"user_Email\" value='' />");
                    }
                    body = body.Replace("{{orderNo}}", "<input type=\"text\" id=\"orderNo\" value=\"" + order.OrderNo + "\" />");
                    body = body.Replace("{{OrderAmount}}", "<input type=\"text\" class=\"currency \" id=\"OrderAmount\" value=\"" + ((double)order.Amount).ToString("N0") + "\" />");
                    if (order.SalerId != null && order.SalerId != 0)
                    {
                        var saler = await _userRepository.GetById((long)order.SalerId);
                        body = body.Replace("{{SalerName}}", "<input type=\"text\" id=\"saler_Name\" value=\"" + saler.FullName + "\" />");
                        body = body.Replace("{{SalerPhone}}", "<input type=\"text\" id=\"saler_Phone\" value=\"" + saler.Phone + "\" />");
                        body = body.Replace("{{SalerEmail}}", "<input type=\"text\" id=\"saler_Email\" value=\"" + saler.Email + "\" />");

                    }
                    else
                    {
                        body = body.Replace("{{SalerName}}", "<input type=\"text\" id=\"saler_Name\" value='' />");
                        body = body.Replace("{{SalerPhone}}", "<input type=\"text\" id=\"saler_Phone\" value='' />");
                        body = body.Replace("{{SalerEmail}}", "<input type=\"text\" id=\"saler_Email\" value='' />");

                    }
                    body = body.Replace("{{OrderPackages}}", Packagesdata);

                    body = body.Replace("{{payment_notification}}", "<textarea type=\"text\"style='min-height: 120px;' id=\"payment_notification\" value=\"\"></textarea>");


                    body = body.Replace("{{TileEmail}}", "<input type=\"text\" id=\"TileEmail\" style=\"text-align: center;font-weight: bold;\"value=\"PHIẾU XÁC NHẬN ĐƠN HÀNG\" />");
                    body = body.Replace("{{NDChuyenKhoan}}", "<input type=\"text\" id=\"NDChuyenKhoan\" value=\"" + order.OrderNo + " CHUYEN KHOAN\" />");

                    var data_VietQRBankList = await _APIService.GetVietQRBankList();
                    var selected_bank = data_VietQRBankList.Count > 0 ? data_VietQRBankList.FirstOrDefault(x => x.shortName.Trim().ToLower().Contains("Techcombank".Trim().ToLower())) : null;
                    string bank_code = "Techcombank";
                    if (selected_bank != null) bank_code = selected_bank.bin;
                    var result = await _APIService.GetVietQRCode("19131835226016", bank_code, order.OrderNo, Convert.ToDouble(order.Amount));
                    var jsonData = JObject.Parse(result);
                    var status = int.Parse(jsonData["code"].ToString());
                    if (status == (int)ResponseType.SUCCESS)
                    {
                        body = body.Replace("{{LinkQRTCB}}", jsonData["data"]["qrDataURL"].ToString());
                    }

                    var selected_bank2 = data_VietQRBankList.Count > 0 ? data_VietQRBankList.FirstOrDefault(x => x.shortName.Trim().ToLower().Contains("HDBANK".Trim().ToLower())) : null;
                    string bank_code2 = "HDBANK";
                    if (selected_bank2 != null) bank_code2 = selected_bank2.bin;
                    var result2 = await _APIService.GetVietQRCode("371704070000023", bank_code2, order.OrderNo, Convert.ToDouble(order.Amount));
                    var jsonData2 = JObject.Parse(result2);
                    var status2 = int.Parse(jsonData2["code"].ToString());
                    if (status2 == (int)ResponseType.SUCCESS)
                    {
                        body = body.Replace("{{LinkQRHDB}}", jsonData2["data"]["qrDataURL"].ToString());
                    }

                    var selected_bank3 = data_VietQRBankList.Count > 0 ? data_VietQRBankList.FirstOrDefault(x => x.shortName.Trim().ToLower().Contains("VietinBank".Trim().ToLower())) : null;
                    string bank_code3 = "VietinBank";
                    if (selected_bank3 != null) bank_code3 = selected_bank3.bin;
                    var result3 = await _APIService.GetVietQRCode("113600558866", bank_code3, order.OrderNo, Convert.ToDouble(order.Amount));
                    var jsonData3 = JObject.Parse(result3);
                    var status3 = int.Parse(jsonData3["code"].ToString());
                    if (status3 == (int)ResponseType.SUCCESS)
                    {
                        body = body.Replace("{{LinkQRVTB}}", jsonData3["data"]["qrDataURL"].ToString());
                    }


                    //string TTChuyenKhoan = "<strong>1. Techcombank – STK: 19131835226016 (Doanh thu phòng khách sạn - Có xuất hoá đơn) </strong>" +
                    //               "<p>CTK: Công ty Cổ phần Thương mại và Dịch vụ Quốc tế Đại Việt </p>" +
                    //               "<p>Chi nhánh: Đông Đô </p> " +
                    //               "<strong>2. HDBANK – STK: 371704070000023 (Doanh thu vé máy bay - Có xuất hoá đơn) </strong>" +
                    //               "<p>CTK: Công ty Cổ phần Thương mại và Dịch vụ Quốc tế Đại Việt </p>" +
                    //               "<p>Chi nhánh: Hà Nội</p> " +
                    //                "<strong>3. Vietin Bank - STK: 113600558866 (Doanh thu Tour - Có xuất hoá đơn) </strong>" +
                    //               "<p>CTK: Công ty Cổ phần Thương mại và Dịch vụ Quốc tế Đại Việt </p>" +
                    //               "<p>Chi nhánh: Tràng An</p> ";
                    //body = body.Replace("{{TTChuyenKhoan}}", "<textarea id='TTChuyenKhoan'>" + TTChuyenKhoan + "</textarea>");
                    body = body.Replace("{{TTNote}}", "<textarea id='TTNote'></textarea>");
                    return body;
                }
                else
                {
                    string workingDirectory = AppDomain.CurrentDomain.BaseDirectory;


                    var template = workingDirectory + @"\EmailTemplate\OrderTemplate.html";
                    string body = File.ReadAllText(template);
                    string Packagesdata = string.Empty;
                    string Packages = string.Empty;
                    var order = await _orderRepository.GetOrderByID(id);

                    if (order == null)
                    {
                        LogHelper.InsertLogTelegram("TourTemplateBody - MailService: NULL DATA with [" + id + "]");
                        return null;
                    }



                    string date = string.Empty;
                    string Packagesdetail = string.Empty;
                    string PackagesOrder = string.Empty;
                    if (model.TourEmail != null && model.TourEmail.Count > 0)
                    {
                        foreach (var item in model.TourEmail)
                        {

                            string note = string.Empty;




                            string Point = string.Empty;

                            Point =
                              "<tr>" +
                                  "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;'>Điểm đi:</td>" +
                                  "<td style='border: 1px solid #999; padding: 5px;'>" + item.tourStartPoint1 + item.tourStartPoint2 + item.tourStartPoint3 + "</td>" +
                                  "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;'>Điểm đến:</td>" +
                                  "<td style='border: 1px solid #999; padding: 5px;'>" + item.tourGroupEndPoint1 + item.tourGroupEndPoint2 + item.tourGroupEndPoint3 + "</td>" +
                              "</tr>";



                            note += "<tr>" +
                                    "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;'>Ngày khởi hành:</td>" +
                                    "<td style='border: 1px solid #999; padding: 5px;' > " + item.tourStartDate + " </td> " +
                                   "<td style= 'border: 1px solid #999; padding: 5px; font-weight: bold;'>Ngày đến:</td>" +
                                   " <td style= 'border: 1px solid #999; padding: 5px;'> " + item.tourEndDate + "</td>" +
                                "</tr>" +

                                Point +
                                  "<tr>" +
                                    "<td style= 'border: 1px solid #999; padding: 5px; font-weight: bold;' > Loại tour:</td>" +
                                   " <td style= 'border: 1px solid #999; padding: 5px;' >" + item.tourORGANIZINGName + "</td>" +
                                    "<td style= 'border: 1px solid #999; padding: 5px; font-weight: bold;' > Số lượng khách (NL/TE/EB):</td>" +
                                    "<td style= 'border: 1px solid #999; padding: 5px;' >" + item.tourTotalAdult + "/" + item.tourTotalChildren + "/" + item.tourTotalBaby + "</td>" +
                                "</tr>" +
                                "<tr>" +
                                    "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;'>Tổng tiền tour:</td>" +
                                    "<td colspan= '3' style = 'border: 1px solid #999; padding: 5px;' >" + item.tourAmount + "</td>" +
                               "</tr>";

                            Packagesdetail = "<tr><td colspan='4' style = 'border: 1px solid #999; padding: 5px; font-weight: bold;text-align: center;' > " + item.TourProductName + "</ td ></tr> " +
                                            "" + note + "";


                            Packagesdata += "<table role='presentation' border='0' width='100%' style='border: 0; border-spacing: 0; text-indent: 0; border-collapse: collapse; font-size: 13px; width: 100%;'>" +
                                Packagesdetail + "</table>";
                        }
                    }
                    if (model.HotelEmail != null && model.HotelEmail.Count > 0)
                    {
                        foreach (var item in model.HotelEmail)
                        {
                            string note = string.Empty;
                            note += "<tr>" +
                                "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;'>Ngày nhận phòng:</td>" +
                                "<td style='border: 1px solid #999; padding: 5px;' >" + item.hotelArrivalDate + " </td> " +
                               "<td style= 'border: 1px solid #999; padding: 5px; font-weight: bold;'>Ngày trả phòng:</td>" +
                               " <td style= 'border: 1px solid #999; padding: 5px;'>" + item.hotelDepartureDate + "</td>" +
                            "</tr>" +
                            "<tr>" +
                                "<td style= 'border: 1px solid #999; padding: 5px; font-weight: bold;' > Số lượng phòng:</td>" +
                               " <td style= 'border: 1px solid #999; padding: 5px;' >" + item.hotelNumberOfRoom + "</td>" +
                                "<td rowspan='2' style= 'border: 1px solid #999; padding: 5px; font-weight: bold;' > Số lượng khách (NL/TE/EB):</td>" +
                                "<td rowspan='2' style= 'border: 1px solid #999; padding: 5px;' >" + item.hotelNumberOfAdult + "/" + item.hotelNumberOfChild + "/" + item.hotelNumberOfInfant + "</td>" +
                            "</tr>" +
                            "<tr>" +
                                "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;'>Số đêm</td>" +
                                "<td style='border: 1px solid #999; padding: 5px;'>" + item.hotelTotalDays + "</td>" +
                            "</tr>" +

                            "<tr>" +
                                "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;'>Tổng tiền phòng:</td>" +
                                "<td colspan= '3' style = 'border: 1px solid #999; padding: 5px;' >" + item.hotelAmount + "</td>" +
                           "</tr>";



                            Packagesdetail = "<td colspan='4' style = 'border: 1px solid #999; padding: 5px; font-weight: bold;text-align: center;' >" + item.HotelName + "</ td > " +
                                            "" + note + "";
                            Packagesdata += "<table role='presentation' border='0' width='100%' style='border: 0; border-spacing: 0; text-indent: 0; border-collapse: collapse; font-size: 13px; width: 100%;'>" +
                               Packagesdetail + "</table>";
                        }
                    }

                    if (model.FyEmail != null && model.FyEmail.Count > 0)
                    {
                        foreach (var item in model.FyEmail)
                        {
                            string note = string.Empty;

                            if (item.fyStartDistrict2 != null && item.fyEndDistrict2 != null)
                            {
                                note += "<tr>" +
                                   "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;'>Ngày khởi hành:</td>" +
                                  "<td style='border: 1px solid #999; padding: 5px;' >" + item.fyStartDate + "</td>" +
                                  "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;'>Ngày hạ cánh</td>" +
                                  "<td style='border: 1px solid #999; padding: 5px;' >" + item.fyEndDate + "</td>" +
                             "</tr>" +
                             "<tr>" +
                                 "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;' >Điểm đi:</td>" +
                                 "<td style='border: 1px solid #999; padding: 5px;' >" + item.fyStartDistrict + "</td>" +
                                 "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;' >Điểm đến:</td>" +
                                 "<td style='border: 1px solid #999; padding: 5px;' >" + item.fyEndDistrict + "</td>" +
                              "</tr>" +
                            "<tr>" +
                                  "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;' > Hãng bay:</td>" +
                                  "<td style='border: 1px solid #999; padding: 5px;' >" + item.fyAirlineName_Vi + "</td>" +
                                   "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;' >Mã giữ chỗ :</td> " +
                                  "<td style='border: 1px solid #999; padding: 5px;' >" + item.fyBookingCode + "</td>" +

                               "</tr>" +
                             "<tr>" +
                                  "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;' > Điểm đi:</td>" +
                                  "<td style='border: 1px solid #999; padding: 5px;' >" + item.fyStartDistrict2 + "</td>" +
                                  "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;' > Điểm đến:</td>" +
                                  "<td style='border: 1px solid #999; padding: 5px;' >" + item.fyEndDistrict2 + "</td>" +
                               "</tr>" +
                               "<tr>" +
                                  "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;' > Hãng bay:</td>" +
                                  "<td style='border: 1px solid #999; padding: 5px;' >" + item.fyAirlineName_Vi2 + "</td>" +
                                   "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;' >Mã giữ chỗ :</td> " +
                                  "<td style='border: 1px solid #999; padding: 5px;' >" + item.fyBookingCode2 + "</td>" +

                               "</tr>" +
                              "<tr>" +
                                 "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;' >Hành trình:</td>" +
                                 "<td style='border: 1px solid #999; padding: 5px;' >khứ hồi</td>" +
                                "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;' > Số lượng khách (NL/TE/EB):</td> " +
                                   "<td style='border: 1px solid #999; padding: 5px;' >" + item.fyAdultNumber + "/" + item.fyChildNumber + "/" + item.fyInfantNumber + "</td>" +
                              "</tr>" +
                               "<tr>" +
                                    "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;'>Tổng tiền vé:</td>" +
                                    "<td colspan= '3' style = 'border: 1px solid #999; padding: 5px;' >" + item.fyAmount + "</td>" +
                               "</tr>";

                            }
                            else
                            {


                                note += "<tr>" +
                              "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;'>Ngày khởi hành:</td>" +
                              "<td style='border: 1px solid #999; padding: 5px;' >" + item.fyStartDate + "</td>" +
                              "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;'>Ngày hạ cánh</td>" +
                              "<td style='border: 1px solid #999; padding: 5px;' >" + item.fyEndDate + "</td>" +
                         "</tr>" +
                          "<tr>" +
                              "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;' > Điểm đi:</td>" +
                              "<td style='border: 1px solid #999; padding: 5px;' >" + item.fyStartDistrict + "</td>" +
                              "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;' > Điểm đến:</td>" +
                              "<td style='border: 1px solid #999; padding: 5px;' >" + item.fyEndDistrict + "</td>" +
                           "</tr>" +
                          "<tr>" +
                              "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;' > Hãng bay:</td>" +
                              "<td style='border: 1px solid #999; padding: 5px;' >" + item.fyAirlineName_Vi + "</td>" +
                               "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;' >Mã giữ chỗ :</td> " +
                              "<td style='border: 1px solid #999; padding: 5px;' >" + item.fyBookingCode + "</td>" +

                           "</tr>" +
                          "<tr>" +
                             "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;' >Hành trình:</td>" +
                             "<td style='border: 1px solid #999; padding: 5px;' >1 chiều</td>" +
                            "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;' > Số lượng khách (NL/TE/EB):</td> " +
                               "<td style='border: 1px solid #999; padding: 5px;' >" + item.fyAdultNumber + "/" + item.fyChildNumber + "/" + item.fyInfantNumber + "</td>" +
                          "</tr>" +
                           "<tr>" +
                                "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;'>Tổng tiền vé:</td>" +
                                "<td colspan= '3' style = 'border: 1px solid #999; padding: 5px;' >" + item.fyAmount + "</td>" +
                           "</tr>";



                            }

                            Packagesdetail = "<td colspan='4' style = 'border: 1px solid #999; padding: 5px; font-weight: bold;text-align: center;' > Dịch vụ vé máy bay</td >" +
                                            "" + note + "";
                            Packagesdata += "<table role='presentation' border='0' width='100%' style='border: 0; border-spacing: 0; text-indent: 0; border-collapse: collapse; font-size: 13px; width: 100%;'>" +
                               Packagesdetail + "</table>";
                        }

                    }
                    if (model.OtherEmail != null && model.OtherEmail.Count > 0)
                    {

                        foreach (var item in model.OtherEmail)
                        {
                            string note = string.Empty;



                            note += "<tr>" +
                                "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;'>Ngày bắt đầu:</td>" +
                                "<td style='border: 1px solid #999; padding: 5px;' >" + item.OtherStartDate + " </td> " +
                               "<td style= 'border: 1px solid #999; padding: 5px; font-weight: bold;'>Ngày ngày kết thúc:</td>" +
                               " <td style= 'border: 1px solid #999; padding: 5px;'>" + item.OtherEndDate + "</td>" +
                            "</tr>" +
                            "<tr>" +
                                "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;'>Tổng tiền dịch vụ:</td>" +
                                "<td colspan= '3' style = 'border: 1px solid #999; padding: 5px;' >" + item.OtherAmount + "</td>" +
                           "</tr>";



                            Packagesdetail = "<td colspan='4' style = 'border: 1px solid #999; padding: 5px; font-weight: bold;text-align: center;' > " + item.OtherServiceName + "</ td > " +
                                            "" + note + "";
                            Packagesdata += "<table role='presentation' border='0' width='100%' style='border: 0; border-spacing: 0; text-indent: 0; border-collapse: collapse; font-size: 13px; width: 100%;'>" +
                                Packagesdetail + "</table>";
                        }



                    }
                    var data_VietQRBankList = await _APIService.GetVietQRBankList();
                    var selected_bank = data_VietQRBankList.Count > 0 ? data_VietQRBankList.FirstOrDefault(x => x.shortName.Trim().ToLower().Contains("Techcombank".Trim().ToLower())) : null;
                    string bank_code = "Techcombank";
                    if (selected_bank != null) bank_code = selected_bank.bin;
                    var result = await _APIService.GetVietQRCode("19131835226016", bank_code, order.OrderNo, Convert.ToDouble(order.Amount));
                    var jsonData = JObject.Parse(result);
                    var status = int.Parse(jsonData["code"].ToString());
                    if (status == (int)ResponseType.SUCCESS)
                    {
                        var url_path = await _APIService.UploadImageQRBase64(order.OrderNo, Convert.ToDouble(order.Amount).ToString(), jsonData["data"]["qrDataURL"].ToString(), "19131835226016");
                    
                        body = body.Replace("{{LinkQRTCB}}", ReadFile.LoadConfig().IMAGE_DOMAIN + url_path);
                    }

                    var selected_bank2 = data_VietQRBankList.Count > 0 ? data_VietQRBankList.FirstOrDefault(x => x.shortName.Trim().ToLower().Contains("HDBANK".Trim().ToLower())) : null;
                    string bank_code2 = "HDBANK";
                    if (selected_bank2 != null) bank_code2 = selected_bank2.bin;
                    var result2 = await _APIService.GetVietQRCode("371704070000023", bank_code2, order.OrderNo, Convert.ToDouble(order.Amount));
                    var jsonData2 = JObject.Parse(result2);
                    var status2 = int.Parse(jsonData2["code"].ToString());
                    if (status2 == (int)ResponseType.SUCCESS)
                    {
                        var url_path2 = await _APIService.UploadImageQRBase64(order.OrderNo, Convert.ToDouble(order.Amount).ToString(), jsonData2["data"]["qrDataURL"].ToString(), "371704070000023");
                        body = body.Replace("{{LinkQRHDB}}", ReadFile.LoadConfig().IMAGE_DOMAIN + url_path2);
                    }

                    var selected_bank3 = data_VietQRBankList.Count > 0 ? data_VietQRBankList.FirstOrDefault(x => x.shortName.Trim().ToLower().Contains("VietinBank".Trim().ToLower())) : null;
                    string bank_code3 = "VietinBank";
                    if (selected_bank3 != null) bank_code3 = selected_bank3.bin;
                    var result3 = await _APIService.GetVietQRCode("113600558866", bank_code3, order.OrderNo, Convert.ToDouble(order.Amount));
                    var jsonData3 = JObject.Parse(result3);
                    var status3 = int.Parse(jsonData3["code"].ToString());
                    if (status3 == (int)ResponseType.SUCCESS)
                    {
                        var url_path3 = await _APIService.UploadImageQRBase64(order.OrderNo, Convert.ToDouble(order.Amount).ToString(), jsonData3["data"]["qrDataURL"].ToString(), "113600558866");
                   
                        body = body.Replace("{{LinkQRVTB}}", ReadFile.LoadConfig().IMAGE_DOMAIN + url_path3);
                    }
                    //string TTChuyenKhoan = string.Empty;
                    //if (model.TTChuyenKhoan != null)
                    //{
                    //    var list = Array.ConvertAll(model.TTChuyenKhoan.Split('\n'), s => (s).ToString());
                    //    foreach (var item in list)
                    //    {
                    //        TTChuyenKhoan += "<div>" + item + "</div>";
                    //    }
                    //}
                    string TTNote = string.Empty;
                    if (model.TTNote != null)
                    {
                        var list = Array.ConvertAll(model.TTNote.Split('\n'), s => (s).ToString());
                        foreach (var item in list)
                        {
                            TTNote += "<div>" + item + "</div>";
                        }
                    }
                    body = body.Replace("{{TileEmail}}", model.TileEmail);
                    body = body.Replace("{{OrderAmount}}", model.OrderAmount.ToString("N0"));
                    body = body.Replace("{{userName}}", model.user_Name);
                    body = body.Replace("{{userPhone}}", model.user_Phone);
                    body = body.Replace("{{userEmail}}", model.user_Email);
                    body = body.Replace("{{orderNo}}", model.OrderNo);
                    body = body.Replace("{{OrderPackages}}", Packagesdata);
                    body = body.Replace("{{SalerName}}", model.saler_Name);
                    body = body.Replace("{{SalerPhone}}", model.saler_Phone);
                    body = body.Replace("{{SalerEmail}}", model.saler_Email);
                    body = body.Replace("{{NDChuyenKhoan}}", model.NDChuyenKhoan);
                    //body = body.Replace("{{TTChuyenKhoan}}", TTChuyenKhoan);
                    body = body.Replace("{{TTNote}}", TTNote);
                    body = body.Replace("{{payment_notification}}", model.PaymentNotification); ;

                    return body;
                }

            }
            catch (Exception ex)
            {

                LogHelper.InsertLogTelegram("OrderTemplateBody - MailService: " + ex.ToString());
                return null;
            }
        }
        public async Task<string> GetOrderFlyTemplateBody(long id, string order_note = "", string payment_notification = "", bool is_edit_form = false)
        {
            try
            {


                var fly_list = _flyBookingDetailRepository.GetListByOrderId(Convert.ToInt32(id));
                if (fly_list == null || fly_list.Count < 1)
                {
                    LogHelper.InsertLogTelegram("GetFlyBookingTemplateBody - MailService: NULL DATA with [" + id + "]");
                    return null;
                }
                var order = await _orderRepository.GetOrderByID(Convert.ToInt32(fly_list[0].OrderId));

                FlyBookingDetail flyBookingDetail = _flyBookingDetailRepository.GetByOrderId(id);
                List<FlyBookingDetail> flyBookingDetailList = _flyBookingDetailRepository.GetListByOrderId(id);
                FlightSegment flightSegment = _flightSegmentRepository.GetByFlyBookingDetailId(flyBookingDetail != null ? flyBookingDetail.Id : 0);
                List<FlightSegment> flightSegmentList = _flightSegmentRepository.GetByFlyBookingDetailIds(flyBookingDetailList.Select(n => n.Id).ToList());
                var listAirportCode = await _airlinesRepository.getAllAirportCode();
                var listAirlines = await _airlinesRepository.getAllAirlines();
                if (order == null) return null;

                //string workingDirectory = Directory.GetCurrentDirectory();
                string workingDirectory = AppDomain.CurrentDomain.BaseDirectory;
                var template = workingDirectory + @"\EmailTemplate\OrderFlyTemplate.html";
                string body = File.ReadAllText(template);

                body = body.Replace("{{orderNo}}", order.OrderNo);

                var data = await _passengerRepository.GetPassengerByOrderId(id);
                List<Baggage> baggages = null;
                if (data != null)
                {
                    var listid = data.Select(s => s.Id).ToList();
                    baggages = _bagageRepository.GetBaggages(listid);
                }

                string list_passenger = String.Empty;
                string list_passenger2 = String.Empty;
                var count = 1;
                if (data != null && data.Count > 0)
                {
                    foreach (var item in data)
                    {
                        Baggage baggageGo = baggages.FirstOrDefault(n => n.PassengerId == item.Id && n.Leg == (int)CommonConstant.FlyBookingDetailType.GO);
                        Baggage baggageBack = baggages.FirstOrDefault(n => n.PassengerId == item.Id && n.Leg == (int)CommonConstant.FlyBookingDetailType.BACK);
                        var flyBookingGo = flyBookingDetailList.FirstOrDefault(n => n.Leg == (int)CommonConstant.FlyBookingDetailType.GO);
                        var flightSegmentGo = flightSegmentList.FirstOrDefault(n => n.FlyBookingId == flyBookingGo?.Id);
                        var flyBookingBack = flyBookingDetailList.FirstOrDefault(n => n.Leg != (int)CommonConstant.FlyBookingDetailType.GO);
                        var flightSegmentBack = flightSegmentList.FirstOrDefault(n => n.FlyBookingId == flyBookingBack?.Id);

                        string baggeGo = String.Empty;
                        string baggeBack = String.Empty;

                        if (flightSegmentGo != null && flightSegmentGo.AllowanceBaggageValue > 0)
                            baggeGo = " + " + flightSegmentGo?.AllowanceBaggageValue + " kg ký gửi ";

                        if (baggageGo != null && baggageGo.WeightValue > 0)
                            baggeGo = " + " + baggageGo?.WeightValue + " kg ký gửi ";

                        if (baggageGo != null && flightSegmentGo != null && baggageGo.WeightValue > 0 && flightSegmentGo.AllowanceBaggageValue > 0)
                            baggeGo = " + " + (flightSegmentGo?.AllowanceBaggageValue + baggageGo?.WeightValue) + " kg ký gửi ";

                        if (flightSegmentBack != null && flightSegmentBack.AllowanceBaggageValue > 0)
                            baggeBack = " + " + flightSegmentBack?.AllowanceBaggageValue + " kg ký gửi ";

                        if (baggageBack != null && baggageBack.WeightValue > 0)
                            baggeBack = " + " + baggageBack?.WeightValue + " kg ký gửi ";

                        if (baggageBack != null && flightSegmentBack != null && baggageBack.WeightValue > 0 && flightSegmentBack.AllowanceBaggageValue > 0)
                            baggeBack = " + " + (flightSegmentBack?.AllowanceBaggageValue + baggageBack?.WeightValue) + " kg ký gửi ";
                        string birthday = string.Empty;
                        if (item.Birthday != null)
                            birthday = item.Birthday.Value.ToString("dd/MM/yyyy");

                        list_passenger += @"<tr  colspan='6' style=" + "\"" + "font-size: 14px;{{isDisplayGo}}" + "\"" + ">" +
                                        "<td style='border: 1px solid #999; padding: 8px;text-align: center;'>" + count + @"</td>" +
                                        "<td colspan='3' style='border: 1px solid #999; padding: 8px;text-align: center;'>" + item.Name + @"</td>" +

                                        "<td colspan='3'style='border: 1px solid #999; padding: 8px;font-size: 14px;text-align: center;'>" +
                                             flightSegmentGo?.HandBaggageValue + " kg xách tay " + baggeGo +
                                        "</td>" +
                                    "</tr> ";
                        list_passenger2 += @"<tr  colspan='6' style=" + "\"" + "font-size: 14px;{{isDisplayBack}}" + "\"" + ">" +
                                           "<td style='border: 1px solid #999; padding: 8px;text-align: center;'>" + count + @"</td>" +
                                           "<td colspan='3' style='border: 1px solid #999; padding: 8px;text-align: center;'>" + item.Name + @"</td>" +

                                           "<td colspan='3' style='border: 1px solid #999; padding: 8px;font-size: 14px;text-align: center;'>" +
                                                flightSegmentBack?.HandBaggageValue + " kg xách tay " + baggeBack +
                                           "</td>" +
                                       "</tr> ";

                        count++;
                    }
                }
                body = body.Replace("{{passengerList}}", list_passenger);
                body = body.Replace("{{passengerList2}}", list_passenger2);

                var images_url = "https://static-image.adavigo.com/uploads/images/airlinelogo/";
                //thông tin chuyến bay đi
                var flyBookingDetailGo = flyBookingDetailList.FirstOrDefault(n => n.Leg == (int)CommonConstant.FlyBookingDetailType.GO);
                if (flyBookingDetailGo != null)
                {
                    body = body.Replace("{{isDisplayGo}}", "");
                    var airportCodeStartPoint = listAirportCode.FirstOrDefault(n => n.Code == flyBookingDetailGo.StartPoint);
                    var airportCodeEndPoint = listAirportCode.FirstOrDefault(n => n.Code == flyBookingDetailGo.EndPoint);
                    body = body.Replace("{{flyOrderNoGo}}", flyBookingDetailGo.BookingCode);

                    FlightSegment flightSegmentDeatl = flightSegmentList.FirstOrDefault(n => n.FlyBookingId == flyBookingDetailGo.Id);
                    if (flightSegmentDeatl != null)
                    {
                        body = body.Replace("{{handBaggageGo}}", flightSegmentDeatl.HandBaggage);//hành lý chiều đi
                        body = body.Replace("{{allowanceBaggageGo}}", flightSegmentDeatl.AllowanceBaggage);//hành lý ký gửi chiều đi
                        //get logo theo flightSegment.FlightNumber, get url ảnh
                        body = body.Replace("{{logoAirlineGo}}", images_url + flyBookingDetailGo.Airline.ToLower() + ".png");
                        var airline = listAirlines.FirstOrDefault(n => n.Code == flyBookingDetailGo.Airline);//get theo flyBookingDetailGo.Airline
                        body = body.Replace("{{flyNameGo}}", airline?.NameVi);
                        body = body.Replace("{{flyCodeGo}}", flightSegmentDeatl.FlightNumber);
                        body = body.Replace("{{dayGo}}", flightSegmentDeatl.StartTime != null ?
                            GetDay(flightSegmentDeatl.StartTime.DayOfWeek) : "");
                        body = body.Replace("{{dateGo}}", flightSegmentDeatl.StartTime != null ?
                            flightSegmentDeatl.StartTime.ToString("dd/MM/yyyy") : "");
                        body = body.Replace("{{addressGoFrom}}", airportCodeStartPoint?.DistrictVi);//HAN - Hà Nội
                        body = body.Replace("{{addressGoTo}}", airportCodeEndPoint?.DistrictVi);// PQC - Phú Quốc
                        var groupclassAirline = await _airlinesRepository.getGroupClassAirlines(flightSegmentDeatl.Class, flyBookingDetailGo.Airline, flyBookingDetailGo.GroupClass);
                        if (groupclassAirline != null)
                            body = body.Replace("{{flyTicketClassGo}}", groupclassAirline?.DetailVi);
                        else
                            body = body.Replace("{{flyTicketClassGo}}", flyBookingDetailGo.GroupClass);

                        body = body.Replace("{{timeFromGo}}", flyBookingDetailGo.StartDate.Value.ToString("HH:mm"));//10:40
                        body = body.Replace("{{timeToGo}}", flyBookingDetailGo.EndDate.Value.ToString("HH:mm"));//22:45
                    }
                }
                else
                {
                    body = body.Replace("{{displayChieuDi}}", @"style=" + "\"" + "display: none!important;" + "\"");
                    body = body.Replace("{{isDisplayGo}}", "display: none!important;");
                }
                //thông tin chuyến bay về
                var flyBookingDetailBack = flyBookingDetailList.FirstOrDefault(n => n.Leg != (int)CommonConstant.FlyBookingDetailType.GO);
                if (flyBookingDetailBack != null)
                {
                    body = body.Replace("{{isDisplayBack}}", "");
                    body = body.Replace("{{flyOrderNoBack}}", flyBookingDetailBack.BookingCode);
                    var airportCodeStartPoint = listAirportCode.FirstOrDefault(n => n.Code == flyBookingDetailBack.StartPoint);
                    var airportCodeEndPoint = listAirportCode.FirstOrDefault(n => n.Code == flyBookingDetailBack.EndPoint);
                    FlightSegment flightSegmentDetail = flightSegmentList.FirstOrDefault(n => n.FlyBookingId == flyBookingDetailBack.Id);
                    body = body.Replace("{{handBaggageBack}}", flightSegmentDetail.HandBaggage);//hành lý chiều về
                    body = body.Replace("{{allowanceBaggageBack}}", flightSegmentDetail.AllowanceBaggage);//hành lý ký gửi chiều về
                    body = body.Replace("{{flyCodeBack}}", flightSegmentDetail.FlightNumber);
                    body = body.Replace("{{addressBackFrom}}", airportCodeStartPoint?.DistrictVi);//HAN - Hà Nội
                    body = body.Replace("{{addressBackTo}}", airportCodeEndPoint?.DistrictVi);// PQC - Phú Quốc
                    body = body.Replace("{{dayBack}}", flightSegmentDetail.StartTime != null ?
                        GetDay(flightSegmentDetail.StartTime.DayOfWeek) : "");
                    body = body.Replace("{{dateBack}}", flightSegmentDetail.StartTime != null ?
                            flightSegmentDetail.StartTime.ToString("dd/MM/yyyy") : "");
                    body = body.Replace("{{timeFromBack}}", flyBookingDetailBack.StartDate.Value.ToString("HH:mm"));//10:40
                    body = body.Replace("{{timeToBack}}", flyBookingDetailBack.EndDate.Value.ToString("HH:mm"));//22:45

                    //get logo theo flightSegment.FlightNumber, get url ảnh
                    body = body.Replace("{{logoAirlineBack}}", images_url + flyBookingDetailBack.Airline.ToLower() + ".png");
                    var airline = listAirlines.FirstOrDefault(n => n.Code == flyBookingDetailBack.Airline);//get theo flyBookingDetailBack.Airline
                    body = body.Replace("{{flyNameBack}}", airline?.NameVi);
                    var groupclassAirline = await _airlinesRepository.getGroupClassAirlines(flightSegmentDetail.Class, flyBookingDetailBack.Airline, flyBookingDetailBack.GroupClass);
                    if (groupclassAirline != null)
                        body = body.Replace("{{flyTicketClassBack}}", groupclassAirline?.DetailVi);
                    else
                        body = body.Replace("{{flyTicketClassBack}}", flyBookingDetailBack.GroupClass);
                }
                else
                {
                    body = body.Replace("{{displayChieuVe}}", @"style=" + "\"" + "display: none!important;" + "\"");
                    body = body.Replace("{{isDisplayBack}}", "display: none!important;");
                }
                return body;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetFlyBookingTemplateBody - MailService: " + ex);
                return null;
            }
        }

        public async Task<string> OrderTemplateSaleDH(long id, string payment_notification = "", bool is_edit_form = false)
        {
            try
            {

                var order = await _orderRepository.GetOrderByID(id);
                var data = await _orderRepository.GetAllServiceByOrderId(id);
                if (order == null)
                {
                    LogHelper.InsertLogTelegram("OrderTemplateSaleDH - MailService: NULL DATA with [" + id + "]");
                    return null;
                }
                if (order != null)
                {
                    if (data != null)
                        foreach (var item in data)
                        {
                            item.Price += item.Profit;
                            if (item.Type.Equals("Tour"))
                            {
                                item.tour = await _tourRepository.GetDetailTourByID(Convert.ToInt32(item.ServiceId));

                                var note = await _hotelBookingRepositories.GetServiceDeclinesByServiceId(item.ServiceId, 5);
                                if (note != null)
                                    item.Note = note.UserName + " đã từ chối lý do: " + note.Note;
                            }
                            if (item.Type.Equals("Khách sạn"))
                            {
                                item.Hotel = await _hotelBookingRepositories.GetDetailHotelBookingByID(Convert.ToInt32(item.ServiceId));
                                var note = await _hotelBookingRepositories.GetServiceDeclinesByServiceId(item.ServiceId, 1);
                                if (note != null)
                                    item.Note = note.UserName + " đã từ chối lý do: " + note.Note;
                            }
                            if (item.Type.Equals("Vé máy bay"))
                            {
                                item.Flight = await _flyBookingDetailRepository.GetDetailFlyBookingDetailById(Convert.ToInt32(item.ServiceId));

                                if (item.Flight.GroupBookingId != null)
                                {
                                    var note = await _hotelBookingRepositories.GetServiceDeclinesByServiceId(item.Flight.GroupBookingId, 3);
                                    if (note != null)
                                        item.Note = note.UserName + " đã từ chối lý do: " + note.Note;
                                }

                            }
                            if (item.Type.Equals("Dịch vụ khác"))
                            {
                                item.OtherBooking = await _otherBookingRepository.GetDetailOtherBookingById(Convert.ToInt32(item.ServiceId));
                                var note = await _hotelBookingRepositories.GetServiceDeclinesByServiceId(item.ServiceId, (int)ServicesType.Other);
                                if (note != null)
                                    item.Note = note.UserName + " đã từ chối lý do: " + note.Note;
                            }
                        }
                    if (data != null && data.Count > 1)
                    {
                        for (int o = 0; o < data.Count - 1; o++)
                        {
                            if (data[o].Flight != null && data[o + 1].Flight != null)
                            {
                                if (data[o].Flight.GroupBookingId == data[o + 1].Flight.GroupBookingId && data[o].Flight.Leg != data[o + 1].Flight.Leg)
                                {
                                    data[o].Flight.StartDistrict2 = data[o + 1].Flight.StartDistrict;
                                    data[o].Flight.EndDistrict2 = data[o + 1].Flight.EndDistrict;
                                    data[o].Flight.AirlineName_Vi2 = data[o + 1].Flight.AirlineName_Vi;
                                    data[o].Flight.Leg2 = 3;
                                    data[o].Flight.BookingCode2 = data[o + 1].Flight.BookingCode;
                                    data[o].Amount = data[o].Flight.Amount + data[o + 1].Flight.Amount;
                                    data[o].EndDate = data[o + 1].EndDate;


                                    data.Remove(data[o + 1]);

                                }
                            }

                        }
                    }

                }
                string Packagesdata = string.Empty;

                if (data != null)
                {
                    foreach (var item in data)
                    {
                        string date = string.Empty;
                        string Packagesdetail = string.Empty;
                        string PackagesOrder = string.Empty;
                        if (item.Type.Equals("Vé máy bay"))
                        {
                            if (item.Flight != null)
                            {
                                if (item.Flight.Leg2 != 3)
                                {
                                    if (item.Flight.Leg == 0)
                                    {
                                        date = item.StartDate.ToString("dd/MM/yyyy");
                                    }
                                    if (item.Flight.Leg == 1)
                                    {
                                        date = item.StartDate.ToString("dd/MM/yyyy");
                                    }
                                }
                                else
                                {
                                    date = item.StartDate.ToString("dd/MM/yyyy") + "-" + item.EndDate.ToString("dd/MM/yyyy");
                                }

                            }
                        }
                        else
                        {
                            date = item.StartDate.ToString("dd/MM/yyyy") + "-" + item.EndDate.ToString("dd/MM/yyyy");
                        }
                        if (item.Type.Equals("Tour"))
                        {
                            string note = string.Empty;
                            if (item.tour != null)
                            {
                                string Point = string.Empty;
                                if (item.tour.TourType == 1)
                                {
                                    Point =
                                      "<tr>" +
                                          "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;'>Điểm đi:</td>" +
                                          "<td style='border: 1px solid #999; padding: 5px;'>" + item.tour.StartPoint1 + "</td>" +
                                          "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;'>Điểm đến:</td>" +
                                          "<td style='border: 1px solid #999; padding: 5px;'>" + item.tour.GroupEndPoint1 + "</td>" +
                                      "</tr>";
                                }
                                if (item.tour.TourType == 2)
                                {
                                    Point =
                                      "<tr>" +
                                          "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;'>Điểm đi:</td>" +
                                          "<td style='border: 1px solid #999; padding: 5px;'>" + item.tour.StartPoint2 + "</td>" +
                                          "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;'>Điểm đến:</td>" +
                                          "<td style='border: 1px solid #999; padding: 5px;'>" + item.tour.GroupEndPoint2 + "</td>" +
                                      "</tr>";
                                }
                                if (item.tour.TourType == 3)
                                {
                                    Point =
                                      "<tr>" +
                                          "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;'>Điểm đi:</td>" +
                                          "<td style='border: 1px solid #999; padding: 5px;'>" + item.tour.StartPoint3 + "</td>" +
                                          "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;'>Điểm đến:</td>" +
                                          "<td style='border: 1px solid #999; padding: 5px;'>" + item.tour.GroupEndPoint3 + "</td>" +
                                      "</tr>";
                                }

                                note += "<tr>" +
                                        "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;'>Ngày khởi hành:</td>" +
                                        "<td style='border: 1px solid #999; padding: 5px;' > " + ((DateTime)item.tour.StartDate).ToString("dd/MM/yyyy") + " </td> " +
                                       "<td style= 'border: 1px solid #999; padding: 5px; font-weight: bold;'>Ngày đến:</td>" +
                                       " <td style= 'border: 1px solid #999; padding: 5px;'> " + ((DateTime)item.tour.EndDate).ToString("dd/MM/yyyy") + "</td>" +
                                    "</tr>" +

                                    Point +
                                      "<tr>" +
                                        "<td style= 'border: 1px solid #999; padding: 5px; font-weight: bold;' > Loại tour:</td>" +
                                       " <td style= 'border: 1px solid #999; padding: 5px;' >" + item.tour.ORGANIZINGName + "</td>" +
                                        "<td style= 'border: 1px solid #999; padding: 5px; font-weight: bold;' > Số lượng khách (NL/TE/EB):</td>" +
                                        "<td style= 'border: 1px solid #999; padding: 5px;' >" + item.tour.TotalAdult + "/" + item.tour.TotalChildren + "/" + item.tour.TotalBaby + "</td>" +
                                    "</tr>" +
                                    "<tr>" +
                                        "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;'>Tổng tiền tour:</td>" +
                                        "<td colspan= '3' style = 'border: 1px solid #999; padding: 5px;' >" + ((double)item.tour.Amount).ToString("N0") + "</td>" +
                                   "</tr>";

                                Packagesdetail = "<td colspan='4' style = 'border: 1px solid #999; padding: 5px; font-weight: bold;text-align: center;' > Dịch vụ tour</ td > " +
                                                "" + note + "" +
                                                "<tr>" +
                                                "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;'>Ghi chú:</td>" +
                                                "<td colspan='3' style='border: 1px solid #999; padding: 5px;'>" + item.Note + "</td></tr>";

                            }
                        }
                        if (item.Type.Equals("Khách sạn"))
                        {
                            if (item.Hotel != null)
                            {
                                string note = string.Empty;

                                var hotedetail = await _hotelBookingRepositories.GetHotelBookingById(Convert.ToInt32(item.ServiceId));

                                note += "<tr>" +
                                    "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;'>Ngày nhận phòng:</td>" +
                                    "<td style='border: 1px solid #999; padding: 5px;' >" + item.Hotel[0].ArrivalDate.ToString("dd/MM/yyyy") + " </td> " +
                                   "<td style= 'border: 1px solid #999; padding: 5px; font-weight: bold;'>Ngày trả phòng:</td>" +
                                   " <td style= 'border: 1px solid #999; padding: 5px;'>" + item.Hotel[0].DepartureDate.ToString("dd/MM/yyyy") + "</td>" +
                                "</tr>" +
                                "<tr>" +
                                    "<td style= 'border: 1px solid #999; padding: 5px; font-weight: bold;' > Số lượng phòng:</td>" +
                                   " <td style= 'border: 1px solid #999; padding: 5px;' >" + item.Hotel[0].TotalRooms + "</td>" +
                                    "<td rowspan='2' style= 'border: 1px solid #999; padding: 5px; font-weight: bold;' > Số lượng khách (NL/TE/EB):</td>" +
                                    "<td rowspan='2' style= 'border: 1px solid #999; padding: 5px;' >" + item.Hotel[0].NumberOfAdult + "/" + item.Hotel[0].NumberOfChild + "/" + item.Hotel[0].NumberOfInfant + "</td>" +
                                "</tr>" +
                                "<tr>" +
                                    "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;'>Số đêm</td>" +
                                    "<td style='border: 1px solid #999; padding: 5px;'>" + item.Hotel[0].TotalDays + "</td>" +
                                "</tr>" +

                                "<tr>" +
                                    "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;'>Tổng tiền phòng:</td>" +
                                    "<td colspan= '3' style = 'border: 1px solid #999; padding: 5px;' >" + item.Hotel[0].TotalAmount.ToString("N0") + "</td>" +
                               "</tr>";



                                Packagesdetail = "<tr><td colspan='4' style = 'border: 1px solid #999; padding: 5px; font-weight: bold;text-align: center;' > Dịch vụ khách sạn " + hotedetail[0].HotelName + "</ td ></tr> " +
                                                "" + note + "" +
                                                      "<tr>" +
                                                      "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;'>Ghi chú:</td>" +
                                                      "<td colspan='3' style='border: 1px solid #999; padding: 5px;'>" + item.Note + "</td></tr>";

                            }
                        }
                        if (item.Type.Equals("Vé máy bay"))
                        {
                            string note = string.Empty;
                            if (item.Flight != null)
                            {
                                if (item.Flight.Leg2 == 3)
                                {
                                    note += "<tr>" +
                                       "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;'>Ngày khởi hành:</td>" +
                                      "<td style='border: 1px solid #999; padding: 5px;' >" + item.Flight.StartDate.ToString("dd/MM/yyyy") + "</td>" +
                                      "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;'>Ngày hạ cánh</td>" +
                                      "<td style='border: 1px solid #999; padding: 5px;' >" + item.Flight.EndDate.ToString("dd/MM/yyyy") + "</td>" +
                                 "</tr>" +
                                 "<tr>" +
                                     "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;' >Điểm đi:</td>" +
                                     "<td style='border: 1px solid #999; padding: 5px;' >" + item.Flight.StartDistrict + "</td>" +
                                     "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;' >Điểm đến:</td>" +
                                     "<td style='border: 1px solid #999; padding: 5px;' >" + item.Flight.EndDistrict + "</td>" +
                                  "</tr>" +
                                "<tr>" +
                                      "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;' > Hãng bay:</td>" +
                                      "<td style='border: 1px solid #999; padding: 5px;' >" + item.Flight.AirlineName_Vi + "</td>" +
                                       "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;' >Mã giữ chỗ :</td> " +
                                      "<td style='border: 1px solid #999; padding: 5px;' >" + item.Flight.BookingCode + "</td>" +

                                   "</tr>" +
                                 "<tr>" +
                                      "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;' > Điểm đi:</td>" +
                                      "<td style='border: 1px solid #999; padding: 5px;' >" + item.Flight.StartDistrict2 + "</td>" +
                                      "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;' > Điểm đến:</td>" +
                                      "<td style='border: 1px solid #999; padding: 5px;' >" + item.Flight.EndDistrict2 + "</td>" +
                                   "</tr>" +
                                     "<tr>" +
                                      "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;' > Hãng bay:</td>" +
                                      "<td style='border: 1px solid #999; padding: 5px;' >" + item.Flight.AirlineName_Vi2 + "</td>" +
                                       "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;' >Mã giữ chỗ :</td> " +
                                      "<td style='border: 1px solid #999; padding: 5px;' >" + item.Flight.BookingCode2 + "</td>" +

                                   "</tr>" +
                                  "<tr>" +
                                     "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;' >Hành trình:</td>" +
                                     "<td style='border: 1px solid #999; padding: 5px;' >Khứ hồi</td>" +
                                    "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;' > Số lượng khách (NL/TE/EB):</td> " +
                                       "<td style='border: 1px solid #999; padding: 5px;' >" + item.Flight.AdultNumber + "/" + item.Flight.ChildNumber + "/" + item.Flight.InfantNumber + "</td>" +
                                  "</tr>" +
                                   "<tr>" +
                                        "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;'>Tổng tiền vé:</td>" +
                                        "<td colspan= '3' style = 'border: 1px solid #999; padding: 5px;' >" + item.Amount.ToString("N0") + "</td>" +
                                   "</tr>";

                                }
                                else
                                {

                                    if (item.Flight.Leg == 0)
                                    {
                                        note += "<tr>" +
                                      "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;'>Ngày khởi hành:</td>" +
                                      "<td style='border: 1px solid #999; padding: 5px;' >" + item.Flight.StartDate.ToString("dd/MM/yyyy") + "</td>" +
                                      "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;'>Ngày hạ cánh</td>" +
                                      "<td style='border: 1px solid #999; padding: 5px;' >" + item.Flight.EndDate.ToString("dd/MM/yyyy") + "</td>" +
                                 "</tr>" +
                                  "<tr>" +
                                      "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;' > Điểm đi:</td>" +
                                      "<td style='border: 1px solid #999; padding: 5px;' >" + item.Flight.StartDistrict + "</td>" +
                                      "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;' > Điểm đến:</td>" +
                                      "<td style='border: 1px solid #999; padding: 5px;' >" + item.Flight.EndDistrict + "</td>" +
                                   "</tr>" +
                                  "<tr>" +
                                      "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;' > Hãng bay:</td>" +
                                      "<td style='border: 1px solid #999; padding: 5px;' >" + item.Flight.AirlineName_Vi + "</td>" +
                                       "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;' >Mã giữ chỗ :</td> " +
                                      "<td style='border: 1px solid #999; padding: 5px;' >" + item.Flight.BookingCode + "</td>" +

                                   "</tr>" +
                                  "<tr>" +
                                     "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;' >Hành trình:</td>" +
                                     "<td style='border: 1px solid #999; padding: 5px;' >1 chiều</td>" +
                                    "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;' > Số lượng khách (NL/TE/EB):</td> " +
                                       "<td style='border: 1px solid #999; padding: 5px;' >" + item.Flight.AdultNumber + "/" + item.Flight.ChildNumber + "/" + item.Flight.InfantNumber + "</td>" +
                                  "</tr>" +
                                   "<tr>" +
                                        "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;'>Tổng tiền vé:</td>" +
                                        "<td colspan= '3' style = 'border: 1px solid #999; padding: 5px;' >" + item.Amount.ToString("N0") + "</td>" +
                                   "</tr>";
                                        /*note += " Chiều đi:" + item.Flight.StartDistrict + "-" + item.Flight.EndDistrict + " - Mã đặt chỗ:" + item.Flight.BookingCode + "&#10" +

                                                        "Chiều về:" + item.Flight.StartDistrict2 + "-" + item.Flight.EndDistrict2 + "-  Mã đặt chỗ: " + item.Flight.BookingCode2 + "&#10";*/
                                    }
                                    if (item.Flight.Leg == 1)
                                    {
                                        note += "<tr>" +
                                      "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;'>Ngày khởi hành:</td>" +
                                      "<td style='border: 1px solid #999; padding: 5px;' >" + item.Flight.StartDate.ToString("dd/MM/yyyy") + "</td>" +
                                      "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;'>Ngày hạ cánh</td>" +
                                      "<td style='border: 1px solid #999; padding: 5px;' >" + item.Flight.EndDate.ToString("dd/MM/yyyy") + "</td>" +
                                 "</tr>" +
                                  "<tr>" +
                                      "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;' > Điểm đi:</td>" +
                                      "<td style='border: 1px solid #999; padding: 5px;' >" + item.Flight.StartDistrict2 + "</td>" +
                                      "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;' > Điểm đến:</td>" +
                                      "<td style='border: 1px solid #999; padding: 5px;' >" + item.Flight.EndDistrict2 + "</td>" +
                                   "</tr>" +
                                  "<tr>" +
                                      "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;' > Hãng bay:</td>" +
                                      "<td style='border: 1px solid #999; padding: 5px;' >" + item.Flight.AirlineName_Vi2 + "</td>" +
                                       "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;' >Mã giữ chỗ :</td> " +
                                      "<td style='border: 1px solid #999; padding: 5px;' >" + item.Flight.BookingCode2 + "</td>" +

                                   "</tr>" +
                                  "<tr>" +
                                     "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;' >Hành trình:</td>" +
                                     "<td style='border: 1px solid #999; padding: 5px;' >1 chiều</td>" +
                                    "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;' > Số lượng khách (NL/TE/EB):</td> " +
                                       "<td style='border: 1px solid #999; padding: 5px;' >" + item.Flight.AdultNumber + "/" + item.Flight.ChildNumber + "/" + item.Flight.InfantNumber + "</td>" +
                                  "</tr>" +
                                   "<tr>" +
                                        "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;'>Tổng tiền vé:</td>" +
                                        "<td colspan= '3' style = 'border: 1px solid #999; padding: 5px;' >" + item.Amount.ToString("N0") + "</td>" +
                                   "</tr>";

                                    }


                                }

                                Packagesdetail = "<tr><td colspan='4' style = 'border: 1px solid #999; padding: 5px; font-weight: bold;text-align: center;' > Dịch vụ " + item.Type + "</td ></tr>" +
                                                "" + note + "" +
                                                "<tr>" +
                                                "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;'>Ghi chú:</td>" +
                                                "<td colspan='3' style='border: 1px solid #999; padding: 5px;'>" + item.Note + "</td></tr>";

                            }
                            if (item.Type.Equals("Dịch vụ khác"))
                            {
                                if (item.OtherBooking != null)
                                {

                                    note += "<tr>" +
                                        "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;'>Ngày bắt đầu:</td>" +
                                        "<td style='border: 1px solid #999; padding: 5px;' >" + item.OtherBooking[0].StartDate.ToString("dd/MM/yyyy") + " </td> " +
                                       "<td style= 'border: 1px solid #999; padding: 5px; font-weight: bold;'>Ngày ngày kết thúc:</td>" +
                                       " <td style= 'border: 1px solid #999; padding: 5px;'>" + item.OtherBooking[0].EndDate.ToString("dd/MM/yyyy") + "</td>" +
                                    "</tr>" +
                                    "<tr>" +
                                        "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;'>Tổng tiền dihcj vụ:</td>" +
                                        "<td colspan= '3' style = 'border: 1px solid #999; padding: 5px;' >" + item.OtherBooking[0].Amount.ToString("N0") + "</td>" +
                                   "</tr>";


                                    Packagesdetail = "<tr><td colspan='4' style = 'border: 1px solid #999; padding: 5px; font-weight: bold;text-align: center;' > Dịch vụ : " + item.OtherBooking[0].ServiceName + "</ td ></tr> " +
                                                    "" + note + "" +
                                                          "<tr>" +
                                                          "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;'>Ghi chú:</td>" +
                                                          "<td colspan='3' style='border: 1px solid #999; padding: 5px;'><input id='hotelNote'type='text'value=\"" + item.Note + "\"></td></tr>";

                                }
                            }


                        }
                        if (item.Type.Equals("Dịch vụ khác"))
                        {
                            if (item.OtherBooking != null)
                            {
                                string note = string.Empty;

                                note += "<tr>" +
                                    "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;'>Ngày bắt đầu:</td>" +
                                    "<td style='border: 1px solid #999; padding: 5px;' >" + item.OtherBooking[0].StartDate.ToString("dd/MM/yyyy") + " </td> " +
                                   "<td style= 'border: 1px solid #999; padding: 5px; font-weight: bold;'>Ngày kết thúc:</td>" +
                                   " <td style= 'border: 1px solid #999; padding: 5px;'>" + item.OtherBooking[0].EndDate.ToString("dd/MM/yyyy") + "</td>" +
                                "</tr>" +
                                "<tr>" +
                                    "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;'>Tổng tiền dịch vụ:</td>" +
                                    "<td colspan= '3' style = 'border: 1px solid #999; padding: 5px;' >" + item.OtherBooking[0].Amount.ToString("N0") + "</td>" +
                               "</tr>";



                                Packagesdetail = "<table class='Other-row' role='presentation' border='0' width='100%' style='border: 0; border-spacing: 0; text-indent: 0; border-collapse: collapse; font-size: 13px; width: 100%;'><tr>" +
                                    "<td colspan='4' style = 'border: 1px solid #999; padding: 5px; font-weight: bold;text-align: center;' > Dịch vụ : " + item.OtherBooking[0].ServiceName + "</td></tr> " +
                                                "" + note + "" +
                                                      "<tr>" +
                                                      "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;'>Ghi chú:</td>" +
                                                      "<td colspan='3' style='border: 1px solid #999; padding: 5px;'>" + item.Note + "</td></tr>";

                            }
                        }
                        Packagesdata += "<table class='Tour-row' role='presentation' border='0' width='100%' style='border: 0; border-spacing: 0; text-indent: 0; border-collapse: collapse; font-size: 13px; width: 100%;'>" +
                            Packagesdetail +
                            "</table>";
                    }
                }
                string Packages = string.Empty;

                Packages = Packagesdata;

                string workingDirectory = AppDomain.CurrentDomain.BaseDirectory;


                var template = workingDirectory + @"\EmailTemplate\OrderTemplate.html";
                string body = File.ReadAllText(template);
                if (order.ClientId != null && order.ClientId != 0)
                {
                    var contact = await _clientRepository.GetClientDetailByClientId((long)order.ClientId);
                    body = body.Replace("{{userName}}", contact.ClientName);
                    body = body.Replace("{{userPhone}}", contact.Phone);
                    body = body.Replace("{{userEmail}}", contact.Email);
                }
                else
                {

                    body = body.Replace("{{userName}}", "");
                    body = body.Replace("{{userPhone}}", "");
                    body = body.Replace("{{userEmail}}", "");
                }
                body = body.Replace("{{orderNo}}", order.OrderNo);
                body = body.Replace("{{OrderAmount}}", ((double)order.Amount).ToString("N0"));
                if (order.SalerId != null && order.SalerId != 0)
                {
                    var saler = await _userRepository.GetById((long)order.SalerId);
                    body = body.Replace("{{SalerName}}", saler.FullName);
                    body = body.Replace("{{SalerPhone}}", saler.Phone);
                    body = body.Replace("{{SalerEmail}}", saler.Email);

                }
                else
                {
                    body = body.Replace("{{SalerName}}", "");
                    body = body.Replace("{{SalerPhone}}", "");
                    body = body.Replace("{{SalerEmail}}", "");

                }
                body = body.Replace("{{OrderPackages}}", Packagesdata);

                body = body.Replace("{{payment_notification}}", "");
                body = body.Replace("{{styleTTCK}}", "style='display: none !important;'");

                string TTChuyenKhoan = "<strong>1. Techcombank – STK: 19131835226016 (Doanh thu phòng khách sạn - Có xuất hoá đơn) </strong>" +
                                   "<p>CTK: Công ty Cổ phần Thương mại và Dịch vụ Quốc tế Đại Việt </p>" +
                                   "<p>Chi nhánh: Đông Đô </p> " +
                                   "<strong>2. HDBANK – STK: 371704070000023 (Doanh thu vé máy bay - Có xuất hoá đơn) </strong>" +
                                   "<p>CTK: Công ty Cổ phần Thương mại và Dịch vụ Quốc tế Đại Việt </p>" +
                                   "<p>Chi nhánh: Hà Nội</p> " +
                                    "<strong>3. Vietin Bank - STK: 113600558866 (Doanh thu Tour - Có xuất hoá đơn) </strong>" +
                                   "<p>CTK: Công ty Cổ phần Thương mại và Dịch vụ Quốc tế Đại Việt </p>" +
                                   "<p>Chi nhánh: Tràng An</p>  ";
                body = body.Replace("{{TTChuyenKhoan}}", TTChuyenKhoan);
                body = body.Replace("{{TTNote}}", order.Note);

                body = body.Replace("{{NDChuyenKhoan}}", order.OrderNo + " CHUYEN KHOAN");
                body = body.Replace("{{TileEmail}}", "PHIẾU XÁC NHẬN ĐƠN HÀNG " + order.OrderNo);

                return body;

            }
            catch (Exception ex)
            {

                LogHelper.InsertLogTelegram("OrderTemplateSaleDH - MailService: " + ex.ToString());
                return null;
            }
        }
        public string GetDay(DayOfWeek day)
        {
            var dayStr = String.Empty;
            if (day == DayOfWeek.Monday)
            {
                dayStr = "Thứ 2";
            }
            if (day == DayOfWeek.Tuesday)
            {
                dayStr = "Thứ 3";
            }
            if (day == DayOfWeek.Wednesday)
            {
                dayStr = "Thứ 4";
            }
            if (day == DayOfWeek.Thursday)
            {
                dayStr = "Thứ 5";
            }
            if (day == DayOfWeek.Friday)
            {
                dayStr = "Thứ 6";
            }
            if (day == DayOfWeek.Saturday)
            {
                dayStr = "Thứ 7";
            }
            if (day == DayOfWeek.Sunday)
            {
                dayStr = "Chủ nhật";
            }
            return dayStr;
        }

        public async Task<string> TemplatePaymentRequest(long id, string profit, int type, EmailYCChiViewModel Emailmodel)
        {
            try
            {
                if (Emailmodel == null)
                {
                    var model = _paymentRequestRepository.GetById((int)id);
                    //var text = XTL.Utils.NumberToText(model.RelateData.Sum(n => n.Amount));
                    string ngay = "Ngày " + model.CreatedDate.Value.Day.ToString();
                    string thang = "Tháng " + model.CreatedDate.Value.Month.ToString();
                    string nam = "Năm " + model.CreatedDate.Value.Year.ToString();
                    string workingDirectory = AppDomain.CurrentDomain.BaseDirectory;
                    var template = workingDirectory + @"\EmailTemplate\TemplatePaymentRequest.html";
                    string body = File.ReadAllText(template);
                    if (type != 1)
                    {
                        body = body.Replace("{{title}}", "GIẤY ĐỀ NGHỊ THANH TOÁN");
                    }
                    else
                    {
                        body = body.Replace("{{title}}", "GIẤY ĐỀ NGHỊ THANH TOÁN HOÀN TRẢ KHÁCH HÀNG");

                    }
                    body = body.Replace("{{Ngay}}", ngay + "," + thang + "," + nam);
                    var data2 = await _contractPayRepository.GetContractPayByOrderId(Convert.ToInt32(model.OrderId));
                    if (data2 != null)
                    {
                        body = body.Replace("{{TTKhachTT}}", data2.Sum(s => s.AmountPay).ToString("N0"));
                    }
                    else
                    {
                        body = body.Replace("{{TTKhachTT}}", "0");
                    }

                    if (model.IsSupplierDebt == true)
                    {
                        body = body.Replace("{{CongNo}}", "Công nợ với NCC");
                    }
                    else
                    {
                        body = body.Replace("{{CongNo}}", "Không công nợ với NCC");
                    }


                    body = body.Replace("{{NguoiDn}}", model.UserCreateFullName);
                    body = body.Replace("{{BoPhan}}", model.DepartmentName);
                    body = body.Replace("{{Maycc}}", model.PaymentCode);
                    body = body.Replace("{{DoanhT}}", model.RelateData.Sum(n => n.Amount).ToString("N0"));
                    body = body.Replace("{{GhiChu}}", "<input class=\"form-control \" style=\"width:100%;\" id = 'GhiChu' value='Đã thanh toán đủ'/>");
                    body = body.Replace("{{BChuDT}}", NumberToString.So_chu((double)model.RelateData.Sum(n => n.Amount)));
                    if (type != 1)
                    {
                        body = body.Replace("{{DoanhTITLE}}", "Doanh thu");
                        body = body.Replace("{{LNDK}}", profit == null ? "0" : profit);
                        body = body.Replace("{{SoTTT}}", model.Amount.ToString("N0"));
                        body = body.Replace("{{BChuTTT}}", NumberToString.So_chu((double)model.Amount));
                        body = body.Replace("{{NCC}}", model.SupplierName);
                        body = body.Replace("{{SoTK}}", model.BankIdName != null ? model.BankIdName + '(' + model.AccountNumber + ')' : model.AccountNumber);
                        body = body.Replace("{{NganHang}}", model.BankName);
                        body = body.Replace("{{ChuTK}}", model.BankName);
                        body = body.Replace("{{Swiftcode}}", "<input class=\"form-control \" style=\"width:100%;\" id='Swiftcode' value=''/>");
                    }
                    else
                    {
                        body = body.Replace("{{style}}", "display: none");
                        body = body.Replace("{{DoanhTITLE}}", "Số tiên hoàn trả");
                        if (model.BankingAccountId != null && model.BankingAccountId != 0)
                        {
                            var BankingAccount = _bankingAccountRepository.GetById((int)model.BankingAccountId);
                            body = body.Replace("{{SoTK}}", BankingAccount.BankId + '(' + BankingAccount.AccountName + ')');
                            body = body.Replace("{{NganHang}}", BankingAccount.BankId);
                            body = body.Replace("{{ChuTK}}", BankingAccount.AccountName);
                        }
                        else
                        {
                            body = body.Replace("{{SoTK}}", "");
                            body = body.Replace("{{NganHang}}", "");
                            body = body.Replace("{{ChuTK}}", "");
                        }

                    }
                    body = body.Replace("{{MaDH}}", model.OrderNo);
                    body = body.Replace("{{MaDV}}", model.ServiceCode);
                    body = body.Replace("{{NDTT}}", model.Note);
                    body = body.Replace("{{HanTT}}", model.PaymentDate.Value.ToString("dd/MM/yyyy HH:mm"));
                    return body;
                }
                else
                {
                    var model = _paymentRequestRepository.GetById((int)id);
                    //var text = XTL.Utils.NumberToText(model.RelateData.Sum(n => n.Amount));

                    string workingDirectory = AppDomain.CurrentDomain.BaseDirectory;
                    var template = workingDirectory + @"\EmailTemplate\TemplatePaymentRequest.html";
                    string ngay = "Ngày " + model.CreatedDate.Value.Day.ToString();
                    string thang = "Tháng " + model.CreatedDate.Value.Month.ToString();
                    string nam = "Năm " + model.CreatedDate.Value.Year.ToString();
                    string body = File.ReadAllText(template);
                    body = body.Replace("{{Ngay}}", ngay + "," + thang + "," + nam);
                    body = body.Replace("{{NguoiDn}}", model.UserCreateFullName);
                    body = body.Replace("{{BoPhan}}", model.DepartmentName);
                    body = body.Replace("{{Maycc}}", model.PaymentCode);
                    body = body.Replace("{{DoanhT}}", model.RelateData.Sum(n => n.Amount).ToString("N0"));
                    body = body.Replace("{{GhiChu}}", Emailmodel.GhiChu);
                    body = body.Replace("{{BChuDT}}", NumberToString.So_chu((double)model.RelateData.Sum(n => n.Amount)));
                    if (type != 1)
                    {
                        body = body.Replace("{{DoanhTITLE}}", "Doanh thu");
                        body = body.Replace("{{title}}", "GIẤY ĐỀ NGHỊ THANH TOÁN");
                        body = body.Replace("{{LNDK}}", profit == null ? "0" : profit);
                        body = body.Replace("{{SoTTT}}", model.Amount.ToString("N0"));
                        body = body.Replace("{{BChuTTT}}", NumberToString.So_chu((double)model.Amount));

                        body = body.Replace("{{NCC}}", model.SupplierName);
                        body = body.Replace("{{Swiftcode}}", Emailmodel.Swiftcode);
                    }
                    else
                    {
                        body = body.Replace("{{style}}", "display: none");
                        body = body.Replace("{{DoanhTITLE}}", "Số tiên hoàn trả");
                        body = body.Replace("{{title}}", "GIẤY ĐỀ NGHỊ THANH TOÁN HOÀN TRẢ KHÁCH HÀNG");

                    }
                    var data2 = await _contractPayRepository.GetContractPayByOrderId(Convert.ToInt32(model.OrderId));
                    if (data2 != null)
                    {
                        body = body.Replace("{{TTKhachTT}}", data2.Sum(s => s.AmountPay).ToString("N0"));
                    }
                    else
                    {
                        body = body.Replace("{{TTKhachTT}}", "0");
                    }
                    if (model.IsSupplierDebt == true)
                    {
                        body = body.Replace("{{CongNo}}", "Công nợ với NCC");
                    }
                    else
                    {
                        body = body.Replace("{{CongNo}}", "Không công nợ với NCC");
                    }
                    body = body.Replace("{{SoTK}}", model.BankIdName != null ? model.BankIdName + '(' + model.AccountNumber + ')' : model.AccountNumber);
                    body = body.Replace("{{NganHang}}", model.BankName);
                    body = body.Replace("{{ChuTK}}", model.BankName);
                    body = body.Replace("{{NDTT}}", model.Note);
                    body = body.Replace("{{HanTT}}", model.PaymentDate.Value.ToString("dd/MM/yyyy HH:mm"));
                    body = body.Replace("{{MaDH}}", model.OrderNo);
                    body = body.Replace("{{MaDV}}", model.ServiceCode);

                    return body;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("TemplatePaymentRequest - MailService: " + ex.ToString());
                return null;
            }
        }
        public async Task<string> PathAttachmentVeVinWonder(VinWonderConfirmBookingOutputDataDataTickets Url)
        {
            try
            {
                List<string> listPathAttachment = new List<string>();
                string workingDirectory = System.AppDomain.CurrentDomain.BaseDirectory;

                //save file to server and get link attackment
                var path = workingDirectory + @"\FileAttackment\";

                string pathVeVinWonderPng = path + Url.ServiceName + ".png";
                string pathVeVinWonderQrCodePng = path + Url.ServiceName + "QrCode.png";
                try
                {
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);
                }
                catch (Exception ex)
                {
                }
                try
                {
                    if (!File.Exists(pathVeVinWonderPng))
                        File.Delete(pathVeVinWonderPng);
                }
                catch (Exception ex)
                {
                }
                try
                {
                    if (!File.Exists(pathVeVinWonderQrCodePng))
                        File.Delete(pathVeVinWonderQrCodePng);
                }
                catch (Exception ex)
                {
                }

                var _httpclient = new HttpClient();
                var response = await _httpclient.GetAsync(Url.QrCodeUrl);

                string type = string.Empty;
                string ImgNL = "https://static-image.adavigo.com/uploads/images/email/NL.jpg";
                string ImgNCT = "https://static-image.adavigo.com/uploads/images/email/NCT.jpg";
                string ImgTE = "https://static-image.adavigo.com/uploads/images/email/TE.jpg";
                string url = Url.QrCodeUrl;
                string[] urls = url.Split("&");
                foreach (var item in urls)
                {
                    if (item.Contains("typeCode="))
                    {
                        type = item.ToString().Substring(9);
                    }
                }



                if (response.IsSuccessStatusCode)
                {
                    string htmlContent = await response.Content.ReadAsStringAsync();
                    string domain = "https://qr.vinwonders.com/images";
                    var html = htmlContent.Replace("./images", domain);
                    await ConvertHtmlToImage(html, pathVeVinWonderPng);
                    return pathVeVinWonderPng;
                }
                else
                {
                    return null;

                }

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("PathAttachmentVeVinWonder - MailService: " + ex.ToString());
                return null;
            }
        }
        public async Task ConvertHtmlToImage(string htmlContent, string outputPath)
        {
            try
            {
                var options = new LaunchOptions
                {
                    Headless = true,
                    Args = new string[]
                    {
                        "--no-sandbox",
                        "--disable-dev-shm-usage",
                        "--incognito"
                    },
                    ExecutablePath = ReadFile.LoadConfig().ChromeLocalPath
                };
                using (var browser = await Puppeteer.LaunchAsync(options))
                {
                    IPage page = await browser.NewPageAsync();

                    await page.SetViewportAsync(new ViewPortOptions
                    {
                        Width = 1050, // Đặt kích thước rộng (chiều ngang) tùy ý
                        Height = 1860// Đặt kích thước cao (chiều dọc) tùy ý
                    });
                    await page.SetContentAsync(htmlContent);
                    // Tạo tùy chọn cho chụp ảnh
                    var screenshotOptions = new ScreenshotOptions
                    {
                        FullPage = true, // Chụp toàn bộ nội dung trang
                        Clip = null, // Không cắt bất kỳ phần nào
                        OmitBackground = false
                    };
                    // Chụp ảnh với tùy chọn đã đặt
                    await page.ScreenshotAsync(outputPath, screenshotOptions);

                    await page.CloseAsync();
                    await browser.CloseAsync();
                    browser.Disconnect();
                    await browser.DisposeAsync();

                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public async Task<string> UploadImageBase64(string base64, string extension, string file_path)
        {
            string ImagePath = string.Empty;
            string tokenData = string.Empty;
            try
            {

                var j_param = new Dictionary<string, string> {
                    { "data_file",base64 },
                    { "extend", extension }};
                string url = ReadFile.LoadConfig().STATIC_IMAGE_DOMAIN;
                using (HttpClient httpClient = new HttpClient())
                {
                    tokenData = CommonHelper.Encode(JsonConvert.SerializeObject(j_param), ReadFile.LoadConfig().API_IMAGE_KEY);
                    var contentObj = new { token = tokenData };
                    var content = new StringContent(JsonConvert.SerializeObject(contentObj), Encoding.UTF8, "application/json");
                    var result = await httpClient.PostAsync(url, content);
                    dynamic resultContent = Newtonsoft.Json.Linq.JObject.Parse(result.Content.ReadAsStringAsync().Result);
                    if (resultContent.status == 0)
                    {
                        return ReadFile.LoadConfig().IMAGE_DOMAIN + resultContent.url_path;
                    }
                    else
                    {
                        LogHelper.InsertLogTelegram("UploadImageBase64. Result: " + resultContent.status + ". Message: " + resultContent.msg);
                    }
                    try
                    {
                        File.Delete(file_path);
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UploadImageBase64 - " + ex.Message.ToString() + " Token:" + tokenData);
            }
            return ImagePath;
        }
        public async Task<bool> SendEmailVinwonderTicket(long orderid, long booking_id, string subject, List<string> to_email, List<string> cc_email, List<string> bcc_email)
        {
            bool ressult = true;
            string path = Directory.GetCurrentDirectory() + @"\Attachments\" + DateTime.Now.ToString("ddMMyyyyHHmmssff") + @"\";

            try
            {

                var order = await _orderRepository.GetOrderByID(orderid);
                MailMessage message = new MailMessage();
                if (string.IsNullOrEmpty(subject))
                    subject = "[Adavigo] Vé Điện Tử Vinwonder Của Quý Khách - Mã Đơn hàng " + order.OrderNo;
                message.Subject = subject;
                //config send email
                string from_mail = new ConfigurationBuilder().AddJsonFile("appsettings.json")
                    .Build().GetSection("MAIL_CONFIG")["FROM_MAIL"];
                string account = new ConfigurationBuilder().AddJsonFile("appsettings.json")
                    .Build().GetSection("MAIL_CONFIG")["USERNAME"];
                string password = new ConfigurationBuilder().AddJsonFile("appsettings.json")
                    .Build().GetSection("MAIL_CONFIG")["PASSWORD"];
                string host = new ConfigurationBuilder().AddJsonFile("appsettings.json")
                    .Build().GetSection("MAIL_CONFIG")["HOST"];
                string port = new ConfigurationBuilder().AddJsonFile("appsettings.json")
                    .Build().GetSection("MAIL_CONFIG")["PORT"];
                message.IsBodyHtml = true;
                message.From = new MailAddress(from_mail);
                message.Body = await GetTemplateVinWordbookingTC(orderid);
                string sendEmailsFrom = account;
                string sendEmailsFromPassword = password;
                SmtpClient smtp = new SmtpClient(host, Convert.ToInt32(port));
                smtp.EnableSsl = true;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Credentials = new NetworkCredential(sendEmailsFrom, sendEmailsFromPassword);
                smtp.Timeout = 200000;
                if (to_email != null && to_email.Count > 0)
                {
                    foreach (var to in to_email)
                    {
                        message.To.Add(to);
                    }
                }
                if (cc_email != null && cc_email.Count > 0)
                {
                    foreach (var cc in cc_email)
                    {
                        message.To.Add(cc);
                    }
                }
                if (bcc_email != null && bcc_email.Count > 0)
                {
                    foreach (var bcc in bcc_email)
                    {
                        message.To.Add(bcc);
                    }
                }
                var attachments = await _AttachFileRepository.GetListByDataID(booking_id, (int)AttachmentType.Email_VinWonder_Ticket);
                if (attachments != null || attachments.Count > 0)
                {
                    var client = new WebClient();
                    try
                    {
                        if (!Directory.Exists(Directory.GetCurrentDirectory() + @"\Attachments\"))
                        {
                            Directory.CreateDirectory(Directory.GetCurrentDirectory() + @"\Attachments\");
                        }

                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                    }
                    catch { }
                    foreach (var attach in attachments)
                    {
                        var url_split = attach.Path.Split("/");
                        client.DownloadFile(attach.Path, path + url_split[url_split.Length - 1]);
                        message.Attachments.Add(new Attachment(path + url_split[url_split.Length - 1]));
                    }

                }


                smtp.Send(message);
                GC.Collect();
                message.Attachments.Dispose();
                message.Dispose();

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("sendMailVinWordbookingTC - Base.MailService: " + ex);
                ressult = false;
            }
            try
            {
                System.IO.DirectoryInfo di = new DirectoryInfo(path);
                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
                foreach (DirectoryInfo dir in di.GetDirectories())
                {
                    dir.Delete(true);
                }
            }
            catch { }
            return ressult;
        }
        public async Task<string> GetTemplateVinWordbookingTC(long orderid)
        {
            try
            {
                var order = await _orderRepository.GetOrderByID(orderid);



                string workingDirectory = AppDomain.CurrentDomain.BaseDirectory;
                var template = workingDirectory + @"EmailTemplate\VinWonder\VinWonderMailTemplate.html";
                string body = File.ReadAllText(template);


                if (order != null)
                {
                    body = body.Replace("{{orderNo}}", order.OrderNo);
                    body = body.Replace("{{orderDate}}", ((DateTime)order.CreateTime).ToString("dd/MM/yyyy"));
                    if (order.ContactClientId == null)
                    {
                        body = body.Replace("{{customerName}}", "");
                        body = body.Replace("{{phone}}", "");
                        body = body.Replace("{{email}}", "");
                    }
                    else
                    {
                        var control = _contactClientRepository.GetByContactClientId((long)order.ContactClientId);
                        if (control != null)
                        {
                            body = body.Replace("{{customerName}}", control.Name);
                            body = body.Replace("{{phone}}", control.Mobile);
                            body = body.Replace("{{email}}", control.Email);
                        }
                        else
                        {
                            body = body.Replace("{{customerName}}", "");
                            body = body.Replace("{{phone}}", "");
                            body = body.Replace("{{email}}", "");
                        }
                    }

                }
                else
                {
                    body = body.Replace("{{orderNo}}", "");
                    body = body.Replace("{{orderDate}}", "");

                }

                var vinWonder = await _vinWonderBookingRepository.GetVinWonderBookingEmailByOrderID(orderid);
                var VinWonderBooking = await _vinWonderBookingRepository.GetVinWonderBookingByOrderId(orderid);


                if (vinWonder != null && VinWonderBooking != null)
                {
                    string vinWonderdetai = string.Empty;
                    var VinWonderBookingTicket = await _vinWonderBookingRepository.GetVinWonderBookingTicketByBookingID(VinWonderBooking[0].Id);
                    foreach (var item in VinWonderBookingTicket)
                    {
                        var data = vinWonder.Where(s => s.BookingTicketId == item.Id && s.BookingId == item.BookingId);
                        string datataleCT = string.Empty;
                        foreach (var item2 in data)
                        {
                            if (item2.typeCode == VinWonderTypeCode.NL)
                            {
                                datataleCT += "<div> " + item2.adt + " x " + item2.Name + "</div>";
                            }
                            if (item2.typeCode == VinWonderTypeCode.TE)
                            {
                                datataleCT += "<div> " + item2.child + " x " + item2.Name + "</div >";
                            }
                            if (item2.typeCode == VinWonderTypeCode.NCT)
                            {
                                datataleCT += "<div> " + item2.old + " x " + item2.Name + "</div>";
                            }


                        }
                        vinWonderdetai += "<tr>" +
                        "<td><strong>" + VinWonderBooking[0].SiteName + "</strong></td>" +
                        "<td>" + datataleCT + "</td>" +
                       "<td> " + ((DateTime)item.DateUsed).ToString("dd/MM/yyyy") + "</ td ></tr>";

                        vinWonderdetai += "<tr> <td colspan=\"4\"> <div style=\"border - bottom:1px solid #E3EBF3;margin: 0 0 10px 0\"></div> </td> </tr>";
                    }
                    body = body.Replace("{{vinWonderTable}}", vinWonderdetai);


                }
                body = body.Replace("{{vinWonderTable}}", "");

                return body;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetTemplateVinWordbookingTC - MailService: " + ex.ToString());
                return null;
            }
        }
        public async Task<List<string>> ListPathAttachmentVinWordbooking(long orderid)
        {
            List<string> listPathAttachment = new List<string>();
            var VinWonderBookingTicketCustomer = new List<VinWonderBookingTicketCustomer>();
            string workingDirectory = System.AppDomain.CurrentDomain.BaseDirectory;

            var templatePaymentReceipt = workingDirectory + @"MailTemplate\VinWonder\VinWonderPaymentReceiptTemplate.html";
            var htmlTemplatePaymentReceipt = File.ReadAllText(templatePaymentReceipt);

            var order = await _orderRepository.GetOrderByID(orderid);
            var contactClient = _contactClientRepository.GetByContactClientId((long)order.ContactClientId);
            var VinWonderBooking = _vinWonderBookingRepository.GetVinWonderBookingByOrderId(orderid).Result;
            if (VinWonderBooking != null)
                VinWonderBookingTicketCustomer = _vinWonderBookingRepository.GetVinWondeCustomerByBookingId(VinWonderBooking[0].Id).Result;
            var vinWonder = _vinWonderBookingRepository.GetVinWonderBookingEmailByOrderID(orderid).Result;
            var ContractPay = _contractPayRepository.GetContractPayByOrderId(orderid).Result;
            //save file to server and get link attackment
            var path = workingDirectory + @"\FileAttachment\";

            string pathPaymentReceiptPdf = path + "PaymentReceiptVinWonderPdf " + order.OrderNo + ".pdf";
            try
            {
                if (Directory.Exists(path))
                    Directory.Delete(path, true);

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
            }
            catch (Exception ex)
            {
            }
            try
            {
                if (!File.Exists(pathPaymentReceiptPdf))
                    File.Delete(pathPaymentReceiptPdf);
            }
            catch (Exception ex)
            {
            }

            htmlTemplatePaymentReceipt = ReplaceTemplatePaymentReceiptVinWordbooking(htmlTemplatePaymentReceipt, order, contactClient, VinWonderBookingTicketCustomer, VinWonderBooking, vinWonder, ContractPay);


            var byteFilePaymentReceipt = EmailHelper.PdfSharpConvert(htmlTemplatePaymentReceipt);


            if (byteFilePaymentReceipt != null)
                /* File.WriteAllBytes(pathPaymentReceiptPdf, byteFilePaymentReceipt);*/
                EmailHelper.ByteArrayToFile(pathPaymentReceiptPdf, byteFilePaymentReceipt);


            listPathAttachment.Add(pathPaymentReceiptPdf);

            return listPathAttachment;
        }
        public string ReplaceTemplatePaymentReceiptVinWordbooking(string htmlTemplate, Order orderInfo, ContactClient contactClient,
            List<VinWonderBookingTicketCustomer> VinWonderBookingTicketCustomer, List<VinWonderBooking> VinWonderBooking, List<ListVinWonderemialViewModel> vinWonder, List<ContractPayDetaiByOrderIdlViewModel> ContractPay)
        {
            var sumProfit = VinWonderBooking.Sum(s => s.TotalProfit);
            htmlTemplate = htmlTemplate.Replace("{{orderNo}}", orderInfo.OrderNo);
            if (ContractPay != null)
            {
                htmlTemplate = htmlTemplate.Replace("{{numberOfPayment}}", ContractPay[0].BillNo);
                htmlTemplate = htmlTemplate.Replace("{{timePayment}}", ContractPay[0].ExportDate != null ? (Convert.ToDateTime(ContractPay[0].ExportDate)).ToString("dd/MM/yyyy") : "");

            }
            else
            {
                htmlTemplate = htmlTemplate.Replace("{{numberOfPayment}}", "");
                htmlTemplate = htmlTemplate.Replace("{{timePayment}}", "");
            }

            if (contactClient != null)
            {
                htmlTemplate = htmlTemplate.Replace("{{customerName}}", contactClient.Name);
                htmlTemplate = htmlTemplate.Replace("{{email}}", contactClient.Email);
                htmlTemplate = htmlTemplate.Replace("{{phone}}", contactClient.Mobile);
            }

            htmlTemplate = htmlTemplate.Replace("{{paymentStatus}}", "Đã thanh toán");
            string paymentType = string.Empty;
            switch (orderInfo.PaymentType)
            {
                case (int)PaymentType.ATM:
                    paymentType = "ATM";
                    break;
                case (int)PaymentType.CHUYEN_KHOAN_TRUC_TIEP:
                    paymentType = "Chuyển khoản ngân hàng";
                    break;
                case (int)PaymentType.VISA_MASTER_CARD:
                    paymentType = "Thẻ VISA/Master Card";
                    break;
                case (int)PaymentType.QR_PAY:
                    paymentType = "Thanh toán QR/PAY";
                    break;
                case (int)PaymentType.KY_QUY:
                    paymentType = "Thanh toán bằng ký quỹ";
                    break;
                case (int)PaymentType.GIU_CHO:
                    paymentType = "Giữ chỗ";
                    break;
                case (int)PaymentType.TAI_VAN_PHONG:
                    paymentType = "Thanh toán tại văn phòng";
                    break;
            }
            htmlTemplate = htmlTemplate.Replace("{{paymentMethod}}", paymentType);
            htmlTemplate = htmlTemplate.Replace("{{total}}", orderInfo.Amount != null ? ((double)orderInfo.Amount).ToString("N0") : "0");
            htmlTemplate = htmlTemplate.Replace("{{fee}}", ((double)sumProfit).ToString("N0"));
            htmlTemplate = htmlTemplate.Replace("{{amount}}", orderInfo.Amount != null ? ((double)orderInfo.Amount).ToString("N0") : "0");

            string passenger = String.Empty;
            var count = 1;
            if (VinWonderBookingTicketCustomer != null)
            {
                foreach (var item in VinWonderBookingTicketCustomer)
                {
                    string personType = string.Empty;
                    string firstName = string.Empty;

                    passenger += "<tr>" +
                        "<td>" + count + "</td>" +
                        "<td>" + item.FullName + "</td>" +
                        "<td>" + ((DateTime)item.Birthday).ToString("dd/MM/yyyy") + "</td>" +
                        "<td>" + item.Phone + "</td>" +
                        "<td>" + item.Email + "</td>" +
                        "</tr>";
                    count++;
                }
                htmlTemplate = htmlTemplate.Replace("{{passengerList}}", passenger);
            }
            else
            {
                htmlTemplate = htmlTemplate.Replace("{{passengerList}}", "");
            }

            string passengerItem = String.Empty;
            count = 1;

            if (vinWonder != null && VinWonderBooking != null)
            {
                string vinWonderdetai = string.Empty;
                var VinWonderBookingTicket = _vinWonderBookingRepository.GetVinWonderBookingTicketByBookingID(VinWonderBooking[0].Id).Result;
                foreach (var item in VinWonderBookingTicket)
                {
                    var data = vinWonder.Where(s => s.BookingTicketId == item.Id && s.BookingId == item.BookingId);
                    string datataleCT = string.Empty;
                    foreach (var item2 in data)
                    {
                        if (item2.typeCode == VinWonderTypeCode.NL)
                        {
                            datataleCT += "<div> " + item2.adt + " x " + item2.Name + "</div>";
                        }
                        if (item2.typeCode == VinWonderTypeCode.TE)
                        {
                            datataleCT += "<div> " + item2.child + " x " + item2.Name + "</div>";
                        }
                        if (item2.typeCode == VinWonderTypeCode.NCT)
                        {
                            datataleCT += "<div> " + item2.old + " x " + item2.Name + "</div>";
                        }


                    }
                    vinWonderdetai += "<tr>" +
                    "<td >" + count + "</td>" +
                    "<td><strong>" + VinWonderBooking[0].SiteName + "</strong></td>" +
                    "<td colspan='2'>" + datataleCT + "</td>" +
                   "<td> " + ((DateTime)item.DateUsed).ToString("dd/MM/yyyy HH:mm:ss") + "</ td >" +
                   "<td> " + ((double)VinWonderBooking[0].Amount).ToString("N0") + "</ td >" +
                   "</tr>";
                    count++;
                }

                htmlTemplate = htmlTemplate.Replace("{{itemList}}", vinWonderdetai);

                return htmlTemplate;
            }
            return htmlTemplate;
        }
        public async Task<string> PathAttachmentVeVinWonder(string Url)
        {
            try
            {
                List<string> listPathAttachment = new List<string>();
                string workingDirectory = System.AppDomain.CurrentDomain.BaseDirectory;

                //save file to server and get link attackment
                var path = workingDirectory + @"\FileAttackment\";
                string file_name = Guid.NewGuid() + ".png";
                string pathVeVinWonderPng = path + file_name;

                try
                {
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);
                }
                catch (Exception ex)
                {
                }
                try
                {
                    if (!File.Exists(pathVeVinWonderPng))
                        File.Delete(pathVeVinWonderPng);
                }
                catch (Exception ex)
                {
                }


                var _httpclient = new HttpClient();
                var response = await _httpclient.GetAsync(Url);

                if (response.IsSuccessStatusCode)
                {

                    using (WebClient client = new WebClient())
                    {
                        client.DownloadFile(new Uri(Url), pathVeVinWonderPng);
                        client.Dispose();
                    }
                    response.Dispose();
                    return pathVeVinWonderPng;
                }
                else
                {

                    return null;

                }

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("PathAttachmentVeVinWonder - MailService: " + ex.ToString());
                return null;
            }
        }



    }
}
