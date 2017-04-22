using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.GoogleSheet
{
    public interface IDownloader
    {
        IEnumerator<DownloadResult> DownloadBuildingsData();
    }
    public class DownloadResult
    {
        public readonly bool IsSuccess;
        public readonly string Data;
        public readonly string Error;
        public DownloadResult(bool isSuccess, string data,  string error)
        {
            IsSuccess = isSuccess;
            Data = data;
            Error = error;
        }
    }
}
