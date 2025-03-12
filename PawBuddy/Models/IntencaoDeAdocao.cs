using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PawBuddy.Models;
/// <summary>
/// class que diz a intenção dos utilizadores em adotar os animais, depois é validado pelo administrador 
/// </summary>
[PrimaryKey(nameof(UtilizadorFK),nameof(AnimalFK))]
public class IntencaoDeAdocao
{
    /// <summary>
    /// Estado da adoção -- enumeração abaixo
    /// </summary>
    public EstadoAdocao Estado  { get; set; }
    
    
    /* *************************
     * formulario
     * **************************
     */
    /// <summary>
    /// idade do utilizador
    /// </summary>
    public int Idade  { get; set; }
    /// <summary>
    /// numero de telemovel
    /// </summary>
    public string Contacto { get; set; }
    /// <summary>
    /// profissão do utilizador
    /// </summary>
    public string Profissao { get; set; }
    /// <summary>
    /// que tipo de residencia onde o utilizador vive
    /// </summary>
    public string Residencia { get; set; }
    /// <summary>
    /// motivo da adoção
    /// </summary>
    public string Motivo  { get; set; }
    /// <summary>
    /// data da submissao do formulario
    /// </summary>
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
