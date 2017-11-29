using XlsLoader.Models.Udt.DrillOrder;

namespace XlsLoader.Models.Loader.DrillOrder.Output
{
    public class EstimateCostWell
    {
        public decimal? WorkOrderId { get; set; }
        public decimal? VirtWell { get; set; }
        public CellsWorkOrder TableData1 { get; set; }
        public CellsWorkOrder TableData2 { get; set; }
        public CellsWorkOrder TableData4 { get; set; }
        public CellsWorkOrder TableData5 { get; set; }
        public CellsWorkOrderDown TableDataDown { get; set; }
    }
}