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
    public class KassaMedewerkerController : ApiController
    {
        [Authorize]
        public List<KassaMedewerker> Get()
        {
            ClaimsPrincipal cp = RequestContext.Principal as ClaimsPrincipal;
            return KassaMedewerkerDA.GetKassaMedewerkers(cp.Claims);
        }


        public HttpResponseMessage Post(KassaMedewerker km)
        {
            ClaimsPrincipal cp = RequestContext.Principal as ClaimsPrincipal;
            int id = KassaMedewerkerDA.InsertKassaMedewerker(km, cp.Claims);

            HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.OK);
            message.Content = new StringContent(id.ToString());
            return message;
        }
    }
}
