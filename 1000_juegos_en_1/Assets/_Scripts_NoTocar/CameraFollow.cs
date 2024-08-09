using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    PlayerController objetivo;
    Vector3 distancia;
    [HideInInspector] public float velocidad = 0f;

    void Awake()
    {
        objetivo = FindObjectOfType<PlayerController>();
        distancia = objetivo.transform.position - transform.position;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 posFinal = objetivo.transform.position;
        posFinal.z = -10;
        transform.position = Vector3.Lerp(transform.position, posFinal, velocidad);
    }
}
