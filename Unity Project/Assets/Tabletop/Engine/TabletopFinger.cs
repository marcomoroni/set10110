using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Tabletop/Finger"), DisallowMultipleComponent]
public class TabletopFinger : MonoBehaviour
{
	private GameObject _card;

	void Update ()
	{
		SetPositionAsMousePosition();

		// Grab a card
		if (_card == null && Input.GetMouseButtonDown(0))
		{
			RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, Vector2.zero);

			foreach (RaycastHit2D hit in hits)
			{
				// Check ray collision
				GameObject collided = hit.collider.gameObject;

				// If it's a card
				if (collided.GetComponent<TabletopCard>() != null)
				{
					_card = collided;

					// Reset rotation
					_card.GetComponent<SmoothAnimations>().SmoothRotate(Quaternion.identity);

					break;
				}
			}
		}
		
		// If holding a card, set the card position as finger position
		if (_card != null && Input.GetMouseButton(0))
		{
			_card.transform.position = transform.position;

			// You can also flip it
			if (Input.GetKeyDown("f"))
			{
				TabletopCard tc = _card.GetComponent<TabletopCard>();
				tc.faceUp = !tc.faceUp;
			}
		}

		// Release card
		if (_card != null && Input.GetMouseButtonUp(0))
		{
			// Remove from original deck
			TabletopCard tc = _card.GetComponent<TabletopCard>();
			if (tc != null)
			{
				GameObject originalDeck = tc.deck;
				if (originalDeck != null)
				{
					originalDeck.GetComponent<TabletopDeck>().RemoveCard(_card);
				}
			}

			// Cast a ray
			RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, Vector2.zero);

			foreach(RaycastHit2D hit in hits)
			{
				// Check ray collision
				GameObject collided = hit.collider.gameObject;

				if (collided.name == "Drop_Area")
				{
					TabletopDeck newDeck = collided.transform.parent.GetComponent<TabletopDeck>();
					newDeck.AddCard(_card, collided);

					break;
				}
			}

			_card = null;
		}

		// If not holding a card, flip a hovered card
		if (_card == null && Input.GetKeyDown("f"))
		{
			// Cast a ray
			RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, Vector2.zero);

			foreach (RaycastHit2D hit in hits)
			{
				// Check ray collision
				TabletopCard c = hit.collider.gameObject.GetComponent<TabletopCard>();

				// If it's a card
				if (c != null)
				{
					c.faceUp = !c.faceUp;

					break;
				}
			}
		}
	}

	private void SetPositionAsMousePosition()
	{
		Vector3 fingerPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		fingerPosition.z = -4.0f;

		transform.position = fingerPosition;
	}
}
