using UnityEngine;

public class CloneDestroyer : MonoBehaviour
{
	private void OnTriggerEnter2D(Collider2D collision) {
		if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("PlayerProjectile")) {
			Destroy(collision.gameObject);
		}
	}
}
