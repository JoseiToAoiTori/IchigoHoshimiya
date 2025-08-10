using NetCord.Gateway;
using NetCord.Hosting.Gateway;

namespace IchigoHoshimiya.Handlers;

public class MessageCreateHandler : IMessageCreateGatewayHandler
{
    public ValueTask HandleAsync(Message message)
    {
        return default;
    }
}