using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace frecevents.web
{
  public class HandleCustomError : HandleErrorAttribute
  {
    public override void OnException(ExceptionContext filterContext)
    {
      if (filterContext.ExceptionHandled)
      {
        return;
      }
      else
      {
        //Determine the return type of the action
        string actionName = filterContext.RouteData.Values["action"].ToString();
        Type controllerType = filterContext.Controller.GetType();
        var method = controllerType.GetMethod(actionName);

        Root.Log.Error(filterContext.Exception, "Unhandled exception.  Controller={0}, Action={1},Method={2}: {3}", controllerType.Name, actionName, method, filterContext.HttpContext.Request.RawUrl);

        var returnType = method.ReturnType;

        //If the action that generated the exception returns a view
        if (returnType.Equals(typeof(ActionResult))
        || (returnType).IsSubclassOf(typeof(ActionResult)))
        {
          filterContext.Result = new ViewResult
          {
            ViewName = "/Home/Error"
          };
        }


      }

      //Make sure that we mark the exception as handled
      filterContext.ExceptionHandled = true;
    }
  }
}
