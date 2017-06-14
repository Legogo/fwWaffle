using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class UiOption : MonoBehaviour {

  public int orderLayer = 0;
  public string action = "";

  //override this function to explain how to use the action id
  //ex : give 'action' to a UiOptionManager to do something with it
  abstract public void validate();
  
  virtual public void onSelection()
  {
    //Debug.Log("selected " + name, gameObject);
  }

  virtual public void onUnSelection()
  {

  }
}
