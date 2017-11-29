using System.Collections.Generic;
using XlsLoader.Models.Common;

namespace XlsLoader.Models.Dictionary
{
    public class DictionarysDrillOrder
    {
        public List<IndWorkOrder> IndWorkOrder { get; set; }
        public List<CommonOrderDictionary> TitleBlockWorkOrder { get; set; }
        public List<CommonOrderDictionary> TitleWorkOrder { get; set; }
        public List<CommonOrderDictionary> LeftTopTable { get; set; }
        public List<CommonOrderDictionary> ParamInputData { get; set; }
        public List<CommonOrderDictionary> TitleInputData { get; set; }
        public List<CommonOrderDictionary> SideTotalData { get; set; }
        public List<CommonOrderDictionary> Units { get; set; }
        public List<CommonOrderDictionary> DrillWorker { get; set; }
        public List<CommonOrderDictionary> TechEconomInd { get; set; }
        public List<CommonOrderDictionary> WispModuleWell { get; set; }
    }
}
