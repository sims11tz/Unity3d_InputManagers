using System;
using Cysharp.Threading.Tasks;
using deVoid.Utils;
using Unity.Collections;
using UnityEngine;
#if UNITY_STANDALONE && !UNITY_EDITOR
using Steamworks;
#endif

public enum InputType
{
	MouseKeyboard,
	Controller
}

public enum ControllerAxisNavDir
{
	None,
	Left,
	Right,
	Up,
	Down
}

public class DavesInputManagerOld : MonoBehaviour, IInputManagerInterface
{
	[SerializeField, ReadOnly] private float _forward;
	[SerializeField, ReadOnly] private float _reverse;
	[SerializeField, ReadOnly] private float _steeringDelta;
	[SerializeField, ReadOnly] private float _boost;
	[SerializeField, ReadOnly] private float _handbrake;
	[SerializeField, ReadOnly] private Vector2 _weaponAimDir;
	[SerializeField, ReadOnly] private bool _isPrimaryFirePressed;
	[SerializeField, ReadOnly] private bool _isSpecialActionPressed;
	[SerializeField, ReadOnly] private bool _isGamepad;
	[SerializeField, ReadOnly] private Vector2 _absoluteControlVector;
	[SerializeField, ReadOnly] private float _absoluteControlAngle;
	
	private float _stickHorizontalThresholdNegative = -0.25f;
	private float _stickHorizontalThresholdPositive = 0.25f;
	private float _stickVerticalThresholdNegative = -0.25f;
	private float _stickVerticalThresholdPositive = 0.25f;

	private float _triggerButtonThreshold = 0.18f;
	
	public bool HasUsedUINavInput => _hasUsedUINavInput;
	private bool _hasUsedUINavInput = false;

	public NavigationFocusMode InputFocusMode
	{
		get { return _inputFocusMode; }
		set { _inputFocusMode = value; }
	}

	private NavigationFocusMode _inputFocusMode = NavigationFocusMode.Mouse;
	private ControlScheme _controlScheme;
	public ControlScheme ControlScheme => _controlScheme;

	public ControllerType ControllerType { get; private set; } = ControllerType.None;

	public bool IsGamePad => _isGamepad;
	
	public void OnEnable()
	{
#if UNITY_SWITCH
		_isGamepad = true;
#endif
		SetControlScheme(GetControlScheme());
		_ = checkSystemType();
	}

	public ControlScheme GetControlScheme()
	{
		return (ControlScheme) _controlScheme;
	}
	
	private async UniTask checkSystemType()
	{
		await UniTask.DelayFrame(2);

		DetectControllerType();
		
		if (SystemInfo.deviceType == DeviceType.Handheld)
		{
			switchNavigationFocusMode(NavigationFocusMode.ControllerKeyboard);
		}
	}

	public void OnDisable()
	{
		// Clean up if necessary
	}
	
	public float RightStickX
	{
		get { return Input.GetAxis("Controller_RightStickHorizontal"); }
	}
	
	public float RightStickY
	{
		get { return Input.GetAxis("Controller_RightStickVertical"); }
	}
	
	public float LeftStickX
	{
		get { return Input.GetAxis("Controller_LeftStickHorizontal"); }
	}
	
	public float LeftStickY
	{
		get { return Input.GetAxis("Controller_LeftStickVertical"); }
	}
	
	//If is Switch
	//If is XBOX gamepad / Steamdeck
	//If is Mouse and Keyboard
	
	//UI Nav Stuff
	//Anykey Stuff
	//Vehicle Game Stuff
	
