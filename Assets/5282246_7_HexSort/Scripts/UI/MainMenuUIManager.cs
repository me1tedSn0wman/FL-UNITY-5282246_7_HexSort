using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUIManager : MonoBehaviour
{
    [Header("Objects")]
    [SerializeField] private SettingsUI settingsUI;
    [SerializeField] private WindowUI infoUI;
    [SerializeField] private LeaderboardUI leaderboardUI;

    [Header("Buttons")]
    [SerializeField] private Button button_Settings;
    [SerializeField] private Button button_Info;
    [SerializeField] private Button button_LeaderBoard;

    [Header("Buttons Difficulty")]
    [SerializeField] private Button button_Easy;
    [SerializeField] private Button button_Normal;
    [SerializeField] private Button button_Hard;

    [SerializeField] private Button button_StartGame;
    [Header("UI Audio Clip")]

    [SerializeField] private AudioClip audioClipUI;
    [SerializeField] private AudioControl audioControl;

    public void Awake()
    {
        settingsUI.SetActive(false);
        infoUI.SetActive(false);
        leaderboardUI.SetActive(false);

        settingsUI.OnPlayAudioUI += PlayAudioUI;
        infoUI.OnPlayAudioUI += PlayAudioUI;
        leaderboardUI.OnPlayAudioUI += PlayAudioUI;


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
        button_LeaderBoard.onClick.AddListener(() => 
        {
            PlayAudioUI();
            leaderboardUI.SetActive(true);
        });

        button_StartGame.onClick.AddListener(() =>
        {
            PlayAudioUI();
            GameManager.LOAD_GAMEPLAY_SCENE();
        });

        /*
         Difficulty
         */

        button_Easy.onClick.AddListener(() =>
        {
            PlayAudioUI();
            button_Easy.gameObject.SetActive(false);
            button_Normal.gameObject.SetActive(true);
            GameManager.Instance.gameDifficulty = GameDifficulty.Normal;
        });

        button_Normal.onClick.AddListener(() =>
        {
            PlayAudioUI();
            button_Normal.gameObject.SetActive(false);
            button_Hard.gameObject.SetActive(true);
            GameManager.Instance.gameDifficulty = GameDifficulty.Hard;
        });

        button_Hard.onClick.AddListener(() =>
        {
            PlayAudioUI();
            button_Hard.gameObject.SetActive(false);
            button_Easy.gameObject.SetActive(true);
            GameManager.Instance.gameDifficulty = GameDifficulty.Easy;
        });

    }

    public void Start() {
        Init();
    }

    private void Init() {
        switch (GameManager.Instance.gameDifficulty) {
            case GameDifficulty.Easy:
                button_Easy.gameObject.SetActive(true);
                button_Normal.gameObject.SetActive(false);
                button_Hard.gameObject.SetActive(false);
                break;
            case GameDifficulty.Normal:
                button_Easy.gameObject.SetActive(false);
                button_Normal.gameObject.SetActive(true);
                button_Hard.gameObject.SetActive(false);
                break;
            case GameDifficulty.Hard:
                button_Easy.gameObject.SetActive(false);
                button_Normal.gameObject.SetActive(false);
                button_Hard.gameObject.SetActive(true);
                break;
        }
    }

    private void PlayAudioUI() { 
        audioControl.PlayOneShot(audioClipUI);
    }

    public void OnDestroy()
    {
        settingsUI.OnPlayAudioUI -= PlayAudioUI;
        infoUI.OnPlayAudioUI -= PlayAudioUI;
    }
}
