# WolframNETLink 💥🔗 - Some Tests 🥼

Let's look at calling Wolfram [Mathematica locally using .NET](https://reference.wolfram.com/language/NETLink/tutorial/CallingTheWolframLanguageFromNET.html) (C#) by leveraging the Wolfram .NET/Link library. This allows for running Wolfram Language code directly from .NET applications. I'll want to compare this with WolframAlpha calls over http since the local approach does require some ...

## Setup

(Find a reference for the main .NET/Link namespace at the end of this readme.)

Mainly, point `.csproj` and provision extra dependencies like so, for MM **14.0** (and net8.0) - just add the ItemGroup and Target entries in you New Console Application in Visual Studio.

```
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <PlatformTarget>x64</PlatformTarget>
  </PropertyGroup>

  <ItemGroup>
    <Reference Include="Wolfram.NETLink">
      <HintPath>C:\Program Files\Wolfram Research\Mathematica\14.0\SystemFiles\Links\NETLink\Wolfram.NETLink.dll</HintPath>
    </Reference>
  </ItemGroup>

  <Target Name="CopyNativeBinaries" AfterTargets="Build">
    <Copy SourceFiles="C:\Program Files\Wolfram Research\Mathematica\14.0\SystemFiles\Links\NETLink\ml64i4.dll"
          DestinationFolder="$(OutDir)" />
  </Target>

</Project>
```

I found that the post-build process _is_ necessary. (May 2024)

## Tech Background: Symbolic Protocol

> [!NOTE]  
> The Wolfram Symbolic Transfer Protocol (WSTP) is a platform-independent protocol for communicating between programs. In more concrete terms, it is a means to send and receive Wolfram Language **expressions**. WSTP is the means by which the notebook front end and kernel communicate with each other. It is also used by a large number of commercial and freeware applications and utilities that link the Wolfram Language and other programs or languages. It is implemented as a library of C-language functions. .NET/Link brings the capabilities of WSTP into .NET in a way that is simpler to use and much more powerful than the raw C-level API.

