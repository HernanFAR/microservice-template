using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using SharedKernel.Domain.Interfaces;
using SharedKernel.Domain.Others;
using Users.Domain.ETOs;
using Users.Domain.Validations;

namespace Users.Domain.Entities.Users
{
    public sealed class ApplicationUser : IdentityUser<Guid>, IAggregateRoot<Guid>
    {
        private static readonly ApplicationUserValidator _Validator = new();

        private ApplicationUser()
        {
            _LoginInformations = new List<UserLoginInformation>();
        }

        internal ApplicationUser(Guid id, DateTime created) : this()
        {
            Id = id;
            Created = created;
        }

        public ApplicationUser(DateTime created) : this(Guid.NewGuid(), created)
        {
            AddEvent(new EventInformation(new ApplicationUserCreated(Id), true));
        }

        public DateTime Created { get; private set; }

        public DateTime Updated { get; private set; }

        public IReadOnlyList<UserLoginInformation> LoginInformations => _LoginInformations;

        private readonly List<UserLoginInformation> _LoginInformations;

        public UserLoginInformation AddLoginInformation(string ip, string? continent, string? region, string? city,
            long? latitude, long? longitude, DateTimeOffset created)
        {
            var loginInformation = new UserLoginInformation(ip, continent, region, city, latitude, longitude, created);

            _LoginInformations.Add(loginInformation);
            Updated = created.DateTime;

            AddEvent(new EventInformation(new ApplicationUserLogged(loginInformation.Id), true));

            return loginInformation;
        }

        public object[] GetKeys()
        {
            return new object[] { Id };
        }

        public bool EntityEquals(IEntity? other)
        {
            if (other == null)
            {
                return false;
            }

            //Same instances must be considered as equal
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            //Must have a IS-A relation of types or must be same type
            var typeOfEntity1 = GetType();
            var typeOfEntity2 = other.GetType();

            if (!typeOfEntity1.IsAssignableFrom(typeOfEntity2) && !typeOfEntity2.IsAssignableFrom(typeOfEntity1))
            {
                return false;
            }

            var entity1Keys = GetKeys();
            var entity2Keys = other.GetKeys();

            for (var i = 0; i < entity1Keys.Length; i++)
            {
                var entity1Key = entity1Keys[i];
                var entity2Key = entity2Keys[i];

                if (entity1Key == null)
                {
                    if (entity2Key == null)
                    {
                        //Both null, so considered as equals
                        continue;
                    }

                    //entity2Key is not null!
                    return false;
                }

                if (entity2Key == null)
                {
                    //entity1Key was not null!
                    return false;
                }


                if (!entity1Key.Equals(entity2Key))
                {
                    return false;
                }
            }

            return true;
        }

        private readonly ICollection<EventInformation> _Events = new Collection<EventInformation>();

        public IEnumerable<EventInformation> GetEvents() => _Events;

        public void AddEvent(EventInformation eventData) => _Events.Add(eventData);

        public void ClearEvents() => _Events.Clear();

        public void Validate() => _Validator.ValidateAndThrow(this);

        public static class Admin
        {
            public static readonly Guid Id = new(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1);

            public const string UserName = "Hernán Álvarez";
            public const string NormalizedUserName = "HERNÁN ÁLVAREZ";
            public const string Email = "h.f.alvarez.rubio@gmail.com";
            public const string NormalizedEmail = "H.F.ALVAREZ.RUBIO@GMAIL.COM";
            public const bool EmailConfirmed = true;
            public const string PasswordHash = "AKGEFunVW6jf5iMPDIVnGMDFxLV3V8zte65VXv0/k0HZx4QaGNcAix9tiJQqP1Qn+A==";
            public const string SecurityStamp = "655ED840-2BE2-4090-A25E-CFC87502D559";
            public const string ConcurrencyStamp = "452241B4-DEE8-4C04-B467-FE18FA73AAB0";
            public const string PhoneNumber = "+569 49798355";
            public const bool PhoneNumberConfirmed = true;
            public const bool TwoFactorEnabled = false;
            public const bool LockoutEnabled = true;

        }
    }
}
