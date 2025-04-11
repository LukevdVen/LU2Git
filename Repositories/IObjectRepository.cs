using System.Collections.Generic;
using System.Threading.Tasks;

public interface IObjectRepository
{
    Task<ObjectDTO> Add(ObjectDTO objectDto); // Changed from CreateObject to Add for consistency
    Task<List<ObjectDTO>> GetObjectsByEnvironment(int environmentId);
    Task<ObjectDTO> Update(ObjectDTO objectDto); // Changed from UpdateObject to Update for consistency
    Task<bool> Delete(int id); // Changed from DeleteObject to Delete for consistency
    Task<bool> DeleteAllObjectsInWorld(int environmentId);
    Task<ObjectDTO> GetObjectById(int id);
    Task<bool> DeleteObjectsByEnvironment(int environmentId);
}
