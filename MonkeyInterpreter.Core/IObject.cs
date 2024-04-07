namespace MonkeyInterpreter.Core;

public interface IObject
{
	ObjectTypeEnum Type();
	string Inspect();
}