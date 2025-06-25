using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public int ProgressAmount;
    [SerializeField]
    public Slider ProgressSlider;

    public GameObject Player;
    public GameObject LoadCanvas;
    public List<GameObject> Levels;
    private int CurrentLevelIndex = 0;

    public GameObject GameOverScreen;
    public TMP_Text SurvivedText;
    private int SurvivedLevelsCount;

    public static event Action OnReset;

    // Start is called before the first frame update
    void Start()
    {
        ProgressAmount = 0;
        ProgressSlider.value = 0;
        Gem.OnGemCollect += IncreaseProgressAmount;
        HoldToLoadLevel.OnHoldComplete += LoadNextlevel;
        PlayerHealth.OnPlayerDied += LoadGameOverScreen;
        LoadCanvas.SetActive(false);
        GameOverScreen.SetActive(false);
    }
    
    private void LoadGameOverScreen ()
    {
        GameOverScreen.SetActive(true);
        SurvivedText.text = "YOU SURVIVED " + SurvivedLevelsCount + " LEVEL";
        if (SurvivedLevelsCount != 1) SurvivedText.text += "S";
        Time.timeScale = 0;
    }

    public void ResetGame ()
    {
        GameOverScreen.SetActive(false);
        SurvivedLevelsCount = 0;
        LoadLevel(0, false);
        OnReset.Invoke();
        Time.timeScale = 1;
    }

    void IncreaseProgressAmount (int amount)
    {
        ProgressAmount += amount;
        ProgressSlider.value = ProgressAmount;
        if (ProgressAmount >= 100)
        {
            LoadCanvas.SetActive(true);
            Debug.Log("Level Complete");
        }
    }
    void LoadLevel(int level, bool wantSurvivedIncrease)
    {
        LoadCanvas.SetActive(false);

        Levels[CurrentLevelIndex].gameObject.SetActive(false);
        Levels[level].gameObject.SetActive(true);

        Player.transform.position = new Vector3(0, 0, 0);

        CurrentLevelIndex = level;
        ProgressAmount = 0;
        ProgressSlider.value = 0;
        if (wantSurvivedIncrease) SurvivedLevelsCount++;
    }

    void LoadNextlevel ()
    {
        int nextLevelIndex = (CurrentLevelIndex == Levels.Count - 1) ? 0 : CurrentLevelIndex + 1;
        LoadLevel(nextLevelIndex, true);
    }
}
