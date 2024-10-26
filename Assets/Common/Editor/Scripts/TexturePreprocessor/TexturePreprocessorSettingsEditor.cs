using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TexturePreprocessorSettings))]
public class TexturePreprocessorSettingsEditor : Editor {
    private bool _showInfo = false;
    public override void OnInspectorGUI() 
    {
        if(GUILayout.Button("Reimport all"))
        {
            var src = target as TexturePreprocessorSettings;
            TextureSettings[] textureSettings = src.texturesSettings;
            src.Prepare();

            for (int i = 0; i < textureSettings.Length; i++)
            {
                AssetDatabase.ImportAsset(textureSettings[i].textureAssetPath, ImportAssetOptions.Default);
            }
        }

        base.OnInspectorGUI();

        EditorGUILayout.Space(5);
        _showInfo = EditorGUILayout.Foldout(_showInfo, "Info");
        if (_showInfo) 
        {
            EditorGUILayout.LabelField("1. Add the item to the Texture Settings\n\n2. Place the texture in the Target field\n\n3. Select the maximum size of the texture (DEFAULT - means that the size will be set\n to the default value in the Texture Preprocessor)\n\n4. Click Reimoprt all to regenerate *.meta files for the whole list of textures.\nTo reimport just one texture, click RMB on the texture and select Reimport.", GUILayout.Height(150));
        }
    }
}
