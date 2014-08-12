#region Copyright © 2014 Michael Reukauff
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParseServiceTests.cs" company="Michael Reukauff">
//   Copyright © 2014 Michael Reukauff
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
#endregion

namespace MichaelReukauff.LexerTest
{
  using Microsoft.VisualStudio.TestTools.UnitTesting;

  using Lexer;

  [TestClass]
  public class ParseServiceTests
  {
    [TestMethod]
    public void ParseService_OK01()
    {
      const string text = "  service test1 { \r\n rpc test2 (test3) returns (test4); }";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      Assert.IsTrue(lex.ParseService());

      Assert.AreEqual(7, lex.Tokens.Count);
      Assert.AreEqual(0, lex.Errors.Count);
      Assert.AreEqual(15, lex.Index);

      Assert.AreEqual(2, lex.Tokens[0].Position);
      Assert.AreEqual(10, lex.Tokens[1].Position);
      Assert.AreEqual(21, lex.Tokens[2].Position);
      Assert.AreEqual(25, lex.Tokens[3].Position);
      Assert.AreEqual(32, lex.Tokens[4].Position);
      Assert.AreEqual(39, lex.Tokens[5].Position);
      Assert.AreEqual(48, lex.Tokens[6].Position);

      Assert.AreEqual(7, lex.Tokens[0].Length);
      Assert.AreEqual(5, lex.Tokens[1].Length);
      Assert.AreEqual(3, lex.Tokens[2].Length);
      Assert.AreEqual(5, lex.Tokens[3].Length);
      Assert.AreEqual(5, lex.Tokens[4].Length);
      Assert.AreEqual(7, lex.Tokens[5].Length);
      Assert.AreEqual(5, lex.Tokens[6].Length);

      Assert.AreEqual(0, lex.Tokens[0].Line);
      Assert.AreEqual(0, lex.Tokens[1].Line);
      Assert.AreEqual(1, lex.Tokens[2].Line);
      Assert.AreEqual(1, lex.Tokens[3].Line);
      Assert.AreEqual(1, lex.Tokens[4].Line);
      Assert.AreEqual(1, lex.Tokens[5].Line);
      Assert.AreEqual(1, lex.Tokens[6].Line);
    }

    [TestMethod]
    public void ParseService_OK02()
    {
      const string text = "  service test1 { \r\n option abc = 123; \r\n rpc test2 (test3) returns (test4); }";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      Assert.IsTrue(lex.ParseService());

      Assert.AreEqual(10, lex.Tokens.Count);
      Assert.AreEqual(0, lex.Errors.Count);
      Assert.AreEqual(21, lex.Index);
    }

    [TestMethod]
    public void ParseService_NOK01()
    {
      const string text = "  service test1 { \r\n rpc test2 (test3) returns (test4); ";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      Assert.IsFalse(lex.ParseService());

      Assert.AreEqual(7, lex.Tokens.Count);
      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(14, lex.Index);
    }

    [TestMethod]
    public void ParseService_NOK02()
    {
      const string text = "  service test1 { \r\n rpc test2 (test3) returns (test4)";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      Assert.IsFalse(lex.ParseService());

      Assert.AreEqual(7, lex.Tokens.Count);
      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(13, lex.Index);
    }

    [TestMethod]
    public void ParseService_NOK03()
    {
      const string text = "  service test1 { \r\n rpc test2 (test3) returns (test4";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      Assert.IsFalse(lex.ParseService());

      Assert.AreEqual(7, lex.Tokens.Count);
      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(12, lex.Index);
    }

    [TestMethod]
    public void ParseService_NOK04()
    {
      const string text = "  service test1 { \r\n rpc test2 (test3) returns (te";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      Assert.IsFalse(lex.ParseService());

      Assert.AreEqual(7, lex.Tokens.Count);
      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(12, lex.Index);
    }

