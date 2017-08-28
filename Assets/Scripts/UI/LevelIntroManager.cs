using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LevelIntroManager : MonoBehaviour {
    
    private Text levelName;
    private Text levelNumber;

    private Animator anim;

    // TODO animator ?

    private void Awake()
    {
        levelNumber = transform.GetChild(0).gameObject.GetComponent<Text>();
        levelName = transform.GetChild(1).gameObject.GetComponent<Text>();

        anim = GetComponent<Animator>();
    }

    /// <summary>
    /// Set level intro to display.
    /// </summary>
    /// <param name="levelNumber">World and level number</param>
    /// <param name="levelName">Level name</param>
    public void SetupLevelIntro(string levelNumber, string levelName)
    {
        this.levelName.text = levelName;
        this.levelNumber.text = levelNumber;
    }

    /// <summary>
    /// Display it with fade.
    /// </summary>
    public void TriggerDisplay()
    {
        anim.SetTrigger("Display");
    }
}
