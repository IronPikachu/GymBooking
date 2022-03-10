#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GymBooking.Data;
using GymBooking.Models.Entities;
using Microsoft.AspNetCore.Identity;

namespace GymBooking.Controllers;
public class GymClassesController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public GymClassesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // GET: GymClasses
    public async Task<IActionResult> Index()
    {
        return View(await _context.GymClass.ToListAsync());
    }

    // GET: GymClasses/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var gymClass = await _context.GymClass
            .Include(gc => gc.AttendingMembers)
            .ThenInclude(am => am.ApplicationUser)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (gymClass == null)
        {
            return NotFound();
        }

        return View(gymClass);
    }

    // GET: GymClasses/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: GymClasses/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Name,StartTime,Duration,Description")] GymClass gymClass)
    {
        if (ModelState.IsValid)
        {
            _context.Add(gymClass);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(gymClass);
    }

    // GET: GymClasses/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var gymClass = await _context.GymClass.FindAsync(id);
        if (gymClass == null)
        {
            return NotFound();
        }
        return View(gymClass);
    }

    // POST: GymClasses/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,StartTime,Duration,Description")] GymClass gymClass)
    {
        if (id != gymClass.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(gymClass);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GymClassExists(gymClass.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(gymClass);
    }

    // GET: GymClasses/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var gymClass = await _context.GymClass
            .FirstOrDefaultAsync(m => m.Id == id);
        if (gymClass == null)
        {
            return NotFound();
        }

        return View(gymClass);
    }

    // POST: GymClasses/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var gymClass = await _context.GymClass.FindAsync(id);
        _context.GymClass.Remove(gymClass);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool GymClassExists(int id)
    {
        return _context.GymClass.Any(e => e.Id == id);
    }

    public async Task<IActionResult> BookingToggle(int? id)
    {
        if (id == null) return BadRequest();    // Bad request, 400

        // Todo 0.2.1: find which user is logged in
        ApplicationUser user = await _userManager.GetUserAsync(User);

        // Todo 0.2.1: verify user? 403 Forbidden

        var att = await _context.UserGymClass.FindAsync(id, user.Id);
        //GymClass? @class = _context.GymClass.Include(gc => gc.AttendingMembers).ToList().Find(gc => gc.Id == id);

        //if (@class == null) return NotFound();  // Not found, 404

        //ApplicationUserGymClass? userAttending = @class.AttendingMembers.Where(ag => ag.ApplicationUserId == user.Id).ToList().Find(am => am.ApplicationUserId == user.Id);

        string message;

        if (att == null) //Add user to gym class
        {
            ApplicationUserGymClass augc = new ApplicationUserGymClass
            {
                ApplicationUserId = user.Id,
                GymClassId = (int)id
                //ApplicationUser = user,
                //GymClass = @class
            };

            _context.UserGymClass.Add(augc);
            message = $"Successfully added {user.Email} to {user.Id}.";
        }
        else    // Remove user from gym class
        {
            _context.Remove(att);
            message = $"Successfully removed {user.Email} from {user.Id}.";
        }

        await _context.SaveChangesAsync();

        return Ok(message);
    }
}

