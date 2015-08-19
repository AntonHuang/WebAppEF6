using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Results;
using System.Web.Mvc;

namespace WebApp.Common
{
    public static class JsonMessage
    {
        public static JsonResult BadRequestJsonResult(object value)
        {
            MyJsonResult r = new MyJsonResult();
            r.Data = value;
            r.StatusCode = (int)HttpStatusCode.BadRequest;
            return r;
        }

        public static JsonResult JsonResult(object value)
        {
            MyJsonResult r = new MyJsonResult();
            r.Data = value;
            r.StatusCode = (int)HttpStatusCode.OK;
            return r;
        }



        public static object ToJson(this ICollection<ModelState> ModelStates) {
            if (ModelStates != null) {
                var modelStates = ModelStates.FirstOrDefault();
                if (modelStates != null && modelStates.Errors != null) {
                    var error = modelStates.Errors.FirstOrDefault();
                    if (error != null) {
                        return new {
                            ErrorMessage = error.ErrorMessage
                        };
                    }
                }
            }

            return new
            {
                ErrorMessage = string.Empty
            };
        }


    }


    public class MyJsonResult : JsonResult
    {

        public int StatusCode { get; set; }

        public MyJsonResult()
        {
            ContentEncoding = Encoding.UTF8;
            ContentType = "application/json";
            this.JsonRequestBehavior = JsonRequestBehavior.AllowGet;

        }

        public override void ExecuteResult(ControllerContext context)
        {
            base.ExecuteResult(context);

            context.HttpContext.Response.StatusCode = StatusCode;
        }
    }
}
