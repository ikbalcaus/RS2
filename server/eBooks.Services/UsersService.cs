using eBooks.Database;
using eBooks.Interfaces;
using eBooks.Models;
using MapsterMapper;

namespace eBooks.Services;

public class UsersService : IUsersService
{
    private readonly EBooksContext _db;
    private readonly IMapper _mapper;
    public UsersService(EBooksContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }
    
    public List<UsersModel> Get()
    {
        var entity = _db.Users.ToList();
        return _mapper.Map<List<UsersModel>>(entity);
    }
}
