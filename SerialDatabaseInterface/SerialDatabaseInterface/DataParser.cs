using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Reflection;
using System.Text;

namespace SerialDatabaseInterface
{
    internal class DataParser
    {
        private List<string> attributes = new List<string>();
        private string message = string.Empty;

        internal Dictionary<string,string> Results = new Dictionary<string,string>();

        internal void AddAttribute(string attr)
        {
            attributes.Add(attr);
        }

        internal bool StageMessage(object sender)
        {
            SerialPort sp = (SerialPort)sender;
            string indata = sp.ReadExisting();
            if (indata.Contains("?"))
            {
                message += indata;
                ParseMessage(message);
                message = string.Empty;
                return true;
            }
            else
            {
                message += indata;
                return false;
            }
        }

        private void ParseMessage(string message)
        {
            Results = new Dictionary<string,string>();

            string[] lines = message.Split('\n');
            
            foreach (string line in lines)
            {
                if (attributes.FirstOrDefault(attr => line.Contains(attr)) != null)
                {
                    if (!Results.ContainsKey(attributes.First(attr => line.Contains(attr))))
                    {
                        Results.Add(attributes.First(attr => line.Contains(attr)), line.Split(":")[1]);
                    }
                }
            }
        }
    }
}
