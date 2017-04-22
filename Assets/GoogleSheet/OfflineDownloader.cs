using System.Collections.Generic;
namespace Assets.GoogleSheet
{
    public class OfflineDownloader : IDownloader
    {
        private static readonly string offlineBuildingsDataPath = "/offlineBuildingsData.json";
        private static readonly string offlineProductsDataPath = "/offlineProductsData.json";
        public IEnumerator<DownloadResult> DownloadBuildingsData()
        {
            for (var a = GetDataFromPath(offlineBuildingsDataPath); a.MoveNext();)
                yield return a.Current;
        }

        public IEnumerator<DownloadResult> DownloadProductsData()
        {
            for (var a = GetDataFromPath(offlineProductsDataPath); a.MoveNext();)
                yield return a.Current;
        }

        private static IEnumerator<DownloadResult> GetDataFromPath(string data)
        {
            var path = UnityEngine.Application.streamingAssetsPath + data;

            if (System.IO.File.Exists(path))
            {
                yield return
                    new DownloadResult(true, System.IO.File.ReadAllText(UnityEngine.Application.streamingAssetsPath + data), "");
                yield break;
            }
            else
            {
                yield return new DownloadResult(false, "", "Data file does not exist");
                yield break;
            }
        }

        public static void SaveBuildingsData(string data)
        {
            var path = UnityEngine.Application.streamingAssetsPath + offlineBuildingsDataPath;
            SaveAtPath(path, data);
        }

        public static void SaveProductsData(string data)
        {
            var path = UnityEngine.Application.streamingAssetsPath + offlineProductsDataPath;
            SaveAtPath(path, data);
        }

        private static void SaveAtPath(string path, string data)
        {
            System.IO.File.WriteAllText(path, data);
        }
    }

}
