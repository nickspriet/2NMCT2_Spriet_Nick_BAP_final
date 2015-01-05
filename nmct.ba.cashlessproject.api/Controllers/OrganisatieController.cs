using nmct.ba.cashlessproject.api.DataAccess;
using nmct.ba.cashlessproject.model.IT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;

namespace nmct.ba.cashlessproject.api.Controllers
{
    public class OrganisatieController : ApiController
    {
        public HttpResponseMessage Put(Organisatie o)
        {
            OrganisatieDA.UpdateOrganisatie(o);
            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
}
