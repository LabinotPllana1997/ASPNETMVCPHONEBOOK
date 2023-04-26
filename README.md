# ASPNETMVCPHONEBOOK

## Installing Dependencies Through NuGet Package Manager

- installed entityframeworkcore.sqlserver and entityframeworkcore.tools. The latter is responsible for the EF migrations and making changes to the database from the application.

- dbcontext class is a class file that acts a bridge between EF core and the database. We can talk to the databse and perform changes.
- Reminder: - Constructors are **special methods in C# that are automatically called when an object of a class is created to initialize all the class data members**.

- Property created of type DbSet. We create a model for Contacts under Models. A Domain subfolder created and a Contact class created.

We defined the following properties for the Contact domain model:

```
namespace ASPNETMVCPHONEBOOK.Models.Domain
{
    public class Contact
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string PhoneNumber { get; set; }
    }
}
```


- We use this domain model in our DbContext class. Our DbSet is of type Contact. The following will be seen in the DbContext C# file:
```
public DbSet<Contact> Contacts { get; set; }
```

The Contacts property will be the table name created from the EF Core Migrations


## Now we need to inject this into our Program.cs file.

- Within the Program.cs file, we add services to the container:

```
builder.Services.AddDbContext<MVCDemoDbContext>(options =>
    options.UseSqlServer(""));
```

- it is good practice to edit the connection string in the appsettings.json file. Here is the connection string created with setting the server, database and trusted_connection=True:

```
  "ConnectionStrings": {
    "MvcDemoConnectionString": "server=WS382;database=MvcPhonebookDb;Trusted_connection=true"
  }
```

In order to then set up the connection, the following commands were executed in NuGet package manager:
```
Add-Migration
Update-Database
```

I ran into an (error?) stating that the Contact type requires a primary key to be defined. This may be due to not stating the type of Id in the Contact class as unique, by stating its type as Guid.

After googling the error and going on StackOverflow, one option is to add the following to the connection string:

```
TrustServerCertificate=True
```

Let's hope this works.

It worked! _Use your principles to follow logic and reasoning. That's what makes you a man. To act in accordance to nature.

It executed a SQL query to create a table Contacts. The following was outputted on the Package Manager Console

```
      CREATE TABLE [Contacts] (
          [Id] uniqueidentifier NOT NULL,
          [FirstName] nvarchar(max) NOT NULL,
          [Surname] nvarchar(max) NOT NULL,
          [PhoneNumber] nvarchar(max) NOT NULL,
          CONSTRAINT [PK_Contacts] PRIMARY KEY ([Id])
      );
```

## Create Pages to Add Contacts

- ContactsController.cs model controller was created under the controllers folder.
- We are adding functionality to add contacts here as well.
- The index method was changed to an Add() method, then a [HttpGet] method was decorated. 
- Then, the view method was right clicked and a Razor view was added, called Add.cshtml. New folder inside the Views folder.
- To navigate to this page, under the shared layout page, we add another tab which is called add contact.

- The following was added under the _ Layout.cshtml file found under the Shared folder. 

```
<li class="nav-item">
	<a class="nav-link text-dark" asp-area="" asp-controller="Contacts" asp-action="Add">Add Contact</a>
</li>
```

Note that the controller was changed to "Contacts" and the action to "Add" because we created an action called 'Add' under the Contacts controller.

- Now when I run the application, there is a new button on the navigation bar, saying 'Add Contact'. Once this is clicked, it takes you to a URL page with the extension /Contacts/Add.
- We want to create a form from which we can add a new contact.
- We are using bootstrap 5, we can use bootstrap classes and make our UI quickly. We search for forms. We copy the basic form HTML and paste this into our Add.cshtml file.

- We edit the form tag by adding method="post" action="Add".

- We now create a view model AddContactViewModel.cs, in which we add the properties similar to the domain model from the domain model Contact.cs. NB: we do not add the ID property to the AdContactViewModel since we do not want the user to add this. We will add this behind the scenes and it will appear in the SQL Database.

- Now, we go back to the add.cshtml and define the model that is coming from the AddContactViewModel:

```
@model ASPNETMVCPHONEBOOK.Models.AddContactViewModel
```

- The input type in the div tag was set to text and asp-for="FirstName" added.
- The following div tags were then added for the surname and phone number to be inputted:

```
    <div class="mb-3">
        <label for="" class="form-label">Surname</label>
        <input type="text" class="form-control" asp-for="Surname">
    </div>
    <div class="mb-3">
        <label for="" class="form-label">Phone Number</label>
        <input type="text" class="form-control" asp-for="PhoneNumber">
    </div>
```

- There is also a button of type "submit", with the class from the bootstrap, as follows:
```
<button type="submit" class="btn btn-primary">Submit</button>
```

Running the application now and clicking on the 'Add Contact' in the nav bar displays the required fields.

## Creating Post Method to get values after hitting submit

- When submitting the form, we should get the values in a post method.
- The following post method was added into the ContactsController class:

```
        [HttpPost]
        public IActionResult Add(AddContactViewModel addContactRequest)
```

- We now call entityframeworkcore to save the values to the database.
- Remember when we specifically did not put a user input for the ID and mentioned we will do this later on ourselves? Well here it is. Within the Post Method, we add the following:

```
var contact = new Contact()
{
	Id = Guid.NewGuid(),
	FirstName = addContactRequest.FirstName,
	Surname = addContactRequest.Surname,
	PhoneNumber = addContactRequest.PhoneNumber
};
```

