/* namespace PawBuddy.Data.Seed;

//temos q fazer add range

internal class DbInitializer {

  internal static async void Initialize(ApplicationDbContext dbContext) {

   

     ArgumentNullException.ThrowIfNull(dbContext, nameof(dbContext));
     dbContext.Database.EnsureCreated();

     // var auxiliar
     bool haAdicao = false;


     // Se não houver Categorias, cria-as
     var categorias = Array.Empty<Categorias>();

     if (dbContext.Categorias.Count() == 5) { //comeca tudo a 0
        categorias = [
           new Categorias { Categoria = "Paisagens" },
           new Categorias { Categoria = "Faróis" },
           new Categorias { Categoria = "Castelos" },
           new Categorias { Categoria = "Pontes" }
           //adicionar outras categorias
        ];
        await dbContext.Categorias.AddRangeAsync(categorias);
        haAdicao = true;
     }


     // Se não houver Utilizadores Identity, cria-os
     var newIdentityUsers = Array.Empty<IdentityUser>();
     //a hasher to hash the password before seeding the user to the db
     var hasher = new PasswordHasher<IdentityUser>();

     if (dbContext.Users.Count() == 1) {
        newIdentityUsers = [
           new IdentityUser{
              UserName="joao.mendes@mail.pt",
              NormalizedUserName="JOAO.MENDES@MAIL.PT",
              Email="joao.mendes@mail.pt",
              NormalizedEmail="JOAO.MENDES@MAIL.PT",
              EmailConfirmed=true,
              SecurityStamp=Guid.NewGuid().ToString("N").ToUpper(),
              ConcurrencyStamp=Guid.NewGuid().ToString(),
              PasswordHash=hasher.HashPassword(null,"Aa0_aa")
           },
           new IdentityUser{
              UserName="maria.sousa@mail.pt",
              NormalizedUserName="MARIA.SOUSA@MAIL.PT",
              Email="maria.sousa@mail.pt",
              NormalizedEmail="MARIA.SOUSA@MAIL.PT",
              EmailConfirmed=true,
              SecurityStamp=Guid.NewGuid().ToString("N").ToUpper(),
              ConcurrencyStamp=Guid.NewGuid().ToString(),
              PasswordHash=hasher.HashPassword(null,"Aa0_aa")
           },
           new IdentityUser{
              UserName="ana.silva@mail.pt",
              NormalizedUserName="ANA.SILVA@MAIL.PT",
              Email="ana.silva@mail.pt",
              NormalizedEmail="ANA.SILVA@MAIL.PT",
              EmailConfirmed=true,
              SecurityStamp=Guid.NewGuid().ToString("N").ToUpper(),
              ConcurrencyStamp=Guid.NewGuid().ToString(),
              PasswordHash=hasher.HashPassword(null,"Aa0_aa")
           }
        ];
        await dbContext.Users.AddRangeAsync(newIdentityUsers);
        haAdicao = true;
     }


     // Se não houver Utilizadores, cria-os
     var utilizadores = Array.Empty<Utilizadores>();
     if (!dbContext.Utilizadores.Any()) {
        utilizadores = [
           new Utilizadores { Nome="João Mendes", Morada="Rua das Flores, 45", CodPostal="2300-000 TOMAR", Pais="Portugal", NIF="123456789", Telemovel="919876543" , UserName=newIdentityUsers[0].UserName },
           new Utilizadores { Nome="Maria Sousa", NIF="123459876", UserName=newIdentityUsers[1].UserName  },
           new Utilizadores { Nome="Ana Paula Silva", NIF="123459867", Telemovel="935678921", UserName=newIdentityUsers[2].UserName  }
          ];
        await dbContext.Utilizadores.AddRangeAsync(utilizadores);
        haAdicao = true;
     }



     // Se não houver Fotografias, cria-as
     var fotografias = Array.Empty<Fotografias>();
     if (!dbContext.Fotografias.Any()) {
        fotografias = [
           new Fotografias{Titulo="Vale das Rosas, Marrocos", Descricao="Cerca de 3000 a 4000 toneladas de rosas silvestres são produzidas no Vale das Rosas todos os anos. A roseira de Damasco que cresce neste vale é muito popular devido à sua intensa fragrância. A maior atração da zona é o 'Festival do Vale das Rosas', que inclui deliciosa gastronomia, danças e cantares.", Ficheiro="AA1291sb.jpg", Data=DateTime.Parse("2021-05-14"), Preco=67, Categoria=categorias[0], Dono=utilizadores[2] },
           new Fotografias{Titulo="Pando, Utah, EUA", Descricao="O Pando é o organismo vivo de maiores dimensões conhecido na Terra. O sistema coletivo de raízes destas 40.000 árvores pesa quase 6000 toneladas e ocupa um espaço de cerca de 43 hectares! As caraterísticas únicas deste bosque, fizeram dele um local famoso. As pessoas visitam-no para admirar e viver a experiência do Pando, especialmente, quando as folhas mudam de cor.", Ficheiro="AA129g7O.jpg", Data=DateTime.Parse("2021-05-13"), Preco=166, Categoria=categorias[0], Dono=utilizadores[2] },
           new Fotografias{Titulo="Smitswinkel Bay, África do Sul", Descricao="A praia de Smitswinkel Bay, na Cidade do Cabo, constitui um bom exemplo de um local famoso que é, também, isolado, escondido e relativamente inacessível, proporcionando vistas magníficas do oceano e das montanhas. Nadar, fazer mergulho, pescar e praticar snorkeling são algumas das suas numerosas atrações.", Ficheiro="AA12ikRt.jpg", Data=DateTime.Parse("2021-10-07"), Preco=143, Categoria=categorias[0], Dono=utilizadores[0] },
           new Fotografias{Titulo="Minterne Magna, Reino Unido", Descricao="A deslumbrante vila de Minterne Magna situa-se na linda região de Dorset. Existe uma vasta gama de flores nos jardins de Minterne, incluindo rododendros, narcisos, jacintos-dos-campos, prímulas e cerejeiras ornamentais. Aproximadamente metade de todos os jacintos-dos-campos silvestres de todo o mundo ocorrem no Reino Unido, graças, sobretudo, ao clima fresco e húmido do país.", Ficheiro="AA128ZdE.jpg", Data=DateTime.Parse("2021-09-23"), Preco=171, Categoria=categorias[0], Dono=utilizadores[2] },
           new Fotografias{Titulo="Os Doze Apóstolos, África do Sul", Descricao="Os Doze Apóstolos fazem parte do complexo de Table Mountain, situado ao longo do Oceano Atlântico, na costa oeste da Península do Cabo. A cordilheira montanhosa estende-se por 6 quilómetros e distingue-se pelas vistas panorâmicas. Esta longa e bela extensão é bordejada por magníficas formações rochosas graníticas, praias deslumbrantes e uma enorme variedade de restaurantes, clubes e hotéis.", Ficheiro="AA10b222.jpg", Data=DateTime.Parse("2023-07-02"), Preco=177, Categoria=categorias[0], Dono=utilizadores[0] },
           new Fotografias{Titulo="Grossglocknerstrasse, Áustria", Descricao="Também conhecida como a Estrada Alpina de Grossglockner, é a estrada de montanha pavimentada mais elevada da Áustria e recebe o nome da montanha mais alta do país. Esta estrada de 48 quilómetros com 36 curvas em cotovelo apertado percorre uma paisagem alpina de caraterísticas únicas, incluindo os campos de neve do vasto glaciar, enquanto explora o maior parque nacional da Áustria.", Ficheiro="AA10aZTc.jpg", Data=DateTime.Parse("2023-03-08"), Preco=71, Categoria=categorias[0], Dono=utilizadores[2] },
           new Fotografias{Titulo="Campos de canola, Alemanha", Descricao="Ao conduzir na Alemanha durante a primavera, é impossível não notar estes belos campos ondulantes de colza em flor, uma cultura comum no país. As flores de um amarelo vibrante atraem muitos visitantes às zonas rurais e os tapetes dourados fazem as delícias dos fotógrafos e dos amantes da natureza.", Ficheiro="AA10kniX.jpg", Data=DateTime.Parse("2024-12-31"), Preco=150, Categoria=categorias[0], Dono=utilizadores[1] },
           new Fotografias{Titulo="Vale do Tugela, África do Sul", Descricao="Esta paisagem mágica situa-se no Parque Nacional de Royal Natal, na província de KwaZulu-Natal. O vale oferece vistas deslumbrantes do sinuoso rio Tugela, que nasce no planalto elevado de Mont-aux-Sources, ergue-se majestosamente como pano de fundo. Os visitantes adoram explorar os trilhos e as cataratas desta reserva, a pé ou a cavalo, desfrutando as vistas mais cativantes do vale.", Ficheiro="AAXDGOZ.jpg", Data=DateTime.Parse("2023-08-28"), Preco=175, Categoria=categorias[0], Dono=utilizadores[2] },
           new Fotografias{Titulo="Gasa, Butão", Descricao="Esta belíssima região, com elevações entre os 1500 e os 4500 metros, constitui o distrito mais setentrional do reino himalaico do Butão. A região é popularmente conhecida pelo 'Snowman Trek', o 'trilho do boneco de neve'. Este distrito montanhoso é habitado por uma pequena população de cerca de 3000 pessoas, a maioria das quais pertence ao povo Layap ⁠— pastores nómadas.", Ficheiro="AAXD1JB.jpg", Data=DateTime.Parse("2021-02-14"), Preco=59, Categoria=categorias[0], Dono=utilizadores[0] },
           new Fotografias{Titulo="Vilas de Merina, Madagáscar", Descricao="Esta linda paisagem pertence à vila de Merina, localizada a sul de Antananarivo, a capital e a maior cidade de Madagáscar. A vila é habitada pelo povo malgaxe ou das 'terras altas', o maior grupo étnico da ilha. Alguns dos muito produtivos socalcos de arroz e quintas que aqui se encontram foram construídos graças à inovadora infraestrutura de irrigação, fazendo da região uma visão inspiradora.", Ficheiro="AAWcxwo.jpg", Data=DateTime.Parse("2023-10-04"), Preco=78, Categoria=categorias[0], Dono=utilizadores[0] },
           new Fotografias{Titulo="Langkawi, Malásia", Descricao="Ao longo das últimas décadas, Langkawi tornou-se um destino favorito para os turistas amantes da praia de todas as nacionalidades. Langkawi é o nome da ilha principal e do arquipélago mais vasto que a rodeia, composto por cerca de 99 ilhas e ilhéus no Estreito de Malaca.", Ficheiro="AA1vs0ln.jpg", Data=DateTime.Parse("2022-01-24"), Preco=123, Categoria=categorias[1], Dono=utilizadores[1] },
           new Fotografias{Titulo="Farol de Galley Head, Irlanda", Descricao="Erguendo-se bem acima das ondas que rebentam na costa pronunciada de Cork, o Farol de Galley Head é uma maravilha de grande utilidade. Aceso pela primeira vez em 1878, o seu poderoso farol quebrou a frequente escuridão marítima e, em noites claras, podia ser visto a até 16 milhas náuticas, o que o tornava, na altura, o farol mais potente do mundo.", Ficheiro="AA1sfVvk.jpg", Data=DateTime.Parse("2023-03-04"), Preco=146, Categoria=categorias[1], Dono=utilizadores[1] },
           new Fotografias{Titulo="Península de Snaefellsnes, Islândia", Descricao="O farol de Svortuloft situa-se no extremo oeste da península de Snaefellsnes, na Islândia. O farol está localizado na extremidade de um rochedo com 4 quilómetros de extensão. A maioria dos viajantes gosta de visitar este farol espetacular durante o verão, para desfrutar da magnífica vista.", Ficheiro="AA17doeJ.jpg", Data=DateTime.Parse("2021-12-31"), Preco=166, Categoria=categorias[1], Dono=utilizadores[0] },
           new Fotografias{Titulo="Happisburgh, Norfolk, Reino Unido", Descricao="Esta bonita imagem mostra um céu azul brilhante, com nuvens de um branco imaculado semelhantes a bolas de algodão que contrastam com o amarelo e o verde dos campos de colza em Norfolk, Reino Unido. As estradas de um castanho poeirento são o complemento perfeito para as riscas de cor vermelha e branca do farol.", Ficheiro="AA129g71.jpg", Data=DateTime.Parse("2022-02-14"), Preco=176, Categoria=categorias[1], Dono=utilizadores[0] },
           new Fotografias{Titulo="Maine, Nova Inglaterra, EUA", Descricao="Um dos mais antigos e históricos faróis do Maine, Portland Head Light tem muitas histórias para contar. Circundado por vistas deslumbrantes em todos os lados, o farol foi construído e concluído em 1791 sob as ordens de George Washington, com a ajuda de um fundo estabelecido por este. Atualmente, a luz está automatizada e as restantes partes do farol estão ao cuidado da Guarda Costeira dos USA.", Ficheiro="AA17dAkA.jpg", Data=DateTime.Parse("2022-08-02"), Preco=178, Categoria=categorias[1], Dono=utilizadores[0] },
           new Fotografias{Titulo="Farol do Cabo Lefkas, Grécia", Descricao="O cabo Lefkas, também conhecido como cabo Doukato, é um dos locais mais pitorescos da ilha de Lefkada, graças à vista panorâmica e à paisagem agreste. O farol localiza-se na extremidade do cabo. Entrou ao serviço em 1890 e continuou em funcionamento até à atualidade. Em tempos, existiu um pequeno templo, dedicado ao deus grego Apolo, no local onde hoje se ergue o farol.", Ficheiro="AA10b1mt.jpg", Data=DateTime.Parse("2022-10-28"), Preco=164, Categoria=categorias[1], Dono=utilizadores[1] },
           new Fotografias{Titulo="Farol de Baily, Irlanda", Descricao="Situada na extremidade sudeste da península de Howth, a estrutura atual do farol de Baily foi construída em 1814. É possível observar vistas espetaculares do farol a partir do Howth Summit, situado nas proximidades. Existe um trilho de caminhada ao longo das falésias que proporciona vistas fantásticas da baía de Dublin, da ilha de Dalkey e das montanhas de Wicklow.", Ficheiro="AAKowXc.jpg", Data=DateTime.Parse("2022-05-27"), Preco=198, Categoria=categorias[1], Dono=utilizadores[1] },
           new Fotografias{Titulo="Farol de Pilsum, Alemanha", Descricao="Localizado na costa alemã do Mar do Norte, este farol foi construído no ano de 1891. Controlou a navegação no canal de Emshörn até 1915. Recentemente, a torre vermelha e amarela tornou-se um popular destino turístico, tendo aparecido em vários filmes e anúncios comerciais. ", Ficheiro="BB1fqr9d.jpg", Data=DateTime.Parse("2020-12-16"), Preco=135, Categoria=categorias[1], Dono=utilizadores[2] },
           new Fotografias{Titulo="Farol de Cap Spartel, Marrocos", Descricao="Localizado acima das Grutas de Hércules, a oeste do porto de Tânger, o farol ergue-se à entrada do Estreito de Gibraltar. Esta estrutura imponente é famosa pela sua arquitetura de estilo árabe tradicional.", Ficheiro="BB19YejK.jpg", Data=DateTime.Parse("2024-05-07"), Preco=114, Categoria=categorias[1], Dono=utilizadores[1] },
           new Fotografias{Titulo="Farol de Petit Minou, França", Descricao="O 'Phare du Petit Minou' é um de entre as dezenas de faróis espalhados pela costa escarpada e por vezes tempestuosa da Bretanha e foi erguido em frente ao forte para ajudar a navegação no estreito Goulet de Brest. Vale a pena admirá-lo de todos os ângulos num passeio de barco.", Ficheiro="AAE8GPV.jpg", Data=DateTime.Parse("2020-02-20"), Preco=102, Categoria=categorias[1], Dono=utilizadores[0] },
           new Fotografias{Titulo="Castelo de Eltz, Alemanha", Descricao="A primeira fase da construção do Castelo de Eltz, na região da Renânia-Palatinado, na Alemanha, teve início em 1157 e, desde então, os membros da família Eltz são proprietários do castelo. Uma disputa entre três irmãos em 1268 fez com que a propriedade fosse dividida entre eles, e o castelo continua dividido entre estes três ramos da família: os Rübenachs, os Rodendorfs e os Kempenich.", Ficheiro="AA1vs0xs.jpg", Data=DateTime.Parse("2020-02-02"), Preco=126, Categoria=categorias[2], Dono=utilizadores[0] },
           new Fotografias{Titulo="Castelo de Rochlitz, Alemanha", Descricao="Numa curva do rio Zwickauer Mulde, as águas calmas são um espelho de um exemplo incrivelmente preservado de arquitetura românica. Trata-se do Castelo de Rochlitz, na Saxónia, Alemanha, iniciado no século XI, tendo sido cuidadosamente preservado desde então. Atualmente, quase todo o castelo de Rochlitz é um museu que permite aos visitantes regressar ao seu apogeu cultural medieval.", Ficheiro="AA1semSD.jpg", Data=DateTime.Parse("2023-05-07"), Preco=134, Categoria=categorias[2], Dono=utilizadores[2] },
           new Fotografias{Titulo="Castelo de Tarasp, Suíça", Descricao="Para um castelo brilhar nesta majestosa paisagem alpina suíça, tem de ser especial. O castelo de Tarasp é um desses destaques. Fundado no século X, o castelo foi ampliado ao longo dos séculos, tornando-se cada vez mais grandioso e impressionante no topo deste afloramento rochoso. Tarasp foi abandonado em 1815 e caiu em ruínas. Tal como outras fortalezas, esta estrutura foi salva por conservacionistas atentos.", Ficheiro="AA1pkmTg.jpg", Data=DateTime.Parse("2024-02-18"), Preco=168, Categoria=categorias[2], Dono=utilizadores[2] },
           new Fotografias{Titulo="Castelo de Bobolice, Polónia", Descricao="Muitas lendas giram em torno desta impressionante fortaleza no sul da Polónia. Construído pela primeira vez no século XIV, o castelo mudou de mãos muitas vezes, como é habitual nos castelos. As verdadeiras fissuras na sua armadura de pedra começaram em 1587, e continuou a degradar-se e era apenas uma ruína melancólica em 1999, altura em que foi lançado um enorme plano de reconstrução. Como se pode ver, o restauro foi um sucesso estrondoso.", Ficheiro="BB1lURv1.jpg", Data=DateTime.Parse("2023-02-11"), Preco=130, Categoria=categorias[2], Dono=utilizadores[0] },
           new Fotografias{Titulo="Castelo de Ross, Irlanda", Descricao="Construído no século XV para o clã O'Donoghue, este castelo nas margens de Lough Leane, no sudoeste da Irlanda, viu vários proprietários chegar e partir. O estilo arquitetónico do Castelo de Ross é puro estilo de defesa: é uma casa-torre e uma torre de menagem, em blocos e robusta, que constituiu uma fortaleza formidável.", Ficheiro="BB1jiHXz.jpg", Data=DateTime.Parse("2024-05-17"), Preco=125, Categoria=categorias[2], Dono=utilizadores[2] },
           new Fotografias{Titulo="Castelo de Kilchurn, Escócia", Descricao="O castelo de Kilchurn é uma fortaleza arruinada que se ergue numa península rochosa no extremo nordeste do Loch Awe. O castelo foi originalmente construído no século XV por Sir Colin Campbell, primeiro Lord of Glenorchy, para servir de base aos Campbells of Glenorchy. O edifício original era constituído por uma torre de cinco andares, com um pátio defendido por um muro exterior, e foi acrescentado no século XVII.", Ficheiro="AA12gNTy.jpg", Data=DateTime.Parse("2023-12-03"), Preco=177, Categoria=categorias[2], Dono=utilizadores[2] },
           new Fotografias{Titulo="Castelo de If, França", Descricao="Esta fortaleza que se ergue sobre uma pequena ilha calcária situada em frente ao porto de Marselha, no sudeste da França, é um monumento histórico mandado construir por Francisco I em 1524. O castelo foi originalmente construído como uma fortaleza naval e, mais tarde, utilizado como uma prisão de estado durante vários séculos. O Castelo de If foi desmilitarizado e abriu ao público em 1890.", Ficheiro="AAWfpHa.jpg", Data=DateTime.Parse("2023-07-13"), Preco=56, Categoria=categorias[2], Dono=utilizadores[0] },
           new Fotografias{Titulo="Castelo de Iwakuni, Japão", Descricao="Este castelo histórico localizado na Prefeitura de Yamaguchi foi originalmente construído em 1608 por Kikkawa Hiroie no Monte Shiroyama. Após a entrada em vigor da lei de Tokugawa 'um castelo por província', o castelo foi desmantelado e uma réplica foi construída em 1962. A torre proporciona uma bela vista da cidade de Iwakuni e expõe um modelo da ponte de Kintaikyo, espadas e armaduras de samurais.", Ficheiro="AAWeGXW.jpg", Data=DateTime.Parse("2020-01-03"), Preco=68, Categoria=categorias[2], Dono=utilizadores[2] },
           new Fotografias{Titulo="Dalian, China", Descricao="O crepúsculo chega a Dalian e, à medida que os carros passam, a ponte da baía de Xinghai adquire uma espécie de beleza absoluta. Os viadutos que vemos aqui alimentam o vão principal da ponte, que atravessa a Baía de Xinghai, canalizando o tráfego através de uma área urbana com mais de 6 milhões de pessoas. Apesar da enorme população, Dalian é conhecida pelos seus espaços verdes abertos, boa qualidade do ar e condições de tráfego aceitáveis.", Ficheiro="AA1vs447.jpg", Data=DateTime.Parse("2023-03-08"), Preco=93, Categoria=categorias[3], Dono=utilizadores[0] },
           new Fotografias{Titulo="Ponte de Seri Wawasan, Malásia", Descricao="Estamos a olhar para uma maravilha da engenharia que se estende pelas margens do Lago Putrajaya. Algumas pessoas descrevem os cabos da Ponte de Seri Wawasan como velas, outras como asas, mas a maioria concorda que o efeito visual é impressionante. Quando o sol se põe, um espetáculo de luzes que ilumina a ponte com cores variadas proporciona uma visão etérea.", Ficheiro="BB1lUToh.jpg", Data=DateTime.Parse("2020-03-19"), Preco=163, Categoria=categorias[3], Dono=utilizadores[2] },
           new Fotografias{Titulo="Bosque Nubloso de Monteverde, Costa Rica", Descricao="Cerca de 10.500 hectares de floresta nublada estão protegidos numa extraordinária reserva natural no norte da Costa Rica. Na maioria dos dias, as pontes suspensas de Monteverde, como a vermelha aqui, levam os visitantes a uma altura suficiente para passear por entre os bancos de nuvens. Toda esta humidade revigorante é ideal para a biodiversidade.", Ficheiro="BB1lUQX1.jpg", Data=DateTime.Parse("2020-02-02"), Preco=100, Categoria=categorias[3], Dono=utilizadores[1] },
           new Fotografias{Titulo="Garganta de Schöllenen, Suíça", Descricao="O poder erosivo da parte superior do rio Reuss esculpe o maciço de Aar na Suíça e abre um espetacular desfiladeiro no granito. A imagem mostra três pontes: a mais distante é a ponte ferroviária com arcos de Schöllenenbahn, que desaparece num túnel. A mais próxima é a 'Terceira Ponte do Diabo', construída em 1958, e logo abaixo está a 'Segunda Ponte do Diabo', datada de 1830.", Ficheiro="BB1jiRez.jpg", Data=DateTime.Parse("2024-11-22"), Preco=132, Categoria=categorias[3], Dono=utilizadores[0] },
           new Fotografias{Titulo="Ponte de Bastei, Saxónia, Alemanha", Descricao="Numa visão espantosa, a bela Ponte de Bastei foi construída nas Montanhas de Elba do Parque Nacional da Suíça Saxónica, na Alemanha. Erguendo-se quase 194 metros acima do rio Elba, esta robusta ponte proporciona vistas esplendorosas sobre o vale circundante. Uma ponte de madeira construída em 1824 para ligar os rochedos esteve originalmente no lugar agora ocupado pela ponte de Bastei e constitui uma atração turística há mais de 200 anos!", Ficheiro="AA17fKb6.jpg", Data=DateTime.Parse("2021-03-06"), Preco=76, Categoria=categorias[3], Dono=utilizadores[0] },
           new Fotografias{Titulo="Ponte Tsing Ma, Hong Kong", Descricao="A ponte Tsing Ma é a 16.ª ponte suspensa mais extensa do mundo, permitindo o tráfego rodoviário e ferroviário em dois níveis simultaneamente. A ponte possui 41 metros de largura, com dois tabuleiros e um vão principal de 1377 metros e uma altura de 206 metros. A ponte tem o nome das duas ilhas que liga entre si – Tsing Yi e Ma Wan.", Ficheiro="AA149WbT.jpg", Data=DateTime.Parse("2021-01-21"), Preco=187, Categoria=categorias[3], Dono=utilizadores[0] },
           new Fotografias{Titulo="A Ponte dos Nove Arcos, Seri Lanca", Descricao="Popularmente conhecida como a 'Ponte no céu', esta espetacular ponte entrou em funcionamento no ano de 1921, durante o período de domínio britânico. Constituindo um dos melhores exemplos da construção ferroviária da era colonial no país, esta ponte com 91 metros de extensão e 7,6 metros de largura eleva-se a 24-30 metros de altura e está situada de forma ideal entre as estações ferroviárias de Ella e Demodara.", Ficheiro="AAU7WFm.jpg", Data=DateTime.Parse("2021-12-15"), Preco=126, Categoria=categorias[3], Dono=utilizadores[0] },
           new Fotografias{Titulo="Ponte Vasco da Gama, Portugal", Descricao="Com um comprimento superior a 16 quilómetros sobre o rio Tejo, é uma das mais extensas pontes da Europa e liga as zonas norte e sul de Portugal. A ponte, que abriu ao público em 1998, foi construída por cerca de 3300 pessoas que trabalharam em simultâneo e demorou 18 meses a ficar terminada. O nome foi-lhe atribuído em homenagem ao explorador português Vasco da Gama.", Ficheiro="AAKof3l.jpg", Data=DateTime.Parse("2024-01-21"), Preco=111, Categoria=categorias[3], Dono=utilizadores[1] },
           new Fotografias{Titulo="Ponte de Raízes de Ritymmen, Meghalaya, Índia", Descricao="Aninhada na bela vila de Nongthymmai, esta é uma das famosas pontes de raízes vivas do estado. A ponte tem um comprimento de 30 metros, é construída com raízes vivas de árvores da borracha e atada a palmeiras de areca.", Ficheiro="BB1fsGhI.jpg", Data=DateTime.Parse("2023-02-18"), Preco=152, Categoria=categorias[3], Dono=utilizadores[1] },
           new Fotografias{Titulo="Tower Bridge, Londres, Inglaterra", Descricao="Construída entre 1886 e 1894 sobre o Rio Tamisa, a ponte é uma das atrações turísticas mais emblemáticas de Londres. Tem duas torres em estilo gótico vitoriano ligadas por duas passadeiras horizontais, que têm como propósito suportar as forças das secções em suspensão da ponte. É utilizada por cerca de 40.000 pessoas diariamente.", Ficheiro="AAG0uvz.jpg", Data=DateTime.Parse("2023-03-04"), Preco=163, Categoria=categorias[3], Dono=utilizadores[0] },
           new Fotografias{Titulo="Viaduto de Millau, França", Descricao="Elevando-se sobre a paisagem cénica, é indiscutivelmente uma das pontes mais bonitas do mundo. A ponte pode até ser facilmente vista do espaço. Este é o Viaduto de Millau, um exemplo perfeito de como a engenharia se encontra com a arte. Erguido sobre o desfiladeiro do Tarn, no sul de França, e com 2.460 metros de comprimento, o Viaduto de Millau é a ponte mais alta do mundo, com uma altura estrutural de 336,4 metros.", Ficheiro="AA1r0tLE.jpg", Data=DateTime.Parse("2023-06-24"), Preco=128, Categoria=categorias[3], Dono=utilizadores[2] }
        ];
        await dbContext.Fotografias.AddRangeAsync(fotografias);
        haAdicao = true;
     }

     // Se não houver Gostos, cria-as
     var gostos = Array.Empty<Gostos>();
     if (!dbContext.Gostos.Any()) {
        gostos = [
           new Gostos{Fotografia=fotografias[0], Utilizador=utilizadores[0], Data=DateTime.Parse("2025-04-07")},
           new Gostos{Fotografia=fotografias[1], Utilizador=utilizadores[0], Data=DateTime.Parse("2025-04-07")},
           new Gostos{Fotografia=fotografias[4], Utilizador=utilizadores[0], Data=DateTime.Parse("2025-04-10")},
           new Gostos{Fotografia=fotografias[5], Utilizador=utilizadores[0], Data=DateTime.Parse("2025-04-11")},
           new Gostos{Fotografia=fotografias[6], Utilizador=utilizadores[0], Data=DateTime.Parse("2025-04-07")},
           new Gostos{Fotografia=fotografias[7], Utilizador=utilizadores[0], Data=DateTime.Parse("2025-04-09")},
           new Gostos{Fotografia=fotografias[8], Utilizador=utilizadores[0], Data=DateTime.Parse("2025-04-13")},
           new Gostos{Fotografia=fotografias[9], Utilizador=utilizadores[0], Data=DateTime.Parse("2025-04-06")},
           new Gostos{Fotografia=fotografias[10], Utilizador=utilizadores[0], Data=DateTime.Parse("2025-04-09")},
           new Gostos{Fotografia=fotografias[12], Utilizador=utilizadores[0], Data=DateTime.Parse("2025-04-13")},
           new Gostos{Fotografia=fotografias[13], Utilizador=utilizadores[0], Data=DateTime.Parse("2025-04-10")},
           new Gostos{Fotografia=fotografias[16], Utilizador=utilizadores[0], Data=DateTime.Parse("2025-04-10")},
           new Gostos{Fotografia=fotografias[17], Utilizador=utilizadores[0], Data=DateTime.Parse("2025-04-07")},
           new Gostos{Fotografia=fotografias[18], Utilizador=utilizadores[0], Data=DateTime.Parse("2025-04-08")},
           new Gostos{Fotografia=fotografias[19], Utilizador=utilizadores[0], Data=DateTime.Parse("2025-04-11")},
           new Gostos{Fotografia=fotografias[21], Utilizador=utilizadores[0], Data=DateTime.Parse("2025-04-11")},
           new Gostos{Fotografia=fotografias[23], Utilizador=utilizadores[0], Data=DateTime.Parse("2025-04-13")},
           new Gostos{Fotografia=fotografias[24], Utilizador=utilizadores[0], Data=DateTime.Parse("2025-04-06")},
           new Gostos{Fotografia=fotografias[25], Utilizador=utilizadores[0], Data=DateTime.Parse("2025-04-06")},
           new Gostos{Fotografia=fotografias[27], Utilizador=utilizadores[0], Data=DateTime.Parse("2025-04-09")},
           new Gostos{Fotografia=fotografias[28], Utilizador=utilizadores[0], Data=DateTime.Parse("2025-04-09")},
           new Gostos{Fotografia=fotografias[29], Utilizador=utilizadores[0], Data=DateTime.Parse("2025-04-10")},
           new Gostos{Fotografia=fotografias[31], Utilizador=utilizadores[0], Data=DateTime.Parse("2025-04-07")},
           new Gostos{Fotografia=fotografias[33], Utilizador=utilizadores[0], Data=DateTime.Parse("2025-04-12")},
           new Gostos{Fotografia=fotografias[35], Utilizador=utilizadores[0], Data=DateTime.Parse("2025-04-08")},
           new Gostos{Fotografia=fotografias[36], Utilizador=utilizadores[0], Data=DateTime.Parse("2025-04-11")},
           new Gostos{Fotografia=fotografias[37], Utilizador=utilizadores[0], Data=DateTime.Parse("2025-04-09")},
           new Gostos{Fotografia=fotografias[0], Utilizador=utilizadores[1], Data=DateTime.Parse("2025-04-10")},
           new Gostos{Fotografia=fotografias[2], Utilizador=utilizadores[1], Data=DateTime.Parse("2025-04-11")},
           new Gostos{Fotografia=fotografias[3], Utilizador=utilizadores[1], Data=DateTime.Parse("2025-04-11")},
           new Gostos{Fotografia=fotografias[4], Utilizador=utilizadores[1], Data=DateTime.Parse("2025-04-06")},
           new Gostos{Fotografia=fotografias[5], Utilizador=utilizadores[1], Data=DateTime.Parse("2025-04-12")},
           new Gostos{Fotografia=fotografias[6], Utilizador=utilizadores[1], Data=DateTime.Parse("2025-04-09")},
           new Gostos{Fotografia=fotografias[7], Utilizador=utilizadores[1], Data=DateTime.Parse("2025-04-13")},
           new Gostos{Fotografia=fotografias[8], Utilizador=utilizadores[1], Data=DateTime.Parse("2025-04-08")},
           new Gostos{Fotografia=fotografias[12], Utilizador=utilizadores[1], Data=DateTime.Parse("2025-04-13")},
           new Gostos{Fotografia=fotografias[17], Utilizador=utilizadores[1], Data=DateTime.Parse("2025-04-06")},
           new Gostos{Fotografia=fotografias[18], Utilizador=utilizadores[1], Data=DateTime.Parse("2025-04-09")},
           new Gostos{Fotografia=fotografias[19], Utilizador=utilizadores[1], Data=DateTime.Parse("2025-04-08")},
           new Gostos{Fotografia=fotografias[20], Utilizador=utilizadores[1], Data=DateTime.Parse("2025-04-12")},
           new Gostos{Fotografia=fotografias[21], Utilizador=utilizadores[1], Data=DateTime.Parse("2025-04-08")},
           new Gostos{Fotografia=fotografias[23], Utilizador=utilizadores[1], Data=DateTime.Parse("2025-04-08")},
           new Gostos{Fotografia=fotografias[24], Utilizador=utilizadores[1], Data=DateTime.Parse("2025-04-09")},
           new Gostos{Fotografia=fotografias[25], Utilizador=utilizadores[1], Data=DateTime.Parse("2025-04-12")},
           new Gostos{Fotografia=fotografias[26], Utilizador=utilizadores[1], Data=DateTime.Parse("2025-04-11")},
           new Gostos{Fotografia=fotografias[27], Utilizador=utilizadores[1], Data=DateTime.Parse("2025-04-12")},
           new Gostos{Fotografia=fotografias[28], Utilizador=utilizadores[1], Data=DateTime.Parse("2025-04-11")},
           new Gostos{Fotografia=fotografias[30], Utilizador=utilizadores[1], Data=DateTime.Parse("2025-04-10")},
           new Gostos{Fotografia=fotografias[31], Utilizador=utilizadores[1], Data=DateTime.Parse("2025-04-10")},
           new Gostos{Fotografia=fotografias[32], Utilizador=utilizadores[1], Data=DateTime.Parse("2025-04-09")},
           new Gostos{Fotografia=fotografias[33], Utilizador=utilizadores[1], Data=DateTime.Parse("2025-04-10")},
           new Gostos{Fotografia=fotografias[34], Utilizador=utilizadores[1], Data=DateTime.Parse("2025-04-09")},
           new Gostos{Fotografia=fotografias[35], Utilizador=utilizadores[1], Data=DateTime.Parse("2025-04-12")},
           new Gostos{Fotografia=fotografias[36], Utilizador=utilizadores[1], Data=DateTime.Parse("2025-04-08")},
           new Gostos{Fotografia=fotografias[38], Utilizador=utilizadores[1], Data=DateTime.Parse("2025-04-08")},
           new Gostos{Fotografia=fotografias[0], Utilizador=utilizadores[2], Data=DateTime.Parse("2025-04-07")},
           new Gostos{Fotografia=fotografias[1], Utilizador=utilizadores[2], Data=DateTime.Parse("2025-04-06")},
           new Gostos{Fotografia=fotografias[3], Utilizador=utilizadores[2], Data=DateTime.Parse("2025-04-09")},
           new Gostos{Fotografia=fotografias[5], Utilizador=utilizadores[2], Data=DateTime.Parse("2025-04-13")},
           new Gostos{Fotografia=fotografias[6], Utilizador=utilizadores[2], Data=DateTime.Parse("2025-04-12")},
           new Gostos{Fotografia=fotografias[7], Utilizador=utilizadores[2], Data=DateTime.Parse("2025-04-06")},
           new Gostos{Fotografia=fotografias[8], Utilizador=utilizadores[2], Data=DateTime.Parse("2025-04-07")},
           new Gostos{Fotografia=fotografias[9], Utilizador=utilizadores[2], Data=DateTime.Parse("2025-04-13")},
           new Gostos{Fotografia=fotografias[10], Utilizador=utilizadores[2], Data=DateTime.Parse("2025-04-12")},
           new Gostos{Fotografia=fotografias[13], Utilizador=utilizadores[2], Data=DateTime.Parse("2025-04-13")},
           new Gostos{Fotografia=fotografias[14], Utilizador=utilizadores[2], Data=DateTime.Parse("2025-04-09")},
           new Gostos{Fotografia=fotografias[15], Utilizador=utilizadores[2], Data=DateTime.Parse("2025-04-12")},
           new Gostos{Fotografia=fotografias[16], Utilizador=utilizadores[2], Data=DateTime.Parse("2025-04-10")},
           new Gostos{Fotografia=fotografias[17], Utilizador=utilizadores[2], Data=DateTime.Parse("2025-04-11")},
           new Gostos{Fotografia=fotografias[18], Utilizador=utilizadores[2], Data=DateTime.Parse("2025-04-09")},
           new Gostos{Fotografia=fotografias[19], Utilizador=utilizadores[2], Data=DateTime.Parse("2025-04-13")},
           new Gostos{Fotografia=fotografias[20], Utilizador=utilizadores[2], Data=DateTime.Parse("2025-04-08")},
           new Gostos{Fotografia=fotografias[21], Utilizador=utilizadores[2], Data=DateTime.Parse("2025-04-10")},
           new Gostos{Fotografia=fotografias[22], Utilizador=utilizadores[2], Data=DateTime.Parse("2025-04-09")},
           new Gostos{Fotografia=fotografias[24], Utilizador=utilizadores[2], Data=DateTime.Parse("2025-04-11")},
           new Gostos{Fotografia=fotografias[25], Utilizador=utilizadores[2], Data=DateTime.Parse("2025-04-07")},
           new Gostos{Fotografia=fotografias[28], Utilizador=utilizadores[2], Data=DateTime.Parse("2025-04-07")},
           new Gostos{Fotografia=fotografias[29], Utilizador=utilizadores[2], Data=DateTime.Parse("2025-04-11")},
           new Gostos{Fotografia=fotografias[31], Utilizador=utilizadores[2], Data=DateTime.Parse("2025-04-07")},
           new Gostos{Fotografia=fotografias[32], Utilizador=utilizadores[2], Data=DateTime.Parse("2025-04-11")},
           new Gostos{Fotografia=fotografias[34], Utilizador=utilizadores[2], Data=DateTime.Parse("2025-04-08")},
           new Gostos{Fotografia=fotografias[35], Utilizador=utilizadores[2], Data=DateTime.Parse("2025-04-09")},
           new Gostos{Fotografia=fotografias[36], Utilizador=utilizadores[2], Data=DateTime.Parse("2025-04-12")},
           new Gostos{Fotografia=fotografias[37], Utilizador=utilizadores[2], Data=DateTime.Parse("2025-04-08")},
           new Gostos{Fotografia=fotografias[38], Utilizador=utilizadores[2], Data=DateTime.Parse("2025-04-08")}
        ];
        await dbContext.Gostos.AddRangeAsync(gostos);
        haAdicao = true;
     }


     try {
        if (haAdicao) {
           // tornar persistentes os dados
           dbContext.SaveChanges();
        }
     }
     catch (Exception ex) {

        throw;
     }
  }
} */
  