using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PT_CSharp_1
{
    static class Extension
    {
        public static DateTime FindOldestFile(this DirectoryInfo dir, DateTime? oldest = null)
        {
            if (oldest == null || oldest < dir.LastWriteTime)
                oldest = dir.LastWriteTime;
            
            foreach (DirectoryInfo d in dir.GetDirectories())
            {
                DateTime date = d.FindOldestFile();
                if (date < oldest)
                    oldest = date;
            }

            foreach (FileSystemInfo info in dir.GetFileSystemInfos())
                if (info.LastWriteTime < oldest)
                    oldest = info.LastWriteTime;

            return (DateTime)oldest;
        }

        public static string Attributes(this FileSystemInfo file)
        {
            string r = (file.Attributes & FileAttributes.ReadOnly) > 0 ? "r" : "-";
            string a = (file.Attributes & FileAttributes.Archive) > 0 ? "a" : "-";
            string h = (file.Attributes & FileAttributes.Hidden) > 0 ? "h" : "-";
            string s = (file.Attributes & FileAttributes.System) > 0 ? "s" : "-";
            return r + a + h + s;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(args[0]);

            DirectoryInfo root = new DirectoryInfo(args[0]);
            ShowDir(root, 2);

            Console.WriteLine("Najstarszy plik: {0}", root.FindOldestFile());

            SortedDictionary<string, long> dictionary = new SortedDictionary<string, long>(new StringComparator());
            foreach (DirectoryInfo dir in root.GetDirectories())
                dictionary.Add(dir.Name, dir.GetFileSystemInfos().Length);

            foreach (FileInfo file in root.GetFiles())
                dictionary.Add(file.Name, file.Length);

            foreach (var entry in dictionary)
                Console.WriteLine("{0} -> {1}", entry.Key, entry.Value);
        }

        static void ShowDir(DirectoryInfo directory, int maxDepth = 0, int level = 0)
        {
            if (level == 0)
            {
                Console.WriteLine("Root: {0} ({1}) {2}",
                    directory.Name,
                    directory.GetFileSystemInfos().Length,
                    directory.Attributes());
            }

            level++;

            foreach (DirectoryInfo dir in directory.GetDirectories())
            {
                Console.WriteLine("{0}{1} ({2}) {3}",
                    new string('\t', level),
                    dir.Name,
                    dir.GetFileSystemInfos().Length,
                    dir.Attributes());

                if(maxDepth > level)
                    ShowDir(dir, maxDepth, level);
            }

            foreach (FileInfo file in directory.GetFiles())
            {
                Console.WriteLine("{0}{1} {2} B {3}",
                    new string('\t', level),
                    file.Name,
                    file.Length / 8,
                    file.Attributes());
            }
        }

        private class StringComparator : IComparer<string> {
            public int Compare(string x, string y)
            {
                if (x.Length != y.Length)
                    return x.Length - y.Length;
                return x.CompareTo(y);
            }
        }
    }
}
