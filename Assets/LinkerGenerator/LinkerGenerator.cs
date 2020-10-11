using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace LinkerGenerator
{
    public class LinkerGenerator
    {
        private readonly LinkerSettings _settings;

        public LinkerGenerator(LinkerSettings settings) => _settings = settings;

        public void Generate()
        {
            var assetsDir = Application.dataPath;
            var dllsDir = Path.Combine(assetsDir, "NugetDlls");

            var dllTags = Directory.EnumerateFiles(dllsDir, "*.dll")
                .Distinct()
                .Select(Path.GetFileNameWithoutExtension)
                .Where(NotIgnored)
                .Select(CreateAssemblyLinkTag)
                .ToList();

            var linkXmlFilePath = Path.Combine(assetsDir, _settings.FolderPath, "link.xml");

            Directory.CreateDirectory(Path.GetDirectoryName(linkXmlFilePath) ?? throw new InvalidOperationException($"No directory in file name {linkXmlFilePath}"));

            using (var fileStream = File.Open(linkXmlFilePath, FileMode.Create))
            using (var streamWriter = new StreamWriter(fileStream))
            {
                streamWriter.WriteLine("<linker>");

                streamWriter.WriteLine();

                dllTags.ForEach(d => streamWriter.WriteLine(d));

                streamWriter.WriteLine();

                streamWriter.WriteLine("</linker>");
            }
        }

        private bool NotIgnored(string assemblyName) => !_settings.AssembliesToIgnore.Contains(assemblyName);

        private static string CreateAssemblyLinkTag(string assemblyName) => $"\t<assembly fullname=\"{assemblyName}\" preserve=\"all\" />";

        [MenuItem("Window / Linker / Generate link.xml")]
        public static void GenerateLinkXml()
        {
            var settings = LinkerSettings.GetOrCreateSettings();

            new LinkerGenerator(settings).Generate();
        }
    }
}
