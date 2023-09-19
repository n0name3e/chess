using UnityEngine;
using UnityEngine.UI;
public class UI : MonoBehaviour
{
    private static UI instance;

    public GameObject infoContainer;

    public static UI Instance { get => instance; set => instance = value; }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        infoContainer.SetActive(GameManager.instance.showAbilities);
    }
    public void DisplayInfoContainer(Piece piece, bool castableAbilities)
    {
        if (!GameManager.instance.showAbilities) return;
        RemoveAbilityButtons();
        if (piece.pieceObject == null)
        {
            infoContainer.transform.GetChild(2).GetComponent<Image>().fillAmount = 0; // health
            infoContainer.transform.GetChild(2).GetChild(0).GetComponent<Text>().text = "0/" + piece.maxHealth.ToString();
            return;
        }
        infoContainer.transform.GetChild(0).GetComponent<Image>().sprite // piece image
            = piece.pieceObject.GetComponent<Image>().sprite;
        infoContainer.transform.GetChild(1).GetComponent<Text>().text = piece.pieceObject.ToString();
        infoContainer.transform.GetChild(2).GetComponent<Image>().fillAmount // health
            = piece.health / piece.maxHealth;   
        infoContainer.transform.GetChild(2).GetChild(0).GetComponent<Text>().text =
            $"{Mathf.Ceil(piece.health)}/{Mathf.Ceil(piece.maxHealth)}";
        infoContainer.transform.GetChild(3).GetComponent<Image>().fillAmount // mana
            = piece.mana / piece.maxMana;   
        infoContainer.transform.GetChild(3).GetChild(0).GetComponent<Text>().text =
            $"{Mathf.Ceil(piece.mana)}/{Mathf.Ceil(piece.maxMana)}";
        AbilityManager.Instance.CreateAbilityButtons(piece, infoContainer.transform.GetChild(4), castableAbilities);
    }
    public void RemoveAbilityButtons()
    {
        foreach (Transform child in infoContainer.transform.GetChild(4))
        {
            Destroy(child.gameObject);
        }
    }
    public void Restart()
    {
        BoardCreator.Instance.DestroyMainBoard();
        GameManager.instance.StartGame();
    }
    public void Quit()
    {
        Application.Quit();
    }
}