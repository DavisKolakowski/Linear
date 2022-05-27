
# Effectv's Linear Update Dashboard

Effectv's Linear Update Dashboard is a tool use for pulling and sorting conflicts from the Skyvision Distribution Servers and displaying them in a simple, easy to manage, single client side data application.


## Controller Reference

#### Get Market List

```http
  //GET: Markets
```

| Parameter | Type     | Description                |
| :-------- | :------- | :------------------------- |
| `HttpGet` | `async Task<IActionResult> Index()` | Fetch Market List |
| `[HttpGet("{name}")]` | `async Task<IActionResult> Details(string name)` | Fetch Spots in Market |


## Fetch Market List Example

```csharp
public async Task<IActionResult> Index()
{
    var markets = await this._context.Markets.ToListAsync();
    var countDict = new Dictionary<int,int>();

    foreach(var market in markets) {
        var spots = await this.GetSpotsByMarketName(market.Name);
        countDict.Add(market.Id, spots.Count());
    }

    var model = new MarketListViewModel()
    {
        Markets = markets,
        MarketSpotCount = countDict
    };
    return _context.Markets != null ?
                    View(model) :
                    Problem("Entity set 'LinearDbContext.Markets'  is null.");
}
```
## Fetch Market Details Example

```csharp
[HttpGet("{name}")]
public async Task<IActionResult> Details(string name)
{
    if (name == null || this._context.Markets == null)
    {
        return this.NotFound();
    }
    var model = new MarketDetailsViewModel()
    {
        SpotsInMarket = await this.GetSpotsByMarketName(name)
    };
    if (model == null)
    {
        return this.NotFound();
    }
    return View(model);
}
```
## Fetch Spots in Market Method

```csharp
public async Task<List<Spot>> GetSpotsByMarketName(string name) 
{
    return await this._context.Markets
            .Where(m => m.Name == name)
            .Include(hq => hq.Headquarters)
                    .ThenInclude(hqds => hqds.DistributionServers)
                        .ThenInclude(ds => ds.DistributionServerSpots)
                            .ThenInclude(dss => dss.Spot)
                                .ThenInclude(spot => spot.DistributionServerSpots)
                                    .ThenInclude(dss => dss.DistributionServer)
                .SelectMany(m => m.Headquarters)
                .SelectMany(hq => hq.DistributionServers)
                .SelectMany(ds => ds.DistributionServerSpots)
                .Select(dss => dss.Spot)
                .Distinct()
                .ToListAsync();
}
```
## View Model References
```csharp
//ViewModel For Index
public class MarketListViewModel
{
    public List<Market> Markets { get; set; } = new List<Market>();
    public Dictionary<int, int> MarketSpotCount = new Dictionary<int, int>();
}
//ViewModel For Spots In Market Details Page
public class MarketDetailsViewModel
{
    public Market? Market { get; set; } = null;
    public List<Spot> SpotsInMarket { get; set; } = new List<Spot>();
}
```
## EF6 Model

```csharp
public class Market
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<Headquarters> Headquarters { get; } = new List<Headquarters>();
    public DateTime? LastUpdated { get; set; }
}

public class Headquarters
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<Market> Markets { get; } = new List<Market>();
    public List<DistributionServer> DistributionServers { get; } = new List<DistributionServer>();
    public DateTime? LastUpdated { get; set; }
}

public class DistributionServer
{
    public int Id { get; set; }
    public string ServerIdentity { get; set; }
    public string ServerFolder { get; set; }
    public Headquarters Headquarters { get; set; }
    public List<DistributionServerSpot> DistributionServerSpots { get; } = new List<DistributionServerSpot>();
    public DateTime? LastUpdated { get; set; }
}

public class DistributionServerSpot
{
    public int Id { get; set; }
    public DistributionServer DistributionServer { get; set; }
    public Spot Spot { get; set; }
    public DateTime? FirstAirDate { get; set; }
    public DateTime? LastUpdated { get; set; }
}

public class Spot
{
    public int Id { get; set; }
    public string SpotCode { get; set; }
    public string Name { set; get; }
    public List<DistributionServerSpot> DistributionServerSpots { get; } = new List<DistributionServerSpot>();
    public DateTime? LastUpdated { get; set; }
}
```
# Effectv's Linear Data Service

Linear's Console Data Application
## Method Reference

#### RegEx Examples

```csharp
//Get most recent file, with the format mmddyy.txt
Regex filenameRegex = new Regex(@"\d{6}\.txt");
var dirFiles = new DirectoryInfo(distServerFolderPath).GetFiles();
var files = dirFiles.Where(path => filenameRegex.IsMatch(path.Name))
    .OrderByDescending(f => f.LastWriteTime);                       
var file = files.FirstOrDefault();
```
```csharp
//Parse data file into a dataset
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
```
#### Dapper Examples
```csharp
//inserts dataset into db with a sproc
public static async Task UpdateDatabaseAsync(string conString)
{
    SqlConnection conn = new SqlConnection(conString);
    await conn.OpenAsync();

    var distServers = await FileScrapeManager.GetDistributionServerAsync(conString);

    foreach (var distServer in distServers)
    {
        var items = await FileScraperService.GetServerDirectoryAsync(distServer.ServerFolder);

        Console.WriteLine($"Deleting spots for {distServer.ServerFolder}");
        await conn.ExecuteAsync($"exec DeleteSpotsByDistributionServer @DistributionServerIdentity=@DistributionServerIdentity", new { DistributionServerIdentity = distServer.ServerIdentity });
        Console.WriteLine("Starting Database Upload...");
        items.ToList().ForEach(async x =>
            await conn.ExecuteAsync("exec dbo.InsertSpot @DistributionServerIdentity = @DistributionServerIdentity, @SpotCode = @SpotCode, @Name = @SpotTitle, @FirstAirDate = @FirstAirDate",
            new
            {
                DistributionServerIdentity = distServer.ServerIdentity,
                SpotCode = x.SpotCode,
                SpotTitle = x.SpotTitle,
                FirstAirDate = x.FirstAirDate
            }));
        Console.WriteLine($"Upload Complete! {items.Count()} Spots have been uploaded");
    }
}
```
```csharp
//queries the distribution server's table to map the script's directory paths to find
public static async Task<IEnumerable<DistributionServerModel>> GetDistributionServerAsync(string conString)
{
    SqlConnection conn = new SqlConnection(conString);
    var ds = await conn.QueryAsync<DistributionServerModel>("SELECT * FROM dbo.DistributionServers WHERE ServerFolder IS NOT NULL");
    return ds;
}
```

## Authors

- Davis Kolakowski


## Features

- Advanced Spots in Market table filtering
- Live Data
- Fullscreen mode
- Cross platform
- Mobile responsive
- Sorts and removes duplicate conflicts automatically
- Provides the last update time for each market
- .NET Core MVC
- EF6


## Feedback

If you have any feedback, please fill out the following related form Linear/.github/ISSUE_TEMPLATE/


## Used By

This project is used by:

- Comcast Corp (Effectv)

