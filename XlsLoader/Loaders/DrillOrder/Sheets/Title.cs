using System;
using System.Collections.Generic;
using System.Linq;
using XlsLoader.Models.Common;
using XlsLoader.Models.Dictionary;
using XlsLoader.Models.Excel;
using XlsLoader.Models.Loader.DrillOrder.Output;
using XlsLoader.Models.Udt.DrillOrder;

namespace XlsLoader.Loaders.DrillOrder.Sheets
{
    public class Title : BaseLoader
    {
        private readonly DictionarysDrillOrder _dicData;
        private List<CommonOrderDictionary> _titleTab3Mapp;
        private readonly Dictionary<int, decimal?> _sideTab1Mapp;

        public Title(DictionarysDrillOrder dic)
        {
            _dicData = dic;
            _sideTab1Mapp = PrepareTab1SideMapping();
        }

        /// <summary>
        /// получить список имен листов excel для зугрузки данных
        /// </summary>
        /// <returns></returns>
        public List<string> GetSheetNamesForLoad(XlsSheet titleSheet, out List<string> someNames)
        {
            const string tabName = "тэхнико-экономические показатели строительства по проекту:";
            const int firstRowShift = 5;
            const string colWellName = "B";
            const string colWellVal = "M";
            const string endRow = "Среднее по ННС";
            var result = new List<string>();
            someNames = new List<string>();

            foreach (var row in titleSheet.Rows)
            {
                if (row.Cells.Count > 0 && !row.Cells[1].Value.ToString().Trim()
                        .Equals(tabName, StringComparison.CurrentCultureIgnoreCase)) continue;
                var rowIdx = (int)row.Cells[0].Index - 1; //индекс элемента коллекции листов

                for (var i = rowIdx + firstRowShift; i < rowIdx + titleSheet.Rows.Count; i++)
                {
                    var cellWellName = titleSheet.Rows[i].Cells.FirstOrDefault(w => w.ColumnName == colWellName);
                    if (string.IsNullOrEmpty(cellWellName?.Value.ToString()) || cellWellName.Value.ToString().Trim()
                            .Equals(endRow, StringComparison.CurrentCultureIgnoreCase)) break;

                    var cellWellVal = titleSheet.Rows[i].Cells.FirstOrDefault(w => w.ColumnName == colWellVal);
                    if (string.IsNullOrEmpty(cellWellVal?.Value.ToString())) continue;

                    var nameSplit = cellWellName.Value.ToString().Trim().Split(' ').FirstOrDefault();

                    var name = cellWellName.Value.ToString().IndexOf("Первая", StringComparison.OrdinalIgnoreCase) != -1
                        ? "Перв. скв"
                        : cellWellName.Value.ToString().IndexOf("Последняя", StringComparison.OrdinalIgnoreCase) != -1
                            ? "Посл. скв"
                            : cellWellName.Value.ToString().Trim();


                    result.Add(name);
                    someNames.Add(string.IsNullOrEmpty(nameSplit) ? name : nameSplit);
                }
                break;
            }
            return result;
        }

        /// <summary>
        /// Основной метод чтения и компановки двнных с листа Excel
        /// </summary>
        /// <returns></returns>
        public TitleDrill PrepareLoadData(XlsSheet sheet)
        {
            const string tab3EndWord = "Среднее по ННС";
            var startRowForTab4 = 0;
            var rows = sheet.Rows;
            _titleTab3Mapp = PrepareTab3DicMapping(new List<XlsRow> { rows[24], rows[26] }, 1, 11);
            _titleTab3Mapp.AddRange(PrepareTab3DicMapping(new List<XlsRow> { rows[24], rows[25] }, 11, 15));
            var tableData1 = new CellsWorkOrder();
            var tableData2 = new CellsWorkOrder();
            var tableData3 = new CellsWorkOrder();
            var tableData4 = new CellsWorkOrder();
            var tableData1List = new List<CellWorkOrder>();
            var tableData2List = new List<CellWorkOrder>();
            var tableData3List = new List<CellWorkOrder>();
            var tableData4List = new List<CellWorkOrder>();

            //--Шапка листа
            for (var i = 8; i < 15; i++)
                tableData1List.AddRange(GetTab1Data(rows[i]));
            tableData1.Value = tableData1List.ToArray();

            //---Ответственные специалисты по проекту            
            tableData2List.AddRange(GetTab2Data(rows[17])); //По технико-технологическим показателям            
            tableData2List.AddRange(GetTab2Data(rows[19])); //По экономическим показателям
            tableData2.Value = tableData2List.ToArray();

            //---Тэхнико-экономические показатели строительства по проекту
            for (var i = 27; i < rows.Count; i++)
            {
                var sideVal = rows[i].Cells[1].Value?.ToString().Trim();
                if (string.IsNullOrEmpty(sideVal) ||
                    sideVal.Equals(tab3EndWord, StringComparison.CurrentCultureIgnoreCase))
                {
                    startRowForTab4 = i;
                    break;
                }
                tableData3List.AddRange(GetTable3Data(rows[i]));
            }
            tableData3.Value = tableData3List.ToArray();

            //---Описание и особенности проекта
            tableData4List.AddRange(GetTab4Data(startRowForTab4, rows));
            tableData4.Value = tableData4List.ToArray();

            return new TitleDrill
            {
                TableData1 = tableData1,
                TableData2 = tableData2,
                TableData3 = tableData3,
                TableData4 = tableData4
            };
        }

