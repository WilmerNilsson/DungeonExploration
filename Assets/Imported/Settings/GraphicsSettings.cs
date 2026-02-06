using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GraphicsSettings : MonoBehaviour
{
    [SerializeField] GameObject warningWindow;
    [SerializeField] TMP_Dropdown resolutionDropdown;
    [SerializeField] TMP_Dropdown screenmodeDropdown;
    [SerializeField] TMP_InputField framerateInputField;

    IUIController uIController;
    CanvasScaleFactorAdjuster canvasScaleFactorAdjuster;

    bool hasMadeChanges = false;

    Resolution oldResolution = new Resolution();
    Resolution newResolution = new Resolution();

    FullScreenMode oldFullScreenMode;
    FullScreenMode newFullScreenMode;

    private void Start()
    {
        uIController = GameObject.FindGameObjectWithTag("MainUI").GetComponent<IUIController>();
        canvasScaleFactorAdjuster = GameObject.FindGameObjectWithTag("MainUI").GetComponent<CanvasScaleFactorAdjuster>();

        ResetSettings();
    }

    void ResetSettings()
    {
        oldResolution.height = Screen.height;
        oldResolution.refreshRateRatio = Screen.currentResolution.refreshRateRatio;
        oldResolution.width = Screen.width;
        newResolution = oldResolution;

        oldFullScreenMode = Screen.fullScreenMode;
        newFullScreenMode = oldFullScreenMode;

        if(oldResolution.width % 640 == 0 && oldResolution.height % 360 == 0)
        {
            resolutionDropdown.SetValueWithoutNotify((oldResolution.height / 360) -1);
        }

        framerateInputField.SetTextWithoutNotify(((int) oldResolution.refreshRateRatio.value).ToString());

        if(newFullScreenMode == FullScreenMode.ExclusiveFullScreen)
        {
            screenmodeDropdown.SetValueWithoutNotify(0);
        }
        else if(newFullScreenMode == FullScreenMode.FullScreenWindow)
        {
            screenmodeDropdown.SetValueWithoutNotify(1);
        }
        else if(newFullScreenMode == FullScreenMode.MaximizedWindow)
        {
            screenmodeDropdown.SetValueWithoutNotify(2);
        }
        else if(newFullScreenMode == FullScreenMode.Windowed)
        {
            screenmodeDropdown.SetValueWithoutNotify(3);
        }
    }

    void HasMadeChangesEnable()
    {
        if(!hasMadeChanges)
        {
            hasMadeChanges = true;

            uIController.ChangeUseWarningScreen(true);
            uIController.OnChangeScreenAction += AbortChanges;
        }
    }

    public void AbortChanges()
    {
        if(hasMadeChanges)
        {
            ResetSettings();

            hasMadeChanges = false;

            uIController.ChangeUseWarningScreen(false);
            uIController.OnChangeScreenAction -= AbortChanges;
        }
    }

    void KeepChanges()
    {
        hasMadeChanges = false;

        oldResolution = newResolution;
        oldFullScreenMode = newFullScreenMode;

        uIController.ChangeUseWarningScreen(false);
        uIController.OnChangeScreenAction -= AbortChanges;
    }

    public void TryApplySettings()
    {
        Screen.SetResolution(newResolution.width, newResolution.height, newFullScreenMode);
        Application.targetFrameRate = (int) newResolution.refreshRateRatio.value;
        canvasScaleFactorAdjuster.AdjustScalingFactorFromInt(newResolution.height / 360);

        uIController.ChangeCanUnpause(false);
        warningWindow.SetActive(true);
    }

    public void WarningWindowAnswer(bool keepChanges)
    {
        if(!keepChanges)
        {
            Screen.SetResolution(oldResolution.width, oldResolution.height, oldFullScreenMode);
            Application.targetFrameRate = (int) oldResolution.refreshRateRatio.value;
            canvasScaleFactorAdjuster.AdjustScalingFactorFromInt(oldResolution.height / 360);
        }
        else
        {
            KeepChanges();
        }

        warningWindow.SetActive(false);
        uIController.ChangeCanUnpause(true);
    }

    public void ChangeResolutionByInt(int value)
    {
        newResolution.width = 640*(value+1);
        newResolution.height = 360*(value+1);

        HasMadeChangesEnable();
    }

    public void ChangePreferedRefreshRateByString(string value)
    {
        newResolution.refreshRateRatio = new RefreshRate {numerator = uint.Parse(value), denominator = 1 };

        HasMadeChangesEnable();
    }

    public void ChangeFullscreenModeByInt(int value)
    {
        if(value == 0)
        {
            newFullScreenMode = FullScreenMode.ExclusiveFullScreen;
        }
        else if(value == 1)
        {
            newFullScreenMode = FullScreenMode.FullScreenWindow;
        }
        else if(value == 2)
        {
            newFullScreenMode = FullScreenMode.MaximizedWindow;
        }
        else if(value == 3)
        {
            newFullScreenMode = FullScreenMode.Windowed;
        }

        HasMadeChangesEnable();
    }
}
