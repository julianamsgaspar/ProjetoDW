namespace PawBuddy.Models;

/// <summary>
/// utilizadores não anónimos da aplicação
/// </summary>
public class Utilizador
{
    /// <summary>
    /// Identificação do utilizador
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// nome do utilizador
    /// </summary>
    public string Nome { get; set; }

    /// <summary>
    /// numero de identificação fiscal
    /// </summary>
    public string Nif { get; set; }

    /// <summary>
    /// numero de telemovel do utilizador
    /// </summary>
    public string Telemovel { get; set; }

    /// <summary>
    /// morada do utilizador
    /// </summary>
    public string Morada { get; set; }

    /// <summary>
    /// email do utilizador
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// pais de origem do utilizador
    /// </summary>
    public string Pais { get; set; }
    
    /* *************************
     * Definção dos relacionamentos
     * **************************
     */
    /// <summary>
    /// Lista de animais que o utilizador tem intenção de adotar 
    /// </summary>
    public ICollection<IntencaoDeAdocao> IntencasDeAdocais { get; set; }
    
    /// <summary>
    /// Lista de doações que o utilizador ja fez aos animais
    /// </summary>
    public ICollection<Doa> Doa { get; set; }
    
    

}