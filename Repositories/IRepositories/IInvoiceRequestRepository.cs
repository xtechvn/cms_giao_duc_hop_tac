using Entities.Models;
using Entities.ViewModels;
using Entities.ViewModels.Funding;
using Entities.ViewModels.Invoice;
using System.Collections.Generic;

namespace Repositories.IRepositories
{
    public interface IInvoiceRequestRepository
    {
        List<InvoiceRequestViewModel> GetInvoiceRequests(InvoiceRequestSearchModel searchModel, out long total, int currentPage = 1, int pageSize = 20);
        List<InvoiceRequestViewModel> GetInvoiceRequestByOrderId(string orderId);
        InvoiceRequestViewModel GetById(int invoiceRequestId);
        string ExportInvoiceRequest(InvoiceRequestSearchModel searchModel, string FilePath);
        int CreateInvoiceRequest(InvoiceRequestViewModel model);
        int UpdateInvoiceRequest(InvoiceRequestViewModel model);
        int RejectRequest(int invoiceRequestId, string noteReject, int userId);
        int ApproveInvoiceRequest(int invoiceRequestId, int userId, int status);
        int DeleteInvoiceRequest(int invoiceRequestId, int userId);
        List<CountStatus> GetCountStatus(InvoiceRequestSearchModel searchModel);
        List<OrderViewModel> GetByClientId(long clientId, int invoiceRequestId = 0, int status = 0);
        List<InvoiceRequestViewModel> GetInvoiceRequestsByClientId(long clientId, int invoiceId = 0);
        List<InvoiceRequestViewModel> GetInvoiceRequestsByInvoiceId(long invoiceId);
        List<InvoiceRequestHistoryViewModel> GetHistoriesByRequestId(long requestId);
        int FinishInvoiceRequest(InvoiceRequestViewModel model);
    }
}
