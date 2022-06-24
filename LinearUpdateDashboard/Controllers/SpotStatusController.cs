// <copyright file="MarketsController.cs" company="Comcast">
// Copyright (c) Comcast. All Rights Reserved.
// </copyright>

namespace LinearUpdateDashboard.Controllers
{
    using System.Text.RegularExpressions;
    using LinearUpdateDashboard.Models;
    using LinearUpdateDashboard.Models.Configuration;
    using LinearUpdateDashboard.ViewModels;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;

    /// <summary>
    ///   <br />
    /// </summary>
    public class SpotStatusController : Controller
    {
        private readonly ILogger<SpotStatusController> _logger;

        private readonly SpotStatusDirectoryConfiguration _config;

        /// <summary>Initializes a new instance of the <see cref="SpotStatusController" /> class.</summary>
        /// <param name="logger">The logger.</param>
        /// <param name="config">The configuration.</param>
        public SpotStatusController(ILogger<SpotStatusController> logger, IOptions<SpotStatusDirectoryConfiguration> config)
        {
            this._logger = logger;
            this._config = config.Value;
        }

        /// <summary>Indexes this instance.</summary>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpGet("SpotStatus/Index")]
        public IActionResult Index()
        {
            return this.View();
        }

        /// <summary>Spots the status results.</summary>
        /// <param name="input">The input.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [HttpPost]
        public IActionResult SpotStatusResults(SpotsInputFormViewModel input)
        {
            if (input.VaultSearch == null)
            {
                return this.NoContent();
            }

            string[] spots = input.VaultSearch.Split(
                new string[] { Environment.NewLine },
                StringSplitOptions.None);

            VaultAndMezzResultsViewModel model = new VaultAndMezzResultsViewModel();
            foreach (var spot in spots)
                {
                    if (string.IsNullOrWhiteSpace(spot))
                    {
                        this._logger.LogWarning("Spot Cannot be Empty!");
                        return this.NoContent();
                    }

                    bool spotExistsInEastVault = this.SpotExistsInDirectory(spot, this._config.EastVaultFolderPrefix);
                    bool spotExistsInWestVault = this.SpotExistsInDirectory(spot, this._config.WestVaultFolderPrefix);
                    this._logger.LogDebug("Attempting to find {0} in {1} and {2}", spot, this._config.EastVaultFolderPrefix, this._config.WestVaultFolderPrefix);
                    if (spotExistsInEastVault && spotExistsInWestVault)
                    {
                        this._logger.LogDebug("{0} found in {1} and {2}", spot, this._config.EastVaultFolderPrefix, this._config.WestVaultFolderPrefix);
                        model.SpotFoundInBothVaults.Add(spot);
                    }
                    else
                    {
                        this._logger.LogDebug("Attempting to find {0} in {1}", spot, this._config.EastVaultFolderPrefix);
                    }

                    if (spotExistsInEastVault && !spotExistsInWestVault)
                    {
                        this._logger.LogDebug("{0} found in {1}", spot, this._config.EastVaultFolderPrefix);
                        model.SpotFoundInEastVault.Add(spot);
                    }
                    else
                    {
                        this._logger.LogDebug("Attempting to find {0} in {1}", spot, this._config.WestVaultFolderPrefix);
                    }

                    if (!spotExistsInEastVault && spotExistsInWestVault)
                    {
                        this._logger.LogDebug("{0} found in {1}", spot, this._config.WestVaultFolderPrefix);
                        model.SpotFoundInWestVault.Add(spot);
                    }
                    else
                    {
                        this._logger.LogDebug("Attempting to find {0} in {1} and {2}", spot, this._config.EastPrimaryMezzFolderPrefix, this._config.WestPrimaryMezzFolderPrefix);
                    }

                    string mezzSpot = this.GetValidSpotIdForMezz(spot);

                    bool spotExistsInEastMezz = this.SpotExistsInDirectory(mezzSpot, this._config.EastPrimaryMezzFolderPrefix);
                    bool spotExistsInWestMezz = this.SpotExistsInDirectory(mezzSpot, this._config.WestPrimaryMezzFolderPrefix);
                    if ((!spotExistsInEastVault && !spotExistsInWestVault) && (spotExistsInEastMezz || spotExistsInWestMezz))
                    {
                        this._logger.LogDebug("{0} found in {1} and {2}", spot, this._config.EastPrimaryMezzFolderPrefix, this._config.WestPrimaryMezzFolderPrefix);
                        model.SpotNotFoundInVaultsButFoundInMezz.Add(spot);
                    }
                    else if (!spotExistsInEastVault && !spotExistsInWestVault && !spotExistsInEastMezz && !spotExistsInWestMezz)
                    {
                        this._logger.LogDebug("{0} not found in {1}, {2}, {3}, and {4}", spot, this._config.EastVaultFolderPrefix, this._config.WestVaultFolderPrefix, this._config.EastPrimaryMezzFolderPrefix, this._config.WestPrimaryMezzFolderPrefix);
                        model.SpotNotFoundInVaultOrMezz.Add(spot);
                    }
                }

            return this.View("Result", model);
        }

