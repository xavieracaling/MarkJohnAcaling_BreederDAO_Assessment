using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.Events;
public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update//
    public Tile[,] TilesGrid;
    public GameObject TilePrefab;
    public Transform TilesContainer;
    public static GameManager Instance;
    public Tile HoveredTile;
    public UnityEvent RefreshTileEvent;
    [Header("Scriptable")]
    public GameInfo GameData;
    [Header("CG")]
    public CanvasGroup UIContainer_CG;
    private void Awake() {
        Instance = this;
        UIContainer_CG.alpha = 0;
    }
    async void Start()
    {
        await UIContainer_CG.DOFade(1f, 1f).SetEase(Ease.InOutSine).AsyncWaitForCompletion();
    }
    [ContextMenu("GenerateTiles")]
    public void GenerateTiles()
    {
        if(TilesContainer.childCount > 0)
        {
            for (int i = TilesContainer.childCount - 1; i >= 0; i--)
            {
                Destroy(TilesContainer.GetChild(i).gameObject);
            }
        }
        TilesGrid = new Tile[GameData.XRow,GameData.YColumn];
        float newWidth = TilePrefab.GetComponent<RectTransform>().sizeDelta.x * GameData.YColumn;
        float newHeight = TilePrefab.GetComponent<RectTransform>().sizeDelta.y * GameData.XRow;
        RectTransform tileRect = TilesContainer.GetComponent<RectTransform>();

        tileRect.sizeDelta = new Vector2(newWidth,newHeight); 
        
        if(GameData.XRow % 2 == 0)
            GameData.UpperMaxValue =  GameData.XRow / 2;
        else 
            GameData.UpperMaxValue =  (GameData.XRow / 2) + 1 ; 

        for (int x = 0; x < GameData.XRow; x++)
        { 
            int xColumnStartingPosition = x * -100;
            int xColumnNewPosition = xColumnStartingPosition +(GameData.XRow - 1) * 50; // my simple logic for centering the column tiles for y pos
            for (int y = 0; y < GameData.YColumn; y++)
            {
                int yColumnStartingPosition = y * 100;
                int yColumnNewPosition = yColumnStartingPosition -(GameData.YColumn - 1) * 50; // my simple logic for centering the column tiles for x pos
                GameObject tile = Instantiate(TilePrefab,TilesContainer);
                Tile tileScript = tile.GetComponent<Tile>();

                tile.transform.localPosition = new Vector2(yColumnNewPosition,xColumnNewPosition); //

                TileType typeTile;
                int randomTile = UnityEngine.Random.Range(0,Enum.GetValues(typeof(TileType)).Length);
                typeTile = (TileType)randomTile;

                tile.name = $"Tile[{x}][{y}] [{typeTile.ToString()}]";

                Sprite tileSprite = GameData.GetTileImage(typeTile);
                tileScript.Initialize(tileSprite,typeTile, x, y);
                TilesGrid[x,y] = tileScript;
                RefreshTileEvent?.AddListener(tileScript.RefreshGetNearFourTiles);
            }
        }
        RefreshTileEvent?.Invoke();
    }
    [ContextMenu("CallRefreshEventTiles")]
    public void CallRefreshEventTiles() => RefreshTileEvent?.Invoke();
    [ContextMenu("GenerateMissingTilesUpOrDown")]

    public void GenerateMissingTilesUpOrDown()
    {
        int called = 0;
        for (int y = 0; y < GameData.YColumn; y++)
        {
            int yColumn = y;
            int xRow = 0;

            //upper half


            List<(int,int)> upperHalfEmptyTilesToFill = new List<(int,int)>();
            List<(int,int)> upperHalfContainedTiles = new List<(int,int)>();

            for (int upperHalf = GameData.UpperMaxValue - 1; upperHalf >= 0; upperHalf--)
            {
                xRow = upperHalf;
                if(TilesGrid[xRow,yColumn].Empty)
                {
                    upperHalfEmptyTilesToFill.Add((xRow, yColumn));
                    Debug.Log($"Not enabled tile : xRow {xRow}, yColumn{yColumn}");
                }
            }
            // for (int i = 0; i < upperHalfEmptyTilesToFill.Count; i++)
            // {
            //     Vector2 targetMovePosition = TilesGrid[upperHalfEmptyTilesToFill[i].Item1,upperHalfEmptyTilesToFill[i].Item2].OriginalPosition;
            //     Transform containedTile = TilesGrid[upperHalfContainedTiles[i].Item1,upperHalfContainedTiles[i].Item2].transform;
            //     containedTile.DOLocalMove(targetMovePosition,0.5f).SetEase(Ease.InOutQuint);
            //     Debug.Log($"to move from  {i + 1}");
            // }
            if(upperHalfEmptyTilesToFill.Count > 0)
            {
                for (int upperHalf = GameData.UpperMaxValue - 1; upperHalf >= 0; upperHalf--)
                {
                    xRow = upperHalf;
                    if(!TilesGrid[xRow,yColumn].Empty)
                    {
                        upperHalfContainedTiles.Add((xRow, yColumn));
                    }
                }
                for (int i = 0; i < upperHalfContainedTiles.Count; i++)
                {
                    Vector2 targetMovePosition = TilesGrid[upperHalfEmptyTilesToFill[i].Item1,upperHalfEmptyTilesToFill[i].Item2].OriginalPosition;
                    Transform containedTile = TilesGrid[upperHalfContainedTiles[i].Item1,upperHalfContainedTiles[i].Item2].transform;
                    containedTile.DOLocalMove(targetMovePosition,0.5f).SetEase(Ease.InOutQuint);
                    Debug.Log($"tiles of upper half  grid  Grid({upperHalfContainedTiles[i].Item1},{upperHalfContainedTiles[i].Item2})");
                }
            }
            


        }
    }
}
