using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class LevelHandler : MonoBehaviour
{
    // Serialized fields

    [SerializeField]
    private bool _dontDestroyOnLoad = false;
    [SerializeField]
    private List<string> _playableLevels = new List<string>();

    // Private fields

    private string _currentScene = null;
    private static LevelHandler _instance = null;

    // Public properties

    public static LevelHandler Instance => _instance;

    // Private methods

    private void Awake()
    {
        if (_instance != null)
            Destroy(gameObject);
        else
        {
            _instance = this;
            if (_dontDestroyOnLoad)
                DontDestroyOnLoad(this);
        }
        _currentScene = SceneManager.GetActiveScene().name;
    }

    private IEnumerator sceneLoader(string levelName)
    {
        yield return null;
        var op = SceneManager.LoadSceneAsync(levelName);
        _currentScene = levelName;
    }

    // Public methods

    public void LoadScene(string sceneName) => StartCoroutine(sceneLoader(sceneName));

    public void LoadLevel(int levelIndex)
    {
        if (levelIndex < _playableLevels.Count && levelIndex >= 0)
        {
            _currentScene = _playableLevels[levelIndex];
            LoadScene(_currentScene);
        }
        else
            ResetLevel();
    }

    public void LoadNextLevel() => LoadLevel(_playableLevels.FindIndex(x => x == _currentScene) + 1);

    public void ResetLevel() => StartCoroutine(sceneLoader(_currentScene));

    public void QuitApplication() => Application.Quit();
}
