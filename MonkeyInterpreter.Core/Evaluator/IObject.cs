namespace MonkeyInterpreter.Core.Evaluator;

public interface IObject
{
	ObjectTypeEnum Type();
	string Inspect();
}