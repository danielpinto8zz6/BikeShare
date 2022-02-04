using System;

namespace Common.Exceptions
{
    public class UserNotFoundException : Exception
    {
        public UserNotFoundException(string username) : base($"User with username: {username} not found!")
        {
        }
    }
}