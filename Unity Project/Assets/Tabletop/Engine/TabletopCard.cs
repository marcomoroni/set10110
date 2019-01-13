using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Tabletop/Card"), DisallowMultipleComponent]
public class TabletopCard : MonoBehaviour
{
	private GameObject _pivot;
	private SmoothAnimations _pivotSmoothAnimations;
	private SpriteRenderer _topSpriteRenderer;
	private SpriteRenderer _bottomSpriteRenderer;

	// Only TabletopDeck component should change this value
	[HideInInspector]
	public GameObject deck;

	private bool _faceUp = true;
	public bool faceUp
	{
		get { return _faceUp; }
		set
		{
			_faceUp = value;

			// Rotate the pivot
			_pivotSmoothAnimations.SmoothRotate(Quaternion.LookRotation(new Vector3(0.0f, 0.0f, _faceUp ? 1.0f : -1.0f), new Vector3(Mathf.Cos((transform.localEulerAngles.z + 90) * Mathf.Deg2Rad), Mathf.Sin((transform.localEulerAngles.z + 90) * Mathf.Deg2Rad), 0.0f)));
		}
	}

	void Start ()
	{
		_pivot = transform.Find("Card_Pivot").gameObject;
		_pivotSmoothAnimations = _pivot.GetComponent<SmoothAnimations>();
		_topSpriteRenderer = _pivot.transform.Find("Sprite_Top").gameObject.GetComponent<SpriteRenderer>();
		_bottomSpriteRenderer = _pivot.transform.Find("Sprite_Bottom").gameObject.GetComponent<SpriteRenderer>();
	}

	public void SetLayerOrder(int i)
	{
		// z-index of transform.position and sprite renderer have to work together.
		// position.z has to be changed because the box collider works with that position

		// [not working]
		transform.position = new Vector3(
			transform.position.x,
			transform.position.y,
			-i * 0.02f);

		_bottomSpriteRenderer.sortingOrder = i;
		_topSpriteRenderer.sortingOrder = i;
	}
}
