using Microsoft.AspNetCore.Mvc;
using LinearUpdateDashboard.ViewModels;
using LinearUpdateDashboard.Models.Configuration;
using Microsoft.Extensions.Options;
using LinearUpdateDashboard.Models;
using System.Text.RegularExpressions;

namespace LinearUpdateDashboard.Controllers
{
    public class PartnerStatusController : Controller
    {
        private readonly ILogger<PartnerStatusController> _logger;

        private readonly SpotStatusDirectoryConfiguration _config;
        public PartnerStatusController(ILogger<PartnerStatusController> logger, IOptions<SpotStatusDirectoryConfiguration> config)
        {
            this._logger = logger;
            this._config = config.Value;         
        }

        [HttpGet("PartnerStatus/Index")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AssetStatusResults(AssetCheckInputFormViewModel input)
        {
            var alticeDir = new DirectoryInfo(_config.AlticeAssetsFolderPrefix);
            var spectrumDir = new DirectoryInfo(_config.SpectrumAssetsFolderPrefix);
            var frontierEastDir = new DirectoryInfo(_config.FrontierEastAssetsFolderPrefix);
            var frontierWestDir = new DirectoryInfo(_config.FrontierWestAssetsFolderPrefix);

            if (input.AssetSearch == null)
            {
                return this.NoContent();
            }

            var spots = input.AssetSearch.Split(
                new string[] { Environment.NewLine },
                StringSplitOptions.None
            );

            AssetsResultsViewModel model = new AssetsResultsViewModel();
                foreach (var spot in spots)
                {                   
                    var spotExistsInAlticeAssets = DirectoryCheck(spot, alticeDir.FullName);
                    var spotExistsInSpectrumAssets = DirectoryCheck(spot, spectrumDir.FullName);
                    var spotExistsInFrontierEastAssets = DirectoryCheck(spot, frontierEastDir.FullName);
                    var spotExistsInFrontierWestAssets = DirectoryCheck(spot, frontierWestDir.FullName);

                        _logger.LogTrace("Attempting to find {0} in {1}", spot, alticeDir);
                        if (spotExistsInAlticeAssets)
                        {
                            _logger.LogTrace("{0} found in {1}", spot, alticeDir);
                            model.SpotFoundInAlticeAssets.Add(spot);
                        }

                        else
                        { 
                        _logger.LogTrace("Attempting to find {0} in {1}", spot, spectrumDir);
                        }
                        if (spotExistsInSpectrumAssets)
                        {
                            _logger.LogTrace("{0} found in {1}", spot, spectrumDir);
                            model.SpotFoundInSpectrumAssets.Add(spot);
                        }

                        else
                        {
                            _logger.LogTrace("Attempting to find {0} in {1}", spot, frontierEastDir);
                        }
                        if (spotExistsInFrontierEastAssets)
                        {
                            _logger.LogTrace("{0} found in {1}", spot, frontierEastDir);
                            model.SpotFoundInFrontierEastAssets.Add(spot);
                        }

                        else
                        {
                            _logger.LogTrace("Attempting to find {0} in {1}", spot, frontierWestDir);
                        }
                        if (spotExistsInFrontierWestAssets)
                        {
                            _logger.LogTrace("{0} found in {1}", spot, frontierWestDir);
                            model.SpotFoundInFrontierWestAssets.Add(spot);
                        }

                        else if (!spotExistsInAlticeAssets && !spotExistsInSpectrumAssets && !spotExistsInFrontierEastAssets && !spotExistsInFrontierWestAssets)
                        {
                            _logger.LogTrace("{0} not found in {1}, {2}, {3}, and {4}", spot, spotExistsInAlticeAssets, spotExistsInSpectrumAssets, spotExistsInFrontierEastAssets, spotExistsInFrontierWestAssets);
                            model.SpotNotFound.Add(spot);
                        }
                }
            return View("Result", model);
        }
        
        public bool DirectoryCheck(string spot, string dir)
        {
            if (string.IsNullOrEmpty(dir))
            {
                _logger.LogError("Directory is not set: ", dir);
                return false;
            }
            if (!Directory.Exists(dir))
            {
                _logger.LogError("Directory does not exist! ", dir);
                return false;
            }
            var file = $"{spot}.mpg";
            var spotFileFullPath = Path.Combine(dir, file);

            _logger.LogTrace("Attempting to find {0} in {1}", file, dir);
            var exists = System.IO.File.Exists(spotFileFullPath);

            if (exists)
            {
                _logger.LogTrace("{0} found in {1}", spot, dir);
            }
            else
            {
                _logger.LogTrace("{0} not found in {1}", spot, dir);
            }

            return exists;
        }
    }
}
