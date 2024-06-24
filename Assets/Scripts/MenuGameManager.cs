using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;
public class MenuGameManager : MonoBehaviour
{
    [Header("Fields")]
    public TMP_InputField InputX;
    public TMP_InputField InputY;
    [Header("CG")]

    public CanvasGroup PlayContainer_CG;
    public GameObject InputContainerGO;
    [Header("Buttons")]
    public Button StartGameBTN;
    [Header("Vectors")]
    public Vector2 InputX_OriginalPos;
    public Vector2 InputY_OriginalPos;
    public Vector2 StartGameBTN_OriginalPos;
    private void Awake() {
        Initialize();
    }
    void Initialize()
    {
        InputX_OriginalPos = InputX.transform.localPosition;
        InputY_OriginalPos = InputY.transform.localPosition;
        StartGameBTN_OriginalPos = StartGameBTN.transform.localPosition;
    }
    public async void PlayGame()
    {
        PlayContainer_CG.DOFade(0f, 1f).SetEase(Ease.InOutSine);
        PlayContainer_CG.blocksRaycasts = false;
        InputX.transform.localPosition = new Vector2(-7.9f,-838f);
        InputY.transform.localPosition = new Vector2(-7.9f,-838f);
        StartGameBTN.transform.localPosition = new Vector2(-7.9f,-838f);

        InputContainerGO.SetActive(true);
        
        InputX.transform.DOLocalMoveY(InputX_OriginalPos.y, 0.5f).SetEase(Ease.InOutSine).SetDelay(0.4f);
        InputY.transform.DOLocalMoveY(InputY_OriginalPos.y, 0.65f).SetEase(Ease.InOutSine).SetDelay(0.55f);
        StartGameBTN.transform.DOLocalMoveY(StartGameBTN_OriginalPos.y, 0.75f).SetEase(Ease.InOutSine).SetDelay(0.66f);
    }
}
