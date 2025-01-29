using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    public GameObject laser;
    public Camera mainCamera;
    public Transform spawnLaser1;
    public Transform spawnLaser2;
    public float shootForse;
    public float spread = 0f;
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            Shoot();
    }

    private void Shoot() 
    {
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f,0));
        RaycastHit hit;
        Vector3 targetPoint;
        if(Physics.Raycast(ray,out hit)) 
        { 
            targetPoint = hit.point;
        }
        else 
        {
            targetPoint = ray.GetPoint(75);
        }

        Vector3 dirWithoutSpread = targetPoint - spawnLaser1.position;
        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);

        Vector3 dirWithSpread = dirWithoutSpread + new Vector3(x,y,0);

        GameObject currentlaser1 = Instantiate(laser, spawnLaser1.position, Quaternion.identity);
        currentlaser1.transform.forward = dirWithSpread.normalized; 
        currentlaser1.GetComponent<Rigidbody>().AddForce(dirWithSpread.normalized * shootForse, ForceMode.Impulse);
        
        GameObject currentlaser2 = Instantiate(laser, spawnLaser2.position, Quaternion.identity);
        currentlaser2.transform.forward = dirWithSpread.normalized;
        currentlaser2.GetComponent<Rigidbody>().AddForce(dirWithSpread.normalized * shootForse, ForceMode.Impulse);

    }
}
