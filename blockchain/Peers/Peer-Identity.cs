namespace Blockchain.Peers
{
    using System.Net.WebSockets;
    using System.Threading;
    using System.Threading.Tasks;
    using Messages;
    using Microsoft.Extensions.Logging;

    public partial class Peer
    {
        private async Task IdentityAsync(
            CancellationToken ct)
        {
            if (_ws is not { State: WebSocketState.Open })
                return;

            _logger.LogTrace(
                "[{Address,15}] Sending identity",
                Address);

            // TODO: Get sensible values
            await SendAsync(
                new IdentityMessage(
                    identity: "GhoaGhoaGhoaGhoaGhoaGhoaGhoaGhoaGhoaGhoaGho=",
                    version: 11,
                    agent: "csharp",
                    name: "csharp-node",
                    port: 9033,
                    head: "0000000017996e3b061fb7118db7007084000a28306a285f193b2551854343bd",
                    work: "1414804792318651",
                    sequence: 23123),
                ct);
        }

        private void HandleIdentity(Message<IdentityMessage> identityMessage)
        {
            var payLoad = identityMessage.Payload;

            _logger.LogTrace(
                "[{Address,15}] Updating identity {Identity}",
                Address,
                payLoad.Identity);

            Identity = payLoad.Identity;

            if (payLoad.Port.HasValue)
                Port = payLoad.Port.Value;

            if (payLoad.Name != null)
                Name = payLoad.Name;
        }
    }
}
