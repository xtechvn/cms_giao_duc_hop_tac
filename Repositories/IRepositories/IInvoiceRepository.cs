using Entities.Models;
using Entities.ViewModels;
using Entities.ViewModels.Funding;
using Entities.ViewModels.Invoice;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repositories.IRepositories
{
    public interface IInvoiceRepository
    {
        List<InvoiceViewModel> GetInvoices(InvoiceSearchModel searchModel, out long total, int currentPage = 1, int pageSize = 20);
        InvoiceViewModel GetById(int invoiceId);
        string ExportInvoice(InvoiceSearchModel searchModel, string FilePath);
        int CreateInvoice(InvoiceViewModel model);
        int UpdateInvoice(InvoiceViewModel model);
        int DeleteInvoice(int invoiceId, int userId);
        Task<List<InvoiceCodeViewModel>> GetListInvoiceCodebyOrderId(string orderIds);
        Task<List<InvoiceRequestViewModel>> GetListInvoiceRequestbyOrderId(string orderIds);
    }
}