- It is now time for the entity to be saved onto the database using the entityframework. We do this by defining a constructor. 
- Just within the Controller class, we wrote ctor then tab tab to generate the constructor. We then ingested into this the MVCDemoDbContext and assigned a field to this.

- A private readonly field is generated for us to talk to our database.
```
        private readonly MVCDemoDbContext mVCDemoDbContext;

        public ContactsController(MVCDemoDbContext mVCDemoDbContext)
        {
            this.mVCDemoDbContext = mVCDemoDbContext;
```

- The field inputs must now be added to the Contacts and changes saved. Here is the line of code added to do so:

```
mVCDemoDbContext.Contacts.Add(contact);
mVCDemoDbContext.SaveChanges();
```

- The saveChanges() method is important because the entityframework does not save the data to the database.

- User momentarily is redirected to the Add page for now...
```
return RedirectToAction("Add");
```

- The whole method is now converted into an asynchronous method. This is to ensure the method runs synchronously, until it reaches the first await expression, at which point the method is suspended until the awaited task is complete. 
- Here is the updated asynchronous method now:

```
public async Task<IActionResult> Add(AddContactViewModel addContactRequest)
{
	var contact = new Contact()
	{
		Id = Guid.NewGuid(),
		FirstName = addContactRequest.FirstName,
		Surname = addContactRequest.Surname,
		PhoneNumber = addContactRequest.PhoneNumber
	};

	await mVCDemoDbContext.Contacts.AddAsync(contact);
	await mVCDemoDbContext.SaveChangesAsync();
	return RedirectToAction("Add");

}
```

- Once hitting submit, then hovering over the request method, the data is seen. Wow. Let's now put these values into a database.

- A breakpoint was hit just after the post method, to see if the request was being made and storing the data. After hitting continue, the asynchronous method uploaded the data into the Contacts table in the SQL Server database created before. Add action working correctly!

## Showing List of Employees on Main Screen

- Creating the list to show to the user. We make an asynchronous method and get all of the contacts to view. 
- We use the DbContext to talk to the Contacts and use the ToListAsync() method to get the contacts, which will then be viewed. We set this to a variable 'contacts'. 
- In order to create a view, we right click the action index() and add a razor view, named index.cshtml.

- Model added, which is going to be a list of type contact:
```
@model List<ASPNETMVCPHONEBOOK.Models.Domain.Contact>
```

- h1 tag added to show this is the contacts list. 
```
<h1>Contacts</h1>
```

- We now create a for loop to loop over the list and display into a table.
- Here is the table head HTML written prior to the for loop:

```
<table class="table">
	<thead>
		<tr>
			<th>Id</th>
			<th>FirstName</th>
			<th>Surname</th>
			<th>PhoneNumber</th>
		</tr>
	</thead>
</table>
```

- The following for loop was added to the table body to loop over the model and add to the table:

```
	<tbody>
		@foreach (var contact in Model)
		{
			<tr>
				<td>@contact.Id</td>
				<td>@contact.FirstName</td>
				<td>@contact.Surname</td>
				<td>@contact.PhoneNumber</td>
			</tr>
		}
	</tbody>
```

- This model is now passed to the View by returning the model.
```
        public async Task<IActionResult> Index()
        {
            var contacts = await mVCDemoDbContext.Contacts.ToListAsync();
            return View(contacts)
        }
```

- We don't have the Contacts displayed. We need to add the index action to the Layout.cshtml file in order to view the current contacts in the database. We do this by adding the following:
```
<li class="nav-item">
	<a class="nav-link text-dark" asp-area="" asp-controller="Contacts" asp-action="Index">Contacts</a>
</li>
```

- Clicking now hot reload button now makes the Contacts button appear. 
- When clicking on the button, Contacts, it should be talking to our database and getting the result back. However, a HTTP ERROR 404 appears.
- Stopping the debug and restarting the application solved this issue! Happy Days!


## Navigating back to Contacts page when new contact added

- When the add is successful, we want to redirect the user back to the index page. 
- We navigate to the ContactsController.cs file and under the Post method, we change the `return RedirectToAction("Add");` to `return RedirectToAction("Index")`.

- Now, we want to be able to click on the contact and be able to do other functionalities, such as delete the contact. We will add a view link to the table next to each contact, to view their details by adding the following to the Index.cshtml file.
```
<td><a href="Contacts/View/@contact.Id">View</a></td>
```

## HTTP Get Method To Click and View Page Of Contact Profile

- We have now added a Get method to the ContactsController that returns a new View razor page (just created) that stores the id of the contact under the View model.
- The following is the Get method, containing the viewmodel that allows us to be directed to a new page, edit contact info, then be redirected to the index page once hitting submit: _NB: a new view.cshtml was also created_

```
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

                return View(viewModel);
            }
            return RedirectToAction("Index");
        }
```
The code above also has an if statement to check that the contact is not null.

## Edit Contact Info and Reflect onto Database and Index Page (Http Post Method)

- We want to be able to edit the info when redirected to the single contact page. We do this by adding the edit functionality through the post method below:

```
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
```
- This asynchronous method checks that the contact model is not null, and when submit is hit, will redirect you back to the contact list page (index) where now the contact info will be updated. 

## Delete Functionality

We want to add another 'Delete' button when the single contact is clicked. This should be of class button danger, shown in red. The following is added to the View.cshtml file:

```
    <button type="submit" class="btn btn-danger"
    asp-action="Delete"
    asp-controller="Contacts">Delete</button>
```

- The controller is set to the Contacts controller, where there is a Post Method, with a Delete action coded. Here is the code:
```
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
```

