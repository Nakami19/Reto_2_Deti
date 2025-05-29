using UnityEngine;

public class DoorMover : MonoBehaviour
{
    public Vector3 openPosition = new Vector3(14f, 2.9823f, 15.41f); // Posici�n final de la puerta
    private Vector3 closedPosition; // Posici�n inicial
    public float speed = 2f; // Velocidad de movimiento
    private bool isOpening = false;

    void Start()
    {
        openPosition = transform.position + new Vector3(4f, 0f, 0f);
    }

    void Update()
    {
        if (isOpening)
        {
            Vector3 newPosition = Vector3.MoveTowards(transform.position, openPosition, Time.deltaTime * speed);
            transform.position = newPosition;

            // Detener el movimiento cuando llegue a la posici�n final
            if (Vector3.Distance(transform.position, openPosition) < 0.01f)
            {
                isOpening = false;
            }
        }

    }

    public void OpenDoor()
    {
        Debug.Log("�La puerta se est� abriendo!"); // Imprimir mensaje en consola
        isOpening = true; // Activa el movimiento de la puerta
    }
}


