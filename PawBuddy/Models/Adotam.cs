using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using PawBuddy.Models;


namespace PawBuddy.Models;
/// <summary>
/// Class associada as adoçoes definitivas 
/// </summary>
[PrimaryKey(nameof(AnimalFK))]
public class Adotam
{
    /// <summary>
    /// data da adoção definitiva
    /// </summary>
    public DateTime dateA { get; set; }
    
    /* *************************
     * Definção dos relacionamentos
     * **************************
     */
    // Relacionamento de 1-N
    /// <summary>
    /// FK para referenciar o utilizador que adota definitivamente um animal
    /// </summary>
    [ForeignKey(nameof(Utilizador))]
    public int UtilizadorFK { get; set; }
    public Utilizador Utilizador { get; set; }
    
    /// <summary>
    /// FK para referenciar o animal que foi adotado 
    /// </summary>
    [ForeignKey(nameof(Animal))]
    public int AnimalFK { get; set; }
    public Animal Animal { get; set; }
}