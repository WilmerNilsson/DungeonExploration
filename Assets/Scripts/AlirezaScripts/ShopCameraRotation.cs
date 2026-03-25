using System;
using UnityEngine;

public class ShopCameraRotation : MonoBehaviour
{
   public GameObject Camera;
   public float RotationSpeed = 5f;
   public int CurrentIndex = 0;
   public float[] TheAngles = { 0, 90, 180};

   [SerializeField] private GameObject[] townButtons;
   
   public GameObject LeftArrowImage;
   public GameObject RightArrowImage;

   private bool canRotate = true;


   private void Update()
   {
       Camera.transform.rotation = Quaternion.Lerp(Camera.transform.rotation, Quaternion.Euler( 0, TheAngles[CurrentIndex%TheAngles.Length], 0 ),RotationSpeed * Time.deltaTime);
   }

   public void ButtonLeft()
    {
        if (canRotate)
        {
            if (CurrentIndex == 0)
            {
                CurrentIndex = townButtons.Length-1;
            }
            else
            {
                CurrentIndex--;
            }
            
            ActivateButton(CurrentIndex);
        
            LeftArrowImage.transform.localScale = new Vector3(0.8f, 0.8f, 1f);
            Invoke(nameof(ResetLeftButton), 0.1f);
        }
    }

    public void ButtonRight()
    {
        if (canRotate)
        {
            CurrentIndex++;
            
            ActivateButton(CurrentIndex);
                    
            RightArrowImage.transform.localScale = new Vector3(0.8f, 0.8f, 1f);
            Invoke(nameof(ResetRightButton), 0.1f);
        }
    }

    private void ActivateButton(int index)
    {
        foreach (var button in townButtons)
        {
            button.SetActive(false);
        }
        townButtons[index%townButtons.Length].SetActive(true);
    }
    
    void ResetRightButton()
    {
        RightArrowImage.transform.localScale = Vector3.one;
    }
    void ResetLeftButton()
    {
        LeftArrowImage.transform.localScale = Vector3.one;
    }

    public void SetMovement(bool value)
    {
        canRotate = value;
    }


}
