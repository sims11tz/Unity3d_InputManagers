using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using deVoid.Utils;

namespace Nswitch.UI
{
	
	public class UIswitch : MonoBehaviour
	{
		private float _fpsCounter;
		private Label _fps;
		
		private IInputManagerInterface _acitveInputManager;

		private GameObject _oldInputGameObject;
		private GameObject _newInputGameObject;

		private Label _IsPaused;
		private Label _IsGamePad;
		private Label _ThrustDelta;
		private Label _SteeringDelta;
		private Label _AbsoluteControlVector;
		private Label _AbsoluteControlAngle;
		private Label _BoostDelta;
		private Label _BoostActiveControl;
		private Label _Handbrake;
		private Label _WeaponAimDir;
		private Label _WeaponAutoAim;
		private Label _IsTryingToFire;
		private Label _IsSpecialActionPressed;
		private Label _ControllerType;

		private Button _InputOld;
		private Button _InputNew;

		private Label _AnyButtonLabel;
		
		private Label _Horizontal;
		private Label _Vertical;
		private Label _LeftStickX;
		private Label _LeftStickY;
		private Label _RightStickX;
		private Label _RightStickY;


		private VisualElement _stuff1;
		private VisualElement _stuff2;
		
		private void OnEnable()
		{
			// Get the root visual element from the UI document
			var uiDocument = GetComponent<UIDocument>();
			var root = uiDocument.rootVisualElement;

			_oldInputGameObject = GameObject.Find("DavesInputManagerOLD");
			_oldInputGameObject.SetActive(true);
			_acitveInputManager = _oldInputGameObject.GetComponent<IInputManagerInterface>();
			
			_newInputGameObject = GameObject.Find("DavesInputManagerNew");
			_newInputGameObject.SetActive(false);
			
			Debug.Log(" UIswitch.OnEnable(*) ");

			_actionLog = root.Query<Label>("actionLog");

			_AnyButtonLabel = root.Query<Label>("AnyButtonLabel");
			
			_IsPaused = root.Query<Label>("IsPaused");
			_IsGamePad = root.Query<Label>("IsGamePad");
			_ThrustDelta = root.Query<Label>("ThrustDelta");
			_SteeringDelta = root.Query<Label>("SteeringDelta");
			_AbsoluteControlVector = root.Query<Label>("AbsoluteControlVector");
			_AbsoluteControlAngle = root.Query<Label>("AbsoluteControlAngle");
			_BoostDelta = root.Query<Label>("BoostDelta");
			_BoostActiveControl = root.Query<Label>("BoostActiveControl");
			_Handbrake = root.Query<Label>("Handbrake");
			_WeaponAimDir = root.Query<Label>("WeaponAimDir");
			_WeaponAutoAim = root.Query<Label>("WeaponAutoAim");
			_IsTryingToFire = root.Query<Label>("IsTryingToFire");
			_IsSpecialActionPressed = root.Query<Label>("IsSpecialActionPressed");
			
			_ControllerType = root.Query<Label>("ControllerType");
			_ControllerType.text = "Controller Type : None";
			
			_Horizontal = root.Query<Label>("Horizontal");
			_Vertical = root.Query<Label>("Vertical");
			_LeftStickX = root.Query<Label>("LeftStickX");
			_LeftStickY = root.Query<Label>("LeftStickY");
			_RightStickX = root.Query<Label>("RightStickX");
			_RightStickY = root.Query<Label>("RightStickY");
			
			_fps = root.Query<Label>("fps");
			
			_InputOld = root.Query<Button>("InputOld");
			_InputOld.RegisterCallback<ClickEvent>(inputOldClick);

			_InputNew = root.Query<Button>("InputNew");
			_InputNew.RegisterCallback<ClickEvent>(inputNewClick);
			
			Button _quit = root.Query<Button>("QuitBtn");
			_quit.RegisterCallback<ClickEvent>(quitClick);
			
			Button _clear = root.Query<Button>("ClearBtn");
			_clear.RegisterCallback<ClickEvent>(clearClick);
			
			Button _toggleStuff = root.Query<Button>("ToggleStuffBtn");
			_toggleStuff.RegisterCallback<ClickEvent>(toggleStuffClick);
			
			
			Button _joystickNamesBtn = root.Query<Button>("JoystickNamesBtn");
			_joystickNamesBtn.RegisterCallback<ClickEvent>(JoystickNamesClick);
			
			_stuff1 = root.Query<VisualElement>("Stuff1");
			_stuff1.style.display = DisplayStyle.Flex;
			
			_stuff2 = root.Query<VisualElement>("Stuff2");
			_stuff2.style.display = DisplayStyle.None;
			
			Signals.Get<PauseSignal>().AddListener(sig_PauseSignal);
			Signals.Get<OnAnyButtonPressedSignal>().AddListener(sig_OnAnyButtonPressedSignal);
			Signals.Get<ControlDeviceChangedSignal>().AddListener(sig_ControlDeviceChangedSignal);
			Signals.Get<NavigationSwitchFoucsModeSignal>().AddListener(sig_NavigationSwitchFoucsModeSignal);
			Signals.Get<NavigationBumperLeftSignal>().AddListener(sig_NavigationBumperLeftSignal);
			Signals.Get<NavigationBumperLeftStartedSignal>().AddListener(sig_NavigationBumperLeftStartedSignal);
			Signals.Get<NavigationBumperLeftCancelledSignal>().AddListener(sig_NavigationBumperLeftCancelledSignal);
			Signals.Get<NavigationBumperRightSignal>().AddListener(sig_NavigationBumperRightSignal);
			Signals.Get<NavigationBumperRightStartedSignal>().AddListener(sig_NavigationBumperRightStartedSignal);
			Signals.Get<NavigationBumperRightCancelledSignal>().AddListener(sig_NavigationBumperRightCancelledSignal);
			Signals.Get<NavigationUpSignal>().AddListener(sig_NavigationUpSignal);
			Signals.Get<NavigationDownSignal>().AddListener(sig_NavigationDownSignal);
			Signals.Get<NavigationLeftSignal>().AddListener(sig_NavigationLeftSignal);
			Signals.Get<NavigationLeftStartedSignal>().AddListener(sig_NavigationLeftStartedSignal);
			Signals.Get<NavigationLeftCancelledSignal>().AddListener(sig_NavigationLeftCancelledSignal);
			Signals.Get<NavigationRightSignal>().AddListener(sig_NavigationRightSignal);
			Signals.Get<NavigationRightStartedSignal>().AddListener(sig_NavigationRightStartedSignal);
			Signals.Get<NavigationRightCancelledSignal>().AddListener(sig_NavigationRightCancelledSignal);
			Signals.Get<NavigationNextSignal>().AddListener(sig_NavigationNextSignal);
			Signals.Get<NavigationMouseLeftClickSignal>().AddListener(sig_NavigationMouseLeftClickSignal);
			Signals.Get<NavigationPreviousSignal>().AddListener(sig_NavigationPreviousSignal);
			Signals.Get<NavigationTabSignal>().AddListener(sig_NavigationTabSignal);
			Signals.Get<AutoAimChangedSignal>().AddListener(sig_AutoAimChangedSignal);
			Signals.Get<ActionLogSignal>().AddListener(sig_ActionLog);
			Signals.Get<ControlDeviceChangedSignal>().AddListener(sig_DeviceChangedSignal);
			
			
			JoystickNamesClick(new ClickEvent());
			
			_ = FPSUpdate();
		}

