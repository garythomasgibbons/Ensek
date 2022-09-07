using RestSharp;

namespace EnSekTestGaryGibbons
{
    public class ApiClient
    {
        public async Task<T> Get<T>(string url)
        {
            var client = new RestClient(url);
            var request = new RestRequest(Method.GET);

            AddAuthHeader(request);

            request.OnBeforeDeserialization = resp => { resp.ContentType = "application/json"; };

            var restResponse = await client.ExecuteTaskAsync<T>(request);
            

            return restResponse.Data;
        }

        public async Task<T> Put<T>(string url)
        {
            var client = new RestClient(url);
            var request = new RestRequest(Method.PUT);

            AddAuthHeader(request);

            var restResponse = await client.ExecuteTaskAsync<T>(request);
            //ThrowOnFailure(url, restResponse);

            return restResponse.Data;
        }

        public async Task<T> Post<T>(string url)
        {
            var client = new RestClient(url);
            var request = new RestRequest(Method.POST);

            AddAuthHeader(request);

            var restResponse = await client.ExecuteTaskAsync<T>(request);
            //ThrowOnFailure(url, restResponse);

            return restResponse.Data;
        }

        private void ThrowOnFailure<T>(string url, IRestResponse<T> restResponse)
        {
            if (!restResponse.IsSuccessful)
            {
                throw restResponse.ErrorException ?? new ArgumentException($"{url}\r\n REST call failed. Status: {restResponse.StatusCode}. Response: {restResponse.Content}");
            }
        }

        private void AddAuthHeader(RestRequest request)
        {
            request.AddHeader("Authorization", "bearer garygibbons");
        }
    }

    
}
