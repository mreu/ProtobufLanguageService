#region Copyright © 2014 Michael Reukauff
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParseEnumTest.cs" company="Michael Reukauff">
//   Copyright © 2014 Michael Reukauff
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
#endregion

namespace MichaelReukauff.LexerTest
{
  using Microsoft.VisualStudio.TestTools.UnitTesting;

  using Lexer;

  [TestClass]
  public class ParseEnumTest
  {
    [TestMethod]
    public void ParseEnum_OK01()
    {
      const string text = "  enum text { \r\n ABC = 1; \r\n DEF = 2; \r\n }";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      Assert.IsTrue(lex.ParseEnum(true));

      Assert.AreEqual(6, lex.Tokens.Count);
      Assert.AreEqual(0, lex.Errors.Count);
      Assert.AreEqual(15, lex.Index);
    }

    [TestMethod]
    public void ParseEnum_OK02()
    {
      const string text = "  enum text { \r\n option allow_alias = true; \r\n ABC = 1; \r\n DEF = 2; \r\n }";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      Assert.IsTrue(lex.ParseEnum(false));

      Assert.AreEqual(9, lex.Tokens.Count);
      Assert.AreEqual(0, lex.Errors.Count);
      Assert.AreEqual(21, lex.Index);
    }

    [TestMethod]
    public void ParseEnum_OK03()
    {
      const string text = "  enum text { \r\n option allow_alias = true; \r\n ABC = 1 [abc=true]; \r\n DEF = 2; \r\n }";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      Assert.IsTrue(lex.ParseEnum(false));

      Assert.AreEqual(11, lex.Tokens.Count);
      Assert.AreEqual(0, lex.Errors.Count);
      Assert.AreEqual(26, lex.Index);
    }

    [TestMethod]
    public void ParseEnum_NOK01()
    {
      const string text = "  enum text \r\n  { \r\n DEF = 2; \r\n "; // closing bracket is missed

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      Assert.IsFalse(lex.ParseEnum(true));

      Assert.AreEqual(4, lex.Tokens.Count);
      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(10, lex.Index);
    }

    [TestMethod]
    public void ParseEnum_NOK02()
    {
      const string text = "  enum text \r\n  { \r\n DEF = 2;"; // closing bracket is missed

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      Assert.IsFalse(lex.ParseEnum(true));

      Assert.AreEqual(4, lex.Tokens.Count);
      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(9, lex.Index);
    }

    [TestMethod]
    public void ParseEnum_NOK03()
    {
      const string text = "  enum text \r\n  { \r\n DEF = 2";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      Assert.IsFalse(lex.ParseEnum(true));

      Assert.AreEqual(4, lex.Tokens.Count);
      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(8, lex.Index);
    }

    [TestMethod]
    public void ParseEnum_NOK04()
    {
      const string text = "  enum text \r\n  { \r\n DEF = ";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      Assert.IsFalse(lex.ParseEnum(true));

      Assert.AreEqual(3, lex.Tokens.Count);
      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(7, lex.Index);
    }

    [TestMethod]
    public void ParseEnum_NOK05()
    {
      const string text = "  enum text \r\n  { \r\n DEF";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      Assert.IsFalse(lex.ParseEnum(true));

      Assert.AreEqual(3, lex.Tokens.Count);
      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(6, lex.Index);
    }

    [TestMethod]
    public void ParseEnum_NOK06()
    {
      const string text = "  enum text \r\n  { \r\n ";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      Assert.IsFalse(lex.ParseEnum(true));

      Assert.AreEqual(2, lex.Tokens.Count);
      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(5, lex.Index);
    }

    [TestMethod]
    public void ParseEnum_NOK07()
    {
      const string text = "  enum text \r\n";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      Assert.IsFalse(lex.ParseEnum(true));

      Assert.AreEqual(2, lex.Tokens.Count);
      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(3, lex.Index);
    }

    [TestMethod]
    public void ParseEnum_NOK08()
    {
      const string text = "  enum text";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      Assert.IsFalse(lex.ParseEnum(true));

      Assert.AreEqual(2, lex.Tokens.Count);
      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(2, lex.Index);
    }

    [TestMethod]
    public void ParseEnum_NOK09()
    {
      const string text = "  enum ";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      Assert.IsFalse(lex.ParseEnum(true));

      Assert.AreEqual(1, lex.Tokens.Count);
      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(1, lex.Index);
    }

    [TestMethod]
    public void ParseEnum_NOK10()
    {
      const string text = "  enum text \r\n  { \r\n DEF = 2; \r\n \r\n"; // closing bracket is missed

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      Assert.IsFalse(lex.ParseEnum(true));

      Assert.AreEqual(4, lex.Tokens.Count);
      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(11, lex.Index);
      Assert.AreEqual(4, lex.Line);
    }

    [TestMethod]
    public void ParseEnum_NOK11()
    {
      const string text = "  enum 1text \r\n  { \r\n DEF = 2; \r\n ";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      Assert.IsFalse(lex.ParseEnum(true));

      Assert.AreEqual(1, lex.Tokens.Count);
      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(1, lex.Index);
    }

    [TestMethod]
    public void ParseEnum_NOK12()
    {
      const string text = "  enum text \r\n  x \r\n DEF = 2; \r\n ";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      Assert.IsFalse(lex.ParseEnum(true));

      Assert.AreEqual(2, lex.Tokens.Count);
      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(3, lex.Index);
    }

    [TestMethod]
    public void ParseEnum_NOK13()
    {
      const string text = "  enum text \r\n  { \r\n DEF x 2; \r\n ";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      Assert.IsFalse(lex.ParseEnum(true));

      Assert.AreEqual(3, lex.Tokens.Count);
      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(6, lex.Index);
    }

    [TestMethod]
    public void ParseEnum_NOK14()
    {
      const string text = "  enum text \r\n  { \r\n DEF = 2 x \r\n ";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      Assert.IsFalse(lex.ParseEnum(true));

      Assert.AreEqual(4, lex.Tokens.Count);
      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(8, lex.Index);
    }

    [TestMethod]
    public void ParseEnum_NOK15()
    {
      const string text = "  enum text \r\n  { \r\n DEF = x; \r\n ";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      Assert.IsFalse(lex.ParseEnum(true));

      Assert.AreEqual(3, lex.Tokens.Count);
      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(7, lex.Index);
    }

    [TestMethod]
    public void ParseEnum_NOK16()
    {
      const string text = "  enum text { \r\n option allow_alias = true \r\n ABC = 1; \r\n DEF = 2; \r\n }";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      Assert.IsFalse(lex.ParseEnum(false));

      Assert.AreEqual(5, lex.Tokens.Count);
      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(9, lex.Index);
    }

    [TestMethod]
    public void ParseEnum_NOK17()
    {
      const string text = "  enum text { \r\n option allow_alias = true; \r\n ABC = 1 [xxx x true]\r\n DEF = 2; \r\n }";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      Assert.IsFalse(lex.ParseEnum(false));

      Assert.AreEqual(8, lex.Tokens.Count);
      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(15, lex.Index);
    }

    [TestMethod]
    public void ParseEnum_NOK18()
    {
      const string text = "  enum text { \r\n option allow_alias = true; \r\n 1ABC = 1 [xxx x true]\r\n DEF = 2; \r\n }";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      Assert.IsFalse(lex.ParseEnum(false));

      Assert.AreEqual(5, lex.Tokens.Count);
      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(10, lex.Index);
    }
  }
}
