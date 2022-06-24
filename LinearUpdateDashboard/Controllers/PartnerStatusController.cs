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
    public class PartnerStatusController : Controller
    {
        private readonly ILogger<PartnerStatusController> _logger;

        private readonly SpotStatusDirectoryConfiguration _config;

        /// <summary>Initializes a new instance of the <see cref="PartnerStatusController" /> class.</summary>
        /// <param name="logger">The logger.</param>
        /// <param name="config">The configuration.</param>
        public PartnerStatusController(ILogger<PartnerStatusController> logger, IOptions<SpotStatusDirectoryConfiguration> config)
        {
            this._logger = logger;
            this._config = config.Value;
        }

        /// <summary>Indexes this instance.</summary>
        /// <returns>Search Partner Directories.</returns>
        [HttpGet("PartnerStatus/Index")]
        public IActionResult Index()
        {
            return this.View();
        }

        /// <summary>Assets the status results.</summary>
        /// <param name="input">The input.</param>
        /// <returns>Post Results of Spot's Status.</returns>
        [HttpPost]
        public IActionResult AssetStatusResults(AssetCheckInputFormViewModel input)
        {
            if (input.AssetSearch == null)
            {
                return this.NoContent();
            }

            string[] spots = input.AssetSearch.Split(
                new string[] { Environment.NewLine },
                StringSplitOptions.None);

            AssetsResultsViewModel model = new AssetsResultsViewModel();
            foreach (var spot in spots)
                {
                        bool spotExistsInAlticeAssets = this.SpotExistsInDirectory(spot, this._config.AlticeAssetsFolderPrefix);
                        this._logger.LogTrace("Attempting to find {0} in {1}", spot, this._config.AlticeAssetsFolderPrefix);
                        if (spotExistsInAlticeAssets)
                        {
                            this._logger.LogInformation("{0} found in {1}", spot, this._config.AlticeAssetsFolderPrefix);
                            model.SpotFoundInAlticeAssets.Add(spot);
                        }
                        else
                        {
                        this._logger.LogTrace("Attempting to find {0} in {1}", spot, this._config.SpectrumAssetsFolderPrefix);
                        }

                        bool spotExistsInSpectrumAssets = this.SpotExistsInDirectory(spot, this._config.SpectrumAssetsFolderPrefix);
                        if (spotExistsInSpectrumAssets)
                        {
                            this._logger.LogInformation("{0} found in {1}", spot, this._config.SpectrumAssetsFolderPrefix);
                            model.SpotFoundInSpectrumAssets.Add(spot);
                        }
                        else
                        {
                            this._logger.LogTrace("Attempting to find {0} in {1}", spot, this._config.FrontierEastAssetsFolderPrefix);
                        }

                        bool spotExistsInFrontierEastAssets = this.SpotExistsInDirectory(spot, this._config.FrontierEastAssetsFolderPrefix);
                        if (spotExistsInFrontierEastAssets)
                        {
                            this._logger.LogInformation("{0} found in {1}", spot, this._config.FrontierEastAssetsFolderPrefix);
                            model.SpotFoundInFrontierEastAssets.Add(spot);
                        }
                        else
                        {
                            this._logger.LogTrace("Attempting to find {0} in {1}", spot, this._config.FrontierWestAssetsFolderPrefix);
                        }

                        bool spotExistsInFrontierWestAssets = this.SpotExistsInDirectory(spot, this._config.FrontierWestAssetsFolderPrefix);
                        if (spotExistsInFrontierWestAssets)
                        {
                            this._logger.LogInformation("{0} found in {1}", spot, this._config.FrontierWestAssetsFolderPrefix);
                            model.SpotFoundInFrontierWestAssets.Add(spot);
                        }
                        else if (!spotExistsInAlticeAssets && !spotExistsInSpectrumAssets && !spotExistsInFrontierEastAssets && !spotExistsInFrontierWestAssets)
                        {
                            this._logger.LogInformation("{0} not found in {1}, {2}, {3}, and {4}", spot, this._config.AlticeAssetsFolderPrefix, this._config.SpectrumAssetsFolderPrefix, this._config.FrontierEastAssetsFolderPrefix, this._config.FrontierWestAssetsFolderPrefix);
                            model.SpotNotFound.Add(spot);
                        }
                }

            return this.View("Result", model);
        }

        /// <summary>Spots the exists in directory.</summary>
        /// <param name="spot">The spot.</param>
        /// <param name="dir">The dir.</param>
        /// <returns>Spots Existing in Partner Directory Paths.</returns>
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

            this._logger.LogTrace("Attempting to find {0} in {1}", file, dir);
            bool exists = System.IO.File.Exists(spotFileFullPath);

            if (exists)
            {
                this._logger.LogInformation("{0} found in {1}", spot, dir);
            }
            else
            {
                this._logger.LogInformation("{0} not found in {1}", spot, dir);
            }

            return exists;
        }
    }
}
