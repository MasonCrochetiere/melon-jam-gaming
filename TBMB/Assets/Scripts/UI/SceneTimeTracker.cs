using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SceneTimeTracker : MonoBehaviour
{
    public static SceneTimeTracker instance;

    private float startTime;
    private float elapsedTime;
    private bool isTracking;

    float bestTime = 420f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == 1)
        {
            startTime = Time.time;
            isTracking = true;
            Debug.Log($"Entered scene: {scene.name} - Timer started");
        }
    }

    void OnSceneUnloaded(Scene scene)
    {
        if (isTracking && scene.buildIndex == 1)
        {
            elapsedTime = Time.time - startTime;
            isTracking = false;
            Debug.Log($"Left scene: {scene.name} - Time spent: {elapsedTime:F2} seconds");

            if (elapsedTime < bestTime)
            {
                bestTime = elapsedTime;
            }
        }
    }

    public float GetElapsedTime()
    {
        if (isTracking)
            return Time.time - startTime;
        return elapsedTime;
    }

    public float GetBestTime()
    {
        return bestTime;
    }
}