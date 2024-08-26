using UnityEngine;

public interface IInputManagerInterface
{
	void ToggleControlScheme();

	string ControlSchemeText { get; }

	float GetThrustDelta();

	float GetSteeringDelta();

	Vector2 GetAbsoluteControlVector();

	float GetAbsoluteControlAngle();

	float GetBoostDelta();

	string GetBoostActiveControl();

	float GetHandbrake();

	Vector2 GetWeaponAimDir();

	bool GetIsTryingToFire();

	bool GetIsSpecialActionPressed();

	ControllerType ControllerType { get; }
	
	string GetJumpButtonName();

	string GetBoostButtonName();

	string GetShootButtonName();

	bool IsGamePad { get; }

	float LeftStickX { get; }
	float LeftStickY { get; }
	float RightStickX { get; }
	float RightStickY { get; }
}