namespace Macropus.Interfaces.User;

// Context need to sign any operations with user/ Request any users permisions/allows/access
public interface IUserContext
{
    IUser User { get; }
}