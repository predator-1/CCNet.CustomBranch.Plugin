using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CCNet.CustomBranch.Plugin
{
    [Serializable]
    public class CustomMessage
    {
        public string Command { get; set; }

        public string Branch { get; set; }

        public string SourcecontrolType { get; set; }

        public string Repo { get; set; }

        public string WorkingDirectory { get; set; }

        public override string ToString()
        {
            return SerializeObject(this);
        }

        public static string SerializeObject<T>(T toSerialize)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(toSerialize.GetType());

            using (StringWriter textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, toSerialize);
                return textWriter.ToString();
            }
        }
    }
}
