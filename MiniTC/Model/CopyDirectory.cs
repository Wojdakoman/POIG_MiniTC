using System.IO;
using System.Linq;

namespace MiniTC.Model
{
    /// <summary>
    /// Manager for copying directories
    /// </summary>
    class CopyDirectory
    {
        public bool DirectoryExists(string file, string path)
        {
            return Directory.Exists($"{path}\\{GetDirectoryName(file)}");
        }

        public void Copy(string file, string destination, bool overrid)
        {
            if (overrid)
            {
                Directory.Delete($"{destination}\\{GetDirectoryName(file)}", true);
            }
            Directory.CreateDirectory($"{destination}\\{GetDirectoryName(file)}");
            CopyMethod(file, $"{destination}\\{GetDirectoryName(file)}");
        }
        /// <summary>
        /// Copy directory with new name
        /// </summary>
        public void CopyWithName(string file, string destination)
        {
            string newName = FindName(file, destination);
            Directory.CreateDirectory($"{destination}\\{newName}");
            CopyMethod(file, $"{destination}\\{newName}");
        }
        /// <summary>
        /// Find first available new name
        /// </summary>
        private string FindName(string file, string destination)
        {
            string name = GetDirectoryName(file);
            int n = 2;
            while (Directory.Exists($"{destination}\\{name} ({n})")) { n++; }

            return $"{name} ({n})";
        }
        /// <summary>
        /// Recursive method to copy directory content
        /// </summary>
        private void CopyMethod(string source, string destination)
        {
            foreach(var d in Directory.GetDirectories(source))
            {
                Directory.CreateDirectory($"{destination}\\{GetDirectoryName(d)}");
                CopyMethod($"{d}", $"{destination}\\{GetDirectoryName(d)}");
            }
            foreach (var f in Directory.GetFiles(source))
            {
                File.Copy($"{f}", $"{destination}\\{Path.GetFileName(f)}");
            }
        }

        private string GetDirectoryName(string path)
        {
            if (path.Last() == '\\') path = path.Substring(0, path.Length - 1);
            string[] temp = path.Split('\\');
            return temp.Last();
        }
    }
}