		private void sig_ActionLog(string str)
		{
			UpdateActionLog(str);
		}
		
		private Label _actionLog;
		private void UpdateActionLog(string action)
		{
			_actionLog.text = action+"\n"+_actionLog.text;
		}
		
		private async UniTaskVoid FPSUpdate()
		{
			while(true)
			{
				_fpsCounter = 1f / Time.unscaledDeltaTime;
				await UniTask.Delay(TimeSpan.FromSeconds(.1f));
			}
		}

		private void Update()
		{
			
			_IsPaused.text = "GamePaused : "+_isPaused.ToString();
			
			_fps.text = "FPS : " + Mathf.Round(_fpsCounter);
			
			_LeftStickX.text = "LeftStickX : " + _acitveInputManager.LeftStickX.ToString();
			_LeftStickY.text = "LestStickY : " + _acitveInputManager.LeftStickY.ToString();
			_RightStickX.text ="RightStickX : " +  _acitveInputManager.RightStickX.ToString();
			_RightStickY.text = "RightStickY : " + _acitveInputManager.RightStickY.ToString();
			
			_IsGamePad.text = "_IsGamePad : " + _acitveInputManager.IsGamePad.ToString();
			_ThrustDelta.text = "ThrustDelta : " + _acitveInputManager.GetThrustDelta();
			_SteeringDelta.text = "SteeringDelta : " + _acitveInputManager.GetSteeringDelta();
			_AbsoluteControlVector.text = "AbsoluteControlVector : " + _acitveInputManager.GetAbsoluteControlVector();
			_AbsoluteControlAngle.text = "AbsoluteControlAngle : " + _acitveInputManager.GetAbsoluteControlAngle();
			_BoostDelta.text = "BoostDelta : " + _acitveInputManager.GetBoostDelta();
			_BoostActiveControl.text = "BoostActiveControl : " + _acitveInputManager.GetBoostActiveControl();
			_Handbrake.text = "Handbrake : " + _acitveInputManager.GetHandbrake();
			_WeaponAutoAim.text = "WeaponAutoAim : "+_autoAim.ToString();
			_WeaponAimDir.text = "WeaponAimDir : " + _acitveInputManager.GetWeaponAimDir();
			_IsTryingToFire.text = "IsTryingToFire : " + _acitveInputManager.GetIsTryingToFire();
			_IsSpecialActionPressed.text = "IsSpecialActionPressed : " + _acitveInputManager.GetIsSpecialActionPressed();
		}

