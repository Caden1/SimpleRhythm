using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BeatMoveController : MonoBehaviour
{
	public float moveDistance = 1.0f;
	private float beatInterval = 0.75f; // Time between each beat for an 80 BPM song.
	private float nextMoveTime = 0.0f;
	private Vector2 startPosition;
	private Vector2 targetPosition;
	private float moveStartTime;
	private Rigidbody2D rb;
	private float startRotation;
	private float targetRotation;

	private void Start() {
		rb = GetComponent<Rigidbody2D>();
	}

	private void Update() {
		if (Time.time >= nextMoveTime) {
			startPosition = rb.position;
			targetPosition = startPosition + new Vector2(moveDistance, 0);
			startRotation = transform.eulerAngles.z;
			targetRotation = startRotation + -90.0f;
			moveStartTime = Time.time;
			nextMoveTime = Time.time + beatInterval;
		}

		// Calculate the fraction of the movement/rotation completed.
		float t = (Time.time - moveStartTime) / beatInterval;
		Vector2 newPosition = Vector2.Lerp(startPosition, targetPosition, Mathf.SmoothStep(0.0f, 1.0f, t));
		rb.MovePosition(newPosition);
		float newRotation = Mathf.Lerp(startRotation, targetRotation, Mathf.SmoothStep(0.0f, 1.0f, t));
		transform.rotation = Quaternion.Euler(0, 0, newRotation);
	}
}
