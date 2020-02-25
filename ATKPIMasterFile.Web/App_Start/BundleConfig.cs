using System.Web;
using System.Web.Optimization;

namespace ATKPIMasterFile.Web
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            //BundleTable.EnableOptimizations = true;
            
            //bundles.Add(new ScriptBundle("~/bundles/jquery")
            //    .Include("~/Scripts/jquery-{version}.js")
            //    .Include("~/Scripts/jquery-ui.js")
            //            );

            //bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
            //            "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            //bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
            //          "~/Scripts/bootstrap.js",
            //          "~/Scripts/respond.js"));

            //bundles.Add(new StyleBundle("~/Content/css").Include(
            //          "~/Content/bootstrap.css",
            //          "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/bundles/original.css")
              .IncludeDirectory("~/Content/themes/original/css", "*.css", true)
              .Include("~/Content/jquery-ui*")
              .Include("~/Content/jquery.tablescroll.css")
              //.Include("~/Content/jquery.ultraselect.css")
              //.Include("~/Scripts/jtable/themes/metro/brown/jtable.min.css")
              //.Include("~/Scripts/jtable/themes/sunny/jquery-ui-1.10.1.custom.min.css")             
              );

            bundles.Add(new ScriptBundle("~/bundles/dzen.js")
               .Include("~/Content/themes/original/js/*.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquery.js")
                   .Include("~/Scripts/jquery-{version}.js")
                   .Include("~/Scripts/jquery.validate*")
                   );

            bundles.Add(new ScriptBundle("~/bundles/knockout.js")
               // Include("~/Scripts/knockout-3.4.2.js")
              .Include("~/Scripts/knockout-{version}.js")
              .Include("~/Scripts/knockout-{version}.debug.js")
              // Include("~/Scripts/knockout.mapping-latest.js").
              // Include("~/Scripts/knockout-es5.min.js")
              );

            bundles.Add(new ScriptBundle("~/bundles/ATKPIMFApp.js")
              .Include("~/Scripts/ATKPIMFApp/SmileLinkFormatter.js")
              .Include("~/Scripts/ATKPIMFApp/jquery.sm-unobtrusive-ajax.js")
              //.Include("~/Scripts/jquery.unobtrusive-ajax.js")
              .Include("~/Scripts/ATKPIMFApp/modal-window.js")
              .Include("~/Scripts/ATKPIMFApp/popUp.js")
              .Include("~/Scripts/ATKPIMFApp/helpers.js")
              .Include("~/Scripts/ATKPIMFApp/interest-buttons.js")
              .Include("~/Scripts/ATKPIMFApp/feed_grid.js")
              .Include("~/Scripts/ATKPIMFApp/feed.js")
              .Include("~/Scripts/ATKPIMFApp/typeahead.js")
              .Include("~/Scripts/ATKPIMFApp/summernote.js")
              .Include("~/Scripts/ATKPIMFApp/analytics.js")
             );

            bundles.Add(new ScriptBundle("~/bundles/additional.js")
                  .Include("~/Scripts/ATKPIMFApp/broadcasting.publish.js")
                  .Include("~/Scripts/ATKPIMFApp/broadcasting.watch.js")
                  .Include("~/Scripts/ATKPIMFApp/viewModel.notificationsViewModel.js")
                  .Include("~/Scripts/ATKPIMFApp/viewModel.chatViewModel.js")
                  .Include("~/Scripts/ATKPIMFApp/viewModel.js")
                  //.Include("~/Scripts/KinkyApp/viewModel.eventsHub.js")
                );

            bundles.Add(new ScriptBundle("~/bundles/jqueryui.js")
                .Include("~/Scripts/jquery-ui.js")
                .Include("~/Scripts/jquery.ui.widget.js")
                .Include("~/Scripts/jquery.ui.touch-punch.js")
                .Include("~/Scripts/jquery-ui-1.12.1.js")
                .Include("~/Scripts/jquery-ui-1.12.1.min.js")
                .Include("~/Scripts/jquery.Jcrop.js")
                .Include("~/Scripts/jquery.mousewheel.js")
                .Include("~/Scripts/swfobject.js")
                .Include("~/Scripts/jquery.tablesorter.min.js")
                //.Include("~/Scripts/jquery.ultraselect.js")
                );

            bundles.Add(new ScriptBundle("~/bundles/jtablesite.js")
              .Include("~/Scripts/jtable/jquery.jtable.js")           
              //.Include("~/Scripts/jtable/jquery.jtable.min.js")              
              .Include("~/Scripts/jtable/extensions/jquery.jtable.export.js")
              //.Include("~/Scripts/jtable/extensions/jquery.jtable.filters.js")
              .Include("~/Scripts/jtable/extensions/jquery.jtable.footer.js")
              .Include("~/Scripts/jtable/external/jszip.js")
              .Include("~/Scripts/jtable/external/jszip-deflate.js")
              .Include("~/Scripts/jtable/external/jszip-inflate.js")
              .Include("~/Scripts/jtable/external/jszip-load.js")
              .Include("~/Scripts/jtable/external/json2xml.js")
              .Include("~/Scripts/jtable/external/xlsx.js")
              .Include("~/Scripts/jtable/extensions/jquery.jtable.bottompanel.js")
              .Include("~/Scripts/jtable/localization/jquery.jtable.ru.js")
              //.Include("~/Scripts/jtable/extensions/jquery.jtable.core.js")
              //.Include("~/Scripts/jtable/extensions/jquery.jtable.forms.js")
              //.Include("~/Scripts/jtable/extensions/jquery.jtable.editing.js")
              //.Include("~/Scripts/jtable/extensions/jquery.jtable.dynamiccolumns.js")
              //.Include("~/Scripts/jtable/extensions/jquery.jtable.paging.js")
              //.Include("~/Scripts/jtable/extensions/jquery.jtable.sorting.js")
              //.Include("~/Scripts/jtable/extensions/jquery.jtable.utils.js")
              );

        }

       
    }
}
