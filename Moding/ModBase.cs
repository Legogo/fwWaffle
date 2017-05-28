using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Le ModBase manipule le RoundState pour communiquer avec les autres objets
 * */

public class ModBase : EngineObject {
    
  protected Plane[] planes;
  protected float waitTimeBeforeRestart = 0f;

  UiScreenReady ui_rdy;

  protected override void build()
  {
    base.build();
    _manager = this;
  }
  
  public override void loadingDoneLate()
  {
    base.loadingDoneLate();

    ui_rdy = GameObject.FindObjectOfType<UiScreenReady>();
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

  virtual protected void updateModRestart(){
    if (ui_rdy != null)
    {
      if (ui_rdy.isLocking()) return;
    }

    Debug.Log("ModBase | <color=cyan>everybody ready</color>");

    modLaunch();
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

    //clear bullets from previous round
    AmmunitionManager.startRound();

    //spawn planes on screen
    respawnPlanes();

    if (ui_rdy != null)
    {
      Debug.Log("ModBase | opening ready menu");
      ui_rdy.open();
    }
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
    
    respawnDeadPlanes();

    return false;
  }

  protected void respawnDeadPlanes()
  {
    planes = PlaneManager.get().getAllPlanes();

    for (int i = 0; i < planes.Length; i++)
    {
      if (planes[i] == null) continue;

      if (planes[i].modSymbol != null && !planes[i].modSymbol.isVisible())
      {
        if (planes[i].modExplosion != null && planes[i].modExplosion.explosionIsDone())
        {
          planes[i].spawn(); // will launch
        }
      }
    }
  }

  virtual protected void respawnPlanes()
  {

    planes = PlaneManager.get().getControlledPlanes();
    if (planes == null) Debug.LogWarning("no players ?");

    ArenaManager am = ArenaManager.get();
    Transform tr = null;

    for (int i = 0; i < planes.Length; i++)
    {
      if (planes[i] == null) continue;

      if (am != null)
      {
        tr = am.getPlayerSpawn((int)planes[i].getPad().gamePadIndex);
      }

      if (tr != null) planes[i].spawn(tr.position);
      else planes[i].spawn();
    }

  }

  protected void launchPlanes()
  {
    planes = PlaneManager.get().getControlledPlanes();
    for (int i = 0; i < planes.Length; i++)
    {
      planes[i].launch();
    }
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
  static public ModBase getMod()
  {
    if (_manager == null)
    {
      _manager = GameObject.FindObjectOfType<ModBase>();
      //if (_manager != null) Debug.Log("<color=yellow>ref game mod is " + _manager.GetType() + "</color>");
      //else Debug.LogWarning("no mods");
    }

    return _manager;
  }

}
