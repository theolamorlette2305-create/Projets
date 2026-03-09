using UnityEngine;
using TMPro;
using System.Collections;

public class StatistiquesJoueur : MonoBehaviour
{
    [Header("Statistiques de Combat")]
    [SerializeField] private int baseDamage = 10;
    [SerializeField] private int damageIncreasePerKill = 5;
    private int bonusDegatsEquipement = 0;

    [Header("Références UI")]
    [SerializeField] private TextMeshProUGUI notificationText;
    [SerializeField] private float notificationDuration = 2.5f;

    private CanvasGroup notifGroup;

    public int CurrentDamage => baseDamage + bonusDegatsEquipement;

    void Start()
    {
        if (notificationText != null)
        {
            // On garde le texte toujours actif mais invisible
            notifGroup = notificationText.GetComponent<CanvasGroup>();
            if (notifGroup == null)
                notifGroup = notificationText.gameObject.AddComponent<CanvasGroup>();
            
            notifGroup.alpha = 0f;
        }
    }

    public void IncreaseDamage()
    {
        baseDamage += damageIncreasePerKill;
        Debug.Log($"PUISSANCE AUGMENTÉE ! Dégâts actuels : {CurrentDamage}");

        if (notificationText != null)
            StartCoroutine(ShowDamageNotification());
    }

    private IEnumerator ShowDamageNotification()
    {
        notificationText.text = "DÉGÂTS AMÉLIORÉS !";

        float fadeDuration = 0.4f;

        // Fade-in
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            notifGroup.alpha = Mathf.Lerp(0, 1, t / fadeDuration);
            yield return null;
        }
        notifGroup.alpha = 1;

        // Temps d’affichage
        yield return new WaitForSeconds(notificationDuration);

        // Fade-out
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            notifGroup.alpha = Mathf.Lerp(1, 0, t / fadeDuration);
            yield return null;
        }
        notifGroup.alpha = 0;
    }
}
