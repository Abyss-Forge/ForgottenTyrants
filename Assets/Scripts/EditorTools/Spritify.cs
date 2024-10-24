#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

public class Spritify : MonoBehaviour
{
    [MenuItem("CONTEXT/TextureImporter/Spritify", priority = 50)]
    private static void Execute(MenuCommand command)
    {
        TextureImporter textureImporter = command.context as TextureImporter;
        if (textureImporter != null)
        {
            textureImporter.textureType = TextureImporterType.Sprite;

            TextureImporterSettings settings = new TextureImporterSettings();
            textureImporter.ReadTextureSettings(settings);
            settings.spriteMeshType = SpriteMeshType.FullRect;
            textureImporter.SetTextureSettings(settings);

            // Save the changes made to the texture importer
            AssetDatabase.ImportAsset(textureImporter.assetPath, ImportAssetOptions.ForceUpdate);
        }
    }

    [MenuItem("CONTEXT/TextureImporter/Spritify", validate = true)]
    private static bool Validate(MenuCommand command)
    {
        TextureImporter textureImporter = command.context as TextureImporter;
        return textureImporter != null;
    }
}

#endif