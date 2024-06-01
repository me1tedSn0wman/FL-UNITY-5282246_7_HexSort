using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameplayUIManager : MonoBehaviour
{
    [Header("Objects")]
    [SerializeField] private SettingsUI settingsUI;
    [SerializeField] private WindowUI infoUI;
    [SerializeField] private GameOverUI gameOverUI;
    [SerializeField] private WindowUI pauseUI;

    [SerializeField] private ScoreUI scoreUI;
   
    [Header("Buttons")]
    [SerializeField] private Button button_Pause;
    [SerializeField] private Button button_BackToMainMenu;
    [SerializeField] private Button button_Settings;
    [SerializeField] private Button button_Info;
    [SerializeField] private Button button_RestartLevel;
    [SerializeField] private Button button_CancelTurn;

    [SerializeField] private Button button_RestoreCameraPosition;

    [Header("Canvas")]
    [SerializeField] private GameObject CancelTurnLockGO;

    [Header("UI Audio Clip")]
    [SerializeField] private AudioClip audioClipUI;
    [SerializeField] private AudioControl audioControl;

    public void Awake()
    {
        settingsUI.SetActive(false);
        infoUI.SetActive(false);
        gameOverUI.SetActive(false);
        pauseUI.SetActive(false);

        settingsUI.OnPlayAudioUI += PlayAudioUI;
        infoUI.OnPlayAudioUI += PlayAudioUI;
        gameOverUI.OnPlayAudioUI += PlayAudioUI;
        pauseUI.OnPlayAudioUI += PlayAudioUI;

        CancelTurnLockGO.SetActive(false);

        button_BackToMainMenu.onClick.AddListener(() =>
        {
            PlayAudioUI();
            GameManager.LOAD_MAIN_MENU_SCENE();
        });
        button_Settings.onClick.AddListener(() =>
        {
            PlayAudioUI();
            settingsUI.SetActive(true);
        });
        button_Info.onClick.AddListener(() =>
        {
            PlayAudioUI();
            infoUI.SetActive(true);
        });

        button_Pause.onClick.AddListener(() =>
        {
            PlayAudioUI();
            pauseUI.SetActive(true);
        });
        button_RestartLevel.onClick.AddListener(() =>
        {
            PlayAudioUI();
            GameplayManager.RESTART();
        });
        button_CancelTurn.onClick.AddListener(() =>
        {
            PlayAudioUI();
            GameplayManager.CANCEL_TURN();
            CancelTurnLockGO.SetActive(true);
        });
        button_RestoreCameraPosition.onClick.AddListener(() =>
        {
            PlayAudioUI();
            GameplayManager.RESTORE_CAMERA_POSITION();
        });

        Init();
    }

    private void Init() { 
    }

    private void PlayAudioUI() { 
        audioControl.PlayOneShot(audioClipUI);
    }

    public void OnDestroy()
    {
        settingsUI.OnPlayAudioUI -= PlayAudioUI;
        infoUI.OnPlayAudioUI -= PlayAudioUI;
        gameOverUI.OnPlayAudioUI -= PlayAudioUI;
    }

    public void UpdateScore(int score) { 
        scoreUI.UpdateScore(score);
    }

    public void GameOver(int score, bool isNewHighScore) {
        gameOverUI.SetValues(
            score,
            isNewHighScore
            );
        gameOverUI.SetActive(true);
    }

    public void UnlockCancelTurn() {
        CancelTurnLockGO.SetActive(false);
    }
}