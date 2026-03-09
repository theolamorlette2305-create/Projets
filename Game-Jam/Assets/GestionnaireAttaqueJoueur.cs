using UnityEngine;

public class GestionnaireAttaqueJoueur : MonoBehaviour
{
    [Header("Références")]
    [Tooltip("Faites glisser ici le GameObject de votre épée.")]
    public GameObject epeeGameObject;

    [Header("Paramètres")]
    [Tooltip("Durée de l'activation de la hitbox en secondes.")]
    public float dureeAttaque = 0.4f;

    private Collider hitboxEpee;
    private LogiqueEpee logiqueEpee;

    void Start()
    {
        hitboxEpee = epeeGameObject.GetComponent<Collider>();
        logiqueEpee = epeeGameObject.GetComponent<LogiqueEpee>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Attaquer();
        }
    }

    private void Attaquer()
    {
        logiqueEpee.CommencerAttaque();
        
        hitboxEpee.enabled = true;

        Invoke("TerminerAttaque", dureeAttaque);
    }

    private void TerminerAttaque()
    {
        hitboxEpee.enabled = false;
    }
}