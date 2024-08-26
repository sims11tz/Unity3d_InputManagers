using System;
using Cysharp.Threading.Tasks;
using deVoid.Utils;
using Unity.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
// using UnityEngine.InputSystem.Switch;
using UnityEngine.InputSystem.XInput;

[RequireComponent(typeof(PlayerInput))]
public class DavesInputManager : MonoBehaviour
{
	[SerializeField, ReadOnly] private float _forward;
	[SerializeField, ReadOnly] private float _reverse;
	[SerializeField, ReadOnly] private float _steeringDelta;
	[SerializeField, ReadOnly] private float _boost;

	[SerializeField, ReadOnly] private float _handbrake;

	[SerializeField, ReadOnly] private Vector2 _weaponAimDir;

	// Buttons
	[SerializeField, ReadOnly] private bool _isPrimaryFirePressed;
	[SerializeField, ReadOnly] private bool _isSpecialActionPressed;

	[SerializeField, ReadOnly] private bool _isGamepad;

	[SerializeField, ReadOnly] private Vector2 _absoluteControlVector;
	[SerializeField, ReadOnly] private float _absoluteControlAngle;

	//Navigation
	public bool HasUsedUINavInput => _hasUsedUINavInput;
	private bool _hasUsedUINavInput = false;
	
	public NavigationFocusMode InputFocusMode
	{
		get
		{
			return _inputFocusMode;
		}
		set
		{
			_inputFocusMode = value;
		}
	}

	private NavigationFocusMode _inputFocusMode = NavigationFocusMode.Mouse;
	
	public bool IsGamePad => _isGamepad;
	
	private PlayerInputActions _inputActions;

	private ControlScheme _controlScheme;
	public ControlScheme ControlScheme => _controlScheme;

	public ControllerType ControllerType { get; private set; } = ControllerType.None;
	
	public void OnEnable()
	{
		#if UNITY_SWITCH && !UNITY_EDITOR
				_isGamepad = true;
		#endif
		
		_inputActions = new PlayerInputActions();
		_inputActions.Enable();
		_inputActions.Vehicle.Pause.performed += OnPausePressed;
		
		_inputActions.Vehicle.ToggleAutoAim.performed += OnToggleAutoAimPressed;
		_inputActions.Vehicle.ToggleAutoAim.started += OnToggleAutoAimStarted;
		_inputActions.Vehicle.ToggleAutoAim.canceled += OnToggleAutoAimCancelled;
		
		// _inputActions.Vehicle.Boost.started += OnBoostStarted;
		// _inputActions.Vehicle.Boost.canceled += OnBoostCancelled;
		
		_inputActions.Vehicle.AnyButton.performed += OnAnyButtonPressed;
		_inputActions.Vehicle.AnyButton.started += OnAnyButtonStarted;
		_inputActions.Vehicle.AnyButton.canceled += OnAnyButtonCancelled;
		
		_inputActions.UI.NavigationUp.performed += OnNavigationUp;
		
		_inputActions.UI.NavigationDown.performed += OnNavigationDown;
		
		_inputActions.UI.NavigationLeftBumper.performed += OnNavigationBumperLeft;
		_inputActions.UI.NavigationLeftBumper.started += OnNavigationBumperLeftStarted;
		_inputActions.UI.NavigationLeftBumper.canceled += OnNavigationBumperLeftCancelled;
		_inputActions.UI.NavigationLeft.performed += OnNavigationLeft;
		_inputActions.UI.NavigationLeft.started += OnNavigationLeftStarted;
		_inputActions.UI.NavigationLeft.canceled += OnNavigationLeftCancelled;
		
		_inputActions.UI.NavigationRightBumper.performed += OnNavigationBumperRight;
		_inputActions.UI.NavigationRightBumper.started += OnNavigationBumperRightStarted;
		_inputActions.UI.NavigationRightBumper.canceled += OnNavigationBumperRightCancelled;
		_inputActions.UI.NavigationRight.performed += OnNavigationRight;
		_inputActions.UI.NavigationRight.started += OnNavigationRightStarted;
		_inputActions.UI.NavigationRight.canceled += OnNavigationRightCancelled;
		
	#if UNITY_SWITCH && !UNITY_EDITOR
		_inputActions.UI.NavigationNext.performed += OnNavigationPrevious;
		_inputActions.UI.NavigationPrevious.performed += OnNavigationNext;
	#else
		_inputActions.UI.NavigationNext.performed += OnNavigationNext;
		_inputActions.UI.NavigationPrevious.performed += OnNavigationPrevious;
	#endif
		
		_inputActions.UI.NavigationNextMouse.performed += OnNavigationNextMouse;
		_inputActions.UI.NavigationLeftMouse.performed += OnNavigationLeftMouse;
		
		_inputActions.UI.NavigationPreviousMouse.performed += OnNavigationPreviousMouse;
		_inputActions.UI.NavigationTab.performed += OnNavigationTab;
		
		_inputActions.UI.MouseAction.performed += onNavigationMouseAction;
		
		SetControlScheme(GetControlScheme());

		_ = checkSystemType();
	}
	
	
	public ControlScheme GetControlScheme()
	{
		return (ControlScheme) _controlScheme;
	}

