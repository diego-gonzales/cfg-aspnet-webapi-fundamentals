using System.ComponentModel.DataAnnotations;
using System.Reflection;
using WebAPIAutores;

namespace WebAPIAuthores.Tests;

[TestClass]
public class UpperFirstLetterAttributeTests
{
    [TestMethod]
    public void UpperFirstLetter_ReturnError()
    {
        // Preparar
        var upperFirstLetter = new UpperFirstLetterAttribute();
        string testValue = "diego";
        var validationContext = new ValidationContext(new { Nombre = testValue });

        // Probar
        var result = upperFirstLetter.GetValidationResult(testValue, validationContext);

        // Verificar
        Assert.AreEqual("La primera letra del campo  debe ser mayúscula", result?.ErrorMessage);
    }
}
