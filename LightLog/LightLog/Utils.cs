using System.IO;

namespace LightLog
{
    public static class Utils
    {
        public static string PathCombine(params string[] paths)
        {
            var winPath = Path.Combine(paths);
            winPath = winPath.Replace(@"\/", "/");
            winPath = winPath.Replace(@"/\", "/");
            winPath = winPath.Replace(@"\\", "/");
            winPath = winPath.Replace(@"//", "/");
            winPath = winPath.Replace(@"\", "/");
            return winPath;
        }
    }
}
