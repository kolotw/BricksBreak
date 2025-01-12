using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

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

    private void Start()
    {
        Shooter.SetActive(false);
        isPlaying = false;
        isWon = false;
        isLost = false;
        勝敗文字.text = "";
        BUT_NEXT.gameObject.SetActive(false);
        BUT_BACK.gameObject.SetActive(false);

        // 訂閱事件
        processCSV.OnBricksGenerated += HandleBricksGenerated;

        // 開始載入關卡
        getGamePlay();
    }

    private void OnDestroy()
    {
        // 取消訂閱事件
        processCSV.OnBricksGenerated -= HandleBricksGenerated;
    }

    private void getGamePlay()
    {
        Level = currentLevel._CurrentLevel;
        if (Level > 9)
        {
            Level = 1;
            currentLevel._CurrentLevel = 1;
        }
        if(SceneManager.GetActiveScene().name == "LV01_基礎場景")
        {
            GameObject.Find("00GameMaster").GetComponent<processCSV>().getLevel(Level);
            rounds.text = "LEVEL: " + Level.ToString();
        }
        if (SceneManager.GetActiveScene().name == "LV_Random")
        {
            GetComponent<產生磚塊>().genBricks();
            Shooter.SetActive(true);
            isPlaying = true;
            rounds.text = "LEVEL: Random";
        }
        
        tx.text = "Ball: " + currentLevel.balls;
    }

    private void HandleBricksGenerated()
    {
        // 磚塊生成完成後，啟動遊戲
        Debug.Log("Bricks generated! Starting gameplay...");
        StartCoroutine(setIsPlaying());
    }

    private IEnumerator setIsPlaying()
    {
        yield return new WaitForSeconds(1f);
        Shooter.SetActive(true);
        isPlaying = true;
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            SceneManager.LoadScene("01_選擇關卡");
        }
        if(SceneManager.GetActiveScene().name == "LV01_基礎場景")
        {
            if (!levelHasLoadCompleted)
            {
                isPlaying = false;
                return;
            }
        }        

        if (isPlaying)
        {
            tx.text = "Ball: " + currentLevel.balls;
            //rounds.text = "LEVEL: " + Level.ToString();
            if (SceneManager.GetActiveScene().name == "LV_Random")
            {
                rounds.text = "Round: " + 第幾回合.ToString();
                //if (第幾回合 > 30) isWon=true;
            }
            // 偵測勝敗
            GameObject[] bb = GameObject.FindGameObjectsWithTag("BRICKS");
            磚塊總數 = bb.Length;
            foreach (GameObject b2 in bb)
            {
                if (b2.name == "AddBall(Clone)") 磚塊總數--;
                if (b2.name == "Spread(Clone)") 磚塊總數--;
                if (b2.name == "-(Clone)") 磚塊總數--;
                if (b2.name == "+(Clone)") 磚塊總數--;
            }

            if (磚塊總數 == 0)
            {
                // WIN
                isWon = true;
            }
        }

        if (isWon)
        {
            Shooter.SetActive(false);
            勝敗文字.text = "WIN";
            BUT_NEXT.gameObject.SetActive(true);
            BUT_BACK.gameObject.SetActive(true);
            isPlaying = false;

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
            BUT_BACK.gameObject.SetActive(true);
            isPlaying = false;
            return;
        }
    }
}
