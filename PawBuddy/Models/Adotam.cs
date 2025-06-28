using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using PawBuddy.Models;


namespace PawBuddy.Models;
///olá
/// <summary>
/// Class associada as adoçoes definitivas 
/// </summary>
[PrimaryKey(nameof(AnimalFK))]
public class Adotam
{
    /// <summary>
    /// idendificação da adoção
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// data da adoção definitiva
    /// </summary>
    [Display(Name = "Data da Adoação")]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)] 
    [DataType(DataType.Date)] 
    public DateTime dateA { get; set; }
    
    /* *************************
     * Definção dos relacionamentos
     * **************************
     */
    // Relacionamento de 1-N
    /// <summary>
    /// FK para referenciar o utilizador que adota definitivamente um animal
    /// </summary>
    [Display(Name = "Utilizador")]
    [ForeignKey(nameof(Utilizador))]
    public int UtilizadorFK { get; set; }
    public Utilizador Utilizador { get; set; }
    
    /// <summary>
    /// FK para referenciar o animal que foi adotado 
    /// </summary>
    [Display(Name = "Animal")]
    [ForeignKey(nameof(Animal))]
    public int AnimalFK { get; set; }
    public Animal Animal { get; set; }
}