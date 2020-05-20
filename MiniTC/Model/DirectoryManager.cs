using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MiniTC.Model
{
    /// <summary>
    /// Main class providing informations about file system
    /// </summary>
    class DirectoryManager
    {
        public string[] AvailableDrives { get => Directory.GetLogicalDrives(); }
        public string CurrentPath { get => _path; }

        private string _path;
        private string _oldPath; //where to return if user dont have access to directory

        public DirectoryManager()
        {
            _path = AvailableDrives[0];
        }

        /// <summary>
        /// Method is used when selecting directory from the list. Adds new path to current. If force = true, current path = path
        /// </summary>
        public void SetPath(string path)
        {
            SetPath(path, false);
        }
        public void SetPath(string path, bool force)
        {
            _oldPath = _path;
            if (force)
            {
                _path = path;
            }
            else
            {
                if (path != "..")
                    if (_path.Last() != '\\')
                        _path = $"{_path}\\{path}";
                    else _path = $"{_path}{path}";
                else
                {
                    _path = RemoveLastSlash(_path);
                }
            }
        }
        /// <summary>
        /// Method is used when user enter path manually
        /// </summary>
        public int UpdatePath(string path)
        {
            if (string.IsNullOrEmpty(path)) return -1;
            if (path.Last() == '\\')
                path = RemoveLastSlash(path);

            if (!Directory.Exists(path)) return 1;
            try
            {
                Directory.GetFiles(path);
            }
            catch (UnauthorizedAccessException e)
            {
                return 2;
            }
            _path = path;
            return 0;
        }
        /// <summary>
        /// Checks if current path still exists (ex. unplugging pendrive). If not, reset current path
        /// </summary>
        public bool HasConnection()
        {
            if (Directory.Exists(_path)) return true;
            else
            {
                _path = AvailableDrives[0];
                return false;
            }
        }

        public string[] GetDirectoryElements()
        {
            List<string> drives = new List<string>();
            if (Directory.GetDirectoryRoot(_path) != _path) drives.Add("..");
            foreach (var d in Directory.GetDirectories(_path))
                drives.Add($"<D> {d.Split('\\').Last()}");

            foreach(var f in Directory.GetFiles(_path))
                drives.Add($"{f.Split('\\').Last()}");
            return drives.ToArray();
        }
        /// <summary>
        /// Method delete "<D>" from directory name
        /// </summary>
        public string ClearName(string name)
        {
            name = name.Replace("<D>", "");
            name = name.Trim();
            return name;
        }
        /// <summary>
        /// Check if user has access to directory
        /// </summary>
        public bool HasAccess()
        {
            try
            {
                //Directory.GetFiles(_path);

                if (File.Exists(_path))
                {
                    File.ReadAllText(_path);
                }
                if (Directory.Exists(_path))
                {
                    Directory.GetFiles(_path);
                }
            } catch(UnauthorizedAccessException e)
            {
                _path = _oldPath;
                return false;
            }
            return true;
        }
        /// <summary>
        /// get drive index. Used when user enter path manually
        /// </summary>
        public int GetNewDriveIndex()
        {
            string drive = Directory.GetDirectoryRoot(_path);
            for (int i = 0; i < AvailableDrives.Length; i++)
            {
                if (AvailableDrives[i] == drive) return i;
            }
            return -1;
        }

        //HELPERS
        private string RemoveLastSlash(string path)
        {
            string[] temp = path.Split('\\');
            string newPath = "";
            for (int i = 0; i < temp.Length - 1; i++)
            {
                newPath += $"{temp[i]}\\";
            }
            if (temp.Length > 2)
                newPath = newPath.Substring(0, newPath.Length - 1);
            return newPath;
        }
    }
}
