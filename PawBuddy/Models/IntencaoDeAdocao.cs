using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PawBuddy.Models;
/// <summary>
/// class que diz a intenção dos utilizadores em adotar os animais, depois é validado pelo administrador 
/// </summary>
//[PrimaryKey(nameof(UtilizadorFK),nameof(AnimalFK))]
public class IntencaoDeAdocao
{
    /// <summary>
    /// idendificação da intenção de adoção
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Estado da adoção -- enumeração abaixo
    /// </summary>
    public EstadoAdocao Estado  { get; set; }
    
    
    /* *************************
     * formulario
     * **************************
     */
    
    
    /// <summary>
    /// profissão do utilizador
    /// </summary>
    [Display(Name = "Ocupação/Profissão")]
    [StringLength(50)] 
    [Required(ErrorMessage = "A {0} é de preenchimento obrigatório.")]
    public string Profissao { get; set; }
    
    /// <summary>
    /// que tipo de residencia onde o utilizador vive
    /// </summary>
    [Display(Name = "Tipo de Residência")]
    [Required(ErrorMessage = "O {0} é de preenchimento obrigatório.")]
    [StringLength(50)] 
    public string Residencia { get; set; }
    
    /// <summary>
    /// motivo da adoção
    /// </summary>
    [Display(Name = "Motivo da Adoção")]
    [Required(ErrorMessage = "A {0} é de preenchimento obrigatório.")] 
    [StringLength(50)] 
    public string Motivo  { get; set; }
    
    /// <summary>
    /// Pergunta se tem outros animais
    /// </summary>
    ///
    [Display(Name = "Tem animais em casa?")]
    [Required(ErrorMessage = "O {0} é de preenchimento obrigatório.")] 
    [RegularExpression("^(Sim|Não)$", ErrorMessage = "O valor de {0} deve ser 'Sim' ou 'Não'.")]
    [StringLength(50)] 
    public string temAnimais { get; set; }
    
    /// <summary>
    /// se tiver animais, quais?
    /// </summary>
    ///
    [Display(Name = "Se sim, quais?")]
    [StringLength(50)] 
    public string quaisAnimais { get; set; }
    
    /// <summary>
    /// data da submissao do formulario
    /// </summary>
    [Display(Name = "Data da Adoção")]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)] 
    [DataType(DataType.Date)] 
    public DateTime DataIA  { get; set; }
    
    
    /* *************************
     * Definção dos relacionamentos
     * **************************
     */
    // Relacionamento de N-M
    /// <summary>
    /// FK para referenciar o utilizador que tem a intenção de adotar um animal
    /// </summary>
    [ForeignKey(nameof(Utilizador))]
    public int UtilizadorFK { get; set; }
    public Utilizador Utilizador { get; set; }
    
    /// <summary>
    /// FK para referenciar o animal que o utilizador tem a intenção de adotar 
    /// </summary>
    [ForeignKey(nameof(Animal))]
    public int AnimalFK { get; set; }
    public Animal Animal { get; set; }

}

// https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/enum
/// <summary>
/// Estados associados ao processo de adoção
/// </summary>
public enum EstadoAdocao
{
    Reservado,
    Emprocesso,
    Emvalidacao,
    Concluido,
	Rejeitado
}
