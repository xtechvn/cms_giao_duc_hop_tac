
using Caching.Elasticsearch;
using Caching.RedisWorker;
using Entities.Models;
using Entities.ViewModels;
using Entities.ViewModels.ElasticSearch;
using Entities.ViewModels.Funding;
using Entities.ViewModels.Hotel;
using Entities.ViewModels.SupplierConfig;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml;
using Repositories.IRepositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;
using WEB.CMS.Customize;

namespace GDHT.CMS.Controllers
{
    [CustomAuthorize]
    public class HotelController : Controller
    {

        private readonly IPolicyRepository _PolicyRepository;
        private readonly IAllCodeRepository _AllCodeRepository;
        private readonly ICampaignRepository _CampaignRepository;
        private readonly IHotelBookingRepositories _hotelBookingRepositories;
        private readonly ISupplierRepository _supplierRepository;
        private readonly ICommonRepository _commonRepository;
        private readonly IBrandRepository _brandRepository;
        private readonly IHotelRepository _HotelRepository;
        private readonly ProgramsESRepository _programsESRepository;
        private readonly IConfiguration _configuration;
        private readonly RedisConn _redisService;
        public HotelController(IPolicyRepository policyRepository, IAllCodeRepository allCodeRepository,
        ISupplierRepository supplierRepository, ICampaignRepository campaignRepository, IBrandRepository brandRepository,
        IHotelBookingRepositories hotelBookingRepositories, ICommonRepository commonRepository, IHotelRepository hotelRepository, IConfiguration configuration)
        {
            _configuration = configuration;
            _PolicyRepository = policyRepository;
            _AllCodeRepository = allCodeRepository;
            _CampaignRepository = campaignRepository;
            _supplierRepository = supplierRepository;
            _hotelBookingRepositories = hotelBookingRepositories;
            _commonRepository = commonRepository;
            _brandRepository = brandRepository;
            _HotelRepository = hotelRepository;
            _programsESRepository = new ProgramsESRepository(configuration["DataBaseConfig:Elastic:Host"]);
            _redisService = new RedisConn(configuration);
            _redisService.Connect();
        }

        #region hotel management
        public async Task<IActionResult> Index()
        {
            ViewBag.Provinces = await _commonRepository.GetProvinceList();
            ViewBag.Brands = await _brandRepository.GetAll();
            return View();
        }

        [HttpPost]
        public IActionResult Search(HotelFilterModel searchModel)
        {
            var model = new GenericViewModel<HotelGridModel>();
            try
            {
                ViewBag.TypeFilter = searchModel.TypeFilter;
                var datas = _HotelRepository.GetHotelPagingList(searchModel);
                model.CurrentPage = searchModel.PageIndex;
                model.ListData = datas.ToList();
                model.PageSize = searchModel.PageSize;
                model.TotalRecord = datas != null && datas.Any() ? datas.First().TotalRow : 0;
                model.TotalPage = (int)Math.Ceiling((double)model.TotalRecord / searchModel.PageSize);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("Search - HotelController: " + ex);
            }
            return PartialView(model);
        }


