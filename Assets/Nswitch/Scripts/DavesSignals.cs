using deVoid.Utils;

public class OnAnyButtonPressedSignal : ASignal { }
public class OnAnyButtonCancelledSignal : ASignal{ }
public class NavigationTabSignal : ASignal{ }
public class NavigationSwitchFoucsModeSignal : ASignal<NavigationFocusMode>{ }
public class NavigationSpecialNorthSignal : ASignal{ }

public class NavigationUpSignal : ASignal
{
}
public class NavigationNextSignal : ASignal{ }
public class NavigationMouseLeftClickSignal : ASignal{ }
public class NavigationPreviousSignal : ASignal{ }
public class NavigationDownSignal : ASignal{ }
public class NavigationLeftSignal : ASignal{ }
public class NavigationLeftStartedSignal : ASignal{ }
public class NavigationLeftCancelledSignal : ASignal{ }
public class NavigationRightStartedSignal : ASignal{ }
public class NavigationRightSignal : ASignal{ }
public class NavigationRightCancelledSignal : ASignal{ }
public class VehicleBoostSignal : ASignal<bool> { }
public class ActionLogSignal : ASignal<string> { }
public class PauseSignal : ASignal { }
public class ControlSchemeChangedSignal : ASignal { }
public class ControlDeviceChangedSignal : ASignal { }
public class OnAnyButtonStartedSignal : ASignal { }
public class NavigationBumperRightStartedSignal : ASignal { }
public class NavigationBumperRightSignal  : ASignal { }
public class NavigationBumperRightCancelledSignal : ASignal { }
public class NavigationBumperLeftStartedSignal : ASignal { }
public class NavigationBumperLeftCancelledSignal : ASignal { }
public class NavigationBumperLeftSignal  : ASignal { }
public class AutoAimChangedSignal  : ASignal<AutoAimChangedSignalInfo> { }
public class AutoAimChangedSignalInfo { public bool enabled; }

public enum ControlScheme
{
	None,
	AbsoluteTwinStick,
	RelativeTwinStick
}

public enum NavigationFocusMode
{
	Mouse,
	ControllerKeyboard
}

public enum EAimingMode
{
	Assisted,
	Auto
}

public enum EShootingMode
{
	Manual,
	Auto
}

public enum ControllerType
{ 
	None,
	Unknown,
	Playstation,
	Xbox,
	Switch,
	SteamDeck
}