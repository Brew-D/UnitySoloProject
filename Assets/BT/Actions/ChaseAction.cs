using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Chase", story: "[Self] Chase [Player] In [DetectRange] With [MoveSpeed]", category: "Action", id: "e09846ff67d4cead9909bde0fb987c20")]
public partial class ChaseAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<GameObject> Player;
    [SerializeReference] public BlackboardVariable<float> DetectRange;
    [SerializeReference] public BlackboardVariable<float> MoveSpeed;
    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        Vector3 direction = (Player.Value.transform.position - Self.Value.transform.position).normalized;
        Vector3 nextPos = new Vector3(Self.Value.transform.position.x + direction.x * MoveSpeed * Time.deltaTime, Self.Value.transform.position.y);
        Self.Value.transform.position = nextPos;

        return Vector2.Distance(Self.Value.transform.position, Player.Value.transform.position) > DetectRange ? Status.Success : Status.Failure;
    }

    protected override void OnEnd()
    {
    }
}

