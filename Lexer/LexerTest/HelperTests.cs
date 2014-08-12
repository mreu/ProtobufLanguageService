#region Copyright © 2014 Michael Reukauff
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HelperTests.cs" company="Michael Reukauff">
//   Copyright © 2014 Michael Reukauff
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
#endregion

namespace MichaelReukauff.LexerTest
{
  using Microsoft.VisualStudio.TestTools.UnitTesting;

  using Lexer;

  /// <summary>
  /// Summary description for UnitTest1
  /// </summary>
  [TestClass]
  public class HelperTests
  {
    #region SplitText
    [TestMethod]
    public void SplitText()
    {
      const string text = "aa bbbbb cccc //j;{}/**/=[]()\"text ccc d\"\r\n asdas";

      var res = Helper.SplitText(text);

      Assert.AreEqual(22, res.Count);
    }

    [TestMethod]
    public void SplitText_NotClosedString()
    {
      const string text = "aaa bbb \"text ccc d eee \r\n fff";

      var res = Helper.SplitText(text);

      Assert.AreEqual(9, res.Count);
    }

    #endregion

    #region IsIdentifier
    [TestMethod]
    public void IsIdentifier_OK1()
    {
      const string text = "abcdABCD098_lkk";

      var res = Helper.IsIdentifier(text);

      Assert.AreEqual(true, res);
    }

    [TestMethod]
    public void IsIdentifier_OK2()
    {
      const string text = "_abcdABCD098_lkk";

      var res = Helper.IsIdentifier(text);

      Assert.AreEqual(true, res);
    }

    [TestMethod]
    public void IsIdentifier_NOK1()
    {
      const string text = "1abcdABCD098_lkk"; // number in first place

      var res = Helper.IsIdentifier(text);

      Assert.AreEqual(false, res);
    }

    [TestMethod]
    public void IsIdentifier_NOK2()
    {
      const string text = "afäbcdABCD098_lkk"; // invalid character

      var res = Helper.IsIdentifier(text);

      Assert.AreEqual(false, res);
    }

    #endregion

    #region IsFieldRule
    [TestMethod]
    public void IsFieldRule_OK1()
    {
      var res = Helper.IsFieldRule("optional");
      Assert.AreEqual(true, res);
    }

    [TestMethod]
    public void IsFieldRule_OK2()
    {
      var res = Helper.IsFieldRule("repeated");
      Assert.AreEqual(true, res);
    }

    [TestMethod]
    public void IsFieldRule_OK3()
    {
      var res = Helper.IsFieldRule("required");
      Assert.AreEqual(true, res);
    }

    [TestMethod]
    public void IsFieldRule_NOK()
    {
      var res = Helper.IsFieldRule("farz");
      Assert.AreEqual(false, res);
    }

    #endregion

    #region IsTrueOrFalse
    [TestMethod]
    public void IsTrueOrFalse_OK1()
    {
      var res = Helper.IsTrueOrFalse("true");
      Assert.AreEqual(true, res);
    }

    [TestMethod]
    public void IsTrueOrFalse_OK2()
    {
      var res = Helper.IsTrueOrFalse("false");
      Assert.AreEqual(true, res);
    }

    [TestMethod]
    public void IsTrueOrFalse_NOK()
    {
      var res = Helper.IsTrueOrFalse("farz");
      Assert.AreEqual(false, res);
    }

    #endregion

    #region GetFieldType
    [TestMethod]
    public void GetFieldType_Test()
    {
      Assert.AreEqual(FieldType.TypeUnknown, Helper.GetFieldType("farz"));
      Assert.AreEqual(FieldType.TypeDouble, Helper.GetFieldType("double"));
      Assert.AreEqual(FieldType.TypeFloat, Helper.GetFieldType("float"));
      Assert.AreEqual(FieldType.TypeUint64, Helper.GetFieldType("uint64"));
      Assert.AreEqual(FieldType.TypeUint32, Helper.GetFieldType("uint32"));
      Assert.AreEqual(FieldType.TypeFixed64, Helper.GetFieldType("fixed64"));
      Assert.AreEqual(FieldType.TypeFixed32, Helper.GetFieldType("fixed32"));
      Assert.AreEqual(FieldType.TypeSfixed32, Helper.GetFieldType("sfixed32"));
      Assert.AreEqual(FieldType.TypeSfixed64, Helper.GetFieldType("sfixed64"));
      Assert.AreEqual(FieldType.TypeInt32, Helper.GetFieldType("int32"));
      Assert.AreEqual(FieldType.TypeInt64, Helper.GetFieldType("int64"));
      Assert.AreEqual(FieldType.TypeSint32, Helper.GetFieldType("sint32"));
      Assert.AreEqual(FieldType.TypeSint64, Helper.GetFieldType("sint64"));
      Assert.AreEqual(FieldType.TypeBool, Helper.GetFieldType("bool"));
      Assert.AreEqual(FieldType.TypeString, Helper.GetFieldType("string"));
      Assert.AreEqual(FieldType.TypeBytes, Helper.GetFieldType("bytes"));
    }

    #endregion

    [TestMethod]
    public void IsInteger_Test()
    {
      Assert.IsTrue(Helper.IsInteger("123"));
      Assert.IsTrue(Helper.IsInteger("-123"));
      Assert.IsFalse(Helper.IsInteger("-9223372036854775809"));
      Assert.IsFalse(Helper.IsInteger("9223372036854775808"));
      Assert.IsFalse(Helper.IsInteger("kljh"));
    }

    [TestMethod]
    public void IsPositiveInteger_Test()
    {
      Assert.IsTrue(Helper.IsPositiveInteger("123"));
      Assert.IsFalse(Helper.IsPositiveInteger("-123"));
      Assert.IsFalse(Helper.IsPositiveInteger("-2147483649"));
      Assert.IsFalse(Helper.IsPositiveInteger("2147483648"));
      Assert.IsFalse(Helper.IsPositiveInteger("kljh"));
    }

    [TestMethod]
    public void IsPositive64Integer_Test()
    {
      Assert.IsTrue(Helper.IsPositive64Integer("123"));
      Assert.IsFalse(Helper.IsPositive64Integer("-123"));
      Assert.IsFalse(Helper.IsPositive64Integer("-9223372036854775808"));
      Assert.IsTrue(Helper.IsPositive64Integer("9223372036854775807"));
      Assert.IsFalse(Helper.IsPositive64Integer("kljh"));
    }

    [TestMethod]
    public void IsDoubleOrFloat_Test()
    {
      Assert.IsTrue(Helper.IsDoubleOrFloat("123"));
      Assert.IsTrue(Helper.IsDoubleOrFloat("-123"));
      Assert.IsTrue(Helper.IsDoubleOrFloat("123E+3"));
      Assert.IsTrue(Helper.IsDoubleOrFloat("-123E+3"));
      Assert.IsTrue(Helper.IsDoubleOrFloat("-1.79769313486231E+308"));
      Assert.IsTrue(Helper.IsDoubleOrFloat("1.79769313486231E+308"));
    }
  }
}
