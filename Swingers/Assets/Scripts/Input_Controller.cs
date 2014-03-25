using UnityEngine;
using System.Collections;
using XInputDotNetPure;

[RequireComponent( typeof(Character))]

public class Input_Controller : MonoBehaviour
{
	private PlayerIndex playerIndex;
	private GamePadState state;
	private GamePadState prevState;

<<<<<<< HEAD
	public int m_playerNum;		//Which player this is
=======
	private bool m_aPressed = false;
>>>>>>> 2fe3a640ca3f1930252a012f7ae05a57be770e13

	Character character;

	// Use this for initialization
	void Awake()
	{
		playerIndex = (PlayerIndex)(m_playerNum-1);

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
				if (!m_aPressed)
				{
					character.PressUp();
					m_aPressed = true;
				}
			}
			else
			{
				m_aPressed = false;
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
