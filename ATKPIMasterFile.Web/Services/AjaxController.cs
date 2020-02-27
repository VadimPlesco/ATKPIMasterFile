using System.IO;
using System.IO.Compression;
using System.Security.Principal;
using System.Web.Routing;
using System.Web.Script.Serialization;
using System.Web.Security;
using ATKPIMasterFile.BusinessLogic.Commands;
using ATKPIMasterFile.BusinessLogic.ViewModels;
using ATKPIMasterFile.DataAccess.Model;
using ATKPIMasterFile.Web;

namespace System.Web.Mvc
{
    public partial class AjaxController : Controller
    {
        private string _title;
        public string Title
        {
            get { return _title; }
            set { _title = value; ViewBag.Title = value; }
        }

        public string Path { get; set; }

        public object CustomData { get; set; }
        public ResponseCode Status = ResponseCode.Ok;

        long _currentUserId = -1;
        public long CurrentUserId
        {
            get
            {
                if (_currentUserId == -1)
                {
                    if (!HttpContext.User.Identity.IsAuthenticated)
                        return 0;

                    if (!long.TryParse(HttpContext.User.Identity.Name, out _currentUserId))
                        throw new Exception("Cannot parse user name");
                }
                return _currentUserId;
            }
        }

        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            //var t = filterContext.HttpContext.Request[MailNotificationCommands.TokenName];
            //if (!string.IsNullOrWhiteSpace(t) && !filterContext.HttpContext.User.Identity.IsAuthenticated)
            //{
            //    var userId = MailNotificationCommands.ParseToken(t);
            //    if (userId > 0)
            //    {
            //        FormsAuthentication.SetAuthCookie(userId.ToString(), false);
            //        HttpContext.User = new GenericPrincipal(new GenericIdentity(userId.ToString(), "Forms"), new string[0]);
            //    }
            //}
            base.OnAuthorization(filterContext);
        }

        protected ActionResult AjaxView(string viewName, string routeName, object routeValues, object model = null)
        {
            SetCacheability();
            if (IsJsonRequest())
            {
                Path = UrlHelper.GenerateUrl(routeName, viewName, "" + RouteData.Values["controller"], new RouteValueDictionary(routeValues), Url.RouteCollection, Url.RequestContext, false);
                return JsonPartialView(viewName, model);
            }
            return View(viewName, model);
        }


        protected ActionResult AjaxView(string view, object model)
        {
            SetCacheability();
            if (IsJsonRequest())
                return JsonPartialView(view, model);
            return View(view, model);
        }


        private void SetCacheability()
        {
            if (IsJsonRequest())
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
            //else
            //    Response.Cache.SetVaryByCustom("IsJson");
        }


        protected ActionResult AjaxView(string view)
        {
            return AjaxView(view, null);
        }

        protected ActionResult AjaxView(object model = null)
        {
            return AjaxView(null, model);
        }

        protected ActionResult AjaxRedirectToRoute(RouteEnum route, object routeValues, bool updateBodyOnly = false)
        {
            if (IsJsonRequest())
                return Json(new JsonResponse(updateBodyOnly ? ResponseCode.RedirectToBody : ResponseCode.Ok)
                {
                    RedirectUrl = Url.RouteUrl(route.ToString(), routeValues)
                }, JsonRequestBehavior.AllowGet);

            return RedirectToRoute(route.ToString(), routeValues);
        }


        protected ActionResult AjaxRedirect(string url, bool updateBodyOnly = false)
        {
            if (IsJsonRequest())
                return Json(new JsonResponse(updateBodyOnly ? ResponseCode.RedirectToBody : ResponseCode.Ok)
                {
                    RedirectUrl = url
                }, JsonRequestBehavior.AllowGet);

            return Redirect(url);
        }

        bool? _isJson;
        public bool IsJsonRequest()
        {
            if (!_isJson.HasValue)
                _isJson = IsJsonRequest(Request);
            return _isJson.Value;
        }

