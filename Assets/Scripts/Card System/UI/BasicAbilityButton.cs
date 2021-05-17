using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BasicAbilityButton : MonoBehaviour
{
    //-----------------
    // member variables
    //-----------------

    [SerializeField] private Button button;
    [SerializeField] private Text abilityName;
    [SerializeField] private Text description;
    [SerializeField] private Text cost;

    public void SetContent(int basicAbilityID, BoardManager boardManager)
    {
        Debug.Assert(CardDatabase.Instance.GetBasicAbilityIDs().Contains(basicAbilityID));
        Card basicAbility = CardDatabase.Instance.GetCardByID(basicAbilityID);
        if(button != null)
        {
            button.onClick.AddListener(
                    () => { boardManager.UseBasicAbility(basicAbilityID); }
            );
        }
        if(abilityName != null)
        {
            abilityName.text = basicAbility.name;
        }
        if(description != null)
        {
            description.text = basicAbility.cardText;
        }
        if(description != null)
        {
            cost.text = basicAbility.cost.ToString();
        }
    }
}
