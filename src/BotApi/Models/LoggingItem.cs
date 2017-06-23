using System;
using Newtonsoft.Json;

namespace BotApi.Models
{
    public class LoggingItem
    {
        [JsonProperty(PropertyName = "id")]
        public string Id  => DateTime.UtcNow.ToString("yyyyMMddHHmmssffffff");
        public DateTime Time { get; set; }
        public string OpponentName { get; set; }
        public int DynamiteCount { get; set; }
        public int PointsToWin { get; set; }
        public int MaxRounds { get; set; }
        public string[] OurMoves { get; set; }
        public string[] OpponentMoves { get; set; }
        public LoggingMove[] Moves { get; set; }
        public string Environment { get; set; }
        public DateTime LogTime => DateTime.Now;
    }
}