using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SystemManager : MonoBehaviour {

  [RuntimeInitializeOnLoadMethod]
  static public void init(){
    DontDestroyOnLoad(new GameObject("[system]").AddComponent<SystemManager>());
  }

  void Update() {
    //quit
    if(Input.GetKeyUp(KeyCode.Escape) || Input.GetKeyUp(KeyCode.Backspace)) {
      Application.Quit();
    }
  }

}
