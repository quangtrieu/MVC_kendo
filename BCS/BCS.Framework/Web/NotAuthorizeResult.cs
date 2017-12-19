using System.Web.Mvc;

namespace BCS.Framework.Web
{
    /// <summary>
    /// Sends 401 Unauthorized to the response.
    /// </summary>
    public class NotAuthorizeResult : ActionResult
    {
        private bool IsNTLM { get; set; }

        /// <summary>
        /// Initializes a new instance of the NotAuthorizeResult class
        /// </summary>
        public NotAuthorizeResult()
        {
            IsNTLM = true;
        }

        /// <summary>
        /// Initializes a new instance of the NotAuthorizeResult class by using the NTLM or not
        /// </summary>
        /// <param name="isNTLM"></param>
        public NotAuthorizeResult(bool isNTLM)
        {
            IsNTLM = isNTLM;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.Buffer = true;
            context.HttpContext.Response.StatusCode = 401;
            context.HttpContext.Response.StatusDescription = "Unauthorized";

            if (IsNTLM)
            {
                context.HttpContext.Response.AddHeader("WWW-Authenticate", "NTLM");
            }

            context.HttpContext.Response.End();
        }
    }

    /// <summary>
    /// Sends file not found code to the response.
    /// </summary>
    public class FileNotFoundResult : ActionResult
    {
        /// <summary>
        /// Enables processing of the result of an action method by a custom type that inherits from the ActionResult class. 
        /// </summary>
        /// <param name="context">The context within which the result is executed.</param>
        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.Buffer = true;
            context.HttpContext.Response.StatusCode = 404;
            context.HttpContext.Response.StatusDescription = "File not found";
            
            context.HttpContext.Response.End();
        }
    }
}