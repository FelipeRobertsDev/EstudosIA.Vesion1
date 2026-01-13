using Google.Cloud.Firestore;
using System.Text.RegularExpressions;

namespace EstudoIA.Version1.Application.Data.UserTourism;

[FirestoreData]
public class User
{
    // ID do documento (ex: user_001)
    [FirestoreDocumentId]
    public string Id { get; set; }

    [FirestoreProperty("nome")]
    public string Nome { get; set; }

    [FirestoreProperty("email")]
    public string Email { get; set; }

    [FirestoreProperty("ativo")]
    public bool Ativo { get; set; } = true;

    [FirestoreProperty("criadoEm")]
    public Timestamp? CriadoEm { get; set; }


    /// <summary>
    /// Construtor padrão
    /// </summary>
    public User()
    {
        CriadoEm = new Timestamp();
    }

    /// <summary>
    /// Verifica se o email é válido
    /// </summary>
    public bool EmailValido()
    {
        if (string.IsNullOrWhiteSpace(Email))
            return false;

        // Regex simples e segura (suficiente para 99% dos casos)
        var regex = new Regex(
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
            RegexOptions.IgnoreCase | RegexOptions.Compiled
        );

        return regex.IsMatch(Email);
    }
}
