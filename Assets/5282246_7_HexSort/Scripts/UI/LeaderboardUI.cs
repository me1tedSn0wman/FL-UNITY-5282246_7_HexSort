using System;
using UnityEngine;
using YG;

public class LeaderboardUI : WindowUI
{
    [Header("Leaderboard")]
    [SerializeField] LeaderboardYG leaderboardYG;

    public new event Action OnPlayAudioUI;

    public void AddScore(int newHighScore) {
        leaderboardYG.NewScore(newHighScore);
    }

    public void OnEnable()
    {
        leaderboardYG.UpdateLB();
    }
    public override void PlayAudioUI()
    {
        OnPlayAudioUI();
    }
}