		private void quitClick(ClickEvent ce)
		{
			Application.Quit();
		}
		
		private void clearClick(ClickEvent ce)
		{
			Debug.Log("clearClick()");
			_actionLog.text = "";
		}
		
		private void JoystickNamesClick(ClickEvent ce)
		{
			UpdateActionLog("JoyStickNames_________________");
			string[] joysticks = Input.GetJoystickNames();
			foreach (var joystick in joysticks)
			{
				UpdateActionLog(joystick);
			}
		}
		
		private void toggleStuffClick(ClickEvent ce)
		{
			if(_stuff1.style.display == DisplayStyle.None)
			{
				_stuff1.style.display = DisplayStyle.Flex;
				_stuff2.style.display = DisplayStyle.None;
			}
			else
			{
				_stuff1.style.display = DisplayStyle.None;
				_stuff2.style.display = DisplayStyle.Flex;
			}
		}
		
		private void inputNewClick(ClickEvent ce)
		{
			_actionLog.text = "";
			UpdateActionLog("SWITCH TO NEW INPUT SYSTEM");
			_newInputGameObject.SetActive(true);
			_oldInputGameObject.SetActive(false);
			
			_acitveInputManager = _newInputGameObject.GetComponent<IInputManagerInterface>();
		}
		
		private void inputOldClick(ClickEvent ce)
		{
			_actionLog.text = "";
			UpdateActionLog("SWITCH TO OLD INPUT SYSTEM");_newInputGameObject.SetActive(true);
			_oldInputGameObject.SetActive(true);
			_newInputGameObject.SetActive(false);
			
			_acitveInputManager = _oldInputGameObject.GetComponent<IInputManagerInterface>();
		}

		private void sig_DeviceChangedSignal()
		{
			_ControllerType.text = "Controller Type : "+_acitveInputManager.ControllerType.ToString();
		}
		
