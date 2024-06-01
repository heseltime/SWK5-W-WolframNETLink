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

> The Wolfram Symbolic Transfer Protocol (WSTP) is a platform-independent protocol for communicating between programs. In more concrete terms, it is a means to send and receive Wolfram Language **expressions**. WSTP is the means by which the notebook front end and kernel communicate with each other. It is also used by a large number of commercial and freeware applications and utilities that link the Wolfram Language and other programs or languages. It is implemented as a library of C-language functions. .NET/Link brings the capabilities of WSTP into .NET in a way that is simpler to use and much more powerful than the raw C-level API.

From [Calling the Wolfram Language from .NET](https://reference.wolfram.com/language/NETLink/tutorial/CallingTheWolframLanguageFromNET.html) (May 31st, 2024, my **highlight**: we look at [expression](https://reference.wolfram.com/language/NETLink/ref/net/Wolfram.NETLink.Expr.html) handling on the .NET side in example 2 in the following)

## Project Structure

Now we try some different MainImplementations do demonstratre so use cases.

```
WolframNETLink
│
├── Program.cs
├── MainImplementation1.cs
├── MainImplementation2.cs
└── MainImplementation3.cs
```

- **1**: Simple PrimeFactor example, interesting: conversion to array of the return values.
- **2**: Expr tests! See [Expression Wolfram Documentation](https://reference.wolfram.com/language/NETLink/ref/net/Wolfram.NETLink.Expr.html) that details how to handle representing WL expressions on the C# side.
- **3**: Full-fledged async example with callback/delagate firing.

## Console Usage

> [!IMPORTANT]  
> TODO