	private void Update()
	{
		float t = Time.unscaledDeltaTime;
		
/*DEBUG STUFF*/
		// for (int xIt=0; xIt <= 19; xIt++)
		// {
		// 	bool buttonPressed = Input.GetButton("Button " + xIt);
		// 	if (buttonPressed)
		// 	{
		// 		Debug.Log("Button " + xIt + " is pressed");
		// 		Signals.Get<ActionLogSignal>().Dispatch("Button " + xIt + " is pressed");
		// 	}
		// }
		//
		// for (int zIt=3; zIt <= 28; zIt++)
		// {
		// 	float  triggerValue = Input.GetAxis("Trigger "+zIt);
		// 	if (triggerValue > 0.4 || triggerValue < -0.4)
		// 	{
		// 		Debug.Log("Trigger " + zIt + " = "+triggerValue);
		// 		Signals.Get<ActionLogSignal>().Dispatch("Trigger " + zIt + " = "+triggerValue);
		// 	}
		// }
		// return;
/*DEBUG STUFF*/

/*UI Navigation Stuff*/
		if(Input.GetButtonDown("Controller_Start")) OnPausePressed();
		if (Input.GetButtonDown("Controller_RightBumper"))
		{
			OnNavigationBumperRight();
			OnNavigationBumperRightStarted();
		}
		if (Input.GetButtonUp("Controller_RightBumper"))  OnNavigationBumperRightCancelled();
		if (Input.GetButtonDown("Controller_LeftBumper"))
		{
			OnNavigationBumperLeft();
			OnNavigationBumperLeftStarted();
		}
		if (Input.GetButtonUp("Controller_LeftBumper"))  OnNavigationBumperLeftCancelled();
		
#if UNITY_SWITCH
		if(Input.GetButtonDown("Switch_DpadUp")) OnNavigationUp();
		if(Input.GetButtonDown("Switch_DpadDown")) OnNavigationDown();
		if(Input.GetButtonDown("Switch_DpadLeft")) OnNavigationLeft();
		if(Input.GetButtonDown("Switch_DpadRight")) OnNavigationRight();
		
		if(Input.GetButtonDown("Controller_East")) OnNavigationNext();
		if(Input.GetButtonDown("Controller_South")) OnNavigationPrevious();
#else
		if (ControllerType == ControllerType.SteamDeck)
		{
			if (Input.GetAxis("SteamDeck_DpadUp") >= _stickVerticalThresholdPositive)
			{
				onNavigationAxisRequest(ControllerAxisNavDir.Up, "SteamDeck_DpadUp");
			}
			else if (Input.GetAxis("SteamDeck_DpadDown") <= _stickVerticalThresholdNegative)
			{
				onNavigationAxisRequest(ControllerAxisNavDir.Down, "SteamDeck_DpadDown");
			}
			else if (Input.GetAxis("SteamDeck_DpadLeft") >= _stickHorizontalThresholdPositive)
			{
				onNavigationAxisRequest(ControllerAxisNavDir.Left, "SteamDeck_DpadLeft");
			}
			else if (Input.GetAxis("SteamDeck_DpadRight") <= _stickHorizontalThresholdNegative)
			{
				onNavigationAxisRequest(ControllerAxisNavDir.Right, "SteamDeck_DpadRight");
			}
		}
		else if(ControllerType == ControllerType.Xbox)
		{
			if (Input.GetAxis("Xbox_DpadUp") >= _stickVerticalThresholdPositive)
			{
				onNavigationAxisRequest(ControllerAxisNavDir.Up, "Xbox_DpadUp");
			}
			else if (Input.GetAxis("Xbox_DpadDown") <= _stickVerticalThresholdNegative)
			{
				onNavigationAxisRequest(ControllerAxisNavDir.Down, "Xbox_DpadDown");
			}
			else if (Input.GetAxis("Xbox_DpadLeft") <= _stickHorizontalThresholdNegative)
			{
				onNavigationAxisRequest(ControllerAxisNavDir.Left, "Xbox_DpadLeft");
			}
			else if (Input.GetAxis("Xbox_DpadRight") >= _stickHorizontalThresholdPositive)
			{
				onNavigationAxisRequest(ControllerAxisNavDir.Right, "Xbox_DpadRight");
			}
		}
		
		if(Input.GetButtonDown("Controller_South")) OnNavigationNext();
		if(Input.GetButtonDown("Controller_East")) OnNavigationPrevious();
#endif

		if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
		{
			OnNavigationNext();
		}
		if(Input.GetKeyDown(KeyCode.Backspace)) OnNavigationPrevious();
		if (Input.GetKeyDown(KeyCode.Escape)) OnPausePressed();
		if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) OnNavigationUp();
		if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) OnNavigationDown();
		if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) OnNavigationLeft();
		if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) OnNavigationRight();
		if (Input.GetKeyDown(KeyCode.Tab)) OnNavigationTab();
		if (Input.GetKeyDown(KeyCode.Period))
		{
			OnNavigationBumperRight();
			OnNavigationBumperRightStarted();
		}
		if (Input.GetKeyUp(KeyCode.Period))  OnNavigationBumperRightCancelled();
		if (Input.GetKeyDown(KeyCode.Comma))
		{
			OnNavigationBumperLeft();
			OnNavigationBumperLeftStarted();
		}
		if (Input.GetKeyUp(KeyCode.Comma))  OnNavigationBumperLeftCancelled();
		
		if (Input.GetMouseButtonDown(0)) OnNavigationLeftMouse();
		if (Input.GetMouseButtonDown(4)) OnNavigationNextMouse();
		if (Input.GetMouseButtonDown(3)) OnNavigationPreviousMouse();
		if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2) || Input.GetMouseButtonDown(3) || Input.GetMouseButtonDown(4))
		{
			onNavigationMouseAction();
		}
