using UnityEngine;
using UnityEngine.SceneManagement; 

[RequireComponent(typeof(SystemedeSante))]
public class GestionMortJoueur : MonoBehaviour
{
    private SystemedeSante systemeSante;
    public Canvas deathCanvas; 

    void Awake()
    {
        systemeSante = GetComponent<SystemedeSante>();
    }

    private void OnEnable()
    {
        systemeSante.OnchangedeSante += VerifierMort;
    }

    private void OnDisable()
    {
        systemeSante.OnchangedeSante -= VerifierMort;
    }

    private void VerifierMort(float pourcentageSante)
    {
        if (systemeSante.IsDead)
        {
            Mourir();
        }
    }

    private void Mourir()
    {
        Debug.Log("Le joueur est mort ! GAME OVER.");

        systemeSante.OnchangedeSante -= VerifierMort;

        GetComponent<Mouvement>().enabled = false;
        deathCanvas.gameObject.SetActive(true);
        
        Destroy(gameObject, 5f);
    }

 

   
}