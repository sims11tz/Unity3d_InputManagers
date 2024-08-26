using UnityEngine;

namespace Nswitch.Scripts
{
	public class Cube : MonoBehaviour
	{
		public float speed = 5f;
		public float rotationSpeed = 100f;

		private Vector3 direction;
		private Camera mainCamera;

		private void OnEnable()
		{
			
		}

		void Start()
		{
			// Initialize the direction with a random direction
			direction = Random.insideUnitSphere;
			direction = Random.insideUnitSphere;
			direction.z = 0; // Keep the movement in the XZ plane
			direction.Normalize();

			// Get the main camera
			mainCamera = Camera.main;
		}

		void Update()
		{
			if (!Application.isPlaying) return;
			
			// Move the cube
			transform.Translate(direction * speed * Time.deltaTime, Space.World);

			// Rotate the cube
			transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);

			// Check for collisions with the screen edges and bounce
			Vector3 viewportPosition = mainCamera.WorldToViewportPoint(transform.position);

			if (viewportPosition.x < 0 || viewportPosition.x > 1)
			{
				direction.x = -direction.x;
				viewportPosition.x = Mathf.Clamp01(viewportPosition.x);
				transform.position = mainCamera.ViewportToWorldPoint(viewportPosition);
			}

			if (viewportPosition.y < 0 || viewportPosition.y > 1)
			{
				direction.y = -direction.y;
				viewportPosition.y = Mathf.Clamp01(viewportPosition.y);
				transform.position = mainCamera.ViewportToWorldPoint(viewportPosition);
			}
		}
	}
}