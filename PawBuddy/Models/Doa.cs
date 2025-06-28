using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;


namespace PawBuddy.Models;
/// <summary>
/// class relacionada as doaçoes que os utilizadores fazem aos animais
/// </summary>
//[PrimaryKey(nameof(UtilizadorFK ),nameof(AnimalFK))]
public class Doa
{
	/// <summary>
	/// identificação da doação
	/// </summary>
	public int Id { get; set; }
	
	/// <summary>
	/// valor da doação
	/// </summary
	
	[Required(ErrorMessage = "Por favor, introduza um valor para doar.")]
    public decimal Valor { get; set; }
	/// <summary>
	/// data da doação
	/// </summary>
	[Display(Name = "Data da Doação")]
	[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)] 
	[DataType(DataType.Date)] 
    public DateTime DataD { get; set; }
	
	/// <summary>
	/// Variável auxiliar usada para validar o preço inserido pelo utilizador
	/// </summary>
	[Required(ErrorMessage = "O {0} é de preenchimento obrigatório.")] 
	[Display(Name = "Valor da doação")] 
	[StringLength(9)] 
	[RegularExpression("[0-9]{1,6}([,.][0-9]{1,2})?", ErrorMessage = "Escreva um número com, no máximo 2 casas decimais, separadas por . ou ,")] 
	[NotMapped]
	public string PrecoAux { get; set; }

	/* *************************
	 * Relacionamentos
	 * **************************
	 */
	//relacionamento de N-M
	
	/// <summary>
	/// FK para referenciar o utilizador que tem doa dinheiro a um animal
	/// </summary>
	[Display(Name = "Utilizador")]
	[ForeignKey(nameof(Utilizador))]
    public int UtilizadorFK { get; set; }
    public Utilizador Utilizador { get; set; }
    
    /// <summary>
    /// FK para referenciar o animal a que foi doado dinheiro
    /// </summary>
    [Display(Name = "Animal")]
    [ForeignKey(nameof(Animal))]
    public int AnimalFK { get; set; }
    public Animal Animal { get; set; }
    
    
}