using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArvidsonFoto.Data
{
    public class AppLogReader
    {
        string folderDataPath = @"..\..\logs\";

        public List<string> ReadData(string appLogFile)
        {
            string inputDataPath = folderDataPath + appLogFile;
            List<string> returnList = new List<string>();
            using (StreamReader reader = File.OpenText(inputDataPath))
            {
                string line = "";
                while ((line = reader.ReadLine()) != null)
                {
                    returnList.Add(line);
                }
            }
            return returnList;
        }
    }
}
