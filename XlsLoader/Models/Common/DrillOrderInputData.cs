namespace XlsLoader.Models.Common
{
    public class DrillOrderInputData
    {
        public byte[] File { get; set; }
        public decimal? ProjectVersionId { get; set; }
        public decimal? PseudoModuleId { get; set; }
        public decimal? OrderId { get; set; }
    }
}