	private int _activeControlSchemeIndex = 0;
	public void SetActiveControlScheme(int controlSchemeIndex)
	{
		_activeControlSchemeIndex = controlSchemeIndex;
	}
	
	public int GetActiveControlSchemeIndex()
	{
		return _activeControlSchemeIndex;
	}

	private async UniTask checkSystemType()
	{
		await UniTask.DelayFrame(2);
		
		if(SystemInfo.deviceType == DeviceType.Handheld)
		{
			switchNavigationFocusMode(NavigationFocusMode.ControllerKeyboard);
		}
	}

	public void OnDisable()
	{
		_inputActions.Vehicle.Pause.performed -= OnPausePressed;
		
		_inputActions.Vehicle.AnyButton.performed -= OnAnyButtonPressed;
		_inputActions.Vehicle.AnyButton.started -= OnAnyButtonStarted;
		_inputActions.Vehicle.AnyButton.canceled -= OnAnyButtonCancelled;
		
		_inputActions.UI.NavigationUp.performed -= OnNavigationUp;
		
		_inputActions.UI.NavigationDown.performed -= OnNavigationDown;
		
		_inputActions.UI.NavigationLeftBumper.performed -= OnNavigationBumperLeft;
		_inputActions.UI.NavigationLeftBumper.started += OnNavigationBumperLeftStarted;
		_inputActions.UI.NavigationLeftBumper.canceled += OnNavigationBumperLeftCancelled;
		_inputActions.UI.NavigationLeft.performed -= OnNavigationLeft;
		_inputActions.UI.NavigationLeft.started -= OnNavigationLeftStarted;
		_inputActions.UI.NavigationLeft.canceled -= OnNavigationLeftCancelled;
		
		_inputActions.UI.NavigationRightBumper.performed -= OnNavigationBumperRight;
		_inputActions.UI.NavigationRightBumper.started += OnNavigationBumperRightStarted;
		_inputActions.UI.NavigationRightBumper.canceled += OnNavigationBumperRightCancelled;
		_inputActions.UI.NavigationRight.performed -= OnNavigationRight;
		_inputActions.UI.NavigationRight.started -= OnNavigationRightStarted;
		_inputActions.UI.NavigationRight.canceled -= OnNavigationRightCancelled;
		
	#if UNITY_SWITCH && !UNITY_EDITOR
		_inputActions.UI.NavigationNext.performed -= OnNavigationPrevious;
		_inputActions.UI.NavigationPrevious.performed -= OnNavigationNext;
	#else
		_inputActions.UI.NavigationNext.performed -= OnNavigationNext;
		_inputActions.UI.NavigationPrevious.performed -= OnNavigationPrevious;
	#endif
		
		_inputActions.UI.NavigationNextMouse.performed -= OnNavigationNextMouse;
		_inputActions.UI.NavigationLeftMouse.performed -= OnNavigationLeftMouse;
		_inputActions.UI.NavigationPreviousMouse.performed -= OnNavigationPreviousMouse;
		_inputActions.UI.NavigationTab.performed -= OnNavigationTab;
		
		_inputActions.UI.MouseAction.performed -= onNavigationMouseAction;
		
		_inputActions.Disable();
	}

