using System.Collections;
using System.Collections.Generic;
using XInputDotNetPure;
using UnityEngine;

public class ControllerPad
{
  public Controller control;
  public PlayerIndex gamePadIndex = PlayerIndex.One;
  public GamePadState state;
  public bool keyboard = false;

  protected int _dodgeSign = 0;
  protected bool _dodge = false;
  protected float _dodgeTimer = 0f;

  protected bool _skipPress = false;

  public ControllerPad(PlayerIndex idx)
  {
    gamePadIndex = idx;
    Debug.Log("<color=green>controller "+idx+" created</color>");
  }

  protected void update_dodge()
  {

    _dodge = false;

    if (_dodgeTimer > 0f)
    {
      _dodgeTimer -= Time.deltaTime;

      float sign = Mathf.Sign(state.ThumbSticks.Left.X);
      if (sign != _dodgeSign)
      {
        //Debug.Log(_dodgeTimer + " , left stick ? " + state.ThumbSticks.Left.X + " , current sign ? " + sign + " | dodge sign ? " + _dodgeSign);

        if (Mathf.Abs(state.ThumbSticks.Left.X) > 0.6f)
        {
          //Debug.Log("<color=red>pad dodge !</color>");
          _dodge = true;
        }
      }
    }
    else if (Mathf.Abs(state.ThumbSticks.Left.X) > 0.6f)
    {
      _dodgeSign = Mathf.FloorToInt(Mathf.Sign(state.ThumbSticks.Left.X));
      //Debug.Log("<color=yellow>sign ? "+_dodgeSign+"</color>");
      _dodgeTimer = 0.3f;
    }

  }

  public void update()
  {

    state = GamePad.GetState(gamePadIndex);
    update_dodge();

  }
  
  public bool isConnected() { return state.IsConnected; }
  public float getLeftStickX() { return state.ThumbSticks.Left.X; }
  public bool pressLeft() { if (keyboard || !state.IsConnected) return Input.GetKey(KeyCode.LeftArrow); return state.ThumbSticks.Left.X < 0f; }
  public bool pressRight() { if (keyboard || !state.IsConnected) return Input.GetKey(KeyCode.RightArrow); return state.ThumbSticks.Left.X > 0f; }

  //public bool pressDash() { if (!state.IsConnected) return false; return state.Buttons.LeftShoulder == ButtonState.Pressed || state.Buttons.B == ButtonState.Pressed; }
  //public bool pressShoot() { if (keyboard || !state.IsConnected) return Input.GetKey(KeyCode.X); return state.Triggers.Left > 0.1f || state.Triggers.Right > 0.1f; }

  //public bool pressDash() { if (keyboard || !state.IsConnected) return false; return state.Buttons.A == ButtonState.Pressed; }

  public float pressDash() { if (keyboard || !state.IsConnected) return 0f; return state.Triggers.Left; }
  public float pressShoot() { if (keyboard || !state.IsConnected) return 0f; return state.Triggers.Right; }
  public bool pressEngine() { if (keyboard || !state.IsConnected) return Input.GetKey(KeyCode.UpArrow); return state.Buttons.A == ButtonState.Pressed; }

  public bool pressTweakReset() { if (!state.IsConnected) return false; return state.Buttons.Y == ButtonState.Pressed; }

  protected bool _tweaking = false;
  public bool pressTweaking()
  {
    if (!state.IsConnected) return false;

    if (state.Buttons.Back == ButtonState.Pressed && !_tweaking) _tweaking = true;
    else if (state.Buttons.Back == ButtonState.Released && _tweaking)
    {
      _tweaking = false;
      return true;
    }
    return false;
  }

  public bool pressSwapLeft() { if (!state.IsConnected) return false; return state.Buttons.LeftShoulder == ButtonState.Pressed; }
  public bool pressSwapRight() { if (!state.IsConnected) return false; return state.Buttons.RightShoulder == ButtonState.Pressed; }

  public bool pressUp() { if (!state.IsConnected) return false; return state.ThumbSticks.Left.Y > 0.5f; }
  public bool pressDown() { if (!state.IsConnected) return false; return state.ThumbSticks.Left.Y < -0.5f; }

  //public bool pressDodge() { return _dodge; }
  public bool pressDodge() { return state.Buttons.B == ButtonState.Pressed; }

  //public bool pressReady() { if (keyboard || !state.IsConnected) return Input.GetKey(KeyCode.Space); return state.Buttons.A == ButtonState.Pressed; }
  public bool pressReady() { return pressEngine(); }
  public bool pressDebug() { if (!state.IsConnected) return false; return state.Buttons.Back == ButtonState.Pressed; }

  public bool pressSkip()
  {
    if (!state.IsConnected) return false;
    if (!_skipPress && state.Buttons.Start == ButtonState.Pressed)
    {
      _skipPress = true;
    }
    else if (_skipPress && state.Buttons.Start == ButtonState.Released)
    {
      _skipPress = false;
      return true;
    }
    return false;
  }
}
