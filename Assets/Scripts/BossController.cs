using UnityEngine;
using System.Collections;

public class BossController : MonoBehaviour
{
	[HideInInspector]
	public bool activateJumpEnemy = false;

	private float bpm = 40f;
	private float beatInterval;
	private float nextMoveTime = 0f;
	private float moveStartTime;
	private float moveDuration;
	private float moveTimer = 0f;
	private Vector2 snapToPosition;
	private Vector2 currentVelocity = Vector2.zero;
	private Rigidbody2D rb;
	private AudioManager40bpm audioManager40bpm;
	private int beatCounter = 0;
	private Animator animator;
	private CircleCollider2D circleCollider;
	private float startGravity;
	private int barCounter = 0;

	private void Start() {
		circleCollider = GetComponent<CircleCollider2D>();
		audioManager40bpm = GameObject.Find("AudioObject").GetComponent<AudioManager40bpm>();
		beatInterval = 60f / bpm;
		moveDuration = beatInterval * 0.5f;
		rb = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
		startGravity = rb.gravityScale;
	}

	private void Update() {
		if (Time.time >= nextMoveTime) {
			moveStartTime = Time.time;
			nextMoveTime = Time.time + beatInterval;

			if (beatCounter == 0) {
				barCounter = (barCounter + 1) % 32; // 32 bars in total, then the boss loop repeats
			}

			switch (barCounter) {
				case 1:
					if (beatCounter == 3) {
						activateJumpEnemy = true; // Set upp to activate on bar 2
						animator.Play("CloseEye");
					}
					break;
				case 2:
					animator.Play("ActivateJumpEnemies");
					break;
				case 3:
					animator.Play("EmptyState");
					break;
				case 4:
					break;
				case 5:
					activateJumpEnemy = false;
					break;
				case 6:
					break;
				case 7:
					break;
				case 8:
					break;
			}

			float moveDistance = 1f;
			float moveXOffset = 0.4f;
			int moveDirection = -1;
			currentVelocity.x = (moveDistance + moveXOffset) * moveDirection;

			beatCounter = (beatCounter + 1) % 4;

			moveTimer = moveDuration;
		}

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

	private void OnCollisionEnter2D(Collision2D collision) {
		if (collision.gameObject.CompareTag("Player")) {
			
		} else if (collision.gameObject.CompareTag("PlayerProjectile")) {
			
		}
	}
}
