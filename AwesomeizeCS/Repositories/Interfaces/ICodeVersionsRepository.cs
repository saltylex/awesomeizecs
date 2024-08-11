using AwesomeizeCS.Domain;

namespace AwesomeizeCS.Repositories.Interfaces
{
    public interface ICodeVersionsRepository
    {
        Task<List<CodeVersion>> GetAllCodeVersionsAsync();
        Task<CodeVersion> GetCodeVersionByIdAsync(Guid id);
        Task CreateCodeVersionAsync(CodeVersion codeVersion);
        Task UpdateCodeVersionAsync(CodeVersion codeVersion);
        Task DeleteCodeVersionAsync(Guid id);
        bool CodeVersionExists(Guid id);
    }
}
