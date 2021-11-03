namespace PinBot.Core
{
    public class PinBotConfig
    {
        public string ConnectionString { get; set; }
        public string Token { get; set; }
        public int MySqlMajor { get; set; }
        public int MySqlMinor { get; set; }
        public int MySqlBuild { get; set; }
        public ulong TempChannelId { get; set; }
    }
}