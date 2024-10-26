using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace Common.Editor.Scripts {
    [InitializeOnLoad]
    public class PackageInstaller : EditorWindow {
        private List<string> _availablePackages = new List<string>();
        private const string PackagesPath = "Assets/BasePackages";
        private const string PackageExtension = ".unitypackage";
        private const string MetaExtension = ".meta";
        private const string InstallText = "Install";
        private const string InstalledText = "Installed ✔";
        private const string ReinstallText = "Reinstall";
        private const string CompleteInstallText = "Complete Installation";
        private const char InstalledPackagePostfix = '_';
        private readonly string _packageSearchPattern = $"*{PackageExtension}";
        private Vector2 _scrollPosition;

        [MenuItem("Window/Package Installer")]
        public static void ShowWindow() {
            GetWindow<PackageInstaller>("Package Installer").titleContent.text = "Package Installer";
        }

        static PackageInstaller() {
            EditorApplication.projectChanged += OnProjectWindowChanged;
            EditorApplication.delayCall += CheckAndShowWindow;
        }

        private static void OnProjectWindowChanged() {
            CheckAndShowWindow();
        }

        private static void CheckAndShowWindow() {
            if (!Directory.Exists(PackagesPath)) {
                CloseAllWindows();
                return;
            }

            ShowWindow();
        }

        private void OnEnable() {
            UpdateAvailablePackages();
        }

        private void OnGUI() {
            var titleStyle = new GUIStyle(GUI.skin.label) {
                fontStyle = FontStyle.Bold,
                fontSize = 25
            };

            GUILayout.Label("Package Installer", titleStyle);

            if (_availablePackages.Count == 0) {
                EditorGUILayout.LabelField("No packages found in " + PackagesPath);
                return;
            }

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            foreach (var package in _availablePackages) {
                GUILayout.BeginHorizontal();

                string label;
                var packageName = label = package;
                if (packageName.EndsWith(InstalledPackagePostfix)) {
                    label = packageName[..^1];
                }

                GUILayout.Label(label);

                if (package.EndsWith(InstalledPackagePostfix)) {
                    GUI.contentColor = Color.green;
                    GUILayout.Label(InstalledText, GUILayout.Width(100));
                    GUI.contentColor = Color.white;

                    if (GUILayout.Button(ReinstallText, GUILayout.Width(80))) {
                        ReinstallPackage(package);
                    }
                } else {
                    if (GUILayout.Button(InstallText, GUILayout.Width(80))) {
                        InstallPackage(package);
                    }
                }

                GUILayout.EndHorizontal();

                GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(3));
            }

            EditorGUILayout.EndScrollView();

            GUILayout.Space(10);

            GUI.backgroundColor = Color.green;
            if (GUILayout.Button(CompleteInstallText, GUILayout.Height(50))) {
                CompleteInstallation();
            }

            GUILayout.Space(30);
            GUI.backgroundColor = Color.white;
        }

        private void UpdateAvailablePackages() {
            if (Directory.Exists(PackagesPath)) {
                _availablePackages = new List<string>(Directory.GetFiles(PackagesPath, _packageSearchPattern)
                    .Select(Path.GetFileNameWithoutExtension));
            } else {
                _availablePackages.Clear();
            }
        }

        private void InstallPackage(string packageName) {
            var packagePath = Path.Combine(PackagesPath, packageName + PackageExtension);
            var metaPath = Path.Combine(PackagesPath, packageName + PackageExtension + MetaExtension);
            var installedPackagePath =
                Path.Combine(PackagesPath, packageName + $"{InstalledPackagePostfix}{PackageExtension}");
            var installedMetaPatch =
                Path.Combine(PackagesPath, packageName + $"{InstalledPackagePostfix}{PackageExtension}{MetaExtension}");

            File.Move(packagePath, installedPackagePath);
            File.Move(metaPath, installedMetaPatch);
            AssetDatabase.ImportPackage(installedPackagePath, true);
            Debug.Log($"Package '{packageName}' installed successfully!");
            UpdateAvailablePackages();
        }

        private void ReinstallPackage(string packageName) {
            var installedPackagePath = Path.Combine(PackagesPath, packageName + PackageExtension);
            AssetDatabase.ImportPackage(installedPackagePath, true);
            Debug.Log($"Package '{packageName}' reinstalled successfully!");
            UpdateAvailablePackages();
        }

        private void CompleteInstallation() {
            if (Directory.Exists(PackagesPath)) {
                Directory.Delete(PackagesPath, true);
                File.Delete(PackagesPath + ".meta");
                Debug.Log("Complete installation: 'Assets/BasePackages' folder deleted.");
                AssetDatabase.Refresh();
                CloseAllWindows();
            } else {
                Debug.Log("Complete installation: 'Assets/BasePackages' folder not found.");
            }

            UpdateAvailablePackages();
        }

        private static void CloseAllWindows() {
            var windows = Resources.FindObjectsOfTypeAll<PackageInstaller>();
            foreach (var window in windows) {
                window.Close();
            }
        }
    }
}