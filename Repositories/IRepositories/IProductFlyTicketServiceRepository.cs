using Entities.Models;
using Entities.ViewModels.PricePolicy;

namespace Repositories.IRepositories
{
    public interface IProductFlyTicketServiceRepository
    {
        ProductFlyTicketService GetByCampaignID(int campaignId);
        int AddCampaginAndProduct(PricePolicySummitModel model, int userId);
        int Update(ProductFlyTicketService model);
    }
}
