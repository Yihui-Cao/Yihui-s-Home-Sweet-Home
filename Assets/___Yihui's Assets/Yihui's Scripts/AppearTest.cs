using UnityEngine;

public class AppearTest : MonoBehaviour
{
    public RectTransform appearA1;
    public float appearA1height = 100, appearA1exaggerated = 5;
    public Transform appearA2;
    public float appearA2height = 100, appearA2exaggerated = 5;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            U.AppearA(appearA1, appearA1height, appearA1exaggerated);
            U.AppearA(appearA2, appearA2height, appearA2exaggerated);
        }
    }
}
