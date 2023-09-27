// using NUnit.Framework;

// [TestFixture]
// public class ExpressionEvaluatorTests
// {
//     [Test]
//     public void EvaluateExpression_ShouldReturnString_WhenStringExpressionProvided()
//     {
//         // Arrange
//         var expression = new StringExpression(new Token(TokenType.String, "\"Hello, World!\""));

//         // Act
//         var result = EvaluateExpression(expression);

//         // Assert
//         Assert.AreEqual("Hello, World!", result);
//     }

//     [Test]
//     public void EvaluateExpression_ShouldReturnNumber_WhenNumberExpressionProvided()
//     {
//         // Arrange
//         var expression = new NumberExpression(new Token(TokenType.Number, "42"));

//         // Act
//         var result = EvaluateExpression(expression);

//         // Assert
//         Assert.AreEqual(42, result);
//     }

//     [Test]
//     public void EvaluateExpression_ShouldReturnBoolean_WhenBooleanExpressionProvided()
//     {
//         // Arrange
//         var expression = new BooleanExpression(new Token(TokenType.Boolean, "true"));

//         // Act
//         var result = EvaluateExpression(expression);

//         // Assert
//         Assert.AreEqual(true, result);
//     }

//     // Add more test cases for other expressions and scenarios

//     [Test]
//     public void EvaluateExpression_ShouldReturnErrorMessage_WhenInvalidExpressionProvided()
//     {
//         // Arrange
//         var expression = new InvalidExpression();

//         // Act
//         var result = EvaluateExpression(expression);

//         // Assert
//         Assert.AreEqual("Semantic Error: Invalid expression", result);
//     }
// }