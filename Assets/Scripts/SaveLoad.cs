using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveLoad : MonoBehaviour
{

    [SerializeField] bool deleteHighScore = false;
    // Scene key:
    // 0 = menu
    // 1 = game

    private void OnValidate()
    {
        if (deleteHighScore)
        {
            PlayerPrefs.DeleteKey("HighScore");
        }
    }

    public void LoadScene(int index = 0)
    {
        SceneManager.LoadScene(index);
    }

    public bool SetHighScore(int score)
    {
        int prevScore = 0;
        if (PlayerPrefs.HasKey("HighScore"))
        {
            prevScore = GetHighScore();
        }

        if (prevScore < score)
        {
            // new highscore
            PlayerPrefs.SetInt("HighScore", score);
            Debug.Log("new highscore! " + score);
            return true;
        }

        return false;

    }

    public int GetHighScore()
    {
        Debug.Log("highscore in storage: " + PlayerPrefs.GetInt("HighScore"));
        return PlayerPrefs.GetInt("HighScore");
    }

    public void SetDeathsCount(int deaths)
    {
        PlayerPrefs.SetInt("DeathsCount", deaths);
    }

    public int GetDeathsCount()
    {
        return PlayerPrefs.GetInt("DeathsCount");
    }
}
