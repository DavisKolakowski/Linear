// <copyright file="MarketsController.cs" company="Comcast">
// Copyright (c) Comcast. All Rights Reserved.
// </copyright>

namespace LinearUpdateDashboard.ViewModels
{
    public class SpotsInputFormViewModel
    {
        /// <summary>Gets or sets the vault search.</summary>
        /// <value>The search model.</value>
        public string VaultSearch { get; set; } = string.Empty;
    }

    public class VaultAndMezzResultsViewModel
    {
        /// <summary>Gets or sets the spot found in both vaults.</summary>
        /// <value>The spot found in both vaults.</value>
        public List<string> SpotFoundInBothVaults { get; set; } = new List<string>();

        /// <summary>Gets or sets the spot found in east vault.</summary>
        /// <value>The spot found in east vault.</value>
        public List<string> SpotFoundInEastVault { get; set; } = new List<string>();

        /// <summary>Gets or sets the spot found in west vault.</summary>
        /// <value>The spot found in west vault.</value>
        public List<string> SpotFoundInWestVault { get; set; } = new List<string>();

        /// <summary>Gets or sets the spot not found in vaults but found in mezz.</summary>
        /// <value>The spot not found in vaults but found in mezz.</value>
        public List<string> SpotNotFoundInVaultsButFoundInMezz { get; set; } = new List<string>();

        /// <summary>Gets or sets the spot not found in vault or mezz.</summary>
        /// <value>The spot not found in vault or mezz.</value>
        public List<string> SpotNotFoundInVaultOrMezz { get; set; } = new List<string>();
    }
}
