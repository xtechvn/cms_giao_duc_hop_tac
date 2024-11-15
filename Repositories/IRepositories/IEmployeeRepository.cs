using Entities.ViewModels;
using Entities.ViewModels.Report;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositories.IRepositories
{
    public interface IEmployeeRepository
    {
        GenericViewModel<ReportEmployeeViewModel> GetEmployeeRevenue(SearchReportEmployeeViewModel searchModel, int currentPage, int pageSize);
        string ExportDeposit(SearchReportEmployeeViewModel searchModel, string FilePath, int currentPage, int pageSize);
    }
}
