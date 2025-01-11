using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    public TextMeshPro tx;
    public TextMeshPro rounds;
    public int �ĴX�^�X = 0;
    int �j���`�� = 0;
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
        �ӱѤ�r.text = "";
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
        tx.text = "Ball: " + GameObject.Find("/�o�g��").GetComponent<�o�g��>().�y��;
    }

    // Update is called once per frame
    void Update()
    {
        if (!levelHasLoadCompleted) return;
        if ((isPlaying))
        {
            tx.text = "Ball: " + GameObject.Find("/�o�g��").GetComponent<�o�g��>().�y��;
            rounds.text = "LEVEL: " + Level.ToString();
            //�����ӱ�
            �j���`�� = GameObject.FindGameObjectsWithTag("BRICKS").Length;
            if (�j���`�� == 0)
            {
                //WIN
                GetComponent<gameMaster>().isWon = true;
            }
        }
        

        if (isWon)
        {
            �ӱѤ�r.text = "WIN";
            BUT_NEXT.gameObject.SetActive(true);
            BUT_BACK.gameObject.SetActive(true);
            isPlaying = false ;

            //���b�o�g�� = false;
            GameObject[] bb = GameObject.FindGameObjectsWithTag("BALL");
            foreach (GameObject bb2 in bb)
            {
                Destroy(bb2);
            }

            return;
        }
        if (isLost) 
        {
            �ӱѤ�r.text = "LOSE";
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
