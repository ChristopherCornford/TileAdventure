using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;


[CreateAssetMenu]
public class VariantTiles : Tile
{
    public VariantType variantType;

    public override void RefreshTile(Vector3Int location, ITilemap tilemap)
    {
        base.RefreshTile(location, tilemap);
        {
        }
    }

    public override void GetTileData(Vector3Int location, ITilemap tilemap, ref TileData tileData)
    {
        base.GetTileData(location, tilemap, ref tileData);
        {
        }
    }
#if UNITY_EDITOR
    public static void CreateVariantTile()
    {
        string path = EditorUtility.SaveFilePanelInProject("Save Variant Tile", "New Variant Tile", "Asset", "Save New Variant Tile", "Assets");

        if (path == "")
        {
            return;
        }

        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<VariantTiles>(), path);
    }
#endif

    public enum VariantType
    {
        Town = 0,
        Enemy,
        Traveler,
        Treasure,
        Boss,
    }
}

