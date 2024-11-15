using Aspose.Cells;
using DAL.Programs;
using Entities.ConfigModels;
using Entities.ViewModels;
using Entities.ViewModels.Programs;
using Microsoft.Extensions.Options;
using Repositories.IRepositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace Repositories.Repositories
{
   public class ProgramsReprository: IProgramsReprository
    {
        private readonly ProgramsDAL _programsDAL;
        public ProgramsReprository(IOptions<DataBaseConfig> dataBaseConfig)
        {
            _programsDAL = new ProgramsDAL(dataBaseConfig.Value.SqlServer.ConnectionString);

        }
        public async Task<GenericViewModel<ProgramsViewModel>> SearchPrograms(ProgramsSearchViewModel searchModel)
        {
            var model = new GenericViewModel<ProgramsViewModel>();
            try
            {

                DataTable dt = await _programsDAL.GetPagingList(searchModel);
                if (dt != null && dt.Rows.Count > 0)
                {
                    model.ListData = dt.ToList<ProgramsViewModel>();
                    model.CurrentPage = searchModel.PageIndex;
                    model.PageSize = searchModel.PageSize;
                    model.TotalRecord = Convert.ToInt32(dt.Rows[0]["TotalRow"]);
                    model.TotalPage = (int)Math.Ceiling((double)model.TotalRecord / model.PageSize);

                }
                return model;

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("SearchPrograms - ProgramsReprository: " + ex);
            }
            return null;
        }
        public async Task<ProgramsViewModel> DetailPrograms(long id)
        {
            
            try
            {
                DataTable dt = await _programsDAL.getDetailById(id);
                if (dt != null && dt.Rows.Count > 0)
                {
                    var data = dt.ToList<ProgramsViewModel>();

                    return data[0];
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("SearchPrograms - ProgramsReprository: " + ex);
            }
            return null;
        }
        public async Task<int> InsertPrograms(ProgramsModel Model)
        {

            try
            {
                return await _programsDAL.InsertPrograms(Model);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("InsertPrograms - ProgramsReprository: " + ex);
            }
            return 0;
        }
        public async Task<int> UpdatePrograms(ProgramsModel Model)
        {

            try
            {
                return await _programsDAL.UpdatePrograms(Model);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdatePrograms - ProgramsReprository: " + ex);
            }
            return 0;
        }
        public async Task<int> UpdateProgramsStatus(int status,long id,long userid)
        {

            try
            {
                return await _programsDAL.UpdateProgramsStatus(status, id, userid);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateProgramsStatus - ProgramsReprository: " + ex);
            }
            return 0;
        }
        public async Task<List<Entities.ViewModels.Programs.HotelModel>> GetlistHotelBySupplierId(long id)
        {

            try
            {
                return await _programsDAL.GetlistHotelBySupplierId(id);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetlistHotelBySupplierId - ProgramsReprository: " + ex);
            }
            return null;
        }
        public async Task<List<Entities.ViewModels.Programs.SupplierModel>> GetlistSupplierByHotelId(long id)
        {

            try
            {
                return await _programsDAL.GetlistSupplierByHotel(id);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetlistHotelBySupplierId - ProgramsReprository: " + ex);
            }
            return null;
        }
        public async Task<GenericViewModel<ProgramsViewModel>> GetlistHotelBySupplierId(ProgramsSearchSupplierId searchModel)
        {

            try
            {
                var model = new GenericViewModel<ProgramsViewModel>();
                DataTable dt=await _programsDAL.GetlistHotelBySupplierId(searchModel);
                if (dt != null && dt.Rows.Count > 0)
                {
                    model.ListData = dt.ToList<ProgramsViewModel>();
                    model.CurrentPage = searchModel.PageIndex;
                    model.PageSize = searchModel.PageSize;
                    model.TotalRecord = Convert.ToInt32(dt.Rows[0]["TotalRow"]);
                    model.TotalPage = (int)Math.Ceiling((double)model.TotalRecord / model.PageSize);

                }
                return model;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetlistHotelBySupplierId - ProgramsReprository: " + ex);
            }
            return null;
        }

        public async Task<string> ExportDeposit(ProgramsSearchViewModel searchModel, string FilePath, FieldPrograms field)
        {
            var pathResult = string.Empty;
            try
            {
                searchModel.PageIndex = -1;
                var data = new List<ProgramsViewModel>();
                DataTable dt = await _programsDAL.GetPagingList(searchModel);
                if (dt != null && dt.Rows.Count > 0)
                {
                    data = dt.ToList<ProgramsViewModel>();
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
   
                    if (field.ProgramCode) { listfieldtext.Add("Mã"); listfield.Add(2); }
                    if (field.ProgramNamef)
                    {
                        listfieldtext.Add("Tên"); listfield.Add(3);
            
                    }
                    if (field.ServiceType) { listfieldtext.Add("Loại dịch vụ"); listfield.Add(4); }
                    if (field.ServiceNamef) { listfieldtext.Add("Tên dịch vụ"); listfield.Add(5); }
                    if (field.SupplierNamef) { listfieldtext.Add("Nhà cung cấp"); listfield.Add(6); }
                    if (field.ApplyDate) { listfieldtext.Add("Thời gian áp dụng"); listfield.Add(7); }
                    if (field.Descriptionf) { listfieldtext.Add("Mô tả"); listfield.Add(8); }
                    if (field.UserCreate) { listfieldtext.Add("Người tạo"); listfield.Add(9); }
                    if (field.CreateDatef) { listfieldtext.Add("Ngày tạo"); listfield.Add(10); }
                    if (field.UserVerify) { listfieldtext.Add("Người duyệt"); listfield.Add(11); }
                    if (field.VerifyDate) { listfieldtext.Add("Ngày duyệt"); listfield.Add(12); }
                   


                    cell.SetColumnWidth(0, 12);
                    for (int i = 1; i <= listfield.Count; i++)
                    {
                        cell.SetColumnWidth(i, 40);
                    }

                    range = cell.CreateRange(0, 0, 1, listfield.Count + 1);
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
                    List<string> Cell = new List<string>() { "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R" };
                    for (int I = 0; I < listfield.Count; I++)
                    {
                        ws.Cells[Cell[I] + Index].PutValue(listfieldtext[I]);
                    }
                    #endregion

                    #region Body

                    range = cell.CreateRange(1, 0, data.Count, listfield.Count + 1);
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
                        for (int I = 0; I < listfield.Count; I++)
                        {
                            for (int f = 0; f < listfield2.Count; f++)
                            {
                               
                                if (listfield2[f] == 2)
                                {
                                    ws.Cells[Cell[I] + RowIndex].PutValue(item.ProgramCode);
                                    listfield2.Remove(listfield2[f]);
                                    f--;
                                    break;
                                }
                                if (listfield2[f] == 3)
                                {
                                    
                                    ws.Cells[Cell[I] + RowIndex].PutValue(item.ProgramName +"-"+item.ProgramStatus  );
                                    listfield2.Remove(listfield2[f]);
                                    f--;
                                    break;
                                }
                                if (listfield2[f] == 4)
                                {
                                    //ws.Cells[Cell[I] + RowIndex].PutValue(item.ServiceTypeName);
                                    listfield2.Remove(listfield2[f]);
                                    f--;
                                    break;
                                }
                                if (listfield2[f] == 5)
                                {
                                    ws.Cells[Cell[I] + RowIndex].PutValue(item.ServiceName );
                                    listfield2.Remove(listfield2[f]);
                                    f--;
                                    break;
                                }
                                if (listfield2[f] == 6)
                                {
                                    ws.Cells[Cell[I] + RowIndex].PutValue(item.FullName);
                                    listfield2.Remove(listfield2[f]);
                                    f--;
                                    break;
                                }
                                if (listfield2[f] == 7)
                                {
                                    ws.Cells[Cell[I] + RowIndex].PutValue(Convert.ToDateTime(item.StartDate).ToString("dd/MM/yyyy")+" - "+ Convert.ToDateTime(item.EndDate).ToString("dd/MM/yyyy"));
                                    listfield2.Remove(listfield2[f]);
                                    f--;
                                    break;
                                }
                                if (listfield2[f] == 8)
                                {
                                    ws.Cells[Cell[I] + RowIndex].PutValue(item.Description);
                                    listfield2.Remove(listfield2[f]);
                                      f--;
                                    break;
                                 
                                }

                                if (listfield2[f] == 9)
                                {
                                    ws.Cells[Cell[I] + RowIndex].PutValue(item.UserName);
                                    listfield2.Remove(listfield2[f]);
                                    f--;
                                    break;
                                }
                                if (listfield2[f] == 10)
                                {
                                    ws.Cells[Cell[I] + RowIndex].PutValue(Convert.ToDateTime(item.CreatedDate).ToString("dd/MM/yyyy"));
                                    listfield2.Remove(listfield2[f]);
                                    f--;
                                    break;

                                }
                                if (listfield2[f] == 11)
                                {
                                    ws.Cells[Cell[I] + RowIndex].PutValue(item.UserVerifyName);
                                    listfield2.Remove(listfield2[f]);
                                    f--;
                                    break;

                                }
                                if (listfield2[f] == 12)
                                {
                                    ws.Cells[Cell[I] + RowIndex].PutValue(Convert.ToDateTime(item.VerifyDate).ToString("dd/MM/yyyy"));
                                    listfield2.Remove(listfield2[f]);
                                    f--;
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
                LogHelper.InsertLogTelegram("ExportDeposit - ProgramsReprository: " + ex);
            }
            return pathResult;
        }
    }
}
