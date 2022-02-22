
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace FHIRProxy.preprocessors
{
      //   1. Write a preprocessor that will:
	 //    2. Check if request has JWTthan get jwt token decrypt it, validate that and get user Id from it and call(https://hapdataplatformdev-apis.fhir.azurehealthcareapis.com/Patient?identifier=http://fhir.medlix.org/firebase-uid%7CYnDU8JzOgOTFd5lDlXM5FapAntV2) to get patient record with patientId,
    //     3. replace { { patientId} } with actual patientId and remove patientId, jwt from url
   //      4. If There is no JWT token in URL get patient Id  from param(Example: patientId= abcd - 1234)
  //       5. Perform step 3.
    class PatientRecordPreProcess : IProxyPreProcess
    {
        public async Task<ProxyProcessResult> Process(string requestBody, HttpRequest req, ILogger log, ClaimsPrincipal principal, string restOfPath)
        {
            var JWT = req.Query["jwt"];
            var patientId = req.Query["patientId"];
            var url = "";
            var baseUrl = Utils.GetEnvironmentVariable("FS-URL", "");
            if (!string.IsNullOrEmpty(JWT) || !string.IsNullOrEmpty(patientId))
            {
                if (!String.IsNullOrEmpty(JWT))
                {
                    log.LogInformation($"incoimg JWT Token: {JWT}");
                    restOfPath = RemoveQueryStringByKey(restOfPath, "jwt");
                }
                //if (!String.IsNullOrEmpty(JWT) )
                //{
                //    log.LogInformation($"incoimg JWT Token: {JWT}");
                //    var handler = new JwtSecurityTokenHandler();
                //    var decodedValue = handler.ReadJwtToken(JWT);
                //    if(decodedValue.ValidTo > DateTime.Now)
                //    {
                //        throw new HttpResponseException(HttpStatusCode.Unauthorized);
                //    }
                //    object userId ;
                //     decodedValue.Payload.TryGetValue("uid", out userId);
                //    //Check if request has JWT than get jwt token decrypt it, validate that and get user Id from it and call(https://hapdataplatformdev-apis.fhir.azurehealthcareapis.com/Patient?identifier=http://fhir.medlix.org/firebase-uid%7CYnDU8JzOgOTFd5lDlXM5FapAntV2) to get patient record with patientId,
                //    // decrypt JWT and check validation
                //    // get patient record
                //}
                //else 
                if (!string.IsNullOrEmpty(patientId))
                {
                    restOfPath = System.Web.HttpUtility.UrlDecode(restOfPath);
                    restOfPath = restOfPath.Replace("{{patientId}}", patientId);
                   url  = RemoveQueryStringByKey(restOfPath, "patientId");
                   

                }
                url = baseUrl + "/" + url;
                return new ProxyProcessResult(true, "", requestBody, null, url);
            }

            return new ProxyProcessResult(true,"",requestBody,null);
        }

        private  string RemoveQueryStringByKey(string url, string key)
        {


            // this gets all the query string key value pairs as a collection
            var newQueryString = HttpUtility.ParseQueryString(url);

            // this removes the key if exists
            newQueryString.Remove(key);

            return newQueryString.ToString();
        }
    }
}
