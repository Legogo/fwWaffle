﻿using UnityEngine;
using System.Collections.Generic;

abstract public class CameraDynamicZoom : RoundObject
{
  protected Camera cam;
  protected Vector3 mid = Vector3.zero;
  protected Vector3 add = Vector3.zero;

  protected Vector2 min = Vector3.zero;
  protected Vector2 max = Vector3.zero;

  protected float camOrthoSizeMinimum = 6f;

  Vector3 midTarget = Vector3.zero;
  public float targetOrtho = 6f;

  public float speed = 0f;
  public Vector2 diff; // vec WxH

  public IGameplayEntity[] targets;
  
  public override void loadingDone()
  {
    base.loadingDone();
    
    cam = gameObject.GetComponent<Camera>();
    
    if (cam == null) Debug.LogError("no cam");
  }

  protected override void roundRestart()
  {
    base.roundRestart();
    event_recountTargets();
  }
  protected override void roundLaunch()
  {
    base.roundLaunch();
    event_recountTargets();
  }

  virtual protected IGameplayEntity[] getTargets() {
    return null;
  }

  public void event_recountTargets() {
    targets = getTargets();

    //Debug.Log(GetType() + " count " + targets.Length + " targets");
    //for (int i = 0; i < targets.Length; i++) Debug.Log("  " + targets[i].transform.name, targets[i].transform);
  }

  override protected void updateEngine()
  {
    base.updateEngine();
    
    //solve points at camera corners
    //ScreenLimits.resize();

    if (targets == null) return;

    //for (int i = 0; i < targets.Length; i++) Debug.Log(targets[i].isCameraTarget() + " ? "+targets[i].transform.position);

    solve_min_max();
    
    update_middle();
    
    update_zoom();
  }
  
  int countValidTargets() {
    int count = 0;
    for (int i = 0; i < targets.Length; i++)
    {
      if (targets[i].isCameraTarget()) count++;
    }
    return count;
  }

  /* récup les points extremes parmis les targets */
  void solve_min_max()
  {
    diff.x = diff.y = 0f;

    //reste en place quand il y a plus personne
    if (countValidTargets() <= 0) return;
    
    float minX = 100000000f;
    float minY = 100000000f;
    float maxX = -10000000f;
    float maxY = -1000000f;

    for (int i = 0; i < targets.Length; i++)
    {
      if (!targets[i].isCameraTarget()) continue;

      minX = Mathf.Min(targets[i].position.x, minX);
      minY = Mathf.Min(targets[i].position.y, minY);
      maxX = Mathf.Max(targets[i].position.x, maxX);
      maxY = Mathf.Max(targets[i].position.y, maxY);
    }

    min.x = minX;
    min.y = minY;
    max.x = maxX;
    max.y = maxY;

    diff = max - min;
  }

  void update_middle()
  {
    //solve target
    midTarget = solveMiddle();

    //update depth
    mid.z = cam.transform.position.z;
    midTarget.z = mid.z;

    if (cam.transform.position != midTarget)
    {
      float factor = 1f;
      ModBase mod = ModBase.getMod();
      if(mod != null) {
        if(mod.isRestarting()) {
          factor = 4f;
        }
      }

      //find next positino
      float dist = Vector3.Distance(cam.transform.position, midTarget);
      float speed = Mathf.Lerp(20f, 40f, Mathf.InverseLerp(0f, 10f, dist));
      
      mid = Vector3.MoveTowards(mid, midTarget, speed * Time.deltaTime * factor);

      //apply new position
      cam.transform.position = mid;
    }
  }

  void update_zoom()
  {
    targetOrtho = Mathf.Max(camOrthoSizeMinimum, solveTargetOrtho());
    
    if (cam.orthographicSize != targetOrtho)
    {
      float dist = Mathf.Abs(targetOrtho - cam.orthographicSize);
      float speed = Mathf.Lerp(15f, 30f, Mathf.InverseLerp(0f, 10f, dist));
      cam.orthographicSize = Mathf.MoveTowards(cam.orthographicSize, targetOrtho, speed * Time.deltaTime);
    }
  }

  // zoom level is solved based on distance between all targets
  float solveTargetOrtho()
  {
    // zoom
    //float currentOrthoSize = cam.orthographicSize;

    float ratio = Camera.main.aspect;
    //Debug.Log(ratio);
    //w2 x h2 == ortho 3
    //w13 x h8 == ortho 6

    //h 27 == ortho 17.5

    bool prioWidth = (diff.x > diff.y * ratio);

    float result = 0f;

    float w = (diff.x - 14f);
    float h = (diff.y - 4f);

    result = (w >= h) ? w : h;

    result = camOrthoSizeMinimum + (result * 0.5f);

    //float w = Mathf.InverseLerp(0f, 40f, diff.x);
    //result = Mathf.Lerp(camOrthoSizeMin, 20f, result);

    Debug.DrawLine(min, max, prioWidth ? Color.cyan : Color.yellow);

    return result;
  }

  Vector2 solveMiddle()
  {
    return min + (diff * 0.5f);
  }

  Vector3 middleAvg()
  {

    if (targets.Length <= 0) return Vector3.zero;

    add.x = 0f;
    add.y = 0f;

    for (int i = 0; i < targets.Length; i++)
    {
      if (targets[i] == null) continue;

      add += targets[i].position;
    }

    add.x = add.x / targets.Length;
    add.y = add.y / targets.Length;
    add.z = 0f;

    return add;
  }

  void OnDrawGizmosSelected()
  {
    Gizmos.color = Color.yellow;
    Gizmos.DrawSphere(mid, 0.1f);
    Gizmos.DrawLine(cam.transform.position, mid);

    Gizmos.color = Color.grey;
    Gizmos.DrawSphere(min, 0.1f);
    Gizmos.DrawLine(cam.transform.position, min);
    
    Gizmos.DrawSphere(max, 0.1f);
    Gizmos.DrawLine(cam.transform.position, max);
    
    Gizmos.color = Color.red;
    if (targets == null) return;
    if (targets.Length <= 0) return;

    for (int i = 0; i < targets.Length; i++)
    {
      if (targets[i] == null) continue;
      Gizmos.DrawCube(targets[i].position, Vector3.one * 0.2f);
    }
  }

}
