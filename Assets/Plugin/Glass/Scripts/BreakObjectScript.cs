using UnityEngine;

public class BreakObjectScript : MonoBehaviour
{
	[SerializeField]
	private GameObject brokenObject;
	[SerializeField]
	private float maximum_magnitude;
	[SerializeField]
	private float radius;
	[SerializeField]
	private float power;

	void OnCollisionEnter(Collision collision)
	{
		if (Mathf.Abs(collision.relativeVelocity.magnitude) > maximum_magnitude)
		{
			Vector3 collision_position = collision.transform.position;
			GameObject broken_object = Instantiate(brokenObject, transform.position, transform.rotation);
			broken_object.transform.localScale = transform.localScale;


			Collider[] colliders = Physics.OverlapSphere(collision_position, radius);

			foreach (var piece in colliders) {
				var rigibody = piece.GetComponent<Rigidbody>();
				if (rigibody != null)
				{
					rigibody.AddExplosionForce(power * collision.relativeVelocity.magnitude, collision_position, radius);
				}

			}
			Destroy(gameObject);
		}
	}
}
