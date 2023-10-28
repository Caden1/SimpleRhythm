using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
	private float beatInterval;
	private float moveStartTime;
	private float moveDuration;
	private int moveDirection;

	private const float bpm = 40f;
	private const float moveDistance = 2f;
	private const float moveXOffset = 0.8f;

	private float nextMoveTime = 0f;
	private float moveTimer = 0f;

	private Vector2 currentVelocity = Vector2.zero;

	private Animator animator;
	private Rigidbody2D rb;

	private void Start() {
		CacheComponents();
		InitializeVariables();
	}

	private void Update() {
		if (IsTimeForNextSpawn()) {
			ProcessMove();
		}

		ApplyMovement();
		SnapTransform();
	}

	private void CacheComponents() {
		animator = GetComponent<Animator>();
		rb = GetComponent<Rigidbody2D>();
		if (GetComponent<SpriteRenderer>().flipX) {
			moveDirection = -1;
		} else {
			moveDirection = 1;
		}
	}

	private void InitializeVariables() {
		beatInterval = 60f / bpm;
		moveDuration = beatInterval * 0.5f;
	}

	private bool IsTimeForNextSpawn() {
		return Time.time >= nextMoveTime;
	}

	private void ProcessMove() {
		float currentTime = Time.time;
		moveStartTime = currentTime;
		nextMoveTime = currentTime + beatInterval;

		currentVelocity.x = (moveDistance + moveXOffset) * moveDirection;
		animator.Play("Projectile");
	}

	private void ApplyMovement() {
		moveTimer = moveDuration;

		if (moveTimer > 0) {
			rb.velocity = new Vector2(currentVelocity.x, rb.velocity.y);
			moveTimer -= Time.deltaTime;
		} else {
			rb.velocity = new Vector2(0f, rb.velocity.y);
		}
	}

	private void SnapTransform() {
		float t = (Time.time - moveStartTime) / moveDuration;

		if (t > 0.95f) {
			transform.position = SnapToGrid(transform.position, 1f);
			animator.Play("EmptyState");
		}
	}

	private Vector2 SnapToGrid(Vector2 position, float gridSize) {
		float x = Mathf.Round(position.x / gridSize) * gridSize;
		float y = Mathf.Round((position.y - 0.5f) / gridSize) * gridSize + 0.5f;
		
		return new Vector2(x, y);
	}
}
