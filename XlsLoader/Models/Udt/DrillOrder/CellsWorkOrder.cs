using Newtonsoft.Json;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;

namespace XlsLoader.Models.Udt.DrillOrder
{
    [OracleCustomTypeMapping("T_CELLS_WORK_ORDER")]
    public class CellsWorkOrder : INullable, IOracleCustomType, IOracleCustomTypeFactory, IOracleArrayTypeFactory
    {
        [JsonIgnore]
        public virtual bool IsNull { get; set; }
        public static CellsWorkOrder Null
        {
            get { return null; }
        }
        public virtual IOracleCustomType CreateObject()
        {
            return new CellsWorkOrder();
        }
        public virtual System.Array CreateArray(int length)
        {
            return new CellWorkOrder[length];
        }
        public virtual System.Array CreateStatusArray(int length) { return null; }
        [OracleArrayMapping()]
        public CellWorkOrder[] Value { get; set; }
        public virtual OracleUdtStatus[] StatusArray { get; set; }
        public virtual void ToCustomObject(OracleConnection con, System.IntPtr pUdt)
        {
            Value = ((CellWorkOrder[])(OracleUdt.GetValue(con, pUdt, 0)));
        }
        public virtual void FromCustomObject(OracleConnection con, System.IntPtr pUdt)
        {
            OracleUdt.SetValue(con, pUdt, 0, Value);
        }
    }
}
