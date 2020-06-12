using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class RelativeTextSize : MonoBehaviour {
	[SerializeField, Range(.01f, .2f)] float size = 0.01f;

	#if UNITY_EDITOR
	[SerializeField] bool updateInEditor = false;
	#endif

	void Awake(){
		GetComponent<Text>().fontSize = (int)(Camera.main.pixelHeight * size);
	}

	#if UNITY_EDITOR
	void Update(){
		if(updateInEditor && !Application.isPlaying){
			GetComponent<Text>().fontSize = (int)(Camera.main.pixelHeight * size);
		}
	}
	#endif
}