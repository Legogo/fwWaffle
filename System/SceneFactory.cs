using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SceneFactory : MonoBehaviour {
  
  public string[] scenes;
  //public GameObject[] prefabs;

  bool _load = false;

	IEnumerator Start () {
    for (int i = 0; i < scenes.Length; i++)
    {
      AsyncOperation async = SceneManager.LoadSceneAsync(scenes[i], LoadSceneMode.Additive);
      while (!async.isDone) yield return null;
    }

    yield return null;
    
    _load = true;

    Debug.Log("{SceneFactory} now removing guides ...");

    SceneTools.removeGuides();
    //Debug.Log("factory is done");

    GameObject.DestroyImmediate(this);
	}

  public bool isDoneLoading() {
    return _load;
  }
	
}
