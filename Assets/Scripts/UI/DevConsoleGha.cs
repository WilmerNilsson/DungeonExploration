using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Unity.AI.Navigation;
using UnityEngine.AI;

public class DevConsoleGha : MonoBehaviour
{
    private class DebugCommand
    {
        public DebugCommand(string id, string description, string format, Action<string> command)
        {
            CommandId = id;
            CommandDescription = description;
            CommandFormat = format;

            _commandS = command;
        }

        public DebugCommand(string id, string description, string format, Action command)
        {
            CommandId = id;
            CommandDescription = description;
            CommandFormat = format;

            _command = command;
        }

        public readonly string CommandId;
        public readonly string CommandDescription;
        public readonly string CommandFormat;
#nullable enable
        private readonly Action<string?>? _commandS;
        private readonly Action? _command;

        public void Invoke(string? value = null)
        {
            if(_commandS != null)
            {
                _commandS.Invoke(value);
            }
            else
            {
                _command!.Invoke();
            }
        }
#nullable disable
    }

    [SerializeField] private TMP_Text infoTextWindow;
    [SerializeField] private TMP_InputField inputTextWindow;
    [SerializeField] private ItemLibrarySO itemLibrarySO;

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
            new DebugCommand("get_resolution", "shows resolution data", "get_resolution", GetResolutionCommand),
            new DebugCommand("teleport", "teleports the player", "teleport x y z", TeleportCommand),
            new DebugCommand("get_pos", "gets the player current possistion", "get_pos", GetPosCommand),
            new DebugCommand("log_path", "gets the debug log path of the application", "log_path", LogPathCommand),
            new DebugCommand("kill_player", "deals 1000 damage to player", "kill_player", KillPlayerCommand),
            new DebugCommand("debug_navmesh", "prints a lot of usefull nav mesh data", "debug_navmesh", DebugNavmeshCommand),
            new DebugCommand("set_sanity", "sets players sanity to value", "set_sanity int", SetSanity),
            new DebugCommand("give_item", "tries to insert a item into player inventory", "give_item itemID", GiveItemCommand),
            new DebugCommand("teleport_to", "tries to teleport player to object by name", "teleport_to name", TeleportToCommand),
            new DebugCommand("unlock_map", "tries to unlock the full map", "unlock_map", UnlockMap)
        };
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (infoTextWindow == null) Debug.LogWarning("info text window is null", this);
        if (inputTextWindow == null) Debug.LogWarning("input text window is null", this);
        if (itemLibrarySO == null) Debug.LogWarning("item library is null", this);
    }
#endif

    public void HandleInput(string input)
    {
        if(input == string.Empty || input == null)
        {
            return;
        }
        inputTextWindow.text = string.Empty;

        string[] properties = input.Split(' ', 2);

        int index = commandList.FindIndex(item => item.CommandId == properties[0]);

        if(index != -1)
        {
            commandList[index].Invoke(properties.Length == 2 ? properties[1] : null);
        }
        else
        {
            infoTextWindow.text += "No command with that id found, try \"help\"\n";
        }
    }

    public void ToggeDevConsole()
    {
        GameManagerSO.Instance.FreezeTime(!toggleObject.activeSelf);
        GameManagerSO.Instance.LockMouse(!toggleObject.activeSelf);
        toggleObject.SetActive(!toggleObject.activeSelf);
        GameManagerSO.DevConsoleActive = toggleObject.activeSelf;
    }

    public static DevConsoleGha GetInstance()
    {
        return Instance;
    }

    private void AddLine(string line)
    {
        infoTextWindow.text += line;
        infoTextWindow.text += "\n";
    }

    #region command methods 

