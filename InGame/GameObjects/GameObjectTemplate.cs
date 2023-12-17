using System;

namespace ProjectZ.InGame.GameObjects;

public class GameObjectTemplate(Type objectType, object[] parameter)
{
    public Type ObjectType = objectType;
    public object[] Parameter = parameter;
}