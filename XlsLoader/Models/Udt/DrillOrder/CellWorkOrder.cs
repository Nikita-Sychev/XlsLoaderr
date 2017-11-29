using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;
using Newtonsoft.Json;

namespace XlsLoader.Models.Udt.DrillOrder
{
    [OracleCustomTypeMapping("T_CELL_WORK_ORDER")]
    public class CellWorkOrder : INullable, IOracleCustomType, IOracleCustomTypeFactory
    {
        [JsonIgnore]
        public virtual bool IsNull { get; set; }
        public static CellWorkOrder Null
        {
            get { return null; }
        }
        public virtual IOracleCustomType CreateObject()
        {
            return new CellWorkOrder();
        }
        [OracleObjectMappingAttribute("SIDE_ID")]
        public decimal? SideId { get; set; }
        [OracleObjectMappingAttribute("TITLE_ID")]
        public decimal? TitleId { get; set; }
        [OracleObjectMappingAttribute("VALUE")]
        public string Value { get; set; }
        public virtual void FromCustomObject(OracleConnection con, System.IntPtr pUdt)
        {
            if (null != SideId)
                OracleUdt.SetValue(con, pUdt, "SIDE_ID", SideId);
            if (null != TitleId)
                OracleUdt.SetValue(con, pUdt, "TITLE_ID", TitleId);
            if (null != Value)
                OracleUdt.SetValue(con, pUdt, "VALUE", Value);
        }
        public virtual void ToCustomObject(OracleConnection con, System.IntPtr pUdt)
        {
            SideId = ((decimal?)(OracleUdt.GetValue(con, pUdt, "SIDE_ID")));
            TitleId = ((decimal?)(OracleUdt.GetValue(con, pUdt, "TITLE_ID")));
            Value = ((string)(OracleUdt.GetValue(con, pUdt, "VALUE")));
        }
    }
}
