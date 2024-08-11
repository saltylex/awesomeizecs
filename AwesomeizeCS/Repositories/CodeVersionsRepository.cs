using AwesomeizeCS.Data;
using AwesomeizeCS.Domain;
using AwesomeizeCS.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AwesomeizeCS.Repositories
{
    public class CodeVersionsRepository : ICodeVersionsRepository
    {
        private readonly ApplicationDbContext _context;

        public CodeVersionsRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<CodeVersion>> GetAllCodeVersionsAsync()
        {
            return await _context.CodeVersion.ToListAsync();
        }

        public async Task<CodeVersion> GetCodeVersionByIdAsync(Guid id)
        {
            return await _context.CodeVersion.FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task CreateCodeVersionAsync(CodeVersion codeVersion)
        {
            codeVersion.Id = Guid.NewGuid();
            _context.Add(codeVersion);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCodeVersionAsync(CodeVersion codeVersion)
        {
            _context.Update(codeVersion);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCodeVersionAsync(Guid id)
        {
            var codeVersion = await _context.CodeVersion.FindAsync(id);
            if (codeVersion != null)
            {
                _context.CodeVersion.Remove(codeVersion);
                await _context.SaveChangesAsync();
            }
        }

        public bool CodeVersionExists(Guid id)
        {
            return _context.CodeVersion.Any(e => e.Id == id);
        }
    }
}
