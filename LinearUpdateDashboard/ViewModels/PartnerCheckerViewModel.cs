namespace LinearUpdateDashboard.ViewModels
{
    public class AssetCheckInputFormViewModel
    {
        public string AssetSearch { get; set; } = string.Empty;
    }
    public class AssetsResultsViewModel
    {
        public List<string> SpotFoundInAlticeAssets { get; set; } = new List<string>();
        public List<string> SpotFoundInSpectrumAssets { get; set; } = new List<string>();
        public List<string> SpotFoundInScenicEastAssets { get; set; } = new List<string>();
        public List<string> SpotFoundInScenicWestAssets { get; set; } = new List<string>();
        public List<string> SpotFoundInFrontierEastAssets { get; set; } = new List<string>();
        public List<string> SpotFoundInFrontierWestAssets { get; set; } = new List<string>();
        public List<string> SpotNotFound { get; set; } = new List<string>();
    }
}
