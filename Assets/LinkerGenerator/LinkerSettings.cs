using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace LinkerGenerator
{
    public sealed class LinkerSettings : ScriptableObject
    {
        private static readonly string SettingsPath = "Assets/LinkerSettings.asset";

        [SerializeField] private List<string> _assembliesToIgnore;

        [SerializeField]
        [Tooltip("Relative path from the Assets folder.")]
        private string _folderPath;

        public IReadOnlyCollection<string> AssembliesToIgnore => _assembliesToIgnore;
        public string FolderPath => _folderPath;

        public static LinkerSettings GetOrCreateSettings()
        {
            var settings = AssetDatabase.LoadAssetAtPath<LinkerSettings>(SettingsPath);
            if (settings == null)
            {
                settings = CreateInstance<LinkerSettings>();

                settings._assembliesToIgnore = new List<string>();
                settings._folderPath = string.Empty;

                AssetDatabase.CreateAsset(settings, SettingsPath);
                AssetDatabase.SaveAssets();
            }

            return settings;
        }

        public static SerializedObject GetSerializedSettings() => new SerializedObject(GetOrCreateSettings());

        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider() =>
            new SettingsProvider("Project/LinkerSettings", SettingsScope.Project)
            {
                label = "Linker",
                guiHandler = searchContext =>
                {
                    var settings = GetSerializedSettings();

                    EditorGUILayout.PropertyField(settings.FindProperty(nameof(_assembliesToIgnore)), new GUIContent("Assemblies to ignore"));
                    EditorGUILayout.PropertyField(settings.FindProperty(nameof(_folderPath)), new GUIContent("Link.xml folder path"));

                    settings.ApplyModifiedProperties();
                },
                keywords = new HashSet<string>(new[] { "Dll", "Linker" }),
            };
    }
}