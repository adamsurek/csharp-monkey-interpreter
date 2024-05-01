## Writing an Interpreter in C#

### Monkey-lang Interpreter

This project is based on Thorsten Ball's [Writing an Interpreter in Go](https://interpreterbook.com/). The end
result is an interpreter for the Monkey Programming Language. Monkey supports the following actions and features:

- Variable binding
- Arithmetic expressions
- A handful of built-in functions
- Various type and data structure implementations
  - Integers
  - Booleans
  - Strings
  - Arrays
  - Hashes (dictionaries)
  - First-class and higher-order functions
  - Closures

### Project Goal

By working through this project, I aimed to achieve the following goals:

- Learn how interpreters work, as well as its fundamental components (lexers, parsers, etc.)
- Become more comfortable with unit testing, specifically using [XUnit](https://xunit.net/)
- Gain hands on experience with Test Driven Development
- Increase my knowledge of C#, .NET, and object-oriented programming
- Become more familiar with Golang