        /// <summary>
        /// компановка данных Шапка листа
        /// </summary>
        /// <returns></returns>
        private List<CellWorkOrder> GetTab1Data(XlsRow row)
        {
            var result = new List<CellWorkOrder>();
            const int sideColIdx = 1;
            const int valueColIdx = 10;

            _sideTab1Mapp.TryGetValue((int)row.Cells[sideColIdx].Index, out var sideId);
            if (sideId == null) return result;
            string value;
            if (row.Cells[valueColIdx].AddressName == "K9" || row.Cells[valueColIdx].AddressName == "K10")
                value = ConvertToOrclDate(row.Cells[valueColIdx]?.Value.ToString());
            else
                value = row.Cells[valueColIdx]?.Value.ToString();
            if (string.IsNullOrEmpty(value)) return result;

            result.Add(new CellWorkOrder
            {
                SideId = sideId,
                TitleId = 1,
                Value = value
            });

            return result;
        }

        private string ConvertToOrclDate(string value)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;
            var date = value.Replace(".", string.Empty).Split(' ');
            return date.Length > 0 ? "5" + date[0] : string.Empty;
        }

        /// <summary>
        /// компановка данных Ответственные специалисты по проекту 
        /// </summary>
        /// <returns></returns>
        private List<CellWorkOrder> GetTab2Data(XlsRow row)
        {
            var result = new List<CellWorkOrder>(3);
            var value1 = row.Cells[10]?.Value.ToString();
            var value2 = row.Cells[15]?.Value.ToString();
            var value3 = row.Cells[16]?.Value.ToString();
            _sideTab1Mapp.TryGetValue((int)row.Cells[1].Index, out var sideId);
            if (sideId == null) return result;

            if (!string.IsNullOrEmpty(value1))
                result.Add(new CellWorkOrder { Value = value1, TitleId = 1, SideId = sideId });
            if (!string.IsNullOrEmpty(value2))
                result.Add(new CellWorkOrder { Value = value2, TitleId = 2, SideId = sideId });
            if (!string.IsNullOrEmpty(value3))
                result.Add(new CellWorkOrder { Value = value3, TitleId = 3, SideId = sideId });

            return result;
        }

        /// <summary>
        /// компановка данных Тэхнико-экономические показатели строительства по проекту
        /// </summary>
        /// <returns></returns>
        private List<CellWorkOrder> GetTable3Data(XlsRow row)
        {
            var result = new List<CellWorkOrder>();
            decimal? sideId = null;
            var parts = row.Cells[1].Value?.ToString().Split('(');
            var input = parts != null && parts.Length > 1 ? parts[0]?.Trim() : row.Cells[1].Value?.ToString().Trim();

            if (decimal.TryParse(input, out var res))
                sideId = res;
            else
                return result;

            for (var i = 2; i < 15; i++)
            {
                var titleId = _titleTab3Mapp.FirstOrDefault(f => f.MappVal == row.Cells[i].ColumnName)?.Key;
                if (titleId == null) continue;

                string value;
                if (row.Cells[i].ColumnName == "H" || row.Cells[i].ColumnName == "I")
                    value = row.Cells[i].Value?.ToString().Replace("ст", string.Empty).Replace("к", string.Empty);
                else
                    value = ChangeDecSep(row.Cells[i]);
                if (string.IsNullOrEmpty(value)) continue;

                result.Add(new CellWorkOrder
                {
                    SideId = sideId,
                    TitleId = titleId,
                    Value = value
                });
            }
            return result;
        }

