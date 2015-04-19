using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TextBox : MonoBehaviour {
	public string[] messages;

	private bool done;
	public bool Done {
		get { return done; }
	}
	private AudioSource audio;
	private Text output;
	private int character, msgIndex;
	// Use this for initialization
	void Start () {
		audio = GetComponent<AudioSource> ();
		output = GetComponent<Text> ();
		character = 1; msgIndex = 0;
		done = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (!done) {
			if (character > messages [msgIndex].Length) {
				if (Input.anyKey) {
					msgIndex++;
					character = 1;
				}
			} else {
				audio.Play();
				output.text = messages [msgIndex].Substring (0, character);
				character++;

			}
			if (msgIndex >= messages.Length) {
				done = true;
			}
		}
	}
}
