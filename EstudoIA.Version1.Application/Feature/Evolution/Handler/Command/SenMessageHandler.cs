using EstudoIA.Version1.Application.Abstractions.Handlers;
using EstudoIA.Version1.Application.Feature.Twilio.Models;
using EstudoIA.Version1.Application.Shared.HttpClients.EvolutionApi;
using EstudosIA.Version1.ApplicationCommon.Results;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;

namespace EstudoIA.Version1.Application.Feature.Twilio.Handler.Command
{
    public class SenMessageHandler : HandlerBase<SendBulkMessageRequest, SendBulkMessageResponse>
    {
        private readonly IWhatsAppEvolutionService _service;

        public SenMessageHandler(IWhatsAppEvolutionService twilioWhatsAppService)
        {
            _service = twilioWhatsAppService;
        }

        protected override async Task<Result<SendBulkMessageResponse>> ExecuteAsync(
            SendBulkMessageRequest request,
            CancellationToken cancellationToken)
        {
            ExcelPackage.License.SetNonCommercialPersonal("EstudoIA");

            var contacts = await ReadExcelAsync(request.ExcelFile, cancellationToken);
            var results = new List<SendBulkMessageItemResult>();

            foreach (var contact in contacts)
            {
                if (string.IsNullOrWhiteSpace(contact.Celular))
                {
                    results.Add(new SendBulkMessageItemResult
                    {
                        Nome = contact.Nome,
                        Celular = contact.Celular,
                        Success = false,
                        Error = "Celular não informado."
                    });

                    continue;
                }

                var finalMessage = ApplyTemplate(request.MessageTemplate, contact);

                try
                {
                    var serviceResult = await _service.SendTextMessageAsync(
                        contact.Celular,
                        finalMessage,
                        request.InstanceName,
                        cancellationToken);

                    results.Add(new SendBulkMessageItemResult
                    {
                        Nome = contact.Nome,
                        Celular = contact.Celular,
                        Success = serviceResult.Success,
                        MessageId = serviceResult.ProviderMessageId,
                        Error = serviceResult.Error
                    });
                }
                catch (Exception ex)
                {
                    results.Add(new SendBulkMessageItemResult
                    {
                        Nome = contact.Nome,
                        Celular = contact.Celular,
                        Success = false,
                        Error = ex.Message
                    });
                }
            }

            var response = new SendBulkMessageResponse
            {
                Success = results.All(x => x.Success),
                TotalRows = contacts.Count,
                SentCount = results.Count(x => x.Success),
                FailedCount = results.Count(x => !x.Success),
                Results = results
            };

            return Result<SendBulkMessageResponse>.Success(response);
        }

        protected override async Task<ValidationResult> ValidateAsync(
            SendBulkMessageRequest request,
            CancellationToken cancellationToken)
        {
            var validator = new SendBulkMessageRequestValidator();
            var fluentResult = await validator.ValidateAsync(request, cancellationToken);

            var errors = new List<ErrorInfo>();

            if (!fluentResult.IsValid)
            {
                errors.AddRange(
                    fluentResult.Errors.Select(e =>
                        ErrorInfo.Create(e.ErrorMessage, e.PropertyName.ToUpperInvariant())));
            }

            if (request.ExcelFile is not null)
            {
                try
                {
                    ExcelPackage.License.SetNonCommercialPersonal("EstudoIA");

                    using var stream = new MemoryStream();
                    await request.ExcelFile.CopyToAsync(stream, cancellationToken);
                    stream.Position = 0;

                    using var package = new ExcelPackage(stream);
                    var worksheet = package.Workbook.Worksheets.FirstOrDefault();

                    if (worksheet is null)
                    {
                        errors.Add(ErrorInfo.Create("A planilha não possui nenhuma aba.", "EXCELFILE"));
                    }
                    else
                    {
                        var headers = GetHeaderMap(worksheet);

                        var hasNome = headers.ContainsKey("nome");
                        var hasCelular = headers.ContainsKey("celular") || headers.ContainsKey("telefone");

                        if (!hasNome)
                            errors.Add(ErrorInfo.Create("A planilha deve conter a coluna 'Nome'.", "EXCELFILE"));

                        if (!hasCelular)
                            errors.Add(ErrorInfo.Create("A planilha deve conter a coluna 'Celular' ou 'Telefone'.", "EXCELFILE"));
                    }
                }
                catch
                {
                    errors.Add(ErrorInfo.Create("Arquivo Excel inválido ou não foi possível ler a planilha.", "EXCELFILE"));
                }
            }

            return errors.Count > 0
                ? ValidationResult.Failure(errors.ToArray())
                : ValidationResult.Success;
        }

