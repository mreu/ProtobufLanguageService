#region Copyright © 2014 Michael Reukauff
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParseExtendTests.cs" company="Michael Reukauff">
//   Copyright © 2014 Michael Reukauff
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
#endregion

namespace MichaelReukauff.LexerTest
{
  using Microsoft.VisualStudio.TestTools.UnitTesting;

  using Lexer;

  [TestClass]
  public class ParseExtendTests
  {
    [TestMethod]
    public void ParseExtend_OK01()
    {
      const string text = "  extend Foo {\r\n optional int32 bar = 126;\r\n }";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      Assert.IsTrue(lex.ParseExtend());

      Assert.AreEqual(6, lex.Tokens.Count);
      Assert.AreEqual(0, lex.Errors.Count);
      Assert.AreEqual(12, lex.Index);

      Assert.AreEqual(2, lex.Tokens[0].Position);
      Assert.AreEqual(9, lex.Tokens[1].Position);
      Assert.AreEqual(17, lex.Tokens[2].Position);
      Assert.AreEqual(26, lex.Tokens[3].Position);
      Assert.AreEqual(32, lex.Tokens[4].Position);
      Assert.AreEqual(38, lex.Tokens[5].Position);

      Assert.AreEqual(6, lex.Tokens[0].Length);
      Assert.AreEqual(3, lex.Tokens[1].Length);
      Assert.AreEqual(8, lex.Tokens[2].Length);
      Assert.AreEqual(5, lex.Tokens[3].Length);
      Assert.AreEqual(3, lex.Tokens[4].Length);
      Assert.AreEqual(3, lex.Tokens[5].Length);

      Assert.AreEqual(CodeType.TopLevelCmd, lex.Tokens[0].CodeType);
      Assert.AreEqual(CodeType.SymRef, lex.Tokens[1].CodeType);
      Assert.AreEqual(CodeType.FieldRule, lex.Tokens[2].CodeType);
      Assert.AreEqual(CodeType.Keyword, lex.Tokens[3].CodeType);
      Assert.AreEqual(CodeType.SymDef, lex.Tokens[4].CodeType);
      Assert.AreEqual(CodeType.Number, lex.Tokens[5].CodeType);
    }

    [TestMethod]
    public void ParseExtend_NOK01()
    {
      const string text = "  extend";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      Assert.IsFalse(lex.ParseExtend());

      Assert.AreEqual(1, lex.Tokens.Count);
      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(1, lex.Index);
    }

    [TestMethod]
    public void ParseExtend_NOK02()
    {
      const string text = "  extend 1Foo {\r\n optional int32 bar = 126;\r\n }";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      Assert.IsFalse(lex.ParseExtend());

      Assert.AreEqual(1, lex.Tokens.Count);
      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(1, lex.Index);
    }

    [TestMethod]
    public void ParseExtend_NOK03()
    {
      const string text = "  extend Foo";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      Assert.IsFalse(lex.ParseExtend());

      Assert.AreEqual(2, lex.Tokens.Count);
      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(2, lex.Index);
    }

    [TestMethod]
    public void ParseExtend_NOK04()
    {
      const string text = "  extend Foo x\r\n optional int32 bar = 126;\r\n }";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      Assert.IsFalse(lex.ParseExtend());

      Assert.AreEqual(2, lex.Tokens.Count);
      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(2, lex.Index);
    }

    [TestMethod]
    public void ParseExtend_NOK05()
    {
      const string text = "  extend Foo {";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      Assert.IsFalse(lex.ParseExtend());

      Assert.AreEqual(2, lex.Tokens.Count);
      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(3, lex.Index);
    }

    [TestMethod]
    public void ParseExtend_NOK06()
    {
      const string text = "  extend Foo {\r\n optional int32 bar x 126;\r\n }";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      Assert.IsFalse(lex.ParseExtend());

      Assert.AreEqual(5, lex.Tokens.Count);
      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(7, lex.Index);
    }

    [TestMethod]
    public void ParseExtend_NOK07()
    {
      const string text = "  extend Foo {\r\n optional int32 bar = 126;";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      Assert.IsFalse(lex.ParseExtend());

      Assert.AreEqual(6, lex.Tokens.Count);
      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(10, lex.Index);
    }

    [TestMethod]
    public void ParseExtend_NOK08()
    {
      const string text = "  extend Foo {\r\n optional int32 bar = 126;\r\n x";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      Assert.IsFalse(lex.ParseExtend());

      Assert.AreEqual(6, lex.Tokens.Count);
      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(12, lex.Index);
    }
  }
}
