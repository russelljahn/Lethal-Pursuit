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

			ButtonMappings = new[]
			{
				new InputControlMapping
				{
					Handle = "Spacebar",
					Target = InputControlType.Action1,
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
					Handle = "Left Mouse Click",
					Target = InputControlType.RightTrigger,
					Source = KeyCodeButton(KeyCode.Mouse0)
				},
				new InputControlMapping
				{
					Handle = "Escape",
					Target = InputControlType.Start,
					Source = KeyCodeButton(KeyCode.Escape)
				},
				new InputControlMapping
				{
					Handle = "Left Control",
					Target = InputControlType.Action3,
					Source = KeyCodeButton(KeyCode.LeftControl)
				},
				new InputControlMapping
				{
					Handle = "Q",
					Target = InputControlType.LeftBumper,
					Source = KeyCodeButton(KeyCode.Q)
				},
				new InputControlMapping
				{
					Handle = "E",
					Target = InputControlType.RightBumper,
					Source = KeyCodeButton(KeyCode.E)
				},
				new InputControlMapping
				{
					Handle = "Tab",
					Target = InputControlType.Select,
					Source = KeyCodeButton(KeyCode.Tab)
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

