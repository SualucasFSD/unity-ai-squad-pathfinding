using UnityEngine;

public class GetPositionsCamera : MonoBehaviour
{
    public LayerMask LayerPoitns;
    public GameObject[] Points=new GameObject[2];
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit p))
            {
                if (p.collider.gameObject.layer ==7)
                {
                    Points[0].transform.position = new Vector3(p.point.x, 2f, p.point.z);
                    if (ManagerFinal.Instance.Coman[0] == null) { return; }
                    ManagerFinal.Instance.Coman[0].GetPath();
                }
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit p))
            {
                if (p.collider.gameObject.layer == 7)
                {
                    Points[1].transform.position = new Vector3(p.point.x, 2f, p.point.z);
                    if (ManagerFinal.Instance.Coman[1] == null) { return; }
                    ManagerFinal.Instance.Coman[1].GetPath();
                }
            }
        }
    }
}
