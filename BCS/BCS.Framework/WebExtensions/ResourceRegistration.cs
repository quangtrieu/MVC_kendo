using System.Globalization;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace BCS.Framework.WebExtensions
{
    /// <summary>
    /// Resource Registration
    /// </summary>
    public class ResourceRegistration : IHtmlString
    {
        private HtmlHelper Helper { get; set; }

        /// <summary>
        /// Contructor of ResourceRegistration class
        /// </summary>
        /// <param name="helper">The HTML helper instance that this method extends.</param>
        public ResourceRegistration(HtmlHelper helper)
        {
            Helper = helper;
        }

        readonly StringBuilder builder = new StringBuilder();

        /// <summary>
        /// Add common framework style sheets and scripts
        /// </summary>
        /// <returns>Resource resgistration</returns>
        public ResourceRegistration Common(bool jquery)
        {
            
            if (jquery)
            {
                AppendScript("~/Assets/plugin/jquery/js/jquery.min.js");
            }

            //AppendScript("~/Assets/plugin/jquery/json2.js");
            //AppendScript("~/Assets/plugin/jquery/jquery.cookie.js");
            //AppendScript("~/Assets/plugin/jquery/jquery.hotkeys.js");

            return this;
        }

        /// <summary>
        /// Add reference script file with specific path
        /// </summary>
        /// <param name="fileName">script file path</param>
        /// <returns>Resource resgistration</returns>
        public ResourceRegistration Script(string fileName)
        {
            AppendScript(fileName);

            return this;
        }

        /// <summary>
        /// Append style link tag
        /// </summary>
        /// <param name="style"></param>
        /// <returns>Resource resgistration</returns>
        public ResourceRegistration Style(string style)
        {
            AppendCss(style);

            return this;
        }

        /// <summary>
        /// Append all style sheet and script resource
        /// </summary>
        /// <returns>Resource resgistration</returns>
        public ResourceRegistration All()
        {
            return All(true);
        }

        /// <summary>
        /// Append all style sheet and script resource
        /// </summary>
        /// <returns>Resource resgistration</returns>
        public ResourceRegistration All(bool jquery)
        {
            //GlobalParameter();

            Common(jquery);

            //JqueryUI();

            //Dialog();

            //UploadArea();

            //MultiUpload();

            //PeoplePicker();

            //DatePicker();

           // Spitter();

            //Numeric();

            //TreeView();

            //CKEditor();

            //Grid(true);

            //AppendScript("~/Assets/js/base/base.js");
            //AppendScript("~/Assets/js/base/setaJs.js");
            //AppendScript("~/Assets/js/base/setaJs.modules.js");
           

            //Validate();

            return this;
        }

        public ResourceRegistration GlobalParameter()
        {
            AppendLine("<script>");
                //AppendFormat("var rootPath = '{0}';", Helper.Url().Content("~/"));
                //AppendFormat("var uploadPath = '{0}';", Helper.Url().Action("AsyncUpload", "Attachment"));
            AppendLine("</script>");

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ResourceRegistration Spitter()
        {
            AppendScript("~/Scripts/fw/splitter/jquery.splitter.1.5.1.js");

            return this;
        }

        /// <summary>
        /// Script and CSS support for Nummeric
        /// </summary>
        /// <returns></returns>
        public ResourceRegistration Numeric()
        {
            AppendCss("~/Scripts/fw/numeric/jquery-ui-numeric.css");
            AppendScript("~/Scripts/fw/numeric/jquery-ui-numeric.js");

            return this;
        }

        #region Chart

        /// <summary>
        /// Append chart style sheet and script resource
        /// </summary>
        /// <returns>Resource resgistration</returns>
        //public ResourceRegistration Chart()
        //{
        //    AppendScript("~/Assets/plugin/jqplot/jquery.jqplot.js");

        //    AppendCss("~/Assets/plugin/jqplot/jquery.jqplot.css");

        //    return this;
        //}

        #endregion

        #region People Picker

        /// <summary>
        /// Append people picker style sheet and script resource
        /// </summary>
        /// <returns>Resource resgistration</returns>
        //public ResourceRegistration PeoplePicker()
        //{
        //    AppendCss("~/Content/fw/jquery.peoplePicker.css");

        //    AppendCss("~/Content/fw/jquery.contextMenu.css");

        //    //AppendCss("~/Scripts/fw/dialog/css/confirm.css");

        //    AppendScript("~/Scripts/fw/contextMenu/jquery.contextMenu.js");

        //    //AppendScript("~/Scripts/fw/dialog/js/jquery.simplemodal.js");

        //    //AppendScript("~/Scripts/fw/peoplePicker/jquery.peoplePicker.js");

        //    return this;
        //} 

        #endregion

        #region JqueryUI

        /// <summary>
        /// Append jquery ui style sheet and script resource
        /// </summary>
        /// <returns>Resource resgistration</returns>
        public ResourceRegistration JqueryUI()
        {
            AppendCss("~/Assets/plugin/jqueryui/css/jquery-ui-1.10.3.custom.min.css");

            AppendScript("~/Assets/plugin/jqueryui/js/jquery-ui-1.10.3.custom.min.js");

            return this;
        }

        #endregion

        #region Dialog

        /// <summary>
        /// Append dialog style sheet files and scripts resource
        /// </summary>
        /// <returns>Resource resgistration</returns>
        public ResourceRegistration Dialog()
        {
            AppendScript("~/Assets/plugin/dialog/ModalForms.ValidationSummary.js");

            return this;
        }

        #endregion

        #region Multiupload

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Resource resgistration</returns>
        //public ResourceRegistration UploadArea()
        //{
        //    AppendCss("~/Scripts/fw/multiupload/advanced/jquery.fileupload-ui.css");

        //    AppendScript("~/Scripts/fw/multiupload/advanced/jquery.iframe-transport.js");

        //    //AppendScript("~/Scripts/fw/multiupload/advanced/jquery.xdr-transport.js");

        //    AppendScript("~/Scripts/fw/multiupload/advanced/jquery.fileupload.js");

        //    //AppendScript("~/Scripts/fw/multiupload/advanced/jquery.fileupload-ui.js");

        //    return this;
        //}

        /// <summary>
        /// Append multi upload resource
        /// </summary>
        /// <returns>Resource resgistration</returns>
        public ResourceRegistration MultiUpload()
        {
#if DEBUG
            AppendScript("~/Assets/plugin/multiupload/jquery.MultiFile.js");
#else
            AppendScript("~/Assets/plugin/multiupload/jquery.MultiFile.pack.js");
#endif
            return this;
        } 

        #endregion

        #region DatePicker

        /// <summary>
        /// Append date picker resource to web page
        /// </summary>
        /// <returns>Resource resgistration</returns>
        //public ResourceRegistration DatePicker()
        //{
        //    if (CultureInfo.CurrentCulture.Name.Equals("ja-JP") || CultureInfo.CurrentCulture.Name.Equals("ja"))
        //        AppendScript("~/Scripts/fw/datepicker/jquery.ui.datepicker-ja.js");

        //    return this;
        //} 

        #endregion

        #region Validation

        /// <summary>
        /// Append date picker resource to web page
        /// </summary>
        /// <returns>Resource resgistration</returns>
        public ResourceRegistration Validate()
        {
            AppendScript("~/Assets/plugin/validation/jquery.validate.min.js");
            AppendScript("~/Assets/plugin/validation/jquery.validate.unobtrusive.min.js");

           //if (CultureInfo.CurrentCulture.Name.Equals("en-US") || CultureInfo.CurrentCulture.Name.Equals("en"))
           //    AppendScript("~/Assets/plugin/localization/messages_eu.js");

            return this;
        } 

        #endregion

        #region TreeView

        /// <summary>
        /// Append tree view resource to web page
        /// </summary>
        /// <returns>Resource resgistration</returns>
        public ResourceRegistration TreeView()
        {
            AppendScript("~/Assets/plugin/treeview/jquery.jstree.js");

            return this;
        }
        
        #endregion

        #region CK Editor

        /// <summary>
        /// Append CKEditor resource to web page
        /// </summary>
        /// <returns>Resource resgistration</returns>
        //public ResourceRegistration CKEditor()
        //{
        //    AppendCss("~/Scripts/fw/ckeditor/skins/office2003/editor.css?t=A1QD");

        //    AppendScript("~/Scripts/fw/ckeditor/ckeditor.js");

        //    AppendScript("~/Scripts/fw/ckeditor/adapters/jquery.js");

        //    if (CultureInfo.CurrentCulture.Name.Equals("ja-JP") || CultureInfo.CurrentCulture.Name.Equals("ja"))
        //        AppendScript("~/Scripts/fw/ckeditor/ckeditor.lang.ja.js");
        //    else
        //        AppendScript("~/Scripts/fw/ckeditor/ckeditor.lang.en.js");

        //    AppendCss("~/Scripts/fw/ajax-uploader/fileuploader.css");

        //    AppendScript("~/Scripts/fw/ajax-uploader/fileuploader.js");

        //    return this;
        //} 

        #endregion

        #region Grid Script

        //private const string GRID_SCRIPT_FOLDER = "~/Assets/plugin/jqGrid/";

        //private static readonly string[] PluginScript = new[]
        //                                     {
        //                                         "plugins/grid.addons.js",
        //                                         "plugins/grid.postext.js",
        //                                         "plugins/grid.setcolumns.js",
        //                                         //"plugins/jquery.contextmenu.js",
        //                                         "plugins/jquery.searchFilter.js",
        //                                         "plugins/jquery.tablednd.js",
        //                                         //"plugins/ui.multiselect.js"
        //                                     };

        ///// <summary>
        ///// Append grid resource to web page
        ///// </summary>
        ///// <returns>Resource resgistration</returns>
        //public ResourceRegistration Grid()
        //{
        //    AppendCss(GRID_SCRIPT_FOLDER + "ui.jqgrid.css");

        //    if (CultureInfo.CurrentCulture.Name.Equals("ja-JP") || CultureInfo.CurrentCulture.Name.Equals("ja"))
        //        AppendScript(GRID_SCRIPT_FOLDER + "i18n/grid.locale-ja.js");
        //    else
        //        AppendScript(GRID_SCRIPT_FOLDER + "i18n/grid.locale-en.js");

        //    AppendScript(GRID_SCRIPT_FOLDER + "jquery.jqGrid.min.js");

        //    return this;
        //}

        //public ResourceRegistration Grid(bool isIncludePlugin)
        //{
        //    Grid();

        //    if (isIncludePlugin)
        //    {
        //        AppendCss(GRID_SCRIPT_FOLDER + "ui.multiselect.css");

        //        foreach (var script in PluginScript)
        //        {
        //            builder.AppendLine(ScriptTag(UrlHelper.GenerateContentUrl(GRID_SCRIPT_FOLDER + script, Helper.ViewContext.HttpContext)));
        //        }
        //    }

        //    return this;
        //} 

        #endregion
        
        /// <summary>
        /// Render resource to view
        /// </summary>
        public void Render()
        {
            Helper.ViewContext.Writer.Write(builder.ToString());
        }

        public override string ToString()
        {
            return builder.ToString();
        }

        public string ToHtmlString()
        {
            return builder.ToString();
        }

        private void AppendFormat(string format, params object[] args)
        {
            builder.AppendFormat(format, args);
        }

        private void AppendLine(string textline)
        {
            builder.AppendLine(textline);
        }

        private void AppendScript(string scriptPath)
        {
            builder.AppendLine(ScriptTag(UrlHelper.GenerateContentUrl(scriptPath, Helper.ViewContext.HttpContext)));
        }

        private void AppendCss(string cssPath)
        {
            builder.AppendLine(StyleTag(UrlHelper.GenerateContentUrl(cssPath, Helper.ViewContext.HttpContext)));
        }

        private string StyleTag(string path)
        {
            return string.Format("<link href=\"{0}\" rel=\"stylesheet\" type=\"text/css\" />", path);
        }

        private string ScriptTag(string path)
        {
            return string.Format("<script src=\"{0}\" type=\"text/javascript\"></script>", path);
        }
    }
}