#nullable enable

    private void TeleportToCommand(string input)
    {
        if (input == null || input == string.Empty)
        {
            AddLine("That command requires paramiters");
            return;
        }

        GameObject obj = GameObject.Find(input);

        if (obj == null)
        {
            AddLine("no object found by name: " + input);
            return;
        }

        GameObject.FindGameObjectWithTag("Player").transform.position = obj.transform.position;
        if (GameObject.FindGameObjectWithTag("Player").TryGetComponent<HumanoidMovement>(out HumanoidMovement humanoidMovement))
        {
            humanoidMovement.SupressMoveFrame();
        }
    }

    private void GiveItemCommand(string input)
    {
        if (input == null || input == string.Empty)
        {
            AddLine("That command requires paramiters");
            return;
        }


        ItemPairing? pair;
        if (itemLibrarySO == null)
        {
            AddLine("item library is null");
            return;
        }
        else if(!itemLibrarySO.TryGetItemPairByName(input, out pair))
        {
            AddLine($"item library had no item by id \"{input}\""); // chose to have quotation marks to highlight any blank spaces
            return;
        }

        if(InvMasterBase.Instance == null)
        {
            AddLine("there is no inventory master");
            return;
        }
        else if(!InvMasterBase.Instance.PlayerInventory.TryInsertItem(pair.UIPrefab.GetComponent<SimpleItem>(), true))
        {
            AddLine("failed to insert item into inventory");
            return;
        }

    }

    private void SetSanity(string input)
    {
        if (input == null)
        {
            AddLine("That command requires paramiters");
            return;
        }

        bool couldParse = int.TryParse(input, out int newValue);
        if (!couldParse)
        {
            AddLine($"could not parse {input} as a sanity value (should be a int)");
            return;
        }

        Sanity.Instance?.SetSanity(newValue);
    }

    private void DebugNavmeshCommand()
    {
        NavMeshSurface navMeshSurface = FindAnyObjectByType<NavMeshSurface>();

        NavMeshAgent[] navMeshAgents = FindObjectsByType<NavMeshAgent>(FindObjectsSortMode.InstanceID);
        
        AddLine("navMeshSurface.isActiveAndEnabled: " + navMeshSurface.isActiveAndEnabled);
        foreach(var agent in navMeshAgents)
        {
            AddLine("is on nav mesh? " + agent.isOnNavMesh);
        }
    }

    private void KillPlayerCommand()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<Health>().TakeDamage(1000);
    }
    
    private void UnlockMap()
    {
        FindFirstObjectByType<MinimapMaster>().UnlockFullMinimap();
    }

    private void HelpCommand(string input)
    {
        if(input == null)
        {
            foreach(DebugCommand command in commandList)
            {
                infoTextWindow.text += $"{command.CommandFormat} - {command.CommandDescription}\n";
            }
            infoTextWindow.text += "\n";
        }
        else
        {
            bool foundCommandId = false;
            foreach(DebugCommand command in commandList)
            {
                if(input == command.CommandId)
                {
                    foundCommandId = true;

                    infoTextWindow.text += $"\n{command.CommandFormat} - {command.CommandDescription}\n\n";
                }
            }
            if(!foundCommandId)
            {
                infoTextWindow.text += $"Failed to find the command \"{input}\", example format: help get_resolution\n";
            }
        }
    }

    private void SetResolutionCommand(string input)
    {
        if(input == null)
        {
            infoTextWindow.text += "That command requires paramiters\n";
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
            infoTextWindow.text += "Wrong number of paramiters\n";
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
            infoTextWindow.text += $"Failed to parse paramiter {failedParamiter}\n";
    }

    private void GetResolutionCommand()
    {
        infoTextWindow.text += $"{Screen.width} x {Screen.height}, {Screen.fullScreenMode}, {Screen.currentResolution.refreshRateRatio}\n\n"; 
    }

    private void LogPathCommand()
    {
        AddLine(Application.consoleLogPath);
    }

    private void TeleportCommand(string input)
    {
        if(input == null)
        {
            infoTextWindow.text += "That command requires paramiters\n";
            return;
        }

        string[] properties = input.Split(' ');
        int failedParamiter;
        bool success;

        Vector3 endLocation;

        if(properties.Length != 3)
        {
            infoTextWindow.text += "Wrong number of paramiters\n";
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

        success = float.TryParse(properties[2], out endLocation.z);
        if (!success)
        {
            failedParamiter = 3;
            goto Failed;
        }

        GameObject.FindGameObjectWithTag("Player").transform.position = endLocation;
        if (GameObject.FindGameObjectWithTag("Player").TryGetComponent<HumanoidMovement>(out HumanoidMovement humanoidMovement))
        {
            humanoidMovement.SupressMoveFrame();
        }

        return;

        Failed:
            infoTextWindow.text += $"Failed to parse paramiter {failedParamiter}\n";
    }

    private void GetPosCommand()
    {
        infoTextWindow.text += $"{GameObject.FindGameObjectWithTag("Player").transform.position}\n\n";
    }

    #endregion
}