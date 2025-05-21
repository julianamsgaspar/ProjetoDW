using System.ComponentModel.DataAnnotations;

namespace PawBuddy.Models;
/// <summary>
/// Class associada ao animal 
/// </summary>
public class Animal
{
	/// <summary>
	/// identificação do animal
	/// </summary>
	[Key]
	public int Id {get; set;}
	/// <summary>
	/// nome do animal
	/// </summary>
	[Required(ErrorMessage = "O {0} é de preenchimento obrigatório.")] 
	[RegularExpression("^[A-Za-zÀ-ÖØ-öø-ÿ]{2,}$", ErrorMessage = "O {0} tem de ser simples e com um mínimo de 2 letras.")]
	public string Nome { get; set; }

	/// <summary>
	/// raça do animal
	/// </summary>
	[Display(Name = "Raça")]
    [Required(ErrorMessage = "O {0} é de preenchimento obrigatório.")] 
	[RegularExpression("^[A-Za-zÀ-ÖØ-öø-ÿ]+([ '-][A-Za-zÀ-ÖØ-öø-ÿ]+)*$", ErrorMessage = "A {0} não pode conter caracteres numéricos.")]
    [StringLength(50)] 
    public string Raca {get; set; }

	/// <summary>
	/// idade do animal
	/// </summary>
	[Required(ErrorMessage = "O {0} é de preenchimento obrigatório.")] 
	[StringLength(50)] 
	public string Idade { get; set; }

	/// <summary>
	/// genero do animal
	/// </summary>
	[Required(ErrorMessage = "O {0} é de preenchimento obrigatório.")] 
	[StringLength(50)] 
    public string Genero {get; set; }

	/// <summary>
	/// especie do animal (gato, cão, etc)
	/// </summary>
	[Display(Name = "Espécie")]
	[Required(ErrorMessage = "O {0} é de preenchimento obrigatório.")] 
	[StringLength(50)] 
    public string Especie { get; set; }

	/// <summary>
	/// cor do animal
	/// </summary>
	[Required(ErrorMessage = "O {0} é de preenchimento obrigatório.")] 
	[StringLength(50)] 
    public string Cor { get; set; }

	/// <summary>
	/// imagem associada ao animal
	/// </summary>
	// [Required(ErrorMessage = "O {0} é de preenchimento obrigatório.")]
	[RegularExpression("^(.+/)?([^/]+\\.(jpg|jpeg|png|gif|bmp|webp))$", ErrorMessage = "O {0} tem de seguir o formato de caminho com uma das seguintes extensões:jpg, jpeg, png, gif, bmp, webp")]
    public string Imagem { get; set; }
	
	/* *************************
	 * Definção dos relacionamentos
	 * **************************
	 */
	/// <summary>
	/// Lista de animais que o utilizador tem intenção de adotar 
	/// </summary>
   
	public ICollection<IntencaoDeAdocao>? IntencaoDeAdocao { get; set; } // pode ser null
    
	/// <summary>
	/// Lista de doações que o utilizador ja fez aos animais
	/// </summary>
    public ICollection<Doa>? Doa { get; set; } // pode ser null
    
    
    
    
    
}