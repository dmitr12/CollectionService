using Server.DI;
using Server.Interfaces;
using Server.Models.DB_Models;
using Server.Models.View_Models;
using Server.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Managers
{
    public class UserManager
    {
        private readonly PersonRepository personRepository;
        private readonly IGeneratorToken generatorToken;

        public UserManager(IGeneratorToken generatorToken, PersonRepository personRepository)
        {
            this.generatorToken = generatorToken;
            this.personRepository = personRepository;
        }

        public async Task<string> RegisterUser(UserRegistrationModel model)
        {
            if(personRepository.UserExists(model.UserName, model.Email))
                return "Пользователь с таким Login или Email уже есть";
            await personRepository.AddItem(new User
            {
                UserName = model.UserName,
                Email = model.Email,
                Password = HashClass.GetHash(model.Password),
                CountCompletedTasks = 0,
                RoleId = 1
            });
            return null;
        }

        public string GetToken(UserAuthenticationModel model)
        {
            User user = personRepository.GetUserByNameOrEmail(model);
            if (user != null)
            {
                var token = generatorToken.GenerateToken(user);
                return token;
            }
            return null;
        }

        public void InceremntUserCompletedTasks(int userId)
        {
            personRepository.IncerementCountCompletedTasks(userId).Wait();
        }

        public List<User> GetAllUsers()
        {
            return personRepository.GetAllItems().Result.ToList();
        }

        public User GetUserById(int userId)
        {
            return personRepository.GetItemById(userId).Result;
        }
    }
}
