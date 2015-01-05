using Microsoft.Owin.Security.OAuth;
using nmct.ba.cashlessproject.api.DataAccess;
using nmct.ba.cashlessproject.model.IT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace nmct.ba.cashlessproject.api.Providers
{
    public class SimpleAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
        }

        public override Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            Organisatie o = OrganisatieDA.CheckCredentials(context.UserName, context.Password);
            if (o == null)
            {
                context.Rejected();
                return Task.FromResult(0);
            }

            var id = new ClaimsIdentity(context.Options.AuthenticationType);
            id.AddClaim(new Claim("dbnaam", o.DbNaam));
            id.AddClaim(new Claim("dblogin", o.DbGebruikersNaam));
            id.AddClaim(new Claim("dbpass", o.DbWachtwoord));

            context.Validated(id);
            return Task.FromResult(0);
        }
    }
}