using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;

public class CameraProtection : MonoBehaviour
{
	private Transform cam;
	private Transform pivot;
	private float originalDistance;
	private float currentDistance;
	private Ray ray = new Ray();
	private RaycastHit[] hits;
	private RayComparison comparer;

	private float castRadius = 0.2f; //Radius of Sphere (Area where we check for colliders)
	private float velocity;
	private float clipMoveTime = 0.1f; //Time taken to move out of a clip
	private float returnTime = 0.4f; //Time taken to move to desired position
	private float closestDistance = 0.5f;  //Closest Distance to put Camera near Player
	// Start is called before the first frame update
	void Start()
	{
		cam = GetComponentInChildren<Camera>().transform;
		pivot = cam.parent;
		originalDistance = cam.localPosition.magnitude;
		currentDistance = originalDistance;
		comparer = new RayComparison();
	}

	// Update is called once per frame
	void LateUpdate()
	{
		float targetDistance = originalDistance;

		ray.origin = pivot.position + pivot.forward * castRadius;
		ray.direction = -pivot.forward;

		var collisions = Physics.OverlapSphere(ray.origin, castRadius);
		bool initialIntersection = false;

		for(int i = 0; i < collisions.Length; i++)
		{
			if((!collisions[i].isTrigger) && !(collisions[i].attachedRigidbody != null && collisions[i].attachedRigidbody.CompareTag("Player"))) //Check to make sure we aren't hitting a trigger (ignore) and it isn't the Player
			{
				initialIntersection = true;
				break;
			}
		}

		if (initialIntersection)
		{
			ray.origin += pivot.forward * castRadius;
			hits = Physics.RaycastAll(ray, originalDistance - castRadius);
		}
		else
		{
			hits = Physics.SphereCastAll(ray, castRadius, originalDistance + castRadius);
		}

		Array.Sort(hits,comparer);
		float closest = Mathf.Infinity;

		for(int i = 0; i < hits.Length; i++)
		{
			if (hits[i].distance < closest && !(hits[i].collider.isTrigger) && !(hits[i].collider.attachedRigidbody != null && hits[i].collider.attachedRigidbody.CompareTag("Player")))
			{
				closest = hits[i].distance;
				targetDistance = -pivot.InverseTransformPoint(hits[i].point).z;
			}
		}

		currentDistance = Mathf.SmoothDamp(currentDistance, targetDistance, ref velocity, currentDistance > targetDistance ? clipMoveTime : returnTime);
		currentDistance = Mathf.Clamp(currentDistance, closestDistance, originalDistance);
		cam.localPosition = -Vector3.forward * currentDistance;

	}

	public class RayComparison : IComparer
	{
		public int Compare(object x, object y)
		{
			return ((RaycastHit) x).distance.CompareTo(((RaycastHit) y).distance);
		}
	}

}
