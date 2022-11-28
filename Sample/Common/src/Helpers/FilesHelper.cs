using System.IO;
using System.Threading.Tasks;

namespace Sample.Common.Helpers;

public static class FilesHelper
{
    public static async Task<string> SaveToCacheAsync(Stream data, string fileName)
    {
        var filePath = Path.Combine(FileSystem.CacheDirectory, fileName);

        if (File.Exists(filePath))
            File.Delete(filePath);

        await using var stream = File.Create(filePath);
        await data.CopyToAsync(stream);
        stream.Close();

        return filePath;
    }
}