        /// <summary>
        /// компановка данных Описание и особенности проекта
        /// </summary>
        /// <returns></returns>
        private List<CellWorkOrder> GetTab4Data(int startSearchRow, List<XlsRow> rows)
        {
            var result = new List<CellWorkOrder>();
            for (var i = startSearchRow; i < rows.Count; i++)
            {
                if (!(rows[i].Cells[1]?.Value.ToString().Trim().Equals("ОПИСАНИЕ И ОСОБЕННОСТИ ПРОЕКТА",
                          StringComparison.CurrentCultureIgnoreCase) ?? false)) continue;
                var value = rows[i + 2].Cells[1]?.Value.ToString();
                if (!string.IsNullOrEmpty(value))
                    result.Add(new CellWorkOrder { Value = value, TitleId = 1, SideId = 1 });
                break;
            }
            return result;
        }

        #region Подготовить справочники с соответствием полей excel и ключей в БД

        private List<CommonOrderDictionary> PrepareTab3DicMapping(List<XlsRow> rows, int startCol, int endCol)
        {
            var result = new List<CommonOrderDictionary>();
            decimal? parentId = null;
            decimal? titleId = null;

            if (rows.Count != 2) return result;
            for (var i = startCol; i < endCol; i++)
            {
                var parentVal = rows[0].Cells[i].Value?.ToString().Trim();
                var titleVal = rows[1].Cells[i].Value?.ToString().Trim();

                parentId = string.IsNullOrEmpty(parentVal)
                    ? parentId
                    : _dicData.TechEconomInd.FirstOrDefault(f =>
                              f.Parent == 0 &&
                              f.Name.Trim().Equals(parentVal, StringComparison.CurrentCultureIgnoreCase))
                          ?.Key ?? parentId;

                if (parentId == null) continue;

                //пока в базе пустые типы скважин делаем так
                switch (rows[0].Cells[i].ColumnName)
                {
                    case "C":
                        titleId = 6;
                        break;
                    case "D":
                        titleId = 7;
                        break;
                    default:
                        titleId = string.IsNullOrEmpty(titleVal)
                            ? titleId
                            : _dicData.TechEconomInd.FirstOrDefault(f =>
                                      f.Parent == parentId &&
                                      !string.IsNullOrEmpty(f.Name) &&
                                      f.Name.Trim().Equals(titleVal, StringComparison.CurrentCultureIgnoreCase))
                                  ?.Key ?? titleId;
                        break;
                }

                if (titleId == null) continue;

                result.Add(new CommonOrderDictionary
                {
                    Key = (decimal)titleId,
                    Parent = (decimal)parentId,
                    MappVal = rows[0].Cells[i].ColumnName
                });
            }

            return result;
        }

        /// <summary>
        /// т.к. в справочнике "Тэхнико-экономические и экономические показатели"(D_TECH_ECONOM_IND, key = 1380), нет 
        /// соответствия с наименованием из excel, пока запишем с жесткой привязкой ключей к строкам в excel и колонке "B"
        /// </summary>
        /// <returns></returns>
        private Dictionary<int, decimal?> PrepareTab1SideMapping()
        {
            var result = new Dictionary<int, decimal?>();
            result.AddRange(new Dictionary<int, decimal?>
            {
                {9, 18}, //Начало поведения (мес/год)               (B8)
                {10, 19}, //Конец проведения работ (мес/год)         (B9)
                {11, 20}, //Буровой подрядчик                       (B10)
                {12, 5}, //Тип буровго станка                       (B11)
                {13, 8}, //Наличие ВСП                              (B12)
                {14, 9}, //Наличие АвИЭС                            (B13)
                {15, 10}, //Наличие автономности                    (B14)
                {18, 21}, //По технико-технологическим показателям  (B17)
                {20, 22} //По экономическим показателям             (B19)
            });
            return result;
        }

        #endregion
    }
}
