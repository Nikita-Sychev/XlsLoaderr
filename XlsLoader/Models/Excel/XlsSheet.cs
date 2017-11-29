using System.Collections.Generic;

namespace XlsLoader.Models.Excel
{
    public class XlsSheet
    {
        public string Name { get; set; }
        public List<XlsRow> Rows { get; set; }
    }
}
