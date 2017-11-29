using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Spreadsheet;

namespace XlsLoader.Models.Excel
{
    public class XlsCell
    {
        public XlsCell()
        {
            DataType = new EnumValue<CellValues>(CellValues.String);
        }

        public XlsCell(XlsCell cell)
        {
            DataType = new EnumValue<CellValues>(CellValues.String);
            Index = cell.Index;
            ColumnName = cell.ColumnName;
            AddressName = cell.AddressName;
            DataType = cell.DataType;
            Value = cell.Value;
        }

        public uint Index { get; set; }
        public string ColumnName { get; set; }
        public string AddressName { get; set; }
        public CellValues DataType { get; set; }
        public object Value { get; set; }
    }
}
