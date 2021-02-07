using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Serilog;

namespace ArvidsonFoto.Data
{
    public class AppLogReader
    {
        string folderDataPath = @".\logs\"; //Pekar på mappen där loggfilen ligger

        /// <summary>
        /// En funktion som läser en loggfil. Tar ett filnamn som parameter.
        /// </summary>
        /// <param name="appLogFile">Filnamnet på filen. T.ex: "appLog20210127.txt"</param>
        /// <returns>En lista med strängar, där varje index-rad är en rad i filen som skickats in som parameter.</returns>
        public List<string> ReadData(string appLogFile)
        {
            string inputDataPath = folderDataPath + appLogFile; //Sätter sökvägen till filen.
            List<string> dataList = new List<string>();
            List<string> returnList = new List<string>();
            try { //Försöker öppna filen och läsa den...
                using (StreamReader reader = File.OpenText(inputDataPath))//Läser filen, eller ger null om ingen fil finns
                {
                    string line = "";
                    while ((line = reader.ReadLine()) != null) //Loopar igenom alla rader (line) i filen
                    {
                        dataList.Add(line);
                    }
                }
            }
            catch (Exception ex)
            {
                dataList.Add(ex.Message);
            }
            
            for (int i = dataList.Count; i > 0; i--)
            {
                returnList.Add(dataList[i-1]);
            }

            
            return returnList;
        }

        public List<string> ExistingLogFiles()
        {
            List<string> returnList = new List<string>();

            DirectoryInfo di = new DirectoryInfo(folderDataPath);
            var files = di.GetFiles("*.txt");
            foreach (var fileInfo in files)
            {
                returnList.Add(fileInfo.Name);
            }

            return returnList;
        }
    }
}
