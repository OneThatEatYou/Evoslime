using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region Singleton
    static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                //check if instance is in scene
                instance = FindObjectOfType<GameManager>();

                if (instance == null)
                {
                    //spawn instance
                    GameObject obj = new GameObject("GameManager");
                    instance = obj.AddComponent<GameManager>();
                    DontDestroyOnLoad(obj);
                }
            }
            return instance;
        }
    }
    #endregion

    public Vector2 ScreenPixelSize { get { return new Vector2(Screen.width, Screen.height); } }

    public MapManager mapManager;
    public EvolutionManager evolutionManager;
    public AudioManager audioManager;

    Image fadeImage;

    bool isFading;

    public const string mainSceneName = "GlobalScene";
    public const string mainMenuSceneName = "MainMenu";

    private void Awake()
    {
        mapManager = new MapManager();
        evolutionManager = new EvolutionManager();
        audioManager = new AudioManager();

        CreateFadeCanvas();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        StartCoroutine(FadeAndChangeScene(1, 0));

        Time.timeScale = 1;
    }

    public void ChangeScene(string sceneName, float fadeTime = 2)
    {
        if (!isFading)
        {
            StartCoroutine(FadeAndChangeScene(0, 1, sceneName, fadeTime));
        }
    }

    void CreateFadeCanvas()
    {
        //creates image overlay for fading in and out
        GameObject fadeCanvasGO = new GameObject("FadeCanvas");
        Canvas fadeCanvas = fadeCanvasGO.AddComponent<Canvas>();
        fadeCanvasGO.AddComponent<GraphicRaycaster>();

        CanvasScaler canvasScaler = fadeCanvasGO.AddComponent<CanvasScaler>();
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.referenceResolution = new Vector2(960, 540);
        canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        canvasScaler.matchWidthOrHeight = 0.5f;

        fadeCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        fadeCanvas.sortingOrder = 2;
        fadeCanvasGO.GetComponent<RectTransform>().sizeDelta = ScreenPixelSize;
        fadeImage = fadeCanvasGO.AddComponent<Image>();
        fadeImage.raycastTarget = false;
        fadeImage.color = Color.clear;
        fadeCanvasGO.transform.SetParent(transform);
    }

    public IEnumerator FadeAndChangeScene(int start, int target, string sceneName = "", float fadeTime = 2)
    {
        //fade out scene
        Color fadeCol = fadeImage.color;
        float t = 0;

        fadeCol.a = start;

        while (Mathf.Abs(fadeImage.color.a - target) > 0.001f)
        {
            fadeCol.a = Mathf.SmoothStep(start, target, t / fadeTime);
            fadeImage.color = fadeCol;
            t += Time.unscaledDeltaTime;

            yield return null;
        }

        fadeCol.a = target;

        if (sceneName != "")
        {
            SceneManager.LoadSceneAsync(sceneName);
        }
    }

    /// <summary>
    /// Instantiate player as a new monster (active scene will be switched to the input active scene)
    /// </summary>
    public GameObject SpawnPlayer(string activeSceneName, MonsterData monsterData)
    {
        Scene activeScene = SceneManager.GetSceneByName(activeSceneName);

        if (!activeScene.IsValid())
        {
            Debug.LogWarning($"Invalid scene: {activeSceneName}");
            return null;
        }

        SceneManager.SetActiveScene(activeScene);

        GameObject player = Instantiate(monsterData.monsterPrefab, Vector2.zero, Quaternion.identity);
        player.AddComponent<PlayerController>();

        return player;
    }
}
