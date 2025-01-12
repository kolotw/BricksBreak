using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

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

    private void Start()
    {
        Shooter.SetActive(false);
        isPlaying = false;
        isWon = false;
        isLost = false;
        �ӱѤ�r.text = "";
        BUT_NEXT.gameObject.SetActive(false);
        BUT_BACK.gameObject.SetActive(false);

        // �q�\�ƥ�
        processCSV.OnBricksGenerated += HandleBricksGenerated;

        // �}�l���J���d
        getGamePlay();
    }

    private void OnDestroy()
    {
        // �����q�\�ƥ�
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
        if(SceneManager.GetActiveScene().name == "LV01_��¦����")
        {
            GameObject.Find("00GameMaster").GetComponent<processCSV>().getLevel(Level);
            rounds.text = "LEVEL: " + Level.ToString();
        }
        if (SceneManager.GetActiveScene().name == "LV_Random")
        {
            GetComponent<���Ϳj��>().genBricks();
            Shooter.SetActive(true);
            isPlaying = true;
            rounds.text = "LEVEL: Random";
        }
        
        tx.text = "Ball: " + currentLevel.balls;
    }

    private void HandleBricksGenerated()
    {
        // �j���ͦ�������A�ҰʹC��
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
            SceneManager.LoadScene("01_������d");
        }
        if(SceneManager.GetActiveScene().name == "LV01_��¦����")
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
                rounds.text = "Round: " + �ĴX�^�X.ToString();
                //if (�ĴX�^�X > 30) isWon=true;
            }
            // �����ӱ�
            GameObject[] bb = GameObject.FindGameObjectsWithTag("BRICKS");
            �j���`�� = bb.Length;
            foreach (GameObject b2 in bb)
            {
                if (b2.name == "AddBall(Clone)") �j���`��--;
                if (b2.name == "Spread(Clone)") �j���`��--;
                if (b2.name == "-(Clone)") �j���`��--;
                if (b2.name == "+(Clone)") �j���`��--;
            }

            if (�j���`�� == 0)
            {
                // WIN
                isWon = true;
            }
        }

        if (isWon)
        {
            Shooter.SetActive(false);
            �ӱѤ�r.text = "WIN";
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
            �ӱѤ�r.text = "LOSE";
            BUT_BACK.gameObject.SetActive(true);
            isPlaying = false;
            return;
        }
    }
}
