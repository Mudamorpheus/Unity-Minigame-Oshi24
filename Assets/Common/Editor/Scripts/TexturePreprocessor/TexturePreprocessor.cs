using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Common.Editor.Scripts {
    enum TextureCompressionLevel {
        Normal,
        High,
        Maximum
    }

    public class TexturePreprocessor : AssetPostprocessor {
        private static TexturePreprocessorSettings _texturesSettings = null;

        void OnPreprocessTexture() {
            if(_texturesSettings == null) 
                _texturesSettings = Resources.Load<TexturePreprocessorSettings>("TexturePreprocessor/Settings");

            var compressionLevel = TextureCompressionLevel.Maximum;
            
            var importer = assetImporter as TextureImporter;

            if ( importer == null ) {
                return;
            }

            if ( importer.assetPath.Contains("Assets/Game") ) {

                if (importer.assetPath.Contains("BigTextures")) {
                    compressionLevel = TextureCompressionLevel.Normal;
                } else if (importer.assetPath.Contains("MidTextures")) {
                    compressionLevel = TextureCompressionLevel.High;
                }

                TextureSettings textureSettings = _texturesSettings?.texturesSettings.FirstOrDefault
                (e => importer.assetPath == e.textureAssetPath);

                var iosSettings = importer.GetPlatformTextureSettings("IPhone");
                importer.SetPlatformTextureSettings(DefineSettings(iosSettings, textureSettings, compressionLevel));

                var androidSettings = importer.GetPlatformTextureSettings("Android");
                importer.SetPlatformTextureSettings(DefineSettings(androidSettings, textureSettings, compressionLevel));
            }
        }

        static TextureImporterPlatformSettings DefineSettings(TextureImporterPlatformSettings settings, TextureSettings texturesSettings, TextureCompressionLevel compressionLevel) {
            settings.overridden = true;

            if (texturesSettings == null || texturesSettings.maxTextureSize == MaxTextureSize.DEFAULT) {
                switch (compressionLevel) {
                    case TextureCompressionLevel.Normal:
                        settings.maxTextureSize = 2048; 
                        break;
                    case TextureCompressionLevel.High:
                    case TextureCompressionLevel.Maximum:
                        settings.maxTextureSize = 1024;
                        break;
                }
            } else {
                settings.maxTextureSize = texturesSettings.maxTextureIntSize;
            }
            switch (compressionLevel) {
                case TextureCompressionLevel.Normal:
                    settings.format = TextureImporterFormat.ASTC_4x4; 
                    break;
                case TextureCompressionLevel.High:
                    settings.format = TextureImporterFormat.ASTC_5x5; 
                    break;
                case TextureCompressionLevel.Maximum:
                    settings.format = TextureImporterFormat.ASTC_6x6; 
                    break;
            }
            return settings;
        }
    }
}