using Caching.RedisWorker;
using Entities.ConfigModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.Features;
using Repositories;
using Repositories.IRepositories;
using Repositories.Repositories;
using GDHT.CMS.Service;
using GDHT.CMS.Service.ServiceInterface;
using WEB.CMS.Customize;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession();
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 1024 * 1024 * 100; // Limit upload size to 100MB
});
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
    options.AccessDeniedPath = new PathString("/Account/RedirectLogin");
    options.LoginPath = new PathString("/Account/RedirectLogin");
    options.ReturnUrlParameter = "url";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60); // nếu dùng ExpireTimeSpan thì  SlidingExpiration phải set là false. Như vậy cho dù tương tác hay k tương tác thì đều timeout theo thời gian đã set
    options.SlidingExpiration = true; //được sử dụng để thiết lập thời gian sống của cookie dựa trên thời gian cuối cùng mà người dùng đã tương tác với ứng dụng . Nếu người dùng tiếp tục tương tác với ứng dụng trước khi cookie hết hạn, thời gian sống của cookie sẽ được gia hạn thêm.

    options.Cookie = new CookieBuilder
    {
        HttpOnly = true,
        Name = "Net.Security.Cookie",
        Path = "/",
        SameSite = SameSiteMode.Lax,
        SecurePolicy = CookieSecurePolicy.SameAsRequest
    };

});
ConfigurationManager configuration = builder.Configuration; // allows both to access and to set up the config

// Get config to instance model
builder.Services.Configure<DataBaseConfig>(configuration.GetSection("DataBaseConfig"));
builder.Services.Configure<MailConfig>(configuration.GetSection("MailConfig"));
builder.Services.Configure<DomainConfig>(configuration.GetSection("DomainConfig"));

// Register services
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddSingleton<IAllCodeRepository, AllCodeRepository>();
builder.Services.AddSingleton<ICommonRepository, CommonRepository>();
builder.Services.AddSingleton<IMenuRepository, MenuRepository>();
builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddTransient<IRoleRepository, RoleRepository>();
builder.Services.AddTransient<IPermissionRepository, PermissionRepository>();
builder.Services.AddTransient<ILabelRepository, LabelRepository>();
builder.Services.AddTransient<IPositionRepository, PositionRepository>();
builder.Services.AddTransient<INoteRepository, NoteRepository>();
builder.Services.AddTransient<IAllCodeRepository, AllCodeRepository>();
builder.Services.AddTransient<IAttachFileRepository, AttachFileRepository>();

builder.Services.AddTransient<ICashbackRepository, CashbackRepository>();
builder.Services.AddTransient<IPaymentRepository, PaymentRepository>();
builder.Services.AddTransient<IArticleRepository, ArticleRepository>();
builder.Services.AddTransient<IProvinceRepository, ProvinceRepository>();
builder.Services.AddTransient<IDistrictRepository, DistrictRepository>();
builder.Services.AddTransient<IWardRepository, WardRepository>();
builder.Services.AddTransient<IMFARepository, MFARepository>();
builder.Services.AddTransient<IOrderRepositor, OrderRepositor>();

builder.Services.AddTransient<ICampaignRepository, CampaignRepository>();
builder.Services.AddTransient<IPriceDetailRepository, PriceDetailRepository>();
builder.Services.AddTransient<IGroupProductRepository, GroupProductRepository>();
builder.Services.AddTransient<IProductRoomServiceRepository, ProductRoomServiceRepository>();
builder.Services.AddTransient<IProductFlyTicketServiceRepository, ProductFlyTicketServiceRepository>();
builder.Services.AddTransient<IServicePriceRoomRepository, ServicePriceRoomRepository>();
builder.Services.AddSingleton<IRoomFunRepository, RoomFunRepository>();

builder.Services.AddTransient<ITelegramRepository, TelegramRepository>();
builder.Services.AddTransient<IClientRepository, ClientRepository>();
builder.Services.AddTransient<IContractPayRepository, ContractPayRepository>();
builder.Services.AddTransient<IOrderRepository, OrderRepository>();
builder.Services.AddTransient<IBagageRepository, BagageRepository>();
builder.Services.AddTransient<IFlightSegmentRepository, FlightSegmentRepository>();
builder.Services.AddTransient<IFlyBookingDetailRepository, FlyBookingDetailRepository>();
builder.Services.AddTransient<IDepositHistoryRepository, DepositHistoryRepository>();
builder.Services.AddTransient<ICustomerManagerRepository, CustomerManagerRepository>();
builder.Services.AddTransient<IPaymentAccountRepository, PaymentAccountRepository>();
builder.Services.AddTransient<IContractRepository, ContractRepository>();
builder.Services.AddTransient<IPolicyRepository, PolicyRepository>();
builder.Services.AddTransient<IIdentifierServiceRepository, IdentifierServiceRepository>();
builder.Services.AddTransient<IAccountClientRepository, AccountClientRepository>();

