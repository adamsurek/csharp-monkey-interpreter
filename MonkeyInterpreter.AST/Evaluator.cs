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
				IObject? prefixRight = Evaluate(prefixExpression.Right);
				return EvaluatePrefixExpression(prefixExpression.Operator, prefixRight);
			
			case InfixExpression infixExpression:
				IObject? infixLeft = Evaluate(infixExpression.Left);
				IObject? infixRight = Evaluate(infixExpression.Right);
				return EvaluateInfixExpression(infixExpression.Operator, infixLeft, infixRight);
			
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

	private static IObject EvaluateInfixExpression(string @operator, IObject left, IObject right)
	{
		switch (true)
		{ 
			case true when left.Type() == ObjectTypeEnum.Integer && right.Type() == ObjectTypeEnum.Integer:
				return EvaluateIntegerInfixExpression(@operator, left, right);
			
			case true when @operator == "==":
				return left == right ? TrueBooleanObject : FalseBooleanObject;
			
			case true when @operator == "!=":
				return left != right ? TrueBooleanObject : FalseBooleanObject;
			
			default:
				return NullObject;
		}
	}

	private static IObject EvaluateIntegerInfixExpression(string @operator, IObject left, IObject right)
	{
		int leftValue = ((IntegerObject)left).Value;
		int rightValue = ((IntegerObject)right).Value;

		switch (@operator)
		{
			case "+":
				return new IntegerObject(leftValue + rightValue);
			case "-":
				return new IntegerObject(leftValue - rightValue);
			case "*":
				return new IntegerObject(leftValue * rightValue);
			case "/":
				return new IntegerObject(leftValue / rightValue);
			case "<":
				return leftValue < rightValue ? TrueBooleanObject : FalseBooleanObject;
			case ">":
				return leftValue > rightValue ? TrueBooleanObject : FalseBooleanObject;
			case "==":
				return leftValue == rightValue ? TrueBooleanObject : FalseBooleanObject;
			case "!=":
				return leftValue != rightValue ? TrueBooleanObject : FalseBooleanObject;
			default:
				return NullObject;
		}
	}
}