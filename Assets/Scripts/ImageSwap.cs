using System.Collections;
using UnityEngine;

public class ImageSwap : MonoBehaviour
{
    public Sprite[] sprites;

	private SpriteRenderer spriteRenderer;
	private float bpm = 40f;
	private float nextBeatTime = 0f;
	private float beatInterval;

	private void Start() {
		spriteRenderer = GetComponent<SpriteRenderer>();
		beatInterval = 60f / bpm;
	}

	private void Update() {
		if (Time.time >= nextBeatTime) {
			nextBeatTime = Time.time + beatInterval;
			StartCoroutine(SwapImages());
		}
	}

	private IEnumerator SwapImages() {
		float swapInterval = 0.04f;

		for (int i = 0; i < sprites.Length; i++) {
			spriteRenderer.sprite = sprites[i];
			yield return new WaitForSeconds(swapInterval);
		}

		spriteRenderer.sprite = sprites[0];
	}
}
