﻿using UnityEngine;
using System.Collections;

public class KeyboardSeek : SimpleSeek {

	public GameObject pulse;
	public enum Player{Player1, Player2};
	public Player playerNumber;

	public bool useKeyboard = false;

	public GameObject geometry;	
	public float deadZone = .75f;

	private bool firePulse = true;
	private bool chargingPulse = false;
	private float startChargingPulse = 0f;
	private Vector3 velocityChange;
	public float startPulsePower = 2000;

	void Update () {

		velocityChange = !useKeyboard ? PlayerJoystickMovement() : Vector3.zero;
		// Movement
		if ((playerNumber == Player.Player1 && Input.GetKey("w")) || (playerNumber == Player.Player2 && Input.GetKey(KeyCode.UpArrow)))
		{
			velocityChange += Vector3.up;
		}
		if ((playerNumber == Player.Player1 && Input.GetKey("a")) || (playerNumber == Player.Player2 && Input.GetKey(KeyCode.LeftArrow)))
		{
			velocityChange -= Vector3.right;
		}
		if ((playerNumber == Player.Player1 && Input.GetKey("s")) || (playerNumber == Player.Player2 && Input.GetKey(KeyCode.DownArrow)))
		{
			velocityChange -= Vector3.up;
		}
		if ((playerNumber == Player.Player1 && Input.GetKey("d")) || (playerNumber == Player.Player2 && Input.GetKey(KeyCode.RightArrow)))
		{
			velocityChange += Vector3.right;
		}
		transform.LookAt(transform.position + velocityChange, transform.up);
		// Sharing.
		/*if((playerNumber == Player.Player1 && Input.GetKeyDown(KeyCode.LeftControl)) || (playerNumber == Player.Player2 && Input.GetKeyDown(KeyCode.RightControl)))
		{
			partnerLink.connection.SendPulse(partnerLink, partnerLink.partner);
		}
		if ((playerNumber == Player.Player1 && Input.GetKey(KeyCode.Space)) || (playerNumber == Player.Player2 && Input.GetKey("[0]")))
		{
			partnerLink.preparingPulse = true;
		}
		if ((playerNumber == Player.Player1 && Input.GetKeyUp(KeyCode.Space)) || (playerNumber == Player.Player2 && Input.GetKeyUp("[0]")))
		{
			partnerLink.preparingPulse = false;
		}*/

		PlayerLookAt(useKeyboard);

		//Draw the line
		if (velocityChange.sqrMagnitude > 0)
		{
			mover.Accelerate(velocityChange);
			if (tracer.lineRenderer == null)
				tracer.StartLine();
			else
				tracer.AddVertex(transform.position);
		}
		else
		{
			mover.SlowDown();
			tracer.DestroyLine();
		}

		geometry.transform.LookAt(transform.position + mover.velocity, geometry.transform.up);
	}

	private Vector3 PlayerJoystickMovement()
	{
		Vector2 leftStickInput = new Vector2(GetAxisMoveHorizontal(), GetAxisMoveVertical());
		return leftStickInput.sqrMagnitude > Mathf.Pow(deadZone, 2f) ? new Vector3(GetAxisMoveHorizontal(),GetAxisMoveVertical(),0) : Vector3.zero;
	}

	void PlayerLookAt(bool assumeForward)
	{
		Vector2 lookAt = geometry.transform.forward;
		if (!assumeForward)
		{
			lookAt = new Vector2(GetAxisAimHorizontal(), GetAxisAimVertical());
		}
		else if (velocityChange.sqrMagnitude == 0 || !chargingPulse || GetButtonFirePulse())
		{
			lookAt = new Vector2();
		}

		if(GetButtonFirePulse() && !chargingPulse)
		{
			chargingPulse = true;
			startChargingPulse = Time.time;
		}

		if(lookAt.sqrMagnitude > Mathf.Pow(deadZone, 2f))
		{
			if(firePulse)
			{
				Vector3 target = transform.position + new Vector3(lookAt.x, lookAt.y, 0);
				transform.LookAt(target, transform.up);
				Vector3 joystickPos = new Vector3(lookAt.x, lookAt.y, 0);

				if(!chargingPulse && !useKeyboard)
				{
					joystickPos *= startPulsePower;
					FirePulse(transform.position + mover.velocity + joystickPos);
					firePulse = false;
				}
				else if(!GetButtonFirePulse() && startChargingPulse > 0)
				{
					var chargeTime = Time.time - startChargingPulse;
					joystickPos *= (startPulsePower * (1 + chargeTime));
					FirePulse(transform.position + mover.velocity + joystickPos);
					firePulse = false;
					chargingPulse = false;
					startChargingPulse = 0f;
				}
			}
		}
		else
		{
			firePulse = true;
			if(!GetButtonFirePulse() && chargingPulse)
			{
				chargingPulse = false;
				startChargingPulse = 0f;
			}
		}
	}

	void FirePulse(Vector3 pulseTarget)
	{
		GameObject go = Instantiate(pulse,transform.position, Quaternion.identity) as GameObject;
		go.GetComponent<MovePulse>().target = pulseTarget;
	}


	
	
	
	#region Helper Methods
	
	private float GetAxisMoveHorizontal(){return Input.GetAxis("MoveHorizontal" + playerNumber.ToString());}
	private float GetAxisMoveVertical(){return Input.GetAxis("MoveVertical" + playerNumber.ToString());}
	private float GetAxisAimHorizontal(){return Input.GetAxis("AimHorizontal" + playerNumber.ToString());}
	private float GetAxisAimVertical(){return Input.GetAxis("AimVertical" + playerNumber.ToString());}
	private float GetAxisGive(){return Input.GetAxis("Give" + playerNumber.ToString());}
	private float GetAxisTake(){return Input.GetAxis("Take" + playerNumber.ToString());}

	private bool GetButtonFirePulse()
	{
		if (!useKeyboard)
		{
			return Input.GetButton("FirePulse" + playerNumber.ToString());
		}
		else
		{
			return (playerNumber == Player.Player1 && Input.GetKey(KeyCode.Space)) || (playerNumber == Player.Player2 && Input.GetKey("[0]"));
		}
	}

	#endregion



}
