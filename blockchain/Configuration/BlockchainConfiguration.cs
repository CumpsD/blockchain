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
        public string? Example
        {
            get;
            [UsedImplicitly] init;
        }
    }
}
