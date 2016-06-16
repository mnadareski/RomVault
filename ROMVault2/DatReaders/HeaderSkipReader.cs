using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;

namespace ROMVault2.DatReaders
{
    public static class HeaderSkipReader
    {
        public static Dictionary<string, int> SkipMapping = new Dictionary<string, int>();

        public static void PopulateSkipMapping()
        {
            foreach (string headerName in Directory.GetFiles("Headers", "*", SearchOption.TopDirectoryOnly))
            {
                // Check if the header file exists
                if (!File.Exists(Path.Combine("Headers", headerName)))
                {
                    return;
                }

                // Read in remapping from file
                XmlDocument doc = new XmlDocument();
                try
                {
                    doc.LoadXml(File.ReadAllText(Path.Combine("Headers", headerName)));
                }
                catch (XmlException ex)
                {
#if Debug
                Console.WriteLine("Header skippers from '" + headerName + "' could not be loaded! " + ex.ToString());
#endif
                    return;
                }

                // Get the detector parent node
                XmlNode node = doc.FirstChild;
                while (node.Name != "detector")
                {
                    node = node.NextSibling;
                }

                // Get the first rule node
                node = node.SelectSingleNode("rule");

                // Now read in the rules
                while (node != null && node.Name == "rule")
                {
                    // Size is the offset for the actual game data
                    int size = (node.Attributes["start_offset"] != null ? Convert.ToInt32(node.Attributes["start_offset"].Value, 16) : 0);

                    // Each rule set can have more than one data rule. We can't really use multiples right now
                    if (node.SelectNodes("data") != null)
                    {
                        foreach (XmlNode child in node.SelectNodes("data"))
                        {
                            // Add an offset to the match if one exists
                            string header = (child.Attributes["offset"] != null && child.Attributes["offset"].Value != "0" ? "^.{" + (Convert.ToInt32(child.Attributes["offset"].Value, 16) * 2) + "}" : "^");
                            header += child.Attributes["value"].Value;

                            // Now add the header and value to the appropriate skipper dictionary
                            SkipMapping.Add(header, size);
                        }
                    }

                    // Get the next node and skip over anything that's not an element
                    node = node.NextSibling;

                    if (node == null)
                    {
                        break;
                    }

                    while (node.NodeType != XmlNodeType.Element && node.Name != "rule")
                    {
                        node = node.NextSibling;
                    }
                }
            }
        }

        public static int GetHeaderSize(string file)
        {
            int size = 0;

            if (SkipMapping.Count == 0)
            {
                PopulateSkipMapping();
                if (SkipMapping.Count == 0)
                    return size;
            }

            // Extract the first 1024 bytes of the file
            BinaryReader br = null;
            string header = "";
            try
            {
                br = new BinaryReader(File.OpenRead(file));
                byte[] hbin = br.ReadBytes(1024);
                header = BitConverter.ToString(hbin).Replace("-", string.Empty);
                br?.Close();
            }
            catch
            {
                br?.Close();
                return size;
            }

            // Loop over the dictionary and see if there are matches
            foreach (KeyValuePair<string, int> entry in SkipMapping)
            {
                if (Regex.IsMatch(header, entry.Key))
                {
                    size = entry.Value;
                    break;
                }
            }

            return size;
        }
    }
}
