using UnityEngine;
using System.Collections;

public class DoorMover : MonoBehaviour
{
    private Vector3 openPosition; // Posición final de la puerta
    private Vector3 closedPosition; // Posición inicial
    public float speed = 2f; // Velocidad de movimiento
    private bool isOpening = false;
    private bool isClosing = false;

    void Start()
    {
        //Hacer que se mueve hasta 10 unidades 
        openPosition = transform.position + new Vector3(10f, 0f, 0f);

        closedPosition = transform.position;
    }

    void Update()
    {
        if (isOpening)
        {
            Vector3 newPosition = Vector3.MoveTowards(transform.position, openPosition, Time.deltaTime * speed); 
            transform.position = newPosition;

            // Detener el movimiento cuando llegue a la posición final
            if (Vector3.Distance(transform.position, openPosition) < 0.01f)
            {
                isOpening = false;
                StartCoroutine(CloseDoorAfterDelay(1f));

            }
        }
        else if (isClosing)
        {
            //Regresar la puerta a su posicion inicial
            transform.position = Vector3.MoveTowards(transform.position, closedPosition, Time.deltaTime * speed);

            if (Vector3.Distance(transform.position, closedPosition) < 0.01f)
            {
                isClosing = false;
            }
        }

    }

    public void OpenDoor()
    {
        Debug.Log("¡La puerta se esta abriendo!"); // Imprimir mensaje en consola
        isOpening = true; // Activa el movimiento de la puerta
    }

    IEnumerator CloseDoorAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Debug.Log("¡La puerta se está cerrando!");
        isClosing = true;
    }
}


