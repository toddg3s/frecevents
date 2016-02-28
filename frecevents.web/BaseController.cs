using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace frecevents.web
{
  public class BaseController : Controller
  {
    protected bool IsPostBack()
    {
      bool isPost = string.Compare(Request.HttpMethod, "POST",
        StringComparison.CurrentCultureIgnoreCase) == 0;
      if (Request.UrlReferrer == null) return false;

      bool isSameUrl = string.Compare(Request.Url.AbsolutePath,
        Request.UrlReferrer.AbsolutePath,
        StringComparison.CurrentCultureIgnoreCase) == 0;

      return isPost && isSameUrl;
    }
  }
}