    [TestMethod]
    public void ParseService_NOK05()
    {
      const string text = "  service test1 { \r\n rpc test2 (test3) returns (";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      Assert.IsFalse(lex.ParseService());

      Assert.AreEqual(6, lex.Tokens.Count);
      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(11, lex.Index);
    }

    [TestMethod]
    public void ParseService_NOK06()
    {
      const string text = "  service test1 { \r\n rpc test2 (test3) returns ";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      Assert.IsFalse(lex.ParseService());

      Assert.AreEqual(6, lex.Tokens.Count);
      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(10, lex.Index);
    }

    [TestMethod]
    public void ParseService_NOK07()
    {
      const string text = "  service test1 { \r\n rpc test2 (test3) retur";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      Assert.IsFalse(lex.ParseService());

      Assert.AreEqual(5, lex.Tokens.Count);
      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(9, lex.Index);
    }

    [TestMethod]
    public void ParseService_NOK08()
    {
      const string text = "  service test1 { \r\n rpc test2 (test3) ";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      Assert.IsFalse(lex.ParseService());

      Assert.AreEqual(5, lex.Tokens.Count);
      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(9, lex.Index);
    }

    [TestMethod]
    public void ParseService_NOK09()
    {
      const string text = "  service test1 { \r\n rpc test2 (test3";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      Assert.IsFalse(lex.ParseService());

      Assert.AreEqual(5, lex.Tokens.Count);
      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(8, lex.Index);
    }

    [TestMethod]
    public void ParseService_NOK10()
    {
      const string text = "  service test1 { \r\n rpc test2 (tes";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      Assert.IsFalse(lex.ParseService());

      Assert.AreEqual(5, lex.Tokens.Count);
      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(8, lex.Index);
    }

    [TestMethod]
    public void ParseService_NOK11()
    {
      const string text = "  service test1 { \r\n rpc test2 (";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      Assert.IsFalse(lex.ParseService());

      Assert.AreEqual(4, lex.Tokens.Count);
      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(7, lex.Index);
    }

    [TestMethod]
    public void ParseService_NOK12()
    {
      const string text = "  service test1 { \r\n rpc test2 ";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      Assert.IsFalse(lex.ParseService());

      Assert.AreEqual(4, lex.Tokens.Count);
      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(6, lex.Index);
    }

    [TestMethod]
    public void ParseService_NOK13()
    {
      const string text = "  service test1 { \r\n rpc te";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      Assert.IsFalse(lex.ParseService());

      Assert.AreEqual(4, lex.Tokens.Count);
      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(6, lex.Index);
    }

    [TestMethod]
    public void ParseService_NOK14()
    {
      const string text = "  service test1 { \r\n rpc ";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      Assert.IsFalse(lex.ParseService());

      Assert.AreEqual(3, lex.Tokens.Count);
      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(5, lex.Index);
    }

    [TestMethod]
    public void ParseService_NOK15()
    {
      const string text = "  service test1 { \r\n rp";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      Assert.IsFalse(lex.ParseService());

      Assert.AreEqual(2, lex.Tokens.Count);
      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(4, lex.Index);
    }

    [TestMethod]
    public void ParseService_NOK16()
    {
      const string text = "  service test1 { \r\n";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      Assert.IsFalse(lex.ParseService());

      Assert.AreEqual(2, lex.Tokens.Count);
      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(4, lex.Index);
    }

    [TestMethod]
    public void ParseService_NOK17()
    {
      const string text = "  service test1 { ";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      Assert.IsFalse(lex.ParseService());

      Assert.AreEqual(2, lex.Tokens.Count);
      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(3, lex.Index);
    }

    [TestMethod]
    public void ParseService_NOK18()
    {
      const string text = "  service test1 ";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      Assert.IsFalse(lex.ParseService());

      Assert.AreEqual(2, lex.Tokens.Count);
      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(2, lex.Index);
    }

