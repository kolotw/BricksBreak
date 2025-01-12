using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class 資源 : MonoBehaviour
{
    public GameObject 縱向刪除;
    public GameObject 橫向刪除;
    public bool 最後要刪除 = false;
    // Start is called before the first frame update
    void Start()
    {
        if(this.name == "-(Clone)")
        {
            this.gameObject.GetComponent<CapsuleCollider>().enabled = false;
        }
        else if (this.name == "+(Clone)")
        {
            this.gameObject.GetComponent<CapsuleCollider>().enabled = false;
        }
        else if (this.name == "Spread(Clone)")
        {
            this.gameObject.GetComponent<CapsuleCollider>().enabled = false;
        }
    }

    
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
                nPos.z = 15;
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
        }
    }
}
