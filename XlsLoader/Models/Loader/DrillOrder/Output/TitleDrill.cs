using XlsLoader.Models.Udt.DrillOrder;

namespace XlsLoader.Models.Loader.DrillOrder.Output
{
    public class TitleDrill
    {
        public decimal? WorkOrderId { get; set; }
        public CellsWorkOrder TableData1 { get; set; }
        public CellsWorkOrder TableData2 { get; set; }
        public CellsWorkOrder TableData3 { get; set; }
        public CellsWorkOrder TableData4 { get; set; }
    }
}

