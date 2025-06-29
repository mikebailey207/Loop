using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    public int level = 1;
    public int pickupsCollected = 0;
    private bool canLevelUp = true;

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

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Instance = null;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        canLevelUp = true;
    }

    public void CollectPickup()
    {
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

    public void LevelUp()
    {
        level++;
        pickupsCollected = 0;

        int nextSceneIndex = Mathf.Clamp(level - 1, 0, SceneManager.sceneCountInBuildSettings - 1);
        SceneManager.LoadScene(nextSceneIndex);
    }
}