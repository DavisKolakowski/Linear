﻿using Dapper;

using Hangfire;

using LinearWebService.Data;
using LinearWebService.Models;

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

using Serilog;
using Serilog.Sinks.SystemConsole;

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

using static System.Net.WebRequestMethods;

namespace LinearWebService.Services
{
    public class DistributionServerEncodeListFileService
    {
        public static IEnumerable<DistributionServerModel> GetDistributionServerDirectoryPaths(string connString)
        {
            var conn = new SqlConnection(connString);
            var distServers = conn.Query<DistributionServerModel>("SELECT * FROM dbo.DistributionServers WHERE ServerFolder IS NOT NULL");

            return distServers;
        }
        public static IEnumerable<SpotFileMapperModel> GetServerDirectorySpotsList(string distServerFolderPath, string connString)
        {
            var conn = new SqlConnection(connString);

            Log.Verbose("Looking for Distribution folder path {0}", distServerFolderPath);
            if (string.IsNullOrEmpty(distServerFolderPath))
            {
                Log.Error("Distribution path is null!");
                return null;
            }

            if (!Directory.Exists(distServerFolderPath))
            {
                Log.Error("Distribution path {0} cannot be found!", distServerFolderPath);
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

            conn.Execute($"UPDATE dbo.DistributionServers SET SpotsLogFileName = @SpotsLogFileName, SpotsLogLastWriteTime = @SpotsLogLastWriteTime  WHERE ServerFolder = @ServerFolder",
                new
                {
                    ServerFolder = distServerFolderPath,
                    SpotsLogFileName = file.FullName,
                    SpotsLogLastWriteTime = file.LastWriteTime,
                });

            string path = Path.GetFullPath(file.ToString());

            //string path = Path.GetFullPath(file.FullName);           
            return ReadSpotsList(path, Encoding.UTF8);
        }
        public static List<SpotFileMapperModel> ReadSpotsList(string path, Encoding encoding)
        {
            Log.Verbose("Attempting to read spots file {0}", path);
            Regex rgx = new Regex(@"^([A-Za-z0-9]+)\s+(?<title>.*)",
                RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var spots = new List<SpotFileMapperModel>();
            var lines = System.IO.File.ReadAllLines(path, encoding);
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
                    Log.Fatal("Index Out of Range: {0}", e.Source);
                }
                catch (RegexParseException e)
                {
                    Log.Fatal("Regex Parse Error: {0}", e.Error);
                }
                catch (SystemException e)
                {
                    Log.Fatal("System Error: {0}", e.InnerException);
                }
            }
            Log.Verbose("Spots file {0} read successfully, returned {1} spots.", Path.GetFileName(path), spots.Count());
            return spots;
        }
    }
}
