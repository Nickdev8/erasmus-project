using UnityEngine;
using UnityEngine.SceneManagement;

public class Healthbar : MonoBehaviour
{
    public int Health = 5;
    [SerializeField] private GameObject[] Healthbars;

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < Healthbars.Length; i++)
        {
            if (i < Health)
            {
                Healthbars[i].SetActive(true);
            }
            else
            {
                Healthbars[i].SetActive(false);
            }
        }
        if (Health <= 0)
        {
            SceneManager.LoadScene("GameOver");
        }
    }
}
