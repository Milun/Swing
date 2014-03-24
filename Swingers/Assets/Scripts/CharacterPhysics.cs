/**
 * Base class of all player controlled characters?
 * 
 * Author: Milton Plotkin
 * Date: 17 March 2013
 */

using UnityEngine;
using System.Collections;

// Prints an error if there is no CharacterController for this object.
[RequireComponent( typeof(CharacterController))]

public class CharacterPhysics : MonoBehaviour
{
	public float	m_gravity = 0.4f;			// Gets added every frame to y-speed when falling.
	public float	m_gravityMax = 10.0f;		// A character will not exceed this speed when in free fall.

	public bool		m_isGrounded = false;		// Is true only when the character is on a surface they can run on.
												// controller.isGrounded returns true regardless of surface. This overwrites this.

	private bool	m_isAnchored = false;		// Becomes true for the first frame when controller.isGrounded == true. 
	private float	m_anchorSize = 5.0f;		// The larger the anchor, the steeper the slopes your character will stick to.
	private float	m_anchorFloor = 0.0f;		// Applies downward force to keep the character on the platform it's standing on.

	private float	m_wallSlope = 0.5f;			// If a slopes normal is steeper than this, slide down it.
	public Vector3	m_wallNormal = Vector3.zero;// The vector of the normal of the collided wall/floor.
	public float	m_anchorWall = 0.0f;		// Moves the character towards vertical walls, in a similar fashion to anchor.

	public float 	m_xSpeed, m_ySpeed = 0.0f;
	private float 	friction = 0.92f;

	CharacterController controller;

	// Use this for initialization
	void Awake ()
	{
		controller = GetComponent<CharacterController>();
	}

	/**
	 * Sets the value of m_gravityCurrent, which in turn gets added to the downward velocity of the character.
	 */
	private void Gravity()
	{
		// If the character is not on the ground, increase its gravity.
		if (!isGrounded)
		{
			if (-m_ySpeed < m_gravityMax)
			{
				m_ySpeed -= m_gravity;
			}
			else
			{
				m_ySpeed = -m_gravityMax;
			}
		}
	}

	/**
	 * Adds to the downward velocity of the character, to keep them grounded on downward slopes.
	 * 
	 * MUST be called after Move().
	 */
	private void AnchorFloor()
	{
		if(m_ySpeed > 0.0f)
		{
			m_isAnchored = false;
			m_anchorFloor = 0.0f;
			isGrounded = false;
			return;
		}

		// If grounded, have the character constantly be pushed towards the ground.
		if (isGrounded)
		{
			m_isAnchored = true;

			m_anchorFloor = -m_anchorSize;
		}
		// If not grounded, do not apply downward force.
		else if (!isGrounded && m_isAnchored)
		{
			m_isAnchored = false;

			// Fixes the jitter that occurs in the first frame that the character is not on the ground.
			controller.Move ( (new Vector3(0.0f, -m_anchorFloor*2.0f, 0.0f)) * Time.deltaTime );

			m_anchorFloor = 0.0f;
		}
	}

	/**
	 * Moves the character towards a wall (for walljumping).
	 **/
	private void AnchorWall()
	{
		// If the wall is perfectly vertical, make the character stick to it.
		// Disable this effect if the character tries to move away from the wall.
		// The 0.01 is to give it some leeway with the floats.
		if  (Mathf.Abs(wallNormal.x) >= 1.0f &&
		     	(
		   			(wallNormal.x < 0.0f && m_xSpeed >= -0.01f) ||
		     		(wallNormal.x > 0.0f && m_xSpeed <= 0.01f)
		     	)
		    )
		{
			m_anchorWall = -wallNormal.x;
		}
		else
		{
			m_anchorWall = 0.0f;
		}
	}

	/**
	 * Moves the character to the right (temporary).
	 * 
	 * Also calls Anchor().
	 */
	private void Move()
	{
		Gravity ();

		Vector3 movementDir = new Vector3(m_xSpeed+m_anchorWall, m_ySpeed + m_anchorFloor, 0.0f);
		controller.Move (movementDir * Time.deltaTime);

		AnchorFloor();
		AnchorWall();
	}

	// Update is called once per frame
	void Update ()
	{
		m_xSpeed *= friction;
		if(Mathf.Abs (m_xSpeed) <= 0.05)
		{
			m_xSpeed = 0.0f;
		}

		// Handle the collider for standing on the ground.
		// If the controller.isGrounded is false, then isGrounded must be false too.
		if (!controller.isGrounded)
		{
			isGrounded = false;
		}

		// Forget about the normal if you're not colliding with anything.
		if (controller.collisionFlags == CollisionFlags.None)
		{
			//if (Mathf.Abs(m_wallNormal.x) <= 0.99f)
			m_wallNormal = Vector3.zero;
		}

		Move ();

	}

	/**
	 * Called automatically whenever the character collides with a wall.
	 * 
	 * Records the position of the wall relative to the player.
	 **/
	void OnControllerColliderHit(ControllerColliderHit col)
	{
		Vector3 normal = col.normal;

		// If you're touching or "standing" on a slope that's too steep, slide down it.
		if (Mathf.Abs(normal.x) > m_wallSlope)
		{
			// Record the side the wall is on relative to you.
			m_wallNormal = normal;

			isGrounded = false;

			// Makes you slide sideways with the wall if it's sloped.
			if (normal.x > 0.0f)
			{
				m_xSpeed = -normal.y*ySpeed;
				return;
			}
			else
			{
				m_xSpeed = normal.y*ySpeed;
				return;
			}
		}
		else
		{
			isGrounded = true;
		}
	}

	public float xSpeed
	{
		get
		{
			return m_xSpeed;
		}

		set
		{	
			// Do NOT move into walls while falling.
			if (isGrounded ||
			    controller.collisionFlags == CollisionFlags.None || 
			   	(wallNormal.x >= 0.0f && value > 0.0f) ||
				(wallNormal.x <= 0.0f && value < 0.0f)
			   )
			{

				m_xSpeed = value;
			}
		}
	}

	public float ySpeed
	{
		get
		{
			return m_ySpeed;
		}

		set
		{
			m_ySpeed = value;
		}
	}

	public bool isGrounded
	{
		get
		{
			return m_isGrounded;
		}

		set
		{
			m_isGrounded = value;
		}
	}

	public bool isOnWall
	{
		get
		{
			return (controller.collisionFlags == CollisionFlags.Sides);
		}
	}

	public Vector3 wallNormal
	{
		get
		{
			return m_wallNormal;
		}
	}
}