using Newtonsoft.Json;



namespace UnbeatableSongHack.CustomMaps;

public class OnlineDatabase
{

    public class PackageBeatmap
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("artist")]
        public string Artist { get; set; }

        [JsonProperty("creator")]
        public string Creator { get; set; }

        [JsonProperty("difficulty")]
        public string Difficulty { get; set; }

        [JsonProperty("audioFileName")]
        public string AudioFileName { get; set; }
    }

    public class Package
    {
        [JsonProperty("filePath")]
        public string FilePath { get; set; }

        [JsonProperty("time")]
        public string Time { get; set; }

        [JsonProperty("beatmaps")]
        public Dictionary<string, PackageBeatmap> Beatmaps { get; set; }

        public Package() { }

        public Package(string filePath, string time)
        {
            FilePath = filePath;
            Time = time;
            Beatmaps = new Dictionary<string, PackageBeatmap>();
        }

        public override string ToString()
        {
            var serialized = JsonConvert.SerializeObject(this, Formatting.Indented);
            return serialized;
        }
    }

    public class Packages
    {
        [JsonProperty("packages")]
        public Package[] PackageList { get; set; }
        public Packages() { }
        public Packages(Package[] packageList)
        {
            PackageList = packageList;
        }
    }

    public class PackagesRoot
    {
        [JsonProperty("packages")]
        public List<Package> PackageList { get; set; }
        public PackagesRoot() { }
        public PackagesRoot(List<Package> packageList)
        {
            PackageList = packageList;
        }
    }

    public static List<Package> GetPackages()
    {
        string packageURL = DatabaseConfig.GetServerPackageListURL();

        Core.GetLogger().Msg("Getting packages from: " + packageURL);

        using (var webClient = new System.Net.WebClient())
        {
            string json = webClient.DownloadString(packageURL);

            // Deserialize the JSON into an array of Package objects
            var packages = JsonConvert.DeserializeObject<PackagesRoot>(json);
            return packages.PackageList;
        }

    }
}
