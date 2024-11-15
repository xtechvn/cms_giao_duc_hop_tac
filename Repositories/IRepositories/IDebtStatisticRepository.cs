using Entities.ViewModels;
using Entities.ViewModels.Funding;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositories.IRepositories
{
    public interface IDebtStatisticRepository
    {
        List<DebtStatisticViewModel> GetDebtStatistics(DebtStatisticViewModel searchModel, out long total, int currentPage = 1, int pageSize = 20);
        List<CountStatus> GetCountStatus(DebtStatisticViewModel searchModel);
        DebtStatisticViewModel GetDetailDebtStatistic(int debtStatisticId);
        string ExportDebtStatistic(DebtStatisticViewModel searchModel, string FilePath);
        string ExportDebtStatistic_OrderList(DebtStatisticViewModel searchModel, string FilePath);
        int CreateDebtStatistic(DebtStatisticViewModel model);
        int UpdateDebtStatistic(DebtStatisticViewModel model);
        int RejectDebtStatistic(string debtStatisticNo, string noteReject, int userId);
        int ApproveDebtStatistic(string debtStatisticNo, int userId, int status);
        int CancelDebtStatistic(string debtStatisticNo, int userId);
        string CheckApproveDebtStatistic(string debtStatisticNo, out string orderNoList);
        List<DebtStatisticViewModel> GetOrderListByClientId(long clientId, DateTime fromDate, DateTime toDate, int debtStatisticId = 0, bool isDetail = false);
        int DeleteDebtStatistic(string debtStatisticNo, int userId);
    }
}
