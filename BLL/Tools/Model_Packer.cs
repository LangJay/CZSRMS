using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Data;
namespace BLL.Tools
{
    using System.IO;
    [XmlRoot("PackDocument")]
    public class PackDocument
    {
        [XmlArray]
        public List<PackFile> PackFiles;
    }

    [XmlType]
    public class PackFile
    {
        [XmlAttribute("ProjectName")]
        public string ProjectName { get; set; }
        [XmlAttribute("CoodinateSystem")]
        public string CoordSystem { get; set; }
        [XmlAttribute("CenterMeridian")]
        public string CenterLong { get; set; }
        [XmlAttribute("Yoffset")]
        public double OffsetN { get; set; }
        [XmlAttribute("Xoffset")]
        public double OffsetE { get; set; }
        [XmlElement("FinishtimeInfo")]
        public DateTime FinishtimeInfo { get; set; }
        [XmlAttribute("FileType")]
        public string FileType { get; set; }
        [XmlAttribute("ProjectType")]
        public string ProjectType { get; set; }
        [XmlAttribute("SurveyingUnitName")]
        public string SurveyingUnitName { get; set; }
        [XmlAttribute("FinishPerson")]
        public string FinishPerson { get; set; }
        [XmlAttribute("Explain")]
        public string Explain { get; set; }
    }

    public static class MyXmlSerializer
    {
        public static void SaveToXml(string filePath, object sourceObj)
        {
            if (filePath.Trim() == "")
                return;
            string tmpFile = Path.GetTempFileName();
            if (sourceObj != null)
            {
                var type = sourceObj.GetType();
                using (StreamWriter writer = new StreamWriter(tmpFile))
                {
                    System.Xml.Serialization.XmlSerializer xmlSerializer = new System.Xml.Serialization.XmlSerializer(type);
                    xmlSerializer.Serialize(writer, sourceObj);
                }
            }
            File.Copy(tmpFile, filePath, true);
            File.Delete(tmpFile);
        }
        public static object LoadFromXml(string filePath, Type type)
        {
            object result = null;

            if (File.Exists(filePath))
            {
                using (StreamReader reader = new StreamReader(filePath, Encoding.UTF8))
                {
                    System.Xml.Serialization.XmlSerializer xmlSerializer = new System.Xml.Serialization.XmlSerializer(type);
                    result = xmlSerializer.Deserialize(reader);
                }
            }
            return result;
        }
    }
}
