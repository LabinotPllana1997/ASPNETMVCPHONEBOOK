using ASPNETMVCPHONEBOOK.Data;
using ASPNETMVCPHONEBOOK.Models;
using ASPNETMVCPHONEBOOK.Models.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASPNETMVCPHONEBOOK.Controllers
{
    public class ContactsController : Controller
    {
        private readonly MVCDemoDbContext mVCDemoDbContext;

        public ContactsController(MVCDemoDbContext mVCDemoDbContext)
        {
            this.mVCDemoDbContext = mVCDemoDbContext;
        }

        [HttpGet]
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var contacts = await mVCDemoDbContext.Contacts.ToListAsync(cancellationToken);
            return View(contacts);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddContactViewModel addContactRequest)
        {
            var contact = new Contact()
            {
                Id = Guid.NewGuid(),
                FirstName = addContactRequest.FirstName,
                Surname = addContactRequest.Surname,
                PhoneNumber = addContactRequest.PhoneNumber
            };

            if (contact.FirstName.Length > 20 | contact.PhoneNumber.Length > 20 | contact.Surname.Length > 20)
            {
                return BadRequest("Too Long! Must be 20 characters!");
            }
            await mVCDemoDbContext.Contacts.AddAsync(contact);
            await mVCDemoDbContext.SaveChangesAsync();
            return RedirectToAction("Index");

        }

        [HttpGet]
        public async Task<IActionResult> View(Guid id)
        {
            var contact =  await mVCDemoDbContext.Contacts.FirstOrDefaultAsync(x => x.Id == id);

            if (contact != null)
            {
                var viewModel = new UpdateContactViewModel()
                {
                    Id = contact.Id,
                    FirstName = contact.FirstName,
                    Surname = contact.Surname,
                    PhoneNumber = contact.PhoneNumber
                };

                return await Task.Run(()=> View("View", viewModel));
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> View(UpdateContactViewModel model)
        {
            var contact = await mVCDemoDbContext.Contacts.FindAsync(model.Id);

            if (contact != null)
            {
                contact.FirstName = model.FirstName;
                contact.Surname = model.Surname;
                contact.PhoneNumber = model.PhoneNumber;

                await mVCDemoDbContext.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(UpdateContactViewModel model)
        {
            var contact = await mVCDemoDbContext.Contacts.FindAsync(model.Id);

            if (contact != null)
            {
                mVCDemoDbContext.Contacts.Remove(contact);
                await mVCDemoDbContext.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }

    }
}
