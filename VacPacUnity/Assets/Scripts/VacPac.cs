using UnityEngine;

public class VacPac : MonoBehaviour
{
    public ParticleSystem slimeAmmo;
    bool shouldSlime;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            shouldSlime = true;
        }
        else
        {
            shouldSlime = false;
        }
    }

    private void FixedUpdate()
    {
        if (shouldSlime)
        {
            slimeAmmo.Emit(1);
        }
    }
}
