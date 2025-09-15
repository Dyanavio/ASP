# Testing ASP project
## For learning purposes

This is the very first project using ASP.Net created with the intention to study the technology itself and some other conventions, protocols, etc.
Throughout the course the following was covered:
* Razor
* Inversion of control
* Single page application aka SPA
* Ways of working with forms
* Display templates
* Concepts of data accessors
* Middlewares
* Filters
* Differences between MVC and API controllers

So here I will list what was implemented as well as some theory I was able to note.

### Razor
Razor is a technology that aims to combine a programming language and a markup language. Its base is &commat; that is substituted by the &amp;commant; entity in HTML

#### Expressions
Expressions are the commands that have result. In the sense of Razor this result is substituted in the place where it was mentioned.
Syntax:
```
@(expression) e.g.
@(2 + 3) = @(2 + 3)
In that sense, the construction @() can be compared to the output operator print(expression)
```

#### Statements
Statements do not have visual results (nothing is printed). Declared via syntax:
```
@{statements}, e.g.
@{
	int x = 10;
	var random = new Random();
}
```

Short form for expressions &commat;expression is allowed if the expressions do not have separators, like
```
@x = @x
@random.Next() = @random.Next()
@x + 1 = @x + 1
```

#### Control Statements
Condition and cycle operators: &commat;if{}, &commat;if{ }else{ }, &commat;switch, &commat;for, &commat;while, &commat;foreach, ...

![Text](ASP/Screenshots/ToReadme/img1.png)

### Structure
All entities have a full CRUD cycle, meaning that on ordinary user can  register (create), view (read), edit(update) and delete their account. 
Some with moderator role or higher can do the same with products and their groups

Here for example is log in modal:
![Text](ASP/Screenshots/ToReadme/img2.png)

This is what expects user befire creating their account. _Do not mind the theory thought..._
![Text](ASP/Screenshots/ToReadme/img3.png)

All CRUD processes are followed by validation
![Text](ASP/Screenshots/ToReadme/img4.png)

After signing in you will be able to view your own profile and purchase history  
![Text](ASP/Screenshots/ToReadme/img5.png)  
Here you can edit your data and restore previous purchases  
![Text](ASP/Screenshots/ToReadme/img9.png)

As was previously mentioned, all tools for moderators to manage the shop are at their disposal  
![Text](ASP/Screenshots/ToReadme/img6.png)

Here is the product's page. For swift navigation there is a carousel of groups with respective links  
![Text](ASP/Screenshots/ToReadme/img7.png)

Here is your cart
![Text](ASP/Screenshots/ToReadme/img8.png)

And one of the previous purchases
![Text](ASP/Screenshots/ToReadme/img10.png)

### Display
Elements of different lists are displayed via Display templates.
Display templates are ways of separating layout of a certain object (model).  
One creates DisplayTemplates (name is important) directory in View directory (Views/Shop).  
Then one creates View itself. If name corresponds to the type name of the model then the choice of template is automatic  
If the model type is complex (collection) or name is impossible, then template is declared directly @Html.FisplayFor(m => group, "Tepmlate name")

```
@foreach (var p in Model.ProductGroup.Products)
{
	@Html.DisplayFor(m => p)
}
```

![Text](ASP/Screenshots/ToReadme/img14.png)

### MVC and API
**Differences between MVC and API controllers**

+ MVC: one method (usually GET) and different addresses (You can reach ONE address with ONE method, action is determined by address)
GET /home/privacy -> HomeController::Privacy()
POST /home/index -> HomeController::Index()   (Post makes no difference, we will end up on Index)

+ API: one address, but different methods
GET  /api/product -> ProductController::ProductsList()
POST /api/product -> ProductController::CreateProduct()
PUT  /api/product

-----------------------------------------------------------------------------------------

MVC - returns IActionResult
API - returns objects of an arbitrary type that ASP changes them to JSON (except for string, it changes to plain/text)

### Careful now
All your actions have a confirmation stage
![Text](ASP/Screenshots/ToReadme/img12.png)

### Inversion of control
Inversion of Control (IoC) is an architectural pattern that selects a separate component (container/injector) which controls other objects' life cycles.
One can say it is the dependencies that are used (i.e. variables that are set by the container while the object is being constructed) instead of new Object.
__Organization consists of several stages:__
+ Describing shared classes (services)
+ Registration of classes in container and stating their type of life cycle
+ Declaring the dependencies in other classes (controller)
+ Launching Resolve to determine the order of implementation of dependencies(creating objects)

DIP (_Dependency inversion principle_) - is one of the SOLID principles that recommends to create dependencies from abstraction of highest level.
Services are described together with interface. Conclusion: a new minimal service is basically two elements: interface and a class.

DI - Dependency Injection - is a way to realise IoC by passing references to the service objects at the points of injection

### SPA
SPA technology provides minimum number of page refreshes. Content is changes via JS or its frameworks.  
Actually, the page remains unchanged (Single) while its content is redrawn.

And of course there would be some tabs created purely for practical uses...
![Text](ASP/Screenshots/ToReadme/img13.png)
