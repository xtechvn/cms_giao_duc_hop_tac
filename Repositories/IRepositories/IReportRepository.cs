using Entities.ViewModels;
using Entities.ViewModels.Report;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.IRepositories
{
    public interface IReportRepository
    {
        public Task<GenericViewModel<OperatorReportViewModel>> GetOperatorReport(OperatorReportSearchModel searchModel, int currentPage = 1, int pageSize = 20);
        public Task<string> ExportOperatorExcel(GenericViewModel<OperatorReportViewModel> model,string file_path);
        Task<SumOperatorReportViewModel> GetSumOperatorReport(OperatorReportSearchModel searchModel);
        public Task<GenericViewModel<ReportClientDebtViewModel>> GetTotalDebtRevenueByClient(ReportClientDebtSearchModel searchModel);
        public Task<string> ExportTotalDebtRevenueByClient(GenericViewModel<ReportClientDebtViewModel> model, string file_path);
        public Task<GenericViewModel<ReportDetailClientDebtViewModel>> GetDetailDebtRevenueByClient(ReportDetailClientDebtSearchModel searchModel);
        public Task<string> ExportDetailDebtRevenueByClient(GenericViewModel<ReportDetailClientDebtViewModel> model, string file_path, ReportDetailClientDebtSearchModel searchModel);

    }
}
