using System;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "TexturePreprocessorSettings", menuName = "SO/TexturePreprocessorSettings", order = 0)]
public class TexturePreprocessorSettings : ScriptableObject 
{
    public TextureSettings[] texturesSettings;
    public void Prepare()
    {
        for (int i = 0; i < texturesSettings.Length; i++)
        {
            texturesSettings[i].textureAssetPath = AssetDatabase.GetAssetPath(texturesSettings[i].target);
            texturesSettings[i].maxTextureIntSize = (int)texturesSettings[i].maxTextureSize; // For optimization
        }
    }
    private void OnValidate() 
    {
        Prepare();
    }
}

[Serializable]
public class TextureSettings
{
    public Texture target;
    [NonSerialized] public string textureAssetPath;
    public MaxTextureSize maxTextureSize;
    [NonSerialized] public int maxTextureIntSize;
}
public enum MaxTextureSize
{
    DEFAULT = 0,
    _32 = 32,
    _64 = 64,
    _128 = 128,
    _256 = 256,
    _512 = 512,
    _1024 = 1024,
    _2048 = 2048
}