	private void Update()
	{
		// gamepad's R/L triggers don't always max at 1 so we manually set it to go max speed
		float forwardValue = _inputActions.Vehicle.Forward.ReadValue<float>();
		float reverseValue = _inputActions.Vehicle.Reverse.ReadValue<float>();
		_forward = (forwardValue > 0.1f) ? 1 : 0;
		_reverse = (reverseValue > 0.1f) ? 1 : 0;
		
		_boost = _inputActions.Vehicle.Boost.ReadValue<float>();
		
		_steeringDelta = _inputActions.Vehicle.Steering.ReadValue<float>();
		_handbrake = _inputActions.Vehicle.Handbrake.ReadValue<float>();
		_isSpecialActionPressed = _inputActions.Vehicle.SpecialAction.ReadValue<float>() > 0.1f;
		
		if(_isGamepad)
			_isPrimaryFirePressed = _weaponAimDir != Vector2.zero;
		else
			_isPrimaryFirePressed = Mouse.current.leftButton.ReadValue() > 0.1f;

		_absoluteControlVector = _inputActions.Vehicle.AbsoluteControl.ReadValue<Vector2>();
		_absoluteControlAngle = Mathf.Atan2(_absoluteControlVector.y, _absoluteControlVector.x) * Mathf.Rad2Deg;
		if(_absoluteControlAngle < 0)
			_absoluteControlAngle += 360;

		// Debug.Log($"Control: {_absoluteControlVector.y}, {_absoluteControlVector.x} Angle: {_absoluteControlAngle}");
	}
	
	public void OnAim(InputAction.CallbackContext context)
	{
		if(context.canceled) _weaponAimDir = Vector2.zero;
		if (!context.performed)  return;
		
		Vector2 aimDir = context.ReadValue<Vector2>();
		if(!_isGamepad)
		{
			// Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
			// Vector2 middleOfScreen = new Vector2(Screen.width / 2, Screen.height / 2);
			// aimDir = (mouseScreenPos - middleOfScreen).normalized;
		}

		_weaponAimDir = aimDir;
	}

	private void OnPausePressed(InputAction.CallbackContext context)
	{
		Signals.Get<PauseSignal>().Dispatch();
	}

	public void ChangeAutoAim(EAimingMode aimingMode)
	{
		
	}

	private bool _autoAim = false;
	private void OnToggleAutoAimPressed(InputAction.CallbackContext context)
	{
		_autoAim = !_autoAim;
		
		AutoAimChangedSignalInfo info = new AutoAimChangedSignalInfo();
		info.enabled = _autoAim;
		Signals.Get<AutoAimChangedSignal>().Dispatch(info);
	}

	private bool _autoAimButtonActive = false;
	public bool AutoAimButtonActive => _autoAimButtonActive;
	private void OnToggleAutoAimStarted(InputAction.CallbackContext context)
	{
		_autoAimButtonActive = true;
	}
	
	private void OnToggleAutoAimCancelled(InputAction.CallbackContext context)
	{
		_autoAimButtonActive = false;
	}
	
	
	private void OnAnyButtonPressed(InputAction.CallbackContext context)
	{
		Signals.Get<OnAnyButtonPressedSignal>().Dispatch();
	}
	
