using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class RoundObject : EngineObject {

  SceneFactory _factory;

  protected override void build()
  {
    EventRound.onRoundRestart += roundRestart;
    EventRound.onRoundLaunch += roundLaunch;
    EventRound.onRoundEnd += roundEnd;
    EventRound.onRoundLose += roundLose;

    base.build();
  }
  
  /* quand on cycle après un round end, mais avant le lancement du round (ex : menu ready) */
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
    //Debug.Log(GetType()+" round state ? "+ RoundState._instance.getState());

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
