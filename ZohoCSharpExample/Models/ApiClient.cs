using RestSharp;
namespace ZohoCSharpExample.Models
{
    public class ApiClient
    {
        private readonly RestClient _client;

        public ApiClient(string baseUrl)
        {
            _client = new RestClient(baseUrl);
        }

        public async Task<RestResponse> GetResourceAsync(string resource, string accessToken)
        {
            var request = new RestRequest(resource, Method.Get);

            if (accessToken != null)
            {
                request.AddHeader("Authorization","Zoho-oauthtoken " + accessToken);
                request.AddHeader("Content-Type","application/json");
            }

            var response = await _client.ExecuteAsync(request);
            return response;
        }

        public async Task<RestResponse> PostResourceAsync(string resource, object requestBody)
        {
            var request = new RestRequest(resource, Method.Post);
            // Add request body if any
            if (requestBody != null)
            {
                request.AddJsonBody(requestBody);
            }

            var response = await _client.ExecuteAsync(request);
            return response;
        }
    }
}