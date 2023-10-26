using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BossShieldEnemy : MonoBehaviour
{
	private GameObject player;
	private string enemyName;
	private float bpm = 40f;
	private float nextMoveTime = 0f;
	private int beatCounter = 0;
	private float moveTimer = 0f;
	private float moveStartTime;
	private float moveDuration;
	private float beatInterval;
	private Rigidbody2D rb;
	private Vector2 currentVelocity = Vector2.zero;
	private Vector2 snapToPosition;
	private Animator animator;
	private AudioManager40bpm audioManager40bpm;

	private void Start() {
		player = GameObject.FindGameObjectWithTag("Player");
		audioManager40bpm = GameObject.Find("AudioObject").GetComponent<AudioManager40bpm>();
		rb = GetComponent<Rigidbody2D>();
		enemyName = gameObject.name;
		beatInterval = 60f / bpm;
		moveDuration = beatInterval * 0.5f;
		animator = GetComponent<Animator>();
		transform.rotation = Quaternion.AngleAxis(90f, Vector3.forward);
	}

	private void Update() {
		if (Time.time >= nextMoveTime) {
			moveStartTime = Time.time;
			nextMoveTime = Time.time + beatInterval;

			float moveDistance = 2f;
			float moveOffset = 0.8f;

			Vector2 directionToPlayer = player.transform.position - transform.position;
			directionToPlayer.Normalize();
			currentVelocity = Vector2.zero;
			if (Mathf.Abs(directionToPlayer.x) >= Mathf.Abs(directionToPlayer.y)) {
				if (directionToPlayer.x >= 0f) {
					currentVelocity.x = (moveDistance + moveOffset) * 1f;
				} else {
					currentVelocity.x = (moveDistance + moveOffset) * -1f;
				}
			} else {
				if (directionToPlayer.y >= 0f) {
					currentVelocity.y = (moveDistance + moveOffset) * 1f;
				} else {
					currentVelocity.y = (moveDistance + moveOffset) * -1f;
				}
			}

			// Rotate projectile to face the direction of movement
			float angle = Mathf.Atan2(currentVelocity.y, currentVelocity.x) * Mathf.Rad2Deg - 180f;  // Subtract 180 degrees because prefab is facing left
			transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

			animator.Play("Shoot");
			if (beatCounter == 0) {
				audioManager40bpm.LoopEnemyChords();
			}

			beatCounter = (beatCounter + 1) % 4;

			moveTimer = moveDuration;
		}

		
		// Perform horizontal movement
		if (moveTimer > 0) {
			rb.velocity = currentVelocity;
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
		float y = Mathf.Round(position.y / gridSize) * gridSize;
		return new Vector2(x, y);
	}

	private void OnCollisionEnter2D(Collision2D collision) {
		if (collision.gameObject.CompareTag("Player")) {
			if (collision.gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Shield")) {
				Destroy(gameObject);
			} else {
				Destroy(collision.gameObject);
			}
		}
	}
}
