using MonkeyInterpreter.Core.AbstractSyntaxTree;

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
			
			case FunctionLiteral functionLiteral:
				List<Identifier> parameters = functionLiteral.Parameters;
				BlockStatement body = functionLiteral.Body;
				return new FunctionObject(parameters, body, env);
			
			case CallExpression callExpression:
				IObject function = Evaluate(callExpression.Function, env);
				if (function.Type() == ObjectTypeEnum.Error)
				{
					return function;
				}

				List<IObject> arguments = EvaluateExpressions(callExpression.Arguments, env);
				if (arguments.Count == 1 && arguments[0].Type() == ObjectTypeEnum.Error)
				{
					return arguments[0];
				}

				return ApplyFunction(function, arguments);

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
			
			case StringLiteral stringLiteral:
				return new StringObject(stringLiteral.Value);
			
			case BooleanLiteral booleanLiteral:
				return booleanLiteral.Value switch
				{
					true => TrueBooleanObject,
					false => FalseBooleanObject
				};
			
			case ArrayLiteral arrayLiteral:
				List<IObject> elements = EvaluateExpressions(arrayLiteral.Elements, env);
				if (elements.Count == 1 && elements[0].Type() == ObjectTypeEnum.Error)
				{
					return elements[0];
				}

				return new ArrayObject(elements);

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

	private static List<IObject> EvaluateExpressions(List<IExpression> expressions, VariableEnvironment env)
	{
		List<IObject> result = new();

		foreach (IExpression expression in expressions)
		{
			IObject evaluated = Evaluate(expression, env);
			if (evaluated.Type() == ObjectTypeEnum.Error)
			{
				return new List<IObject> { evaluated };
			}
			
			result.Add(evaluated);
		}

		return result;
	}

	private static IObject ApplyFunction(IObject function, List<IObject> arguments)
	{
		switch (function)
		{
			case FunctionObject functionObject:
				VariableEnvironment extendedEnvironment = ExtendFunctionEnvironment(functionObject, arguments);
				IObject evaluated = Evaluate(functionObject.Body, extendedEnvironment);
				return UnwrapReturnValue(evaluated);
			
			case BuiltInObject builtInObject:
				return builtInObject.Function(arguments.ToArray());
			
			default:
				return GenerateError("Not a function: {0}", function.Type());
		}
		


	}

	private static VariableEnvironment ExtendFunctionEnvironment(FunctionObject function, List<IObject> arguments)
	{
		VariableEnvironment environment = new VariableEnvironment().EncloseEnvironment(function.VariableEnvironment);

		for (int i = 0; i < function.Parameters.Count; i++)
		{
			environment.Set(function.Parameters[i].Value, arguments[i]);
		}

		return environment;
	}

	private static IObject UnwrapReturnValue(IObject @object)
	{
		if (@object is ReturnValueObject returnValueObject)
		{
			return returnValueObject;
		}

		return @object;
	}

	private static IObject EvaluateIdentifier(Identifier identifier, VariableEnvironment env)
	{
		IObject? value = env.Get(identifier.Value);
		if (value is not null)
		{
			 return value;
		}

		value = BuiltIns.BuiltInFunctions.GetValueOrDefault(identifier.Value);
		if (value is not null)
		{
			return value;
		}
			
		return GenerateError("Identifier not found: {0}", identifier.Value);
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
			
			case true when left.Type() == ObjectTypeEnum.String && right.Type() == ObjectTypeEnum.String:
				return EvaluateStringInfixExpression(@operator, left, right);
			
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
	
	private static IObject EvaluateStringInfixExpression(string @operator, IObject left, IObject right)
	{
		string leftValue = ((StringObject)left).Value;
		string rightValue = ((StringObject)right).Value;
		
		switch (@operator)
		{
			case "+": 
				return new StringObject(leftValue + rightValue);
			
			case "==":
				return leftValue == rightValue ? TrueBooleanObject : FalseBooleanObject;
			
			case "!=":
				return leftValue != rightValue ? TrueBooleanObject : FalseBooleanObject;
			
			default:
				return GenerateError("Unknown operator: {0} {1} {2}", left.Type(), @operator, right.Type());
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

	public static ErrorObject GenerateError(string format, params object[] objects)
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