using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
  資源 磚塊 的互動
  - 橫向全刪
  + 直橫全刪
  Spread 改變球的方向
  AddBall 加球
 */
public class 資源 : MonoBehaviour
{
    public GameObject 縱向刪除;
    public GameObject 橫向刪除;
    public bool 最後要刪除 = false; 
    
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "BALL") 
        {            
            if(this.transform.name== "-(Clone)")
            {
                GameObject go = Instantiate(橫向刪除, this.transform.position, Quaternion.identity);
                Destroy(go, 0.05f);
                最後要刪除 = true;
            }
            else if (this.transform.name == "+(Clone)")
            {
                Vector3 nPos = this.transform.position;
                nPos.z = 14.9f;
                GameObject go = Instantiate(橫向刪除, this.transform.position, Quaternion.identity);
                GameObject ga = Instantiate(縱向刪除, nPos, Quaternion.identity);

                Destroy(go, 0.05f);
                Destroy(ga, 0.05f);
                最後要刪除 = true;
            }
            else if (this.transform.name == "Spread(Clone)")
            {
                // 分散方向
                int 角度 = Random.Range(0, 359);
                other.transform.eulerAngles = new Vector3(other.transform.eulerAngles.x,
                    角度,
                other.transform.eulerAngles.z);
                other.gameObject.GetComponent<Rigidbody>().velocity = other.transform.forward * 20;
                最後要刪除 = true;
            }
            else if(this.transform.name == "AddBall(Clone)")
            {
                if (other.transform.name == "Cube_ball_VertP(Clone)" || other.transform.name == "Cube_ball(Clone)")
                {
                    return;
                }
                currentLevel.balls++;
                Destroy(this.gameObject);
                最後要刪除 = true;
            }
        }
    }
}