        public async Task<IActionResult> AddOrUpdate(int id)
        {
            var model = new HotelUpsertViewModel()
            {
                IsDisplayWebsite = false
            };

            if (id > 0)
            {
                var hotelPosition =  _HotelRepository.GetListHotelPosition(id);
                
                var hotePositionB2B = hotelPosition != null && hotelPosition.Count > 0 ? hotelPosition.Where(S => S.PositionType == Utilities.Contants.PositionType.B2B).ToList() : null;
                var hotePositionB2C = hotelPosition != null && hotelPosition.Count > 0 ? hotelPosition.Where(S => S.PositionType == Utilities.Contants.PositionType.B2C).ToList() : null;
                var hotel = _HotelRepository.GetHotelById(id);
                //var HotelPosition = _HotelRepository.GetListHotelPosition(id);

                if (hotel != null)
                {
                    model = new HotelUpsertViewModel()
                    {
                        Id = hotel.Id,
                        Name = hotel.Name,
                        ShortName = hotel.ShortName,
                        Description = hotel.Description,
                        SupplierId = hotel.SupplierId,
                        RatingStar = hotel.RatingStar,
                        VerifyDate = hotel.VerifyDate,
                        ProvinceId = hotel.ProvinceId,
                        Street = hotel.Street,
                        ChainBrands = hotel.ChainBrands,
                        Email = hotel.Email,
                        EstablishedYear = hotel.EstablishedYear,
                        Telephone = hotel.Telephone,
                        TaxCode = hotel.TaxCode,
                        HotelId = hotel.HotelId,
                        HotelType = hotel.HotelType,
                        SalerId = hotel.SalerId,
                        IsDisplayWebsite = hotel.IsDisplayWebsite ?? false,
                        ImageThumb = hotel.ImageThumb,
                        City = hotel.City,
                        IsCommitFund = hotel.IsCommitFund,
                        Star = hotel.Star,
                        PositionB2B = hotePositionB2B != null && hotePositionB2B.Count > 0 ? (int)hotePositionB2B[0].Position : 0,
                        PositionB2C = hotePositionB2C != null && hotePositionB2C.Count > 0 ? (int)hotePositionB2C[0].Position : 0,
                    };
                }
            }


            if (model.SupplierId != null && model.SupplierId > 0)
            {
                var supplier_model = await _commonRepository.GetSupplierById((int)model.SupplierId);
                if (supplier_model != null) model.SupplierName = supplier_model.FullName;
            }
            if (model.ProvinceId == null && model.ProvinceId > 0)
            {
                ViewBag.District = await _commonRepository.GetDistrictList(model.ProvinceId.ToString());
            }
            ViewBag.Provinces = await _commonRepository.GetProvinceList();
            ViewBag.Brands = await _brandRepository.GetAll();
            ViewBag.VerifyTimeList = _AllCodeRepository.GetListByType("EXPIRE_TIME_VERIFY_TYPE");

            return PartialView(model);
        }

        [HttpPost]
        public IActionResult Save(HotelUpsertViewModel model)
        {
            try
            {
                var result = _HotelRepository.SaveHotel(model);
                
                
                
             
                if (result > 0)
                {
                    int db_index = Convert.ToInt32(_configuration["Redis:Database:db_search_result"]);
                    string cache_name = CacheName.HotelExclusiveListB2C_POSITION + model.City.Trim();
                    _redisService.DeleteCacheByKeyword(cache_name, db_index);

                    return new JsonResult(new
                    {
                        isSuccess = true,
                        message = "Cập nhật khách sạn thành công"
                    });
                }
                else
                {
                    return new JsonResult(new
                    {
                        isSuccess = false,
                        message = "Cập nhật khách sạn thất bại"
                    });
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    isSuccess = false,
                    message = ex.Message
                });
            }
        }
        #endregion

