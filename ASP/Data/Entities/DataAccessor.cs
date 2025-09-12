using ASP.Models.Api.Group;
using ASP.Models.Api.Product;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace ASP.Data.Entities
{
    public class DataAccessor(DataContext dataContext, ILogger<DataAccessor> logger)
    {
        private readonly DataContext _dataContext = dataContext;
        private readonly ILogger<DataAccessor> _logger = logger;

        public bool IsGroupSlugUsed(string slug)
        {
            return _dataContext.ProductGroups.Any(g => g.Slug == slug);
        }
        public bool IsGroupNameUsed(string name)
        {
            return _dataContext.ProductGroups.Any(g => g.Name == name);
        }

        public bool IsProductSlugUsed(string slug)
        {
            return _dataContext.Products.Any(p => p.Slug == slug);
        }
        public bool IsProductNameUsed(string name)
        {
            return _dataContext.Products.Any(p => p.Name == name);
        }

        public void AddProductGroup(ApiGroupDataModel model)
        {
            _dataContext.ProductGroups.Add(new ProductGroup()
            {
                Id = Guid.NewGuid(),
                Name = model.Name,
                Description = model.Description,
                Slug = model.Slug,
                ImageUrl = model.ImageUrl,
                ParentId = model.ParentId == null ? null : Guid.Parse(model.ParentId),
                DeletedAt = null
            });
            try
            {
                _dataContext.SaveChanges();
            }
            catch (Exception e)
            {
                _logger.LogError($"AddProductGroup: {e.Message}");
                throw;
            }
        }
        public void AddProduct(ApiProductDataModel model)
        {
            Guid groupId;
            try { groupId = Guid.Parse(model.GroupId); }
            catch { throw; }

            _dataContext.Products.Add(new Product()
            {
                Id = Guid.NewGuid(),
                GroupId = groupId,
                Name = model.Name,
                Description = model.Description,
                Slug = model.Slug,
                ImageUrl = model.ImageUrl,
                Price = model.Price,
                Stock = model.Stock,
                DeletedAt = null,

            });
            try
            {
                _dataContext.SaveChanges();
            }
            catch (Exception e)
            {
                _logger.LogError($"AddProduct: {e.Message}");
                throw;
            }
        }

        public IEnumerable<ProductGroup> GetProductGroups()
        {
            // AsEnumerable changes IQueryable to IEnumerable and executes the query
            return _dataContext.ProductGroups.AsNoTracking().Where(group => group.DeletedAt == null).AsEnumerable();
        }
        public ProductGroup? GetProductGroupBySlug(string slug)
        {
            return _dataContext.ProductGroups.Include(group => group.Products).AsNoTracking().FirstOrDefault(group => group.Slug == slug && group.DeletedAt == null);
        }
        public Product? GetProductBySlug(string slug)
        {
            return _dataContext.Products
                    .AsNoTracking()
                    .FirstOrDefault(p => (p.Slug == slug || p.Id.ToString() == slug) && p.DeletedAt == null);
        }

        public UserAccess? GetUserAccessByLogin(string userLogin, bool isEditable = false)
        {
            IQueryable<UserAccess> source = _dataContext.UserAccesses.Include(ua => ua.UserData).Include(ua => ua.UserRole);
            if (!isEditable) source = source.AsNoTracking();
            return source.FirstOrDefault(ua => ua.Login == userLogin && ua.UserData.DeletedAt == null);
        }

        public void UpdateUserData(UserData userData)
        {
        }

        public async Task<bool> DeleteUserAsync(string login)
        {
            UserAccess? ua = await _dataContext.UserAccesses.Include(ua => ua.UserData).FirstOrDefaultAsync(ua => ua.Login == login);
            if (ua == null) return false;

            ua.UserData.Birthdate = null;
            ua.UserData.Name = "";
            ua.UserData.Email = "";
            ua.UserData.DeletedAt = DateTime.Now;

            try
            {
                await _dataContext.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogWarning("DeleteUserAsync: {e}", e.Message);
                return false;
            }
        }

        public void AddToCart(string userId, string productId)
        {
            Guid userGuid = Guid.Parse(userId);
            Guid productGuid = Guid.Parse(productId);

            var user = _dataContext.Users.Find(userGuid) ?? throw new ArgumentException("user was not found", nameof(userId)); ; // Find searches by primary key
            var product = _dataContext.Products.Find(productGuid) ?? throw new ArgumentException("product was not found", nameof(productId));

            // If user has an open cart, and
            //      if the cart already has this item, then increase the number
            //      otherwise add a new CartItem
            // otherwise create a new cart and add the item

            var cart = _dataContext.Carts.Include(c => c.CartItems).FirstOrDefault(c => c.UserId == userGuid && c.PaidAt == null && c.DeletedAt == null);
            if (cart == null)
            {
                cart = new Cart()
                {
                    Id = Guid.NewGuid(),
                    CreatedAt = DateTime.Now,
                    Price = 0,
                    UserId = userGuid,
                };
                _dataContext.Carts.Add(cart);
            }

            CartItem? cartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productGuid);
            if (cartItem == null)
            {
                cartItem = new CartItem()
                {
                    Id = Guid.NewGuid(),
                    CartId = cart.Id,
                    Price = product.Price,
                    Quantity = 1,
                    ProductId = productGuid
                };
                _dataContext.CartItems.Add(cartItem);
                cart.Price += cartItem.Price; // TODO: DiscountService
            }
            else
            {
                cartItem.Quantity += 1;
                cartItem.Price += product.Price;
                cart.Price += product.Price; // TODO: DiscountService
            }
            _dataContext.SaveChanges();
        }

        public IEnumerable<CartItem> GetActiveCartItems(string userId)
        {
            var cart = GetActiveCart(userId);
            return cart?.CartItems ?? [];
        }
        public IEnumerable<Cart> GetCarts()
        {
            return [];
        }
        public Cart? GetActiveCart(string userId, bool isEditable = false) 
        {
            Guid userGuid = Guid.Parse(userId);
            var user = _dataContext.Users.Find(userGuid) ?? throw new ArgumentException("user was not found", nameof(userId)); ; // Find searches by primary key

            IQueryable<Cart> source = _dataContext.Carts.Include(c => c.CartItems).ThenInclude(ci => ci.Product);
            if (!isEditable) source = source.AsNoTracking();

            return source.FirstOrDefault(c => c.UserId == userGuid && c.PaidAt == null && c.DeletedAt == null);

        }
        public void ModifyCart(string userId, string productId, int increment)
        {
            Guid userGuid = Guid.Parse(userId);
            Guid productGuid = Guid.Parse(productId);

            var user = _dataContext.Users.Find(userGuid) ?? throw new ArgumentException("user was not found", nameof(userId)); ; // Find searches by primary key
            Cart cart = GetActiveCart(userId, isEditable: true) ?? throw new ArgumentException("active cart was not found");

            // If the cart does not contain the product, the query is incorrect
            CartItem cartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productGuid) ?? throw new ArgumentException("product was not found", nameof(productId));

            // If increment is negative and the result becomes negative, then that's an error
            int newQuantity = cartItem.Quantity + increment;
            if(newQuantity < 0)
            {
                throw new ArgumentException("incremenet causes negative quantity");
            }
            // If increment overflows the number of products in stock, then that's an error
            if (newQuantity > cartItem.Product.Stock)
            {
                throw new ArgumentOutOfRangeException();
            }
            if(newQuantity == 0)
            {
                // Delete...
                cart.Price -= cartItem.Price;
                _dataContext.CartItems.Remove(cartItem);
            }
            else
            {
                cartItem.Quantity = newQuantity;
                // TODO: DiscountService
                cartItem.Price += increment * cartItem.Product.Price;
                cart.Price += increment * cartItem.Product.Price;
            }
            _dataContext.SaveChanges();
        }

        public void CheckoutActiveCart(string userId)
        {
            Cart cart = GetActiveCart(userId, isEditable: true) ?? throw new ArgumentException("active cart was not found");
            cart.PaidAt = DateTime.Now;
            _dataContext.SaveChanges();
        }

        public void DiscardActiveCart(string userId)
        {
            Cart cart = GetActiveCart(userId, isEditable: true) ?? throw new ArgumentException("active cart was not found");
            cart.DeletedAt = DateTime.Now;
            _dataContext.SaveChanges();
        }

        public void RemoveFromCart(string userId, string productId)
        {
            Guid userGuid = Guid.Parse(userId);
            Guid productGuid = Guid.Parse(productId);

            var user = _dataContext.Users.Find(userGuid) ?? throw new ArgumentException("user was not found", nameof(userId)); ; // Find searches by primary key
            Cart cart = GetActiveCart(userId, isEditable: true) ?? throw new ArgumentException("active cart was not found");

            CartItem cartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productGuid) ?? throw new ArgumentException("product was not found", nameof(productId));

            if (!cart.CartItems.Contains(cartItem))
            {
                throw new InvalidDataException("Cart has no such item");
            }

            cart.Price -= cartItem.Price;

            _dataContext.CartItems.Remove(cartItem);
            _dataContext.SaveChanges();

        }
    }
}