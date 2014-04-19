using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace InControl
{
	public class RazerHydraProfile : UnityInputDeviceProfile
	{
		public RazerHydraProfile()
		{
			Name = "Razer Hydra";
			Meta = "A Razer Hydra profile for steering a spaceship.";

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
					Target = InputControlType.RightTrigger,
					Source = KeyCodeButton( KeyCode.Space )
				},
				new InputControlMapping
				{
					Handle = "Left Shift",
					Target = InputControlType.LeftTrigger,
					Source = KeyCodeButton( KeyCode.LeftShift )
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

