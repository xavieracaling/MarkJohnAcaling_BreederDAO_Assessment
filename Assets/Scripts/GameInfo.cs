using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
[CreateAssetMenu(fileName = "GameData", menuName = "ScriptableObjects/GameData", order = 1)]
public class GameInfo  : ScriptableObject
{
    public int XRow;
    public int YColumn;
    public List<ImageTiles> TilesImages = new List<ImageTiles>();

    public Sprite GetTileImage (TileType typeTile) => TilesImages.Where(w => w.TypeTile == typeTile).Select(w => w.TileSprite).Single();

}
[Serializable]
public struct ImageTiles
{
    public TileType TypeTile;
    public Sprite TileSprite;
}