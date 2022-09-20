using System;
using DailyDuty.Utilities;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace KitchenSync.Utilities;

internal unsafe class BaseNode
{
    private readonly AtkUnitBase* node;

    public BaseNode(string addon)
    {
        node = (AtkUnitBase*) Service.GameGui.GetAddonByName(addon, 1);
    }

    public BaseNode Print()
    {
        Chat.Print("AtkUnitBase", new IntPtr(node));

        return this;
    }

    public ResNode GetRootNode()
    {
        if (node == null) throw new NullReferenceException();

        return new ResNode(node->RootNode);
    }

    public ResNode GetResNode(uint id)
    {
        if (node == null) throw new NullReferenceException();

        var targetNode = node->GetNodeById(id);

        return new ResNode(targetNode);
    }

    public ComponentNode GetComponentNode(uint id)
    {
        if (node == null) throw new NullReferenceException();

        var targetNode = (AtkComponentNode*) node->GetNodeById(id);

        return new ComponentNode(targetNode);
    }

    public ComponentNode GetNestedNode(params uint[] idList)
    {
        var startingNode = GetComponentNode(idList[0]);

        for (var i = 1; i < idList.Length; ++i)
        {
            startingNode = startingNode.GetComponentNode(idList[i]);
        }

        return startingNode;
    }
}

internal unsafe class ResNode
{
    private readonly AtkResNode* node;

    public ResNode(AtkResNode* node)
    {
        this.node = node;
    }

    public ResNode Print()
    {
        Chat.Print("AtkResNode", new IntPtr(node));
    
        return this;
    }
}

internal unsafe class ComponentNode
{
    private readonly AtkComponentNode* node;
    private readonly AtkComponentBase* componentBase;

    public ComponentNode(AtkComponentNode* node)
    {
        this.node = node;
        componentBase = node->Component;
    }

    public ComponentNode Print()
    {
        Chat.Print("AtkComponentNode", new IntPtr(node));
    
        return this;
    }

    public ComponentNode GetComponentNode(uint id)
    {
        if(componentBase == null) throw new NullReferenceException();

        var targetNode = Node.GetNodeByID<AtkComponentNode>(componentBase->UldManager, id);

        return new ComponentNode(targetNode);
    }

    public AtkImageNode* GetImageNode(uint id)
    {
        return Node.GetNodeByID<AtkImageNode>(componentBase->UldManager, id);
    }

}

internal static unsafe class Node
{
    public static T* GetNodeByID<T>(AtkUldManager uldManager, uint nodeId) where T : unmanaged 
    {
        for (var i = 0; i < uldManager.NodeListCount; i++) 
        {
            var currentNode = uldManager.NodeList[i];

            if (currentNode->NodeID != nodeId) continue;

            return (T*) currentNode;
        }

        return null;
    }
}