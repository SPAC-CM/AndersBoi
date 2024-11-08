using System.Net.Http.Headers;
using System.Net.Mime;

namespace FileProject
{
    public class HTTP_Manager
    {
        private readonly HttpClient client = new();
        private string downloadsPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                "Downloads");

        Random r = new Random(22);

        public async Task<(int, bool)> DownloadFileAsync(URL_Data data, int collextionIndex)
        {
            var errorReturn = (collextionIndex, false);
            try
            {

                // Get the Downloads folder path
                string downloadsPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                    "Downloads");

                // Generate a unique filename
                string uniqueFileName = Path.Combine(downloadsPath, $"temp_{Guid.NewGuid()}.pdf");

                // Download the file
                using (var client = new HttpClient())
                {
                    // Set the user agent string
                    client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/124.0.6367.61 Safari/537.36");

                    // Request URL
                    var response = await client.GetAsync(data.URL);
                    if (response.IsSuccessStatusCode)
                    {
                        //Check for content
                        if (response.Content.Headers.ContentLength.Value > 0 &&
                            response.Content.Headers.ContentType.MediaType == "application/pdf")
                        {
                            //Download Content
                            using (var fileStream = new FileStream(uniqueFileName, FileMode.Create))
                            {
                                await response.Content.CopyToAsync(fileStream);
                            }

                            // Rename the file
                            File.Move(uniqueFileName, Path.Combine(downloadsPath, data.BR_Nummer + ".pdf"));

                            //Tell download status
                            return (collextionIndex, true);
                        }
                        //else
                        //{
                        //    //Debug HTTP request
                        //    Console.WriteLine($"{data.BR_Nummer}, Lenght: {response.Content.Headers.ContentLength.Value}. Type: {response.Content.Headers.ContentType.MediaType}");
                        //}
                    }
                    else
                    {
                        //Console.WriteLine($"{data.BR_Nummer}: {response}"); Tell respond
                        return errorReturn;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"{data.BR_Nummer}: {e.Message}");
            }

            return errorReturn;
        }

        public async Task<string> GetAsync(string uri)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync("https://example.com");
                    response.EnsureSuccessStatusCode(); // Throws if not successful

                    int statusCode = (int)response.StatusCode;
                    return $"Status Code: {(int)statusCode}";
                }
            }
            catch (HttpRequestException e)
            {
                return ($"An error occurred: {e.Message}");
            }
        }

#if (DEBUG)
        public async Task<(int, bool)> ProxyDownload(URL_Data targetData, int dataIndex)
        {
            bool foundDownload = true;
            if (Random.Shared.Next(100) > 75)
                foundDownload = false;

            await Task.Delay(50 + Random.Shared.Next(100));

            return (dataIndex, foundDownload);
        }
#endif


    }
}
