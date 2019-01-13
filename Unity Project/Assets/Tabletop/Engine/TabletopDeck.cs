using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// It contains data about a deck spot, including its card.
/// </summary>
public class TabletopDeckSpot
{
	/// <value>Transform data without scatter.</value>
	public TransformData transformData;

	/// <value>Transform data of scatter.</value>
	// Scatter data has to be stored here because it doesn't change when cards are laied down
	public TransformData scatterData;

	public GameObject card;

	public TabletopDeckSpot()
	{
		this.transformData = new TransformData();
		this.scatterData = new TransformData();
	}

	public TabletopDeckSpot(GameObject card)
	{
		this.transformData = new TransformData();
		this.scatterData = new TransformData();
		this.card = card;
	}
}

[AddComponentMenu("Tabletop/Deck"), DisallowMultipleComponent]
public class TabletopDeck : MonoBehaviour
{
	/// <summary>
	/// The style of the deck.
	/// </summary>
	public TabletopDeckStyle style;

	public bool allowAddCardsToTop = true;
	public bool allowAddCardsToBottom = true;
	public bool allowAddCardsToMiddle = true;

	public bool allowRemoveCardsFromTop = true;
	public bool allowRemoveCardsFromBottom = true;
	public bool allowRemoveCardsFromMiddle = true;

	/// <summary>
	/// The list of spots for the cards. <c>0</c> is the bottom one.
	/// It can include ghost cards.
	/// </summary>
	private List<TabletopDeckSpot> _spots = new List<TabletopDeckSpot>();

	private float _timeForGradually = 0.1f;

	public float timeForGradually
	{
		get { return _timeForGradually; }
		set { _timeForGradually = Mathf.Max(0, value); }
	}

	// [ADD COMMENTS]
	private GameObject _dropArea;
	private List<GameObject> _dropAreas = new List<GameObject>();

	void Start()
	{
		// Create the first drop area
		_dropArea = new GameObject("Drop_Area");
		_dropArea.transform.SetParent(transform, false);
		BoxCollider2D bc = _dropArea.AddComponent(typeof(BoxCollider2D)) as BoxCollider2D;
		bc.size = style.GetEdgeDropAreaSize();
		_dropAreas.Add(_dropArea);
		UpdateDropAreas();
	}

	#region Add and remove cards
	public void AddCards(List<GameObject> cards, GameObject dropArea, bool gradually = true)
	{
		AddCards(cards, _dropAreas.IndexOf(dropArea), gradually);
	}

	public void AddCard(GameObject card, GameObject dropArea, bool gradually = true)
	{
		AddCard(card, _dropAreas.IndexOf(dropArea), gradually);
	}

	/// <summary>
	/// Adds a card to the deck.
	/// </summary>
	/// <param name="card">Card.</param>
	/// <param name="index">Position on the deck. <c>0</c> is bottom, <c>-1</c> is top, <c>-2</c> is second from top and so on.</param>
	public void AddCard(GameObject card, int index = -1, bool gradually = true)
	{
		List<GameObject> temp = new List<GameObject>();
		temp.Add(card);
		AddCards(temp, index, gradually);
	}

	/// <summary>
	/// Adds a list of cards to the deck.
	/// </summary>
	/// <param name="cards">Cards.</param>
	/// <param name="index">Position on the deck. <c>0</c> is bottom, <c>-1</c> is top, <c>-2</c> is second from top and so on.</param>
	/// <param name="gradually">If <c>false</c> the animations have no delay.</param>
	public void AddCards(List<GameObject> cards, int index = -1, bool gradually = true)
	{
		// Remove spots with cards already in deck, so they will go to their new spot
		foreach (GameObject card in cards)
			_spots.RemoveAll(spot => spot.card == card);

		// Clamp and add cards to deck (start from top of index is negative)
		//
		//     Example with 4 cards:
		//
		//                          [ ][ ][ ][ ]
		//     Index:                0  1  2  3  4
		//     Negative index:   -5 -4 -3 -2 -1
		index = Mathf.Clamp(index, -_spots.Count - 1, _spots.Count);
		List<TabletopDeckSpot> newSpots = new List<TabletopDeckSpot>();
		for(int i = 0; i < cards.Count; i++)
		{
			TabletopDeckSpot newSpot = new TabletopDeckSpot(cards[i]);
			newSpots.Add(newSpot);

			// Save this deck to card
			cards[i].GetComponent<TabletopCard>().deck = gameObject;

			// Generate new scatter data for new cards
			newSpot.scatterData = style.GenerateScatter();
		}
		if (index >= 0) _spots.InsertRange(index, newSpots);
		else _spots.InsertRange(_spots.Count + index + 1, newSpots);

		// Lay down cards in table
		LayDown(index, gradually);
	}

