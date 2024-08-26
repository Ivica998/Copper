using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Copper.Helpers
{
    public static class SavingHelper
    {
        public static void Save<T>(string path, T obj) where T : class, new()
        {
            using (FileStream fs = File.OpenWrite(path))
            {
                StreamWriter sw = new StreamWriter(fs);
                var props = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (var prop in props)
                {
                    if (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
                    {
                        Type itemType = prop.PropertyType.GetGenericArguments()[0];
                        dynamic list = (dynamic)prop.GetValue(obj);
                        StringBuilder sb = new StringBuilder();
                        foreach (var item in list)
                        {
                            sb.Append(item + "|");
                        }
                        if (sb.Length > 0)
                            sb.Remove(sb.Length - 1, 1);
                        sw.WriteLine(prop.Name + "=" + sb.ToString() + ";");
                    }
                    else if (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                    {
                        dynamic list = (dynamic)prop.GetValue(obj);
                        StringBuilder sb = new StringBuilder();
                        foreach (var item in list)
                        {
                            sb.Append(item.Key + ":" + item.Value + "|");
                        }
                        if (sb.Length > 0)
                            sb.Remove(sb.Length - 1, 1);
                        sw.WriteLine(prop.Name + "=" + sb.ToString() + ";");
                    }
                    else if (prop.PropertyType.IsEnum == true)
                        sw.WriteLine(prop.Name + "=" + prop.GetValue(obj) + ";");
                    else
                        sw.WriteLine(prop.Name + "=" + prop.GetValue(obj) + ";");
                }
                sw.Close();
            }

        }

        public static T Load<T>(string path) where T : class, new()
        {
            T instance = new T();

            using (FileStream fs = File.OpenRead(path))
            {
                StreamReader sr = new StreamReader(fs);
                var props = typeof(T).GetType().GetProperties(BindingFlags.Public);
                string output = sr.ReadToEnd();
                output.Replace("\n", "");
                string[] members = output.Split(';', StringSplitOptions.RemoveEmptyEntries);
                foreach (var item in members)
                {
                    string[] itemString = output.Split('=', StringSplitOptions.None);
                    PropertyInfo prop = typeof(T).GetProperty(itemString[0]);
                    if (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
                    {
                        Type itemType = prop.PropertyType.GetGenericArguments()[0];
                        List<object> valueList = new List<object>();
                        string[] valueString = itemString[1].Split('|', StringSplitOptions.None);
                        foreach (var value in valueString)
                        {
                            object valObj = Activator.CreateInstance(itemType);
                            valObj = Convert.ChangeType(value, itemType);
                            valueList.Add(valObj);
                        }
                        prop.SetValue(instance, valueList);
                    }
                    else if (prop.PropertyType.IsEnum == true)
                        prop.SetValue(instance, itemString[1]);
                    else
                        prop.SetValue(instance, itemString[1]);

                }
                sr.Close();

            }

            return instance;
        }
    }
}
