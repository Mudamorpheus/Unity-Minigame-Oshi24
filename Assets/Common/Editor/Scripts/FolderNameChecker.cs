using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;

namespace Common.Editor.Scripts {
    public class FolderNameChecker : AssetPostprocessor {
        private const string TargetFolder = "Assets/Game";
        private static string packagesFolder;
        private static readonly char[] CharBlackList = { '_', '!', '-' };

        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths) {
            CheckNames(importedAssets);
        }

        private static void CheckNames(string[] assetPaths) {
            foreach (var assetPath in assetPaths) {
                if (!IsFileOrFolderInTargetFolder(assetPath)) continue;
                var isDirectory = (File.GetAttributes(assetPath) & FileAttributes.Directory) ==
                                  FileAttributes.Directory;
                if (!isDirectory) continue;
                if (assetPath.Contains("ThirdParty")) continue;
                if (assetPath.Contains("Sprites")) continue;
                if (assetPath.Contains("Textures")) continue;
                if (assetPath.Contains("Resources")) continue;
                var folderName = Path.GetFileName(assetPath);
                if (CharBlackList.Any(x => folderName.Contains(x))) {
                    LogError(assetPath);
                }
            }
        }

        private static bool IsFileOrFolderInTargetFolder(string assetPath) {
            return assetPath.StartsWith(TargetFolder);
        }

        private new static void LogError(string path) {
            var errorMessage =
                $"Invalid folder name detected: {path}. Names cannot contain: | {string.Join(" | ", CharBlackList)} | characters";
            Debug.LogError(errorMessage);
        }
    }
}