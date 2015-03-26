using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {	
	
	public float speed = 5f;
	public float navigateSpeed = 20f;
	public bool isLookingAtMap = true;
	public GameObject centerObj;
	private float originX = 0.0f;
	private float originY = 0.0f;
	private static Vector3 originPos;
	private static Quaternion originRot;
	private GameObject player;
	private Vector3 playerPos;
	private Vector3 deltaPos;

	private bool isNavigatingMap = false;
	private bool isReturning = false;
	private float totalHorizontalAngle = 0.0f;
	private float totalVerticalAngle = 0.0f;
	private float totalNavigateAngle = 0.0f;
	
	void Start () {
		originPos = transform.position;
		originRot = transform.rotation;
		if (isLookingAtMap==false) {
			centerObj = GameObject.Find ("Player");

			playerPos = centerObj.transform.position;
			deltaPos = playerPos - originPos;
		}


		transform.LookAt (centerObj.transform);



	}

	void Update () {
		if (centerObj != null) {
			if (isLookingAtMap == false) {
				playerPos = centerObj.transform.position;
				transform.position = playerPos - deltaPos;
			}

			//		if (Input.GetMouseButtonDown (1)) {
			//			transform.RotateAround (centerObj.transform.position, Vector3.up, 0.1f * speed);
			//		}


			// navigate the map
			if (Input.GetMouseButtonDown (2)) {
				isNavigatingMap = true;

				//			transform.position = originPos;
				//			transform.rotation = originRot;
			}


			if (isNavigatingMap) {
				navigateMap ();
			}

			// begin dragging
			if (Input.GetMouseButtonDown (1)) {
				originX = Input.mousePosition.x;
				originY = Input.mousePosition.y;
				totalHorizontalAngle = 0f;
				//			totalVerticalAngle = 0f;
			}
			if (Input.GetMouseButtonUp (1)) {
				totalHorizontalAngle = totalHorizontalAngle % 360f;
				if (totalHorizontalAngle > 180f) {
					totalHorizontalAngle -= 360f;
				} else if (totalHorizontalAngle < -180f) {
					totalHorizontalAngle += 360f;
				}

				isReturning = true;

				//			Debug.Log ("dd"+ totalVerticalAngle);
			}

			if (Input.GetMouseButton (1)) {
				float currentX = Input.mousePosition.x;
				float currentY = Input.mousePosition.y;
				float deltaX = currentX - originX;
				float deltaY = currentY - originY;

				// rotate horizontally
				float horizontalAngle = deltaX * 0.1f * speed;
				totalHorizontalAngle += horizontalAngle;
				transform.RotateAround (centerObj.transform.position, Vector3.up, horizontalAngle);
				//			Debug.Log (transform.rotation);

				// rotate vetically
				//			Vector3 targetVec = transform.position - centerObj.transform.position;
				//			Vector3 rotVec = Vector3.Cross( targetVec, Vector3.up );
				//			float verticalAngle = deltaY*0.1f*speed;
				//			totalVerticalAngle += verticalAngle;
				//			if(Mathf.Abs(transform.rotation.x)<0.6f){
				//				transform.RotateAround(centerObj.transform.position, rotVec, verticalAngle);
				//			}
				//			Debug.Log (totalHorizontalAngle);
				//			Debug.Log (Mathf.Abs(360-transform.rotation.x));
				originX = currentX;
				originY = currentY;
			} else {
				//			transform.RotateAround(centerObj.transform.position, Vector3.up, 0.1f*speed);
				if (isNavigatingMap == false && isReturning == true) {
					int hDir = 1;
					//				int vDir = 1;
					if (totalHorizontalAngle < 0f) {
						hDir = -1;
					}
					//				if(totalVerticalAngle < 0f){
					//					vDir = -1;
					//				}
					//				totalHorizontalAngle = Mathf.Abs(totalHorizontalAngle) % 360.0f;
					//				Debug.Log (totalHorizontalAngle);



					float horizontalAngle = 5.0f;
					//				float verticalAngle = 5.0f;
					if (Mathf.Abs (totalHorizontalAngle) - hDir * horizontalAngle > 0) {
						transform.RotateAround (centerObj.transform.position, Vector3.up, - hDir * horizontalAngle);
						totalHorizontalAngle = totalHorizontalAngle - hDir * horizontalAngle;

					} else {
						transform.RotateAround (centerObj.transform.position, Vector3.up, - hDir * (horizontalAngle - totalHorizontalAngle));
						isReturning = false;
						//					transform.RotateAround(centerObj.transform.position, Vector3.up, -(horizontalAngle+totalHorizontalAngle));
					}
					//				if(Mathf.Abs (totalVerticalAngle)- vDir * verticalAngle > 0){
					//					Vector3 targetVec = transform.position - centerObj.transform.position;
					//					Vector3 rotVec = Vector3.Cross( targetVec, Vector3.up );
					//					transform.RotateAround(centerObj.transform.position, rotVec, - vDir * verticalAngle);
					//					totalVerticalAngle = totalVerticalAngle - vDir * verticalAngle;
					//				}else{
					//					transform.RotateAround(centerObj.transform.position, Vector3.up, - hDir * (horizontalAngle-totalHorizontalAngle));
					//					isReturning = false;
					//				}
					//				transform.position = Vector3.Lerp(transform.position, originPos, 5f * Time.deltaTime);
					//				transform.rotation = Quaternion.Lerp(transform.rotation, originRot, 5f * Time.deltaTime);
				}

			}
		} else {
			centerObj = GameObject.Find ("Player");
		}
	}

	void navigateMap(){
		float angle = 0.1f * navigateSpeed;
//		Debug.Log (totalAngle);
		totalNavigateAngle += angle;
		if (totalNavigateAngle <= 360) {
			transform.RotateAround (centerObj.transform.position, Vector3.up, angle);
		} else {
			transform.RotateAround (centerObj.transform.position, Vector3.up, angle+360-totalNavigateAngle);
			isNavigatingMap = false;
			totalNavigateAngle = 0.0f;
		}
	}
}
