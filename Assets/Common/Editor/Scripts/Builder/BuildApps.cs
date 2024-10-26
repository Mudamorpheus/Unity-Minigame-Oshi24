using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Scripting;

namespace Builder {
    public class AndroidBuildConfig {
        [JsonProperty(PropertyName = "product_name")]
        public string ProductName;

        [JsonProperty(PropertyName = "build_name")]
        public string BuildName;

        [JsonProperty(PropertyName = "package")]
        public string Package;

        [JsonProperty(PropertyName = "bundle_version")]
        public string BundleVersion;

        [JsonProperty(PropertyName = "bundle_version_code")]
        public int BundleVersionCode;

        [JsonProperty(PropertyName = "keystore_name")]
        public string KeystoreName;

        [JsonProperty(PropertyName = "android_min_sdk")]
        public int AndroidMinSdk;

        [JsonProperty(PropertyName = "android_target_sdk")]
        public int AndroidTargetSdk;

        [JsonProperty(PropertyName = "obfuscator")]
        public string ObfuscatorName;
    }

    public class iOSBuildConfig {
        [JsonProperty(PropertyName = "product_name")]
        public string ProductName;

        [JsonProperty(PropertyName = "build_name")]
        public string BuildName;

        [JsonProperty(PropertyName = "package")]
        public string Package;

        [JsonProperty(PropertyName = "bundle_version")]
        public string BundleVersion;

        [JsonProperty(PropertyName = "bundle_version_code")]
        public string BundleVersionCode;

        [JsonProperty(PropertyName = "apptracking_localization_version")]
        public string AppTrackingLocalizationVersion;

        [JsonProperty(PropertyName = "camera_permission_version")]
        public string CameraPermissionVersion;

        [JsonProperty(PropertyName = "obfuscator")]
        public string ObfuscatorName;
    }

    public class AndroidBuildHelper {
        [JsonProperty(PropertyName = "builds_root")]
        public string BuildsRoot;
    }

    public class iOSBuildHelper {
        [JsonProperty(PropertyName = "builds_root")]
        public string BuildsRoot;
    }

    public class CommonConfig {
        public iOSBuildConfig IOSBuildConfig;
        public AndroidBuildConfig AndroidBuildConfig;
    }

    public class CommonBuildHelper {
        public iOSBuildHelper IOSBuildHelper;
        public AndroidBuildHelper AndroidBuildHelper;
    }

    [Serializable]
    public enum AndroidBundleType {
        APK,
        AAB
    }

    public class BuildApps : MonoBehaviour {
        private const string BuildsLocation = "-buildpath";
        private const string AndroidBundle = "-bundletype";
        private const string helperProdName = "build_config_helper_prod.json";
        private const string helperTestName = "build_config_helper_test.json";
        private const string helperDevName = "build_config_helper_dev.json";
        private static string helperName;
        private static AndroidBuildConfig androidBuildConfig;
        public static iOSBuildConfig iOSBuildConfig;
        public static AndroidBuildHelper androidBuildHelper;
        public static iOSBuildHelper iOSBuildHelper;
        private static CommonConfig commonConfig;
        private static CommonBuildHelper commonBuildHelper;
        private static string _devBuildPath = "";
        private static bool _trackingExist = false;
        private const string AttObjectName = "ATT";

