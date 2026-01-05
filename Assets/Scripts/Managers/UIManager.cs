using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    private GameMaster gm;
    private AudioManager audioManager;
    private SaveLoad saveLoad;

    [Header("Game UI")]
    [SerializeField] TextMeshProUGUI distanceText;
    [SerializeField] GameObject gameUICanvas;

    [SerializeField] GameObject speedUpPanel;
    [SerializeField] GameObject pausePanel;
    [SerializeField] Image MuteImg;
    [SerializeField] Sprite muteSprite;
    [SerializeField] Sprite unMuteSprite;

    [SerializeField] GameObject scorePanel;
    private Animator speedUpAnim;

    [SerializeField] TextMeshProUGUI scoreText;

    [Header("Game Over UI Screen")]
    [SerializeField] GameObject gameOverUI;
    [SerializeField] TextMeshProUGUI distanceGameOverText;
    [SerializeField] TextMeshProUGUI scoreGameOverText; // points earned
    [SerializeField] TextMeshProUGUI totalText;
    [SerializeField] GameObject newHighscore;
    [SerializeField] GameObject continueBtn;

    [Header("Menu")]
    [SerializeField] GameObject highScoreUI;
    [SerializeField] TextMeshProUGUI highScoreTextValue;
    [SerializeField] GameObject creditsUIPanel;
    [SerializeField] GameObject removeAdsBtn;

    private string currScene;

    private void Start()
    {
        saveLoad = FindObjectOfType<SaveLoad>();
        currScene = SceneManager.GetActiveScene().name;
        Debug.Log("Scene = " + currScene);
        if (currScene != "Menu")
        {
            gm = GameMaster.Instance;
            speedUpAnim = speedUpPanel.GetComponent<Animator>();
        }
        else if (currScene == "Menu")
        {
            // check if to display highscore text
            if (PlayerPrefs.HasKey("HighScore"))
            {
                int highScore = saveLoad.GetHighScore();
                if (highScore > 0)
                {
                    Debug.Log("Get High Score: " + highScore);
                    highScoreUI.SetActive(true);
                    highScoreTextValue.text = highScore.ToString();
                }
            }

            if (PlayerPrefs.GetInt("AdsRemoved", 0) == 1)
            {
                HideRemoveAdsBtn();
            }
        }

        audioManager = FindObjectOfType<AudioManager>();
    }

    public void ShowHideDeathUI(bool isNewHighscore = false)
    {
        if (gameOverUI.activeSelf)
        {
            gameUICanvas.SetActive(true);
            gameOverUI.SetActive(false);
            return;
        }
        gameUICanvas.SetActive(false);
        gameOverUI.SetActive(true);
        distanceGameOverText.text = distanceText.text;
        scoreGameOverText.text = '+' + gm.GetPoints().ToString();
        SetTotalText();
        // set highscore text if new highscore
        // set highscore obj active
        if (isNewHighscore)
        {
            newHighscore.SetActive(true);
        }
        //Hide Continue after 1 death
        if (gm.deaths >= 2)
        {
            continueBtn.SetActive(false);
        }

    }

    public void SetDistanceText(int distance)
    {
        distanceText.text = distance.ToString();
    }

    public void SetTotalText()
    {
        int total = gm.GetPoints() + gm.GetDistance();
        totalText.text = total.ToString();
    }

    public void SetScoreText(int points)
    {
        scoreText.text = points.ToString();
    }

    public void ShowHideSpeedUpPanel()
    {
        StartCoroutine(showHidePanel());
    }

    IEnumerator showHidePanel()
    {
        yield return new WaitForSeconds(0.5f); // delay
        speedUpAnim.SetTrigger("SlideIn");
        yield return new WaitForSeconds(2.5f); // delay
        speedUpAnim.SetTrigger("SlideOut");
    }

    public void PauseGame()
    {
        pausePanel.SetActive(true);
        CheckMuteSprite();

        Time.timeScale = 0f;
        scorePanel.GetComponent<CanvasGroup>().interactable = false;
    }

    public void ResumeGame()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
        scorePanel.GetComponent<CanvasGroup>().interactable = true;
    }

    public void ToggleMute()
    {
        audioManager.MuteAudioToggle();
        // switch sprite
        CheckMuteSprite();

    }

    private void CheckMuteSprite()
    {
        if (AudioListener.volume > 0)
        {
            // un-muted
            MuteImg.sprite = unMuteSprite;
        }
        else
        {
            MuteImg.sprite = muteSprite;
        }
    }

    public void ToggleShowCreditsPanel()
    {
        if (creditsUIPanel.activeSelf)
        {
            creditsUIPanel.SetActive(false);
        }
        else
        {
            creditsUIPanel.SetActive(true);
        }
    }

    public void HideRemoveAdsBtn()
    {
        removeAdsBtn.SetActive(false);

    }
}
