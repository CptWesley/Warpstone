[![CircleCI](https://circleci.com/gh/CptWesley/Warpstone.svg?style=shield)](https://circleci.com/gh/CptWesley/Warpstone)
[![CodeCov](https://codecov.io/gh/CptWesley/Warpstone/branch/master/graph/badge.svg)](https://codecov.io/gh/CptWesley/Warpstone/)
[![BetterCodeHub](https://bettercodehub.com/edge/badge/CptWesley/Warpstone?branch=master)](https://bettercodehub.com/results/CptWesley/Warpstone)
[![NuGet](https://img.shields.io/nuget/v/Warpstone.svg)](https://www.nuget.org/packages/Warpstone/)  

![Warpstone](https://raw.githubusercontent.com/CptWesley/Warpstone/master/logo.png)
# Warpstone
Parser combinator forged deep within Ikit's forges in the Under-City from a shard of Morrslieb itself.

## What is it?
Warpstone is a parser combinator library written in C# targetting .NET Standard 2.0, meaning that it can be used by any application running either .NET Framework >= 4.6.1, .NET Core >= 2.0 or Mono >= 5.4.
The main focus of the library is to provide a powerful lightweight parser combinator framework for developers to create their own parsers with.

## Why did you make it?
After following a course on compiler construction I was intrigued and wanted a way of parsing languages within code without requiring external tools to generate a parser for me first.
During my quest I stumbled upon the concept of parser combinators and started looking for existing libraries for .NET.
I soon stumbled upon [Pidgin](https://github.com/benjamin-hodgson/Pidgin).
After toying around with the library for a bit I felt dissatisfied with some of it's syntax and I still felt like I didn't fully understand the concepts it was using behind the scenes.
This lead me to create my own parser combinator library, with syntax inspired by Pidgin.

## Why the name? (Or: What are all those strange words in the description?!?!?)
Coming up with names is difficult, I wanted to get the project started and at the time I was consumed by the Warhammer Fantasy universe.

## Downloads
[NuGet](https://www.nuget.org/packages/Warpstone/)

## Usage
It's useful to take a look at one of the available example projects for JSON parsing and expression parsing.

To start off, one first needs to add (one of) the following imports:
```cs
using Warpstone;
using static Warpstone.Parsers.BasicParsers;
using static Warpstone.Parsers.CommonParsers;
using static Warpstone.Parsers.ExpressionParsers;
```