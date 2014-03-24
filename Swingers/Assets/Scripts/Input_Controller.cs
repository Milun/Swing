using UnityEngine;
using System.Collections;
using XInputDotNetPure;

[RequireComponent( typeof(Character))]

public class Input_Controller : MonoBehaviour
{
	private PlayerIndex playerIndex;
	private GamePadState state;
	private GamePadState prevState;

	Character character;

	// Use this for initialization
	void Awake()
	{
		playerIndex = (PlayerIndex)0;

		character = GetComponent<Character>();
	}
	
	// Update is called once per frame
	void Update ()
	{
		state = GamePad.GetState(playerIndex);

		if(state.IsConnected)
		{
			character.PressHorizontal(state.ThumbSticks.Left.X);

			Vector3 currentAim = new Vector3(state.ThumbSticks.Left.X,state.ThumbSticks.Left.Y,0);
			if(currentAim == Vector3.zero)
			{
				currentAim = new Vector3(1,1,0);
			}
			character.Aim(currentAim);

			if(state.Buttons.A == ButtonState.Pressed)
			{
				character.PressUp();
			}

			if(state.Buttons.RightShoulder == ButtonState.Pressed)
			{
				character.FireGrapple();
			}

			if(state.Buttons.RightShoulder == ButtonState.Released)
			{
				character.ReleaseGrapple();
			}

			prevState = state;
		}
		else
		{
			//THERE IS NO CONTROLLER, CHAOS ENSUES
		}
	}
}
