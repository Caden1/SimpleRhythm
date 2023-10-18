using UnityEngine;

public class GateScript : MonoBehaviour
{
	private void OnTriggerEnter2D(Collider2D collision) {
		if (collision.gameObject.CompareTag("Player")) {
			Debug.Log("Level Complete");
		}
	}
}
