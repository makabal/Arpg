using UnityEditor;
using UnityEngine;

public sealed class PixelArtTextureImporter : AssetPostprocessor
{
    private static bool IsManagedPixelArt(string path)
    {
        return path.StartsWith("Assets/Art/UI/Generated/")
            || path.StartsWith("Assets/Art/UI/SourcePack/")
            || path.StartsWith("Assets/Art/World/Generated/");
    }

    private void OnPreprocessTexture()
    {
        if (!IsManagedPixelArt(assetPath))
            return;

        TextureImporter importer = (TextureImporter)assetImporter;
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.spritePixelsPerUnit = assetPath.StartsWith("Assets/Art/World/") ? 64f : 100f;
        importer.alphaIsTransparency = true;
        importer.mipmapEnabled = false;
        importer.filterMode = FilterMode.Point;
        importer.wrapMode = TextureWrapMode.Clamp;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.crunchedCompression = false;
        importer.npotScale = TextureImporterNPOTScale.None;
        importer.maxTextureSize = 4096;
    }
}
