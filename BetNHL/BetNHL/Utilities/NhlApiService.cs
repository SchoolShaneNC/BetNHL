using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace BetNHL.Utilities
{
    //Jeeves class: the app’s helpful assistant.
    public static class NhlApiService
    {
        //Stores the base URL of your local Web API
        //here is the live url as well, depending on if you want to run it locally or through the internet    https://api-web.nhle.com/
        // public static Uri DBUri = new Uri("http://localhost:5172/");   https://hackathonapishanemilton2026.azurewebsites.net/
        // public static Uri DBUri = new Uri("http://localhost:5159");
        public static Uri DBUri = new Uri("https://localhost:7281/");
        //for samsung emulator
       // public static Uri DBUri = new Uri("https://10.0.2.2:7281/");


        public static ApiException CreateApiException(HttpResponseMessage response)
        {
            var httpErrorObject = response.Content.ReadAsStringAsync().Result;

            // Create an anonymous object to use as the template for deserialization:
            var anonymousErrorObject =
                new { message = "", errors = new Dictionary<string, string[]>() };

            // Deserialize:
            var deserializedErrorObject =
                JsonConvert.DeserializeAnonymousType(httpErrorObject, anonymousErrorObject);

            // Now wrap into an exception which best fullfills the needs of your application:
            var ex = new ApiException(response);

            //Check for a message
            if (deserializedErrorObject?.message != null)
            {
                ex.Data.Add(-1, deserializedErrorObject?.message);
            }
            if (deserializedErrorObject?.errors != null)
            {
                foreach (var err in deserializedErrorObject.errors)
                {
                    var message = (err.Value != null && err.Value.Length > 0)
                        ? err.Value[0]
                        : "Unknown error";

                    ex.Data.Add(err.Key, message);
                }
            }
            return ex;
        }
    }
}

