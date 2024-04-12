using MonkeyInterpreter.Core;

namespace MonkeyInterpreter.AST;

public static class Evaluator
{
	private static readonly BooleanObject TrueBooleanObject = new(true);
	private static readonly BooleanObject FalseBooleanObject = new(false);
	public static readonly NullObject NullObject = new();

	public static IObject Evaluate(INode node)
	{
		switch (node)
		{
			case AbstractSyntaxTree ast:
				return EvaluateAst(ast);
			
			case ExpressionStatement expressionStatement:
				if (expressionStatement.Expression is null)
				{
					return NullObject;
				}
				return Evaluate(expressionStatement.Expression);
			
			case PrefixExpression prefixExpression:
				IObject prefixRight = Evaluate(prefixExpression.Right);
				return EvaluatePrefixExpression(prefixExpression.Operator, prefixRight);
			
			case InfixExpression infixExpression:
				IObject infixLeft = Evaluate(infixExpression.Left);
				IObject infixRight = Evaluate(infixExpression.Right);
				return EvaluateInfixExpression(infixExpression.Operator, infixLeft, infixRight);
			
			case BlockStatement blockStatement:
				return EvaluateBlockStatement(blockStatement);
			
			case IfExpression ifExpression:
				return EvaluateIfExpression(ifExpression);
			
			case ReturnStatement returnStatement:
				IObject value = Evaluate(returnStatement.ReturnValue);
				return new ReturnValueObject(value);
			
			case IntegerLiteral integerLiteral:
				return new IntegerObject(integerLiteral.Value);
			
			case BooleanLiteral booleanLiteral:
				return booleanLiteral.Value switch
				{
					true => TrueBooleanObject,
					false => FalseBooleanObject
				};
			
			default:
				return NullObject;
		}

	}

	private static IObject EvaluateAst(AbstractSyntaxTree ast)
	{
		IObject result = NullObject;
		
		foreach (IStatement statement in ast.Statements)
		{
			result = Evaluate(statement);

			if (result is ReturnValueObject returnValueObject)
			{
				return returnValueObject.Value;
			}
		}

		return result;
	}

	private static IObject EvaluatePrefixExpression(string @operator, IObject right)
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

	private static IObject EvaluateBlockStatement(BlockStatement blockStatement)
	{
		IObject result = NullObject;
		
		foreach (IStatement statement in blockStatement.Statements)
		{
			result = Evaluate(statement);

			if (result != NullObject && result.Type() == ObjectTypeEnum.ReturnValue)
			{
				return result;
			}
		}

		return result;
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

	private static IObject EvaluateIfExpression(IfExpression ifExpression)
	{
		IObject condition = Evaluate(ifExpression.Condition);

		if (IsTruthy(condition))
		{
			return Evaluate(ifExpression.Consequence);
		}
		else if (ifExpression.Alternative is not null)
		{
			return Evaluate(ifExpression.Alternative);
		}
		else
		{
			return NullObject;
		}
	}

	private static bool IsTruthy(IObject @object)
	{
		return true switch
		{
			true when @object == NullObject => false,
			true when @object == TrueBooleanObject => true,
			true when @object == FalseBooleanObject => false,
			_ => true
		};
	}
}