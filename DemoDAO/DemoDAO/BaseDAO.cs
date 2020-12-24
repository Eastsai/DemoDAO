using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Runtime.InteropServices;

namespace DemoDAO
{
    //也可以在class層級ConvertDataToGenericClass<T>，但被繼承或宣告時，上層也必須使用<T>，使用上較不彈性
    public class BaseDAO : ConvertDatas
    {                   
        //回傳sqlcmd
        //適用where條件的數量會變動時
        public SqlCommand GetQuerySqlCommand(string sqlStr,List<CommandExpress> exps) {
            StringBuilder sb = new StringBuilder(sqlStr);
            sb.Append(GetWhereExpString(exps));
            SqlCommand sCmd = new SqlCommand(sb.ToString());
            foreach (var item in exps)
            {
                sCmd.Parameters.AddWithValue(item.ColumnName,item.ColumnValue);
            }            
            return sCmd;
        }

        //回傳sqlcmd
        //適用set、where條件的數量會變動時
        public SqlCommand GetUpdateSqlCommand(string sqlStr,Dictionary<string,string> sets,List<CommandExpress> exps) {
            SqlCommand sCmd = new SqlCommand();
            StringBuilder sb = new StringBuilder(sqlStr);            
            if (sets != null) {
                sb.Append(GetSetExpString(sets));
                sb.Append(GetWhereExpString(exps));
                sCmd.CommandText = sb.ToString();
                foreach (var item in sets)
                {
                    sCmd.Parameters.AddWithValue(item.Key,item.Value);
                }
                foreach (var item in exps)
                {
                    sCmd.Parameters.AddWithValue(item.ColumnName,item.ColumnValue);
                }               
            }            
            return sCmd;
        }

        //回傳sqlcmd
        //適用where條件的數量會變動時
        public SqlCommand GetDeleteSqlCommand(string sqlStr, List<CommandExpress> exps) {
            StringBuilder sb = new StringBuilder(sqlStr);
            sb.Append(GetWhereExpString(exps));
            SqlCommand sCmd = new SqlCommand(sb.ToString());
            foreach (var item in exps)
            {
                sCmd.Parameters.AddWithValue(item.ColumnName,item.ColumnValue);
            }
            return sCmd;
        }

        //傳入sql script
        //回傳SqlCommand,可選擇是否傳入parameters
        public SqlCommand GetSqlCommand(string sqlstr, [Optional] Dictionary<string, string> param)
        {
            SqlCommand sCmd = new SqlCommand(sqlstr);

            if (param != null)
            {
                foreach (var item in param)
                {
                    sCmd.Parameters.AddWithValue(item.Key, item.Value);
                }
            }
            return sCmd;
        }

        //傳入sql script 
        //回傳異動資料筆數(ExecuteNonQuery)
        public int ExecuteSqlNonQuery(SqlCommand cmd,string db) {
            int result = 0;                     
            using (SqlConnection sc = new SqlConnection(db))
            {
                cmd.Connection = sc;
                sc.Open();
                result = cmd.ExecuteNonQuery();
                sc.Close();
            }
            return result;
        }

        //傳入sql script 
        //回傳第一列第一行(ExecuteScalar)        
        public string ExecuteSqlScalar(SqlCommand cmd, string db)
        {
            string result = String.Empty;
            using (SqlConnection sc = new SqlConnection(db))
            {
                cmd.Connection = sc;
                sc.Open();
                result = cmd.ExecuteScalar().ToString();
                sc.Close();
            }
            return result;
        }

        //傳入sql script 
        //回傳List<T>        
        public List<T> ExecuteSqlDataReader<T>(SqlCommand cmd, string db) where T : new()
        {
            List<T> result = new List<T>();
            
            using (SqlConnection sc = new SqlConnection(db))
            {
                cmd.Connection = sc;
                sc.Open();                
                using (SqlDataReader sdr = cmd.ExecuteReader())
                {
                    result = ConvertSqlDataReaderToList<T>(sdr);
                }
                sc.Close();                
            }
 
            return result;
        }

        //傳入sql script 
        //回傳DataTable        
        public DataTable ExecuteSqlDataTable<T>(SqlCommand cmd,string db)
        {
            DataTable result = new DataTable();
            using (SqlConnection sc = new SqlConnection(db))
            {
                cmd.Connection = sc;
                sc.Close();
                using (SqlDataReader sdr = cmd.ExecuteReader())
                {
                    result = (DataTable)sdr.GetEnumerator();
                }
                sc.Close();
            }
            return result;
        }

        public string GetSetExpString(Dictionary<string,string> sets) {
            StringBuilder sb = new StringBuilder();
            if (sets != null) {
                sb.Append(" set");
                foreach (var item in sets)
                {
                    sb.Append($" {item.Key} = @{item.Value},");
                }
                sb.Remove(sb.Length-1,1);
            }                                    
            return sb.ToString();
        }

        public string GetWhereExpString(List<CommandExpress> exps) {
            StringBuilder sb = new StringBuilder();
            int cntExp = 0;
            foreach (var item in exps)
            {
                string colName = item.ColumnName;
                string colValue = item.ColumnValue;
                string behaviorAct = "";
                string behaviorOperator = "";

                switch (item.ColBehaviorAct.ToString())
                {
                    case "OR":
                        behaviorAct = "or";
                        break;
                    case "AND":
                        behaviorAct = "and";
                        break;
                    default:
                        break;
                }
                switch (item.ColBehaviorOperator.ToString())
                {
                    case "Equal":
                        behaviorOperator = "=";
                        break;
                    case "Less":
                        behaviorOperator = "<";
                        break;
                    case "Greater":
                        behaviorOperator = ">";
                        break;
                    case "Like":
                        behaviorOperator = "like";
                        break;
                    default:
                        break;
                }

                behaviorAct = (cntExp == 0) ? " where" : behaviorAct;
      
                sb.Append($" {behaviorAct} {colName} {behaviorOperator} @{colName}");
                cntExp++;
            }

            return sb.ToString();
        }
    }

    //BuildQuerySqlcmd
    public class CommandExpress
    {
        //查詢行為:AND,OR,
        //運算子 : =,>,<,like        
        public BehaviorAct ColBehaviorAct { get; set; }
        public BehaviorOperator ColBehaviorOperator { get; set; }
        public string ColumnName { get; set; }
        public string ColumnValue { get; set; }

        public enum BehaviorAct
        {
            OR = 1,
            AND = 2
        }
        
        public enum BehaviorOperator
        {
            Equal = 1,
            Less = 2,
            Greater = 3,
            Like = 4,        
        }

    }
    
}
