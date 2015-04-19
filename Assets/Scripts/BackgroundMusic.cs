using UnityEngine;

public class BackgroundMusic : MonoBehaviour {
	public AudioSource music;
	private static BackgroundMusic instance = null;
	public static BackgroundMusic Instance {
		get { return instance; }
	}
	void Awake() {
		if (instance != null && instance != this) {
			Destroy(this.gameObject);
			return;
		} else {
			instance = this;
		}
		DontDestroyOnLoad(this.gameObject);
		music.Play ();
	}
}