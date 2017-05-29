using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;

abstract public class SceneFactory : MonoBehaviour {

  public enum eLoadingStates { BUILD = 0, ASYNC = 1, AFTER = 2, IDLE = 3 };
  static public eLoadingStates _state = eLoadingStates.BUILD;
  
  protected List<AsyncOperation> _asyncs;

  protected string[] systemList;
  public string[] scenes;

  void Start() {
    call_loading_system();
  }

  void call_loading_system() {
    _state = eLoadingStates.ASYNC;
    
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

    //for (int i = 0; i < items.Length; i++) Debug.Log(items[i].name);

    //Debug.Log("SceneFactory | after ...");

    //before the for() loop because some newly created object from afterLoading callback need to call their afterLoading callbacks
    _state = eLoadingStates.AFTER;

    for (int i = 0; i < items.Length; i++)
    {
      items[i].afterLoading();
    }

    Debug.Log("SceneFactory | now removing guides ...");
    SceneTools.removeGuides();

    EngineState._instance.endOfLoading();

    _state = eLoadingStates.IDLE;

    MonoBehaviour[] comps = gameObject.GetComponents<MonoBehaviour>();
    if (comps.Length > 1)
    {
      Debug.LogError("factory will be destroy, no other component must be on gameobject, " + comps.Length);
      for (int i = 0; i < comps.Length; i++) Debug.Log(comps[i].GetType());
    }

    GameObject.DestroyImmediate(gameObject);
	}

  protected IEnumerator waitForAsyncs(Action onDone)
  {
    //Debug.Log(_asyncs.Count + " asyncs loading");

    while (_asyncs.Count > 0) yield return null;
    
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
