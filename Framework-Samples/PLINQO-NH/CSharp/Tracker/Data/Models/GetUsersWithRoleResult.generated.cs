using System;
using System.Collections.Generic;
using System.Text;
using NHibernate;
using Tracker.Data;

namespace Tracker.Data.Entities
{
	public partial class GetUsersWithRoleResult
    {
        public GetUsersWithRoleResult(System.Int32 id, System.String emailAddress, System.String firstName, System.String lastName, System.Byte[] avatar, System.DateTime createdDate, System.DateTime modifiedDate, System.Byte[] rowVersion, System.String passwordHash, System.String passwordSalt, System.String comment, System.Boolean isApproved, System.DateTime lastLoginDate, System.DateTime lastActivityDate, System.DateTime lastPasswordChangeDate, System.String avatarType)
        {
            Id = id;
            EmailAddress = emailAddress;
            FirstName = firstName;
            LastName = lastName;
            Avatar = avatar;
            CreatedDate = createdDate;
            ModifiedDate = modifiedDate;
            RowVersion = rowVersion;
            PasswordHash = passwordHash;
            PasswordSalt = passwordSalt;
            Comment = comment;
            IsApproved = isApproved;
            LastLoginDate = lastLoginDate;
            LastActivityDate = lastActivityDate;
            LastPasswordChangeDate = lastPasswordChangeDate;
            AvatarType = avatarType;
        }
        
        public System.Int32 Id { get; set; }
        
        public System.String EmailAddress { get; set; }
        
        public System.String FirstName { get; set; }
        
        public System.String LastName { get; set; }
        
        public System.Byte[] Avatar { get; set; }
        
        public System.DateTime CreatedDate { get; set; }
        
        public System.DateTime ModifiedDate { get; set; }
        
        public System.Byte[] RowVersion { get; set; }
        
        public System.String PasswordHash { get; set; }
        
        public System.String PasswordSalt { get; set; }
        
        public System.String Comment { get; set; }
        
        public System.Boolean IsApproved { get; set; }
        
        public System.DateTime LastLoginDate { get; set; }
        
        public System.DateTime LastActivityDate { get; set; }
        
        public System.DateTime LastPasswordChangeDate { get; set; }
        
        public System.String AvatarType { get; set; }
	}
}