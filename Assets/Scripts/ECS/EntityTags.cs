using System;
using Unity.Entities;

public struct BoidTag: IComponentData { }
public struct MoveInDirectionTag: IComponentData { }
public struct MoveForwardTag : IComponentData { }
public struct MoveTowardTargetTag : IComponentData { }
public struct RotateToTargetTag: IComponentData { }
public struct RotateToFixedTargetTag: IComponentData { }
public struct RotateToDirectionTag : IComponentData { }
public struct ExplodeAndDeleteTag: IComponentData { }
public struct EnemyTag: IComponentData { }
public struct ShootableTag: IComponentData { }
public struct PlayerTag : IComponentData { }
public struct BulletTag : IComponentData { }
public struct PickupTag : IComponentData { }
public struct MagneticPickupTag : IComponentData { }

public struct LightTag: IComponentData { }