using System.IO;
using System.Linq;

namespace MiniTC.Model
{
    /// <summary>
    /// Manager for copying files
    /// </summary>
    class CopyFile
    {
        public bool FileExists(string file, string path)
        {
            string fileName = Path.GetFileName(file);
            return File.Exists($"{path}\\{fileName}");
        }

        public void Copy(string file, string destination, bool overrid)
        {
            string fileName = Path.GetFileName(file);
            if (destination.Last() != '\\') fileName = $"\\{fileName}";
            File.Copy(file, $"{destination}{fileName}", overrid);
        }
        /// <summary>
        /// Copy file but paste with new name: "oldName (number).extension"
        /// </summary>
        public void CopyWithName(string file, string destination)
        {
            string newName = FindName(file, destination);
            if (destination.Last() != '\\') newName = $"\\{newName}";
            File.Copy(file, $"{destination}{newName}", false);
        }
        /// <summary>
        /// Find first available new name
        /// </summary>
        private string FindName(string file, string destination)
        {
            string fileName = Path.GetFileNameWithoutExtension(file);
            string fileExt = Path.GetExtension(file);
            int n = 2;
            while (File.Exists($"{destination}\\{fileName} ({n}){fileExt}")) { n++; }

            return $"{fileName} ({n}){fileExt}";
        }
    }
}
