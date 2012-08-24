using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace AppXplorer.Model
{
    class AppInfo
    {
        public string Name { get; set; }
        public List<string> Formats { get; set; }
        public List<string> FileTypes { get; set; }
        public List<string> Protocols { get; set; }
        public List<string> FileTypeAssociations { get; set; }

        public bool SupportsAllFileTypes { get; set; }

        public AppInfo(string name)
        {
            Name = name;
            Formats = new List<string>();
            FileTypes = new List<string>();
            Protocols = new List<string>();
            FileTypeAssociations = new List<string>();
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            if (Formats.Count != 0)
            {
                sb.Append("SHARE TARGET CONTRACT - SUPPORTED DATA FORMATS:\n");
                foreach (var format in Formats)
                {
                    sb.AppendFormat("{0}\n", format);
                }
            }

            if (FileTypes.Count != 0 || SupportsAllFileTypes)
            {
                sb.Append("\nSHARE TARGET CONTRACT - SUPPORTED FILE TYPES:\n");
                foreach (var fileType in FileTypes)
                {
                    sb.AppendFormat("{0}\n", fileType);
                }

                if (SupportsAllFileTypes)
                    sb.Append("Supports any file type\n");
            }

            if (Protocols.Count != 0)
            {
                sb.Append("\nPROTOCOLS:\n");
                foreach (var protocol in Protocols)
                {
                    sb.AppendFormat("{0}\n", protocol);
                }
            }
            if (FileTypeAssociations.Count != 0)
            {
                sb.Append("\nFILE TYPE ASSOCIATIONS:\n");
                foreach (var fileType in FileTypeAssociations)
                {
                    sb.AppendFormat("{0}\n", fileType);
                }
            }

            return sb.ToString();
        }

        public static IList<AppInfo> ParseManifest(string manifestPath)
        {
            var appInfoList = new List<AppInfo>();

            var doc = new XmlDocument();
            var ns = new XmlNamespaceManager(doc.NameTable);
            ns.AddNamespace("m", "http://schemas.microsoft.com/appx/2010/manifest");
            doc.Load(manifestPath);

            var node = doc.DocumentElement.SelectSingleNode("m:Properties/m:Framework", ns);
            if (node != null && node.InnerText == "true")
                return appInfoList;

            node = doc.DocumentElement.SelectSingleNode("m:Identity", ns);
            var packageName = node.Attributes.GetNamedItem("Name").InnerText;
            var packageVer = node.Attributes.GetNamedItem("Version").InnerText;

            var appNodes = doc.DocumentElement.SelectNodes("descendant::m:Application", ns);
            foreach (XmlNode appNode in appNodes)
            {
                var appName = appNode.Attributes.GetNamedItem("Id").InnerText;

                var appInfo = new AppInfo(packageName + " - " + appName + " " + packageVer);
                var nodes = appNode.SelectNodes("descendant::m:DataFormat", ns);
                foreach (XmlNode dataNode in nodes)
                {
                    appInfo.Formats.Add(dataNode.InnerText);
                }

                nodes = appNode.SelectNodes("descendant::m:ShareTarget/m:SupportedFileTypes/m:FileType", ns);
                foreach (XmlNode dataNode in nodes)
                {
                    appInfo.FileTypes.Add(dataNode.InnerText);
                }

                nodes = appNode.SelectNodes("descendant::m:ShareTarget/m:SupportedFileTypes/m:SupportsAnyFileType", ns);
                appInfo.SupportsAllFileTypes = nodes.Count != 0;

                nodes = appNode.SelectNodes("descendant::m:Protocol", ns);
                foreach (XmlNode dataNode in nodes)
                {
                    appInfo.Protocols.Add(dataNode.Attributes.GetNamedItem("Name").InnerText);
                }

                nodes = appNode.SelectNodes("descendant::m:FileTypeAssociation/m:SupportedFileTypes/m:FileType", ns);
                foreach (XmlNode dataNode in nodes)
                {
                    appInfo.FileTypeAssociations.Add(dataNode.InnerText);
                }

                appInfoList.Add(appInfo);
            }

            return appInfoList;
        }

    }
}
