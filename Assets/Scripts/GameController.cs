using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameController : MonoBehaviour
{
    // �o�g�������ܼ�
    private Ray cameraRay;
    private RaycastHit hit;
    [SerializeField] private LayerMask layerMask;
    private Vector3 shootDirection;
    private Quaternion rotation;

    [Header("�o�g���]�m")]
    public GameObject ballPrefab;
    public GameObject shootPoint;
    public LineRenderer lineRenderer;
    public float shootSpeed = 30f;
    public float shootInterval = 0.05f;
    public float ballSize = 0.25f;
    public float timeOut = 5f;

    [Header("�C�����A")]
    public bool isShooting = false;
    private bool isDown = false;
    public bool isWon = false;
    public bool isLost = false;
    public bool isPlaying = false;
    public bool levelHasLoadCompleted = false;

    [Header("UI����")]
    public Text gameStatusText;
    public Button nextButton;
    public Button backButton;
    public TextMeshPro ballCountText;
    public TextMeshPro roundText;

    [Header("�C���]�m")]
    public int nowLevel = 1;
    public int currentRound = 0;
    private int totalBricks = 0;
    public GameObject shooter;

    void Start()
    {
        InitializeGame();
    }

    void InitializeGame()
    {
        // ��l�Ƶo�g���]�m
        if (lineRenderer != null)
        {
            lineRenderer.enabled = false;
        }
        else
        {
            //Debug.LogError("LineRenderer not assigned!");
        }

        // ��l�ƹC�����A
        shooter.SetActive(false);
        isPlaying = false;
        isWon = false;
        isLost = false;

        // ��l��UI
        if (gameStatusText != null) gameStatusText.text = "";
        if (nextButton != null) nextButton.gameObject.SetActive(false);
        if (backButton != null) backButton.gameObject.SetActive(false);

        // �q�\�ƥ�
        processCSV.OnBricksGenerated += HandleBricksGenerated;

        // �}�l���J���d
        LoadGamePlay();
    }

    void LoadGamePlay()
    {
        if (nowLevel > 9)
        {
            nowLevel = 1;
            currentLevel._CurrentLevel = 1;
        }

        if (currentLevel._CurrentLevel > 0)
        {
            nowLevel = currentLevel._CurrentLevel;
            GameObject.Find("00GameMaster").GetComponent<processCSV>().getLevel(nowLevel);
            if (roundText != null) roundText.text = "LEVEL: " + nowLevel.ToString();
        }
        else 
        {
            GetComponent<���Ϳj��>().genBricks();
            if (shooter != null) shooter.SetActive(true);
            isPlaying = true;
            if (roundText != null) roundText.text = "LEVEL: Random";
            downOne();
        }

        UpdateBallCountText();
    }

    void Update()
    {
        HandleEscapeKey();
        if (!CanPlay()) return;

        HandleGameplayLogic();

        // �u���b�C���i�椤�B���O��Ӫ��A�ɤ~�B�z�g���޿�
        if (isPlaying && !isWon && !isShooting)
        {
            HandleShootingLogic();
        }
    }

    bool CanPlay()
    {
        //Level
        if (currentLevel._CurrentLevel > 0 && !levelHasLoadCompleted)
        {
            isPlaying = false;
            return false;
        }
        return true;
    }

    void HandleGameplayLogic()
    {
        if (isPlaying)
        {
            UpdateUI();
            CheckWinCondition();
        }

        HandleGameEndStates();
    }

    void HandleShootingLogic()
    {
        // ���ˬd�O�_���y�b���W
        HandleBallRecycling();

        // �T�O�S���y�b���W�ɤ~��o�g
        if (!isShooting)
        {
            // ����ṳ���g�u
            cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            // ����ƹ�����ɪ��˷��޿�
            if (Input.GetMouseButton(0))
            {
                if (lineRenderer != null)
                {
                    lineRenderer.enabled = true;
                    HandleAiming();
                }
            }
            // ����ƹ�����ɪ��o�g�޿�
            else if (Input.GetMouseButtonUp(0))
            {
                if (lineRenderer != null)
                {
                    lineRenderer.enabled = false;
                }
                StartShooting();
            }
        }
    }

    void HandleBallRecycling()
    {
        if (GameObject.FindGameObjectsWithTag("BALL").Length == 0 )
        {
            //Debug.Log("No balls found, triggering recycle");
            isShooting = false;
            GameObject.Find("/��/�U").GetComponent<�^���y>().���ʵo�g��();
            
            //var recycler = GameObject.Find("/��/�U")?.GetComponent<�^���y>();
            //if (recycler != null)
            //{
            //    Debug.Log("���ʵo�g��");
            //    recycler.���ʵo�g��();
            //}

            if (isDown)
            {
                //Debug.Log("Triggering downOne");
                downOne();
                isDown = false;
            }
        }
    }

    void HandleAiming()
    {
        if (Physics.Raycast(cameraRay, out hit, 35f, layerMask))
        {
            // �p��g����V
            Vector3 hitPoint = hit.point;
            hitPoint.y = 0.25f;
            shootDirection = (hitPoint - shootPoint.transform.position).normalized;

            // �p��g�����|
            Ray shootRay = new Ray(shootPoint.transform.position + shootDirection * ballSize, shootDirection);
            RaycastHit wallHit;

            if (Physics.SphereCast(shootRay, ballSize, out wallHit, 35f, layerMask))
            {
                Vector3 collisionPoint = wallHit.point;
                Vector3 reflectDirection = Vector3.Reflect(shootDirection, wallHit.normal).normalized;

                // �p��Ϯg���|
                Ray reflectedRay = new Ray(collisionPoint + reflectDirection * ballSize, reflectDirection);
                RaycastHit secondHit;
                Vector3 secondHitPoint = collisionPoint + reflectDirection * 20f;

                if (Physics.Raycast(reflectedRay, out secondHit, 20f, layerMask))
                {
                    secondHitPoint = secondHit.point;
                }

                // �]�mLineRenderer�����|�I
                lineRenderer.positionCount = 3;
                lineRenderer.SetPosition(0, shootPoint.transform.position);
                lineRenderer.SetPosition(1, collisionPoint);
                lineRenderer.SetPosition(2, secondHitPoint);
            }

            // ��s�o�g������
            UpdateShooterRotation();
        }
    }

    void UpdateShooterRotation()
    {
        rotation = Quaternion.LookRotation(shootDirection, Vector3.up);
        shooter.transform.rotation = Quaternion.Slerp(shooter.transform.rotation, rotation, 20 * Time.deltaTime);
        shooter.transform.eulerAngles = new Vector3(0f, shooter.transform.eulerAngles.y, 0f);
    }

    void StartShooting()
    {
        if (lineRenderer != null)
        {
            lineRenderer.enabled = false;
        }

        //Debug.Log("Starting shooting sequence");
        isShooting = true;
        isDown = true;
        StartCoroutine(ShootSequence());
        currentRound++;
    }

    IEnumerator ShootSequence()
    {
        
        //Debug.Log($"Shooting {currentLevel.balls} balls");
        for (int i = 0; i < currentLevel.balls; i++)
        {
            if (isWon)
            {
                isShooting = false;
                yield break;
            }
            CreateBall();
            yield return new WaitForSeconds(shootInterval);
        }
        isShooting = false;
        //Debug.Log("Finished shooting sequence");
    }

    void CreateBall()
    {
        if (ballPrefab == null || shootPoint == null)
        {
            //Debug.LogError("Ball prefab or shoot point not assigned!");
            return;
        }

        // �Ыزy��
        GameObject ball = Instantiate(ballPrefab, shootPoint.transform.position, Quaternion.identity);
        ball.transform.localScale = new Vector3(ballSize, ballSize, ballSize);

        // �]�m�y�骺��V�M�t��
        ball.transform.forward = shootDirection;
        ball.transform.eulerAngles = new Vector3(0f, transform.eulerAngles.y, 0f);

        Rigidbody rb = ball.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = shootDirection * shootSpeed;
        }

        // �]�m�P���ɶ�
        Destroy(ball, timeOut);
    }

    public void downOne()
    {
        //Debug.Log("Starting downOne sequence");
        StartCoroutine(DownOneCoroutine());
    }

    IEnumerator DownOneCoroutine()
    {
        GameObject[] bricks = GameObject.FindGameObjectsWithTag("BRICKS");
        //Debug.Log($"Found {bricks.Length} bricks to move down");

        List<Coroutine> movementCoroutines = new List<Coroutine>();

        foreach (GameObject brick in bricks)
        {
            if (brick != null)
            {
                Vector3 newPosition = brick.transform.position;
                newPosition.z -= 1;
                movementCoroutines.Add(StartCoroutine(MoveDownSequence(brick, newPosition)));

                if (newPosition.z < 0)
                {
                    if(brick.name == "AddBall(Clone)" || brick.name == "Spread(Clone)" || brick.name == "+(Clone)" || brick.name == "-(Clone)")
                    {
                        Destroy(brick);
                        //yield break;
                        //break;
                    }
                    else
                    {
                        isLost = true;
                    }                    
                    //Debug.Log("Game lost - brick passed bottom boundary");
                }

                var resource = brick.GetComponent<�귽>();
                if (resource != null && resource.�̫�n�R��)
                {
                    Destroy(brick);
                }
            }
        }

        foreach (Coroutine coroutine in movementCoroutines)
        {
            yield return coroutine;
        }

        if (currentLevel._CurrentLevel == 0 && currentRound < 31)
        {
            var brickGenerator = GameObject.Find("/00GameMaster")?.GetComponent<���Ϳj��>();
            if (brickGenerator != null)
            {
                //Debug.Log("Generating new bricks");
                brickGenerator.genBricks();
            }
        }

        //Debug.Log("DownOne sequence completed");
    }

    IEnumerator MoveDownSequence(GameObject obj, Vector3 targetPos)
    {
        if (obj == null) yield break;

        float duration = 0.5f;
        float elapsedTime = 0f;
        Vector3 startPosition = obj.transform.position;

        while (elapsedTime < duration && obj != null)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            obj.transform.position = Vector3.Lerp(startPosition, targetPos, t);
            yield return null;
        }

        if (obj != null)
        {
            obj.transform.position = targetPos;
        }
    }

    void UpdateUI()
    {
        UpdateBallCountText();
        if (currentLevel._CurrentLevel==0 && roundText != null)
        {
            roundText.text = "Round: " + currentRound.ToString();
        }
    }

    void UpdateBallCountText()
    {
        if (ballCountText != null)
        {
            ballCountText.text = "Ball: " + currentLevel.balls;
        }
    }

    void CheckWinCondition()
    {
        GameObject[] bricks = GameObject.FindGameObjectsWithTag("BRICKS");
        totalBricks = bricks.Length;

        foreach (GameObject brick in bricks)
        {
            if (brick.name == "AddBall(Clone)" ||
                brick.name == "Spread(Clone)" ||
                brick.name == "-(Clone)" ||
                brick.name == "+(Clone)")
            {
                totalBricks--;
            }
        }

        if (totalBricks == 0)
        {
            isWon = true;
        }
    }

    void HandleGameEndStates()
    {
        if (isWon)
        {
            HandleWinState();            
        }
        else if (isLost)
        {
            HandleLoseState();
        }
    }

    void HandleWinState()
    {
        if (shooter != null) shooter.SetActive(false);
        if (gameStatusText != null) gameStatusText.text = "WIN";
        if (nextButton != null) nextButton.gameObject.SetActive(true);
        if (backButton != null) backButton.gameObject.SetActive(true);
        isPlaying = false;

        DestroyAllBalls();
    }

    void HandleLoseState()
    {
        if (gameStatusText != null) gameStatusText.text = "LOSE";
        if (backButton != null) backButton.gameObject.SetActive(true);
        isPlaying = false;
    }

    void DestroyAllBalls()
    {
        GameObject[] balls = GameObject.FindGameObjectsWithTag("BALL");
        foreach (GameObject ball in balls)
        {
            Destroy(ball);
        }
    }

    void HandleEscapeKey()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            SceneManager.LoadScene("01_������d");
        }
    }

    private void HandleBricksGenerated()
    {
        StartCoroutine(setIsPlaying());
    }

    private IEnumerator setIsPlaying()
    {
        yield return new WaitForSeconds(1f);
        if (shooter != null) shooter.SetActive(true);
        isPlaying = true;
    }

    private void OnDestroy()
    {
        processCSV.OnBricksGenerated -= HandleBricksGenerated;
    }
}