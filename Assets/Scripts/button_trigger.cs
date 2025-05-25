using System.Collections;
using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    //Objeto que tiene las plataformas
    public GameObject plataformas;


    private void OnTriggerEnter(Collider other)
    {
        //Si lo activa el jugador que aparezcan las plataformas
        if (other.CompareTag("Player")) 
        {
            StartCoroutine(mostrarPlaforma());
        }
    }

    IEnumerator mostrarPlaforma() {
        //Las plataformas aparecen una por una
        foreach (Transform plataforma in plataformas.transform)
        {
            //Activo la plataforma para que se muestre
            plataforma.gameObject.SetActive(true);
            // Espera entre cada plataforma
            yield return new WaitForSeconds(0.5f); 
        }
    }
}
