using MongoDB.Bson;

namespace devtools_proj.Persistence.Entities.EntityInterfaces;

public interface IHasId
{
    public ObjectId Id { get; set; }
}