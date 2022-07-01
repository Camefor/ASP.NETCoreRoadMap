using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;
using System;

namespace ISExample.Models
{
    [CollectionName("Users")] //指定此类将映射到 MongoDB 中的“Users”集合。
    public class ApplicationUser : MongoIdentityUser<Guid> //在 MongoIdentityUser 中对 T 类型使用 Guid 意味着集合的主键将是 Guid 类型
    {
    }
}
