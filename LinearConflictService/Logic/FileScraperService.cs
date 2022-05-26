using LinearConflictService.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Sinks.SystemConsole;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

using static System.Net.WebRequestMethods;

namespace LinearConflictService.Logic
{
    internal class FileScraperService
    {
        public static async Task<IEnumerable<SpotFileMapperModel>> GetServerDirectoryAsync(string distServerFolderPath)
        {
            Log.Logger.Information("Looking for Distribution folder path {0}", distServerFolderPath);
            if (string.IsNullOrEmpty(distServerFolderPath))
            {
                Log.Logger.Error("Distribution path is null!");
                return null;
            }

            if (!Directory.Exists(distServerFolderPath))
            {
                Log.Logger.Error("Distribution path {0} cannot be found!", distServerFolderPath);
                return null;
            }


            Regex filenameRegex = new Regex(@"\d{6}\.txt");
            var dirFiles = new DirectoryInfo(distServerFolderPath).GetFiles();
            var files = dirFiles.Where(path => filenameRegex.IsMatch(path.Name))
                .OrderByDescending(f => f.LastWriteTime);                       
            var file = files.FirstOrDefault();

            if (file == null)
            {
                return null;
            }
            string path = Path.GetFullPath(file.ToString());

            //string path = Path.GetFullPath(file.FullName);           
            return await ReadSpotsListAsync(path, Encoding.UTF8);                    
        }
        public static async Task<List<SpotFileMapperModel>> ReadSpotsListAsync(string path, Encoding encoding)
        {
            Log.Logger.Information("Attempting to read spots file {0}", path);
            Regex rgx = new Regex(@"^([A-Za-z0-9]+)\s+(?<title>.*)",
                RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var spots = new List<SpotFileMapperModel>();
            var lines = await System.IO.File.ReadAllLinesAsync(path, encoding);
            for (int i = 0; i < lines.Length; i++)
            {
                try
                {
                    if (i % 6 == 0)
                    {
                        var m = rgx.Match(lines[i + 1]);
                        var items = new SpotFileMapperModel()
                        {
                            SpotCode = Path.GetFileName(lines[i + 0]).ToString(),
                            SpotTitle = m.Groups["title"].Value,
                            FirstAirDate = Convert.ToDateTime(lines[i + 4])
                        };
                        spots.Add(items);
                    }
                }
                catch (IndexOutOfRangeException e)
                {
                    Console.WriteLine(e.Source);
                }
                catch (RegexParseException e)
                {
                    Console.WriteLine(e.Error);
                }
                catch (SystemException e)
                {
                    Console.WriteLine(e.InnerException);
                }
            }
            Log.Logger.Information("Spots file {0} read successfully, returned {1} spots.", lines, spots.Count());
            return spots;
        }
    }
}
