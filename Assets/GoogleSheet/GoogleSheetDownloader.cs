using System.Collections.Generic;
using Assets.Plugins.Utilities;
using UnityEngine.Networking;

namespace Assets.GoogleSheet
{
    public class GoogleSheetDownloader : IDownloader
    {
        private string baseApi = "https://sheets.googleapis.com/v4/spreadsheets/1cHxP5oWtfoY6R-fYtKdlQrmYlGUs3nP-HFnfaeWJLac";
        private string apiKey = "?key=AIzaSyDgmsCh4oj9JfPEH4137Sxo2NM2wSBzDDg";

        public IEnumerator<DownloadResult> DownloadBuildingsData()
        {
            var www = UnityWebRequest.Get(baseApi + "/values/Base!A8:XX1000" + apiKey);
            DeadbitLog.Log(www.url, LogCategory.Database, LogPriority.Low);

            var request = www.Send();
            while (!request.isDone) //Downloading...
                yield return null;

            if (www.isError) //Return Error
            {
                yield return new DownloadResult(false, "", www.error);
                yield break;
            }

            while (!www.downloadHandler.isDone) //Downloading data
                yield return null;

            yield return new DownloadResult(true, www.downloadHandler.text, "");
            yield break;
        }

        public IEnumerator<DownloadResult> DownloadProductsData()
        {
            var www = UnityWebRequest.Get(baseApi + "/values/Products!A3:B1000" + apiKey);
            DeadbitLog.Log(www.url, LogCategory.Database, LogPriority.Low);

            var request = www.Send();
            while (!request.isDone) //Downloading...
                yield return null;

            if (www.isError) //Return Error
            {
                yield return new DownloadResult(false, "", www.error);
                yield break;
            }

            while (!www.downloadHandler.isDone) //Downloading data
                yield return null;

            yield return new DownloadResult(true, www.downloadHandler.text, "");
            yield break;
        }
    }
}
