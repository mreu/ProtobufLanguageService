#region Copyright © 2014 Michael Reukauff
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParsePackageTests.cs" company="Michael Reukauff">
//   Copyright © 2014 Michael Reukauff
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
#endregion

namespace MichaelReukauff.LexerTest
{
  using Microsoft.VisualStudio.TestTools.UnitTesting;

  using Lexer;

  [TestClass]
  public class ParsePackageTests
  {
    [TestMethod]
    public void Package_OK1()
    {
      const string text = "    package Test1;";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      lex.ParsePackage();

      Assert.AreEqual(2, lex.Tokens.Count);
      Assert.AreEqual(4, lex.Tokens[0].Position);
      Assert.AreEqual(7, lex.Tokens[0].Length);
      Assert.AreEqual(CodeType.TopLevelCmd, lex.Tokens[0].CodeType);
      Assert.AreEqual(0, lex.Line);
      Assert.AreEqual(12, lex.Tokens[1].Position);
      Assert.AreEqual(5, lex.Tokens[1].Length);
      Assert.AreEqual(CodeType.Namespace, lex.Tokens[1].CodeType);
      Assert.AreEqual(0, lex.Line);
    }

    [TestMethod]
    public void Package_OK2()
    {
      const string text = "    package Test1.Test2.Test3;";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      lex.ParsePackage();

      Assert.AreEqual(4, lex.Tokens.Count);
      Assert.AreEqual(4, lex.Tokens[0].Position);
      Assert.AreEqual(7, lex.Tokens[0].Length);
      Assert.AreEqual(CodeType.TopLevelCmd, lex.Tokens[0].CodeType);
      Assert.AreEqual(0, lex.Line);
      Assert.AreEqual(12, lex.Tokens[1].Position);
      Assert.AreEqual(5, lex.Tokens[1].Length);
      Assert.AreEqual(CodeType.Namespace, lex.Tokens[1].CodeType);
      Assert.AreEqual(0, lex.Line);
      Assert.AreEqual(18, lex.Tokens[2].Position);
      Assert.AreEqual(5, lex.Tokens[2].Length);
      Assert.AreEqual(CodeType.Namespace, lex.Tokens[2].CodeType);
      Assert.AreEqual(0, lex.Line);
      Assert.AreEqual(24, lex.Tokens[3].Position);
      Assert.AreEqual(5, lex.Tokens[3].Length);
      Assert.AreEqual(CodeType.Namespace, lex.Tokens[3].CodeType);
      Assert.AreEqual(0, lex.Line);
    }

    [TestMethod]
    public void Package_NOK01()
    {
      const string text = "    package Teäst1;";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      lex.ParsePackage();

      Assert.AreEqual(2, lex.Tokens.Count);
      Assert.AreEqual(4, lex.Tokens[0].Position);
      Assert.AreEqual(7, lex.Tokens[0].Length);
      Assert.AreEqual(CodeType.TopLevelCmd, lex.Tokens[0].CodeType);
      Assert.AreEqual(0, lex.Line);

      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(12, lex.Errors[0].Position);
      Assert.AreEqual(6, lex.Errors[0].Length);
      Assert.AreEqual(0, lex.Line);
    }

    [TestMethod]
    public void Package_NOK02()
    {
      const string text = "    package Teäst1.kjhkj.jjjj;";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      lex.ParsePackage();

      Assert.AreEqual(4, lex.Tokens.Count);
      Assert.AreEqual(4, lex.Tokens[0].Position);
      Assert.AreEqual(7, lex.Tokens[0].Length);
      Assert.AreEqual(CodeType.TopLevelCmd, lex.Tokens[0].CodeType);
      Assert.AreEqual(0, lex.Line);

      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(12, lex.Errors[0].Position);
      Assert.AreEqual(6, lex.Errors[0].Length);
      Assert.AreEqual(0, lex.Line);
    }

    [TestMethod]
    public void Package_NOK03()
    {
      const string text = "    package"; // still not finished line

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      lex.ParsePackage();

      Assert.AreEqual(1, lex.Tokens.Count);
      Assert.AreEqual(4, lex.Tokens[0].Position);
      Assert.AreEqual(7, lex.Tokens[0].Length);
      Assert.AreEqual(CodeType.TopLevelCmd, lex.Tokens[0].CodeType);
      Assert.AreEqual(0, lex.Line);

      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(11, lex.Errors[0].Position);
      Assert.AreEqual(1, lex.Errors[0].Length);
      Assert.AreEqual(0, lex.Line);
    }

