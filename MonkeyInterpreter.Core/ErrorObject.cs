namespace MonkeyInterpreter.Core;

public class ErrorObject : IObject
{
	public string Message;

	public ErrorObject(string message)
	{
		Message = message;
	}

	public ObjectTypeEnum Type()
	{
		return ObjectTypeEnum.Error;
	}

	public string Inspect()
	{
		return "ERROR: " + Message;
	}
}