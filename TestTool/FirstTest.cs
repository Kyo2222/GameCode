using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class FirstTest
{
    [Test]
    public void Addition_IsCorrect()
    {
        int result = 1 + 1;
        Assert.AreEqual(2, result);
    }

    [Test]
    public void String_IsNotEmpty()
    {
        string playerName = "Hi";
        Assert.IsNotEmpty(playerName);
    }

    [Test]
    public void Subtraction_IsCorrect_WillFail()
    {
        int result = 5 - 3;
        Assert.AreEqual(99, result);
    }
}
