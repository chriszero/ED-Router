using RestSharp;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace libspanch
{
    public class SpanchApi
    {
        private const string baseUri = "https://spansh.co.uk/api/";

        /// <summary>
        /// If response is null, the request may be queued by the server, repeat request 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request"></param>
        /// <returns></returns>
        private T Execute<T>(RestRequest request) where T : new()
        {
            var client = new RestClient();
            client.UserAgent = "libspanch";
            client.BaseUrl = new Uri(baseUri);
            var response = client.Execute<T>(request);

            if (response.ErrorException != null)
            {
                const string message = "Error retrieving response.  Check inner details for more info.";
                throw new ApplicationException(message, response.ErrorException);
            }

            return response.Data;
        }

        /// <summary>
        /// If response is null, the request may be queued by the server, repeat request 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request"></param>
        /// <returns></returns>
        private async Task<T> ExecuteAsync<T>(RestRequest request) where T : new()
        {
            var client = new RestClient
            {
                UserAgent = "libspanch",
                BaseUrl = new Uri(baseUri)
            };

            var responseTask = new TaskCompletionSource<IRestResponse<T>>();
            client.ExecuteAsync<T>(request, responseTask.SetResult);

            var response = await responseTask.Task;

            if (response.ErrorException != null)
            {
                const string message = "Error retrieving response.  Check inner details for more info.";
                throw new ApplicationException(message, response.ErrorException);
            }

            return response.Data;
        }

        public NeutronPlotterRoute PlotRoute(string startSystem, string destinationSystem, double jumpRange, int efficiency = 60)
        {
            var request = new RestRequest("route");
            request.AddParameter("efficiency", efficiency)
                .AddParameter("range", jumpRange)
                .AddParameter("from", startSystem)
                .AddParameter("to", destinationSystem);

            var response = Execute<RequestResult>(request);

            var routeRequest = new RestRequest("results/{job}");
            routeRequest.AddParameter("job", response.Job, ParameterType.UrlSegment);

            var routeResponse = Execute<RequestResult<NeutronPlotterRoute>>(routeRequest);
			if(routeResponse.Error != null)
			{
				throw new RouteException(routeResponse.Error);
			}

            while (routeResponse.Status.ToLower() == "queued")
            {
                routeResponse = Execute<RequestResult<NeutronPlotterRoute>>(routeRequest);
                // wait few ms? limit via time?
                Thread.Sleep(1000);
            }

            return routeResponse.Result;
        }

        public async Task<NeutronPlotterRoute> PlotRouteAsync(string startSystem, string destinationSystem, double jumpRange, int efficiency = 60)
        {
            var request = new RestRequest("route");
            request.AddParameter("efficiency", efficiency)
                .AddParameter("range", jumpRange)
                .AddParameter("from", startSystem)
                .AddParameter("to", destinationSystem);

            var response = await ExecuteAsync<RequestResult>(request);

            var routeRequest = new RestRequest("results/{job}");
            routeRequest.AddParameter("job", response.Job, ParameterType.UrlSegment);

            var routeResponse = await ExecuteAsync<RequestResult<NeutronPlotterRoute>>(routeRequest);
            if (routeResponse.Error != null)
            {
                throw new RouteException(routeResponse.Error);
            }

            while (routeResponse.Status.ToLower() == "queued")
            {
                routeResponse = await ExecuteAsync<RequestResult<NeutronPlotterRoute>>(routeRequest);
                // wait few ms? limit via time?
                await Task.Delay(1000);
            }

            routeResponse.Result.Id = response.Job;
            return routeResponse.Result;
        }

        /// <summary>
        /// Returns a list of systems that maches "system"
        /// </summary>
        /// <param name="system"></param>
        /// <returns></returns>
        public List<string> GetSystems(string system)
        {
            var request = new RestRequest("systems");
            request.AddParameter("q", system);

            var response = Execute<List<string>>(request);
            return response;
        }

        public List<ScanSystem> Route2Riches(string from, string to, int radius, int range, int maxResults, int maxDistance = 1000000, int minValue = 300000)
        {
            var request = new RestRequest("riches/route");
            request.AddParameter("radius", radius)
                .AddParameter("range", range)
                .AddParameter("from", from)
                .AddParameter("to", to)
                .AddParameter("max_results", maxResults)
                .AddParameter("max_distance", maxDistance)
                .AddParameter("min_value", minValue);

            var response = Execute<RequestResult>(request);

            var routeRequest = new RestRequest("results/{job}");
            routeRequest.AddParameter("job", response.Job, ParameterType.UrlSegment);

            var routeResponse = Execute<RequestResult<List<ScanSystem>>>(routeRequest);
            while (routeResponse.Status.ToLower() == "queued")
            {
                routeResponse = Execute<RequestResult<List<ScanSystem>>>(routeRequest);
                // wait few ms? limit via time?
                Thread.Sleep(1000);
            }

            return routeResponse.Result;
        }

    }

    internal class RequestResult
    {
        public string Status { get; set; }
        public string Job { get; set; }
        public string Error { get; set; }
    }

    internal class RequestResult<T> : RequestResult
    {
        public T Result { get; set; }
    }
}
