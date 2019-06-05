using ATKPIMasterFile.DataAccess.Model;

namespace ATKPIMasterFile.BusinessLogic.ViewModels
{
    public class JsonResponse
    {
        public JsonResponse(ResponseCode code = ResponseCode.Ok)
        {
            Status = code.ToString();
        }

        public string Status { get; set; }

        public string ErrorMessage { get; set; }

        public string Html { get; set; }

        public string Url { get; set; }

        public string Title { get; set; }

        public string RedirectUrl { get; set; }

        public object CustomData { get; set; }
    }
}