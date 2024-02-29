﻿using MonkeyInterpreter.REPL;

namespace MonkeyInterpreter.CLI;

public static class Program
{
	public static void Main(string[] args)
	{
		Console.WriteLine("Enter Monkey commands");
		
		Repl.Start(Console.In, Console.Out);
	}
}