//TODO --- on mouse move change device check
// _weaponAimDir = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
		
		if(Input.GetAxis("Controller_LeftStickHorizontal") <= _stickHorizontalThresholdNegative)
		{
			onNavigationAxisRequest(ControllerAxisNavDir.Left,"Controller_LeftStickHorizontal");
		}
		else if(Input.GetAxis("Controller_LeftStickHorizontal") >= _stickHorizontalThresholdPositive)
		{
			onNavigationAxisRequest(ControllerAxisNavDir.Right, "Controller_LeftStickHorizontal");
		}
		else if(Input.GetAxis("Controller_LeftStickVertical") >= _stickVerticalThresholdPositive)
		{
			onNavigationAxisRequest(ControllerAxisNavDir.Up, "Controller_LeftStickVertical");
		}
		else if(Input.GetAxis("Controller_LeftStickVertical") <= _stickVerticalThresholdNegative)
		{
			onNavigationAxisRequest(ControllerAxisNavDir.Down, "Controller_LeftStickVertical");
		}
		
		
/*UI Navigation Stuff*/

		
		
/*Gameplay Stuff*/
		_boostActiveControleString = "";
		if(!_isGamepad)
		{
			_forward = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow) ? 1 : 0;
			_reverse = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow) ? 1 : 0;
			_boost = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) ? 1 : 0;
			if (_boost > 0) _boostActiveControleString = "leftShift";
			
			_steeringDelta = Input.GetAxis("Keyboard_Horizontal");
			_handbrake = Input.GetKey(KeyCode.Space) ? 1 : 0;
			
			_weaponAimDir = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
			_isPrimaryFirePressed = Input.GetMouseButton(0);
			_isSpecialActionPressed = Input.GetMouseButton(1);
			
			_absoluteControlVector = new Vector2(Input.GetAxis("Keyboard_Horizontal"), Input.GetAxis("Keyboard_Vertical"));
		}
		else
		{
		#if UNITY_SWITCH
				_forward = (Input.GetButton("Switch_ZR")) ? 1: 0;
				_reverse = (Input.GetButton("Switch_ZL")) ? 1: 0;
		#else
			if (ControllerType == ControllerType.SteamDeck)
			{
				_forward = (Input.GetAxis("SteamDeck_RT") > _triggerButtonThreshold) ? 1: 0;
				_reverse = (Input.GetAxis("SteamDeck_LT") > _triggerButtonThreshold) ? 1: 0;
			}
			else if(ControllerType == ControllerType.Xbox)
			{
				_forward = (Input.GetAxis("Xbox_TriggerRT") > _triggerButtonThreshold) ? 1: 0;
				_reverse = (Input.GetAxis("Xbox_TriggerLT") > _triggerButtonThreshold) ? 1: 0;
			}
		#endif
			
			
			if (Input.GetButton("Controller_RightBumper"))
			{
				_boostActiveControleString = "rightShoulder";
				_boost = 1;
			}
			else if (Input.GetButton("Controller_East"))
			{
				_boostActiveControleString = "buttonEast";
				_boost = 1;
			}
			else
			{
				_boost = 0;
			}

			_handbrake = (Input.GetButton("Controller_LeftBumper")) ? 1 : 0;
			
			_weaponAimDir = new Vector2(Input.GetAxis("Controller_RightStickHorizontal"), Input.GetAxis("Controller_RightStickVertical"));
			_isPrimaryFirePressed = _weaponAimDir != Vector2.zero;

			if (Input.GetButtonDown("Controller_North"))
			{
				OnToggleAutoAimPressed();
			}

			_isSpecialActionPressed = Input.GetButton("Controller_South");
			
			_absoluteControlVector = new Vector2(Input.GetAxis("Controller_LeftStickHorizontal"), Input.GetAxis("Controller_LeftStickVertical"));
		}

		
		_absoluteControlAngle = Mathf.Atan2(_absoluteControlVector.y, _absoluteControlVector.x) * Mathf.Rad2Deg;
		if (_absoluteControlAngle < 0) _absoluteControlAngle += 360;
