using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
//Script to handle levelling up, and to handle speedrunning
public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    public int level = 1;
    public int pickupsCollected = 0;

    private bool canLevelUp = true;

    // Speedrun timer
    private float runTimer = 0f;
    private bool timerRunning = false;

    private const int finalLevel = 5;
    private const float defaultBestTime = 3600f;

    [SerializeField]
    TextMeshProUGUI timerText;
    [SerializeField]
    TextMeshProUGUI bestTimeText;
    [SerializeField]
    TextMeshProUGUI endScreenText;
    [SerializeField]
    TextMeshProUGUI levelText;
    [SerializeField]
    GameObject endScreen;

    private AudioSource pickUpSound;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        pickUpSound = GetComponent<AudioSource>();
        levelText.text = "Level " + level + "/5";
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Instance = null;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
            RestartGame();

        if (timerRunning)
        {
            runTimer += Time.deltaTime;
            timerText.text = FormatTime(GetCurrentRunTime());
            bestTimeText.text = "Best: " + FormatTime(GetBestRunTime());
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        canLevelUp = true;

        // Start the timer if it's the first level
        if (level == 1 && !timerRunning)
        {
            runTimer = 0f;
            timerRunning = true;
        }
    }

    public void RestartGame()
    {   
        level = 1;
        levelText.text = "Level " + level + "/5";
        pickupsCollected = 0;
        timerRunning = true;
        runTimer = 0;
        endScreen.SetActive(false);
        SceneManager.LoadScene("Level1");
    }

    public void CollectPickup()
    {
        pickUpSound.Play();
        pickupsCollected++;
        CheckProgress();
    }

    private void CheckProgress()
    {
        if (pickupsCollected >= level && canLevelUp)
        {
            LevelUp();
            canLevelUp = false;
        }
    }

    public string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        int milliseconds = Mathf.FloorToInt((time * 100f) % 100f); // 2 decimal places

        return string.Format("{0:00}:{1:00}.{2:00}", minutes, seconds, milliseconds);
    }

    public void LevelUp()
    {
        level++;
        pickupsCollected = 0;
        levelText.text = "Level " + level + "/5";
        // Stop the timer if finishing the last level
 
        if (level > finalLevel && timerRunning)
        {
            timerRunning = false;
            Debug.Log("Run complete in " + FormatTime(runTimer));

            float bestTime = PlayerPrefs.GetFloat("BestRunTime", float.MaxValue);

            if (runTimer < bestTime)
            {
                PlayerPrefs.SetFloat("BestRunTime", runTimer);
                PlayerPrefs.Save();
               
            }          

            // Show end screen
            if (endScreen != null)
            {
                endScreen.SetActive(true);
                endScreenText.text = "Game complete in " + FormatTime(runTimer);
            }

            return; // Don't load another scene
        }

     
        int nextSceneIndex = Mathf.Clamp(level - 1, 0, SceneManager.sceneCountInBuildSettings - 1);

        SceneManager.LoadScene(nextSceneIndex);
    }

    public float GetCurrentRunTime() => runTimer;

    public float GetBestRunTime()
    {
        return PlayerPrefs.GetFloat("BestRunTime", defaultBestTime);
    }
}