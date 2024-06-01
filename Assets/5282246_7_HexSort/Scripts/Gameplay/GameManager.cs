using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;
using Utils.TextControl;
using YG;

public enum GameState { 
    PreGame,
    MainMenu,
    Pause,
    GamePlay,
    GameOver,
}

public enum GameDifficulty { 
    Easy,
    Normal,
    Hard,
}

public class GameManager : Soliton<GameManager>
{
    public static bool IS_MOBILE;
    [SerializeField] private bool simulateMobile = false;

    [SerializeField] private GameState _gameState;
    public GameState gameState {
        get { return _gameState; }
        set { _gameState = value; }
    }

    [Header("Difficulty")]
    [SerializeField] public GameDifficulty gameDifficulty;

    [Header("Screen Size")]
    public Vector2Int screenSize;
    public static event Action<Vector2Int> OnScreenSizeChanged;

    

    [Header("Score Management")]
    public int highScore = 0;

    [SerializeField] private string scoreLeaderBoardName = "scoreList";
    public string lang;

    private static float _animationSpeed = 1f;
    public static float animationSpeed {
        get { return _animationSpeed; }
        set { 
            _animationSpeed = value;
            OnAnimationSpeedChanged(value);
        }
    }
    public static event Action<float> OnAnimationSpeedChanged;


    public override void Awake() { 
        base.Awake();

        gameState = GameState.PreGame;
        IS_MOBILE = false
            || simulateMobile;

        gameDifficulty = GameDifficulty.Normal;
        screenSize = new Vector2Int(Screen.width, Screen.height);

        YandexGame.GetDataEvent += GetLoadDataYG;

        lang = YandexGame.EnvironmentData.language;
        Debug.Log(YandexGame.EnvironmentData.language);
        SET_LANGUAGE(lang);

        gameState = GameState.MainMenu;
    }

    public void Update()
    {
        UpdateScreenSize();
    }

    #region SCORE

    public void UpdateHighScore(int score) {
        if (score <= highScore) return;
        highScore = score;
        SaveDataYG();
        YandexGame.NewLeaderboardScores(Instance.scoreLeaderBoardName, score);
    }

    #endregion SCORE

    #region TECH_MANAGEMENT

    private void UpdateScreenSize() {
        Vector2Int newScreenSize = new Vector2Int(Screen.width, Screen.height);
        if (screenSize != newScreenSize) {
            OnScreenSizeChanged(newScreenSize);
            screenSize = newScreenSize;
        }
    }

    #endregion TECH_MANAGEMENT

    #region SAVES

    public void SaveDataYG() {
        YandexGame.savesData.highScore = highScore;
        Debug.Log("High Score:" + highScore);
        YandexGame.SaveProgress();
    }

    public void GetLoadDataYG() {
        highScore = 0;
        highScore = YandexGame.savesData.highScore;
    }

    #endregion SAVES

    #region SCENE_MANAGEMENT

    public static void LOAD_MAIN_MENU_SCENE() {
        SceneManager.LoadScene("MainMenuScene", LoadSceneMode.Single);
        Instance.gameState = GameState.MainMenu;
    }

    public static void LOAD_GAMEPLAY_SCENE() {
        SceneManager.LoadScene("GameplayScene", LoadSceneMode.Single);
        Instance.gameState = GameState.GamePlay;
    }

    public static void RELOAD_LEVEL() { 
        SceneManager.LoadScene("GameplayScene", LoadSceneMode.Single);
        Instance.gameState = GameState.GamePlay;
    }

    #endregion SCENE_MANAGEMENT

    public void OnDestroy()
    {
        YandexGame.GetDataEvent -= GetLoadDataYG;
    }

    #region LANGUAGE
    public static void SET_LANGUAGE(TextLang textLang) {
        TextControlManager.textLang = textLang;
    }

    public static void SET_LANGUAGE(string lang) {
        switch (lang) {
            case "ru":
                SET_LANGUAGE(TextLang.Rus);
                break;
            case "en":
                SET_LANGUAGE(TextLang.Eng);
                break;
            case "es":
                SET_LANGUAGE(TextLang.Esp);
                break;
            case "fr":
                SET_LANGUAGE(TextLang.Fra);
                break;
            case "tr":
                SET_LANGUAGE(TextLang.Tur);
                break;
            default:
                SET_LANGUAGE(TextLang.Rus);
                break;
        }
    }


    #endregion LANGUAGE

    public int GetHighScore() {
        return highScore;
    }
}
