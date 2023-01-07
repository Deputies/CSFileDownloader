using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace FileDownloader
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Check if the file containing the list of URLs exists
            string fileName = "urls.txt";
            if (!File.Exists(fileName))
            {
                // If the file doesn't exist, create it and add some example URLs
                string[] exampleUrls =
                {
                    "https://example.com/image.jpg",
                    "https://example.com/video.mp4",
                    "https://another-example.com/file.png"
                };
                File.WriteAllLines(fileName, exampleUrls);
            }

            // Read the file containing the list of URLs
            string[] lines = File.ReadAllLines(fileName);

            // Determine the number of threads to use based on the processor count
            int processorCount = Environment.ProcessorCount;
            int maxThreads = processorCount * 2;

            // Create an HttpClient for downloading the files
            using (HttpClient client = new HttpClient())
            {
                // Use a SemaphoreSlim to limit the number of concurrent downloads
                using (SemaphoreSlim semaphore = new SemaphoreSlim(maxThreads))
                {
                    // Create a list of tasks for downloading the files
                    List<Task> tasks = new List<Task>();
                    foreach (string line in lines)
                    {
                        // Wait for a slot to be available in the semaphore
                        await semaphore.WaitAsync();

                        // Download the file asynchronously
                        tasks.Add(Task.Run(async () =>
                        {
                        try
                        {
                            // Extract the file name and domain from the URL
                            Uri uri = new Uri(line);
                            string file = Path.GetFileName(uri.LocalPath);
                            string domain = uri.Host;

                            // Create a directory for the domain, if it doesn't already exist
                            string directory = Path.Combine(Environment.CurrentDirectory, domain);
                            if (!Directory.Exists(directory))
                            {
                                Directory.CreateDirectory(directory);
                            }

                            // Download the file and save it to the domain's directory
                            string path = Path.Combine(directory, file);
                            using (HttpResponseMessage response = await client.GetAsync(line))
                            {
                                response.EnsureSuccessStatusCode();
                                using (Stream contentStream = await response.Content.ReadAsStreamAsync())
                                using (FileStream fileStream = new FileStream(path, FileMode.Create))
                                {
                                    await contentStream.CopyToAsync(fileStream);
                                }
                            }
                            Console.WriteLine($"Successfully downloaded {file} from {domain}");
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine($"Error downloading file: {e.Message}");
                        }
                            finally
                            {
                                // Release the semaphore slot
                                semaphore.Release();
                            }
                        }));
                    }

                    // Wait for all the tasks to complete
                    await Task.WhenAll(tasks);
                }
            }
        }
    }
}
