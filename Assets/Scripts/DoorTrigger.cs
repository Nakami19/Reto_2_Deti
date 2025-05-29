using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    public DoorMover doorMover; // Referencia al script de la puerta

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Objeto detectado: " + other.name); // Verificar quién entra en el trigger

        if (other.CompareTag("Player")) // Solo activa si el objeto tiene el tag "Player"
        {
            Debug.Log("¡El jugador ha activado el trigger!"); // Mensaje en consola
            doorMover.OpenDoor(); // Llama a la función de apertura de la puerta
        }
    }
}


