using UnityEngine;

namespace VarjoExample
{
    public class Shoot : MonoBehaviour
    {
        public float energyFactor;
        public GameObject projectilePrefab;
        public Transform projectileOrigin;

        bool buttonDown;
        float energy;
        Controller controller;
        Rigidbody rb;
        GameObject projectile;

        void Start()
        {
            controller = GetComponent<Controller>();
        }

        void Update()
        {
            if (controller.Primary2DAxisClick)
            {
                if (!buttonDown)
                {
                    // Button is pressed, projectile is created
                    buttonDown = true;
                    projectile = Instantiate(projectilePrefab, projectileOrigin.transform.position, projectileOrigin.transform.rotation);
                    rb = projectile.GetComponent<Rigidbody>();
                    rb.isKinematic = true;
                    projectile.transform.parent = projectileOrigin;
                }
                else
                {
                    // Button is held down, projectile gets energy
                    energy = energy + Time.deltaTime * energyFactor;
                }
            }
            else if (!controller.Primary2DAxisClick && buttonDown)
            {
                // Button is released, projectile is released
                if (projectile && rb)
                {
                    rb.isKinematic = false;
                    projectile.transform.parent = null;
                    rb.AddForce(projectileOrigin.transform.forward * energy, ForceMode.Impulse);
                }
                buttonDown = false;
                energy = 0f;
            }
        }
    }
}