namespace GoogleChecker
{
    public class GoogleSearchRequest
    {
        /// <summary>
        /// The term used to look up on google.com
        /// </summary>
        public string QueryTerm { get; set; }

        /// <summary>
        /// The string to search for on the results returned by google.com
        /// </summary>
        public string SearchTerm { get; set; }

        /// <summary>
        /// The amount of results google will return, max seems to be 100
        /// </summary>
        public int NumOfResults { get; set; }
    }
}
