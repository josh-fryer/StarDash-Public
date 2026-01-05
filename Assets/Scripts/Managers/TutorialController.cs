using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialController : MonoBehaviour
{
    private GameMaster gameMaster;
    [SerializeField] Spawner spawner;
    [SerializeField] bool enableTutorial = true;
    [Tooltip("Dev tool. Turn off for prod")]
    [SerializeField] bool alwaysStartTutorial;
    [Header("UI")]
    [SerializeField] GameObject tutorialUIScreen;


    // Start is called before the first frame update
    void Start()
    {
        gameMaster = GameMaster.Instance;
    }

    public bool IsTutorialCompleted()
    {
        int isCompleted = PlayerPrefs.GetInt("TutorialDone"); // defualt for not exists is 0
        if(alwaysStartTutorial)
        {
            isCompleted = 0;
        }

        Debug.Log("tutorial is Completed = " + isCompleted);
        if (isCompleted == 0 && enableTutorial)
        {
            StartTutorial();
            return false;
        }
        else
        {
            return true;
        }
    }


    public void StartTutorial()
    {
        gameMaster = GameMaster.Instance;
        gameMaster.isAlive = false; // pause timer and difficulty progression
        // show UI
        tutorialUIScreen.SetActive(true);

        // tutorial is complete when button is tapped from screen to start spawners
    }

    public void TutorialCompleted()
    {
        tutorialUIScreen.SetActive(false);
        gameMaster.isAlive = true;
        spawner.StartSpawners();
        PlayerPrefs.SetInt("TutorialDone", 1);
    }
}
