using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Data.SqlClient;
using System.Data;

namespace DemoDAO
{
    //提供各種資料型別的轉換
    /// <inheritdoc/>
    /// <include file='docs.xml' path='docs/members[@name="ConvertDatas"]/ConvertDatas/*'/>      
    public class ConvertDatas
    {             
        //傳入資料:SqlDataReader
        //回傳List<T>，T為自定型別     
        //以<T>定義的屬性為基準，用屬性名稱去找sqldatareader對應的column的值
        //屬性名稱要與sql select的column名稱一致，但數量不用        
        /// <include file='docs.xml' path='docs/members[@name="ConvertDatas"]/ConvertSqlDataReaderToList/*'/>
        public static List<T> ConvertSqlDataReaderToList<T>(SqlDataReader datas) where T : new() 
        {
            List<T> result = new List<T>();
            PropertyInfo[] pi = new T().GetType().GetProperties();
                                  
            if (datas.HasRows)
            {
                while (datas.Read())
                {
                    T r = new T();
                    for (int i = 0; i < pi.Count(); i++)
                    {
                        Type propertyType = pi[i].PropertyType;
                        string propertyName = pi[i].Name;
                        var value = datas[datas.GetOrdinal(propertyName)];
                        pi[i].SetValue(r, Convert.ChangeType(value, propertyType));
                    }
                    result.Add(r);
                }
            }
            return result;
        }

        //傳入DataTable
        //回傳List<T>
        /// <include file = 'docs.xml' path='docs/members[@name="ConvertDatas"]/ConvertDataTableToList/*'/>
        public static List<T> ConvertDataTableToList<T>(DataTable datas) where T : new()
        { 
            List<T> result = new List<T>();
            PropertyInfo[] pi = new T().GetType().GetProperties();
            foreach (DataRow item in datas.Rows)
            {
                T r = new T();
                for (int i = 0; i < pi.Count(); i++)
                {
                    Type propertyType = pi[i].PropertyType;
                    string propertyName = pi[i].Name;
                    var value = item[propertyName];
                    pi[i].SetValue(r, Convert.ChangeType(value, propertyType));
                }
                result.Add(r);
            }
            return result;
        }

        //傳入string[] 
        //回傳<T>
        //string[]長度須與<T> property數量一樣，依序抓取<T>定義的型別做轉換   
        /// <include file = 'docs.xml' path='docs/members[@name="ConvertDatas"]/ConvertStringArrayToObjectT/*'/>     
        public static T ConvertStringArrayToObjectT<T>(string[] datas) where T : new()
        {
            T result = new T();
            PropertyInfo[] pi = result.GetType().GetProperties();      

            for (int i = 0; i < pi.Length; i++)
            {
                Type propertyType = pi[i].PropertyType;
                pi[i].SetValue(result,Convert.ChangeType(datas[i],propertyType));
            }

            return result;
        }

        //傳入:List<T>
        //回傳:DataTable
        /// <include file = 'docs.xml' path='docs/members[@name="ConvertDatas"]/ConvertListToDataTable/*'/>     
        public static DataTable ConvertListToDataTable<T>(List<T> datas) where T : new()
        {
            DataTable result = new DataTable();
            
            //取得T的屬性，新增DataTable相對應的column
            PropertyInfo[] pi = new T().GetType().GetProperties();
            foreach (var item in pi)
            {
                result.Columns.Add(new DataColumn(item.Name,item.PropertyType));                
            }

            //逐筆將List<T>轉為Row
            foreach (var item in datas)
            {
                DataRow dr = result.NewRow();
                //讀取屬性，寫入Row的每個Column
                foreach (var i in pi)
                {
                    string PropertyName = i.Name;
                    Type PropertyType = i.PropertyType;
                    var objValue = i.GetValue(item);                    
                    dr[PropertyName] = Convert.ChangeType(objValue, PropertyType);
                }                
                result.Rows.Add(dr);
            }
            return result;
        }        
    }
}
