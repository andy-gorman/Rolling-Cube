using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour {

	public float smooth;
	public float smoothPos;
	public float xSpeed;
	public float ySpeed;
	public float zoomSpeed;

	private Transform player;

	private float velocityX;
	private float velocityY;
	private float velocityZoom;

	private Vector3 relCameraPos;
	private float relCameraPosMag;
	private float newMag;
	private Vector3 initialPos;
	private float initialMag;

	public int direction;

	private float accumulatedH;
	private float accumulatedV;
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
		accumulatedH = 0.0f;
		accumulatedV = 0.0f;
		vec = Vector3.right;

		player = GameObject.Find ("Player").transform;
		relCameraPos = transform.position - player.position;
		relCameraPosMag = relCameraPos.magnitude;
		newMag = relCameraPosMag;

		initialMag = relCameraPosMag;
		initialPos = transform.position;
			
		transform.LookAt (player);

		tempPos = player.position;

	}

	public void resetCamera()
	{
		relCameraPosMag = initialMag;
		relCameraPos = initialPos;
	}

	void FixedUpdate ()
	{
		
		if (player) {
			tempPos = Vector3.Lerp(tempPos,player.position,smoothPos*Time.deltaTime);
			transform.position = tempPos + relCameraPos;

			transform.RotateAround(player.position,Vector3.up,smooth*accumulatedH);
			accumulatedH*= (1.0f-smooth);
			
			vec = Vector3.Cross(transform.position-tempPos,Vector3.up);
			transform.RotateAround (player.position,vec,smooth*accumulatedV);
			accumulatedV*= (1.0f-smooth);

			relCameraPos = transform.position - tempPos;
			transform.LookAt(player);

			// left button to rotate 
			if(Input.GetMouseButton (0)){
				// horizontal 
				velocityX = Input.GetAxis ("Mouse X");
				velocityX *= xSpeed;
				accumulatedH += velocityX;
				//transform.RotateAround (player.position,Vector3.up,velocityX);

				// vertical
				velocityY = Input.GetAxis ("Mouse Y");
				velocityY *= ySpeed;
				velocityY *= -1;
				accumulatedV += velocityY;
				//transform.RotateAround (player.position,vec,velocityY);

				//relCameraPos = transform.position - player.position;


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
				resetCamera();
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
		}
	}
}
