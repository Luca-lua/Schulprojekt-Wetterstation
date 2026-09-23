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

        internal Dictionary<string,float> Results = new Dictionary<string, float>();
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
            Results = new Dictionary<string,float>();

            string[] lines = message.Split('\n');
            
            foreach (string line in lines)
            {
                if (attributes.FirstOrDefault(attr => line.Contains(attr)) != null)
                {
                    // Create or overwrite (2nd dataset has priority)
                    if (!Results.ContainsKey(attributes.First(attr => line.Contains(attr))))
                    {
                        Results.Add(attributes.First(attr => line.Contains(attr)), float.Parse(line.Split(":")[1].Replace(".",",")));
                    }
                    else
                    {
                        Results[attributes.First(attr => line.Contains(attr))] = float.Parse(line.Split(":")[1].Replace(".", ","));
                    }
                }
            }
        }
    }
}