        private static string GetBuildLocation(BuildTarget buildTarget) {
            var args = Environment.GetCommandLineArgs();
            var indexOfBuildLocation = Array.IndexOf(args, BuildsLocation);
            var destPath = string.Empty;
            if (indexOfBuildLocation >= 0) {
                indexOfBuildLocation++;
                Debug.Log($"Build Location for {buildTarget.ToString()} set to {args[indexOfBuildLocation]}");
                destPath = args[indexOfBuildLocation];
                if (!destPath.EndsWith(Path.DirectorySeparatorChar.ToString())) {
                    destPath += Path.DirectorySeparatorChar;
                }
            } else {
                var helperPath = Path.GetDirectoryName(Application.dataPath) + Path.DirectorySeparatorChar +
                                 "!Builder" + Path.DirectorySeparatorChar +
                                 "Configs" + Path.DirectorySeparatorChar + helperName;
                if (File.Exists($"{helperPath}")) {
                    commonBuildHelper = DeserializeFromFile<CommonBuildHelper>(helperPath);
                    if (commonBuildHelper != null) {
                        androidBuildHelper = commonBuildHelper.AndroidBuildHelper;
                        iOSBuildHelper = commonBuildHelper.IOSBuildHelper;
                        switch (buildTarget) {
                            case BuildTarget.Android:
                                if (androidBuildHelper != null) destPath = androidBuildHelper.BuildsRoot;
                                break;
                            case BuildTarget.iOS:
                                if (iOSBuildHelper != null) destPath = iOSBuildHelper.BuildsRoot;
                                break;
                        }
                    }
                }
            }

            switch (buildTarget) {
                case BuildTarget.Android:
                    if (androidBuildConfig != null) {
                        return destPath + androidBuildConfig.BuildName + "_" +
                               androidBuildConfig.BundleVersion + "_" +
                               androidBuildConfig.BundleVersionCode;
                    } else {
                        return destPath + Application.identifier + "_" +
                               PlayerSettings.bundleVersion + "_" +
                               PlayerSettings.Android.bundleVersionCode;
                    }
                case BuildTarget.iOS:
                    if (iOSBuildConfig != null) {
                        return destPath + iOSBuildConfig.BuildName + "_" +
                               iOSBuildConfig.BundleVersion + "_" +
                               iOSBuildConfig.BundleVersionCode;
                    } else {
                        return destPath + Application.identifier + "_" +
                               PlayerSettings.bundleVersion + "_" + PlayerSettings.iOS.buildNumber;
                    }

                default:
                    return EditorUserBuildSettings.GetBuildLocation(buildTarget);
            }
        }

