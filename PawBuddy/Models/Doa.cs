using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;


namespace PawBuddy.Models;
/// <summary>
/// class relacionada as doaçoes que os utilizadores fazem aos animais
/// </summary>
[PrimaryKey(nameof(UtilizadorFK ),nameof(AnimalFK))]
public class Doa
{
	
	/// <summary>
	/// valor da doação
	/// </summary>
    public decimal Valor { get; set; }
	/// <summary>
	/// data da doação
	/// </summary>
    public DateTime DataD { get; set; }

	/* *************************
	 * Relacionamentos
	 * **************************
	 */
	//relacionamento de N-M
	
	/// <summary>
	/// FK para referenciar o utilizador que tem doa dinheiro a um animal
	/// </summary>
    [ForeignKey(nameof(Utilizador))]
    public int UtilizadorFK { get; set; }
    public Utilizador Utilizador { get; set; }
    
    /// <summary>
    /// FK para referenciar o animal a que foi doado dinheiro
    /// </summary>
    [ForeignKey(nameof(Animal))]
    public int AnimalFK { get; set; }
    public Animal Animal { get; set; }
    
    
}