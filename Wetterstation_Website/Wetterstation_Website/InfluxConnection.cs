using System;
using System.Linq;
using System.Threading.Tasks;
using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Core;
using InfluxDB.Client.Writes;
using Wetterstation_Website.JSON_builder;

namespace Wetterstation_Website
{
    public class InfluxConnection
    {
        public InfluxDBClient Client { get; private set; }
        public DatabaseInfos infos { get; private set;  }
        public InfluxConnection() 
        {
            infos = DatabaseInfos.FromJsonFile("./DB_Connection.json");

            string token = TokenBuilder.FromJson("./API_Token.json");

            Client = new InfluxDBClient(infos.IP_Address,token);
        }
    }
}
