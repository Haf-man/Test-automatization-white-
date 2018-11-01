using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace ConfigurationReader
{
    /// <summary>
    /// Read and write configuration files from and into XML documents. 
    /// </summary>
    static public class ConfigurationReaderWriter
    {
        /// <summary>
        /// Read configuration from XML file
        /// </summary>
        /// <param name="path">The path to configuration file</param>
        static public Configuration ReadConfiguration(string path)
        {
            Configuration configuration;

            XmlSerializer xmlSerializer = new XmlSerializer(typeof (Configuration));

            using (FileStream fs = new FileStream(path, FileMode.Open))
            using (XmlReader xmlReader = XmlReader.Create(fs))
            {
                configuration = (Configuration)xmlSerializer.Deserialize(xmlReader);
            }

            return configuration;
        }

        /// <summary>
        /// Write configuration file to XML file
        /// </summary>
        /// <param name="path">The path to an existiting (or not) xml file</param>
        /// <exception cref="System.ArgumentNullException">Thrown when configuration = null</exception>
        static public void WriteConfiguration(string path, Configuration configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException("configuration");

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Configuration));
            XmlWriterSettings settings = new XmlWriterSettings { Indent = true, NewLineHandling = NewLineHandling.Entitize };

            using (FileStream fs = new FileStream(path, FileMode.Create))
            using (XmlWriter xmlWriter = XmlWriter.Create(fs, settings))
            {
                xmlSerializer.Serialize(xmlWriter, configuration);
            }
        }
    }
}
