using UnityEngine;
using System.Collections;

public class PlayerProjectile : MonoBehaviour
{
	private float bpm = 40f;
	private float nextMoveTime = 0f;
	private float moveDistance = 2f;
	private float moveXOffset = 0.8f;
	private float moveTimer = 0f;
	private float beatInterval;
	private float moveStartTime;
	private float moveDuration;
	private int moveDirection;

	private Vector2 snapToPosition;
	private Vector2 currentVelocity = Vector2.zero;
	private Animator animator;
	private Rigidbody2D rb;

	private void Start() {
		beatInterval = 60f / bpm;
		moveDuration = beatInterval * 0.5f;
		animator = GetComponent<Animator>();
		rb = GetComponent<Rigidbody2D>();
		if (GetComponent<SpriteRenderer>().flipX) {
			moveDirection = -1;
		} else {
			moveDirection = 1;
		}
	}
	private void Update() {
		if (Time.time >= nextMoveTime) {
			moveStartTime = Time.time;
			nextMoveTime = Time.time + beatInterval;
			
			currentVelocity.x = (moveDistance + moveXOffset) * moveDirection;
			animator.Play("Projectile");
		}

		moveTimer = moveDuration;

		// Perform horizontal movement
		if (moveTimer > 0) {
			rb.velocity = new Vector2(currentVelocity.x, rb.velocity.y);
			moveTimer -= Time.deltaTime;
		} else {
			rb.velocity = new Vector2(0f, rb.velocity.y);
		}

		float t = (Time.time - moveStartTime) / moveDuration;

		if (t > 0.95f) {
			StartCoroutine(HandlePositionSnap());
			transform.position = snapToPosition;
			animator.Play("EmptyState");
		}
	}

	private IEnumerator HandlePositionSnap() {
		snapToPosition = SnapToGrid(transform.position, 1f);
		yield return null;
	}

	private Vector2 SnapToGrid(Vector2 position, float gridSize) {
		float x = Mathf.Round(position.x / gridSize) * gridSize;
		float y = Mathf.Round((position.y - 0.5f) / gridSize) * gridSize + 0.5f;
		
		return new Vector2(x, y);
	}
}
