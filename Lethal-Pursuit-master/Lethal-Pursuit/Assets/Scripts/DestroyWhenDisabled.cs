using UnityEngine;
using System.Collections;

public class DestroyWhenDisabled : MonoBehaviour {

	void OnDisable() {
		GameObject.Destroy(this.gameObject);
	}
}
