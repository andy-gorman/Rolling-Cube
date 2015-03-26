using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class IntroController : MonoBehaviour {

	
	public float[] TimeQueues;
	public string[] Messages;
	public float deltaFade;
	public Text text;
	public Color textColor;

	public AudioSource track;
	private int CurMsgIndex;
	private bool changing;
	// Use this for initialization
	void Start () {
		CurMsgIndex = 0;
		changing = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (!changing) {
			if (CurMsgIndex < TimeQueues.Length &&  track.time >= TimeQueues [CurMsgIndex]) {
				StartCoroutine (SwitchText());
			}
		}
		if (Input.GetKeyDown (KeyCode.Return)) {
			Application.LoadLevel ("Level_1");
		}
	}

	IEnumerator SwitchText() {
		changing = true;
		float time = 0; 
		while (text.color.a != 0) {
			text.color = Color.Lerp (textColor, Color.clear, time);
			time += Time.deltaTime * deltaFade;
			yield return 0;
		}

		text.text = Messages [CurMsgIndex];
		time = 0; 
		while(text.color != textColor) {
			text.color = Color.Lerp (Color.clear, textColor, time);
			time += Time.deltaTime * deltaFade;
			yield return 0;
		}
		CurMsgIndex++;
		changing = false;

	}
}