    [TestMethod]
    public void ParseService_NOK19()
    {
      const string text = "  service te";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      Assert.IsFalse(lex.ParseService());

      Assert.AreEqual(2, lex.Tokens.Count);
      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(2, lex.Index);
    }

    [TestMethod]
    public void ParseService_NOK20()
    {
      const string text = "  service 1te";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      Assert.IsFalse(lex.ParseService());

      Assert.AreEqual(1, lex.Tokens.Count);
      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(1, lex.Index);
    }

    [TestMethod]
    public void ParseService_NOK21()
    {
      const string text = "  service";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      Assert.IsFalse(lex.ParseService());

      Assert.AreEqual(1, lex.Tokens.Count);
      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(1, lex.Index);
    }

    [TestMethod]
    public void ParseService_NOK22()
    {
      const string text = "  service sdf x";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      Assert.IsFalse(lex.ParseService());

      Assert.AreEqual(2, lex.Tokens.Count);
      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(2, lex.Index);
    }

    [TestMethod]
    public void ParseService_NOK23()
    {
      const string text = "  service test1 { \r\n rpc 2test2 ";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      Assert.IsFalse(lex.ParseService());

      Assert.AreEqual(3, lex.Tokens.Count);
      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(5, lex.Index);
    }

    [TestMethod]
    public void ParseService_NOK24()
    {
      const string text = "  service test1 { \r\n rpc test2 5";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      Assert.IsFalse(lex.ParseService());

      Assert.AreEqual(4, lex.Tokens.Count);
      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(6, lex.Index);
    }

    [TestMethod]
    public void ParseService_NOK25()
    {
      const string text = "  service test1 { \r\n rpc test2 (5)";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      Assert.IsFalse(lex.ParseService());

      Assert.AreEqual(4, lex.Tokens.Count);
      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(7, lex.Index);
    }

    [TestMethod]
    public void ParseService_NOK26()
    {
      const string text = "  service test1 { \r\n rpc test2 (hh g";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      Assert.IsFalse(lex.ParseService());

      Assert.AreEqual(5, lex.Tokens.Count);
      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(8, lex.Index);
    }

    [TestMethod]
    public void ParseService_NOK27()
    {
      const string text = "  service test1 { \r\n rpc test2 (h) returns d";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      Assert.IsFalse(lex.ParseService());

      Assert.AreEqual(6, lex.Tokens.Count);
      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(10, lex.Index);
    }

    [TestMethod]
    public void ParseService_NOK28()
    {
      const string text = "  service test1 { \r\n rpc test2 (h) returns (1";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      Assert.IsFalse(lex.ParseService());

      Assert.AreEqual(6, lex.Tokens.Count);
      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(11, lex.Index);
    }

    [TestMethod]
    public void ParseService_NOK29()
    {
      const string text = "  service test1 { \r\n rpc test2 (h) returns (ss x";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      Assert.IsFalse(lex.ParseService());

      Assert.AreEqual(7, lex.Tokens.Count);
      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(12, lex.Index);
    }

    [TestMethod]
    public void ParseService_NOK30()
    {
      const string text = "  service test1 { \r\n rpc test2 (h) returns (ss) s";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      Assert.IsFalse(lex.ParseService());

      Assert.AreEqual(7, lex.Tokens.Count);
      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(13, lex.Index);
    }

    [TestMethod]
    public void ParseService_NOK31()
    {
      const string text = "  service test1 { \r\n rpc test2 (h) returns (ss); d";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      Assert.IsFalse(lex.ParseService());

      Assert.AreEqual(7, lex.Tokens.Count);
      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(14, lex.Index);
    }

    [TestMethod]
    public void ParseService_NOK32()
    {
      const string text = "  service test1 { \r\n option abc = 123 \r\n rpc test2 (test3) returns (test4); }";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      Assert.IsFalse(lex.ParseService());

      Assert.AreEqual(5, lex.Tokens.Count);
      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(9, lex.Index);
    }
  }
}
