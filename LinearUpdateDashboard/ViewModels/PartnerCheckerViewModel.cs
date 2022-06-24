namespace LinearUpdateDashboard.ViewModels
{
    public class AssetCheckInputFormViewModel
    {
        /// <summary>Gets or sets the asset search.</summary>
        /// <value>The asset search.</value>
        public string AssetSearch { get; set; } = string.Empty;
    }

    public class AssetsResultsViewModel
    {
        /// <summary>Gets or sets the spot found in altice assets.</summary>
        /// <value>The spot found in altice assets.</value>
        public List<string> SpotFoundInAlticeAssets { get; set; } = new List<string>();

        /// <summary>Gets or sets the spot found in spectrum assets.</summary>
        /// <value>The spot found in spectrum assets.</value>
        public List<string> SpotFoundInSpectrumAssets { get; set; } = new List<string>();

        /// <summary>Gets or sets the spot found in scenic east assets.</summary>
        /// <value>The spot found in scenic east assets.</value>
        public List<string> SpotFoundInScenicEastAssets { get; set; } = new List<string>();

        /// <summary>Gets or sets the spot found in scenic west assets.</summary>
        /// <value>The spot found in scenic west assets.</value>
        public List<string> SpotFoundInScenicWestAssets { get; set; } = new List<string>();

        /// <summary>Gets or sets the spot found in frontier east assets.</summary>
        /// <value>The spot found in frontier east assets.</value>
        public List<string> SpotFoundInFrontierEastAssets { get; set; } = new List<string>();

        /// <summary>Gets or sets the spot found in frontier west assets.</summary>
        /// <value>The spot found in frontier west assets.</value>
        public List<string> SpotFoundInFrontierWestAssets { get; set; } = new List<string>();

        /// <summary>Gets or sets the spot not found.</summary>
        /// <value>The spot not found.</value>
        public List<string> SpotNotFound { get; set; } = new List<string>();
    }
}
