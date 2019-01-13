using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TabletopEditorWindow : EditorWindow
{
	// For scrollable area
	Vector2 scrollPosition = Vector2.zero;

	[SerializeField] private List<DatabaseCard> _databaseCards = new List<DatabaseCard>();

	// Default values
	[SerializeField] public Sprite defaultFrontSprite;
	[SerializeField] public Sprite defaultBackSprite;
	[SerializeField] public int defaultQuantity = 1;

	[SerializeField] public GameObject cardPrefab;

	#region Save and load data

	//https://answers.unity.com/questions/119978/how-do-i-have-an-editorwindow-save-its-data-inbetw.html

	protected void OnEnable()
	{
		// Here we retrieve the data if it exists or we save the default field initialisers we set above
		var data = EditorPrefs.GetString("TabletopEditorWindow", JsonUtility.ToJson(this, false));
		// Then we apply them to this window
		JsonUtility.FromJsonOverwrite(data, this);
	}

	protected void OnDisable()
	{
		// We get the Json data
		var data = JsonUtility.ToJson(this, false);
		// And we save it
		EditorPrefs.SetString("TabletopEditorWindow", data);
	}

	#endregion

	[MenuItem("Window/Tabletop/Cards Database")]
	public static void ShowWindow()
	{
		GetWindow<TabletopEditorWindow>(false, "Tabletop Cards Database", true);
	}

	private void DrawToolbar()
	{
		EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

		if (GUILayout.Button("New Card", EditorStyles.toolbarButton))
		{
			DatabaseCard c = new DatabaseCard(defaultFrontSprite, defaultBackSprite, defaultQuantity);
			_databaseCards.Add(c);
		}

		if (GUILayout.Button("Delete All Cards", EditorStyles.toolbarButton))
		{
			if (EditorUtility.DisplayDialog(
				"Delete all cards?",
				"Are you sure you want to delete all the cards in the database?",
				"Delete All", "Nope"))
			{
				_databaseCards.Clear();
			}
		}

		EditorGUILayout.Space();
		GUILayout.FlexibleSpace();

		// Only in edit mode
		EditorGUI.BeginDisabledGroup(EditorApplication.isPlaying);
		if (GUILayout.Button("Delete Cards in Scene", EditorStyles.toolbarButton))
		{
			DeleteAllCardsInScene();
		}
		if (GUILayout.Button("Update Scene", EditorStyles.toolbarButton))
		{
			DeleteAndInstantiateCardsInScene();
		}
		EditorGUI.EndDisabledGroup();

		EditorGUILayout.EndHorizontal();
	}

	private void DrawPrefabs()
	{
		GUILayout.Label("Prefabs", EditorStyles.boldLabel);
		cardPrefab = EditorGUILayout.ObjectField("Card", cardPrefab, typeof(GameObject), false) as GameObject;
	}

	private void DrawDefaultFields()
	{
		GUILayout.Label("Defaults", EditorStyles.boldLabel);
		defaultFrontSprite = EditorGUILayout.ObjectField("Front Sprite", defaultFrontSprite, typeof(Sprite), false) as Sprite;
		defaultBackSprite = EditorGUILayout.ObjectField("Back Sprite", defaultBackSprite, typeof(Sprite), false) as Sprite;		
		defaultQuantity = Mathf.Max(0, EditorGUILayout.IntField("Quantity", defaultQuantity));
	}

	private void DrawListOfCards()
	{
		// Calculate tot number of cards
		int cardsTot = 0;
		foreach (DatabaseCard c in _databaseCards)
		{
			cardsTot += c.quantity;
		}

		GUILayout.Label("Cards (" + _databaseCards.Count + " types, "+ cardsTot + " total)", EditorStyles.boldLabel);

		for (int i = 0; i < _databaseCards.Count; i++)
		{
			DrawCardInfo(_databaseCards[i]);
		}
	}

	private void DrawCardInfo(DatabaseCard card)
	{
		EditorGUILayout.BeginHorizontal();

		card.frontSprite = EditorGUILayout.ObjectField(card.frontSprite, typeof(Sprite), false, GUILayout.Width(60), GUILayout.Height(60)) as Sprite;

		card.backSprite = EditorGUILayout.ObjectField(card.backSprite, typeof(Sprite), false, GUILayout.Width(60), GUILayout.Height(60)) as Sprite;

		EditorGUILayout.Space();
		GUILayout.FlexibleSpace();

		card.quantity = Mathf.Max(0, EditorGUILayout.IntField(card.quantity, GUILayout.MaxWidth(50)));

		EditorGUILayout.Space();
		GUILayout.FlexibleSpace();

		if (GUILayout.Button("Duplicate", GUILayout.ExpandWidth(false)))
		{
			DatabaseCard c = new DatabaseCard(card.frontSprite, card.backSprite, card.quantity);
			// Use `Insert` instead of `Add`, so when duplicating, the new card is shown below the original one
			_databaseCards.Insert(_databaseCards.IndexOf(card) + 1, c);
		}

		if (GUILayout.Button("Delete", GUILayout.ExpandWidth(false)))
		{
			_databaseCards.Remove(card);
		}

		EditorGUILayout.EndHorizontal();
	}

	private void OnGUI()
	{
		DrawToolbar();
		scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
		DrawPrefabs();
		EditorGUILayout.Space();
		DrawDefaultFields();
		EditorGUILayout.Space();
		DrawListOfCards();
		EditorGUILayout.EndScrollView();
	}

	private void DeleteAllCardsInScene()
	{
		// Delete all cards (GameObjects tagged `Card`)
		GameObject[] cardsToDelete = GameObject.FindGameObjectsWithTag("Card");
		foreach (GameObject c in cardsToDelete) DestroyImmediate(c);
	}

	private void DeleteAndInstantiateCardsInScene()
	{
		if (cardPrefab == null)
		{
			Debug.LogError("Card prefab missing.");
			return;
		}

		DeleteAllCardsInScene();

		// Create new cards
		foreach (DatabaseCard c in _databaseCards)
		{
			for (int i = 0; i < c.quantity; i++)
			{
				GameObject newCard = Instantiate(cardPrefab, new Vector3(0, 0, 0), Quaternion.identity);
				newCard.name = "Card";

				SpriteRenderer srTop = newCard.transform.Find("Card_Pivot").gameObject.transform.Find("Sprite_Top").gameObject.GetComponent<SpriteRenderer>();
				srTop.sprite = c.frontSprite;

				SpriteRenderer srBottom = newCard.transform.Find("Card_Pivot").gameObject.transform.Find("Sprite_Bottom").gameObject.GetComponent<SpriteRenderer>();
				srBottom.sprite = c.backSprite;
			}
		}
	}
}


[System.Serializable]
public class DatabaseCard
{
	[SerializeField] public Sprite frontSprite;
	[SerializeField] public Sprite backSprite;
	[SerializeField] public int quantity = 1;

	public DatabaseCard() { }
	public DatabaseCard(Sprite f, Sprite b) { frontSprite = f; backSprite = b; }
	public DatabaseCard(Sprite f, Sprite b, int q) { frontSprite = f; backSprite = b; quantity = q; }
}