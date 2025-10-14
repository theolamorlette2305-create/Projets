using UnityEngine;

[RequireComponent(typeof(SystemedeSante))]
public class Ennemi : MonoBehaviour
{
    private StatistiquesJoueur statsJoueur;
    private SystemedeSante systemeSante; 

    void Awake()
    {
        systemeSante = GetComponent<SystemedeSante>();
        statsJoueur = FindObjectOfType<StatistiquesJoueur>();
    }

    private void OnEnable()
    {
        Debug.Log(gameObject.name + " s'est abonné à l'événement de santé.");
        systemeSante.OnchangedeSante += VerifierEtat;
    }

    private void OnDisable()
    {
        systemeSante.OnchangedeSante -= VerifierEtat;
    }
    
    private void VerifierEtat(float pourcentageSante)
    {
        Debug.Log(gameObject.name + " a reçu un événement de santé. Mort ? " + systemeSante.IsDead);
        if (systemeSante.IsDead)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log(gameObject.name + " a été vaincu !");

        if (statsJoueur != null)
        {
            statsJoueur.IncreaseDamage();
            SystemedeSante santeJoueur = statsJoueur.GetComponent<SystemedeSante>();

            if (santeJoueur != null){
                santeJoueur.IncreaseHealth();
            }
        }

        systemeSante.OnchangedeSante -= VerifierEtat;

        Destroy(gameObject);
    }
}