using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wetterstation_Website.JSON_builder
{
    public class DatabaseInfos
    {
        public string IP_Address { get; set; }
        public string Organisation { get; set; }
        public string Bucket { get; set; }

        internal static void CreateExampleJson(string path)
        {
            DatabaseInfos infos = new DatabaseInfos();
            infos.Bucket = "home";
            infos.IP_Address = "http://localhost:8086";
            infos.Organisation = "docs";

            FileStream file = File.Create(path);
            file.Close();

            string json_content = JsonConvert.SerializeObject(infos,Formatting.Indented);

            File.WriteAllText(path,json_content);
        }

        internal static DatabaseInfos FromJsonFile(string path)
        {
            string file_content = File.ReadAllText(path);
            DatabaseInfos infos = JsonConvert.DeserializeObject<DatabaseInfos>(file_content);
            return infos;
        }
    }
}
