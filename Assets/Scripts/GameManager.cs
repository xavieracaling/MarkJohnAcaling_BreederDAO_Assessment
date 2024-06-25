using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update//
    public GameObject TilePrefab;
    public Transform TilesContainer;
    [Header("Scriptable")]
    public GameInfo GameData;
    [Header("CG")]
    public CanvasGroup UIContainer_CG;
    private void Awake() {
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
        for (int x = 0; x < GameData.XRow; x++)
        {
            int xColumnStartingPosition = x * -100;
            int xColumnNewPosition = xColumnStartingPosition +(GameData.XRow - 1) * 50; // my simple logic for centering the column tiles for y pos
            for (int y = 0; y < GameData.YColumn; y++)
            {
                int yColumnStartingPosition = y * 100;
                int yColumnNewPosition = yColumnStartingPosition -(GameData.YColumn - 1) * 50; // my simple logic for centering the column tiles for x pos
                GameObject tile = Instantiate(TilePrefab,TilesContainer);
                tile.transform.localPosition = new Vector2(yColumnNewPosition,xColumnNewPosition); //
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
