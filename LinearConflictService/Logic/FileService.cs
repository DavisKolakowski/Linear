using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.RegularExpressions;

using LinearConflictService.Models;

using Microsoft.Data.SqlClient;

using static System.Net.Mime.MediaTypeNames;

namespace LinearConflictService.Logic
{
    internal class FileService
    {
        public static async Task<IEnumerable<SpotFileMapperModel>> ReadSpotsListAsync(string distServer, string conflLog)
        {
            string dirPath = @$"C:\{distServer}\c$\encodelist";
            string path = Path.GetFullPath(@$"{dirPath}\{conflLog}.txt");
            if (!File.Exists(path))
            {
                Console.WriteLine("File Not Found!");
            }
            Console.WriteLine("Reading File...");
            return await ReadSpotsListAsync(path, Encoding.UTF8);
        }

        public static async Task<List<SpotFileMapperModel>> ReadSpotsListAsync(string path, Encoding encoding)
        {    
            Regex rgx = new Regex(@"^([A-Za-z0-9]+)\s+(?<title>.*)",
                RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var spots = new List<SpotFileMapperModel>();            
            var lines = await File.ReadAllLinesAsync(path, encoding);                          
                for (int i = 0; i < lines.Length; i++)
                {
                        try
                        {
                            if (i % 6 == 0)
                            {
                                var m = rgx.Match(lines[i + 1]);
                                var items = new SpotFileMapperModel()
                                {
                                    SpotCode = lines[i + 0].Replace(@"d:\mpegfiles\", string.Empty),
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
            return spots;
        }
    }
}