	public void RemoveCard(GameObject card, bool gradually = true)
	{
		// Find with spot contains the card
		TabletopDeckSpot s = _spots.Find(spot => spot.card == card);

		if (s == null)
		{
			Debug.LogWarning("Trying to remove a card not in deck.");
			return;
		}

		// Remeber index
		int index = _spots.IndexOf(s);

		// Remove spot
		_spots.Remove(s);

		card.GetComponent<TabletopCard>().deck = null;

		// Lay down cards in table
		LayDown(index - 1, gradually);
	}
	#endregion

	#region Lay down
	/// <summary>
	/// Laies down cards.
	/// </summary>
	/// <param name="fromIndex">First card to lay down. If <c>-1</c> the first card is the top one.</param>
	/// <param name="gradually">If set to <c>false</c> all cards are positioned at the same time.</param>
	public void LayDown(int fromIndex = 0, bool gradually = true)
	{
		UpdateDropAreas();

		// Stop if there is no deck style
		if (style == null)
		{
			Debug.Log("Cannot lay down cards of \"" + gameObject.name + "\": deck style missing.");
			return;
		}

		// Set correct z index before anything else
		for(int i = 0; i < _spots.Count; i++)
		{
			// Set layer order (from the card component)
			TabletopCard tc = _spots[i].card.GetComponentInChildren<TabletopCard>();
			if (tc != null)
			{
				tc.SetLayerOrder(i);
			}
		}

		// Apply transform
		LayDownCorutineArgs args;
		args.fromIndex = fromIndex;
		args.gradually = gradually;
		StopCoroutine("LayDownCorutine");
		StartCoroutine("LayDownCorutine", args);
	}

	// Arguments to pass to corutine
	// Unfortunately this is the only way to pass more than one parameter to a courutine.
	private struct LayDownCorutineArgs
	{
		public int fromIndex;
		public bool gradually;
	}

	private IEnumerator LayDownCorutine(LayDownCorutineArgs args)
	{
		// Get real index [this is copied code, maybe use a function]
		if (args.fromIndex < 0) args.fromIndex = _spots.Count - 1;
		args.fromIndex = Mathf.Clamp(args.fromIndex, 0, _spots.Count);

		// Generate transform data
		List<TransformData> transformDatas = style.GetCardsTransformData(_spots.Count);

		// Start from the args.fromIndex, then gradually go to the cards on its side. Set the
		// transform of this cards by getting it from the deck style and the smoothly move them
		// using the smooth movements component
		int sideIndex = 0;
		while (sideIndex <= _spots.Count - 1)
		{
			if (sideIndex == 0)
			{
				// Initial card
				ApplyDeckStyleTransformToCard(args.fromIndex, transformDatas[args.fromIndex]);
            }
			else
			{
				if (args.fromIndex - sideIndex >= 0)
				{
					// Card before
					ApplyDeckStyleTransformToCard(args.fromIndex - sideIndex, transformDatas[args.fromIndex - sideIndex]);
				}
				if (args.fromIndex + sideIndex <= _spots.Count - 1)
				{
					// Card after
					ApplyDeckStyleTransformToCard(args.fromIndex + sideIndex, transformDatas[args.fromIndex + sideIndex]);
				}
			}

			sideIndex++;

			if (args.gradually) yield return new WaitForSeconds(_timeForGradually);
		}

		// Set layer order (from the card component) again (or the z pos will be 0)
		// [TODO: DO NOT REPEAT]
		for (int i = 0; i < _spots.Count; i++)
		{
			// Set layer order (from the card component)
			TabletopCard tc = _spots[i].card.GetComponentInChildren<TabletopCard>();
			if (tc != null)
			{
				tc.SetLayerOrder(i);
			}
		}

		yield return null;
	}

	private void ApplyDeckStyleTransformToCard(int index, TransformData t)
	{
		// Call the smooth transformations component
		SmoothAnimations smoothAnimations = _spots[index].card.GetComponent<SmoothAnimations>();
		if (smoothAnimations != null)
		{
			smoothAnimations.SmoothTranslate(transform.position + t.position + _spots[index].scatterData.position);
			smoothAnimations.SmoothRotate(transform.rotation * (t.rotation * _spots[index].scatterData.rotation));
		}
		else
		{
			Debug.LogError("Card is missing SmoothAnimations component.");
		}
	}

