using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class EngineObject : MonoBehaviour {

  protected bool _lockUpdate = false; // update lock

  void Awake() {
    build();
  }

  virtual protected void build() {}

  void Start() {
    
    subscribeSystemEvent();

    onStart();
    
    //enabled = false empeche l'appel de Update dans Unity
    //on doit attendre que les assets soient chargé avant de balancer les updates (system loading)
    enabled = false;

    if (!SceneFactory.isLoading()){
      loadingDone();
      loadingDoneLate();
    }

  }

  virtual protected void onStart(){

  }
  
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

  virtual public void loadingDone() {
    
  }

  virtual public void loadingDoneLate(){
    enabled = true;
  }

  protected string info = "";
  virtual public string toString(){ info = ""; return info; }


}
