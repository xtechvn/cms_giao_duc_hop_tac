using System;
using System.Collections.Generic;
using System.Text;
using Entities.Models;

namespace Entities.ViewModels.Contract
{
	public class ContractViewModel : Entities.Models.Contract
	{

		public string SalerId_Name { get; set; }
		public string ClientId_Name { get; set; }
		public string VerifyStatus_Name { get; set; }
		public string UserIdCreate_Name { get; set; }
		public string UserIdUpdate_Name { get; set; }
		public string AgencyType_Name { get; set; }
		public string PermisionType_Name { get; set; }
		public string ClienType_Name { get; set; }
		public string asDebtType_Name { get; set; }
		public string ContractStatus_Name { get; set; }
		public double ProductFlyTicketDebtAmount { get; set; }
		public double HotelDebtAmout { get; set; }
		public double ProductFlyTicketDepositAmount { get; set; }
		public double HotelDepositAmout { get; set; }
		public double VinWonderDebtAmount { get; set; }
		public double TourDebtAmount { get; set; }
		public double TouringCarDebtAmount { get; set; }
		public double VinWonderDepositAmount { get; set; }
		public double TourDepositAmount { get; set; }
		public double TouringCarDepositAmount { get; set; }
		public string ContractExpireDate { get; set; }
		public string ContractExpireStatus { get; set; }
		public int ContractExpire { get; set; }

		public string ClientNote { get; set; }
		

		public Array ServiceType_Name { get; set; }
		public string ServiceTypeName { get; set; }

		public string Client_Phone { get; set; }
		public string Client_Email { get; set; }
		public int ContractViewModel_Statuse { get; set; }

		public int? IsPrivate { get; set; }
		public int? Id { get; set; }
	}
	public class ContractSearchViewModel
	{
		public string VerifyStatus { get; set; }
		public int ClientId { get; set; } = -1;
		public string ContractNo { get; set; }
		public string ClientName { get; set; }
		public string Phone { get; set; }
		public string Email { get; set; }
		public string ContractStatus { get; set; }
		public string ContractExpire { get; set; }
		public string ClientAgencyType { get; set; }
		public string ClientType { get; set; }
		public string PermissionType { get; set; }
		public string ExpireDateFrom { get; set; }
		public string ExpireDateTo { get; set; }
		public string DebtType { get; set; }
		public string SalerId { get; set; }
		public string UserCreate { get; set; }
		public string CreateDateFrom { get; set; }
		public string CreateDateTo { get; set; }
		public string UserVerify { get; set; }
		public string VerifyDateFrom { get; set; }
		public string VerifyDateTo { get; set; }
		public string SalerPermission { get; set; }
	
		public int PageIndex { get; set; } = -1;
		public int PageSize { get; set; } = -1;

	}
	public class ContractModel {
		public long ContractId { get; set; }
		public string ContractNo { get; set; }
		public DateTime ContractDate { get; set; }
		public DateTime ExpireDate { get; set; }
		public int ClientId { get; set; }
		public int SalerId { get; set; }
		public DateTime CreateDate { get; set; }
		public DateTime UpdateLast { get; set; }
		public byte VerifyStatus { get; set; }
		public int? UserIdCreate { get; set; }
		public int? UserIdUpdate { get; set; }
		public int? UserIdVerify { get; set; }
		public DateTime VerifyDate { get; set; }
		public int? TotalVerify { get; set; }
		public int? ContractStatus { get; set; }
		public int? DebtType { get; set; }
		public int? UpdatedBy { get; set; }
		public string ServiceType { get; set; }
		public int? PolicyId { get; set; }
		public string Note { get; set; }
		public int? ClientType { get; set; }
		public int? PermisionType { get; set; }
	}
	public class TotalConTract
	{
		public int Total { get; set; }
		public int ContractStatus { get; set; }
		public string ContractStatusName { get; set; }
	}
}
