using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using XlsLoader.Enums;

namespace XlsLoader.Utils
{
    public static class Utils
    {
        public static void CheckFileType(string fileExt, ActionType action)
        {
            if (ActionType.UploadSourceDataForCalculation != action) return;
            if (!Enum.GetNames(typeof(FileTypeForUpload)).Contains(fileExt))
                throw new Exception("Недопустимый тип файла для загрузки. Выберите: xlsx, xltx, xlam, xlsm, xltm.");
        }

        public static async Task<dynamic> GetHttpContent(string contName, ICollection<HttpContent> content)
        {
            foreach (var item in content)
            {
                if (item.Headers.ContentDisposition.Name.Trim('\"') == contName && contName == "file")
                {
                    CheckFileType(item.Headers.ContentDisposition.FileName.Trim('\"').Split('.')
                        .LastOrDefault(), ActionType.UploadSourceDataForCalculation);
                    return await item.ReadAsByteArrayAsync();
                }

                if (item.Headers.ContentDisposition.Name.Trim('\"') == contName)
                    return Convert.ToDecimal(await item.ReadAsStringAsync() ?? "0");
            }
            return null;
        }
    }
}
