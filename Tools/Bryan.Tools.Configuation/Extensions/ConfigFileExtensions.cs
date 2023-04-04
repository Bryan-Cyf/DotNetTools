using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Linq;

public static class ConfigFileExtensions
{
    /// <summary>
    /// 添加自定义配置文件夹
    /// </summary>
    /// <param name="folderName">文件夹名称</param>
    public static void AddConfigurationFolder(this IConfigurationBuilder builder, string folderName = "Configuration")
    {
        string currentDirectory = Environment.CurrentDirectory;
        folderName = Directory.Exists($"{currentDirectory}/{folderName}") ? folderName : folderName.ToLower();
        var configFolderPath = $"{currentDirectory}/{folderName}";

        //遍历配置文件夹下所有json文件
        if (Directory.Exists(configFolderPath))
        {
            DirectoryInfo configDir = new DirectoryInfo(configFolderPath);
            var fileItems = configDir.GetFiles("*.json");
            if (fileItems != null && fileItems.Any())
            {
                foreach (var fileItem in fileItems)
                {
                    builder.AddJsonFile($"{folderName}/{fileItem.Name}", false, true);
                }
            }
        }
    }

    /// <summary>
    /// 添加自定义配置文件
    /// </summary>
    /// <param name="folderName">文件夹名称</param>
    /// <param name="fileName">文件名</param>
    public static void AddConfigurationFile(this IConfigurationBuilder builder, string folderName = null, string fileName = null)
    {
        string currentDirectory = Environment.CurrentDirectory;
        string configFilePath;
        if (!string.IsNullOrEmpty(folderName))
        {
            folderName = Directory.Exists($"{currentDirectory}/{folderName}") ? folderName : folderName.ToLower();
            configFilePath = $"{currentDirectory}/{folderName}/{fileName}";
        }
        else
        {
            configFilePath = $"{currentDirectory}/{fileName}";
        }

        if (File.Exists(configFilePath))
        {
            builder.AddJsonFile(configFilePath, false, true);
        }
    }
}
