using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

namespace LinkerGenerator
{
    public class FileGenerator
    {
        private readonly LinkerSettings _settings;

        public FileGenerator(LinkerSettings settings) => _settings = settings;

        public void Generate()
        {
            var assetsDir = Application.dataPath;

            var dlls = _settings.AddDlls ? GetDllAssemblyNames(assetsDir) : new string[0];
            var asmdefs = _settings.AddAsmdefs ? GetAsmdefAssemblyNames() : new string[0];

            var assembliesToPreserve = dlls
                .Concat(asmdefs)
                .Where(NotIgnored)
                .ToArray();

            if (assembliesToPreserve.Length == 0) Debug.Log("Found nothing to preserve.");

            var linkXmlFilePath = Path.Combine(assetsDir, _settings.FolderPath, "link.xml");

            Directory.CreateDirectory(Path.GetDirectoryName(linkXmlFilePath) ?? throw new InvalidOperationException($"No directory in file name {linkXmlFilePath}"));

            using (var fileStream = File.Open(linkXmlFilePath, FileMode.Create))
            using (var streamWriter = new StreamWriter(fileStream))
            {
                streamWriter.WriteLine("<linker>");

                streamWriter.WriteLine();

                assembliesToPreserve.Select(CreateAssemblyLinkTag).ForEach(d => streamWriter.WriteLine(d));

                streamWriter.WriteLine();

                streamWriter.WriteLine("</linker>");
            }
        }

        private static string[] GetAsmdefAssemblyNames() =>
            CompilationPipeline.GetAssemblies(AssembliesType.PlayerWithoutTestAssemblies)
                .Select(a => a.name)
                .Distinct()
                .ToArray();

        private static string[] GetDllAssemblyNames(string assetsDir) =>
            Directory.EnumerateFiles(assetsDir, "*.dll", SearchOption.AllDirectories)
                .Distinct()
                .Select(Path.GetFileNameWithoutExtension)
                .ToArray();

        private bool NotIgnored(string assemblyName) => !_settings.AssembliesToIgnore.Contains(assemblyName);

        private static string CreateAssemblyLinkTag(string assemblyName) => $"\t<assembly fullname=\"{assemblyName}\" preserve=\"all\" />";

        [MenuItem("Window / Linker / Generate link.xml")]
        public static void GenerateLinkXml()
        {
            var settings = LinkerSettings.GetOrCreateSettings();

            new FileGenerator(settings).Generate();
        }
    }
}
