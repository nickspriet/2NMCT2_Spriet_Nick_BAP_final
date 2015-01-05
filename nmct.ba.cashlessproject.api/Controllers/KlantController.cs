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
    public class KlantController : ApiController
    {
        [Authorize]
        public List<Klant> Get()
        {
            ClaimsPrincipal cp = RequestContext.Principal as ClaimsPrincipal;
            return KlantDA.GetKlanten(cp.Claims);
        }

        public HttpResponseMessage Post(Klant k)
        {
            ClaimsPrincipal cp = RequestContext.Principal as ClaimsPrincipal;
            int id = KlantDA.InsertKlant(k, cp.Claims);

            HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.OK);
            message.Content = new StringContent(id.ToString());
            return message;
        }

        public HttpResponseMessage Put(Klant k)
        {
            ClaimsPrincipal cp = RequestContext.Principal as ClaimsPrincipal;
            KlantDA.UpdateKlant(k, cp.Claims);

            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        /*
        public HttpResponseMessage Delete(int id)
        {
            ClaimsPrincipal cp = RequestContext.Principal as ClaimsPrincipal;
            KlantDA.DeleteCustomer(id, cp.Claims);
            return new HttpResponseMessage(HttpStatusCode.OK);
        }
        */
    }
}
