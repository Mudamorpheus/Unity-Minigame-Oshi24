using Builder;
#if UNITY_IOS
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
#endif

namespace L10nPostProcessor {
    public class LocalizePostProcessor {
    #if UNITY_IOS
        [PostProcessBuild]
        public static void OnPostprocessBuild(BuildTarget buildTarget, string buildPath) {
            if (buildTarget == BuildTarget.iOS) {
                L10NLoader.LoadL10NFile();
                var project = new PBXProject();
                var projectPath = PBXProject.GetPBXProjectPath(buildPath);
                project.ReadFromFile(projectPath);

                if (project != null) {
                    var targetId = "";
                #if UNITY_2019_3_OR_NEWER
                    targetId = project.GetUnityFrameworkTargetGuid();
                #else
                        targetId = project.TargetGuidByName("Unity-iPhone");
                #endif
                    project.AddFrameworkToProject(targetId, "AppTrackingTransparency.framework", true);
                    project.AddFrameworkToProject(targetId, "AdSupport.framework", false);
                    project.AddFrameworkToProject(targetId, "StoreKit.framework", false);
                    project.WriteToFile(PBXProject.GetPBXProjectPath(buildPath));
                }

                /*
                 * PList
                 */
                var plist = new PlistDocument();
                plist.ReadFromString(File.ReadAllText(buildPath + "/Info.plist"));
                if (plist != null) {
                    // Get root
                    var rootDict = plist.root;
                    var mainTargetId = "";
                #if UNITY_2019_3_OR_NEWER
                    mainTargetId = project.GetUnityMainTargetGuid();
                #else
                    mainTargetId = project.TargetGuidByName("Unity-iPhone");
                #endif
                    //Add localization
                    
                    var appTrackingVersion = BuildApps.iOSBuildConfig != null ? BuildApps.iOSBuildConfig.AppTrackingLocalizationVersion : "Version1";
                    var appTrackingDictionary = L10NLoader.AppTrackingDict[appTrackingVersion];
                    
                    var cameraPermissionVersion = BuildApps.iOSBuildConfig != null ? BuildApps.iOSBuildConfig.CameraPermissionVersion : "Version1";
                    var cameraPermissionDictionary = L10NLoader.CameraPermissionDict[cameraPermissionVersion];
                    
                    rootDict.SetString("NSUserTrackingUsageDescription", appTrackingDictionary["EN"]);
                    rootDict.SetString("NSCameraUsageDescription", cameraPermissionDictionary["EN"]);
                    File.WriteAllText(buildPath + "/Info.plist", plist.WriteToString());
                    
                    foreach (var (systemLanguage, localizedMessageString) in appTrackingDictionary) {
                        AddDescriptionLocalizedString(
                            "NSUserTrackingUsageDescription",
                            localizedMessageString,
                            systemLanguage,
                            buildPath,
                            project,
                            mainTargetId);
                    }
                    
                    foreach (var (systemLanguage, localizedMessageString) in cameraPermissionDictionary) {
                        AddDescriptionLocalizedString(
                            "NSCameraUsageDescription",
                            localizedMessageString,
                            systemLanguage,
                            buildPath,
                            project,
                            mainTargetId);
                    }
                }
                project.WriteToFile(PBXProject.GetPBXProjectPath(buildPath));
            }
        }

        private static void AddDescriptionLocalizedString(string localizationKey, string locationUsageDescription,
                                                          string localeCode, string buildPath, PBXProject project,
                                                          string targetGuid) {
            const string resourcesDirectoryName = "Localizations";
            var resourcesDirectoryPath = Path.Combine(buildPath, resourcesDirectoryName);
            var localeSpecificDirectoryName = localeCode + ".lproj";
            var localeSpecificDirectoryPathToCreate = Path.Combine(resourcesDirectoryPath, localeSpecificDirectoryName);
            var localeSpecificDirectoryPath = Path.Combine(resourcesDirectoryName, localeSpecificDirectoryName);
            var infoPlistStringsFilePath = Path.Combine(localeSpecificDirectoryPathToCreate, "InfoPlist.strings");

            // Create intermediate directories as needed.
            if (!Directory.Exists(resourcesDirectoryPath)) {
                Directory.CreateDirectory(resourcesDirectoryPath);
            }

            if (!Directory.Exists(localeSpecificDirectoryPathToCreate)) {
                Directory.CreateDirectory(localeSpecificDirectoryPathToCreate);
            }

            var localizedDescriptionLine1 = $"\"{localizationKey}\" = \"" + locationUsageDescription + "\";\n";
            // File already exists, update it in case the value changed between builds.
            if (File.Exists(infoPlistStringsFilePath)) {
                var output = new List<string>();
                var lines = File.ReadAllLines(infoPlistStringsFilePath);
                var keyUpdated = false;
                foreach (var line in lines) {
                    if (line.Contains(localizationKey)) {
                        output.Add(localizedDescriptionLine1);
                        keyUpdated = true;
                    } else {
                        output.Add(line);
                    }
                }

                if (!keyUpdated) {
                    output.Add(localizedDescriptionLine1);
                }

                File.WriteAllText(infoPlistStringsFilePath, string.Join("\n", output.ToArray()) + "\n");
            }
            // File doesn't exist, create one.
            else {
                File.WriteAllText(infoPlistStringsFilePath,
                    "/* Localized versions of Info.plist keys */\n" + localizedDescriptionLine1);
            }

            var guid = project.AddFolderReference(localeSpecificDirectoryPath,
                Path.Combine(resourcesDirectoryName, localeSpecificDirectoryName), PBXSourceTree.Source);
            project.AddFileToBuild(targetGuid, guid);
        }
    #endif
    }
}
