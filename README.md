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
![Text](ASP/Screenshots/ToReadme/img2.png)


![Text](ASP/Screenshots/ToReadme/img3.png)
![Text](ASP/Screenshots/ToReadme/img4.png)
![Text](ASP/Screenshots/ToReadme/img5.png)
![Text](ASP/Screenshots/ToReadme/img6.png)
![Text](ASP/Screenshots/ToReadme/img7.png)
![Text](ASP/Screenshots/ToReadme/img8.png)
![Text](ASP/Screenshots/ToReadme/img9.png)
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


![Text](ASP/Screenshots/ToReadme/img11.png)
![Text](ASP/Screenshots/ToReadme/img12.png)

And of course there would be some tabs created purely for practical uses...
![Text](ASP/Screenshots/ToReadme/img13.png)
