using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class gameMaster : MonoBehaviour
{
    public bool isWon = false;
    public bool isLost = false;
    public Text 勝敗文字;
    public Button BUT_NEXT;
    public Button BUT_BACK;
    public int Level = 1;
    public TextMeshPro tx;
    public TextMeshPro rounds;
    public int 第幾回合 = 0;
    int 磚塊總數 = 0;
    public bool isPlaying = false;
    public GameObject Shooter;
    public bool levelHasLoadCompleted = false;
    // Start is called before the first frame update
    public void Start()
    {
        Shooter.SetActive(false);
        isPlaying = false ;
        isWon = false;
        isLost = false;
        勝敗文字.text = "";
        BUT_NEXT.gameObject.SetActive(false);
        BUT_BACK.gameObject.SetActive(false);
        StartCoroutine(setIsPlaying());
        
    }
    void getGamePlay()
    {
        Level = currentLevel._CurrentLevel;
        if (Level > 9) 
        {  
            Level = 1;
            currentLevel._CurrentLevel = 1;
        }
        GameObject.Find("00GameMaster").GetComponent<processCSV>().getLevel(Level);
        rounds.text = "LEVEL: " + Level.ToString();
        tx.text = "Ball: " + GameObject.Find("/發射器").GetComponent<發射器>().球數;
    }

    // Update is called once per frame
    void Update()
    {
        if (!levelHasLoadCompleted) return;
        if ((isPlaying))
        {
            tx.text = "Ball: " + GameObject.Find("/發射器").GetComponent<發射器>().球數;
            rounds.text = "LEVEL: " + Level.ToString();
            //偵測勝敗
            磚塊總數 = GameObject.FindGameObjectsWithTag("BRICKS").Length;
            if (磚塊總數 == 0)
            {
                //WIN
                GetComponent<gameMaster>().isWon = true;
            }
        }
        

        if (isWon)
        {
            勝敗文字.text = "WIN";
            BUT_NEXT.gameObject.SetActive(true);
            BUT_BACK.gameObject.SetActive(true);
            isPlaying = false ;

            //正在發射中 = false;
            GameObject[] bb = GameObject.FindGameObjectsWithTag("BALL");
            foreach (GameObject bb2 in bb)
            {
                Destroy(bb2);
            }

            return;
        }
        if (isLost) 
        {
            勝敗文字.text = "LOSE";
            //BUT_NEXT.gameObject.SetActive(true);
            BUT_BACK.gameObject.SetActive(true);
            isPlaying = false;
            return;
        }
    }
    IEnumerator setIsPlaying()
    {
        yield return new WaitForSeconds(1f);
        Shooter.SetActive(true);
        isPlaying = true;
        getGamePlay();
    }
}