	// TO IMPLEMENT LATER
	/*private void ApplyDeckStyleTransformToCards(int indexStart, int indexEnd, List<TransformData> t)
	{
		for (int index = indexStart; index <= indexEnd; index++)
		{
			ApplyDeckStyleTransformToCard(index, t[index]);
		}
	}*/
	#endregion

	public void Shuffle()
	{
		for (int i = 0; i < _spots.Count; i++)
		{
			TabletopDeckSpot temp = _spots[i];
			int randomIndex = Random.Range(i, _spots.Count);
			_spots[i] = _spots[randomIndex];
			_spots[randomIndex] = temp;
		}

		// Get new scatter data
		for (int i = 0; i < _spots.Count; i++)
		{
			_spots[i].scatterData = style.GenerateScatter();
		}

		// Lay down cards
		LayDown(gradually: false);
	}

	// Update the drop areas.
	// This function either generates or activates drop area.
	// If there are too many areas they are deactivated instead of destroyed.
	// Then, illegal moves areas are deactivated as well.
	private void UpdateDropAreas()
	{
		int dropAreasWanted = _spots.Count + 1;

		// Deactivate all but first
		for (int i = 1; i < _dropAreas.Count; i++)
		{
			_dropAreas[i].SetActive(false);
		}

		// Reactivate (or create) the amount of drop areas wanted
		// and apply rules about where player can add cards
		for (int i = 1; i < dropAreasWanted; i++)
		{
			if (i < _dropAreas.Count)
			{
				// Reactivate
				_dropAreas[i].SetActive(true);
			}
			else
			{
				// Create area
				GameObject newArea = Instantiate(_dropArea, transform, false);
				newArea.name = "Drop_Area";
				_dropAreas.Add(newArea);
			}
		}

		// Set box collider sizes
		Vector2 boxColliderSizeMiddle = style.GetMiddleDropAreaSize();
		Vector2 boxColliderSizeEdge = style.GetEdgeDropAreaSize();
		for (int i = 0; i < dropAreasWanted; i++)
		{
			BoxCollider2D bc = _dropAreas[i].GetComponent<BoxCollider2D>();
			
			// If can add cards to middle use small drop area
			if (allowAddCardsToMiddle)
			{
				bc.size = boxColliderSizeMiddle;
			}
			else
			{
				bc.size = boxColliderSizeEdge;
			}

			// If there's only one card, force big drop area
			if (dropAreasWanted == 1)
			{
				bc.size = boxColliderSizeEdge;
			}
		}

		// Apply rules about where the player can add cards
		// (deactivate illegal positions)
		/*if (dropAreasWanted > 1)
		{
			// Bottom
			if (!allowAddCardsToBottom)
				_dropAreas[0].SetActive(false);

			// Top
			if (!allowAddCardsToTop)
				_dropAreas[dropAreasWanted - 1].SetActive(false);

			// Middle
			if (!allowAddCardsToMiddle)
			{
				for (int i = 1; i < dropAreasWanted - 1; i++)
				{
					_dropAreas[i].SetActive(false);
				}
			}
		}*/

		// Update transform
		List<TransformData> transformDatas = style.GetDropAreasTransformData(_spots.Count);
		for (int i = 0; i < dropAreasWanted; i++)
		{
			GameObject da = _dropAreas[i];
			TransformData t = transformDatas[i];

			da.transform.position = transform.position + new Vector3(t.position.x, t.position.y, -5 - (i * 0.02f));
			da.transform.rotation = transform.rotation * t.rotation;
		}
	}

	#region Gizmos

	public int gizmoCardsCount = 6;
	public bool showCardGizmos = true;

	void DrawGizmoCard(TransformData t, int cardIndex)
	{
		Color originalColor = Gizmos.color;

		Gizmos.color = Color.Lerp(originalColor, Color.white, (float)cardIndex / (gizmoCardsCount - 1));
		Gizmos.DrawWireCube(transform.position + t.position, new Vector3(0.2f, 0.2f, 0));

		Gizmos.color = originalColor;
	}

	void OnDrawGizmos()
	{
		// Draw in cyan, yellow if selected
		Gizmos.color = Selection.Contains(gameObject) ? Color.yellow : Color.cyan;

		// Draw deck origin
		Gizmos.DrawCube(transform.position, new Vector3(0.4f, 0.4f, 0));

		// Draw some cards
		if (style != null)
		{
			if (showCardGizmos)
			{
				List<TransformData> transformDatas = style.GetCardsTransformData(gizmoCardsCount);
				for (int i = 0; i < gizmoCardsCount; i++)
				{
					DrawGizmoCard(transformDatas[i], i);
				}
			}
		}
	}

	#endregion
}
