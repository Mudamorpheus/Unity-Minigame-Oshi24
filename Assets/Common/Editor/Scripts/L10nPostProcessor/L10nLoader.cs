using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace L10nPostProcessor {
    public class TranslateVersions {
        [JsonProperty(PropertyName = "Version1")]
        public Dictionary<string, string> Version1;
        [JsonProperty(PropertyName = "Version2")]
        public Dictionary<string, string> Version2;
        [JsonProperty(PropertyName = "Version3")]
        public Dictionary<string, string> Version3;
        [JsonProperty(PropertyName = "Version4")]
        public Dictionary<string, string> Version4;
        [JsonProperty(PropertyName = "Version5")]
        public Dictionary<string, string> Version5;
    }

    public class L10NConfig {
        [JsonProperty(PropertyName = "AppTrackingTranslates")]
        public TranslateVersions AppTrackingTranslates;
        [JsonProperty(PropertyName = "CameraPermissionTranslates")]
        public TranslateVersions CameraPermissionTranslates;
    }
    
    public static class L10NLoader {
        public static Dictionary<string, Dictionary<string, string>> AppTrackingDict;
        public static Dictionary<string, Dictionary<string, string>> CameraPermissionDict;
        private static T DeserializeFromFile<T>(string filePath) {
            var textAsset = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<T>(textAsset);
        }
        
        public static void LoadL10NFile() {
            var configPath = Path.GetDirectoryName(Application.dataPath) + Path.DirectorySeparatorChar + 
                             "!Builder" + Path.DirectorySeparatorChar +
                             "Configs" + Path.DirectorySeparatorChar + "L10n.json";
            if (!File.Exists($"{configPath}")) 
                return; 
            
            var l10NConfig  = DeserializeFromFile<L10NConfig>(configPath);
            if (l10NConfig == null) 
                return;
            AppTrackingDict = new Dictionary<string, Dictionary<string, string>> {
                {"Version1", l10NConfig.AppTrackingTranslates.Version1},
                {"Version2", l10NConfig.AppTrackingTranslates.Version2},
                {"Version3", l10NConfig.AppTrackingTranslates.Version3},
                {"Version4", l10NConfig.AppTrackingTranslates.Version4},
                {"Version5", l10NConfig.AppTrackingTranslates.Version5}
            };

            CameraPermissionDict = new Dictionary<string, Dictionary<string, string>> {
                {"Version1", l10NConfig.CameraPermissionTranslates.Version1},
                {"Version2", l10NConfig.CameraPermissionTranslates.Version2},
                {"Version3", l10NConfig.CameraPermissionTranslates.Version3},
                {"Version4", l10NConfig.CameraPermissionTranslates.Version4},
                {"Version5", l10NConfig.CameraPermissionTranslates.Version5}
            };
        }
    }
}
