using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Linq;
using System.Net;

namespace TourFunction
{
    public static class TourAvailabilityView
    {
        [FunctionName("TourAvailabilityView")]
        public static HttpResponseMessage Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            [CosmosDB(
                databaseName: "%Database%",
                collectionName: "%Container%",
                ConnectionStringSetting = "AzureconnectionString",
                SqlQuery ="SELECT c.tourDate,c.hooked,c.officeId,c.tourWaveList FROM c")] IEnumerable<object> documents,
            ILogger log)
        {
            HttpResponseMessage resp=new HttpResponseMessage();
            try
            {
                log.LogInformation("Starting View");
                if (documents.ToList().Count == 0)
                {
                    resp.StatusCode = HttpStatusCode.NotFound;
                }
                else
                {
                    resp.StatusCode = HttpStatusCode.OK;
                    resp.Content= new StringContent(JsonConvert.SerializeObject(documents), UnicodeEncoding.UTF8, "application/json");

                }
            }
            catch(Exception ex)
            {
                log.LogError(ex.Message +" "+ex.InnerException);
                resp.StatusCode = HttpStatusCode.InternalServerError;
                resp.Content = new StringContent(ex.Message, UnicodeEncoding.UTF8, "application/text");
            }
            return resp;

        }
    }
}
