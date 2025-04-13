using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("api/[Controller]")]
    [Authorize]
    public class Environment2DController : ControllerBase
    {
        private readonly IEnvironmentRepository _environmentRepository;

        public Environment2DController(IEnvironmentRepository repository)
        {
            _environmentRepository = repository;
        }

        [HttpGet("userworlds")]
        public async Task<ActionResult<IEnumerable<Environment2D>>> GetAll([FromQuery]string UserName)
        {
            var environments = await _environmentRepository.GetAll(UserName);
            if (!environments.Any())
            {
                return NoContent();
            }
            return Ok(environments);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Environment2D>> GetById(int id)
        {
            var environment = await _environmentRepository.GetById(id);
            if (environment == null)
            {
                return NotFound();
            }
            return Ok(environment);
        }


        [HttpPost]
        public async Task<ActionResult<Environment2D>> Add(Environment2D environment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdEnvironment = await _environmentRepository.Add(environment);
            return CreatedAtAction(nameof(GetById), new { id = createdEnvironment.Id }, createdEnvironment);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var result = await _environmentRepository.Delete(id);
            if (!result)
            {
                return NotFound();
            }

            return Ok();
        }




       
    }


