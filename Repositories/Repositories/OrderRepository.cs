using Aspose.Cells;
using DAL;
using DAL.StoreProcedure;
using Entities.ConfigModels;
using Entities.Models;
using Entities.ViewModels;
using Entities.ViewModels.Funding;
using Entities.ViewModels.HotelBookingCode;
using Entities.ViewModels.OrderManual;
using Entities.ViewModels.Report;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Repositories.IRepositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;

namespace Repositories.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly OrderDAL _OrderDal;
        private readonly ClientDAL _clientDAL;
        private readonly ContractPayDAL contractPayDAL;
        private readonly AllCodeDAL allCodeDAL;
        private readonly UserDAL userDAL;
        private readonly HotelBookingDAL hotelBookingDAL;
        private readonly HotelBookingCodeDAL _hotelBookingCodeDAL;
        private readonly ContractPayDAL _contractPayDAL;


        public OrderRepository(IOptions<DataBaseConfig> dataBaseConfig)
        {
            _OrderDal = new OrderDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
            contractPayDAL = new ContractPayDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
            allCodeDAL = new AllCodeDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
            userDAL = new UserDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
            hotelBookingDAL = new HotelBookingDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
            _clientDAL = new ClientDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
            _contractPayDAL = new ContractPayDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
            _hotelBookingCodeDAL = new HotelBookingCodeDAL(dataBaseConfig.Value.SqlServer.ConnectionString);

        }
        public async Task<GenericViewModel<OrderViewModel>> GetTotalCountOrder(OrderViewSearchModel searchModel, int currentPage, int pageSize)
        {
            var model = new GenericViewModel<OrderViewModel>();
            try
            {
                DataTable dt = await _OrderDal.GetPagingList(searchModel, currentPage, pageSize, ProcedureConstants.GET_TOTALCOUNT_ORDER);
                if (dt != null && dt.Rows.Count > 0)
                {

                    model.TotalRecord = dt.Rows[0]["Total"].Equals(DBNull.Value) ? 0 : Convert.ToInt32(dt.Rows[0]["Total"].ToString());
                    model.TotalRecord1 = dt.Rows[0]["TotalStatusTab1"].Equals(DBNull.Value) ? 0 : Convert.ToInt32(dt.Rows[0]["TotalStatusTab1"]);
                    model.TotalRecord2 = dt.Rows[0]["TotalStatusTab2"].Equals(DBNull.Value) ? 0 : Convert.ToInt32(dt.Rows[0]["TotalStatusTab2"]);
                    model.TotalRecord3 = dt.Rows[0]["TotalStatusTab3"].Equals(DBNull.Value) ? 0 : Convert.ToInt32(dt.Rows[0]["TotalStatusTab3"]);
                    model.TotalrecordErr = dt.Rows[0]["TotalStatusTab4"].Equals(DBNull.Value) ? 0 : Convert.ToInt32(dt.Rows[0]["TotalStatusTab4"]);
                    model.TotalRecord4 = dt.Rows[0]["TotalStatusTab5"].Equals(DBNull.Value) ? 0 : Convert.ToInt32(dt.Rows[0]["TotalStatusTab5"]);
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetTotalCountOrder in OrderRepository: " + ex);
            }
            return model;
        }
        private async Task<GenericViewModel<OrderViewModel>> GetOrders(OrderViewSearchModel searchModel, int currentPage, int pageSize)
        {
            var model = new GenericViewModel<OrderViewModel>();
            try
            {
                DataTable dt = await _OrderDal.GetPagingList(searchModel, currentPage, pageSize, ProcedureConstants.GETALLORDER_SEARCH);
                if (dt != null && dt.Rows.Count > 0)
                {
                    model.ListData = (from row in dt.AsEnumerable()
                                      select new OrderViewModel
                                      {
                                          OrderId = row["OrderId"].ToString(),
                                          OrderCode = row["OrderNo"].ToString(),
                                          StartDate = !row["StartDate"].Equals(DBNull.Value) ? Convert.ToDateTime(row["StartDate"]).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) : "",
                                          EndDate = !row["EndDate"].Equals(DBNull.Value) ? Convert.ToDateTime(row["EndDate"]).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) : "",
                                          ClientName = row["ClientName"].ToString(),
                                          ClientNumber = row["Phone"].ToString(),
                                          ClientEmail = row["Email"].ToString(),
                                          Note = row["Note"].ToString(),
                                          PaymentStatus = row["PaymentStatus"].ToString(),
                                          Payment = !row["Payment"].Equals(DBNull.Value) ? Convert.ToDouble(row["Payment"].ToString()) : 0,
                                          Amount = !row["Amount"].Equals(DBNull.Value) ? Convert.ToDouble(row["Amount"].ToString()) : 0,
                                          UtmSource = row["UtmSource"].ToString(),
                                          Profit = !row["Profit"].Equals(DBNull.Value) ? Convert.ToDouble(row["Profit"].ToString()) : 0,
                                          Status = row["Status"].ToString(),
                                          StatusCode = !row["StatusCode"].Equals(DBNull.Value) ? Convert.ToInt32(row["StatusCode"]) : -1,
                                          CreateDate = row["CreateTime"].Equals(DBNull.Value) ? "" : Convert.ToDateTime(row["CreateTime"]).ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture),
                                          CreateName = row["CreateName"].ToString(),
                                          UpdateName = row["UpdateName"].ToString(),
                                          UpdateDate = row["UpdateLast"].Equals(DBNull.Value) ? "" : Convert.ToDateTime(row["UpdateLast"]).ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture),
                                          SalerName = row["SalerName"].ToString(),
                                          ServiceType = row["ServiceType"].ToString(),
                                          SaleGroupName = row["SalerGroupName"].ToString(),
                                          Vouchercode = row["code"].ToString(),
                                          PaymentStatusName = row["PaymentStatusName"].ToString(),
                                          PermisionTypeName = row["PermisionTypeName"].ToString(),
                                          OperatorIdName = row["OperatorIdName"].ToString(),
                                          SalerUserName = row["SalerUserName"].ToString(),
                                          SalerEmail = row["SalerEmail"].ToString(),
                                          UtmMedium = row["UtmMedium"].ToString(),


                                      }).ToList();

                    model.CurrentPage = currentPage;
                    model.PageSize = pageSize;
                    model.TotalRecord = Convert.ToInt32(dt.Rows[0]["TotalRow"]);
                    model.TotalPage = (int)Math.Ceiling((double)model.TotalRecord / model.PageSize);
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetOrders in OrderRepository: " + ex);
            }
            return model;
        }

        public async Task<GenericViewModel<OrderViewModel>> GetList(OrderViewSearchModel searchModel, int currentPage, int pageSize)
        {
            var model = new GenericViewModel<OrderViewModel>();

            var model1 = await GetOrders(searchModel, currentPage, pageSize);
            var model3 = await GetTotalCountOrder(searchModel, currentPage, pageSize);

            try
            {
                if (model1.ListData != null)
                {
                    model.ListData = (from md1 in model1.ListData
                                      select new OrderViewModel
                                      {
                                          OrderId = md1.OrderId,
                                          OrderCode = md1.OrderCode,
                                          StartDate = md1.StartDate,
                                          EndDate = md1.EndDate,
                                          ClientName = md1.ClientName,
                                          ClientNumber = md1.ClientNumber,
                                          ClientEmail = md1.ClientEmail,
                                          Note = md1.Note,
                                          Payment = md1.Payment,
                                          Amount = md1.Amount,
                                          UtmSource = md1.UtmSource,
                                          Profit = md1.Profit,
                                          PaymentStatus = md1.PaymentStatus,
                                          //StatusDetail = md2.StatusDetail,
                                          Status = md1.Status,
                                          StatusCode = md1.StatusCode,
                                          CreateDate = md1.CreateDate,
                                          CreateName = md1.CreateName,
                                          UpdateName = md1.UpdateName,
                                          UpdateDate = md1.UpdateDate,
                                          SalerName = md1.SalerName,
                                          SaleGroupName = md1.SaleGroupName,
                                          PaymentStatusName = md1.PaymentStatusName,
                                          PermisionTypeName = md1.PermisionTypeName,
                                          Vouchercode = md1.Vouchercode,
                                          OperatorIdName = md1.OperatorIdName,
                                          SalerEmail = md1.SalerEmail,
                                          SalerUserName = md1.SalerUserName,
                                          UtmMedium = md1.UtmMedium,
                                      }
                                                    ).ToList();
                }
                else
                {
                    //  LogHelper.InsertLogTelegram("GetList -  OrderRepository: No Order Count with" + JsonConvert.SerializeObject(searchModel));

                }
                model.CurrentPage = model1.CurrentPage;
                model.PageSize = model1.PageSize;
                model.TotalPage = model1.TotalPage;
                model.TotalRecord = model3.TotalRecord;
                model.TotalRecord1 = model3.TotalRecord1;
                model.TotalRecord2 = model3.TotalRecord2;
                model.TotalRecord3 = model3.TotalRecord3;
                model.TotalRecord4 = model3.TotalRecord4;
                model.TotalrecordErr = model3.TotalrecordErr;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetList in OrderRepository: " + ex);
            }
            return model;
        }

        public async Task<Order> CreateOrder(Order order)
        {
            try
            {

                var id = await _OrderDal.CreateOrder(order);
                if (order.OrderId > 0)
                {
                    return order;
                }

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("CreateOrder in OrderRepository: " + ex);
            }
            return null;
        }

        public async Task<Order> GetOrderByID(long id)
        {
            try
            {

                return _OrderDal.GetByOrderId(id);

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetOrderByID - OrderRepository: " + ex);
            }
            return null;
        }

        public List<OrderViewModel> GetByClientId(long clientId, int payId = 0, int status = 0)
        {
            try
            {
                var listOrder = new List<OrderViewModel>();
                var listOrderOutput = new List<OrderViewModel>();
                var dt = _OrderDal.GetListOrderByClientId(clientId, ProcedureConstants.SP_GetDetailOrderByClientId, status);
                if (dt != null && dt.Rows.Count > 0)
                {
                    listOrder = (from row in dt.AsEnumerable()
                                 select new OrderViewModel
                                 {
                                     OrderId = row["OrderId"].ToString(),
                                     OrderCode = row["OrderNo"].ToString(),
                                     StartDate = !row["StartDate"].Equals(DBNull.Value) ? Convert.ToDateTime(row["StartDate"]).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) : "",
                                     EndDate = !row["EndDate"].Equals(DBNull.Value) ? Convert.ToDateTime(row["EndDate"]).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) : "",
                                     Status = row["OrderStatus"].ToString(),
                                     PaymentStatus = row["PaymentStatus"].ToString(),
                                     SalerName = row["SalerName"].ToString(),
                                     CreateDate = row["CreateTime"].Equals(DBNull.Value) ? "" : Convert.ToDateTime(row["CreateTime"]).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                                     Amount = !row["Amount"].Equals(DBNull.Value) ? Convert.ToDouble(row["Amount"].ToString()) : 0,
                                     IsFinishPayment = Convert.ToInt32(row["IsFinishPayment"].ToString()),
                                 }).ToList();
                    var listContractPayDetail = contractPayDAL.GetByContractDataIds(listOrder.Select(n => Convert.ToInt64(n.OrderId)).ToList());
                    foreach (var item in listOrder)
                    {
                        OrderViewModel orderViewModel = new OrderViewModel();
                        var detail = listContractPayDetail.Where(n => n.DataId != null
                                && n.DataId.Value == Convert.ToInt64(item.OrderId) && n.PayId == payId).FirstOrDefault();
                        var TotalDisarmed = listContractPayDetail.Where(n => n.DataId != null
                                && n.DataId.Value == Convert.ToInt64(item.OrderId)).ToList().Sum(n => n.Amount);
                        item.TotalDisarmed = (double)TotalDisarmed;
                        item.TotalAmount = item.Amount;
                        item.TotalNeedPayment = item.Amount - item.TotalDisarmed;
                        item.CopyProperties(orderViewModel);
                        if (detail != null)
                        {
                            orderViewModel.PayDetailId = detail.Id;
                            orderViewModel.IsChecked = true;
                            orderViewModel.Amount = (double)detail?.Amount;
                            orderViewModel.Payment = (double)detail?.Amount;
                        }

                        if (item.TotalNeedPayment > 0 || (item.Amount == 0 && item.IsFinishPayment == 0))
                        {
                            if (payId <= 0)
                                orderViewModel.Amount = item.TotalNeedPayment;
                            listOrderOutput.Add(orderViewModel);
                        }

                    }
                    if (payId != 0)
                    {
                        var allCode_ORDER_STATUS = allCodeDAL.GetListByType(AllCodeType.ORDER_STATUS);
                        var listOrderId = listOrderOutput.Select(n => Convert.ToInt64(n.OrderId)).ToList();
                        listContractPayDetail = contractPayDAL.GetByContractPayIds(new List<int>() { payId });
                        var listOrderDisable = listContractPayDetail.Where(n => !listOrderId.Contains(n.DataId.Value)).ToList();
                        foreach (var item in listOrderDisable)
                        {
                            OrderViewModel orderViewModel = new OrderViewModel();
                            var order = listOrder.FirstOrDefault(n => Convert.ToInt64(n.OrderId) == item.DataId);
                            if (order != null)
                            {
                                order.CopyProperties(orderViewModel);
                                orderViewModel.Amount = (double)item?.Amount;
                                orderViewModel.Payment = (double)item?.Amount;
                                orderViewModel.TotalDisarmed = order.Amount;
                                orderViewModel.TotalAmount = order.Amount;
                            }
                            else
                            {
                                var orderInfo = _OrderDal.GetByOrderId(item.DataId.Value);
                                if (orderInfo != null)
                                {
                                    orderViewModel.OrderId = orderInfo.OrderId.ToString();
                                    orderViewModel.OrderCode = orderInfo.OrderNo;
                                    orderViewModel.StartDate = orderInfo.StartDate != null ?
                                        orderInfo.StartDate.Value.ToString("dd:MM:yyyy") : string.Empty;
                                    orderViewModel.EndDate = orderInfo.EndDate != null ?
                                        orderInfo.EndDate.Value.ToString("dd:MM:yyyy") : string.Empty;
                                    orderViewModel.Status = allCode_ORDER_STATUS.FirstOrDefault(n => n.CodeValue == orderInfo.OrderStatus)?.Description;
                                    orderViewModel.SalerName = userDAL.GetById(orderInfo.SalerId != null ? orderInfo.SalerId.Value : 0).Result?.FullName;
                                    orderViewModel.Amount = (double)item?.Amount;
                                    orderViewModel.Payment = (double)item?.Amount;
                                    orderViewModel.TotalDisarmed = orderInfo.Amount.Value;
                                    orderViewModel.TotalAmount = orderInfo.Amount.Value;
                                }
                            }
                            orderViewModel.PayDetailId = item.Id;
                            orderViewModel.IsChecked = true;

                            orderViewModel.IsDisabled = true;
                            listOrderOutput.Add(orderViewModel);
                        }
                    }
                }
                return listOrderOutput;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetByClientId - OrderRepository: " + ex);
            }
            return new List<OrderViewModel>();
        }

        public async Task<Order> GetOrderByOrderNo(string orderNo)
        {
            try
            {
                return _OrderDal.GetByOrderNo(orderNo);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetOrderByOrderNo - OrderRepository: " + ex);
            }
            return null;
        }
        public async Task<List<ProductServiceName>> ProductServiceName(string OrderId)
        {
            var ListData = new List<ProductServiceName>();
            try
            {

                DataTable dt = await _OrderDal.GetDetailOrderServiceByOrderId(Convert.ToInt32(OrderId));
                if (dt != null && dt.Rows.Count > 0)
                {
                    ListData = (from row in dt.AsEnumerable()
                                select new ProductServiceName
                                {
                                    OrderId = row["OrderId"].ToString(),
                                    ServiceName = row["ServiceName"].ToString(),
                                    StatusName = row["StatusName"].ToString(),
                                    ServiceId = row["ServiceId"].ToString(),
                                    Type = row["Type"].ToString(),
                                    Status = Convert.ToInt32(row["Status"].ToString()),
                                }).ToList();
                    foreach (var item in ListData)
                    {
                        DataTable dt2 = await hotelBookingDAL.GetServiceDeclinesByServiceId(item.ServiceId, Convert.ToInt32(item.Type));
                        if (dt2 != null && dt2.Rows.Count > 0)
                        {
                            var data2 = dt2.ToList<ServiceDeclinesViewModel>();
                            item.Note = data2[0].Note;
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("ProductServiceName- OrderRepository: " + ex);
            }
            return ListData;
        }
        public async Task<double> UpdateOrderDetail(long OrderId, long user_id)
        {
            try
            {
                var result = await _OrderDal.UpdateOrderDetail(OrderId, user_id);
                var order = _OrderDal.GetByOrderId(OrderId);
                if (order == null || order.OrderId <= 0)
                {
                    return result;
                }
                await UndoContractPayByOrderId(OrderId, (int)user_id);

                return result;

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateOrderAmount - OrderRepository: " + ex);
            }
            return -2;
        }
        public async Task<int> UpdateOrderStatus(long OrderId, long Status, long UpdatedBy, long UserVerify)
        {
            try
            {
                return await _OrderDal.UpdateOrderStatus(OrderId, Status, UpdatedBy, UserVerify);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateOrderAmount - OrderRepository: " + ex);
            }
            return 0;
        }
        public async Task<List<OrderServiceViewModel>> GetAllServiceByOrderId(long OrderId)
        {
            //var data = new List<OrderServiceViewModel>();
            try
            {
                DataTable dt = await _OrderDal.GetAllServiceByOrderId(OrderId);
                if (dt != null && dt.Rows.Count > 0)
                {
                    var listData = dt.ToList<OrderServiceViewModel>();
                    return listData;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateOrderAmount - OrderRepository: " + ex);
            }
            return null;
        }

        public List<OrderViewModel> GetBySupplierId(long clientId, int payId = 0)
        {
            try
            {
                var listOrder = new List<OrderViewModel>();

                return listOrder;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetBySupplierId - OrderRepository: " + ex);
                return new List<OrderViewModel>();
            }
        }

        public List<OrderViewModel> GetOrderByClientId(long clientId, int payId = 0)
        {
            try
            {
                var listOrder = new List<OrderViewModel>();
                var listOrderOutput = new List<OrderViewModel>();
                var dt = _OrderDal.GetListOrderByClientId(clientId, ProcedureConstants.SP_GetDetailOrderByClientId);
                if (dt != null && dt.Rows.Count > 0)
                {
                    listOrder = (from row in dt.AsEnumerable()
                                 select new OrderViewModel
                                 {
                                     OrderId = row["OrderId"].ToString(),
                                     OrderCode = row["OrderNo"].ToString(),
                                     StartDate = !row["StartDate"].Equals(DBNull.Value) ? Convert.ToDateTime(row["StartDate"]).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) : "",
                                     EndDate = !row["EndDate"].Equals(DBNull.Value) ? Convert.ToDateTime(row["EndDate"]).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) : "",
                                     Status = row["OrderStatus"].ToString(),
                                     PaymentStatus = row["PaymentStatus"].ToString(),
                                     SalerName = row["SalerName"].ToString(),
                                     CreateDate = row["CreateTime"].Equals(DBNull.Value) ? "" : Convert.ToDateTime(row["CreateTime"]).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                                     Amount = !row["Amount"].Equals(DBNull.Value) ? Convert.ToDouble(row["Amount"].ToString()) : 0,
                                 }).ToList();
                    var listContractPayDetail = contractPayDAL.GetByContractDataIds(listOrder.Select(n => Convert.ToInt64(n.OrderId)).ToList());
                    foreach (var item in listOrder)
                    {
                        OrderViewModel orderViewModel = new OrderViewModel();
                        var detail = listContractPayDetail.Where(n => n.DataId != null
                                && n.DataId.Value == Convert.ToInt64(item.OrderId) && n.PayId == payId).FirstOrDefault();
                        var TotalDisarmed = listContractPayDetail.Where(n => n.DataId != null
                                && n.DataId.Value == Convert.ToInt64(item.OrderId)).ToList().Sum(n => n.Amount);
                        item.TotalDisarmed = (double)TotalDisarmed;
                        item.TotalAmount = item.Amount;
                        item.TotalNeedPayment = item.Amount - item.TotalDisarmed;
                        item.CopyProperties(orderViewModel);
                        if (detail != null)
                        {
                            orderViewModel.PayDetailId = detail.Id;
                            orderViewModel.IsChecked = true;
                            orderViewModel.Amount = (double)detail?.Amount;
                            orderViewModel.Payment = (double)detail?.Amount;
                        }

                        if (item.TotalNeedPayment > 0)
                            listOrderOutput.Add(orderViewModel);
                    }
                    if (payId != 0)
                    {
                        var allCode_ORDER_STATUS = allCodeDAL.GetListByType(AllCodeType.ORDER_STATUS);
                        var listOrderId = listOrderOutput.Select(n => Convert.ToInt64(n.OrderId)).ToList();
                        listContractPayDetail = contractPayDAL.GetByContractPayIds(new List<int>() { payId });
                        var listOrderDisable = listContractPayDetail.Where(n => !listOrderId.Contains(n.DataId.Value)).ToList();
                        foreach (var item in listOrderDisable)
                        {
                            OrderViewModel orderViewModel = new OrderViewModel();
                            var order = listOrder.FirstOrDefault(n => Convert.ToInt64(n.OrderId) == item.DataId);
                            if (order != null)
                            {
                                order.CopyProperties(orderViewModel);
                                orderViewModel.Amount = (double)item?.Amount;
                                orderViewModel.Payment = (double)item?.Amount;
                                orderViewModel.TotalDisarmed = order.Amount;
                                orderViewModel.TotalAmount = order.Amount;
                            }
                            else
                            {
                                var orderInfo = _OrderDal.GetByOrderId(item.DataId.Value);
                                orderViewModel.OrderId = orderInfo.OrderId.ToString();
                                orderViewModel.OrderCode = orderInfo.OrderNo;
                                orderViewModel.StartDate = orderInfo.StartDate != null ?
                                    orderInfo.StartDate.Value.ToString("dd:MM:yyyy") : string.Empty;
                                orderViewModel.EndDate = orderInfo.EndDate != null ?
                                    orderInfo.EndDate.Value.ToString("dd:MM:yyyy") : string.Empty;
                                orderViewModel.Status = allCode_ORDER_STATUS.FirstOrDefault(n => n.CodeValue == orderInfo.OrderStatus)?.Description;
                                orderViewModel.SalerName = userDAL.GetById(orderInfo.SalerId != null ? orderInfo.SalerId.Value : 0).Result?.FullName;
                                orderViewModel.Amount = (double)item?.Amount;
                                orderViewModel.Payment = (double)item?.Amount;
                                orderViewModel.TotalDisarmed = orderInfo.Amount.Value;
                                orderViewModel.TotalAmount = orderInfo.Amount.Value;
                            }
                            orderViewModel.PayDetailId = item.Id;
                            orderViewModel.IsChecked = true;

                            orderViewModel.IsDisabled = true;
                            listOrderOutput.Add(orderViewModel);
                        }
                    }
                }
                return listOrderOutput;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetByClientId - OrderRepository: " + ex);
            }
            return new List<OrderViewModel>();
        }
        public int UpdateOrder(Order model)
        {
            return _OrderDal.UpdateOrder(model);
        }
        public async Task<int> IsClientAllowedToDebtNewService(double service_amount, long client_id, long order_id, int service_type)
        {
            try
            {
                var client = await _clientDAL.GetClientDetail(client_id);
                if (client != null && client.ClientType == (int)ClientType.kl)
                {
                    return (int)DebtType.DEBT_ACCEPTED;
                }
                return _OrderDal.IsClientAllowedToDebtNewService(service_amount, client_id, order_id, service_type);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("IsClientAllowedToDebtNewService - OrderRepository: " + ex);
            }
            return (int)DebtType.DEBT_NOT_ACCEPTED;
        }
        public async Task<long> UpdateOrderSaler(long order_id, int user_commit)
        {
            return await _OrderDal.UpdateOrderSaler(order_id, user_commit);
        }

        public int UpdateOrderOperator(long order_id)
        {
            return _OrderDal.UpdateOrderOperator(order_id);
        }
        public async Task<long> UpdateOrderAmountFundCustomer(long order_id)
        {
            return await _OrderDal.UpdateOrderAmountFundCustomer(order_id);
        }
        public async Task<long> UpdateOrderFinishPayment(long OrderId, long Status)
        {
            try
            {
                var update = await _OrderDal.UpdateOrderFinishPayment(OrderId, Status);
                return update;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateOrderFinishPayment - OrderRepository: " + ex);
            }
            return -1;
        }
        public async Task<long> UpdateServiceStatusByOrderId(long OrderId, long StatusFilter, long Status)
        {
            try
            {
                var update = await _OrderDal.UpdateServiceStatusByOrderId(OrderId, StatusFilter, Status);
                return update;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateOrderFinishPayment - OrderRepository: " + ex);
            }
            return -1;
        }
        public async Task<long> UpdateAllServiceStatusByOrderId(long OrderId, long Status)
        {
            try
            {
                var update = await _OrderDal.UpdateAllServiceStatusByOrderId(OrderId, Status);
                return update;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateOrderFinishPayment - OrderRepository: " + ex);
            }
            return -1;
        }
        public async Task<long> RePushDeclineServiceToOperator(long OrderId)
        {
            try
            {
                var update = await _OrderDal.RePushDeclineServiceToOperator(OrderId);
                return update;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("RePushDeclineServiceToOperator - OrderRepository: " + ex);
            }
            return -1;
        }
        public async Task<string> ExportDeposit(OrderViewSearchModel searchModel, string FilePath, FieldOrder field, int currentPage, int pageSize)
        {
            var pathResult = string.Empty;
            try
            {
                currentPage = -1;
                var data = new List<OrderViewModel>();
                DataTable dt = await _OrderDal.GetPagingList(searchModel, currentPage, pageSize, ProcedureConstants.GETALLORDER_SEARCH);
                if (dt != null && dt.Rows.Count > 0)
                {
                    data = (from row in dt.AsEnumerable()
                            select new OrderViewModel
                            {
                                OrderId = row["OrderId"].ToString(),
                                OrderCode = row["OrderNo"].ToString(),
                                StartDate = !row["StartDate"].Equals(DBNull.Value) ? Convert.ToDateTime(row["StartDate"]).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) : "",
                                EndDate = !row["EndDate"].Equals(DBNull.Value) ? Convert.ToDateTime(row["EndDate"]).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) : "",
                                ClientName = row["ClientName"].ToString(),
                                ClientNumber = row["Phone"].ToString(),
                                ClientEmail = row["Email"].ToString(),
                                Note = row["Note"].ToString(),
                                Payment = !row["Payment"].Equals(DBNull.Value) ? Convert.ToDouble(row["Payment"].ToString()) : 0,
                                Amount = !row["Amount"].Equals(DBNull.Value) ? Convert.ToDouble(row["Amount"].ToString()) : 0,
                                UtmSource = row["UtmSource"].ToString(),
                                Profit = !row["Profit"].Equals(DBNull.Value) ? Convert.ToDouble(row["Profit"].ToString()) : 0,
                                Status = row["Status"].ToString(),
                                StatusCode = !row["StatusCode"].Equals(DBNull.Value) ? Convert.ToInt32(row["StatusCode"]) : -1,
                                CreateDate = row["CreateTime"].Equals(DBNull.Value) ? "" : Convert.ToDateTime(row["CreateTime"]).ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture),
                                CreateName = row["CreateName"].ToString(),
                                UpdateName = row["UpdateName"].ToString(),
                                UpdateDate = row["UpdateLast"].Equals(DBNull.Value) ? "" : Convert.ToDateTime(row["UpdateLast"]).ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture),
                                SalerName = row["SalerName"].ToString(),
                                ServiceType = row["ServiceType"].ToString(),
                                SaleGroupName = row["SalerGroupName"].ToString(),
                                Vouchercode = row["code"].ToString(),
                                PaymentStatusName = row["PaymentStatusName"].ToString(),
                                PermisionTypeName = row["PermisionTypeName"].ToString(),
                                OperatorIdName = row["OperatorIdName"].ToString(),
                                SalerUserName = row["SalerUserName"].ToString(),
                                SalerEmail = row["SalerEmail"].ToString(),
                                UtmMedium = row["UtmMedium"].ToString(),
                            }).ToList();
                }
                else
                {
                    LogHelper.InsertLogTelegram("GetList -  OrderRepository: No Order Count with" + JsonConvert.SerializeObject(searchModel));

                }
                if (data != null && data.Count > 0)
                {
                    Workbook wb = new Workbook();
                    Worksheet ws = wb.Worksheets[0];
                    ws.Name = "Danh sách đơn hàng";
                    Cells cell = ws.Cells;

                    var range = ws.Cells.CreateRange(0, 0, 1, 1);
                    StyleFlag st = new StyleFlag();
                    st.All = true;
                    Style style = ws.Cells["A1"].GetStyle();

                    #region Header


                    // Set column width
                    var listfield = new List<int>();
                    var listfieldtext = new List<string>();
                    if (field.OrderNo) { listfieldtext.Add("Mã đơn"); listfield.Add(1); }
                    if (field.DateOrder)
                    {
                        listfieldtext.Add("Ngày bắt đầu"); listfield.Add(2);
                        listfieldtext.Add("kết thúc");
                    }
                    if (field.ClientOrder)
                    {
                        listfieldtext.Add("Khách hàng"); listfield.Add(3);
                        listfieldtext.Add("Số điện thoại khách hàng");
                        listfieldtext.Add(" Email khách hàng"); ;
                    }
                    if (field.NoteOrder) { listfieldtext.Add("Nhãn đơn"); listfield.Add(4); }
                    if (field.PayOrder)
                    {
                        listfieldtext.Add("Doanh thu"); listfield.Add(5);
                        listfieldtext.Add("Khách đã thanh toán");
                        listfieldtext.Add("Khách phải trả");
                    }
                    if (field.UtmSource) { listfieldtext.Add("Nguồn"); listfield.Add(6); }
                    if (field.ProfitOrder) { listfieldtext.Add("Lợi nhuận"); listfield.Add(7); }
                    if (field.tum_medium) { listfieldtext.Add("Mã giới thiệu"); listfield.Add(8); }
                    if (field.SttOrder) { listfieldtext.Add("Trạng thái"); listfield.Add(9); }
                    if (field.StartDateOrder) { listfieldtext.Add("Ngày tạo"); listfield.Add(10); }
                    if (field.CreatedName) { listfieldtext.Add("Người tạo"); listfield.Add(11); }
                    if (field.UpdatedDate) { listfieldtext.Add("Ngày cập nhật"); listfield.Add(12); }
                    if (field.UpdatedName) { listfieldtext.Add("Người cập nhật"); listfield.Add(13); }
                    if (field.MainEmp) { listfieldtext.Add("Nhân viên chính"); listfield.Add(14); }
                    if (field.SubEmp) { listfieldtext.Add("Nhân viên phụ"); listfield.Add(15); }
                    if (field.Voucher) { listfieldtext.Add("Voucher"); listfield.Add(16); }
                    if (field.Operator) { listfieldtext.Add("Điều hành viên"); listfield.Add(17); }
                    if (field.HINHTHUCTT) { listfieldtext.Add("Hình thức thanh toán"); listfield.Add(18); }

                    listfieldtext.Add("Mã code dịch vụ"); listfield.Add(19);

                    cell.SetColumnWidth(0, 8);
                    for (int i = 1; i <= listfieldtext.Count; i++)
                    {
                        cell.SetColumnWidth(i, 40);
                    }

                    range = cell.CreateRange(0, 0, 1, listfieldtext.Count + 1);
                    style = ws.Cells["A1"].GetStyle();
                    style.Font.IsBold = true;
                    style.IsTextWrapped = true;
                    style.ForegroundColor = Color.FromArgb(33, 88, 103);
                    style.BackgroundColor = Color.FromArgb(33, 88, 103);
                    style.Pattern = BackgroundType.Solid;
                    style.Font.Color = Color.White;
                    style.VerticalAlignment = TextAlignmentType.Center;
                    style.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
                    style.Borders[BorderType.TopBorder].Color = Color.Black;
                    style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
                    style.Borders[BorderType.BottomBorder].Color = Color.Black;
                    style.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
                    style.Borders[BorderType.LeftBorder].Color = Color.Black;
                    style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                    style.Borders[BorderType.RightBorder].Color = Color.Black;
                    range.ApplyStyle(style, st);

                    int Index = 1;
                    // Set header value
                    ws.Cells["A1"].PutValue("STT");
                    List<string> Cell = new List<string>() { "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "AA", "AB", "AC", "AD", "AE", "AF", "AG", "AH", "AI", "AJ", "AK", "AL", "AM", "AN", "AO", "AP" };
                    for (int I = 0; I < listfieldtext.Count; I++)
                    {
                        ws.Cells[Cell[I] + Index].PutValue(listfieldtext[I]);
                    }
                    #endregion

                    #region Body

                    range = cell.CreateRange(1, 0, data.Count, listfieldtext.Count + 1);
                    style = ws.Cells["A2"].GetStyle();
                    style.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
                    style.Borders[BorderType.TopBorder].Color = Color.Black;
                    style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
                    style.Borders[BorderType.BottomBorder].Color = Color.Black;
                    style.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
                    style.Borders[BorderType.LeftBorder].Color = Color.Black;
                    style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                    style.Borders[BorderType.RightBorder].Color = Color.Black;
                    style.VerticalAlignment = TextAlignmentType.Center;
                    range.ApplyStyle(style, st);

                    Style alignCenterStyle = ws.Cells["A2"].GetStyle();
                    alignCenterStyle.HorizontalAlignment = TextAlignmentType.Center;

                    Style numberStyle = ws.Cells[Cell[listfield.Count] + "2"].GetStyle();
                    numberStyle.Number = 3;
                    numberStyle.HorizontalAlignment = TextAlignmentType.Right;
                    numberStyle.VerticalAlignment = TextAlignmentType.Center;

                    int RowIndex = 1;

                    foreach (var item in data)
                    {
                        string ttchitiet = string.Empty;
                        var listfield2 = new List<int>();
                        listfield2.AddRange(listfield);
                        RowIndex++;
                        ws.Cells["A" + RowIndex].PutValue(RowIndex - 1);
                        ws.Cells["A" + RowIndex].SetStyle(alignCenterStyle);
                        for (int I = 0; I < listfieldtext.Count; I++)
                        {
                            for (int f = 0; f < listfield2.Count; f++)
                            {
                                if (listfield2[f] == 1)
                                {
                                    ws.Cells[Cell[I] + RowIndex].PutValue(item.OrderCode);
                                    listfield2.Remove(listfield2[f]);
                                    f--;
                                    break;
                                }
                                if (listfield2[f] == 2)
                                {
                                    ws.Cells[Cell[I] + RowIndex].PutValue(item.StartDate);
                                    I++;
                                    ws.Cells[Cell[I] + RowIndex].PutValue(item.EndDate);
                                    listfield2.Remove(listfield2[f]);
                                    f--;
                                    break;
                                }
                                if (listfield2[f] == 3)
                                {
                                    ws.Cells[Cell[I] + RowIndex].PutValue(item.ClientName);
                                    I++;
                                    ws.Cells[Cell[I] + RowIndex].PutValue(item.ClientNumber);
                                    I++;
                                    ws.Cells[Cell[I] + RowIndex].PutValue(item.ClientEmail);
                                    listfield2.Remove(listfield2[f]);
                                    f--;
                                    break;
                                }
                                if (listfield2[f] == 4)
                                {
                                    ws.Cells[Cell[I] + RowIndex].PutValue(item.Note);
                                    listfield2.Remove(listfield2[f]);
                                    f--;
                                    break;
                                }
                                if (listfield2[f] == 5)
                                {
                                    ws.Cells[Cell[I] + RowIndex].PutValue(item.Amount == 0 ? 0 : item.Amount);
                                    ws.Cells[Cell[I] + RowIndex].SetStyle(numberStyle);
                                    I++;
                                    ws.Cells[Cell[I] + RowIndex].PutValue(item.Payment == 0 ? 0 : item.Payment);
                                    ws.Cells[Cell[I] + RowIndex].SetStyle(numberStyle);
                                    I++;
                                    ws.Cells[Cell[I] + RowIndex].PutValue((item.Amount - item.Payment));
                                    ws.Cells[Cell[I] + RowIndex].SetStyle(numberStyle);
                                    listfield2.Remove(listfield2[f]);
                                    f--;
                                    break;
                                }
                                if (listfield2[f] == 6)
                                {
                                    ws.Cells[Cell[I] + RowIndex].PutValue(item.UtmSource);
                                    listfield2.Remove(listfield2[f]);
                                    f--;
                                    break;
                                }
                                if (listfield2[f] == 7)
                                {
                                    ws.Cells[Cell[I] + RowIndex].PutValue(item.Profit);
                                    ws.Cells[Cell[I] + RowIndex].SetStyle(numberStyle);
                                    listfield2.Remove(listfield2[f]);
                                    f--;
                                    break;
                                }
                                if (listfield2[f] == 8)
                                {
                                    ws.Cells[Cell[I] + RowIndex].PutValue(item.UtmMedium);
                                    listfield2.Remove(listfield2[f]);
                                    f--;
                                    break;
                                }
                                if (listfield2[f] == 9)
                                {
                                    switch (item.StatusCode)
                                    {
                                        case (int)(OrderStatus.CREATED_ORDER):
                                            {
                                                ws.Cells[Cell[I] + RowIndex].PutValue(item.Status);
                                                listfield2.Remove(listfield2[f]);

                                            }
                                            break;
                                        case (int)(OrderStatus.CONFIRMED_SALE):
                                        case (int)(OrderStatus.WAITING_FOR_OPERATOR):
                                            {

                                                ws.Cells[Cell[I] + RowIndex].PutValue(item.Status);
                                                listfield2.Remove(listfield2[f]);

                                            }
                                            break;
                                        case (int)(OrderStatus.WAITING_FOR_ACCOUNTANT):
                                            {
                                                ws.Cells[Cell[I] + RowIndex].PutValue(item.Status);
                                                listfield2.Remove(listfield2[f]);

                                            }
                                            break;
                                        case (int)(OrderStatus.FINISHED):
                                            {
                                                ws.Cells[Cell[I] + RowIndex].PutValue(item.Status);
                                                listfield2.Remove(listfield2[f]);

                                            }
                                            break;
                                        case ((int)(OrderStatus.CANCEL)):
                                        case ((int)(OrderStatus.ACCOUNTANT_DECLINE)):
                                        case ((int)(OrderStatus.OPERATOR_DECLINE)):
                                            {
                                                ws.Cells[Cell[I] + RowIndex].PutValue(item.Status);
                                                listfield2.Remove(listfield2[f]);

                                            }
                                            break;
                                    }
                                    f--;
                                    break;
                                }

                                if (listfield2[f] == 10)
                                {
                                    ws.Cells[Cell[I] + RowIndex].PutValue(item.CreateDate);
                                    listfield2.Remove(listfield2[f]);
                                    f--;
                                    break;
                                }
                                if (listfield2[f] == 11)
                                {
                                    ws.Cells[Cell[I] + RowIndex].PutValue(item.CreateName);
                                    listfield2.Remove(listfield2[f]);
                                    f--;
                                    break;

                                }
                                if (listfield2[f] == 12)
                                {
                                    ws.Cells[Cell[I] + RowIndex].PutValue(item.UpdateDate);
                                    listfield2.Remove(listfield2[f]);
                                    f--;
                                    break;

                                }
                                if (listfield2[f] == 13)
                                {
                                    ws.Cells[Cell[I] + RowIndex].PutValue(item.UpdateName);
                                    listfield2.Remove(listfield2[f]);
                                    f--;
                                    break;

                                }
                                if (listfield2[f] == 14)
                                {
                                    ws.Cells[Cell[I] + RowIndex].PutValue(item.SalerName + "\n" + item.SalerUserName + "\n" + item.SalerEmail);
                                    listfield2.Remove(listfield2[f]);
                                    f--;
                                    break;

                                }

                                if (listfield2[f] == 15)
                                {
                                    ws.Cells[Cell[I] + RowIndex].PutValue(item.SaleGroupName.TrimEnd(' ', ','));
                                    listfield2.Remove(listfield2[f]);
                                    f--;
                                    break;

                                }
                                if (listfield2[f] == 16)
                                {
                                    ws.Cells[Cell[I] + RowIndex].PutValue(item.Vouchercode);
                                    listfield2.Remove(listfield2[f]);
                                    f--;
                                    break;

                                }
                                if (listfield2[f] == 17)
                                {
                                    ws.Cells[Cell[I] + RowIndex].PutValue(item.OperatorIdName.TrimEnd(' ', ','));
                                    listfield2.Remove(listfield2[f]);
                                    f--;
                                    break;

                                }
                                if (listfield2[f] == 18)
                                {
                                    ws.Cells[Cell[I] + RowIndex].PutValue((item.PermisionTypeName == null || item.PermisionTypeName.Trim() == "" ? "Không công nợ" : item.PermisionTypeName) + " - " + @item.PaymentStatusName);
                                    listfield2.Remove(listfield2[f]);
                                    f--;
                                    break;

                                }

                                if (listfield2[f] == 19)
                                {

                                    DataTable dt_HotelBookingCode = await _hotelBookingCodeDAL.GetListHotelBookingCodeByOrderId(Convert.ToInt32(item.OrderId));
                                    if (dt_HotelBookingCode != null && dt_HotelBookingCode.Rows.Count > 0)
                                    {
                                        var ListHotelBookingCode = dt_HotelBookingCode.ToList<HotelBookingCodeModel>();
                                        ws.Cells[Cell[I] + RowIndex].PutValue(string.Join(",", ListHotelBookingCode.Select(x => x.BookingCode)));
                                        listfield2.Remove(listfield2[f]);
                                        f--;

                                    }

                                    break;

                                }
                            }


                        }



                    }
                    #endregion
                    wb.Save(FilePath);
                    pathResult = FilePath;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("ExportDeposit - OrderRepository: " + ex);
            }
            return pathResult;
        }
        public async Task<bool> ReCheckandUpdateOrderPayment(long OrderId)
        {
            try
            {
                var data = contractPayDAL.GetByOrderId(OrderId, ProcedureConstants.SP_GetListContractPayByOrderId).ToList<ContractPayViewModel>();
                var order = _OrderDal.GetByOrderId(OrderId);

                if (order != null && order.OrderId > 0 && order.Amount > 0 && data != null && data.Count > 0 && order.Amount <= data.Sum(x => x.AmountPayDetail))
                {
                    order.PaymentStatus = (int)PaymentStatus.PAID;
                    order.DebtStatus = (int)PaymentStatus.PAID;
                    _OrderDal.Update(order);
                }
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("ReCheckandUpdateOrderPayment - OrderRepository: " + ex);
                return false;
            }
        }
        public List<long> GetAllOrderIDs()
        {
            return _OrderDal.GetAllOrderIDs();
        }
        public async Task<bool> UndoContractPayByOrderId(long order_id, int user_summit)
        {
            try
            {
                var order = _OrderDal.GetByOrderId(order_id);
                if (order.OrderStatus == (int)OrderStatus.CANCEL)
                {
                    var list_contract_pay = _contractPayDAL.GetByOrderId(order_id, ProcedureConstants.SP_GetListContractPayByOrderId).ToList<ContractPayViewModel>();
                    if (list_contract_pay != null && list_contract_pay.Count > 0)
                    {
                        //_contractPayDAL.UpdateContractPayDetail(string.Join(",", list_contract_pay.Select(x => x.PayId)), order_id, (order.Amount == null ? 0 : (double)order.Amount), user_summit);
                        foreach (var contract in list_contract_pay)
                        {
                            _contractPayDAL.UndoContractPayByCancelService(contract.PayId, order_id, user_summit);
                        }

                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UndoContractPayByOrderId - OrderRepository: " + ex);
                return false;
            }

        }
        public async Task<TotalCountSumOrder> GetTotalCountSumOrder(OrderViewSearchModel searchModel, int currentPage, int pageSize)
        {
            var model = new TotalCountSumOrder();
            try
            {
                DataTable dt = await _OrderDal.GetPagingList(searchModel, currentPage, pageSize, ProcedureConstants.GET_TOTALCOUNT_ORDER);
                if (dt != null && dt.Rows.Count > 0)
                {


                    model.Profit = dt.Rows[0]["Profit"].Equals(DBNull.Value) ? 0 : Convert.ToDouble(dt.Rows[0]["Profit"]);
                    model.Amount = dt.Rows[0]["Amount"].Equals(DBNull.Value) ? 0 : Convert.ToDouble(dt.Rows[0]["Amount"]);
                    model.Price = dt.Rows[0]["Price"].Equals(DBNull.Value) ? 0 : Convert.ToDouble(dt.Rows[0]["Price"]);
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetTotalCountSumOrder in OrderRepository: " + ex);
            }
            return model;
        }
        public async Task<TotalCustomerCareFundViewModel> GetTotalCustomerCareFund(string ids, long ClientId)
        {
            try
            {
                return await _OrderDal.GetTotalCustomerCareFund(ids, ClientId);
            }catch(Exception ex)
            {
                LogHelper.InsertLogTelegram("GetTotalCustomerCareFund in OrderRepository: " + ex);
            }
            return null;
        }
        public async Task<long> OrderBookClosing(OrderBookClosingViewModel model)
        {
            try
            {
                //var CheckBookClosing =await CheckBookClosingByDate((DateTime)model.FromDate,model.ToDate);
                //if (CheckBookClosing > 0)
                //{
                //    return 0;
                //}
                //else
                //{
                //    return await _OrderDal.OrderBookClosing(model);

                //}
                return await _OrderDal.OrderBookClosing(model);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("OrderBookClosing in OrderRepository: " + ex);
            }
            return 0;
        }
        public async Task<long> CheckBookClosingByDate(DateTime FromDate, DateTime ToDate)
        {
            try
            {
                return await _OrderDal.CheckBookClosingByDate(FromDate, ToDate);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("CheckBookClosingByDate in OrderRepository: " + ex);
            }
            return 0;
        } 
        public async Task<long> UpdateBookClosingByOrderId(long OrderId, long IsLock, long UpdateBy)
        {
            try
            {
                return await _OrderDal.UpdateBookClosingByOrderId(OrderId, IsLock, UpdateBy);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateBookClosingByOrderId in OrderRepository: " + ex);
            }
            return 0;
        }
        public async Task<List<OrderBookClosingRequestViewModel>> GetListOrderBookClosingByOrderId(long OrderId )
        {
           
            try
            {
                DataTable dt = await _OrderDal.GetListOrderBookClosingByOrderId(OrderId);
                if (dt != null && dt.Rows.Count > 0)
                {

                    var data = dt.ToList<OrderBookClosingRequestViewModel>();

                    return data;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetTotalCountSumOrder in OrderRepository: " + ex);
            }
            return null;
        }
    }
}