builder.Services.AddTransient<IHotelBookingRepositories, HotelBookingRepositories>();
builder.Services.AddTransient<IHotelBookingRoomRepository, HotelBookingRoomsRepository>();
builder.Services.AddTransient<IHotelBookingRoomRatesRepository, HotelBookingRoomRatesRepository>();
builder.Services.AddTransient<IHotelBookingRoomExtraPackageRepository, HotelBookingRoomExtraPackageRepository>();
builder.Services.AddTransient<IHotelBookingGuestRepository, HotelBookingGuestRepository>();
builder.Services.AddTransient<IContactClientRepository, ContactClientRepository>();
builder.Services.AddTransient<IBankingAccountRepository, BankingAccountRepository>();
builder.Services.AddTransient<IUserAgentRepository, UserAgentRepository>();

builder.Services.AddTransient<IAirlinesRepository, AirlinesRepository>();
builder.Services.AddTransient<IPassengerRepository, PassengerRepository>();
builder.Services.AddTransient<ITourRepository, TourRepository>();
builder.Services.AddTransient<ISupplierRepository, SupplierRepository>();
builder.Services.AddTransient<INationalRepository, NationalRepository>();
builder.Services.AddTransient<IPaymentRequestRepository, PaymentRequestRepository>();
builder.Services.AddTransient<IPaymentVoucherRepository, PaymentVoucherRepository>();
builder.Services.AddTransient<IHotelBookingCodeRepository, HotelBookingCodeRepository>();
builder.Services.AddTransient<IBrandRepository, BrandRepository>();
builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddTransient<IDepartmentRepository, DepartmentRepository>();
builder.Services.AddTransient<IDashboardRepository, DashboardRepository>();
builder.Services.AddTransient<ITourPackagesOptionalRepository, TourPackagesOptionalRepository>();
builder.Services.AddTransient<IPlaygroundDetaiRepository, PlaygroundDetaiRepository>();
builder.Services.AddTransient<IInvoiceRequestRepository, InvoiceRequestRepository>();
builder.Services.AddTransient<IInvoiceRepository, InvoiceRepository>();
builder.Services.AddTransient<IOtherBookingRepository, OtherBookingRepository>();
builder.Services.AddTransient<IVinWonderBookingRepository, VinWonderBookingRepository>();
builder.Services.AddTransient<IReportRepository, ReportRepository>();
builder.Services.AddTransient<IProgramsPackageReprository, ProgramsPackageReprository>();
builder.Services.AddTransient<IProgramsReprository, ProgramsReprository>();
builder.Services.AddTransient<IHotelRepository, HotelRepository>();
builder.Services.AddTransient<IDebtStatisticRepository, DebtStatisticRepository>();
builder.Services.AddTransient<IRequestRepository, RequestRepository>();
// Setting Redis                     
builder.Services.AddSingleton<RedisConn>();
builder.Services.AddSingleton<ManagementUser>();
var app = builder.Build();
if (!app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error/Index");
    app.UseHsts();
}

app.UseSession();

app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();
// app.UseAntiXssMiddleware();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "setupManual",
                 pattern: "/product/setup-manual",
                 defaults: new { controller = "product", action = "SetupManual" });
app.MapControllerRoute(name: "transactionsms",
  pattern: "/transactionsms",
  defaults: new { controller = "TransactionSms", action = "Index" });
app.MapControllerRoute(name: "Order",
 pattern: "/Order/{id?}",
 defaults: new { controller = "Order", action = "Orderdetails" });
app.MapControllerRoute(name: "SetService",
 pattern: "SetService/fly/detail/{group_booking_id}",
 defaults: new { controller = "SetService", action = "FlyDetail" });
app.MapControllerRoute(name: "SetService",
 pattern: "SetService/Tour/Detail/{id}",
 defaults: new { controller = "SetService", action = "TourDetail" });
app.MapControllerRoute(name: "SetService",
pattern: "SetService/Others/Detail/{id}",
defaults: new { controller = "SetService", action = "OtherDetail" });
app.MapControllerRoute(name: "SetService",
pattern: "SetService/VinWonder/Detail/{id}",
defaults: new { controller = "SetService", action = "VinWonderDetail" });


app.MapControllerRoute(name: "AccountSetup",
pattern: "/Account/2FA",
defaults: new { controller = "Account", action = "Setup2FA" });
app.MapControllerRoute(name: "ProgramsPackage",
pattern: "/ProgramsPackage/DetailListProgramsPackage/{id}/{Packageid}/{ProgramName}/{RoomTypeid}",
defaults: new { controller = "ProgramsPackage", action = "DetailListProgramsPackage" });
app.MapControllerRoute(name: "ProgramsPackage",
pattern: "/ProgramsPackage/AddListProgramsPackage/{id}/{Packageid}/{ProgramName}/{RoomTypeid}/{type}",
defaults: new { controller = "ProgramsPackage", action = "AddListProgramsPackage" });
app.MapControllerRoute(name: "ProgramsPackage",
pattern: "/ProgramsPackage/ProgramsPriceHotelIndex",
defaults: new { controller = "ProgramsPackage", action = "ProgramsPriceHotelIndex" });

app.MapControllerRoute(name: "RequestHotelBooking",
pattern: "/RequestHotelBooking/Detail/{hotel_booking_id}/{ClientId}/{id}",
defaults: new { controller = "RequestHotelBooking", action = "Detail" });

app.Run();
