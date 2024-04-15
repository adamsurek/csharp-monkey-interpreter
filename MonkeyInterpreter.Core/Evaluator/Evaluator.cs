﻿using MonkeyInterpreter.Core.AbstractSyntaxTree;

namespace MonkeyInterpreter.Core.Evaluator;

public static class Evaluator
{
	private static readonly BooleanObject TrueBooleanObject = new(true);
	private static readonly BooleanObject FalseBooleanObject = new(false);
	private static readonly NullObject NullObject = new();

	public static IObject Evaluate(INode node, VariableEnvironment env)
	{
		switch (node)
		{
			case AbstractSyntaxTree.AbstractSyntaxTree ast:
				return EvaluateAst(ast, env);
			
			case ExpressionStatement expressionStatement:
				if (expressionStatement.Expression is null)
				{
					return NullObject;
				}
				return Evaluate(expressionStatement.Expression, env);
			
			case LetStatement letStatement:
				IObject letValue = Evaluate(letStatement.Value, env);
				if (letValue.Type() == ObjectTypeEnum.Error)
				{
					return letValue;
				}
				env.Set(letStatement.Name.Value, letValue);
				
				return NullObject;
			
			case Identifier identifier:
				return EvaluateIdentifier(identifier, env);
			
			case ReturnStatement returnStatement:
				IObject returnValue = Evaluate(returnStatement.ReturnValue, env);
				if (returnValue.Type() == ObjectTypeEnum.Error)
				{
					return returnValue;
				}
				
				return new ReturnValueObject(returnValue);
			
			case PrefixExpression prefixExpression:
				IObject prefixRight = Evaluate(prefixExpression.Right, env);
				if (prefixRight.Type() == ObjectTypeEnum.Error)
				{
					return prefixRight;
				}
				
				return EvaluatePrefixExpression(prefixExpression.Operator, prefixRight);
			
			case InfixExpression infixExpression:
				IObject infixLeft = Evaluate(infixExpression.Left, env);
				if (infixLeft.Type() == ObjectTypeEnum.Error)
				{
					return infixLeft;
				}
				
				IObject infixRight = Evaluate(infixExpression.Right, env);
				if (infixRight.Type() == ObjectTypeEnum.Error)
				{
					return infixRight;
				}
				
				return EvaluateInfixExpression(infixExpression.Operator, infixLeft, infixRight);
			
			case BlockStatement blockStatement:
				return EvaluateBlockStatement(blockStatement, env);
			
			case IfExpression ifExpression:
				return EvaluateIfExpression(ifExpression, env);
			
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

	private static IObject EvaluateAst(AbstractSyntaxTree.AbstractSyntaxTree ast, VariableEnvironment env)
	{
		IObject result = NullObject;
		
		foreach (IStatement statement in ast.Statements)
		{
			result = Evaluate(statement, env);

			switch (result)
			{
				case ReturnValueObject returnValueObject:
					return returnValueObject.Value;
				
				case ErrorObject errorObject:
					return errorObject;
			}
		}

		return result;
	}

	private static IObject EvaluateIdentifier(Identifier identifier, VariableEnvironment env)
	{
		IObject? value = env.Get(identifier.Value);
		if (value is null)
		{
			return GenerateError("Identifier not found: {0}", identifier.Value);
		}

		return value;
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
				return GenerateError("Unknown operator: {0}{1}", @operator, right.Type());
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
			return GenerateError("Unknown operator: -{0}", right.Type());
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
			
			case true when left.Type() != right.Type():
				return GenerateError("Type mismatch: {0} {1} {2}",
					left.Type(), @operator, right.Type());
			
			default:
				return GenerateError("Unknown operator: {0} {1} {2}",
					left.Type(), @operator, right.Type());
		}
	}

	private static IObject EvaluateBlockStatement(BlockStatement blockStatement, VariableEnvironment env)
	{
		IObject result = NullObject;
		
		foreach (IStatement statement in blockStatement.Statements)
		{
			result = Evaluate(statement, env);

			if (result != NullObject && (result.Type() == ObjectTypeEnum.ReturnValue || result.Type() == ObjectTypeEnum.Error))
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
				return GenerateError("Unknown operator: {0} {1} {2}",
					left.Type(), @operator, right.Type());
		}
	}

	private static IObject EvaluateIfExpression(IfExpression ifExpression, VariableEnvironment env)
	{
		IObject condition = Evaluate(ifExpression.Condition, env);
		if (condition.Type() == ObjectTypeEnum.Error)
		{
			return condition;
		}

		if (IsTruthy(condition))
		{
			return Evaluate(ifExpression.Consequence, env);
		}
		
		if (ifExpression.Alternative is not null)
		{
			return Evaluate(ifExpression.Alternative, env);
		}
		
		return NullObject;
	}

	private static ErrorObject GenerateError(string format, params object[] objects)
	{
		return new ErrorObject(string.Format(format, objects));
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