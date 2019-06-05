using System.IO;
using System.Web.Mvc;

namespace ATKPIMasterFile.BusinessLogic.ViewModels
{
    [ModelBinder(typeof(FileUploadModelBinder))]
    public class FileUpload
    {
        public string Filename { get; set; }
        public Stream InputStream { get; set; }
        public int ContentLength { get; set; }
    }

    public class FileUploadModelBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var request = controllerContext.RequestContext.HttpContext.Request;
            var formUpload = request.Files.Count > 0;

            // find filename
            var xFileName = request.Headers["X-File-Name"];
            var qqFile = request["qqfile"];
            var formFilename = formUpload ? request.Files[0].FileName : null;

            var upload = new FileUpload
            {
                Filename = xFileName ?? qqFile ?? formFilename,
                InputStream = formUpload ? request.Files[0].InputStream : request.InputStream,
                ContentLength = formUpload ? request.Files[0].ContentLength : request.ContentLength
            };

            return upload;
        }
    }
}