using UnityEngine;
using UnityEngine.UI;

public class BarredeSante : MonoBehaviour
{
    [SerializeField] private Slider Sliderdesante;          
    [SerializeField] private SystemedeSante Systemdesante;  
    [SerializeField] private float smoothSpeed = 5f;

    private float targetFillAmount = 1f;

    void Start()
    {
        if (Sliderdesante == null || Systemdesante == null) return;

        Sliderdesante.minValue = 0f;
        Sliderdesante.maxValue = 1f;
        Sliderdesante.value = 1f;

        float initialHealth = Systemdesante.ObtenirSanteNormalisee();
        targetFillAmount = Mathf.Clamp01(initialHealth);

        Systemdesante.OnchangedeSante += HandleHealthChanged;
    }

    void Update()
    {
        if (Sliderdesante != null)
        {
            Sliderdesante.value = Mathf.Lerp(Sliderdesante.value, targetFillAmount, Time.deltaTime * smoothSpeed);
        }
    }

    private void HandleHealthChanged(float normalizedHealth)
    {
        targetFillAmount = Mathf.Clamp01(normalizedHealth);
    }
}
