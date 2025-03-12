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
    public string Nome { get; set; }

	/// <summary>
	/// raça do animal
	/// </summary>
    public string Raca {get; set; }

	/// <summary>
	/// idade do animal
	/// </summary>
    public string Idade { get; set; }

	/// <summary>
	/// genero do animal
	/// </summary>
    public string Genero {get; set; }

	/// <summary>
	/// especie do animal (gato, cão, etc)
	/// </summary>
    public string Especie { get; set; }

	/// <summary>
	/// cor do animal
	/// </summary>
    public string Cor { get; set; }

	/// <summary>
	/// imagem associada ao animal
	/// </summary>
    public string Imagem { get; set; }
	
	/* *************************
	 * Definção dos relacionamentos
	 * **************************
	 */
	/// <summary>
	/// Lista de animais que o utilizador tem intenção de adotar 
	/// </summary>
    public ICollection<IntencaoDeAdocao> IntencasDeAdocao { get; set; }
    
	/// <summary>
	/// Lista de doações que o utilizador ja fez aos animais
	/// </summary>
    public ICollection<Doa> Doa { get; set; }
    
    
    
    
    
}