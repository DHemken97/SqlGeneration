using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using ServiceStack.OrmLite;
using ServiceStack.OrmLite.SqlServer;

namespace SQLGenerator
{
    public static class SqlClassGenerator
    {
        private static SqlServer2016OrmLiteDialectProvider provider => new SqlServer2016OrmLiteDialectProvider();
        public static string GetTable<TClass>()
        {
            var statement =  provider.ToCreateTableStatement(typeof(TClass));
            var varchars = statement.Split('\n').ToList();
            statement = statement.Replace("VARCHAR", "NVARCHAR");
            foreach(var varchar in varchars)
            {
                var name = varchar.Trim().Split(' ').FirstOrDefault();
                var prop = typeof(TClass).GetProperties().FirstOrDefault(p => $"\"{p.Name}\"" == name);
                var max = GetMax(prop);
                if (max != null)
                    statement = statement.Replace($"\"{prop.Name}\" NVARCHAR(8000)", $"\"{prop.Name}\" NVARCHAR({max})");
            }
                return statement;
        }
        public static string GetView<TClass>()
        {
            var select = provider.ToSelectStatement(typeof(TClass), "");
            return $"Create View vw{typeof(TClass).Name} as {select}";
        }
        public static string GetInsertStatement<TClass>()
        {
            OrmLiteConfig.DialectProvider = provider;
            var paramList = GetParams<TClass>(true);
            var item = Activator.CreateInstance<TClass>();
            var insert = provider.ToInsertRowStatement(new SqlCommand(),item);
            return $"Create Procedure spInsert{typeof(TClass).Name}\r\n" +
                $"{string.Join(",\r\n",paramList)}\r\nAS Begin\r\n" +
                $"{insert}\r\nEnd";
        }
        public static string GetUpdateStatement<TClass>()
        {
            OrmLiteConfig.DialectProvider = provider;
            var paramList = GetParams<TClass>(true);
            var item = Activator.CreateInstance<TClass>();
            var insert = provider.ToInsertRowStatement(new SqlCommand(), item);
            return $"Create Procedure spUpdate{typeof(TClass).Name}\r\n" +
                $"{string.Join(",\r\n", paramList)}\r\nAS Begin\r\n" +
                $"{insert}\r\nEnd";
        }


        public static IEnumerable<string> GetParams<TClass>(bool includeAtSymbol=false)
        {
            var vals = new List<string>();
            foreach (var prop in typeof(TClass).GetProperties())
            {
                var item = $"{prop.Name} {provider.GetColumnTypeDefinition(prop.PropertyType, GetMax(prop), null)}";
                vals.Add(includeAtSymbol?$"@{item}":item);
            }
            return vals;
        }
        private static int? GetMax(PropertyInfo property)
        {

            int? max = null;
            if (property == null) return null;
            MaxLengthAttribute attribute = (MaxLengthAttribute)property.GetCustomAttributes(true).FirstOrDefault(a => a.GetType() == typeof(MaxLengthAttribute));
            max = attribute?.Length;
            return max;
        }
    }
}
