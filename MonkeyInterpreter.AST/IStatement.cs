namespace MonkeyInterpreter.AST;

public interface IStatement : INode
{
	public LetStatement StatementNode();
}