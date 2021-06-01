using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DarkestLoadOrder.ModHelper
{
    using System.Xml.Serialization;

    using Serialization.Project;

    public class ModResolverOffline
    {
        public static async Task<byte[]> GetModThumbnail(string iconFile)
        {
            return await File.ReadAllBytesAsync(iconFile);;
        }

        public static async Task<Dictionary<ulong, ModLocalItem>> GetModInfos(string modFolderPath)
        {
            var directories = Directory.GetDirectories(modFolderPath);

            if (directories.Length == 0)
            {
                return null;
            }
            
            var resolvedMods = new Dictionary<ulong, ModLocalItem>();
            foreach (var directory in directories)
            {
                var files = Directory.GetFiles(directory).ToHashSet();

                if (!files.TryGetValue(directory + "\\project.xml", out var projectFile))
                    continue;

                var serializer = new XmlSerializer(typeof(Project));

                using var reader = new StringReader(projectFile);

                var project = (Project) serializer.Deserialize(reader);

                if (project == null)
                {
                    continue;
                }

                files.TryGetValue(project.PreviewIconFile, out var projectIcon);

                var modId = ulong.Parse(project.PublishedFileId);
                var modItem = new ModLocalItem()
                {
                    ModPublishedId = modId,
                    ModTitle       = project.Title,
                    ModDescription = project.ItemDescription,
                    ModThumbnail   = await GetModThumbnail(projectIcon)
                };

                if (!resolvedMods.ContainsKey(modId))
                    resolvedMods.Add(modId, modItem);
            }

            return resolvedMods;
        }
    }
}