using UnityEngine;

public class EnemyDestroyer : MonoBehaviour
{
	private void OnTriggerEnter2D(Collider2D collision) {
		if (collision.gameObject.CompareTag("Enemy")) {
			Destroy(collision.gameObject);
		}
	}
}
