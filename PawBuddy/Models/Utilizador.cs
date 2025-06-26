using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace PawBuddy.Models;

/// <summary>
/// Utilizadores não anónimos da aplicação
/// </summary>
public class Utilizador
{
    /// <summary>
    /// Identificação único do utilizador
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// nome do utilizador
    /// </summary>
    [Required(ErrorMessage = "O {0} é de preenchimento obrigatório.")]
    [StringLength(50, ErrorMessage = "O {0} não pode exceder {1} caracteres.")]
    public string Nome { get; set; }
    
    /// <summary>
    /// data de nascimento do utilizador
    /// </summary>
    [Required(ErrorMessage = "A {0} é de preenchimento obrigatório.")]
    [Display(Name = "Data de Nascimento")]
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    [MaiorDeIdade(ErrorMessage = "O utilizador deve ter pelo menos 18 anos.")]
    public DateTime DataNascimento { get; set; }
    
    /// <summary>
    /// número de identificação fiscal
    /// </summary>
    [Display(Name = "NIF")]
    [Required(ErrorMessage = "O {0} é de preenchimento obrigatório.")]
    [StringLength(9, MinimumLength = 9, ErrorMessage = "O {0} deve ter 9 dígitos.")]
    [RegularExpression(@"^[1-9][0-9]{8}$", ErrorMessage = "NIF inválido.")]
    public string Nif { get; set; }

    /// <summary>
    /// número de telemóvel do utilizador
    /// </summary>
    [Display(Name = "Telemóvel")]
    [Required(ErrorMessage = "O {0} é de preenchimento obrigatório.")]
    [StringLength(15, ErrorMessage = "Número inválido.")]
    [RegularExpression(@"^(\+351|00351|351)?[2-9][0-9]{8}$", 
                     ErrorMessage = "Formato: +351/00351/351 + 9 dígitos")]
    public string Telemovel { get; set; }

    /// <summary>
    /// morada do utilizador
    /// </summary>
    [Required(ErrorMessage = "A {0} é de preenchimento obrigatório.")]
    [StringLength(100, ErrorMessage = "A {0} não pode exceder {1} caracteres.")]
    public string Morada { get; set; }
    
    /// <summary>
    /// Código Postal da morada do utilizador
    /// </summary>
    [Display(Name = "Código Postal")]
    [Required(ErrorMessage = "O {0} é de preenchimento obrigatório.")]
    [RegularExpression(@"^\d{4}-\d{3}$", ErrorMessage = "Formato: xxxx-xxx")]
    public string CodPostal { get; set; }

    /// <summary>
    /// email do utilizador
    /// </summary>
    [Required(ErrorMessage = "O {0} é de preenchimento obrigatório.")]
    [EmailAddress(ErrorMessage = "Email inválido.")]
    [StringLength(100, ErrorMessage = "O {0} não pode exceder {1} caracteres.")]
    public string Email { get; set; }

    /// <summary>
    /// país de origem do utilizador
    /// </summary>
    [Display(Name = "Nacionalidade")]
    [Required(ErrorMessage = "A {0} é de preenchimento obrigatório.")]
    [StringLength(50, ErrorMessage = "A {0} não pode exceder {1} caracteres.")]
    public string Pais { get; set; }

    /// <summary>
    /// Relação com o IdentityUser
    /// </summary>
    public string IdentityUserId { get; set; }
    
    /// <summary>
    /// Navegação para o IdentityUser
    /// </summary>
    public IdentityUser IdentityUser { get; set; }
    
    /* *************************
     * Definção dos relacionamentos
     * **************************
     */
    /// <summary>
    /// Lista de animais que o utilizador tem intenção de adotar 
    /// </summary>
    public ICollection<IntencaoDeAdocao> IntencaoDeAdocao { get; set; } = new List<IntencaoDeAdocao>();
    
    /// <summary>
    /// Lista de doações que o utilizador já fez aos animais
    /// </summary>
    public ICollection<Doa> Doa { get; set; } = new List<Doa>();


}