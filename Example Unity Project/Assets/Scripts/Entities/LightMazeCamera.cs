using UnityEngine;
using System.Collections;

public class LightMazeCamera : MonoBehaviour {

    [SerializeField]
    private readonly LightMazeMap _map;
    private Camera _camera;

    void Start() {
        _camera = GetComponent<Camera>();
	}

    void Update() {
        Vector3 cameraPos = _camera.transform.position;
        _camera.transform.position = new Vector3((float)_map.mapWidth / 2 - 0.5f, cameraPos.y, cameraPos.z);
	}
}
