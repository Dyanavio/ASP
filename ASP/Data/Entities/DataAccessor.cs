using ASP.Models.Api.Group;
using ASP.Models.Api.Product;
using Microsoft.EntityFrameworkCore;

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

        public UserAccess? GetUserAccessByLogin(string userLogin, bool isEditable = false)
        {
            IQueryable<UserAccess> source = _dataContext.UserAccesses.Include(ua => ua.UserData).Include(ua => ua.UserRole);
            if (isEditable) source = source.AsNoTracking();
            return source.FirstOrDefault(ua => ua.Login == userLogin && ua.UserData.DeletedAt == null);
        }

        public void UpdateUserData(UserData userData)
        {
        }

        public async Task<bool> DeleteUserAsync(string login)
        {
            UserAccess? ua = await _dataContext.UserAccesses.Include(ua => ua.UserData).FirstOrDefaultAsync(ua => ua.Login == login);
            if(ua == null) return false;

            ua.UserData.Birthdate = null;
            ua.UserData.Name = "";
            ua.UserData.Email = "";
            ua.UserData.DeletedAt = DateTime.Now;

            try
            {
                await _dataContext.SaveChangesAsync();
                return true;
            }
            catch(Exception e)
            {
                _logger.LogWarning("DeleteUserAsync: {e}", e.Message);
                return false;
            }

        }
        
    }
}
