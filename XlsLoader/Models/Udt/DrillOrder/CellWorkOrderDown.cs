using Newtonsoft.Json;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;

namespace XlsLoader.Models.Udt.DrillOrder
{
    [OracleCustomTypeMapping("T_CELL_WORK_ORDER_DOWN")]
    public class CellWorkOrderDown : INullable, IOracleCustomType, IOracleCustomTypeFactory
    {
        [JsonIgnore]
        public virtual bool IsNull { get; set; }
        public static CellWorkOrderDown Null
        {
            get { return null; }
        }
        public virtual IOracleCustomType CreateObject()
        {
            return new CellWorkOrderDown();
        }
        [OracleObjectMapping("INDICATOR_ID")]
        public decimal? IndicatorId { get; set; }
        [OracleObjectMapping("TITLE_ID")]
        public decimal? TitleId { get; set; }
        [OracleObjectMapping("BLOCK_TITLE_ID")]
        public decimal? BlockTitleId { get; set; }
        [OracleObjectMapping("UNIT_ID")]
        public decimal? UnitId { get; set; }
        [OracleObjectMapping("VALUE")]
        public decimal? Value { get; set; }
        public virtual void FromCustomObject(OracleConnection con, System.IntPtr pUdt)
        {
            if (null != IndicatorId)
                OracleUdt.SetValue(con, pUdt, "INDICATOR_ID", IndicatorId);
            if (null != TitleId)
                OracleUdt.SetValue(con, pUdt, "TITLE_ID", TitleId);
            if (null != BlockTitleId)
                OracleUdt.SetValue(con, pUdt, "BLOCK_TITLE_ID", BlockTitleId);
            if (null != UnitId)
                OracleUdt.SetValue(con, pUdt, "UNIT_ID", UnitId);
            if (null != Value)
                OracleUdt.SetValue(con, pUdt, "VALUE", Value);
        }
        public virtual void ToCustomObject(OracleConnection con, System.IntPtr pUdt)
        {
            IndicatorId = ((decimal?)(OracleUdt.GetValue(con, pUdt, "INDICATOR_ID")));
            TitleId = ((decimal?)(OracleUdt.GetValue(con, pUdt, "TITLE_ID")));
            BlockTitleId = ((decimal?)(OracleUdt.GetValue(con, pUdt, "BLOCK_TITLE_ID")));
            UnitId = ((decimal?)(OracleUdt.GetValue(con, pUdt, "UNIT_ID")));
            Value = ((decimal?)(OracleUdt.GetValue(con, pUdt, "VALUE")));
        }
    }
}