/*Gameplay Stuff*/
	}
	
	private bool _axistNavActive = false;
	private ControllerAxisNavDir _axisNavDir = ControllerAxisNavDir.None;
	private string _axisNavName = "";
	private async UniTask navigationAxisTick()
	{
		await UniTask.Delay(TimeSpan.FromSeconds(0.45f));
		
		if (_axisNavDir == ControllerAxisNavDir.Left)
		{
			_axistNavActive = false;
			_axisNavDir = ControllerAxisNavDir.None;
			if(Input.GetAxis(_axisNavName) <= _stickHorizontalThresholdNegative)
			{
				onNavigationAxisRequest(ControllerAxisNavDir.Left, _axisNavName);
			}
		}
		else if (_axisNavDir == ControllerAxisNavDir.Right)
		{
			_axistNavActive = false;
			_axisNavDir = ControllerAxisNavDir.None;
			if(Input.GetAxis(_axisNavName) >= _stickHorizontalThresholdPositive)
			{
				onNavigationAxisRequest(ControllerAxisNavDir.Right, _axisNavName);
			}
		}
		else if (_axisNavDir == ControllerAxisNavDir.Up)
		{
			_axistNavActive = false;
			_axisNavDir = ControllerAxisNavDir.None;
			if(Input.GetAxis(_axisNavName) >= _stickVerticalThresholdPositive)
			{
				onNavigationAxisRequest(ControllerAxisNavDir.Up, _axisNavName);
			}
		}
		else if (_axisNavDir == ControllerAxisNavDir.Down)
		{
			_axistNavActive = false;
			_axisNavDir = ControllerAxisNavDir.None;
			if(Input.GetAxis(_axisNavName) >= _stickVerticalThresholdPositive)
			{
				onNavigationAxisRequest(ControllerAxisNavDir.Down, _axisNavName);
			}
		}
	}
	
	private void onNavigationAxisRequest(ControllerAxisNavDir dir, string axisNavName)
	{
		if (_axistNavActive)  return;

		_axisNavName = axisNavName;
		_axistNavActive = true;
		_axisNavDir = dir;

		switch (dir)
		{
			case ControllerAxisNavDir.Left : OnNavigationLeft(); break;
			case ControllerAxisNavDir.Right : OnNavigationRight(); break;
			case ControllerAxisNavDir.Down : OnNavigationDown(); break;
			case ControllerAxisNavDir.Up : OnNavigationUp(); break;
		}
		
		_ = navigationAxisTick();
	}
	
	public void OnAim(Vector2 aimDir)
	{
		_weaponAimDir = aimDir;
	}

	private void OnPausePressed()
	{
		Signals.Get<PauseSignal>().Dispatch();
	}

	public void ChangeAutoAim(EAimingMode aimingMode)
	{
		// Implement auto-aim logic here
	}

	private bool _autoAim = false;
	private void OnToggleAutoAimPressed()
	{
		_autoAim = !_autoAim;

		AutoAimChangedSignalInfo info = new AutoAimChangedSignalInfo();
		info.enabled = _autoAim;
		Signals.Get<AutoAimChangedSignal>().Dispatch(info);
	}

	private bool _autoAimButtonActive = false;
	public bool AutoAimButtonActive => _autoAimButtonActive;

	public void OnAnyButtonPressed()
	{
		Signals.Get<OnAnyButtonPressedSignal>().Dispatch();
	}

	public void switchNavigationFocusMode(NavigationFocusMode mode)
	{
		// Debug.Log(" InputManager.switchNavigationFocusMode() newMode-"+mode+"  _inputFocusMode-"+_inputFocusMode);
		if(_inputFocusMode != mode)
		{
			// Debug.Log("InputManager.switchNavigationFocusMode--------- DISPATCH DISPATCHDISPATCH DISPATCHDISPATCH DISPATCH");
			_inputFocusMode = mode;
			Signals.Get<NavigationSwitchFoucsModeSignal>().Dispatch(_inputFocusMode);
		}
	}
	
	// UI Events
	private void OnNavigationUp()
	{
		_hasUsedUINavInput = true;
		switchNavigationFocusMode(NavigationFocusMode.ControllerKeyboard);
		OnAnyButtonPressed();
		Signals.Get<NavigationUpSignal>().Dispatch();
	}

	private void OnNavigationDown()
	{
		_hasUsedUINavInput = true;
		switchNavigationFocusMode(NavigationFocusMode.ControllerKeyboard);
		OnAnyButtonPressed();
		Signals.Get<NavigationDownSignal>().Dispatch();
	}

	private void OnNavigationLeft()
	{
		_hasUsedUINavInput = true;
		switchNavigationFocusMode(NavigationFocusMode.ControllerKeyboard);
		OnAnyButtonPressed();
		Signals.Get<NavigationLeftSignal>().Dispatch();
	}

	private void OnNavigationRight()
	{
		_hasUsedUINavInput = true;
		switchNavigationFocusMode(NavigationFocusMode.ControllerKeyboard);
		OnAnyButtonPressed();
		Signals.Get<NavigationRightSignal>().Dispatch();
	}

	public void OnNavigationNext()
	{
		NavigationFocusMode preMode = _inputFocusMode;
		
		switchNavigationFocusMode(NavigationFocusMode.ControllerKeyboard);
		_hasUsedUINavInput = true;

		OnAnyButtonPressed();
		
		if(preMode == NavigationFocusMode.ControllerKeyboard)
		{//Make them click the controller to switch to controller mode before they can select things.
			Signals.Get<NavigationNextSignal>().Dispatch();
		}
	}
	
	public void OnNavigationNextMouse()
	{
		_hasUsedUINavInput = true;
		OnAnyButtonPressed();
		Signals.Get<NavigationNextSignal>().Dispatch();
	}
	
	public void OnNavigationLeftMouse()
	{
		_hasUsedUINavInput = true;
		OnAnyButtonPressed();
		Signals.Get<NavigationMouseLeftClickSignal>().Dispatch();
	}
	
	public void OnNavigationPrevious()
	{
		switchNavigationFocusMode(NavigationFocusMode.ControllerKeyboard);
		_hasUsedUINavInput = true;
		OnAnyButtonPressed();
		Signals.Get<NavigationPreviousSignal>().Dispatch();
	}
	
	public void OnNavigationPreviousMouse()
	{
		_hasUsedUINavInput = true;
		OnAnyButtonPressed();
		Signals.Get<NavigationPreviousSignal>().Dispatch();
	}
	
	public void OnNavigationBumperLeft()
	{
		_hasUsedUINavInput = true;
		switchNavigationFocusMode(NavigationFocusMode.ControllerKeyboard);
		OnAnyButtonPressed();
		Signals.Get<NavigationBumperLeftSignal>().Dispatch();
	}
	
	public void OnNavigationBumperLeftStarted()
	{
		_hasUsedUINavInput = true;
		Signals.Get<NavigationBumperLeftStartedSignal>().Dispatch();
	}
	
	public void OnNavigationBumperLeftCancelled()
	{
		_hasUsedUINavInput = true;
		Signals.Get<NavigationBumperLeftCancelledSignal>().Dispatch();
	}
	
	public void OnNavigationBumperRight()
	{
		_hasUsedUINavInput = true;
		OnAnyButtonPressed();
		switchNavigationFocusMode(NavigationFocusMode.ControllerKeyboard);
		Signals.Get<NavigationBumperRightSignal>().Dispatch();
	}
	
	public void OnNavigationBumperRightStarted()
	{
		_hasUsedUINavInput = true;
		Signals.Get<NavigationBumperRightStartedSignal>().Dispatch();
	}
	
	public void OnNavigationBumperRightCancelled()
	{
		_hasUsedUINavInput = true;
		Signals.Get<NavigationBumperRightCancelledSignal>().Dispatch();
	}
	
	private void OnNavigationTab()
	{
		_hasUsedUINavInput = true;
		OnAnyButtonPressed();
		Signals.Get<NavigationTabSignal>().Dispatch();
	}

	public void onNavigationMouseAction()
	{
		switchNavigationFocusMode(NavigationFocusMode.Mouse);
	}
	
	public void SetControlScheme(ControlScheme controlSchemeType)
	{
		_controlScheme = controlSchemeType;
	}

	public void ToggleControlScheme()
	{
		SetControlScheme((ControlScheme == ControlScheme.RelativeTwinStick) ? ControlScheme.AbsoluteTwinStick : ControlScheme.RelativeTwinStick);
	}

	public string ControlSchemeText
	{
		get
		{
			switch (_controlScheme)
			{
				case ControlScheme.None: return "None";
				case ControlScheme.RelativeTwinStick: return "Relative";
				case ControlScheme.AbsoluteTwinStick: return "Absolute";
			}

			return "Unknown";
		}
	}

	public float GetThrustDelta()
	{
		return _forward - _reverse;
	}

	public float GetSteeringDelta()
	{
		return _steeringDelta;
	}

	public Vector2 GetAbsoluteControlVector()
	{
		return _absoluteControlVector;
	}

	public float GetAbsoluteControlAngle()
	{
		return _absoluteControlAngle;
	}

	public float GetBoostDelta()
	{
		return _boost;
	}

	private string _boostActiveControleString;
	public string GetBoostActiveControl()
	{
		return _boostActiveControleString;
	}

	public float GetHandbrake()
	{
		return _handbrake;
	}

	public Vector2 GetWeaponAimDir()
	{
		return _weaponAimDir;
	}

	public bool GetIsTryingToFire()
	{
		return _isPrimaryFirePressed;
	}

	public bool GetIsSpecialActionPressed()
	{
		return _isSpecialActionPressed;
	}

	public string GetJumpButtonName()
	{
		return _isGamepad ? "[X]" : "[RMB]";
	}

	public string GetBoostButtonName()
	{
		return _isGamepad ? "[R1]" : "[SHIFT]";
	}

	public string GetShootButtonName()
	{
		return _isGamepad ? "[RStick]" : "[LMB]";
	}
	
	private void DetectControllerType()
	{
#if UNITY_STANDALONE
		bool isSteamDeck = false;
		try
		{
			if (Steamworks.SteamUtils.IsSteamRunningOnSteamDeck())
			{
				isSteamDeck = true;
			}
		}
		catch (System.Exception e)
		{
			Debug.Log(e.ToString());
		}

		if (isSteamDeck || SystemInfo.operatingSystem.Contains("SteamOS"))
		{
			ControllerType = ControllerType.SteamDeck;
		}
		else if (Input.GetJoystickNames().Length > 0)
		{
			string joystickName = Input.GetJoystickNames()[0];
			if (joystickName.Contains("PlayStation"))
			{
				ControllerType = ControllerType.Playstation;
			}
			else if (joystickName.Contains("Xbox"))
			{
				ControllerType = ControllerType.Xbox;
			}
			else if (joystickName.Contains("Switch"))
			{
				ControllerType = ControllerType.Switch;
			}
			else
			{
				Debug.LogWarning($"Unknown controller type {joystickName} is being used.");
				ControllerType = ControllerType.Unknown;
			}
		}
#elif UNITY_SWITCH
		ControllerType = ControllerType.Switch;
#endif
		Signals.Get<ControlDeviceChangedSignal>().Dispatch();
	}
}
