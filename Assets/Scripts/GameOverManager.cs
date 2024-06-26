using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Asyncoroutine;
using DG.Tweening;
public class GameOverManager : MonoBehaviour
{
    [Header("CG")]
    public CanvasGroup UIContainer_CG;
    [Header("UI")]
    public TextMeshProUGUI ScoreUI;
    public TextMeshProUGUI TotalSwapsUI;
    [Header("Scriptable")]
    public GameInfo GameData;
    async void Start()
    {
        ScoreUI.text = $"Score: {GameData.CurrentScore}";
        TotalSwapsUI.text = $"Total Swaps: {GameData.CurrentSwaps}";
        await UIContainer_CG.DOFade(1f, 1f).SetEase(Ease.InOutSine).AsyncWaitForCompletion();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
