using System.Collections.Generic;

namespace XlsLoader.Models.Excel
{
    public class XlsRow
    {
        public int Index { get; set; }
        public XlsRow()
        {
            Cells = new List<XlsCell>();
        }

        public List<XlsCell> Cells { get; set; }
    }
}
