﻿using nmct.ba.cashlessproject.api.DataAccess;
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
    public class KassaController : ApiController
    {
        [Authorize]
        public List<Kassa> Get()
        {
            ClaimsPrincipal cp = RequestContext.Principal as ClaimsPrincipal;
            return KassaDA.GetKassas(cp.Claims);
        }
    }
}
