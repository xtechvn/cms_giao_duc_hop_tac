using Entities.Models;
using Entities.ViewModels;
using Entities.ViewModels.Funding;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositories.IRepositories
{
    public interface IPaymentRequestRepository
    {
        List<PaymentRequestViewModel> GetPaymentRequests(PaymentRequestSearchModel searchModel, out long total, int currentPage = 1, int pageSize = 20);
        List<CountStatus> GetCountStatus(PaymentRequestSearchModel searchModel);
        PaymentRequestViewModel GetById(int paymentRequestId);
        string ExportPaymentRequest(PaymentRequestSearchModel searchModel, string FilePath);
        int CreatePaymentRequest(PaymentRequestViewModel model);
        int UpdatePaymentRequest(PaymentRequestViewModel model);
        int RejectPaymentRequest(string paymentRequestNo, string noteReject, int userId);
        int UndoApprove(string paymentRequestNo, string note, int userId, int status);
        int ApprovePaymentRequest(string paymentRequestNo, int userId, int status);
        PaymentRequest GetByRequestNo(string paymentRequestNo);
        int DeletePaymentRequest(string paymentRequestNo, int userId);
        List<PaymentRequestViewModel> GetServiceListBySupplierId(long supplierId, int requestId = 0, int serviceId = 0);
        List<PaymentRequestViewModel> GetServiceListByClientId(long clientId, int requestId = 0);
        List<PaymentRequestViewModel> GetByClientId(long clientId, long Type, int paymentVoucherId = 0);
        List<PaymentRequestViewModel> GetBySupplierId(long supplierId, int paymentVoucherId = 0, string requestType="1,2");
        List<PaymentRequestViewModel> GetByServiceId(long serviceId, int type, int requestType = 0);
        List<PaymentRequestViewModel> GetRequestByClientId(long clientId, long orderid = 0);
        List<OrderPaymentRequest> GetListPaymentRequestByOrderId(int Orderid);
    }
}