        private static void LoadBuildConfig(BuildTarget target) {
            var configName = "build_config.json";
            var configPath =
                $"{Path.GetDirectoryName(Application.dataPath)}" +
                $"{Path.DirectorySeparatorChar}!Builder{Path.DirectorySeparatorChar}{configName}";

            if (!File.Exists($"{configPath}")) {
                return;
            }

            commonConfig = DeserializeFromFile<CommonConfig>(configPath);
            if (commonConfig != null) {
                androidBuildConfig = commonConfig.AndroidBuildConfig;
                iOSBuildConfig = commonConfig.IOSBuildConfig;
            }

            switch (target) {
                case BuildTarget.Android: {
                    if (androidBuildConfig != null) {
                        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, androidBuildConfig.Package);
                        PlayerSettings.bundleVersion = androidBuildConfig.BundleVersion;
                        PlayerSettings.productName = androidBuildConfig.ProductName;
                        PlayerSettings.Android.bundleVersionCode = androidBuildConfig.BundleVersionCode;
                        PlayerSettings.Android.useCustomKeystore = false;

                        if (androidBuildConfig.KeystoreName == null ||
                            string.IsNullOrEmpty(androidBuildConfig.KeystoreName)) {
                            throw new Exception("===>>> ERROR! Keystore not set!");
                        }
                        
                        PlayerSettings.Android.useCustomKeystore = true;
                        PlayerSettings.Android.useCustomKeystore = true;
                        PlayerSettings.Android.keystoreName =  Path.GetDirectoryName(Application.dataPath) + Path.DirectorySeparatorChar +
                                                               "!Builder" + Path.DirectorySeparatorChar +
                                                               "Android" + Path.DirectorySeparatorChar +
                                                               androidBuildConfig.KeystoreName + ".keystore";
                        AssetDatabase.Refresh();
                        System.Threading.Thread.Sleep(2000);
                        PlayerSettings.Android.keystorePass = androidBuildConfig.KeystoreName;
                        PlayerSettings.Android.keyaliasName = androidBuildConfig.KeystoreName;
                        PlayerSettings.Android.keyaliasPass = androidBuildConfig.KeystoreName;
                    }

                    break;
                }

                case BuildTarget.iOS: {
                    if (iOSBuildConfig != null) {
                        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, iOSBuildConfig.Package);
                        PlayerSettings.bundleVersion = iOSBuildConfig.BundleVersion;
                        PlayerSettings.productName = iOSBuildConfig.ProductName;
                        PlayerSettings.iOS.buildNumber = iOSBuildConfig.BundleVersionCode;
                    }

                    break;
                }
            }
        }

        private static void CheckIosAppTracking() {
            if (GameObject.Find(AttObjectName)) {
                if (FindObjectOfType<GetAtt>()) {
                    _trackingExist = true;
                }
            }
        }

        private static void IterateAllScenes(Action callback) {
            var scenes = EditorBuildSettings.scenes;
            foreach (var scene in scenes) {
                var scenePath = scene.path;
                CheckLunar(scenePath);
                CheckIosAppTracking();
            }

            callback?.Invoke();
        }

        private static void CheckLunar(string scenePath) {
            EditorSceneManager.OpenScene(scenePath);
            var gameObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
            foreach (var go in gameObjects) {
                if (!go.name.Contains("LunarConsole")) continue;

                UnityEngine.Object.DestroyImmediate(go);
                EditorSceneManager.SaveScene(SceneManager.GetActiveScene(), string.Join("/", scenePath));
            }
        }

        private static void DisableObfuscator() {
            var projectPath = $"{Path.GetDirectoryName(Application.dataPath)}" +
                              $"{Path.DirectorySeparatorChar}Assets";
            
            var configsPath = $"{Path.GetDirectoryName(Application.dataPath)}" +
                              $"{Path.DirectorySeparatorChar}" +
                              $"!Builder{Path.DirectorySeparatorChar}" +
                              $"Configs{Path.DirectorySeparatorChar}" +
                              $"Obfuscator_configs{Path.DirectorySeparatorChar}ops";
            
            var fileOpsDestination = $"{projectPath}{Path.DirectorySeparatorChar}" +
                                     $"OPS{Path.DirectorySeparatorChar}" +
                                     $"Obfuscator{Path.DirectorySeparatorChar}" +
                                     $"Settings{Path.DirectorySeparatorChar}Obfuscator_Settings.json";

            File.Copy($"{configsPath}{Path.DirectorySeparatorChar}disabled_Obfuscator_Settings.json", fileOpsDestination,
                true);
        }

        private static void EnableObfuscator() {
            var projectPath = $"{Path.GetDirectoryName(Application.dataPath)}" +
                              $"{Path.DirectorySeparatorChar}Assets";
            
            var configsPath = $"{Path.GetDirectoryName(Application.dataPath)}" +
                              $"{Path.DirectorySeparatorChar}" +
                              $"!Builder{Path.DirectorySeparatorChar}" +
                              $"Configs{Path.DirectorySeparatorChar}" +
                              $"Obfuscator_configs{Path.DirectorySeparatorChar}ops";
            
            var fileOpsDestination = $"{projectPath}{Path.DirectorySeparatorChar}" +
                                     $"OPS{Path.DirectorySeparatorChar}" +
                                     $"Obfuscator{Path.DirectorySeparatorChar}" +
                                     $"Settings{Path.DirectorySeparatorChar}Obfuscator_Settings.json";
            
            if (File.Exists(
                    $"{configsPath}{Path.DirectorySeparatorChar}enabled_Obfuscator_Settings.json")) {
                File.Copy(
                    $"{configsPath}{Path.DirectorySeparatorChar}enabled_Obfuscator_Settings.json",
                    fileOpsDestination, true);
            }
        }

        private static void SetupAndroidProjectSettings() {
            EditorUserBuildSettings.development = false;
            EditorUserBuildSettings.androidBuildType = AndroidBuildType.Release;
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
            PlayerSettings.SetIl2CppCompilerConfiguration(BuildTargetGroup.Android, Il2CppCompilerConfiguration.Release);
            PlayerSettings.SetManagedStrippingLevel(BuildTargetGroup.Android, ManagedStrippingLevel.Minimal);

            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64 | AndroidArchitecture.ARMv7;

            PlayerSettings.Android.preferredInstallLocation = AndroidPreferredInstallLocation.Auto;
            PlayerSettings.Android.forceSDCardPermission = false;

            PlayerSettings.Android.minSdkVersion = (AndroidSdkVersions) androidBuildConfig.AndroidMinSdk;
            PlayerSettings.Android.targetSdkVersion = (AndroidSdkVersions) androidBuildConfig.AndroidTargetSdk;
            PlayerSettings.stripEngineCode = true;
            PlayerSettings.SetManagedStrippingLevel(BuildTargetGroup.Android, ManagedStrippingLevel.Minimal);

            EditorUserBuildSettings.androidBuildSubtarget = MobileTextureSubtarget.ASTC;
        #if UNITY_2022_3
            PlayerSettings.insecureHttpOption = InsecureHttpOption.AlwaysAllowed;
            PlayerSettings.SetIl2CppCodeGeneration(NamedBuildTarget.Android, Il2CppCodeGeneration.OptimizeSize);
        #else
            EditorUserBuildSettings.il2CppCodeGeneration = Il2CppCodeGeneration.OptimizeSize;
        #endif
        }

        private static void SetupAndroidDevProjectSettings() {
            EditorUserBuildSettings.development = true;
            EditorUserBuildSettings.exportAsGoogleAndroidProject = false;
            PlayerSettings.Android.useCustomKeystore = false;
            EditorUserBuildSettings.androidBuildType = AndroidBuildType.Development;
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.Mono2x);
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARMv7;
            PlayerSettings.Android.preferredInstallLocation = AndroidPreferredInstallLocation.Auto;
            PlayerSettings.Android.forceSDCardPermission = false;
            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel23;
            PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevelAuto;
            EditorUserBuildSettings.androidBuildSubtarget = MobileTextureSubtarget.ASTC;
        #if UNITY_2022_3
            PlayerSettings.insecureHttpOption = InsecureHttpOption.AlwaysAllowed;
            PlayerSettings.SetIl2CppCodeGeneration(NamedBuildTarget.Android, Il2CppCodeGeneration.OptimizeSize);
        #else
            EditorUserBuildSettings.il2CppCodeGeneration = Il2CppCodeGeneration.OptimizeSize;
        #endif
        }

        private static void SetupIOSProjectSettings() {
            PlayerSettings.iOS.targetOSVersionString = "14.0";
            EditorUserBuildSettings.development = false;
            PlayerSettings.stripEngineCode = true;
            PlayerSettings.SetManagedStrippingLevel(BuildTargetGroup.iOS, ManagedStrippingLevel.Minimal);
            EditorUserBuildSettings.androidBuildSubtarget = MobileTextureSubtarget.ASTC;
        #if UNITY_2022_3
            PlayerSettings.insecureHttpOption = InsecureHttpOption.AlwaysAllowed;
            PlayerSettings.SetIl2CppCodeGeneration(NamedBuildTarget.iOS, Il2CppCodeGeneration.OptimizeSize);
        #else
            EditorUserBuildSettings.il2CppCodeGeneration = Il2CppCodeGeneration.OptimizeSize;
        #endif
            EditorUserBuildSettings.symlinkSources = false;
            EditorUserBuildSettings.iOSXcodeBuildConfig = XcodeBuildConfig.Release;
        }

        private static T DeserializeFromFile<T>(string filePath) {
            var textAsset = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<T>(textAsset);
        }

        private static AndroidBundleType GetAndroidBundleType() {
            var args = System.Environment.GetCommandLineArgs();
            var indexOfAndroidBundle = System.Array.IndexOf(args, AndroidBundle);
            if (indexOfAndroidBundle < 0) return AndroidBundleType.APK;

            indexOfAndroidBundle++;
            Debug.Log($"Bundle type set to {args[indexOfAndroidBundle]}");
            return args[indexOfAndroidBundle].Equals("aab", StringComparison.InvariantCultureIgnoreCase)
                ? AndroidBundleType.AAB
                : AndroidBundleType.APK;
        }

        private static string[] GetBuildScenes() {
            var scenes = new List<string>();

            foreach (var scene in EditorBuildSettings.scenes) {
                if (scene == null)
                    continue;

                if (scene.enabled)
                    scenes.Add(scene.path);
            }

            return scenes.ToArray();
        }

        [MenuItem("[PROJECT]/MODES/Developer mode", false, 5)]
        public static void DeveloperMode() {
            var defines =
                PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            if (!defines.Contains("TEST")) {
                defines += ";TEST";
            }

            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, defines);

            PlayerSettings.SetStackTraceLogType(LogType.Assert, StackTraceLogType.ScriptOnly);
            PlayerSettings.SetStackTraceLogType(LogType.Error, StackTraceLogType.ScriptOnly);
            PlayerSettings.SetStackTraceLogType(LogType.Exception, StackTraceLogType.ScriptOnly);
            PlayerSettings.SetStackTraceLogType(LogType.Log, StackTraceLogType.ScriptOnly);
            PlayerSettings.SetStackTraceLogType(LogType.Warning, StackTraceLogType.ScriptOnly);
        #if UNITY_ANDROID
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.Mono2x);
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARMv7;
            PlayerSettings.applicationIdentifier = "com.game.azart";
            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel23;
            PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevelAuto;
        #elif UNITY_IOS
            PlayerSettings.applicationIdentifier = "com.luckyclubb";
            PlayerSettings.iOS.appleDeveloperTeamID = "MQ6V2K6YBY";
            PlayerSettings.iOS.appleEnableAutomaticSigning = true;
            PlayerSettings.iOS.iOSManualProvisioningProfileType = ProvisioningProfileType.Development;
        #endif
        }

        [MenuItem("[PROJECT]/MODES/Production mode", false, 5)]
        public static void ProductionMode() {
            var defines =
                PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            if (defines.Contains("TEST")) {
                defines = defines.Replace("TEST", "");
            }

            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, defines);

            PlayerSettings.SetStackTraceLogType(LogType.Assert, StackTraceLogType.None);
            PlayerSettings.SetStackTraceLogType(LogType.Error, StackTraceLogType.None);
            PlayerSettings.SetStackTraceLogType(LogType.Exception, StackTraceLogType.None);
            PlayerSettings.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
            PlayerSettings.SetStackTraceLogType(LogType.Warning, StackTraceLogType.None);
            PlayerSettings.stripEngineCode = true;
        #if UNITY_ANDROID
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
            PlayerSettings.SetIl2CppCompilerConfiguration(BuildTargetGroup.Android, Il2CppCompilerConfiguration.Release);
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64 | AndroidArchitecture.ARMv7;
            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel23;
            PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevelAuto;
            PlayerSettings.SetManagedStrippingLevel(BuildTargetGroup.Android, ManagedStrippingLevel.Minimal);
        #elif UNITY_IOS
        #if USE_GEO
            PlayerSettings.iOS.locationUsageDescription = "Enable geolocation?";
        #else
            PlayerSettings.iOS.locationUsageDescription = string.Empty;
        #endif
            PlayerSettings.iOS.appleDeveloperTeamID = "";
            PlayerSettings.iOS.appleEnableAutomaticSigning = false;
            PlayerSettings.iOS.iOSManualProvisioningProfileType = ProvisioningProfileType.Distribution;
            PlayerSettings.SetManagedStrippingLevel(BuildTargetGroup.iOS, ManagedStrippingLevel.Minimal);
        #endif
            RemovePlugins();
        }

        private static void RemovePlugins() {
            const string helpersPath = "Assets/Helpers";
            const string packagesPath = "Assets/BasePackages";

            AssetDatabase.DeleteAssets(new[] {helpersPath, packagesPath}, new List<string>());
        }

        [UnityEditor.MenuItem("[BUILD]/iOS BUILDS/TEST Build")]
        static void iOSTestBuild() {
            helperName = helperTestName;
            IterateAllScenes(() => {
                if (!_trackingExist) {
                    throw new Exception("===>>> ERROR! ATT not found");
                }

                LoadBuildConfig(BuildTarget.iOS);
                DeveloperMode();
                DisableObfuscator();
                Build(BuildTarget.iOS);
            });
        }

        [UnityEditor.MenuItem("[BUILD]/iOS BUILDS/PRODUCTION Build")]
        static void iOSProductionBuild() {
            helperName = helperProdName;
            IterateAllScenes(() => {
                if (!_trackingExist) {
                    throw new Exception("===>>> ERROR! ATT not found");
                }

                LoadBuildConfig(BuildTarget.iOS);
                ProductionMode();
                EnableObfuscator();
                Build(BuildTarget.iOS);
            });
        }

        [UnityEditor.MenuItem("[BUILD]/===>>> ANDROID TEST BUILD <<<===", false, 2)]
        static void AndroidBuildDevAPK() {
            _devBuildPath = EditorUtility.SaveFilePanel(
                "Save Android developer build",
                "",
                $"{Application.productName}_{PlayerSettings.bundleVersion}_{PlayerSettings.Android.bundleVersionCode}",
                "apk");
            if (string.IsNullOrEmpty(_devBuildPath)) {
                return;
            }

            DeveloperMode();
            DisableObfuscator();
            Build(BuildTarget.Android, false, true);
        }

        [UnityEditor.MenuItem("[BUILD]/ANDROID BUILDS/APK TEST Build")]
        static void AndroidBuildTestAPK() {
            helperName = helperTestName;
            LoadBuildConfig(BuildTarget.Android);
            DeveloperMode();
            DisableObfuscator();
            System.Threading.Thread.Sleep(5000);
            Build(BuildTarget.Android, false);
        }

        [UnityEditor.MenuItem("[BUILD]/ANDROID BUILDS/APK PRODUCTION Build")]
        static void AndroidBuildProdAPK() {
            IterateAllScenes(() => {
                helperName = helperProdName;
                LoadBuildConfig(BuildTarget.Android);
                ProductionMode();
                EnableObfuscator();
                System.Threading.Thread.Sleep(5000);
                Build(BuildTarget.Android, false);
            });
        }

        [UnityEditor.MenuItem("[BUILD]/ANDROID BUILDS/AAB PRODUCTION Build")]
        static void AndroidBuildProdAAB() {
            IterateAllScenes(() => {
                helperName = helperProdName;
                LoadBuildConfig(BuildTarget.Android);
                ProductionMode();
                EnableObfuscator();
                Build(BuildTarget.Android, true);
            });
        }

        private static void Build(BuildTarget target, bool androidPlayBundle = false, bool androidDevBuild = false) {
            var path = string.Empty;
            switch (target) {
                case BuildTarget.Android: {
                    path = androidDevBuild ? _devBuildPath : GetBuildLocation(target);
                    if (androidPlayBundle) {
                        EditorUserBuildSettings.buildAppBundle = true;
                        path += ".aab";
                    } else {
                        EditorUserBuildSettings.buildAppBundle = false;
                        if (!androidDevBuild) path += ".apk";
                    }

                    break;
                }
                case BuildTarget.iOS:
                    path = GetBuildLocation(target);
                    break;
            }

            var scenes = GetBuildScenes();
            if (scenes == null || scenes.Length == 0)
                return;

            Debug.Log($"Path: \"{path}\"");
            for (var i = 0; i < scenes.Length; ++i) {
                Debug.Log($"Scene[{i}]: \"{scenes[i]}\"");
            }

            Debug.Log($"Creating Directory \"{path}\" if it does not exist");
            var directoryInfo = new FileInfo(path).Directory;
            directoryInfo?.Create();

            Debug.Log($"Switching Build Target to {target}");
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildPipeline.GetBuildTargetGroup(target), target);
            PlayerSettings.stripEngineCode = true;

            switch (target) {
                case BuildTarget.Android:
                    if (!androidDevBuild)
                        SetupAndroidProjectSettings();
                    else
                        SetupAndroidDevProjectSettings();
                    break;
                case BuildTarget.iOS:
                    SetupIOSProjectSettings();
                    break;
            }

            EditorUserBuildSettings.connectProfiler = false;
            EditorUserBuildSettings.allowDebugging = false;

            var buildPlayerOptions = new BuildPlayerOptions {
                scenes = scenes,
                locationPathName = path,
                target = target,
                options = BuildOptions.CompressWithLz4HC | BuildOptions.CleanBuildCache
            };

            Debug.Log("Starting Build!");
            var report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            var summary = report.summary;

            switch (summary.result) {
                case BuildResult.Succeeded:
                    Debug.Log($"Build succeeded: {summary.totalSize} bytes");
                    break;
                case BuildResult.Failed:
                    Debug.Log("Build failed");
                    break;
                default:
                    Debug.Log($"Build not succeeded. Result {summary.result}");
                    break;
            }
        }

        [MenuItem("[PROJECT]/===>>> Clean preferences <<<===", false, 0)]
        private static void CleanPrefs() {
            PlayerPrefs.DeleteAll();
            Caching.ClearCache();
        #if UNITY_EDITOR
            FileUtil.DeleteFileOrDirectory(Application.temporaryCachePath);
        #endif
            Debug.Log("===>>> Preferences cleaned");
        }
    }
}
