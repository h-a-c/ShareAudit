using Dionach.ShareAudit.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Dionach.ShareAudit.Modules.Services
{
    public class FileSystemStoreService : IFileSystemStoreService
    {
        private bool _readOnly = false;
        private bool _writeOnly = false;
        public string ExportDefaultFilename => DateTime.Now.ToString("yyyyMMddHHmm", CultureInfo.InvariantCulture);

        public string ExportFilter => "Share Audit Export (*.csv)|*.csv";

        public string ShareAuditDefaultFilename => DateTime.Now.ToString("yyyyMMddHHmm", CultureInfo.InvariantCulture);

        public string ShareAuditFilter => "Share Audit File (*.shareaudit)|*.shareaudit";

        public async Task CreateProjectAsync(string path)
        {
            await SaveProjectAsync(new Project(), path);
        }

        public async Task ExportProjectAsync(Project project, string path)
        {
            await Task.Run(() =>
            {
                var username = $"{project.Configuration.Credentials.Username}@{project.Configuration.Credentials.Domain}";
                var sb = new StringBuilder();

                if (!project.Configuration.EnableReadOnly && !project.Configuration.EnableWriteOnly && !project.Configuration.EnableSharesOnly && !project.Configuration.EnableHostOnly && project.Configuration.Files.Files.Length == 0 )
                {
                    sb.AppendLine("\"UNC Path\",\"Type\",\"Accessible\",\"Effective Read\",\"Effective Write\",\"Username\"");

                    foreach (var host in project.Hosts)
                    {
                        sb.AppendLine($"\"\\\\{host.Name}\",\"Host\",\"{host.Accessible}\",\"N/A\",\"N/A\",\"{username}\"");
                        foreach (var share in host.Shares)
                        {
                            WriteFolderEntry(sb, share, username);
                        }
                    }
                }
                else
                {
                    sb.AppendLine("\"UNC Path\",\"Type\",\"Accessible\",\"Effective Read\",\"Effective Write\",\"Username\"");

                    // Write the shares read/write etc.
                    foreach (var host in project.Hosts)
                    {
                        if (project.Configuration.EnableHostOnly && !host.Accessible)
                        {
                                continue;
                        } else
                        {
                            sb.AppendLine($"\"\\\\{host.Name}\",\"Host\",\"{host.Accessible}\",\"N/A\",\"N/A\",\"{username}\"");
                        }
                        foreach (var share in host.Shares)
                        {
                            CustomWriteFolderEntry(sb, share, username, project.Configuration);
                        }
                    }
                }

                File.WriteAllText(path, sb.ToString());
            });
        }

        private void CustomWriteFolderEntry(StringBuilder sb, IFolderEntry entry, string username, Dionach.ShareAudit.Model.Configuration config)
        {
            string lineAddition = $"\"{entry.FullName}\",\"{((entry is Share) ? "Share" : "Directory")}\",\"{((entry is Share) ? (entry as Share).Accessible : entry.EffectiveAccess.Read)}\",\"{entry.EffectiveAccess.Read}\",\"{entry.EffectiveAccess.Write}\",\"{username}\"";
            if (config.EnableReadOnly && !entry.EffectiveAccess.Read) { return; }
            if (config.EnableWriteOnly && !entry.EffectiveAccess.Write) { return; }
            List<string> file_list = config.Files.Files.Split(',').ToList<string>();
            foreach (var bad_file in file_list)
            {
                if (entry.FullName.Contains(bad_file.Trim()) && bad_file.Length > 0)
                {
                    return;
                }
            }
            sb.AppendLine(lineAddition);
            if (config.EnableSharesOnly) { return; }
            foreach (var childEntry in entry.FileSystemEntries)
            {
                if (childEntry is IFolderEntry)
                {
                    CustomWriteFolderEntry(sb, childEntry as IFolderEntry, username, config);
                }
                else
                {
                    if (config.EnableReadOnly && !entry.EffectiveAccess.Read) { return; }
                    if (config.EnableWriteOnly && !entry.EffectiveAccess.Write) { return; }
                    sb.AppendLine($"\"{childEntry.FullName}\",\"File\",\"{childEntry.EffectiveAccess.Read}\",\"{childEntry.EffectiveAccess.Read}\",\"{childEntry.EffectiveAccess.Write}\",\"{username}\"");
                }
            }
        }

        public async Task<Project> LoadProjectAsync(string path)
        {
            return await Task.Run(() =>
            {
                using (var reader = File.OpenRead(path))
                {
                    var serializer = new XmlSerializer(typeof(Project));
                    return serializer.Deserialize(reader) as Project;
                }
            });
        }

        public async Task SaveProjectAsync(Project project, string path)
        {
            await Task.Run(() =>
            {
                var settings = new XmlWriterSettings
                {
                    Indent = true
                };

                using (var writer = XmlWriter.Create(path, settings))
                {
                    var serializer = new XmlSerializer(typeof(Project));
                    serializer.Serialize(writer, project);
                }
            });
        }

        private void WriteFolderEntry(StringBuilder sb, IFolderEntry entry, string username)
        {
            sb.AppendLine($"\"{entry.FullName}\",\"{((entry is Share) ? "Share" : "Directory")}\",\"{((entry is Share) ? (entry as Share).Accessible : entry.EffectiveAccess.Read)}\",\"{entry.EffectiveAccess.Read}\",\"{entry.EffectiveAccess.Write}\",\"{username}\"");

            foreach (var childEntry in entry.FileSystemEntries)
            {
                if (childEntry is IFolderEntry)
                {
                    WriteFolderEntry(sb, childEntry as IFolderEntry, username);
                }
                else
                {
                    sb.AppendLine($"\"{childEntry.FullName}\",\"File\",\"{childEntry.EffectiveAccess.Read}\",\"{childEntry.EffectiveAccess.Read}\",\"{childEntry.EffectiveAccess.Write}\",\"{username}\"");
                }
            }
        }
    }
}
