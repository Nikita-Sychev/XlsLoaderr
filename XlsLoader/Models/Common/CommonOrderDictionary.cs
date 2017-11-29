using System.ComponentModel.DataAnnotations.Schema;

namespace XlsLoader.Models.Common
{
    public class CommonOrderDictionary
    {
        [Column("KEY")]
        public decimal Key { get; set; }
        [Column("NAME")]
        public string Name { get; set; }
        [Column("VALUE")]
        public string Value { get; set; }
        [Column("MAPP_VAL")]
        public string MappVal { get; set; }
        [Column("PARENT_KEY")]
        public decimal Parent { get; set; }
    }
}
