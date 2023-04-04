using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Tools
{
    public static class FileHelper
    {
        public static string CurrentPath
        {
            get
            {
                var dir = AppDomain.CurrentDomain.BaseDirectory;
                return dir;
            }
        }

        /// <summary>
        /// 删除整个文件夹
        /// </summary>
        /// <param name="srcPath"></param>
        public static void DelectDir(string srcPath, bool isDelFile = false)
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(srcPath);
                FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();  //返回目录中所有文件和子目录
                foreach (FileSystemInfo i in fileinfo)
                {
                    try
                    {
                        if (i is DirectoryInfo)            //判断是否文件夹
                        {
                            DirectoryInfo subdir = new DirectoryInfo(i.FullName);
                            subdir.Delete(true);          //删除子目录和文件
                        }
                        else if (isDelFile)
                        {
                            File.Delete(i.FullName);      //删除指定文件
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        /// <summary>
        /// 获取文件夹下的所有文件(包括子目录)
        /// </summary>
        /// <param name="path">文件夹路径</param>
        /// <returns>所有文件的路径</returns>
        public static List<string> GetFiles(string path)
        {
            var result = new List<string>();
            DirectoryInfo dir = new DirectoryInfo(path);
            FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();
            foreach (FileSystemInfo i in fileinfo)
            {
                try
                {
                    if (i is DirectoryInfo)            //判断是否文件夹
                    {
                        var temp = i as DirectoryInfo;
                        result.AddRange(GetFiles(temp.FullName));
                    }
                    else if (i is FileInfo)
                    {
                        var temp = i as FileInfo;
                        result.Add(temp.FullName);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return result;
        }

        /// <summary>
        /// 获取文件夹下的所有文件(不包括子目录)
        /// </summary>
        /// <param name="path">文件夹路径</param>
        /// <returns>所有文件的路径</returns>
        public static List<string> GetCurrentDirFiles(string path)
        {
            var result = new List<string>();
            DirectoryInfo dir = new DirectoryInfo(path);
            FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();
            foreach (FileSystemInfo i in fileinfo)
            {
                try
                {
                    if (i is FileInfo)
                    {
                        var temp = i as FileInfo;
                        result.Add(temp.FullName);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return result;
        }

        public static string CombineCurrentPath(string fileName)
        {
            return Path.Combine(CurrentPath, fileName);
        }

    }
}
