using UnityEngine;
using System.Collections;

public class VideoController : MonoBehaviour {

	public MovieTexture myMovie;

	void Start () {
		PlayerPrefs.DeleteAll ();
		myMovie.Play ();
	}

	// Update is called once per frame
	void OnGUI () {
		GUI.DrawTexture(new Rect(0,0,Screen.width,Screen.height),myMovie ,ScaleMode.StretchToFill);
		if (Input.GetKeyDown (KeyCode.Return)) {
			myMovie.Stop();
			Application.LoadLevel ("Level_1");
		}
	}

}
