using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XlsLoader.Enums
{
    public enum CalendarLevel
    {
        Year = 1,
        Month = 4
    }

    public enum CostValueType
    {
        Amount = 0,
        Cost = 1
    }

    public enum ActionType
    {
        UploadSourceDataForCalculation = 1
    }

    public enum FileTypeForUpload
    {
        xlsx = 1,
        xlsm,
        xltx,
        xlam,
        xltm
    }
}
