using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;

namespace DemoDAO
{
    public class SampleDAO : BaseDAO
    {

        public string db = @"data source=.;user id=a;password=";       

        public void DemoGetQuerySqlCommand() {
            string sqlstr = "select * from [TestDB01].[dbo].[]";
            List<CommandExpress> exps = new List<CommandExpress>();
            exps.Add(new CommandExpress() {
                ColBehaviorAct = CommandExpress.BehaviorAct.AND,
                ColBehaviorOperator = CommandExpress.BehaviorOperator.Equal,
                ColumnName = "",
                ColumnValue = ""
            });
            exps.Add(new CommandExpress()
            {
                ColBehaviorAct = CommandExpress.BehaviorAct.AND,
                ColBehaviorOperator = CommandExpress.BehaviorOperator.Like,
                ColumnName = "",
                ColumnValue = "%%"
            });
            SqlCommand scmd = GetQuerySqlCommand(sqlstr, exps);
            List<Cust> result = ExecuteSqlDataReader<Cust>(scmd, db);
            foreach (var item in result)
            {
                Console.WriteLine($"{item.Sno},{item.Name},{item.City}");
            }
        }

        public void DemoGetUpdateSqlCommand() {
            string sqlstr = "update [TestDB01].[dbo].[]";
            Dictionary<string, string> sets = new Dictionary<string, string>();
            sets.Add("", "");
            sets.Add("", "");
            List<CommandExpress> exps = new List<CommandExpress>();
            exps.Add(new CommandExpress() {
                ColBehaviorAct = CommandExpress.BehaviorAct.AND,
                ColBehaviorOperator = CommandExpress.BehaviorOperator.Equal,
                ColumnName = "",
                ColumnValue = ""
            });
            SqlCommand scmd = GetUpdateSqlCommand(sqlstr, sets, exps);
            int r = ExecuteSqlNonQuery(scmd, db);
            Console.WriteLine(r);

        }

        public void DemoGetDeleteSqlCommand() {
            string sqlstr = "delete from [TestDB01].[dbo].[]";
            List<CommandExpress> exps = new List<CommandExpress>();
            exps.Add(new CommandExpress()
            {
                ColBehaviorAct = CommandExpress.BehaviorAct.AND,
                ColBehaviorOperator = CommandExpress.BehaviorOperator.Equal,
                ColumnName = "",
                ColumnValue = ""
            });
            SqlCommand scmd = GetDeleteSqlCommand(sqlstr,exps);
            int r = ExecuteSqlNonQuery(scmd,db);
            Console.WriteLine(r);

        }

        public void DemoExecuteSqlNonQuery()
        {
            string sqlstr = "select * from [TestDB01].[dbo].[]";
            SqlCommand scmd = GetSqlCommand(sqlstr);
            int result = ExecuteSqlNonQuery(scmd,db);
            Console.WriteLine(result);
        }

        public void DemoExecuteSqlScalar() {
            string sqlstr = "select top 1 C  from [TestDB01].[dbo].[]";
            SqlCommand scmd = GetSqlCommand(sqlstr);
            string result = ExecuteSqlScalar(scmd,db);
            Console.WriteLine(result);
        }

        public void DemoExecuteSqlDataReader() {
            string sqlstr = @"select * from [TestDB01].[dbo].[] where C like @C";
            Dictionary<string, string> qdata = new Dictionary<string, string>();
            qdata.Add("C","%%");
            SqlCommand scmd = GetSqlCommand(sqlstr,qdata);
            List<Cust> result = ExecuteSqlDataReader<Cust>(scmd,db);
            foreach (var item in result)
            {
                Console.WriteLine($"{item.Sno},{item.Name},{item.City}");
            }
        }

        public void DemoExecuteSqlDataTable() {
            string sqlstr = @"select * from [TestDB01].[dbo].[] ";
            SqlCommand scmd = GetSqlCommand(sqlstr);
            List<Cust> cdata = ExecuteSqlDataReader<Cust>(scmd,db);
            DataTable dt = ConvertListToDataTable<Cust>(cdata);
            foreach (DataRow item in dt.Rows)
            {
                Console.WriteLine(item[0]);
            }
        }
    }
}