	private void OnAnyButtonStarted(InputAction.CallbackContext context)
	{
		Signals.Get<OnAnyButtonStartedSignal>().Dispatch();
	}
	
	private void OnAnyButtonCancelled(InputAction.CallbackContext context)
	{
		Signals.Get<OnAnyButtonCancelledSignal>().Dispatch();
	}

	// private Vector2 _prevMousePosition;
	public void OnDeviceChanged(PlayerInput playerInput)
	{
		bool oldIsGamepad = _isGamepad;
	#if UNITY_SWITCH && !UNITY_EDITOR
		_isGamepad = true;
	#else
		_isGamepad = playerInput.currentControlScheme.Equals("Gamepad");
	#endif
		
		if(_isGamepad == oldIsGamepad)
		{
			return;
		}
		
		if(_isGamepad)
		{
			#if !UNITY_EDITOR
			if(Cursor.visible)
			{
				Cursor.visible = false;
			}
			#endif

			bool isSteamDeck = false;
#if UNITY_STANDALONE
			try
			{
				// if(Steamworks.SteamUtils.IsSteamRunningOnSteamDeck())
				// {
				// 	isSteamDeck = true;
				// }
			}
			catch(Exception e)
			{
				Debug.Log(e.ToString());
			}
#endif

#if UNITY_STANDALONE
			if(isSteamDeck || SystemInfo.operatingSystem.Contains("SteamOS"))
			{
				ControllerType = ControllerType.SteamDeck;
			}
			else if(Gamepad.current is DualShockGamepad)
			{
				ControllerType = ControllerType.Playstation;
			}
			else if(Gamepad.current is XInputController)
			{
				ControllerType = ControllerType.Xbox;
			}
			// else if(Gamepad.current is SwitchProControllerHID)
			// {
			// 	ControllerType = ControllerType.Switch;
			// }
			// else
			// {
			// 	Debug.LogWarning($"Unknown controller type {Gamepad.current.GetType().Name} is being used.");
			// 	ControllerType = ControllerType.Unknown;
			// }
#elif UNITY_SWITCH
			ControllerType = ControllerType.Switch;
#endif
		}
		else
		{
			if(!Cursor.visible)
			{
				Cursor.visible = true;
			}
			
			ControllerType = ControllerType.None;
		}
		
		Signals.Get<ControlDeviceChangedSignal>().Dispatch();
	}
	
