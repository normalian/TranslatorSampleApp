using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TranslatorSampleApp
{
    class Program
    {
        static readonly string route = "/translator/text/batch/v1.0/batches";
        private static readonly string endpoint = "your cognitive service endpoint, ex. - https://xxxxxxxxxx.cognitiveservices.azure.com";
        private static readonly string subscriptionKey = "your subscription key, ex. - bc99245c29154aa38b30fxxxxxxxxxx";
        private static readonly string sourceUrl = "your Azure Storage SAS, ex. - https://xxxxxxxxxxxxxxxstorage.blob.core.windows.net/source?sp=rl&st=2021-08-13T17:15:57Z&se=2021-08-14T01:15:57Z&spr=https&sv=2020-08-04&sr=c&sig=PAoWGevARKZa35Is8bbNmIpcOamMoEha5Qf%2B7atKEts%3D";
        private static readonly string targetUrl = "your Azure Storage SAS, ex. - https://xxxxxxxxxxxxxxxstorage.blob.core.windows.net/target?sp=rcwl&st=2021-08-13T17:16:18Z&se=2021-08-14T01:16:18Z&spr=https&sv=2020-08-04&sr=c&sig=tis86CawTaxTGU4r7WMTFwbH3hzg0rpkwEk3C1qg3sc%3D";

        static readonly HttpClient client = new HttpClient();

        static async Task Main(string[] args)
        {
            var json = new TranslationInput()
            {
                inputs = new Input[1]
            };
            json.inputs[0] = new Input()
            {
                source = new Source()
                {
                    language = "ja",
                    sourceUrl = sourceUrl,
                    storageSource = "AzureBlob"
                },
                targets = new Target[1]
            };
            json.inputs[0].targets[0] = new Target()
            {
                category = "general",
                language = "en",
                storageSource = "AzureBlob",
                targetUrl = targetUrl
            };

            Console.WriteLine("## initiate translation job");
            var operationLocationId = await StartTranslationJobAsync(json);
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("");

            if (string.IsNullOrEmpty(operationLocationId) == false)
            {
                TranslationJobStatus job;
                do
                {
                    Console.WriteLine("## check status of translation job");
                    job = await GetTranslationJobStatusAsync(operationLocationId);
                    Console.WriteLine($"job status = {job.status}");
                    Console.WriteLine($"job summary: success = {job.summary.success}, failed = {job.summary.failed}");

                    // to avoid frequent requests
                    Thread.Sleep(TimeSpan.FromSeconds(5));
                    Console.WriteLine("");
                    Console.WriteLine("");
                    Console.WriteLine("");

                } while (job.status != "Succeeded" && job.status != "Failed" && job.status != "ValidationFailed");
            }
            Console.ReadLine();

        }

        static async Task<TranslationJobStatus> GetTranslationJobStatusAsync(string operationLocationId)
        {
            using HttpRequestMessage request = new HttpRequestMessage();
            {
                request.Method = HttpMethod.Get;
                request.RequestUri = new Uri(endpoint + route + "/" + operationLocationId);
                request.Headers.Add("Ocp-Apim-Subscription-Key", subscriptionKey);


                HttpResponseMessage response = await client.SendAsync(request);
                string result = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"Status code: {response.StatusCode}");
                Console.WriteLine($"Response Headers: {response.Headers}");
                Console.WriteLine();
                Console.WriteLine(result);
                return JsonConvert.DeserializeObject<TranslationJobStatus>(result);
            }
        }

        static async Task<string> StartTranslationJobAsync(TranslationInput input)
        {
            using HttpRequestMessage request = new HttpRequestMessage();
            {

                StringContent content = new StringContent(JsonConvert.SerializeObject(input), Encoding.UTF8, "application/json");

                request.Method = HttpMethod.Post;
                request.RequestUri = new Uri(endpoint + route);
                request.Headers.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
                request.Content = content;

                HttpResponseMessage response = await client.SendAsync(request);

                Console.WriteLine($"Status code: {response.StatusCode}");
                Console.WriteLine();
                Console.WriteLine($"Response Headers:");
                Console.WriteLine(response.Headers);
                Console.WriteLine(await response.Content.ReadAsStringAsync());

                if (response.IsSuccessStatusCode)
                {
                    var operationLocation = response.Headers.GetValues("Operation-Location").FirstOrDefault();
                    Console.WriteLine($"Operation Location = {operationLocation}");

                    var operationLocationId = operationLocation.Split('/').LastOrDefault();
                    Console.WriteLine($"Operation Location id = {operationLocationId}");

                    return operationLocationId;
                }
                else
                    Console.WriteLine("@@@ you need to confirm error details");
            }

            return null;
        }
    }


    public class TranslationInput
    {
        public Input[] inputs { get; set; }
    }

    public class Input
    {
        public Source source { get; set; }
        public Target[] targets { get; set; }
    }

    public class Source
    {
        public string sourceUrl { get; set; }
        public string storageSource { get; set; }
        public string language { get; set; }
    }

    public class Target
    {
        public string targetUrl { get; set; }
        public string storageSource { get; set; }
        public string category { get; set; }
        public string language { get; set; }
    }


    public class TranslationJobStatus
    {
        public string id { get; set; }
        public DateTime createdDateTimeUtc { get; set; }
        public DateTime lastActionDateTimeUtc { get; set; }
        public string status { get; set; }
        public Summary summary { get; set; }
    }

    public class Summary
    {
        public int total { get; set; }
        public int failed { get; set; }
        public int success { get; set; }
        public int inProgress { get; set; }
        public int notYetStarted { get; set; }
        public int cancelled { get; set; }
        public int totalCharacterCharged { get; set; }
    }
}