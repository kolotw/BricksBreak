using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class gameMaster : MonoBehaviour
{
    public bool isWon = false;
    public bool isLost = false;
    public Text �ӱѤ�r;
    public Button BUT_NEXT;
    public Button BUT_BACK;
    public int Level = 1;
    // Start is called before the first frame update
    void Start()
    {
        �ӱѤ�r.text = "";
        BUT_NEXT.gameObject.SetActive(false);
        BUT_BACK.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (isWon)
        {
            �ӱѤ�r.text = "WIN";
            BUT_NEXT.gameObject.SetActive(true);
            BUT_BACK.gameObject.SetActive(true);
            return;
        }
        if (isLost) 
        {
            �ӱѤ�r.text = "LOSE";
            //BUT_NEXT.gameObject.SetActive(true);
            BUT_BACK.gameObject.SetActive(true);
            return;
        }
    }
}
