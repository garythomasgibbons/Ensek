using RestSharp;

namespace EnSekTestGaryGibbons
{
    public class ApiClient
    {
        public async Task<T> Get<T>(string url, string token)
        {
            var client = new RestClient(url);
            var request = new RestRequest(Method.GET);

            AddAuthHeader(request, token);

            request.OnBeforeDeserialization = resp => { resp.ContentType = "application/json"; };

            var restResponse = await client.ExecuteTaskAsync<T>(request);            

            return restResponse.Data;
        }

        public async Task<T> Put<T>(string url, string token)
        {
            var client = new RestClient(url);
            var request = new RestRequest(Method.PUT);

            AddAuthHeader(request);

            var restResponse = await client.ExecuteTaskAsync<T>(request);

            return restResponse.Data;
        }

        public async Task<T> Post<T>(string url, string token)
        {
            var client = new RestClient(url);
            var request = new RestRequest(Method.POST);

            AddAuthHeader(request, token);

            var restResponse = await client.ExecuteTaskAsync<T>(request);

            return restResponse.Data;
        }

        public async Task<T> Post<T>(string url, object requestBody)
        {
            var client = new RestClient(url);
            var request = new RestRequest(Method.POST);

            AddAuthHeader(request);

            request.RequestFormat = DataFormat.Json;
            request.AddBody(requestBody);

            var restResponse = await client.ExecuteTaskAsync<T>(request);

            return restResponse.Data;
        }

        private void AddAuthHeader(RestRequest request, string token="")
        {
            if (token == "")
            {
                request.AddHeader("Authorization", "Bearer garygibbons");
            }
            else
            {
                request.AddHeader("Authorization", "Bearer " + token);
            }
        }
    }

    
}
