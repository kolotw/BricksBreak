using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class 發射器 : MonoBehaviour
{
    Ray CameraRay;
    RaycastHit hit;
    [SerializeField] LayerMask layerMask;
    Vector3 方向;
    Quaternion 旋轉;
    public GameObject myBall;
    public GameObject 發射點;
    public LineRenderer line;
    public int 球數 = 10;
    public float 射速 = 30f;
    public float 間隔 = 0.05f;
    public float 球尺寸 = 0.25f;
    public float 逾時 = 5f;
    public bool 正在發射中 = false;
    bool isDown = false;
    public gameMaster gm;
    void Start()
    {
        line.GetComponent<LineRenderer>().enabled = false;
        gm = GameObject.Find("00GameMaster").GetComponent<gameMaster>();
    }

    void Update()
    {
        if(gm==null) {
            gm = GameObject.Find("00GameMaster").GetComponent<gameMaster>();
            return; 
        }
        if (!gm.isPlaying)
        {
            //GameObject.Find("/Canvas/filepath").GetComponent<Text>().text = "notPlaying";
            isDown = false;
            StopCoroutine(發射序列());
            return;
        }

        if (GameObject.FindGameObjectsWithTag("BALL").Length == 0)
        {
            正在發射中 = false;
            GameObject.Find("/牆/下").GetComponent<回收球>().移動發射器();
            if (isDown)
            {
                downOne();
                isDown = false;
            }
        }

        if (!正在發射中 && !gm.isWon)
        {
            CameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Input.GetMouseButton(0))
            {
                line.GetComponent<LineRenderer>().enabled = true;

                if (Physics.Raycast(CameraRay, out hit, 35f, layerMask))
                {
                    // 計算發射方向
                    Vector3 h = hit.point;
                    h.y = 0.25f;
                    方向 = (h - 發射點.transform.position).normalized;

                    // 從發射點發射射線，考慮球尺寸
                    Ray shootRay = new Ray(發射點.transform.position + 方向 * 球尺寸, 方向);
                    RaycastHit wallHit;
                    //Physics.SphereCast(shootRay, 球尺寸, out wallHit, 35f, layerMask)
                    if (Physics.SphereCast(shootRay, 球尺寸, out wallHit, 35f, layerMask))
                    {
                        // 計算實際碰撞點和反射方向
                        Vector3 碰撞點 = wallHit.point;
                        Vector3 反射方向 = Vector3.Reflect(方向, wallHit.normal).normalized;

                        // 計算第二次碰撞
                        Ray 反射射線 = new Ray(碰撞點 + 反射方向 * 球尺寸, 反射方向);
                        RaycastHit 第二次碰撞;
                        Vector3 第二碰撞點 = 碰撞點 + 反射方向 * 20f;

                        if (Physics.Raycast(反射射線, out 第二次碰撞, 20f, layerMask))
                        {
                            第二碰撞點 = 第二次碰撞.point;
                        }

                        // 設置 LineRenderer 的位置
                        line.positionCount = 3;

                        line.SetPosition(0, 發射點.transform.position);
                        line.SetPosition(1, 碰撞點);
                        line.SetPosition(2, 第二碰撞點);

                        // 繪製除錯射線
                        //Debug.DrawRay(發射點.transform.position, 方向 * wallHit.distance, Color.red);
                        //Debug.DrawRay(碰撞點, 反射方向 * 20f, Color.green);
                    }

                    // 更新發射器旋轉
                    旋轉 = Quaternion.LookRotation(方向, Vector3.up);
                    transform.rotation = Quaternion.Slerp(transform.rotation, 旋轉, 20 * Time.deltaTime);
                    transform.eulerAngles = new Vector3(0f, transform.eulerAngles.y, 0f);
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                line.GetComponent<LineRenderer>().enabled = false;
                正在發射中 = true;
                isDown = true;
                StartCoroutine(發射序列());
                gm.第幾回合++;
            }
        }
    }

    IEnumerator 發射序列()
    {
        for (int i = 0; i < currentLevel.balls; i++)
        {
            產生球();
            yield return new WaitForSeconds(間隔);
        }
    }

    void 產生球()
    {
        GameObject ball = Instantiate(myBall, 發射點.transform.position, Quaternion.identity);
        ball.transform.localScale = new Vector3(球尺寸, 球尺寸, 球尺寸);

        // 設置球的方向和速度
        ball.transform.forward = 方向;
        ball.transform.eulerAngles = new Vector3(0f, transform.eulerAngles.y, 0f);
        ball.GetComponent<Rigidbody>().velocity = 方向 * 射速;

        Destroy(ball, 逾時);
    }

    public void downOne()
    {
        StartCoroutine(DownOneCoroutine());
    }

    IEnumerator DownOneCoroutine()
    {
        GameObject[] go = GameObject.FindGameObjectsWithTag("BRICKS");
        Vector3 nPos = Vector3.zero;
        List<Coroutine> movementCoroutines = new List<Coroutine>();

        // 開始所有磚塊的移動
        foreach (GameObject bb in go)
        {
            nPos = bb.transform.position;
            nPos.z = nPos.z - 1;

            // 將每個磚塊的移動協程添加到列表中
            movementCoroutines.Add(StartCoroutine(依序下降(bb, nPos)));

            if (nPos.z < 0)
            {
                gm.isLost = true;
            }

            if (bb.gameObject.GetComponent<資源>() != null)
            {
                if (bb.gameObject.GetComponent<資源>().最後要刪除 == true)
                {
                    Destroy(bb.gameObject);
                }
            }
        }

        // 等待所有移動完成
        foreach (Coroutine coroutine in movementCoroutines)
        {
            yield return coroutine;
        }

        // 在所有磚塊移動完成後才產生新的磚塊
        if (SceneManager.GetActiveScene().name == "LV_Random")
        {
            if (gm.第幾回合 < 31)
                GameObject.Find("/00GameMaster").GetComponent<產生磚塊>().genBricks();
        }
    }

    IEnumerator 依序下降(GameObject a, Vector3 b)
    {
        if (a == null) yield break;

        float duration = 0.5f; // 移動持續時間
        float elapsedTime = 0f; // 經過的時間
        Vector3 startPosition = a.transform.position;

        while (elapsedTime < duration)
        {
            if (a == null) yield break;
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            a.transform.position = Vector3.Lerp(startPosition, b, t);
            yield return null;
        }

        if (a != null)
        {
            a.transform.position = b;
        }
    }
}