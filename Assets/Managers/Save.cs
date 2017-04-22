
using System.Collections;
using System.Collections.Generic;
using Assets.Currency;
#if UNITY_EDITOR || UNITY_STANDALONE
using System.IO;
#endif
using Assets.Plugins.Utilities;
using UnityEngine;

namespace Assets.Managers
{
    class Save
    {
        private static readonly string MapSaveKey = "mapSave";
        private static readonly string CashSaveKey = "cashSave";
        public static void SaveGame(Map map, IWallet wallet)
        {
            SaveWallet(wallet);
            SaveMap(map);
        }

        public static void LoadSavedGame(Map map, IWallet wallet)
        {
            LoadWallet(wallet);
            LoadMap(map);
        }

        private static void SaveWallet(IWallet wallet)
        {
            var json = JsonUtility.ToJson(wallet);
            DeadbitLog.Log("Saving wallet...", LogCategory.General, LogPriority.Medium);
            DeadbitLog.Log("Save wallet: " + json, LogCategory.General, LogPriority.Low);
#if UNITY_EDITOR || UNITY_STANDALONE
            DeadbitPrefs.Save(CashSaveKey, json);
#else
            PlayerPrefs.SetString(CashSaveKey, json);
            PlayerPrefs.Save();
#endif
        }
        private static void SaveMap(Map map)
        {
            var json = JsonUtility.ToJson(map);
            Debug.Log(json);
            DeadbitLog.Log("Saving map...", LogCategory.Buildings, LogPriority.Medium);
            DeadbitLog.Log("Save map: " + json, LogCategory.Buildings, LogPriority.Low);
#if UNITY_EDITOR || UNITY_STANDALONE
            DeadbitPrefs.Save(MapSaveKey, json);
#else
            PlayerPrefs.SetString(MapSaveKey, json);
            PlayerPrefs.Save();
#endif
        }
        private static void LoadWallet(IWallet wallet)
        {
            string json = "";
#if UNITY_EDITOR || UNITY_STANDALONE
            json = DeadbitPrefs.Load(CashSaveKey, "");
#else
            json = PlayerPrefs.GetString(CashSaveKey);
#endif
            JsonUtility.FromJsonOverwrite(json, wallet);
        }
        private static void LoadMap(Map map)
        {
            var json = "";
            DeadbitLog.Log("Loading..." + json, LogCategory.Buildings, LogPriority.Medium);
#if UNITY_EDITOR || UNITY_STANDALONE
            json = DeadbitPrefs.Load(MapSaveKey, "");
#else
            json = PlayerPrefs.GetString(MapSaveKey);
#endif
            DeadbitLog.Log("Loaded save: " + json, LogCategory.Buildings, LogPriority.Low);
            JsonUtility.FromJsonOverwrite(json, map);
        }

        public static void SaveMapFields(MapFields map)
        {
#if UNITY_EDITOR
            var json = JsonUtility.ToJson(map);
            File.WriteAllText(Application.streamingAssetsPath + "/" + map.MapName, json);
#endif
        }
        public static IEnumerator LoadMapFields(string mapName)
        {
            var json = "";
#if UNITY_EDITOR || UNITY_STANDALONE
            var path = Application.streamingAssetsPath + "/" + mapName;
            if (File.Exists(path))
                json = File.ReadAllText(path);
#else
            var path = "jar:file://" + Application.dataPath + "!/assets/" + mapName;
            var www = new WWW(path);
            yield return www;
            json = www.text;
#endif
            yield return JsonUtility.FromJson<MapFields>(json);
        }
    }
}
