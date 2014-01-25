using UnityEngine;

[RequireComponent(typeof(PlatformerCharacter2D))]
public class Platformer2DUserControl : MonoBehaviour 
{
	private PlatformerCharacter2D character;
    private bool jump;


	void Awake()
	{
		character = GetComponent<PlatformerCharacter2D>();
	}

    void Update ()
    {
        // Read the jump input in Update so button presses aren't missed.
        if (CrossPlatformInput.GetButtonDown("Jump"))
            jump = true;
    }

	void FixedUpdate()
	{
		// Read the inputs.
		bool crouch = Input.GetKey(KeyCode.LeftControl);
		float h = CrossPlatformInput.GetAxis("Horizontal");

		// Pass all parameters to the character control script.
		character.Move( h, crouch , jump );

        // Reset the jump input once it has been used.
	    jump = false;
	}
}