From [Calling the Wolfram Language from .NET](https://reference.wolfram.com/language/NETLink/tutorial/CallingTheWolframLanguageFromNET.html) (May 31st, 2024, my **highlight**: we look at [expression](https://reference.wolfram.com/language/NETLink/ref/net/Wolfram.NETLink.Expr.html) handling on the .NET side in example 2 in the following, but ...) The idea boils down to the following:

> [!IMPORTANT]  
> The Expr class is a representation of arbitrary Mathematica expressions in .NET. Exprs are created by reading an expression from a link (using the GetExpr method), they can be decomposed into component Exprs with properties and methods like Head and Part, and their structure can be queried with methods like Length, NumberQ, and VectorQ. All these methods will be familiar to Mathematica programmers, and their Expr counterparts work similarly.
> From [relevant WL reference](https://reference.wolfram.com/language/NETLink/ref/net/Wolfram.NETLink.Expr.html)

## Project Structure and Usage

Now we try some different MainImplementations to demonstratre some use cases: the overall project structure follows this diagram.

```
WolframNETLink
│
├── Program.cs
├── MainImplementation1.cs
├── MainImplementation2.cs
└── MainImplementation3.cs
```

### General Usage

Follow the setup and start in Visual Studio, then select the implementation want to test out: You will see a Mathematical Kernel instance pop up in the background: **That**'s the basis for this project and what we are testing.

![Screenshot 2024-06-01 143636](https://github.com/heseltime/SWK5-W-WolframNETLink/assets/66922223/c35e7500-5ead-4154-9377-2fcd361aba55)

### Code Template

Each of the implementations called by the console app follow this structure of the Run-method:

```
public static void Run()
{
    // Initialize the Wolfram Engine
    IKernelLink ml = MathLinkFactory.CreateKernelLink();

    try
    {
        // Discard the initial InputNamePacket the kernel will send
        ml.WaitAndDiscardAnswer(); // <-- A detail.

        // Make the call to the kernel
        ml.Evaluate("..."); // <-- Main thing: the expression to evalate is passed as a string
        ml.WaitForAnswer();

        // Read the result back when available
        object result = ml.Get... // the available getters will be listed in a moment
        // --- Now do something with the result, but perhaps check it first
        //        to see if it matches the expectation
    }
    catch (MathLinkException e)
    {
        // MathLink Exception handling
        Console.WriteLine($"MathLinkException: {e.Message}");
    }
    finally
    {
        // Always close the link when done
        ml.Close();
    }
}
```

The available getters provided by MathLink as an object/class (interface, actually) are full listed [in the WL reference](https://reference.wolfram.com/language/NETLink/ref/net/Wolfram.NETLink.IMathLinkMembers.html), here just the ones with "Get" in the name:

```markdown
### IMathLink Methods with "Get" in the Name

- `GetArgCount`  
  Reads the argument count of an expression being read manually.

- `GetArray`  
  Overloaded. Reads an array of the specified type and depth.

- `GetBoolean`  
  Reads the Mathematica symbols True or False as a bool.

- `GetBooleanArray`  
  Reads a list as a one-dimensional array of bools.

- `GetByteArray`  
  Reads a list as a one-dimensional array of bytes.

- `GetByteString`  
  Reads a Mathematica string as an array of bytes.

- `GetCharArray`  
  Reads a list as a one-dimensional array of chars.

- `GetComplex`  
  Reads a complex number. This can be an integer, real, or a Mathematica expression with head Complex.

- `GetComplexArray`  
  Reads a list as a one-dimensional array of complex numbers.

- `GetData`  
  Gets a specified number of bytes in the textual form of the expression currently being read.

- `GetDecimal`  
  Reads a Mathematica integer or real number or integer as a decimal.

- `GetDecimalArray`  
  Reads a list as a one-dimensional array of decimals.

- `GetDouble`  
  Reads a Mathematica real number or integer as a double.

- `GetDoubleArray`  
  Reads a list as a one-dimensional array of doubles.

- `GetExpr`  
  Reads an arbitrary expression from the link and creates an Expr from it.

- `GetExpressionType`  
  Gives the type of the current element in the expression currently being read.

- `GetFunction`  
  Reads a function name and argument count.

- `GetInt16Array`  
  Reads a list as a one-dimensional array of shorts.

- `GetInt32Array`  
  Reads a list as a one-dimensional array of ints.

- `GetInt64Array`  
  Reads a list as a one-dimensional array of longs.

- `GetInteger`  
  Reads a Mathematica integer as a 32-bit integer.

- `GetNextExpressionType`  
  Gives the type of the next element in the expression currently being read.

- `GetObject`  
  Reads a single expression off the link and returns an appropriate object.

- `GetSingleArray`  
  Reads a list as a one-dimensional array of floats.

- `GetString`  
  Reads a Mathematica character string.

- `GetStringArray`  
  Reads a list as a one-dimensional array of strings.

- `GetStringCRLF`  
  Reads a Mathematica character string and translates newlines into Windows format.

- `GetSymbol`  
  Reads a Mathematica symbol as a string.
```

Examples of metadata you can get out of the expression representation is printed afterwards:

```
// Example: Other things you can do with the Expr object
// Print the head of the expression
Console.WriteLine("Head of the result: " + result.Head);

// Check the type of the expression
Console.WriteLine("Is the result a list? " + result.ListQ());

// Get the length of the expression (number of parts)
Console.WriteLine("Number of parts: " + result.Length);

// Convert the entire expression to a string
Console.WriteLine("Expression as a string: " + result.StringQ());
```

### MainImplementation1: PrimeFactor 

**A simple example with array conversion to handle evaluation result**

The most interesting code here is the one making the call to the kernel evaluation, inside the template code, storing it in a suitable container and printing to console:

```
// Send a more complex command
ml.Evaluate("FactorInteger[123456789]");
ml.WaitForAnswer();

// Read the result as a 2D array of integers
int[,] factorResult = (int[,])ml.GetArray(typeof(int), 2);
Console.WriteLine("Factors of 123456789:");
for (int i = 0; i < factorResult.GetLength(0); i++)
{
    Console.WriteLine($"Prime: {factorResult[i, 0]}, Exponent: {factorResult[i, 1]}");
}
```

![image](https://github.com/heseltime/SWK5-W-WolframNETLink/assets/66922223/b91d8138-5125-43ff-be7d-d938000d1d57)

**Update**: The correct method for retrieving a 2D array from the Wolfram.NETLink is to use the **GetArray** method

```
// Read the result as a 2D array of integers
object result = ml.GetArray(typeof(int), 2); // <-- Correct way to do the conversion
if (result is int[,] factorResult)
{
    Console.WriteLine("Implementation 1 - Factors of 123456789:");
    for (int i = 0; i < factorResult.GetLength(0); i++)
    {
        Console.WriteLine($"Prime: {factorResult[i, 0]}, Exponent: {factorResult[i, 1]}");
    }
}
```

So, just to write down at least the simple example in full:

```
public static class MainImplementation1
{
    public static void Run()
    {
        // Initialize the Wolfram Engine
        IKernelLink ml = MathLinkFactory.CreateKernelLink();

        try
        {
            // Discard the initial InputNamePacket the kernel will send
            ml.WaitAndDiscardAnswer();

            ml.Evaluate("FactorInteger[123456789]");
            ml.WaitForAnswer();

            // Read the result as a 2D array of integers
            object result = ml.GetArray(typeof(int), 2);
            if (result is int[,] factorResult)
            {
                Console.WriteLine("Implementation 1 - Factors of 123456789:");
                for (int i = 0; i < factorResult.GetLength(0); i++)
                {
                    Console.WriteLine($"Prime: {factorResult[i, 0]}, Exponent: {factorResult[i, 1]}");
                }
            }
            else
            {
                Console.WriteLine("Unexpected result type.");
            }
        }
        catch (MathLinkException e)
        {
            Console.WriteLine($"MathLinkException: {e.Message}");
        }
        finally
        {
            // Always close the link when done
            ml.Close();
        }
    }
}
```

### MainImplementation2: Expr Tests! 

[Expression Wolfram Documentation](https://reference.wolfram.com/language/NETLink/ref/net/Wolfram.NETLink.Expr.html) that details how to handle representing WL expressions on the C# side: the main method under test here is **GetArray**.

The result of running will be similar for this implementation:

![image](https://github.com/heseltime/SWK5-W-WolframNETLink/assets/66922223/d5daf740-e19f-4535-a9e1-cc52366be63c)

But the evaluation-processing back on the .Net side is different, now using an expression representation, for which there are two main use cases (the first is used here, to read via **GetExpr**, see the documentation at the end of this readme, and process the result and output some metadata):

```
Expr e = ml.GetExpr();
// ... Later, write it to a different MathLink:
otherML.Put(e);
e.Dispose();
```

This leads to a possibility of later writing to the MathLink - as the central communication element, implementing `IKernelLink` and produced by `MathLinkFactory`, providing the methods `WaitAndDiscardAnswer`, `Evaluate`, `WaitForAnswer` and importantly `GetExpr`.

> [!TIP]
> Many of the IKernelLink methods take either a string or an Expr. If it is not convenient to build a string of Mathematica input, you can use an Expr. There are two ways to build an Expr: you can use a constructor, or you can create a loopback link as a scratchpad, build the expression on this link with a series of Put calls, then read the expression off the loopback link using GetExpr. Here is an example that creates an Expr that represents 2+2 and computes it in Mathematica using these two techniques:

```
// First method: Build it using Expr constructors:
Expr symbolPlus = new Expr(ExpressionType.Symbol, "Plus");
Expr e1 = new Expr(symbolPlus, 2, 2);
// ml is a KernelLink
string result = ml.EvaluateToOutputForm(e1, 0);
   
// Second method: Build it on an ILoopbackLink with MathLink calls:
ILoopbackLink loop = MathLinkFactory.CreateLoopbackLink();
loop.PutFunction("Plus", 2);
loop.Put(2);
loop.Put(2);
Expr e2 = loop.GetExpr();
loop.Close();
result = ml.EvaluateToOutputForm(e2, 0);
e2.Dispose();
```

(Following the [expression class reference](https://reference.wolfram.com/language/NETLink/ref/net/Wolfram.NETLink.Expr.html).)

In the code the relevant part looks like:

```
// Get the result as an expression
Expr result = ml.GetExpr();

// Process the result
if (result.Head.ToString() == "List")
{
    Console.WriteLine("Implementation 2 - Factors of 123456789:");
    foreach (Expr factor in result.Args)
    {
        int prime = (int)factor.Part(1).AsInt64();
        int exponent = (int)factor.Part(2).AsInt64();
        Console.WriteLine($"Prime: {prime}, Exponent: {exponent}");
    }
}
else
{
    Console.WriteLine("Unexpected result type.");
}
```

### MainImplementation3: Async Programming

**Full-fledged async example with callback/delagate firing**

A note on tread safety as far as expressions go:

>[!TIP]
>Like Mathematica expressions, Exprs are immutable, meaning they can never be changed once they are created. Operations that might appear to modify an Expr (like Delete) return new modified Exprs without changing the original. Because Exprs are immutable, they are also thread-safe, meaning that any number of threads can access a given Expr at the same time.

## The Main .NET/Link Namespace

### .NET/Link API Version 1.7

Per the [official Wolfram documentation](https://reference.wolfram.com/language/NETLink/ref/net/Wolfram.NETLink.html).

#### Classes

| Class                        | Description                                                                                                                                    |
|------------------------------|------------------------------------------------------------------------------------------------------------------------------------------------|
| `Expr`                       | A representation of arbitrary Mathematica expressions in .NET.                                                                                  |
| `ExprFormatException`        | The exception thrown by the "AsXXX" methods of the Expr class (e.g., AsInt64, AsDouble, AsArray, etc.)                                         |
| `MathDelegate`               | Contains the `CreateDelegate` method, which creates delegate objects that invoke a specified Mathematica function.                             |
| `MathematicaNotReadyException` | This exception is thrown in `RequestTransaction` when the kernel is not in a state where it is receptive to calls that originate in .NET.     |
| `MathKernel`                 | `MathKernel` is a non-visual component that provides a very high-level interface for interacting with Mathematica.                             |
| `MathLinkException`          | The exception thrown by methods in the `IMathLink` and `IKernelLink` interfaces when a link error occurs.                                       |
| `MathLinkFactory`            | `MathLinkFactory` is the class that is used to construct objects of the various link interfaces (`IKernelLink`, `IMathLink`, and `ILoopbackLink`). |
| `NETLinkConstants`           | A handful of constants, including the .NET/Link version number.                                                                                |
| `StdLink`                    | `StdLink` is a container for some methods and state related to the link back to the kernel.                                                    |
| `TypeLoader`                 | `TypeLoader` is the class responsible for loading all assemblies and types from the Mathematica functions `LoadNETAssembly` and `LoadNETType`. |

#### Interfaces

| Interface   | Description                                                                                                   |
|-------------|---------------------------------------------------------------------------------------------------------------|
| `IKernelLink` | The link interface that most programmers will use.                                                           |
| `ILinkMark`  | Represents a mark in the incoming MathLink data stream that you can seek back to.                             |
| `ILoopbackLink` | Represents a special type of link known as a loopback link.                                                 |
| `IMathLink`  | `IMathLink` is the low-level interface that is the root of all link objects in .NET/Link.                     |

#### Delegates

| Delegate       | Description                                                              |
|----------------|--------------------------------------------------------------------------|
| `MessageHandler` | Represents the method that will handle the `MessageArrived` event.       |
| `PacketHandler` | Represents the method that will handle the `PacketArrived` event.        |
| `YieldFunction` | Represents the method that will handle the `Yield` event.                |

#### Enumerations

| Enumeration                  | Description                                                                                              |
|------------------------------|----------------------------------------------------------------------------------------------------------|
| `ExpressionType`             | Designates the type of a Mathematica expression being read or written on a link, or in an `Expr`.        |
| `MathKernel.ResultFormatType` | Values for the `ResultFormat` property. These values specify the format in which results from computations should be returned. |
| `MathLinkMessage`            | Designates the type of a low-level MathLink message.                                                     |
| `PacketType`                 | Designates a MathLink packet type. Used by the `PacketHandler` delegate, and returned by `NextPacket`.    |

### Expr Members (.NET/Link API Version 1.7)

#### Public Static (Shared) Fields

| Field         |
|---------------|
| `INT_MINUSONE` |
| `INT_ONE`      |
| `INT_ZERO`     |
| `SYM_FALSE`    |
| `SYM_INTEGER`  |
| `SYM_LIST`     |
| `SYM_REAL`     |
| `SYM_STRING`   |
| `SYM_SYMBOL`   |
| `SYM_TRUE`     |

#### Public Static (Shared) Methods

| Method             | Description                                      |
|--------------------|--------------------------------------------------|
| `CreateFromLink`   | Creates an Expr by reading it off a link.        |

#### Public Static (Shared) Operators and Type Conversions

| Operator/Conversion                 | Description                                                                  |
|-------------------------------------|------------------------------------------------------------------------------|
| `Equality Operator`                 | Implements a value-based equality comparison similar to Mathematica's SameQ. |
| `Inequality Operator`               | Implements a value-based inequality comparison.                              |
| `Explicit Expr to Double Conversion`| Converts the Expr to a double value, same as calling the `AsDouble` method.  |
| `Explicit Expr to Int64 Conversion` | Converts the Expr to a long integer value, same as calling the `AsInt64` method. |
| `Explicit Expr to String Conversion`| Converts the Expr to a string representation, same as calling the `ToString` method. |

#### Public Instance Constructors

| Constructor | Description                  |
|-------------|------------------------------|
| `Expr`      | Overloaded. Creates a new Expr object. |

#### Public Instance Properties

| Property    | Description                                                                 |
|-------------|-----------------------------------------------------------------------------|
| `Args`      | Gets an array of Exprs representing the arguments of this Expr.             |
| `Dimensions`| Gets an array of integers representing the dimensions of this Expr.         |
| `Head`      | Gets the Expr representing the head of this Expr.                           |
| `Item`      | Gets a part based on its position index. This is the indexer for the class. |
| `Length`    | Gets the length of this Expr.                                               |

#### Public Instance Methods

| Method                | Description                                                                                                           |
|-----------------------|-----------------------------------------------------------------------------------------------------------------------|
| `AsArray`             | Converts the Expr to an array of the requested type and depth.                                                        |
| `AsDouble`            | Gives the double value for Exprs that can be represented as doubles.                                                  |
| `AsInt64`             | Gives the Int64 value for Exprs that can be represented as integers.                                                  |
| `AtomQ`               | Tells whether the Expr represents a Mathematica atom. Works like the Mathematica function `AtomQ`.                    |
| `ComplexQ`            | Tells whether the Expr represents a Mathematica Complex number.                                                       |
| `Delete`              | Returns a new Expr that has the same head but the nth element deleted. Works like the Mathematica function `Delete`.  |
| `Dispose`             | Frees resources that the Expr uses internally.                                                                        |
| `Equals`              | Implements a value-based equality comparison similar to Mathematica's `SameQ`.                                        |
| `GetHashCode`         |                                                                                                                       |
| `GetObjectData`       | Populates the `SerializationInfo` object with the Expr's internal state information.                                  |
| `GetType`             | Inherited from `Object`.                                                                                              |
| `Insert`              | Returns a new Expr that has the same head but with e inserted into position n. Works like the Mathematica function `Insert`. |
| `IntegerQ`            | Tells whether the Expr represents a Mathematica integer. Works like the Mathematica function `IntegerQ`.              |
| `ListQ`               | Tells whether the Expr represents a Mathematica list. Works like the Mathematica function `ListQ`.                    |
| `MatrixQ`             | Overloaded. Tells whether the Expr represents a Mathematica matrix. Works like the Mathematica function `MatrixQ`.    |
| `NumberQ`             | Tells whether the Expr represents a Mathematica number. Works like the Mathematica function `NumberQ`.                |
| `Part`                | Overloaded. Gives the Expr representing the specified part of this Expr. Works like the Mathematica function `Part`.  |
| `Put`                 | Not intended for general use.                                                                                         |
| `RationalQ`           | Tells whether the Expr represents a Mathematica Rational number.                                                     |
| `RealQ`               | Tells whether the Expr represents a Mathematica Real number.                                                         |
| `StringQ`             | Tells whether the Expr represents a Mathematica string. Works like the Mathematica function `StringQ`.                |
| `SymbolQ`             | Tells whether the Expr represents a Mathematica symbol.                                                              |
| `Take`                | Returns a new Expr that has the same head but only the first n elements of this Expr. Works like the Mathematica function `Take`. |
| `ToString`            | Returns a representation of the Expr as a Mathematica `InputForm` string.                                             |
| `TrueQ`               | Tells whether the Expr represents the Mathematica symbol True. Works like the Mathematica function `TrueQ`.           |
| `VectorQ`             | Overloaded. Tells whether the Expr represents a Mathematica vector. Works like the Mathematica function `VectorQ`.    |


(Referenced June 1st, 2024.)

