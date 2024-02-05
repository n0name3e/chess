using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityManager : MonoBehaviour
{
    private static AbilityManager instance;
    string jsonData;
    AbilityDataList abilityList;
    public List<Ability> generalAbilities = new List<Ability>();

    Vector2 anchorPosition;

    public static AbilityManager Instance { get => instance; set => instance = value; }
    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        jsonData = Resources.Load<TextAsset>("json/abilities").text;
        abilityList = JsonUtility.FromJson<AbilityDataList>(jsonData);
    }
    System.Collections.IEnumerator Start()
    {
        yield return null;
        foreach (Piece piece in BoardCreator.mainBoard.pieces)
        {
            foreach (Ability ability in piece.abilities)
            {
                ability.OnStart?.Invoke(piece);
            }
        }
    }
    public AbilityData FindAbility(string name)
    {
        return abilityList.abilities.Find(x => x.name == name);
    }


    public void CastAbility(Ability ability, Tile tile, Piece castingPiece)
    {
        if (!ability.canBeCasted()) return;
        ability.OnGeneralAbilityTileChoose?.Invoke(tile);

        ability.OnTileChoose?.Invoke(tile, castingPiece);
        
        if (ability.OnUntargetedAbilityChoose != null)
        {
            if (ability.OnUntargetedAbilityChoose.Invoke(castingPiece) == false) return;
        }
        if (GameManager.Instance.KingInCheck(BoardCreator.mainBoard, Colors.Black).check)
        {
            GameManager.Instance.EndTurn();
        }
        ability.cooldown = FindAbility(ability.name).cooldown;
        castingPiece.mana -= FindAbility(ability.name).manaCost;
        if (ability.abilityData.chargesConditions.Count > 0)
        {
            ability.charges--;
        }
        UI.Instance.DisplayInfoContainer(castingPiece, true); // updates ui
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="piece"></param>
    /// <param name="container">container of abilities</param>
    public void CreateAbilityButtons(Piece piece, Transform container, Transform generalAbilityContainer, bool castableAbilities)
    {
        anchorPosition = container.GetComponent<GridLayoutGroup>().cellSize;
        for (int i = 0; i < piece.abilities.Count; i++)
        {
            CreateAbility(piece.abilities[i], container, castableAbilities);
        } 
        for (int j = 0; j < generalAbilities.Count; j++)
        {
            CreateAbility(generalAbilities[j], generalAbilityContainer, castableAbilities);          
        }      
    }

    private void CreateAbility(Ability ability, Transform container, bool castableAbilities)
    {
        GameObject button = Instantiate(new GameObject("Ability"), container);
        button.AddComponent<RectTransform>();
        button.transform.SetParent(container);
        button.AddComponent<Image>();
        Sprite sprite = Resources.Load<Sprite>("abilityIcons/" + ability.name);
        if (sprite != null)
        {
            button.GetComponent<Image>().sprite = sprite;
        }
        else
        {
            GameObject text = Instantiate(new GameObject("text"));
            RectTransform rt = text.AddComponent<RectTransform>();
            Text t = text.AddComponent<Text>();
            t.text = ability.name;
            t.transform.SetParent(button.transform);
            rt.anchoredPosition = Vector2.zero;
            t.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
            t.color = Color.black;
            t.alignment = TextAnchor.MiddleCenter;
            t.fontSize = 25;
        }

        float mana = FindAbility(ability.name).manaCost;
        if (mana > 0)
        {
            GameObject manaCost = new GameObject("mana");
            RectTransform rt = manaCost.AddComponent<RectTransform>();
            manaCost.transform.SetParent(button.transform);
            rt.anchoredPosition = Vector2.zero;
            rt.sizeDelta = anchorPosition;
            Text mpCostText = manaCost.AddComponent<Text>();
            mpCostText.alignment = TextAnchor.LowerRight;
            mpCostText.color = Color.blue;
            mpCostText.fontSize = 40;
            mpCostText.text = mana.ToString();
            mpCostText.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
        }

        CreateCooldown(ability, button.transform);
        CreateCharges(ability, button.transform);

        if (!castableAbilities) return;

        if (FindAbility(ability.name).targeted)
        {
            if (ability.general)
            {
                button.AddComponent<Button>().onClick.AddListener(delegate
                {
                    TileSelector.Instance.SetSelectedAbility(ability, ability.OnGeneralAbilityChoose(BoardCreator.mainBoard));
                    //TileSelector.Instance.HighlightAbilityTiles(ability.OnTargetedAbilityChoose(piece)); 
                });
            }
            else
            {
                button.AddComponent<Button>().onClick.AddListener(delegate
                {
                    TileSelector.Instance.SetSelectedAbility(ability, ability.OnTargetedAbilityChoose(ability.owner));
                });
            }

        }
        else
        {
            button.AddComponent<Button>().onClick.AddListener(delegate { /*if (ability.canBeCasted()) /* => */ CastAbility(ability, null, ability.owner); });
        }
    }
    public void CreateCooldown(Ability ability, Transform abilityObject)
    {
        int currentCooldown = ability.cooldown;
        if (currentCooldown <= 0) return;
        int cooldown = FindAbility(ability.name).cooldown;

        GameObject uiCooldown = new GameObject("cooldown");
        Image image = uiCooldown.AddComponent<Image>();
        image.sprite = Resources.Load<Sprite>("Square");
        uiCooldown.transform.SetParent(abilityObject);
        RectTransform rt = uiCooldown.GetComponent<RectTransform>();

        rt.anchoredPosition = Vector2.zero;
        rt.sizeDelta = anchorPosition;

        image.color = new Color(0.3f, 0.3f, 0.3f, 2/3f);
        image.type = Image.Type.Filled;
        image.fillAmount = (float) currentCooldown / cooldown; //currentCooldown / cooldown;
        image.fillClockwise = false;
        image.fillOrigin = (int) Image.Origin360.Top;

        GameObject uiTextCooldown = new GameObject("cd");
        Text text = uiTextCooldown.AddComponent<Text>();
        text.transform.SetParent(uiCooldown.transform);
        text.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        text.alignment = TextAnchor.MiddleCenter;
        text.text = currentCooldown.ToString();
        text.fontSize = 50;
        text.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
    }
    public void CreateCharges(Ability ability, Transform abilityObject)
    {
        if (ability.abilityData.chargesConditions.Count <= 0) return;
        GameObject uiTextCooldown = new GameObject("charges");
        Text text = uiTextCooldown.AddComponent<Text>();
        text.transform.SetParent(abilityObject.transform);

        RectTransform rt = text.GetComponent<RectTransform>();
        rt.anchoredPosition = Vector2.zero;
        rt.sizeDelta = anchorPosition;

        text.alignment = TextAnchor.LowerLeft;
        text.color = Color.black;
        text.text = ability.charges.ToString();
        text.fontSize = 40;
        text.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
    }
}
