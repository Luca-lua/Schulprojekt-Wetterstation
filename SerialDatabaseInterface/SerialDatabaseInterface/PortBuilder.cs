using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;
using Newtonsoft.Json;

namespace SerialDatabaseInterface
{
    internal class PortBuilder
    {
        internal static SerialPort BuildFromJson(string path)
        {
            string content = File.ReadAllText(path);

            ArduinoConfigTemplate template = JsonConvert.DeserializeObject<ArduinoConfigTemplate>(content);

            return new SerialPort($"COM{template.COM_port}",template.baudrate);
        }
    }

    public class ArduinoConfigTemplate()
    {
        public int COM_port {  get; set; }
        public int baudrate { get; set; }
    }
}
