using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Web.Optimization;

namespace ProductionPortal_Solb
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            // jQuery must be loaded only once.
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                "~/assets/js/jquery.min.js"
            ));

            // Core CSS used by the complete application.
            bundles.Add(new StyleBundle("~/bundles/core-css").Include(
                "~/assets/css/bootstrap.min.css",
                "~/assets/css/style.css"
            ));

            // Plugin CSS currently used by the layout/application.
            bundles.Add(new StyleBundle("~/bundles/plugin-css").Include(
                "~/assets/plugins/chartist-js/chartist.min.css",
                "~/assets/plugins/switchery/switchery.min.css",
                "~/assets/plugins/datepicker/datepicker.min.css",
                "~/assets/plugins/datatables/dataTables.bootstrap4.min.css",
                "~/assets/plugins/datatables/buttons.bootstrap4.min.css",
                "~/assets/plugins/datatables/responsive.bootstrap4.min.css"
            ));

            // Core JavaScript. jQuery is intentionally not repeated here.
            bundles.Add(new ScriptBundle("~/bundles/core-js").Include(
                "~/assets/js/popper.min.js",
                "~/assets/js/bootstrap.min.js",
                "~/assets/js/modernizr.min.js",
                "~/assets/js/detect.js",
                "~/assets/js/jquery.slimscroll.js",
                "~/assets/js/sidebar-menu.js",
                "~/assets/js/main.js"
            ));

            // Application plugins.
            bundles.Add(new ScriptBundle("~/bundles/plugin-js").Include(
                "~/assets/plugins/datatables/jquery.dataTables.min.js",
                "~/assets/plugins/datatables/dataTables.bootstrap4.min.js",
                "~/assets/plugins/datatables/dataTables.buttons.min.js",
                "~/assets/plugins/datatables/buttons.bootstrap4.min.js",
                "~/assets/plugins/datatables/jszip.min.js",
                "~/assets/plugins/datatables/pdfmake.min.js",
                "~/assets/plugins/datatables/vfs_fonts.js",
                "~/assets/plugins/datatables/buttons.html5.min.js",
                "~/assets/plugins/datatables/buttons.print.min.js",
                "~/assets/plugins/datatables/buttons.colVis.min.js",
                "~/assets/plugins/datatables/dataTables.responsive.min.js",
                "~/assets/plugins/datatables/responsive.bootstrap4.min.js",

                "~/assets/plugins/chartist-js/chartist.min.js",
                "~/assets/plugins/chartist-js/chartist-plugin-tooltip.min.js",

                "~/assets/plugins/datepicker/datepicker.min.js",
                "~/assets/plugins/datepicker/i18n/datepicker.en.js",

                "~/assets/plugins/switchery/switchery.min.js",

                "~/assets/js/init/to-do-list-init.js",
                "~/assets/js/init/dashborad.js",
                "~/assets/js/init/switchery-init.js"
            ));

            // Enable minification and bundling in production.
            BundleTable.EnableOptimizations = true;
        }
    }
}