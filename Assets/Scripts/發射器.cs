using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class 發射器 : MonoBehaviour
{
    Ray CameraRay;
    RaycastHit hit;
    Ray turretRay;
    RaycastHit hitWall;

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

    bool canShoot = false;
    public bool 正在發射中 = false;
    bool isDown = false;

    // Start is called before the first frame update
    void Start()
    {
        line.GetComponent<LineRenderer>().enabled = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(GameObject.FindGameObjectsWithTag("BALL").Length == 0)
        {
            canShoot = true;
            正在發射中 = false;
            GameObject.Find("/牆/下").GetComponent<回收球>().移動發射器();
            if (isDown)
            {
                downOne();
                isDown = false;
            }
        }
        else { canShoot = false; }

        if (!正在發射中) 
        {
            CameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Input.GetMouseButton(0)) 
            {
                line.GetComponent<LineRenderer>().enabled = true;
                if (Physics.Raycast(CameraRay, out hit, 15f, layerMask))  //從Camera出發，取得 滑鼠 / 手指 觸碰的位置
                {
                    turretRay = new Ray(transform.position, transform.forward);
                    if(Physics.Raycast(turretRay, out hitWall, 15f, layerMask)) //從炮台出發，取得 碰到 牆 的位置 
                    {
                        方向 = hitWall.point - 發射點.transform.position;
                        Debug.DrawRay(發射點.transform.position, 方向, Color.red);
                        //畫反彈 / 反射 的線
                        Vector3 反射方向 = Vector3.Reflect(方向, hitWall.normal);
                        Debug.DrawRay(hitWall.point, 反射方向, Color.green);

                        //畫線
                        line.SetPosition(0,發射點.transform.position); 
                        line.SetPosition(1,hitWall.point);
                        line.SetPosition(2, hitWall.point + 反射方向.normalized * 20);
                    }

                }
                方向 = hit.point - 發射點.transform.position;
                旋轉 = Quaternion.LookRotation(方向, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, 旋轉, 20 * Time.deltaTime);
                this.transform.eulerAngles = new Vector3(0f,this.transform.eulerAngles.y,0f);
            }
            if (Input.GetMouseButtonUp(0))
            {
                line.GetComponent<LineRenderer>().enabled = false;
                正在發射中 = true;
                isDown = true;
                //發射球
                StartCoroutine(發射序列());
            }
        }        
        
    }
    IEnumerator 發射序列()
    {
        for (int i = 0; i < 球數; i++)
        {
            產生球();
            yield return new WaitForSeconds(間隔);
        }
    }
    void 產生球()
    {
        GameObject ball =  Instantiate(myBall, 發射點.transform.position, Quaternion.identity);
        ball.transform.localScale = new Vector3(球尺寸, 球尺寸, 球尺寸);
        ball.transform.LookAt(發射點.transform.position);
        ball.transform.eulerAngles = new Vector3(0f,this.transform.eulerAngles.y, 0f);
        ball.GetComponent<Rigidbody>().velocity = transform.forward * 射速;

        Destroy(ball, 逾時);
    }
    void downOne()
    {
        GameObject[] go = GameObject.FindGameObjectsWithTag("BRICKS");
        Vector3 nPos = Vector3.zero;
        foreach (GameObject bb in go)
        {
            nPos = bb.transform.position;
            nPos.z = nPos.z - 1;
            bb.transform.position = nPos;
        }
        GameObject.Find("/00GameMaster").GetComponent<產生磚塊>().genBricks();
    }
}
