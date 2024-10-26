using UnityEditor;

namespace Common.Editor.Scripts {
    public class AudioPreprocessor : AssetPostprocessor {
        void OnPreprocessAudio() {
            var importer = assetImporter as AudioImporter;

            if ( importer == null ) {
                return;
            }
            
            if (importer.assetPath.Contains("Assets/Game")) {
                importer.forceToMono = true;
                    
                var iosSettings = importer.GetOverrideSampleSettings("IPhone");
                importer.SetOverrideSampleSettings("IPhone", DefineSettings(iosSettings));
                    
                var androidSettings = importer.GetOverrideSampleSettings("Android");
                importer.SetOverrideSampleSettings("Android", DefineSettings(androidSettings));
            }
        }
        static AudioImporterSampleSettings DefineSettings(AudioImporterSampleSettings settings) {
            settings.quality = 0;
            settings.sampleRateSetting = AudioSampleRateSetting.OverrideSampleRate;
            settings.sampleRateOverride = 22050;
            return settings;
        }
    }
}