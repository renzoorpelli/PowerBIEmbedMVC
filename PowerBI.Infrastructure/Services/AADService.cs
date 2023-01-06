using System;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using PowerBIReports.Domain.Entities;

namespace PowerBI.Infrastructure.Services
{
    public interface IAADService
    {
        /// <summary>
        /// metodo encargado de obtener el token de acceso de PBI, utilizando el metodo de autenticacion "Sevice Principal"
        /// </summary>
        /// <returns>el token de acceso</returns>
        Task<string> GetAcessToken();
    }
    public class AADService : IAADService
    {
        private readonly IOptions<AzureAD> options;

        public AADService(IOptions<AzureAD> options)
        {
            this.options = options;
        }

        public async Task<string> GetAcessToken()
        {
            if(options.Value.AuthenticationMode.Equals("serviceprincipal", StringComparison.InvariantCultureIgnoreCase))
            {
                //reemplazo la palabra clave por el id de nuestro tenant para generar la url de auth
               var tenatSpecificUrl = options.Value.AuthorityUrl.Replace("organizations", options.Value.TenantId);

                //creo un cliente confidencial para autorizar la aplicacion con azure active directory
                IConfidentialClientApplication clientApplication = ConfidentialClientApplicationBuilder
                    .Create(options.Value.ClientId)
                    .WithClientSecret(options.Value.ClientSecret)
                    .WithAuthority(tenatSpecificUrl)
                    .Build();
                AuthenticationResult authenticationResult = await clientApplication.AcquireTokenForClient(options.Value.ScopeBase).ExecuteAsync();
                return authenticationResult.AccessToken;
            }
            return String.Empty;
        }
    }
}
