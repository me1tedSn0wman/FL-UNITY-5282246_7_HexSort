using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YG;

public class GameOverUI : WindowUI
{
    [Header("GameOverUI")]
    [SerializeField] private TextMeshProUGUI text_FinalScoreCount;
    [SerializeField] private TextMeshProUGUI text_NewRecordText;

    [SerializeField] private Button button_Restart;
    [SerializeField] private Button button_BackToMainMenu;

    public new event Action OnPlayAudioUI;

    public override void Awake()
    {
        base.Awake();
        text_NewRecordText.gameObject.SetActive(false);

        button_Restart.onClick.AddListener(() =>
        {
            PlayAudioUI();
            GameplayManager.RESTART();
        });
        button_BackToMainMenu.onClick.AddListener(() =>
        {
            PlayAudioUI();
            GameManager.LOAD_MAIN_MENU_SCENE();
        });
    }



    public void SetValues(int finalScore, bool isHighScore) {
        
        text_FinalScoreCount.text = finalScore.ToString();

        if (isHighScore) text_NewRecordText.gameObject.SetActive(true);
    }

    public override void PlayAudioUI()
    {
        OnPlayAudioUI();
    }
}
