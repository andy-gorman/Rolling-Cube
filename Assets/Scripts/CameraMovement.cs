using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraMovement : MonoBehaviour {

	public float smooth;
	public float xSpeed;
	public float ySpeed;
	public float zoomSpeed;

	public float velocityX;
	public float velocityY;
	public float velocityZoom;
	private Transform player;
	private Vector3 relCameraPos;
	private float relCameraPosMag;
	private Vector3 newPos;
	private float newMag;
	private Vector3 initialPos;
	private float initialMag;

	public int direction;
	

	void Start()
	{
		smooth = 1.5f;
		xSpeed = 5f;
		ySpeed = 5f;
		zoomSpeed = 5f;
		direction = 1;

		player = GameObject.Find ("Player").transform;
		relCameraPos = transform.position - player.position;
		relCameraPosMag = relCameraPos.magnitude;
		newMag = relCameraPosMag;

		initialMag = relCameraPosMag;
		initialPos = relCameraPos;
			
		transform.LookAt (player);
	}

	void FixedUpdate ()
	{
		if (player) {
			newPos = player.position + relCameraPos;
			transform.position = newPos;
//			transform.position = Vector3.Lerp (transform.position, newPos, smooth*Time.deltaTime);
	

			// left button to rotate 
			if(Input.GetMouseButton (0)){
				// horizontal 
				velocityX = Input.GetAxis ("Mouse X");
				velocityX *= xSpeed;
				transform.RotateAround (player.position,Vector3.up,velocityX);
				// vertical
				velocityY = Input.GetAxis ("Mouse Y");
				velocityY *= ySpeed;
				velocityY *= -1;
				Vector3 vec = Vector3.Cross(transform.position-player.position,Vector3.up);
				transform.RotateAround (player.position,vec,velocityY);

				relCameraPos = transform.position - player.position;

				//position calculationg
				float xjudge = transform.position.x - player.position.x;
				float zjudge = transform.position.z - player.position.z;

				float judge1 = zjudge - xjudge;
				float judge2 = zjudge + xjudge;


				//determine the operating way ,1 for positive is z+, 2 for positive is x+ , 3 for positive is z-, 4 for positive is x-;
				if(judge1<0 && judge2<0){
					direction = 1;
				}
				else if (judge1<0 && judge2>0){
					direction = 4;
				}
				else if (judge1>0 && judge2>0){
					direction = 3;
				}
				else if (judge1>0 && judge2<0){
					direction = 2;
				}
			}

			else if(Input.GetAxis("Mouse ScrollWheel") != 0)
			{
				velocityZoom = Input.GetAxis("Mouse ScrollWheel")*zoomSpeed*-1;
				newMag = relCameraPosMag + velocityZoom;
				relCameraPos = (transform.position - player.position)*(newMag / relCameraPosMag);
				relCameraPosMag = newMag;
			}

			else if(Input.GetMouseButtonDown(1))
			{
				relCameraPosMag = initialMag;
				relCameraPos = initialPos;
			}
		}
	}
}
