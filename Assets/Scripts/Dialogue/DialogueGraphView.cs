using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogueGraphView : GraphView
{
    private readonly Vector2 defaultNodeSize = new Vector2(150, 200);
    
    public DialogueGraphView()
    {
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
        
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        var grid = new GridBackground();
        Insert(0, grid);
        grid.StretchToParentSize();
        
        AddElement(GenerateEntryPointNode());
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        var compatiblePorts = new List<Port>();
        ports.ForEach((port) =>
        {
            if (startPort != port && startPort.node != port.node)
            {
                compatiblePorts.Add(port);
            }
        });
        
        return compatiblePorts;
    }

    private Port GeneratePort(NewDialogueNode node, Direction portDirection,
        Port.Capacity capacity = Port.Capacity.Single)
    {
        return node.InstantiatePort(Orientation.Horizontal, portDirection, capacity, typeof(float)); //Arbitrary type
    }

    private NewDialogueNode GenerateEntryPointNode()
    {
        var node = new NewDialogueNode
        {
            title = "START",
            GUID = Guid.NewGuid().ToString(),
            DialogueText = "ENTRYPOINT",
            EntryPoint = true
        };
        
        var generatedPort = GeneratePort(node, Direction.Output);
        generatedPort.portName = "Next";
        node.outputContainer.Add(generatedPort);
        
        node.RefreshExpandedState();
        node.RefreshPorts();
        
        node.SetPosition(new Rect(100, 200, 100, 150));
        return node;
    }

    public void CreateNode(string nodeName)
    {
        AddElement(CreateDialogueNode(nodeName));
    }
    
    public NewDialogueNode CreateDialogueNode(string nodeName)
    {
        var dialogueNode = new NewDialogueNode
        {
            title = nodeName,
            DialogueText = nodeName,
            GUID = Guid.NewGuid().ToString()
        };

        var button = new Button(() => {AddChoicePort(dialogueNode);});
        button.text = "New Choice";
        dialogueNode.titleContainer.Add(button);

        var inputPort = GeneratePort(dialogueNode, Direction.Input, Port.Capacity.Multi);
        inputPort.portName = "Input";
        dialogueNode.inputContainer.Add(inputPort);
        dialogueNode.RefreshExpandedState();
        dialogueNode.RefreshPorts();
        dialogueNode.SetPosition(new Rect(Vector2.zero, defaultNodeSize));
        
        return dialogueNode;
    }

    private void AddChoicePort(NewDialogueNode dialogueNode)
    {
        var generatedPort = GeneratePort(dialogueNode, Direction.Output);

        var outputPortCount = dialogueNode.outputContainer.Query("connector").ToList().Count;
        var outputName = $"Choice {outputPortCount}";
        
        dialogueNode.outputContainer.Add(generatedPort);
        dialogueNode.RefreshPorts();
        dialogueNode.RefreshExpandedState();
    }
}
