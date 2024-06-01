using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private int MAX_SCORE = 1000000;

    [SerializeField] private int prevScore;

    [SerializeField] public int score;
    public int highScore;
    public bool isNewHighScore;

    public void Awake()
    {
        ResetScore();
        highScore = GameManager.Instance.GetHighScore();
    }

    public void SetHighScore(int highScore) {
        this.highScore = highScore;
    }

    public void AddScore(int addScore) {
        score = Mathf.Min(score + addScore, MAX_SCORE);
        GameplayManager.Instance.gameplayUIManager.UpdateScore(score);

        if (score > highScore) {
            highScore = score;
            GameManager.Instance.UpdateHighScore(highScore);
        }
    }

    public void ResetScore() {
        score = 0;
        GameplayManager.Instance.gameplayUIManager.UpdateScore(score);
    }

    public void SaveStateScore() {
        prevScore = score;
    }

    public void LoadStateScore() {
        score = prevScore;
        GameplayManager.Instance.gameplayUIManager.UpdateScore(score);
    }
}