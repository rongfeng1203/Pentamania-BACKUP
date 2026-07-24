using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CrystalBallSolidReceiver : MonoBehaviour
{
    private CrystalBallObject ball;

    void Awake()
    {
        GetComponent<Collider>().isTrigger = true;
        ball = GetComponentInParent<CrystalBallObject>();
    }


    void OnTriggerEnter(Collider other)
    {
        PassableIngredientObject ingredientObject = other.GetComponent<PassableIngredientObject>();

        if (ingredientObject == null)
        {
            return;
        }

        ball.IngredientObjectEntered(ingredientObject);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PassableIngredientObject>() == null)
        {
            return;
        }

        ball.IngredientObjectExit();
    }
}