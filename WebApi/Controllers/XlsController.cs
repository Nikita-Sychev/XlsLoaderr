using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using CoreWebApi.Controllers;
using XlsLoader.Utils;
using XlsLoader.Services;
using XlsLoader.Loaders.DrillOrder;
using XlsLoader.Models.Common;

namespace WebApi.Controllers
{
    public class XlsController : BaseApiController
    {
        private DictionaryService _dicService = null;
        private DrillOrderLoader _drillOrderLoader = null;
        private DrillOrderLoader DrillOrderLoader =>
            _drillOrderLoader ?? (_drillOrderLoader = new DrillOrderLoader(GetUserIdentity()));
        private DictionaryService DicService => _dicService ?? (_dicService = new DictionaryService(GetUserIdentity()));

        [HttpPost]
        [Route("api/Xls/Upload")]
        public async Task<IHttpActionResult> XlsUpload()
        {
            if (!Request.Content.IsMimeMultipartContent())
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

            try
            {
                var provider = new MultipartMemoryStreamProvider();
                await Request.Content.ReadAsMultipartAsync(provider);
                if (provider.Contents == null || provider.Contents.Count == 0)
                    throw new Exception("Отсутствуют данные для загрузки.");
                var inputData = new DrillOrderInputData
                {
                    File = await Utils.GetHttpContent("file", provider.Contents),
                    ProjectVersionId = await Utils.GetHttpContent("projectVersionId", provider.Contents),
                    OrderId = await Utils.GetHttpContent("orderId", provider.Contents),
                    PseudoModuleId = await Utils.GetHttpContent("moduleId", provider.Contents)
                };

                var dicData = DicService.GetDrillOrderDictionarys();
                var result =
                    await CreateResultAsync(() => DrillOrderLoader.UploadDrillOrderExcelAsync(inputData, dicData));

                return result;
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
