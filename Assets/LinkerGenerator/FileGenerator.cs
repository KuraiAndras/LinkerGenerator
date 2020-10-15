using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
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

            var assembliesToPreserve = Enumerable
                .Empty<string>()
                .Concat(_settings.AddDlls ? GetDllAssemblyNames(assetsDir) : Array.Empty<string>())
                .Concat(_settings.AddAsmdefs ? GetAsmdefAssemblyNames() : Array.Empty<string>())
                .Distinct()
                .Where(NotIgnoredByName)
                .Where(NotIgnoredByPattern)
                .OrderBy(s => s);

            var linkXmlFilePath = Path.Combine(assetsDir, _settings.FolderPath, "link.xml");

            Directory.CreateDirectory(Path.GetDirectoryName(linkXmlFilePath) ?? throw new InvalidOperationException($"No directory in file name {linkXmlFilePath}"));

            using (var fileStream = File.Open(linkXmlFilePath, FileMode.Create))
            using (var streamWriter = new StreamWriter(fileStream))
            {
                Enumerable
                    .Empty<string>()
                    .Concat("<linker>")
                    .Concat(string.Empty)
                    .Concat(assembliesToPreserve.Select(assemblyName => $"    <assembly fullname=\"{assemblyName}\" preserve=\"all\" />"))
                    .Concat(string.Empty)
                    .Concat("</linker>")
                    .ForEach(streamWriter.WriteLine);
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

        private bool NotIgnoredByName(string assemblyName) => !_settings.AssembliesToIgnore.Contains(assemblyName);

        private bool NotIgnoredByPattern(string assemblyName) => !_settings
            .AssemblyPatternsToIgnore
            .Any(p =>
            {
                try
                {
                    return new Regex(p).IsMatch(assemblyName);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Invalid regex: {e.Message}");
                    return false;
                }
            });

        [MenuItem("Window / Linker / Generate link.xml")]
        public static void GenerateLinkXml()
        {
            var settings = LinkerSettings.GetOrCreateSettings();

            new FileGenerator(settings).Generate();
        }
    }
}
