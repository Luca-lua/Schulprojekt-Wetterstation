using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wetterstation_Website.JSON_builder
{
    internal class TokenBuilder
    {
        internal static void CreateExampleJson(string path)
        {
            InfluxTokenTemplate template = new InfluxTokenTemplate();
            template.API_token = "Insert Token Here";

            string content = JsonConvert.SerializeObject(template,Formatting.Indented);

            FileStream stream = File.Create(path);
            stream.Close();

            File.WriteAllText(path, content);
        }
        internal static string FromJson(string path)
        {
            string content = File.ReadAllText(path);

            InfluxTokenTemplate token_object = JsonConvert.DeserializeObject<InfluxTokenTemplate>(content);

            return token_object.API_token;
        }
    }

    public class InfluxTokenTemplate()
    {
        public string API_token { get; set; }
    }
}
