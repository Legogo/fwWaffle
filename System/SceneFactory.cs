using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;

public class SceneFactory : MonoBehaviour {

  public enum eLoadingStates { BUILD = 0, LOADING = 1, IDLE = 2 };
  static public eLoadingStates _state = eLoadingStates.BUILD;
  
  protected List<AsyncOperation> _asyncs;

  protected string[] systemList;
  public string[] scenes;
  //public GameObject[] prefabs;

  bool _load = false;

  void Start() {
    
    call_loading_system();
  }

  void call_loading_system() {
    _state = eLoadingStates.LOADING;
    
    define_scenes_list();

    List<string> allScenes = new List<string>();
    
    allScenes.AddRange(systemList);
    allScenes.AddRange(scenes);

    _asyncs = new List<AsyncOperation>();

    for (int i = 0; i < allScenes.Count; i++)
    {
      StartCoroutine(process_loadScene(allScenes[i]));
    }

    StartCoroutine(waitForAsyncs(setup));
  }

  void setup() {
    
    EngineObject[] items = GameObject.FindObjectsOfType<EngineObject>();
    for (int i = 0; i < items.Length; i++)
    {
      items[i].loadingDone();
    }

    for (int i = 0; i < items.Length; i++)
    {
      items[i].loadingDoneLate();
    }

    Debug.Log("SceneFactory | now removing guides ...");
    SceneTools.removeGuides();

    EngineState._instance.endOfLoading();

    _state = eLoadingStates.IDLE;

    GameObject.DestroyImmediate(gameObject);
	}

  protected IEnumerator waitForAsyncs(Action onDone)
  {
    //Debug.Log(_asyncs.Count + " asyncs loading");

    while (_asyncs.Count > 0) yield return null;

    yield return null;

    onDone();
  }

  protected IEnumerator process_loadScene(string sceneLoad)
  {

    //can't reload same scene
    if (SceneTools.isSceneOfName(sceneLoad)) yield break;

    AsyncOperation async = SceneManager.LoadSceneAsync(sceneLoad, LoadSceneMode.Additive);
    _asyncs.Add(async);

    //Debug.Log("  package '<b>" + sceneLoad + "</b>' | starting loading");

    while (!async.isDone) yield return null;

    _asyncs.Remove(async);

    //Debug.Log("  package '<b>" + sceneLoad + "</b>' | done loading (" + _asyncs.Count + " left)");
  }

  public bool isDoneLoading() {
    return _load;
  }

  /* define here contextual scene loading */
  virtual protected void define_scenes_list()
  {
    string[] list = { "resources-engine" };
    systemList = list;
  }

  static public bool isLoading() {
    return _state < eLoadingStates.IDLE;
  }
}
