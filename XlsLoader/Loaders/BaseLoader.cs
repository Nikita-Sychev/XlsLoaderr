using System;
using System.Collections.Generic;
using System.Globalization;
using NLog;
using XlsLoader.Models.Excel;

namespace XlsLoader.Loaders
{
    public class BaseLoader
    {
        protected static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public List<XlsSheet> SheetsData { get; set; }
        protected string DecSeparator { get; set; }

        public BaseLoader()
        {
            DecSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            if (DecSeparator != "," && DecSeparator != ".")
                throw new Exception("В системе задан разделитель целой и дробной части отличный от \",\" и \".\"");
        }

        //меняет разделитель целой и дробной части на установленный в системе
        protected virtual string ChangeDecSep(XlsCell cell)
        {
            var replace = cell.Value.ToString().Replace("%", string.Empty);
            return DecSeparator == "."
                ? replace.Replace(",", DecSeparator)
                : DecSeparator == ","
                    ? replace.Replace(".", DecSeparator)
                    : string.Empty;
        }

        protected virtual decimal? GetValueFromCell(XlsCell cell)
        {
            if (double.TryParse(ChangeDecSep(cell), out var valDub))
                return (decimal?)valDub;
            return null;
        }
    }
}
