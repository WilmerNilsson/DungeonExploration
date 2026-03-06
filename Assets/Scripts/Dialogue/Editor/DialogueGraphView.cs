using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogueGraphView : GraphView
{
    public readonly Vector2 DefaultNodeSize = new Vector2(150, 200);

    public Blackboard Blackboard;
    public List<ExposedProperty> ExposedProperties = new List<ExposedProperty>();
    private NodeSearchWindow _searchWindow;
    
    public DialogueGraphView(EditorWindow editorWindow)
    {
        styleSheets.Add(Resources.Load<StyleSheet>("DialogueGraphEditor"));
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
        
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        var grid = new GridBackground();
        Insert(0, grid);
        grid.StretchToParentSize();
        
        AddElement(GenerateEntryPointNode());
        AddSearchWindow(editorWindow);
    }

    private void AddSearchWindow(EditorWindow editorWindow)
    {
        _searchWindow = ScriptableObject.CreateInstance<NodeSearchWindow>();
        _searchWindow.Init(editorWindow, this);
        nodeCreationRequest = context =>
            SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), _searchWindow);
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        var compatiblePorts = new List<Port>();
        ports.ForEach((port) =>
        {
            if (startPort != port && startPort.node != port.node && startPort.direction != port.direction)
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
            ButtonText = "ENTRYPOINT",
            EntryPoint = true,
            HasBeenRead = true
        };
        
        var generatedPort = GeneratePort(node, Direction.Output, Port.Capacity.Multi);
        generatedPort.portName = "Next";
        node.outputContainer.Add(generatedPort);
        
        node.capabilities &= ~Capabilities.Movable;
        node.capabilities &= ~Capabilities.Deletable;
        
        node.RefreshExpandedState();
        node.RefreshPorts();
        
        node.SetPosition(new Rect(100, 200, 100, 150));
        return node;
    }

    public void CreateNode(string nodeName, Vector2 position, TextAsset textAsset, bool hasBeenRead, int readDuring, int unlockWait)
    {
        AddElement(CreateDialogueNode(nodeName, position, textAsset,  hasBeenRead, readDuring, unlockWait));
    }
    
    public NewDialogueNode CreateDialogueNode(string nodeName, Vector2 position, TextAsset textAsset, bool hasBeenRead, int readDuring, int unlockWait)
    {
        string newTitle = "Dialogue Node";
        if (textAsset)
        {
            newTitle = textAsset.name;
        }
        var dialogueNode = new NewDialogueNode
        {
            title = newTitle,
            ButtonText = nodeName,
            GUID = Guid.NewGuid().ToString(),
            DialogueAsset = textAsset,
            HasBeenRead = hasBeenRead,
            ReadRun = readDuring,
            RunWaitAmount = unlockWait
        };
        
        var inputPort = GeneratePort(dialogueNode, Direction.Input, Port.Capacity.Multi);
        inputPort.portName = "Input";
        dialogueNode.inputContainer.Add(inputPort);

        dialogueNode.styleSheets.Add(Resources.Load<StyleSheet>("NodeGraphEditor"));
        
        var button = new Button(() => {AddChoicePort(dialogueNode);});
        button.text = "New Choice";
        dialogueNode.titleContainer.Add(button);
        
        var assetField = new ObjectField("Text asset");
        assetField.RegisterValueChangedCallback(evt =>
        {
            dialogueNode.DialogueAsset = (TextAsset)evt.newValue;
            dialogueNode.title = evt.newValue.name;
        });
        assetField.SetValueWithoutNotify(dialogueNode.DialogueAsset);
        dialogueNode.mainContainer.Add(assetField);
        
        var textField = new TextField("Button text");
        textField.RegisterValueChangedCallback(evt =>
        {
            dialogueNode.ButtonText = evt.newValue;
        });
        textField.SetValueWithoutNotify(dialogueNode.ButtonText);
        dialogueNode.mainContainer.Add(textField);

        var readField = new Toggle("Has been read");
        readField.RegisterValueChangedCallback(evt =>
        {
            dialogueNode.HasBeenRead = evt.newValue;
        });
        readField.SetValueWithoutNotify(dialogueNode.HasBeenRead);
        dialogueNode.mainContainer.Add(readField);
        
        var readRunField = new IntegerField("Run this was read during");
        readRunField.RegisterValueChangedCallback(evt =>
        {
            dialogueNode.ReadRun = evt.newValue;
        });
        readRunField.SetValueWithoutNotify(dialogueNode.ReadRun);
        dialogueNode.mainContainer.Add(readRunField);
        
        var runWaitField = new IntegerField("Runs after unlock");
        runWaitField.RegisterValueChangedCallback(evt =>
        {
            dialogueNode.RunWaitAmount = evt.newValue;
        });
        runWaitField.SetValueWithoutNotify(dialogueNode.RunWaitAmount);
        dialogueNode.mainContainer.Add(runWaitField);
        
        dialogueNode.RefreshExpandedState();
        dialogueNode.RefreshPorts();
        dialogueNode.SetPosition(new Rect(position, DefaultNodeSize));
        
        return dialogueNode;
    }

    public void AddChoicePort(NewDialogueNode dialogueNode, string overriddenPortName = "")
    {
        var generatedPort = GeneratePort(dialogueNode, Direction.Output);

        var oldLabel = generatedPort.contentContainer.Q<Label>("type");
        generatedPort.contentContainer.Remove(oldLabel);

        var outputPortCount = dialogueNode.outputContainer.Query("connector").ToList().Count;
        var outputName = $"Choice {outputPortCount}";
        
        var choicePortName = string.IsNullOrEmpty(overriddenPortName)
            ? $"Choice {outputPortCount + 1}"
            : overriddenPortName;

        var textField = new TextField
        {
            name = string.Empty,
            value = choicePortName
        };
        textField.RegisterValueChangedCallback(evt => generatedPort.portName = evt.newValue);
        generatedPort.contentContainer.Add(new Label("  "));
        generatedPort.contentContainer.Add(textField);
        var deleteButton = new Button(() => RemovePort(dialogueNode, generatedPort))
        {
            text = "X"
        };
        generatedPort.contentContainer.Add(deleteButton);
        
        generatedPort.portName = choicePortName;
        
        dialogueNode.outputContainer.Add(generatedPort);
        dialogueNode.RefreshPorts();
        dialogueNode.RefreshExpandedState();
    }
    
    private void RemovePort(NewDialogueNode dialogueNode, Port generatedPort)
    {
        var targetEdge = edges.ToList().Where(x =>
            x.output.portName == generatedPort.portName && x.output.node == generatedPort.node);

        if (targetEdge.Any())
        {
            var edge = targetEdge.First();
            edge.input.Disconnect(edge);
            RemoveElement(targetEdge.First());
        }
        
        dialogueNode.outputContainer.Remove(generatedPort);
        dialogueNode.RefreshPorts();
        dialogueNode.RefreshExpandedState();
    }

    public void ClearBlackboardAndExposedProperties()
    {
        ExposedProperties.Clear();
        Blackboard.Clear();
    }

    public void AddPropertyToBlackboard(ExposedProperty exposedProperty)
    {
        var localPropertyName = exposedProperty.PropertyName;
        var localPropertyValue = exposedProperty.PropertyValue;
        while (ExposedProperties.Any(x => x.PropertyName == localPropertyName))
        {
            localPropertyName = $"{localPropertyName}(1)";
        }
        
        var property = new ExposedProperty();
        property.PropertyName  = localPropertyName;
        property.PropertyValue = localPropertyValue;
        ExposedProperties.Add(property);

        var container = new VisualElement();
        var blackboardField = new BlackboardField{text = property.PropertyName, typeText = "string property"};
        container.Add(blackboardField);
        
        var propertyValueTextField = new TextField("Value")
        {
            value = localPropertyValue
        };
        propertyValueTextField.RegisterValueChangedCallback(evt =>
        {
            var changingPropertyIndex = ExposedProperties.FindIndex(x => x.PropertyName == property.PropertyName);
            ExposedProperties[changingPropertyIndex].PropertyValue = evt.newValue;
        });
        var blackBoardValueRow = new BlackboardRow(blackboardField,propertyValueTextField);
        container.Add(blackBoardValueRow);
        
        Blackboard.Add(container);
    }
}
