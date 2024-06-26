using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Asyncoroutine;
using System.Linq;
public class Tile : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler
{
    public Vector2 OriginalScale;
    public Vector2 OriginalPosition;
    public Image TileImage;
    public TileType TypeTile;
    public bool UpperHalfVerticalCategory;
    [Header("Grid")]
    public int GridXRow;
    public int GridYColumn;
    [Header("4 Near Cross Tiles")]
    public Tile TileUp;
    public Tile TileDown;
    public Tile TileLeft;
    public Tile TileRight;
    public bool Empty;
    public void Initialize(Sprite newSprite, TileType tileType, int gridXRow, int gridYColumn)
    {
        TileImage.sprite = newSprite;
        TypeTile = tileType;
        GridXRow = gridXRow;
        GridYColumn = gridYColumn;
        OriginalPosition = transform.localPosition;
       
        if(GridXRow < GameManager.Instance.GameData.UpperMaxValue)
            UpperHalfVerticalCategory =true;
        OriginalScale = transform.localScale;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log($"{gameObject.name} down");
        if(GameManager.Instance.GameOver) return;
        if(!GameManager.Instance.TapEnable) return;
        AudioManager.Instance.Clicked.Play();
        transform.DOKill();
        transform.DOScale(OriginalScale * 1.15f,0.3f).SetEase(Ease.OutSine);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        GameManager.Instance.HoveredTile = this;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if(GameManager.Instance.GameOver) return;
        if(!GameManager.Instance.TapEnable) return;
        transform.DOKill();
        transform.DOScale(OriginalScale ,0.2f).SetEase(Ease.InOutSine);
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
        TileLeft = null;
        TileRight = null;
        TileUp = null;
        TileDown = null;
        if(GridYColumn > 0 )
            TileLeft = GameManager.Instance.TilesGrid[GridXRow,GridYColumn - 1];

        if(GridYColumn <  GameManager.Instance.TilesGrid.GetLength(1) - 1)
            TileRight = GameManager.Instance.TilesGrid[GridXRow,GridYColumn + 1];

        if(GridXRow >  0)
            TileUp = GameManager.Instance.TilesGrid[GridXRow - 1,GridYColumn ];
        
        if(GridXRow <  GameManager.Instance.TilesGrid.GetLength(0) - 1)
            TileDown = GameManager.Instance.TilesGrid[GridXRow+ 1,GridYColumn ];
    }
    public void SetActiveGO() 
    {
        gameObject.SetActive(true);
        Debug.Log($"SetActive to {gameObject.activeSelf} with {gameObject.name}" );
    } 
    async Task switchTile(Tile toSwitchTile, Tile myTile)
    {
        
        GameManager.Instance.TapEnable = false;
        Task switchedProgress = null;
        int targetTileSwitchGridX = toSwitchTile.GridXRow;
        int targetTileSwitchGridY = toSwitchTile.GridYColumn;

        int myTileSwitchGridX = GridXRow;
        int mySwitchGridY = GridYColumn;
        
        Vector2 myTileVector = OriginalPosition;
        Vector2 targetTileVector = toSwitchTile.OriginalPosition;

        Vector2 myTileVectorTemp = OriginalPosition;
        Vector2 targetTileVectorTemp = toSwitchTile.OriginalPosition;

        GameManager.Instance.TilesGrid[toSwitchTile.GridXRow, toSwitchTile.GridYColumn] = null;
        GameManager.Instance.TilesGrid[GridXRow, GridYColumn] = null;

        GameManager.Instance.TilesGrid[toSwitchTile.GridXRow, toSwitchTile.GridYColumn] = myTile;
        GameManager.Instance.TilesGrid[GridXRow, GridYColumn] = toSwitchTile;

        myTile.GridXRow = targetTileSwitchGridX;
        myTile.GridYColumn = targetTileSwitchGridY;

        toSwitchTile.GridXRow = myTileSwitchGridX;
        toSwitchTile.GridYColumn = mySwitchGridY;
        
        // myTile.transform.DOKill();
        // toSwitchTile.transform.DOKill();

        switchedProgress = myTile.transform.DOLocalMove(targetTileVector, 0.5f).SetEase(Ease.InOutSine).AsyncWaitForCompletion();
        toSwitchTile.transform.DOLocalMove(myTileVector, 0.5f).SetEase(Ease.InOutSine);
        AudioManager.Instance.Switch.Play();

        GameManager.Instance.RefreshTileEvent?.Invoke();

        OriginalPosition = targetTileVectorTemp;
        toSwitchTile.OriginalPosition = myTileVectorTemp;

        await switchedProgress;
    }
    public async void ValidateNearTile(Tile targetTileSwitch)
    {
        Task switchedProgress = null;
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
            if(!finalTileToSwitch.Empty)
            {
                Tile toSwitchTile = targetTileSwitch;
                Tile myTile = this;
                switchedProgress = switchTile(toSwitchTile, this);    
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

          

                Debug.Log($"tagetNumberOfCombinationsVertical {tagetNumberOfCombinationsVertical.Count} {toSwitchTile.TypeTile.ToString()}");
                Debug.Log($"tagetNumberOfCombinationsHorizontal {tagetNumberOfCombinationsHorizontal.Count} {toSwitchTile.TypeTile.ToString()}");

                Debug.Log($"myNumberOfCombinationsVertical {myNumberOfCombinationsVertical.Count} {TypeTile}");
                Debug.Log($"myNumberOfCombinationsHorizontal {myNumberOfCombinationsHorizontal.Count} {TypeTile}");

                bool matchA = MatchedTiles(myNumberOfCombinationsVertical);
                bool matchB = MatchedTiles(myNumberOfCombinationsHorizontal);

                bool matchC = MatchedTiles(tagetNumberOfCombinationsVertical);
                bool matchD = MatchedTiles(tagetNumberOfCombinationsHorizontal);
          
                if(matchA || matchB || matchC || matchD)
                {
                    Debug.Log("Found a match!");
                    AudioManager.Instance.Matched.Play();
                    GameManager.Instance.GameData.CurrentScore += (myNumberOfCombinationsVertical.Count - 1  + myNumberOfCombinationsHorizontal.Count) * 10;
                    GameManager.Instance.UpdateUI();
                    await new WaitForSeconds(0.95f);
                    GameManager.Instance.GenerateMissingTilesUpOrDown();
                }
                else
                {
                    await switchedProgress;
                    await switchTile(toSwitchTile, this); 
                    GameManager.Instance.TapEnable = true;
                    Debug.Log("NO combination");
                }
            }
            
        }
    }
    public async void ValidateMyWholeSingleCombination(Tile tile)
    {
        List<Tile> myNumberOfCombinationsVertical = new List<Tile>();
        List<Tile> myNumberOfCombinationsHorizontal = new List<Tile>();
        
        myNumberOfCombinationsVertical.Add(tile);
        myNumberOfCombinationsHorizontal.Add(tile);

        tile.ValidateSingleCombination(myNumberOfCombinationsVertical,tile.TypeTile,tile.TileDown,true);
        tile.ValidateSingleCombination(myNumberOfCombinationsVertical,tile.TypeTile,tile.TileUp,true);

        tile.ValidateSingleCombination(myNumberOfCombinationsHorizontal,tile.TypeTile,tile.TileLeft);
        tile.ValidateSingleCombination(myNumberOfCombinationsHorizontal,tile.TypeTile,tile.TileRight);
        
        bool matchA = MatchedTiles(myNumberOfCombinationsVertical);
        bool matchB = MatchedTiles(myNumberOfCombinationsHorizontal);

        if(matchA || matchB )
        {
            Debug.Log("Found a match!");
            AudioManager.Instance.Matched.Play();
            GameManager.Instance.GameData.CurrentScore += (myNumberOfCombinationsVertical.Count - 1 + myNumberOfCombinationsHorizontal.Count) * 10 ;
            GameManager.Instance.UpdateUI();
            await new WaitForSeconds(0.95f);
            GameManager.Instance.GenerateMissingTilesUpOrDown();
        }
        
    }
    public bool MatchedTiles(List<Tile> listOfTiles)
    {
        bool match = listOfTiles.Count >= 3; 
        if(GameManager.Instance.TapEnable) 
            GameManager.Instance.TapEnable = false;
        if(listOfTiles.Count >= 3)
        {
            foreach (Tile item in listOfTiles)
            {
                item.Empty = true;
                item.transform.DOScale(0,0.5f).SetEase(Ease.InOutSine).SetDelay(0.3f).OnComplete(() => 
                {
                    if(item.Empty)
                    {
                        item.gameObject.SetActive(false);
                    }
                });
                //Destroy(item);
            }
        }
        return match;

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
