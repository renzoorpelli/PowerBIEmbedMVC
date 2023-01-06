using Microsoft.PowerBI.Api;
using Microsoft.PowerBI.Api.Models;
using Microsoft.Rest;
using PowerBIReports.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PowerBI.Infrastructure.Services
{
    public interface IPBIEmbedService
    {
        /// <summary>
        /// metodo encargado de generar el cliente PowerBI
        /// </summary>
        /// <returns></returns>
        Task<PowerBIClient> GeneratePowerBiClient();

        /// <summary>
        /// metodo encargado de obtener los parametros para insertar al reporte
        /// </summary>
        /// <param name="workspaceId">el id del workspace donde se encuentra el reporte</param>
        /// <param name="reportId">el id del reporte</param>
        /// <param name="additionalDatasetId">id adicionales de datasets para agergar al reporte</param>
        /// <returns></returns>
        Task<EmbedParams> GetEmbedParam(Guid workspaceId, Guid reportId, [Optional] Guid additionalDatasetId);
        /// <summary>
        /// metodo encargado de obtener el embed token para un SOLO reporte, esta funcion no funciona con reportes RDL
        /// </summary>
        /// <param name="reportId">el id del reporte</param>
        /// <param name="datasetIds">los multiples datasets</param>
        /// <param name="targetWorkspaceId"></param>
        /// <returns>retorna el token</returns>
        Task<EmbedToken> GetEmbedToken(Guid reportId, IList<Guid> datasetIds, [Optional] Guid targetWorkspaceId);
        
       /// <summary>
       /// metodo encargado de obtener el token para un reporte de tipo RDL
       /// </summary>
       /// <param name="targetWorkspaceId">el id del target workspace</param>
       /// <param name="reportId">el id del reporte RDL</param>
       /// <param name="accessLevel">el acceso otrogado</param>
       /// <returns>el embed token</returns>
        Task<EmbedToken> GetEmbedTokenForRDLReport(Guid targetWorkspaceId, Guid reportId, string accessLevel = "view");
    }
    public class PBIEmbedService : IPBIEmbedService
    {
        private readonly AADService aAdService;
        private static PowerBIClient powerBIClient;
        public PBIEmbedService(AADService aAdService)
        {
            this.aAdService = aAdService;
        }

        public async Task<PowerBIClient> GeneratePowerBiClient()
        {
            if(powerBIClient is null) 
            {
                string token = await aAdService.GetAcessToken();
                if (token != String.Empty)
                {
                    var tokenCredentials = new TokenCredentials(token, "Bearer");
                    powerBIClient = new PowerBIClient(new Uri("https://api.powerbi.com"), tokenCredentials);
                }
            }
            return powerBIClient;
        }

        public async Task<EmbedParams> GetEmbedParam(Guid workspaceId, Guid reportId, [Optional] Guid additionalDatasetId)
        {
            PowerBIClient pbiClient = await this.GeneratePowerBiClient();

            //obtengo inforrmacion del reporte pasandole el workspace y el id del reporte
            var pbiReport = pbiClient.Reports.GetReportInGroup(workspaceId, reportId);

            //vergicia que el dataset no sea nulo, en caso de no serlo no es un reporte RDL
            var isRDLReport = String.IsNullOrEmpty(pbiReport.DatasetId);
            EmbedToken embedToken;

            //genero el token para el reporte RDL
            if (isRDLReport)
            {
                //Obtiene el token para el reporte RDL
                embedToken = await GetEmbedTokenForRDLReport(workspaceId, reportId);
            }
            else
            {
                var datasetIds = new List<Guid>();

                // anado los dataset
                datasetIds.Add(Guid.Parse(pbiReport.DatasetId));
                
                // confirmo si el paraemtro opcional no es nulo
                if (additionalDatasetId != Guid.Empty)
                {
                    datasetIds.Add(additionalDatasetId);
                }
                embedToken = await GetEmbedToken(reportId, datasetIds, workspaceId);
            }
            // agrego la data de los reportos para insertar
            var embedReports = new List<EmbedReport>()
            {
                new EmbedReport()
                {
                    ReportId = pbiReport.Id, ReportName= pbiReport.Name, EmbedUrl = pbiReport.EmbedUrl
                }
            };

            var embedParams = new EmbedParams
            {
                EmbedReport = embedReports,
                Type = "Report",
                EmbedToken = embedToken
            };

            return embedParams;

        }

        public async Task<EmbedToken> GetEmbedToken(Guid reportId, IList<Guid> datasetIds, [Optional]Guid targetWorkspaceId)
        {
            PowerBIClient pbiClient = await this.GeneratePowerBiClient();

            // creo una peticion para obtener el embed token
            var tokenRequest = new GenerateTokenRequestV2(
                reports: new List<GenerateTokenRequestV2Report>()
                {
                    new GenerateTokenRequestV2Report(reportId)
                },
                datasets: datasetIds.Select(d => new GenerateTokenRequestV2Dataset(d.ToString())).ToList(),
                targetWorkspaces: targetWorkspaceId != Guid.Empty ? new List<GenerateTokenRequestV2TargetWorkspace>()
                {
                    new GenerateTokenRequestV2TargetWorkspace(targetWorkspaceId) } : null
                );
            var embedToken = pbiClient.EmbedToken.GenerateToken(tokenRequest);
            return embedToken;

        }
       
        public async Task<EmbedToken> GetEmbedTokenForRDLReport(Guid targetWorkspaceId, Guid reportId, string accessLevel = "view")
        {
            PowerBIClient pbiClient = await this.GeneratePowerBiClient();

            // genero la peticion para un reporte de tipo RDL
            var generateTokenRequestParameters = new GenerateTokenRequest(
                accessLevel: accessLevel
            );

            // genero el token
            var embedToken = pbiClient.Reports.GenerateTokenInGroup(targetWorkspaceId, reportId, generateTokenRequestParameters);

            return embedToken;
        }

    }
}
