

namespace UnbeatableSongHack.CustomMaps
{
    public class DatabaseConfig
    {
        public static string ServerURL = "http://64.225.60.116:8080/";
        public static string ServerPackagesList = "packages.json";

        public static string GetServerPackageListURL()
        {
            return ServerURL + ServerPackagesList;
        }

        public static string GetServerPackageDownloadURL(string packageName)
        {
            return ServerURL + packageName;
        }
    }
}
