using MonkeyInterpreter.Core;

namespace MonkeyInterpreter.AST;

public static class Evaluator
{
	private static readonly BooleanObject TrueBooleanObject = new(true);
	private static readonly BooleanObject FalseBooleanObject = new(false);
	private static readonly NullObject NullObject = new();
	
	public static IObject? Evaluate(INode node)
	{
		switch (node)
		{
			case AbstractSyntaxTree ast:
				return EvaluateStatements(ast.Statements);
			
			case ExpressionStatement expressionStatement:
				return Evaluate(expressionStatement.Expression);
			
			case PrefixExpression prefixExpression:
				IObject? right = Evaluate(prefixExpression.Right);
				return EvaluatePrefixExpression(prefixExpression.Operator, right);
			
			case IntegerLiteral integerLiteral:
				return new IntegerObject(integerLiteral.Value);
			
			case BooleanLiteral booleanLiteral:
				return booleanLiteral.Value switch
				{
					true => TrueBooleanObject,
					false => FalseBooleanObject
				};
		}

		return null;
	}

	private static IObject? EvaluateStatements(List<IStatement> statements)
	{
		IObject? result = null;
		
		foreach (IStatement statement in statements)
		{
			result = Evaluate(statement);
		}

		return result;
	}

	private static IObject? EvaluatePrefixExpression(string @operator, IObject right)
	{
		switch (@operator)
		{
			case "!":
				return EvaluateBangOperatorExpression(right);
			case "-":
				return EvaluateMinusOperatorExpression(right);
			default:
				return NullObject;
		}
	}

	private static IObject EvaluateBangOperatorExpression(IObject right)
	{
		return right switch
		{
			BooleanObject boolean => boolean.Value switch
			{
				true => FalseBooleanObject,
				false => TrueBooleanObject
			},
			NullObject _ => TrueBooleanObject,
			_ => FalseBooleanObject
		};
	}

	private static IObject EvaluateMinusOperatorExpression(IObject right)
	{
		if (right.Type() != ObjectTypeEnum.Integer)
		{
			return NullObject;
		}
		
		return new IntegerObject(-((IntegerObject)right).Value);
	}
}