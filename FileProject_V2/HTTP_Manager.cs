using System.Net.Http.Headers;
using System.Net.Mime;

namespace FileProject
{
    public class HTTP_Manager
    {
        /// <summary>
        /// Manage the HTTP request and download
        /// </summary>

        //To test
        Random r = new Random(22);

        public async Task<(int, bool)> DownloadFileAsync(URL_Data data, int collextionIndex, string downloadsPath)
        {
            var errorReturn = (collextionIndex, false);
            try
            {

		//Ensures that the downloads path exist
		System.IO.Directory.CreateDirectory(downloadsPath);

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
                    }
                    else
                    {
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


#if (DEBUG)
        /// <summary>
        /// Used as proxy to test download.
        /// </summary>
        /// <param name="targetData"></param>
        /// <param name="dataIndex"></param>
        /// <returns></returns>
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
