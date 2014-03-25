using UnityEngine;
using System.Collections;

[RequireComponent( typeof(CharacterPhysics))]
public class CharacterCommon : MonoBehaviour
{
	public GameObject grapplePointPrefab;
	public GameObject grappleRopePrefab;
	private GameObject currentGrapplePoint = null;
	private GameObject currentGrappleRope = null;

	public int		m_jumpCount = 0;			// How many times you've jumped already.
	public int		m_jumpMax = 2;				// Maximum amount of times you can jump.
	private float	m_jumpHeight = 10.0f;		// How high you jump each time.

	public float	runAcc = 4.0f;				// How fast you gain speed while starting a run.
	public float 	runSpeed = 9.0f;			// Maximum speed the character reaches while running.

	private int 	m_facing = 1;				// Can equal 1 or -1. 1 = Facing right.
	private Vector3 m_aim = Vector3.zero;		// Where the character is aiming their grappling hook
	private Vector3 m_grappleDirection;
	private GameObject m_armPos;				// The position of the arms bone from where the rope stretches.

	private float	m_wallTimer = 0.0f;
	private bool	m_onWall = false;
	private const float	m_wallLimit = 2.0f;
	
	CharacterPhysics charPhysics;

	// Use this for initialization
	void Awake ()
	{
		charPhysics = GetComponent<CharacterPhysics>();
	}

	void Start ()
	{
		// Get a reference to the characters arm (this will need to be changed based on character).
		m_armPos = transform.Find ("Armature/b_root/b_chest/b_chest_L/b_chest_upper_L/b_chest_lower_L/b_chest_hand_L/b_chest_hand_L_001").gameObject;
	}
	
	// Update is called once per frame
	void Update ()
	{
		// Allow the character to jump again after landing.
		// The ySpeed check prevents this from triggering when the player jumps again.
		if (charPhysics.isGrounded)
		{
			// Only do this once.
			if (m_jumpCount != 0)
			{
				m_jumpCount = 0;
			}
			else
			{
				// Be SURE to reset yspeed or they'll fastfall after going off again!
				charPhysics.ySpeed = 0.0f;
			}
		}

		if (!charPhysics.isGrounded && Mathf.Abs(charPhysics.wallNormal.x) >= 1.0f)
		{
			if (!m_onWall)
			{
				m_onWall = true;
				m_wallTimer = 1.0f;
			}

			m_wallTimer += Time.deltaTime;

			if (charPhysics.wallNormal.x > 0.0f)
			{
				m_facing = 1;
			}
			else
			{
				m_facing = -1;
			}
			// Negative is right, positive is left.
			
			/*	MILTON
			 * 	SLIDING ANIMATION GOES HERE 
			 */
			charPhysics.isGrounded = false;

			if (charPhysics.ySpeed < 0)
			{
				if (m_wallTimer / m_wallLimit <= 1)
				{
					charPhysics.ySpeed *= (m_wallTimer / m_wallLimit);
				}
				if (charPhysics.ySpeed > 0)
				{
					charPhysics.ySpeed = 0;
				}
			}
		}
		else
		{
			m_onWall = false;
		}

		if(currentGrapplePoint != null)
		{
			UpdateGrappleRope();

			if(currentGrapplePoint.GetComponent<GrappleLogic>().isSet)
			{
				float distance = currentGrapplePoint.GetComponent<GrappleLogic>().ropeLength;
				transform.position = (transform.position - currentGrapplePoint.transform.position).normalized * distance + currentGrapplePoint.transform.position;
			
				charPhysics.gravityMax = charPhysics.defaultGravityMax/2.0f;
			}
		}
	}
	
