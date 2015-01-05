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
    public class MedewerkerController : ApiController
    {
        [Authorize]
        public List<Medewerker> Get()
        {
            ClaimsPrincipal cp = RequestContext.Principal as ClaimsPrincipal;
            return MedewerkerDA.GetMedewerkers(cp.Claims);
        }

        public HttpResponseMessage Post(Medewerker m)
        {
            ClaimsPrincipal cp = RequestContext.Principal as ClaimsPrincipal;
            int id = MedewerkerDA.InsertMedewerker(m, cp.Claims);

            HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.OK);
            message.Content = new StringContent(id.ToString());
            return message;
        }


        public HttpResponseMessage Put(Medewerker m)
        {
            ClaimsPrincipal cp = RequestContext.Principal as ClaimsPrincipal;
            MedewerkerDA.UpdateMedewerker(m, cp.Claims);

            return new HttpResponseMessage(HttpStatusCode.OK);
        }


        public HttpResponseMessage Delete(int id)
        {
            ClaimsPrincipal cp = RequestContext.Principal as ClaimsPrincipal;
            MedewerkerDA.DeleteMedewerker(id, cp.Claims);

            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
}
