using UnityEngine;
using System.Collections;

public class GenerateTower : MonoBehaviour {

	public int height = 20;
	public int width = 20;
	public int depth = 20;
	public float spaceBetweenGameObjects = Mathf.Epsilon;
	public GameObject gameObjectToClone;


	// Use this for initialization
	void Start () {
		Vector3 size = gameObjectToClone.renderer.bounds.size;


		for (int i = 0; i < width; ++i) {
			Vector3 clonePosition = new Vector3(
				gameObjectToClone.transform.position.x,
				gameObjectToClone.transform.position.y,
				gameObjectToClone.transform.position.z
			);
			
			clonePosition.x = gameObjectToClone.transform.position.x + i*size.x + spaceBetweenGameObjects;

			for (int j = 0; j < depth; ++j) {

				clonePosition.z = gameObjectToClone.transform.position.z + j*size.z + spaceBetweenGameObjects;

				for (int k = 0; k < height; ++k) {
					clonePosition.y = gameObjectToClone.transform.position.y + k*size.y + spaceBetweenGameObjects;
					GameObject clone = GameObject.Instantiate(gameObjectToClone, clonePosition, Quaternion.identity) as GameObject;
					clone.transform.parent = this.transform;
				}
			}

		}
	}





	// Update is called once per frame
	void Update () {


	}





}
