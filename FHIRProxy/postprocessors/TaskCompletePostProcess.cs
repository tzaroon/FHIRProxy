using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace FHIRProxy.postprocessors
{
     class TaskCompletePostProcess : IProxyPostProcess
    {

        public async Task<ProxyProcessResult> Process(FHIRResponse response, HttpRequest req, ILogger log, ClaimsPrincipal principal)
        {
            // fetches task ID from Params
            var taskId = req.Query["closeTaskId"];
            if (!String.IsNullOrEmpty(taskId))
            {
                req.Method = "PATCH";
               // req.Path = "Task/"+taskId;

                string json = JsonSerializer.Serialize(new
                {
                    path = "/status",
                    value = "completed"
                });

                var jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
                var operationStrings = "[ { \"op\": \"replace\", \"path\": \"/status\", \"value\": \"completed\" } ] ";
                var ops = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Microsoft.AspNetCore.JsonPatch.Operations.Operation>>(operationStrings);

                var patchDocument = new Microsoft.AspNetCore.JsonPatch.JsonPatchDocument(ops, new Newtonsoft.Json.Serialization.DefaultContractResolver());

               // patchDocument.ApplyTo(jsonObj);
                var result =   await FHIRClient.CallFHIRServer("Task/" + taskId, operationStrings, req.Method, log, "application/json-patch+json");
            }


            ProxyProcessResult rslt = new ProxyProcessResult();
            /* Use the passed response to modify/filter then return modified results in Response member of ProxyProcessResult Object 
               Remember to return an error or exception OperationOutcome Response and to prevent further processing set Continue member to False and Response to a valida OperationOutcome resource*/
            rslt.Response = response;
            return rslt;
        }
    }
}