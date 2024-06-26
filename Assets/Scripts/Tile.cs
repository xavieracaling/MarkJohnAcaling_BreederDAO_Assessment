using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class Tile : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler
{
    public Vector2 OriginalPosition;
    public Image TileImage;
    public TileType TypeTile;
    [Header("Grid")]
    public int GridXRow;
    public int GridYColumn;
    [Header("4 Near Cross Tiles")]
    public Tile TileUp;
    public Tile TileDown;
    public Tile TileLeft;
    public Tile TileRight;
    public void Initialize(Sprite newSprite, TileType tileType, int gridXRow, int gridYColumn)
    {
        TileImage.sprite = newSprite;
        TypeTile = tileType;
        GridXRow = gridXRow;
        GridYColumn = gridYColumn;
        OriginalPosition = transform.localPosition;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log($"{gameObject.name} down");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        GameManager.Instance.HoveredTile = this;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log($"{GameManager.Instance.HoveredTile.name} up");
        ValidateNearTile(GameManager.Instance.HoveredTile);
    }
    public void CheckValidSwitch()
    {

    }
    private void OnDestroy() {
        GameManager.Instance.RefreshTileEvent?.RemoveListener(RefreshGetNearFourTiles);
    }
    [ContextMenu("GetResultNearFourTiles")]
    public void GetResultNearFourTiles()
    {
        RefreshGetNearFourTiles();
        Debug.Log(TileLeft == null ? "No Tile Left" : $"Tile Left: {TileLeft.gameObject.name}");
        Debug.Log(TileRight == null ? "No Tile Right" : $"Tile Right: {TileRight.gameObject.name}");
        Debug.Log(TileUp == null ? "No Tile Up" : $"Tile Up: {TileUp.gameObject.name}");
        Debug.Log(TileDown == null ? "No Tile Down" : $"Tile Down: {TileDown.gameObject.name}");
    }
    public void RefreshGetNearFourTiles()
    {
        if(GridYColumn > 0 )
            TileLeft = GameManager.Instance.TilesGrid[GridXRow,GridYColumn - 1];

        if(GridYColumn <  GameManager.Instance.TilesGrid.GetLength(1) - 1)
            TileRight = GameManager.Instance.TilesGrid[GridXRow,GridYColumn + 1];

        if(GridXRow >  0)
            TileUp = GameManager.Instance.TilesGrid[GridXRow - 1,GridYColumn ];
        
        if(GridXRow <  GameManager.Instance.TilesGrid.GetLength(0) - 1)
            TileDown = GameManager.Instance.TilesGrid[GridXRow+ 1,GridYColumn ];
    }
    public void ValidateNearTile(Tile targetTileSwitch)
    {
        Tile finalTileToSwitch = null;

        if(targetTileSwitch == TileUp)
            finalTileToSwitch = TileUp;
        
        if(targetTileSwitch == TileDown)
            finalTileToSwitch = TileDown;

        if(targetTileSwitch == TileLeft)
            finalTileToSwitch = TileLeft;

        if(targetTileSwitch == TileRight)
            finalTileToSwitch = TileRight;
        
        if(finalTileToSwitch != null)
        {
            Tile toSwitchTile = targetTileSwitch;
            Tile myTile = this;

            int targetTileSwitchGridX = targetTileSwitch.GridXRow;
            int targetTileSwitchGridY = targetTileSwitch.GridYColumn;

            int myTileSwitchGridX = GridXRow;
            int mySwitchGridY = GridYColumn;
            
            Vector2 myTileVector = OriginalPosition;
            Vector2 targetTileVector = toSwitchTile.OriginalPosition;

            Vector2 myTileVectorTemp = OriginalPosition;
            Vector2 targetTileVectorTemp = toSwitchTile.OriginalPosition;

            GameManager.Instance.TilesGrid[targetTileSwitch.GridXRow, targetTileSwitch.GridYColumn] = null;
            GameManager.Instance.TilesGrid[GridXRow, GridYColumn] = null;

            GameManager.Instance.TilesGrid[targetTileSwitch.GridXRow, targetTileSwitch.GridYColumn] = myTile;
            GameManager.Instance.TilesGrid[GridXRow, GridYColumn] = toSwitchTile;

            myTile.GridXRow = targetTileSwitchGridX;
            myTile.GridYColumn = targetTileSwitchGridY;

            toSwitchTile.GridXRow = myTileSwitchGridX;
            toSwitchTile.GridYColumn = mySwitchGridY;
            
            myTile.transform.DOKill();
            toSwitchTile.transform.DOKill();
            myTile.transform.DOLocalMove(targetTileVector, 0.5f).SetEase(Ease.InOutSine);
            toSwitchTile.transform.DOLocalMove(myTileVector, 0.5f).SetEase(Ease.InOutSine);
            GameManager.Instance.RefreshTileEvent?.Invoke();

            OriginalPosition = targetTileVectorTemp;
            toSwitchTile.OriginalPosition = myTileVectorTemp;
            
            if(myTileVectorTemp.x == targetTileVectorTemp.x) // same vertical switch
            {
                Debug.Log("Switched Vertical");
            }
            if(myTileVectorTemp.y == targetTileVectorTemp.y) // same horizontal switch
            {
                Debug.Log("Switched Horizontal");
            }
            List<Tile> myNumberOfCombinationsTotal = new List<Tile>();
            List<Tile> myNumberOfCombinationsVertical = new List<Tile>();
            List<Tile> myNumberOfCombinationsHorizontal = new List<Tile>();
            
            List<Tile> tagetNumberOfCombinationsTotal = new List<Tile>();
            List<Tile> tagetNumberOfCombinationsVertical = new List<Tile>();
            List<Tile> tagetNumberOfCombinationsHorizontal = new List<Tile>();
            
            myNumberOfCombinationsTotal.Add(this);
            tagetNumberOfCombinationsTotal.Add(toSwitchTile);

            myNumberOfCombinationsVertical.Add(this);
            myNumberOfCombinationsHorizontal.Add(this);

            tagetNumberOfCombinationsVertical.Add(toSwitchTile);
            tagetNumberOfCombinationsHorizontal.Add(toSwitchTile);


            ValidateSingleCombination(myNumberOfCombinationsVertical,TypeTile,TileDown,true);
            ValidateSingleCombination(myNumberOfCombinationsVertical,TypeTile,TileUp,true);

            ValidateSingleCombination(myNumberOfCombinationsHorizontal,TypeTile,TileLeft);
            ValidateSingleCombination(myNumberOfCombinationsHorizontal,TypeTile,TileRight);
        

            ValidateSingleCombination(tagetNumberOfCombinationsVertical,toSwitchTile.TypeTile,toSwitchTile.TileDown,true);
            ValidateSingleCombination(tagetNumberOfCombinationsVertical,toSwitchTile.TypeTile,toSwitchTile.TileUp,true);

            ValidateSingleCombination(tagetNumberOfCombinationsHorizontal,toSwitchTile.TypeTile,toSwitchTile.TileLeft);
            ValidateSingleCombination(tagetNumberOfCombinationsHorizontal,toSwitchTile.TypeTile,toSwitchTile.TileRight);

            
            // ValidateSingleCombination(tagetNumberOfCombinationsTotal,toSwitchTile.TypeTile,toSwitchTile.TileLeft);
            // ValidateSingleCombination(tagetNumberOfCombinationsTotal,toSwitchTile.TypeTile,toSwitchTile.TileRight);
            // ValidateSingleCombination(tagetNumberOfCombinationsTotal,toSwitchTile.TypeTile,toSwitchTile.TileDown);
            // ValidateSingleCombination(tagetNumberOfCombinationsTotal,toSwitchTile.c.TileUp);
        

            Debug.Log($"tagetNumberOfCombinationsVertical {tagetNumberOfCombinationsVertical.Count} {toSwitchTile.TypeTile.ToString()}");
            Debug.Log($"tagetNumberOfCombinationsHorizontal {tagetNumberOfCombinationsHorizontal.Count} {toSwitchTile.TypeTile.ToString()}");

            Debug.Log($"myNumberOfCombinationsVertical {myNumberOfCombinationsVertical.Count} {TypeTile}");
            Debug.Log($"myNumberOfCombinationsHorizontal {myNumberOfCombinationsHorizontal.Count} {TypeTile}");
            if(myNumberOfCombinationsVertical.Count >= 3)
            {
                foreach (Tile item in myNumberOfCombinationsVertical)
                {
                    item.transform.DOScale(0,0.5f).SetEase(Ease.InOutBack).SetDelay(0.3f);
                }
            }
            if(myNumberOfCombinationsHorizontal.Count >= 3)
            {
                foreach (Tile item in myNumberOfCombinationsHorizontal)
                {
                    item.transform.DOScale(0,0.5f).SetEase(Ease.InOutBack).SetDelay(0.3f);
                }
            }

            if(tagetNumberOfCombinationsVertical.Count >= 3)
            {
                foreach (Tile item in tagetNumberOfCombinationsVertical)
                {
                    item.transform.DOScale(0,0.5f).SetEase(Ease.InOutBack).SetDelay(0.3f);
                }
            }
            if(tagetNumberOfCombinationsHorizontal.Count >= 3)
            {
                foreach (Tile item in tagetNumberOfCombinationsHorizontal)
                {
                    item.transform.DOScale(0,0.5f).SetEase(Ease.InOutBack).SetDelay(0.3f);
                }
            }
        }
    }
    public void ValidateSingleCombination(List<Tile> PassedOriginalListTiles, TileType originalTileType, Tile targetTile, bool vertical = false) // created a recursive checks
    {
        if(targetTile != null)
        {
            if(targetTile.TypeTile == originalTileType)
            {
                bool repetitiveTile = false;
                for (int i = 0; i < PassedOriginalListTiles.Count; i++)
                {
                    if(PassedOriginalListTiles[i] == targetTile)
                    {
                        repetitiveTile = true;
                        break;
                    }
                }

                if(!repetitiveTile)
                {
                    PassedOriginalListTiles.Add(targetTile);
                    if(vertical)
                    {
                        ValidateSingleCombination(PassedOriginalListTiles,originalTileType,targetTile.TileDown, vertical);
                        ValidateSingleCombination(PassedOriginalListTiles,originalTileType,targetTile.TileUp, vertical);
                    }
                    else 
                    {
                        ValidateSingleCombination(PassedOriginalListTiles,originalTileType,targetTile.TileLeft);
                        ValidateSingleCombination(PassedOriginalListTiles,originalTileType,targetTile.TileRight);
                    }

                }
                    
            }
        }
        
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
