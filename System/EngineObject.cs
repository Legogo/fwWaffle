using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class EngineObject : MonoBehaviour {

  protected bool _lockUpdate = false; // update lock

  void Awake() {

    subscribeSystemEvent();

    build();

  }

  void Start()
  {
    //need to wait a frame for the default factory to be created if not in scene

    //Debug.Log(name + " , " + SceneFactory.isLoading());
    if (!SceneFactory.isLoading())
    {
      afterLoading();
    }


    //enabled = false empeche l'appel de Update dans Unity
    //on doit attendre que les assets soient chargé avant de balancer les updates (system loading)

    enabled = false;
  }

  virtual protected void build() {}
  
  virtual protected void subscribeSystemEvent() {

    //par default tout le monde réagit a la pause
    EventEngine.onPause += toggleLock;

  }

  protected void toggleLock(bool pauseState) {
    _lockUpdate = pauseState;
  }

  protected void Update() {
    updateSystem();
    
    if (!canUpdate()) return;
    
    updateEngine();
  }
  
  virtual protected void updateSystem() {
    
  }
  virtual protected void updateEngine() {
    
  }

  virtual protected bool canUpdate()
  {
    return !_lockUpdate;
  }

  virtual public void afterLoading() {
    enabled = true;
  }

  protected string info = "";
  virtual public string toString(){ info = ""; return info; }


}
