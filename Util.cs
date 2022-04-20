using System;
using System.IO;
namespace LuqinMiniAppBase
{
    public class Util
    {
        public static string workingPath = $"{Environment.CurrentDirectory}";

        public static string GetDbConStr(string fileName)
        {
            string conStr = "";

            string filePath = workingPath + fileName;

            using (StreamReader sr = new StreamReader(filePath, true))
            {
                conStr = sr.ReadToEnd();
                sr.Close();
            }
            return conStr;
        }
    }
}
