using UnityEngine;

public class CameraSuivi : MonoBehaviour
{
    [Tooltip("La cible que la caméra doit suivre (le joueur).")]
    public Transform cible;

    [Tooltip("La vitesse à laquelle la caméra rattrape la cible. Plus la valeur est élevée, plus le suivi est rigide.")]
    [Range(0.1f, 10f)]
    public float vitesseSuivi = 5f;

    
    private Vector3 decalage;

    
    void Start()
    {
       
        if (cible == null)
        {
            Debug.LogError("Aucune cible n'est assignée au script de suivi de la caméra !");
            return;
        }
        decalage = transform.position - cible.position;
    }

   
    void LateUpdate()
    {
        if (cible == null) return;

        Vector3 positionCible = cible.position + decalage;

        transform.position = Vector3.Lerp(transform.position, positionCible, vitesseSuivi * Time.deltaTime);
    }
}