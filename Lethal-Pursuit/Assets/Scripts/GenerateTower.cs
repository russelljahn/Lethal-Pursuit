using UnityEngine;
using System.Collections;

public class GenerateTower : MonoBehaviour {

	public int height = 20;
	public int width = 20;
	public int depth = 20;
	public float spaceBetweenGameObjects = Mathf.Epsilon;
	public GameObject gameObjectToClone;
	public Transform cloneTransform;

	// Use this for initialization
	void Start () {
		Vector3 size = gameObjectToClone.renderer.bounds.size;


		for (int i = 0; i < width; ++i) {
			Vector3 clonePosition = new Vector3(
				cloneTransform.position.x,
				cloneTransform.position.y,
				cloneTransform.position.z
			);
			
			clonePosition.x = cloneTransform.position.x + i*size.x + spaceBetweenGameObjects;

			for (int j = 0; j < depth; ++j) {

				clonePosition.z = cloneTransform.position.z + j*size.z + spaceBetweenGameObjects;

				for (int k = 0; k < height; ++k) {
					clonePosition.y = cloneTransform.position.y + k*size.y + spaceBetweenGameObjects;
					
//					Debug.Log("About to network generate a tower");
//					Debug.Log("Clone position " + clonePosition);
					
					GameObject clone = null;
					if(NetworkManager.IsSinglePlayer()) {
						clone = GameObject.Instantiate(gameObjectToClone, 
						                               clonePosition, 
						                               Quaternion.identity) as GameObject;
						clone.transform.parent = this.transform;
					}
					else if(Network.isServer){
//						Debug.Log("Going to network generate a tower");
						clone = Network.Instantiate(Resources.Load ("Cube"),
						                            clonePosition, 
						                            Quaternion.identity,
						                            1) as GameObject;
						clone.transform.parent = this.transform;
					}
				}
			}

		}
//		gameObjectToClone.SetActive(false);
	}





	// Update is called once per frame
	void Update () {


	}





}
