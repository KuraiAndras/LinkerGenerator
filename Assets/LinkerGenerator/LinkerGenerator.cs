using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace LinkerGenerator
{
    public static class LinkerGenerator
    {
        private static readonly string[] DontPreserveAssemblies =
        {
            "System.Net.Http",
            "System.Text.Json",
        };

        [MenuItem("Window / Linker / Generate link.xml")]
        public static void GenerateLinkXml()
        {
            var assetsDir = Application.dataPath;
            var dllsDir = Path.Combine(assetsDir, "NugetDlls");

            var dllTags = Directory.EnumerateFiles(dllsDir, "*.dll")
                .Select(Path.GetFileNameWithoutExtension)
                .Where(AllowedForPreserve)
                .Select(CreateAssemblyLinkTag)
                .ToList();

            var linkXmlFilePath = Path.Combine(assetsDir, "link.xml");

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

        private static string CreateAssemblyLinkTag(string assemblyName) => $"\t<assembly fullname=\"{assemblyName}\" preserve=\"all\" />";
        private static bool AllowedForPreserve(string assemblyName) => !DontPreserveAssemblies.Contains(assemblyName);
    }
}
