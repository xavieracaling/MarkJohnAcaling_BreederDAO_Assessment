using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.Events;
using Asyncoroutine;
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

    public float YUpperHide;
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
                
                GenerateTile(x,y,xColumnNewPosition, yColumnNewPosition);
            }
        }
        YUpperHide = TilesGrid[0,0].OriginalPosition.y + 100;
        RefreshTileEvent?.Invoke();
    }
    public void GenerateTile(int x, int y, int xColumnNewPosition, int yColumnNewPosition)
    {
            
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
    [ContextMenu("CallRefreshEventTiles")]
    public void CallRefreshEventTiles() => RefreshTileEvent?.Invoke();
    [ContextMenu("GenerateMissingTilesUpOrDown")]

    public async void GenerateMissingTilesUpOrDown()
    {
        int called = 0;
        for (int y = 0; y < GameData.YColumn; y++)
        {
            int yColumn = y;
            int xRow = 0;

            //upper half
            List<(int,int,Vector2, Tile,GameObject)> upperHalfEmptyTilesToFill = new List<(int,int,Vector2, Tile,GameObject)>();
            List<(int,int,Vector2, Tile)> upperHalfContainedTiles = new List<(int,int,Vector2, Tile)>();
            for (int upperHalf = GameData.UpperMaxValue - 1; upperHalf >= 0; upperHalf--) //getting tiles of upperHalfEmptyTilesToFill
            {
                xRow = upperHalf;
                if(TilesGrid[xRow,yColumn].Empty)
                {
                    Tile checkTile = TilesGrid[xRow,yColumn];
                    upperHalfEmptyTilesToFill.Add((xRow, yColumn,TilesGrid[xRow,yColumn].OriginalPosition,checkTile, TilesGrid[xRow,yColumn].gameObject));
                }
            }
         
            if(upperHalfEmptyTilesToFill.Count > 0) //if there is empty tile on a vertical
            {
                int lastRowUpperHalfEmpty = upperHalfEmptyTilesToFill[upperHalfEmptyTilesToFill.Count - 1].Item1;
                for (int upperHalf = lastRowUpperHalfEmpty ; upperHalf >= 0; upperHalf--) //getting remaining tiles if there are on the vertical grid
                {
                    xRow = upperHalf;
                    
                    if(!TilesGrid[xRow,yColumn].Empty)
                    {
                        Tile checkTile = TilesGrid[xRow,yColumn];

                        upperHalfContainedTiles.Add((xRow, yColumn,TilesGrid[xRow,yColumn].OriginalPosition, checkTile));
                    }
                }

            
                for (int i = 0; i < upperHalfContainedTiles.Count; i++) //contained tiles go down
                {
                    Tile originalTile = upperHalfContainedTiles[i].Item4;
                    if(i == 0 ) // just base it on the first empty tiles only then container tiles data will follow that pattern
                    {
                        originalTile.GridXRow = upperHalfEmptyTilesToFill[i].Item1;
                        originalTile.GridYColumn = upperHalfEmptyTilesToFill[i].Item2;
                        originalTile.OriginalPosition = upperHalfEmptyTilesToFill[i].Item3;
                        
                    }
                    else 
                    {
                        originalTile.GridXRow = upperHalfContainedTiles[i - 1].Item1;
                        originalTile.GridYColumn = upperHalfContainedTiles[i - 1].Item2;
                        originalTile.OriginalPosition = upperHalfContainedTiles[i - 1].Item3;

                    }

                    TilesGrid[originalTile.GridXRow, originalTile.GridYColumn ] = originalTile;
                    originalTile.transform.DOLocalMove(originalTile.OriginalPosition,0.25f).SetEase(Ease.InOutSine);
                }

                for (int i = 0; i < upperHalfEmptyTilesToFill.Count; i++) //empty tiles will fill on top
                {
                    Debug.Log($"EMPTY TILES TO FILL {i}");
                    Tile emptyTile = upperHalfEmptyTilesToFill[i].Item4;
                    emptyTile.Empty = false;
                    GameObject tileGO = upperHalfEmptyTilesToFill[i].Item5;
                    Debug.Log($"is empty tile null {emptyTile == null}, {i}");
                    if(upperHalfContainedTiles.Count - 1  - i >= 0)
                    {
                        emptyTile.GridXRow = upperHalfContainedTiles[upperHalfContainedTiles.Count - 1  - i].Item1;
                        emptyTile.GridYColumn = upperHalfContainedTiles[upperHalfContainedTiles.Count - 1  - i].Item2;
                        emptyTile.OriginalPosition = upperHalfContainedTiles[upperHalfContainedTiles.Count - 1  - i].Item3;


                    }   
                    else 
                    {
                        emptyTile.GridXRow = upperHalfEmptyTilesToFill[i].Item1;
                        emptyTile.GridYColumn = upperHalfEmptyTilesToFill[i].Item2;
                        emptyTile.OriginalPosition = upperHalfEmptyTilesToFill[i].Item3;

                    }
                    emptyTile.transform.DOKill();
                    emptyTile.transform.localPosition = new Vector2(emptyTile.OriginalPosition.x,  YUpperHide);
                    emptyTile.transform.localScale = Vector3.one;

                    emptyTile.gameObject.name = "new move !!";
                    emptyTile.gameObject.SetActive(true);
                    Debug.Log($"new tile empty fill {emptyTile.gameObject.name}, {i}");

                    TilesGrid[emptyTile.GridXRow, emptyTile.GridYColumn ] = emptyTile;
                    emptyTile.transform.DOLocalMove(emptyTile.OriginalPosition,0.25f).SetEase(Ease.InOutSine).SetDelay(0.1f);
                }

            }
                RefreshTileEvent?.Invoke();
            


        }
    }
}
