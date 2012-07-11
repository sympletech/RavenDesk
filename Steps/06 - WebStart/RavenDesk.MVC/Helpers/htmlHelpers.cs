using System;
using System.Text;
using System.Web.Mvc;

namespace RavenDesk.MVC.Helpers
{
    public static class MyHtmlHelpers
    {
        #region Data Display Helpers


        public static MvcHtmlString DisplayDataField(this HtmlHelper helper, string label, object data)
        {
            var sbOutput = new StringBuilder();
            sbOutput.AppendLine("<div class='display-entry'>");
            sbOutput.AppendLine(String.Format("   <div class='display-label'>{0}</div>", label));
            sbOutput.Append("   <div class='display-field'>");

            data = data ?? "";

            sbOutput.Append(data + "</div>");
            sbOutput.AppendLine("</div>");

            return new MvcHtmlString(sbOutput.ToString());
        }


        public static MvcHtmlString EditDataField(this HtmlHelper helper, string label, MvcHtmlString editor)
        {
            return EditDataField(helper, label, editor, null , null);
        }
       
        public static MvcHtmlString EditDataField(this HtmlHelper helper, string label, MvcHtmlString editor, MvcHtmlString validator)
        {
            return EditDataField(helper, label, editor, validator, null);
        }

        public static MvcHtmlString EditDataField(this HtmlHelper helper, string label, MvcHtmlString editor, MvcHtmlString validator, MvcHtmlString instructions)
        {
            var sbOutput = new StringBuilder();
            sbOutput.AppendFormat("<div class='control-group'>");
            sbOutput.AppendFormat("<label class='control-label'>{0}</label>", label);
            sbOutput.Append("<div class='controls'>");
            sbOutput.Append(editor);

            sbOutput.Append(validator);

            if (string.IsNullOrEmpty((instructions ?? new MvcHtmlString("")).ToString()) != true)
            {
                sbOutput.AppendFormat("<p class='help-block'>{0}</p>", instructions);
            }

            sbOutput.Append(@"</div>");
            sbOutput.Append(@"</div>");

            return new MvcHtmlString(sbOutput.ToString());
        }

        #endregion

        #region General HTML

        public static MvcHtmlString ToHtmlString(this HtmlHelper helper, String sourceString)
        {
            return new MvcHtmlString(sourceString);
        }

        public static MvcHtmlString TimeSpanString(this HtmlHelper helper, TimeSpan? span)
        {
            if (span != null)
            {
                return new MvcHtmlString(new DateTime(span.Value.Ticks).ToString("h:mm tt"));
            }

            return new MvcHtmlString("");
        }

        #endregion

    }
}