using UnityEngine;

public class Poursuite : MonoBehaviour
{
    [Header("Paramètres de Comportement")]
    [Tooltip("La vitesse de déplacement de l'ennemi.")]
    [SerializeField] private float vitesse = 2f;
    [Tooltip("Les dégâts infligés par chaque attaque.")]
    [SerializeField] private int degats = 15;
    [Tooltip("La distance à laquelle l'ennemi s'arrête pour attaquer.")]
    [SerializeField] private float porteeAttaque = 2f;
    [Tooltip("Le nombre d'attaques par seconde.")]
    [SerializeField] private float cadenceAttaque = 1f;

    
    private GameObject Cible;
    private SystemedeSante santeCible;
    private Rigidbody rb;
    private float prochaineAttaquePossible = 0f;

    private bool aDetecteLeJoueur = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
       
        if (aDetecteLeJoueur && Cible != null)
        {
            Vector3 vecteurVersCible = Cible.transform.position - this.transform.position;
            float distance = vecteurVersCible.magnitude;

            if (distance > porteeAttaque)
            {
                Vector3 direction = vecteurVersCible.normalized;
                Vector3 targetPosition = rb.position + direction * vitesse * Time.deltaTime;
                rb.MovePosition(targetPosition);
            }

            else
            {
                rb.linearVelocity = Vector3.zero;
                if (Time.time >= prochaineAttaquePossible)
                {
                    Attaquer();
                }
            }
        }
    }

    
    private void OnTriggerEnter(Collider other)
    {
        if (aDetecteLeJoueur) return;

        if (other.CompareTag("Player"))
        {
            Debug.Log(gameObject.name + " a détecté le joueur ! La poursuite commence.");
            
            Cible = other.gameObject;
            santeCible = Cible.GetComponent<SystemedeSante>();
            aDetecteLeJoueur = true;
        }
    }

    private void Attaquer()
    {
        if (santeCible != null)
        {
            santeCible.TakeDamage(degats);
        }
        prochaineAttaquePossible = Time.time + 1f / cadenceAttaque;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, porteeAttaque);
    }
}