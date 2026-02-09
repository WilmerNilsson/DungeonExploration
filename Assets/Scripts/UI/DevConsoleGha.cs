using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DevConsoleGha : MonoBehaviour
{
    private class DebugCommand
    {
        public DebugCommand(string id, string description, string format, Action<string> command)
        {
            _commandId = id;
            _commandDescription = description;
            _commandFormat = format;

            _commandS = command;
        }

        public DebugCommand(string id, string description, string format, Action command)
        {
            _commandId = id;
            _commandDescription = description;
            _commandFormat = format;

            _command = command;
        }

        public string _commandId { get; }
        public string _commandDescription { get; }
        public string _commandFormat { get; }
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
        private Action<string>? _commandS;
        private Action? _command;
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.

        public void Invoke(string value = null)
        {
            if(_commandS != null)
            {
                _commandS.Invoke(value);
            }
            else
            {
                _command.Invoke();
            }
        }
    }

    [SerializeField] TMP_Text _infoTextWindow;
    [SerializeField] TMP_InputField _inputTextWindow;

    private GameObject toggleObject;
    public static DevConsoleGha Instance {  get; private set; }
    private List<DebugCommand> commandList;

    //GameManagerSO gameManager;

    private void Awake()
    {
        Instance = this;
        toggleObject = transform.GetChild(0).gameObject;

        //gameManager = GameManagerSO.GetGameManagerSOInstance();

        commandList = new List<DebugCommand>
        {
            new DebugCommand("help", "Shows a list of commands. Or shows info about a command", "help (command)", HelpCommand),
            new DebugCommand("get_resolution", "shows resolution data", "get_resolution", GetResolutionCommand)
        };
    }

    public void HandleInput(string input)
    {
        if(input == string.Empty || input == null)
        {
            return;
        }
        _inputTextWindow.text = string.Empty;

        string[] properties = input.Split(' ', 2);

        int index = commandList.FindIndex(item => item._commandId == properties[0]);

        if(index != -1)
        {
            commandList[index].Invoke(properties.Length == 2 ? properties[1] : null);
        }
        else
        {
            _infoTextWindow.text += "No command with that id found, try \"help\"\n";
        }
    }

    public void ToggeDevConsole()
    {
        GameManagerSO.Instance.FreezeTime(!toggleObject.activeSelf);
        GameManagerSO.Instance.LockMouse(!toggleObject.activeSelf);
        toggleObject.SetActive(!toggleObject.activeSelf);
    }

    public static DevConsoleGha GetInstance()
    {
        return Instance;
    }

    /* #region command methods */

    private void HelpCommand(string input)
    {
        if(input == null)
        {
            foreach(DebugCommand command in commandList)
            {
                _infoTextWindow.text += $"{command._commandFormat} - {command._commandDescription}\n";
            }
            _infoTextWindow.text += "\n";
        }
        else
        {
            bool foundCommandId = false;
            foreach(DebugCommand command in commandList)
            {
                if(input == command._commandId)
                {
                    foundCommandId = true;

                    _infoTextWindow.text += $"\n{command._commandFormat} - {command._commandDescription}\n\n";
                }
            }
            if(!foundCommandId)
            {
                _infoTextWindow.text += $"Failed to find the command \"{input}\", example format: help get_resolution\n";
            }
        }
    }

    private void SetResolutionCommand(string input)
    {
        if(input == null)
        {
            _infoTextWindow.text += "That command requires paramiters\n";
            return;
        }

        string[] properties = input.Split(" ");
        int failedParamiter;
        bool success = false;

        int width;
        int height;
        object fullScreenMode;
        uint refreshRate;

        if(properties.Length != 4)
        {
            _infoTextWindow.text += "Wrong number of paramiters\n";
            return;
        }

        success = int.TryParse(properties[0], out width);
        if(!success)
        {
            failedParamiter = 1;
            goto Failed;
        }

        success = int.TryParse(properties[1], out height);
        if(!success)
        {
            failedParamiter = 2;
            goto Failed;
        }

        success = System.Enum.TryParse(typeof(FullScreenMode), properties[2], out fullScreenMode);
        if(!success)
        {
            failedParamiter = 3;
            goto Failed;
        }

        success = uint.TryParse(properties[3], out refreshRate);
        if(!success)
        {
            failedParamiter = 4;
            goto Failed;
        }

        Screen.SetResolution(width, height, (FullScreenMode) fullScreenMode, new RefreshRate{numerator = refreshRate, denominator = 1});
        return;

        Failed:
            _infoTextWindow.text += $"Failed to parse paramiter {failedParamiter}\n";
    }

    private void GetResolutionCommand()
    {
        _infoTextWindow.text += $"{Screen.width} x {Screen.height}, {Screen.fullScreenMode}, {Screen.currentResolution.refreshRateRatio}\n\n"; 
    }

    private void TeleportCommand(string input)
    {
        if(input == null)
        {
            _infoTextWindow.text += "That command requires paramiters\n";
            return;
        }

        string[] properties = input.Split(' ');
        int failedParamiter;
        bool success;

        Vector2 endLocation;

        if(properties.Length != 2)
        {
            _infoTextWindow.text += "Wrong number of paramiters\n";
            return;
        }
        
        success = float.TryParse(properties[0], out endLocation.x);
        if(!success)
        {
            failedParamiter = 1;
            goto Failed;
        }

        success = float.TryParse(properties[1], out endLocation.y);
        if(!success)
        {
            failedParamiter = 2;
            goto Failed;
        }

        GameObject.FindGameObjectWithTag("Player").transform.position = endLocation;
        return;

        Failed:
            _infoTextWindow.text += $"Failed to parse paramiter {failedParamiter}\n";
    }

    private void GetPosCommand()
    {
        _infoTextWindow.text += $"{(Vector2) GameObject.FindGameObjectWithTag("Player").transform.position}\n\n";
    }

    /* #endregion */
}