using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectoryToFile
{
    class Program
    {
        static void Main(string[] args)
        {
            string dir = args[0];
            string outputFile = args[1];

            var relevantDirs = GetRelevantSubDirectoriesRecursively(dir, new[] { "node_modules", ".meteor", ".idea" });
            var files = new List<string>();

            foreach (var subdir in relevantDirs)
            {
                IEnumerable<string> htmlFiles = Directory.GetFiles(subdir, "*.html");
                IEnumerable<string> jsFiles = Directory.GetFiles(subdir, "*.js");
                IEnumerable<string> cssFiles = Directory.GetFiles(subdir, "*.css");

                jsFiles = jsFiles.Where(x => !x.EndsWith("min.js"));

                files.AddRange(htmlFiles);
                files.AddRange(jsFiles);
                files.AddRange(cssFiles);
            }

            using (StreamWriter sw = new StreamWriter(outputFile, false, new UTF8Encoding(false)))
            {
                foreach (var file in files)
                {
                    sw.WriteLine("@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");
                    sw.WriteLine(file.Replace(dir, ""));
                    sw.WriteLine("@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");
                    sw.WriteLine();
                    var lines = File.ReadAllLines(file, Encoding.Default);

                    for (int i = 1; i <= lines.Length; i++)
                    {
                        sw.WriteLine($"{i}\t{lines[i - 1]}");
                    }
                    //sw.Write(File.ReadAllText(file));

                    sw.WriteLine();
                }
            }
        }

        static List<string> GetRelevantSubDirectoriesRecursively(string dir, IReadOnlyCollection<string> dirsToExclude)
        {
            Stack<string> dirsStack = new Stack<string>();
            List<string> subdirs = new List<string>();

            dirsStack.Push(dir);

            while (dirsStack.Any())
            {
                var subdir = dirsStack.Pop();
                subdirs.Add(subdir);

                foreach (var subsubdir in GetRelevantSubDirectories(subdir, dirsToExclude))
                {
                    dirsStack.Push(subsubdir);
                }
            }

            return subdirs;
        }

        static List<string> GetRelevantSubDirectories(string dir, IReadOnlyCollection<string> dirsToExclude)
        {
            var dirs = new List<string>();

            foreach (var subdir in Directory.GetDirectories(dir))
            {
                var dirinfo = new DirectoryInfo(subdir);

                if (!dirsToExclude.Contains(dirinfo.Name))
                {
                    dirs.Add(subdir);
                }
            }

            return dirs;
        }
    }
}
