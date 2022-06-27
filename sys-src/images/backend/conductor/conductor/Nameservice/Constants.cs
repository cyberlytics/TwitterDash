namespace conductor.Nameservice
{
    public static class Constants
    {
        public static readonly HashSet<string> Valid_Languages = new() { "de", "en", "fr", "nl" };
        public const int TweetLimit = 500;
        public static readonly Dictionary<int, string> Valid_Countries = new()
        {
            { 23424829, "de" },
            { 23424975, "uk" },
            { 23424977, "us" },
            { 23424909, "nl" },
            { 23424819, "fr" },
            { 23424775, "ca" },
            { 23424803, "ir" },
            { 23424748, "australia" },
            { 23424750, "austria" },
            { 23424916, "new zealand" },
        };
    }
   
}
