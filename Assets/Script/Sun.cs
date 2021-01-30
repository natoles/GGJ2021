using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sun : MonoBehaviour {

	[SerializeField]
	Transform rotationCenter;

	[SerializeField]
	float angularSpeed = 2f;
	

	float posX, posY, angle = 0f;

	// Update is called once per frame
	void Update () {
		float rotationRadius = Mathf.Lerp(4000, 7000, Mathf.Abs((Mathf.Cos(angle))/2) );
		posX = rotationCenter.position.x + Mathf.Cos (angle) * rotationRadius;
		posY = rotationCenter.position.y + Mathf.Sin (angle) * rotationRadius;
		transform.position = new Vector2 (posX, posY);
		angle = angle + Time.deltaTime * angularSpeed;

		float clr = Mathf.Lerp(255, 180, Mathf.Abs((Mathf.Cos(angle)-1)/2) );
		GetComponentInChildren<Light>().color=new Color(250f,clr,clr,250f);
		
		

		if (angle >= 360f)
			angle = 0f;
	}
	
}
