using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Le ModBase manipule le RoundState pour communiquer avec les autres objets
 * */

public class ModBase : EngineObject {
    
  protected float waitTimeBeforeRestart = 0f;

  protected override void build()
  {
    base.build();
    _manager = this;
  }

  protected override void updateEngine()
  {
    base.updateEngine();

    RoundState.eRoundStates st = RoundState._instance.getState();

    if (st == RoundState.eRoundStates.RESTART) updateModRestart();
    else if (st == RoundState.eRoundStates.LIVE) updateModLive();
    else if (st == RoundState.eRoundStates.END) updateModEnd();
  }
  
  virtual protected void updateModLive(){
    
    if (isModDone())
    {
      modEnd();
    }
    
  }

  protected void updateModRestart(){
    
    if (lockUpdateModRestart()) return;

    Debug.Log("ModBase | <color=cyan>everybody ready</color>");

    modLaunch();
  }

  virtual protected bool lockUpdateModRestart() {
    
    return false;
  }

  virtual protected void updateModEnd(){
    if (waitTimeBeforeRestart > 0f)
    {
      waitTimeBeforeRestart -= Time.deltaTime;

      //obliger d'injecter du temps pour relancer le round...
      if(waitTimeBeforeRestart <= 0f){
        modRestart();
      }

      return;
    }

  }

  //entry point
  virtual public void modRestart()
  {
    RoundState._instance.roundRestart();
  }

  virtual public void modLaunch(){
    RoundState._instance.roundLaunch();
  }

  virtual public void modEnd()
  {
    RoundState._instance.roundEnd();
  }

  protected void callForRestart(float time = 0.01f){
    waitTimeBeforeRestart = time;
  }

  virtual public bool isModDone(){
    return false;
  }

  public bool isRestarting(){
    return RoundState._instance.getState() <= RoundState.eRoundStates.RESTART;
  }

  public override string toString()
  {
    base.toString();
    info += "\nRoundState : "+RoundState._instance.getState();
    return info;
  }

  virtual public int countNeededPlanes(){
    return 0;
  }

  static protected ModBase _manager;
  static public ModBase getMod() { return _manager; }
  static public T getMod<T>() where T : ModBase
  {
    if (_manager == null)
    {
      _manager = GameObject.FindObjectOfType<T>();
      //if (_manager != null) Debug.Log("<color=yellow>ref game mod is " + _manager.GetType() + "</color>");
      //else Debug.LogWarning("no mods");
    }

    return (T)_manager;
  }

}
