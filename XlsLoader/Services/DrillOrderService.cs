using System;
using System.Collections.Generic;
using System.Data;
using Contracts;
using SqlWork.Contracts;
using XlsLoader.Base.Connect;
using XlsLoader.Models.Loader.DrillOrder.Output;

namespace XlsLoader.Services
{
    public class DrillOrderService : BaseService
    {
        public DrillOrderService(IUserIdentity identity) : base(identity, "sqlwork")
        {

        }

        public bool LoadExcelWorkOrder(EstimateCostWell model)
        {
            var p0 = new SqlParam() { Name = "p_work_order_id", Val = model.WorkOrderId, ValType = DataType.Integer, Direction = ParameterDirection.Input };
            var p1 = new SqlParam() { Name = "p_virt_well", Val = model.VirtWell, ValType = DataType.Integer, Direction = ParameterDirection.Input };
            var p2 = new SqlParam() { Name = "p_data_down", Val = model.TableDataDown, ValType = DataType.Object, Direction = ParameterDirection.Input };
            var p3 = new SqlParam() { Name = "p_data_1", Val = model.TableData1, ValType = DataType.Object, Direction = ParameterDirection.Input };
            var p4 = new SqlParam() { Name = "p_data_2", Val = model.TableData2, ValType = DataType.Object, Direction = ParameterDirection.Input };
            var p5 = new SqlParam() { Name = "p_data_4", Val = model.TableData4, ValType = DataType.Object, Direction = ParameterDirection.Input };
            var p6 = new SqlParam() { Name = "p_data_5", Val = model.TableData5, ValType = DataType.Object, Direction = ParameterDirection.Input };
            var p7 = new SqlParam() { Name = "status", Val = null, ValType = DataType.String, Direction = ParameterDirection.Output };
            sqlExecutor.ExecuteProcedure("work_order.load_excel_work_order", new SqlParam[] { p0, p1, p2, p3, p4, p5, p6, p7 }, true);

            var msg = (string)p7.Val;
            if (!string.IsNullOrEmpty(msg))
                throw new Exception(msg);

            return true;
        }

        public bool LoadExcelTitleWorkOrder(TitleDrill model)
        {
            var p0 = new SqlParam() { Name = "p_work_order_id", Val = model.WorkOrderId, ValType = DataType.Integer, Direction = ParameterDirection.Input };
            var p1 = new SqlParam() { Name = "p_title", Val = model.TableData1, ValType = DataType.Object, Direction = ParameterDirection.Input };
            var p2 = new SqlParam() { Name = "p_worker", Val = model.TableData2, ValType = DataType.Object, Direction = ParameterDirection.Input };
            var p3 = new SqlParam() { Name = "p_indicator", Val = model.TableData3, ValType = DataType.Object, Direction = ParameterDirection.Input };
            var p4 = new SqlParam() { Name = "p_comment", Val = model.TableData4, ValType = DataType.Object, Direction = ParameterDirection.Input };
            var p5 = new SqlParam() { Name = "status", Val = null, ValType = DataType.String, Direction = ParameterDirection.Output };
            sqlExecutor.ExecuteProcedure("work_order.load_excel_title_work_order", new SqlParam[] { p0, p1, p2, p3, p4, p5 }, true);

            var msg = (string)p5.Val;
            if (!string.IsNullOrEmpty(msg))
                throw new Exception(msg);

            return true;
        }
    }
}
