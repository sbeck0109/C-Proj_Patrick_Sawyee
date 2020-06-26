using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http; // for session
using Microsoft.AspNetCore.Identity; // for password hashing
using CsharpProject.Models;

namespace CsharpProject.Controllers
{
	public class HomeController : Controller
	{
		private ProjectContext dbContext;
		private int? uid
		{
			get
			{
				return HttpContext.Session.GetInt32("UserId");
			}
		}
		private bool isLoggedIn
		{
			get
			{
				return uid != null;
			}
		}

		public HomeController(ProjectContext context)
		{
			dbContext = context;
		}

		// ROUTE:               METHOD:                VIEW:
		// -----------------------------------------------------------------------------------
		// GET("")              Index()                Index.cshtml
		// POST("/register")    Create(User user)      ------ (Index.cshtml to display errors)
		// POST("/login")       Login(LoginUser user)  ------ (Index.cshtml to display errors)
		// GET("/logout")       Logout()               ------





 
		[HttpGet("")]
		public IActionResult Index()
		{
			List<Property> AllProperties = dbContext.Properties.ToList();
			return View("Index");
		}

		[HttpGet]
		public async Task<IActionResult> Index(string searchString)
{
    var properties = from p in dbContext.Properties
                 select p;

    if (!String.IsNullOrEmpty(searchString))
    {
        properties = movies.Where(s => s.Title.Contains(searchString));
    }

    return View(await movies.ToListAsync());
}



		[HttpGet("/logandreg")]
		public IActionResult LogAndReg()
		{
			//List<User> AllUsers = dbContext.Users.ToList();
			return View("LogAndReg");
		}

		[HttpPost("/register")]
		public IActionResult Create(User user)
		{
			if (ModelState.IsValid)
			{
				// If a User exists with provided email
				if (dbContext.Users.Any(u => u.Email == user.Email))
				{
					// Manually add a ModelState error to the Email field
					ModelState.AddModelError("Email", "Email already in use!");
					return View("Index");
				}

				// hash password
				PasswordHasher<User> Hasher = new PasswordHasher<User>();
				user.Password = Hasher.HashPassword(user, user.Password);

				// create user
				dbContext.Add(user);
				dbContext.SaveChanges();

				// sign user into session
				var NewUser = dbContext.Users.FirstOrDefault(u => u.Email == user.Email);
				int UserId = NewUser.UserId;
				HttpContext.Session.SetInt32("UserId", UserId);
				HttpContext.Session.SetString("FullName", user.FullName());

				// go to success
				return RedirectToAction("Dashboard");
			}
			// display errors
			else
			{
				return View("Index");
			}
		}

		[HttpPost("/login")]
		public IActionResult Login(LoginUser user)
		{
			if (ModelState.IsValid)
			{
				var userInDb = dbContext.Users.FirstOrDefault(u => u.Email == user.LoginEmail);
				if (userInDb == null)
				{
					// Add an error to ModelState and return to View!
					ModelState.AddModelError("LoginEmail", "Invalid Email/Password");
					return View("Index");
				}
				// Initialize hasher object
				var hasher = new PasswordHasher<LoginUser>();

				// verify provided password against hash stored in db
				var result = hasher.VerifyHashedPassword(user, userInDb.Password, user.LoginPassword);
				if (result == 0)
				{
					// handle failure (this should be similar to how "existing email" is handled)
					ModelState.AddModelError("LoginPassword", "Password is invalid.");
					return View("Index");
				}

				// sign user into session
				int UserId = userInDb.UserId;
				HttpContext.Session.SetInt32("UserId", UserId);
				HttpContext.Session.SetString("FullName", userInDb.FullName());

				return RedirectToAction("Dashboard");
			}
			// display errors
			else
			{
				return View("Index");
			}
		}

		[HttpGet("/logout")]
		public IActionResult Logout()
		{
			HttpContext.Session.Clear();
			return RedirectToAction("Dashboard");
		}

		[HttpGet("/subscribe")]
		public IActionResult Subscribe()
		{
			//List<User> AllUsers = dbContext.Users.ToList();
			return View("Subscribe");
		}

		[HttpGet("/community")]
		public IActionResult Community()
		{
			return View();
		}

		[HttpGet("/About")]
		public IActionResult About()
		{
			return View();
		}


		///////////// BEGINNING OF CRUD METHODS FOR Property MODEL /////////////

		// GET ALL Properties
		[HttpGet("Dashboard")]
		public IActionResult Dashboard()
		{
			List<Property> AllProperties = dbContext.Properties
				.Include(p => p.Creator)
				.ToList();

			return View("Dashboard", AllProperties);
		}

