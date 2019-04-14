using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSelectPlayer : MonoBehaviour
{
    public void OnPlayerSelectButtonClicked()
    {
        if (this.gameObject.CompareTag("ProgressMenuPlayerSelectButton"))
        {
            char[] numbers = { '(', 'C', 'l', 'o', 'n', 'e', ')'};
            foreach (Player iterator in AppController.instance.allPlayerProgressData.playersList)
            {
                string trimmed = this.gameObject.name.Trim(numbers);
                if (trimmed == iterator.PlayerID.ToString())
                {
                    ProgressMenu.clickedPlayer = iterator;
                    Debug.Log("clicked player : " + ProgressMenu.clickedPlayer.PlayerName);
                    return;
                }
            }
        }
    }
}
