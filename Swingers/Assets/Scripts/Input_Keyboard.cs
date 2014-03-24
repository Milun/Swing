using UnityEngine;
using System.Collections;

// Prints an error if there is no CharacterController for this object.
[RequireComponent( typeof(Character))]

public class Input_Keyboard : MonoBehaviour
{

	KeyCode moveLeft = KeyCode.LeftArrow;
	KeyCode moveRight = KeyCode.RightArrow;
	KeyCode jump = KeyCode.Z;

	bool pressJump = false;

	Character character;

	// Use this for initialization
	void Awake ()
	{
		character = GetComponent<Character>();
	}
	
	// Update is called once per frame
	void Update ()
	{
		Vector3 moveDirection = Vector3.zero;

		if(Input.GetKey (moveLeft))
		{
			character.PressHorizontal (-1.0f);
		}
		else if(Input.GetKey (moveRight))
		{
			character.PressHorizontal (1.0f);
		}

		// Only triggers once when you press the button.
		if(Input.GetKey (jump) && !pressJump)
		{
			character.PressUp ();
			pressJump = true;
		}
		else if (!Input.GetKey (jump))
		{
			pressJump = false;
		}

		if(Input.GetKey (KeyCode.X))
		{
			character.Aim(new Vector3(1,1,0));
			character.FireGrapple ();
		}
		
	}
}