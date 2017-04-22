using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Plugins.Utilities
{
    public static class DeadbitPrefs
    {
        static string directoryPath = "Saves";
        static string fileName = "save.mt";
        static string FilePath { get { return Path.Combine(directoryPath, fileName); } }

        public static void Save<T1, T2>(T1 Key, T2 Value) where T1 : IConvertible where T2 : IConvertible
        {
            try
            {
                CheckFileExisting();
                SaveDictionary(AddKey(Key.ToString(CultureInfo.InvariantCulture), Value.ToString(CultureInfo.InvariantCulture)));
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public static T2 Load<T1, T2>(T1 Key, T2 DefaultValue = default(T2)) where T1 : IConvertible where T2 : IConvertible
        {
            try
            {
                CheckFileExisting();
                var dic = ReadPrefs();
                try
                {
                    return dic.ContainsKey(Key.ToString(CultureInfo.InvariantCulture))
                        ? (T2)((IConvertible)dic[Key.ToString(CultureInfo.InvariantCulture)]).ToType(typeof(T2),
                            CultureInfo.InvariantCulture)
                        : DefaultValue;
                }
                catch (Exception e)
                {
                    Debug.LogWarning(e.Message);
                }
            }
            catch (Exception)
            {
                // ignored
            }
            return DefaultValue;
        }
        public static void DeleteKey(string key)
        {
            CheckFileExisting();
            SaveDictionary(RemoveKey(key.ToString(CultureInfo.InvariantCulture)));
        }

        public static string ReadString(string key, string defaultValue)
        {
            var dic = ReadPrefs();
            return dic.ContainsKey(key) ? dic[key] : defaultValue;

        }

        private static Dictionary<string, string> AddKey(string key, string value)
        {
            var dic = ReadPrefs();
            if (dic.ContainsKey(key))
                dic[key] = value;
            else
                dic.Add(key, value);
            return dic;
        }

        private static Dictionary<string, string> RemoveKey(string key)
        {
            var dic = ReadPrefs();
            if (dic.ContainsKey(key))
                dic.Remove(key);
            return dic;
        }

        private static void SaveDictionary(Dictionary<string, string> dic)
        {
            try
            {
                var longstring = string.Join("\n", dic.Select(m => m.Key + ";" + m.Value).ToArray());//.XOR();
                File.WriteAllText(FilePath, longstring);
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }

        private static Dictionary<string, string> ReadPrefs()
        {
            var dic = new Dictionary<string, string>();
            try
            {
                var all = new List<string>();

                var val = File.ReadAllBytes(FilePath);
                var str = Encoding.UTF8.GetString(val);//.XOR();
                if (!string.IsNullOrEmpty(str))
                    all = str.Split('\n').Where(a => (!string.IsNullOrEmpty(a) && a.Length > 2)).ToList();

                foreach (var s in all)
                {
                    var splitted = s.Split(';');
                    if (splitted.Length != 2)
                    {
                        Debug.LogWarning("Something went wrong! : " + s);
                        continue;
                    }
                    if (!dic.ContainsKey(splitted[0]))
                        dic.Add(splitted[0], splitted[1]);
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
            return dic;
        }

        private static void CheckFileExisting()
        {
            try
            {
                if (!Directory.Exists(directoryPath))
                    Directory.CreateDirectory(directoryPath);
                if (!File.Exists(FilePath))
                    using (File.Create(FilePath))
                    {
                    }
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }
    }
}