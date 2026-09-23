using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;
using System;
using System.Diagnostics;
using System.IO.Ports;

namespace SerialDatabaseInterface
{
    // Requires 2 JSON Files in Projekt directory 
    // > API_Token.json >> class: InfluxTokenTemplate
    // > COM.json >> class: ArduinoConfigTemplate
    internal class Program
    {
        private static DataParser dataParser;
        private static InfluxDBClient client;
        private static WriteApi writeApi;
        private static DatabaseInfos databaseInfos;
        static void Main(string[] args)
        {
            SerialPort port = PortBuilder.BuildFromJson("./COM.json");

            databaseInfos = DatabaseInfos.FromJsonFile("./DB_Connection.json");

            var token = TokenBuilder.FromJson("./API_Token.json");

            client = new InfluxDBClient(databaseInfos.IP_Address, token);
            writeApi = client.GetWriteApi();


            port.DataReceived += Port_DataReceived;
            port.Open();

            dataParser = new DataParser();
            dataParser.AddAttribute("Temperature");
            dataParser.AddAttribute("Humidity");
            dataParser.AddAttribute("IAQ_Index");
            dataParser.AddAttribute("UV_Index");
            dataParser.AddAttribute("QNE_Pressure");

            bool exit = false;
            Console.WriteLine("Application running. \nPress ^C to exit...");

            while(!exit)
            {
                ConsoleKeyInfo key = Console.ReadKey();
                if(key.Modifiers == ConsoleModifiers.Control 
                    && key.Key == ConsoleKey.C)
                {
                    exit = true;
                }
            }
            port.Close();
        }

        private static void Port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if(dataParser.StageMessage(sender))
            {
                foreach(KeyValuePair<string,float> attr in dataParser.Results)
                {
                    Console.WriteLine($"{attr.Key}: \t {attr.Value}");
                    var point = PointData
                        .Measurement(attr.Key)
                        .Field(attr.Key, attr.Value)
                        .Timestamp(DateTime.UtcNow, InfluxDB.Client.Api.Domain.WritePrecision.Ns);

                    writeApi.WritePoint(point, databaseInfos.Bucket, databaseInfos.Organisation);
                }
            }
        }
    }
}