    [TestMethod]
    public void Package_NOK04()
    {
      const string text = "    package dfs"; // still not finished line

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      lex.ParsePackage();

        Assert.AreEqual(2, lex.Tokens.Count);
      Assert.AreEqual(12, lex.Tokens[1].Position);
      Assert.AreEqual(3, lex.Tokens[1].Length);
      Assert.AreEqual(CodeType.Namespace, lex.Tokens[1].CodeType);
      Assert.AreEqual(0, lex.Line);

      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(15, lex.Errors[0].Position);
      Assert.AreEqual(1, lex.Errors[0].Length);
      Assert.AreEqual(0, lex.Line);
    }

    [TestMethod]
    public void Package_NOK05()
    {
      const string text = "    package dfs."; // still not finished line

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      lex.ParsePackage();

      Assert.AreEqual(2, lex.Tokens.Count);
      Assert.AreEqual(12, lex.Tokens[1].Position);
      Assert.AreEqual(3, lex.Tokens[1].Length);
      Assert.AreEqual(CodeType.Namespace, lex.Tokens[1].CodeType);
      Assert.AreEqual(0, lex.Line);

      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(16, lex.Errors[0].Position);
      Assert.AreEqual(1, lex.Errors[0].Length);
      Assert.AreEqual(0, lex.Line);
    }

    [TestMethod]
    public void Package_NOK06()
    {
      const string text = "    package dfs.abc"; // still not finished line

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      lex.ParsePackage();

      Assert.AreEqual(3, lex.Tokens.Count);
      Assert.AreEqual(16, lex.Tokens[2].Position);
      Assert.AreEqual(3, lex.Tokens[2].Length);
      Assert.AreEqual(CodeType.Namespace, lex.Tokens[2].CodeType);
      Assert.AreEqual(0, lex.Line);

      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(19, lex.Errors[0].Position);
      Assert.AreEqual(1, lex.Errors[0].Length);
      Assert.AreEqual(0, lex.Line);
    }

    [TestMethod]
    public void Package_NOK07()
    {
      const string text = "    package dfs.abc."; // still not finished line

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      lex.ParsePackage();

      Assert.AreEqual(3, lex.Tokens.Count);
      Assert.AreEqual(16, lex.Tokens[2].Position);
      Assert.AreEqual(3, lex.Tokens[2].Length);
      Assert.AreEqual(CodeType.Namespace, lex.Tokens[2].CodeType);
      Assert.AreEqual(0, lex.Line);

      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(20, lex.Errors[0].Position);
      Assert.AreEqual(1, lex.Errors[0].Length);
      Assert.AreEqual(0, lex.Line);
    }

    [TestMethod]
    public void Package_NOK08()
    {
      const string text = "    package dfs.abc;";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text), HasPackage = true };

      lex.ParsePackage();

      Assert.AreEqual(3, lex.Tokens.Count);
      Assert.AreEqual(16, lex.Tokens[2].Position);
      Assert.AreEqual(3, lex.Tokens[2].Length);
      Assert.AreEqual(CodeType.Namespace, lex.Tokens[2].CodeType);
      Assert.AreEqual(0, lex.Line);

      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(4, lex.Errors[0].Position);
      Assert.AreEqual(7, lex.Errors[0].Length);
      Assert.AreEqual(0, lex.Line);
    }

    [TestMethod]
    public void Package_NOK09()
    {
      const string text = "    package dfs\r\naaa";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      lex.ParsePackage();

      Assert.AreEqual(2, lex.Tokens.Count);
      Assert.AreEqual(12, lex.Tokens[1].Position);
      Assert.AreEqual(3, lex.Tokens[1].Length);
      Assert.AreEqual(CodeType.Namespace, lex.Tokens[1].CodeType);
      Assert.AreEqual(0, lex.Line);

      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(17, lex.Errors[0].Position);
      Assert.AreEqual(3, lex.Errors[0].Length);
    }

    [TestMethod]
    public void Package_NOK10()
    {
      const string text = "    package dfs\r\n";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      lex.ParsePackage();

      Assert.AreEqual(2, lex.Tokens.Count);
      Assert.AreEqual(12, lex.Tokens[1].Position);
      Assert.AreEqual(3, lex.Tokens[1].Length);
      Assert.AreEqual(CodeType.Namespace, lex.Tokens[1].CodeType);
      Assert.AreEqual(0, lex.Line);

      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(17, lex.Errors[0].Position);
      Assert.AreEqual(1, lex.Errors[0].Length);
    }
  }
}
