using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SerialDatabaseInterface
{
    internal class TokenBuilder
    {
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
