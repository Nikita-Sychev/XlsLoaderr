using System.Collections.Generic;
using System.Linq;
using Contracts;
using SqlWork;
using XlsLoader.Base.Connect;
using XlsLoader.Models.Common;
using XlsLoader.Models.Dictionary;

namespace XlsLoader.Services
{
    public class DictionaryService : BaseService
    {
        public DictionaryService(IUserIdentity identity) : base(identity, "sqlwork")
        {

        }

        #region Drill Order

        public DictionarysDrillOrder GetDrillOrderDictionarys()
        {
            return new DictionarysDrillOrder
            {
                IndWorkOrder = GetIndWorkOrder(),
                TitleBlockWorkOrder = GetOrderDicData("t1278"),
                TitleWorkOrder = GetOrderDicData("t1271"),
                LeftTopTable = GetOrderDicData("t1286"),
                ParamInputData = GetOrderDicData("t1289"),
                TitleInputData = GetOrderDicData("t1292"),
                SideTotalData = GetOrderDicData("t1298"),
                Units = GetOrderDicData("t414"),
                DrillWorker = GetOrderDicData("t1366"),
                TechEconomInd = GetTechEconomInd()
            };
        }

        private List<IndWorkOrder> GetIndWorkOrder()
        {
            const string query = @"select t.key, TRIM(BOTH ' ' FROM t.name) as name, t.parent_key from t1265 t order by t.key";
            var orm = new Orm<IndWorkOrder>();
            return sqlExecutor.GetQueryResult(query, (rec) => orm.ReadObject(rec)).ToList();
        }

        private List<CommonOrderDictionary> GetOrderDicData(string tableName)
        {
            var query = $@"select t.key, TRIM(BOTH ' ' FROM t.name) as name, '' as value, '' as mapp_val, '' as parent_key from {tableName} t order by t.key";
            var orm = new Orm<CommonOrderDictionary>();
            return sqlExecutor.GetQueryResult(query, (rec) => orm.ReadObject(rec)).ToList();
        }

        private List<CommonOrderDictionary> GetTechEconomInd()
        {
            var query = $@"select t.key, TRIM(BOTH ' ' FROM t.name) as name, '' as value, '' as mapp_val, t.PARENT_KEY from t1380 t order by t.key";
            var orm = new Orm<CommonOrderDictionary>();
            return sqlExecutor.GetQueryResult(query, (rec) => orm.ReadObject(rec)).ToList();
        }

        public List<CommonOrderDictionary> GetWispModuleWell(decimal workOrderId, IEnumerable<string> wellsName)
        {
            var nameList = wellsName.ToList();
            {
                if (nameList.Count <= 0) return new List<CommonOrderDictionary>();
                var names =
                    ("(" + nameList.Aggregate(string.Empty, (current, item) => (string)(current + $"'{item}',")) + ")").Replace(",)", ")");
                var query =
                    $@"select t.key, TRIM(BOTH ' ' FROM t.name) as name, '' as value, '' as mapp_val, '' as parent_key from t1274 t where work_order_id = {
                            workOrderId
                        } and t.name in {names} order by t.key";
                var orm = new Orm<CommonOrderDictionary>();
                return sqlExecutor.GetQueryResult(query, (rec) => orm.ReadObject(rec)).ToList();
            }
        }

        #endregion
    }
}
