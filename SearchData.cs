using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Drawing;


namespace SearchEngine
{
    public class SearchData
    {
        public string Word { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }

        //image
        public string ThumbnailUrl { get; set; }
        public Image ThumbnailImage { get; set; }
        public int Position { get; set; }
        public int Distance { get; set; }
        public int Frequency { get; set; }

        // Method to create SearchData
        // Custom JSON parsing method
        public static List<SearchData> ParseFromJson(string jsonResponse)
        {
            var results = new List<SearchData>();
            try
            {
                using (JsonDocument doc = JsonDocument.Parse(jsonResponse))
                {
                    var root = doc.RootElement;

                    if (root.TryGetProperty("organic_results", out JsonElement organicResults))
                    {
                        foreach (var result in organicResults.EnumerateArray())
                        {
                            var searchData = new SearchData
                            {
                                Title = result.GetProperty("title").GetString(),
                                Url = result.GetProperty("link").GetString(),
                                Description = result.TryGetProperty("snippet", out JsonElement snippet)
                                    ? snippet.GetString()
                                    : "No description available",
                                // Add thumbnail URL if available
                                ThumbnailUrl = result.TryGetProperty("thumbnail", out JsonElement imageElement)
                                    ? imageElement.GetString()
                                    : null
                            };
                            // Optional: Load thumbnail image if URL exists
                            if (!string.IsNullOrEmpty(searchData.ThumbnailUrl))
                            {
                                try
                                {
                                    using (var webClient = new System.Net.WebClient())
                                    {
                                        byte[] imageBytes = webClient.DownloadData(searchData.ThumbnailUrl);
                                        using (var ms = new System.IO.MemoryStream(imageBytes))
                                        {
                                            searchData.ThumbnailImage = Image.FromStream(ms);
                                        }
                                    }
                                }
                                catch
                                {
                                    // Handle image loading error
                                    searchData.ThumbnailImage = null;
                                }
                            }
                            results.Add(searchData);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log or handle parsing error
                Console.WriteLine($"JSON Parsing Error: {ex.Message}");
            }
            return results;
        }
    }
}