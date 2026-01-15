using EstudoIA.Version1.Application.Abstractions.Handlers;
using EstudoIA.Version1.Application.Data.UserTripPlans;
using EstudoIA.Version1.Application.Feature.IA.Turismo.Models;
using EstudoIA.Version1.Application.Shared.HttpClients.IA;
using EstudoIA.Version1.Application.Shared.HttpClients.IA.Gemini;
using EstudosIA.Version1.ApplicationCommon.Results;
using FluentValidation;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace EstudoIA.Version1.Application.Feature.IA.Turismo.Handler.Command;

public class CreateTurismoHandler : HandlerBase<TourismSummaryRequest, TourismSummaryResponse>
{
   
    private readonly TourismSummaryService _service;
    private readonly IUserTripPlansContext _userTripPlansContext;


    public CreateTurismoHandler(TourismSummaryService tourismSummaryService, IUserTripPlansContext userTripPlansContext)
    {
        _service = tourismSummaryService;
        _userTripPlansContext = userTripPlansContext;
    }

    protected override async Task<Result<TourismSummaryResponse>> ExecuteAsync(
    TourismSummaryRequest request,
    CancellationToken cancellationToken)
    {
        if (request.IsMock)
        {
            const string mockJson = """
            {
              "City": "Lisboa",
              "Country": "Portugal",
              "Spots": [
                {
                  "Id": "3a85e050-9664-461b-9cac-6ec1fb0f53fb",
                  "Name": "Castelo de São Jorge",
                  "Lat": 38.713442,
                  "Lng": -9.133373,
                  "Tips": [
                    "Compre ingressos online para evitar filas.",
                    "Use calçado confortável para subir as colinas."
                  ],
                  "WhyGo": "Para explorar a história de Lisboa e apreciar vistas deslumbrantes do alto das colinas. Ideal para fotos e passeios ao pôr do sol.",
                  "Safety": { "Notes": "", "Scams": "low", "Overall": "medium", "Robbery": "medium", "Pickpocketing": "medium" },
                  "Category": "cultura",
                  "ImageUrl": "https://upload.wikimedia.org/wikipedia/commons/thumb/1/17/Castelo_de_S%C3%A3o_Jorge_%2810251035013%29.jpg/1280px-Castelo_de_S%C3%A3o_Jorge_%2810251035013%29.jpg",
                  "IsInRoute": false,
                  "PlaceQuery": "Castelo de São Jorge, Lisboa",
                  "PriceRange": "R$ 10-20",
                  "TimeNeeded": "2-3 horas",
                  "ImageSource": "wikipedia",
                  "HowToGetThere": "Metro até Santa Apolónia e caminhada de 15 minutos ou bonde 28.",
                  "OneLineSummary": "Fortaleza histórica com vistas panorâmicas da cidade e ruínas romanas.",
                  "OpeningHoursNote": "Aberto das 9h às 18h (fechado às segundas-feiras).",
                  "NeighborhoodOrArea": "Alfama"
                },
                {
                  "Id": "10a8ca6e-b8ac-4ea0-9661-db6c32b20c38",
                  "Name": "Alfama",
                  "Lat": 38.700000,
                  "Lng": -9.117000,
                  "Tips": ["Ouça fado em pequenos bares à noite.", "Cuidado com carteiristas em locais turísticos."],
                  "WhyGo": "Para vivenciar a Lisboa mais autêntica, com fado, mercados locais e mirantes como o Miradouro de Santa Luzia.",
                  "Safety": { "Notes": "", "Scams": "low", "Overall": "medium", "Robbery": "medium", "Pickpocketing": "medium" },
                  "Category": "cultura",
                  "ImageUrl": "https://upload.wikimedia.org/wikipedia/commons/thumb/c/c1/Alfama%2C_Lisboa_-_2010-09-09.jpg/1280px-Alfama%2C_Lisboa_-_2010-09-09.jpg",
                  "IsInRoute": false,
                  "PlaceQuery": "Alfama, Lisboa",
                  "PriceRange": "R$ 0-50 (dependendo de atividades)",
                  "TimeNeeded": "3-4 horas",
                  "ImageSource": "wikipedia",
                  "HowToGetThere": "Bonde 28 ou metro até Santa Apolónia.",
                  "OneLineSummary": "Bairro histórico com ruas estreitas, casas coloridas e vistas para o rio Tejo.",
                  "OpeningHoursNote": "Explorar a pé a qualquer hora do dia.",
                  "NeighborhoodOrArea": "Alfama"
                },
                {
                  "Id": "483fa84c-47e1-4214-ad31-7d81af4ba257",
                  "Name": "Mosteiro dos Jerónimos",
                  "Lat": 38.697860,
                  "Lng": -9.206677,
                  "Tips": ["Combine com a Torre de Belém e o Padrão dos Descobrimentos.", "Chegue cedo para evitar multidões."],
                  "WhyGo": "Para admirar a arquitetura renascentista e entender a era das descobertas portuguesas. Tombado como Patrimônio da Humanidade.",
                  "Safety": { "Notes": "", "Scams": "low", "Overall": "medium", "Robbery": "medium", "Pickpocketing": "medium" },
                  "Category": "cultura",
                  "ImageUrl": "https://upload.wikimedia.org/wikipedia/commons/thumb/4/45/Mosteiro_dos_Jeronimos_-_Left_Wing.jpg/1280px-Mosteiro_dos_Jeronimos_-_Left_Wing.jpg",
                  "IsInRoute": false,
                  "PlaceQuery": "Mosteiro dos Jerónimos, Lisboa",
                  "PriceRange": "R$ 10-20",
                  "TimeNeeded": "2-3 horas",
                  "ImageSource": "wikipedia",
                  "HowToGetThere": "Metro até Belém ou autocarro 727.",
                  "OneLineSummary": "Monastério manuelino com detalhes arquitetônicos impressionantes e história ligada às Grandes Navegações.",
                  "OpeningHoursNote": "Aberto das 10h às 17h30 (fechado às segundas-feiras).",
                  "NeighborhoodOrArea": "Belém"
                },
                {
                  "Id": "668ad688-7cd8-4191-9c59-e0ea2d6b2e5b",
                  "Name": "Torre de Belém",
                  "Lat": 38.691137,
                  "Lng": -9.215931,
                  "Tips": ["Visite ao pôr do sol para fotos incríveis.", "Compre ingresso combinado com o Mosteiro dos Jerónimos."],
                  "WhyGo": "Para ver de perto um ícone da história portuguesa e tirar fotos icônicas junto ao rio Tejo.",
                  "Safety": { "Notes": "", "Scams": "low", "Overall": "medium", "Robbery": "medium", "Pickpocketing": "medium" },
                  "Category": "cultura",
                  "ImageUrl": "https://upload.wikimedia.org/wikipedia/commons/thumb/6/65/Torre_Bel%C3%A9m_April_2009-4a.jpg/1280px-Torre_Bel%C3%A9m_April_2009-4a.jpg",
                  "IsInRoute": true,
                  "PlaceQuery": "Torre de Belém, Lisboa",
                  "PriceRange": "R$ 8-15",
                  "TimeNeeded": "1-2 horas",
                  "ImageSource": "wikipedia",
                  "HowToGetThere": "Metro até Belém ou autocarro 727.",
                  "OneLineSummary": "Torre medieval à beira-rio, símbolo de Lisboa e das Grandes Navegações.",
                  "OpeningHoursNote": "Aberto das 10h às 17h (fechado às segundas-feiras).",
                  "NeighborhoodOrArea": "Belém"
                },
                {
                  "Id": "ce00b566-7ecb-4e2b-b066-f7d0b4318354",
                  "Name": "Padrão dos Descobrimentos",
                  "Lat": 38.693596,
                  "Lng": -9.205712,
                  "Tips": ["Suba até o topo para vistas incríveis.", "Visite o museu no interior para mais contexto histórico."],
                  "WhyGo": "Para explorar a história das descobertas marítimas e ter vistas panorâmicas do rio Tejo.",
                  "Safety": { "Notes": "", "Scams": "low", "Overall": "medium", "Robbery": "medium", "Pickpocketing": "medium" },
                  "Category": "cultura",
                  "ImageUrl": "https://upload.wikimedia.org/wikipedia/commons/thumb/b/ba/Monumento_a_los_Descubrimientos%2C_Lisboa%2C_Portugal%2C_2012-05-12%2C_DD_19.JPG/1280px-Monumento_a_los_Descubrimientos%2C_Lisboa%2C_Portugal%2C_2012-05-12%2C_DD_19.JPG",
                  "IsInRoute": false,
                  "PlaceQuery": "Padrão dos Descobrimentos, Lisboa",
                  "PriceRange": "R$ 10-15",
                  "TimeNeeded": "1-2 horas",
                  "ImageSource": "wikipedia",
                  "HowToGetThere": "Metro até Belém ou autocarro 727.",
                  "OneLineSummary": "Monumento em homenagem aos navegadores portugueses, com estátuas de figuras históricas.",
                  "OpeningHoursNote": "Aberto das 10h às 18h (fechado às segundas-feiras).",
                  "NeighborhoodOrArea": "Belém"
                },
                {
                  "Id": "d3af50b8-aaa1-45c0-8a71-ec476dc6a281",
                  "Name": "Miradouro da Senhora do Monte",
                  "Lat": 38.718887,
                  "Lng": -9.132624,
                  "Tips": ["Chegue antes do pôr do sol para garantir um bom lugar.", "Combine com uma visita ao Miradouro da Graça."],
                  "WhyGo": "Para apreciar a cidade de um dos pontos mais altos, com vista para o Castelo de São Jorge e o rio Tejo.",
                  "Safety": { "Notes": "", "Scams": "low", "Overall": "medium", "Robbery": "medium", "Pickpocketing": "medium" },
                  "Category": "cultura",
                  "ImageUrl": "https://upload.wikimedia.org/wikipedia/commons/thumb/3/31/Miradouro_Nossa_Senhora_do_Monte_II.jpg/1280px-Miradouro_Nossa_Senhora_do_Monte_II.jpg",
                  "IsInRoute": false,
                  "PlaceQuery": "Miradouro da Senhora do Monte, Lisboa",
                  "PriceRange": "R$ 0",
                  "TimeNeeded": "30 minutos a 1 hora",
                  "ImageSource": "wikipedia",
                  "HowToGetThere": "Caminhada de 15 minutos desde o Miradouro da Graça ou bonde 28.",
                  "OneLineSummary": "Mirante com vista panorâmica de 360 graus de Lisboa, ideal para pôr do sol.",
                  "OpeningHoursNote": "Aberto 24 horas.",
                  "NeighborhoodOrArea": "Graça"
                },
                {
                  "Id": "0873bcf4-0501-4aa8-a36c-203b11bae107",
                  "Name": "Bairro Alto",
                  "Lat": 38.712753,
                  "Lng": -9.146295,
                  "Tips": ["Experimente pratos como bacalhau à Brás e pastéis de bacalhau.", "Use táxi ou Uber para voltar ao hotel à noite."],
                  "WhyGo": "Para experimentar a culinária portuguesa em tavernas tradicionais e desfrutar de noites animadas com fado ao vivo.",
                  "Safety": { "Notes": "", "Scams": "low", "Overall": "medium", "Robbery": "medium", "Pickpocketing": "medium" },
                  "Category": "gastronomia",
                  "ImageUrl": "https://upload.wikimedia.org/wikipedia/commons/4/42/Bairro_Alto_Lisboa_1.JPG",
                  "IsInRoute": false,
                  "PlaceQuery": "Bairro Alto, Lisboa",
                  "PriceRange": "R$ 30-80 por refeição",
                  "TimeNeeded": "2-3 horas (para jantar e bebidas)",
                  "ImageSource": "wikipedia",
                  "HowToGetThere": "Metro até Baixa-Chiado ou caminhada desde o Rossio.",
                  "OneLineSummary": "Bairro boêmio com ruas cheias de bares, restaurantes e vida noturna.",
                  "OpeningHoursNote": "Restaurantes abertos para almoço e jantar; bares até tarde da noite.",
                  "NeighborhoodOrArea": "Bairro Alto"
                },
                {
                  "Id": "adb8e5e5-c871-49f2-9a20-5dc1652d0ca3",
                  "Name": "Mercado da Ribeira (Time Out Market)",
                  "Lat": 38.7070608,
                  "Lng": -9.1456691,
                  "Tips": ["Experimente o famoso pastel de nata na Manteigaria.", "Chegue cedo para evitar filas nos restaurantes populares."],
                  "WhyGo": "Para provar pratos de diferentes restaurantes em um único lugar, com ambiente animado e variedade de opções.",
                  "Safety": { "Notes": "", "Scams": "low", "Overall": "medium", "Robbery": "medium", "Pickpocketing": "medium" },
                  "Category": "gastronomia",
                  "ImageUrl": "https://upload.wikimedia.org/wikipedia/commons/9/9a/Mercado_Ribeira_Lisbon.JPG",
                  "IsInRoute": false,
                  "PlaceQuery": "Mercado da Ribeira, Lisboa",
                  "PriceRange": "R$ 15-50 por refeição",
                  "TimeNeeded": "1-2 horas",
                  "ImageSource": "wikipedia",
                  "HowToGetThere": "Metro até Cais do Sodré ou caminhada desde a Baixa.",
                  "OneLineSummary": "Mercado gourmet com diversas opções de comida internacional e portuguesa.",
                  "OpeningHoursNote": "Aberto todos os dias das 10h às 00h.",
                  "NeighborhoodOrArea": "Cais do Sodré"
                },
                {
                  "Id": "b93aea1b-4800-4e3e-8ee2-eab1f0002073",
                  "Name": "Pastéis de Belém",
                  "Lat": 38.6975105,
                  "Lng": -9.2032276,
                  "Tips": ["Compre na hora para garantir pastéis frescos.", "Experimente também as outras sobremesas tradicionais."],
                  "WhyGo": "Para provar o verdadeiro pastel de nata, recém-saído do forno, acompanhado de um café.",
                  "Safety": { "Notes": "", "Scams": "low", "Overall": "medium", "Robbery": "medium", "Pickpocketing": "medium" },
                  "Category": "gastronomia",
                  "ImageUrl": "https://upload.wikimedia.org/wikipedia/commons/thumb/0/0c/Pasteis_de_Belem.jpg/1280px-Pasteis_de_Belem.jpg",
                  "IsInRoute": false,
                  "PlaceQuery": "Pastéis de Belém, Lisboa",
                  "PriceRange": "R$ 1-2 por pastel",
                  "TimeNeeded": "30 minutos",
                  "ImageSource": "wikipedia",
                  "HowToGetThere": "Metro até Belém ou autocarro 727.",
                  "OneLineSummary": "Padaria histórica famosa pelo pastel de nata original de Lisboa.",
                  "OpeningHoursNote": "Aberto todos os dias das 8h às 22h.",
                  "NeighborhoodOrArea": "Belém"
                },
                {
                  "Id": "16a6b835-e6e5-4352-aedd-856560cc2de1",
                  "Name": "Parque das Nações",
                  "Lat": 38.767497,
                  "Lng": -9.089667,
                  "Tips": ["Alugue uma bicicleta para explorar a ciclovia ao longo do rio.", "Visite o Oceanário para uma experiência única."],
                  "WhyGo": "Para explorar a Lisboa contemporânea, com espaços verdes, o Oceanário e a ponte Vasco da Gama.",
                  "Safety": { "Notes": "", "Scams": "low", "Overall": "medium", "Robbery": "medium", "Pickpocketing": "medium" },
                  "Category": "aventura",
                  "ImageUrl": "https://upload.wikimedia.org/wikipedia/commons/thumb/8/86/Parque_das_Na%C3%A7%C3%B5es_%28Lisboa%29_localiza%C3%A7%C3%A3o.svg/1280px-Parque_das_Na%C3%A7%C3%B5es_%28Lisboa%29_localiza%C3%A7%C3%A3o.svg.png",
                  "IsInRoute": false,
                  "PlaceQuery": "Parque das Nações, Lisboa",
                  "PriceRange": "R$ 0-20 (dependendo de atividades)",
                  "TimeNeeded": "2-3 horas",
                  "ImageSource": "wikipedia",
                  "HowToGetThere": "Metro até Oriente.",
                  "OneLineSummary": "Área moderna com jardins, ciclovias e vistas para o rio, ideal para caminhadas e cicloturismo.",
                  "OpeningHoursNote": "Aberto 24 horas.",
                  "NeighborhoodOrArea": "Parque das Nações"
                },
                {
                  "Id": "71fc8284-95d0-40f5-b46d-fd9aaba8f7f5",
                  "Name": "Oceanário de Lisboa",
                  "Lat": 38.763542,
                  "Lng": -9.093741,
                  "Tips": ["Compre ingressos online para evitar filas.", "Chegue cedo para evitar multidões."],
                  "WhyGo": "Para uma experiência educativa e deslumbrante, com tubarões, raias e peixes tropicais em um ambiente interativo.",
                  "Safety": { "Notes": "", "Scams": "low", "Overall": "medium", "Robbery": "medium", "Pickpocketing": "medium" },
                  "Category": "aventura",
                  "ImageUrl": "https://upload.wikimedia.org/wikipedia/commons/thumb/3/38/Lisbon_Oceanarium_%2834535938846%29.jpg/1280px-Lisbon_Oceanarium_%2834535938846%29.jpg",
                  "IsInRoute": false,
                  "PlaceQuery": "Oceanário de Lisboa, Lisboa",
                  "PriceRange": "R$ 30-50",
                  "TimeNeeded": "2-3 horas",
                  "ImageSource": "wikipedia",
                  "HowToGetThere": "Metro até Oriente.",
                  "OneLineSummary": "Um dos maiores aquários da Europa, com tanque central impressionante e diversas espécies marinhas.",
                  "OpeningHoursNote": "Aberto todos os dias das 10h às 19h.",
                  "NeighborhoodOrArea": "Parque das Nações"
                },
                {
                  "Id": "fe0558f8-b9f4-4e72-ac92-f9e0b807f8c5",
                  "Name": "Sintra - Palácio da Pena",
                  "Lat": 38.787590,
                  "Lng": -9.390474,
                  "Tips": ["Use calçado confortável para explorar os jardins.", "Visite também o Castelo dos Mouros perto dali."],
                  "WhyGo": "Para explorar um dos palácios mais icônicos de Portugal, com arquitetura romântica e história fascinante.",
                  "Safety": { "Notes": "", "Scams": "low", "Overall": "medium", "Robbery": "medium", "Pickpocketing": "medium" },
                  "Category": "cultura",
                  "ImageUrl": "https://upload.wikimedia.org/wikipedia/commons/thumb/3/3e/Sintra_-_Palacio_da_Pena_%2820332995770%29.jpg/1280px-Sintra_-_Palacio_da_Pena_%2820332995770%29.jpg",
                  "IsInRoute": false,
                  "PlaceQuery": "Palácio da Pena, Sintra",
                  "PriceRange": "R$ 15-25",
                  "TimeNeeded": "3-4 horas",
                  "ImageSource": "wikipedia",
                  "HowToGetThere": "Trem até Sintra (40 minutos) e ônibus ou táxi até o palácio.",
                  "OneLineSummary": "Palácio colorido e de contos de fadas, cercado por jardins exuberantes e vistas deslumbrantes.",
                  "OpeningHoursNote": "Aberto todos os dias das 9h30 às 18h (fechado às segundas-feiras).",
                  "NeighborhoodOrArea": "Sintra (perto de Lisboa)"
                },
                {
                  "Id": "95b5d8c9-a1bd-44c6-a72b-5ef2e0db8cd7",
                  "Name": "Convento do Carmo",
                  "Lat": 38.712139,
                  "Lng": -9.140246,
                  "Tips": ["Explore o centro histórico próximo, como a Praça do Comércio.", "Visite o museu arqueológico no interior."],
                  "WhyGo": "Para ver de perto a história do terremoto de 1755 e admirar a arquitetura gótica preservada.",
                  "Safety": { "Notes": "", "Scams": "low", "Overall": "medium", "Robbery": "medium", "Pickpocketing": "medium" },
                  "Category": "cultura",
                  "ImageUrl": "https://upload.wikimedia.org/wikipedia/commons/thumb/6/6b/Portuguese_National_Republican_Guard_%28GNR%29_headquarters%2C_Lisbon%2C_Portugal.jpg/1280px-Portuguese_National_Republican_Guard_%28GNR%29_headquarters%2C_Lisbon%2C_Portugal.jpg",
                  "IsInRoute": false,
                  "PlaceQuery": "Convento do Carmo, Lisboa",
                  "PriceRange": "R$ 5-10",
                  "TimeNeeded": "1-2 horas",
                  "ImageSource": "wikipedia",
                  "HowToGetThere": "Metro até Baixa-Chiado.",
                  "OneLineSummary": "Igreja gótica com fachada parcialmente destruída, revelando detalhes arquitetônicos únicos.",
                  "OpeningHoursNote": "Aberto todos os dias das 10h às 18h.",
                  "NeighborhoodOrArea": "Chiado"
                },
                {
                  "Id": "3f451eb7-dc65-4bf0-adc5-7960a573a661",
                  "Name": "Praça do Comércio",
                  "Lat": 38.707779,
                  "Lng": -9.136744,
                  "Tips": ["Experimente um café num dos cafés históricos.", "Explore a Baixa e o Chiado próximos."],
                  "WhyGo": "Para apreciar a arquitetura imperial e sentir a atmosfera central de Lisboa, com vistas para o rio.",
                  "Safety": { "Notes": "", "Scams": "low", "Overall": "medium", "Robbery": "medium", "Pickpocketing": "medium" },
                  "Category": "cultura",
                  "ImageUrl": "https://upload.wikimedia.org/wikipedia/commons/thumb/0/0a/Arco_Triunfal_da_Rua_Augusta%2C_Plaza_del_Comercio%2C_Lisboa%2C_Portugal%2C_2012-05-12%2C_DD_02.JPG/1280px-Arco_Triunfal_da_Rua_Augusta%2C_Plaza_del_Comercio%2C_Lisboa%2C_Portugal%2C_2012-05-12%2C_DD_02.JPG",
                  "IsInRoute": false,
                  "PlaceQuery": "Praça do Comércio, Lisboa",
                  "PriceRange": "R$ 0-20 (cafés e lojas)",
                  "TimeNeeded": "1-2 horas",
                  "ImageSource": "wikipedia",
                  "HowToGetThere": "Metro até Terreiro do Paço.",
                  "OneLineSummary": "Praça histórica com vista para o rio Tejo, ladeada por edifícios neoclássicos e cafés.",
                  "OpeningHoursNote": "Aberto 24 horas; cafés e lojas têm horários variados.",
                  "NeighborhoodOrArea": "Baixa"
                },
                {
                  "Id": "3b2fbbf9-7782-489d-a6b2-2c7c470dc6a9",
                  "Name": "Miradouro de Santa Catarina",
                  "Lat": 38.709547,
                  "Lng": -9.147644,
                  "Tips": ["Chegue cedo para garantir um bom lugar.", "Combine com um passeio de bonde."],
                  "WhyGo": "Para apreciar a vista icônica da ponte e do rio, especialmente ao pôr do sol.",
                  "Safety": { "Notes": "", "Scams": "low", "Overall": "medium", "Robbery": "medium", "Pickpocketing": "medium" },
                  "Category": "cultura",
                  "ImageUrl": "https://upload.wikimedia.org/wikipedia/commons/thumb/1/14/Miradouro_de_Santa_Catarina_-_Lisboa_-_Portugal_%2849396454011%29.jpg/1280px-Miradouro_de_Santa_Catarina_-_Lisboa_-_Portugal_%2849396454011%29.jpg",
                  "IsInRoute": false,
                  "PlaceQuery": "Miradouro de Santa Catarina, Lisboa",
                  "PriceRange": "R$ 0",
                  "TimeNeeded": "30 minutos a 1 hora",
                  "ImageSource": "wikipedia",
                  "HowToGetThere": "Bonde 25E ou caminhada desde o Cais do Sodré.",
                  "OneLineSummary": "Mirante com vista para o rio Tejo e a Ponte 25 de Abril, ideal para fotos e relaxamento.",
                  "OpeningHoursNote": "Aberto 24 horas.",
                  "NeighborhoodOrArea": "Alcântara"
                },
                {
                  "Id": "e75c01bf-086a-4db3-98a6-f6521eda2db1",
                  "Name": "Catedral de Lisboa (Sé de Lisboa)",
                  "Lat": 38.709881,
                  "Lng": -9.132584,
                  "Tips": ["Visite o museu no interior para mais contexto histórico.", "Explore as ruas de Alfama próximas."],
                  "WhyGo": "Para explorar um dos monumentos mais antigos de Lisboa e admirar sua arquitetura e arte sacra.",
                  "Safety": { "Notes": "", "Scams": "low", "Overall": "medium", "Robbery": "medium", "Pickpocketing": "medium" },
                  "Category": "cultura",
                  "ImageUrl": "https://upload.wikimedia.org/wikipedia/commons/thumb/b/b7/Lisboa_May_2013-1.jpg/1280px-Lisboa_May_2013-1.jpg",
                  "IsInRoute": false,
                  "PlaceQuery": "Catedral de Lisboa, Lisboa",
                  "PriceRange": "R$ 5-10",
                  "TimeNeeded": "1-2 horas",
                  "ImageSource": "wikipedia",
                  "HowToGetThere": "Metro até Santa Apolónia ou bonde 28.",
                  "OneLineSummary": "Catedral românica com história secular e interior impressionante.",
                  "OpeningHoursNote": "Aberto todos os dias das 9h às 19h.",
                  "NeighborhoodOrArea": "Alfama"
                },
                {
                  "Id": "4154f26c-2c28-4e51-8a5e-42975a6e932e",
                  "Name": "LX Factory",
                  "Lat": 38.7034979,
                  "Lng": -9.1788730,
                  "Tips": ["Experimente os cafés e restaurantes temáticos.", "Visite a livraria Ler Devagar para livros raros."],
                  "WhyGo": "Para explorar a cena cultural alternativa de Lisboa, com opções gastronômicas e compras únicas.",
                  "Safety": { "Notes": "", "Scams": "low", "Overall": "medium", "Robbery": "medium", "Pickpocketing": "medium" },
                  "Category": "gastronomia",
                  "ImageUrl": "https://upload.wikimedia.org/wikipedia/commons/thumb/5/54/LX_Factory_%2822379332901%29.jpg/1280px-LX_Factory_%2822379332901%29.jpg",
                  "IsInRoute": false,
                  "PlaceQuery": "LX Factory, Lisboa",
                  "PriceRange": "R$ 20-60 (refeições e compras)",
                  "TimeNeeded": "2-3 horas",
                  "ImageSource": "wikipedia",
                  "HowToGetThere": "Trem até Alcântara-Mar ou Uber.",
                  "OneLineSummary": "Espaço criativo com lojas, restaurantes e arte em um antigo complexo industrial.",
                  "OpeningHoursNote": "Aberto todos os dias das 12h às 20h.",
                  "NeighborhoodOrArea": "Alcântara"
                },
                {
                  "Id": "b56de5d3-14b2-4472-8ac0-dc9416537d15",
                  "Name": "Elevador de Santa Justa",
                  "Lat": 38.7121301,
                  "Lng": -9.1394297,
                  "Tips": ["Chegue cedo para evitar filas.", "Explore o bairro alto próximo."],
                  "WhyGo": "Para subir ao topo e ter vistas deslumbrantes da cidade, especialmente ao pôr do sol.",
                  "Safety": { "Notes": "", "Scams": "low", "Overall": "medium", "Robbery": "medium", "Pickpocketing": "medium" },
                  "Category": "cultura",
                  "ImageUrl": "https://upload.wikimedia.org/wikipedia/commons/b/b4/Lisbon_locator_map.png",
                  "IsInRoute": false,
                  "PlaceQuery": "Elevador de Santa Justa, Lisboa",
                  "PriceRange": "R$ 5-10 (ida e volta)",
                  "TimeNeeded": "30 minutos",
                  "ImageSource": "wikipedia",
                  "HowToGetThere": "Metro até Baixa-Chiado.",
                  "OneLineSummary": "Elevador histórico que liga a Baixa ao bairro alto de Carmo, com vista panorâmica.",
                  "OpeningHoursNote": "Aberto todos os dias das 7h30 às 23h.",
                  "NeighborhoodOrArea": "Baixa/Carmo"
                },
                {
                  "Id": "2286a851-bea6-4aef-9c6f-b058b9dc7737",
                  "Name": "Praia de Carcavelos",
                  "Lat": 38.679236,
                  "Lng": -9.334808,
                  "Tips": ["Leve protetor solar e água para dias quentes.", "Experimente os restaurantes de frutos do mar próximos."],
                  "WhyGo": "Para um dia de sol e mar, com opções de surf e restaurantes à beira-mar.",
                  "Safety": { "Notes": "", "Scams": "low", "Overall": "medium", "Robbery": "medium", "Pickpocketing": "medium" },
                  "Category": "praias",
                  "ImageUrl": "https://upload.wikimedia.org/wikipedia/commons/thumb/4/49/Summer_in_October_%2837404902536%29.jpg/1280px-Summer_in_October_%2837404902536%29.jpg",
                  "IsInRoute": false,
                  "PlaceQuery": "Praia de Carcavelos, Cascais",
                  "PriceRange": "R$ 0-20 (aluguel de guarda-sol e cadeiras)",
                  "TimeNeeded": "3-4 horas",
                  "ImageSource": "wikipedia",
                  "HowToGetThere": "Trem até Cascais (40 minutos) e ônibus ou táxi até a praia.",
                  "OneLineSummary": "Praia de areia larga e ondas boas para surf, perto de Lisboa.",
                  "OpeningHoursNote": "Aberto 24 horas.",
                  "NeighborhoodOrArea": "Cascais (perto de Lisboa)"
                },
                {
                  "Id": "3a5456f6-a0b5-4982-9888-52f592c70a1c",
                  "Name": "Museu do Fado",
                  "Lat": 38.711124,
                  "Lng": -9.127608,
                  "Tips": ["Assista a um show de fado à noite em um bar próximo.", "Explore as ruas de Alfama após a visita."],
                  "WhyGo": "Para entender a história do fado e assistir a apresentações autênticas no bairro de Alfama.",
                  "Safety": { "Notes": "", "Scams": "low", "Overall": "medium", "Robbery": "medium", "Pickpocketing": "medium" },
                  "Category": "cultura",
                  "ImageUrl": "https://upload.wikimedia.org/wikipedia/commons/d/d0/Museu_do_Fado_Lisbon_Portugal_01.png",
                  "IsInRoute": false,
                  "PlaceQuery": "Museu do Fado, Lisboa",
                  "PriceRange": "R$ 10-15",
                  "TimeNeeded": "1-2 horas",
                  "ImageSource": "wikipedia",
                  "HowToGetThere": "Metro até Santa Apolónia ou bonde 28.",
                  "OneLineSummary": "Museu dedicado ao fado, com exposições interativas e performances ao vivo.",
                  "OpeningHoursNote": "Aberto todos os dias das 10h às 18h.",
                  "NeighborhoodOrArea": "Alfama"
                }
              ],
              "SafetyIndex": {
                "Notes": "Cuidado com distrações em locais movimentados como Alfama e Baixa. Use bolsas com fecho e evite exibir objetos de valor.",
                "Scams": "medium",
                "Overall": "medium",
                "Robbery": "low",
                "SourceNote": "Varia por bairro e horário; confira fontes oficiais e notícias locais.",
                "Pickpocketing": "high"
              },
              "SafetyNotes": "Lisboa é geralmente segura, mas cuidado com carteiristas em áreas turísticas e no transporte público. Evite áreas isoladas à noite.",
              "BestTimeToVisit": "Primavera (março a maio) e outono (setembro a novembro) para clima ameno e menos turistas. Verão pode ser quente e lotado.",
              "LocalTransportTips": "Use o transporte público (metro, elétricos e autocarro) com cartão Viva Viagem. Taxis e Uber são acessíveis. Caminhar é ótimo para explorar bairros históricos."
            }
            
            """;


            using var mockDoc = JsonDocument.Parse(mockJson);

            var responseMock = JsonSerializer.Deserialize<TourismSummaryResponse>(
                mockDoc.RootElement.GetRawText(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            ) ?? new TourismSummaryResponse();

            // garante ids
            if (responseMock.Spots is not null)
            {
                foreach (var spot in responseMock.Spots)
                {
                    if (spot.Id == Guid.Empty)
                        spot.Id = Guid.NewGuid();
                }
            }

            // ✅ nomes diferentes para não conflitar com o fluxo normal
            var mockRouteDoc = JsonSerializer.SerializeToDocument(responseMock);
            var mockFixedDoc = EnsureSpotIdAndInRoute(mockRouteDoc);

            await _userTripPlansContext.UpsertRouteAsync(
                userId: request.UserId,
                city: responseMock.City ?? request.City,
                country: responseMock.Country ?? request.Country,
                route: mockFixedDoc,
                cancellationToken: cancellationToken);

            return Result<TourismSummaryResponse>.Success(responseMock);
        }

        // ===== fluxo normal (IA) =====
        var response = await _service.GetSummaryWithImagesAsync(request, cancellationToken);

        if (response?.Spots is not null)
        {
            foreach (var spot in response.Spots)
            {
                if (spot.Id == Guid.Empty)
                    spot.Id = Guid.NewGuid();
            }
        }

        var routeDoc = JsonSerializer.SerializeToDocument(response);
        var fixedDoc = EnsureSpotIdAndInRoute(routeDoc);

        await _userTripPlansContext.UpsertRouteAsync(
            userId: request.UserId,
            city: response.City ?? request.City,
            country: response.Country ?? request.Country,
            route: fixedDoc,
            cancellationToken: cancellationToken);

        return Result<TourismSummaryResponse>.Success(response);
    }


    private static JsonDocument EnsureSpotIdAndInRoute(JsonDocument doc)
    {
        var root = JsonNode.Parse(doc.RootElement.GetRawText()) as JsonObject;
        if (root is null) return doc;

        // spots pode ser "spots" ou "Spots"
        var spotsNode = root["spots"] ?? root["Spots"];
        if (spotsNode is not JsonArray spotsArray) return doc;

        foreach (var item in spotsArray)
        {
            if (item is not JsonObject spotObj) continue;

            // Id / id
            var idNode = spotObj["id"] ?? spotObj["Id"];
            if (idNode is null || !Guid.TryParse(idNode.ToString(), out _))
            {
                // mantém o padrão do JSON que já veio (camelCase)
                if (spotObj.ContainsKey("id")) spotObj["id"] = Guid.NewGuid().ToString();
                else spotObj["Id"] = Guid.NewGuid().ToString();
            }

            // IsInRoute / isInRoute
            var inRouteNode = spotObj["isInRoute"] ?? spotObj["IsInRoute"];
            if (inRouteNode is null)
            {
                if (spotObj.ContainsKey("isInRoute")) spotObj["isInRoute"] = false;
                else spotObj["IsInRoute"] = false;
            }
        }

        return JsonDocument.Parse(root.ToJsonString());
    }

    protected override async Task<ValidationResult> ValidateAsync(TourismSummaryRequest request, CancellationToken cancellationToken)
    {
        var validator = new TourismRequestValidator();
        var result = await validator.ValidateAsync(request, cancellationToken);
        if (!result.IsValid)
        {
            var errors = result.Errors
            .Select(e => ErrorInfo.Create(e.ErrorMessage, e.PropertyName.ToUpperInvariant()))
            .ToArray(); 
            return ValidationResult.Failure(errors);
        }

        return ValidationResult.Success;
    }


    public class TourismRequestValidator : AbstractValidator<TourismSummaryRequest>
    {
        public TourismRequestValidator()
        {
            RuleFor(x => x.Country)
                .NotEmpty()
                .WithMessage("O país é obrigatório.")
                .MaximumLength(100);

            RuleFor(x => x.City)
                .NotEmpty()
                .WithMessage("A cidade é obrigatória.")
                .MaximumLength(100);


            RuleFor(x => x.Language)
                .NotEmpty()
                .WithMessage("O idioma é obrigatório.")
                .Matches(@"^[a-z]{2}(-[A-Z]{2})?$")
                .WithMessage("Formato de idioma inválido. Ex: pt-BR, en-US.");

            // Opcionais, mas com regras se vierem preenchidos
            RuleFor(x => x.TravelerProfile)
                .MaximumLength(50)
                .When(x => !string.IsNullOrWhiteSpace(x.TravelerProfile));

            RuleFor(x => x.Budget)
                .MaximumLength(50)
                .When(x => !string.IsNullOrWhiteSpace(x.Budget));
        }
    }

}