	//UI Events
	public void onNavigationMouseAction(InputAction.CallbackContext context)
	{
		switchNavigationFocusMode(NavigationFocusMode.Mouse);
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
	
	public void OnNavigationBumperLeft(InputAction.CallbackContext context)
	{
		_hasUsedUINavInput = true;
		switchNavigationFocusMode(NavigationFocusMode.ControllerKeyboard);
		Signals.Get<NavigationBumperLeftSignal>().Dispatch();
	}
	
	public void OnNavigationBumperLeftStarted(InputAction.CallbackContext context)
	{
		_hasUsedUINavInput = true;
		Signals.Get<NavigationBumperLeftStartedSignal>().Dispatch();
	}
	
	public void OnNavigationBumperLeftCancelled(InputAction.CallbackContext context)
	{
		_hasUsedUINavInput = true;
		Signals.Get<NavigationBumperLeftCancelledSignal>().Dispatch();
	}
	
	public void OnNavigationBumperRight(InputAction.CallbackContext context)
	{
		_hasUsedUINavInput = true;
		switchNavigationFocusMode(NavigationFocusMode.ControllerKeyboard);
		Signals.Get<NavigationBumperRightSignal>().Dispatch();
	}
	
	public void OnNavigationBumperRightStarted(InputAction.CallbackContext context)
	{
		_hasUsedUINavInput = true;
		Signals.Get<NavigationBumperRightStartedSignal>().Dispatch();
	}
	
	public void OnNavigationBumperRightCancelled(InputAction.CallbackContext context)
	{
		_hasUsedUINavInput = true;
		Signals.Get<NavigationBumperRightCancelledSignal>().Dispatch();
	}
	
	public void OnNavigationUp(InputAction.CallbackContext context)
	{
		_hasUsedUINavInput = true;
		switchNavigationFocusMode(NavigationFocusMode.ControllerKeyboard);
		Signals.Get<NavigationUpSignal>().Dispatch();
	}
	
	public void OnNavigationDown(InputAction.CallbackContext context)
	{
		_hasUsedUINavInput = true;
		switchNavigationFocusMode(NavigationFocusMode.ControllerKeyboard);
		Signals.Get<NavigationDownSignal>().Dispatch();
	}
	
	public void OnNavigationLeft(InputAction.CallbackContext context)
	{
		_hasUsedUINavInput = true;
		switchNavigationFocusMode(NavigationFocusMode.ControllerKeyboard);
		Signals.Get<NavigationLeftSignal>().Dispatch();
	}
	
	public void OnNavigationLeftStarted(InputAction.CallbackContext context)
	{
		_hasUsedUINavInput = true;
		Signals.Get<NavigationLeftStartedSignal>().Dispatch();
	}
	
	public void OnNavigationLeftCancelled(InputAction.CallbackContext context)
	{
		_hasUsedUINavInput = true;
		Signals.Get<NavigationLeftCancelledSignal>().Dispatch();
	}
	
	public void OnNavigationRight(InputAction.CallbackContext context)
	{
		_hasUsedUINavInput = true;
		switchNavigationFocusMode(NavigationFocusMode.ControllerKeyboard);
		Signals.Get<NavigationRightSignal>().Dispatch();
	}
	
	public void OnNavigationRightStarted(InputAction.CallbackContext context)
	{
		_hasUsedUINavInput = true;
		Signals.Get<NavigationRightStartedSignal>().Dispatch();
	}
	
	public void OnNavigationRightCancelled(InputAction.CallbackContext context)
	{
		_hasUsedUINavInput = true;
		Signals.Get<NavigationRightCancelledSignal>().Dispatch();
	}
	
	public void OnNavigationNext(InputAction.CallbackContext context)
	{
		NavigationFocusMode preMode = _inputFocusMode;
		
		switchNavigationFocusMode(NavigationFocusMode.ControllerKeyboard);
		_hasUsedUINavInput = true;

		if(preMode == NavigationFocusMode.ControllerKeyboard)
		{//Make them click the controller to switch to controller mode before they can select things.
			Signals.Get<NavigationNextSignal>().Dispatch();
		}
	}
	
	public void OnNavigationNextMouse(InputAction.CallbackContext context)
	{
		_hasUsedUINavInput = true;
		Signals.Get<NavigationNextSignal>().Dispatch();
	}
	
	public void OnNavigationLeftMouse(InputAction.CallbackContext context)
	{
		_hasUsedUINavInput = true;
		Signals.Get<NavigationMouseLeftClickSignal>().Dispatch();
	}
	
	public void OnNavigationPrevious(InputAction.CallbackContext context)
	{
		switchNavigationFocusMode(NavigationFocusMode.ControllerKeyboard);
		_hasUsedUINavInput = true;
		Signals.Get<NavigationPreviousSignal>().Dispatch();
	}
	
	public void OnNavigationPreviousMouse(InputAction.CallbackContext context)
	{
		_hasUsedUINavInput = true;
		Signals.Get<NavigationPreviousSignal>().Dispatch();
	}
	
	public void OnNavigationTab(InputAction.CallbackContext context)
	{
		_hasUsedUINavInput = true;
		Signals.Get<NavigationTabSignal>().Dispatch();
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
			switch(_controlScheme)
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

	public string GetBoostActiveControl()
	{
		return _inputActions == null || _inputActions.Vehicle.Boost.activeControl==null?"":_inputActions.Vehicle.Boost.activeControl.name;
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
}