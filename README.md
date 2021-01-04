# SimpleCQRS

This is a re-implementation of Greg Young's [SimpleCQRS](https://github.com/gregoryyoung/m-r) project, arguably the de-facto sample application for CQRS & Event Sourcing.  This re-implementation is by Craig Phillips.

The original project was written in C# 4.0/5.0 and targets the full .NET Framework 4.0/4.5.

This implementation is still written in C# but targets .NET Core 3.1 and uses ASP.NET Core as the GUI.

The intention is to evolve this project to address some of the [issues raised against the original project](https://github.com/gregoryyoung/m-r/issues) as well as attempting to improve the code with a more modern implementation using newer idiomatic constructs of the evolved .NET Core framework and the C# language whilst keeping the project simple and approachable as a reference project for how to build an application that leverages the CQRS and Event Sourcing patterns.
