namespace ASP.Models.Rest
{
    public class RestMeta
    {
        public long ServerTime { get; set; } = DateTime.Now.Ticks;
        public string ResourceName { get; set; } = null!;
        public string ResourceUrl { get; set; } = null!;
        public string Method { get; set; } = "GET";
        public string DataType { get; set; } = null!;
        public long Cache { get; set; }
        public string[] Manipulations { get; set; } = ["GET"];
        public Dictionary<string, string> Links { get; set; } = [];

    }
}
