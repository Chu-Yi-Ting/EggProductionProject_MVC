using HtmlAgilityPack;


namespace EggProductionProject_MVC.HTTPModels
{
    public class WebScrapingService
    {
        private readonly HttpClient _httpClient;

        public WebScrapingService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<List<string>>> FetchTableDataAsync(string url)
        {
            var data = new List<List<string>>();

            var html = await _httpClient.GetStringAsync(url);
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var table = doc.GetElementbyId("ctl00_ctl00_cpl_MainContent_cpl_BasicMainContent_ctl00_T_54_51");

            if (table != null)
            {
                var rows = table.SelectNodes(".//tr");
                if (rows != null)
                {
                    foreach (var row in rows)
                    {
                        var cells = row.SelectNodes(".//td|.//th");
                        if (cells != null)
                        {
                            var rowData = new List<string>();
                            foreach (var cell in cells)
                            {
                                rowData.Add(cell.InnerText.Trim());
                            }
                            data.Add(rowData);
                        }
                    }
                }
            }

            return data;
        }
    }

}


// Services/WebScrapingService.cs



