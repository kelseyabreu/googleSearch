using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GoogleChecker
{
    class Program
    {

        static async Task Main(string[] args)
        {
            GoogleSearchRequest request = new GoogleSearchRequest()
            {
                QueryTerm = "efiling integration",
                SearchTerm = "www.infotrack.com",
                NumOfResults = 100
            };

            List<int> positionAppeared = await GoogleSearchService.SearchAsync(request);
            Console.WriteLine(string.Join(",", positionAppeared));
        }
    }
}
