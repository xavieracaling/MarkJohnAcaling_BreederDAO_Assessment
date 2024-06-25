using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class Tile : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler
{
    public Image TileImage;
    public TileType TypeTile;

    public void Initialize(Sprite newSprite, TileType tileType)
    {
        TileImage.sprite = newSprite;
        TypeTile = tileType;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log($"{gameObject.name} [{TypeTile.ToString()}] down");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        GameManager.Instance.HoveredTile = this;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log($"{GameManager.Instance.HoveredTile.name} [{TypeTile.ToString()}] up");

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
