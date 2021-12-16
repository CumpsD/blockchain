namespace Blockchain.Peers
{
    using System.Net.WebSockets;
    using System.Threading;
    using System.Threading.Tasks;
    using Messages;
    using Microsoft.Extensions.Logging;

    public partial class Peer
    {
        private const int VERSION_PROTOCOL = 12;
        private const int VERSION_PROTOCOL_MIN = 12;

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
                    identity: "yM9Gk00zKfXuoD0u+f9xUyi5gmBSSQjzkg+15HTXSUg=", //"GhoaGhoaGhoaGhoaGhoaGhoaGhoaGhoaGhoaGhoaGho=",
                    version: VERSION_PROTOCOL,
                    agent: "if/cli/src",
                    name: "testnet",
                    port: 9033,
                    head: "0000000017996e3b061fb7118db7007084000a28306a285f193b2551854343bd",
                    work: "1414804792318651",
                    sequence: 23123),
                ct);
        }

        private void HandleIdentity(Message<IdentityMessage> identityMessage)
        {
            var payLoad = identityMessage.Payload;

            if (payLoad.Version < VERSION_PROTOCOL_MIN)
            {
                _logger.LogTrace(
                    "[{Address,15}] Peer {Identity} does not meet minimum protocol version, has {Version}, needs {MinVersion}",
                    Address,
                    payLoad.Identity,
                    payLoad.Version,
                    VERSION_PROTOCOL_MIN);

                _peerPool.RemovePeer(
                    Address,
                    "Invalid protocol version");

                return;
            }

            _logger.LogInformation(
                "[{Address,15}] Connected to {Identity} / {Name} ({Address}:{Port})",
                Address,
                payLoad.Identity,
                string.IsNullOrWhiteSpace(Name) ? "*" : Name,
                Address,
                Port);

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