		//  GET One Single Property
		[HttpGet("property/{propertyId}")]
		public IActionResult Property(int propertyId)
		{
			Property OneSingleProperty = dbContext.Properties
				  .Include(p => p.Creator)
				  .Include(p => p.Associations) //added
					.ThenInclude(a =>a.User)
				  .FirstOrDefault(b => b.PropertyId == propertyId);

            if (OneSingleProperty == null)
            {
                return RedirectToAction("Dashboard");
            }
			return View("PropertyDetails", OneSingleProperty);
		}

//POST Like for property
[HttpGet("property/{propertyId}/like")]
public IActionResult Like(int propertyId)
{
	if (!isLoggedIn)
	{
		return RedirectToAction("Dashboard", "Home");
	}
	User user = dbContext.Users.FirstOrDefault(u => u.UserId == Convert.ToInt32(HttpContext.Session.GetInt32("UserId")));
	Property property = dbContext.Properties.FirstOrDefault(w => w.PropertyId == propertyId);

	Association alreadyLiked = dbContext.Associations.FirstOrDefault(a => a.PropertyId == propertyId && a.UserId == uid);

	if (alreadyLiked != null){
		dbContext.Associations.Remove(alreadyLiked);
	}else
	{
		Association newLike = new Association()
		{
			UserId = (int)uid,
			PropertyId = propertyId,
		};

		dbContext.Associations.Add(newLike);
	}

		Property OneSingleProperty = dbContext.Properties
				  .Include(p => p.Creator)
				  .Include(p => p.Associations)
					.ThenInclude(a =>a.User)
				  .FirstOrDefault(b => b.PropertyId == propertyId);

            if (OneSingleProperty == null)
            {
                return RedirectToAction("Dashboard");
            }
				dbContext.SaveChanges();
			return View("PropertyDetails", OneSingleProperty);
	}

		//POST Delete One Single Property
		[HttpGet("property/{propertyId}/delete")]
		public IActionResult DeleteProperty(int propertyId)
		{
			Property OneSingleProperty = dbContext.Properties
				.Include(p => p.Creator)
				.FirstOrDefault(b => b.PropertyId == propertyId);

			int? userId = HttpContext.Session.GetInt32("UserId");

			if (userId != OneSingleProperty.UserId || userId == null)
			{
				return RedirectToAction("Dashboard");
			}
			if (OneSingleProperty != null)
			{
				dbContext.Properties.Remove(OneSingleProperty);
				dbContext.SaveChanges();
			}
			return RedirectToAction("Dashboard");
		}
		//display a createProperty form
		[HttpGet]
		[Route("/addProperty")]
		public IActionResult CreatePropertyForm()
		{
			if (!isLoggedIn)
			{
				return RedirectToAction("Index");
			}
			return View("CreatePropertyForm");
		}

		//POST Create One Single Property
		[HttpPost("/addProperty")]
		public IActionResult CreateProperty(Property property)
		{
			property.UserId = Convert.ToInt32(HttpContext.Session.GetInt32("UserId"));

			if (ModelState.IsValid)
			{
				dbContext.Add(property);
				dbContext.SaveChanges();
				return RedirectToAction("Dashboard");
			}
			return View("CreatePropertyForm");
		}

		//Edit post
		[HttpGet("/property/{propertyId}/edit")]
		public IActionResult Edit(int propertyId){

		if (!isLoggedIn)
		{
		return RedirectToAction("Index");
		}
		Property selectedProperty = dbContext.Properties.FirstOrDefault(property => property.PropertyId == propertyId);
		if (selectedProperty == null || selectedProperty.UserId != uid)
		{
		return RedirectToAction("Index");
		}
		return View("Edit", selectedProperty);
		}
		


		//edit & update post
		[HttpPost("/property/{propertyId}/edit")]
		public IActionResult Update(int propertyId, Property editedProperty)
		{
			if (ModelState.IsValid == false)
			{
					editedProperty.PropertyId = propertyId;
					return View("Edit", editedProperty);
			}

			Property selectedProperty = dbContext.Properties.FirstOrDefault(property => property.PropertyId == propertyId);

			if (selectedProperty == null)
			{
					return RedirectToAction("Dashboard");
			}

			selectedProperty.Title = editedProperty.Title;
			selectedProperty.Address = editedProperty.Address;
			selectedProperty.Description = editedProperty.Description;
			selectedProperty.Bedrooms = editedProperty.Bedrooms;
			selectedProperty.Baths = editedProperty.Baths;
			selectedProperty.SqFt = editedProperty.SqFt;
			selectedProperty.LotSize = editedProperty.LotSize;
			selectedProperty.Year = editedProperty.Year;
			selectedProperty.Price = editedProperty.Price;
			selectedProperty.ImgUrl = editedProperty.ImgUrl;
			selectedProperty.UpdatedAt = DateTime.Now;

			dbContext.Properties.Update(selectedProperty);
			dbContext.SaveChanges();
			return RedirectToAction("Dashboard", new { propertyId = selectedProperty.PropertyId });
	}




		///////////// END OF CRUD METHODS FOR Property MODEL /////////////

		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
