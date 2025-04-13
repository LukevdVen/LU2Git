using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ObjectsController : ControllerBase
{
    private readonly IObjectRepository _objectRepository;

    public ObjectsController(IObjectRepository objectRepository)
    {
        _objectRepository = objectRepository;
    }

    // GET api/objects/{environmentId}
    [HttpGet("{environmentId}")]
    public async Task<ActionResult<IEnumerable<ObjectDTO>>> GetObjects(int environmentId)
    {
        var objects = await _objectRepository.GetObjectsByEnvironment(environmentId);
        return Ok(objects);
    }


    // POST api/objects
    [HttpPost]
    public async Task<ActionResult<ObjectDTO>> SaveObjects([FromBody] List<ObjectDTO> objectDTOs)
    {
        if (objectDTOs == null || objectDTOs.Count == 0)
        {
            return BadRequest("No objects provided.");
        }

        var createdObjects = new List<ObjectDTO>();

        foreach (var objectDTO in objectDTOs)
        {
            var createdObject = await _objectRepository.Add(objectDTO);
            createdObjects.Add(createdObject);
        }

        return CreatedAtAction(nameof(GetObjects), new { environmentId = createdObjects[0].EnvironmentId }, createdObjects);
    }

    // DELETE api/objects/{environmentId}
    [HttpDelete("{environmentId}")]
    public async Task<ActionResult> DeleteObjectsByEnvironment(int environmentId)
    {
        var result = await _objectRepository.DeleteObjectsByEnvironment(environmentId);
        if (!result)
        {
            return NotFound();
        }

        return Ok();
    }
}
