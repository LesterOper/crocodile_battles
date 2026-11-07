using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CodeBase.Infrastructure
{
  public class SceneLoader
  {
    private readonly ICoroutineRunner _coroutineRunner;

    public SceneLoader(ICoroutineRunner coroutineRunner) => _coroutineRunner = coroutineRunner;

    public void Load(string name, Action onLoaded = null) => _coroutineRunner.StartCoroutine(LoadScene(name, onLoaded));
    public void Load(int index, Action onLoaded = null) => _coroutineRunner.StartCoroutine(LoadScene(index, onLoaded));

    public void ReloadCurrentScene(Action onLoaded = null) => _coroutineRunner.StartCoroutine(LoadCurrentScene(onLoaded));
    private IEnumerator LoadScene(string nextSceneName, Action onLoaded = null)
    {
      //SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);
      AsyncOperation wait = SceneManager.LoadSceneAsync(nextSceneName);
      while (!wait.isDone)
        yield return null;
      yield return new WaitForSeconds(0.3f);
      onLoaded?.Invoke();
    }

    private IEnumerator LoadCurrentScene(Action onLoaded = null)
    {
      //SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);
      AsyncOperation wait = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
      while (!wait.isDone)
        yield return null;
      onLoaded?.Invoke();
    }
    
    private IEnumerator LoadScene(int index, Action onLoaded = null)
    {
      //SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);
      AsyncOperation waitNextScene = SceneManager.LoadSceneAsync(index);
      while (!waitNextScene.isDone)
        yield return null;
      onLoaded?.Invoke();
    }
  }
}