using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace LinkerGenerator
{
    public sealed class LinkerSettings : ScriptableObject
    {
        private const string SettingsPath = "Assets/LinkerSettings.asset";

        public const string AssembliesToIgnoreName = nameof(_assembliesToIgnore);
        public const string AssemblyPatternsToIgnoreName = nameof(_assemblyPatternsToIgnore);
        public const string FolderPathName = nameof(_folderPath);
        public const string AddDllsName = nameof(_addDlls);
        public const string AddAsmdefsName = nameof(_addAsmdefs);
        public const string AddRspsName = nameof(_addRsps);

        [SerializeField] private List<string> _assembliesToIgnore;
        [SerializeField] private List<string> _assemblyPatternsToIgnore;

        [SerializeField]
        [Tooltip("Relative path from the Assets folder.")]
        private string _folderPath;

        [SerializeField] private bool _addDlls;
        [SerializeField] private bool _addAsmdefs;
        [SerializeField] private bool _addRsps;

        public IReadOnlyCollection<string> AssembliesToIgnore => _assembliesToIgnore;
        public IReadOnlyCollection<string> AssemblyPatternsToIgnore => _assemblyPatternsToIgnore;

        public string FolderPath => _folderPath;
        public bool AddDlls => _addDlls;
        public bool AddAsmdefs => _addAsmdefs;
        public bool AddRsps => _addRsps;

        public static LinkerSettings GetOrCreateSettings()
        {
            var settings = AssetDatabase.LoadAssetAtPath<LinkerSettings>(SettingsPath);
            if (settings != null) return settings;

            settings = CreateInstance<LinkerSettings>();

            settings._assembliesToIgnore = new List<string>();
            settings._folderPath = string.Empty;
            settings._addDlls = true;
            settings._addAsmdefs = false;

            AssetDatabase.CreateAsset(settings, SettingsPath);
            AssetDatabase.SaveAssets();

            return settings;
        }

        public static SerializedObject GetSerializedSettings() => new SerializedObject(GetOrCreateSettings());
    }
}