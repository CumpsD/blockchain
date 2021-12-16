namespace Blockchain.Configuration
{
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics.CodeAnalysis;
    using UsedImplicitly = JetBrains.Annotations.UsedImplicitlyAttribute;

    public class BlockchainConfiguration
    {
        // ReSharper disable once InconsistentNaming
        [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Convention for configuration is .Section")]
        public const string Section = "Blockchain";

        [Required, NotNull]
        public string? BootstrapNodeAddress
        {
            get;
            [UsedImplicitly] init;
        }

        [Required, NotNull]
        public int? BootstrapNodePort
        {
            get;
            [UsedImplicitly] init;
        }

        [Required, NotNull]
        public int? DefaultWebsocketPort
        {
            get;
            [UsedImplicitly] init;
        }

        [Required, NotNull]
        public int? UpdatePeerListIntervalInSeconds
        {
            get;
            [UsedImplicitly] init;
        }

        [Required, NotNull]
        public int? PrintPeerInfoIntervalInSeconds
        {
            get;
            [UsedImplicitly] init;
        }

        [Required, NotNull]
        public int? TargetPeerCount
        {
            get;
            [UsedImplicitly] init;
        }
    }
}
