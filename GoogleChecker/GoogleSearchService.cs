using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GoogleChecker
{
    public static class GoogleSearchService
    {
        /// <summary>
        /// Searches google.com with a specified query and then returns a list of 
        /// result positions based on the search term
        /// </summary>
        /// <param name="request"></param>
        /// <returns>A list of position where the search term was found on the google search</returns>
        public static async Task<List<int>> SearchAsync(GoogleSearchRequest request)
        {
            string queryTerm = request.QueryTerm.Replace(" ", "+");
            string googleUrl = $"https://www.google.com/search?num={request.NumOfResults}&q={queryTerm}";
            HttpClient client = new HttpClient();

            var response = await client.GetAsync(googleUrl);
            var responseHtml = response.Content.ReadAsStringAsync().Result;
            var resultList = BuildResultList(responseHtml);

            List<int> searchPosition = new List<int>();
            for (int i = 0; i < resultList.Count; i++)
            {
                if (resultList[i].Contains(request.SearchTerm))
                    searchPosition.Add(i + 1);
            }

            if (searchPosition.Count == 0)
                searchPosition.Add(0);

            return searchPosition;
        }

        /// <summary>
        /// We build a result list by removing unnecesary html and traversing the DOM elements
        /// </summary>
        /// <param name="htmlToParse"></param>
        /// <returns>A list of Html Strings that represent different google results</returns>
        private static List<string> BuildResultList(string htmlToParse)
        {
            // remove any non result related html: <head> <style> <footer>
            int lastIndexOfStyles = htmlToParse.LastIndexOf("</style>") + 8;
            htmlToParse = htmlToParse.Substring(lastIndexOfStyles);

            int indexOfFooter = htmlToParse.IndexOf("<footer>");
            htmlToParse = htmlToParse.Substring(0, indexOfFooter);

            // From here, we will split the results based on the opening and closing Div element. 
            List<string> resultList = new List<string>();
            string tempHtml = htmlToParse;
            int nestedLevel = 0;
            int currentPosition = 0;
            int startingIndex = 0;
            int endingIndex = 0;

            while (tempHtml != "")
            {
                int openDivIndex = tempHtml.IndexOf("<div", currentPosition);
                int closeDivIndex = tempHtml.IndexOf("</div>", currentPosition);

                // If you see an opening div, before a closing div you assume it's going 1 nested level deeper
                if (openDivIndex != -1 && openDivIndex < closeDivIndex)
                {
                    currentPosition = openDivIndex + 5;
                    nestedLevel = nestedLevel + 1;
                    if (nestedLevel == 1)
                        startingIndex = openDivIndex;
                }
                else // If you see an closing div, before a opening div you assume it's going up a nested level 
                {
                    currentPosition = closeDivIndex + 6;
                    nestedLevel = nestedLevel - 1;
                    if (nestedLevel == 0)
                        endingIndex = closeDivIndex + 6;
                }

                // Finally, when the nested level is 0, you can conclude that it's one entire result
                if (nestedLevel == 0)
                {
                    string searchResult = tempHtml.Substring(startingIndex, endingIndex - startingIndex);

                    // We do not want the related searches or anything that doesn't match the '<div><div' format 
                    if (searchResult.Contains("<div><div") && !searchResult.Contains("related searches</div>", StringComparison.OrdinalIgnoreCase))
                        resultList.Add(tempHtml.Substring(startingIndex, endingIndex - startingIndex));

                    tempHtml = tempHtml.Substring(endingIndex);
                    startingIndex = 0;
                    endingIndex = 0;
                    currentPosition = 0;
                }
            }

            return resultList;
        }
    }
}
