using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace LinkerGenerator
{
    public static class LinkerUi
    {
        [MenuItem("Window / Linker / Generate link.xml")]
        public static void GenerateLinkXml()
        {
            var settings = LinkerSettings.GetOrCreateSettings();

            new FileGenerator(settings).Generate();
        }

        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider() =>
            new SettingsProvider("Project/LinkerSettings", SettingsScope.Project)
            {
                label = "Linker",
                guiHandler = searchContext =>
                {
                    var settings = LinkerSettings.GetSerializedSettings();

                    EditorGUILayout.PropertyField(settings.FindProperty(LinkerSettings.AssembliesToIgnoreName), new GUIContent("Assemblies to ignore"));
                    EditorGUILayout.PropertyField(settings.FindProperty(LinkerSettings.AssemblyPatternsToIgnoreName), new GUIContent("Assemblies to ignore (Regex)"));
                    EditorGUILayout.PropertyField(settings.FindProperty(LinkerSettings.FolderPathName), new GUIContent("Link.xml folder path"));
                    EditorGUILayout.PropertyField(settings.FindProperty(LinkerSettings.AddDllsName), new GUIContent("Scan for DLLs"));
                    EditorGUILayout.PropertyField(settings.FindProperty(LinkerSettings.AddAsmdefsName), new GUIContent("Scan for ASMDEF files"));
                    EditorGUILayout.PropertyField(settings.FindProperty(LinkerSettings.AddRspsName), new GUIContent("Scan for Rsp files"));

                    settings.ApplyModifiedProperties();
                },
                keywords = new HashSet<string>(new[] { "Dll", "Linker" }),
            };
    }
}