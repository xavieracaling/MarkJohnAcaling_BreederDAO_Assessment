using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
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

    // Update is called once per frame
    void Update()
    {
        
    }
}
