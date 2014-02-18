using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace InControl
{
	public class SpaceshipKeyboardProfile2 : UnityInputDeviceProfile
	{
		public SpaceshipKeyboardProfile2()
		{
			Name = "Keyboard";
			Meta = "A keyboard profile for steering a spaceship.";

			SupportedPlatforms = new[]
			{
				"Windows",
				"Mac",
				"Linux"
			};

			Sensitivity = 1.0f;
			DeadZone = 0.0f;

			ButtonMappings = new[]
			{
				new InputControlMapping
				{
					Handle = "Spacebar",
					Target = InputControlType.RightTrigger,
					Source = KeyCodeButton( KeyCode.Space )
				},
				new InputControlMapping
				{
					Handle = "Left Shift",
					Target = InputControlType.LeftTrigger,
					Source = KeyCodeButton( KeyCode.LeftShift )
				},
				new InputControlMapping
				{
					Handle = "E",
					Target = InputControlType.Action3,
					Source = KeyCodeButton(KeyCode.Mouse0)
				},
			};

			AnalogMappings = new[]
			{
				new InputControlMapping
				{
					Handle = "Arrow Keys X",
					Target = InputControlType.LeftStickX,
					Source = KeyCodeAxis( KeyCode.LeftArrow, KeyCode.RightArrow )
				},
				new InputControlMapping
				{
					Handle = "Arrow Keys Y",
					Target = InputControlType.LeftStickY,
					Source = KeyCodeAxis( KeyCode.DownArrow, KeyCode.UpArrow )
				},
			};
		}
	}
}

