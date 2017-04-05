# ProtobufLanguageService
Protobuf Editor for Visual Studio with Intellisense and Syntax Coloring for googles protobuf syntax.

You can download this Visual Studio Extension from the [Visual Studio Gallery](https://visualstudiogallery.msdn.microsoft.com/4bc0f38c-b058-4e05-ae38-155e053c19c5)
Source code is found [here](https://github.com/mreu/ProtobufLanguageService)

This Visual Studio Language Service supports googles protobuf definitions in the following ways:
* Brace Matching
* Code Completion
* Highlight Word
* Outlining
* Quickinfo
* Syntax Coloring 

C# code generation will follow in Version 2.0 (this will be optional and can be switched off). Supported are protbuf-net from Marc Gravell and ~~protobuf-csharp from Jon Skeet~~ the C# code generation in the new protoc.exe.

The syntax coloring is now based on my own lexer/parser. Maybe this will change in the future and I use the output from googles protoc.exe (the protobuf compiler).

The standard colors are used for syntax highlighting with the exception of the Enum field-names. You can set the colors in Visual Studio in Tools -> Options -> Environment -> Fonts and Colors. The name of the colors starts with protobuf. Please scroll down.

Errors are shown in the protobuf section of the output pane and in the error pane with line and column number. You can double click to go directly to the syntax error.

If you have any suggestions, please feel free to add an issue.

Changelog:
* 1.10.0 - April 5, 2017
  * Support of Visual Studio 2017
* 1.9.0 - October 29, 2016
  * Support of maps
* 1.8.2 - October 23, 2016
  * Support of namespaces in rpc request and response names 
* 1.8.1 - September 18, 2016
  * Added stream keyword in rpc definition 
* 1.8.0 - July 3, 2016
  * First implementation of proto3 syntax 
* 1.7.4 - August 8, 2015
  * Fixed "Install Error : Microsoft.VisualStudio.ExtensionManager.MissingTargetFrameworkException: The extension 'Protobuf Language Service' requires a version of the .NET Framework that is not installed." 
* 1.7.3 - July 18, 2015
  * Fixed name resolution. Names beginning with a dot now works 
* 1.7.2 - May 14, 2015
  * Support for Visual Studio 2015 RC added 
* 1.7.1 - February 06, 2015
  * Some linebreaks are not handled correctly 
* 1.7.0 - January 31, 2015 - Added better support of options 
  * Fixed: crash if complete text was deleted 
* 1.6.1 - January 04, 2015
  * Fixed: crash in brace matching module 
  * Fixed: messages, enums etc. must not end with a semicolon but can
* 1.6.0 - December 29, 2014
  * Support of protobuf 2.6 
* 1.5.1 - November 14, 2014
  * Support of oneof inside messages
* 1.5.0 - November 2, 2014
  * Support of oneof 
* 1.4.0 - October 30, 2014
  * Allow extended types from another package 
* 1.3.0 - August 12, 2014
  * Multiple rpcs in a service 
* 1.2.0 - June 15, 2014
  * No autocomplete inside of comments 
* 1.1.0 - May 27, 2014
  * Support of hexadecimal values (i.e. 0xnn) 
* 1.0.0 - Jan. 26, 2014
  * Final release 
* 0.8.0 - Jan. 03, 2014
  * This version was not published, because of my new lexer/parser
* 0.7.0 - Dec. 20, 2013
  * Improvments in the parser 
  * Syntax errors are shown with a red squiggle line
* 0.6.0 - Dec. 6, 2013
  * This version was not published, because of the new lexer/parser 
  * Visual Studio 2013 Support
* 0.5.1 - Nov. 25, 2013
  * Change of Bitmap and Url
* 0.5.0 - Nov. 23, 2013
  * First public version 
