using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Assets.GoogleSheet;
using Assets.Plugins.Utilities;
using UnityEngine;

namespace Assets.UI.BuildingsList
{
    public class BuildingsList : MonoBehaviour
    {
        [SerializeField]
        private BuildingListElement _buildingListElementPrefab;

        private bool _inited = false;
        public void Init()
        {
            if (_inited)
                return;
            _inited = true;
            StartCoroutine(Init(Database.Database.Buildings));
        }
        
        private IEnumerator Init(ReadOnlyCollection<Building> buildings)
        {
            yield return null;

            var stopwatchAnalytics = new Stopwatch();
            var stopwatch = new Stopwatch();

            stopwatchAnalytics.Start();
            stopwatch.Start();
            foreach (var building in buildings)
            {
                (Instantiate(_buildingListElementPrefab, transform, false) as BuildingListElement).Init(building);
                if (stopwatch.ElapsedMilliseconds > Game.CoroutineLifetime)
                {
                    yield return null;
                    stopwatch.Reset();
                    stopwatch.Start();
                }
            }
            DeadbitLog.Log("Creating UI buildings list: " + stopwatchAnalytics.ElapsedMilliseconds + "ms", LogCategory.Analytics, LogPriority.Low);

            yield break;
        }
    }
}
