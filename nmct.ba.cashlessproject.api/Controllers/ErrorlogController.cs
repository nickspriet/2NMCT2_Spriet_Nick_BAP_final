using nmct.ba.cashlessproject.api.DataAccess;
using nmct.ba.cashlessproject.model.Management;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;

namespace nmct.ba.cashlessproject.api.Controllers
{
    public class ErrorlogController : ApiController
    {
        [Authorize]
        public List<Errorlog> Get()
        {
            ClaimsPrincipal cp = RequestContext.Principal as ClaimsPrincipal;
            return ErrorlogDA.GetErrorlogs(cp.Claims);
        }

        public HttpResponseMessage Post(Errorlog e)
        {
            ClaimsPrincipal cp = RequestContext.Principal as ClaimsPrincipal;
            int id = ErrorlogDA.InsertErrorlogs(e, cp.Claims);

            HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.OK);
            message.Content = new StringContent(id.ToString());
            return message;
        }

       public HttpResponseMessage Delete()
       {
           ClaimsPrincipal cp = RequestContext.Principal as ClaimsPrincipal;
           ErrorlogDA.DeleteErrorlogs(cp.Claims);
           return new HttpResponseMessage(HttpStatusCode.OK);
       }
    }
}
