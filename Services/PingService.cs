using IchigoHoshimiya.Interfaces;

namespace IchigoHoshimiya.Services;

public class PingService : IPingService
{
    public string Ping() => "Pong!";
}