using UnityEngine;


namespace VarjoExample
{
    public class Shoot : MonoBehaviour
    {
        Controller controller;
        bool buttonDown;
        float energy;
        public float energyFactor;
        public GameObject projectile;
        public Transform projectileOrigin;
        Rigidbody rb;
        GameObject bullet;

        void Start()
        {
            controller = GetComponent<Controller>();
        }

        void Update()
        {
            if (controller.Primary2DAxisClick)
            {
                if (!buttonDown) //Button is pressed, projectile is visible
                {
                    buttonDown = true;
                    bullet = Instantiate(projectile, projectileOrigin.transform.position, projectileOrigin.transform.rotation);
                    rb = bullet.GetComponent<Rigidbody>();
                    rb.isKinematic = true;
                    bullet.transform.parent = projectileOrigin;
                }
                else // button is held down, projectile gets energy
                {
                    energy = energy + Time.deltaTime * energyFactor;
                }
            }
            else if (!controller.Primary2DAxisClick && buttonDown) // Button is released, projectile is released
            {
                buttonDown = false;
                rb.isKinematic = false;
                bullet.transform.parent = null;
                rb.AddForce(projectileOrigin.transform.forward * energy, ForceMode.Impulse);
                energy = 0f;

            }
        }
    }
}