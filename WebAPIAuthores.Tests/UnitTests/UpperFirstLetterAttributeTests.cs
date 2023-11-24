using System.ComponentModel.DataAnnotations;
using WebAPIAutoresAvanzados;

namespace WebAPIAuthores.Tests;

[TestClass]
public class UpperFirstLetterAttributeTests
{
    [TestMethod]
    public void LowerFirstLetter_ReturnError()
    {
        // Preparar
        var upperFirstLetter = new UpperFirstLetterAttribute();
        string testValue = "diego";
        var validationContext = new ValidationContext(new { Nombre = testValue });

        // Probar
        var result = upperFirstLetter.GetValidationResult(testValue, validationContext);

        // Verificar
        Assert.AreEqual("La primera letra del campo  debe ser mayúscula", result.ErrorMessage);
    }

    [TestMethod]
    public void EmptyString_NotReturnError()
    {
        var upperFirstLetter = new UpperFirstLetterAttribute();
        string testValue = "";
        var validationContext = new ValidationContext(new { Nombre = testValue });

        // en el caso de GetValidationResult(), cuando no existe ningún error de validación, el resultado es "null", no "true".
        var result = upperFirstLetter.GetValidationResult(testValue, validationContext);

        Assert.IsNull(result);
    }

    [TestMethod]
    public void UpperFirstLetter_NotReturnError()
    {
        var upperFirstLetter = new UpperFirstLetterAttribute();
        var testValue = "Diego";
        var validationContext = new ValidationContext(new { Nombre = testValue });

        var result = upperFirstLetter.GetValidationResult(testValue, validationContext);

        Assert.IsNull(result);
    }
}
