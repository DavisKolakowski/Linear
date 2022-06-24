namespace LinearUpdateDashboard.Models.Configuration
{
    public class SpotStatusDirectoryConfiguration
    {
        /// <summary>Gets or sets the east vault folder prefix.</summary>
        /// <value>The east vault folder prefix.</value>
        public String EastVaultFolderPrefix { get; set; } = String.Empty;

        /// <summary>Gets or sets the west vault folder prefix.</summary>
        /// <value>The west vault folder prefix.</value>
        public String WestVaultFolderPrefix { get; set; } = String.Empty;

        /// <summary>Gets or sets the east primary mezz folder prefix.</summary>
        /// <value>The east primary mezz folder prefix.</value>
        public String EastPrimaryMezzFolderPrefix { get; set; } = String.Empty;

        /// <summary>Gets or sets the west primary mezz folder prefix.</summary>
        /// <value>The west primary mezz folder prefix.</value>
        public String WestPrimaryMezzFolderPrefix { get; set; } = String.Empty;

        /// <summary>Gets or sets the altice assets folder prefix.</summary>
        /// <value>The altice assets folder prefix.</value>
        public String AlticeAssetsFolderPrefix { get; set; } = String.Empty;

        /// <summary>Gets or sets the spectrum assets folder prefix.</summary>
        /// <value>The spectrum assets folder prefix.</value>
        public String SpectrumAssetsFolderPrefix { get; set; } = String.Empty;

        /// <summary>Gets or sets the frontier east assets folder prefix.</summary>
        /// <value>The frontier east assets folder prefix.</value>
        public String FrontierEastAssetsFolderPrefix { get; set; } = String.Empty;

        /// <summary>Gets or sets the frontier west assets folder prefix.</summary>
        /// <value>The frontier west assets folder prefix.</value>
        public String FrontierWestAssetsFolderPrefix { get; set; } = String.Empty;
    }
}
