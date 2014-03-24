using UnityEngine;
using System.Collections;

[RequireComponent( typeof(CharacterCommon))]
[RequireComponent( typeof(CharacterPhysics))]
public class CharacterAnimation : MonoBehaviour
{
	CharacterCommon 	charCommon;
	CharacterPhysics 	charPhysics;
	
	private int facingLast = 0;		// The direction the character was facing in the last frame.
	private int facingCurr = 0;		// The current direction the character is facing.
	private int facingDest = 0;		// The destination the character needs to turn to face.
	private const int facingSpeed = 15;	// The speed the character turns in that direction.
	
	// Use this for initialization
	void Awake ()
	{
		charCommon 	= GetComponent<CharacterCommon>();
		charPhysics = GetComponent<CharacterPhysics>();
	}
	
	/**
	 * Returns a value from 0 - 1 which is the percentage of where the current animation is in being played.
	 **/
	private float GetAnimPercent(string animName)
	{
		return animation[animName].time/animation[animName].length;
	}
	
	/**
	 * Handles walking AND running
	 **/
	void AnimWalk()
	{
		// Speed from 0 - 1, where 1 is the maximum running speed.
		float speed = Mathf.Abs(charPhysics.xSpeed) / charCommon.runSpeed;
		
		// Synch the walking animation and running animation (as if they were playing simultaneously.
		// This allows the two animations to be transitioned between smoothly.
		
		// Note: With the current knight animation, this doesnt work as his run and walk cycles are inverted.
		// Whoops.
		if (!animation.IsPlaying("anim_walk") && animation.IsPlaying("anim_run"))
		{
			animation["anim_walk"].time = GetAnimPercent("anim_run")*animation["anim_walk"].length;
		}
		else if (!animation.IsPlaying("anim_run") && animation.IsPlaying("anim_walk"))
		{
			animation["anim_run"].time = GetAnimPercent("anim_walk")*animation["anim_run"].length;
		}
		
		// Check if the speed is low enough for the character to walk.
		if (speed < 0.7f)
		{
			animation["anim_walk"].speed = speed*4;
			
			// Smoothly transitions towards this animation (takes half a second).
			animation.CrossFade("anim_walk", 0.6f);
		}
		else
		{
			animation["anim_run"].speed = speed;
			
			// Smoothly transitions towards this animation (takes half a second).
			animation.CrossFade("anim_run", 0.6f);
		}
	}
	
	private void Turn()
	{
		if (charCommon.facing != facingLast)
		{
			facingLast = charCommon.facing;
			facingDest = 180 - 90*facingLast;
		}
		
		if (facingCurr != facingDest)
		{
			if (facingCurr >= 360)
			{
				facingCurr -= 360;
			}
			
			facingCurr += facingSpeed;
			transform.localEulerAngles = new Vector3(0.0f, (float)facingCurr, 0.0f);
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		Turn ();
		
		if (charPhysics.isGrounded)
		{
			AnimWalk();
		}
		else
		{
			animation.CrossFade("anim_jump", 0.1f);
		}
	}
}
