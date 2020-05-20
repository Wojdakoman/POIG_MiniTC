using System;
using System.IO;
using System.Text;

namespace MiniTC.Model
{
    class CopyManager
    {
        public enum ElementType
        {
            File, Directory, MoveUp
        }
        public ElementType GetElementType(string file)
        {
            if (file.Contains("<D>")) return ElementType.Directory;
            else if (file.Contains("...")) return ElementType.MoveUp;
            else return ElementType.File;
        }
        /// <summary>
        /// Removes "<D>" from path
        /// </summary>
        public string ClearName(string path)
        {
            StringBuilder newPath = new StringBuilder();
            string[] temp = path.Split('\\');

            for (int i = 0; i < temp.Length; i++)
            {
                string element = temp[i].Replace("<D>", "");
                element = element.Trim();
                newPath.Append($"{element}\\");
            }
            //newPath = newPath.ToString();
            if (temp.Length > 2)
                return newPath.ToString().Substring(0, newPath.ToString().Length - 1);
            else return newPath.ToString();
        }
        /// <summary>
        /// Check if command can be executed
        /// </summary>
        /// <param name="LPidx">Selected index on left panel</param>
        /// <param name="RPidx">Selected index on right panel</param>
        /// <param name="LPpath">Current path of left panel</param>
        /// <param name="RPpath">Current path of right panel</param>
        /// <param name="LPitems">Items list in left panel</param>
        /// <param name="RPitems">Items list in right panel</param>
        public bool GetCommandCondition(int LPidx, int RPidx, string LPpath, string RPpath, string[] LPitems, string[] RPitems)
        {
            if (LPidx > -1)
                if (LPitems[LPidx] == "..") return false;
            if (RPidx > -1)
                if (RPitems[RPidx] == "..") return false;
            return ((LPidx != -1 && RPidx == -1) || (LPidx == -1 && RPidx != -1)) && LPpath != RPpath;
        }
        /// <summary>
        /// Checks if user has permissions to copy file or directory
        /// </summary>
        public bool HasAccess(string file)
        {
            try
            {
                //Directory.GetFiles(_path);

                if (File.Exists(file))
                {
                    File.ReadAllText(file);
                }
                if (Directory.Exists(file))
                {
                    Directory.GetFiles(file);
                }
            }
            catch (UnauthorizedAccessException e)
            {
                return false;
            }
            return true;
        }
    }
}
