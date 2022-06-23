using Microsoft.AspNetCore.Mvc;
using LinearUpdateDashboard.ViewModels;
using LinearUpdateDashboard.Models.Configuration;
using Microsoft.Extensions.Options;
using LinearUpdateDashboard.Models;
using System.Text.RegularExpressions;

namespace LinearUpdateDashboard.Controllers
{
    public class SpotStatusController : Controller
    {
        private readonly ILogger<SpotStatusController> _logger;

        private readonly SpotStatusDirectoryConfiguration _config;
        public SpotStatusController(ILogger<SpotStatusController> logger, IOptions<SpotStatusDirectoryConfiguration> config)
        {
            this._logger = logger;
            this._config = config.Value;         
        }

        [HttpGet("SpotStatus/Index")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SpotStatusResults(SpotsInputFormViewModel input)
        {
            var eastVaultDir = new DirectoryInfo(_config.EastVaultFolderPrefix);
            var westVaultDir = new DirectoryInfo(_config.WestVaultFolderPrefix);
            var eastMezzDir = new DirectoryInfo(_config.EastPrimaryMezzFolderPrefix);
            var westMezzDir = new DirectoryInfo(_config.WestPrimaryMezzFolderPrefix);

            if (input.VaultSearch == null)
            {
                return this.NoContent();
            }

            var spots = input.VaultSearch.Split(
                new string[] { Environment.NewLine },
                StringSplitOptions.None
            );

            VaultAndMezzResultsViewModel model = new VaultAndMezzResultsViewModel();
                foreach (var spot in spots)
                {                   
                    var spotExistsInEastVault = DirectoryCheck(spot, eastVaultDir.FullName);
                    var spotExistsInWestVault = DirectoryCheck(spot, westVaultDir.FullName);
                    var spotExistsInEastMezz = DirectoryCheck(ValidateSpot(spot), eastMezzDir.FullName);
                    var spotExistsInWestMezz = DirectoryCheck(ValidateSpot(spot), westMezzDir.FullName);

                        _logger.LogTrace("Attempting to find {0} in {1} and {2}", spot, eastVaultDir, westVaultDir);
                        if (spotExistsInEastVault && spotExistsInWestVault)
                        {
                            _logger.LogTrace("{0} found in {1} and {2}", spot, eastVaultDir, westVaultDir);
                            model.SpotFoundInBothVaults.Add(spot);
                        }

                        else
                        { 
                        _logger.LogTrace("Attempting to find {0} in {1}", spot, eastVaultDir);
                        }
                        if (spotExistsInEastVault && !spotExistsInWestVault)
                        {
                            _logger.LogTrace("{0} found in {1}", spot, eastVaultDir);
                            model.SpotFoundInEastVault.Add(spot);
                        }

                        else
                        {
                            _logger.LogTrace("Attempting to find {0} in {1}", spot, westVaultDir);
                        }
                        if (!spotExistsInEastVault && spotExistsInWestVault)
                        {
                            _logger.LogTrace("{0} found in {1}", spot, westVaultDir);
                            model.SpotFoundInWestVault.Add(spot);
                        }

                        else
                        {
                            _logger.LogTrace("Attempting to find {0} in {1} and {2}", spot, eastMezzDir, westMezzDir);
                        }
                        if ((!spotExistsInEastVault && !spotExistsInWestVault) && (spotExistsInEastMezz || spotExistsInWestMezz))
                        {
                            _logger.LogTrace("{0} found in {1} and {2}", spot, eastMezzDir, westMezzDir);
                            model.SpotNotFoundInVaultsButFoundInMezz.Add(spot);
                        }

                        else if (!spotExistsInEastVault && !spotExistsInWestVault && !spotExistsInEastMezz && !spotExistsInWestMezz)
                        {
                            _logger.LogTrace("{0} not found in {1}, {2}, {3}, and {4}", spot, eastVaultDir, westVaultDir, eastMezzDir, westMezzDir);
                            model.SpotNotFoundInVaultOrMezz.Add(spot);
                        }
                }
            return View("Result", model);
        }
        public string ValidateSpot(string spot)
        {
            string hPattern = @"^.{2}H\d+";
            string aPattern = @"^.{2}A\d+";
            string fourPattern = @"^.{2}4\d+";
            string sevenPattern = @"^.{2}7\d+";
            Regex hVar = new Regex(hPattern);
            Regex aVar = new Regex(aPattern);
            Regex fourVar = new Regex(fourPattern);
            Regex sevenVar = new Regex(sevenPattern);

            var spotCharArray = spot.ToCharArray();
            _logger.LogTrace("Verifying variant of AdCopy {0}", spot);
            if (hVar.IsMatch(spot))
            {
                spotCharArray[2] = '8';
                spot = string.Concat(spotCharArray);
                _logger.LogTrace("Match found! Mezz ID = {0}", spot);
            }
            if (aVar.IsMatch(spot))
            {
                spotCharArray[2] = '1';
                spot = string.Concat(spotCharArray);
                _logger.LogTrace("Match found! Mezz ID = {0}", spot);
            }
            if (fourVar.IsMatch(spot))
            {
                spotCharArray[2] = '8';
                spot = string.Concat(spotCharArray);
                _logger.LogTrace("Match found! Mezz ID = {0}", spot);
            }
            if (sevenVar.IsMatch(spot))
            {
                spotCharArray[2] = '8';
                spot = string.Concat(spotCharArray);
                _logger.LogTrace("Match found! Mezz ID = {0}", spot);
            }
            _logger.LogTrace("Returning new ID {0}", spot);
            return spot;
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