        public bool IsModal()
        {
            //const string modalParamName = "modal";
            //if (!Request.QueryString.AllKeys.Contains(modalParamName)) return false;
            return Request.Headers["isModal"] == "true" || Status == ResponseCode.Blocker || Status == ResponseCode.BlockerBottom;//QueryString[modalParamName] == "true";
        }

        private static bool IsJsonRequest(HttpRequestBase request)
        {
            return (request.Headers["Accept"] ?? "").Contains("application/json");
        }

        public ActionResult JsonPartialView(object model)
        {
            return JsonPartialView(null, model);
        }

        public ActionResult AjaxJson(JsonResponse jr)
        {
            var jRes = Json(jr, JsonRequestBehavior.AllowGet);
            jRes.MaxJsonLength = int.MaxValue;
            return jRes;
        }

        public ActionResult JsonPartialView(string viewName, object model)
        {
            var acceptEncoding = HttpContext.Request.Headers["Accept-Encoding"];
            if (acceptEncoding != null)
            {
                if (acceptEncoding.Contains("gzip"))
                {
                    Response.Filter = new GZipStream(Response.Filter, CompressionMode.Compress);
                    Response.AppendHeader("Content-Encoding", "gzip");
                }
                else if (acceptEncoding.Contains("deflate"))
                {
                    Response.Filter = new DeflateStream(Response.Filter, CompressionMode.Compress);
                    Response.AppendHeader("Content-Encoding", "deflate");
                }
            }

            var url = Path ?? Request.Url.PathAndQuery.ToLower().Replace("x-requested-with=xmlhttprequest", "").TrimEnd('?', '&');
            var jRes = Json(new JsonResponse(Status) { Html = RenderPartialViewToString(viewName, model), Url = url, Title = Title, CustomData = CustomData }, JsonRequestBehavior.AllowGet);
            jRes.MaxJsonLength = int.MaxValue;
            return jRes;
        }

        public static void WriteErrorResponse(HttpContext context, object model, string filePath, string layoutPath, string layoutPathBody, string layoutPathModal, string title, string errorMsg)
        {
            var contextWrapper = new HttpContextWrapper(context);
            var controllerContext = new ControllerContext(new RequestContext(contextWrapper, new RouteData()), new AjaxController());
            if (IsJsonRequest(contextWrapper.Request))
            {
                var razor = new RazorView(controllerContext, filePath, context.Request.Headers["isModal"] == "true" ? layoutPathModal : layoutPathBody, false, null);
                using (var stringWriter = new StringWriter())
                {
                    razor.Render(new ViewContext(controllerContext, razor, new ViewDataDictionary(model), new TempDataDictionary(), stringWriter), stringWriter);
                    context.Response.Write(new JavaScriptSerializer().Serialize(new JsonResponse(ResponseCode.Error) { ErrorMessage = errorMsg, Html = stringWriter.GetStringBuilder().ToString(), Url = context.Request.Path, Title = title }));
                }
            }
            else
            {
                var razor = new RazorView(controllerContext, filePath, layoutPath, false, null);
                razor.Render(new ViewContext(controllerContext, razor, new ViewDataDictionary(model), new TempDataDictionary(), context.Response.Output), context.Response.Output);
                context.Response.ContentType = "text/html; charset=utf-8";
            }

        }

        protected string RenderPartialViewToString(string viewName, object model)
        {
            if (string.IsNullOrEmpty(viewName))
            {
                viewName = ControllerContext.RouteData.GetRequiredString("action");
            }

            ViewData.Model = model;

            using (var stringWriter = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, stringWriter);
                viewResult.View.Render(viewContext, stringWriter);

                return stringWriter.GetStringBuilder().ToString();
            }
        }

        protected override void HandleUnknownAction(string actionName)
        {
            throw new Exception("UnknownAction");
        }
    }
}