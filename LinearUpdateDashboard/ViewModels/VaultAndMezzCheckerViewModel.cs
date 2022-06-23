namespace LinearUpdateDashboard.ViewModels
{
    public class SpotsInputFormViewModel
    {
        public string VaultSearch { get; set; } = string.Empty;
    }
    public class VaultAndMezzResultsViewModel
    {
        public List<string> SpotFoundInBothVaults { get; set; } = new List<string>();
        public List<string> SpotFoundInEastVault { get; set; } = new List<string>();
        public List<string> SpotFoundInWestVault { get; set; } = new List<string>();
        public List<string> SpotNotFoundInVaultsButFoundInMezz { get; set; } = new List<string>();
        public List<string> SpotNotFoundInVaultOrMezz { get; set; } = new List<string>();
    }
}
