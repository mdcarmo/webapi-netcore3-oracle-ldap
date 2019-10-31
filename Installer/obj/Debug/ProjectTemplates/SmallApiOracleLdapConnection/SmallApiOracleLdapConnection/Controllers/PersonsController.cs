using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using $safeprojectname$.Domain.Contracts;
using $safeprojectname$.Domain.Entity;
using $safeprojectname$.Dto;
using System;
using System.Threading.Tasks;

namespace $safeprojectname$.Controllers
{
    /// <summary>
    /// Async Operations 
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class PersonsController : ControllerBase
    {
        private readonly ILogger<PersonsController> _logger;
        private readonly IPersonManager _personManager;
        private readonly IMapper _mapper;

        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="personManager">Manager Person</param>
        /// <param name="mapper">Automapper Dto to Person and Person to DTO</param>
        /// <param name="logger">logger with NLOG</param>
        public PersonsController(IPersonManager personManager, IMapper mapper, ILogger<PersonsController> logger)
        {
            _personManager = personManager;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Get all persons example
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _personManager.GetAllAsync();

            //has exception - log only if has exception
            if (result.HasException)
            {
                _logger.LogError(string.Format("Error: {0}", result.ExceptionMessage));
                return BadRequest(result.ExceptionMessage);
            }

            return Ok(result);
        }

        /// <summary>
        /// Get person by id example
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id:int}")]
        [HttpGet]
        public async Task<IActionResult> Get(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var result = await _personManager.GetByIdAsync(Convert.ToInt32(id));

            //has exception - log only if has exception
            if (result.HasException)
            {
                _logger.LogError(string.Format("Error: {0}", result.ExceptionMessage));
                return BadRequest(result.ExceptionMessage);
            }

            return Ok(result);
        }

        /// <summary>
        /// Get all persons by procedure
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getbyproc")]
        public async Task<IActionResult> GetByProcedure()
        {
            var result = await _personManager.GetAllPersonByProcedure();

            //has exception - log only if has exception
            if (result.HasException)
            {
                _logger.LogError(string.Format("Error: {0}", result.ExceptionMessage));
                return BadRequest(result.ExceptionMessage);
            }

            return Ok(result);
        }

        /// <summary>
        /// Insert a person example
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] PersonDTO dto)
        {
            var person = _mapper.Map<Person>(dto);
            var result = await _personManager.CreateAsync(person);

            //has business rules errors 
            if (result.HasBusinessErrors)
                return Conflict(result);

            //has exception - log only if has exception
            if (result.HasException)
            {
                _logger.LogError(string.Format("Error: {0}", result.ExceptionMessage));
                return BadRequest(result.ExceptionMessage);
            }

            return Ok(result);
        }

        /// <summary>
        /// Update a person example
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        [Route("{id:int}")]
        [HttpPut]
        public async Task<IActionResult> Put(int id, [FromBody] PersonDTO dto)
        {
            var person = _mapper.Map<Person>(dto);
            person.ID = id;

            var result = await _personManager.UpdateAsync(person);

            //has business rules errors 
            if (result.HasBusinessErrors)
                return Conflict(result);

            //has exception - log only if has exception
            if (result.HasException)
            {
                _logger.LogError(string.Format("Error: {0}", result.ExceptionMessage));
                return BadRequest(result.ExceptionMessage);
            }

            //return id person
            return Ok(result);

        }

        /// <summary>
        /// Delete a person example
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id:int}")]
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _personManager.DeleteAsync(id);

            if (result.HasBusinessErrors)
                return Conflict(result);

            if (result.HasException)
            {
                _logger.LogError(string.Format("Error: {0}", result.ExceptionMessage));
                return BadRequest(result.ExceptionMessage);
            }

            return Ok(result);
        }
    }
}