using InfluxDB.Client;
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
        static void Main(string[] args)
        {
            SerialPort port = PortBuilder.BuildFromJson("./COM.json");

            port.DataReceived += Port_DataReceived;
            port.Open();

            dataParser = new DataParser();
            dataParser.AddAttribute("Temperature");
            dataParser.AddAttribute("Humidity");
            dataParser.AddAttribute("IAQ_Index");
            dataParser.AddAttribute("UV_Sensor");
            dataParser.AddAttribute("QNE_Pressure");

            var token = TokenBuilder.FromJson("./API_Token.json");
            const string bucket = "Wetterstation_sensordata";
            const string org = "docs";

            client = new InfluxDBClient("http://localhost:8086", token);
            writeApi = client.GetWriteApi();

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
                foreach(KeyValuePair<string,string> attr in dataParser.Results)
                {
                    Console.WriteLine($"{attr.Key}: \t {attr.Value}");
                    var point = PointData
                        .Measurement(attr.Key)
                        .Field(attr.Key, attr.Value)
                        .Timestamp(DateTime.UtcNow, InfluxDB.Client.Api.Domain.WritePrecision.Ns);

                    writeApi.WritePoint(point, "Wetterstation_sensordata","docs");
                }
            }
        }
    }
}
