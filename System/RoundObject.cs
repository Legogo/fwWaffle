using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class RoundObject : EngineObject {

  SceneFactory _factory;

  protected override void build()
  {
    base.build();

    //Debug.Log(GetType() + " build()");

    EventRound.onRoundRestart += roundRestart;
    EventRound.onRoundLaunch += roundLaunch;
    EventRound.onRoundEnd += roundEnd;
    EventRound.onRoundLose += roundLose;
  }

  protected override void onStart()
  {
    base.onStart();

    StartCoroutine(processWaitFactory());
  }
  
  IEnumerator processWaitFactory(){

    yield return null;

    enabled = false;

    _factory = GameObject.FindObjectOfType<SceneFactory>();

    if (_factory != null){
      while (!_factory.isDoneLoading()) yield return null;
    }

    onFactoryDone();
  }

  virtual protected void onFactoryDone(){
    enabled = true;
  }

  virtual protected void roundRestart(){
    //Debug.Log(GetType() + " roundRestart");
  }
  virtual protected void roundLaunch(){}
  virtual protected void roundEnd(){}
  virtual protected void roundLose(){}

  // tout le temps en route
  protected override void updateEngine()
  {
    base.updateEngine();

    //Debug.Log("update "+GetType()+" can ? "+canUpdateRound());
    
    //lock in menu round update
    if(RoundState._instance.getState() == RoundState.eRoundStates.RESTART){
      updateRoundMenu();
      return;
    }

    if (RoundState._instance.getState() == RoundState.eRoundStates.END)
    {
      updateRoundEnd();
      return;
    }
    
    //lock if not live
    if (RoundState._instance.getState() != RoundState.eRoundStates.LIVE){
      return;
    }

    //specific locking
    if (!canUpdateRound()) return;
    
    updateRound();
  }

  //en route seuleemnt pendant les menus
  virtual protected void updateRoundMenu(){}

  //en route seulement pendant que ça game
  virtual protected void updateRound(){}

  virtual protected void updateRoundEnd(){}

  //filtre ingame
  virtual protected bool canUpdateRound(){
    return true;
  }
}
