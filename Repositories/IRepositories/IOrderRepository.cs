using Entities.Models;
using Entities.ViewModels;
using Entities.ViewModels.OrderManual;
using Entities.ViewModels.Report;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.IRepositories
{
    public interface IOrderRepository
    {
        Task<GenericViewModel<OrderViewModel>> GetTotalCountOrder(OrderViewSearchModel searchModel, int currentPage, int pageSize);
        Task<GenericViewModel<OrderViewModel>> GetList(OrderViewSearchModel searchModel, int currentPage, int pageSize);
        Task<Order> CreateOrder(Order order);
        List<OrderViewModel> GetByClientId(long clientId, int payId = 0, int status = 0);
        /// <summary>
        /// Function get order list for payment request
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="payId"></param>
        /// <returns></returns>
        List<OrderViewModel> GetOrderByClientId(long clientId, int payId = 0);
        List<OrderViewModel> GetBySupplierId(long clientId, int payId = 0);
        Task<Order> GetOrderByID(long id);
        Task<Order> GetOrderByOrderNo(string orderNo);
        Task<double> UpdateOrderDetail(long OrderId, long user_id);
        Task<int> UpdateOrderStatus(long OrderId, long Status, long UpdatedBy, long UserVerify);
        Task<List<OrderServiceViewModel>> GetAllServiceByOrderId(long OrderId);
        public int UpdateOrder(Order model);
        Task<long> UpdateOrderSaler(long order_id, int user_commit);
        Task<List<ProductServiceName>> ProductServiceName(string OrderId);
        Task<int> IsClientAllowedToDebtNewService(double service_amount, long client_id, long order_id, int service_type);
        public int UpdateOrderOperator(long order_id);
        Task<long> UpdateOrderFinishPayment(long OrderId,long Status);
        Task<long> UpdateServiceStatusByOrderId(long OrderId, long StatusFilter, long Status);
        Task<long> RePushDeclineServiceToOperator(long OrderId);
        Task<string> ExportDeposit(OrderViewSearchModel searchModel, string FilePath, FieldOrder field, int currentPage, int pageSize);
        Task<bool> ReCheckandUpdateOrderPayment(long OrderId);
        public List<long> GetAllOrderIDs();
        Task<long> UpdateAllServiceStatusByOrderId(long OrderId, long Status);
        Task<bool> UndoContractPayByOrderId(long order_id, int user_summit);
        Task<TotalCountSumOrder> GetTotalCountSumOrder(OrderViewSearchModel searchModel, int currentPage, int pageSize);
        Task<TotalCustomerCareFundViewModel> GetTotalCustomerCareFund(string ids, long ClientId);
        Task<long> OrderBookClosing(OrderBookClosingViewModel model);
        Task<long> UpdateBookClosingByOrderId(long OrderId, long IsLock, long UpdateBy);
        Task<long> CheckBookClosingByDate(DateTime FromDate, DateTime ToDate);
        Task<long> UpdateOrderAmountFundCustomer(long order_id);
        Task<List<OrderBookClosingRequestViewModel>> GetListOrderBookClosingByOrderId(long OrderId);
    }
}
