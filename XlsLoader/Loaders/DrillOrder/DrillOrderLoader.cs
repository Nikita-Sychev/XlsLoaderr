using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Contracts;
using DocumentFormat.OpenXml.Packaging;
using XlsLoader.Loaders.DrillOrder.Sheets;
using XlsLoader.Models.Common;
using XlsLoader.Models.Dictionary;
using XlsLoader.Models.Loader.DrillOrder.Output;
using XlsLoader.Services;
using XlsLoader.Excel;

namespace XlsLoader.Loaders.DrillOrder
{
    public class DrillOrderLoader : BaseLoader
    {
        private readonly IUserIdentity _userIdentity;
        public Stream FileStream { get; set; }
        private DictionarysDrillOrder _dicData;
        private EstimateWell _sheetWorker;
        private Title _sheetTitleWorker;

        private DrillOrderService _drillOrderService;
        private DictionaryService _dicService = null;

        private DrillOrderService DrillOrderService =>
            _drillOrderService ?? (_drillOrderService = new DrillOrderService(_userIdentity));
        private DictionaryService DicService => _dicService ?? (_dicService = new DictionaryService(_userIdentity));

        public DrillOrderLoader(IUserIdentity userIdentity)
        {
            _userIdentity = userIdentity;
        }

        public Task<bool> UploadDrillOrderExcelAsync(DrillOrderInputData inputData, DictionarysDrillOrder dicData)
        {
            _dicData = dicData;
            _sheetWorker = new EstimateWell(_dicData);
            _sheetTitleWorker = new Title(_dicData);
            return Task.Run(() => UploadDrillOrderExcel(inputData));
        }

        private bool UploadDrillOrderExcel(DrillOrderInputData inputData)
        {
            const string sheetTitleName = "Титул";
            try
            {
                using (var stream = new MemoryStream(inputData.File))
                {
                    using (var doc = SpreadsheetDocument.Open(stream, false))
                    {
                        //---загрузка титульного листа
                        var titleSheet = OpenXml.GetSheetData(sheetTitleName, doc.WorkbookPart);
                        var outputTitleData = _sheetTitleWorker.PrepareLoadData(titleSheet);
                        outputTitleData.WorkOrderId = inputData.OrderId;
                        DrillOrderService.LoadExcelTitleWorkOrder(outputTitleData);

                        //---получить keys скважин по которым будем загружть листы со скважиными
                        //ключи по скважинам создаются при загрузке листа Титул
                        GetDicWispModuleWell(inputData.OrderId ?? 0, outputTitleData);

                        //---загрузка листов со сметной стоимостью скважин
                        var sheetsName = _sheetTitleWorker.GetSheetNamesForLoad(titleSheet, out var someNames);
                        for (var i = 0; i < sheetsName.Count; i++)
                        {
                            var sheet = OpenXml.GetSheetData(sheetsName[i], doc.WorkbookPart);
                            var outputData = _sheetWorker.PrepareLoadData(sheet);
                            outputData.WorkOrderId = inputData.OrderId;
                            outputData.VirtWell = _dicData.WispModuleWell.FirstOrDefault(f => f.Name.Equals(someNames[i], StringComparison.CurrentCultureIgnoreCase))?.Key;
                            DrillOrderService.LoadExcelWorkOrder(outputData);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return true;
        }

        private void GetDicWispModuleWell(decimal workOrderId, TitleDrill outTitleData)
        {
            _dicData.WispModuleWell = DicService.GetWispModuleWell(workOrderId, PrepareWellsName(outTitleData));
        }

        private IEnumerable<string> PrepareWellsName(TitleDrill outTitleData)
        {
            return outTitleData.TableData3.Value.Where(w => w.SideId != null).Select(s => s.SideId.ToString()).Distinct();
        }
    }
}
