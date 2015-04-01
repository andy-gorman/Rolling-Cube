using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour {

	public float smooth;
	public float smoothPos;
	public float xSpeed;
	public float ySpeed;
	public float zoomSpeed;

	public bool fix = false;
	public GameObject fixObject;

	private Transform player;

	private float velocityX;
	private float velocityY;
	private float velocityZoom;

	private Vector3 relCameraPos;
	private float relCameraPosMag;
	private float newMag;
	private Vector3 initialPos;
	private float initialMag;
	private bool backFrombound;

	public int direction;

	private Vector3 vec;

	private Vector3 tempPos;


	void Start()
	{
		smooth = 0.01f;
		smoothPos = 1.5f;
		xSpeed = 5f;
		ySpeed = 5f;
		zoomSpeed = 5f;
		direction = 1;
		velocityX = 0.0f;
		velocityY = 0.0f;
		vec = Vector3.right;
		backFrombound = false;

	//	player = GameObject.Find ("Player").transform;
	//	relCameraPos = transform.position - player.position;
	//	relCameraPosMag = relCameraPos.magnitude;
		newMag = 0;

		initialMag = relCameraPosMag;
		initialPos = transform.position;
			
		//transform.LookAt (player);

//		tempPos = player.position;

	}

	public void resetCamera()
	{
		relCameraPosMag = initialMag;
		relCameraPos = initialPos;
	}

	void FixedUpdate ()
	{
		
		if (player) {
			if (fix != true ){
				tempPos = Vector3.Lerp(tempPos,player.position,smoothPos*Time.deltaTime);
			}
			transform.position = tempPos + relCameraPos;

			transform.RotateAround(tempPos,Vector3.up,velocityX);

			Vector3 angle1 = transform.position-tempPos;
			Vector3 angle2 = angle1 ;
			angle2.y -= angle2.y;
			
			if (Vector3.Angle(angle2,angle1)>60){
				backFrombound =true;
				if(transform.position.y > tempPos.y){
					velocityY = -0.25f;
				}
				else{
					velocityY = +0.25f;
				}
			}
			else if(backFrombound == true){
				backFrombound = false;
				velocityY = 0.0f;
			}
			vec = Vector3.Cross(transform.position-tempPos,Vector3.up);
			transform.RotateAround (tempPos,vec,velocityY);

			relCameraPos = transform.position - tempPos;
			relCameraPosMag = relCameraPos.magnitude;

			if(fixObject != null && fix == true){
				transform.LookAt(fixObject.transform);
			}
			else{
				transform.LookAt(player);
			}
			// left button to rotate 
			if(Input.GetMouseButton (0)){
				// horizontal 
				velocityX = Input.GetAxis ("Mouse X");
				velocityX *= xSpeed;

				// vertical
				velocityY = Input.GetAxis ("Mouse Y");
				velocityY *= ySpeed;
				velocityY *= -1;
		//		vec = Vector3.Cross(transform.position-tempPos,Vector3.up);
				


			}

			else if(Input.GetAxis("Mouse ScrollWheel") != 0)
			{
				if(relCameraPosMag >= 5 && relCameraPosMag <= 50){
					velocityZoom = Input.GetAxis("Mouse ScrollWheel")*zoomSpeed*-1;
					newMag = relCameraPosMag + velocityZoom;
					relCameraPos = (transform.position - player.position)*(newMag / relCameraPosMag);
					relCameraPosMag = newMag;
				}
				else if (relCameraPosMag < 5){
					newMag = 5;
					relCameraPos = (transform.position - player.position)*(newMag / relCameraPosMag);
					relCameraPosMag = 5;
				}
				else {
					newMag = 50;
					relCameraPos = (transform.position - player.position)*(newMag / relCameraPosMag);
					relCameraPosMag = 50;
				}

			}

			else if(Input.GetMouseButtonDown(1))
			{
				resetCamera();
			}
			else if (Input.GetMouseButtonUp (0)){
				velocityX = 0.0f;
				velocityY = 0.0f;

			}

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
		else {
			player = GameObject.Find ("Player").transform;
				relCameraPos = transform.position - player.position;
				relCameraPosMag = relCameraPos.magnitude;
			newMag = 0;
			
			initialMag = relCameraPosMag;
			initialPos = transform.position;
			
			//transform.LookAt (player);
			
					tempPos = player.position;
		}
	}
}
