﻿using UnityEngine;
using System.Collections;

public class GrappleLogic : MonoBehaviour
{
	private Vector3 m_direction;
	private float m_speed;
	private float m_ropeLength;
	private GameObject m_player;

	private float m_life = 200.0f;		//How long before we delete this
	private float m_timeAlive = 0.0f;	//How long this grapple object has existed

	private bool m_grapplePointSet = false;

	// Use this for initialization
	void Awake()
	{
		m_speed = 40.0f;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(!m_grapplePointSet)
		{
			transform.position += m_direction * m_speed * Time.deltaTime;

			m_timeAlive += Time.deltaTime;
			if(m_timeAlive >= m_life)
			{
				//Release after a certain time
				m_player.GetComponent<CharacterCommon>().ReleaseGrapple();
			}
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.tag == "Character")
		{
			//then we got a grapple on
		}
		else//this WILL check the object can be grappled on before doing this
		{
			m_grapplePointSet = true;
			m_ropeLength = Vector3.Distance(m_player.transform.position,transform.position);
		}
	}

	public void SetDirection(Vector3 newDirection)
	{
		m_direction = newDirection;
	}

	public bool isSet
	{
		get
		{
			return m_grapplePointSet;
		}
	}

	public GameObject player
	{
		get
		{
			return m_player;
		}

		set
		{
			m_player = value;
		}
	}

	public float ropeLength
	{
		get
		{
			return m_ropeLength;
		}

		set
		{
			m_ropeLength = value;
		}
	}
}
