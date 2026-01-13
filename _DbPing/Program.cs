using Npgsql;

var cs =
  "Host=dpg-d5j9sq7gi27c73eqk210-a.oregon-postgres.render.com;" +
  "Port=5432;" +
  "Database=tourism_bht4;" +
  "Username=tourism_user;" +
  "Password=***;" +
  "Ssl Mode=Require;" +
  "Trust Server Certificate=true;" +
  "SslNegotiation=Direct;" +
  "Timeout=60;" +
  "Command Timeout=60;" +
  "Keepalive=15;" +
  "Tcp Keepalive=true;" +
  "Pooling=false;";

await using var conn = new NpgsqlConnection(cs);
await conn.OpenAsync();
Console.WriteLine("Conectou!");
