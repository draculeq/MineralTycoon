using System.Collections.Generic;
using Assets.Buildings.TimeQueue;
using Assets.Currency;
using Assets.Managers;
using Assets.Plugins.Utilities;
using Assets.UI;
using UnityEngine;

namespace Assets
{
    public class Game : Singleton<Game>
    {
        public GUIStyle DebugBuildingGuiStyle;
        public static bool ShowDebugData;
        public static IWallet Wallet;
        public static Map Map;
        public static ProductionQueue ProductionQueue;
        public static Transporter Transporter;
        public static Database.Database Database;
        public static bool Started = false;
        public static int CoroutineLifetime = 15;
        private static MenuRoot _menuRoot;

        public List<MeshRenderer> Trees;
        protected override void Awake()
        {
            base.Awake();
            Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.ScriptOnly);
            ShowDebugData = false;

            Wallet = GetComponentInChildren<Wallet>();
            Database = GetComponentInChildren<Database.Database>();
            Transporter = GetComponentInChildren<Transporter>();
            Map = GetComponentInChildren<Map>();
            _menuRoot = FindObjectOfType<MenuRoot>();
            
            ProductionQueue = new GameObject("Production Queue").AddComponent<ProductionQueue>();
            ProductionQueue.transform.SetParent(transform);
        }

        void Start()
        {
            _menuRoot.Init();
        }

        float _deltaTime = 0.0f;

        void Update()
        {
            _deltaTime += (Time.deltaTime - _deltaTime) * 0.1f;
            if (Input.GetKey(KeyCode.Alpha1))
                QualitySettings.shadowResolution = ShadowResolution.VeryHigh;
            if (Input.GetKey(KeyCode.Alpha2))
                QualitySettings.shadowResolution = ShadowResolution.High;
            if (Input.GetKey(KeyCode.Alpha3))
                QualitySettings.shadowResolution = ShadowResolution.Medium;
            if (Input.GetKey(KeyCode.Alpha4))
                QualitySettings.shadowResolution = ShadowResolution.Low;
            if (Input.GetKey(KeyCode.F1))
                QualitySettings.shadowDistance -= 0.5f;
            if (Input.GetKey(KeyCode.F2))
                QualitySettings.shadowDistance += 0.5f;
        }
        
        void OnGUI()
        {
            int w = Screen.width, h = Screen.height;

            GUIStyle style = new GUIStyle();

            Rect rect = new Rect(0, 0, w, h * 2 / 100);
            style.alignment = TextAnchor.UpperLeft;
            style.fontSize = h * 2 / 100;
            style.normal.textColor = new Color(0.0f, 0.0f, 0.5f, 1.0f);
            float msec = _deltaTime * 1000.0f;
            float fps = 1.0f / _deltaTime;
            string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);

            GUI.Label(rect, text, style);
        }

    }
}