        /// <summary>Gets the valid spot identifier for mezz.</summary>
        /// <param name="spot">The spot.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        public string GetValidSpotIdForMezz(string spot)
        {
            char[] spotCharArray = spot.ToCharArray();

            string hPattern = @"^.{2}H\d+";
            Regex hVar = new Regex(hPattern);
            this._logger.LogDebug("Verifying variant of AdCopy {0}", spot);
            if (hVar.IsMatch(spot))
            {
                this._logger.LogInformation("Match found! {0}", spot);
                spotCharArray[2] = '8';
                spot = string.Concat(spotCharArray);
                this._logger.LogInformation("Mezz ID = {0}", spot);
            }

            string aPattern = @"^.{2}A\d+";
            Regex aVar = new Regex(aPattern);
            if (aVar.IsMatch(spot))
            {
                this._logger.LogInformation("Match found! {0}", spot);
                spotCharArray[2] = '1';
                spot = string.Concat(spotCharArray);
                this._logger.LogInformation("Mezz ID = {0}", spot);
            }

            string fourPattern = @"^.{2}4\d+";
            Regex fourVar = new Regex(fourPattern);
            if (fourVar.IsMatch(spot))
            {
                this._logger.LogInformation("Match found! {0}", spot);
                spotCharArray[2] = '8';
                spot = string.Concat(spotCharArray);
                this._logger.LogInformation("Mezz ID = {0}", spot);
            }

            string sevenPattern = @"^.{2}7\d+";
            Regex sevenVar = new Regex(sevenPattern);
            if (sevenVar.IsMatch(spot))
            {
                this._logger.LogInformation("Match found! {0}", spot);
                spotCharArray[2] = '8';
                spot = string.Concat(spotCharArray);
                this._logger.LogInformation("Mezz ID = {0}", spot);
            }
            else
            {
                this._logger.LogDebug("Spot is already valid {0}", spot);
            }

            this._logger.LogDebug("Returning ID {0}", spot);
            return spot;
        }

        /// <summary>Spots the exists in directory.</summary>
        /// <param name="spot">The spot.</param>
        /// <param name="dir">The dir.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        public bool SpotExistsInDirectory(string spot, string dir)
        {
            if (string.IsNullOrEmpty(dir))
            {
                this._logger.LogError("Directory is not set: ", dir);
                return false;
            }

            if (!Directory.Exists(dir))
            {
                this._logger.LogError("Directory does not exist! ", dir);
                return false;
            }

            string file = $"{spot}.mpg";
            string spotFileFullPath = Path.Combine(dir, file);

            this._logger.LogDebug("Attempting to find {0} in {1}", file, dir);
            bool exists = System.IO.File.Exists(spotFileFullPath);

            if (exists)
            {
                this._logger.LogDebug("{0} found in {1}", spot, dir);
            }
            else
            {
                this._logger.LogDebug("{0} not found in {1}", spot, dir);
            }

            return exists;
        }
    }
}
