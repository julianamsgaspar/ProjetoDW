# PawBuddy - Desenvolvimento Web

## Desenvolvido por

- **Inês Maciel** (nº 25938)  
- **Juliana Gaspar** (nº 26545)

## Pré-requisitos

- [.NET SDK 8.0+](https://dotnet.microsoft.com/en-us/download)  
- [MySQL Server](https://dev.mysql.com/downloads/mysql/) instalado e configurado  
- Editor recomendado: Visual Studio Code ou Visual Studio 2022+  
- Ferramentas EF Core (instaladas via `dotnet tool install --global dotnet-ef`)

## Aspetos Técnicos

A aplicação foi desenvolvida com:

- **ASP.NET Core MVC**
- **Entity Framework Core**
- **MySQL** como base de dados
- **ASP.NET Identity** para gestão de utilizadores e papéis (roles)
- **Seed automático** de papéis "Admin" e "Cliente"
- Criação automática de um **utilizador administrador** (apenas em ambiente de desenvolvimento)
