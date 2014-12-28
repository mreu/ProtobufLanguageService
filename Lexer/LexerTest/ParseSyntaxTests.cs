#region Copyright © 2014 Michael Reukauff
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParseSyntaxTests.cs" company="Michael Reukauff">
//   Copyright © 2014 Michael Reukauff
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
#endregion

namespace MichaelReukauff.LexerTest
{
    using MichaelReukauff.Lexer;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ParseSyntaxTests
    {
        [TestMethod]
        public void ParseEnum_OK01()
        {
            const string text = "  syntax = \"proto2\";\r\n";

            var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

            Assert.IsTrue(lex.ParseSyntax());

            Assert.AreEqual(2, lex.Tokens.Count);
            Assert.AreEqual(0, lex.Errors.Count);
            Assert.AreEqual(7, lex.Index);
        }

        [TestMethod]
        public void ParseEnum_OK02()
        {
            const string text = "  syntax = \"proto3\";\r\n";

            var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

            Assert.IsTrue(lex.ParseSyntax());

            Assert.AreEqual(2, lex.Tokens.Count);
            Assert.AreEqual(0, lex.Errors.Count);
            Assert.AreEqual(7, lex.Index);
        }

        [TestMethod]
        public void ParseEnum_NOK01()
        {
            const string text = "  syntax = \"proto3\r\n";

            var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

            Assert.IsFalse(lex.ParseSyntax());

            Assert.AreEqual(2, lex.Tokens.Count);
            Assert.AreEqual(1, lex.Errors.Count);
            Assert.AreEqual(5, lex.Index);
        }

        [TestMethod]
        public void ParseEnum_NOK02()
        {
            const string text = "  syntax = \"proto3;\r\n";

            var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

            Assert.IsFalse(lex.ParseSyntax());

            Assert.AreEqual(2, lex.Tokens.Count);
            Assert.AreEqual(1, lex.Errors.Count);
            Assert.AreEqual(4, lex.Index);
        }


        [TestMethod]
        public void ParseEnum_NOK03()
        {
            const string text = "  syntax = \"proto3\"\r\n";

            var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

            Assert.IsFalse(lex.ParseSyntax());

            Assert.AreEqual(2, lex.Tokens.Count);
            Assert.AreEqual(1, lex.Errors.Count);
            Assert.AreEqual(6, lex.Index);
        }

        [TestMethod]
        public void ParseEnum_NOK04()
        {
            const string text = "  syntax = \"proto3\" x\r\n";

            var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

            Assert.IsFalse(lex.ParseSyntax());

            Assert.AreEqual(2, lex.Tokens.Count);
            Assert.AreEqual(1, lex.Errors.Count);
            Assert.AreEqual(5, lex.Index);
        }

        [TestMethod]
        public void ParseEnum_NOK05()
        {
            const string text = "  syntax = \"proto\";\r\n";

            var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

            Assert.IsFalse(lex.ParseSyntax());

            Assert.AreEqual(1, lex.Tokens.Count);
            Assert.AreEqual(1, lex.Errors.Count);
            Assert.AreEqual(3, lex.Index);
        }

        [TestMethod]
        public void ParseEnum_NOK06()
        {
            const string text = "  syntax = \"proto\"\r\n";

            var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

            Assert.IsFalse(lex.ParseSyntax());

            Assert.AreEqual(1, lex.Tokens.Count);
            Assert.AreEqual(1, lex.Errors.Count);
            Assert.AreEqual(3, lex.Index);
        }

        [TestMethod]
        public void ParseEnum_NOK07()
        {
            const string text = "  syntax = \"proto\r\n";

            var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

            Assert.IsFalse(lex.ParseSyntax());

            Assert.AreEqual(1, lex.Tokens.Count);
            Assert.AreEqual(1, lex.Errors.Count);
            Assert.AreEqual(3, lex.Index);
        }

        [TestMethod]
        public void ParseEnum_NOK08()
        {
            const string text = "  syntax = \"\";\r\n";

            var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

            Assert.IsFalse(lex.ParseSyntax());

            Assert.AreEqual(1, lex.Tokens.Count);
            Assert.AreEqual(1, lex.Errors.Count);
            Assert.AreEqual(3, lex.Index);
        }

        [TestMethod]
        public void ParseEnum_NOK09()
        {
            const string text = "  syntax = proto\";\r\n";

            var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

            Assert.IsFalse(lex.ParseSyntax());

            Assert.AreEqual(1, lex.Tokens.Count);
            Assert.AreEqual(1, lex.Errors.Count);
            Assert.AreEqual(2, lex.Index);
        }

        [TestMethod]
        public void ParseEnum_NOK10()
        {
            const string text = "  syntax = ;\r\n";

            var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

            Assert.IsFalse(lex.ParseSyntax());

            Assert.AreEqual(1, lex.Tokens.Count);
            Assert.AreEqual(1, lex.Errors.Count);
            Assert.AreEqual(2, lex.Index);
        }

        [TestMethod]
        public void ParseEnum_NOK11()
        {
            const string text = "  syntax = \r\n";

            var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

            Assert.IsFalse(lex.ParseSyntax());

            Assert.AreEqual(1, lex.Tokens.Count);
            Assert.AreEqual(1, lex.Errors.Count);
            Assert.AreEqual(3, lex.Index);
        }

        [TestMethod]
        public void ParseEnum_NOK12()
        {
            const string text = "  syntax \"proto\"";

            var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

            Assert.IsFalse(lex.ParseSyntax());

            Assert.AreEqual(1, lex.Tokens.Count);
            Assert.AreEqual(1, lex.Errors.Count);
            Assert.AreEqual(1, lex.Index);
        }

        [TestMethod]
        public void ParseEnum_NOK13()
        {
            const string text = "  syntax = proto\"";

            var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

            Assert.IsFalse(lex.ParseSyntax());

            Assert.AreEqual(1, lex.Tokens.Count);
            Assert.AreEqual(1, lex.Errors.Count);
            Assert.AreEqual(2, lex.Index);
        }

        [TestMethod]
        public void ParseEnum_NOK14()
        {
            const string text = "  syntax = ";

            var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

            Assert.IsFalse(lex.ParseSyntax());

            Assert.AreEqual(1, lex.Tokens.Count);
            Assert.AreEqual(1, lex.Errors.Count);
            Assert.AreEqual(2, lex.Index);
        }

        [TestMethod]
        public void ParseEnum_NOK15()
        {
            const string text = "  syntax = \"";

            var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

            Assert.IsFalse(lex.ParseSyntax());

            Assert.AreEqual(1, lex.Tokens.Count);
            Assert.AreEqual(1, lex.Errors.Count);
            Assert.AreEqual(3, lex.Index);
        }

        [TestMethod]
        public void ParseEnum_NOK16()
        {
            const string text = "  syntax ";

            var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

            Assert.IsFalse(lex.ParseSyntax());

            Assert.AreEqual(1, lex.Tokens.Count);
            Assert.AreEqual(1, lex.Errors.Count);
            Assert.AreEqual(1, lex.Index);
        }
    }
}