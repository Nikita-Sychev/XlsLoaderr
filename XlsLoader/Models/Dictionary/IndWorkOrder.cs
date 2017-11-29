using System.ComponentModel.DataAnnotations.Schema;

namespace XlsLoader.Models.Dictionary
{
    public class IndWorkOrder
    {
        [Column("KEY")]
        public decimal Key { get; set; }
        [Column("NAME")]
        public string Name { get; set; }
        [Column("PARENT_KEY")]
        public decimal ParentKey { get; set; }
    }
}