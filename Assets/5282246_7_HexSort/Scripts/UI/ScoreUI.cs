using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text_ScoreCount;

    public void Awake()
    {
        UpdateScore(0);
    }

    public void UpdateScore(int score) { 
        text_ScoreCount.text = score.ToString();
    }
}
