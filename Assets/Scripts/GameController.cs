using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameController : MonoBehaviour
{
    // 發射器相關變數
    private Ray cameraRay;
    private RaycastHit hit;
    [SerializeField] private LayerMask layerMask;
    private Vector3 shootDirection;
    private Quaternion rotation;

    [Header("發射器設置")]
    public GameObject ballPrefab;
    public GameObject shootPoint;
    public LineRenderer lineRenderer;
    public float shootSpeed = 30f;
    public float shootInterval = 0.05f;
    public float ballSize = 0.25f;
    public float timeOut = 5f;

    [Header("遊戲狀態")]
    public bool isShooting = false;
    private bool isDown = false;
    public bool isWon = false;
    public bool isLost = false;
    public bool isPlaying = false;
    public bool levelHasLoadCompleted = false;

    [Header("UI元素")]
    public Text gameStatusText;
    public Button nextButton;
    public Button backButton;
    public TextMeshPro ballCountText;
    public TextMeshPro roundText;

    [Header("遊戲設置")]
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
        // 初始化發射器設置
        if (lineRenderer != null)
        {
            lineRenderer.enabled = false;
        }
        else
        {
            //Debug.LogError("LineRenderer not assigned!");
        }

        // 初始化遊戲狀態
        shooter.SetActive(false);
        isPlaying = false;
        isWon = false;
        isLost = false;

        // 初始化UI
        if (gameStatusText != null) gameStatusText.text = "";
        if (nextButton != null) nextButton.gameObject.SetActive(false);
        if (backButton != null) backButton.gameObject.SetActive(false);

        // 訂閱事件
        processCSV.OnBricksGenerated += HandleBricksGenerated;

        // 開始載入關卡
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
            GetComponent<產生磚塊>().genBricks();
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

        // 只有在遊戲進行中且不是獲勝狀態時才處理射擊邏輯
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
        // 先檢查是否有球在場上
        HandleBallRecycling();

        // 確保沒有球在場上時才能發射
        if (!isShooting)
        {
            // 獲取攝像機射線
            cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            // 按住滑鼠左鍵時的瞄準邏輯
            if (Input.GetMouseButton(0))
            {
                if (lineRenderer != null)
                {
                    lineRenderer.enabled = true;
                    HandleAiming();
                }
            }
            // 釋放滑鼠左鍵時的發射邏輯
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
            GameObject.Find("/牆/下").GetComponent<回收球>().移動發射器();
            
            //var recycler = GameObject.Find("/牆/下")?.GetComponent<回收球>();
            //if (recycler != null)
            //{
            //    Debug.Log("移動發射器");
            //    recycler.移動發射器();
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
            // 計算射擊方向
            Vector3 hitPoint = hit.point;
            hitPoint.y = 0.25f;
            shootDirection = (hitPoint - shootPoint.transform.position).normalized;

            // 計算射擊路徑
            Ray shootRay = new Ray(shootPoint.transform.position + shootDirection * ballSize, shootDirection);
            RaycastHit wallHit;

            if (Physics.SphereCast(shootRay, ballSize, out wallHit, 35f, layerMask))
            {
                Vector3 collisionPoint = wallHit.point;
                Vector3 reflectDirection = Vector3.Reflect(shootDirection, wallHit.normal).normalized;

                // 計算反射路徑
                Ray reflectedRay = new Ray(collisionPoint + reflectDirection * ballSize, reflectDirection);
                RaycastHit secondHit;
                Vector3 secondHitPoint = collisionPoint + reflectDirection * 20f;

                if (Physics.Raycast(reflectedRay, out secondHit, 20f, layerMask))
                {
                    secondHitPoint = secondHit.point;
                }

                // 設置LineRenderer的路徑點
                lineRenderer.positionCount = 3;
                lineRenderer.SetPosition(0, shootPoint.transform.position);
                lineRenderer.SetPosition(1, collisionPoint);
                lineRenderer.SetPosition(2, secondHitPoint);
            }

            // 更新發射器旋轉
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

        // 創建球體
        GameObject ball = Instantiate(ballPrefab, shootPoint.transform.position, Quaternion.identity);
        ball.transform.localScale = new Vector3(ballSize, ballSize, ballSize);

        // 設置球體的方向和速度
        ball.transform.forward = shootDirection;
        ball.transform.eulerAngles = new Vector3(0f, transform.eulerAngles.y, 0f);

        Rigidbody rb = ball.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = shootDirection * shootSpeed;
        }

        // 設置銷毀時間
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

                var resource = brick.GetComponent<資源>();
                if (resource != null && resource.最後要刪除)
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
            var brickGenerator = GameObject.Find("/00GameMaster")?.GetComponent<產生磚塊>();
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
            SceneManager.LoadScene("01_選擇關卡");
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