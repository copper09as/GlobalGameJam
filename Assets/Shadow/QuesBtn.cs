using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuesBtn : MonoBehaviour
{
   [SerializeField]GameObject quesPanel;
    public void OnQuesBtnClick()
    {
        if(quesPanel.activeSelf)
        {
            quesPanel.SetActive(false);
        }
        else
        {
            quesPanel.SetActive(true);
        }
    }
}
