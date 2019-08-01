using ExpenseProcessingSystem.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.Services
{
    public class FilterQueryService
    {
        public string generateFilterQuery(PropertyInfo[] properties, List<dynamic> mList, FiltersViewModel filters, string modelStartStr)
        {
            string query = "";
            //var tmp = mList.GetType();
            //var tmp2 = type.GetProperties();
            //var modelStartStr = tmp2.FirstOrDefault().Name.Split("_").First();
            foreach (var property in properties)
            {
                var propertyName = property.Name;
                string subStr = propertyName.Substring(propertyName.IndexOf('_') + 1);
                var toStr = property.GetValue(filters.NotifFil).ToString();
                if (toStr != "" && toStr != "0" && toStr != "1/1/0001 12:00:00 AM")
                {
                    if (property.PropertyType.Name == "String") // IF string VAL
                    {
                        query += modelStartStr+"_" + subStr + ".Contains(" + property.GetValue(filters.NotifFil).ToString() + ") ";
                    }else if (property.PropertyType.Name == "int" || property.PropertyType.Name == "double" || property.PropertyType.Name == "float")
                    {
                        query += modelStartStr + "_" + subStr + " == " + property.GetValue(filters.NotifFil).ToString() + " ";
                    }
                    else if (property.PropertyType.Name == "DateTime") // IF int VAL
                    {
                        query += modelStartStr + "_" + subStr + ".ToShortDateString() == " + property.GetValue(filters.NotifFil).ToString() + " ";
                    }
                    query += "&& ";
                }
            }
            if (query.Length > 0)
            {
                query = query.Substring(0, query.Length - 3);
            }
            return query;
        }
    }
}
