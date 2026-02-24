using UnityEngine;

public class ShopCameraRotation : MonoBehaviour
{
    

   public GameObject Camera;
   public float RotationSpeed = 5f;
   public int CurrentIndex = 0;
   public float[] TheAngles = { 0, 90, 180, 270 };

   public GameObject SmitheryTxt;
   public GameObject DungeonTxt;
    void Update()
    {
        if (CurrentIndex == 0)
        {
            SmitheryTxt.SetActive(true);
        }
        else
        {
            SmitheryTxt.SetActive(false);
        }
        
        if (CurrentIndex == 1)
        {
            DungeonTxt.SetActive(true);
        }
        else
        {
            DungeonTxt.SetActive(false);
        }
        
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            CurrentIndex = (CurrentIndex + 1) % TheAngles.Length;
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            CurrentIndex = (CurrentIndex - 1 + TheAngles.Length) % TheAngles.Length;

        }
        
        Quaternion LookYangle = Quaternion.Euler( 0, TheAngles[CurrentIndex], 0 );
        Camera.transform.rotation = Quaternion.Lerp( Camera.transform.rotation, LookYangle,
            RotationSpeed * Time.deltaTime);
        
        
    }
}
