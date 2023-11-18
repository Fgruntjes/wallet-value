
namespace App.WalletValue;

static class PathUtils
{
    internal static string Resolve(string path)
    {
        return path
            .Replace("~", Environment.GetFolderPath(Environment.SpecialFolder.UserProfile))
            .Replace("//", "");
    }
}