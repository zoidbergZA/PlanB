using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    [SerializeField] private Image health;
    [SerializeField] private Text healthText;

    private RectTransform healthRect;

    public Vulnerable Target { get; set; }

    void Awake()
    {
        healthRect = health.GetComponent<RectTransform>();
    }
    
	void Update ()
	{
	    transform.rotation = Camera.main.transform.rotation;

	    if (Target)
	    {
	        float healthFrac = Target.Hitpoints/Target.MaxHP;
            
            health.color = Color.Lerp(Color.red, Color.green, healthFrac);
	        healthRect.localScale = new Vector3(healthFrac, 1, 1);

            healthText.text = Mathf.FloorToInt(Target.Hitpoints) + "/" + Target.MaxHP;
	    }
	}
}
