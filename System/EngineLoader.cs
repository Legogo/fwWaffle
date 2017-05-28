using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class EngineLoader : MonoBehaviour {
  
  public enum eLoadingStates { BUILD = 0, LOADING = 1, IDLE = 2 };
  static public eLoadingStates _state = eLoadingStates.BUILD;

  protected List<AsyncOperation> _asyncs;

  protected bool SHOW_DEBUG = false;

  [RuntimeInitializeOnLoadMethod]
  static protected void create() {
    Debug.Log("<color=gray>Engine entry point</color>");
    
    new GameObject("[loader]").AddComponent<EngineLoader>();
  }

  void Awake() {
    _state = eLoadingStates.BUILD;
    _asyncs = new List<AsyncOperation>();
  }

  void Start() {
    call_loading_system();
  }
  
  /* importer tout ce qui va servir au jeu */
  protected void call_loading_system() {
    _state = eLoadingStates.LOADING;

    if (SHOW_DEBUG) Debug.Log("start of <color=green>system loading</color> ...");
    
    //cameras, debug, input
    StartCoroutine(process_loadScene("resources-engine"));
    //StartCoroutine(process_loadScene("resources-planes"));

    if (SceneTools.isSceneOfType("mod-")) {
      StartCoroutine(process_loadScene("resources-mod"));
    }

    StartCoroutine(waitForAsyncs(setup));
  }

  IEnumerator waitForAsyncs(Action onDone) {
    //Debug.Log(_asyncs.Count + " asyncs loading");

    while (_asyncs.Count > 0) yield return null;

    yield return null;

    onDone();
  }
  
  IEnumerator process_loadScene(string sceneLoad) {
    
    //can't reload same scene
    if (SceneTools.isSceneOfName(sceneLoad)) yield break;

    AsyncOperation async = SceneManager.LoadSceneAsync(sceneLoad, LoadSceneMode.Additive);
    _asyncs.Add(async);

    //Debug.Log("  package '<b>" + sceneLoad + "</b>' | starting loading");

    while (!async.isDone) yield return null;

    _asyncs.Remove(async);

    if (SHOW_DEBUG) Debug.Log("  package '<b>"+sceneLoad+"</b>' | done loading ("+_asyncs.Count+" left)");
  }


  /* un setup global quand les resources sont dispos (sans level) */
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

    if (SHOW_DEBUG) Debug.Log("... <b>system setup</b> loading done");

    EngineState._instance.endOfLoading();

    _state = eLoadingStates.IDLE;

    GameObject.DestroyImmediate(gameObject);
  }
  
  static public bool isLoading(){
    return _state < eLoadingStates.IDLE;
  }
}
