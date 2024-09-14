# ExampleExceptionUsage
This project looks at three ways to handle exceptions.
A simple library (FileGetter.cs) with three methods.

- String GetFileContentsSimple(String fullyQualifiedFileName)
- String GetFileContentsTraditionalStackedCatch(String fullyQualifiedFileName)
- String GetFileContentsFilteredCatch(String fullyQualifiedFileName)

Any suggestions on how to create and initialize variables within a filtered
catch block (once for all the filtered catch blocks) would be appreciated.

# Testing
There is an MSTest project included to test all three methods.

