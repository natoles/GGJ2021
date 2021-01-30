using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sun : MonoBehaviour {

	[SerializeField]
	Transform rotationCenter;

	[SerializeField]
	float rotationRadius = 2f, angularSpeed = 2f;

	float posX, posY, angle = 0f;

	// Update is called once per frame
	void Update () {
		posX = rotationCenter.position.x + Mathf.Cos (angle) * rotationRadius;
		posY = rotationCenter.position.y + Mathf.Sin (angle) * rotationRadius;
		transform.position = new Vector2 (posX, posY);
		angle = angle + Time.deltaTime * angularSpeed;

		float clr = Mathf.Lerp(0, 255, Mathf.Abs((Mathf.Cos(angle)-1)/2) );
		GetComponentInChildren<Light>().color=new Color(255f,clr,clr,255f);
		
		

		if (angle >= 360f)
			angle = 0f;
	}
	
}
