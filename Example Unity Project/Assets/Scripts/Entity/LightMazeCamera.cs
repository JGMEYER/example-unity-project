using UnityEngine;
using System.Collections;

public class LightMazeCamera : MonoBehaviour
{

    [SerializeField]
    private LightMazeMap map;
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = GetComponent<Camera>();
    }

    private void Update()
    {
        Vector3 cameraPos = mainCamera.transform.position;
        mainCamera.transform.position = new Vector3((float)map.MapWidth / 2 - 0.5f, cameraPos.y, cameraPos.z);
    }

}
