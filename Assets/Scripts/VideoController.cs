using UnityEngine;
using System.Collections;

public class VideoController : MonoBehaviour {

	public bool isStop = false;

	public MovieTexture myMovie;
	private GameObject menuBox;
	private Camera menuCamera;

	void Start () {
		PlayerPrefs.DeleteAll ();
		myMovie.Play ();
		menuBox = GameObject.Find ("menu");
//		menuCamera = menuBox.
//		menuCamera = GameObject.Find ("MenuCamera").;
	}

	// Update is called once per frame
	void OnGUI () {
		if (isStop == false) {
			GUI.DrawTexture (new Rect (0, 0, Screen.width, Screen.height), myMovie, ScaleMode.StretchToFill);
//			Camera.main.Render();
		} else {
//			Camera.main.Render ();
		}
		if (Input.GetKeyDown (KeyCode.Return)) {
			myMovie.Stop();
			isStop = true;
//			Application.LoadLevel ("Level_1");
		}
	}

}
