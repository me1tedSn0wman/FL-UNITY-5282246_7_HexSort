using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils.TextControl;

public class SettingsUI : WindowUI
{
    [Header("Sliders")]
    [SerializeField] private Slider slider_SoundVolume;
    [SerializeField] private Slider slider_MusicVolume;
    [SerializeField] private Slider slider_AnimationSpeed;

    [Header("Textes")]
    [SerializeField] private TextMeshProUGUI text_SoundVolume;
    [SerializeField] private TextMeshProUGUI text_MusicVolume;
    [SerializeField] private TextMeshProUGUI text_AnimationSpeed;

    [Header("Dropdown")]
    [SerializeField] private TMP_Dropdown dropdown_Language;

    public new event Action OnPlayAudioUI;

    public override void Awake() { 
        base.Awake();

        slider_SoundVolume.onValueChanged.AddListener((value) =>
        {
            AudioControlManager.soundVolume = value;
            text_SoundVolume.text = ((int)(value * 100)).ToString();
            PlayAudioUI();
        });

        slider_MusicVolume.onValueChanged.AddListener((value) =>
        {
            AudioControlManager.musicVolume = value;
            text_MusicVolume.text = ((int)(value * 100)).ToString();
            PlayAudioUI();
        });

        slider_AnimationSpeed.onValueChanged.AddListener((value) =>
        {
            GameManager.animationSpeed = value;
            string animSpr = String.Format("{0:f2}", value);
            text_AnimationSpeed.text = animSpr;
            PlayAudioUI();
        }
        );

        dropdown_Language.onValueChanged.AddListener((value) =>
        {
            PlayAudioUI();
            SetLanguage(value);
        });

        UpdateInitValues(); ;
    }

    public override void PlayAudioUI()
    {
        OnPlayAudioUI();
    }

    public void UpdateInitValues() {
        float tSoundVol = AudioControlManager.soundVolume;
        float tMusicVol = AudioControlManager.musicVolume;
        float tAnimationSpeed = GameManager.animationSpeed;

        slider_SoundVolume.SetValueWithoutNotify(tSoundVol);
        slider_MusicVolume.SetValueWithoutNotify(tMusicVol);
        slider_AnimationSpeed.SetValueWithoutNotify(tAnimationSpeed);

        text_SoundVolume.text = ((int)(tSoundVol * 100)).ToString();
        text_MusicVolume.text = ((int)(tMusicVol * 100)).ToString();

        string animSpr = String.Format("{0:f2}", tAnimationSpeed);
        text_AnimationSpeed.text = animSpr;

        dropdown_Language.SetValueWithoutNotify(TextControlManager.GetTextLangInt());
    }

    public void SetLanguage(int value) {
        switch (value) {
            case 0:
                GameManager.SET_LANGUAGE(TextLang.Rus);
                break;
            case 1:
                GameManager.SET_LANGUAGE(TextLang.Eng);
                break;
            case 2:
                GameManager.SET_LANGUAGE(TextLang.Esp);
                break;
            case 3:
                GameManager.SET_LANGUAGE(TextLang.Fra);
                break;
            case 4:
                GameManager.SET_LANGUAGE(TextLang.Tur);
                break;
        }
    }
}
