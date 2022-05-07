using System;
using Unity.Entities;

public struct MoveInDirectionTag: IComponentData { }
public struct MoveForwardTag : IComponentData { }
public struct MoveTowardTargetTag : IComponentData { }
public struct RotateToTargetTag: IComponentData { }
public struct RotateToDirectionTag : IComponentData { }
public struct ExplodeAndDeleteTag: IComponentData { }
public struct EnemyTag: IComponentData { }
public struct PlayerTag : IComponentData { }
public struct BulletTag : IComponentData { }

