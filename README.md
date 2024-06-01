# WolframNETLink 💥🔗 - Some Tests 🥼

Let's look at calling Wolfram [Mathematica locally using .NET](https://reference.wolfram.com/language/NETLink/tutorial/CallingTheWolframLanguageFromNET.html) (C#) by leveraging the Wolfram .NET/Link library. This allows for running Wolfram Language code directly from .NET applications. I'll want to compare this with WolframAlpha calls over http since the local approach does require some:

## Setup

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

The available getters provided by MathLink as an object/class are full listed [in the WL reference](https://reference.wolfram.com/language/NETLink/ref/net/Wolfram.NETLink.IMathLinkMembers.html), here just the ones with "Get" in the name:

```markdown
## IMathLink Methods with "Get" in the Name

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

### MainImplementation1: PrimeFactor 

**A simple example with array conversion to handle evaluation result**

The most interesting code here is the one making the call to the kernel evaluation, inside the infrastructure code, storing it in a suitable container and printing to console:

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

**Update**: The correct method for retrieving a 2D array from the Wolfram.NETLink is to use the GetArray method

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

### MainImplementation2: Expr tests! 

**See [Expression Wolfram Documentation](https://reference.wolfram.com/language/NETLink/ref/net/Wolfram.NETLink.Expr.html) that details how to handle representing WL expressions on the C# side.**

### MainImplementation3: Async Programming

**Full-fledged async example with callback/delagate firing**

A note on tread safety as far as expressions go:

>[!TIP]
>Like Mathematica expressions, Exprs are immutable, meaning they can never be changed once they are created. Operations that might appear to modify an Expr (like Delete) return new modified Exprs without changing the original. Because Exprs are immutable, they are also thread-safe, meaning that any number of threads can access a given Expr at the same time.


## Console Usage

> [!WARNING]  
> TODO
