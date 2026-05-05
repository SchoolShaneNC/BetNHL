using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BetNHL.Utilities
{
    //attaches the JWT token to outgoing requests and handles 401 responses by clearing the token from secure storage.
    public class AuthMessageHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            try
            {

                var token = await SecureStorage.GetAsync("auth_token");

                if (!string.IsNullOrEmpty(token))
                {
                    request.Headers.Authorization =
                        new AuthenticationHeaderValue("Bearer", token);
                }

                var response = await base.SendAsync(request, cancellationToken);

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    SecureStorage.Remove("auth_token");
                }

                return response;



            }
            catch(Exception ex) 
            {
                // Handle exceptions as needed
                throw new HttpRequestException("An error occurred while trying to login.", ex);
            }
    
        }
    }
}
