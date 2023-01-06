using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Text.Json;
using PowerBI.Infrastructure.Services;
using PowerBIReports.Domain.Entities;

namespace PowerBIReports.Host.Controllers
{
    public class EmbedInfoController : Controller
    {
        private readonly PBIEmbedService pBIEmbedService;
        private readonly IOptions<AzureAD> azureAd;
        private readonly IOptions<PowerBIModel> powerBI;

        public EmbedInfoController(PBIEmbedService pBIEmbedService, IOptions<AzureAD> azureAd, IOptions<PowerBIModel> powerBI)
        {
            this.pBIEmbedService = pBIEmbedService;
            this.azureAd = azureAd;
            this.powerBI = powerBI;
        }

        /// <summary>
        /// metodo encargado de retornar el embed token, la url y el token de expiracion al cliente en formato JSON
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<string> GetEmbedInfo()
        {
            try
            {
                EmbedParams embedParams = await pBIEmbedService.GetEmbedParam(new Guid(powerBI.Value.WorkspaceId), new Guid(powerBI.Value.ReportId));

                return JsonSerializer.Serialize<EmbedParams>(embedParams);
            }
            catch (Exception ex)
            {
                HttpContext.Response.StatusCode = 500;
                return ex.Message + "\n\n" + ex.StackTrace;
            }
        }
    }
}
