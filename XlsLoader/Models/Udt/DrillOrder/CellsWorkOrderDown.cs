using Newtonsoft.Json;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;

namespace XlsLoader.Models.Udt.DrillOrder
{
    [OracleCustomTypeMapping("T_CELLS_WORK_ORDER_DOWN")]
    public class CellsWorkOrderDown : INullable, IOracleCustomType, IOracleCustomTypeFactory, IOracleArrayTypeFactory
    {
        [JsonIgnore]
        public virtual bool IsNull { get; set; }
        public static CellsWorkOrderDown Null
        {
            get { return null; }
        }
        public virtual IOracleCustomType CreateObject()
        {
            return new CellsWorkOrderDown();
        }
        public virtual System.Array CreateArray(int length)
        {
            return new CellWorkOrderDown[length];
        }
        public virtual System.Array CreateStatusArray(int length) { return null; }
        [OracleArrayMapping()]
        public CellWorkOrderDown[] Value { get; set; }
        public virtual OracleUdtStatus[] StatusArray { get; set; }
        public virtual void ToCustomObject(OracleConnection con, System.IntPtr pUdt)
        {
            Value = ((CellWorkOrderDown[])(OracleUdt.GetValue(con, pUdt, 0)));
        }
        public virtual void FromCustomObject(OracleConnection con, System.IntPtr pUdt)
        {
            OracleUdt.SetValue(con, pUdt, 0, Value);
        }
    }
}
