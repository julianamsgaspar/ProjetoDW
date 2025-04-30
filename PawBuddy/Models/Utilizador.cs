using System.ComponentModel.DataAnnotations;

namespace PawBuddy.Models;

/// <summary>
/// utilizadores não anónimos da aplicação
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
    [StringLength(50)] 
    public string Nome { get; set; }
    
    /// <summary>
    /// idade do utilizador
    /// </summary>
    [Required(ErrorMessage = "O {0} é de preenchimento obrigatório.")]
    [Display(Name = "Data de Nascimento")]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)] 
    public DateTime Idade  { get; set; }
    
    /// <summary>
    /// numero de identificação fiscal
    /// </summary>
    [Display(Name = "NIF")]
    [Required(ErrorMessage = "O {0} é de preenchimento obrigatório.")] 
    [StringLength(50)] 
    [RegularExpression("([1-9])[0-9]{8}", ErrorMessage = "O {0} só pode conter digitos. No mínimo 6.")]
    public string Nif { get; set; }

    /// <summary>
    /// numero de telemovel do utilizador
    /// </summary>
    [Display(Name = "Telemóvel")]
    [Required(ErrorMessage = "O {0} é de preenchimento obrigatório.")] 
    [StringLength(50)] 
    [RegularExpression("([+]|00)?[0-9]{6,17}", ErrorMessage = "o {0} só pode conter digitos. No mínimo 6.")]
    public string Telemovel { get; set; }

    /// <summary>
    /// morada do utilizador
    /// </summary>
    [Required(ErrorMessage = "A {0} é de preenchimento obrigatório.")] 
    [StringLength(50)] 
    public string Morada { get; set; }
    
    /// <summary>
    /// Código Postal da  morada do utilizador
    /// </summary>
    [Display(Name = "Código Postal")]
    [Required(ErrorMessage = "O {0} é de preenchimento obrigatório.")] 
    [StringLength(50)] 
    [RegularExpression("[1-9][0-9]{3}[-|\\s][0-9]{3}", ErrorMessage = "O {0} tem de seguir o formato xxxx-xxx")] 
    public string CodPostal { get; set; }

    /// <summary>
    /// email do utilizador
    /// </summary>
    [Required(ErrorMessage = "O {0} é de preenchimento obrigatório.")]
    [StringLength(50)] 
    public string Email { get; set; }

    /// <summary>
    /// pais de origem do utilizador
    /// </summary>
    [Display(Name = "Nacionalidade")]
    [StringLength(50)] 
    [Required(ErrorMessage = "A {0} é de preenchimento obrigatório.")] 
    public string Pais { get; set; }
    
    /* *************************
     * Definção dos relacionamentos
     * **************************
     */
    /// <summary>
    /// Lista de animais que o utilizador tem intenção de adotar 
    /// </summary>
    public ICollection<IntencaoDeAdocao> IntencaoDeAdocao { get; set; } = [];
    
    /// <summary>
    /// Lista de doações que o utilizador ja fez aos animais
    /// </summary>
    public ICollection<Doa> Doa { get; set; } = [];
    
    

}