        private static async Task<List<ExcelContactRow>> ReadExcelAsync(
            IFormFile excelFile,
            CancellationToken cancellationToken)
        {
            using var stream = new MemoryStream();
            await excelFile.CopyToAsync(stream, cancellationToken);
            stream.Position = 0;

            using var package = new ExcelPackage(stream);
            var worksheet = package.Workbook.Worksheets.FirstOrDefault();

            if (worksheet is null)
                return new List<ExcelContactRow>();

            var headers = GetHeaderMap(worksheet);
            var lastRow = worksheet.Dimension?.Rows ?? 0;

            headers.TryGetValue("nome", out var nomeCol);
            headers.TryGetValue("sobrenome", out var sobrenomeCol);
            headers.TryGetValue("email", out var emailCol);

            var celularCol = 0;
            if (!headers.TryGetValue("celular", out celularCol))
                headers.TryGetValue("telefone", out celularCol);

            var rows = new List<ExcelContactRow>();

            for (int row = 2; row <= lastRow; row++)
            {
                var nome = GetCellValue(worksheet, row, nomeCol);
                var sobrenome = GetCellValue(worksheet, row, sobrenomeCol);
                var email = GetCellValue(worksheet, row, emailCol);
                var celular = GetCellValue(worksheet, row, celularCol);

                if (string.IsNullOrWhiteSpace(nome) &&
                    string.IsNullOrWhiteSpace(sobrenome) &&
                    string.IsNullOrWhiteSpace(email) &&
                    string.IsNullOrWhiteSpace(celular))
                {
                    continue;
                }

                rows.Add(new ExcelContactRow
                {
                    Nome = nome,
                    Sobrenome = sobrenome,
                    Email = email,
                    Celular = NormalizePhone(celular)
                });
            }

            return rows;
        }

        private static Dictionary<string, int> GetHeaderMap(ExcelWorksheet worksheet)
        {
            var map = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            var lastColumn = worksheet.Dimension?.Columns ?? 0;

            for (int col = 1; col <= lastColumn; col++)
            {
                var rawHeader = worksheet.Cells[1, col].Text?.Trim();

                if (string.IsNullOrWhiteSpace(rawHeader))
                    continue;

                var normalized = NormalizeHeader(rawHeader);

                if (!map.ContainsKey(normalized))
                    map[normalized] = col;
            }

            return map;
        }

        private static string NormalizeHeader(string header)
        {
            return header
                .Trim()
                .ToLowerInvariant()
                .Replace(" ", "")
                .Replace("_", "")
                .Replace("-", "");
        }

        private static string? GetCellValue(ExcelWorksheet worksheet, int row, int col)
        {
            if (col <= 0)
                return null;

            return worksheet.Cells[row, col].Text?.Trim();
        }

        private static string ApplyTemplate(string template, ExcelContactRow contact)
        {
            if (string.IsNullOrWhiteSpace(template))
                return string.Empty;

            var nome = contact.Nome?.Trim() ?? string.Empty;
            var sobrenome = contact.Sobrenome?.Trim() ?? string.Empty;
            var email = contact.Email?.Trim() ?? string.Empty;
            var celular = contact.Celular?.Trim() ?? string.Empty;
            var nomeCompleto = $"{nome} {sobrenome}".Trim();

            return template
                .Replace("{{nome}}", nome, StringComparison.OrdinalIgnoreCase)
                .Replace("{{sobrenome}}", sobrenome, StringComparison.OrdinalIgnoreCase)
                .Replace("{{nome_completo}}", nomeCompleto, StringComparison.OrdinalIgnoreCase)
                .Replace("{{email}}", email, StringComparison.OrdinalIgnoreCase)
                .Replace("{{telefone}}", celular, StringComparison.OrdinalIgnoreCase);
        }

        private static string NormalizePhone(string? phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return string.Empty;

            var digits = new string(phone.Where(char.IsDigit).ToArray());

            if (string.IsNullOrWhiteSpace(digits))
                return string.Empty;

            if (!digits.StartsWith("55"))
                digits = $"55{digits}";

            return digits;
        }

        public class SendBulkMessageRequestValidator : AbstractValidator<SendBulkMessageRequest>
        {
            public SendBulkMessageRequestValidator()
            {
                RuleFor(x => x.ExcelFile)
                    .NotNull()
                    .WithMessage("O arquivo Excel é obrigatório.");

                RuleFor(x => x.MessageTemplate)
                    .NotEmpty()
                    .WithMessage("A mensagem é obrigatória.");

                RuleFor(x => x.ExcelFile.FileName)
                    .Must(fileName =>
                        !string.IsNullOrWhiteSpace(fileName) &&
                        (fileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase) ||
                         fileName.EndsWith(".xls", StringComparison.OrdinalIgnoreCase)))
                    .When(x => x.ExcelFile is not null)
                    .WithMessage("O arquivo deve ser um Excel válido (.xlsx ou .xls).");

                RuleFor(x => x.InstanceName)
                    .NotEmpty()
                    .WithMessage("InstanceName é obrigatória");
            }
        }
    }
}