using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Here you have access to how the menu grabs inputs
/// </summary>

public class UiInputLayer {

  public Action onUp;
  public Action onDown;
  public Action onLeft;
  public Action onRight;

  public Action onValidation;
  public Action onSkip;

  bool _inputValid = false;
  bool _inputSkip = false;
  Vector2 _inputDirection = Vector2.zero;
  
  public void setupEvents(Action callbackValidation, Action callbackSkip)
  {
    onValidation += callbackValidation;
    onSkip += callbackSkip;
  }

  public void setupDirectionEvents(Action callbackLeft, Action callbackRight, Action callbackUp, Action callbackDown)
  {
    onLeft += callbackLeft;
    onRight += callbackRight;
    onUp += callbackUp;
    onDown += callbackDown;
  }

  virtual public void updateSystem()
  {
    //if (!hasActiveInput()) return;
    
    if (padValidation() != _inputValid)
    {
      if (_inputValid)
      {
        _inputValid = false;
        onValidation();
      }
      else
      {
        _inputValid = true;
      }
    }

    if (padSkip() && !_inputSkip)
      {
        _inputSkip = true;
      }
      else if (!padSkip() && _inputSkip)
      {
        onSkip();
        _inputSkip = false;
      }

    if (_inputDirection.y == 0f)
    {
      //press
      if (padUp()) _inputDirection.y = 1f;
      else if (padDown()) _inputDirection.y = -1f;
    }
    else
    {
      //release
      if (!padUp() && _inputDirection.y == 1f)
      {
        _inputDirection.y = 0f;
        if(onUp != null) onUp();
      }
      else if (!padDown() && _inputDirection.y == -1f)
      {
        _inputDirection.y = 0f;
        onDown();
      }
    }

    if (_inputDirection.x == 0f)
    {
      //press
      if (padLeft()) _inputDirection.x = -1f;
      else if (padRight()) _inputDirection.x = 1f;
    }
    else
    {
      //release
      if (!padLeft() && _inputDirection.x == -1f)
      {
        _inputDirection.x = 0f;
        onLeft();
      }
      else if (!padRight() && _inputDirection.x == 1f)
      {
        _inputDirection.x = 0f;
        onRight();
      }
    }

  }

  virtual public bool padUp() { return Input.GetKey(KeyCode.UpArrow); }
  virtual public bool padDown() { return Input.GetKey(KeyCode.DownArrow); }
  virtual public bool padLeft() { return Input.GetKey(KeyCode.LeftArrow); }
  virtual public bool padRight() { return Input.GetKey(KeyCode.RightArrow); }

  virtual public bool padSkip()
  {
    if (Input.GetKeyUp(KeyCode.Escape)) return true;
    return false;
  }

  virtual public bool padValidation()
  {
    if (Input.GetKeyUp(KeyCode.Space)) return true;
    return false;
  }

  // to know if anything can return input
  // default is keyboard --> always true
  virtual public bool hasActiveInput()
  {
    return true;
  }
}
