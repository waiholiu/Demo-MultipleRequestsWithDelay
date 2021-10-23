using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace demoMultipleCalls
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var client = new RestClient("https://jsonplaceholder.typicode.com/comments");

            List<RestRequest> lstRestRequests = new List<RestRequest>();

            // this bit crafts the queries I want to call
            for (int i =0 ; i < 25; i++)
            {
                
                var request = new RestRequest(Method.GET);
                request.AddQueryParameter("postId", i.ToString());

                lstRestRequests.Add(request);
            }

            // this list stores all my results
            List<IRestResponse> finishedTasks = new List<IRestResponse>();

            // split the list of queries into batches of 10 each
            var batchCalls = splitList(lstRestRequests, 10);
            foreach (var batch in batchCalls)
            {
                //call each batch of 30 at the same time
                finishedTasks.AddRange(await Task.WhenAll(batch.Select(r => ExecuteRequest(r, client))));

                //after executing the current batch, wait 5 seconds
                await Task.Delay(5000);
                Console.WriteLine("taken time off, ready to go again");
            }

            // use the results for something.
            foreach (var task in finishedTasks)
            {
                Console.WriteLine(task.Content.ToString());
            }

        }


        // this method fires off the request
        private static Task<IRestResponse> ExecuteRequest(RestRequest r, RestClient client)
        {
            Console.WriteLine("Calling API " + r.Parameters[0]);
            return client.ExecuteAsync(r);
        }


        // Splits a list into multiple smaller lists of specified batch size
        private static IEnumerable<List<T>> splitList<T>(List<T> locations, int size = 100)
        {
            for (int i = 0; i < locations.Count; i += size)
            {
                yield return locations.GetRange(i, Math.Min(size, locations.Count - i));
            }
        }


    }
}
