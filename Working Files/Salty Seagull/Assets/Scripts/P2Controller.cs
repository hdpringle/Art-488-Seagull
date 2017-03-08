using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P2Controller : PlayerController {

	protected override void GetControls()
	{
		input.moveForward = Input.GetAxis("P2 Forward");
		input.turnUD = FABS (Input.GetAxis ("P2 Vertical"), Input.GetAxis ("P2 Mouse Y"));
		input.turnLR = FABS (Input.GetAxis("P2 Horizontal"), Input.GetAxis("P2 Mouse X"));
		input.drop = Input.GetAxis ("P2 Drop");
	}
}