		private void sig_NavigationMouseLeftClickSignal()
		{
			UpdateActionLog("sig_NavigationMouseLeftClickSignal");
		}
		
		private void sig_NavigationPreviousSignal()
		{
			UpdateActionLog("sig_NavigationPreviousSignal");
		}

		private bool _autoAim = false;
		public void sig_AutoAimChangedSignal(AutoAimChangedSignalInfo info)
		{
			UpdateActionLog("sig_AutoAimChangedSignal : "+info.enabled);
			_autoAim = info.enabled;
		}
		
		private void sig_NavigationTabSignal()
		{
			UpdateActionLog("sig_NavigationTabSignal");
		}
				
		private void sig_NavigationLeftSignal()
		{
			UpdateActionLog("sig_NavigationLeftSignal");
		}
		
		private void sig_NavigationLeftStartedSignal()
		{
			UpdateActionLog("sig_NavigationLeftStartedSignal");
		}
		
		private void sig_NavigationLeftCancelledSignal()
		{
			UpdateActionLog("sig_NavigationLeftCancelledSignal");
		}
		
		private void sig_NavigationRightSignal()
		{
			UpdateActionLog("sig_NavigationRightSignal");
		}
		
		private void sig_NavigationRightStartedSignal()
		{
			UpdateActionLog("sig_NavigationRightStartedSignal");
		}
		
		private void sig_NavigationRightCancelledSignal()
		{
			UpdateActionLog("sig_NavigationRightCancelledSignal");
		}
		
		private void sig_NavigationNextSignal()
		{
			UpdateActionLog("sig_NavigationNextSignal");
		}
		
		private void sig_NavigationBumperRightSignal()
		{
			UpdateActionLog("sig_NavigationBumperRightSignal");
		}
		
		private void sig_NavigationBumperRightStartedSignal()
		{
			UpdateActionLog("sig_NavigationBumperRightStartedSignal");
		}
		
		private void sig_NavigationBumperRightCancelledSignal()
		{
			UpdateActionLog("sig_NavigationBumperRightCancelledSignal");
		}
		
		private void sig_NavigationUpSignal()
		{
			UpdateActionLog("sig_NavigationUpSignal");
		}
		
		private void sig_NavigationDownSignal()
		{
			UpdateActionLog("sig_NavigationDownSignal");
		}

		private bool _isPaused = false; 
		private void sig_PauseSignal()
		{
			_isPaused = !_isPaused;
			UpdateActionLog("sig_PauseSignal");
		}
		
		private void sig_OnAnyButtonPressedSignal()
		{
			// UpdateActionLog("sig_OnAnyButtonPressedSignal");
			_ = showAnyKey();
		}

		private bool _showingAnyKey = false;
		private async UniTask showAnyKey()
		{
			if (_showingAnyKey) return;
			_showingAnyKey = true;
			_AnyButtonLabel.style.backgroundColor = new StyleColor(Color.green);
			await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
			_AnyButtonLabel.style.backgroundColor = new StyleColor(Color.clear);
			_showingAnyKey = false;
		}
		
		private void sig_ControlDeviceChangedSignal()
		{
			UpdateActionLog("sig_ControlDeviceChangedSignal");
		}
		
		private void sig_NavigationSwitchFoucsModeSignal(NavigationFocusMode mode)
		{
			UpdateActionLog("sig_NavigationSwitchFoucsModeSignal="+mode);
		}
		
		private void sig_NavigationBumperLeftSignal()
		{
			UpdateActionLog("sig_NavigationBumperLeftSignal");
		}
		
		private void sig_NavigationBumperLeftStartedSignal()
		{
			UpdateActionLog("sig_NavigationBumperLeftStartedSignal");
		}
		
		private void sig_NavigationBumperLeftCancelledSignal()
		{
			UpdateActionLog("sig_NavigationBumperLeftCancelledSignal");
		}
		
	}
}