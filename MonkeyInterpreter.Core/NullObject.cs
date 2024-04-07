﻿namespace MonkeyInterpreter.Core;

public class NullObject : IObject
{
	private const ObjectTypeEnum ObjectType = ObjectTypeEnum.Null;

	public string Inspect()
	{
		return "null";
	}

	public ObjectTypeEnum Type()
	{
		return ObjectType;
	}
}
