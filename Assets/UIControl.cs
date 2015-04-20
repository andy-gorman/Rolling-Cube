using UnityEngine;
using System.Collections;
using System;

public class UIControl : MonoBehaviour {

	private GameObject worldGallery;
	private GameObject menuBox;
	private RectTransform worldGalleryRect;
	private float startTime;
	private Quaternion startRot;
	private Quaternion endRot;
	private Vector3 startPos;
	private Vector3 endPos;
	private bool onMove = false;
	private bool onCameraMove = false;
	public int currentWorld = 1;
	private GameObject[] worlds;
	private GameObject mainMenu;

	private int worldCount = 4;

	void Start(){
		worldGallery = GameObject.Find ("WorldGallery");
		worlds = GameObject.FindGameObjectsWithTag ("WorldPanel");
		mainMenu = GameObject.Find ("MainMenu");
//		worlds = worlds);
//		for (int i = 0; i<worldCount; i++) {
//			string name = "World"+(i+1);
//			Debug.Log(name);
//			worlds[i] = GameObject.Find(name);
//		}
		menuBox = GameObject.Find ("MenuBox");
		menuBox.SetActive(true);
		worldGalleryRect = worldGallery.GetComponent<RectTransform> ();

		foreach(GameObject w in worlds){
			w.SetActive(false);
			Debug.Log ("World_");
		}
		worldGallery.SetActive(false);
//		worlds[0].SetActive(true);
	}


	public void GoSelectWorld(){
		mainMenu.SetActive (false);
		worldGallery.SetActive(true);
		startRot = menuBox.transform.rotation;
		endRot = menuBox.transform.rotation * Quaternion.Euler (90,0,0);
		startTime = Time.time;
		onMove = true;
	}

	public void PrevWorld(){
		if (currentWorld <= 1) {
			return;
		}
//		Debug.Log ("PrevWorld");
//		startPos = worldGalleryRect.position;
//		endPos = startPos + new Vector3 (800, 0, 0);
		startRot = menuBox.transform.rotation;
		endRot = menuBox.transform.rotation * Quaternion.Euler (0,0,90);
		startTime = Time.time;
		onMove = true;
		worlds[currentWorld-1].SetActive(false);
		currentWorld --;

//		worldGallery.
	}

	public void NextWorld(){
		if (currentWorld >= worldCount) {
			return;
		}
//		Debug.Log ("NextWorld");
//		startPos = worldGalleryRect.position;
//		endPos = startPos - new Vector3 (800, 0, 0);
		startRot = menuBox.transform.rotation;
		endRot = menuBox.transform.rotation * Quaternion.Euler (0,0,-90);
		startTime = Time.time;
		onMove = true;
		worlds[currentWorld-1].SetActive(false);
		currentWorld ++;
//		menuBox.transform.Rotate (Vector3.up, Time.deltaTime*100f);
	}

	void OnGUI(){
		if (Input.GetKeyDown (KeyCode.RightArrow)) {
			NextWorld ();
		} else if (Input.GetKeyDown (KeyCode.LeftArrow)) {
			PrevWorld ();
		}
	}

	void Update(){


//		menuBox.transform.Rotate (Vector3.right, Time.deltaTime*100f);
//		Debug.Log (Time.time);
		if (onMove) {
//
//			Vector3 nowPos = worldGalleryRect.position;
//			float moveDist = Vector3.Distance(new Vector3 (500, 0, 0));
//			float nowDist = Vector3.Distance(nowPos - startPos);
			float deltaTime = Time.time - startTime;
//			Debug.Log(startRot);
//			Debug.Log(endRot);
			float frac = deltaTime/1.0f;
			if(frac <=1.2){
				menuBox.transform.rotation = Quaternion.Slerp(startRot, endRot, frac );
//				worldGalleryRect.position = Vector3.Lerp (startPos, endPos, frac);
			}else{
				worlds[currentWorld-1].SetActive(true);
				onMove = false;
			}
		}
		if (onCameraMove) {
			float deltaTime = Time.time - startTime;
			float frac = deltaTime/1.0f;
			if(frac <=1.2){
				Camera.main.transform.position = Vector3.Lerp(startPos, endPos, frac );
				//				worldGalleryRect.position = Vector3.Lerp (startPos, endPos, frac);
			}
		}
	}

	public void GoToLevel(int level){
		Application.LoadLevel ("Level_"+level);
	}
	

	public void ControlMenu(int itemIndex){
		switch (itemIndex) {
		case 1: //New Game
			Application.LoadLevel ("Level_1");
			break;
		case 2: //Continue
			//TODO Continue
			break;
		case 3: //Select Level
			GoSelectWorld();
			break;
		case 4: //Options
			GoOptions();
			break;
		case 5: //Help
			//TODO Help
			break;
		}
	}


	public void GoOptions(){
		mainMenu.SetActive (false);
		onCameraMove = true;
		startPos = Camera.main.transform.position;
		endPos = startPos + new Vector3 (-300, 200, 500);
		startTime = Time.time;
	}

//	void MoveObject (Vector3 startPos, Vector3 endPos, float time) {
//		var i = 0.0;
//		var rate = 1.0/time;
//		while (i < 1.0) {
//			i += Time.deltaTime * rate;
//			worldGalleryRect.position = Vector3.Lerp(startPos, endPos, i);
	//			yield;
//		}
//	}

}
