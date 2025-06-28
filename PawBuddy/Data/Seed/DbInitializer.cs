using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PawBuddy.Models;
using System;

namespace PawBuddy.Data.Seed;

/// <summary>
/// Classe responsável por inicializar e povoar a base de dados com dados de exemplo
/// </summary>
internal class DbInitializer 
{
    /// <summary>
    /// Método principal que coordena toda a inicialização da base de dados
    /// </summary>
    /// <param name="dbContext"></param>
    internal static async void Initialize(ApplicationDbContext dbContext) 
    {
         
        
        ArgumentNullException.ThrowIfNull(dbContext, nameof(dbContext));
        dbContext.Database.EnsureCreated();

        // var auxiliar
        bool haAdicao = false;

        
           
        var animaisLista = Array.Empty<Animal>();
        // Se não houver animais, cria-os
        if (! dbContext.Animal.Any())
        {
            // Lista de animais a adicionar à base de dados
             animaisLista =
            [
                
                // Lista de objetos do tipo Animal a ser inserida
                new Animal
                {
                Nome = "Bobby", Raca = "Labrador Retriever", Especie = "Cão", Genero = "Macho", Cor = "Preto",
                Imagem = "images/animal0.jpg", Idade = "Bebé"
                },
                new Animal
                {
                    Nome = "Luna", Raca = "Golden Retriever", Especie = "Cão", Genero = "Fêmea", Cor = "Dourado",
                    Imagem = "images/animal1.jpg", Idade = "Adulta"
                },
                new Animal
                {
                    Nome = "Max", Raca = "Pastor Alemão", Especie = "Cão", Genero = "Macho", Cor = "Preto e castanho",
                    Imagem = "images/animal2.jpg", Idade = "Jovem"
                },
                new Animal
                {
                    Nome = "Bella", Raca = "SRD", Especie = "Cão", Genero = "Fêmea", Cor = "Branco",
                    Imagem = "images/animal3.jpg", Idade = "Adulta"
                },
                new Animal
                {
                    Nome = "Charlie", Raca = "Bulldog Francês", Especie = "Cão", Genero = "Macho", Cor = "Tigrado",
                    Imagem = "images/animal4.jpg", Idade = "Idoso"
                },
                new Animal
                {
                    Nome = "Mia", Raca = "Siamês", Especie = "Gato", Genero = "Fêmea", Cor = "Cinza",
                    Imagem = "images/animal5.jpg", Idade = "Adulta"
                },
                new Animal
                {
                    Nome = "Oliver", Raca = "Persa", Especie = "Gato", Genero = "Macho", Cor = "Branco",
                    Imagem = "images/animal6.jpg", Idade = "Adulto"
                },
                new Animal
                {
                    Nome = "Lily", Raca = "SRD", Especie = "Gato", Genero = "Fêmea", Cor = "Malhado",
                    Imagem = "images/animal7.jpg", Idade = "Idosa"
                },
                new Animal
                {
                    Nome = "Simba", Raca = "SRD", Especie = "Gato", Genero = "Macho", Cor = "Laranja",
                    Imagem = "images/animal8.jpg", Idade = "Bebé"
                },
                new Animal
                {
                    Nome = "Biscoito", Raca = "Coelho Anão", Especie = "Coelho", Genero = "Macho", Cor = "Branco",
                    Imagem = "images/animal9.jpg", Idade = "Idoso"
                },
                new Animal
                {
                    Nome = "Pipoca", Raca = "Coelho Angorá", Especie = "Coelho", Genero = "Fêmea", Cor = "Cinza",
                    Imagem = "images/animal10.jpg", Idade = "Adulta"
                },
                new Animal
                {
                    Nome = "Piu", Raca = "Canário", Especie = "Pássaro", Genero = "Macho", Cor = "Amarelo",
                    Imagem = "images/animal11.jpg", Idade = "Jovem"
                },
                new Animal
                {
                    Nome = "Loro", Raca = "Periquito", Especie = "Pássaro", Genero = "Macho", Cor = "Verde",
                    Imagem = "images/animal12.jpg", Idade = "Jovem"
                },
                new Animal
                {
                    Nome = "Rex", Raca = "Rottweiler", Especie = "Cão", Genero = "Macho", Cor = "Preto e castanho",
                    Imagem = "images/animal13.png", Idade = "Bebé"
                },
                new Animal
                {
                    Nome = "Duna", Raca = "SRD", Especie = "Gato", Genero = "Fêmea", Cor = "Cinza e branco",
                    Imagem = "images/animal14.jpg", Idade = "Adulta"
                }
            ];
            
            // Adiciona os animais e guarda as alterações
            await dbContext.Animal.AddRangeAsync(animaisLista);
            haAdicao = true;
            
        }

        
    
        var novosUtilizadoresIdentity =Array.Empty<IdentityUser>();
        // Se não houver utilizadores Identity, cria-os
        if (! dbContext.Users.Any())
        {
            // Criação dos utilizadores para autenticação (Identity)
            var hasher = new PasswordHasher<IdentityUser>();

            // Criação de novos utilizadores com dados de autenticação
            novosUtilizadoresIdentity = [
                new IdentityUser
                {
                UserName = "Andre_Silva",
                NormalizedUserName = "ANDRE_SILVA",
                Email = "andresilva@mail.pt",
                NormalizedEmail = "ANDRESILVA@MAIL.PT",
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString("N").ToUpper(),
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                PasswordHash = hasher.HashPassword(null, "Aa0_aa")
                },
                new IdentityUser
                {
                    UserName = "JoséMendes",
                    NormalizedUserName = "JOSÉMENDES",
                    Email = "jmendes@mail.pt",
                    NormalizedEmail = "JMENDES@MAIL.PT",
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString("N").ToUpper(),
                    ConcurrencyStamp = Guid.NewGuid().ToString(),
                    PasswordHash = hasher.HashPassword(null, "Aa0_aa")
                },
                new IdentityUser
                {
                    UserName = "Pedro_Mendes",
                    NormalizedUserName = "PEDRO_MENDES",
                    Email = "pedroM@mail.pt",
                    NormalizedEmail = "PEDROM@MAIL.PT",
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString("N").ToUpper(),
                    ConcurrencyStamp = Guid.NewGuid().ToString(),
                    PasswordHash = hasher.HashPassword(null, "Aa0_aa")
                },
                new IdentityUser
                {
                    UserName = "Ana_Santos",
                    NormalizedUserName = "ANA_SANTOS",
                    Email = "anasantos@mail.pt",
                    NormalizedEmail = "ANASANTOS@MAIL.PT",
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString("N").ToUpper(),
                    ConcurrencyStamp = Guid.NewGuid().ToString(),
                    PasswordHash = hasher.HashPassword(null, "Aa0_aa")
                },
                new IdentityUser
                {
                    UserName = "Maria_Oliveira",
                    NormalizedUserName = "MARIA_OLIVEIRA",
                    Email = "mariaoliveira@mail.pt",
                    NormalizedEmail = "MARIAOLIVEIRA@MAIL.PT",
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString("N").ToUpper(),
                    ConcurrencyStamp = Guid.NewGuid().ToString(),
                    PasswordHash = hasher.HashPassword(null, "Aa0_aa")
                },
                new IdentityUser
                {
                    UserName = "Carlos_Rodrigues",
                    NormalizedUserName = "CARLOS_RODRIGUES",
                    Email = "carlosr@mail.pt",
                    NormalizedEmail = "CARLOSR@MAIL.PT",
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString("N").ToUpper(),
                    ConcurrencyStamp = Guid.NewGuid().ToString(),
                    PasswordHash = hasher.HashPassword(null, "Aa0_aa")
                },
                new IdentityUser
                {
                    UserName = "Sofia_Almeida",
                    NormalizedUserName = "SOFIA_ALMEIDA",
                    Email = "sofiaalmeida@mail.pt",
                    NormalizedEmail = "SOFIAALMEIDA@MAIL.PT",
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString("N").ToUpper(),
                    ConcurrencyStamp = Guid.NewGuid().ToString(),
                    PasswordHash = hasher.HashPassword(null, "Aa0_aa")
                },
                new IdentityUser
                {
                    UserName = "Rui_Pereira",
                    NormalizedUserName = "RUI_PEREIRA",
                    Email = "ruip@mail.pt",
                    NormalizedEmail = "RUIP@MAIL.PT",
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString("N").ToUpper(),
                    ConcurrencyStamp = Guid.NewGuid().ToString(),
                    PasswordHash = hasher.HashPassword(null, "Aa0_aa")
                },
                new IdentityUser
                {
                    UserName = "Ines_Ferreira",
                    NormalizedUserName = "INES_FERREIRA",
                    Email = "inesf@mail.pt",
                    NormalizedEmail = "INESF@MAIL.PT",
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString("N").ToUpper(),
                    ConcurrencyStamp = Guid.NewGuid().ToString(),
                    PasswordHash = hasher.HashPassword(null, "Aa0_aa")
                },
                new IdentityUser
                {
                    UserName = "Miguel_Costa",
                    NormalizedUserName = "MIGUEL_COSTA",
                    Email = "miguelcosta@mail.pt",
                    NormalizedEmail = "MIGUELCOSTA@MAIL.PT",
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString("N").ToUpper(),
                    ConcurrencyStamp = Guid.NewGuid().ToString(),
                    PasswordHash = hasher.HashPassword(null, "Aa0_aa")
                }


            ];

            await dbContext.Users.AddRangeAsync(novosUtilizadoresIdentity);
            haAdicao = true;
        }

    
        var utilizadoress = Array.Empty<Utilizador>();
        // Se não houver utilizadores, cria-os q    
        if (novosUtilizadoresIdentity.Length !=0)
        {
            // Obtém os utilizadores Identity criados anteriormente
            

            // Criação dos perfis de utilizador
            utilizadoress = [
            
                new Utilizador
                {
                    Nome = "Andre_Silva",
                    Morada = "Rua  das Flores, 10",
                    CodPostal = "1000-001 LISBOA",
                    Pais = "Portugal",
                    Nif = "111111111",
                    Telemovel = "912345678",
                    DataNascimento = new DateTime(1990, 5, 21),
                    Email = "andresilva@mail.pt",
                    IdentityUser = novosUtilizadoresIdentity[0]
                },
                new Utilizador
                {
                    Nome = "JoséMendes",
                    Morada = "Avenida da Liberdade, 25",
                    CodPostal = "1250-001 LISBOA",
                    Pais = "Portugal",
                    Nif = "222222222",
                    Telemovel = "923456789",
                    DataNascimento = new DateTime(1984, 10, 21),
                    Email = "jmendes@mail.pt",
                    IdentityUser = novosUtilizadoresIdentity[1]
                },
                new Utilizador
                {
                    Nome = "Pedro_Mendes",
                    Morada = "Rua do Sol, 5",
                    CodPostal = "1200-001 LISBOA",
                    Pais = "Portugal",
                    Nif = "333333333",
                    DataNascimento = new DateTime(2001, 11, 24),
                    Telemovel = "934567890",
                    Email = "pedroM@mail.pt",
                    IdentityUser = novosUtilizadoresIdentity[2]
                },
                new Utilizador
                {
                    Nome = "Ana_Santos",
                    Morada = "Rua da Prata, 30",
                    CodPostal = "1100-001 LISBOA",
                    Pais = "Portugal",
                    Nif = "444444444",
                    DataNascimento = new DateTime(2000, 2, 3),
                    Telemovel = "945678901",
                    Email = "anasantos@mail.pt",
                    IdentityUser = novosUtilizadoresIdentity[3]
                },
                new Utilizador
                {
                    Nome = "Maria_Oliveira",
                    Morada = "Rua do Ouro, 15",
                    CodPostal = "1200-002 LISBOA",
                    Pais = "Portugal",
                    Nif = "555555555",
                    Telemovel = "956789012",
                    DataNascimento = new DateTime(1995, 4, 10),
                    Email = "mariaoliveira@mail.pt",
                    IdentityUser = novosUtilizadoresIdentity[4]
                },
                new Utilizador
                {
                    Nome = "Carlos_Rodrigues",
                    Morada = "Rua Augusta, 100",
                    CodPostal = "1100-002 LISBOA",
                    Pais = "Portugal",
                    Nif = "666666666",
                    Telemovel = "967890123",
                    DataNascimento = new DateTime(1991, 4, 16),
                    Email = "carlosr@mail.pt",
                    IdentityUser = novosUtilizadoresIdentity[5]
                },
                new Utilizador
                {
                    Nome = "Sofia_Almeida",
                    Morada = "Avenida Almirante Reis, 50",
                    CodPostal = "1150-001 LISBOA",
                    Pais = "Portugal",
                    Nif = "777777777",
                    Telemovel = "978901234",
                    DataNascimento = new DateTime(2002, 7, 14),
                    Email = "sofiaalmeida@mail.pt",
                    IdentityUser = novosUtilizadoresIdentity[6]
                },
                new Utilizador
                {
                    Nome = "Rui_Pereira",
                    Morada = "Rua Castilho, 20",
                    CodPostal = "1250-002 LISBOA",
                    Pais = "Portugal",
                    Nif = "888888888",
                    Telemovel = "989012345",
                    DataNascimento = new DateTime(1988, 8, 19),
                    Email = "ruip@mail.pt",
                    IdentityUser = novosUtilizadoresIdentity[7]
                },
                new Utilizador
                {
                    Nome = "Ines_Ferreira",
                    Morada = "Rua Garrett, 8",
                    CodPostal = "1200-003 LISBOA",
                    Pais = "Portugal",
                    Nif = "999999999",
                    Telemovel = "990123456",
                    DataNascimento = new DateTime(1983, 4, 15),
                    Email = "inesf@mail.pt",
                    IdentityUser = novosUtilizadoresIdentity[8]
                },
                new Utilizador
                {
                    Nome = "Miguel_Costa",
                    Morada = "Rua do Alecrim, 12",
                    CodPostal = "1200-004 LISBOA",
                    Pais = "Portugal",
                    Nif = "101010101",
                    DataNascimento = new DateTime(1990, 5, 21),
                    Telemovel = "901234567",
                    Email = "miguelcosta@mail.pt",
                    IdentityUser = novosUtilizadoresIdentity[9]
                }

            ];

            // Adiciona os perfis de utilizador à base de dados
            await dbContext.Utilizador.AddRangeAsync(utilizadoress);
            haAdicao = true;
        }

        var adocoes = Array.Empty<Adotam>();
        // Se não houver adoções, cria-as
        if (utilizadoress.Length !=0 && animaisLista.Length !=0)
        {

            adocoes = 
            [
                new Adotam
                {
                    Animal = animaisLista[3], // Bella
                    Utilizador = utilizadoress[0], // André Silva
                    dateA = DateTime.Now.AddDays(-30),
                },
                new Adotam
                {
                    Animal = animaisLista[6], // Oliver
                    Utilizador = utilizadoress[3], // Ana Santos
                    dateA = DateTime.Now.AddDays(-15),

                },
                new Adotam
                {
                    Animal = animaisLista[8], // Simba
                    Utilizador = utilizadoress[5], // Carlos Rodrigues
                    dateA = DateTime.Now.AddDays(-10),

                },
                new Adotam
                {
                    Animal = animaisLista[12], // Loro
                    Utilizador = utilizadoress[7], // Rui Pereira
                    dateA = DateTime.Now.AddDays(-5),

                }
            ];

            await dbContext.Adotam.AddRangeAsync(adocoes);
            haAdicao = true;
        }

        var doacoes = Array.Empty<Doa>();
        // Se não houver doações, cria-as
        if (utilizadoress.Length !=0 && animaisLista.Length !=0)
        {
            

            doacoes = [
            
                new Doa
                {
                    Animal = animaisLista[0], // Bobby
                    Utilizador = utilizadoress[1], // José Mendes
                    DataD = DateTime.Now.AddDays(-20),
                  Valor = (decimal)65.5
                },
                new Doa
                {
                    Animal = animaisLista[2], // Max
                    Utilizador = utilizadoress[4], // Maria Oliveira
                    DataD = DateTime.Now.AddDays(-12),
                  Valor= (Decimal) 10.0

                },
                new Doa
                {
                    Animal = animaisLista[5], // Mia
                    Utilizador = utilizadoress[6], // Sofia Almeida
                    DataD = DateTime.Now.AddDays(-8),
                  Valor= (Decimal) 22.2

                },
                new Doa
                {
                    Animal = animaisLista[9], // Biscoito
                    Utilizador = utilizadoress[8], // Inês Ferreira
                    DataD = DateTime.Now.AddDays(-3),
                  Valor= (Decimal) 13.5

                }
            ];

            await dbContext.Doa.AddRangeAsync(doacoes);
            haAdicao = true;
        }

        var intencoes = Array.Empty<IntencaoDeAdocao>();
        // Se não houver intenções de adoção, cria-as
        if (utilizadoress.Length !=0 && animaisLista.Length !=0)
        {
            intencoes = [
            
                new IntencaoDeAdocao
                {
                    Animal = animaisLista[1], // Luna
                    Utilizador = utilizadoress[0], // André Silva
                    DataIA = DateTime.Now.AddDays(-2),
                    Profissao = "Engenheiro",
                    Residencia = "Apartamento",
                    Motivo = "Quero um companheiro para minha família",
                    temAnimais = "Não",
                    Estado = EstadoAdocao.Emprocesso
                },
                 new IntencaoDeAdocao
                 {
                     Animal = animaisLista[1], // Luna
                     Utilizador = utilizadoress[2], // Pedro Mendes
                     DataIA = DateTime.Now.AddDays(-2).AddHours(2),
                     Profissao = "Professor",
                     Residencia = "Casa com jardim",
                     temAnimais = "Sim",
                     quaisAnimais = "Tenho um cadela chamada Nina de 10 anos",
                     Motivo = "Adoro cães e quero dar um lar amoroso",
                     Estado = EstadoAdocao.Emprocesso
                 },
                 new IntencaoDeAdocao
                 {
                     Animal = animaisLista[1], // Luna
                     Utilizador = utilizadoress[4], // Maria Oliveira
                     DataIA = DateTime.Now.AddDays(-2).AddHours(4),
                     Profissao = "Médica",
                     Residencia = "Moradia",
                     temAnimais = "Não",
                     Motivo = "Quero um cão para me fazer companhia",
                     Estado = EstadoAdocao.Emprocesso
                
                },
                
                // Em validação (apenas 1 pedido)
                new IntencaoDeAdocao
                {
                    Animal = animaisLista[4], // Charlie
                    Utilizador = utilizadoress[3], // Ana Santos
                    DataIA = DateTime.Now.AddDays(-1),
                    Profissao = "Arquiteta",
                    Residencia = "Apartamento",
                    temAnimais = "Não",
                    Motivo = "Quero um cão pequeno para meu apartamento",
                    Estado = EstadoAdocao.Emvalidacao
                },
                
                // Reservado
                new IntencaoDeAdocao
                {
                    Animal = animaisLista[7], // Lily
                    Utilizador = utilizadoress[5], // Carlos Rodrigues
                    DataIA = DateTime.Now.AddDays(-3),
                    Profissao = "Advogado",
                    Residencia = "Apartamento",
                    Motivo = "Quero uma gata para minha filha",
                    temAnimais = "Sim",
                    quaisAnimais = "Já tenho um papagaio",
                    Estado = EstadoAdocao.Reservado
                },
                
                // Concluído
                new IntencaoDeAdocao
                {
                    Animal = animaisLista[3], // Bella
                    Utilizador = utilizadoress[0], // André Silva
                    DataIA = DateTime.Now.AddDays(-30),
                    Profissao = "Engenheiro",
                    Residencia = "Apartamento",
                    temAnimais = "Não",
                    Motivo = "Quero um cão para minha família",
                     Estado = EstadoAdocao.Concluido
                 }
            ];

            await dbContext.Intencao.AddRangeAsync(intencoes);
            haAdicao = true;
        }
        try 
        {
            if (haAdicao) 
            {
                dbContext.SaveChanges();
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Erro ao salvar alterações no banco de dados", ex);
        }
    }
}

