using AwesomeizeCS.Domain;

namespace AwesomeizeCS.Services.Interfaces
{
    public interface ICodeVersionsService
    {
        Task<List<CodeVersion>> GetAllCodeVersionsAsync();
        Task<CodeVersion> GetCodeVersionByIdAsync(Guid id);
        Task CreateCodeVersionAsync(CodeVersion codeVersion);
        Task UpdateCodeVersionAsync(CodeVersion codeVersion);
        Task DeleteCodeVersionAsync(Guid id);
        bool CodeVersionExists(Guid id);
    }
}