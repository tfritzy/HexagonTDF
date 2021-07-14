using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityButton : MonoBehaviour
{
    public int AbilityIndex;
    private Image cooldown;
    private Text abilityStatusMessage;

    // Start is called before the first frame update
    void Start()
    {
        if (AbilityIndex >= Managers.Board.Hero.Abilities.Count)
        {
            this.gameObject.SetActive(false);
        }

        abilityStatusMessage = this.transform.Find("AbilityStatusMessage").GetComponent<Text>();
        cooldown = this.transform.Find("Cooldown").GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        cooldown.fillAmount = Managers.Board.Hero.Abilities[this.AbilityIndex].RemainingCooldownPercent();
        abilityStatusMessage.text = Managers.Board.Hero.Abilities[this.AbilityIndex].CurrentStatusMessage();
    }

    public void Trigger()
    {
        Managers.Board.Hero.Abilities[this.AbilityIndex].Cast();
    }
}
