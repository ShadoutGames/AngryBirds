using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private Button[] buttons;
    private int currentLevel;

    [SerializeField]
    private GameObject backgroundPanel;
    [SerializeField]
    private GameObject levelsPanel;

    private void Start()
    {
        levelsPanel.gameObject.SetActive(false);
        backgroundPanel.gameObject.SetActive(true);
        currentLevel = PlayerPrefs.GetInt("Level");

        if (currentLevel == 0)
        {
            currentLevel = 1;
        }

        for (int i = 0; i < currentLevel; i++)
        {
            buttons[i].interactable = true;
            var currentLevelStar = PlayerPrefs.GetInt((i+1).ToString());
            for (int j = currentLevelStar; j < 3; j++)
            {
                buttons[i].transform.GetChild(j).gameObject.SetActive(false);
            }
        }
        for (int i = currentLevel; i < buttons.Length; i++)
        {
            buttons[i].interactable = false;
            for (int j = 0; j < 3; j++)
            {
                buttons[i].transform.GetChild(j).gameObject.SetActive(false);
            }
        }

        for (int i = 1; i <= buttons.Length; i++)
        {
            int temp = i; // closure problem
            buttons[i - 1].onClick.AddListener(() => GetLevel(temp));
            buttons[i - 1].transform.GetChild(6).GetComponent<Text>().text = i.ToString();
        }

    }

    private void GetLevel(int level)
    {
        SceneManager.LoadScene(level);
    }

    public void GetCurrentLevel()
    {
        SceneManager.LoadScene(currentLevel);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void GetLevelsPanel()
    {
        backgroundPanel.gameObject.SetActive(false);
        levelsPanel.gameObject.SetActive(true);
    }
}