        #region hotel detail
        public IActionResult Detail(int id)
        {
            var hotel = _HotelRepository.GetHotelDetailById(id);

            ViewBag.HotelUtilities = _AllCodeRepository.GetListByType("HOTEL_UTILITIES");
            return View(hotel);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateSurchargeNote(string body, int hotel_id)
        {
            try
            {
                if (body == null)
                    return new JsonResult(new
                    {
                        isSuccess = false,
                        message = "Lưu thông tin thất bại"
                    });
                var result = await _HotelRepository.UpdateHotelSurchargeNote(body, hotel_id);
                return new JsonResult(new
                {
                    isSuccess = result,
                    message = result ? "Lưu thông tin thành công" : "Lưu thông tin thất bại"
                });
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateSurchargeNote: " + ex.Message);
                return new JsonResult(new
                {
                    isSuccess = false,
                    message = ex.Message
                });
            }
        }
        #region BankingAccount

        [HttpPost]
        public IActionResult BankingAccountListing(int hotel_id)
        {
            var model = new GenericViewModel<HotelBankingAccountGridModel>();
            try
            {
                var datas = _HotelRepository.GetHotelBankingAccountList(hotel_id);
                model.CurrentPage = 1;
                model.ListData = datas.ToList();
                model.PageSize = 20;
                model.TotalRecord = datas != null && datas.Any() ? datas.FirstOrDefault().TotalRow : 0;
                model.TotalPage = (int)Math.Ceiling((double)model.TotalRecord / 20);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("SupplierController - BankingAccountListing: " + ex);
            }
            return PartialView(model);
        }

        public IActionResult BankingAccountUpsert(int id, int hotel_id)
        {
            var model = new HotelBankingAccount()
            {
                HotelId = hotel_id
            };

            try
            {
                if (id > 0) model = _HotelRepository.GetHotelBankingAccountById(id);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("SupplierController - PaymentUpsert: " + ex);
            }
            return PartialView(model);
        }

        [HttpPost]
        public IActionResult BankingAccountUpsert(HotelBankingAccount model)
        {
            try
            {
                var result = _HotelRepository.UpsertHotelBankingAccount(model);

                if (result > 0)
                {
                    return new JsonResult(new
                    {
                        isSuccess = true,
                        message = "Lưu thông tin thành công"
                    });
                }
                else
                {
                    return new JsonResult(new
                    {
                        isSuccess = false,
                        message = "Lưu thông tin thất bại"
                    });
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("PaymentUpsert - SupplierController: " + ex.Message);
                return new JsonResult(new
                {
                    isSuccess = false,
                    message = ex.Message
                });
            }
        }

        [HttpPost]
        public IActionResult BankingAccountDelete(int id)
        {
            try
            {
                var result = _HotelRepository.DeleteHotelBankingAccount(id);

                if (result > 0)
                {
                    return new JsonResult(new
                    {
                        isSuccess = true,
                        message = "Xóa thông tin thành công"
                    });
                }
                else
                {
                    return new JsonResult(new
                    {
                        isSuccess = false,
                        message = "Xóa thông tin thất bại"
                    });
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("PaymentDelete - SupplierController: " + ex.Message);
                return new JsonResult(new
                {
                    isSuccess = false,
                    message = ex.Message
                });
            }
        }
        #endregion

        #region Contact
        [HttpPost]
        public IActionResult ContactListing(int hotel_id)
        {
            var model = new GenericViewModel<HotelContactGridModel>();
            try
            {
                var datas = _HotelRepository.GetHotelContactList(hotel_id);
                model.CurrentPage = 1;
                model.ListData = datas.ToList();
                model.PageSize = 20;
                model.TotalRecord = datas != null && datas.Any() ? datas.FirstOrDefault().TotalRow : 0;
                model.TotalPage = (int)Math.Ceiling((double)model.TotalRecord / 20);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("SupplierController - ContactListing: " + ex);
            }
            return PartialView(model);
        }

        public IActionResult ContactUpsert(int id, int hotel_id)
        {
            var model = new HotelContact()
            {
                HotelId = hotel_id
            };

            try
            {
                if (id > 0) model = _HotelRepository.GetHotelContactById(id);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("SupplierController - ContactUpsert: " + ex);
            }
            return PartialView(model);
        }

        [HttpPost]
        public IActionResult ContactUpsert(HotelContact model)
        {
            try
            {
                var result = _HotelRepository.UpsertHotelContact(model);

                if (result > 0)
                {
                    return new JsonResult(new
                    {
                        isSuccess = true,
                        message = "Lưu thông tin thành công"
                    });
                }
                else
                {
                    return new JsonResult(new
                    {
                        isSuccess = false,
                        message = "Lưu thông tin thất bại"
                    });
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("ContactUpsert - SupplierController: " + ex.Message);
                return new JsonResult(new
                {
                    isSuccess = false,
                    message = ex.Message
                });
            }
        }

        [HttpPost]
        public IActionResult ContactDelete(int id)
        {
            try
            {
                var result = _HotelRepository.DeleteHotelContact(id);

                if (result > 0)
                {
                    return new JsonResult(new
                    {
                        isSuccess = true,
                        message = "Xóa thông tin thành công"
                    });
                }
                else
                {
                    return new JsonResult(new
                    {
                        isSuccess = false,
                        message = "Xóa thông tin thất bại"
                    });
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("ContactDelete - SupplierController: " + ex.Message);
                return new JsonResult(new
                {
                    isSuccess = false,
                    message = ex.Message
                });
            }
        }
        #endregion

        #region Surcharge
        [HttpPost]
        public IActionResult SurchargeListing(int hotel_id, int page_index, int page_size)
        {
            var model = new GenericViewModel<HotelSurchargeGridModel>();
            try
            {
                var datas = _HotelRepository.GetHotelSurchargeList(hotel_id, page_index, page_size);
                model.CurrentPage = page_index;
                model.ListData = datas.ToList();
                model.PageSize = page_size;
                model.TotalRecord = datas != null && datas.Any() ? datas.FirstOrDefault().TotalRow : 0;
                model.TotalPage = (int)Math.Ceiling((double)model.TotalRecord / model.PageSize);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("SurchargeListing: " + ex);
            }
            return PartialView(model);
        }

        public IActionResult SurchargeUpsert(int id, int hotel_id)
        {
            var model = new HotelSurcharge()
            {
                HotelId = hotel_id,
                Status = 1
            };

            try
            {
                if (id > 0) model = _HotelRepository.GetHotelSurchargeById(id);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("SurchargeUpsert: " + ex);
            }
            return PartialView(model);
        }

        [HttpPost]
        public IActionResult SurchargeUpsert(HotelSurcharge model)
        {
            try
            {
                var result = _HotelRepository.UpsertHotelSurcharge(model);

                if (result > 0)
                {
                    return new JsonResult(new
                    {
                        isSuccess = true,
                        message = "Lưu thông tin thành công"
                    });
                }
                else
                {
                    return new JsonResult(new
                    {
                        isSuccess = false,
                        message = "Lưu thông tin thất bại"
                    });
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("SurchargeUpsert: " + ex.Message);
                return new JsonResult(new
                {
                    isSuccess = false,
                    message = ex.Message
                });
            }
        }

        [HttpPost]
        public IActionResult SurchargeDelete(int id)
        {
            try
            {
                var result = _HotelRepository.DeleteHotelSurcharge(id);

                if (result > 0)
                {
                    return new JsonResult(new
                    {
                        isSuccess = true,
                        message = "Xóa thông tin thành công"
                    });
                }
                else
                {
                    return new JsonResult(new
                    {
                        isSuccess = false,
                        message = "Xóa thông tin thất bại"
                    });
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("SurchargeDelete: " + ex.Message);
                return new JsonResult(new
                {
                    isSuccess = false,
                    message = ex.Message
                });
            }
        }
        #endregion

        #region Room

        [HttpPost]
        public IActionResult RoomListing(int hotel_id, int page_index, int page_size)
        {
            var model = new GenericViewModel<HotelRoomGridModel>();
            try
            {
                var datas = _HotelRepository.GetHotelRoomList(hotel_id, page_index, page_size);
                model.CurrentPage = page_index;
                model.ListData = datas.ToList();
                model.PageSize = page_size;
                model.TotalRecord = datas != null && datas.Any() ? datas.FirstOrDefault().TotalRow : 0;
                model.TotalPage = (int)Math.Ceiling((double)model.TotalRecord / model.PageSize);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("SurchargeListing: " + ex);
            }
            return PartialView(model);
        }

        public IActionResult RoomUpsert(int id, int hotel_id, bool is_copy = false)
        {
            var model = new HotelRoomUpsertModel()
            {
                HotelId = hotel_id,
                IsActive = true,
                IsDisplayWebsite = true
            };

            try
            {
                if (id > 0)
                {
                    var room = _HotelRepository.GetHotelRoomById(id);

                    model = new HotelRoomUpsertModel
                    {
                        Avatar = room.Avatar,
                        BedRoomType = room.BedRoomType,
                        Code = room.Code,
                        HotelId = room.HotelId,
                        Name = room.Name,
                        Description = room.Description,
                        TypeOfRoom = room.TypeOfRoom,
                        Extends = room.Extends,
                        IsActive = room.IsActive,
                        IsDisplayWebsite = room.IsDisplayWebsite,
                        NumberOfAdult = room.NumberOfAdult,
                        NumberOfBedRoom = room.NumberOfBedRoom,
                        NumberOfChild = room.NumberOfChild,
                        NumberOfRoom = room.NumberOfRoom,
                        RoomAvatar = room.RoomAvatar,
                        RoomArea = room.RoomArea
                    };

                    if (is_copy)
                    {
                        model.Id = 0;
                        model.RoomAvatar = String.Empty;
                        model.Avatar = String.Empty;
                        model.IsActive = true;
                        model.IsDisplayWebsite = false;
                    }
                    else
                    {
                        model.Id = room.Id;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("RoomUpsert: " + ex);
            }

            ViewBag.TypeOfRooms = _AllCodeRepository.GetListByType("TYPE_OF_ROOM");
            ViewBag.RoomBedTypes = _AllCodeRepository.GetListByType("BedRoomType");
            ViewBag.RoomPackageTypes = _AllCodeRepository.GetListByType("HOTELROOM_UTILITIES");
            return PartialView(model);
        }

        [HttpPost]
        public IActionResult RoomUpsert(HotelRoomUpsertModel model)
        {
            try
            {
                var result = _HotelRepository.UpsertHotelRoom(model);

                if (result > 0)
                {
                    return new JsonResult(new
                    {
                        isSuccess = true,
                        message = "Lưu thông tin thành công"
                    });
                }
                else
                {
                    return new JsonResult(new
                    {
                        isSuccess = false,
                        message = "Lưu thông tin thất bại"
                    });
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("SurchargeUpsert: " + ex.Message);
                return new JsonResult(new
                {
                    isSuccess = false,
                    message = ex.Message
                });
            }
        }

        [HttpPost]
        public IActionResult RoomDelete(int id)
        {
            try
            {
                var result = _HotelRepository.DeleteHotelRoom(id);

                if (result > 0)
                {
                    return new JsonResult(new
                    {
                        isSuccess = true,
                        message = "Xóa thông tin thành công"
                    });
                }
                else if (result == -2)
                {
                    return new JsonResult(new
                    {
                        isSuccess = false,
                        message = "Phòng đã được cài đặt cho chương trình. Bạn không thể xóa dữ liệu."
                    });
                }
                else
                {
                    return new JsonResult(new
                    {
                        isSuccess = false,
                        message = "Xóa thông tin thất bại"
                    });
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("RoomDelete: " + ex.Message);
                return new JsonResult(new
                {
                    isSuccess = false,
                    message = ex.Message
                });
            }
        }
        #endregion

        #region Ultility

        [HttpPost]
        public async Task<IActionResult> UltilityUpsert(int hotel_id, string extends)
        {
            try
            {
                var result = await _HotelRepository.UpsertHotelUltilities(hotel_id, extends);

                if (result > 0)
                {
                    return new JsonResult(new
                    {
                        isSuccess = true,
                        message = "Lưu thông tin thành công"
                    });
                }
                else
                {
                    return new JsonResult(new
                    {
                        isSuccess = false,
                        message = "Lưu thông tin thất bại"
                    });
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("SurchargeUpsert: " + ex.Message);
                return new JsonResult(new
                {
                    isSuccess = false,
                    message = ex.Message
                });
            }
        }
        #endregion

        #endregion

        public IActionResult Suggest(string text, int size = 20)
        {
            var data = _HotelRepository.GetSuggestionHotelList(text, size);
            return new JsonResult(data.Select(s => new
            {
                id = s.Id,
                name = s.Name
            }));
        }

        public IActionResult UpsertHotel()
        {
            try
            {
                return PartialView();
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpsertHotel - HotelController: " + ex);
                return PartialView();
            }
        }

        [HttpPost]
        public async Task<IActionResult> ImportHotel(IFormFile file)
        {

            try
            {
                var stream = new MemoryStream();
                await file.CopyToAsync(stream);
                var data_list = new List<AddHotelViewModel>();
                using (var package = new ExcelPackage(stream))
                {
                    ExcelWorksheet ws = package.Workbook.Worksheets.FirstOrDefault();

                    var endRow = ws.Cells.End.Row;
                    var startRow = 2;

                    for (int row = startRow; row <= endRow; row++)
                    {
                        var cellRange = ws.Cells[row, 1, row, 11];
                        var isRowEmpty = cellRange.All(c => c.Value == null);
                        if (isRowEmpty)
                        {
                            break;
                        }
                        var data_name = ws.Cells[row, 1].Value;
                        var data_Street = ws.Cells[row, 2].Value;
                        var account_number = ws.Cells[row, 3].Value;
                        var bannk_name = ws.Cells[row, 4].Value;
                        var bank_branch = ws.Cells[row, 5].Value;
                        var bank_account_name = ws.Cells[row, 6].Value;
                        var data = new AddHotelViewModel
                        {
                            Name = (data_name ?? string.Empty).ToString(),
                            Street = (data_Street ?? string.Empty).ToString(),
                            AccountNumber = (account_number ?? string.Empty).ToString(),
                            BankName = (bannk_name ?? string.Empty).ToString(),
                            Branch = (bank_branch ?? string.Empty).ToString(),
                            BankAccountName = (bank_account_name ?? string.Empty).ToString(),
                        };

                        data_list.Add(data);
                    }


                }

                ViewBag.CheckedAll = true;
                return PartialView(data_list);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("ImportHotel - HotelController: " + ex.ToString());
                return PartialView();
            }
        }
        [HttpPost]
        public async Task<IActionResult> SetUpHotel(List<AddHotelViewModel> model)
        {
            try
            {
                if (model != null && model.Count > 0)
                {
                    var userId = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                    foreach (var item in model)
                    {
                        var Supplier = new SupplierConfigUpsertModel();
                        Supplier.FullName = item.Name;
                        Supplier.ContactName = "";
                        Supplier.CreatedBy = userId;
                        Supplier.IsDisplayWebsite = false;
                        Supplier.BankId = null;
                        Supplier.BankAccountName = item.BankAccountName ?? "";
                        Supplier.BankAccountNumber = item.AccountNumber ?? "";
                        Supplier.BankBranch = item.Branch;
                        Supplier.Phone = "";
                        Supplier.ContactPhone = "";
                        int createsupplier = 0;
                        try
                        {
                            createsupplier = _supplierRepository.Add(Supplier);
                        }
                        catch
                        {

                        }
                        if (createsupplier <= 0)
                        {
                            var suplier = _supplierRepository.GetByIDOrName(Supplier.SupplierId, Supplier.FullName);
                            createsupplier = suplier;
                        }
                        var Hotel = new Hotel();
                        Hotel.Name = item.Name;
                        Hotel.Street = item.Street;
                        Hotel.CreatedBy = userId;
                        Hotel.UpdatedBy = userId;
                        Hotel.CreatedDate = DateTime.Now;
                        Hotel.UpdatedDate = DateTime.Now;
                        Hotel.SupplierId = createsupplier;
                        Hotel.HotelId = "0";
                        int hotel_id = await _hotelBookingRepositories.CreateHotel(Hotel);
                        if (hotel_id > 0)
                        {
                            var hotel_banking_account = new HotelBankingAccount()
                            {
                                AccountName = item.BankAccountName ?? "",
                                AccountNumber = item.AccountNumber ?? "",
                                BankId = item.BankName ?? "",
                                Branch = item.Branch ?? "",
                                CreatedBy = userId,
                                CreatedDate = DateTime.Now,
                                HotelId = hotel_id,
                                UpdatedBy = userId,
                                UpdatedDate = DateTime.Now,

                            };
                            var result = _HotelRepository.UpsertHotelBankingAccountByName(hotel_banking_account);
                        }
                    }

                    return new JsonResult(new
                    {
                        isSuccess = true,
                        message = "Thêm khách sạn thành công"
                    });
                }
                return new JsonResult(new
                {
                    isSuccess = false,
                    message = "Thêm khách sạn không thành công"
                });
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("SetUpHotel - HotelController: " + ex.ToString());
                return new JsonResult(new
                {
                    isSuccess = false,
                    message = "xẩy ra lỗi vui lòng liên hệ IT"
                });
            }
        }
        public async Task<IActionResult> SuggestDistrict(string id)
        {
            var data = await _commonRepository.GetDistrictList(id);
            return new JsonResult(data);
        }

        public async Task<IActionResult> ProgramSuggestionByHotelid(string txt_search)
        {

            try
            {
                if (txt_search == null)
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.FAILED,
                        data = new List<ProgramsViewModel>()
                    });
                }
                var data = await _programsESRepository.GetProgramsSuggesstionByHotelid(txt_search);
                if (data != null && data.Count > 0)
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.SUCCESS,
                        data = data
                    });
                }
                else
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.FAILED,
                        data = new List<ProgramsViewModel>()
                    });
                }

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("ProgramSuggestion - ProgramsController: " + ex.ToString());
                return Ok(new
                {
                    status = (int)ResponseType.FAILED,
                    data = new List<ProgramsViewModel>()
                });
            }

        }
    }
}
