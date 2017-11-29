using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using XlsLoader.Models.Excel;

namespace XlsLoader.Excel
{
    public static class OpenXml
    {
        public static List<XlsSheet> GetSheets(Stream fileStream)
        {
            try
            {
                using (var myDoc = SpreadsheetDocument.Open(fileStream, false))
                    return GetSheetsData(myDoc.WorkbookPart);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// получить данные листа по его имени
        /// </summary>
        public static XlsSheet GetSheetData(string sheetName, WorkbookPart workbookPart)
        {
            if (string.IsNullOrEmpty(sheetName))
                throw new Exception("Не задано имя листа");

            var sheet = workbookPart.Workbook.GetFirstChild<Sheets>().Elements<Sheet>()
                .FirstOrDefault(s => s.Name.Value?.Trim() == sheetName);
            if (sheet == null)
                throw new Exception("В файле отсутствует лист \"" + sheetName + "\"");
            var sharedItems = workbookPart.SharedStringTablePart.SharedStringTable.Elements<SharedStringItem>();
            var cellFrm = workbookPart.WorkbookStylesPart.Stylesheet.CellFormats.ChildElements;
            return new XlsSheet
            {
                Name = sheetName,
                Rows = GetRows(
                    ((WorksheetPart)workbookPart.GetPartById(sheet.Id.Value)).Worksheet.GetFirstChild<SheetData>(),
                    cellFrm, sharedItems as SharedStringItem[] ?? sharedItems.ToArray()) //!!!
            };
        }

        /// <summary>
        /// получить данные всех листов в книге
        /// </summary>
        public static List<XlsSheet> GetSheetsData(WorkbookPart workbookPart)
        {
            var sheets = workbookPart.Workbook.GetFirstChild<Sheets>().Elements<Sheet>();
            if (sheets == null)
                throw new Exception("В документе отсутствуют листы");
            var cellFrm = workbookPart.WorkbookStylesPart.Stylesheet.CellFormats.ChildElements;
            var sharedItems = workbookPart.SharedStringTablePart.SharedStringTable.Elements<SharedStringItem>();
            return sheets.Select(sheet => new XlsSheet
            {
                Name = sheet.Name,
                //Data = ((WorksheetPart) workbookPart.GetPartById(sheet.Id.Value)).Worksheet.GetFirstChild<SheetData>(),
                Rows = GetRows(((WorksheetPart)workbookPart.GetPartById(sheet.Id.Value)).Worksheet.GetFirstChild<SheetData>(), cellFrm, sharedItems as SharedStringItem[] ?? sharedItems.ToArray())  //!!!
            }).ToList();
        }

        /// <summary>
        /// Собирает данные по листу в список строка/ячейка
        /// </summary>                
        private static List<XlsRow> GetRows(OpenXmlElement data, OpenXmlElementList cellFrm, SharedStringItem[] sharedItems, int startRow = 0, int endRow = 0)
        {
            var rows = data.Elements<Row>().ToList();
            var result = new List<XlsRow>(rows.Count);
            endRow = endRow > 0 ? endRow : rows.Count;

            for (var i = startRow; i < endRow; i++)
            {
                var cells = rows[i].Descendants<Cell>().ToList();
                result.Add(new XlsRow { Cells = new List<XlsCell>(cells.Count) }); //! Cells может оказаться пустым
                uint indexRow = 0;

                for (var j = 0; j < cells.Count; j++)
                {
                    var cell = cells[j];
                    if (cell == null) continue;
                    if (j == 0) indexRow = (uint)GetIndexRow(cell);
                    if (indexRow == 0) indexRow = (uint)(i + startRow);

                    var item = new XlsCell
                    {
                        AddressName = cell.CellReference,
                        Value = GetCellValue(cellFrm, sharedItems, cell),
                        Index = indexRow,
                        ColumnName = GetColumnName(cell, (int)indexRow)
                    };
                    result.LastOrDefault()?.Cells.Add(item);
                }
            }
            return result;
        }

        /// <summary>
        /// Получает индекс строки
        /// </summary>
        private static int GetIndexRow(CellType cell)
        {
            var str = cell.CellReference.ToString().Substring(1);
            if (string.IsNullOrEmpty(str)) return 0;
            int.TryParse(str, out int index);
            return index;
        }

        /// <summary>
        /// Получает имя колонки (A, B, C)
        /// </summary>
        private static string GetColumnName(CellType cell, int indexRow)
        {
            var str = cell.CellReference.ToString();
            if (string.IsNullOrEmpty(str)) return string.Empty;
            var index = str.IndexOf(indexRow.ToString(), StringComparison.Ordinal);
            return index > 0 ? str.Substring(0, index) : string.Empty;
        }

        /// <summary>
        /// Получает содержимое ячейки в зависимости от типа данных
        /// </summary>
        private static object GetCellValue(OpenXmlElementList cellFrm, SharedStringItem[] sharedItems, CellType cell)
        {
            if (cell.DataType == null)
                return GetCellValNoDataType(cellFrm, cell);

            // в зависимости от типа ячейки
            switch (cell.DataType.Value)
            {
                case CellValues.String:
                    return cell.CellValue?.Text?.Trim();
                case CellValues.InlineString:
                    return cell.InlineString?.Text?.Text?.Trim();
                case CellValues.SharedString:
                    return GetCellValSharedString(sharedItems, cell);
                case CellValues.Boolean:
                case CellValues.Number:
                case CellValues.Date:
                case CellValues.Error:
                    return cell.InnerText?.Trim();
                default:
                    return cell.InnerText?.Trim();
            }
        }

        /// <summary>
        /// если dataType не указан, возвращаем значение cell.innerText
        /// </summary>
        private static object GetCellValNoDataType(OpenXmlElementList cellFrm, CellType cell)
        {
            if (cell.CellValue == null) return cell.InnerText;
            if (cell.StyleIndex == null) return cell.CellValue;
            var cf = cellFrm[int.Parse(cell.StyleIndex.InnerText)] as CellFormat;
            if (cf != null)
            {
                if (cf.NumberFormatId == 177) //--- MM/YYYY
                    return DateTime.FromOADate(Convert.ToDouble(cell.CellValue.Text));
                if (cf.NumberFormatId < 14 || cf.NumberFormatId > 22)
                    return GetCellValue(cell.CellValue);
            }
            var date = DateTime.Parse("1900-01-01")
                .AddDays(double.Parse(cell.CellValue.Text.Replace(".", ",")) - 2);
            return (date.Day < 10 ? ("0" + date.Day) : date.Day.ToString()) + "." +
                   (date.Month < 10 ? ("0" + date.Month) : date.Month.ToString()) + "." + date.Year;
        }

        /// <summary>
        /// получить значение для типа CellValues.SharedString
        /// </summary>
        private static object GetCellValSharedString(SharedStringItem[] sharedItems, OpenXmlElement cell)
        {
            if (!int.TryParse(cell.InnerText, out int id)) return string.Empty;
            var item = sharedItems.ElementAt(id);
            return item.Text?.Text?.Trim() ?? (item.InnerText?.Trim() ??
                                              item.InnerXml?.Trim() ??
                                              string.Empty);
        }

        private static string GetCellValue(object cell)
        {
            return cell.GetType() != typeof(CellValue)
                ? cell.ToString().Trim()
                : ((CellValue)cell).InnerText?.Trim() ??
                  (((CellValue)cell).InnerXml?.Trim() ??
                   (((CellValue)cell).Text?.Trim() ??
                    cell.ToString().Trim()));
        }
    }
}
