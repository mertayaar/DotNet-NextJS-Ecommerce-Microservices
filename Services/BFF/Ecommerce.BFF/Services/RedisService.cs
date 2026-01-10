using StackExchange.Redis;

namespace Ecommerce.BFF.Services
{
    public class RedisService
    {
   public string _host { get; set; }
        public int _port { get; set; }
        public int _db { get; set; }
        private ConnectionMultiplexer _connectionMultiplexer;
        public RedisService(string host, int port, int db)
        {
            _host = host;
            _port = port;
            _db = db;
        }

        public void Connect() => _connectionMultiplexer = ConnectionMultiplexer.Connect($"{_host}:{_port},abortConnect=false");
        public IDatabase GetDb() => _connectionMultiplexer.GetDatabase(_db);
    }   
}
