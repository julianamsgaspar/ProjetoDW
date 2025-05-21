using System;
using System.ComponentModel.DataAnnotations;

/// <summary>
/// Validação personalizada para garantir que o utilizador tem pelo menos 18 anos.
/// </summary>
public class MaiorDeIdadeAttribute : ValidationAttribute
{
    /// <summary>
    /// Verifica se a data de nascimento corresponde a uma idade mínima de 18 anos.
    /// </summary>
    /// <param name="value">Data de nascimento fornecida.</param>
    /// <param name="validationContext">Contexto da validação.</param>
    /// <returns>Resultado da validação.</returns>
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value is not DateTime dataNascimento)
        {
            return new ValidationResult("Data de nascimento inválida.");
        }

        var idade = DateTime.Today.Year - dataNascimento.Year;
        if (dataNascimento.Date > DateTime.Today.AddYears(-idade)) idade--;

        if (idade < 18)
        {
            return new ValidationResult("O utilizador deve ter pelo menos 18 anos.");
        }

        return ValidationResult.Success;
    }
}