	// Makes the character jump and also handles multiple jumps.
	public void Jump()
	{
		// If the slope is PERFECTLY VERTICAL, you can wall jump off of it.
		if (!charPhysics.isGrounded && Mathf.Abs(charPhysics.wallNormal.x) >= 1.0f)
		{
			charPhysics.ySpeed = m_jumpHeight;
			charPhysics.xSpeed = charPhysics.wallNormal.x*runSpeed*2.0f;

			if (charPhysics.wallNormal.x > 0.0f)
			{
				m_facing = 1;
			}
			else
			{
				m_facing = -1;
			}
			// Negative is right, positive is left.

			charPhysics.isGrounded = false;
		}
		else if(m_jumpCount < m_jumpMax)
		{
			charPhysics.ySpeed = m_jumpHeight;

			// If you're in the air when you jump for the firt time, you lose one of your jumps.
			if (m_jumpCount == 0 && !charPhysics.isGrounded)
				m_jumpCount++;

			m_jumpCount++;
		}
	}

	/**
	 * Makes the character accelerate in speed along the ground while walking/running.
	 * Note: If the player is already moving at a speed faster than their run speed, this will NOT
	 * restrict that, and instead the characters natural friction will. This is intentional. It allows moonwalking.
	 * 
	 * @param speed From negative to positive values. Negative accelerates the character left. Range[-1, 1].
	 */
	public void Run(float speed)
	{
		// Get the speed we're trying to reach.
		float targetSpeed = speed*runSpeed;
		float direction = 1.0f;
		if (speed < 0.0f)
		{
			direction = -1.0f;
		}

		// Accelerate towards the target speed. Do not accelerate if you're going faster than it.
		if (
				(direction > 0.0f && charPhysics.xSpeed < targetSpeed) ||
				(direction < 0.0f && charPhysics.xSpeed > targetSpeed)
		   )
		{
			charPhysics.xSpeed = charPhysics.xSpeed + direction*runAcc;
		}

		// Change direction variable.
		if (charPhysics.isGrounded && Mathf.Abs(speed) > 0.0f)
		{
			m_facing = (int)direction;
		}
	}

	public int facing
	{
		get
		{
			return m_facing;
		}
	}

	public void Aim(Vector3 newAim)
	{
		m_aim = newAim;
	}

	/**
	 * Shoots out the grapple hook.
	 */
	public void FireGrapple()
	{
		if(currentGrapplePoint == null)
		{
			// Create a new grapple.
			currentGrapplePoint = Instantiate (grapplePointPrefab, transform.position,transform.rotation) as GameObject;
			currentGrappleRope  =  Instantiate (grappleRopePrefab) as GameObject;

			// Remove the old grapple.
			Destroy(currentGrapplePoint,100);
			Destroy(currentGrappleRope,100);

			GrappleLogic grappleLogic = currentGrapplePoint.GetComponent<GrappleLogic>();

			if(m_aim == Vector3.zero)
			{
				grappleLogic.SetDirection(new Vector3(1,1,0));
			}
			else
			{
				grappleLogic.SetDirection(m_aim);
			}

			grappleLogic.player = gameObject;
		}
	}

	/**
	 * Updates the ropes position if a grapple exists.
	 **/
	private void UpdateGrappleRope()
	{
		// Update the location of the rope.
		if (currentGrappleRope != null)
		{
			LineRenderer lr = currentGrappleRope.GetComponent<LineRenderer>();

			// Set the position on the players end.
			if (m_armPos != null)
			{
				// NOTE! Thisll need to  be reworked. It seems inconsistent where it comes out of.
				Vector3 temp = new Vector3(m_armPos.transform.position.x, m_armPos.transform.position.y + charPhysics.m_gravity, 0.0f);
				lr.SetPosition(0, temp);
			}

			// Set the position on the grapples end (if it exists).
			if (currentGrapplePoint != null)
			{
				lr.SetPosition(1, currentGrapplePoint.transform.position);
			}
		}
	}

	public void ReleaseGrapple()
	{
		if(currentGrapplePoint != null)
		{
			Destroy(currentGrapplePoint.gameObject);
			currentGrapplePoint = null;

			m_jumpCount = 0; //So we can jump again and do next grapple
			charPhysics.gravityMax = charPhysics.defaultGravityMax;
		}
	}
}
