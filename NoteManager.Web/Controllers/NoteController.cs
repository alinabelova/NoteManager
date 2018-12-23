using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NoteManager.DataAccess.Repository;
using NoteManager.Models;
using NoteManager.Web.Contracts;

namespace NoteManager.Web.Controllers
{
    [Produces("application/json")]
    [Route("api/Note")]
    [Authorize]
    public class NoteController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRepository<Note> _noteRepository;

        public NoteController(UserManager<ApplicationUser> userManager,
            IRepository<Note> noteRepository)
        {
            _userManager = userManager;
            _noteRepository = noteRepository;
        }

        /// <summary>
        /// Get all notes for current user
        /// </summary>
        /// <param name="contract">PaginationContract</param>
        /// <returns>List of ordered notes for current page</returns>
        [Route("Get")]
        [HttpPost]
        public async Task<IActionResult> Get([FromBody]PaginationContract contract)
        {
            try
            {
                var usr = await _userManager.FindByEmailAsync(HttpContext.User.Identity.Name);
                var pagination = contract.Pagination;
                if (usr == null)
                {
                    return BadRequest("User not found");
                }
                var notes = _noteRepository
                    .GetWithInclude(n => n.UserId == usr.Id);
                if (notes == null)
                {
                    return null;
                }
                switch (pagination.SortBy)
                {
                    case "name":
                        notes = pagination.Descending
                            ? notes.OrderByDescending(n => n.Name)
                            : notes.OrderBy(n => n.Name);
                        break;
                    case "createdAt":
                        notes = pagination.Descending
                            ? notes.OrderByDescending(n => n.CreatedAt)
                            : notes.OrderBy(n => n.CreatedAt);
                        break;
                    case "modifiedAt":
                        notes = pagination.Descending
                            ? notes.OrderByDescending(n => n.ModifiedAt)
                            : notes.OrderBy(n => n.ModifiedAt);
                        break;
                    case "position":
                        notes = pagination.Descending
                            ? notes.OrderByDescending(n => n.Position)
                            : notes.OrderBy(n => n.Position);
                        break;
                    default:
                        notes = pagination.Descending
                            ? notes.OrderByDescending(n => n.Position)
                            : notes.OrderBy(n => n.Position);
                        break;
                }

                if (pagination.RowsPerPage > 0)
                    notes = notes.Skip((pagination.Page - 1) * pagination.RowsPerPage).Take(pagination.RowsPerPage);
                notes = notes.ToList();

                return Ok(notes);
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }
        /// <summary>
        /// Get total count for current user
        /// </summary>
        /// <returns>Total count</returns>
        [Route("GetTotalElements")]
        [HttpPost]
        public async Task<IActionResult> GetTotalElements()
        {
            try
            {
                var usr = await _userManager.FindByEmailAsync(HttpContext.User.Identity.Name);
                int count = await _noteRepository.TotalElementsAsync(n => n.UserId == usr.Id);
                return Ok(count);
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }

        /// <summary>
        /// Create new note for current user
        /// </summary>
        /// <param name="contract">NoteContract</param>
        /// <returns></returns>
        [Route("Create")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody]NoteContract contract)
        {
            try
            {
                var note = contract.Note;
                var curentUser = await _userManager.FindByEmailAsync(HttpContext.User.Identity.Name);
                note.UserId = curentUser.Id;
                await _noteRepository.CreateAsync(note);
                return Ok(note);
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }

        /// <summary>
        /// Update note
        /// </summary>
        /// <param name="contract">NoteContract</param>
        /// <returns></returns>
        [Route("Update")]
        [HttpPost]
        public async Task<IActionResult> Update([FromBody]NoteContract contract)
        {
            try
            {
                var note = contract.Note;
                var itemForUpdate = await _noteRepository.FindByIdAsync(note.Id);
                if (itemForUpdate == null)
                    return BadRequest("Item not found");
                var curentUser = await _userManager.FindByEmailAsync(HttpContext.User.Identity.Name);
                itemForUpdate.UserId = curentUser.Id;
                itemForUpdate.Name = note.Name;
                itemForUpdate.Description = note.Description;
                itemForUpdate.ModifiedAt = DateTime.Now;
                itemForUpdate.Position = note.Position;

                await _noteRepository.UpdateAsync(itemForUpdate);
                return Ok(itemForUpdate);
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }

        /// <summary>
        /// Remove note
        /// </summary>
        /// <param name="id">note Id</param>
        /// <returns></returns>
        [Authorize(Roles = "admin")]
        [Route("Remove")]
        [HttpPost]
        public async Task<IActionResult> Remove(int id)
        {
            try
            {
                var itemForRemove = await _noteRepository.FindByIdAsync(id);
                if (itemForRemove == null)
                    return BadRequest("Item not found");
                await _noteRepository.RemoveAsync(itemForRemove);
                return Ok();
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }
    }
}