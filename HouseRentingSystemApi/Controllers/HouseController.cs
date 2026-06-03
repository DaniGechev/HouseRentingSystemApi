using HouseRentingSystemApi.Constants;
using HouseRentingSystemApi.Models;
using HouseRentingSystemApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HouseRentingSystemApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HouseController : ControllerBase
    {
        private readonly IHouseService houseService;

        public HouseController(IHouseService houseService)
        {
            this.houseService = houseService;
        }

        [HttpGet("All")]
        [Produces(typeof(IEnumerable<HouseDetailModel>))]
        public async Task<IActionResult> GetAll()
        {
            var model = await houseService.GetAllAsync();
            return Ok(model);
        }

        [HttpGet("me")]
        public IActionResult Me()
        {
            var isAuthenticated = User.Identity?.IsAuthenticated;
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Ok(new { isAuthenticated, userId });
        }

        [HttpGet("{id}")]
        [Produces(typeof(HouseDetailModel))]
        public async Task<IActionResult> GetById(int id)
        {
            var house = await houseService.GetByIdAsync(id);

            if (house == null)
            {
                return NotFound();
            }

            return Ok(house);
        }

        [Authorize(Roles = RoleConstants.Agent)]
        [HttpPost("All")]
        [Produces(typeof(HouseDetailModel))]
        public async Task<IActionResult> Create([FromBody] HouseDetailModel model)
        {
            if (ModelState.IsValid == false)
            {
                return BadRequest();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var created = await houseService.CreateAsync(model, userId!);

            return Created("api/House/All", created);
        }

        [Authorize(Roles = RoleConstants.Agent)]
        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(int id, [FromBody] HouseEditModel model)
        {
            if (ModelState.IsValid == false)
            {
                return BadRequest();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var status = await houseService.EditAsync(id, model, userId!);

            return status switch
            {
                OperationStatus.NotFound => NotFound(),
                OperationStatus.Forbidden => Forbid(),
                _ => NoContent()
            };
        }

        [Authorize(Roles = RoleConstants.Agent)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var status = await houseService.DeleteAsync(id, userId!);

            return status switch
            {
                OperationStatus.NotFound => NotFound(),
                OperationStatus.Forbidden => Forbid(),
                _ => NoContent()
            };
        }
    }
}
