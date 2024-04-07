using System.Linq.Expressions;
using MonkeyInterpreter.AST;
using MonkeyInterpreter.Core;
using Boolean = MonkeyInterpreter.Core.Boolean;

namespace MonkeyInterpreter.AST;

public static class Evaluator
{
	private static readonly Boolean TrueBooleanObject = new(true);
	private static readonly Boolean FalseBooleanObject = new(false);
	private static readonly Null NullObject = new();
	
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
				return new Integer(integerLiteral.Value);
			
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
			default:
				return NullObject;
		}
	}

	private static IObject? EvaluateBangOperatorExpression(IObject right)
	{
		return right switch
		{
			Boolean boolean => boolean.Value switch
			{
				true => FalseBooleanObject,
				false => TrueBooleanObject
			},
			Null _ => TrueBooleanObject,
			_ => FalseBooleanObject
		};
	}
}