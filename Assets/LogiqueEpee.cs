using UnityEngine;
using System.Collections.Generic; 

public class LogiqueEpee : MonoBehaviour
{
    [Header("Paramètres de l'Attaque")]
    public float forceRecul = 5f;

    private StatistiquesJoueur statsJoueur;
    private List<Collider> ennemisTouches;

    void Awake()
    {
        statsJoueur = GetComponentInParent<StatistiquesJoueur>();
        ennemisTouches = new List<Collider>();
    }

    public void CommencerAttaque()
    {
        ennemisTouches.Clear();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (ennemisTouches.Contains(other))
        {
            return;
        }

        SystemedeSante sante = other.GetComponent<SystemedeSante>();
        if (sante != null)
        {
            sante.TakeDamage(statsJoueur.CurrentDamage);

            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 directionRecul = (other.transform.position - transform.root.position).normalized;
                rb.AddForce(directionRecul * forceRecul, ForceMode.Impulse);
            }

            ennemisTouches.Add(other);
        }
    }
}