public interface IEnvironmentRepository
{
    Task<List<Environment2D>> GetAll(string userName);
    Task<Environment2D> GetById(int id);
    Task<Environment2D> Add(Environment2D environment);
    Task<Environment2D> Update(Environment2D environment);
    Task<bool> Delete(int id);
}
