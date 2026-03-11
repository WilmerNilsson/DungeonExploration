using UnityEngine;

public class ShopCameraRotation : MonoBehaviour
{
    

   public GameObject Camera;
   public float RotationSpeed = 5f;
   public int CurrentIndex = 0;
   public float[] TheAngles = { 0, 90, 180, 270 };

   public GameObject SmitheryTxt;
   public GameObject DungeonTxt;
   public GameObject MerchantTxt;
   public GameObject ExitGameTxt;

   public GameObject LeftArrowImage;
   public GameObject RightArrowImage;
   
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

            /*if (Input.GetKey(KeyCode.E))
            {
                GameManagerSO.Instance.SavefileManager.SaveFromTown(false, "BuildingScene");
            }*/
        }
        else
        {
            DungeonTxt.SetActive(false);
        }

        if (CurrentIndex == 2)
        {
            MerchantTxt.SetActive(true);
        }
        else
        {
            MerchantTxt.SetActive(false);
        }
        
        if (CurrentIndex == 3)
        {
            ExitGameTxt.SetActive(true);
        }
        else
        {
            ExitGameTxt.SetActive(false);
        }
        
        
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            CurrentIndex = (CurrentIndex + 1) % TheAngles.Length;
            RightArrowImage.transform.localScale = new Vector3(0.8f, 0.8f, 1f);
            Invoke(nameof(ResetRightButton), 0.1f);
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            CurrentIndex = (CurrentIndex - 1 + TheAngles.Length) % TheAngles.Length;
            
            LeftArrowImage.transform.localScale = new Vector3(0.8f, 0.8f, 1f);
            Invoke(nameof(ResetLeftButton), 0.1f);

        }
        
        Quaternion LookYangle = Quaternion.Euler( 0, TheAngles[CurrentIndex], 0 );
        Camera.transform.rotation = Quaternion.Lerp( Camera.transform.rotation, LookYangle,
            RotationSpeed * Time.deltaTime);
        
        
    }
    public void ButtonLeft()
    {
        CurrentIndex = (CurrentIndex - 1 + TheAngles.Length) % TheAngles.Length;
        
        LeftArrowImage.transform.localScale = new Vector3(0.8f, 0.8f, 1f);
        Invoke(nameof(ResetLeftButton), 0.1f);
    }

    public void ButtonRight()
    {
        CurrentIndex = (CurrentIndex + 1) % TheAngles.Length;
        
        RightArrowImage.transform.localScale = new Vector3(0.8f, 0.8f, 1f);
        Invoke(nameof(ResetRightButton), 0.1f);
    }
    
    void ResetRightButton()
    {
        RightArrowImage.transform.localScale = Vector3.one;
    }
    void ResetLeftButton()
    {
        LeftArrowImage.transform.localScale = Vector3.one;
    }

}
